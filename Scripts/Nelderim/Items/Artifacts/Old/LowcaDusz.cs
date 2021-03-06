using System;
using Server;

namespace Server.Items
{
	public class LowcaDusz : Scythe
	{
        public override int LabelNumber { get { return 1065810; } } // Lowca Dusz
        public override int InitMinHits { get { return 60; } }
        public override int InitMaxHits { get { return 60; } }

		[Constructable]
		public LowcaDusz()
		{
			Hue = 0x497;
			Slayer = SlayerName.Exorcism;
			WeaponAttributes.HitLeechHits = 30;
			Attributes.WeaponDamage = 40;
			WeaponAttributes.HitLowerDefend = 30;
			Attributes.AttackChance = 10;
			Attributes.WeaponSpeed = 20;
		}

		public LowcaDusz( Serial serial ) : base( serial )
		{
		}
		public override void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy )
		{
			phys = pois = nrgy = 30;
			fire = 10;
			cold = 0;
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( Slayer == SlayerName.DaemonDismissal )
			{
				Slayer = SlayerName.Exorcism;
			}
		}
	}
}