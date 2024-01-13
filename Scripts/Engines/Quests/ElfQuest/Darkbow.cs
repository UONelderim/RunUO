using System;
using Server;

namespace Server.Items
{
	public class LightBow : CompositeBow
	{
		public override int ArtifactRarity{ get{ return 11; } }

		public override int InitMinHits{ get{ return 255; } }
		public override int InitMaxHits{ get{ return 255; } }

		[Constructable]
		public LightBow()
		{
			Hue = 1154;
			Name = "Cedrowy Luk Swiatla";
			Attributes.WeaponSpeed = -10;
			Attributes.WeaponDamage = 20;
			Attributes.CastRecovery = 2;
			WeaponAttributes.HitFireball = 10;
		}


		public LightBow( Serial serial ) : base( serial )
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
