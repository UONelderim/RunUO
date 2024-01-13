using System;
using Server;

namespace Server.Items
{
	public class DarkBow : CompositeBow
	{
		public override int ArtifactRarity{ get{ return 11; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public DarkBow()
		{
			Hue = 1102;
			Name = "Cedrowy Luk Ciemnosci";
			Attributes.WeaponSpeed = -10;
			Attributes.WeaponDamage = 20;
			Attributes.CastRecovery = 2;
			WeaponAttributes.HitFireball = 10;
		}


		public DarkBow( Serial serial ) : base( serial )
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
