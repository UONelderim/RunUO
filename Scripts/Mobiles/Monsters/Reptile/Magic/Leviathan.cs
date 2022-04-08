using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "zwloki leviatana" )]
	public class Leviathan : BaseCreature
	{
		private Mobile m_Fisher;

		public Mobile Fisher
		{
			get{ return m_Fisher; }
			set{ m_Fisher = value; }
		}

		[Constructable]
		public Leviathan() : this( null )
		{
		}

		[Constructable]
		public Leviathan( Mobile fisher ) : base( AIType.AI_BattleMage, FightMode.Closest, 12, 1, 0.2, 0.4 )
		{
			m_Fisher = fisher;

			// May not be OSI accurate; mostly copied from krakens
			Name = "leviatan";
			Body = 77;
			BaseSoundID = 353;

			Hue = 0x481;

			SetStr( 1000 );
			SetDex( 501, 520 );
			SetInt( 251, 300 );
			//SetInt( 501, 515 );

			SetHits( 1500 );

			SetDamage( 25, 33 );

			SetDamageType( ResistanceType.Physical, 70 );
			SetDamageType( ResistanceType.Cold, 30 );

			SetResistance( ResistanceType.Physical, 55, 65 );
			SetResistance( ResistanceType.Fire, 45, 55 );
			SetResistance( ResistanceType.Cold, 45, 55 );
			SetResistance( ResistanceType.Poison, 35, 45 );
			SetResistance( ResistanceType.Energy, 25, 35 );

			//SetSkill( SkillName.EvalInt, 90, 100 );
			//SetSkill( SkillName.Magery, 90, 100 );
			//SetSkill( SkillName.MagicResist, 90, 100 );
			//SetSkill( SkillName.Tactics, 90, 100 );
			//SetSkill( SkillName.Wrestling, 90, 100 );
			
			SetSkill( SkillName.EvalInt, 97.6, 107.5 );
			SetSkill( SkillName.Magery, 97.6, 107.5 );
			SetSkill( SkillName.MagicResist, 97.6, 107.5 );
			SetSkill( SkillName.Tactics, 97.6, 107.5 );
			SetSkill( SkillName.Wrestling, 97.6, 107.5 );

			Fame = 24000;
			Karma = -24000;

			VirtualArmor = 50;

			CanSwim = true;
			CantWalk = true;

			PackItem( new MessageInABottle() );
			PackItem( new MessageInABottle() );

			Rope rope = new Rope();
			rope.ItemID = 0x14F8;
			PackItem( rope );

			rope = new Rope();
			rope.ItemID = 0x14FA;
			PackItem( rope );
			
			if (Utility.RandomDouble() < 0.25)
                PackItem(new TreasureMap(6, Map.Felucca));
			
		}

		public override bool HasBreath{ get{ return true; } }
		public override int BreathPhysicalDamage{ get{ return 30; } } // TODO: Verify damage type
		public override int BreathColdDamage{ get{ return 30; } }
		public override int BreathFireDamage{ get{ return 0; } }
		public override int BreathEffectHue{ get{ return 0x1ED; } }
		public override double BreathDamageScalar{ get{ return 0.05; } }
		public override double BreathMinDelay{ get{ return 7.5; } }
		public override double BreathMaxDelay{ get{ return 10.0; } }

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 5 );
		}
				
		//public override double TreasureMapChance{ get{ return 0.25; } }
		//public override int TreasureMapLevel{ get{ return 5; } }
		//+1 if paragon
		
		public Leviathan( Serial serial ) : base( serial )
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

		public static void GiveArtifactTo( Mobile m )
		{
            Item item = ArtifactHelper.CreateRandomFishingArtifact();

			if ( item == null )
				return;

			// TODO: Confirm messages
			if ( m.AddToBackpack( item ) )
				//m.SendMessage( "As a reward for slaying the mighty leviathan, an artifact has been placed in your backpack." );
                m.SendMessage("W nagrode za pokonanie mitycznego lewiatana otrzymujesz artefakt, ktory laduje w twoim plecaku.");
			else
				//m.SendMessage( "As your backpack is full, your reward for destroying the legendary leviathan has been placed at your feet." );
                m.SendMessage("Twoj plecak jest przepelniony, przez co artefakt bedacy nagroda za pokonanie lewiatana laduje u twoich stop.");
		}

		public override void OnKilledBy( Mobile mob )
		{
			base.OnKilledBy( mob );

			if ( Paragon.CheckArtifactChance( mob, this ) )
			{
				GiveArtifactTo( mob );

				if ( mob == m_Fisher )
					m_Fisher = null;
			}
		}

		public override void OnDeath( Container c )
		{
			base.OnDeath( c );

			if ( m_Fisher != null )
				GiveArtifactTo( m_Fisher );

			m_Fisher = null;
		}
	}
}