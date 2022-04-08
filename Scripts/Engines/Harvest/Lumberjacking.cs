using System;
using System.Collections.Generic;
using Server.Mobiles;
using Server;
using Server.Items;
using Server.Regions;

namespace Server.Engines.Harvest
{
	public class Lumberjacking : HarvestSystem
	{
		private static Lumberjacking m_System;

		public static Lumberjacking System
		{
			get
			{
				if ( m_System == null )
					m_System = new Lumberjacking();

				return m_System;
			}
		}

		private HarvestDefinition m_Wood;

		public HarvestDefinition Definition
		{
			get{ return m_Wood; }
		}

		private Lumberjacking()
		{
			HarvestResource[] res;
			HarvestVein[] veins;

			#region Lumberjacking
			HarvestDefinition lumber = new HarvestDefinition();

            // typ regionu okreslajacy zloza drewna
            lumber.RegionType = typeof(LumberRegion);

			// Resource banks are every 4x3 tiles
			lumber.BankWidth = 4;
			lumber.BankHeight = 3;

			// Ilosc drewna w jednym "drzewie":
			lumber.MinTotal = 20; //20
			lumber.MaxTotal = 40; //45

			// A resource bank will respawn its content every 20 to 30 minutes
			lumber.MinRespawn = TimeSpan.FromMinutes( 20.0 );
			lumber.MaxRespawn = TimeSpan.FromMinutes( 30.0 );

			// Skill checking is done on the Lumberjacking skill
			lumber.Skill = SkillName.Lumberjacking;

			// Set the list of harvestable tiles
			lumber.Tiles = m_TreeTiles;

			// Players must be within 2 tiles to harvest
			lumber.MaxRange = 2;

			// Ilosc wydobywanego jednorazowo surowca (zaleznosc liniowa od skilla)
            lumber.ConsumedPerHarvestMin = 6;
            lumber.ConsumedPerHarvestMax = 6;
            // zombie

			// The chopping effect
            // 24.06.2012 :: zombie :: wydluzenie animacji scinania
			lumber.EffectActions = new int[]{ 13 };
			lumber.EffectSounds = new int[]{ 0x13E };
			lumber.EffectCounts = (Core.AOS ? new int[]{ 3 } : new int[]{ 1, 2, 2, 2, 3 });
			lumber.EffectDelay = TimeSpan.FromSeconds( 1.25 );
			lumber.EffectSoundDelay = TimeSpan.FromSeconds( 0.7 );
            // zombie

			lumber.NoResourcesMessage = 500493; // Nie ma tu juz wiecej uzytecznego drewna.
			lumber.FailMessage = 500495; // Przez chwile rabiesz drzewo, lecz nie udalo ci sie uzyskac uzytecznego materialu.
			lumber.OutOfRangeMessage = 500446; // Jestes za daleko.
			lumber.PackFullMessage = 500497; // Twoj plecak jest przepelniony, nie zmiesci wiecej drewna!
			lumber.ToolBrokeMessage = 500499; // Twoj topor pekl pod sila uderzenia.

			if ( Core.ML )
			{
				res = new HarvestResource[]
				{
					new HarvestResource(  00.0, 00.0, 100.0, 1072540, typeof( Log ), typeof(WoodTreefellow) ),
					new HarvestResource(  65.0, 25.0, 105.0, 1072541, typeof( OakLog ), typeof(OakTreefellow) ),             // Zywiczne
					new HarvestResource(  75.0, 35.0, 115.0, 1072542, typeof( AshLog ), typeof(AshTreefellow) ),             // Puste
					new HarvestResource(  85.0, 45.0, 125.0, 1072543, typeof( YewLog ), typeof(YewTreefellow) ),             // Skamieniale
					new HarvestResource(  95.0, 55.0, 135.0, 1072544, typeof( HeartwoodLog ), typeof(HeartwoodTreefellow) ), // Gietkie
					new HarvestResource( 100.0, 60.0, 140.0, 1072545, typeof( BloodwoodLog ), typeof(BloodwoodTreefellow) ), // Opalone
					new HarvestResource( 100.0, 60.0, 140.0, 1072546, typeof( FrostwoodLog ), typeof(FrostwoodTreefellow) ), // Zmarzniete
				};


				veins = new HarvestVein[]
				{
					new HarvestVein( 80.0, 0.0, res[0], null ),	// Ordinary Logs				(original chance: 75.6)
					new HarvestVein( 07.5, 0.0, res[1], res[0] ), // Oak		Zywiczne		(original chance: 10.0)
					new HarvestVein( 05.2, 0.0, res[2], res[0] ), // Ash		Puste			(original chance: 06.0)
					new HarvestVein( 03.3, 0.0, res[3], res[0] ), // Yew		Skamieniale		(original chance: 03.5)
					new HarvestVein( 02.0, 0.0, res[4], res[0] ), // Heartwood	Gietkie			(original chance: 02.0)
					new HarvestVein( 01.0, 0.0, res[5], res[0] ), // Bloodwood	Opalone			(original chance: 01.0)
					new HarvestVein( 01.0, 0.0, res[6], res[0] ), // Frostwood	Zmarzniete		(original chance: 01.0)
				};

				lumber.BonusResources = new BonusHarvestResource[]
				{
					new BonusHarvestResource( 0, 96.65-4, null, null ),	//Nothing		
					new BonusHarvestResource( 100, 01.50, 1072548, typeof( BarkFragment ) ),
					new BonusHarvestResource( 100, 00.75, 1072550, typeof( LuminescentFungi ) ),
					new BonusHarvestResource( 100, 00.50, 1072547, typeof( SwitchItem ) ),
					new BonusHarvestResource( 100, 00.50, 1072549, typeof( ParasiticPlant ) ),
					new BonusHarvestResource( 100, 00.10, 1072551, typeof( BrilliantAmber ) ),
					new BonusHarvestResource( 0, 4, 1070063 , typeof( SpidersSilk ) ),
				};
			}
			else
			{
				res = new HarvestResource[]
				{
					new HarvestResource( 00.0, 00.0, 100.0, 500498, typeof( Log ) )
				};

				veins = new HarvestVein[]
				{
					new HarvestVein( 100.0, 0.0, res[0], null )
				};
			}

			lumber.Resources = res;
			lumber.Veins = veins;

			/* // Przykladowy region z bardzo kolorowym drewnem:
			lumber.MapRegionVeins = new Dictionary<Map, Dictionary<string, HarvestVein[]>>();
			lumber.MapRegionVeins[Map.Felucca] = new Dictionary<string, HarvestVein[]>();
			lumber.MapRegionVeins[Map.Felucca]["Dziki Port"] = new HarvestVein[]
				{
					new HarvestVein( 58.4, 0.0, res[0], null ),	// Ordinary Logs
					new HarvestVein( 30.0, 0.5, res[1], res[0] ), // Oak		Zywiczne
					new HarvestVein( 10.0, 0.5, res[2], res[0] ), // Ash		Puste
					new HarvestVein( 05.0, 0.5, res[3], res[0] ), // Yew		Skamieniale
					new HarvestVein( 03.0, 0.5, res[4], res[0] ), // Heartwood	Gietkie
					new HarvestVein( 03.0, 0.5, res[5], res[0] ), // Bloodwood	Opalone
					new HarvestVein( 03.0, 0.5, res[6], res[0] ), // Frostwood	Zmarzniete
				};
			 */

			lumber.PlaceAtFeetIfFull = true;

			lumber.RaceBonus = Core.ML;
			lumber.RandomizeVeins = Core.ML;

            m_Wood = lumber;
			Definitions.Add( lumber );
			#endregion
		}

