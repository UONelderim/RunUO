using System;
using Server.Spells;

namespace Server.Items
{
	public abstract partial class BaseTrap : Item
	{
		public virtual bool PassivelyTriggered{ get{ return false; } }
		public virtual TimeSpan PassiveTriggerDelay{ get{ return TimeSpan.Zero; } }
		public virtual int PassiveTriggerRange{ get{ return -1; } }
		public virtual TimeSpan ResetDelay{ get{ return TimeSpan.Zero; } }

		private DateTime m_NextPassiveTrigger, m_NextActiveTrigger;
                        
		public virtual void OnTrigger( Mobile from )
		{
		}

		public override bool HandlesOnMovement{ get{ return true; } } // Tell the core that we implement OnMovement

		public virtual int GetEffectHue()
		{
			int hue = this.Hue & 0x3FFF;

			if ( hue < 2 )
				return 0;

			return hue - 1;
		}

		public bool CheckRange( Point3D loc, Point3D oldLoc, int range )
		{
			return CheckRange( loc, range ) && !CheckRange( oldLoc, range );
		}

		public bool CheckRange( Point3D loc, int range )
		{
			return ( (this.Z + 8) >= loc.Z && (loc.Z + 16) > this.Z )
				&& Utility.InRange( GetWorldLocation(), loc, range );
		}

		public override void OnMovement( Mobile m, Point3D oldLocation )
		{
			base.OnMovement( m, oldLocation );

			if ( m.Location == oldLocation )
				return;

			if ( CheckRange( m.Location, oldLocation, 0 ) && DateTime.Now >= m_NextActiveTrigger )
			{
				m_NextActiveTrigger = m_NextPassiveTrigger = DateTime.Now + ResetDelay;

				OnTrigger( m );
			}
			else if ( PassivelyTriggered && CheckRange( m.Location, oldLocation, PassiveTriggerRange ) && DateTime.Now >= m_NextPassiveTrigger )
			{
				m_NextPassiveTrigger = DateTime.Now + PassiveTriggerDelay;

				OnTrigger( m );
			}
		}

        // 11.07.2012 :: zombie 
        public BaseTrap( int itemID ) : this( itemID, 0 )
		{
		}

		public BaseTrap( int itemID, int level ) : base( itemID )
		{
			Movable = false;
            Visible = false;

            try
            {
                if( Enum.IsDefined( typeof( TrapLevel ), level ) )
                    Level = (TrapLevel)level;
            }
            catch
            {
                Level = TrapLevel.Small;
            }    
		}

        public int Damage
        {
            get 
            { 
                int damage = 0;

                switch ( Level )
                {
                    case TrapLevel.Small:
                        damage = 30;
                        break;
                    case TrapLevel.Medium:
                        damage = 60;
                        break;
                    case TrapLevel.Big:
                        damage = 80;
                        break;
                }

                return damage; 
            }
        }
        // zombie

		public BaseTrap( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            switch ( version )
            {
                case 1:
                {
                    Level = (TrapLevel)reader.ReadInt();

                    goto case 0;
                }
				case 0:
				{
					break;
				}
            }
		}
	}
}