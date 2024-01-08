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
	public class ZrodloTyton : WeedPlantZbieractwo
    {
        public override Type CropType => typeof(PlainTobaccoCrop);

		public ZrodloTyton() : base( 0x0CC7 ) 
		{ 
			Hue = 2129;
			Name = "Zwiedly Krzak Tytoniu";	
			Stackable = true;			
		}

		public ZrodloTyton( Serial serial ) : base( serial ) 
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
	
	public class SurowiecTyton : WeedCropZbieractwo
    {
        public override Type ReagentType => typeof(PlainTobacco);
		
		public SurowiecTyton( int amount ) : base( amount, 0x0F88 )
		{
			Hue = 2129;
			Name = "zwiedly krzaczek tytoniu";
			Stackable = true;
		}

		public SurowiecTyton() : this( 1 )
		{
		}

		public SurowiecTyton( Serial serial ) : base( serial )
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