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

	
	public class SzczepkaWilczaJagoda : BaseSeedling
    {
        public override Type PlantType => typeof(KrzakWilczaJagoda);

		[Constructable]
		public SzczepkaWilczaJagoda( int amount ) : base( amount, 0x18E7 ) 
		{
			Hue = 0;
			Name = "Szczepka wilczej jagody";
			Stackable = true;
		}

		[Constructable]
		public SzczepkaWilczaJagoda() : this( 1 )
		{
		}

		public SzczepkaWilczaJagoda( Serial serial ) : base( serial )
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
	
	public class KrzakWilczaJagoda : Plant
    {
        public override Type SeedType => typeof(SzczepkaWilczaJagoda);
        public override Type CropType => typeof(PlonWilczaJagoda);

		[Constructable] 
		public KrzakWilczaJagoda() : base( 0x18E6 )
		{
			GrowingTimeInSeconds = WeedHelper.DefaultHerbGrowingTimeInSeconds;
			Hue = 0;
			Name = "Krzak wilczych jagod";
			Stackable = true;
		}

		public KrzakWilczaJagoda( Serial serial ) : base( serial ) 
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
	
	public class PlonWilczaJagoda : Crop
    {
        public override Type ReagentType => typeof(Nightshade);
		
		[Constructable]
		public PlonWilczaJagoda( int amount ) : base( amount, 0x18E8 )
		{
			Hue = 0;
			Name = "Galazka wilczej jagody";
			Stackable = true;
		}

		[Constructable]
		public PlonWilczaJagoda() : this( 1 )
		{
		}

		public PlonWilczaJagoda( Serial serial ) : base( serial )
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