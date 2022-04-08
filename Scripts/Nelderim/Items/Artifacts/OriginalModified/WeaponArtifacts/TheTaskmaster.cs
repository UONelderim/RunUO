using System;
using Server;

namespace Server.Items
{
	public class TheTaskmaster : WarFork
	{
        public override int LabelNumber { get { return 1061110; } } // Ciemiezca
		public override int InitMinHits{ get{ return 60; } }
		public override int InitMaxHits{ get{ return 60; } }

		[Constructable]
		public TheTaskmaster()
		{
			Hue = 0x4F8;
			WeaponAttributes.HitPoisonArea = 100;
			Attributes.BonusDex = 5;
            Attributes.BonusStam = 8;
			Attributes.AttackChance = 15;
			Attributes.WeaponDamage = 50;
		}

		public override void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy )
		{
			phys = fire = cold = nrgy = 0;
			pois = 100;
		}

		public TheTaskmaster( Serial serial ) : base( serial )
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