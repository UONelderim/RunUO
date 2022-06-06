using System;
using System.Collections.Generic;
using Server.Commands;
using Server.Mobiles;
using Server.Network;
using Nelderim.Towns;

namespace Server.Gumps
{
	public class WhoGump : Gump
	{
		public static void Initialize()
		{
			CommandSystem.Register("Who", AccessLevel.Counselor, new CommandEventHandler(WhoList_OnCommand));
			CommandSystem.Register("WhoList", AccessLevel.Counselor, new CommandEventHandler(WhoList_OnCommand));
		}

		[Usage("WhoList [filter]")]
		[Aliases("Who")]
		[Description("Lists all connected clients. Optionally filters results by name.")]
		private static void WhoList_OnCommand(CommandEventArgs e)
		{
			e.Mobile.SendGump( new WhoGump( e.Mobile, e.ArgString ) );
		}

		public static bool OldStyle = PropsConfig.OldStyle;

		public static readonly int GumpOffsetX = PropsConfig.GumpOffsetX;
		public static readonly int GumpOffsetY = PropsConfig.GumpOffsetY;

		public static readonly int TextHue = PropsConfig.TextHue;
		public static readonly int TextOffsetX = PropsConfig.TextOffsetX;

		public static readonly int OffsetGumpID = PropsConfig.OffsetGumpID;
		public static readonly int HeaderGumpID = PropsConfig.HeaderGumpID;
		public static readonly int EntryGumpID = PropsConfig.EntryGumpID;
		public static readonly int BackGumpID = PropsConfig.BackGumpID;
		public static readonly int SetGumpID = PropsConfig.SetGumpID;

		public static readonly int SetWidth = PropsConfig.SetWidth;
		public static readonly int SetOffsetX = PropsConfig.SetOffsetX, SetOffsetY = PropsConfig.SetOffsetY;
		public static readonly int SetButtonID1 = PropsConfig.SetButtonID1;
		public static readonly int SetButtonID2 = PropsConfig.SetButtonID2;

		public static readonly int PrevWidth = PropsConfig.PrevWidth;
		public static readonly int PrevOffsetX = PropsConfig.PrevOffsetX, PrevOffsetY = PropsConfig.PrevOffsetY;
		public static readonly int PrevButtonID1 = PropsConfig.PrevButtonID1;
		public static readonly int PrevButtonID2 = PropsConfig.PrevButtonID2;

		public static readonly int NextWidth = PropsConfig.NextWidth;
		public static readonly int NextOffsetX = PropsConfig.NextOffsetX, NextOffsetY = PropsConfig.NextOffsetY;
		public static readonly int NextButtonID1 = PropsConfig.NextButtonID1;
		public static readonly int NextButtonID2 = PropsConfig.NextButtonID2;

		public static readonly int OffsetSize = PropsConfig.OffsetSize;

		public static readonly int EntryHeight = PropsConfig.EntryHeight;
		public static readonly int BorderSize = PropsConfig.BorderSize;

		private static bool PrevLabel = false, NextLabel = false;

		private static readonly int PrevLabelOffsetX = PrevWidth + 1;
		private static readonly int PrevLabelOffsetY = 0;

		private static readonly int NextLabelOffsetX = -29;
		private static readonly int NextLabelOffsetY = 0;

		private static readonly int EntryWidth = 180;
		private static readonly int EntryCount = 15;

		// <nel>
		private const int RaceEntryWidth = 100;

		private const int GoWidth = 24;
		public const int GoButtonID1 = 2224; // 2224
		public const int GoButtonID2 = 2224;

		private const int BringWidth = 24;
		public const int BringButtonID1 = 2223; // 2223
		public const int BringButtonID2 = 2223;

		private const int TownEntryWidth = 80;

		private const int GuildEntryWidth = 80;
		// </nel>

		private static readonly int TotalWidth = OffsetSize + EntryWidth + OffsetSize + SetWidth + OffsetSize;
		private static readonly int TotalHeight = OffsetSize + ((EntryHeight + OffsetSize) * (EntryCount + 1));

