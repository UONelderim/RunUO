using Server.Items;
using Server.Mobiles;
using System;
using System.Collections;
using System.Xml;

namespace Server.Nelderim
{
	public class NelderimGuardProfile
	{
		public string Name { get; }
		private double m_Factor = 1.0f;
		private double m_Span = 1.0f;
		private int m_MaxStr, m_MaxDex, m_MaxInt, m_Hits, m_Damage;
		private string m_Title;
		private string m_NonHumanName;
        private string m_IsEnemyFunction;
        private int m_FightMode;
        private int m_NonHumanBody;
        private int m_NonHumanSound;
        private int m_NonHumanHue;
		private ArrayList m_Skills;
		private ArrayList m_SkillMaxValue;
		private int m_PhysicalResistanceSeed;
		private int m_FireResistSeed;
		private int m_ColdResistSeed;
		private int m_PoisonResistSeed;
		private int m_EnergyResistSeed;
		private ArrayList m_ItemType;
		private ArrayList m_ItemHue;
		private ArrayList m_BackpackItem;
		private ArrayList m_BackpackItemHue;
		private ArrayList m_BackpackItemAmount;
		private Type m_Mount;
		private int m_MountHue;

		public string IsEnemyFunction => m_IsEnemyFunction;

		public NelderimGuardProfile(string name)
		{
			Name = name;
			m_Skills = new ArrayList();
			m_SkillMaxValue = new ArrayList();
			m_ItemType = new ArrayList();
			m_ItemHue = new ArrayList();
			m_BackpackItem = new ArrayList();
			m_BackpackItemHue = new ArrayList();
			m_BackpackItemAmount = new ArrayList();

			int cutFrom = name.LastIndexOf("/") + 1;
			int cutTo = name.IndexOf(".");

			var fileName = name.Substring(cutFrom, cutTo - cutFrom);

			ReadProfile(name);
		}

