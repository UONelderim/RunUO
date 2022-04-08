using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "a gnarled tree corpse" )]
	public class AReaperRedux : BaseCreature
	{
        public override void AddWeaponAbilities()
        {
            WeaponAbilities.Add( WeaponAbility.Dismount, 0.4 );
        }

		[Constructable]
		public AReaperRedux() : base( AIType.AI_BattleMage, FightMode.Closest, 12, 1, 0.2, 0.4 )
		{
			Name = "a reaper redux";
			Body = 285;
			BaseSoundID = 442;

			SetStr( 826, 836 );
			SetDex( 255, 265 );
			SetInt( 645, 655 );

			SetHits( 893, 903 );

			SetDamage( 13, 20 );//was 10, 15

			SetDamageType( ResistanceType.Physical, 0 );
			SetDamageType( ResistanceType.Cold, 60 );
			SetDamageType( ResistanceType.Energy, 40 );

			SetResistance( ResistanceType.Physical, 35, 45 );
			SetResistance( ResistanceType.Fire, 15, 25 );
			SetResistance( ResistanceType.Cold, 10, 20 );
			SetResistance( ResistanceType.Poison, 40, 50 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.EvalInt, 90.1, 100.0 );
			SetSkill( SkillName.Magery, 90.1, 100.0 );
			SetSkill( SkillName.MagicResist, 100.1, 125.0 );
			SetSkill( SkillName.Tactics, 45.1, 60.0 );
			SetSkill( SkillName.Wrestling, 50.1, 60.0 );

			Fame = 18000;
			Karma = -18000;

			VirtualArmor = 40;

			PackItem( new Log( 3 ) );
			PackItem( new Kindling( 10 ) );
            PackItem( new Engines.Plants.Seed() );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich );
			AddLoot( LootPack.MedScrolls, 2 );
		}

		public override bool CanRummageCorpses{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }
		public override int TreasureMapLevel{ get{ return 4; } }

		public AReaperRedux( Serial serial ) : base( serial )
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
