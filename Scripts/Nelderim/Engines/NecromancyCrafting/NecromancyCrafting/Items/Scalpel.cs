using System;
using Server;
using Server.Engines.Craft;

namespace Server.Items
{
	[FlipableAttribute( 0x13E4, 0x13E3 )]
	public class Scalpel : BaseTool
	{
		public override CraftSystem CraftSystem{ get{ return DefNecromancyCrafting.CraftSystem; } }

		[Constructable]
		public Scalpel() : base( 0x10E7 )
		{
			Name = "Skalpel";
			Hue = 1150;
			Weight = 2.0;
		}

		[Constructable]
		public Scalpel( int uses ) : base( uses, 0x10E7 )
		{
			Name = "Skalpel";
			Hue = 1150;
			Weight = 2.0;
			Layer = Layer.OneHanded;
		}

		public Scalpel( Serial serial ) : base( serial )
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