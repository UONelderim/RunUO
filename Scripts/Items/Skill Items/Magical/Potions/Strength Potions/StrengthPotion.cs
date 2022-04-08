using System;
using Server;

namespace Server.Items
{
	public class StrengthPotion : BaseStrengthPotion
	{
		public override int StrOffset{ get{ return 10; } }
        // 06.07.2012 :: zombie :: czas dzialania pota wynosi 5 min (wczesniej 2)
        public override TimeSpan Duration { get { return TimeSpan.FromMinutes( 5.0 ); } }
        // zombie

		[Constructable]
		public StrengthPotion(int amount) : base( PotionEffect.Strength )
		{
            Amount = amount;
			Weight = 0.5;
		}

		[Constructable]
		public StrengthPotion() : this(1)
		{
		}

		public StrengthPotion( Serial serial ) : base( serial )
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