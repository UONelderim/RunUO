#region References

using System.Collections.Generic;
using System.Linq;
using Server.Items;
using Server.Regions;

#endregion

namespace Server.SicknessSys.Illnesses
{
	public static class Vampirism
	{
		private static int illnessType = 102;
		private static string name;
		private static int statDrain;
		private static int baseDamage;
		private static int powerDegenRate;

		public static int IllnessType
		{
			get
			{
				return illnessType;
			}
		}

		public static string Name
		{
			get
			{
				if (name == null)
				{
					name = BaseVirus.GetRandomName(IllnessType);
				}
				return name;
			}
		}

		public static int StatDrain
		{
			get
			{
				if (statDrain == 0)
				{
					statDrain = BaseVirus.GetRandomDamage(IllnessType);
				}
				return statDrain;
			}
		}

		public static int BaseDamage
		{
			get
			{
				if (baseDamage == 0)
				{
					baseDamage = BaseVirus.GetRandomDamage(IllnessType);
				}
				return baseDamage;
			}
		}

		public static int PowerDegenRate
		{
			get
			{
				if (powerDegenRate == 0)
				{
					powerDegenRate = BaseVirus.GetRandomDegen(IllnessType);
				}
				return powerDegenRate;
			}
		}


		public static void UpdateBody(VirusCell cell)
		{
			IllnessMutationLists.SetMutation(cell);
		}

		public static void VampireWeakness(VirusCell cell)
		{
			bool DoDamage = false;
			bool DoMinDamage = false;

			if (cell.Level < 100)
			{
				List<Garlic> garlics = new List<Garlic>();
				foreach (Item item in cell.PM.GetItemsInRange(3))
				{
					if (item is Garlic)
				{				
					garlics.Add(item as Garlic);
				}
			}


				DoMinDamage = garlics.Count > 0;

				Item garlicBP = cell.PM.Backpack.FindItemByType(typeof(Garlic));
				if (garlicBP != null)
					{
    					DoMinDamage = true;
					}

			}

			if (!SicknessHelper.IsNight(cell.PM) && !cell.PM.Region.IsPartOf("DungeonRegion"))

			{
				if (!SicknessHelper.InDoors(cell.PM))
				{
					if (!SicknessHelper.IsFullyCovered(cell.PM))
					{
						DoDamage = true;
					}
				}
			}
			
			if (!SicknessHelper.IsNight(cell.PM) && !cell.PM.Region.IsPartOf("HousingRegion"))

			{
				if (!SicknessHelper.InDoors(cell.PM))
				{
					if (!SicknessHelper.IsFullyCovered(cell.PM))
					{
						DoDamage = false;
					}
				}
			}

			if (DoDamage || DoMinDamage)
			{
				int damage = BaseDamage * cell.Stage;

				if (DoMinDamage && damage != 0)
					damage = damage / 10;

				cell.PM.Damage(damage);

				if (DoMinDamage)
					cell.PM.SendMessage("Czujesz obecnosc czosnku!");
				else
					cell.PM.SendMessage("Zostales wystawiony na dzialanie swiatla. Schowaj sie, bo zginiesz!");

				SicknessAnimate.RunScreamAnimation(cell.PM);
			}
		}
	}
}
