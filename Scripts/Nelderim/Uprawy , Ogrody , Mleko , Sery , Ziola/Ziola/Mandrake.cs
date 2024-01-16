using System;
using Server.Network;
using Server.Targeting;

namespace Server.Items.Crops
{
	public class MandrakePlant : BaseCropDeprecated   // DEPRECATED (usuwane w deserializacji)
    {
		public MandrakePlant() : base( Utility.RandomList( 0x18DF, 0x18E0 ) ) 
		{
            // DEPRECATED (usuwane w deserializacji)

            Movable = false; 
			Name = "przegnity Kwiat Mandragory";
		}

		public MandrakePlant( Serial serial ) : base( serial ) 
		{
		} 

		public override void Serialize( GenericWriter writer ) 
		{ 
			base.Serialize( writer ); 
			writer.Write( (int) 0 ); 
		} 

		public override void Deserialize( GenericReader reader ) 
		{ 
			base.Deserialize( reader ); 
			int version = reader.ReadInt();

			Name = "przegnity Kwiat Mandragory";
			// Delete();
		} 
	} 
	
	[FlipableAttribute( 0x18DD, 0x18DE )]
	public class MandrakeUprooted : Item    // DEPRECATED (usuwane w deserializacji)
    {
		public MandrakeUprooted() : this( 1 )
		{
            // DEPRECATED (usuwane w deserializacji)
        }

        public MandrakeUprooted( int amount ) : base( Utility.RandomList( 0x18DD, 0x18DE ) )
		{
            // DEPRECATED (usuwane w deserializacji)

            Stackable = false;
			Weight = 1.0;
			
			Movable = true; 
			Amount = amount;

			Name = "przegnity korzen mandragory";
		}

		public MandrakeUprooted( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			Name = "przegnity korzen mandragory";
			// Delete();
		}
	}
}
