using System;
using Server;
using Server.Items;

namespace Server.Items
{
	public class WaterloggedBoots : Boots
	{		
		public override int LabelNumber{ get{ return 1074364; } } // Waterlogged boots
		
		[Constructable]
		public WaterloggedBoots() : base()
		{
		}

		public WaterloggedBoots( Serial serial ) : base( serial )
		{		
		}
		
		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			
			list.Add( 1073634 ); // An aquarium decoration
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