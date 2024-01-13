using System;

namespace Server.Items
{
	public class EvilHead : Item
	{

		[Constructable]
		public EvilHead() : base( 0x1DA0 )
		{
			Name = "Asheive's head";
			Weight = 4.0;
		}

		public EvilHead( Serial serial ) : base( serial )
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
