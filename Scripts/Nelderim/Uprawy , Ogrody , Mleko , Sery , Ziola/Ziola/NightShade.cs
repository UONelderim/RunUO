using System;
using Server.Network;
using Server.Targeting;

namespace Server.Items.Crops
{
	public class NightshadePlant : BaseCropDeprecated // DEPRECATED (usuwane w deserializacji)
    {
		public NightshadePlant() : base( Utility.RandomList( 0x18E5, 0x18E6 ) ) 
		{
            // DEPRECATED (usuwane w deserializacji)

            Movable = false; 
			Name = "przegnite wilcze jagody";
		} 

		public NightshadePlant( Serial serial ) : base( serial ) 
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

			Name = "przegnite wilcze jagody";
			// Delete();
		} 
	} 
	
	[FlipableAttribute( 0x18E7, 0x18E8 )]
	public class NightshadeUprooted : Item  // DEPRECATED (usuwane w deserializacji)
    {
		public NightshadeUprooted() : this( 1 )
		{
            // DEPRECATED (usuwane w deserializacji)
        }

        public NightshadeUprooted( int amount ) : base( Utility.RandomList( 0x18E7, 0x18E8 ) )
		{
            // DEPRECATED (usuwane w deserializacji)

            Stackable = false;
			Weight = 1.0;
			
			Movable = true; 
			Amount = amount;

			Name = "przegnity Korzen wilczych jagod";
		}

		public NightshadeUprooted( Serial serial ) : base( serial )
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
			
			Name = "przegnity Korzen wilczych jagod";
			// Delete();
		}
	}
}
