using System;
using Server;

namespace Server.Items
{
	public class BreathOfTheDead : BoneHarvester
	{
        public override int LabelNumber { get { return 1061109; } } // Oddech Smierci
		public override int InitMinHits{ get{ return 60; } }
		public override int InitMaxHits{ get{ return 60; } }

		[Constructable]
		public BreathOfTheDead()
		{
			Hue = 0x455;
			WeaponAttributes.HitLeechHits = 100;
			Attributes.SpellDamage = 10;
			Attributes.WeaponDamage = 50;
            Attributes.WeaponSpeed = 20;
        }

        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy)
        {
            phys = 30;
            fire = 0;
            cold = 40;
            pois = 30;
            nrgy = 0;
        }

		public BreathOfTheDead( Serial serial ) : base( serial )
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