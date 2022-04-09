using System;

namespace Server.Items
{
	public class Drums : BaseInstrument
	{
		[Constructable]
		public Drums() : base( 0xE9C, 0x38, 0x39 )
		{
			Weight = 3.0;
		}

		public Drums( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version == 0 )
				if (Weight == 4.0)
					Weight = 3.0;
		}
	}
}