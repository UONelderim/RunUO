using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "zwloki strzygi" )]
	public class Bogle : BaseCreature
	{
		[Constructable]
		public Bogle() : base( AIType.AI_BattleMage, FightMode.Closest, 12, 1, 0.2, 0.4 )
		{
			Name = "strzyga";
			Body = 153;
            Hue = 246;
			BaseSoundID = 0x482;

			SetStr( 76, 100 );
			SetDex( 76, 95 );
			SetInt( 36, 60 );

			SetHits( 46, 60 );

			SetDamage( 7, 11 );

			SetSkill( SkillName.EvalInt, 55.1, 70.0 );
			SetSkill( SkillName.Magery, 55.1, 70.0 );
			SetSkill( SkillName.MagicResist, 55.1, 70.0 );
			SetSkill( SkillName.Tactics, 45.1, 60.0 );
			SetSkill( SkillName.Wrestling, 45.1, 55.0 );

			Fame = 4000;
			Karma = -4000;

			VirtualArmor = 28;
			PackItem( Loot.RandomWeapon() );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Meager );
		}
		
		public override int Bones{ get{ return 1; } }
		
		public override bool BleedImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }

		public Bogle( Serial serial ) : base( serial )
		{
		}

		public override OppositionGroup OppositionGroup
		{
			get{ return OppositionGroup.FeyAndUndead; }
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