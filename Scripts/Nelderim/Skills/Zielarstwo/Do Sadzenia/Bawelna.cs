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
	// TODO: ustawic dodatkowy skill krawiectwo i zwiekszyc progi umozliwiajace zbieranie
	
	public class SzczepkaBawelna : WeedSeedZiolaUprawne
	{
		public override Item CreateWeed() { return new KrzakBawelna(); }

		[Constructable]
		public SzczepkaBawelna( int amount ) : base( amount, 6946 ) 
		{
			Hue = 661;
			Name = "Ziarno bawelny";
			Stackable = true;
		}

		[Constructable]
		public SzczepkaBawelna() : this( 1 )
		{
		}

		public SzczepkaBawelna( Serial serial ) : base( serial )
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
	
	public class KrzakBawelna : WeedPlantZiolaUprawne
	{
		public override void CreateCrop(Mobile from, int count) { from.AddToBackpack( new PlonBawelna(count/2) ); }

		public override void CreateSeed(Mobile from, int count) { from.AddToBackpack( new SzczepkaBawelna(count*3) ); } 

		[Constructable] 
		public KrzakBawelna() : base( 3155 )
		{ 
			Hue = 0;
			Name = "Krzak bawelny";
			Stackable = true;
		}

		public KrzakBawelna( Serial serial ) : base( serial ) 
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
	
	public class PlonBawelna : WeedCropZiolaUprawne
	{
		public override void CreateReagent(Mobile from, int count) { from.AddToBackpack( new Cotton(count) ); }
		
		[Constructable]
		public PlonBawelna( int amount ) : base( amount, 3577 )
		{
			Hue = 661;
			Name = "Klebek bawelny";
			Stackable = true;
		}

		[Constructable]
		public PlonBawelna() : this( 1 )
		{
		}

		public PlonBawelna( Serial serial ) : base( serial )
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