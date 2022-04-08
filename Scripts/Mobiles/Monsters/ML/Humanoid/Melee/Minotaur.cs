using System;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "zwloki minotaura" )]
	public class Minotaur : BaseCreature
	{
        public override void AddWeaponAbilities()
        {
            WeaponAbilities.Add( WeaponAbility.ParalyzingBlow, 0.4 );;
        }

		[Constructable]
		public Minotaur() : base( AIType.AI_Melee, FightMode.Closest, 12, 1, 0.2, 0.4 ) // NEED TO CHECK
		{
			Name = "minotaur";
			Body = 263;
			BaseSoundID = 604;

			SetStr( 336, 385 );
			SetDex( 96, 115 );
			SetInt( 31, 55 );

			SetHits( 202, 231 );
			SetMana( 0 );

			SetDamage( 7, 23 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 55, 65 );
			SetResistance( ResistanceType.Fire, 25, 35 );
			SetResistance( ResistanceType.Cold, 30, 40 );
			SetResistance( ResistanceType.Poison, 30, 40 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.Meditation, 0 );
			SetSkill( SkillName.EvalInt, 0 );
			SetSkill( SkillName.Magery, 0 );
			SetSkill( SkillName.Poisoning, 0 );
			SetSkill( SkillName.Anatomy, 0 );
			SetSkill( SkillName.MagicResist, 60.3, 105.0 );
			SetSkill( SkillName.Tactics, 80.1, 100.0 );
			SetSkill( SkillName.Wrestling, 80.1, 90.0 );

			Fame = 5000;
			Karma = -5000;

			VirtualArmor = 44; // Don't know what it should be
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );  // Need to verify
		}

        public override double AttackMasterChance { get { return 0.3; } }
		public override int Meat{ get{ return 4; } }
		public override int TreasureMapLevel{ get{ return 3; } }

		// Using Tormented Minotaur sounds - Need to veryfy
		public override int GetAngerSound()
		{
			return 0x597;
		}

		public override int GetIdleSound()
		{
			return 0x596;
		}

		public override int GetAttackSound()
		{
			return 0x599;
		}

		public override int GetHurtSound()
		{
			return 0x59a;
		}

		public override int GetDeathSound()
		{
			return 0x59c;
		}

		public Minotaur( Serial serial ) : base( serial )
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
