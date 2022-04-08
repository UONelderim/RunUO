using System;
using Server;

namespace Server.Items
{
	public class OrcishVisage : OrcHelm
	{
        public override int LabelNumber { get { return 1070691; } } // Orkowa Wizura
        public override int InitMinHits { get { return 60; } }
        public override int InitMaxHits { get { return 60; } }

		public override int BasePhysicalResistance{ get{ return 18; } }
		public override int BaseFireResistance{ get{ return 5; } }
		public override int BaseColdResistance{ get{ return 6; } }
		public override int BasePoisonResistance{ get{ return 5; } }
		public override int BaseEnergyResistance{ get{ return 6; } }

		[Constructable]
		public OrcishVisage()
		{
			Hue = 0x592;
			Attributes.BonusStr = 10;
			Attributes.BonusStam = 12;
			Attributes.RegenHits = 2;
		}

		public OrcishVisage( Serial serial ) : base( serial )
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