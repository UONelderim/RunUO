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

	
	public class SzczepkaMandragora : BaseSeedling
    {
        public override Type PlantType => typeof(KrzakMandragora);

		[Constructable]
		public SzczepkaMandragora( int amount ) : base( amount, 0x18DD ) 
		{
			Hue = 0;
			Name = "Szczepka mandragory";
			Stackable = true;
		}

		[Constructable]
		public SzczepkaMandragora() : this( 1 )
		{
		}

		public SzczepkaMandragora( Serial serial ) : base( serial )
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
	
	public class KrzakMandragora : Plant
    {
        public override Type SeedType => typeof(SzczepkaMandragora);
        public override Type CropType => typeof(PlonMandragora);

		[Constructable] 
		public KrzakMandragora() : base( 0x18E0 )
		{
			GrowingTimeInSeconds = WeedHelper.DefaultHerbGrowingTimeInSeconds;
			Hue = 0;
			Name = "Mandragora";
			Stackable = true;
		}

		public KrzakMandragora( Serial serial ) : base( serial ) 
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
	
	public class PlonMandragora : Crop
    {
        public override Type ReagentType => typeof(MandrakeRoot);
		
		[Constructable]
		public PlonMandragora( int amount ) : base( amount, 0x18DE )
		{
			Hue = 0;
			Name = "Swiezy korzen mandragory";
			Stackable = true;
		}

		[Constructable]
		public PlonMandragora() : this( 1 )
		{
		}

		public PlonMandragora( Serial serial ) : base( serial )
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