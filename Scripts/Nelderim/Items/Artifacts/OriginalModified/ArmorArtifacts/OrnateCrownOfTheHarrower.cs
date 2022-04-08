using System;
using Server;

namespace Server.Items
{
	public class OrnateCrownOfTheHarrower : BoneHelm
	{

        public override int InitMinHits { get { return 60; } }
        public override int InitMaxHits { get { return 60; } }

		public override int BasePoisonResistance{ get{ return 17; } }

		[Constructable]
		public OrnateCrownOfTheHarrower()
		{
			Hue = 0x4F6;
			Name = "Korona Przedwiecznego";
			Attributes.RegenHits = 2;
			Attributes.RegenStam = 3;
			Attributes.WeaponDamage = 25;
		}

		public OrnateCrownOfTheHarrower( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( version < 1 )
			{
				if ( Hue == 0x55A )
					Hue = 0x4F6;

				PoisonBonus = 0;
			}
		}
	}
}