using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
	public class PrismOfLightTele : Teleporter
	{			
		[Constructable]
		public PrismOfLightTele() : base( new Point3D( 6474, 188, 0 ), Map.Trammel )
		{
		}
		
		public PrismOfLightTele( Serial serial ) : base( serial )
		{
		}
	
		public override bool OnMoveOver( Mobile m )
		{
			if ( m.NetState == null || !m.NetState.SupportsExpansion( Expansion.ML ) )
			{
				m.SendLocalizedMessage( 1072608 ); // You must upgrade to the Mondain's Legacy expansion in order to enter here.				
				return true;
			}	
			else if ( /*!MondainsLegacy.PrismOfLight &&*/ (int) m.AccessLevel < (int) AccessLevel.GameMaster )
			{
				m.SendLocalizedMessage( 1042753, "Prism of Light" ); // ~1_SOMETHING~ has been temporarily disabled.
				return true;
			}
		
			if ( m.Backpack != null )
			{
				/*if ( m.Backpack.FindItemByType( typeof( PrismOfLightAdmissionTicket ), true ) != null )
					return base.OnMoveOver( m );*/
			}
			
			m.SendLocalizedMessage( 1074277 ); // No admission without a ticket.
			
			return true;	
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
		}
	}

    public class PrismOfLightTeleFel : Teleporter
    {
        [Constructable]
        public PrismOfLightTeleFel()
            : base(new Point3D(6474, 188, 0), Map.Felucca)
        {
        }

        public PrismOfLightTeleFel(Serial serial)
            : base(serial)
        {
        }

        public override bool OnMoveOver(Mobile m)
        {
            if (m.NetState == null || !m.NetState.SupportsExpansion(Expansion.ML))
            {
                m.SendLocalizedMessage(1072608); // You must upgrade to the Mondain's Legacy expansion in order to enter here.				
                return true;
            }
            else if (/*!MondainsLegacy.PrismOfLight &&*/ (int)m.AccessLevel < (int)AccessLevel.GameMaster)
            {
                m.SendLocalizedMessage(1042753, "Prism of Light"); // ~1_SOMETHING~ has been temporarily disabled.
                return true;
            }

            if (m.Backpack != null)
            {
                /*if (m.Backpack.FindItemByType(typeof(PrismOfLightAdmissionTicket), true) != null)
                    return base.OnMoveOver(m);*/
            }

            m.SendLocalizedMessage(1074277); // No admission without a ticket.

            return true;
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }

	public class CrystalFieldTele : Item
	{
		public override TimeSpan DecayTime{ get{ return TimeSpan.FromMinutes( 1 ); } }	
	
		[Constructable]
		public CrystalFieldTele() : base( 0x3818 )
		{
			Movable = false;
		}
		
		public CrystalFieldTele( Serial serial ) : base( serial )
		{
		}
	
		public override bool OnMoveOver( Mobile m )
		{
			if ( m.Player )
			{
				if ( Utility.RandomBool() )
					m.MoveToWorld( new Point3D( 6523, 71, -10 ), m.Map );
				
				Delete();
				return false;
			}
			else
				return true;
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
		}
	}
}