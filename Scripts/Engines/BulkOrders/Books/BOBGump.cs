using System;
using System.Collections;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Prompts;

namespace Server.Engines.BulkOrders
{
	public class BOBGump : Gump
	{
		private PlayerMobile m_From;
		private BulkOrderBook m_Book;
		private ArrayList m_List;

		private int m_Page;

		private const int LabelColor = 0x7FFF;

		public Item Reconstruct( object obj )
		{
			Item item = null;

			if ( obj is BOBLargeEntry )
				item = ((BOBLargeEntry)obj).Reconstruct();
			else if ( obj is BOBSmallEntry )
				item = ((BOBSmallEntry)obj).Reconstruct();

			return item;
		}

		public bool CheckFilter( object obj )
		{
			if ( obj is BOBLargeEntry )
			{
				BOBLargeEntry e = (BOBLargeEntry)obj;

				return CheckFilter( e.Material, e.Material2, e.AmountMax, true, e.RequireExceptional, e.DeedType, ( e.Entries.Length > 0 ? e.Entries[0].ItemType : null ) );
			}
			else if ( obj is BOBSmallEntry )
			{
				BOBSmallEntry e = (BOBSmallEntry)obj;

				return CheckFilter( e.Material, e.Material2, e.AmountMax, false, e.RequireExceptional, e.DeedType, e.ItemType );
			}

			return false;
		}

		public bool CheckFilter( BulkMaterialType mat, BulkMaterialType mat2, int amountMax, bool isLarge, bool reqExc, BODType deedType, Type itemType )
		{
			BOBFilter f = ( m_From.UseOwnFilter ? m_From.BOBFilter : m_Book.Filter );

			if ( f.IsDefault )
				return true;

			if ( f.Quality == 1 && reqExc )
				return false;
			else if ( f.Quality == 2 && !reqExc )
				return false;

			if ( f.Quantity == 1 && amountMax != 10 )
				return false;
			else if ( f.Quantity == 2 && amountMax != 15 )
				return false;
			else if ( f.Quantity == 3 && amountMax != 20 )
				return false;

			if ( f.Type == 1 && isLarge )
				return false;
			else if ( f.Type == 2 && !isLarge )
				return false;

			switch ( f.Material )
			{
				case  1: if( deedType != BODType.Smith ) return false; break;
				case  2: if( deedType != BODType.Tailor) return false; break;
				case 17: if( deedType != BODType.Fletcher) return false; break;

				case  3: if( mat != BulkMaterialType.None || BGTClassifier.Classify( deedType, itemType ) != BulkGenericType.Iron) return false; break;
				case  4: if( mat != BulkMaterialType.DullCopper) return false; break;
				case  5: if( mat != BulkMaterialType.ShadowIron) return false; break;
				case  6: if( mat != BulkMaterialType.Copper) return false; break;
				case  7: if( mat != BulkMaterialType.Bronze) return false; break;
				case  8: if( mat != BulkMaterialType.Gold) return false; break;
				case  9: if( mat != BulkMaterialType.Agapite) return false; break;
				case 10: if( mat != BulkMaterialType.Verite) return false; break;
				case 11: if( mat != BulkMaterialType.Valorite) return false; break;

				case 12: if( mat != BulkMaterialType.None || BGTClassifier.Classify( deedType, itemType ) != BulkGenericType.Cloth) return false; break;
				case 13: if( mat != BulkMaterialType.None || BGTClassifier.Classify( deedType, itemType ) != BulkGenericType.Leather) return false; break;
				case 14: if( mat != BulkMaterialType.Spined) return false; break;
				case 15: if( mat != BulkMaterialType.Horned) return false; break;
				case 16: if( mat != BulkMaterialType.Barbed) return false; break;

				case 18: if( mat != BulkMaterialType.None || BGTClassifier.Classify( deedType, itemType ) != BulkGenericType.Wood) return false; break;
				case 19: if( mat != BulkMaterialType.Oak) return false; break;
				case 20: if( mat != BulkMaterialType.Ash) return false; break;
				case 21: if( mat != BulkMaterialType.Yew) return false; break;
				case 22: if( mat != BulkMaterialType.Heartwood) return false; break;
				case 23: if( mat != BulkMaterialType.Bloodwood) return false; break;
				case 24: if( mat != BulkMaterialType.Frostwood) return false; break;
			}

			switch (f.Material2) {
				case 1: if (mat2 != BulkMaterialType.BowstringLeather) return false; break;
				case 2: if (mat2 != BulkMaterialType.BowstringGut) return false; break;
				case 3: if (mat2 != BulkMaterialType.BowstringCannabis) return false; break;
				case 4: if (mat2 != BulkMaterialType.BowstringSilk) return false; break;
			}
			return true;
		}

