using System;
using Server;

namespace Server.Items
{
	public class BoneCrusher : WarMace
	{
        public override int LabelNumber { get { return 1061596; } } // Lamacz Kosci
		public override int InitMinHits{ get{ return 60; } }
		public override int InitMaxHits{ get{ return 60; } }

		[Constructable]
		public BoneCrusher()
		{
			ItemID = 0x1406;
			Hue = 0x60C;
			WeaponAttributes.HitLowerDefend = 50;
			Attributes.BonusStr = 10;
			Attributes.BonusStam = 5;
			Attributes.WeaponDamage = 75;
		}
		
        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy)
        {
            phys = 60;
            fire = 10;
            cold = 10;
            pois = 10;
            nrgy = 10;
        }

		public BoneCrusher( Serial serial ) : base( serial )
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

			if ( Hue == 0x604 )
				Hue = 0x60C;

			if ( ItemID == 0x1407 )
				ItemID = 0x1406;
		}
	}
}