using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "resztki kryształowego węża głębinowego" )]
	public class CrystalSeaSerpent : BaseCreature
	{
		public override double DifficultyScalar{ get{ return 1.02; } }
		[Constructable]
		public CrystalSeaSerpent() : base( AIType.AI_BattleMage, FightMode.Closest, 12, 1, 0.2, 0.4 )
		{
			Name = "kryształowy wąż głębinowy";
			Body = 0x96;
			Hue = 0x47E;
			BaseSoundID = 0x1BF;

			SetStr( 250, 450 );
			SetDex( 100, 150 );
			SetInt( 90, 190 );

			SetHits( 230, 330 );

			SetDamage( 10, 18 );

			SetDamageType( ResistanceType.Physical, 10 );
			SetDamageType( ResistanceType.Cold, 45 );
			SetDamageType( ResistanceType.Energy, 45 );

			SetResistance( ResistanceType.Physical, 50, 70 );
			SetResistance( ResistanceType.Cold, 70, 90 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 60, 80 );

			SetSkill( SkillName.MagicResist, 60.0, 75.0 );
			SetSkill( SkillName.Tactics, 60.0, 70.0 );
			SetSkill( SkillName.Wrestling, 60.0, 70.0 );
			
			CanSwim = true;
			CantWalk = true;

			Fame = 8000;
			Karma = -8000;
			
			//PackArcaneScroll( 0, 1 );
			
			if ( Utility.RandomDouble() < 0.05 )
				PackItem( new SpecialFishingNet() );
		}
		
		public override void GenerateLoot()
		{
			AddLoot( LootPack.Average );
		}		
		
		public override void OnDeath( Container c )
		{
			base.OnDeath( c );		
			
			/*if ( Utility.RandomDouble() < 0.05 )
				c.DropItem( new CrushedCrystals() );
				
			if ( Utility.RandomDouble() < 0.1 )
				c.DropItem( new IcyHeart() );
				
			if ( Utility.RandomDouble() < 0.1 )
				c.DropItem( new LuckyDagger() );
			*/
		}
		
		#region Breath
		public override double BreathDamageScalar{ get{ return 0.4; } }
		public override int BreathFireDamage{ get{ return 0; } }
		public override int BreathColdDamage{ get{ return 100; } }		
		public override int BreathEffectHue{ get{ return 0x47E; } }
		public override bool HasBreath{ get{ return true; } } 
		#endregion

		public override int Hides{ get{ return 10; } }
		public override HideType HideType{ get{ return HideType.Horned; } }
		public override int Scales{ get{ return 8; } }
		public override ScaleType ScaleType{ get{ return ScaleType.Blue; } }
		public override int Meat{ get{ return 10; } }
		public override int TreasureMapLevel{ get{ return 3; } }

		public CrystalSeaSerpent( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			
			writer.Write( (int) 1 ); // version 
			writer.Write( (int)0 ); // BaseSerpent version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			
			int version = reader.ReadInt();
			if(version == 1)
				reader.ReadInt();
		}
	}
}	
