using System;

namespace Server.Items
{
    public class CrimsonCincture : Obi/*, ITokunoDyable*/
	{

        public override int InitMinHits { get { return 60; } }
        public override int InitMaxHits { get { return 60; } }

		[Constructable]
		public CrimsonCincture() : base()
		{
			Hue = 0x485;
			Name = "Karmazynowy Pas";
			Attributes.BonusDex = 5;
			Attributes.BonusHits = 5;
			Attributes.RegenHits = 2;
            SkillBonuses.SetValues(0, SkillName.Cooking, 10);
		}

		public CrimsonCincture( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			
			int version = reader.ReadInt();
		}
	}
}

