using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Server.Mobiles;
using Server.Commands;
using Server.Items;
using System.Text.Json;

namespace Server.Nelderim
{
    public class NelderimRegionSystem
    {
        private static string BaseDir = Path.Combine(Core.BaseDirectory, "Data", "NelderimRegions");
        private static string XmlPath = Path.Combine(BaseDir, "NelderimRegions.xml");
        private static string JsonPath = Path.Combine(BaseDir, "NelderimRegions.json");

        internal static Dictionary<string, NelderimRegion> NelderimRegions = new();
        internal static Dictionary<string, NelderimGuardProfile> GuardProfiles = new();

        public static void Initialize()
        {
            Load();
            CommandSystem.Register("RELoad", AccessLevel.Administrator, _ => Load());
            CommandSystem.Register("RESave", AccessLevel.Administrator, _ => Save());
            RumorsSystem.Load();
        }

        private static void Load()
        {
            NelderimRegions.Clear();
            Console.Write("NelderimRegions: Loading...");
            try
            {
                if (File.Exists(Path.Combine(BaseDir, "NelderimRegions.xml")))
                {
                    LoadXml();
                    Save();
                    // File.Delete(Path.Combine(BaseDir, "NelderimRegions.xml")); //TODO: uncomment me once working
                }
                else
                {
                    var regions = JsonSerializer.Deserialize<HashSet<NelderimRegion>>(JsonPath);
                    foreach (var nelderimRegion in regions)
                    {
                        NelderimRegions.Add(nelderimRegion.Name, nelderimRegion);
                    }
                }
            }
            catch (Exception e){
                Console.WriteLine("NelderimRegions: Error!");
                Console.WriteLine(e.ToString());
                return;
            }
            Console.WriteLine("NelderimRegions: Loaded.");
        }

        private static void LoadXml()
        {
            var doc = new XmlDocument();
            doc.Load(XmlPath);

            var root = doc["NelderimRegions"];

            foreach (XmlElement reg in root.GetElementsByTagName("region"))
            {
                var newRegion = new NelderimRegion();
                newRegion.Name = reg.GetAttribute("name");
                newRegion.Parent = reg.GetAttribute("parent");

                var oreveins = reg.GetElementsByTagName("oreveins");

                if (oreveins.Count > 0)
                {
                    var pop = oreveins.Item(0) as XmlElement;

                    for (CraftResource res = CraftResource.Iron; res <= CraftResource.Valorite; res++)
                    {
                        newRegion.Resources[res] = XmlConvert.ToDouble(pop.GetAttribute(res.ToString()));
                    }
                }

                var woodveins = reg.GetElementsByTagName("woodveins");
                if (woodveins.Count > 0)
                {
                    var pop = woodveins.Item(0) as XmlElement;
                    for (CraftResource res = CraftResource.RegularWood; res <= CraftResource.Frostwood; res++)
                    {
                        newRegion.Resources[res] = XmlConvert.ToDouble(pop.GetAttribute(res.ToString()));
                    }
                }

                //Banned followers are deducted from banned schools
                var bannedSchools = reg.GetElementsByTagName("bannedschools");

                if (bannedSchools.Count > 0)
                {
                    var pop = bannedSchools.Item(0) as XmlElement;
                    NelderimRegionSchools schools = new NelderimRegionSchools();

                    schools.Magery = XmlConvert.ToInt32(pop.GetAttribute("magery")) == 1;
                    schools.Chivalry = XmlConvert.ToInt32(pop.GetAttribute("chivalry")) == 1;
                    schools.Necromancy = XmlConvert.ToInt32(pop.GetAttribute("necromancy")) == 1;
                    schools.Spellweaving = XmlConvert.ToInt32(pop.GetAttribute("druidism")) == 1;

                    newRegion.BannedSchools = schools;
                }

                var intoleranceTag = reg.GetElementsByTagName("intolerance");

                if (intoleranceTag.Count > 0)
                {
                    var pop = intoleranceTag.Item(0) as XmlElement;

                    foreach (var race in Race.AllRaces)
                    {
                        string attr = pop.GetAttribute(race.Name);
                        newRegion.Intolerance[race] = XmlConvert.ToInt32(attr == "" ? "0" : attr) / 100f;
                    }
                }

                var races = reg.GetElementsByTagName("races");

                if (races.Count > 0)
                {
                    var pop = races.Item(0) as XmlElement;

                    foreach (var race in Race.AllRaces)
                    {
                        string attr = pop.GetAttribute(race.Name);
                        newRegion.Population[race] = XmlConvert.ToInt32(attr == "" ? "0" : attr) / 100f;
                    }
                }

                var g = reg.GetElementsByTagName("guards").Item(0) as XmlElement;

                foreach (XmlElement guard in g.GetElementsByTagName("guard"))
                {
                    var type = (GuardType)XmlConvert.ToInt32(guard.GetAttribute("type"));
                    var guardDef = new NelderimRegionGuard();
                    guardDef.Name = guard.GetAttribute("file");
                    guardDef.Female = XmlConvert.ToDouble(guard.GetAttribute("female"));

                    var guardRaces = guard.GetElementsByTagName("races").Item(0) as XmlElement;

                    foreach (var race in Race.AllRaces)
                    {
                        string attr = guardRaces.GetAttribute(race.Name);
                        guardDef.Population[race] = XmlConvert.ToInt32(attr == "" ? "0" : attr) / 100f;
                    }

                    newRegion.Guards[type] = guardDef;
                }

                NelderimRegions.Add(newRegion.Name, newRegion);
            }
        }

