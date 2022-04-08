using System;
using Server;
using Server.Targeting; 
using Server.Network;
using Server.Mobiles;   
using Server.Misc;           
using Server.Items;
using Server.Timers.LodowySmok;

namespace Server.Spells.LodowySmok
{
    public class IceBreathSpell2 : MagerySpell
		{
			public int starthue;
			
			private static readonly SpellInfo m_Info = new SpellInfo(
					"Lodowy Podmuch", "An Nox Fortis",
					218,
					9012

				);

			public IceBreathSpell2( Mobile caster, Item scroll  ) : base( caster, scroll, m_Info )
			{
			}

            public override SpellCircle Circle
            {
                get
                {
                    return SpellCircle.Third;
                }
            }

            public override void OnCast()
			{
				Caster.Target = new InternalTarget( this );
				
			}
			
			public int Gethue()
			{
				return (starthue);
			}

			public void Target( Mobile m )
			{
				if ( !Caster.CanSee( m ) )
				{
					Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
				}
				else if ( Core.AOS && (m.Frozen || m.Paralyzed || (m.Spell != null && m.Spell.IsCasting)) )
				{
					Caster.SendLocalizedMessage( 1061923 ); // The target is already frozen.
				}
				else if ( CheckHSequence( m ) )
				{
					SpellHelper.Turn( Caster, m );

					SpellHelper.CheckReflect( (int)this.Circle, Caster, ref m );
					

					if ((( Caster.Skills[SkillName.Magery].Value * Utility.RandomMinMax( 1, 2 )) / m.Skills[SkillName.MagicResist].Value + 1 ) > 1 )
					{	
						Caster.Mana = Caster.Mana - 100;
						m.Frozen = true;
                        m.CantWalk = true;
				        m.Squelched = true;
						m.PlaySound( 0x16 );
						starthue = m.Hue;

						m.Hue = 1266; // Go to ice hue
						m.SendMessage("Lodowy Podmuch zmrozil krew w twych zylach!");
						IceBreathTimer2 Timer = new IceBreathTimer2 ( m, this );
						Timer.Start();
					}
					else
					   m.SendMessage("Zdolales przeciwstawic sie lodowemu podmuchowi!");
					
				}

			FinishSequence();
			}

		}		
			
		public class InternalTarget : Target
		{
			private IceBreathSpell2 m_Owner;

			public InternalTarget( IceBreathSpell2 owner ) : base( 12, false, TargetFlags.Harmful )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is Mobile )
					m_Owner.Target( (Mobile)o );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
				
		}
}
