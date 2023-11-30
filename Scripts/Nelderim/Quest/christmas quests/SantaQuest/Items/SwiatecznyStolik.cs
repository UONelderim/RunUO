
using System;
using Server;
using Server.Items;
namespace Server.Items
{
        
	public class SwiatecznyStolik :  Item
	{

        [Constructable]
		public SwiatecznyStolik ()
		{
			Weight = 10.0; 
            Name = "Odswietny Stoliczek"; 
            ItemID = 0x118D;  
            Hue = 0;                  
        }

        public SwiatecznyStolik(Serial serial)
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