		public override bool CheckHarvest( Mobile from, Item tool )
		{
			if ( !base.CheckHarvest( from, tool ) )
				return false;
            if ( from.Mounted )
            {
                from.SendLocalizedMessage( 1071120 ); // Nie mozesz wykonywac tej czynnosci bedac konno!
                return false;
            }
            else if ( from.IsBodyMod && !from.Body.IsHuman )
            {
                from.SendLocalizedMessage( 1071121 ); // Musisz przybrac ludzka forme, aby moc wykonac ta czynnosc!
                return false;
            }
			if ( tool.Parent != from )
			{
				from.SendLocalizedMessage( 500487 ); // Musisz miec siekiere w rece, zeby scinac drzewo.
				return false;
			}

			return true;
		}

		public override bool CheckHarvest( Mobile from, Item tool, HarvestDefinition def, object toHarvest )
		{
			if ( !base.CheckHarvest( from, tool, def, toHarvest ) )
				return false;

			if ( tool.Parent != from )
			{
				from.SendLocalizedMessage( 500487 ); // Musisz miec siekiere w rece, zeby scinac drzewo.
				return false;
			}

			return true;
		}

        public override HarvestVein MutateVein(Mobile from, Item tool, HarvestDefinition def, HarvestBank bank, object toHarvest, HarvestVein vein)
        {
            if (tool is TreefellowsAxe && def == m_Wood)
            {
                HarvestVein[] veinList;
                def.GetRegionVein(out veinList, bank.Map, bank.X, bank.Y);
                int veinIndex = Array.IndexOf(veinList, vein);

                if (veinIndex >= 0 && veinIndex < (veinList.Length - 1))
                    return veinList[veinIndex + 1];
            }

            return base.MutateVein(from, tool, def, bank, toHarvest, vein);
        }

        public override Type GetResourceType(Mobile from, Item tool, HarvestDefinition def, Map map, Point3D loc, HarvestResource resource)
        {
            //if (def == m_Wood)
            //{
                return resource.Types[0];
            //}

            return base.GetResourceType(from, tool, def, map, loc, resource);
        }

		public override void OnBadHarvestTarget( Mobile from, Item tool, object toHarvest )
		{
			from.SendLocalizedMessage( 500489 ); // Nie mozesz tego zrobic.
		}

        public override bool BeginHarvesting( Mobile from, Item tool )
        {
            if ( !base.BeginHarvesting( from, tool ) )
                return false;

            from.SendLocalizedMessage( 1071122 ); // Co chcesz wyrabac toporem?
            return true;
        }

