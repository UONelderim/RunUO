using System;
using Server;

namespace Server.Items
{
	public class PadsOfTheCuSidhe : FurBoots
	{


		[Constructable]
		public PadsOfTheCuSidhe() : base( 0x47E )
		{
			Name = "Buty Wilczego Jeźdźcy";
		}

		public PadsOfTheCuSidhe( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}