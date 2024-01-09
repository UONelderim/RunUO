using System;
using Server.Network;
using Server.Targeting;

namespace Server.Items.Crops
{
	public class GinsengPlant : BaseCrop	// DEPRECATED (usuwane w deserializacji)
	{
		public GinsengPlant() : base( Utility.RandomList( 0x18E9, 0x18EA ) ) 
		{
            // DEPRECATED (usuwane w deserializacji)

            Movable = false; 
			Name = "Przegnita sadzonka zen-szenia";
		}

		public GinsengPlant( Serial serial ) : base( serial ) 
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

			// Delete();
		} 
	} 
	
	[FlipableAttribute( 0x18E7, 0x18E8 )]
	public class GinsengUprooted : Item // DEPRECATED (usuwane w deserializacji)
    {
		public GinsengUprooted() : this( 1 )
		{
            // DEPRECATED (usuwane w deserializacji)
        }

        public GinsengUprooted( int amount ) : base( Utility.RandomList( 0x18EB, 0x18EC ) )
		{
            // DEPRECATED (usuwane w deserializacji)

            Stackable = false;
			Weight = 1.0;
			
			Movable = true; 
			Amount = amount;

			Name = "przegnity korzen zen-szenia";
		}

		public GinsengUprooted( Serial serial ) : base( serial )
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

			// Delete();
        }
	}
}
