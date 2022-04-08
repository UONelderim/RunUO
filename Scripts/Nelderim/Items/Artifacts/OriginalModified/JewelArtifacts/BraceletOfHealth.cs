using System;
using Server;

namespace Server.Items
{
	public class BraceletOfHealth : GoldBracelet
	{
        public override int LabelNumber { get { return 1061103; } } // Branzoleta Zdrowia
        public override int InitMinHits { get { return 45; } }
        public override int InitMaxHits { get { return 45; } }		

		[Constructable]
		public BraceletOfHealth()
		{
			Hue = 0x21;
			Attributes.BonusHits = 15;
			Attributes.RegenHits = 10;
		}

		public BraceletOfHealth( Serial serial ) : base( serial )
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