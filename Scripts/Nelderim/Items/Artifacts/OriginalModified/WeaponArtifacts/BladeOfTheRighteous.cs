using System;
using Server;

namespace Server.Items
{
	public class BladeOfTheRighteous : Longsword
	{
        public override int LabelNumber { get { return 1061107; } } // Ostrze Sprawiedliwosci
		public override int InitMinHits{ get{ return 60; } }
		public override int InitMaxHits{ get{ return 60; } }

		[Constructable]
		public BladeOfTheRighteous()
		{
			Hue = 0x47E;
			Slayer = SlayerName.Exorcism;
			WeaponAttributes.HitLeechHits = 50;
			WeaponAttributes.UseBestSkill = 1;
			Attributes.BonusHits = 10;
			Attributes.WeaponDamage = 50;
		}

        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy)
        {
            phys = 70;
            fire = 30;
            cold = 0;
            pois = 0;
            nrgy = 0;
        }

		public BladeOfTheRighteous( Serial serial ) : base( serial )
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

			if ( Slayer == SlayerName.None )
				Slayer = SlayerName.Exorcism;
		}
	}
}