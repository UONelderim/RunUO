using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "zwloki zywiolaka lodu" )]
	public class IceElemental : BaseCreature
	{
		[Constructable]
		public IceElemental () : base( AIType.AI_BattleMage, FightMode.Closest, 12, 1, 0.2, 0.4 )
		{
			Name = "zywiolak lodu";
			Body = 161;
			BaseSoundID = 268;

			SetStr( 156, 185 );
			SetDex( 96, 115 );
			SetInt( 171, 192 );

			SetHits( 94, 111 );

			SetDamage( 10, 21 );

			SetDamageType( ResistanceType.Physical, 25 );
			SetDamageType( ResistanceType.Cold, 75 );

			SetResistance( ResistanceType.Physical, 35, 45 );
			SetResistance( ResistanceType.Fire, 5, 10 );
			SetResistance( ResistanceType.Cold, 50, 60 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 20, 30 );

			SetSkill( SkillName.EvalInt, 10.5, 60.0 );
			SetSkill( SkillName.Magery, 10.5, 60.0 );
			SetSkill( SkillName.MagicResist, 30.1, 80.0 );
			SetSkill( SkillName.Tactics, 70.1, 100.0 );
			SetSkill( SkillName.Wrestling, 60.1, 100.0 );

			Fame = 4000;
			Karma = -4000;

			VirtualArmor = 40;

			PackItem( new BlackPearl( 2 ) );
			PackReg( 2 );
		}

        public override void OnCarve(Mobile from, Corpse corpse, Item with)
        {
            if (!IsBonded && !corpse.Carved && !IsChampionSpawn)
            {
                if (Utility.RandomDouble() < 0.05)
                    corpse.DropItem(new Blackmoor());
					if (Utility.RandomDouble() < 0.5)
                    corpse.DropItem(new DryIce());
            }

            base.OnCarve(from, corpse, with);
        }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average, 2 );
		}
		public override bool BleedImmune{ get{ return true; } }

		public IceElemental( Serial serial ) : base( serial )
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