using System;
using Server.Items;

namespace Server.Items
{
	public class SongWovenMantle : LeafArms
	{

        public override int InitMinHits { get { return 60; } }
        public override int InitMaxHits { get { return 60; } }

		public override int BasePhysicalResistance{ get{ return 14; } }
		public override int BaseColdResistance{ get{ return 14; } }
		public override int BaseEnergyResistance{ get{ return 16; } }

		[Constructable]
		public SongWovenMantle()
		{
			Hue = 0x493;
			Name = "Naramienniki Barda";
			SkillBonuses.SetValues( 0, SkillName.Musicianship, 10.0 );
			Attributes.Luck = 100;
			Attributes.DefendChance = 5;
		}

		public SongWovenMantle( Serial serial ) : base( serial )
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