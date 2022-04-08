using System;
using Server;

namespace Server.Items
{
	public class AxeOfTheHeavens : DoubleAxe
	{
        public override int LabelNumber { get { return 1061106; } } // Niebianski Topor
		public override int InitMinHits{ get{ return 60; } }
		public override int InitMaxHits{ get{ return 60; } }

		[Constructable]
		public AxeOfTheHeavens()
		{
			Hue = 0x4D5;
			WeaponAttributes.HitLightning = 50;
			Attributes.AttackChance = 15;
			Attributes.DefendChance = 15;
			Attributes.WeaponDamage = 50;
		}
		
		public override void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy )
		{
			phys = 20;
			pois = 40;
			nrgy = 40;

			cold = fire = 0;
		}

		public AxeOfTheHeavens( Serial serial ) : base( serial )
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