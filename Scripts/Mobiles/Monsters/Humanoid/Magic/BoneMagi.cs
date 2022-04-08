using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "zwloki koscianego maga" )]
	public class BoneMagi : BaseCreature
	{
		[Constructable]
		public BoneMagi() : base( AIType.AI_BattleMage, FightMode.Closest, 12, 1, 0.2, 0.4 )
		{
			Name = "kosciany mag";
			Body = 148;
			BaseSoundID = 451;

			SetStr( 76, 100 );
			SetDex( 56, 75 );
			SetInt( 186, 210 );

			SetHits( 46, 60 );

			SetDamage( 3, 7 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 35, 40 );
			SetResistance( ResistanceType.Fire, 20, 30 );
			SetResistance( ResistanceType.Cold, 50, 60 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.EvalInt, 60.1, 70.0 );
			SetSkill( SkillName.Magery, 60.1, 70.0 );
			SetSkill( SkillName.MagicResist, 55.1, 70.0 );
			SetSkill( SkillName.Tactics, 45.1, 60.0 );
			SetSkill( SkillName.Wrestling, 45.1, 55.0 );

			Fame = 3000;
			Karma = -3000;

			VirtualArmor = 38;

			PackReg( 3 );
			PackNecroReg( 3, 10 );

		}

        public override void OnCarve(Mobile from, Corpse corpse, Item with)
        {
            if (!IsBonded && !corpse.Carved && !IsChampionSpawn)
            {
                if (Utility.RandomDouble() < 0.05)
                    corpse.DropItem(new Pumice());
					if (Utility.RandomDouble() < 0.5)
                    corpse.DropItem(new TrappedGhost());
					if (Utility.RandomDouble() < 0.5)
                    corpse.DropItem(new Soul());
            }

            base.OnCarve(from, corpse, with);
        }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
			AddLoot( LootPack.Potions );
		}
		
		public override int Bones{ get{ return 1; } }
		
		public override bool BleedImmune{ get{ return true; } }
		
		public override OppositionGroup OppositionGroup
		{
			get{ return OppositionGroup.FeyAndUndead; }
		}

		public override Poison PoisonImmune{ get{ return Poison.Regular; } }

		public BoneMagi( Serial serial ) : base( serial )
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