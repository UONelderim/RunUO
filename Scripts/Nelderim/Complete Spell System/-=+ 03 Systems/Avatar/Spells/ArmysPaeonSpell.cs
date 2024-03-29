using System;
using System.Collections;
using Server;
using Server.Mobiles;
using Server.Network;
using Server.Items;
using Server.Spells;

namespace Server.ACC.CSS.Systems.Avatar
{
	public class AvatarArmysPaeonSpell : AvatarSpell
	{

		private static SpellInfo m_Info = new SpellInfo(
		                                                "Witalność Armii", "Vitalium Engrevo Maxi",
		                                                //SpellCircle.First,
		                                                212,
		                                                9041
		                                               );

        public override SpellCircle Circle
        {
            get { return SpellCircle.First; }
        }

		public override double CastDelay{ get{ return 2; } }
		public override int RequiredTithing{ get{ return 50; } }
		public override double RequiredSkill{ get{ return 60.0; } }
		public override int RequiredMana{ get{ return 15; } }

		public AvatarArmysPaeonSpell( Mobile caster, Item scroll) : base( caster, scroll, m_Info )
		{
			                    if (this.Scroll != null)
                        Scroll.Consume();
		}

		public override void OnCast()
		{
			if( CheckSequence() )
			{
				ArrayList targets = new ArrayList();

				IPooledEnumerable eable = Caster.GetMobilesInRange( 3 );
				foreach ( Mobile m in eable )
				{
					if ( Caster.CanBeBeneficial( m, false, true ) && !(m is Golem) )
						targets.Add( m );
				}
				eable.Free();

				for ( int i = 0; i < targets.Count; ++i )
				{
					Mobile m = (Mobile)targets[i];

					TimeSpan duration = TimeSpan.FromSeconds( Caster.Skills[CastSkill].Value * 0.6 );
					int rounds = (int)( Caster.Skills[SkillName.Anatomy].Value * 0.5 );

					new ExpireTimer( m, 0, rounds, TimeSpan.FromSeconds( 5 ) ).Start();

					m.FixedParticles( 0x376A, 9, 32, 5030, 1153, 3, EffectLayer.Waist );
				}
			}

			FinishSequence();
		}

		private class ExpireTimer : Timer
		{
			private Mobile m_Mobile;
			private int m_Round;
			private int m_Totalrounds;

			public ExpireTimer( Mobile m, int round, int totalrounds, TimeSpan delay ) : base( delay )
			{
				m_Mobile = m;
				m_Round = round;
				m_Totalrounds = totalrounds;
			}

			protected override void OnTick()
			{
				if ( m_Mobile != null )
				{

					m_Mobile.Hits += 5;

					if ( m_Round >= m_Totalrounds )
					{
						m_Mobile.SendMessage( "Efekt modlitwy wygasa." );

					}
					else
					{
						m_Round += 1;
						new ExpireTimer( m_Mobile, m_Round, m_Totalrounds, TimeSpan.FromSeconds( 2 ) ).Start();
					}
				}
			}
		}
	}
}
