// 06.05.18 :: Migalart :: utworzenie

using System;
using Server;
using Server.Misc;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "zwloki przekletego" )]
	public class NPrzeklety : BaseCreature
	{
        public override void AddWeaponAbilities()
        {
            WeaponAbilities.Add( WeaponAbility.Dismount, 0.2 );
            WeaponAbilities.Add( WeaponAbility.ParalyzingBlow, 0.2 );
        }

		[Constructable]
		public NPrzeklety () : base( AIType.AI_Boss, FightMode.Closest, 12, 2, 0.2, 0.4 )
		{
			Name = "przeklety";
			Body = 259;
			BaseSoundID = 0x45A;

			SetStr( 246, 280 );
			SetDex( 101, 125 );
			SetInt( 561, 685 );

			SetDamage( 11, 16 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 35, 55 );
			SetResistance( ResistanceType.Fire, 30, 50 );
			SetResistance( ResistanceType.Cold, 30, 50 );
			SetResistance( ResistanceType.Poison, 30, 50 );
			SetResistance( ResistanceType.Energy, 30, 50 );

			SetSkill( SkillName.Necromancy, 80.1, 100.0 );
			SetSkill( SkillName.SpiritSpeak, 80.1, 100.0 );
			SetSkill( SkillName.EvalInt, 80.1, 102.5 );
			SetSkill( SkillName.Magery, 80.1, 102.5 );
			SetSkill( SkillName.MagicResist, 80.1, 95.0 );
			SetSkill( SkillName.Tactics, 60.1, 75.0 );
			SetSkill( SkillName.Wrestling, 60.1, 70.0 );

			Fame = 8000;
			Karma = -8000;

			VirtualArmor = 40;

		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 1 );
			AddLoot( LootPack.HighScrolls, 1 );
		}

        public override double AttackMasterChance { get { return 0.45; } }
		public override bool CanRummageCorpses{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 1; } }
		public override int Meat{ get{ return 1; } }

		public NPrzeklety( Serial serial ) : base( serial )
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
