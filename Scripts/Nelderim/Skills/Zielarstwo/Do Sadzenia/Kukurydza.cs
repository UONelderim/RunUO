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
	public class SzczepkaKukurydza : SeedWarzywo
    {
        public override Type PlantType => typeof(KrzakKukurydza);

        [Constructable]
		public SzczepkaKukurydza( int amount ) : base( amount, 0xF27) 
		{
			Hue = 0x5E2;
			Name = "Nasiona kukurydzy";
			Stackable = true;
		}

		[Constructable]
		public SzczepkaKukurydza() : this( 1 )
		{
		}

		public SzczepkaKukurydza( Serial serial ) : base( serial )
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
	
	public class KrzakKukurydza : PlantWarzywo
    {
        public override Type SeedType => typeof(SzczepkaKukurydza);
        public override Type CropType => typeof(Corn);

        [Constructable] 
		public KrzakKukurydza() : base(0xC7D)
		{
            // seedling -
            //plant.PickGraphic = (0xC7E);
            //plant.FullGraphic = (0xC7D);
            Hue = 0;
			Name = "kukurydza";
			Stackable = true;
        }

		public KrzakKukurydza( Serial serial ) : base( serial ) 
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