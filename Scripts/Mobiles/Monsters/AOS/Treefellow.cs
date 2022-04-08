using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "zwloki drzewca" )]
	public class Treefellow : BaseCreature
	{
        public override void AddWeaponAbilities()
        {
            WeaponAbilities.Add( WeaponAbility.Dismount, 0.4 );;
        }

		[Constructable]
		public Treefellow() : base( AIType.AI_Melee, FightMode.Evil, 12, 1, 0.2, 0.4 )
		{
			Name = "drzewiec";
			Body = 301;

			SetStr( 196, 220 );
			SetDex( 31, 55 );
			SetInt( 66, 90 );

			SetHits( 118, 132 );

			SetDamage( 12, 16 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 20, 25 );
			SetResistance( ResistanceType.Cold, 50, 60 );
			SetResistance( ResistanceType.Poison, 30, 35 );
			SetResistance( ResistanceType.Energy, 20, 30 );

			SetSkill( SkillName.MagicResist, 40.1, 55.0 );
			SetSkill( SkillName.Tactics, 65.1, 90.0 );
			SetSkill( SkillName.Wrestling, 65.1, 85.0 );

			Fame = 500;
			Karma = 1500;

			VirtualArmor = 24;
			PackItem( new Log( Utility.RandomMinMax( 10, 15 ) ) );
			
			switch ( Utility.Random( 4 ) )
			{
				case 0: PackItem( new OakLog( Utility.RandomMinMax( 5, 10 ) ) ); break;
				case 1: PackItem( new AshLog( Utility.RandomMinMax( 5, 10 ) ) ); break;
				case 2: PackItem( new YewLog( Utility.RandomMinMax( 5, 10 ) ) ); break;
				case 3: PackItem( new BloodwoodLog( Utility.RandomMinMax( 4, 8 ) ) ); break;
			}

            if (0.02 > Utility.RandomDouble())
                PackItem(new TreefellowsAxe());
			if (0.5 > Utility.RandomDouble())
                PackItem(new PetrafiedWood());
		}

		public override OppositionGroup OppositionGroup
		{
			get{ return OppositionGroup.FeyAndUndead; }
		}

		public override int GetIdleSound()
		{
			return 443;
		}

		public override int GetDeathSound()
		{
			return 31;
		}

		public override int GetAttackSound()
		{
			return 672;
		}

		public override bool BleedImmune{ get{ return true; } }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
		}

		public Treefellow( Serial serial ) : base( serial )
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

			if ( BaseSoundID == 442 )
				BaseSoundID = -1;
		}
	}
}