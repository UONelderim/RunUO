using System;
using Server.Mobiles;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using Server.Items;
using System.Linq;

namespace Server.Commands
{
	public class MobilesDifficulty
	{
		public static void Initialize()
		{
			CommandSystem.Register( "diff", AccessLevel.Administrator, new CommandEventHandler( MobilesDifficulty_OnCommand ) );
		}

		public static void MobilesDifficulty_OnCommand( CommandEventArgs e )
		{
			DateTime now = DateTime.Now; 
			
			string fileName = String.Format( "{0}_{1}-{2}-{3} {4}-{5}-{6}.csv", "mobilesDifficulty", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second );
			fileName = Path.Combine( Core.BaseDirectory, fileName);
			
			List<string> classNamesList = GetAllClasses( "Server.Mobiles" );

			bool header = true;
			using (StreamWriter writer = new StreamWriter(fileName))
			{
				foreach (string className in classNamesList)
				{
					try
					{
						BaseCreature bc =
							(BaseCreature) Activator.CreateInstance(
								(Type) ScriptCompiler.FindTypeByName(className, false));
						if (bc.AI != AIType.AI_Animal && bc.AI != AIType.AI_Vendor)
						{
							bc.GenerateDifficulty();
							Dictionary<string, object> props = new Dictionary<string, object>();
							props.Add("className", className);
							props.Add("Name", bc.Name);
							props.Add("Difficulty", bc.Difficulty);
							props.Add("BaseDifficulty", bc.BaseDifficulty);
							props.Add("DifficultyScalar", bc.DifficultyScalar);
							props.Add("AI", bc.AI);
							props.Add("DPS", bc.DPS);
							props.Add("Life", bc.Life);
							props.Add("Melee DPS", bc.MeleeDPS);
							props.Add("Magic DPS", bc.MagicDPS);
							props.Add("BreathDamage", bc.HasBreath ? (double) bc.BreathComputeDamage() / 12.5 : 0);

							props.Add("DamageMin", bc.DamageMin);
							props.Add("DamageMax", bc.DamageMax);
							props.Add("WeaponAbilitiesBonus", bc.WeaponAbilitiesBonus);
							props.Add("HitPoisonBonus", bc.HitPoisonBonus);

							props.Add("BardDiff", BaseInstrument.GetBaseDifficulty(bc));
							props.Add("Str", bc.Str);
							props.Add("Int", bc.Int);
							props.Add("HitsMax", bc.HitsMax);
							props.Add("StamMax", bc.StamMax);
							props.Add("ManaMax", bc.ManaMax);
							props.Add("SwitchTargetChance", bc.SwitchTargetChance);
							props.Add("AttackMasterChance", bc.AttackMasterChance);
							props.Add("VirtualArmor", bc.VirtualArmor);
							props.Add("BasePhysicalResistance", bc.BasePhysicalResistance);
							props.Add("BaseFireResistance", bc.BaseFireResistance);
							props.Add("BaseColdResistance", bc.BaseColdResistance);
							props.Add("BasePoisonResistance", bc.BasePoisonResistance);
							props.Add("BaseEnergyResistance", bc.BaseEnergyResistance);
							props.Add("PhysicalDamage", bc.PhysicalDamage);
							props.Add("FireDamage", bc.FireDamage);
							props.Add("ColdDamage", bc.ColdDamage);
							props.Add("PoisonDamage", bc.PoisonDamage);
							props.Add("EnergyDamage", bc.EnergyDamage);
							props.Add("RangePerception", bc.RangePerception);
							props.Add("ActiveSpeed", bc.ActiveSpeed);
							props.Add("CanHeal", bc.CanHeal ? 1 : 0);
							props.Add("HealScalar", bc.HealScalar);
							props.Add("HealTrigger", bc.HealTrigger);
							props.Add("HealDelay", bc.HealDelay);
							props.Add("HealInterval", bc.HealInterval);
							props.Add("BleedImmune", bc.BleedImmune ? 1 : 0);
							props.Add("PoisonImmune", bc.PoisonImmune != null ? bc.PoisonImmune.Level : 0);
							props.Add("HitPoison", bc.HitPoison != null ? bc.HitPoison.Level : 0);
							props.Add("HitPoisonChance", bc.HitPoisonChance);
							props.Add("CanAreaPoison", bc.CanAreaPoison ? 1 : 0);
							props.Add("HitAreaPoison", bc.HitAreaPoison != null ? bc.HitAreaPoison.Level : 0);
							props.Add("AreaPoisonRange", bc.AreaPoisonRange);
							props.Add("AreaPosionChance", bc.AreaPosionChance);
							props.Add("AreaPoisonDelay", (bc.AreaPoisonDelay).TotalSeconds);
							props.Add("CanAreaDamage", bc.CanAreaDamage ? 1 : 0);
							props.Add("AreaDamageRange", bc.AreaDamageRange);
							props.Add("AreaDamageScalar", bc.AreaDamageScalar);
							props.Add("AreaDamageChance", bc.AreaDamageChance);
							props.Add("AreaDamageDelay", (bc.AreaDamageDelay).TotalSeconds);
							props.Add("AreaPhysicalDamage", bc.AreaPhysicalDamage);
							props.Add("AreaFireDamage", bc.AreaFireDamage);
							props.Add("AreaColdDamage", bc.AreaColdDamage);
							props.Add("AreaPoisonDamage", bc.AreaPoisonDamage);
							props.Add("AreaEnergyDamage", bc.AreaEnergyDamage);
							props.Add("Unprovokable", bc.Unprovokable ? 1 : 0);
							props.Add("Uncalmable", bc.Uncalmable ? 1 : 0);
							props.Add("ReacquireDelay", (bc.ReacquireDelay).TotalSeconds);

							foreach (WeaponAbility wa in WeaponAbility.Abilities)
							{
								if (wa != null)
									props.Add(wa.GetType().Name + " chance",
										bc.WeaponAbilities.ContainsKey(wa) ? bc.WeaponAbilities[wa] : 0);
							}

							// Skills
							SkillName[] skills = new SkillName[]
							{
								SkillName.Anatomy, SkillName.Parry, SkillName.DetectHidden,
								SkillName.EvalInt, SkillName.Healing, SkillName.Hiding,
								SkillName.Inscribe, SkillName.Magery, SkillName.MagicResist,
								SkillName.Tactics, SkillName.Poisoning, SkillName.Archery,
								SkillName.SpiritSpeak, SkillName.Swords, SkillName.Macing,
								SkillName.Fencing, SkillName.Wrestling, SkillName.Lumberjacking,
								SkillName.Meditation, SkillName.Necromancy, SkillName.Focus,
								SkillName.Chivalry, SkillName.Bushido, SkillName.Ninjitsu
							};

							for (int i = 0; i < skills.Length; i++)
							{
								Skill s = bc.Skills[skills[i]];
								props.Add(s.Name, s.Value);
							}

							// Zapis
							if (header)
							{
								string[] keys = new string[props.Keys.Count];
								props.Keys.CopyTo(keys, 0);
								writer.WriteLine(String.Join("|", keys));
								header = false;
							}

							List<string> vals = new List<string>();

							foreach (object o in props.Values)
							{
								string s = String.Empty;

								try
								{
									s = o.ToString();
								}
								catch
								{
								}

								vals.Add(s);
							}

							writer.WriteLine(String.Join("|", vals.ToArray()));
						}
					}
					catch
					{
					}
				}
			}

			e.Mobile.SendMessage( 0x400, "Log zostal zapisany do: {0}", fileName );
		}

		public static List<string> GetAllClasses( string nameSpace )
		{
			List<string> returnList = new List<string>();
			IEnumerable<Type> types = Assembly.GetAssembly(typeof(BaseCreature)).GetTypes()
				.Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(BaseCreature)));
			foreach (var type in types)
			{
				returnList.Add( type.Name );
			}
			return returnList;
		}		
	}
}
