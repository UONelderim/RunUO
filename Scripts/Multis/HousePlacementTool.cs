using System;
using System.Collections;
using Server;
using Server.Gumps;
using Server.Multis;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{
	public class HousePlacementTool : Item
	{
		public override int LabelNumber{ get{ return 1060651; } } // a house placement tool

		[Constructable]
		public HousePlacementTool() : base( 0x14F6 )
		{
			Weight = 3.0;
			LootType = LootType.Blessed;
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( IsChildOf( from.Backpack ) )
				from.SendGump( new HousePlacementCategoryGump( from ) );
			else
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
		}

		public HousePlacementTool( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( Weight == 0.0 )
				Weight = 3.0;
		}
	}

	public class HousePlacementCategoryGump : Gump
	{
		private Mobile m_From;

		private const int LabelColor = 0x7FFF;
		private const int LabelColorDisabled = 0x4210;

		public HousePlacementCategoryGump( Mobile from ) : base( 50, 50 )
		{
			m_From = from;

			from.CloseGump( typeof( HousePlacementCategoryGump ) );
			from.CloseGump( typeof( HousePlacementListGump ) );

			AddPage( 0 );

			AddBackground( 0, 0, 270, 145, 5054 );

			AddImageTiled( 10, 10, 250, 125, 2624 );
			AddAlphaRegion( 10, 10, 250, 125 );

			AddHtmlLocalized( 10, 10, 250, 20, 1060239, LabelColor, false, false ); // <CENTER>HOUSE PLACEMENT TOOL</CENTER>

			AddButton( 10, 110, 4017, 4019, 0, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 45, 110, 150, 20, 3000363, LabelColor, false, false ); // Close

			AddButton( 10, 40, 4005, 4007, 1, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 45, 40, 200, 20, 1060390, LabelColor, false, false ); // Classic Houses

			AddButton( 10, 60, 4005, 4007, 2, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 45, 60, 200, 20, 1060391, LabelColor, false, false ); // 2-Story Customizable Houses

			AddButton( 10, 80, 4005, 4007, 3, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 45, 80, 200, 20, 1060392, LabelColor, false, false ); // 3-Story Customizable Houses
		}

		public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
		{
			if ( !m_From.CheckAlive() || m_From.Backpack == null || m_From.Backpack.FindItemByType( typeof( HousePlacementTool ) ) == null  )
				return;

			switch ( info.ButtonID )
			{
				case 1: // Classic Houses
				{
					m_From.SendGump( new HousePlacementListGump( m_From, HousePlacementEntry.ClassicHouses ) );
					break;
				}
				case 2: // 2-Story Customizable Houses
				{
					m_From.SendGump( new HousePlacementListGump( m_From, HousePlacementEntry.TwoStoryFoundations ) );
					break;
				}
				case 3: // 3-Story Customizable Houses
				{
					m_From.SendGump( new HousePlacementListGump( m_From, HousePlacementEntry.ThreeStoryFoundations ) );
					break;
				}
			}
		}
	}

	public class HousePlacementListGump : Gump
	{
		private Mobile m_From;
		private HousePlacementEntry[] m_Entries;

		private const int LabelColor = 0x7FFF;
		private const int LabelHue = 0x480;

		public HousePlacementListGump( Mobile from, HousePlacementEntry[] entries ) : base( 50, 50 )
		{
			m_From = from;
			m_Entries = entries;

			from.CloseGump( typeof( HousePlacementCategoryGump ) );
			from.CloseGump( typeof( HousePlacementListGump ) );

			AddPage( 0 );

			AddBackground( 0, 0, 520, 420, 5054 );

			AddImageTiled( 10, 10, 500, 20, 2624 );
			AddAlphaRegion( 10, 10, 500, 20 );

			AddHtmlLocalized( 10, 10, 500, 20, 1060239, LabelColor, false, false ); // <CENTER>HOUSE PLACEMENT TOOL</CENTER>

			AddImageTiled( 10, 40, 500, 20, 2624 );
			AddAlphaRegion( 10, 40, 500, 20 );

			AddHtmlLocalized( 50, 40, 225, 20, 1060235, LabelColor, false, false ); // House Description
			AddHtmlLocalized( 275, 40, 75, 20, 1060236, LabelColor, false, false ); // Storage
			AddHtmlLocalized( 350, 40, 75, 20, 1060237, LabelColor, false, false ); // Lockdowns
			AddHtmlLocalized( 425, 40, 75, 20, 1060034, LabelColor, false, false ); // Cost

			AddImageTiled( 10, 70, 500, 280, 2624 );
			AddAlphaRegion( 10, 70, 500, 280 );

			AddImageTiled( 10, 360, 500, 20, 2624 );
			AddAlphaRegion( 10, 360, 500, 20 );

			AddHtmlLocalized( 10, 360, 250, 20, 1060645, LabelColor, false, false ); // Bank Balance:
			AddLabel( 250, 360, LabelHue, Banker.GetBalance( from ).ToString() );

			AddImageTiled( 10, 390, 500, 20, 2624 );
			AddAlphaRegion( 10, 390, 500, 20 );

			AddButton( 10, 390, 4017, 4019, 0, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 50, 390, 100, 20, 3000363, LabelColor, false, false ); // Close

			for ( int i = 0; i < entries.Length; ++i )
			{
				int page = 1 + (i / 14);
				int index = i % 14;

				if ( index == 0 )
				{
					if ( page > 1 )
					{
						AddButton( 450, 390, 4005, 4007, 0, GumpButtonType.Page, page );
						AddHtmlLocalized( 400, 390, 100, 20, 3000406, LabelColor, false, false ); // Next
					}

					AddPage( page );

					if ( page > 1 )
					{
						AddButton( 200, 390, 4014, 4016, 0, GumpButtonType.Page, page - 1 );
						AddHtmlLocalized( 250, 390, 100, 20, 3000405, LabelColor, false, false ); // Previous
					}
				}

				HousePlacementEntry entry = entries[i];

				int y = 70 + (index * 20);

				AddButton( 10, y, 4005, 4007, 1 + i, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 50, y, 225, 20, entry.Description, LabelColor, false, false );
				AddLabel( 275, y, LabelHue, entry.Storage.ToString() );
				AddLabel( 350, y, LabelHue, entry.Lockdowns.ToString() );
				AddLabel( 425, y, LabelHue, entry.Cost.ToString() );
			}
		}

		public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
		{
			if ( !m_From.CheckAlive() || m_From.Backpack == null || m_From.Backpack.FindItemByType( typeof( HousePlacementTool ) ) == null  )
				return;

			int index = info.ButtonID - 1;

			if ( index >= 0 && index < m_Entries.Length )
			{
				//if ( m_From.AccessLevel < AccessLevel.Administrator && BaseHouse.HasAccountHouse( m_From ) )
                if (m_From.AccessLevel < AccessLevel.Administrator && BaseHouse.HasHouse(m_From))
					m_From.SendLocalizedMessage( 501271 ); // You already own a house, you may not place another!
				else
					m_From.Target = new NewHousePlacementTarget( m_Entries, m_Entries[index] );
			}
			else
			{
				m_From.SendGump( new HousePlacementCategoryGump( m_From ) );
			}
		}
	}

	public class NewHousePlacementTarget : MultiTarget
	{
		private HousePlacementEntry m_Entry;
		private HousePlacementEntry[] m_Entries;

		private bool m_Placed;

		public NewHousePlacementTarget( HousePlacementEntry[] entries, HousePlacementEntry entry ) : base( entry.MultiID, entry.Offset )
		{
			Range = 14;

			m_Entries = entries;
			m_Entry = entry;
		}

		protected override void OnTarget( Mobile from, object o )
		{
			if ( !from.CheckAlive() || from.Backpack == null || from.Backpack.FindItemByType( typeof( HousePlacementTool ) ) == null  )
				return;

			IPoint3D ip = o as IPoint3D;

			if ( ip != null )
			{
				if ( ip is Item )
					ip = ((Item)ip).GetWorldTop();

				Point3D p = new Point3D( ip );

				Region reg = Region.Find( new Point3D( p ), from.Map );

				if ( from.AccessLevel >= AccessLevel.Administrator || reg.AllowHousing( from, p ) )
					m_Placed = m_Entry.OnPlacement( from, p );
				else if ( reg.IsPartOf( typeof( TreasureRegion ) ) )
					from.SendLocalizedMessage( 1043287 ); // The house could not be created here.  Either something is blocking the house, or the house would not be on valid terrain.
				else
					from.SendLocalizedMessage( 501265 ); // Housing can not be created in this area.
			}
		}

		protected override void OnTargetFinish( Mobile from )
		{
			if ( !from.CheckAlive() || from.Backpack == null || from.Backpack.FindItemByType( typeof( HousePlacementTool ) ) == null  )
				return;

			if ( !m_Placed )
				from.SendGump( new HousePlacementListGump( from, m_Entries ) );
		}
	}

	public class HousePlacementEntry
	{
		private Type m_Type;
		private int m_Description;
		private int m_Storage;
		private int m_Lockdowns;
		private int m_NewStorage;
		private int m_NewLockdowns;
		private int m_Vendors;
		private int m_Cost;
		private int m_MultiID;
		private Point3D m_Offset;

		public Type Type{ get{ return m_Type; } }

		public int Description{ get{ return m_Description; } }
		public int Storage{ get{ return BaseHouse.NewVendorSystem ? m_NewStorage : m_Storage; } }
		public int Lockdowns{ get{ return BaseHouse.NewVendorSystem ? m_NewLockdowns : m_Lockdowns; } }
		public int Vendors{ get{ return m_Vendors; } }
		public int Cost{ get{ return m_Cost; } }

		public int MultiID{ get{ return m_MultiID; } }
		public Point3D Offset{ get{ return m_Offset; } }

		public HousePlacementEntry( Type type, int description, int storage, int lockdowns, int newStorage, int newLockdowns, int vendors, int cost, int xOffset, int yOffset, int zOffset, int multiID )
		{
			m_Type = type;
			m_Description = description;
			m_Storage = storage;
			m_Lockdowns = lockdowns;
			m_NewStorage = newStorage;
			m_NewLockdowns = newLockdowns;
			m_Vendors = vendors;
			m_Cost = cost;

			m_Offset = new Point3D( xOffset, yOffset, zOffset );

			m_MultiID = multiID;
		}

		public BaseHouse ConstructHouse( Mobile from )
		{
			try
			{
				object[] args;

				if ( m_Type == typeof( HouseFoundation ) )
					args = new object[4]{ from, m_MultiID, m_Storage, m_Lockdowns };
				else if ( m_Type == typeof( SmallOldHouse ) || m_Type == typeof( SmallShop ) || m_Type == typeof( TwoStoryHouse ) )
					args = new object[2]{ from, m_MultiID };
				else
					args = new object[1]{ from };

				return Activator.CreateInstance( m_Type, args ) as BaseHouse;
			}
			catch
			{
			}

			return null;
		}

		public void PlacementWarning_Callback( Mobile from, bool okay, object state )
		{
			if ( !from.CheckAlive() || from.Backpack == null || from.Backpack.FindItemByType( typeof( HousePlacementTool ) ) == null  )
				return;

			PreviewHouse prevHouse = (PreviewHouse)state;

			if ( !okay )
			{
				prevHouse.Delete();
				return;
			}

			if ( prevHouse.Deleted )
			{
				/* Too much time has passed and the test house you created has been deleted.
				 * Please try again!
				 */
				from.SendGump( new NoticeGump( 1060637, 30720, 1060647, 32512, 320, 180, null, null ) );

				return;
			}

			Point3D center = prevHouse.Location;
			Map map = prevHouse.Map;

			prevHouse.Delete();

			ArrayList toMove;
			//Point3D center = new Point3D( p.X - m_Offset.X, p.Y - m_Offset.Y, p.Z - m_Offset.Z );
			HousePlacementResult res = HousePlacement.Check( from, m_MultiID, center, out toMove );

			switch ( res )
			{
				case HousePlacementResult.Valid:
				{
					//if ( from.AccessLevel < AccessLevel.Administrator && BaseHouse.HasAccountHouse( from ) )
                    if (from.AccessLevel < AccessLevel.Administrator && BaseHouse.HasHouse(from))
					{
						from.SendLocalizedMessage( 501271 ); // You already own a house, you may not place another!
					}
					else
                    {
                        BaseHouse house = ConstructHouse( from );

                        if ( house == null )
                            return;

                        house.Price = m_Cost;

                        if ( from.AccessLevel >= AccessLevel.Administrator )
                        {
                            from.SendMessage( "{0} gold would have been withdrawn from your bank if you were not a GM.", m_Cost.ToString() );
                        }
                        else
                        {
                            if ( Banker.Withdraw( from, m_Cost ) )
                            {
                                from.SendLocalizedMessage( 1060398, m_Cost.ToString() ); // ~1_AMOUNT~ gold has been withdrawn from your bank box.
                            }
                            else
                            {
                                house.RemoveKeys( from );
                                house.Delete();
                                from.SendLocalizedMessage( 1060646 ); // You do not have the funds available in your bank box to purchase this house.  Try placing a smaller house, or adding gold or checks to your bank box.
                                return;
                            }
                        }

                        house.MoveToWorld( center, from.Map );

                        for ( int i = 0; i < toMove.Count; ++i )
                        {
                            object o = toMove[i];

                            if ( o is Mobile )
                                ( (Mobile)o ).Location = house.BanLocation;
                            else if ( o is Item )
                                ( (Item)o ).Location = house.BanLocation;
                        }
                    }

					break;
				}
				case HousePlacementResult.BadItem:
				case HousePlacementResult.BadLand:
				case HousePlacementResult.BadStatic:
				case HousePlacementResult.BadRegionHidden:
				case HousePlacementResult.NoSurface:
				{
					from.SendLocalizedMessage( 1043287 ); // The house could not be created here.  Either something is blocking the house, or the house would not be on valid terrain.
					break;
				}
				case HousePlacementResult.BadRegion:
				{
					from.SendLocalizedMessage( 501265 ); // Housing cannot be created in this area.
					break;
				}
				case HousePlacementResult.BadRegionTemp:
				{
					from.SendLocalizedMessage( 501270 ); //Lord British has decreed a 'no build' period, thus you cannot build this house at this time.
					break;
				}
			}
		}

		public bool OnPlacement( Mobile from, Point3D p )
		{
			if ( !from.CheckAlive() || from.Backpack == null || from.Backpack.FindItemByType( typeof( HousePlacementTool ) ) == null  )
				return false;

			ArrayList toMove;
			Point3D center = new Point3D( p.X - m_Offset.X, p.Y - m_Offset.Y, p.Z - m_Offset.Z );
			HousePlacementResult res = HousePlacement.Check( from, m_MultiID, center, out toMove );

			switch ( res )
			{
				case HousePlacementResult.Valid:
				{
					//if ( from.AccessLevel < AccessLevel.Administrator && BaseHouse.HasAccountHouse( from ) )
                    if (from.AccessLevel < AccessLevel.Administrator && BaseHouse.HasHouse(from))
					{
						from.SendLocalizedMessage( 501271 ); // You already own a house, you may not place another!
					}
					else
					{
						from.SendLocalizedMessage( 1011576 ); // This is a valid location.

						PreviewHouse prev = new PreviewHouse( m_MultiID );

						MultiComponentList mcl = prev.Components;

						Point3D banLoc = new Point3D( center.X + mcl.Min.X, center.Y + mcl.Max.Y + 1, center.Z );

						for ( int i = 0; i < mcl.List.Length; ++i )
						{
							MultiTileEntry entry = mcl.List[i];

							int itemID = entry.m_ItemID & 0x3FFF;

							if ( itemID >= 0xBA3 && itemID <= 0xC0E )
							{
								banLoc = new Point3D( center.X + entry.m_OffsetX, center.Y + entry.m_OffsetY, center.Z );
								break;
							}
						}

						for ( int i = 0; i < toMove.Count; ++i )
						{
							object o = toMove[i];

							if ( o is Mobile )
								((Mobile)o).Location = banLoc;
							else if ( o is Item )
								((Item)o).Location = banLoc;
						}

						prev.MoveToWorld( center, from.Map );

						/* You are about to place a new house.
						 * Placing this house will condemn any and all of your other houses that you may have.
						 * All of your houses on all shards will be affected.
						 * 
						 * In addition, you will not be able to place another house or have one transferred to you for one (1) real-life week.
						 * 
						 * Once you accept these terms, these effects cannot be reversed.
						 * Re-deeding or transferring your new house will not uncondemn your other house(s) nor will the one week timer be removed.
						 * 
						 * If you are absolutely certain you wish to proceed, click the button next to OKAY below.
						 * If you do not wish to trade for this house, click CANCEL.
						 */
						from.SendGump( new WarningGump( 1060635, 30720, 1049583, 32512, 420, 280, new WarningGumpCallback( PlacementWarning_Callback ), prev ) );

						return true;
					}

					break;
				}
				case HousePlacementResult.BadItem:
				case HousePlacementResult.BadLand:
				case HousePlacementResult.BadStatic:
				case HousePlacementResult.BadRegionHidden:
				case HousePlacementResult.NoSurface:
				{
					from.SendLocalizedMessage( 1043287 ); // The house could not be created here.  Either something is blocking the house, or the house would not be on valid terrain.
					break;
				}
				case HousePlacementResult.BadRegion:
				{
					from.SendLocalizedMessage( 501265 ); // Housing cannot be created in this area.
					break;
				}
				case HousePlacementResult.BadRegionTemp:
				{
					from.SendLocalizedMessage( 501270 ); // Lord British has decreed a 'no build' period, thus you cannot build this house at this time.
					break;
				}
			}

			return false;
		}

		private static Hashtable m_Table;

		static HousePlacementEntry()
		{
			m_Table = new Hashtable();

			FillTable( m_ClassicHouses );
			FillTable( m_TwoStoryFoundations );
			FillTable( m_ThreeStoryFoundations );
		}

		public static HousePlacementEntry Find( BaseHouse house )
		{
			object obj = m_Table[house.GetType()];

			if ( obj is HousePlacementEntry )
			{
				return ((HousePlacementEntry)obj);
			}
			else if ( obj is ArrayList )
			{
				ArrayList list = (ArrayList)obj;

				for ( int i = 0; i < list.Count; ++i )
				{
					HousePlacementEntry e = (HousePlacementEntry)list[i];

					if ( e.m_MultiID == (house.ItemID & 0x3FFF) )
						return e;
				}
			}
			else if ( obj is Hashtable )
			{
				Hashtable table = (Hashtable)obj;

				obj = table[house.ItemID & 0x3FFF];

				if ( obj is HousePlacementEntry )
					return (HousePlacementEntry)obj;
			}

			return null;
		}

		private static void FillTable( HousePlacementEntry[] entries )
		{
			for ( int i = 0; i < entries.Length; ++i )
			{
				HousePlacementEntry e = entries[i];

				object obj = m_Table[e.m_Type];

				if ( obj == null )
				{
					m_Table[e.m_Type] = e;
				}
				else if ( obj is HousePlacementEntry )
				{
					ArrayList list = new ArrayList();

					list.Add( obj );
					list.Add( e );

					m_Table[e.m_Type] = list;
				}
				else if ( obj is ArrayList )
				{
					ArrayList list = (ArrayList)obj;

					if ( list.Count == 8 )
					{
						Hashtable table = new Hashtable();

						for ( int j = 0; j < list.Count; ++j )
							table[((HousePlacementEntry)list[j]).m_MultiID] = list[j];

						table[e.m_MultiID] = e;

						m_Table[e.m_Type] = table;
					}
					else
					{
						list.Add( e );
					}
				}
				else if ( obj is Hashtable )
				{
					((Hashtable)obj)[e.m_MultiID] = e;
				}
			}
		}

		private static HousePlacementEntry[] m_ClassicHouses = new HousePlacementEntry[]
			{
				new HousePlacementEntry( typeof( SmallOldHouse ),		1011303,	300,	130,	300,	130,	3,	130000,		0,	4,	0,	0x0064	),	// Stone and plaster house
				new HousePlacementEntry( typeof( SmallOldHouse ),		1011304,	300,	130,	300,	130,	3,	130000,		0,	4,	0,	0x0066	),	// Field stone house
				new HousePlacementEntry( typeof( SmallOldHouse ),		1011305,	300,	130,	300,	130,	3,	130000,		0,	4,	0,	0x0068	),	// Small brick house
				new HousePlacementEntry( typeof( SmallOldHouse ),		1011306,	300,	130,	300,	130,	3,	130000,		0,	4,	0,	0x006A	),	// Wooden house
				new HousePlacementEntry( typeof( SmallOldHouse ),		1011307,	300,	130,	300,	130,	3,	130000,		0,	4,	0,	0x006C	),	// Wood and plaster house
				new HousePlacementEntry( typeof( SmallOldHouse ),		1011308,	300,	212,	300,	130,	3,	130000,		0,	4,	0,	0x006E	),	// Thatched-roof cottage
				new HousePlacementEntry( typeof( SmallShop ),			1011321,	500,	260,	500,	260,	5,	220000,	   -1,	4,	0,	0x00A0	),	// Small stone workshop
				new HousePlacementEntry( typeof( SmallShop ),			1011322,	550,	275,	550,	275,	5,	250000,		0,	4,	0,	0x00A2	),	// Small marble workshop
				new HousePlacementEntry( typeof( SmallTower ),			1011317,	670,	335,	670,	335,	6,	310000,		3,	4,	0,	0x0098	),	// Small stone tower
				new HousePlacementEntry( typeof( SandStonePatio ),		1011320,	1200,	600,	1200,	600,	8,	560000,	   -1,	4,	0,	0x009C	),	// Sandstone house with patio
				new HousePlacementEntry( typeof( LogCabin ),			1011318,	1200,	600,	1200,	600,	8,	560000,		1,	6,	0,	0x009A	),	// Two-story log cabin
				new HousePlacementEntry( typeof( TwoStoryVilla ),		1011319,	1200,	700,	1400,	700,	8,	660000,		3,	6,	0,	0x009E	),	// Two-story villa
				new HousePlacementEntry( typeof( GuildHouse ),			1011309,	1600,	800,	1600,	800,	10,	660000,	   -1,	7,	0,	0x0074	),	// Brick house
				new HousePlacementEntry( typeof( TwoStoryHouse ),		1011310,	1800,	900,	1800,	900,	10,	780000,	   -3,	7,	0,	0x0076	),	// Two-story wood and plaster house
				new HousePlacementEntry( typeof( TwoStoryHouse ),		1011311,	1800,	900,	1800,	900,	10,	780000,	   -3,	7,	0,	0x0078	),	// Two-story stone and plaster house
				new HousePlacementEntry( typeof( LargePatioHouse ),		1011315,	2000,	1000,	2000,	1000,	10,	1000000,	   -4,	7,	0,	0x008C	),	// Large house with patio
				new HousePlacementEntry( typeof( LargeMarbleHouse ),	1011316,	2000,	1000,	2000,	1000,	10,	1000000,	   -4,	7,	0,	0x0096	),	// Marble house with patio
				new HousePlacementEntry( typeof( Tower ),				1011312,	2500,	1250,	2500,	1250,	12,	1500000,		0,	7,	0,	0x007A	),	// Tower
				new HousePlacementEntry( typeof( Keep ),				1011313,	3400,	1700,	3400,	1700,	15,	2000000,		0, 11,	0,	0x007C	),	// Small stone keep
				new HousePlacementEntry( typeof( Castle ),				1011314,	4800,	2400,	4800,	2400,	20,	3000000,		0, 16,	0,	0x007E	)	// Castle
			};

		public static HousePlacementEntry[] ClassicHouses{ get{ return m_ClassicHouses; } }

		

		private static HousePlacementEntry[] m_TwoStoryFoundations = new HousePlacementEntry[]
			{
				new HousePlacementEntry( typeof( HouseFoundation ),		1060241,	300,	140,	300,	140,	3,	80000,		0,	4,	0,	0x13EC	), // 7x7 2-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060242,	400,	180,	400,	180,	4,	100000,		0,	5,	0,	0x13ED	), // 7x8 2-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060243,	500,	250,	500,	250,	5,	130000,		0,	5,	0,	0x13EE	), // 7x9 2-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060244,	600,	300,	600,	300,	5,	160000,		0,	6,	0,	0x13EF	), // 7x10 2-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060245,	700,	350,	700,	350,	5,	190000,		0,	6,	0,	0x13F0	), // 7x11 2-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060246,	800,	400,	800,	400,	6,	220000,		0,	7,	0,	0x13F1	), // 7x12 2-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060253,	400,	160,	400,	160,	4,	100000,		0,	4,	0,	0x13F8	), // 8x7 2-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060254,	500,	250,	500,	250,	5,	135000,		0,	5,	0,	0x13F9	), // 8x8 2-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060255,	600,	300,	600,	300,	5,	165000,		0,	5,	0,	0x13FA	), // 8x9 2-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060256,	800,	400,	800,	400,	5,	220000,		0,	6,	0,	0x13FB	), // 8x10 2-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060257,	900,	450,	900,	450,	6,	250000,		0,	6,	0,	0x13FC	), // 8x11 2-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060258,	1000,	500,	1000,	500,	8,	280000,		0,	7,	0,	0x13FD	), // 8x12 2-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060259,	1100,	550,	1100,	550,	8,	310000,		0,	7,	0,	0x13FE	), // 8x13 2-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060265,	500,	250,	500,	250,	5,	135000,		0,	4,	0,	0x1404	), // 9x7 2-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060266,	600,	300,	600,	300,	5,	165000,		0,	5,	0,	0x1405	), // 9x8 2-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060267,	800,	400,	800,	400,	5,	220000,		0,	5,	0,	0x1406	), // 9x9 2-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060268,	900,	450,	900,	450,	6,	250000,		0,	6,	0,	0x1407	), // 9x10 2-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060269,	1100,	550,	1100,	550,	8,	310000,		0,	6,	0,	0x1408	), // 9x11 2-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060270,	1200,	600,	1200,	600,	8,	340000,		0,	7,	0,	0x1409	), // 9x12 2-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060271,	1400,	700,	1400,	700,	8,	400000,		0,	7,	0,	0x140A	), // 9x13 2-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060277,	600,	300,	600,	300,	5,	160000,		0,	4,	0,	0x1410	), // 10x7 2-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060278,	800,	400,	800,	400,	5,	220000,		0,	5,	0,	0x1411	), // 10x8 2-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060279,	900,	450,	900,	450,	6,	250000,		0,	5,	0,	0x1412	), // 10x9 2-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060280,	1100,	550,	1100,	550,	8,	310000,		0,	6,	0,	0x1413	), // 10x10 2-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060281,	1200,	600,	1200,	600,	8,	340000,		0,	6,	0,	0x1414	), // 10x11 2-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060282,	1300,	650,	1300,	650,	8,	370000,		0,	7,	0,	0x1415	), // 10x12 2-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060283,	1500,	750,	1500,	750,	8,	430000,		0,	7,	0,	0x1416	), // 10x13 2-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060289,	700,	350,	700,	350,	5,	190000,		0,	4,	0,	0x141C	), // 11x7 2-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060290,	900,	450,	900,	450,	6,	250000,		0,	5,	0,	0x141D	), // 11x8 2-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060291,	1100,	550,	1100,	550,	8,	310000,		0,	5,	0,	0x141E	), // 11x9 2-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060292,	1200,	600,	1200,	600,	8,	340000,		0,	6,	0,	0x141F	), // 11x10 2-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060293,	1300,	650,	1300,	650,	8,	370000,		0,	6,	0,	0x1420	), // 11x11 2-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060294,	1500,	750,	1500,	750,	8,	420000,		0,	7,	0,	0x1421	), // 11x12 2-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060295,	1600,	800,	1600,	800,	10,	460000,		0,	7,	0,	0x1422	), // 11x13 2-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060301,	800,	400,	800,	400,	6,	220000,		0,	4,	0,	0x1428	), // 12x7 2-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060302,	1000,	500,	1000,	500,	8,	280000,		0,	5,	0,	0x1429	), // 12x8 2-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060303,	1200,	600,	1200,	600,	8,	340000,		0,	5,	0,	0x142A	), // 12x9 2-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060304,	1300,	650,	1300,	650,	8,	370000,		0,	6,	0,	0x142B	), // 12x10 2-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060305,	1500,	750,	1500,	750,	8,	420000,		0,	6,	0,	0x142C	), // 12x11 2-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060306,	1600,	800,	1600,	800,	10,	460000,		0,	7,	0,	0x142D	), // 12x12 2-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060307,	1700,	850,	1700,	850,	10,	500000,		0,	7,	0,	0x142E	), // 12x13 2-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060314,	1100,	550,	1100,	550,	8,	310000,		0,	5,	0,	0x1435	), // 13x8 2-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060315,	1400,	700,	1400,	700,	8,	400000,		0,	5,	0,	0x1436	), // 13x9 2-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060316,	1500,	750,	1500,	750,	8,	430000,		0,	6,	0,	0x1437	), // 13x10 2-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060317,	1600,	800,	1600,	800,	10,	460000,		0,	6,	0,	0x1438	), // 13x11 2-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060318,	1700,	850,	1700,	850,	10,	500000,		0,	7,	0,	0x1439	), // 13x12 2-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060319,	1800,	900,	1800,	900,	12, 540000,		0,	7,	0,	0x143A	)  // 13x13 2-Story Customizable House
			};

		public static HousePlacementEntry[] TwoStoryFoundations{ get{ return m_TwoStoryFoundations; } }





		private static HousePlacementEntry[] m_ThreeStoryFoundations = new HousePlacementEntry[]
			{
				new HousePlacementEntry( typeof( HouseFoundation ),		1060272,	1500,	750,	1500,	750,	8,	450000,		0,	8,	0,	0x140B	), // 9x14 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060284,	1600,	800,	1600,	800,	10,	490000,		0,	8,	0,	0x1417	), // 10x14 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060285,	1700,	850,	1700,	850,	10,	530000,		0,	8,	0,	0x1418	), // 10x15 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060296,	1800,	900,	1800,	900,	10,	550000,		0,	8,	0,	0x1423	), // 11x14 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060297,	1900,	950,	1900,	950,	12,	590000,		0,	8,	0,	0x1424	), // 11x15 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060298,	2000,	1000,	2000,	1000,	12,	630000,		0,	9,	0,	0x1425	), // 11x16 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060308,	1900,	950,	1900,	950,	12,	600000,		0,	8,	0,	0x142F	), // 12x14 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060309,	2000,	1000,	2000,	1000,	12,	640000,		0,	8,	0,	0x1430	), // 12x15 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060310,	2100,	1050,	2100,	1050,	12,	670000,		0,	9,	0,	0x1431	), // 12x16 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060311,	2200,	1100,	2200,	1100,	12,	700000,		0,	9,	0,	0x1432	), // 12x17 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060320,	2000,	1000,	2000,	1000,	12,	650000,		0,	8,	0,	0x143B	), // 13x14 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060321,	2100,	1050,	2100,	1050,	12,	690000,		0,	8,	0,	0x143C	), // 13x15 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060322,	2200,	1100,	2200,	1100,	12,	730000,		0,	9,	0,	0x143D	), // 13x16 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060323,	2300,	1150,	2300,	1150,	15,	770000,		0,	9,	0,	0x143E	), // 13x17 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060324,	2400,	1200,	2437,	1200,	15,	800000,		0,	10,	0,	0x143F	), // 13x18 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060327,	1500,	750,	1800,	750,	8,	450000,		0,	5,	0,	0x1442	), // 14x9 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060328,	1600,	800,	1600,	800,	10,	490000,		0,	6,	0,	0x1443	), // 14x10 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060329,	1800,	900,	1800,	900,	10,	550000,		0,	6,	0,	0x1444	), // 14x11 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060330,	1900,	950,	1900,	950,	12,	600000,		0,	7,	0,	0x1445	), // 14x12 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060331,	2000,	1000,	2000,	1000,	12,	650000,		0,	7,	0,	0x1446	), // 14x13 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060332,	2200,	1100,	2200,	1100,	12,	700000,		0,	8,	0,	0x1447	), // 14x14 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060333,	2300,	1150,	2300,	1150,	12,	750000,		0,	8,	0,	0x1448	), // 14x15 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060334,	2400,	1200,	2400,	1200,	15,	800000,		0,	9,	0,	0x1449	), // 14x16 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060335,	2500,	1250,	2500,	1250,	15,	850000,		0,	9,	0,	0x144A	), // 14x17 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060336,	2600,	1300,	2600,	1300,	15,	900000,		0,	10,	0,	0x144B	), // 14x18 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060340,	1700,	850,	1700,	850,	10,	530000,		0,	6,	0,	0x144F	), // 15x10 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060341,	1900,	950,	1900,	950,	12,	590000,		0,	6,	0,	0x1450	), // 15x11 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060342,	2000,	1000,	2000,	1000,	12,	640000,		0,	7,	0,	0x1451	), // 15x12 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060343,	2100,	1050,	2100,	1050,	12,	690000,		0,	7,	0,	0x1452	), // 15x13 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060344,	2300,	1150,	2300,	1150,	12,	750000,		0,	8,	0,	0x1453	), // 15x14 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060345,	2500,	1250,	2500,	1250,	15,	820000,		0,	8,	0,	0x1454	), // 15x15 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060346,	2600,	1300,	2600,	13000,	15,	870000,		0,	9,	0,	0x1455	), // 15x16 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060347,	2700,	1350,	2700,	1350,	15,	930000,		0,	9,	0,	0x1456	), // 15x17 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060348,	2900,	1450,	2900,	1450,	15,	1000000,		0,	10,	0,	0x1457	), // 15x18 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060353,	2000,	1000,	2000,	1000,	12,	630000,		0,	6,	0,	0x145C	), // 16x11 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060354,	2100,	1050,	2100,	1050,	12,	670000,		0,	7,	0,	0x145D	), // 16x12 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060355,	2200,	1100,	2200,	1100,	12,	730000,		0,	7,	0,	0x145E	), // 16x13 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060356,	2400,	1200,	2400,	1200,	15,	800000,		0,	8,	0,	0x145F	), // 16x14 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060357,	2600,	1300,	2600,	1300,	15,	930000,		0,	8,	0,	0x1460	), // 16x15 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060358,	2800,	1400,	2800,	1400,	15,	990000,		0,	9,	0,	0x1461	), // 16x16 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060359,	3000,	1500,	3000,	1500,	15,	1060000,		0,	9,	0,	0x1462	), // 16x17 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060360,	3200,	1600,	3200,	1218,	15,	1120000,		0,	10,	0,	0x1463	), // 16x18 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060366,	2200,	1100,	2200,	1100,	12,	700000,		0,	7,	0,	0x1469	), // 17x12 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060367,	2300,	1150,	2300,	1150,	15,	770000,		0,	7,	0,	0x146A	), // 17x13 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060368,	2500,	1250,	2500,	1250,	15,	850000,		0,	8,	0,	0x146B	), // 17x14 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060369,	2700,	1350,	2700,	1350,	15,	930000,		0,	8,	0,	0x146C	), // 17x15 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060370,	3000,	1500,	3000,	1500,	15,	1060000,		0,	9,	0,	0x146D	), // 17x16 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060371,	3300,	1650,	3300,	1650,	15,	1180000,		0,	9,	0,	0x146E	), // 17x17 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060372,	3600,	1800,	3600,	1800,	15,	1280000,		0,	10,	0,	0x146F	), // 17x18 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060379,	2400,	1059,	2437,	2400,	15,	800000,		0,	7,	0,	0x1476	), // 18x13 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060380,	2600,	1059,	2600,	2600,	15,	900000,		0,	8,	0,	0x1477	), // 18x14 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060381,	2900,	1059,	2900,	2900,	15,	1000000,		0,	8,	0,	0x1478	), // 18x15 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060382,	3200,	1059,	3200,	3200,	15,	1120000,		0,	9,	0,	0x1479	), // 18x16 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060383,	3600,	1059,	3600,	3600,	15,	1280000,		0,	9,	0,	0x147A	), // 18x17 3-Story Customizable House
				new HousePlacementEntry( typeof( HouseFoundation ),		1060384,	4000,	1059,	4000,	4000,	15,	1400000,		0,	10,	0,	0x147B	)  // 18x18 3-Story Customizable House
			};

		public static HousePlacementEntry[] ThreeStoryFoundations{ get{ return m_ThreeStoryFoundations; } }
	}
}