		private void ReadProfile(string file) {
			try
			{
				XmlReaderSettings settings = new XmlReaderSettings();
				settings.ValidationType = ValidationType.DTD;
				settings.IgnoreWhitespace = true;
				//settings.ValidationEventHandler += new ValidationEventHandler( ValidationCallBack );

				XmlReader xml = XmlReader.Create(file, settings); //XmlTextReader( file );

				//xml.WhitespaceHandling = WhitespaceHandling.None;

				//XmlValidatingReader validXML = new XmlValidatingReader(xml);
				//validXML.ValidationType = ValidationType.DTD;

				XmlDocument doc = new XmlDocument();
				doc.Load(xml);

				XmlElement reader;
				XmlNodeList nodes;

				nodes = doc.GetElementsByTagName("title");
				if (nodes.Count >= 1)
					m_Title = (nodes.Item(0) as XmlElement).GetAttribute("value");

				nodes = doc.GetElementsByTagName("nonHuman");
				if (nodes.Count >= 1)
				{
					reader = nodes.Item(0) as XmlElement;
                    if (reader != null)
					{
						string attr;

						attr = reader.GetAttribute("body");
                        if (!String.IsNullOrEmpty(attr))
							m_NonHumanBody = XmlConvert.ToInt32(attr);

						attr = reader.GetAttribute("sound");
						if (!String.IsNullOrEmpty(attr))
							m_NonHumanSound = XmlConvert.ToInt32(attr);

						attr = reader.GetAttribute("hue");
						if (!String.IsNullOrEmpty(attr))
							m_NonHumanHue = XmlConvert.ToInt32(attr);

                        m_NonHumanName = reader.GetAttribute("name");
                    }
				}

				m_FightMode = (int) FightMode.Criminal; // default

                nodes = doc.GetElementsByTagName("behavior");
				if (nodes.Count >= 1)
				{
					reader = nodes.Item(0) as XmlElement;
					if (reader != null)
					{
                        string attr;

                        attr = reader.GetAttribute("fightMode");
                        if (!String.IsNullOrEmpty(attr))
                            m_FightMode = XmlConvert.ToInt32(attr);

                        m_IsEnemyFunction = reader.GetAttribute("isEnemyFunction");
						if (!String.IsNullOrEmpty(m_IsEnemyFunction) && typeof(BaseNelderimGuard).GetMethod(m_IsEnemyFunction)==null)
							Console.WriteLine("ERROR: Klasa BaseNelderimGuard nie posaida metody '" + m_IsEnemyFunction + "' okreslonej w m_IsEnemyFunction.");
                    }
				}


                foreach (XmlElement skill in doc.GetElementsByTagName("skill")) {
					m_Skills.Add((SkillName)XmlConvert.ToInt32(skill.GetAttribute("index")));
					m_SkillMaxValue.Add(XmlConvert.ToDouble(skill.GetAttribute("base")) * m_Factor);
				}

				foreach (XmlElement stat in doc.GetElementsByTagName("stat"))
					switch (stat.GetAttribute("name")) {
						case "str":
							m_MaxStr = (int)(XmlConvert.ToInt32(stat.GetAttribute("value")) * m_Factor);
							break;
						case "dex":
							m_MaxDex = (int)(XmlConvert.ToInt32(stat.GetAttribute("value")) * m_Factor);
							break;
						case "int":
							m_MaxInt = (int)(XmlConvert.ToInt32(stat.GetAttribute("value")) * m_Factor);
							break;
					}

				reader = doc.GetElementsByTagName("hits").Item(0) as XmlElement;
				m_Hits = XmlConvert.ToInt32(reader.GetAttribute("value"));


				reader = doc.GetElementsByTagName("damage").Item(0) as XmlElement;
				m_Damage = (int)(XmlConvert.ToInt32(reader.GetAttribute("value")) * m_Factor);


				reader = doc.GetElementsByTagName("resistances").Item(0) as XmlElement;
				m_PhysicalResistanceSeed = (int)(XmlConvert.ToInt32(reader.GetAttribute("physical")) * m_Factor);
				m_FireResistSeed = (int)(XmlConvert.ToInt32(reader.GetAttribute("fire")) * m_Factor);
				m_ColdResistSeed = (int)(XmlConvert.ToInt32(reader.GetAttribute("cold")) * m_Factor);
				m_PoisonResistSeed = (int)(XmlConvert.ToInt32(reader.GetAttribute("poison")) * m_Factor);
				m_EnergyResistSeed = (int)(XmlConvert.ToInt32(reader.GetAttribute("energy")) * m_Factor);

				foreach (XmlElement layer in doc.GetElementsByTagName("layer")) {
					int index = XmlConvert.ToInt32(layer.GetAttribute("index"));

					if (index != 7 && index != 5) {
						m_ItemType.Add(layer.GetAttribute("item"));
						m_ItemHue.Add(XmlConvert.ToInt32(layer.GetAttribute("hue")));

						if (layer.HasChildNodes && (string)m_ItemType[m_ItemType.Count - 1] == "Server.Items.Backpack") {
							// Console.WriteLine( "Backpack!" );

							foreach (XmlElement backpackItem in layer.ChildNodes) {
								m_BackpackItem.Add(backpackItem.GetAttribute("type"));
								m_BackpackItemHue.Add(XmlConvert.ToInt32(backpackItem.GetAttribute("hue")));
								m_BackpackItemAmount.Add(XmlConvert.ToInt32(backpackItem.GetAttribute("amount")));
							}
						}
					}
				}

				reader = doc.GetElementsByTagName("mount").Item(0) as XmlElement;

				if (!reader.HasAttribute("mounted")) {
					m_Mount = ScriptCompiler.FindTypeByFullName(reader.GetAttribute("type"), false);
					m_MountHue = XmlConvert.ToInt32(reader.GetAttribute("hue"));
				} else {
					m_Mount = null;
					m_MountHue = 0;
				}

				xml.Close();
			} catch (Exception e) {
				Console.WriteLine(e.ToString());
			}
		}

		private bool IsHuman()
		{
			return m_NonHumanBody == 0;
		}

