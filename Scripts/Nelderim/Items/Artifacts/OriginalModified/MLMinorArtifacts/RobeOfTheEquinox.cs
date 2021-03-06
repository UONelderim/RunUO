using System;
using Server;

namespace Server.Items
{
	[Flipable( 0x1F03, 0x1F04 )]
	public class RobeOfTheEquinox : BaseOuterTorso
	{
        public override int LabelNumber { get { return 1075042; } } // Szata Rownonocy

		[Constructable]
		public RobeOfTheEquinox() : base( 0x1F04, 0xD6 )
		{
			Weight = 3.0;

			Attributes.Luck = 95;
		}

		public RobeOfTheEquinox( Serial serial ) : base( serial )
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