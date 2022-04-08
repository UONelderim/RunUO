using System;
using Server;

namespace Server.Items
{
	public class LesserCurePotion : BaseCurePotion
	{
		private static CureLevelInfo[] m_OldLevelInfo = new CureLevelInfo[]
			{
				new CureLevelInfo( Poison.Lesser,  0.75 ), // 75% chance to cure lesser poison
				new CureLevelInfo( Poison.Regular, 0.50 ), // 50% chance to cure regular poison
				new CureLevelInfo( Poison.Greater, 0.15 )  // 15% chance to cure greater poison
			};

		private static CureLevelInfo[] m_AosLevelInfo = new CureLevelInfo[]
			{
				new CureLevelInfo( Poison.Lesser,  0.75 ),
				new CureLevelInfo( Poison.Regular, 0.50 ),
				new CureLevelInfo( Poison.Greater, 0.25 )
			};

		public override CureLevelInfo[] LevelInfo{ get{ return Core.AOS ? m_AosLevelInfo : m_OldLevelInfo; } }

		[Constructable]
		public LesserCurePotion(int amount) : base( PotionEffect.CureLesser )
		{
            Amount = amount;
			Weight = 0.5;
		}

		[Constructable]
		public LesserCurePotion() : this(1)
		{
		}

		public LesserCurePotion( Serial serial ) : base( serial )
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