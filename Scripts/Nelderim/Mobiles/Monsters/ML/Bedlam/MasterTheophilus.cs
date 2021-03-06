using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "resztki nekromanty" )]
	public class MasterTheophilus : BaseCreature
	{
		public override double DifficultyScalar{ get{ return 1.02; } }
		[Constructable]
		public MasterTheophilus() : base( AIType.AI_NecroMage, FightMode.Closest, 12, 6, 0.2, 0.4 )
		{
			Name = "Mistrz Teofil";
			Title = "nekromanta";
			BaseSoundID = 0x1C3;
			Body = Utility.Random( 0x7D, 1 );

			SetStr( 137, 187 );
			SetDex( 253, 301 );
			SetInt( 393, 444 );

			SetHits( 663, 876 );

			SetDamage( 15, 20 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 55, 60 );
			SetResistance( ResistanceType.Fire, 50, 58 );
			SetResistance( ResistanceType.Cold, 50, 60 );
			SetResistance( ResistanceType.Poison, 50, 60 );
			SetResistance( ResistanceType.Energy, 50, 60 );

			SetSkill( SkillName.Wrestling, 69.9, 105.3 );
			SetSkill( SkillName.Tactics, 113.0, 117.9 );
			SetSkill( SkillName.MagicResist, 127.0, 132.8 );
			SetSkill( SkillName.Magery, 138.1, 143.7 );
			SetSkill( SkillName.EvalInt, 125.6, 133.8 );
			SetSkill( SkillName.Necromancy, 125.6, 133.8 );
			SetSkill( SkillName.SpiritSpeak, 125.6, 133.8 );
			SetSkill( SkillName.Meditation, 128.8, 132.9 );
			
			AddItem( new Shoes( 0x537 ) );
			AddItem( new Robe( 0x452 ) );
			
			for ( int i = 0; i < 2; i ++ )
				if ( Utility.RandomBool() )
					PackNecroScroll( Utility.RandomMinMax( 5, 9 ) );
				else
					PackScroll( 4, 7 );
				
			PackReg( 7 );
			PackReg( 7 );
			PackReg( 8 );
		}
		
		public override void GenerateLoot()
		{
			AddLoot( LootPack.AosUltraRich, 4 );
		}
		public override bool AllureImmune { get { return true; } }

		public override void OnDeath( Container c )
		{
			base.OnDeath( c );		
			
			if ( Paragon.ChestChance > Utility.RandomDouble() )
				c.DropItem( new ParagonChest( Name, TreasureMapLevel ) );
		}
		
		//public override bool GivesMinorArtifact{ get{ return true; } }
		public override int TreasureMapLevel{ get{ return 5; } }
	
		public MasterTheophilus( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			
			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			
			int version = reader.ReadInt();
		}
	}
}

