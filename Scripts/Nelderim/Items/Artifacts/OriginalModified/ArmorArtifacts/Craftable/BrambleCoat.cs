using System;
using Server.Items;

namespace Server.Items
{
	public class BrambleCoat : WoodlandChest
	{
        public override int LabelNumber { get { return 1072925; } } // Ciernisty Plaszcz
        public override int InitMinHits { get { return 60; } }
        public override int InitMaxHits { get { return 60; } }

		public override int BasePhysicalResistance{ get{ return 10; } }
		public override int BaseFireResistance{ get{ return 8; } }
		public override int BaseColdResistance{ get{ return 7; } }
		public override int BasePoisonResistance{ get{ return 8; } }
		public override int BaseEnergyResistance{ get{ return 7; } }

		[Constructable]
		public BrambleCoat()
		{
			Hue = 2155;
			Name = "Ciernisty Plaszcz";

			Attributes.BonusHits = 4;
			Attributes.Luck = 150;
			Attributes.ReflectPhysical = 25;
			Attributes.DefendChance = 15;
		}

		public BrambleCoat( Serial serial ) : base( serial )
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