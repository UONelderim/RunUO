using System;

namespace Server.Items
{
	public class LordsHead : Item
	{

		[Constructable]
		public LordsHead() : base( 0x1DA0 )
		{
			Name = "Glioron's head";
			Weight = 4.0;
		}

		public LordsHead( Serial serial ) : base( serial )
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
