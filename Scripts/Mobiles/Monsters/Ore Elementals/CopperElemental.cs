using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "zwloki zywiolaka miedzi" )]
	public class CopperElemental : BaseCreature
	{
		[Constructable]
		public CopperElemental() : this( 5 )
		{
		}

		[Constructable]
		public CopperElemental( int oreAmount ) : base( AIType.AI_Melee, FightMode.Closest, 12, 1, 0.2, 0.4 )
		{
			Name = "zywiolak miedzi";
			Body = 109;
			BaseSoundID = 268;

			SetStr( 226, 255 );
			SetDex( 126, 145 );
			SetInt( 71, 92 );

			SetHits( 136, 153 );

			SetDamage( 9, 16 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 30, 40 );
			SetResistance( ResistanceType.Fire, 30, 40 );
			SetResistance( ResistanceType.Cold, 30, 40 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 10, 20 );

			SetSkill( SkillName.MagicResist, 50.1, 95.0 );
			SetSkill( SkillName.Tactics, 60.1, 100.0 );
			SetSkill( SkillName.Wrestling, 60.1, 100.0 );

			Fame = 4800;
			Karma = -4800;

			VirtualArmor = 26;

			PackItem( new CopperOre( oreAmount ) );
		}

        public override void OnCarve(Mobile from, Corpse corpse, Item with)
        {
            if (!IsBonded && !corpse.Carved && !IsChampionSpawn)
            {
                if (Utility.RandomDouble() < 0.05)
                    corpse.DropItem(new Blackmoor());
            }

            base.OnCarve(from, corpse, with);
        }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
		}

		public override bool BleedImmune{ get{ return true; } }
		public override bool AutoDispel{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 1; } }

		public override void CheckReflect( Mobile caster, ref bool reflect )
		{
			reflect = true; // Every spell is reflected back to the caster
		}

		public CopperElemental( Serial serial ) : base( serial )
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