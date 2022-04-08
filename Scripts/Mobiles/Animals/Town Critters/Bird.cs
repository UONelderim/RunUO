using System;
using Server;
using Server.Misc;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "zwloki ptaka" )]
	public class Bird : BaseCreature
	{
		[Constructable]
		public Bird() : base( AIType.AI_Animal, FightMode.Aggressor, 12, 1, 0.2, 0.4 )
		{
			if ( Utility.RandomBool() )
			{
				Hue = 0x901;

				switch ( Utility.Random( 3 ) )
				{
					case 0: Name = "wrona"; break;
					case 2: Name = "kruk"; break;
					case 1: Name = "sroka"; break;
				}
			}
			else
			{
				Hue = Utility.RandomBirdHue();
				Name = NameList.RandomName( "bird" );
			}

			Body = 6;
			BaseSoundID = 0x1B;

			VirtualArmor = Utility.RandomMinMax( 0, 6 );

			SetStr( 10 );
			SetDex( 25, 35 );
			SetInt( 10 );

			SetDamage( 0 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetSkill( SkillName.Wrestling, 4.2, 6.4 );
			SetSkill( SkillName.Tactics, 4.0, 6.0 );
			SetSkill( SkillName.MagicResist, 4.0, 5.0 );

			Fame = 150;
			Karma = 0;

			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = -6.9;

            // 07.01.2013 :: szczaw :: dodane - jaja do lootu
            Bird.AddEggs(this);
		}

		public override MeatType MeatType{ get{ return MeatType.Bird; } }
		public override int Meat{ get{ return 1; } }
		public override int Feathers{ get{ return 8; } }
		public override FoodType FavoriteFood{ get{ return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }

        // 07.01.2013 :: szczaw :: dodane - jaja do lootu, metoda wspólna ¿eby trzymaæ wszystko w jednym miejscu.
        public static void AddEggs( BaseCreature bird )
        {
            const int chancePercent = 10;
            const int amountMin = 1;
            const int amountMax = 1;

            AddEggs(bird, chancePercent, amountMin, amountMax);
        }

        public static void AddEggs( BaseCreature bird, int chancePercent, int amountMin, int amountMax )
        {
            int amount = Utility.RandomMinMax(amountMin, amountMax);

            if(bird != null)
                if(chancePercent > Utility.Random(100))
                    bird.AddItem(new Eggs(amount));
        }

		public Bird( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( Hue == 0 )
				Hue = Utility.RandomBirdHue();
		} 
	}

	[CorpseName( "a bird corpse" )]
	public class TropicalBird : BaseCreature
	{
		[Constructable]
		public TropicalBird() : base( AIType.AI_Animal, FightMode.Aggressor, 12, 1, 0.2, 0.4 )
		{
			Hue = Utility.RandomBirdHue();
			Name = "tropikalny ptak";

			Body = 6;
			BaseSoundID = 0xBF;

			VirtualArmor = Utility.RandomMinMax( 0, 6 );

			SetStr( 10 );
			SetDex( 25, 35 );
			SetInt( 10 );

			SetDamage( 0 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetSkill( SkillName.Wrestling, 4.2, 6.4 );
			SetSkill( SkillName.Tactics, 4.0, 6.0 );
			SetSkill( SkillName.MagicResist, 4.0, 5.0 );

			Fame = 150;
			Karma = 0;

			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = -6.9;

            Bird.AddEggs(this);
		}

		public override MeatType MeatType{ get{ return MeatType.Bird; } }
		public override int Meat{ get{ return 1; } }
		public override int Feathers{ get{ return 8; } }
		public override FoodType FavoriteFood{ get{ return FoodType.FruitsAndVegies | FoodType.GrainsAndHay; } }

		public TropicalBird( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		} 
	}
}