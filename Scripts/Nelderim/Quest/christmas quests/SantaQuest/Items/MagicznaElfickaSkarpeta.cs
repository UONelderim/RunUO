//Created by Milva
using System;
using Server;
using Server.Items;
namespace Server.Items
{
        
	public class MagicznaElfickaSkarpeta :  Item
	{

        [Constructable]
		public MagicznaElfickaSkarpeta ()
		{
			Weight = 1.0; 
            Name = "Magiczna Elficka Skarpeta"; 
            ItemID = 0x2B14;  
            Hue = 1151;                  
        }

        public MagicznaElfickaSkarpeta(Serial serial)
            : base(serial)
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}