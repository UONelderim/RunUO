using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	public class FleshRipper : AssassinSpike
	{
        public override int LabelNumber { get { return 1075045; } } // Wypruwacz Flakow
		public override int InitMinHits{ get{ return 60; } }
		public override int InitMaxHits{ get{ return 60; } }

		[Constructable]
		public FleshRipper()
		{
			Hue = 0x341;

			SkillBonuses.SetValues( 0, SkillName.Anatomy, 10.0 );

			Attributes.BonusStr = 5;
			Attributes.AttackChance = 15;
			Attributes.WeaponSpeed = 40;

			WeaponAttributes.UseBestSkill = 1;
		}

		public FleshRipper( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}