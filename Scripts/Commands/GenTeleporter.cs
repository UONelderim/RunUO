using System;
using System.Collections;
using Server;
using Server.Items;

namespace Server.Commands
{
	public class GenTeleporter
	{
		public GenTeleporter()
		{
		}

		public static void Initialize()
		{
			CommandSystem.Register( "TelGen", AccessLevel.Administrator, new CommandEventHandler( GenTeleporter_OnCommand ) );
		}

		[Usage( "TelGen" )]
		[Description( "Generates world/dungeon teleporters for all facets." )]
		public static void GenTeleporter_OnCommand( CommandEventArgs e )
		{
			e.Mobile.SendMessage( "Generating teleporters, please wait." );

			int count = new TeleportersCreator().CreateTeleporters();

			count += new SHTeleporter.SHTeleporterCreator().CreateSHTeleporters();

			e.Mobile.SendMessage( "Teleporter generating complete. {0} teleporters were generated.", count );
		}

		public class TeleportersCreator
		{
			private int m_Count;
			
			public TeleportersCreator()
			{
			}

			private static Queue m_Queue = new Queue();

			public static bool FindTeleporter( Map map, Point3D p )
			{
				IPooledEnumerable eable = map.GetItemsInRange( p, 0 );

				foreach ( Item item in eable )
				{
					if ( item is Teleporter && !(item is KeywordTeleporter) && !(item is SkillTeleporter) )
					{
						int delta = item.Z - p.Z;

						if ( delta >= -12 && delta <= 12 )
							m_Queue.Enqueue( item );
					}
				}

				eable.Free();

				while ( m_Queue.Count > 0 )
					((Item)m_Queue.Dequeue()).Delete();

				return false;
			}

			public void CreateTeleporter( Point3D pointLocation, Point3D pointDestination, Map mapLocation, Map mapDestination, bool back )
			{
				if ( !FindTeleporter( mapLocation, pointLocation ) )
				{
					m_Count++;
				
					Teleporter tel = new Teleporter( pointDestination, mapDestination );

					tel.MoveToWorld( pointLocation, mapLocation );
				}

				if ( back && !FindTeleporter( mapDestination, pointDestination ) )
				{
					m_Count++;

					Teleporter telBack = new Teleporter( pointLocation, mapLocation );

					telBack.MoveToWorld( pointDestination, mapDestination );
				}
			}

			public void CreateTeleporter( int xLoc, int yLoc, int zLoc, int xDest, int yDest, int zDest, Map map, bool back )
			{
				CreateTeleporter( new Point3D( xLoc, yLoc, zLoc ), new Point3D( xDest, yDest, zDest ), map, map, back);
			}

			public void CreateTeleporter( int xLoc, int yLoc, int zLoc, int xDest, int yDest, int zDest, Map mapLocation, Map mapDestination, bool back )
			{
				CreateTeleporter( new Point3D( xLoc, yLoc, zLoc ), new Point3D( xDest, yDest, zDest ), mapLocation, mapDestination, back);
			}

			public void DestroyTeleporter( int x, int y, int z, Map map )
			{
				Point3D p = new Point3D( x, y, z );
				IPooledEnumerable eable = map.GetItemsInRange( p, 0 );

				foreach ( Item item in eable )
				{
					if ( item is Teleporter && !(item is KeywordTeleporter) && !(item is SkillTeleporter) && item.Z == p.Z )
						m_Queue.Enqueue( item );
				}

				eable.Free();

				while ( m_Queue.Count > 0 )
					((Item)m_Queue.Dequeue()).Delete();
			}

			public void CreateTeleportersMap( Map map )
			{
			//blank
			}

			public void CreateTeleportersMap2( Map map )
			{
				/*
				// Update: add new ones
				/*
				for ( int i = 0; i < 4; ++i )
				{
					CreateTeleporter( 650 + i, 1297, -58, 626 + i, 1526, -28, map, false );
					CreateTeleporter( 626 + i, 1527, -28, 650 + i, 1298, i == 1 ? -59 : -58, map, false );
				}
				*/

			}

