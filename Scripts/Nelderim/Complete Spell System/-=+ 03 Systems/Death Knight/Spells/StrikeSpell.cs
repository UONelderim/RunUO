using System;
using System.Collections;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Items;

namespace Server.Spells.DeathKnight
{
	public class StrikeSpell : DeathKnightSpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Uderzenie", "Naberius Impetus",
				230,
				9022
			);

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 3 ); } }
		public override int RequiredTithing{ get{ return 140; } }
		public override double RequiredSkill{ get{ return 80.0; } }
		public override int RequiredMana{ get{ return 30; } }

		public StrikeSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public void Target( Mobile m )
		{
			if ( !Caster.CanSee( m ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
			else if ( CheckHSequence( m ) /*&& CheckFizzle()*/ )
			{
				m.FixedParticles( 0x36BD, 20, 10, 5044, EffectLayer.Head );
				m.PlaySound( 0x307 );

				SpellHelper.Turn( Caster, m );

				double damage = GetKarmaPower( Caster ) / 2;

				SpellHelper.Damage( TimeSpan.Zero, m, Caster, damage, 0, 0, 0, 0, 100 );
			}

			FinishSequence();
		}


		private class InternalTarget : Target
		{
			private StrikeSpell m_Owner;

			public InternalTarget( StrikeSpell owner ) : base( 12, false, TargetFlags.Harmful )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is Mobile )
				{
					m_Owner.Target( (Mobile)o );
				}
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}