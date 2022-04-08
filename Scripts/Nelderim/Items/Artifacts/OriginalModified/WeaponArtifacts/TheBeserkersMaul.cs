using System;
using Server;

namespace Server.Items
{
	public class TheBeserkersMaul : Maul
	{
        public override int LabelNumber { get { return 1061108; } } // Berserkerski Szal
		public override int InitMinHits{ get{ return 60; } }
		public override int InitMaxHits{ get{ return 60; } }

		[Constructable]
		public TheBeserkersMaul()
		{
			Hue = 0x21;
			Attributes.WeaponSpeed = 75;
			Attributes.WeaponDamage = 50;
		}

        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy)
        {
            phys = 80;
            fire = 0;
            cold = 0;
            pois = 0;
            nrgy = 20;
        }

		public TheBeserkersMaul( Serial serial ) : base( serial )
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