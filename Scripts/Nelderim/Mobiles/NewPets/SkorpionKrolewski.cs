using Server;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using System.Collections;

namespace Server.Mobiles
{
[CorpseName( "zwloki Skorpiona krolewskiego" )]
	public class SkorpionKrolewski : BaseCreature
	{

		[Constructable]
		public SkorpionKrolewski() :  base( AIType.AI_Melee, FightMode.Weakest, 12, 1, 0.2, 0.4 )
		{
			BaseSoundID = 96;
            Hue = 2687;
			Body = 48;
			Name = "Skorpion krolewski";

			SetStr( 296, 325 );
			SetDex( 206, 295 );
			SetInt( 236, 275 );

			SetDamage( 12, 23 );

			SetHits( 298, 355 );

			SetDamageType( ResistanceType.Cold, 50 );
			SetDamageType( ResistanceType.Poison, 50 );
			SetDamageType( ResistanceType.Physical, 0 );

			SetResistance( ResistanceType.Physical, 55, 75 );
			SetResistance( ResistanceType.Fire, 60, 70 );
			SetResistance( ResistanceType.Cold, 55, 65 );
			SetResistance( ResistanceType.Poison, 65, 75 );
			SetResistance( ResistanceType.Energy, 30, 45 );

			SetSkill( SkillName.Bushido, 50.1, 99.0 );
			SetSkill( SkillName.Meditation, 50.1, 110.0 );
			SetSkill( SkillName.Parry, 59.1, 100.0 );
			SetSkill( SkillName.Tactics, 97.6, 100.0 );
			SetSkill( SkillName.Wrestling, 90.1, 92.5 );
			SetSkill( SkillName.DetectHidden, 90.1, 120.5 );

			Fame = 15000;
			Karma = -15000;

			VirtualArmor = 60;

			Tamable = true;
			ControlSlots = 3;
			MinTameSkill = 104;
			
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 1 );
			AddLoot( LootPack.Gems, 2 );
		}

		public override int GetAngerSound()
		{
			if ( !Controlled )
				return 98;

			return base.GetAngerSound();
		}

		public override int TreasureMapLevel{ get{ return 3; } }
		public override int Meat{ get{ return 5; } }
		public override int Hides{ get{ return 17; } }
		public override HideType HideType{ get{ return HideType.Spined; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override bool BardImmune{ get{ return false; } } 	
		public override Poison HitPoison{ get{ return Poison.Deadly; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Arachnid; } }
	
		
        public override void AddWeaponAbilities()
        {
            WeaponAbilities.Add( WeaponAbility.Feint, 0.11 );
            WeaponAbilities.Add( WeaponAbility.DoubleStrike, 0.11 );
            WeaponAbilities.Add( WeaponAbility.DefenseMastery, 0.11 );
        }
		
		public SkorpionKrolewski( Serial serial ) : base( serial )
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

			if ( BaseSoundID == 96 )
				BaseSoundID = 96;
		}
	}
}