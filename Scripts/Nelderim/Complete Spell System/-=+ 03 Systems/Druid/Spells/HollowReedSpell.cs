using System;
using System.Collections;
using Server;
using Server.Targeting;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Spells;
using Server.Spells.Seventh;
using Server.Gumps;

namespace Server.ACC.CSS.Systems.Druid
{
	public class DruidHollowReedSpell : DruidSpell
	{
		private static SpellInfo m_Info = new SpellInfo(
		                                                "Siła Natury", "En Crur Aeta Sec En Ess ",
                                                        //SpellCircle.Second,
		                                                203,
		                                                9061,
		                                                false,
		                                                Reagent.Bloodmoss,
		                                                Reagent.MandrakeRoot,
		                                                Reagent.Nightshade
		                                               );

        public override SpellCircle Circle
        {
            get { return SpellCircle.Second; }
        }

		public override double CastDelay{ get{ return 1.5; } }
		public override double RequiredSkill{ get{ return 30.0; } }
		public override int RequiredMana{ get{ return 30; } }
		
		private static Hashtable m_Table = new Hashtable();

		public DruidHollowReedSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
			                    if (this.Scroll != null)
                        Scroll.Consume();
		}
		
		public static double GetScalar( Mobile m )
		{
			double val = 1.0;

			if ( m.CanBeginAction( typeof( DruidHollowReedSpell ) ) )
				val = 1.5;

			return val;
		}
		

		public override void OnCast()
		{
			if ( !Caster.CanBeginAction( typeof( DruidHollowReedSpell ) ) )
			{
				Caster.SendLocalizedMessage( 1005559 );
			}

			else if ( CheckSequence() )
			{
				object[] mods = new object[]
				{
					new StatMod( StatType.Dex, "[Druid] Dex Offset", (int)((Caster.Skills[SkillName.Zielarstwo].Value + Caster.Skills[SkillName.Magery].Value)/12), TimeSpan.Zero ),
					new StatMod( StatType.Str, "[Druid] Str Offset", (int)((Caster.Skills[SkillName.Zielarstwo].Value + Caster.Skills[SkillName.Magery].Value)/12), TimeSpan.Zero ),
					new StatMod( StatType.Int, "[Druid] Int Offset", (int)((Caster.Skills[SkillName.Zielarstwo].Value + Caster.Skills[SkillName.Magery].Value)/12), TimeSpan.Zero ),

				};

				m_Table[Caster] = mods;

				Caster.AddStatMod( (StatMod)mods[0] );
				Caster.AddStatMod( (StatMod)mods[1] );
				Caster.AddStatMod( (StatMod)mods[2] );


				double span = 10.0 * DruidHollowReedSpell.GetScalar( Caster );
				new InternalTimer( Caster, TimeSpan.FromMinutes( (int)span ) ).Start();
				
			}
		}
		
		
		public static void RemoveEffect( Mobile m )
		{
			object[] mods = (object[])m_Table[m];

			if ( mods != null )
			{
				m.RemoveStatMod( ((StatMod)mods[0]).Name );
				m.RemoveStatMod( ((StatMod)mods[1]).Name );
				m.RemoveStatMod( ((StatMod)mods[2]).Name );
			}

			m_Table.Remove( m );

			m.EndAction( typeof( DruidHollowReedSpell ) );

			m.BodyMod = 0;
		}
		
		
		private class InternalTimer : Timer
		{
			private Mobile m_Owner;
			private DateTime m_Expire;

			public InternalTimer( Mobile owner, TimeSpan duration ) : base( TimeSpan.Zero, TimeSpan.FromSeconds( 0.1 ) )
			{
				m_Owner = owner;
				m_Expire = DateTime.Now + duration;

			}

			protected override void OnTick()
			{
				if ( DateTime.Now >= m_Expire )
				{
					DruidHollowReedSpell.RemoveEffect( m_Owner );
					Stop();
				}
			}
		}
	}
}