			public void CreateTeleportersMap3( Map map )
			{

				//Yomotsu Mines Exit
				/*
				CreateTeleporter( 3, 128, -1, 259, 785, 64, map, Map.Tokuno, false );
				CreateTeleporter( 4, 128, -1, 259, 785, 64, map, Map.Tokuno, false );
				CreateTeleporter( 5, 128, -1, 259, 785, 64, map, Map.Tokuno, false  );
				CreateTeleporter( 6, 128, -1, 259, 785, 64, map, Map.Tokuno, false );
				CreateTeleporter( 7, 128, -1, 259, 785, 64, map, Map.Tokuno, false );
				CreateTeleporter( 8, 128, -1, 259, 785, 64, map, Map.Tokuno, false );

				//Fan Dancer Exit
				CreateTeleporter( 64, 336, 11, 983, 195, 24, map, Map.Tokuno, false );
				CreateTeleporter( 64, 337, 11, 983, 195, 24, map, Map.Tokuno, false );
				CreateTeleporter( 64, 338, 11, 983, 195, 24, map, Map.Tokuno, false );
				CreateTeleporter( 64, 339, 11, 983, 195, 24, map, Map.Tokuno, false );

				// Dungeon Bedlam
				CreateTeleporter(  84, 1673, -2, 156, 1613, 0, map, false );
				CreateTeleporter( 156, 1609, 17,  87, 1673, 0, map, false );
				CreateTeleporter( 157, 1609, 17,  87, 1673, 0, map, false );
				*/
			}

			public void CreateTeleportersMap4( Map map )
			{
				//Yomotso Mines Entrance
				/*
				CreateTeleporter( 257, 783, 63, 5, 128, -1, map, Map.Malas, false );
				CreateTeleporter( 258, 783, 63, 5, 128, -1, map, Map.Malas, false );
				CreateTeleporter( 259, 783, 63, 5, 128, -1, map, Map.Malas, false );
				CreateTeleporter( 260, 783, 63, 5, 128, -1, map, Map.Malas, false );

				//Fan dancer Entrance
				CreateTeleporter( 988, 194, 15, 67, 337, -1, map, Map.Malas, false );
				CreateTeleporter( 988, 195, 15, 67, 337, -1, map, Map.Malas, false );
				CreateTeleporter( 987, 196, 15, 67, 337, -1, map, Map.Malas, false );
				CreateTeleporter( 988, 197, 18, 67, 337, -1, map, Map.Malas, false );
				*/

			}
			public void CreateTeleportersTrammel( Map map )
			{
				// Haven
				//CreateTeleporter( 3632, 2566, 0, 3632, 2566, 20, map, true );
			}

