using System;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Spells.Eighth
{
	public class AirElementalSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Air Elemental", "Kal Vas Xen Hur",
				269,
				9010,
				false,
				Reagent.Bloodmoss,
				Reagent.MandrakeRoot,
				Reagent.SpidersSilk
			);

		public override SpellCircle Circle { get { return SpellCircle.Eighth; } }

		public AirElementalSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		private int SummonedControlSlots { get { return AllowPoisonElemental ? SummonedPoisonElemental.SummonedControlSlots : 2; } }
		private bool AllowPoisonElemental { get { return ((Caster.Skills.Magery.Fixed + Caster.Skills.Poisoning.Fixed) / 2) >= 1000; } }


		public override bool CheckCast()
		{
			if ( !base.CheckCast() )
				return false;

			if ( (Caster.Followers + SummonedControlSlots) > Caster.FollowersMax )
			{
				Caster.SendLocalizedMessage( 1049645 ); // You have too many followers to summon that creature.
				return false;
			}

			return true;
		}

		public override void OnCast()
		{
			if ( CheckSequence() )
			{
				TimeSpan duration = TimeSpan.FromSeconds( (2 * Caster.Skills.Magery.Fixed) / 5 );

				if (AllowPoisonElemental)
					SpellHelper.Summon(new SummonedPoisonElemental(), Caster, 0x217, duration, false, false);
				else
					SpellHelper.Summon( new SummonedAirElemental(), Caster, 0x217, duration, false, false );
			}

			FinishSequence();
		}
	}
}