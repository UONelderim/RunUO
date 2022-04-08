using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	public class Bonesmasher : DiamondMace
	{
        public override int LabelNumber { get { return 1075030; } } // Miazdzyciel Kosci
		public override int InitMinHits{ get{ return 60; } }
		public override int InitMaxHits{ get{ return 60; } }

		[Constructable]
		public Bonesmasher()
		{
			ItemID = 0x2D30;
			Hue = 0x482;

			SkillBonuses.SetValues( 0, SkillName.Macing, 10.0 );

			WeaponAttributes.HitLeechMana = 40;
			Attributes.WeaponDamage = 30;
		}

		public Bonesmasher( Serial serial ) : base( serial )
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

			if (Attributes.WeaponDamage != 30)
			Attributes.WeaponDamage = 30;
		}
	}
}