		public void Make(BaseNelderimGuard target) {
			foreach (Layer layer in Enum.GetValues(typeof(Layer))) {
				Item item = target.FindItemOnLayer(layer);

				if (item != null)
					item.Delete();
			}

			if (target.Mounted) {
				if (target.Mount is Mobile) {
					BaseMount mount;
					mount = (BaseMount)target.Mount;
					mount.Delete();
				} else if (target.Mount is Item) {
					Item mount;
					mount = (Item)target.Mount;
					mount.Delete();
				}
			}

			target.ActiveSpeed /= m_Factor;
			target.PassiveSpeed /= m_Factor;

			target.FightMode = (FightMode) m_FightMode;
			target.IsEnemyFunction = m_IsEnemyFunction;

			BaseCreature bc = target;

			bc.SetStr((int)(m_MaxStr * m_Factor * m_Span), (int)(m_MaxStr * m_Factor));
			bc.SetDex((int)(m_MaxDex * m_Factor * m_Span), (int)(m_MaxDex * m_Factor));
			bc.SetInt((int)(m_MaxInt * m_Factor * m_Span), (int)(m_MaxInt * m_Factor));

			bc.SetHits((int)(m_Hits * m_Factor * m_Span), (int)(m_Hits * m_Factor));

			bc.SetDamage((int)(m_Damage * m_Factor * m_Span), (int)(m_Damage * m_Factor));

			bc.SetResistance(ResistanceType.Physical, (int)(m_PhysicalResistanceSeed * m_Factor * m_Span), (int)(m_PhysicalResistanceSeed * m_Factor));
			bc.SetResistance(ResistanceType.Fire, (int)(m_FireResistSeed * m_Factor * m_Span), (int)(m_FireResistSeed * m_Factor));
			bc.SetResistance(ResistanceType.Cold, (int)(m_ColdResistSeed * m_Factor * m_Span), (int)(m_ColdResistSeed * m_Factor));
			bc.SetResistance(ResistanceType.Poison, (int)(m_PoisonResistSeed * m_Factor * m_Span), (int)(m_PoisonResistSeed * m_Factor));
			bc.SetResistance(ResistanceType.Energy, (int)(m_EnergyResistSeed * m_Factor * m_Span), (int)(m_EnergyResistSeed * m_Factor));

			for (int i = 0; i < m_Skills.Count; i++)
				bc.SetSkill((SkillName)m_Skills[i], (double)m_SkillMaxValue[i] * m_Factor * m_Span, (double)m_SkillMaxValue[i] * m_Factor);

			for (int i = 0; i < m_ItemType.Count; i++) {
				Item item = (Item)Activator.CreateInstance((Type)ScriptCompiler.FindTypeByFullName(m_ItemType[i] as string, false));
				item.Hue = (int)m_ItemHue[i];
				item.LootType = LootType.Blessed;
				item.InvalidateProperties();
				bc.EquipItem(item);
			}

			Item backpack = bc.FindItemOnLayer(Layer.Backpack);

			if (backpack == null) {
				backpack = new Backpack();
				bc.AddItem(backpack);
			} else if (!(backpack is Container)) {
				backpack.Delete();
				backpack = new Backpack();
				bc.AddItem(backpack);
			}

			backpack.Movable = false;

			for (int i = 0; i < m_BackpackItem.Count; i++) {
				Item item = (Item)Activator.CreateInstance((Type)ScriptCompiler.FindTypeByFullName(m_BackpackItem[i] as string, false));
				item.Hue = (int)m_BackpackItemHue[i];
				item.Amount = (int)m_BackpackItemAmount[i];

				if (item.Stackable == false)
					item.LootType = LootType.Blessed;

				item.InvalidateProperties();
				(backpack as Container).DropItem(item);
			}

			backpack.InvalidateProperties();

			if (m_Mount != null && IsHuman()) {

				object someMount = (object)Activator.CreateInstance(m_Mount);

				if (someMount is BaseMount) {
					BaseMount mount = (BaseMount)someMount;
					mount.Hue = m_MountHue;
					mount.Rider = target;
					mount.ControlMaster = target;
					mount.Controlled = true;
					mount.InvalidateProperties();
				} else if (someMount is EtherealMount) {
					EtherealMount mount = (EtherealMount)someMount;
					mount.Hue = m_MountHue;
					mount.Rider = target;
					mount.InvalidateProperties();
				}
			}

			int rand = Server.Utility.Random(0, 99);
            int cumsum = 0, index = 0;

            Mobile mob = target;

			for (int i = 0; i < Race.AllRaces.Count; i++)
			{
				if ((cumsum += m_Races[i]) > rand)
				{
					index = i;
					break;
				}
			}
			Race guardRace = Race.AllRaces[index];
			mob.Race = guardRace;

			if (IsHuman())
            {
                mob.Female = (Utility.RandomDouble() < m_Female);
                mob.Body = (mob.Female) ? 401 : 400;

                guardRace.MakeRandomAppearance(mob);
                mob.Name = NameList.RandomName(guardRace, mob.Female);

                mob.SpeechHue = Utility.RandomDyedHue();
                mob.Title = m_Title;
            }
            else
            {
                mob.Name = m_NonHumanName;
                mob.Body = (m_NonHumanBody != 0) ? m_NonHumanBody : 400; // sanity (Body==0 makes mobile invisible)
                mob.BaseSoundID = m_NonHumanSound;
                mob.Hue = m_NonHumanHue;
            }

			mob.InvalidateProperties();
		}
	}
}
