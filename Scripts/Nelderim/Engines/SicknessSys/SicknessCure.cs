#region References

using Server.Mobiles;
using Server.SicknessSys.Gumps;
using Server.SicknessSys.Items;

#endregion

namespace Server.SicknessSys
{
	static class SicknessCure
	{
		public static void Cure(PlayerMobile pm, VirusCell cell)
		{
			if (cell != null && pm != null)
			{
				cell.IsMovingGump = true;

				if (pm.HasGump(typeof(PowerGump)))
					pm.CloseGump(typeof(PowerGump));

				SicknessAnimate.RunCureAnimation(pm);

				pm.SendMessage("Czujesz sie lepiej!");

				pm.CloseAllGumps();
				
				cell.Delete();

				Item claws = pm.Backpack.FindItemByType(typeof(WereClaws));

				if (claws != null)
					claws.Delete();
			}
		}

		public static bool SelfCureIllness(VirusCell cell)
		{
			int curechance = 0;

			switch (cell.Illness)
			{
				case IllnessType.Cold:
					curechance = 3;
					break;
				case IllnessType.Flu:
					curechance = 2;
					break;
				case IllnessType.Virus:
					curechance = 1;
					break;
				case IllnessType.Vampirism:
					curechance = 0;
					break;
				case IllnessType.Lycanthropia:
					curechance = 0;
					break;
				default:
					curechance = 0;
					break;
			}

			if (curechance > 0)
			{
				int rnd = Utility.RandomMinMax(1, 90000 / curechance);

				if (rnd <= curechance)
				{
					Cure(cell.PM, cell);
					return true;
				}
			}

			return false;
		}
	}
}
