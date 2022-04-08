using System;
using Server;

namespace Server.Items
{
	public class OrnamentOfTheMagician : GoldBracelet
	{
        public override int LabelNumber { get { return 1061105; } } // Duma Maga
        public override int InitMinHits { get { return 45; } }
        public override int InitMaxHits { get { return 45; } }

		[Constructable]
		public OrnamentOfTheMagician()
		{
			Hue = 0x554;
			Attributes.CastRecovery = 3;
			Attributes.CastSpeed = 2;
			Attributes.LowerManaCost = 10;
			Attributes.LowerRegCost = 20;
			Resistances.Energy = 10;
		}

		public OrnamentOfTheMagician( Serial serial ) : base( serial )
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

			if ( Hue == 0x12B )
				Hue = 0x554;
		}
	}
}