		public int GetIndexForPage( int page )
		{
			int index = 0;

			while ( page-- > 0 )
				index += GetCountForIndex( index );

			return index;
		}

		public int GetCountForIndex( int index )
		{
			int slots = 0;
			int count = 0;

			ArrayList list = m_List;

			for ( int i = index; i >= 0 && i < list.Count; ++i )
			{
				object obj = list[i];

				if ( CheckFilter( obj ) )
				{
					int add;

					if ( obj is BOBLargeEntry )
						add = ((BOBLargeEntry)obj).Entries.Length;
					else
						add = 1;

					if ( (slots + add) > 10 )
						break;

					slots += add;
				}

				++count;
			}

			return count;
		}

		private int GetPagesCount()
		{
			int page = 0;
			int slots = 0;

			ArrayList list = m_List;
			if (list == null)
				return 0;

			for (int i = 0; i < list.Count; ++i)
			{
				object obj = list[i];

				if (CheckFilter(obj))
				{
					int add;

					if (obj is BOBLargeEntry)
						add = ((BOBLargeEntry)obj).Entries.Length;
					else
						add = 1;

					if ((slots + add) > 10)
					{
						// Next page
						++page;
						slots = 0;
					}

					slots += add;
				}
			}

			return page + 1;
		}

		public object GetMaterialName( BulkMaterialType mat, BODType type, Type itemType )
		{
			switch ( type )
			{
				case BODType.Smith:
				{
					switch ( mat )
					{
						case BulkMaterialType.None: return 1062226;
						case BulkMaterialType.DullCopper: return 1018332;
						case BulkMaterialType.ShadowIron: return 1018333;
						case BulkMaterialType.Copper: return 1018334;
						case BulkMaterialType.Bronze: return 1018335;
						case BulkMaterialType.Gold: return 1018336;
						case BulkMaterialType.Agapite: return 1018337;
						case BulkMaterialType.Verite: return 1018338;
						case BulkMaterialType.Valorite: return 1018339;
					}

					break;
				}
				case BODType.Tailor:
				{
					switch ( mat )
					{
						case BulkMaterialType.None:
						{
							if ( itemType.IsSubclassOf( typeof( BaseArmor ) ) || itemType.IsSubclassOf( typeof( BaseShoes ) ) )
								return 1062235;

							return 1044286;
						}
						case BulkMaterialType.Spined: return 1062236;
						case BulkMaterialType.Horned: return 1062237;
						case BulkMaterialType.Barbed: return 1062238;
					}

					break;
				}
				case BODType.Fletcher: {
						switch (mat) {
							case BulkMaterialType.None: return 1018365;
							case BulkMaterialType.Oak: return 1018366;
							case BulkMaterialType.Ash: return 1018367;
							case BulkMaterialType.Yew: return 1018368;
							case BulkMaterialType.Heartwood: return 1018369;
							case BulkMaterialType.Bloodwood: return 1018370;
							case BulkMaterialType.Frostwood: return 1018371;
							case BulkMaterialType.BowstringLeather: return 1018372;
							case BulkMaterialType.BowstringGut: return 1018373;
							case BulkMaterialType.BowstringCannabis: return 1018374;
							case BulkMaterialType.BowstringSilk: return 1018375;
						}

						break;
					}
			}

