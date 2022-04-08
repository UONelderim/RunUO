using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "zwloki wiekszego padliniaka" )]
	public class Corpser2 : BaseCreature
	{
        private static IWeapon m_Weapon = new MieczDalekiegoZasiegu(3); // zwiekszony zasieg
        public override IWeapon GetDefaultWeapon() { return m_Weapon; } // zwiekszony zasieg

		[Constructable]
		public Corpser2() : base( AIType.AI_RangedMelee, FightMode.Closest, 12, 1, 0.2, 0.4 )
		{
			Name = "wiekszy padliniak";
			Body = 8;
			BaseSoundID = 684;
			Hue = 470;

			SetStr( 250, 300 );
			SetDex( 26, 45 );
			SetInt( 26, 40 );

			SetHits( 250, 300 );
			SetMana( 0 );

			SetDamage( 14, 28 );

			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Poison, 50 );

			SetResistance( ResistanceType.Physical, 35, 40 );
			SetResistance( ResistanceType.Fire, 25, 35 );
			SetResistance( ResistanceType.Cold, 20, 30 );
			SetResistance( ResistanceType.Poison, 40, 50 );
			SetResistance( ResistanceType.Energy, 25, 35 );

			SetSkill( SkillName.MagicResist, 15.1, 20.0 );
			SetSkill( SkillName.Tactics, 45.1, 60.0 );
			SetSkill( SkillName.Wrestling, 45.1, 60.0 );

			Fame = 1000;
			Karma = -1000;

			VirtualArmor = 18;

			if ( 0.25 > Utility.RandomDouble() )
				PackItem( new Board( 10 ) );
			else
				PackItem( new Log( 10 ) );

			PackItem( new MandrakeRoot( 3 ) );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Meager );
		}

		public override Poison PoisonImmune{ get{ return Poison.Lesser; } }
		public override bool DisallowAllMoves{ get{ return true; } }

		public Corpser2( Serial serial ) : base( serial )
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

			if ( BaseSoundID == 352 )
				BaseSoundID = 684;
		}
	}
}
