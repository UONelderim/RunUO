// 05.08.16 :: troyan :: odpowiada za operacje dla gracza przy jego logowaniu na serwer
// 05.12.07 :: troyan :: AntiMacro Info System
// 05.12.13 :: troyan :: MOTD
// 05.12.19 :: troyan :: dodanie konstruktora MOTDGump pod podglad
// 05.12.22 :: troyan :: naprawa niezamykajacego sie MOTD
// 05.12.24 :: troyan :: Christmas Gifts
// 06.01.04 :: troyan :: PHS - informacja o poziomie 
// 06.01.07 :: troyan :: przechwytywanie wyjatkow
// 06.01.12 :: troyan :: zamkniecie akcji Christmas Gifts 2005
// 06.12.20 :: Migalart :: Christmas Gifts na 2006

using System;
using System.Collections.Generic;
using Server;
using Server.Mobiles;
using Server.Accounting;
using Server.Nelderim;
using Server.Gumps;
using Server.Network;
using Server.Items;
using Server.Commands;

namespace Server.Misc
{
	public class OnLogin
	{
		public static void Configure()
		{
			EventSink.Login += new LoginEventHandler( EventSink_Login );
		}

		private static void EventSink_Login( LoginEventArgs e )
		{
			try
			{
				Account acct = e.Mobile.Account as Account;
	
				if ( acct == null )
					return;

				#region gifts
				
                /*
                Console.WriteLine( acct.LastLogin );
				bool ChristmassGift = acct.LastLogin < DateTime.Parse ( "12/26/2006 2:00:00 PM" ) 
					&& acct.LastLogin > DateTime.Parse ( "12/1/2006 0:00:01 AM" )
					&& DateTime.Now < DateTime.Parse ( "1/10/2007 0:00:01 AM" )
					&& DateTime.Now > DateTime.Parse ( "12/26/2006 2:00:00 PM" );
                */
				
				#endregion
				
				acct.LastLogin = DateTime.Now;
				
				PlayerMobile pm = e.Mobile as PlayerMobile;
				
				if ( pm == null )
					return;

				//pm.SendGump( new MOTDGump( pm ) );
				//pm.SendLocalizedMessage( 505609, pm.GainFactor.ToString(), 167 ); // Twoj mnoznik przyrostow wynosi ~1_VALUE~.
				
				//if ( !pm.CanBeginAction( "Grab" ) )
					//pm.EndAction( "Grab" );
				
                /*
				#region gifts
				
				if ( ChristmassGift && pm.AccessLevel < AccessLevel.Counselor )
				{
					Container bp = pm.Backpack;
					
					if ( bp == null )
						return;
					
					int gClass = 6;
					
					if ( acct.Created < DateTime.Parse ( "5/1/2006 0:00:01 AM" ) )
						gClass++;
					
					if ( acct.Created < DateTime.Parse ( "12/1/2005 0:00:01 AM" ) )
						gClass++;
					
					if ( acct.Created < DateTime.Parse ( "7/1/2005  0:00:01 AM" ) )
						gClass++;
					
					RewardScroll rs = new RewardScroll( gClass );
					
					rs.LabelOfCreator = "[G06] " + CommandLogging.Format( pm );
					rs.Label3 = "created by Mad Santa";
					rs.Label1 = "Gwiazdka 2006";
					
					bp.DropItem( rs );
					pm.SendLocalizedMessage( 505602, "", 167 ); // W Twoim plecaku zmaterialozowal sie swiateczny prezent od Ekipy Nelderim!
				}
					
				#endregion
                */
			}
			catch ( Exception exc )
			{
				Console.WriteLine( exc.ToString() );
			}
		}
	}
}

//namespace Server.Gumps
//{
//	public class MOTDGump : Gump
//	{
//		private string m_News;
//		private bool m_IsTest;
		
//		public MOTDGump( PlayerMobile pm ) : base( 140, 80 )
//		{
//			m_IsTest = false;
			
//			try
//			{
//                List<RumorRecord> news = RumorsSystem.GetRumors( pm as Mobile, NewsType.MOTD );

//				if ( news.Count > 0 )
//				{
//					m_News = "";
					
//					foreach( RumorRecord rr in news )
//					{
//						if ( rr.StartRumor > pm.LastMOTD )
//						{
//							m_News += "<div align=\"center\" color=\"2100\">" + rr.Title + "</div>";
							
//							string[] phrases = rr.Text.Split( '#' );
							
//							foreach( string txt in phrases )
//								m_News += txt + "\n";
//						}
//					}
					
//					if ( m_News == "" )
//						return;
//				}
//				else
//					return;
				
//				AddPage( 0 );
//				AddImage( 0, 0, 1228 );
//				AddImage( 340, 255, 9005 );
				
//				AddHtml( 71, 6, 248, 18, "<div align=\"center\" color=\"2100\">Nowiny Nelderim</div>", false, false );
//				AddHtml( 28, 33, 350, 218, "<BASEFONT COLOR=\"black\">"+ m_News +"</BASEFONT>", false, true );
					
//				AddButton( 20, 294, 9722, 9721, 1, GumpButtonType.Reply, 0 );
//				AddLabel( 55 ,299, 2100, "Przeczytane, do zapomnienia" );
//			}
//			catch ( Exception exc )
//			{
//				Console.WriteLine( exc.ToString() );
//			}
//		}
		
//		public MOTDGump( RumorRecord rr ) : base( 140, 80 )
//		{
//			m_IsTest = true;
			
//			if ( rr == null )
//				return;
			
//			try
//			{
//				m_News = "";
				
//				m_News += "<div align=\"center\" color=\"2100\">" + rr.Title + "</div>";
//				string[] phrases = rr.Text.Split( '#' );
//				foreach( string txt in phrases )
//					m_News += txt + "\n";
				
//				if ( m_News == "" )
//					return;
				
//				AddPage( 0 );
//				AddImage( 0, 0, 1228 );
//				AddImage( 340, 255, 9005 );
				
//				AddHtml( 71, 6, 248, 18, "<div align=\"center\" color=\"2100\">Nowiny Nelderim</div>", false, false );
//				AddHtml( 28, 33, 350, 218, "<BASEFONT COLOR=\"black\">"+ m_News +"</BASEFONT>", false, true );
//			}
//			catch ( Exception exc )
//			{
//				Console.WriteLine( exc.ToString() );
//			}
//		}
		
//		public override void OnResponse( NetState sender, RelayInfo info )
//		{
//			if ( !m_IsTest && info.ButtonID == 1 )
//				( sender.Mobile as PlayerMobile ).LastMOTD = DateTime.Now;
//		}
//	} 
//}
