#region References

using System.Collections.Generic;
using Server.Mobiles;
using Server.SicknessSys.Items;
using System.Collections;

#endregion

namespace Server.SicknessSys
{
	static class SicknessInfect
	{
		public static void Infect(PlayerMobile pm, IllnessType type, string sickness = null)
		{
			Item cell = pm.Backpack.FindItemByType(typeof(VirusCell));

			bool IsImmune = SicknessHelper.IsImmune(pm, type);

		//	if (pm.Backpack.FindItemByType(typeof(WhiteCell)) is WhiteCell whitecell && type != IllnessType.Vampirism &&
		//	    type != IllnessType.Lycanthropia)	
				
				WhiteCell whitecell = pm.Backpack.FindItemByType(typeof(WhiteCell)) as WhiteCell;
			if (whitecell != null && type != IllnessType.Vampirism && type != IllnessType.Lycanthropia)
			{
				int chance = SicknessHelper.GetSickChance(pm, 0);

				if (chance < whitecell.ViralResistance)
					IsImmune = true;
			}

			if (!IsImmune)
			{
				if (cell == null && pm != null)
				{
					SicknessAnimate.RunInfectedAnimation(pm);

					pm.AddToBackpack(new VirusCell(pm, type, sickness));

					VirusCell vc = pm.Backpack.FindItemByType(typeof(VirusCell)) as VirusCell;
					SicknessCore.VirusCellList.Add(vc);

				//	if (type == IllnessType.Vampirism)							
				//		pm.AddToBackpack(new VampireRobe(pm, 0x1F03, 1));
				
					
					Item item = pm.Backpack.FindItemByType(typeof(WhiteCell));
					if (item == null)
					{
						pm.AddToBackpack(new WhiteCell(pm, vc));
					}
					else
					{
						WhiteCell wc = item as WhiteCell;
						if (wc != null && Utility.RandomBool())
						{
							wc.ViralResistance++;
						}
					}
				}
			}
		}

		public static void SpreadVirus(PlayerMobile pm, VirusCell cell)
		{
			int rnd = Utility.RandomMinMax(1, 100);

			ArrayList result = new ArrayList();
			IPooledEnumerable eable = pm.GetMobilesInRange(3);
			foreach (Mobile c in eable)
			{
				if (c is PlayerMobile)
					result.Add(c as PlayerMobile);
			}
			eable.Free();

			if (result.Count > 0);

			{
				foreach (PlayerMobile player in result)
				{
					Item cellCheck = player.Backpack.FindItemByType(typeof(VirusCell));

					if (cellCheck == null && rnd < (int)cell.Illness * 10)
						Infect(pm, cell.Illness, cell.Sickness);
				}
			}
		}

		public static void OutBreak(List<VirusCell> CellList)
		{
			try
			{
				foreach (VirusCell cell in CellList)
				{
					int Chance = SicknessHelper.GetSickChance(cell.PM, 0);

					if (Chance != 0)
					{
						int Illness = Utility.RandomMinMax(1, 3);

						int rndInfect = Utility.RandomMinMax(1, 50000 * Illness);

						if (Chance == 100)
						{
							Infect(cell.PM, (IllnessType)Illness);
						}
						else
						{
							if (rndInfect <= Chance * 10)
							{
								Infect(cell.PM, (IllnessType)Illness);
							}
						}
					}
				}
			}
			catch
			{
				//do nothing : failed attempt
			}
		}
	}
}
