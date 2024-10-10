using System;
using System.Collections;
using Server;
using Server.Commands;
using Server.Engines.XmlSpawner2;
using Ultima;

namespace Server.Items
{
	public class StealableArtifactsSpawner : Item
	{
		public class StealableEntry
		{
			private Spot[] m_Spots;
			private StealableType[] m_Types;

			private Map m_Map;
			private Point3D m_Location;
			private Type m_Type;

			private int m_MinDelay;
			private int m_MaxDelay;
			private int m_Hue;

			public Spot[] Locations { get { return m_Spots; } }
			public StealableType[] Types { get { return m_Types; } }

			public Map Map { get { return m_Map; } }
			public Point3D Spot { get { return m_Location; } }
			public Type Type { get { return m_Type; } }

			public int MinDelay { get { return m_MinDelay; } }
			public int MaxDelay { get { return m_MaxDelay; } }
			public int Hue { get { return m_Hue; } }

			public StealableEntry(Spot[] spots, int minDelay, int maxDelay, StealableType[] types)
			{
				m_Spots = spots;
				m_Types = types;

				m_MinDelay = minDelay;
				m_MaxDelay = maxDelay;
			}

			public StealableEntry(Map map, Point3D location, int minDelay, int maxDelay, Type type) : this(map, location, minDelay, maxDelay, type, 0)
			{
			}

			public StealableEntry(Map map, Point3D location, int minDelay, int maxDelay, Type type, int hue)
			{
				m_Map = map;
				m_Location = location;
				m_Type = type;

				m_MinDelay = minDelay;
				m_MaxDelay = maxDelay;
				m_Hue = hue;
			}

			public Item CreateInstance()
			{
				Type type = m_Type;
				int amount = 1;
				if (m_Types != null && m_Types.Length > 0)
				{
					var typeInfo = Utility.RandomList(m_Types);
					type = typeInfo.Type;
					amount = typeInfo.Amount;
				}

				Map map = m_Map;
				Point3D location = m_Location;
				if (m_Spots != null && m_Spots.Length > 0)
				{
					var spot = Utility.RandomList(m_Spots);
					map = spot.Map;
					location = spot.Point;
				}

				Item item = (Item)Activator.CreateInstance(type);

				if (m_Hue > 0)
					item.Hue = m_Hue;

				item.Amount = amount;

				item.Movable = false;
				item.MoveToWorld(location, map);

				return item;
			}
		}

		public class Spot
		{
			private Map m_Map;
			private Point3D m_Point;
			public Map Map { get { return m_Map; } }
			public Point3D Point { get { return m_Point; } }

			public Spot(Map map, Point3D point)
			{
				m_Map = map;
				m_Point = point;
			}
		}

		public class StealableType
		{
			private Type m_Type;
			private int m_Amount;
			public Type Type { get { return m_Type; } }
			public int Amount { get { return m_Amount; } }

			public StealableType(Type type, int amount = 1)
			{
				m_Type = type;
				m_Amount = amount;
			}

		}

		private static Spot[] m_DungeonsA = new Spot[]
		{
			new Spot(Map.Felucca, new Point3D(2660, 1939, 0)),	// minotaury
			new Spot(Map.Felucca, new Point3D(5333, 1149, 0)),	// wulkan
			new Spot(Map.Felucca, new Point3D(6038, 81, 4)),	// garth
			new Spot(Map.Felucca, new Point3D(665, 1476, 45)),	// alcala
			new Spot(Map.Felucca, new Point3D(1668, 994, 20)),	// elbrind
			new Spot(Map.Felucca, new Point3D(5721, 474, 4)),	// barad-dur
			new Spot(Map.Felucca, new Point3D(5647, 1835, 0)),	// hall torech

			//new Spot(Map.Felucca, new Point3D(6915, 62, 0)),	// pokraczna bestia
			//new Spot(Map.Felucca, new Point3D(6635, 77, 1)),	// blyskow
			new Spot(Map.Malas, new Point3D(131, 1174, 0)),	// pokraczna bestia
			new Spot(Map.Malas, new Point3D(259, 765, 1)),	// blyskow

			//new Spot(Map.Felucca, new Point3D(6197, 46, -1)),	// elghinn
			//new Spot(Map.Malas, new Point3D(53, 46, -1)),	// elghinn
		};

