using System;
using Server;
using Server.Mobiles;

namespace Server.Engines.CannedEvil
{
	public enum ChampionSpawnType
	{
		Abyss,
		Arachnid,
		ColdBlood,
		ForestLord,
		VerminHorde,
		UnholyTerror,
		SleepingDragon,
		Minotaur,
		OrcCommander,
		Morena,
		Pyre,
		Glade
		
	}

	public class ChampionSpawnInfo
	{
		private string m_Name;
		private Type m_Champion;
		private Type[][] m_SpawnTypes;
		private string[] m_LevelNames;
		private double m_SpawnScalar;

		public string Name { get { return m_Name; } }
		public Type Champion { get { return m_Champion; } }
		public Type[][] SpawnTypes { get { return m_SpawnTypes; } }
		public string[] LevelNames { get { return m_LevelNames; } }
		public double SpawnScalar { get { return m_SpawnScalar; } }

		public ChampionSpawnInfo( string name, Type champion, string[] levelNames, Type[][] spawnTypes, double spawnScalar )
		{
			m_Name = name;
			m_Champion = champion;
			m_LevelNames = levelNames;
			m_SpawnTypes = spawnTypes;
			m_SpawnScalar = spawnScalar;
		}

		public static ChampionSpawnInfo[] Table{ get { return m_Table; } }