			return "Invalid";
		}

		public BOBGump( PlayerMobile from, BulkOrderBook book ) : this( from, book, book.LastPage, null )
		{
		}

		public override void OnResponse( Server.Network.NetState sender, RelayInfo info )
		{
			int index = info.ButtonID;

			switch ( index )
			{
				case 0: // EXIT
				{
					break;
				}
				case 1: // Set Filter
				{
					m_From.SendGump( new BOBFilterGump( m_From, m_Book ) );

					break;
				}
				case 2: // Previous page
				{
					if ( m_Page > 0 )
						m_From.SendGump( new BOBGump( m_From, m_Book, m_Page - 1, m_List ) );

					return;
				}
				case 3: // Next page
				{
					if ( GetIndexForPage( m_Page + 1 ) < m_List.Count )
						m_From.SendGump( new BOBGump( m_From, m_Book, m_Page + 1, m_List ) );

					break;
				}
				case 4: // Price all
				{
					if ( m_Book.IsChildOf( m_From.Backpack ) )
					{
						m_From.Prompt = new SetPricePrompt( m_Book, null, m_Page, m_List );
						m_From.SendMessage( "Type in a price for all deeds in the book:" );
					}

					break;
				}
				default:
				{
					bool canDrop = m_Book.IsChildOf( m_From.Backpack );
					bool canPrice = canDrop || (m_Book.RootParent is PlayerVendor);

					index -= 5;

					int type = index % 2;
					index /= 2;

					if ( index < 0 || index >= m_List.Count )
						break;

					object obj = m_List[index];

					if ( !m_Book.Entries.Contains( obj ) )
					{
						m_From.SendLocalizedMessage( 1062382 ); // The deed selected is not available.
						break;
					}

					if ( type == 0 ) // Drop
					{
						if ( m_Book.IsChildOf( m_From.Backpack ) )
						{
							Item item = Reconstruct( obj );

							if ( item != null )
							{
								m_From.AddToBackpack( item );
								m_From.SendLocalizedMessage( 1045152 ); // The bulk order deed has been placed in your backpack.

								m_Book.Entries.Remove( obj );
								m_Book.InvalidateProperties();

								if ( m_Book.Entries.Count > 0 )
									m_From.SendGump( new BOBGump( m_From, m_Book ) );
								else
									m_From.SendLocalizedMessage( 1062381 ); // The book is empty.
							}
							else
							{
								m_From.SendMessage( "Internal error. The bulk order deed could not be reconstructed." );
							}
						}
					}
					else // Set Price | Buy
					{
						if ( m_Book.IsChildOf( m_From.Backpack ) )
						{
							m_From.Prompt = new SetPricePrompt( m_Book, obj, m_Page, m_List );
							m_From.SendLocalizedMessage( 1062383 ); // Type in a price for the deed:
						}
						else if ( m_Book.RootParent is PlayerVendor )
						{
							PlayerVendor pv = (PlayerVendor)m_Book.RootParent;

							VendorItem vi = pv.GetVendorItem( m_Book );

							int price = 0;

							if ( vi != null && !vi.IsForSale )
							{
								if ( obj is BOBLargeEntry )
									price = ((BOBLargeEntry)obj).Price;
								else if ( obj is BOBSmallEntry )
									price = ((BOBSmallEntry)obj).Price;
							}

							if ( price == 0 )
								m_From.SendLocalizedMessage( 1062382 ); // The deed selected is not available.
							else
								m_From.SendGump( new BODBuyGump( m_From, m_Book, obj, price ) );
						}
					}

					break;
				}
			}
		}

		private class SetPricePrompt : Prompt
		{
			private BulkOrderBook m_Book;
			private object m_Object;
			private int m_Page;
			private ArrayList m_List;

			public SetPricePrompt( BulkOrderBook book, object obj, int page, ArrayList list )
			{
				m_Book = book;
				m_Object = obj;
				m_Page = page;
				m_List = list;
			}

