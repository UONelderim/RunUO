using System;
using Server.Items;
using Server.Network;

namespace Server.Items
{
	public class SantasGiftBox2023 : GiftBox
	{
		[Constructable]
		public  SantasGiftBox2023 ()
		{
			Name = "Prezenty Od Pana";
			Hue = 1150;
            

            DropItem(new ChoinkaZThila());
            DropItem(new MagicznaElfickaSkarpeta());
            DropItem(new SwiatecznyStolik());
			
		
		}

        public SantasGiftBox2023(Serial serial)
            : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}