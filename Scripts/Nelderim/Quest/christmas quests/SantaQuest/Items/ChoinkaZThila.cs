//Created by Milva
using System;
using Server;
using Server.Items;
namespace Server.Items
{
        
	public class ChoinkaZThila :  Item
	{

        [Constructable]
		public ChoinkaZThila ()
		{
			Weight = 1.0; 
            Name = "Uschniete Pustynne Drzewko z Thila"; 
            ItemID = 0x3021;  
            Hue = 1151;                  
        }

        public ChoinkaZThila(Serial serial)
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