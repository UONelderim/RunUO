using System;
using Server;

namespace Server.Items
{
	public class LuckyDagger : Item
	{	
		[Constructable]
		public LuckyDagger() : base( 0xF52 )
		{
			Weight = 1;
			Hue = 0x8A5;
			Name = "szczęśliwy sztylet";
		}

		public LuckyDagger( Serial serial ) : base( serial )
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

