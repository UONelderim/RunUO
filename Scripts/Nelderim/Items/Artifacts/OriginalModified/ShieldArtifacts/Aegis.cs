using System;
using Server;

namespace Server.Items
{
	public class Aegis : HeaterShield
	{
		public override int LabelNumber{ get{ return 1061602; } } // Aegis
        public override int InitMinHits { get { return 60; } }
        public override int InitMaxHits { get { return 60; } }

		public override int BasePhysicalResistance{ get{ return 15; } }

		[Constructable]
		public Aegis()
		{
			Hue = 0x47E;
			Attributes.ReflectPhysical = 15;
			Attributes.DefendChance = 15;
			Attributes.LowerManaCost = 8;
		}

		public Aegis( Serial serial ) : base( serial )
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