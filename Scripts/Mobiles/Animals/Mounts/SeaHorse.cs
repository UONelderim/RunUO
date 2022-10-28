using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "zwloki konia morskiego" )]
	public class SeaHorse : BaseMount
	{
		[Constructable]
		public SeaHorse() : this( "kon morski" )
		{
		}

		[Constructable]
		public SeaHorse( string name ) : base( name, 0x90, 0x3EB3, AIType.AI_Animal, FightMode.Aggressor, 12, 1, 0.2, 0.4 )
		{
			InitStats( Utility.Random( 50, 30 ), Utility.Random( 50, 30 ), 10 );
			Skills[SkillName.MagicResist].Base = 25.0 + (Utility.RandomDouble() * 5.0);
			Skills[SkillName.Wrestling].Base = 35.0 + (Utility.RandomDouble() * 10.0);
			Skills[SkillName.Tactics].Base = 30.0 + (Utility.RandomDouble() * 15.0);
			
			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = 29.1;
		}
		
		public override int Meat{ get{ return 3; } }
		public override int Hides{ get{ return 4; } }
		public override FoodType FavoriteFood{ get{ return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }

		public SeaHorse( Serial serial ) : base( serial )
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