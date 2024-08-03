//
// Identyczne jak MeleeAI, nie uzywac!!
//

namespace Server.Mobiles
{
	public class MountedAI : BaseAI
	{
		public MountedAI(BaseCreature m) : base (m)
		{
		}

		public override bool DoActionWander()
		{
			m_Mobile.DebugSay( "I have no combatant" );

            OnGuardActionWarden();

			if ( ( m_Mobile.GetDistanceToSqrt( m_Mobile.Home ) > m_Mobile.RangePerception * 4 || !m_Mobile.InLOS( m_Mobile.Home ) ) && !( m_Mobile.Home == new Point3D( 0, 0, 0) ) )
			{
				m_Mobile.DebugSay( "I am to far" );
				m_Mobile.SetLocation( ( m_Mobile as BaseCreature ).Home, false );
			}		

			if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, false ) )
			{
				if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "I have detected {0}, attacking", m_Mobile.FocusMob.Name );

                OnGuardActionAttack( m_Mobile.FocusMob );
						
				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;
			}
			else
			{
				base.DoActionWander();
			}

			return true;
		}

		public override bool DoActionCombat()
		{
			Mobile combatant = m_Mobile.Combatant;

			if ( combatant == null || combatant.Deleted || combatant.Map != m_Mobile.Map || !combatant.Alive || combatant.IsDeadBondedPet )
			{
				m_Mobile.DebugSay( "My combatant is gone, so my guard is up" );

				Action = ActionType.Guard;
				
				// DeleteBody( m_Mobile );

				return true;
			}	
			
			if ( !m_Mobile.InRange( combatant, m_Mobile.RangePerception ) )
			{
				// They are somewhat far away, can we find something else?

				if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, false ) )
				{
					m_Mobile.Combatant = m_Mobile.FocusMob;
					m_Mobile.FocusMob = null;
				}
				else if ( !m_Mobile.InRange( combatant, m_Mobile.RangePerception * 3 ) )
				{
					m_Mobile.Combatant = null;
				}

				combatant = m_Mobile.Combatant;
				
				// Przeciwnik zwial, wiec wracam na posterunek
				
				if ( combatant == null )
				{
					m_Mobile.DebugSay( "My combatant has fled, so I am returning to my post" );
					Action = ActionType.Guard;
					
					m_Mobile.SetLocation( m_Mobile.Home, false );
					
					return true;
				}
			}

			if ( MoveTo( combatant, true, m_Mobile.RangeFight ) )
			{
				m_Mobile.Direction = m_Mobile.GetDirectionTo( combatant );
			}
			else if ( AcquireFocusMob( m_Mobile.RangePerception, m_Mobile.FightMode, false, false, false ) )
			{
				if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "My move is blocked, so I am going to attack {0}", m_Mobile.FocusMob.Name );

				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;

				return true;
			}
			else if ( m_Mobile.GetDistanceToSqrt( combatant ) > m_Mobile.RangePerception + 1 )
			{
				if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "I cannot find {0}, so my guard is up", combatant.Name );

				Action = ActionType.Guard;

				return true;
			}
			else
			{
				if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "I should be closer to {0}", combatant.Name );
			}

			if ( m_Mobile.Hits < m_Mobile.HitsMax * 0.1 )
			{
				// We are low on health, should we flee?

				bool flee = false;

				if ( m_Mobile.Hits < m_Mobile.Combatant.Hits )
				{
					// We are more hurt than them

					int diff = m_Mobile.Combatant.Hits - m_Mobile.Hits;

					flee = ( Utility.Random( 0, 100 ) < ( 5 + diff ) ); // (5 + diff)% chance to flee
				}
				else
				{
					flee = Utility.Random( 0, 100 ) < 5; // 5% chance to flee
				}

				if ( flee )
				{
					if ( m_Mobile.Debug )
						m_Mobile.DebugSay( "Panic! Leave the outpost! I am going to flee from {0}", m_Mobile.Combatant.Name );

					Action = ActionType.Flee;
				}
			}

			return true;
		}

		public override bool DoActionGuard()
		{	
			if ( AcquireFocusMob(m_Mobile.RangePerception, m_Mobile.FightMode, false, false, false ) )
			{
				if ( m_Mobile.Debug )
					m_Mobile.DebugSay( "I have detected {0}, attacking", m_Mobile.FocusMob.Name );
	
				m_Mobile.Combatant = m_Mobile.FocusMob;
				Action = ActionType.Combat;
			}
			else
			{
				if( !m_Mobile.CanBeginAction( "grab" ) )
				{
					m_Mobile.DebugSay( "I must wait when i finish looting corpse" );
					Action = ActionType.Guard;
				}
				else
				{
					base.DoActionGuard();
				}
			}

			return true;
		}
	}
}
