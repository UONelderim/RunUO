using System;
using Server;

namespace Server.Items
{
	public class AcidProofRobe : Robe
	{

        public override int InitMinHits { get { return 60; } }
        public override int InitMaxHits { get { return 60; } }

		public override int BasePoisonResistance{ get{ return 4; } }


		[Constructable]
		public AcidProofRobe()
		{
			Hue = 0x455;
			Name = "Szata Kwasu";
			LootType = LootType.Regular;
		}

		public AcidProofRobe( Serial serial ) : base( serial )
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

			if (version < 1 && Hue == 1)
			{
				Hue = 0x455;
			}
		}
	}
}
