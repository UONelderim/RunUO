#region References

using Server;
using System.Collections.Generic;
using System.Linq;
using Server.Misc;
using Server.Mobiles;
using Server.SicknessSys.Gumps;
using Server.SicknessSys.Illnesses;
using Server.SicknessSys.Items;
using Server.Targeting;
using Server.Network;
using System;
using Server.Spells;
using Server.Items;
using System.Collections;
using System.Runtime.Serialization;
using Nelderim.Scripts.Nelderim.Engines.SicknessSys.Illnesses;
using Server.Multis;
using Server.Regions;
using Weather = Server.Misc.Weather;

#endregion

namespace Server.SicknessSys
{
	
	public static class SicknessHelper
	{
		public static bool IsSpecialVirus(VirusCell cell)
		{
			bool IsSpecial = false;

			if (cell.Illness == IllnessType.Lycanthropia)
				IsSpecial = true;
			if (cell.Illness == IllnessType.Vampirism)
				IsSpecial = true;

			return IsSpecial;
		}

		public static void SendHeartGump(VirusCell cell)
		{
			if (cell != null)
			{
				PlayerMobile pm = cell.PM;

				if (pm != null && cell != null)
				{
					IllnessMutationLists.SetMutation(cell);

					int HeartBaseHue = cell.BaseHeart;

					int stageAdjusted = GetStageAdjustment(cell);

					int beatMod = GetPowerMod(cell);

					int powerCheck = Utility.RandomMinMax(1, cell.PowerDegenRate * 2);

					if (!cell.IsMovingGump)
					{
						PowerGump pg;

						if (cell.HeartBeat < 4 - beatMod)
						{
							if (powerCheck > cell.PowerDegenRate)
								ImmuneSystem.UpdateImmuneNegative(cell);

							pg = new PowerGump(pm, cell, HeartBaseHue, cell.GumpX, cell.GumpY);

							cell.HeartBeat++;
						}
						else
						{
							if (cell.IsContagious && !IsSpecialVirus(cell))
							{
								SicknessInfect.SpreadVirus(pm, cell);
							}

							bool RunImmune = Utility.RandomBool();

							if (RunImmune)
								ImmuneSystem.UpdateImmuneNegative(cell);

							pg = new PowerGump(pm, cell, stageAdjusted, cell.GumpX, cell.GumpY, true);

							cell.HeartBeat = 0;
						}

						pm.SendGump(pg);
					}
					else
					{
						if (cell.IsMovingRelease < 10)
						{
							cell.IsMovingRelease++;
						}
						else
						{
							cell.IsMovingGump = false;
							cell.IsMovingRelease = 0;
						}
					}
				}
				else
				{
					cell.Delete();
				}
			}
		}

		private static int GetPowerMod(VirusCell cell)
		{
			int mod = 0;

			if (!IsLowHealth(cell.PM))
			{
				if (cell.Power < (cell.MaxPower / 4) * 3 || cell.PM.Hits < (cell.PM.Hits / 4) * 3)
				{
					mod++;

					if (cell.Power < (cell.MaxPower / 4) * 2 || cell.PM.Hits < cell.PM.Hits / 2)
					{
						mod++;

						if (cell.Power < cell.MaxPower / 4 || cell.PM.Hits < cell.PM.Hits / 4)
						{
							mod++;
						}
					}
				}
			}
			else
			{
				mod = 3;
			}

			if (cell.Level == 100)
				mod = 3;
			else if (cell.Stage == 3 && mod < 2)
			{
				mod = 2;
			}
			else if (cell.Stage == 2 && mod < 1)
			{
				mod = 1;
			}

			return mod;
		}

		private static int GetStageAdjustment(VirusCell cell)
		{
			int adj = 0;

			if (cell.Stage == 1)
			{
				adj = 2751;
			}

			if (cell.Stage == 2)
			{
				adj = 2752;
			}

			if (cell.Stage == 3)
			{
				adj = 2750;
			}

			return adj;
		}

		public static bool IsDark(PlayerMobile pm)
		{
			if (pm != null)
			{
				if (pm.LightLevel > 5)
					return false;
				return true;
			}

			return false;
		}

		public static bool IsNight(PlayerMobile pm)
		{
			return ServerTime.TimeUnit < 5 || ServerTime.TimeUnit > 25;
		}

		public static int GetSickChance(PlayerMobile pm, int chance)
		{
			int ClothingMod;

			if (IsFullyExposed(pm))
			{
				ClothingMod = 5;
			}
			else if (IsPartiallyExposed(pm))
			{
				ClothingMod = 0;
			}
			else
			{
				ClothingMod = -5;
			}

			if (chance >= 0)
			{
				if (IsNight(pm))
					chance += (5 + ClothingMod);
				if (IsForest(pm))
					chance += (7 + ClothingMod);
				if (IsJungle(pm))
					chance += (11 + ClothingMod);
				if (IsSand(pm))
					chance += (13 + ClothingMod);
				if (IsSnow(pm))
					chance += (17 + ClothingMod);
				if (IsCave(pm))
					chance += (19 + ClothingMod);
				if (IsSwamp(pm))
					chance += (21 + ClothingMod);

				if (IsWeather(pm))
				{
					chance += (31 + ClothingMod);
				}

				if (IsLowHealth(pm))
					chance += (41 + ClothingMod);

				if (AreRatsClose(pm))
					chance += (50 + ClothingMod);
			}

			if (chance < 1)
				return 1;
			if (chance < 100)
				return chance;
			return 100;
		}

