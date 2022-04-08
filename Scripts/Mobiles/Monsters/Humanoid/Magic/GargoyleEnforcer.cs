using System;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "zwloki gargulca msciciela" )]
	public class GargoyleEnforcer : BaseCreature
	{
        public override void AddWeaponAbilities()
        {
            WeaponAbilities.Add( WeaponAbility.WhirlwindAttack, 0.4 );;
        }

		[Constructable]
		public GargoyleEnforcer() : base( AIType.AI_BattleMage, FightMode.Closest, 12, 1, 0.2, 0.4 )
		{
			Name = "gargulec msciciel";
			Body = 0x2F2;
			BaseSoundID = 0x174;

			SetStr( 760, 850 );
			SetDex( 102, 150 );
			SetInt( 152, 200 );

			SetHits( 482, 485 );

			SetDamage( 7, 14 );

			SetResistance( ResistanceType.Physical, 40, 60 );
			SetResistance( ResistanceType.Fire, 50, 60 );
			SetResistance( ResistanceType.Cold, 20, 30 );
			SetResistance( ResistanceType.Poison, 25, 35 );
			SetResistance( ResistanceType.Energy, 15, 25 );

			SetSkill( SkillName.MagicResist, 120.1, 130.0 );
			SetSkill( SkillName.Tactics, 70.1, 80.0 );
			SetSkill( SkillName.Wrestling, 80.1, 90.0 );
			SetSkill( SkillName.Swords, 80.1, 90.0 );
			SetSkill( SkillName.Anatomy, 70.1, 80.0 );
			SetSkill( SkillName.Magery, 80.1, 90.0 );
			SetSkill( SkillName.EvalInt, 70.3, 100.0 );
			SetSkill( SkillName.Meditation, 70.3, 100.0 );

			Fame = 5000;
			Karma = -5000;

			VirtualArmor = 50;

			if ( 0.2 > Utility.RandomDouble() )
				PackItem( new GargoylesPickaxe() );
			
			PackReg( 4 );
		}

        public override void OnCarve(Mobile from, Corpse corpse, Item with)
        {
            if (!IsBonded && !corpse.Carved && !IsChampionSpawn)
            {
                if (Utility.RandomDouble() < 0.07)
                    corpse.DropItem(new Bloodspawn());
                if (Utility.RandomDouble() < 0.15)
                    corpse.DropItem(new DaemonBone());
            }

            base.OnCarve(from, corpse, with);
        }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );
		}

		public override int Meat{ get{ return 1; } }

		public GargoyleEnforcer( Serial serial ) : base( serial )
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