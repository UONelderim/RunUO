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
	public class SzczepkaPszenica : SeedWarzywo
    {
        public override Type PlantType => typeof(KrzakPszenica);

        [Constructable]
		public SzczepkaPszenica( int amount ) : base( amount, 0xF27) 
		{
			Hue = 0x5E2;
			Name = "Nasiona pszenicy";
			Stackable = true;
		}

		[Constructable]
		public SzczepkaPszenica() : this( 1 )
		{
		}

		public SzczepkaPszenica( Serial serial ) : base( serial )
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
	
	public class KrzakPszenica : PlantWarzywo
    {
        public override Type SeedType => typeof(SzczepkaPszenica);
        public override Type CropType => typeof(Wheat);

        [Constructable] 
		public KrzakPszenica() : base(Utility.RandomList(0xC58, 0xC5A, 0xC5B))
		{
            // seedling 0xDAE, 0xDAF
            //plant.PickGraphic = Utility.RandomList(0xC55, 0xC56, 0xC57, 0xC59);
            //plant.FullGraphic = Utility.RandomList(0xC58, 0xC5A, 0xC5B);
            Hue = 0;
			Name = "pszenica";
			Stackable = true;
        }

		public KrzakPszenica( Serial serial ) : base( serial ) 
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