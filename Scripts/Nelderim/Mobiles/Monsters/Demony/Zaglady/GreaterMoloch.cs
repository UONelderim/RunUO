using System;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "zwloki wielkiego demona zaglady" )]
	public class GreaterMoloch : BaseCreature
	{
		public override double DifficultyScalar{ get{ return 1.20; } }
		public override double AttackMasterChance { get { return 0.15; } }
		
        public override void AddWeaponAbilities()
        {
            WeaponAbilities.Add( WeaponAbility.CrushingBlow, 0.4 );
        }

		[Constructable]
		public GreaterMoloch() : base( AIType.AI_Melee, FightMode.Strongest, 11, 1, 0.2, 0.4 )
		{
			Name = "wielki demon zaglady";
			Body = 0x311;
			Hue = 2101;
			BaseSoundID = 0x300;

			SetStr( 1331, 1460 );
			SetDex( 76, 95 );
			SetInt( 41, 65 );

			SetHits( 571, 600 );

			SetDamage( 35, 43 );

			SetResistance( ResistanceType.Physical, 80, 90 );
			SetResistance( ResistanceType.Fire, 60, 70 );
			SetResistance( ResistanceType.Cold, 40, 50 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 20, 30 );

			SetSkill( SkillName.MagicResist, 65.1, 75.0 );
			SetSkill( SkillName.Tactics, 105.1, 110.0 );
			SetSkill( SkillName.Wrestling, 100.1, 110.0 );

			Fame = 17500;
			Karma = -17500;

			VirtualArmor = 52;
		}
		
		public override void OnCarve(Mobile from, Corpse corpse, Item with)
		{
            if (!IsBonded && !corpse.Carved && !IsChampionSpawn)
            {
                if (Utility.RandomDouble() < 0.10)
                    corpse.DropItem(new Bloodspawn());
                if (Utility.RandomDouble() < 0.30)
                    corpse.DropItem(new DaemonBone());
            }

			base.OnCarve(from, corpse, with);
		}
		
		public override void GenerateLoot()
		{
			// 07.01.2013 :: szczaw :: usuniecie PackGold
			//PackGold(1000, 2000 );
			AddLoot( LootPack.UltraRich );
			AddLoot( LootPack.FilthyRich );
		}
public override bool BardImmune { get { return false; } }
		public override Poison PoisonImmune{ get{ return Poison.Regular; } }
        
		public GreaterMoloch( Serial serial ) : base( serial )
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
