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

namespace OpenRA.Mods.Common.Traits
{
	[Desc("Modifies the range of weapons fired by this actor.")]
	public class RangeMultiplierInfo : ConditionalTraitInfo
	{
		[FieldLoader.Require]
		[Desc("Percentage modifier to apply.")]
		public readonly int Modifier = 100;

		public override object Create(ActorInitializer init) { return new RangeMultiplier(this); }
	}

	public class RangeMultiplier : ConditionalTrait<RangeMultiplierInfo>, IRangeModifierInfo
	{
		public RangeMultiplier(RangeMultiplierInfo info)
			: base(info) { }

		int IRangeModifierInfo.GetRangeModifierDefault() { return IsTraitDisabled ? 100 : Info.Modifier; }
	}
}
