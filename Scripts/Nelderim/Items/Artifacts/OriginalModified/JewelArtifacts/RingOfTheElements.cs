using System;
using Server;

namespace Server.Items
{
	public class RingOfTheElements : GoldRing
	{
        public override int LabelNumber { get { return 1061104; } } // Pierscien Zywiolow
        public override int InitMinHits { get { return 45; } }
        public override int InitMaxHits { get { return 45; } }

		[Constructable]
		public RingOfTheElements()
		{
			Hue = 0x4E9;
			Attributes.Luck = 200;
			Resistances.Fire = 16;
			Resistances.Cold = 16;
			Resistances.Poison = 16;
			Resistances.Energy = 16;
		}

		public RingOfTheElements( Serial serial ) : base( serial )
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