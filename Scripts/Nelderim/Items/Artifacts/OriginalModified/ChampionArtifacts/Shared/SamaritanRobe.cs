using System;
using Server;

namespace Server.Items
{
	public class SamaritanRobe : Robe
	{
        public override int LabelNumber { get { return 1065829; } } // Szata Samarytanina
        public override int InitMinHits { get { return 50; } }
        public override int InitMaxHits { get { return 500; } }

		public override int BasePhysicalResistance{ get{ return 5; } }
		public override bool CanFortify{ get{ return true; } }

		[Constructable]
		public SamaritanRobe()
		{
			Hue = 0x2a3;
		}

		public SamaritanRobe( Serial serial ) : base( serial )
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
