using System;
using Server;

namespace Server.Items
{
	public class AgilityPotion : BaseAgilityPotion
	{
		public override int DexOffset{ get{ return 10; } }
        // 06.07.2012 :: zombie :: czas dzialania pota wynosi 5 min (wczesniej 2)
		public override TimeSpan Duration{ get{ return TimeSpan.FromMinutes( 5.0 ); } }
        // zombie

		[Constructable]
		public AgilityPotion(int amount) : base( PotionEffect.Agility )
		{
            Amount = amount;
			Weight = 0.5;
		}

		[Constructable]
		public AgilityPotion() : this(1)
		{
		}

		public AgilityPotion( Serial serial ) : base( serial )
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