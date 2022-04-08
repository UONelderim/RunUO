using System;
using Server;
using Server.Network;

namespace Server.Items
{
	public enum GasTrapType
	{
		NorthWall,
		WestWall,
		Floor
	}

	public class GasTrap : BaseTrap
	{
		private Poison m_Poison;

		public Poison Poison
		{
			get{ return m_Poison; }
			set{ m_Poison = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public GasTrapType Type
		{
			get
			{
				switch ( ItemID )
				{
					case 0x113C: return GasTrapType.NorthWall;
					case 0x1147: return GasTrapType.WestWall;
					case 0x11A8: return GasTrapType.Floor;
				}

				return GasTrapType.WestWall;
			}
			set
			{
				ItemID = GetBaseID( value );
			}
		}

		public static int GetBaseID( GasTrapType type )
		{
			switch ( type )
			{
				case GasTrapType.NorthWall: return 0x113C;
				case GasTrapType.WestWall: return 0x1147;
				case GasTrapType.Floor: return 0x11A8;
			}

			return 0;
		}
        
        // 11.07.2012 :: zombie
        [Constructable]
		public GasTrap() : this( 0 )
		{
		}

		[Constructable]
		public GasTrap( int level ) : base( 0x1B71, level )
		{
		}
        // zombie

		public override bool PassivelyTriggered{ get{ return false; } }
		public override TimeSpan PassiveTriggerDelay{ get{ return TimeSpan.Zero; } }
		public override int PassiveTriggerRange{ get{ return 0; } }
		public override TimeSpan ResetDelay{ get{ return TimeSpan.FromSeconds( 0.0 ); } }

		public override void OnTrigger( Mobile from )
		{
            switch ( Level )
            {
                case TrapLevel.Small:
                    m_Poison = Poison.Lesser;
                break;
                case TrapLevel.Medium:
                    m_Poison = Poison.Greater;
                break;
                case TrapLevel.Big: 
                    m_Poison = Poison.Deadly;
                break;
            }

			if ( m_Poison == null || !from.Player || !from.Alive || from.AccessLevel > AccessLevel.Player )
				return;

			Effects.SendLocationEffect( Location, Map, GetBaseID( this.Type ) - 2, 16, 3, GetEffectHue(), 0 );
			Effects.PlaySound( Location, Map, 0x231 );

			from.ApplyPoison( from, m_Poison );

			from.LocalOverheadMessage( MessageType.Regular, 0x22, 500855 ); // You are enveloped by a noxious gas cloud!
		}

		public GasTrap( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			Poison.Serialize( m_Poison, writer );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_Poison = Poison.Deserialize( reader );
					break;
				}
			}
		}
	}
}