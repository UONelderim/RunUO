using System;
using Server;

namespace Server.Items
{
	public class HebanowyPlomien : BlackStaff
	{
        public override int LabelNumber { get { return 1065807; } } // Hebanowy Plomien
        public override int InitMinHits { get { return 60; } }
        public override int InitMaxHits { get { return 60; } }

		[Constructable]
		public HebanowyPlomien()
		{
			Hue = 0x4EA;
			Attributes.SpellChanneling = 1;
			Attributes.CastSpeed = 1;
			Attributes.CastRecovery = 1;
			Attributes.LowerManaCost = 6;
			Attributes.LowerRegCost = 10;
			Attributes.WeaponDamage = 30;
		}

		public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy)
        {
            phys = 60;
            fire = 40;
            cold = 0;
            pois = 0;
            nrgy = 0;
        }
		
		public HebanowyPlomien( Serial serial ) : base( serial )
		{
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
		}
	}
}