        private static void Save()
        {
            Console.Write("NelderimRegions: Saving...");
            try
            {
                File.WriteAllText(JsonPath, JsonSerializer.Serialize(NelderimRegions.Values));
            }
            catch (Exception e)
            {
                Console.WriteLine("NelderimRegions: Error!");
                Console.WriteLine(e.ToString());
                return;
            }
            Console.Write("NelderimRegions: Saved!");
        }

        public static NelderimRegion GetRegion(string regionName)
        {
            if (NelderimRegions.TryGetValue(regionName, out var result))
            {
                return result;
            }
            Console.WriteLine($"Unable to find region {regionName}");
            return null;
        }

        internal static NelderimGuardProfile GetGuardProfile(string name)
        {
            if (!GuardProfiles.ContainsKey(name))
            {
                var newProfile = new NelderimGuardProfile(name);
                GuardProfiles.Add(name, newProfile);
            }
            return GuardProfiles[name];
        }
        
        private static Func<Race, String>[] IntoleranceEmote =
        {
	        r => $"*mierzy zlowrogim spojrzeniem {r.GetName(Cases.Biernik)}*",
	        r => $"*z odraza sledzi wzrokiem {r.GetName(Cases.Biernik)}*",
	        _ => "*spluwa*!",
	        _ => "*prycha*",
        };
        
        private static Func<Race, String>[] IntoleranceSaying =
		{
			r => $"Co za czasy! Wszedzie {r.GetName(Cases.Mianownik, true)}",
			r => $"Zejdz mi z drogi {r.GetName(Cases.Wolacz)}!",
			r => $"Lepiej opusc ta okolice {r.GetName(Cases.Wolacz)}! Moze Cie spotkac krzywda!",
			r => $"{r.GetName(Cases.Wolacz)}! Psie jeden! Wynos sie z mego rewiru!",
			r => $"Nie chce Cie tu widziec {r.GetName(Cases.Wolacz)}!",
			r => $"{r.GetName(Cases.Wolacz)}! Psie jeden! To nie jest miejsce dla takich jak TY!",
			r => $"Co za czasy! Wszedzie {r.GetName(Cases.Mianownik, true)}!"
		};
		
		private static void MentionIntolerance(Mobile source, Mobile target)
		{
			source.Emote(Utility.RandomList(IntoleranceEmote).Invoke(target.Race));
			source.Yell(Utility.RandomList(IntoleranceSaying).Invoke(target.Race));
			target.SendMessage($"Zdaje sie, ze w tej okolicy nie lubi sie {target.Race.GetName(Cases.Dopelniacz, true)}!",0x25);
		}

		public static bool ActIntolerativeHarmful(Mobile source, Mobile target, bool msg = true)
		{
			try
			{
				if (source != null && target != null && source.InLOS(target))
				{
					var region = GetRegion(source.Region.Name);

					while (region.Intolerance == null) //TODO: Move this to within region
						region = GetRegion(region.Parent);

					double intolerance = region.Intolerance[target.Race];

					if (intolerance >= 30)
					{
						if (msg)
						{
							MentionIntolerance(source, target);
						}

						// szansa na crim
						if (intolerance > 50)
						{
							double distMod = 0;

							if (source is BaseNelderimGuard)
							{
								var distance = source.GetDistanceToSqrt(target);

								if (distance <= 3)
									distMod = 0.1;
								else if (distance >= 7)
									distMod = -0.1;
							}

							double minVal = source is BaseVendor ? 30 : 50;
							var chance = ((intolerance - minVal) * 2 + distMod) / 100;
							return chance >= Utility.RandomDouble();
						}
					}
				}
			}
			catch (Exception exc)
			{
				Console.WriteLine(exc.ToString());
				return false;
			}

			return false;
		}
    }
}