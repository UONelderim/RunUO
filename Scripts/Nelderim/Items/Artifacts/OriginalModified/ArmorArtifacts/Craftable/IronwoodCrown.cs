using System;
using Server;

namespace Server.Items
{
	public class IronwoodCrown : RavenHelm
	{
        public override int LabelNumber { get { return 1072924; } } // Korona Z Zelaznego Drzewa
        public override int InitMinHits { get { return 60; } }
        public override int InitMaxHits { get { return 60; } }

		public override int BasePhysicalResistance{ get{ return 10; } }
		public override int BaseFireResistance{ get{ return 6; } }
		public override int BaseColdResistance{ get{ return 7; } }
		public override int BasePoisonResistance{ get{ return 7; } }
		public override int BaseEnergyResistance{ get{ return 10; } }

		[Constructable]
		public IronwoodCrown()
		{
			Hue = 99;
			Name = "Korona Z Żelaznego Drzewa";
			Attributes.BonusStr = 5;
			Attributes.BonusDex = 5;
			Attributes.BonusInt = 5;
		}

		public IronwoodCrown( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}