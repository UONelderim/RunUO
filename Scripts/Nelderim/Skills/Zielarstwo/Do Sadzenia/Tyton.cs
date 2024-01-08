using System;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Targets;
using Server.Items;
using Server.Targeting;
using Server.Spells;
using Server.Mobiles;

namespace Server.Items.Crops
{

	
	public class SzczepkaTyton : WeedSeedZiolaUprawne
    {
        public override Type PlantType => typeof(PlainTobaccoPlant);

		public SzczepkaTyton( int amount ) : base( amount, 0x166F ) 
		{
			Hue = 2129;
			Name = "Zwiedla szczepka tytoniu";
			Stackable = true;
		}

		public SzczepkaTyton() : this( 1 )
		{
		}

		public SzczepkaTyton( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int)0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

            Delete();
        }
	}
	
	public class KrzakTyton : WeedPlantZiolaUprawne
    {
        public override Type SeedType => typeof(PlainTobaccoSapling);
        public override Type CropType => typeof(PlainTobaccoCrop);

		public KrzakTyton() : base( 0x0F88 )
		{ 
			Hue = 2129;
			Name = "Zwiedly Tyton";
			Stackable = true;
		}

		public KrzakTyton( Serial serial ) : base( serial ) 
		{ 
			//m_plantedTime = DateTime.Now;	// ???
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

			Delete();
		} 
	} 
	
	public class PlonTyton : WeedCropZiolaUprawne
    {
        public override Type ReagentType => typeof(PlainTobacco);
		
		public PlonTyton( int amount ) : base( amount, 0x16C0 )
		{
			Hue = 2129;
			Name = "Swieza lodyga tytoniu";
			Stackable = true;
		}

		public PlonTyton() : this( 1 )
		{
		}

		public PlonTyton( Serial serial ) : base( serial )
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

            Delete();
        }
	}


}