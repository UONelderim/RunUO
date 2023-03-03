#region References

using Nelderim.Scripts.Nelderim.Engines.SicknessSys.Illnesses;
using Server.Items;
using Server.Mobiles;

#endregion

namespace Server.SicknessSys.Illnesses
{
	public static class Lycanthropia
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
		public static void LycanthropiaWeakness(VirusCell cell)
		{
			bool isSilverSlayer = false;
			PlayerMobile m = cell.PM.Combatant as PlayerMobile;
			
			//if (cell.Level < 100)								// Czy slayer ma na niego działać?
			//{
			//    IEnumerable<Silver> result = from c in cell.PM.GetItemsInRange(3)
			//                                 where c is Silver
			//                                 select c as Silver;

			//    DoDamage = result.Any();

			//    Item resultBP = cell.PM.Backpack.FindItemByType(typeof(Silver));
			//    if (resultBP != null)
			//        DoDamage = true;
			//}

			if (m != null)
			{
				BaseWeapon bw1 = m.FindItemOnLayer(Layer.OneHanded) as BaseWeapon;
				BaseWeapon bw2 = m.FindItemOnLayer(Layer.TwoHanded) as BaseWeapon;

				if (bw1 != null && (bw1.Slayer == SlayerName.Silver || bw1.Slayer2 == SlayerName.Silver))
				{
					isSilverSlayer = true;
				}
				else if (bw2 != null && (bw2.Slayer == SlayerName.Silver || bw2.Slayer2 == SlayerName.Silver))
				{
					isSilverSlayer = true; //przepisane
				}
			}
		}

		public static bool IsMutated(Mobile m)
		{
			if (m == null || m.Backpack == null) 
				return false;
			
			VirusCell cell = m.Backpack.FindItemByType(typeof(VirusCell)) as VirusCell;
			return cell != null && 
			       cell.Illness == SicknessSys.IllnessType.Lycanthropia && 
			       cell.Stage > 0 &&
			       cell.PM != null &&
			       cell.PM.Body != cell.DefaultBody;
		}

		/*public static void LycanthropiaWeakness(VirusCell cell)  //tak było
		{
			bool DoDamage = false;

			

			bool IsSilverSlayer = false;

			if (cell.PM.Combatant != null && cell.PM.Combatant is PlayerMobile m)
			{
				if (m.FindItemOnLayer(Layer.OneHanded) is BaseWeapon bw1)
				{
					if (bw1.Slayer == SlayerName.Silver || bw1.Slayer2 == SlayerName.Silver)
						IsSilverSlayer = true;
				}


				if (m.FindItemOnLayer(Layer.TwoHanded) is BaseWeapon bw2)
				{
					if (bw2.Slayer == SlayerName.Silver || bw2.Slayer2 == SlayerName.Silver)
						IsSilverSlayer = true;
				}
			}


			if (DoDamage || IsSilverSlayer)
			{
				int damage = BaseDamage * cell.Stage;

				cell.PM.Damage(damage);

				SicknessAnimate.RunGrowlAnimation(cell.PM);
			}
		}*/
	}
}
