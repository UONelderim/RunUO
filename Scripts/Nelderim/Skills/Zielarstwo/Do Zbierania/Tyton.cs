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
		public override void CreateCrop(Mobile from, int count) { from.AddToBackpack( new PlainTobaccoCrop(1) ); }

		public override bool GivesSeed{ get{ return false; } }

		public ZrodloTyton() : base( 0x0CC7 ) 
		{ 
			Hue = 2129;
			Name = "Zwiedniety Krzak Tytoniu";	
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
		} 
	} 
	
	public class SurowiecTyton : WeedCropZbieractwo
	{
		public override void CreateReagent(Mobile from, int count) { from.AddToBackpack( new PlainTobacco(1) ); }
		
		public SurowiecTyton( int amount ) : base( amount, 0x0F88 )
		{
			Hue = 2129;
			Name = "Zwiedniety krzaczek tytoniu";
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
		}
	}


}