		private static StealableType[] m_ResourcesA = new StealableType[]
		{
			new StealableType(typeof(PowderOfTranslocation), 10),
			new StealableType(typeof(PowerScrollPowder), 80),
			new StealableType(typeof(ArtefaktowyPyl), 100),
			new StealableType(typeof(PowderForTinkering)),
			new StealableType(typeof(PowderForWood)),
			new StealableType(typeof(BasePigment)),
			new StealableType(typeof(ValoriteIngot), 250),
			new StealableType(typeof(VeriteIngot), 300),
			new StealableType(typeof(BowstringFiresteed), 8),
			new StealableType(typeof(BowstringUnicorn), 8),
			new StealableType(typeof(BowstringNightmare), 8),
			new StealableType(typeof(BowstringKirin), 8),
		};


		private static StealableEntry[] m_Entries = new StealableEntry[]
			{
				new StealableEntry( m_DungeonsA, 60*24 * 9, 60*24 * 11, m_ResourcesA ),   // co ~10 dni
				new StealableEntry( m_DungeonsA, 60*24 * 19, 60*24 * 21, m_ResourcesA ),  // co ~20 dni
				new StealableEntry( m_DungeonsA, 60*24 * 29, 60*24 * 31, m_ResourcesA ),  // co ~30 dni

				// Customowe Arty - Felucca & Malas Dungi				
				new StealableEntry( Map.Felucca, new Point3D( 5302,  1648, 0 ), 18432, 27648, typeof( PrzekleteFeyLeggings ) ), //Saew
				new StealableEntry( Map.Felucca, new Point3D( 1547,  798, 0 ), 18432, 27648, typeof( PrzekletySoulSeeker ) ), // Elbrind
				new StealableEntry( Map.Felucca, new Point3D( 5702,  418, 4 ), 18432, 27648, typeof( PrzekletyMieczeAmrIbnLuhajj ) ), // Barad
				new StealableEntry( Map.Felucca, new Point3D( 5614,  683, 5 ), 18432, 27648, typeof( PrzekletaVioletCourage ) ), // Ophidianie
				new StealableEntry( Map.Malas, new Point3D( 5807,  211, 50 ), 18432, 27648, typeof( PrzekletyArcticDeathDealer) ),  // Ulnhyr Orbben
				new StealableEntry( Map.Felucca, new Point3D( 6004, 2551, 0 ), 18432, 27648, typeof( PrzekleteSongWovenMantle ) ), // Podmrok mrowki


				// Doom - Artifact rarity 1
				new StealableEntry( Map.Malas, new Point3D( 317,  56, -1 ), 72, 108, typeof( RockArtifact ) ), // doom
				new StealableEntry( Map.Malas, new Point3D( 360,  31,  8 ), 72, 108, typeof( SkullCandleArtifact ) ), // doom
				new StealableEntry( Map.Malas, new Point3D( 369, 372, -1 ), 72, 108, typeof( BottleArtifact ) ), // doom
				new StealableEntry( Map.Malas, new Point3D( 378, 372,  0 ), 72, 108, typeof( DamagedBooksArtifact ) ), // doom
				// Doom - Artifact rarity 2
				new StealableEntry( Map.Malas, new Point3D( 432,  16, -1 ), 144, 216, typeof( StretchedHideArtifact ) ), // doom
				new StealableEntry( Map.Malas, new Point3D( 489,   9,  0 ), 144, 216, typeof( BrazierArtifact ) ), // doom
				// Doom - Artifact rarity 3
				new StealableEntry( Map.Malas, new Point3D( 471,  96, -1 ), 288, 432, typeof( LampPostArtifact ), GetLampPostHue() ), // doom
				new StealableEntry( Map.Malas, new Point3D( 421, 198,  2 ), 288, 432, typeof( BooksNorthArtifact ) ), // doom
				new StealableEntry( Map.Malas, new Point3D( 431, 189, -1 ), 288, 432, typeof( BooksWestArtifact ) ), // doom
				new StealableEntry( Map.Malas, new Point3D( 435, 196, -1 ), 288, 432, typeof( BooksFaceDownArtifact ) ), // doom
				// Doom - Artifact rarity 5
				new StealableEntry( Map.Malas, new Point3D( 447,   9,  8 ), 1152, 1728, typeof( StuddedLeggingsArtifact ) ), // doom
				new StealableEntry( Map.Malas, new Point3D( 423,  28,  0 ), 1152, 1728, typeof( EggCaseArtifact ) ), // doom
				new StealableEntry( Map.Malas, new Point3D( 347,  44,  4 ), 1152, 1728, typeof( SkinnedGoatArtifact ) ), // doom
				new StealableEntry( Map.Malas, new Point3D( 497,  57, -1 ), 1152, 1728, typeof( GruesomeStandardArtifact ) ), //doom
				new StealableEntry( Map.Malas, new Point3D( 381, 375, 11 ), 1152, 1728, typeof( BloodyWaterArtifact ) ), // doom
				new StealableEntry( Map.Malas, new Point3D( 489, 369,  2 ), 1152, 1728, typeof( TarotCardsArtifact ) ), // doom
				new StealableEntry( Map.Malas, new Point3D( 497, 369,  5 ), 1152, 1728, typeof( BackpackArtifact ) ), // doom
				// Doom - Artifact rarity 7
				new StealableEntry( Map.Malas, new Point3D( 475,  23,  4 ), 4608, 6912, typeof( StuddedTunicArtifact ) ), // doom
				new StealableEntry( Map.Malas, new Point3D( 423,  28,  0 ), 4608, 6912, typeof( CocoonArtifact ) ), // doom
				// Doom - Artifact rarity 8
				new StealableEntry( Map.Malas, new Point3D( 354,  36, -1 ), 9216, 13824, typeof( SkinnedDeerArtifact ) ), // doom
				// Doom - Artifact rarity 9
				new StealableEntry( Map.Malas, new Point3D( 433,  11, -1 ), 18432, 27648, typeof( SaddleArtifact ) ), // doom
				new StealableEntry( Map.Malas, new Point3D( 403,  31,  4 ), 18432, 27648, typeof( LeatherTunicArtifact ) ), // doom
				// Doom - Artifact rarity 10
				new StealableEntry( Map.Malas, new Point3D( 257,  70, -2 ), 36864, 55296, typeof( ZyronicClaw ) ), // doom
				new StealableEntry( Map.Malas, new Point3D( 354, 176,  7 ), 36864, 55296, typeof( TitansHammer ) ), // doom
				new StealableEntry( Map.Malas, new Point3D( 369, 389, -1 ), 36864, 55296, typeof( BladeOfTheRighteous ) ), // doom
				new StealableEntry( Map.Malas, new Point3D( 469,  96,  5 ), 36864, 55296, typeof( InquisitorsResolution ) ), // doom
				// Doom - Artifact rarity 12
				new StealableEntry( Map.Malas, new Point3D( 487, 364, -1 ), 147456, 221184, typeof( RuinedPaintingArtifact ) ), // doom

				// Yomotsu Mines - Artifact rarity 1
				new StealableEntry( Map.Felucca, new Point3D(  5446, 503, 10 ), 72, 108, typeof( Basket1Artifact ) ), // Kanały Tasandora
				new StealableEntry( Map.Felucca, new Point3D(  5580, 496, 5 ), 72, 108, typeof( Basket2Artifact ) ), // Kanały Tasandora
				// Yomotsu Mines - Artifact rarity 2
				new StealableEntry( Map.Felucca, new Point3D(  5469,  572, 5 ), 144, 216, typeof( Basket4Artifact ) ), // Kanały Tasandora
				new StealableEntry( Map.Felucca, new Point3D(   5445,  590, 5 ), 144, 216, typeof( Basket5NorthArtifact ) ), // Kanały Tasandora
				new StealableEntry( Map.Felucca, new Point3D(  5405,  648,  -25 ), 144, 216, typeof( Basket5WestArtifact ) ), // Kanały Tasandora
				// Yomotsu Mines - Artifact rarity 3
				new StealableEntry( Map.Felucca, new Point3D( 5389,   731, -25 ), 288, 432, typeof( Urn1Artifact ) ), // Kanały Tasandora
				new StealableEntry( Map.Felucca, new Point3D(  5494,  699, -26 ), 288, 432, typeof( Urn2Artifact ) ),
				new StealableEntry( Map.Felucca, new Point3D( 5299,  810, 4 ), 288, 432, typeof( Sculpture1Artifact ) ), // Hurengrav Lochy
				new StealableEntry( Map.Felucca, new Point3D( 1523,  818, 0 ), 288, 432, typeof( Sculpture2Artifact ) ), // Elbrind
				new StealableEntry( Map.Felucca, new Point3D( 1545,  797, 40 ), 288, 432, typeof( TeapotNorthArtifact ) ), // Elbrind
				new StealableEntry( Map.Felucca, new Point3D( 1633, 835, 15 ), 288, 432, typeof( TeapotWestArtifact ) ), // Elbrind
				new StealableEntry( Map.Felucca, new Point3D(  1685,  840, 20 ), 288, 432, typeof( TowerLanternArtifact ) ), // Elbrind
				// Yomotsu Mines - Artifact rarity 9
				new StealableEntry( Map.Felucca, new Point3D(  5355,   1695, 0 ), 18432, 27648, typeof( ManStatuetteSouthArtifact ) ), // Seaw

				// Fan Dancer's Dojo - Artifact rarity 1
				new StealableEntry( Map.Felucca, new Point3D( 5447, 1704, 0 ), 72, 108, typeof( Basket3NorthArtifact ) ), // Mrówki
				new StealableEntry( Map.Felucca, new Point3D( 5460, 2073, 0 ), 72, 108, typeof( Basket3WestArtifact ) ), // Smoczy Dung
				// Fan Dancer's Dojo - Artifact rarity 2
				new StealableEntry( Map.Felucca, new Point3D(  5473, 1963, 5 ), 144, 216, typeof( Basket6Artifact ) ), // Smoczy Dung
				new StealableEntry( Map.Felucca, new Point3D( 5496, 1964, 5 ), 144, 216, typeof( ZenRock1Artifact ) ), // Smoczy Dung
				// Fan Dancer's Dojo - Artifact rarity 3
				new StealableEntry( Map.Felucca, new Point3D(  5923, 2387, 0 ), 288, 432, typeof( FanNorthArtifact ) ), // Podmrok
				new StealableEntry( Map.Felucca, new Point3D(  5686, 2444, 48 ), 288, 432, typeof( FanWestArtifact ) ), // Podmrok
				new StealableEntry( Map.Felucca, new Point3D(  5686, 2460, 42 ), 288, 432, typeof( BowlsVerticalArtifact ) ), // Podmrok
				new StealableEntry( Map.Felucca, new Point3D(  5695, 3114, 5 ), 288, 432, typeof( ZenRock2Artifact ) ), // Podmrok
				new StealableEntry( Map.Felucca, new Point3D( 5492, 2936, 35 ), 288, 432, typeof( ZenRock3Artifact ) ), // Podmrok
				// Fan Dancer's Dojo - Artifact rarity 4
				new StealableEntry( Map.Felucca, new Point3D( 5574, 2054,  0 ), 576, 864, typeof( Painting1NorthArtifact ) ), // Smoczy
				new StealableEntry( Map.Felucca, new Point3D(  5795, 1896,  5 ), 576, 864, typeof( Painting1WestArtifact ) ), // Lodowy
				new StealableEntry( Map.Felucca, new Point3D(  5164, 2181,  25 ), 576, 864, typeof( Painting2NorthArtifact ) ), // Krysztalowe Smoki
				new StealableEntry( Map.Felucca, new Point3D(  5659, 1714,  0 ), 576, 864, typeof( Painting2WestArtifact ) ), // Hall Torech
				new StealableEntry( Map.Felucca, new Point3D( 5774, 1717, 0 ), 576, 864, typeof( TripleFanNorthArtifact ) ), // Hall Torech
				new StealableEntry( Map.Felucca, new Point3D(  5238, 1786, 13 ), 576, 864, typeof( TripleFanWestArtifact ) ), // Tyr Reviaren
				new StealableEntry( Map.Felucca, new Point3D( 5728, 1808, 0 ), 576, 864, typeof( BowlArtifact ) ), // Hall Torech
				new StealableEntry( Map.Felucca, new Point3D(  5893, 1093, 0 ), 576, 864, typeof( CupsArtifact ) ), // Świątynia Matki
				new StealableEntry( Map.Felucca, new Point3D( 5404, 1783, 0 ), 576, 864, typeof( BowlsHorizontalArtifact ) ), // Loen Torech
				new StealableEntry( Map.Felucca, new Point3D( 5900, 876, 4 ), 576, 864, typeof( SakeArtifact ) ), // Róża
				// Fan Dancer's Dojo - Artifact rarity 5
				new StealableEntry( Map.Felucca, new Point3D( 5358, 1666, 0 ), 1152, 1728, typeof( SwordDisplay1NorthArtifact ) ), // Saew
				new StealableEntry( Map.Felucca, new Point3D(  5325, 1454, 0 ), 1152, 1728, typeof( SwordDisplay1WestArtifact ) ), //  Swiatynia Smierci Tas
				new StealableEntry( Map.Felucca, new Point3D( 5270, 1410, 0 ), 1152, 1728, typeof( Painting3Artifact ) ), // Wulkan
				// Fan Dancer's Dojo - Artifact rarity 6
				new StealableEntry( Map.Felucca, new Point3D(  5538, 1467, 0 ), 2304, 3456, typeof( Painting4NorthArtifact ) ), // Podziemia Twierdzy
				new StealableEntry( Map.Felucca, new Point3D(  5883, 1177, 4 ), 2304, 3456, typeof( Painting4WestArtifact ) ), // Swiatynia Matki
				new StealableEntry( Map.Felucca, new Point3D( 5822, 1177, 0 ), 2304, 3456, typeof( SwordDisplay2NorthArtifact ) ), // Swiatynia Matki
				new StealableEntry( Map.Felucca, new Point3D( 5676, 741, 0 ), 2304, 3456, typeof( SwordDisplay2WestArtifact ) ), // Lochy Ophidian
				// Fan Dancer's Dojo - Artifact rarity 7
				new StealableEntry( Map.Felucca, new Point3D(  5315, 1786, 0 ), 4608, 6912, typeof( FlowersArtifact ) ),
				// Fan Dancer's Dojo - Artifact rarity 8
				new StealableEntry( Map.Felucca, new Point3D(  5379, 726, -25 ), 9216, 13824, typeof( DolphinLeftArtifact ) ), // Kanaly Tasandora
				new StealableEntry( Map.Felucca, new Point3D( 5384, 328, 5 ), 9216, 13824, typeof( DolphinRightArtifact ) ), // Cmentarz Tasandora
				new StealableEntry( Map.Felucca, new Point3D(  5238, 361    ,  0 ), 9216, 13824, typeof( SwordDisplay3SouthArtifact ) ), // Kopalnia Tasandora
				new StealableEntry( Map.Felucca, new Point3D( 5321, 56, 0 ), 9216, 13824, typeof( SwordDisplay3EastArtifact ) ), // Komnaty Bialego Wilka
				new StealableEntry( Map.Malas, new Point3D( 162, 647, -1 ), 9216, 13824, typeof( SwordDisplay4WestArtifact ) ),
				new StealableEntry( Map.Malas, new Point3D( 124, 624,  0 ), 9216, 13824, typeof( Painting5NorthArtifact ) ),
				new StealableEntry( Map.Malas, new Point3D( 146, 649,  2 ), 9216, 13824, typeof( Painting5WestArtifact ) ),
				// Fan Dancer's Dojo - Artifact rarity 9
				new StealableEntry( Map.Malas, new Point3D( 100, 488, -1 ), 18432, 27648, typeof( SwordDisplay4NorthArtifact ) ),
				new StealableEntry( Map.Malas, new Point3D( 175, 606,  0 ), 18432, 27648, typeof( SwordDisplay5NorthArtifact ) ),
				new StealableEntry( Map.Malas, new Point3D( 157, 608, -1 ), 18432, 27648, typeof( SwordDisplay5WestArtifact ) ),
				new StealableEntry( Map.Malas, new Point3D( 187, 643,  1 ), 18432, 27648, typeof( Painting6NorthArtifact ) ),
				new StealableEntry( Map.Malas, new Point3D( 146, 623,  1 ), 18432, 27648, typeof( Painting6WestArtifact ) ),
				new StealableEntry( Map.Malas, new Point3D( 178, 629, -1 ), 18432, 27648, typeof( ManStatuetteEastArtifact ) )
			};

