using System;
using Server;

namespace Server.Items
{
	public class BladeOfInsanity : Katana
	{
        public override int LabelNumber { get { return 1061088; } } // Ostrze Szalenstwa
		public override int InitMinHits{ get{ return 60; } }
		public override int InitMaxHits{ get{ return 60; } }

		[Constructable]
		public BladeOfInsanity()
		{
			Hue = 0x76D;
			WeaponAttributes.HitLeechStam = 100;
			Attributes.RegenStam = 2;
			Attributes.WeaponSpeed = 30;
			Attributes.WeaponDamage = 50;
		}

        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy)
        {
            phys = 50;
            fire = 25;
            cold = 25;
            pois = 0;
            nrgy = 0;
        }

		public BladeOfInsanity( Serial serial ) : base( serial )
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

			if ( Hue == 0x44F )
				Hue = 0x76D;
		}
	}
}