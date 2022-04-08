using System;

namespace Server.Items
{
	public class PendantOfTheMagi : GoldNecklace
	{
        public override int LabelNumber { get { return 1072937; } } // Wisior Trzech Kroli
				public override int InitMinHits{ get{ return 60; } }
		public override int InitMaxHits{ get { return 60; } }

		[Constructable]
		public PendantOfTheMagi()
		{
			Hue = 0x48D;
			Attributes.BonusInt = 10;
			Attributes.RegenMana = 3;
			Attributes.SpellDamage = 5;
			Attributes.LowerManaCost = 10;
			Attributes.LowerRegCost = 15;
		}

		public PendantOfTheMagi( Serial serial ) : base( serial )
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
