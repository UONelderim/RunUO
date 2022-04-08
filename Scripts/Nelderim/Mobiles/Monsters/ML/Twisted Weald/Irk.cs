using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	public class Irk : Changeling
	{
		public override double DifficultyScalar{ get{ return 1.12; } }
		[Constructable]
		public Irk() : base()
		{
			Name = "Irk";
			Hue = 0x489;

			SetStr( 23, 183 );
			SetDex( 259, 360 );
			SetInt( 374, 546 );

			SetHits( 1006, 1064 );
			SetStam( 259, 360 );
			SetMana( 374, 546 );

			SetDamage( 25, 30 );

			SetResistance( ResistanceType.Physical, 80, 90 );
			SetResistance( ResistanceType.Fire, 41, 49 );
			SetResistance( ResistanceType.Cold, 40, 50 );
			SetResistance( ResistanceType.Poison, 41, 50 );
			SetResistance( ResistanceType.Energy, 40, 49 );

			SetSkill( SkillName.Wrestling, 120.3, 123.0 );
			SetSkill( SkillName.Tactics, 120.1, 131.8 );
			SetSkill( SkillName.MagicResist, 132.3, 165.8 );
			SetSkill( SkillName.Magery, 60.9, 80.7 );
			SetSkill( SkillName.EvalInt, 60.4, 90.0 );
			SetSkill( SkillName.Meditation, 108.9, 119.1 );
		}

		public Irk( Serial serial ) : base( serial )
		{
		}
		
		//public override bool GivesMinorArtifact{ get{ return true; } }
		
		public override void GenerateLoot()
		{
			AddLoot( LootPack.AosUltraRich, 3 );
		}				
		
		public override void OnDeath( Container c )
		{
			base.OnDeath( c );		
			
			/*if ( Utility.RandomDouble() < 0.25 )
				c.DropItem( new IrksBrain() );
				
			if ( Utility.RandomDouble() < 0.025 )
				c.DropItem( new PaladinGloves() );*/
				if ( Utility.RandomDouble() < 0.2 )
                PackItem(new TreasureMap(5, Map.Felucca));
				if ( Utility.RandomDouble() < 0.2 )
				PackItem( new FireRuby() );
		}
		
    public override OppositionGroup OppositionGroup
		{
			get{ return OppositionGroup.FeyAndUndead; }
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
