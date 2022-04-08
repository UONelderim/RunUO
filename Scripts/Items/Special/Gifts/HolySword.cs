using System;
using Server;

namespace Server.Items
{
	public class HolySword : Longsword
	{


		public override int InitMinHits{ get{ return 60; } }
		public override int InitMaxHits{ get{ return 60; } }

		[Constructable]
		public HolySword()
		{
			Hue = 0x482;
			Name = "Święty Miecz";
			LootType = LootType.Regular;

			Slayer = SlayerName.Silver;

			Attributes.WeaponDamage = 40;

			WeaponAttributes.LowerStatReq = 100;
			WeaponAttributes.UseBestSkill = 1;
		}

		public HolySword( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( (int) 0 ); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}