using System;
using System.Collections;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "Zwloki Lady Marai" )]
	public class LadyMarai : BaseCreature
	{
		public override double DifficultyScalar{ get{ return 1.02; } }
		[Constructable]
		public LadyMarai() : base( AIType.AI_Melee, FightMode.Closest, 12, 1, 0.015, 0.075 )
		{
			Name = "lady marai";
			Hue = 0x21;
			Body = 0x93;
			BaseSoundID = 0x1C3;

			SetStr( 221, 304 );
			SetDex( 98, 138 );
			SetInt( 54, 99 );

			SetHits( 694, 846 );

			SetDamage( 15, 25 );

			SetDamageType( ResistanceType.Physical, 40 );
			SetDamageType( ResistanceType.Cold, 60 );

			SetResistance( ResistanceType.Physical, 55, 65 );
			SetResistance( ResistanceType.Fire, 40, 50 );
			SetResistance( ResistanceType.Cold, 70, 80 );
			SetResistance( ResistanceType.Poison, 40, 50 );
			SetResistance( ResistanceType.Energy, 50, 60 );

			SetSkill( SkillName.Wrestling, 126.6, 137.2 );
			SetSkill( SkillName.Tactics, 128.7, 134.5 );
			SetSkill( SkillName.MagicResist, 102.1, 119.1 );
			SetSkill( SkillName.Anatomy, 126.2, 136.5 );
			
			AddItem( new PlateLegs() );
		}
		
		public override void GenerateLoot()
		{
			AddLoot( LootPack.AosUltraRich, 4 );
		}

        public override void AddWeaponAbilities()
        {
            WeaponAbilities.Add( WeaponAbility.CrushingBlow, 0.4 );
        }

		public override void OnDeath( Container c )
		{
			base.OnDeath( c );		
			
			/*if ( Utility.RandomDouble() < 0.15 )
				c.DropItem( new DisintegratingThesisNotes() );
							
			if ( Utility.RandomDouble() < 0.1 )
				c.DropItem( new ParrotItem() );*/
		}
		
		//public override bool GivesMinorArtifact{ get{ return true; } }
	
		public LadyMarai( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			
			writer.Write( (int) 1 ); // version
			writer.Write( (int)0 ); // SkeletalKnight version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			
			int version = reader.ReadInt();
			if ( version > 0 )
				reader.ReadInt(); // SkeletalKnight version
		}
	}
}

