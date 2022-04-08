using System;
using Server;

namespace Server.Items
{
	public class HolyKnightsBreastplate : PlateChest
	{
        public override int LabelNumber { get { return 1061097; } } // Napiersnik Slugi Pana
        public override int InitMinHits { get { return 60; } }
        public override int InitMaxHits { get { return 60; } }

		public override int BasePhysicalResistance{ get{ return 35; } }

		[Constructable]
		public HolyKnightsBreastplate()
		{
			Hue = 0x47E;
			Attributes.BonusHits = 15;
			Attributes.ReflectPhysical = 15;
			ArmorAttributes.LowerStatReq = 50;
		}

		public HolyKnightsBreastplate( Serial serial ) : base( serial )
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
				PhysicalBonus = 0;
		}
	}
}