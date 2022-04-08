using System;
using Server;

namespace Server.Items
{
	public class LesserExplosionPotion : BaseExplosionPotion
	{
		public override int MinDamage { get { return 5; } }
		public override int MaxDamage { get { return 10; } }

		[Constructable]
		public LesserExplosionPotion(int amount) : base( PotionEffect.ExplosionLesser )
		{
            Amount = amount;
			Weight = 0.5;
		}

		[Constructable]
		public LesserExplosionPotion() : this(1)
		{
		}

		public LesserExplosionPotion( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}