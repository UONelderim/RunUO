using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "zwloki bialego wyrma" )]
	public class WhiteWyrm : BaseCreature
	{
		[Constructable]
		public WhiteWyrm () : base( AIType.AI_BattleMage, FightMode.Closest, 12, 1, 0.2, 0.4 )
		{
			Body = Utility.RandomBool() ? 180 : 49;
			Name = "sniezny wyrm";
			BaseSoundID = 362;

			SetStr( 721, 760 );
			SetDex( 101, 130 );
			SetInt( 386, 425 );

			SetHits( 433, 456 );

			SetDamage( 17, 25 );

			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Cold, 50 );

			SetResistance( ResistanceType.Physical, 55, 70 );
			SetResistance( ResistanceType.Fire, 15, 25 );
			SetResistance( ResistanceType.Cold, 80, 90 );
			SetResistance( ResistanceType.Poison, 40, 50 );
			SetResistance( ResistanceType.Energy, 40, 50 );

			SetSkill( SkillName.EvalInt, 90.0, 110.0 );
			SetSkill( SkillName.Magery, 90.0, 120.0 );
			SetSkill( SkillName.MagicResist, 99.1, 110.0 );
			SetSkill( SkillName.Tactics, 90.1, 100.0 );
			SetSkill( SkillName.Wrestling, 75.1, 100.0 );
			SetSkill( SkillName.Meditation, 70.0, 100.0 );
			SetSkill( SkillName.Anatomy, 70.0, 100.0 );

			Fame = 18000;
			Karma = -18000;

			VirtualArmor = 64;

			Tamable = true;
			ControlSlots = 3;
			MinTameSkill = 96.3;
		}

		public override void OnCarve(Mobile from, Corpse corpse, Item with)
		{
            if( !IsBonded && !corpse.Carved )
            {
			    if ( Utility.RandomDouble() < 0.25 )
				    corpse.DropItem( new WyrmsHeart() );
			}

            base.OnCarve(from, corpse, with);
		}
		
		public override void AddWeaponAbilities()
		{
			WeaponAbilities.Add( WeaponAbility.WhirlwindAttack, 0.3 );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 2 );
			AddLoot( LootPack.Average );
			AddLoot( LootPack.Gems, Utility.Random( 2, 3 ) );
		}

		public override bool ReacquireOnMovement{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 4; } }
		public override int Meat{ get{ return 8; } }
		public override int Hides{ get{ return 12; } }
		public override HideType HideType{ get{ return HideType.Barbed; } }
		public override int Scales{ get{ return 5; } }
		public override ScaleType ScaleType{ get{ return ScaleType.White; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat | FoodType.Gold; } }
		public override bool CanAngerOnTame { get { return true; } }

		public WhiteWyrm( Serial serial ) : base( serial )
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

			if ( Core.AOS && Body == 49 )
				Body = 180;
		}
	}
}