		private static readonly int BackWidth = BorderSize + TotalWidth + BorderSize;
		private static readonly int BackHeight = BorderSize + TotalHeight + BorderSize;

		private Mobile m_Owner;
		private List<Mobile> m_Mobiles;
		private int m_Page;

		private class InternalComparer : IComparer<Mobile>
		{
			public static readonly IComparer<Mobile> Instance = new InternalComparer();

			public InternalComparer()
			{
			}

			public int Compare(Mobile x, Mobile y)
			{
				if (x == null || y == null)
					throw new ArgumentException();

				if (x.AccessLevel > y.AccessLevel)
					return -1;
				else if (x.AccessLevel < y.AccessLevel)
					return 1;
				else
					return Insensitive.Compare(x.Name, y.Name);
			}
		}

		public WhoGump(Mobile owner, string filter)
			: this(owner, BuildList(owner, filter), 0)
		{
		}

		public WhoGump(Mobile owner, List<Mobile> list, int page)
			: base(GumpOffsetX, GumpOffsetY)
		{
			owner.CloseGump(typeof(WhoGump));

			m_Owner = owner;
			m_Mobiles = list;

			Initialize(page);
		}

		public static List<Mobile> BuildList(Mobile owner, string filter)
		{
			if (filter != null && (filter = filter.Trim()).Length == 0)
				filter = null;
			else
				filter = filter.ToLower();

			List<Mobile> list = new List<Mobile>();
			List<NetState> states = NetState.Instances;

			for (int i = 0; i < states.Count; ++i)
			{
				Mobile m = states[i].Mobile;

				if (m != null && (m == owner || !m.Hidden || owner.AccessLevel > m.AccessLevel || (m is PlayerMobile && ((PlayerMobile)m).VisibilityList.Contains(owner))))
				{
					if (filter != null && (m.Name == null || m.Name.ToLower().IndexOf(filter) < 0))
						continue;

					list.Add(m);
				}
			}

			list.Sort(InternalComparer.Instance);

			return list;
		}

