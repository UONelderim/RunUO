using System;
using Server.Targeting;
using Server.Network;
using Server.Items;

namespace Server.Spells.Second
{
	public class RemoveTrapSpell : MagerySpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Remove Trap", "An Jux",
				212,
				9001,
				Reagent.Bloodmoss,
				Reagent.SulfurousAsh
			);

		public override SpellCircle Circle { get { return SpellCircle.Second; } }

		public RemoveTrapSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
			Caster.SendMessage( "What do you wish to untrap?" );
		}

        // 10.07.2012 :: zombie
		public void Target( Item item )
		{
			if ( !Caster.CanSee( item ) )
			{
				Caster.SendLocalizedMessage( 500237 ); // Target can not be seen.
			}
            else if ( item is TrapableContainer && ((TrapableContainer)item).TrapType != TrapType.None && ((TrapableContainer)item).TrapType != TrapType.MagicTrap )
            {
                base.DoFizzle();
            }
            else if ( CheckSequence() )
            {
                if( item is TrapableContainer )
                {
                    TrapableContainer cont = (TrapableContainer)item;
                    SpellHelper.Turn( Caster, item );

                    Point3D loc = item.GetWorldLocation();

                    Effects.SendLocationParticles( EffectItem.Create( loc, item.Map, EffectItem.DefaultDuration ), 0x376A, 9, 32, 5015 );
                    Effects.PlaySound( loc, item.Map, 0x1F0 );

                    cont.TrapType = TrapType.None;
                    cont.TrapPower = 0;
                    cont.TrapLevel = 0;
                }
                else if( item is BaseTrap )
                {
                    BaseTrap trap = (BaseTrap)item;
                    trap.Untrap( Caster, null, true );
                }
            }

			FinishSequence();
		}
        // zombie

		private class InternalTarget : Target
		{
			private RemoveTrapSpell m_Owner;

			public InternalTarget( RemoveTrapSpell owner ) : base( 12, false, TargetFlags.None )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
                // 10.07.2012 :: zombie
				if ( o is TrapableContainer || o is BaseTrap )
				{
					m_Owner.Target( (Item)o );
				}
				else
				{
					from.SendMessage( "Nie mozesz tego rozbroic." );
				}
                // zombie
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}