		public static StealableEntry[] Entries { get { return m_Entries; } }

		private static Type[] m_TypesOfEntries = null;
		public static Type[] TypesOfEntires
		{
			get
			{
				if (m_TypesOfEntries == null)
				{
					m_TypesOfEntries = new Type[m_Entries.Length];

					for (int i = 0; i < m_Entries.Length; i++)
						m_TypesOfEntries[i] = m_Entries[i].Type;
				}

				return m_TypesOfEntries;
			}
		}

		private static StealableArtifactsSpawner m_Instance;

		public static StealableArtifactsSpawner Instance { get { return m_Instance; } }

		private static int GetLampPostHue()
		{
			if (0.9 > Utility.RandomDouble())
				return 0;

			return Utility.RandomList(0x455, 0x47E, 0x482, 0x486, 0x48F, 0x4F2, 0x58C, 0x66C);
		}


		public static void Initialize()
		{
			CommandSystem.Register("GenStealArties", AccessLevel.Administrator, new CommandEventHandler(GenStealArties_OnCommand));
			CommandSystem.Register("RemoveStealArties", AccessLevel.Administrator, new CommandEventHandler(RemoveStealArties_OnCommand));
		}

		[Usage("GenStealArties")]
		[Description("Generates the stealable artifacts spawner.")]
		private static void GenStealArties_OnCommand(CommandEventArgs args)
		{
			Mobile from = args.Mobile;

			if (Create())
				from.SendMessage("Stealable artifacts spawner generated.");
			else
				from.SendMessage("Stealable artifacts spawner already present.");
		}