		public void Initialize(int page)
		{
			m_Page = page;

			int count = m_Mobiles.Count - (page * EntryCount);

			if (count < 0)
				count = 0;
			else if (count > EntryCount)
				count = EntryCount;

			int totalHeight = OffsetSize + ((EntryHeight + OffsetSize) * (count + 1));

			AddPage(0);

			AddBackground(0, 0, BackWidth + GoWidth + BringWidth + (RaceEntryWidth + OffsetSize) + (TownEntryWidth + OffsetSize) + (GuildEntryWidth + OffsetSize), BorderSize + totalHeight + BorderSize, BackGumpID);
			AddImageTiled(BorderSize, BorderSize, TotalWidth - (OldStyle ? SetWidth + OffsetSize : 0), totalHeight, OffsetGumpID);

			int x = BorderSize + OffsetSize;
			int y = BorderSize + OffsetSize;

			int emptyWidth = TotalWidth - PrevWidth - NextWidth - (OffsetSize * 4) - (OldStyle ? SetWidth + OffsetSize : 0);

			if (!OldStyle)
				AddImageTiled(x - (OldStyle ? OffsetSize : 0), y, emptyWidth + (OldStyle ? OffsetSize * 2 : 0), EntryHeight, EntryGumpID);

			AddLabel(x + TextOffsetX, y, TextHue, String.Format("Page {0} of {1} ({2})", page + 1, (m_Mobiles.Count + EntryCount - 1) / EntryCount, m_Mobiles.Count));

			x += emptyWidth + OffsetSize;

			if (OldStyle)
				AddImageTiled(x, y, TotalWidth - (OffsetSize * 3) - SetWidth, EntryHeight, HeaderGumpID);
			else
				AddImageTiled(x, y, PrevWidth, EntryHeight, HeaderGumpID);

			if (page > 0)
			{
				AddButton(x + PrevOffsetX, y + PrevOffsetY, PrevButtonID1, PrevButtonID2, 1, GumpButtonType.Reply, 0);

				if (PrevLabel)
					AddLabel(x + PrevLabelOffsetX, y + PrevLabelOffsetY, TextHue, "Previous");
			}

			x += PrevWidth + OffsetSize;

			if (!OldStyle)
				AddImageTiled(x, y, NextWidth, EntryHeight, HeaderGumpID);

			if ((page + 1) * EntryCount < m_Mobiles.Count)
			{
				AddButton(x + NextOffsetX, y + NextOffsetY, NextButtonID1, NextButtonID2, 2, GumpButtonType.Reply, 1);

				if (NextLabel)
					AddLabel(x + NextLabelOffsetX, y + NextLabelOffsetY, TextHue, "Next");
			}

			// <nel>
			x += OffsetSize + SetWidth;

			AddImageTiled(x, y, (TownEntryWidth + OffsetSize) + RaceEntryWidth + (GuildEntryWidth + OffsetSize) + GoWidth + BringWidth + (3 * OffsetSize), EntryHeight, HeaderGumpID);

			// </nel>

			for (int i = 0, index = page * EntryCount; i < EntryCount && index < m_Mobiles.Count; ++i, ++index)
			{
				x = BorderSize + OffsetSize;
				y += EntryHeight + OffsetSize;

				Mobile m = m_Mobiles[index];


				// Gump postaci
				if (SetGumpID != 0)
					AddImageTiled(x, y, SetWidth, EntryHeight, SetGumpID);

				if (m.NetState != null && !m.Deleted)
					AddButton(x + SetOffsetX, y + SetOffsetY, SetButtonID1, SetButtonID2, i + 3, GumpButtonType.Reply, 0);

				// Imi�
				x += OffsetSize + SetWidth;

				AddImageTiled(x, y, EntryWidth, EntryHeight, EntryGumpID);
				AddLabelCropped(x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, GetHueFor(m), m.Deleted ? "(deleted)" : m.Name);

				// Go
				x += EntryWidth + OffsetSize;

				if (SetGumpID != 0)
					AddImageTiled(x, y, GoWidth, EntryHeight, SetGumpID);

				if (m.NetState != null && !m.Deleted)
					AddButton(x + SetOffsetX, y + SetOffsetY + 2, GoButtonID1, GoButtonID2, i + 3 + EntryCount, GumpButtonType.Reply, 0);

				// Rasa
				x += GoWidth + OffsetSize;

				AddImageTiled(x, y, RaceEntryWidth, EntryHeight, SetGumpID);
				AddLabelCropped(x + TextOffsetX, y, EntryWidth - TextOffsetX, EntryHeight, GetRaceHueFor(m), GetRaceFor(m));

				// Obywatelstwo
				x += RaceEntryWidth + OffsetSize;

				AddImageTiled(x, y, TownEntryWidth, EntryHeight, SetGumpID);
				AddLabelCropped(x + TextOffsetX + GetTownOffsetFor(m), y, EntryWidth - TextOffsetX, EntryHeight, GetTownHueFor(m), GetTownFor(m));

				x += TownEntryWidth + OffsetSize;

				//Gildia
				AddImageTiled(x, y, GuildEntryWidth, EntryHeight, SetGumpID);
				AddLabelCropped(x + TextOffsetX + GetGuildOffsetFor(m), y, EntryWidth - TextOffsetX, EntryHeight, GetGuildHueFor(m), GetGuildFor(m));

				x += GuildEntryWidth + OffsetSize;

				// Bring
				if (SetGumpID != 0)
					AddImageTiled(x, y, BringWidth, EntryHeight, SetGumpID);

				if (m.NetState != null && !m.Deleted)
					AddButton(x + SetOffsetX, y + SetOffsetY + 2, BringButtonID1, BringButtonID2, i + 3 + (2 * EntryCount), GumpButtonType.Reply, 0);
			}
		}

		private static int GetHueFor(Mobile m)
		{
			switch (m.AccessLevel)
			{
				case AccessLevel.Owner:
				case AccessLevel.Developer:
				case AccessLevel.Administrator: return 0x516;
				case AccessLevel.Seer: return 0x144;
				case AccessLevel.GameMaster: return 0x21;
				case AccessLevel.Counselor: return 0x2;
				case AccessLevel.Player:
				default:
					{
						if (m.Kills >= 5)
							return 0x21;
						else if (m.Criminal)
							return 0x3B1;

						return 0x58;
					}
			}
		}

