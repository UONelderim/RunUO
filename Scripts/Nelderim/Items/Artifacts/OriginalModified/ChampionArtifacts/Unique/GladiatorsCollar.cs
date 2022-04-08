using System;
using Server;

namespace Server.Items
{
	public class GladiatorsCollar : PlateGorget
	{
        public override int LabelNumber { get { return 1065845; } } // Kolnierz Gladiatora
        public override int InitMinHits { get { return 50; } }
        public override int InitMaxHits { get { return 50; } }

		public override int BasePhysicalResistance{ get{ return 18; } }
		public override int BaseFireResistance{ get{ return 18; } }
		public override int BaseColdResistance{ get{ return 17; } }
		public override int BasePoisonResistance{ get{ return 18; } }
		public override int BaseEnergyResistance{ get{ return 16; } }

		public override bool CanFortify{ get{ return true; } }

		[Constructable]
		public GladiatorsCollar()
		{
			Hue = 0x26d;

			Attributes.BonusHits = 10;
			Attributes.AttackChance = 10;

			ArmorAttributes.MageArmor = 1;
		}

		public GladiatorsCollar( Serial serial ) : base( serial )
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
