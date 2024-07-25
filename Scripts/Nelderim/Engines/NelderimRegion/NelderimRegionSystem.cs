using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Server.Mobiles;
using Server.Items;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Server.Nelderim
{
    public class NelderimRegionSystem
    {
        internal static string BaseDir = Path.Combine(Core.BaseDirectory, "Data", "NelderimRegions");
        private static string XmlPath = Path.Combine(BaseDir, "NelderimRegions.xml");
        private static string JsonPath = Path.Combine(BaseDir, "NelderimRegions.json");

        internal static Dictionary<string, NelderimRegion> NelderimRegions = new();
        internal static Dictionary<string, NelderimGuardProfile> GuardProfiles = new();
        
        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
	        WriteIndented = true,
	        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
	        Converters = { new ShortDoubleJsonConverter()},
	        
        };

        public static void Initialize()
        {
            Load();
            RumorsSystem.Load();
        }

        private static void Load()
        {
            NelderimRegions.Clear();
            // if (File.Exists(JsonPath))
            // {
            var region = JsonSerializer.Deserialize<NelderimRegion>(File.ReadAllText(JsonPath), SerializerOptions);
            Add(region);
            // }
            // else
            // {
	            // LoadXml();
	            // Save();
            // }

            Console.WriteLine("NelderimRegions: Loaded.");
        }

        private static void Add(NelderimRegion region)
        {
	        if (region.Regions != null)
	        {
		        region.Regions.Sort();
		        foreach (var subRegion in region.Regions)
		        {
			        subRegion.Parent = region;
			        Add(subRegion);
		        }
	        }
	        region.Validate();
	        NelderimRegions.Add(region.Name, region);
        }

        private static void LoadXml()
        {
            var doc = new XmlDocument();
            doc.Load(XmlPath);

            var root = doc["NelderimRegions"];

            Dictionary<NelderimRegion, string> parents = new();
            
            foreach (XmlElement reg in root.GetElementsByTagName("region"))
            {
                var newRegion = new NelderimRegion();
                newRegion.Name = reg.GetAttribute("name");
                var parent = reg.GetAttribute("parent");
                if (parent != "")
	                parents[newRegion] = parent;

                var oreveins = reg.GetElementsByTagName("oreveins");

                if (oreveins.Count > 0)
                {
	                newRegion.Resources = new Dictionary<CraftResource, double>();
                    var pop = oreveins.Item(0) as XmlElement;

                    for (CraftResource res = CraftResource.Iron; res <= CraftResource.Valorite; res++)
                    {
                        newRegion.Resources[res] = XmlConvert.ToDouble(pop.GetAttribute(res.ToString()));
                    }
                }

                var woodveins = reg.GetElementsByTagName("woodveins");
                if (woodveins.Count > 0)
                {
	                newRegion.Resources ??= new Dictionary<CraftResource, double>();
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
	                newRegion.BannedSchools = new NelderimRegionSchools();
                    var pop = bannedSchools.Item(0) as XmlElement;

                    newRegion.BannedSchools.Magery = XmlConvert.ToInt32(pop.GetAttribute("magery")) == 1;
                    newRegion.BannedSchools.Chivalry = XmlConvert.ToInt32(pop.GetAttribute("chivalry")) == 1;
                    newRegion.BannedSchools.Necromancy = XmlConvert.ToInt32(pop.GetAttribute("necromancy")) == 1;
                    newRegion.BannedSchools.Spellweaving = XmlConvert.ToInt32(pop.GetAttribute("druidism")) == 1;
                    
                    if(newRegion.BannedSchools.Magery == false && 
                       newRegion.BannedSchools.Chivalry == false && 
                       newRegion.BannedSchools.Necromancy == false && 
                       newRegion.BannedSchools.Spellweaving == false)
					{
	                    newRegion.BannedSchools = null;
					}
                }

                var intoleranceTag = reg.GetElementsByTagName("intolerance");

                if (intoleranceTag.Count > 0)
                {
	                newRegion.Intolerance = new Dictionary<string, double>();
                    var pop = intoleranceTag.Item(0) as XmlElement;
		
                    foreach (var race in Race.AllRaces)
                    {
                        string attr = pop.GetAttribute(race.Name);
                        if(attr == "" || attr == "0")
	                        continue;
                        
                        newRegion.Intolerance[race.Name] = XmlConvert.ToDouble(attr) / 100f;
                    }
                }

                var races = reg.GetElementsByTagName("races");

                if (races.Count > 0)
                {
	                newRegion.Population = new Dictionary<string, double>();
                    var pop = races.Item(0) as XmlElement;

                    foreach (var race in Race.AllRaces)
                    {
                        string attr = pop.GetAttribute(race.Name);
                        if(attr == "" || attr == "0")
	                        continue;
                        
						newRegion.Population[race.Name] = XmlConvert.ToDouble(attr) / 100f;
                    }
                }

                var g = reg.GetElementsByTagName("guards").Item(0) as XmlElement;

                if(g != null)
                {
	                newRegion.Guards = new Dictionary<GuardType, NelderimRegionGuard>();
	                foreach (XmlElement guard in g.GetElementsByTagName("guard"))
	                {
		                var type = (GuardType)XmlConvert.ToInt32(guard.GetAttribute("type"));
		                var guardDef = new NelderimRegionGuard();
		                guardDef.Name = guard.GetAttribute("file");
		                guardDef.Female = XmlConvert.ToDouble(guard.GetAttribute("female"));

		                var guardRaces = guard.GetElementsByTagName("races").Item(0) as XmlElement;

		                foreach (var race in Race.AllRaces)
		                {
			                guardDef.Population ??= new Dictionary<string, double>();
			                string attr = guardRaces.GetAttribute(race.Name);
			                if(attr == "" || attr == "0")
				                continue;
			                
			                guardDef.Population[race.Name] = XmlConvert.ToDouble(attr) / 100f;
		                }

		                newRegion.Guards[type] = guardDef;
	                }
                }

                newRegion.Validate();
                NelderimRegions.Add(newRegion.Name, newRegion);
            }
            
            foreach (var keyValuePair in parents)
            {
	            var region = keyValuePair.Key;
	            var parent =  NelderimRegions[keyValuePair.Value];
	            region.Parent = parent;
	            parent.Regions ??= new List<NelderimRegion>();
	            parent.Regions.Add(region);

            }
        }

        private static void Save()
        {
            File.WriteAllText(JsonPath, JsonSerializer.Serialize(NelderimRegions["Default"], SerializerOptions));
            Console.WriteLine("NelderimRegions: Saved!");
        }

        public static NelderimRegion GetRegion(string regionName)
        {
	        if(regionName == null)
		        return null;
	        
            if (NelderimRegions.TryGetValue(regionName, out var result))
            {
                return result;
            }
            return NelderimRegions["Default"]; //Fallback to default for non specified regions
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
						region = region.Parent;

					double intolerance = region.Intolerance[target.Race.Name];

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