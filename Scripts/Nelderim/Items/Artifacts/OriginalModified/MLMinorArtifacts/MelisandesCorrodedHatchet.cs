using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	public class MelisandesCorrodedHatchet : Hatchet
	{
        public override int LabelNumber { get { return 1072115; } } // Skorodowana Siekiera Melisandy
				public override int InitMinHits{ get{ return 60; } }
		public override int InitMaxHits{ get{ return 60; } }

		[Constructable]
		public MelisandesCorrodedHatchet()
		{
			Hue = 0x494;

			SkillBonuses.SetValues( 0, SkillName.Lumberjacking, 5.0 );

			Attributes.SpellChanneling = 1;
			Attributes.WeaponSpeed = 15;
			Attributes.WeaponDamage = -50;
		}

		public MelisandesCorrodedHatchet( Serial serial ) : base( serial )
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