		private static readonly ChampionSpawnInfo[] m_Table = new ChampionSpawnInfo[]
			{
				new ChampionSpawnInfo( "Otchlani", typeof( Semidar ), new string[]{ "Wrog", "Zabojca", "Najezdzca" }, new Type[][]	// Abyss
				{																											// Abyss
					new Type[]{ typeof( StrongMongbat ), typeof( Imp ) },													// Level 1
					new Type[]{ typeof( Gargoyle ), typeof( Harpy ) },														// Level 2
					new Type[]{ typeof( FireGargoyle ), typeof( StoneGargoyle ) },											// Level 3
					new Type[]{ typeof( CommonDaemon ), typeof( Succubus ) }														// Level 4
				}, 0.9 ),
				new ChampionSpawnInfo( "Pajeczakow", typeof( Mephitis ), new string[]{ "Pogromca", "Zabojca", "Anihilator" }, new Type[][]	// Arachnid
				{																											// Arachnid
					new Type[]{ typeof( Scorpion ), typeof( GiantSpider ) },												// Level 1
					new Type[]{ typeof( TerathanDrone ), typeof( TerathanWarrior ) },										// Level 2
					new Type[]{ typeof( DreadSpider ), typeof( TerathanMatriarch ) },										// Level 3
					new Type[]{ typeof( PoisonElemental ), typeof( TerathanAvenger ) }										// Level 4
				}, 0.9 ),
				new ChampionSpawnInfo( "Gadow", typeof( Rikktor ), new string[]{ "Nienawidzi", "Pogromca", "Niszczyciel" }, new Type[][]	// Cold Blood
				{																											// Cold Blood
					new Type[]{ typeof( Lizardman ), typeof( Snake ) },														// Level 1
					new Type[]{ typeof( LavaLizard ), typeof( OphidianWarrior ) },											// Level 2
					new Type[]{ typeof( Drake ), typeof( OphidianArchmage ) },												// Level 3
					new Type[]{ typeof( Dragon ), typeof( OphidianKnight ) }												// Level 4
				}, 0.9 ),
				new ChampionSpawnInfo( "Wladcy Lasu", typeof( LordOaks ), new string[]{ "Wrog", "Klatwa", "Pogromca" }, new Type[][]	// Forest Lord
				{																											// Forest Lord
					new Type[]{ typeof( Pixie ), typeof( ShadowWisp ) },													// Level 1
					new Type[]{ typeof( Kirin ), typeof( Wisp ) },															// Level 2
					new Type[]{ typeof( Centaur ), typeof( Unicorn ) },														// Level 3
					new Type[]{ typeof( EtherealWarrior ), typeof( SerpentineDragon ) }										// Level 4
				}, 0.8 ),
				new ChampionSpawnInfo( "Hordy Szkodnikow", typeof( Barracoon ), new string[]{ "Adwersarz", "Ujarzmiciel", "Pogromca" }, new Type[][]	// Vermin Horde
				{																											// Vermin Horde
					new Type[]{ typeof( GiantRat ), typeof( BarakSlime ) },														// Level 1
					new Type[]{ typeof( DireWolf ), typeof( Ratman ) },														// Level 2
					new Type[]{ typeof( HellHound ), typeof( RatmanMage ) },												// Level 3
					new Type[]{ typeof( RatmanArcher ), typeof( SilverSerpent ) }											// Level 4
				}, 1.0 ),
				new ChampionSpawnInfo( "Terroru zza Grobu", typeof( Neira ), new string[]{ "Pogromca", "Pomsta", "Nemesis" }, new Type[][]	// Unholy Terror
				{   																										// Unholy Terror

                    new Type[]{ typeof( Bogle ), typeof( Ghoul ), typeof( Wraith ) },                                       // Level 1
					new Type[]{ typeof( BoneMagi ), typeof( Mummy ) },			 					                        // Level 2
					new Type[]{ typeof( BoneKnight ), typeof( Lich ) },							                            // Level 3
					new Type[]{ typeof( LichLord ), typeof( RottingCorpse ) }												// Level 4
				}, 0.9 ),
				new ChampionSpawnInfo( "Śpiący Smok", typeof( Serado ), new string[]{ "Rywal", "Pogromca", "Antagonista" } , new Type[][]
				{																											// Sleeping Dragon
					new Type[]{ typeof( DeathwatchBeetleHatchling ), typeof( Lizardman ) },
					new Type[]{ typeof( DeathwatchBeetle ), typeof( Kappa ) },
					new Type[]{ typeof( LesserHiryu ), typeof( RevenantLion ) },
					new Type[]{ typeof( Hiryu ), typeof( Oni ) }
				}, 0.5 ),
				new ChampionSpawnInfo("Polany", typeof(Twaulo), new[] { "Niszczyciel", "Tepiciel", "Plaga" }, new Type[][] // Glade
				{ 
					new[] { typeof(Pixie), typeof(ShadowWisp) },
					new[] { typeof(Centaur), typeof(MLDryad) },
					new[] { typeof(Satyr), typeof(CuSidhe) },
					new[] { typeof(FrostwoodTreefellow), typeof(RagingGrizzlyBear) }
				}, 0.5 ),
				
				new ChampionSpawnInfo("Minotaur", typeof(Meraktus), new[] { "Pogromca", "Pomsta", "Nemesis" }, new Type[][] // Minotaur
				{ 
					new[] { typeof(Minotaur), typeof(ShadowWisp) },
					new[] { typeof(NPrzeklety), typeof(MinotaurCaptain) },
					new[] { typeof(NZapomniany), typeof(MinotaurMage) },
					new[] { typeof(SilverSerpent), typeof(MinotaurLord) }
				}, 0.7 ),
				new ChampionSpawnInfo("Ogniste Ptaszysko", typeof(Pyre), new[] { "Rywal", "Pogromca", "Antagonista" }, new Type[][] // Pyre
				{ 
					new[] { typeof(FireElemental), typeof(OgnistyWojownik), typeof(OgnistyNiewolnik) },
					new[] { typeof(DullCopperElemental), typeof(FireGargoyle), typeof(GargoyleEnforcer)},
					new[] { typeof(EnslavedGargoyle), typeof(OgnistySmok), typeof(FireBeetle) },
					new[] { typeof(FireSteed), typeof(PrastaryOgnistySmok), typeof(feniks) }
				}, 0.6 ),
				new ChampionSpawnInfo("Morena", typeof(MorenaAwatar), new[] { "Rywal", "Pogromca", "Antagonista" }, new Type[][] // Morena
				{ 
					new[] { typeof(Ghoul), typeof(Skeleton), typeof(PatchworkSkeleton) },
					new[] { typeof(WailingBanshee), typeof(BoneMagi), typeof(BoneKnight)},
					new[] { typeof(LichLord), typeof(FleshGolem), typeof(Mummy2) },
					new[] { typeof(SkeletalDragon), typeof(RottingCorpse), typeof(AncientLich) }
				}, 0.6 ),
				new ChampionSpawnInfo("Kapitan Legionu Orkow", typeof(KapitanIIILegionuOrkow), new[] { "Rywal", "Pogromca", "Antagonista" }, new Type[][] // Morena
				{ 
					new[] { typeof(Orc), typeof(Ratman), typeof(Goblin) },
					new[] { typeof(OrcishMage), typeof(LesserGoblinSapper), typeof(Troll)},
					new[] { typeof(JukaWarrior), typeof(OrcCaptain), typeof(TrollLord) },
					new[] { typeof(JukaMage), typeof(OrcBomber), typeof(OgreLord) }
				}, 0.9 )
			};

		public static ChampionSpawnInfo GetInfo( ChampionSpawnType type )
		{
			int v = (int)type;

			if( v < 0 || v >= m_Table.Length )
				v = 0;

			return m_Table[v];
		}
	}
}