		[Usage("RemoveStealArties")]
		[Description("Removes the stealable artifacts spawner and every not yet stolen stealable artifacts.")]
		private static void RemoveStealArties_OnCommand(CommandEventArgs args)
		{
			Mobile from = args.Mobile;

			if (Remove())
				from.SendMessage("Stealable artifacts spawner removed.");
			else
				from.SendMessage("Stealable artifacts spawner not present.");
		}

		public static bool Create()
		{
			if (m_Instance != null && !m_Instance.Deleted)
				return false;

			m_Instance = new StealableArtifactsSpawner();
			return true;
		}

		public static bool Remove()
		{
			if (m_Instance == null)
				return false;

			m_Instance.Delete();
			m_Instance = null;
			return true;
		}

		public static StealableInstance GetStealableInstance(Item item)
		{
			if (Instance == null)
				return null;

			return (StealableInstance)Instance.m_Table[item];
		}


		public class StealableInstance
		{
			private StealableEntry m_Entry;
			private Item m_Item;
			private DateTime m_NextRespawn;

			public StealableEntry Entry { get { return m_Entry; } }

			public Item Item
			{
				get { return m_Item; }
				set
				{
					if (m_Item != null && value == null)
					{
						int delay = Utility.RandomMinMax(this.Entry.MinDelay, this.Entry.MaxDelay);
						this.NextRespawn = DateTime.Now + TimeSpan.FromMinutes(delay);
					}

					if (Instance != null)
					{
						if (m_Item != null)
							Instance.m_Table.Remove(m_Item);

						if (value != null)
							Instance.m_Table[value] = this;
					}

					m_Item = value;
				}
			}

