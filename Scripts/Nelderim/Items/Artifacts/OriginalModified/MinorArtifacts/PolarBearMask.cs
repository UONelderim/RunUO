using System;
using Server;

namespace Server.Items
{
	public class PolarBearMask : BearMask
	{
        public override int LabelNumber { get { return 1070637; } } // Maska Niedzwiedzia Polarnego
        public override int InitMinHits { get { return 60; } }
        public override int InitMaxHits { get { return 60; } }

		public override int BasePhysicalResistance{ get{ return 15; } }
		public override int BaseColdResistance{ get{ return 21; } }

		[Constructable]
		public PolarBearMask()
		{
			Hue = 0x481;

			Attributes.RegenHits = 2;
			Attributes.BonusHits = 5;
			Attributes.DefendChance = 5;
			Attributes.NightSight = 1;
		}

		public PolarBearMask( Serial serial ) : base( serial )
		{
		}
		
		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 2 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version < 2 )
			{
				Resistances.Physical = 0;
				Resistances.Cold = 0;
			}

			if ( Attributes.NightSight == 0 )
				Attributes.NightSight = 1;
		}
	}
}