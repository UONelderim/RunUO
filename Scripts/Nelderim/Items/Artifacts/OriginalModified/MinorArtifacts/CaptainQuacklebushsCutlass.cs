using System;
using Server;

namespace Server.Items
{
	public class CaptainQuacklebushsCutlass : Cutlass
	{
        public override int LabelNumber { get { return 1063474; } } // Kapitanski Sejmitar
		public override int InitMinHits{ get{ return 60; } }
		public override int InitMaxHits{ get{ return 60; } }

		[Constructable]
		public CaptainQuacklebushsCutlass()
		{
			Hue = 0x66C;
			Attributes.BonusDex = 5;
			Attributes.AttackChance = 10;
			Attributes.WeaponSpeed = 20;
			Attributes.WeaponDamage = 50;
			WeaponAttributes.UseBestSkill = 1;
		}
		
		public override void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy )
		{
			fire = 30;
			phys = 70;
			cold = pois = nrgy = 0;
		}

		public CaptainQuacklebushsCutlass( Serial serial ) : base( serial )
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

			if( Attributes.AttackChance == 50 )
				Attributes.AttackChance = 10;
		}
	}
}