			public DateTime NextRespawn
			{
				get { return m_NextRespawn; }
				set { m_NextRespawn = value; }
			}

			public StealableInstance(StealableEntry entry) : this(entry, null, DateTime.Now)
			{
			}

			public StealableInstance(StealableEntry entry, Item item, DateTime nextRespawn)
			{
				m_Item = item;
				m_NextRespawn = nextRespawn;
				m_Entry = entry;
			}

			public void CheckRespawn()
			{
				if (this.Item != null && (this.Item.Deleted || this.Item.Movable || this.Item.Parent != null))
					this.Item = null;

				if (this.Item == null && DateTime.Now >= this.NextRespawn)
				{
					this.Item = this.Entry.CreateInstance();
				}
			}
		}

		private Timer m_RespawnTimer;
		private StealableInstance[] m_Artifacts;
		private Hashtable m_Table;

		public override string DefaultName
		{
			get { return "Stealable Artifacts Spawner - Internal"; }
		}

		private StealableArtifactsSpawner() : base(1)
		{
			Movable = false;

			m_Artifacts = new StealableInstance[m_Entries.Length];
			m_Table = new Hashtable(m_Entries.Length);

			for (int i = 0; i < m_Entries.Length; i++)
			{
				m_Artifacts[i] = new StealableInstance(m_Entries[i]);
			}

			m_RespawnTimer = Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromMinutes(15.0), new TimerCallback(CheckRespawn));
		}

