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
		public override Item CreateWeed() { return new PlainTobaccoPlant(); }

		public SzczepkaTyton( int amount ) : base( amount, 0x0CB0) 
		{
			Hue = 2129;
			Name = "Zwiednieta szczepka tytoniu";
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
		}
	}
	
	public class KrzakTyton : WeedPlantZiolaUprawne
	{
        public override string MsgGotSeed { get { return "Ta roslina nie daje szczepki."; } }

        public override void CreateCrop(Mobile from, int count) { from.AddToBackpack( new PlainTobaccoCrop(1) ); }

		public override void CreateSeed(Mobile from, int count) {  }

        public KrzakTyton() : base(0x0C97)
		{ 
			Hue = 2129;
			Name = "Zwiedly tyton";
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
		} 
	} 
	
	public class PlonTyton : WeedCropZiolaUprawne
	{
		public override void CreateReagent(Mobile from, int count) { from.AddToBackpack( new PlainTobacco(1) ); }
		
		public PlonTyton( int amount ) : base( amount, 0x0C93)
		{
			Hue = 2129;
			Name = "Zgnite liscie tytoniu";
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
		}
	}


}