			public void CreateTeleportersFelucca( Map map )
			{
				//Kopalnie
				CreateTeleporter( 1313, 2473, 0, 5629, 499, 0, map, true );
				CreateTeleporter( 1314, 2473, 0, 5630, 499, 0, map, true );
				CreateTeleporter( 3356, 2645, 0, 5270, 264, 0, map, true );
				CreateTeleporter( 3357, 2645, 0, 5271, 264, 0, map, true );
				CreateTeleporter( 2821, 885, 0, 5350, 879, 0, map, true );
				CreateTeleporter( 2821, 886, 0, 5350, 880, 0, map, true );	
				CreateTeleporter( 4167, 1463, 0, 5286, 895, 0, map, true );
				CreateTeleporter( 4168, 1463, 0, 5287, 895, 0, map, true );
				CreateTeleporter( 4717, 2226, 0, 5317, 2412, 4, map, true );
				CreateTeleporter( 4717, 2227, 0, 5317, 2413, 4, map, true );
				CreateTeleporter( 4717, 2228, 0, 5317, 2414, 4, map, true );
				//Zagroda_Malluan
				CreateTeleporter( 3364, 2640, 1, 5848, 595, 0, map, true );				
				CreateTeleporter( 3365, 2640, 1, 5849, 595, 0, map, true );	
				//Zagroda_Bedwyrgard
				CreateTeleporter( 1325, 2469, 0, 5904, 596, 0, map, true );				
				CreateTeleporter( 1326, 2469, 0, 5905, 596, 0, map, true );					
				//SpinedDungeony
				CreateTeleporter( 3334, 2880, 0, 5372, 842, 0, map, true );
				CreateTeleporter( 3335, 2880, 0, 5373, 842, 0, map, true );
				CreateTeleporter( 4341, 1455, 0, 5123, 824, 0, map, true );
				CreateTeleporter( 4342, 1455, 0, 5124, 824, 0, map, true );
				CreateTeleporter( 2975, 1006, 0, 5274, 964, 0, map, true );
				CreateTeleporter( 2976, 1006, 0, 5275, 964, 0, map, true );
				//Kanaly
				CreateTeleporter( 3188, 3419, 0, 5904, 341, 20, map, true );
				CreateTeleporter( 3189, 3419, 0, 5905, 341, 20, map, true );
				//Bagienne_Jaskinie
				CreateTeleporter( 3381, 3057, 10, 3389, 3018, 0, map, true );
				CreateTeleporter( 3381, 3058, 10, 3389, 3019, 0, map, true );
				CreateTeleporter( 3446, 2973, 0, 3414, 3113, 0, map, true );
				CreateTeleporter( 3447, 2973, 0, 3415, 3113, 0, map, true );
				CreateTeleporter( 3467, 3048, 0, 3519, 2992, 0, map, true );
				CreateTeleporter( 3467, 3049, 0, 3519, 2993, 0, map, true );
				CreateTeleporter( 3590, 2939, 0, 5437, 233, 0, map, true );
				CreateTeleporter( 3590, 2940, 0, 5437, 234, 0, map, true );				
				//Krysztalowy
				CreateTeleporter( 3350, 3087, 0, 5239, 520, 0, map, true );
				CreateTeleporter( 3351, 3087, 0, 5240, 520, 0, map, true );
				CreateTeleporter( 5471, 471, 1, 5493, 440, 0, map, true );
				//Jaskinia2l
				CreateTeleporter( 3473, 2932, 0, 5387, 3646, 0, map, true );
				CreateTeleporter( 3474, 2932, 0, 5388, 3646, 0, map, true );
				CreateTeleporter( 5128, 3601, 0, 5787, 3209, 17, map, true );
				CreateTeleporter( 5128, 3602, 0, 5787, 3210, 17, map, true );
				CreateTeleporter( 5747, 3502, -20, 6129, 3259, 17, map, true );
				//Orc cave 2lvle
				CreateTeleporter( 2881, 1301, 0, 5136, 217, 0, map, true );
				CreateTeleporter( 2882, 1301, 0, 5137, 217, 0, map, true );
				CreateTeleporter( 5232, 109, -25, 5270, 163, 0, map, true );				
				//Piramida
				CreateTeleporter( 4034, 1660, 15, 5175, 774, 0, map, true );
				CreateTeleporter( 4035, 1660, 15, 5176, 774, 0, map, true );
				CreateTeleporter( 6056, 267, 0, 5943, 2204, 5, map, true );
				CreateTeleporter( 6057, 267, 0, 5944, 2204, 5, map, true );
				CreateTeleporter( 6058, 267, 0, 5945, 2204, 5, map, true );
				//Pustynna_Twierdza_Tilki
				CreateTeleporter( 3835, 1615, 0, 5324, 2541, 35, map, true );
				CreateTeleporter( 3836, 1615, 0, 5325, 2541, 35, map, true );
				//Norstyg_08
				CreateTeleporter( 1507, 2518, 0, 5228, 426, 0, map, true );		
				//Kosciany_09 polaczenie Rewian-Joram
				CreateTeleporter( 5127, 3424, -1, 5196, 523, 61, map, true );
				CreateTeleporter( 5128, 3424, 1, 5197, 523, 61, map, true );
				CreateTeleporter( 5129, 3424, 1, 5198, 523, 61, map, true );
				CreateTeleporter( 5130, 3424, -1, 5199, 523, 61, map, true );
				CreateTeleporter( 5271, 3399, 15, 3592, 2776, 0, map, true );
				CreateTeleporter( 5272, 3399, 15, 3593, 2776, 0, map, true );
				CreateTeleporter( 5280, 3880, 0, 5151, 586, 26, map, true );
				CreateTeleporter( 5280, 3881, 0, 5151, 587, 26, map, true );
				CreateTeleporter( 4376, 1294, 1, 5378, 3936, 0, map, true );
				CreateTeleporter( 4376, 1295, 1, 5378, 3937, 0, map, true );
				//Pajaki_10
				CreateTeleporter( 2968, 884, 0, 5389, 609, 37, map, true );	
				CreateTeleporter( 2968, 884, 0, 5388, 609, 39, map, true );				
				//Norstyg-to-Darkelot
				CreateTeleporter( 1013, 2705, 0, 5341, 2218, 0, map, true );
				CreateTeleporter( 1014, 2705, 0, 5342, 2218, 0, map, true );
				CreateTeleporter( 5174, 3904, 0, 5319, 2114, 30, map, true );
				CreateTeleporter( 5175, 3904, 0, 5320, 2114, 30, map, true );
				CreateTeleporter( 4661, 538, 10, 5170, 4047, 0, map, true );
				CreateTeleporter( 4662, 538, 10, 5171, 4047, 0, map, true );				
				//Wulkan
				CreateTeleporter( 3929, 624, 10, 5596, 368, 9, map, true );
				CreateTeleporter( 3930, 624, 10, 5597, 368, 9, map, true );
				CreateTeleporter( 5570, 286, -13, 5860, 929, 11, map, true );				
				CreateTeleporter( 5570, 285, -13, 5860, 928, 11, map, true );
				CreateTeleporter( 5570, 284, -13, 5860, 927, 11, map, true );
				CreateTeleporter( 5886, 639, -8, 6051, 834, 16, map, true );				
				CreateTeleporter( 5887, 639, -9, 6052, 834, 14, map, true );
				CreateTeleporter( 5888, 639, -7, 6053, 834, 14, map, true );
				CreateTeleporter( 5889, 639, -7, 6054, 834, 16, map, true );
				CreateTeleporter( 5890, 639, -6, 6055, 834, 16, map, true );
				CreateTeleporter( 5400, 2302, 9, 6006, 593, -10, map, true );
				CreateTeleporter( 5400, 2303, 8, 6006, 594, -10, map, true );
				CreateTeleporter( 5400, 2304, 8, 6006, 595, -10, map, true );
				CreateTeleporter( 5400, 2305, 10, 6006, 596, -10, map, true );
				CreateTeleporter( 5400, 2306, 9, 6006, 597, -10, map, true );
				//Klejnotowe smoki
				CreateTeleporter( 3970, 558, 20, 5772, 3150, 2, map, true );
				CreateTeleporter( 3971, 558, 20, 5773, 3150, 1, map, true );
				CreateTeleporter( 3972, 558, 22, 5774, 3150, 2, map, true );
				CreateTeleporter( 5659, 3053, -22, 5672, 2940, 20, map, true );
				CreateTeleporter( 5660, 3053, -22, 5673, 2940, 20, map, true );
				CreateTeleporter( 5661, 3053, -22, 5674, 2940, 20, map, true );
				CreateTeleporter( 5662, 3053, -22, 5675, 2940, 20, map, true );
				CreateTeleporter( 5683, 2951, -27, 5663, 3127, 20, map, true );
				CreateTeleporter( 5683, 2952, -27, 5663, 3128, 20, map, true );
				CreateTeleporter( 5683, 2953, -27, 5663, 3129, 20, map, true );
				CreateTeleporter( 5683, 2954, -27, 5663, 3130, 20, map, true );
				//2lvle standard big
				CreateTeleporter( 5221, 342, 5, 5504, 299, -5, map, true );				
				CreateTeleporter( 4276, 930, 0, 5131, 250, 5, map, true );
				//Blue wejscie ma byc od S w dungu
				CreateTeleporter( 1735, 3001, 0, 5207, 958, 18, map, true );
				CreateTeleporter( 1736, 3001, 0, 5208, 958, 18, map, true );
				//Spiders 
				CreateTeleporter( 3435, 3349, 0, 5568, 583, 5, map, true );
				CreateTeleporter( 3436, 3349, 0, 5569, 583, 5, map, true );
				CreateTeleporter( 5550, 672, -21, 5632, 415, 21, map, true );		
				//RubbishDung
				CreateTeleporter( 2627, 2033, 0, 5675, 257, 80, map, true );
				CreateTeleporter( 2627, 2034, 0, 5675, 258, 80, map, true );
				CreateTeleporter( 5719, 260, 5, 5268, 200, 30, map, true );
				CreateTeleporter( 5720, 260, 5, 5268, 200, 30,  map, true );
				//UD_Wschodni_4lvl
				CreateTeleporter( 3770, 2847, 0, 5490, 2376, 0, map, true );
				CreateTeleporter( 3771, 2847, 0, 5491, 2376, 0, map, true );
				CreateTeleporter( 5519, 2325, -20, 5607, 2266, 20, map, true );
				CreateTeleporter( 5520, 2325, -20, 5608, 2266, 20, map, true );
				CreateTeleporter( 5521, 2325, -20, 5609, 2266, 20, map, true );
				CreateTeleporter( 5649, 2315, 5, 5440, 2386, 5, map, true );
				CreateTeleporter( 5521, 2501, -40, 5608, 2385, 20, map, true );
				CreateTeleporter( 5522, 2501, -40, 5609, 2385, 20, map, true );
				//Tower
                CreateTeleporter( 5264, 708, 17, 3399, 369, 2, map, true );
				CreateTeleporter( 5263, 708, 17, 3398, 369, 2, map, true );
				CreateTeleporter( 5262, 708, 17, 3397, 369, 2, map, true );				
				//Jaskina StareHuren
				CreateTeleporter( 1702, 2637, 0, 5346, 621, 52, map, true );
				CreateTeleporter( 1703, 2637, 0, 5347, 621, 52, map, true );
				//PodwodnaJaskinia
				CreateTeleporter( 1980, 1922, 0, 5432, 751, 96, map, true );
				//OldNelderim Ophidians
				CreateTeleporter( 3308, 974, 0, 5233, 2288, 22, map, true );
				CreateTeleporter( 3309, 974, 0, 5234, 2288, 22, map, true );
				CreateTeleporter( 3310, 974, 0, 5235, 2288, 22, map, true );	
				//Lodowy_podziemia
				CreateTeleporter( 1583, 2442, 0, 5225, 2578, 100, map, true );				
				//WodnyZLabiryntem
				CreateTeleporter( 5, 2, 1, 5666, 2116, 1, map, true );
				CreateTeleporter( 5, 2, 1, 5667, 2116, 1, map, true );
				CreateTeleporter( 5729, 2200, 1, 5731, 2186, 0, map, true );
				CreateTeleporter( 5642, 2196, 1, 5755, 2188, 0, map, true );				
				CreateTeleporter( 5739, 2126, 12, 5775, 2210, 1, map, true );
				CreateTeleporter( 5795, 2184, 1, 5773, 2183, 1, map, true );
				CreateTeleporter( 5747, 2150, 1, 5816, 2182, 1, map, true );
				//Dung_zamek_norstyg_4lvl
				CreateTeleporter( 1107, 2822, 10, 5582, 6, 69, map, true );
				CreateTeleporter( 1108, 2822, 10, 5583, 6, 69, map, true );
				CreateTeleporter( 5675, 93, 2, 5595, 184, 39, map, true );
				CreateTeleporter( 5675, 94, 2, 5595, 185, 39, map, true );
				CreateTeleporter( 5675, 95, 2, 5595, 186, 39, map, true );
				CreateTeleporter( 5642, 130, 24, 5724, 26, 0, map, true );
				CreateTeleporter( 5642, 131, 24, 5724, 27, 0, map, true );
				CreateTeleporter( 5642, 132, 24, 5724, 28, 0, map, true );
				CreateTeleporter( 5789, 45, 46, 5736, 127, -5, map, true );
				CreateTeleporter( 5789, 46, 46, 5736, 128, -5, map, true );
				CreateTeleporter( 5789, 47, 46, 5736, 129, -5, map, true );
				CreateTeleporter( 5698, 175, 2, 5696, 199, 66, map, true );
				CreateTeleporter( 5698, 176, 2, 5696, 200, 66, map, true );
				CreateTeleporter( 5698, 177, 10, 5696, 201, 66, map, true );
				//Barad
				CreateTeleporter( 6058, 2310, -28, 4112, 1100, 0, map, true );
				CreateTeleporter( 6059, 2310, -28, 4113, 1100, 0, map, true );
				CreateTeleporter( 6000, 2226, -46, 6104, 2427, -13, map, true );
				CreateTeleporter( 6000, 2227, -46, 6104, 2428, -13, map, true );
				CreateTeleporter( 6000, 2228, -46, 6104, 2429, -13, map, true );
				//Dung
				CreateTeleporter( 3580, 3091, 0, 5382, 2184, 25, map, true );
				CreateTeleporter( 3581, 3091, 0, 5383, 2184, 25, map, true );
				//Jaskinia_jezioro_Norstyg
				CreateTeleporter( 1787, 3060, 0, 5288, 3761, -2, map, true );
				CreateTeleporter( 1788, 3060, 0, 5289, 3761, -2, map, true );
				//Jezioro_lawy
				CreateTeleporter( 1965, 2500, 0, 5796, 2569, 0, map, true );
				//Ogniste_Smoki
				CreateTeleporter( 1214, 2747, 10, 5987, 3824, 0, map, true );
				CreateTeleporter( 5930, 3864, 0, 5930, 3873, 0, map, true );
				CreateTeleporter( 5931, 3864, 0, 5931, 3873, 0, map, true );
				CreateTeleporter( 5932, 3864, 0, 5932, 3873, 0, map, true );
				CreateTeleporter( 5821, 3756, 5, 5748, 3838, 10, map, true );
				//Przejscia_Podmrok_Powierzchnia
				CreateTeleporter( 3324, 3062, 0, 5847, 1968, 0, map, true );
				CreateTeleporter( 3324, 3061, 0, 5847, 1967, 0, map, true );
				CreateTeleporter( 3324, 3060, 0, 5847, 1966, 0, map, true );
				CreateTeleporter( 3324, 3059, 0, 5847, 1965, 0, map, true );
				CreateTeleporter( 1536, 2766, 0, 5305, 1914, 5, map, true );	
				CreateTeleporter( 1536, 2765, 0, 5305, 1913, 5, map, true );	
				CreateTeleporter( 3043, 1146, 0, 5854, 1283, 0, map, true );
				CreateTeleporter( 3044, 1146, 0, 5855, 1283, 0, map, true );	
				CreateTeleporter( 3045, 1146, 0, 5856, 1283, 0, map, true );	
				CreateTeleporter( 3046, 1146, 0, 5857, 1283, 0, map, true );
				CreateTeleporter( 4536, 1128, 20, 6104, 1372, 0, map, true );	
				CreateTeleporter( 4537, 1128, 20, 6105, 1372, 0, map, true );	
				CreateTeleporter( 4538, 1128, 20, 6106, 1372, 0, map, true );
				CreateTeleporter( 5606, 1647, 0, 2448, 1990, 0, map, true );
				CreateTeleporter( 5606, 1648, 0, 2448, 1991, 0, map, true );
				
			}

			public int CreateTeleporters()
			{
				CreateTeleportersMap( Map.Felucca );
				CreateTeleportersMap( Map.Trammel );
				CreateTeleportersTrammel( Map.Trammel );
				CreateTeleportersFelucca( Map.Felucca );
				CreateTeleportersMap2( Map.Ilshenar );
				CreateTeleportersMap3( Map.Malas );
				CreateTeleportersMap4( Map.Tokuno );
				return m_Count;
			}
		}
	}
}
