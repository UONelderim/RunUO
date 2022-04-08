using System;
using Server;

namespace Server.Items
{
	public class StaffOfTheMagi : BlackStaff
	{
        public override int LabelNumber { get { return 1061600; } } // Laska Zywiolow Magii
		public override int InitMinHits{ get{ return 60; } }
		public override int InitMaxHits{ get{ return 60; } }

		[Constructable]
		public StaffOfTheMagi()
		{
			Hue = 0x481;
			WeaponAttributes.MageWeapon = 30;
			Attributes.SpellChanneling = 1;
			Attributes.CastSpeed = 1;
		}

		public override void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy )
		{
            phys = 20;
            fire = 20;
            cold = 20;
            pois = 20;
            nrgy = 20;
		}

		public StaffOfTheMagi( Serial serial ) : base( serial )
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

			if ( WeaponAttributes.MageWeapon == 0 )
				WeaponAttributes.MageWeapon = 30;

			if ( ItemID == 0xDF1 )
				ItemID = 0xDF0;
		}
	}
}