		public static bool IsForest(PlayerMobile pm)				
		{
			return CheckTile(pm, "forest");
		}

		public static bool IsJungle(PlayerMobile pm)
		{
			return CheckTile(pm, "jungle");
		}

		public static bool IsSand(PlayerMobile pm)
		{
			return CheckTile(pm, "sand");
		}

		public static bool IsSnow(PlayerMobile pm)
		{
			return CheckTile(pm, "snow");
		}

		public static bool IsCave(PlayerMobile pm)
		{
			return CheckTile(pm, "cave");
		}

		public static bool IsSwamp(PlayerMobile pm)
		{
			return CheckTile(pm, "NoName");
		}

		private static bool CheckTile(PlayerMobile pm, string name)
		{
			if (pm != null)
			{
				Tile tileID = pm.Map.Tiles.GetLandTile(pm.X, pm.Y);

				LandData landData = TileData.LandTable[tileID.ID & 0x3FFF];

				return landData.Name.IndexOf(name, StringComparison.OrdinalIgnoreCase) >= 0;
			}

			return false;
		}

		public static bool InDoors(PlayerMobile pm)
		{
			bool CheckAbove = false;

			if (pm != null)
			{
				Tile[] tiles = pm.Map.Tiles.GetStaticTiles(pm.X, pm.Y);

				if (tiles.Length > 0)
				{
					foreach (var item in tiles)
					{
						if (item.Z > pm.Z)
							CheckAbove = true;
					}
				}
			}

			return CheckAbove;
		}

		public static bool IsWeather(Server.Mobiles.PlayerMobile player)

		{
			Map facet = player.Map;
			bool weather = false;

			if (facet == null)
				return false;

			List<Weather> list = Weather.GetWeatherList(facet);

			for (int i = 0; i < list.Count; ++i)
			{
				Weather w = list[i];

				for (int j = 0; j < w.Area.Length; ++j)
				{
					int weatherX = w.Area[j].X - (w.Area[j].Width / 2);
					int weatherXX = w.Area[j].X + (w.Area[j].Width / 2);

					int weatherY = w.Area[j].Y - (w.Area[j].Height / 2);
					int weatherYY = w.Area[j].Y + (w.Area[j].Height / 2);

					if (weatherX < player.X && weatherXX > player.X)
					{
						if (weatherY < player.Y && weatherYY > player.Y && !InDoors(player))
						{
							weather = true;
						}
					}
				}
			}

			return weather;
		}
	/*																		// konflikt z poprzednią porcją kodu (player i pm)
		private static bool InDoors(Server.Mobiles.PlayerMobile pm)
		{
			BaseHouse house = BaseHouse.GetHouseAt(pm.Location, pm.Map);

			return house != null && pm.Map.MapIndex == 0;
		}*/



		public static bool IsFullyCovered(PlayerMobile pm)
		{
			bool IsCovered = false;

			if (pm != null)
			{
				if (GetClothing(pm) > 5)
					IsCovered = true;
			}

			return IsCovered;
		}

		public static bool IsFullyExposed(PlayerMobile pm)
		{
			bool IsExposed = false;

			if (pm != null)
			{
				if (GetClothing(pm) == 0)
					IsExposed = true;
			}

			return IsExposed;
		}

		public static bool IsPartiallyExposed(PlayerMobile pm)
		{
			bool IsExposed = false;

			if (pm != null)
			{
				if (GetClothing(pm) < 5)
					IsExposed = true;
			}

			return IsExposed;
		}

		private static int GetClothing(PlayerMobile pm)
		{
			int NumberOfClothing = 0;

			Item item = pm.FindItemOnLayer(Layer.OuterTorso);

			if (pm.FindItemOnLayer(Layer.Helm) != null)
				NumberOfClothing+=5;
			//if (pm.FindItemOnLayer(Layer.Head) != null)
			//	NumberOfClothing++;
			if (pm.FindItemOnLayer(Layer.Neck) != null)
				NumberOfClothing++;
			if (pm.FindItemOnLayer(Layer.InnerTorso) != null)
				NumberOfClothing++;
			if (pm.FindItemOnLayer(Layer.MiddleTorso) != null)
				NumberOfClothing++;
			if (pm.FindItemOnLayer(Layer.OuterTorso) != null)
			/*{
				if (item is VampireRobe)							
					NumberOfClothing += 5;
				else
					NumberOfClothing++;
			}*/

			if (pm.FindItemOnLayer(Layer.Pants) != null)
				NumberOfClothing++;
			if (pm.FindItemOnLayer(Layer.Arms) != null)
				NumberOfClothing++;
			if (pm.FindItemOnLayer(Layer.Gloves) != null)
				NumberOfClothing++;
			if (pm.FindItemOnLayer(Layer.OuterLegs) != null)
				NumberOfClothing++;

			return NumberOfClothing;
		}

		public static bool IsLowHealth(PlayerMobile pm)
		{
			if (pm != null)
			{
				if (pm.Hits < pm.HitsMax / 4)
					return true;
				return false;
			}

			return false;
		}

		public static bool AreRatsClose(PlayerMobile pm)
		{
			bool hasRat = false;
			foreach (Mobile m in pm.GetMobilesInRange(3))
			{
				if (m is Rat)
				{
					hasRat = true;
					break;
				}
			}
			return hasRat;
		}
	}
}