		public override void OnHarvestStarted( Mobile from, Item tool, HarvestDefinition def, object toHarvest )
		{
			base.OnHarvestStarted( from, tool, def, toHarvest );
			
			if( Core.ML )
				from.RevealingAction();
		}

        private static int[] m_Offsets = new int[]
            {
                -1, -1,
                -1,  0,
                -1,  1,
                 0, -1,
                 0,  1,
                 1, -1,
                 1,  0,
                 1,  1
            };

        public override void OnHarvestFinished(Mobile from, Item tool, HarvestDefinition def, HarvestVein vein, HarvestBank bank, HarvestResource resource, object harvested)
        {
            if (tool is TreefellowsAxe && def == m_Wood && 0.1 > Utility.RandomDouble())
            {
                HarvestResource res = vein.PrimaryResource;

                if (res == resource && res.Types.Length >= 2)
                {
                    try
                    {
                        Map map = from.Map;

                        if (map == null)
                            return;

                        BaseCreature spawned = Activator.CreateInstance(res.Types[1], new object[] { 25 }) as BaseCreature;

                        if (spawned != null)
                        {
                            int offset = Utility.Random(8) * 2;

                            for (int i = 0; i < m_Offsets.Length; i += 2)
                            {
                                int x = from.X + m_Offsets[(offset + i) % m_Offsets.Length];
                                int y = from.Y + m_Offsets[(offset + i + 1) % m_Offsets.Length];

                                if (map.CanSpawnMobile(x, y, from.Z))
                                {
                                    spawned.MoveToWorld(new Point3D(x, y, from.Z), map);
                                    spawned.Combatant = from;
                                    return;
                                }
                                else
                                {
                                    int z = map.GetAverageZ(x, y);

                                    if (map.CanSpawnMobile(x, y, z))
                                    {
                                        spawned.MoveToWorld(new Point3D(x, y, z), map);
                                        spawned.Combatant = from;
                                        return;
                                    }
                                }
                            }

                            spawned.MoveToWorld(from.Location, from.Map);
                            spawned.Combatant = from;
                        }
                    }
                    catch
                    {
                    }
                }
            }
        }

		public static void Initialize()
		{
			Array.Sort( m_TreeTiles );
		}

		#region Tile lists
		private static int[] m_TreeTiles = new int[]
			{
				0x4CCA, 0x4CCB, 0x4CCC, 0x4CCD, 0x4CD0, 0x4CD3, 0x4CD6, 0x4CD8,
				0x4CDA, 0x4CDD, 0x4CE0, 0x4CE3, 0x4CE6, 0x4CF8, 0x4CFB, 0x4CFE,
				0x4D01, 0x4D41, 0x4D42, 0x4D43, 0x4D44, 0x4D57, 0x4D58, 0x4D59,
				0x4D5A, 0x4D5B, 0x4D6E, 0x4D6F, 0x4D70, 0x4D71, 0x4D72, 0x4D84,
				0x4D85, 0x4D86, 0x52B5, 0x52B6, 0x52B7, 0x52B8, 0x52B9, 0x52BA,
				0x52BB, 0x52BC, 0x52BD,

				0x4CCE, 0x4CCF, 0x4CD1, 0x4CD2, 0x4CD4, 0x4CD5, 0x4CD7, 0x4CD9,
				0x4CDB, 0x4CDC, 0x4CDE, 0x4CDF, 0x4CE1, 0x4CE2, 0x4CE4, 0x4CE5,
				0x4CE7, 0x4CE8, 0x4CF9, 0x4CFA, 0x4CFC, 0x4CFD, 0x4CFF, 0x4D00,
				0x4D02, 0x4D03, 0x4D45, 0x4D46, 0x4D47, 0x4D48, 0x4D49, 0x4D4A,
				0x4D4B, 0x4D4C, 0x4D4D, 0x4D4E, 0x4D4F, 0x4D50, 0x4D51, 0x4D52,
				0x4D53, 0x4D5C, 0x4D5D, 0x4D5E, 0x4D5F, 0x4D60, 0x4D61, 0x4D62,
				0x4D63, 0x4D64, 0x4D65, 0x4D66, 0x4D67, 0x4D68, 0x4D69, 0x4D73,
				0x4D74, 0x4D75, 0x4D76, 0x4D77, 0x4D78, 0x4D79, 0x4D7A, 0x4D7B,
				0x4D7C, 0x4D7D, 0x4D7E, 0x4D7F, 0x4D87, 0x4D88, 0x4D89, 0x4D8A,
				0x4D8B, 0x4D8C, 0x4D8D, 0x4D8E, 0x4D8F, 0x4D90, 0x4D95, 0x4D96,
				0x4D97, 0x4D99, 0x4D9A, 0x4D9B, 0x4D9D, 0x4D9E, 0x4D9F, 0x4DA1,
				0x4DA2, 0x4DA3, 0x4DA5, 0x4DA6, 0x4DA7, 0x4DA9, 0x4DAA, 0x4DAB,
				0x52BE, 0x52BF, 0x52C0, 0x52C1, 0x52C2, 0x52C3, 0x52C4, 0x52C5,
				0x52C6, 0x52C7
			};
		#endregion
	}
}