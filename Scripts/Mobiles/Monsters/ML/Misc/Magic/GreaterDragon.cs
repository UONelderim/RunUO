using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "zwloki wielkiego smoka" )]
	public class GreaterDragon : BaseCreature
	{
		public override bool StatLossAfterTame { get { return true; } }

		[Constructable]
		public GreaterDragon () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.3, 0.5 )
		{
			Name = "wielki smok";
			Body = Utility.RandomList( 12, 59 );
			BaseSoundID = 362;

			SetStr( 850, 1100 );
			SetDex( 81, 150 );
			SetInt( 475, 675 );

			SetHits( 1200, 1500 );
			SetStam( 120, 150 );

			SetDamage( 24, 33 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 60, 85 );
			SetResistance( ResistanceType.Fire, 65, 90 );
			SetResistance( ResistanceType.Cold, 40, 55 );
			SetResistance( ResistanceType.Poison, 40, 60 );
			SetResistance( ResistanceType.Energy, 50, 75 );

			SetSkill( SkillName.Meditation, 0 );
			SetSkill( SkillName.EvalInt, 110.0, 140.0 );
			SetSkill( SkillName.Magery, 110.0, 140.0 );
			SetSkill( SkillName.Poisoning, 0 );
			SetSkill( SkillName.Anatomy, 0 );
			SetSkill( SkillName.MagicResist, 110.0, 140.0 );
			SetSkill( SkillName.Tactics, 110.0, 140.0 );
			SetSkill( SkillName.Wrestling, 115.0, 145.0 );

			Fame = 22000;
			Karma = -15000;

			VirtualArmor = 60;

			Tamable = true;
			ControlSlots = 5;
			MinTameSkill = 104.7;
		}
		
		public override void OnCarve(Mobile from, Corpse corpse, Item with)
		{
            if (!IsBonded && !corpse.Carved && !IsChampionSpawn)
            {
			    if( Utility.RandomDouble() < 0.08 )
				    corpse.DropItem( new DragonsHeart() );
			    if( Utility.RandomDouble() < 0.20 )
				    corpse.DropItem( new DragonsBlood() );
            }

			base.OnCarve(from, corpse, with);
		}
		
		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 3 );
			AddLoot( LootPack.Gems, 3 );
		}

		public override bool ReacquireOnMovement{ get{ return !Controlled; } }
		public override bool HasBreath{ get{ return true; } } // fire breath enabled
		public override bool AutoDispel{ get{ return !Controlled; } }
		public override int TreasureMapLevel{ get{ return 5; } }
		public override int Meat{ get{ return 8; } }
		public override int Hides{ get{ return 15; } }
		public override HideType HideType{ get{ return HideType.Barbed; } }
		public override int Scales{ get{ return 5; } }
		public override ScaleType ScaleType{ get{ return ( Body == 12 ? ScaleType.Yellow : ScaleType.Red ); } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override bool CanAngerOnTame { get { return true; } }

        public override void AddWeaponAbilities()
        {
            WeaponAbilities.Add( WeaponAbility.BleedAttack, 0.4 );
        }

		public GreaterDragon( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 1 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			SetDamage( 24, 33 );

			if( version == 0 )
			{
				Server.SkillHandlers.AnimalTaming.ScaleStats( this, 0.50 );
				Server.SkillHandlers.AnimalTaming.ScaleSkills( this, 0.85 ); // 90% * 80% = 72% of original skills trainable to 90%
				Skills[SkillName.Magery].Base = Skills[SkillName.Magery].Cap; // Greater dragons have a 90% cap reduction and 90% skill reduction on magery
			}
		}
	}
}
