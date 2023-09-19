using System;
using Server;

namespace Server.Items
{
	public class Beastmaster : GoldRing
	{
		
		public override int InitMinHits { get { return 60; } }
        public override int InitMaxHits { get { return 60; } }


		[Constructable]
		public Beastmaster()
		{
			Name = "Pierscien Wladcy Bestii";
			Hue = 1153;
			Attributes.BonusDex = 10;
			Attributes.BonusStr = -20;
            Attributes.BonusInt = 10;
            SkillBonuses.SetValues( 0, SkillName.AnimalTaming, 5.0 );
			SkillBonuses.SetValues( 1, SkillName.AnimalLore, 5.0 );
			Resistances.Poison = -20;
			Resistances.Fire = -20;

	}

		public Beastmaster ( Serial serial ) : base( serial )
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