			public override void OnResponse( Mobile from, string text )
			{
				if ( m_Object != null && !m_Book.Entries.Contains( m_Object ) )
				{
					from.SendLocalizedMessage( 1062382 ); // The deed selected is not available.
					return;
				}

				int price = Utility.ToInt32( text );

				if ( price < 0 || price > 250000000 )
				{
					from.SendLocalizedMessage( 1062390 ); // The price you requested is outrageous!
				}
				else if ( m_Object == null )
				{
					for ( int i = 0; i < m_List.Count; ++i )
					{
						object obj = m_List[i];

						if ( !m_Book.Entries.Contains( obj ) )
							continue;

						if ( obj is BOBLargeEntry )
							((BOBLargeEntry)obj).Price = price;
						else if ( obj is BOBSmallEntry )
							((BOBSmallEntry)obj).Price = price;
					}

					from.SendMessage( "Deed prices set." );

					if ( from is PlayerMobile )
						from.SendGump( new BOBGump( (PlayerMobile)from, m_Book, m_Page, m_List ) );
				}
				else if ( m_Object is BOBLargeEntry )
				{
					((BOBLargeEntry)m_Object).Price = price;

					from.SendLocalizedMessage( 1062384 ); // Deed price set.

					if ( from is PlayerMobile )
						from.SendGump( new BOBGump( (PlayerMobile)from, m_Book, m_Page, m_List ) );
				}
				else if ( m_Object is BOBSmallEntry )
				{
					((BOBSmallEntry)m_Object).Price = price;

					from.SendLocalizedMessage( 1062384 ); // Deed price set.

					if ( from is PlayerMobile )
						from.SendGump( new BOBGump( (PlayerMobile)from, m_Book, m_Page, m_List ) );
				}
			}
		}