		public override void OnResponse(NetState state, RelayInfo info)
		{
			Mobile from = state.Mobile;

			switch (info.ButtonID)
			{
				case 0: // Closed
					{
						return;
					}
				case 1: // Previous
					{
						if (m_Page > 0)
							from.SendGump(new WhoGump(from, m_Mobiles, m_Page - 1));

						break;
					}
				case 2: // Next
					{
						if ((m_Page + 1) * EntryCount < m_Mobiles.Count)
							from.SendGump(new WhoGump(from, m_Mobiles, m_Page + 1));

						break;
					}
				default:
					{

						if (info.ButtonID - 3 < EntryCount)
						{

							int index = (m_Page * EntryCount) + (info.ButtonID - 3);

							if (index >= 0 && index < m_Mobiles.Count)
							{
								Mobile m = (Mobile)m_Mobiles[index];

								if (m.Deleted)
								{
									from.SendMessage("That player has deleted their character.");
									from.SendGump(new WhoGump(from, m_Mobiles, m_Page));
								}
								else if (m.NetState == null)
								{
									from.SendMessage("That player is no longer online.");
									from.SendGump(new WhoGump(from, m_Mobiles, m_Page));
								}
								else if (m == m_Owner || !m.Hidden || m_Owner.AccessLevel > m.AccessLevel)
								{
									from.SendGump(new ClientGump(from, m.NetState));
								}
								else
								{
									from.SendMessage("You cannot see them.");
									from.SendGump(new WhoGump(from, m_Mobiles, m_Page));
								}
							}
						}
						else if (info.ButtonID - 3 < 2 * EntryCount)
						{
							int index = (m_Page * EntryCount) + (info.ButtonID - 3 - EntryCount);

							if (index >= 0 && index < m_Mobiles.Count)
							{
								Mobile m = (Mobile)m_Mobiles[index];

								if (m.Deleted)
								{
									from.SendMessage("That player has deleted their character.");
									from.SendGump(new WhoGump(from, m_Mobiles, m_Page));
								}
								else if (m.NetState == null)
								{
									from.SendMessage("That player is no longer online.");
									from.SendGump(new WhoGump(from, m_Mobiles, m_Page));
								}
								else if (m == m_Owner || !m.Hidden || m_Owner.AccessLevel > m.AccessLevel)
								{
									from.MoveToWorld(m.Location, m.Map);
								}
								else
								{
									from.SendMessage("You cannot see them.");
									from.SendGump(new WhoGump(from, m_Mobiles, m_Page));
								}
							}
						}
						else
						{
							int index = (m_Page * EntryCount) + (info.ButtonID - 3 - (2 * EntryCount));

							if (index >= 0 && index < m_Mobiles.Count)
							{
								Mobile m = (Mobile)m_Mobiles[index];

								if (m.Deleted)
								{
									from.SendMessage("That player has deleted their character.");
									from.SendGump(new WhoGump(from, m_Mobiles, m_Page));
								}
								else if (m.NetState == null)
								{
									from.SendMessage("That player is no longer online.");
									from.SendGump(new WhoGump(from, m_Mobiles, m_Page));
								}
								else if (m == m_Owner || !m.Hidden || m_Owner.AccessLevel > m.AccessLevel)
								{
									m.MoveToWorld(from.Location, from.Map);
								}
								else
								{
									from.SendMessage("You cannot see them.");
									from.SendGump(new WhoGump(from, m_Mobiles, m_Page));
								}
							}
						}


						break;
					}					
			}
		}

