using System;
using Server;

namespace Server.Items
{
	public class InquisitorsResolution : PlateGloves
	{
        public override int LabelNumber { get { return 1060206; } } // Pewnosc Inkwizytora
        public override int InitMinHits { get { return 50; } }
        public override int InitMaxHits { get { return 50; } }

		public override int BaseColdResistance{ get{ return 22; } }
		public override int BaseEnergyResistance{ get{ return 17; } }

		[Constructable]
		public InquisitorsResolution()
		{
			Hue = 0x4F2;
			Attributes.CastRecovery = 3;
			Attributes.LowerManaCost = 8;
			ArmorAttributes.MageArmor = 1;
		}

		public InquisitorsResolution( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version < 1 )
			{
				ColdBonus = 0;
				EnergyBonus = 0;
			}
		}
	}
}