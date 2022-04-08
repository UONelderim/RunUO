using System;
using Server.Items;
using Server.Targeting;
using System.Collections;

namespace Server.Mobiles
{
	[CorpseName( "zgliszcza lady lissith" )] 
	public class LadyLissith : BaseCreature
	{
		public override double DifficultyScalar{ get{ return 1.02; } }
		[Constructable]
		public LadyLissith() : base( AIType.AI_Melee, FightMode.Closest, 12, 1, 0.2, 0.4 )
		{
			Name = "Lady Lissith";
			Body =  0x9D;
			Hue = 0x497;
			BaseSoundID = 0x388; 

			SetStr( 81, 130 );
			SetDex( 116, 152 );
			SetInt( 44, 100 );

			SetHits( 245, 370 );
			SetStam( 116, 152 );
			SetMana( 44, 100 );

			SetDamage( 20, 25 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 40, 50 );
			SetResistance( ResistanceType.Fire, 31, 39 );
			SetResistance( ResistanceType.Cold, 30, 40 );
			SetResistance( ResistanceType.Poison, 71, 80 );
			SetResistance( ResistanceType.Energy, 30, 40 );

			SetSkill( SkillName.Wrestling, 108.6, 123.0 );
			SetSkill( SkillName.Tactics, 102.7, 115.9 );
			SetSkill( SkillName.MagicResist, 78.8, 95.6 );
			SetSkill( SkillName.Anatomy, 68.6, 106.8 );
			SetSkill( SkillName.Poisoning, 96.6, 112.9 );
			
			PackItem( new SpidersSilk( 5 ) );
			PackItem( new LesserPoisonPotion() );
			PackItem( new LesserPoisonPotion() );
		}

		public LadyLissith( Serial serial ) : base( serial )
		{
		}
		
		public override void GenerateLoot()
		{
			AddLoot( LootPack.AosUltraRich, 3 );
		}
				
        public override void AddWeaponAbilities()
        {
            WeaponAbilities.Add( WeaponAbility.BleedAttack, 0.4 );
        }

		public override void OnDeath( Container c )
		{
			base.OnDeath( c );			
			
			/*if ( Utility.RandomDouble() < 0.025 )				
				c.DropItem( new GreymistChest() );
			
			if ( Utility.RandomDouble() < 0.45 )	
				c.DropItem( new LissithsSilk() );*/
				
			if ( Utility.RandomDouble() < 0.1 )
				c.DropItem( new ParrotItem() );
		}
		
		//public override bool GivesMinorArtifact{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Deadly; } }
		public override Poison HitPoison{ get{ return Poison.Deadly; } }
    
    public override OppositionGroup OppositionGroup
		{
			get{ return OppositionGroup.FeyAndUndead; }
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			
			writer.Write( (int) 1 ); // version
			writer.Write( (int)0 ); // GiantBlackWidow version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			
			int version = reader.ReadInt();
			if ( version > 0 )
				reader.ReadInt();// GiantBlackWidow version
		}
	}
}
