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
		MeraktusTheTormented = 100,
		Pyre,
		Morena,
		OrcCommander
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
				},
				{
					ChampionSpawnType.MeraktusTheTormented, new ChampionSpawnInfo("Minotaur",
						typeof(Meraktus),
						new[] { "Pogromca", "Pomsta", "Nemesis" },
						new[]
						{
							new[] { typeof(Minotaur), typeof(ShadowWisp) },
							new[] { typeof(NPrzeklety), typeof(MinotaurCaptain) },
							new[] { typeof(NZapomniany), typeof(MinotaurMage) },
							new[] { typeof(SilverSerpent), typeof(MinotaurLord) }
						},
						0.7)
				},
				{
					ChampionSpawnType.Pyre, new ChampionSpawnInfo("Ogniste Ptaszysko",
						typeof(Pyre),
						new[] { "Rywal", "Pogromca", "Antagonista" },
						new[]
						{
							new[] { typeof(FireElemental), typeof(OgnistyWojownik), typeof(OgnistyNiewolnik) },
							new[] { typeof(DullCopperElemental), typeof(FireGargoyle), typeof(GargoyleEnforcer) },
							new[] { typeof(EnslavedGargoyle), typeof(OgnistySmok), typeof(FireBeetle) },
							new[] { typeof(FireSteed), typeof(PrastaryOgnistySmok), typeof(feniks) }
						},
						0.6)
				},
				{
					ChampionSpawnType.Morena, new ChampionSpawnInfo("Morena",
						typeof(MorenaAwatar),
						new[] { "Rywal", "Pogromca", "Antagonista" },
						new[]
						{
							new[] { typeof(Ghoul), typeof(Skeleton), typeof(PatchworkSkeleton) },
							new[] { typeof(WailingBanshee), typeof(BoneMagi), typeof(BoneKnight) },
							new[] { typeof(LichLord), typeof(FleshGolem), typeof(Mummy2) },
							new[] { typeof(SkeletalDragon), typeof(RottingCorpse), typeof(AncientLich) }
						},
						0.6)
				},
				{
					ChampionSpawnType.OrcCommander, new ChampionSpawnInfo("Kapitan Legionu Orkow",
						typeof(KapitanIIILegionuOrkow),
						new[] { "Rywal", "Pogromca", "Antagonista" },
						new[]
						{
							new[] { typeof(Orc), typeof(Ratman), typeof(Goblin) },
							new[] { typeof(OrcishMage), typeof(LesserGoblinSapper), typeof(Troll) },
							new[] { typeof(JukaWarrior), typeof(OrcCaptain), typeof(TrollLord) },
							new[] { typeof(JukaMage), typeof(OrcBomber), typeof(OgreLord) }
						},
						0.9)
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