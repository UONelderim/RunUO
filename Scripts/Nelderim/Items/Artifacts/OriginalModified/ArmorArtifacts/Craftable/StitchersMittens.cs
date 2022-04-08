using System;
using Server.Items;

namespace Server.Items
{
	public class StitchersMittens : LeafGloves
	{
        public override int LabelNumber { get { return 1072932; } } // Rekawice Szwacza
        public override int InitMinHits { get { return 60; } }
        public override int InitMaxHits { get { return 60; } }

		public override int BasePhysicalResistance{ get{ return 20; } }
		public override int BaseColdResistance{ get{ return 20; } }

		[Constructable]
		public StitchersMittens()
		{
			Hue = 0x481;

			SkillBonuses.SetValues( 0, SkillName.Healing, 10.0 );
			Name = "Rękawice Szwacza";
			Attributes.BonusDex = 5;
			Attributes.LowerRegCost = 20;
		}

		public StitchersMittens( Serial serial ) : base( serial )
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