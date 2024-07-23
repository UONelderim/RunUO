using System;
using System.Collections.Generic;
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
		Glade,
	}

	public class ChampionSpawnInfo
	{
		private string m_Name;
		private Type m_Champion;
		private Type[][] m_SpawnTypes;
		private string[] m_LevelNames;
		private double m_SpawnScalar;

		public string Name => m_Name;
		public Type Champion => m_Champion;
		public Type[][] SpawnTypes => m_SpawnTypes;
		public string[] LevelNames => m_LevelNames;
		public double SpawnScalar => m_SpawnScalar;

		public ChampionSpawnInfo( string name, Type champion, string[] levelNames, Type[][] spawnTypes, double spawnScalar )
		{
			m_Name = name;
			m_Champion = champion;
			m_LevelNames = levelNames;
			m_SpawnTypes = spawnTypes;
			m_SpawnScalar = spawnScalar;
		}

		public static Dictionary<ChampionSpawnType, ChampionSpawnInfo> Table => m_Table;

		private static readonly Dictionary<ChampionSpawnType, ChampionSpawnInfo> m_Table = new()
			{
				{
					ChampionSpawnType.Abyss, new ChampionSpawnInfo("Otchlani",
						typeof(Semidar),
						new[] { "Wrog", "Zabojca", "Najezdzca" },
						new[]
						{
							new[] { typeof(StrongMongbat), typeof(Imp) },
							new[] { typeof(Gargoyle), typeof(Harpy) },
							new[] { typeof(FireGargoyle), typeof(StoneGargoyle) },
							new[] { typeof(CommonDaemon), typeof(Succubus) }
						},
						0.9)
				},
				{
					ChampionSpawnType.Arachnid, new ChampionSpawnInfo("Pajeczakow",
						typeof(Mephitis),
						new[] { "Pogromca", "Zabojca", "Anihilator" },
						new[]
						{
							new[] { typeof(Scorpion), typeof(GiantSpider) },
							new[] { typeof(TerathanDrone), typeof(TerathanWarrior) },
							new[] { typeof(DreadSpider), typeof(TerathanMatriarch) },
							new[] { typeof(PoisonElemental), typeof(TerathanAvenger) }
						},
						0.9)
				},
				{
					ChampionSpawnType.ColdBlood, new ChampionSpawnInfo("Gadow",
						typeof(Rikktor),
						new[] { "Nienawidzi", "Pogromca", "Niszczyciel" },
						new[]
						{
							new[] { typeof(Lizardman), typeof(Snake) },
							new[] { typeof(LavaLizard), typeof(OphidianWarrior) },
							new[] { typeof(Drake), typeof(OphidianArchmage) },
							new[] { typeof(Dragon), typeof(OphidianKnight) }
						},
						0.9)
				},
				{
					ChampionSpawnType.ForestLord, new ChampionSpawnInfo("Wladcy Lasu",
						typeof(LordOaks),
						new[] { "Wrog", "Klatwa", "Pogromca" },
						new[]
						{
							new[] { typeof(Pixie), typeof(ShadowWisp) },
							new[] { typeof(Kirin), typeof(Wisp) },
							new[] { typeof(Centaur), typeof(Unicorn) },
							new[] { typeof(EtherealWarrior), typeof(SerpentineDragon) }
						},
						0.8)
				},
				{
					ChampionSpawnType.VerminHorde, new ChampionSpawnInfo("Hordy Szkodnikow",
						typeof(Barracoon),
						new[] { "Adwersarz", "Ujarzmiciel", "Pogromca" },
						new[]
						{
							new[] { typeof(GiantRat), typeof(BarakSlime) },
							new[] { typeof(DireWolf), typeof(Ratman) },
							new[] { typeof(HellHound), typeof(RatmanMage) },
							new[] { typeof(RatmanArcher), typeof(SilverSerpent) }
						},
						1.0)
				},
				{
					ChampionSpawnType.UnholyTerror, new ChampionSpawnInfo("Terroru zza Grobu",
						typeof(Neira),
						new[] { "Pogromca", "Pomsta", "Nemesis" },
						new[] 
						{

							new[] { typeof(Bogle), typeof(Ghoul), typeof(Wraith) },
							new[] { typeof(BoneMagi), typeof(Mummy) },
							new[] { typeof(BoneKnight), typeof(Lich) },
							new[] { typeof(LichLord), typeof(RottingCorpse) }
						},
						0.9)
				},
				{
					ChampionSpawnType.SleepingDragon, new ChampionSpawnInfo("Śpiący Smok",
						typeof(Serado),
						new[] { "Rywal", "Pogromca", "Antagonista" },
						new[]
						{
							new[] { typeof(DeathwatchBeetleHatchling), typeof(Lizardman) },
							new[] { typeof(DeathwatchBeetle), typeof(Kappa) },
							new[] { typeof(LesserHiryu), typeof(RevenantLion) },
							new[] { typeof(Hiryu), typeof(Oni) }
						},
						0.5)
				},
				{
					ChampionSpawnType.Glade, new ChampionSpawnInfo("Polany",
						typeof(Twaulo),
						new[] { "Niszczyciel", "Tepiciel", "Plaga" },
						new[]
						{
							new[] { typeof(Pixie), typeof(ShadowWisp) },
							new[] { typeof(Centaur), typeof(MLDryad) },
							new[] { typeof(Satyr), typeof(CuSidhe) },
							new[] { typeof(FrostwoodTreefellow), typeof(RagingGrizzlyBear) }
						},
						0.5)
				}
			};

		public static ChampionSpawnInfo GetInfo( ChampionSpawnType type )
		{
			if (Table.TryGetValue(type, out var info))
			{
				return info;
			}
			
			Console.WriteLine($"Unable to get ChampionSpawnInfo for {type}");
			return Table[ChampionSpawnType.Abyss];
		}
	}
}