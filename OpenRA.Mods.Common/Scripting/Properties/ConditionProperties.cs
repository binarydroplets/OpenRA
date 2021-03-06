#region Copyright & License Information
/*
 * Copyright 2007-2016 The OpenRA Developers (see AUTHORS)
 * This file is part of OpenRA, which is free software. It is made
 * available to you under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version 3 of
 * the License, or (at your option) any later version. For more
 * information, see COPYING.
 */
#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenRA.Mods.Common.Traits;
using OpenRA.Scripting;
using OpenRA.Traits;

namespace OpenRA.Mods.Common.Scripting
{
	[ScriptPropertyGroup("General")]
	public class ConditionProperties : ScriptActorProperties, Requires<ConditionManagerInfo>
	{
		readonly ConditionManager conditionManager;
		readonly Dictionary<string, Stack<int>> legacyShim = new Dictionary<string, Stack<int>>();

		public ConditionProperties(ScriptContext context, Actor self)
			: base(context, self)
		{
			conditionManager = self.Trait<ConditionManager>();
		}

		[Desc("Grant an external condition on this actor and return the revocation token.",
			"Conditions must be defined on an ExternalConditions trait on the actor.",
			"If duration > 0 the condition will be automatically revoked after the defined number of ticks")]
		public int GrantCondition(string condition, int duration = 0)
		{
			if (!conditionManager.AcceptsExternalCondition(Self, condition))
				throw new InvalidDataException("Condition `{0}` has not been listed on an ExternalConditions trait".F(condition));

			return conditionManager.GrantCondition(Self, condition, true, duration);
		}

		[Desc("Revoke a condition using the token returned by GrantCondition.")]
		public void RevokeCondition(int token)
		{
			conditionManager.RevokeCondition(Self, token);
		}

		[Desc("Check whether this actor accepts a specific external condition.")]
		public bool AcceptsCondition(string condition)
		{
			return conditionManager.AcceptsExternalCondition(Self, condition);
		}

		[Desc("Grant an upgrade to this actor. DEPRECATED! Will be removed.")]
		public void GrantUpgrade(string upgrade)
		{
			Game.Debug("GrantUpgrade is deprecated. Use GrantCondition instead.");
			legacyShim.GetOrAdd(upgrade).Push(GrantCondition(upgrade));
		}

		[Desc("Revoke an upgrade that was previously granted using GrantUpgrade. DEPRECATED! Will be removed.")]
		public void RevokeUpgrade(string upgrade)
		{
			Game.Debug("RevokeUpgrade is deprecated. Use RevokeCondition instead.");
			Stack<int> tokens;
			if (!legacyShim.TryGetValue(upgrade, out tokens) || !tokens.Any())
				throw new InvalidDataException("Attempting to revoke upgrade `{0}` that has not been granted.".F(upgrade));

			RevokeCondition(tokens.Pop());
		}

		[Desc("Grant a limited-time upgrade to this actor. DEPRECATED! Will be removed.")]
		public void GrantTimedUpgrade(string upgrade, int duration)
		{
			Game.Debug("GrantTimedUpgrade is deprecated. Use GrantCondition instead.");
			GrantCondition(upgrade, duration);
		}

		[Desc("Check whether this actor accepts a specific upgrade. DEPRECATED! Will be removed.")]
		public bool AcceptsUpgrade(string upgrade)
		{
			Game.Debug("AcceptsUpgrade is deprecated. Use AcceptsCondition instead.");
			return AcceptsCondition(upgrade);
		}
	}
}