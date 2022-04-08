using System;

namespace Server.Items
{
	public class EssenceOfBattle : GoldRing
	{
        public override int LabelNumber { get { return 1072935; } } // Esensja Bitwy
				public override int InitMinHits{ get{ return 45; } }
		public override int InitMaxHits{ get { return 45; } }

		[Constructable]
		public EssenceOfBattle()
		{
			Hue = 0x550;
			Attributes.BonusDex = 7;
			Attributes.BonusStr = 7;
			Attributes.WeaponSpeed = 10;
			Attributes.WeaponDamage = 30;
		}

		public EssenceOfBattle( Serial serial ) : base( serial )
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
