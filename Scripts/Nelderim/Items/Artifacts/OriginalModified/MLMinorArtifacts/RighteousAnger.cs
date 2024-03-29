using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	public class RighteousAnger : ElvenMachete
	{
        public override int LabelNumber { get { return 1075049; } } // Sprawiedliwy Gniew
		public override int InitMinHits{ get{ return 60; } }
		public override int InitMaxHits{ get{ return 60; } }

		[Constructable]
		public RighteousAnger()
		{
			Name = "Sprawiedliwy Gniew";
			Hue = 0x284;

			Attributes.AttackChance = 15;
			Attributes.DefendChance = 5;
			Attributes.WeaponSpeed = 35;
			Attributes.WeaponDamage = 40;
		}

		public RighteousAnger( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.WriteEncodedInt( 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadEncodedInt();
		}
	}
}