		#region TownConfig
		public static int GetTownHueFor(Mobile m)
		{
			if (m.Deleted)
				return 945;

			 /*			
			 * 0 czarny
			 * 88 blekit -
			 * 54 zolty -
			 * 342 pomarancz 
			 * 36 czerwony -
			 * 67 zielony -
			 * 902 szary
			 * 2306 bialy
			 * 
			 */

			if ( TownDatabase.IsCitizenOfWhichTown(m) == Towns.None )
				return 902;
			if ( TownDatabase.IsCitizenOfWhichTown(m) == Towns.Tasandora )
				return 84;
			if ( TownDatabase.IsCitizenOfWhichTown(m) == Towns.Garlan )
				return 51;
			if ( TownDatabase.IsCitizenOfWhichTown(m) == Towns.Twierdza )
				return 65;
			if ( TownDatabase.IsCitizenOfWhichTown(m) == Towns.Noamuth_Quortek )
				return 35;

			return 945;
		}

		private static int GetTownOffsetFor(Mobile m)
		{
			if (m.Deleted)
				return 0;

			return 0;
		}

		public static string GetTownFor(Mobile m)
		{
			if (m.Deleted)
				return "(deleted)";

            if (TownDatabase.IsCitizenOfAnyTown(m))
            {
                Towns fromIsCtizenWhere = TownDatabase.IsCitizenOfWhichTown(m);
                return fromIsCtizenWhere.ToString();
            }

			return "------";
		}
		#endregion

		#region RaceConfig
		public static int GetRaceHueFor(Mobile m)
		{
			if (m.Deleted)
				return 945;
			 /*			
			 * 0 czarny
			 * 88 blekit
			 * 54 zolty
			 * 342 pomarancz
			 * 36 czerwony
			 * 67 zielony
			 * 902 szary
			 * 2306 bialy
			 * 
			 */
			if ( m.Race == None.Instance)
				return 945;
			else if ( m.Race == Tamael.Instance )
				return 88;
			else if ( m.Race == Jarling.Instance )
				return 54;
			else if ( m.Race == Naur.Instance )
				return 36;
			//else if ( m.Race == Race.Rewianczyk )
			//	return 67;
			else if ( m.Race == Elf.Instance )
				return 945;
			else if ( m.Race == Drow.Instance )
				return 945;
			else if ( m.Race == Krasnolud.Instance )
				return 945;

			return 945;
		}

		private static int GetRaceOffsetFor(Mobile m)
		{
			if (m.Deleted)
				return 30;

			if ( m.Race == None.Instance)
				return 30;
			else if ( m.Race == Tamael.Instance )
				return 0;
			else if ( m.Race == Jarling.Instance)
				return 0; 
			else if ( m.Race == Naur.Instance)
				return 0;
			else if ( m.Race == Elf.Instance)
				return 0;
			else if ( m.Race == Drow.Instance)
				return 0;
			else if ( m.Race == Krasnolud.Instance)
				return 0;

			return 30;
		}

		public static string GetRaceFor(Mobile m)
		{
			if (m.Deleted)
				return "(deleted)";

			if ( m.Race == None.Instance)
				return "(brak)";
			else if ( m.Race == Tamael.Instance)
				return "Tamael";
			else if ( m.Race == Jarling.Instance)
				return "Jarling";
			else if ( m.Race == Naur.Instance)
				return "Naur";
			else if ( m.Race == Elf.Instance)
				return "Elf";
			else if ( m.Race == Drow.Instance)
				return "Drow";
			else if ( m.Race == Krasnolud.Instance)
				return "Krasnolud";

			return "(?????)";
		}
		#endregion

		#region GuildConfig
		private static int GetGuildOffsetFor(Mobile m)
		{
			return 0; //!!!!!!!!!!!!
			if (m.Deleted)
				return 23;

			if (m.Guild == null)
				return 13;

			return 15;
		}

		public static int GetGuildHueFor(Mobile m)
		{
			if (m.Deleted)
				return 0x3B1;

			if (m.Guild == null)
				return 0x3B1;

			return 167;
		}

		public static string GetGuildFor(Mobile m)
		{
			if (m.Deleted)
				return "(deleted)";

			if (m.Guild == null)
				return "( --- )";
			else
				return "[ " + m.Guild.Abbreviation + " ]";

		}
		#endregion
	}
}