		public override void OnDelete()
		{
			base.OnDelete();

			if (m_RespawnTimer != null)
			{
				m_RespawnTimer.Stop();
				m_RespawnTimer = null;
			}

			foreach (StealableInstance si in m_Artifacts)
			{
				if (si.Item != null)
					si.Item.Delete();
			}

			m_Instance = null;
		}

		public void CheckRespawn()
		{
			foreach (StealableInstance si in m_Artifacts)
			{
				si.CheckRespawn();
			}
		}

		public StealableArtifactsSpawner(Serial serial) : base(serial)
		{
			m_Instance = this;
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.WriteEncodedInt(0); // version

			writer.WriteEncodedInt(m_Artifacts.Length);

			for (int i = 0; i < m_Artifacts.Length; i++)
			{
				StealableInstance si = m_Artifacts[i];

				writer.Write((Item)si.Item);
				writer.WriteDeltaTime((DateTime)si.NextRespawn);
			}
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadEncodedInt();

			m_Artifacts = new StealableInstance[m_Entries.Length];
			m_Table = new Hashtable(m_Entries.Length);

			int length = reader.ReadEncodedInt();

			for (int i = 0; i < length; i++)
			{
				Item item = reader.ReadItem();
				DateTime nextRespawn = reader.ReadDeltaTime();

				if (i < m_Artifacts.Length)
				{
					StealableInstance si = new StealableInstance(m_Entries[i], item, nextRespawn);
					m_Artifacts[i] = si;

					if (si.Item != null)
						m_Table[si.Item] = si;
				}
			}

			for (int i = length; i < m_Entries.Length; i++)
			{
				m_Artifacts[i] = new StealableInstance(m_Entries[i]);
			}

			m_RespawnTimer = Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromMinutes(15.0), new TimerCallback(CheckRespawn));
		}
	}
}