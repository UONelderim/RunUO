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
	public class SzczepkaMarchew : SeedWarzywo
    {
        public override Type PlantType => typeof(KrzakMarchew);

        [Constructable]
		public SzczepkaMarchew( int amount ) : base( amount, 0xF27) 
		{
			Hue = 0x5E2;
			Name = "Nasiona marchwi";
			Stackable = true;
		}

		[Constructable]
		public SzczepkaMarchew() : this( 1 )
		{
		}

		public SzczepkaMarchew( Serial serial ) : base( serial )
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
	
	public class KrzakMarchew : PlantWarzywo
    {
        public override Type SeedType => typeof(SzczepkaMarchew);
        public override Type CropType => typeof(Carrot);

        [Constructable] 
		public KrzakMarchew() : base(0xC76)
		{
            // seedling 0xC68
            //plant.PickGraphic = (0xC69);
            //plant.FullGraphic = (0xC76);
            Hue = 0;
			Name = "marchew";
			Stackable = true;
        }

		public KrzakMarchew( Serial serial ) : base( serial ) 
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