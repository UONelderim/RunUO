using System;
using Server;

namespace Server.Items
{
	public class ShaminoCrossbow : RepeatingCrossbow
	{


		public override int InitMinHits{ get{ return 60; } }
		public override int InitMaxHits{ get{ return 60; } }

		[Constructable]
		public ShaminoCrossbow()
		{
			Hue = 0x504;
			Name = "Kusza B³yskawiczna";
			LootType = LootType.Regular;

			Attributes.AttackChance = 15;
			Attributes.WeaponDamage = 40;
			WeaponAttributes.HitLightning = 30;
			WeaponAttributes.LowerStatReq = 100;
		}

		public ShaminoCrossbow( Serial serial ) : base( serial )
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