using System;
using Server;

namespace Server.Items
{
	public class SeaHorse : BaseFish
	{		
		public override int LabelNumber{ get{ return 1074414; } } // A sea horse
		
		[Constructable]
		public SeaHorse() : base ( 0x3B10 )
		{
		}

		public SeaHorse( Serial serial ) : base( serial )
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