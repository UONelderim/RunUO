using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "zwloki goblina" )]
	public class Goblin : BaseCreature
	{
		[Constructable]
		public Goblin() : base( AIType.AI_Melee, FightMode.Closest, 12, 1, 0.2, 0.4 ) 
		{
			Name = "goblin";
			Body = 182; 
			Hue = 2212;
			BaseSoundID = 1114; 

		    	SetStr( 60 );
			SetDex( 20 );
			SetInt( 50 );

			SetHits( 70, 90 );

			SetDamage( 6, 8 );

			SetDamageType( ResistanceType.Physical, 100 );
			SetDamageType( ResistanceType.Cold, 0 );
			SetDamageType( ResistanceType.Fire, 0 );
			SetDamageType( ResistanceType.Energy, 0 );
			SetDamageType( ResistanceType.Poison, 0 );

			SetResistance( ResistanceType.Physical, 35 );
			SetResistance( ResistanceType.Fire, 36 );
			SetResistance( ResistanceType.Cold, 29 );
			SetResistance( ResistanceType.Poison, 35 );
			SetResistance( ResistanceType.Energy, 35 );

			//SetSkill( SkillName.EvalInt, nn.n, nn.n );
			//SetSkill( SkillName.Magery, nn.n, nn.n );
			//SetSkill( SkillName.MagicResist, nn.n, nn.n );
			SetSkill( SkillName.Tactics, 21.9, 37.8 );
			SetSkill( SkillName.Wrestling, 87.4, 94.2 );
			SetSkill( SkillName.Anatomy, 52.1, 57.8 );
			//SetSkill( SkillName.Poisoning, nn.n, nn.n );
			//SetSkill( SkillName.Meditation, nn.n, nn.n );
			

			Fame = 2500;
			Karma = -2500;

			VirtualArmor = 2;

			//Tamable = true; 
         		//ControlSlots = nn; 
         		//MinTameSkill = nn.n;

			//PackGem();
			// 07.01.2013 :: szczaw :: usuniecie PackGold
			//PackGold( 34, 83  );
			//PackItem( new GargoylesPickaxe () );
			//PackMagicItems( 1 );
			PackSlayer();
			//PackScroll( nn, nn );
			//PackPotion();

			//PackNecroReg( Utility.Random( nn, nn ) );
							if ( Utility.RandomDouble() < 0.3 )
				PackItem( new BowstringCannabis() );
			
		}

		//public override FoodType FavoriteFood{ get{ return FoodType.nn; } }
		//public override PackInstinct PackInstinct{ get{ return PackInstinct.nn; } }
		public override bool CanRummageCorpses{ get{ return true; } }
		//public override bool HasBreath{ get{ return true; } }
		//public override bool AutoDispel{ get{ return true; } }
		//public override bool BardImmune{ get{ return true; } }
		//public override bool Unprovokable{ get{ return true; } }
		//public override bool Uncalmable{ get{ return true; } }
		//public override Poison PoisonImmune{ get{ return Poison.nn; } }
		//public override Poison HitPoison{ get{ return Poison.nn; } }
		//public override bool AlwaysMurderer{ get{ return true; } }
		//public override int TreasureMapLevel{ get{ return nn; } }
		//public override int Meat{ get{ return 3, 5; } }
		//public override int Hides{ get{ return nn; } } 
      		//public override HideType HideType{ get{ return HideType.nn; } }
		//public override int Scales{ get{ return nn; } }
		//public override ScaleType ScaleType{ get{ return ScaleType.nn; } }

		//public override OppositionGroup OppositionGroup
		//{
		//	get{ return OppositionGroup.nn; }
		//}

		public Goblin( Serial serial ) : base( serial )
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
