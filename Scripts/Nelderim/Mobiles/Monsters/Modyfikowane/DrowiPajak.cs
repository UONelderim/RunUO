// 09.09.18 ::juri :: utworzenie

using System;
using System.Collections;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "resztki ogromnego pajaka" )]
	public class DrowiPajak : BaseCreature
	{
		public override bool BleedImmune { get { return false; } }
		public override double SwitchTargetChance { get { return 0.5; } }
		public override void  AddWeaponAbilities()
		{
            WeaponAbilities.Add( WeaponAbility.ConcussionBlow, 0.4 );
		}
		
		private static bool OverrideRules = true;

		private static double m_AbilityChance = 0.5;

		private static int m_ReturnTime = 120;

		private static int m_MinTime	= 10;
		private static int m_MaxTime	= 20;

		private DateTime m_NextAbilityTime;

		private DateTime m_NextReturnTime;

		private ArrayList m_Minions;
		
		[Constructable]
		public DrowiPajak() : base( AIType.AI_Boss, FightMode.Strongest, 12, 1, 0.11, 0.4 )
		{
			Name = "Ilharess Myrlocha'r";
			Body = 173;
			Hue = 0x3E8;
			BaseSoundID = 1170;

			SetStr( 500 );
			SetDex( 150 );
			SetInt( 400 );

			SetHits( 8000 );
			SetMana( 3000 );
			SetStam( 1000 );

			SetDamage( 25, 35 );

			SetDamageType( ResistanceType.Physical, 5 );
			SetDamageType( ResistanceType.Poison, 95 );

			SetResistance( ResistanceType.Physical, 98 );
			SetResistance( ResistanceType.Fire, 75 );
			SetResistance( ResistanceType.Cold, 90 );
			SetResistance( ResistanceType.Poison, 120 );
			SetResistance( ResistanceType.Energy, 90 );

			SetSkill( SkillName.EvalInt, 150.0 );
			SetSkill( SkillName.Magery, 150.0 );
			SetSkill( SkillName.Meditation, 150.0 );
			SetSkill( SkillName.Poisoning, 200.0 );
			SetSkill( SkillName.MagicResist, 120.0 );
			SetSkill( SkillName.Wrestling, 90.0 );

			Fame = 5000;
			Karma = -15000;

			VirtualArmor = 60;

			m_Minions = new ArrayList();
			
			PackItem( new DeadlyPoisonPotion() );
			PackItem( new SpidersSilk( 20 ) );	
		}

		public override void GenerateLoot()
		{
			// 07.01.2013 :: szczaw :: usuniecie PackGold
			//PackGold(6000, 8000 );
			AddLoot( LootPack.FilthyRich, 2 );
			AddLoot( LootPack.UltraRich, 2 );
			AddLoot( LootPack.HighScrolls, 1 );
		}

		public override bool AutoDispel{ get{ return true; } }
		public override bool BardImmune{ get{ return true; } } 
		public override Poison PoisonImmune{ get{ return Poison.Deadly; } }		
		public override Poison HitPoison{ get{ return Poison.Deadly; } }

		public override bool CanPaperdollBeOpenedBy( Mobile from )
		{
			return false;
		}

		private int CountAliveMinions()
		{
			int alive = 0;

			foreach( Mobile m in m_Minions )
			{
				if ( m.Alive && !m.Deleted ) alive++;
			}

			return alive;
		}


		public override void OnDamage( int amount, Mobile from, bool willKill )
		{
			if ( from != null && !willKill && amount > 10 && from.Player && 10 > Utility.Random( 100 ) )
			{
				string[] toSay = new string[]
					{
						"Pradawna Magia Wysysa z Ciebie Zycie!",
						"Paralizuje Cie Strach!",
						"Odczuwasz Na Sobie Niszczycielska Aure!",
					};

				this.Say( true, String.Format( toSay[Utility.Random( toSay.Length )] ) );
			}

			base.OnDamage( amount, from, willKill );
		}



		private void SpawnMinions()
		{
			if ( CountAliveMinions() != 0 ) return;

			m_Minions.Clear();

			Map map = this.Map;

			if ( map == null ) return;
			
			int type = Utility.Random( 2 );			

			int minions = Utility.RandomMinMax( 6, 8 );

			for ( int i = 0; i < minions; ++i )
			{
				BaseCreature minion;

				switch ( type )
				{
				default:
				case 0: minion = new GiantSpider(); break;
				case 1: minion = new DreadSpider(); break;
				}

				minion.Team = this.Team;

				bool validLocation = false;

				Point3D loc = this.Location;

				for ( int j = 0; !validLocation && j < 5; ++j )
				{
					int x = X + Utility.Random( 8 ) - 4;
					int y = Y + Utility.Random( 8 ) - 4;
					int z = map.GetAverageZ( x, y );

					if ( validLocation = map.CanFit( x, y, this.Z, 16, false, false ) )
						loc = new Point3D( x, y, Z );
					else if ( validLocation = map.CanFit( x, y, z, 16, false, false ) )
						loc = new Point3D( x, y, z );
				}

				minion.MoveToWorld( loc, map );
				minion.Combatant = Combatant;

				m_Minions.Add( minion );
			}

			m_NextReturnTime = DateTime.Now + TimeSpan.FromSeconds( m_ReturnTime );
		}

		public override void OnThink()
		{
			
			if ( !OverrideRules || Combatant == null )
			{
				base.OnThink();

				return;
			}

			if ( DateTime.Now >= m_NextAbilityTime )
			{
				if ( m_AbilityChance > Utility.RandomDouble() )
				{
					SpawnMinions();
				}

				m_NextAbilityTime = DateTime.Now + TimeSpan.FromSeconds( Utility.RandomMinMax( m_MinTime, m_MaxTime ) );
			}

			base.OnThink();
		}
		
		public DrowiPajak( Serial serial ) : base( serial )
		{
			m_Minions = new ArrayList();
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
