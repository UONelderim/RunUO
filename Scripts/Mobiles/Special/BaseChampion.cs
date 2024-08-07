using System;
using System.Collections;
using Server;
using Server.Items;
using Server.Engines.CannedEvil;
using System.Collections.Generic;

namespace Server.Mobiles
{
	public abstract class BaseChampion : BaseCreature
	{
		public BaseChampion( AIType aiType ) : this( aiType, FightMode.Closest )
		{
		}

		public BaseChampion( AIType aiType, FightMode mode ) : base( aiType, mode, 18, 1, 0.1, 0.2 )
		{
		}

		public BaseChampion( Serial serial ) : base( serial )
		{
		}

		public abstract ChampionSkullType SkullType{ get; }

	//	public abstract Type[] UniqueList{ get; }
	//	public abstract Type[] SharedList{ get; }
		public abstract Type[] DecorativeList{ get; }
		public abstract MonsterStatuetteType[] StatueTypes { get; }

		public virtual bool NoGoodies { get { return false; } }

		private int m_PSDropCount = 6;
		public int PSDropCount { 
			get { return m_PSDropCount; } 
			set { m_PSDropCount = value; } 
		}

		private int m_TotalGoldDrop = 10000;

		public int TotalGoldDrop {
			get { return m_TotalGoldDrop; }
			set { m_TotalGoldDrop = value; }
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

		public Item GetArtifact()
		{
			double random = Utility.RandomDouble();
			if ( 0 >= random )
		/*		return CreateArtifact( UniqueList );
			else if ( 0 >= random )
				return CreateArtifact( SharedList );
			else*/ if ( 0.1 >= random )
				return CreateArtifact( DecorativeList );
			return null;
		}

		public Item CreateArtifact( Type[] list )
		{
			if ( list.Length == 0 )
				return null;

			int random = Utility.Random( list.Length );

			Type type = list[random];

			Item artifact = Loot.Construct( type );

			if ( artifact is MonsterStatuette && StatueTypes.Length > 0 )
			{
				((MonsterStatuette)artifact).Type = StatueTypes[Utility.Random( StatueTypes.Length )];
				((MonsterStatuette)artifact).LootType = LootType.Regular;
			}

			return artifact;
		}

		protected virtual PowerScroll CreateRandomPowerScroll()
		{
			int level;
			double random = Utility.RandomDouble();

			if (0.05 >= random)
				level = 20; // 5%
			else if (0.20 >= random)
				level = 15; // 15%
			else if (0.50 >= random)
				level = 10; // 30%
			else
				level = 5; // 50%

			return PowerScroll.CreateRandomNoCraft( level, level );
		}

		public void GivePowerScrolls()
		{
			if ( Map != Map.Felucca || PSDropCount == 0)
				return;

			List<Mobile> toGive = new List<Mobile>();
			List<DamageStore> rights = GetLootingRights( DamageEntries, HitsMax );

			for ( int i = rights.Count - 1; i >= 0; --i )
			{
				DamageStore ds = rights[i];

				if ( ds.m_HasRight )
					toGive.Add( ds.m_Mobile );
			}

			if ( toGive.Count == 0 )
				return;

			for( int i = 0; i < toGive.Count; i++ )
			{
				Mobile m = toGive[i];

				if( !(m is PlayerMobile) )
					continue;

				bool gainedPath = false;

				int pointsToGain = 800;

				if( VirtueHelper.Award( m, VirtueName.Valor, pointsToGain, ref gainedPath ) )
				{
					if( gainedPath )
						m.SendLocalizedMessage( 1054032 ); // You have gained a path in Valor!
					else
						m.SendLocalizedMessage( 1054030 ); // You have gained in Valor!

					//No delay on Valor gains
				}
			}

			// Randomize
			for ( int i = 0; i < toGive.Count; ++i )
			{
				int rand = Utility.Random( toGive.Count );
				(toGive[i], toGive[rand]) = (toGive[rand], toGive[i]);
			}

			for ( int i = 0; i < m_PSDropCount; ++i )
			{
				Mobile m = toGive[i % toGive.Count];

				PowerScroll ps = CreateRandomPowerScroll();

				if ( m.AccessLevel > AccessLevel.Player )
				{
					ps.ModifiedBy = m.Account.Username;
					ps.ModifiedDate = DateTime.Now;
				}
				GivePowerScrollTo( m, ps );
			}
		}

		public static void GivePowerScrollTo( Mobile m, PowerScroll ps )
		{
			if( ps == null || m == null )	//sanity
				return;

			m.SendLocalizedMessage( 1049524 ); // You have received a scroll of power!

			if( !Core.SE || m.Alive )
				m.AddToBackpack( ps );
			else
			{
				if( m.Corpse != null && !m.Corpse.Deleted )
					m.Corpse.DropItem( ps );
				else
					m.AddToBackpack( ps );
			}
			if(ps.Value > 110d)
				Console.WriteLine($"PS: {ps.Skill.ToString()}_{ps.Value}({ps.Serial}): {m.Name}({m.Serial})");

			if( m is PlayerMobile )
			{
				PlayerMobile pm = (PlayerMobile)m;

				for( int j = 0; j < pm.JusticeProtectors.Count; ++j )
				{
					Mobile prot = pm.JusticeProtectors[j];

					if( prot.Map != m.Map || prot.Kills >= 5 || prot.Criminal || !JusticeVirtue.CheckMapRegion( m, prot ) )
						continue;

					int chance = 0;

					switch( VirtueHelper.GetLevel( prot, VirtueName.Justice ) )
					{
						case VirtueLevel.Seeker: chance = 60; break;
						case VirtueLevel.Follower: chance = 80; break;
						case VirtueLevel.Knight: chance = 100; break;
					}

					if( chance > Utility.Random( 100 ) )
					{
						//PowerScroll powerScroll = new PowerScroll( ps.Skill, ps.Value );
                        int val = (int)ps.Value - 100;
                        PowerScroll powerScroll = PowerScroll.CreateRandomNoCraft(val, val);

						prot.SendLocalizedMessage( 1049368 ); // You have been rewarded for your dedication to Justice!

						if( !Core.SE || prot.Alive )
							prot.AddToBackpack( powerScroll );
						else
						{
							if( prot.Corpse != null && !prot.Corpse.Deleted )
								prot.Corpse.DropItem( powerScroll );
							else
								prot.AddToBackpack( powerScroll );
						}
						if(powerScroll.Value > 110d)
							Console.WriteLine(
								$"PS Prot: {prot.Serial} {prot.Name}: {powerScroll.Value} {powerScroll.Skill.ToString()}");
					}
				}
			}
		}

		public override bool OnBeforeDeath()
		{
			if ( !NoKillAwards )
			{
				GivePowerScrolls();

				if ( NoGoodies )
					return base.OnBeforeDeath();

				Map map = Map;

				if ( map != null )
				{
					int radius = 3;
					int goldStackCount = (int)(Math.PI * radius * radius);
					//for ( int x = -12; x <= 12; ++x )
					for ( int x = -radius; x <= radius; ++x )
					{
						//for ( int y = -12; y <= 12; ++y )
						for ( int y = -radius; y <= radius; ++y )
						{
							double dist = Math.Sqrt(x*x+y*y);

							if ( dist <= radius )
								new GoodiesTimer( map, X + x, Y + y, TotalGoldDrop / goldStackCount ).Start();
						}
					}
				}
			}

			return base.OnBeforeDeath();
		}

		public override void OnDeath( Container c )
		{
			if ( Map == Map.Felucca )
			{
				//TODO: Confirm SE change or AoS one too?
				List<DamageStore> rights = GetLootingRights( DamageEntries, HitsMax );
				List<Mobile> toGive = new List<Mobile>();

				for ( int i = rights.Count - 1; i >= 0; --i )
				{
					DamageStore ds = rights[i];

					if ( ds.m_HasRight )
						toGive.Add( ds.m_Mobile );
				}

				if (SkullType != ChampionSkullType.None)
				{
					if (toGive.Count > 0)
						toGive[Utility.Random(toGive.Count)].AddToBackpack(new ChampionSkull(SkullType));
					else
						c.DropItem(new ChampionSkull(SkullType));
				}
			}

			base.OnDeath( c );
		}

		private class GoodiesTimer : Timer
		{
			private Map m_Map;
			private int m_X, m_Y, m_Gold;

			public GoodiesTimer( Map map, int x, int y, int gold ) : base( TimeSpan.FromSeconds( Utility.RandomDouble() * 5.0 ) )
			{
				m_Map = map;
				m_X = x;
				m_Y = y;
				m_Gold = gold;
			}

			protected override void OnTick()
			{
				int z = m_Map.GetAverageZ( m_X, m_Y );
				bool canFit = m_Map.CanFit( m_X, m_Y, z, 6, false, false );

				for ( int i = -3; !canFit && i <= 3; ++i )
				{
					canFit = m_Map.CanFit( m_X, m_Y, z + i, 6, false, false );

					if ( canFit )
						z += i;
				}

				if ( !canFit )
					return;

				//Gold g = new Gold( 35, 50 );
				Gold g = new Gold( (int)(m_Gold * 0.95), (int)(m_Gold * 1.05));
				
				g.MoveToWorld( new Point3D( m_X, m_Y, z ), m_Map );

				if ( 0.5 >= Utility.RandomDouble() )
				{
					switch ( Utility.Random( 3 ) )
					{
						case 0: // Fire column
						{
							Effects.SendLocationParticles( EffectItem.Create( g.Location, g.Map, EffectItem.DefaultDuration ), 0x3709, 10, 30, 5052 );
							Effects.PlaySound( g, g.Map, 0x208 );

							break;
						}
						case 1: // Explosion
						{
							Effects.SendLocationParticles( EffectItem.Create( g.Location, g.Map, EffectItem.DefaultDuration ), 0x36BD, 20, 10, 5044 );
							Effects.PlaySound( g, g.Map, 0x307 );

							break;
						}
						case 2: // Ball of fire
						{
							Effects.SendLocationParticles( EffectItem.Create( g.Location, g.Map, EffectItem.DefaultDuration ), 0x36FE, 10, 10, 5052 );

							break;
						}
					}
				}
			}
		}
	}
}