		public BOBGump( PlayerMobile from, BulkOrderBook book, int page, ArrayList list ) : base( 12, 24 )
		{
			from.CloseGump( typeof( BOBGump ) );
			from.CloseGump( typeof( BOBFilterGump ) );

			m_From = from;
			m_Book = book;

			if ( list == null )
			{
				list = new ArrayList( book.Entries.Count );

				for ( int i = 0; i < book.Entries.Count; ++i )
				{
					object obj = book.Entries[i];

					if ( CheckFilter( obj ) )
						list.Add( obj );
				}
			}

			m_List = list;

			int pagesCount = GetPagesCount();
			page = (page < pagesCount) ? page : pagesCount - 1;
			m_Page = page;
			m_Book.LastPage = m_Page;

			int index = GetIndexForPage(m_Page);
			int count = GetCountForIndex( index );

			int tableIndex = 0;

			PlayerVendor pv = book.RootParent as PlayerVendor;

			bool canDrop = book.IsChildOf( from.Backpack );
			bool canBuy = ( pv != null );
			bool canPrice = ( canDrop || canBuy );

			if ( canBuy )
			{
				VendorItem vi = pv.GetVendorItem( book );

				canBuy = ( vi != null && !vi.IsForSale );
			}

			int width = 700;

			if ( !canPrice )
				width = 616;

			X = (624 - width) / 2;

			AddPage( 0 );

			AddBackground( 10, 10, width, 439, 5054 );
			AddImageTiled( 18, 20, width - 17, 420, 2624 );


			if (canDrop)
				AddImageTiled(24, 64, 32, 352, 1416); //Drop

			AddImageTiled( 58, 64, 36, 352, 200 ); //Typ
			AddImageTiled( 96, 64, 133, 352, 1416 ); //Przedmiot
			AddImageTiled( 231, 64, 80, 352, 200 ); //Jakosc
			AddImageTiled( 313, 64, 80, 352, 1416 ); //Material
			AddImageTiled( 395, 64, 100, 352, 200 ); //Material2
			AddImageTiled( 497, 64, 76, 352, 1416); //Ilosc

			if (canPrice) {
				AddImageTiled(575, 64, 78, 352, 200); //Cena
				AddImageTiled(655, 64, 42, 352, 1416); //Ustaw
			}

			for ( int i = index; i < (index + count) && i >= 0 && i < list.Count; ++i )
			{
				object obj = list[i];

				if ( !CheckFilter( obj ) )
					continue;

				AddImageTiled( 24, 94 + (tableIndex * 32), canPrice ? 673 : 589, 2, 2624 );

				if ( obj is BOBLargeEntry )
					tableIndex += ((BOBLargeEntry)obj).Entries.Length;
				else if ( obj is BOBSmallEntry )
					++tableIndex;
			}

			AddAlphaRegion( 18, 20, width - 17, 420 );
			AddImage( 5, 5, 10460 );
			AddImage( width - 15, 5, 10460 );
			AddImage( 5, 424, 10460 );
			AddImage( width - 15, 424, 10460 );

			AddHtmlLocalized( canPrice ? 266 : 224, 32, 200, 32, 1062220, LabelColor, false, false ); // Bulk Order Book
			AddHtmlLocalized( 63, 64, 200, 32, 1062213, LabelColor, false, false ); // Type
			AddHtmlLocalized( 147, 64, 200, 32, 1062214, LabelColor, false, false ); // Item
			AddHtmlLocalized( 246, 64, 200, 32, 1062215, LabelColor, false, false ); // Quality
			AddHtmlLocalized( 336, 64, 200, 32, 1062216, LabelColor, false, false ); // Material
			AddHtmlLocalized( 426, 64, 200, 32, 1062216, LabelColor, false, false ); // Material
			AddHtmlLocalized( 516, 64, 200, 32, 1062217, LabelColor, false, false ); // Amount

			AddButton( 35, 32, 4005, 4007, 1, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 70, 32, 200, 32, 1062476, LabelColor, false, false ); // Set Filter

			BOBFilter f = ( from.UseOwnFilter ? from.BOBFilter : book.Filter );

			if ( f.IsDefault )
				AddHtmlLocalized( canPrice ? 470 : 386, 32, 120, 32, 1062475, 16927, false, false ); // Using No Filter
			else if ( from.UseOwnFilter )
				AddHtmlLocalized( canPrice ? 470 : 386, 32, 120, 32, 1062451, 16927, false, false ); // Using Your Filter
			else
				AddHtmlLocalized( canPrice ? 470 : 386, 32, 120, 32, 1062230, 16927, false, false ); // Using Book Filter

			AddButton( 437, 416, 4017, 4018, 0, GumpButtonType.Reply, 0 );
			AddHtmlLocalized( 472, 416, 120, 20, 1011441, LabelColor, false, false ); // EXIT

			if ( canDrop )
				AddHtmlLocalized( 26, 64, 50, 32, 1062212, LabelColor, false, false ); // Drop

			if ( canPrice )
			{
				AddHtmlLocalized( 596, 64, 200, 32, 1062218, LabelColor, false, false ); // Price

				if ( canBuy )
				{
					AddHtmlLocalized( 656, 64, 200, 32, 1062219, LabelColor, false, false ); // Buy
				}
				else
				{
					AddHtmlLocalized( 656, 64, 200, 32, 1062227, LabelColor, false, false ); // Set

					AddButton( 542, 416, 4005, 4007, 4, GumpButtonType.Reply, 0 );
					AddHtml( 577, 416, 120, 20, "<BASEFONT COLOR=#FFFFFF>Wycen wszystkie</FONT>", false, false );
				}
			}

			tableIndex = 0;

			AddHtml(45, 418, 90, 20, "<BASEFONT COLOR=#FFFFFF>Strona: " + (m_Page+1) + "/" + pagesCount + "</FONT>", false, false); // Page number

			if ( m_Page > 0 )
			{
				AddButton( 137, 416, 4014, 4016, 2, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 172, 416, 150, 20, 1011067, LabelColor, false, false ); // Previous page
			}

			if ( GetIndexForPage( m_Page + 1 ) < list.Count )
			{
				AddButton( 287, 416, 4005, 4007, 3, GumpButtonType.Reply, 0 );
				AddHtmlLocalized( 322, 416, 150, 20, 1011066, LabelColor, false, false ); // Next page
			}

			for ( int i = index; i < (index + count) && i >= 0 && i < list.Count; ++i )
			{
				object obj = list[i];

				if ( !CheckFilter( obj ) )
					continue;

				if ( obj is BOBLargeEntry )
				{
					BOBLargeEntry e = (BOBLargeEntry)obj;

					int y = 96 + (tableIndex * 32);

					if ( canDrop )
						AddButton( 35, y + 2, 5602, 5606, 5 + (i * 2), GumpButtonType.Reply, 0 );

					if ( canDrop || (canBuy && e.Price > 0) )
					{
						AddButton( 659, y + 2, 2117, 2118, 6 + (i * 2), GumpButtonType.Reply, 0 );
						AddLabel( 580, y, 1152, e.Price.ToString() );
					}

					AddHtmlLocalized( 61, y, 50, 32, 1062225, LabelColor, false, false ); // Large

					for ( int j = 0; j < e.Entries.Length; ++j )
					{
						BOBLargeSubEntry sub = e.Entries[j];

						AddHtmlLocalized( 103, y, 130, 32, sub.Number, LabelColor, false, false );

						if ( e.RequireExceptional )
							AddHtmlLocalized( 235, y, 80, 20, 1060636, LabelColor, false, false ); // exceptional
						else
							AddHtmlLocalized( 235, y, 80, 20, 1011542, LabelColor, false, false ); // normal

						object name = GetMaterialName( e.Material, e.DeedType, sub.ItemType );

						if ( name is int )
							AddHtmlLocalized( 316, y, 100, 20, (int)name, LabelColor, false, false );
						else if ( name is string )
							AddLabel( 316, y, 1152, (string)name );

						object name2 = GetMaterialName(e.Material2, e.DeedType, sub.ItemType);

						if (name2 is int)
							AddHtmlLocalized(406, y, 100, 20, (int)name2, LabelColor, false, false);
						else if (name2 is string)
							AddLabel(406, y, 1152, (string)name2);

						AddLabel( 511, y, 1152, String.Format( "{0} / {1}", sub.AmountCur, e.AmountMax ) );

						++tableIndex;
						y += 32;
					}
				}
				else if ( obj is BOBSmallEntry )
				{
					BOBSmallEntry e = (BOBSmallEntry)obj;

					int y = 96 + (tableIndex++ * 32);

					if ( canDrop )
						AddButton( 35, y + 2, 5602, 5606, 5 + (i * 2), GumpButtonType.Reply, 0 );

					if ( canDrop || (canBuy && e.Price > 0) )
					{
						AddButton( 659, y + 2, 2117, 2118, 6 + (i * 2), GumpButtonType.Reply, 0 );
						AddLabel( 580, y, 1152, e.Price.ToString() );
					}

					AddHtmlLocalized( 61, y, 50, 32, 1062224, LabelColor, false, false ); // Small

					AddHtmlLocalized( 103, y, 130, 32, e.Number, LabelColor, false, false );

					if ( e.RequireExceptional )
						AddHtmlLocalized( 235, y, 80, 20, 1060636, LabelColor, false, false ); // exceptional
					else
						AddHtmlLocalized( 235, y, 80, 20, 1011542, LabelColor, false, false ); // normal

					object name = GetMaterialName( e.Material, e.DeedType, e.ItemType );

					if ( name is int )
						AddHtmlLocalized( 316, y, 100, 20, (int)name, LabelColor, false, false );
					else if ( name is string )
						AddLabel( 316, y, 1152, (string)name );

					object name2 = GetMaterialName(e.Material2, e.DeedType, e.ItemType);

					if (name2 is int)
						AddHtmlLocalized(406, y, 100, 20, (int)name2, LabelColor, false, false);
					else if (name2 is string)
						AddLabel(406, y, 1152, (string)name2);

					AddLabel(511, y, 1152, String.Format("{0} / {1}", e.AmountCur, e.AmountMax));
				}
			}
		}
	}
}