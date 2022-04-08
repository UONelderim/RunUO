/***************************************************************************
 *                                 IRCBot.cs
 *                            -------------------
 * 							Nelderim rel. Piencu 1.1
 * 							  http:\\nelderim.org
 *
 ***************************************************************************/
// 06.05.28 :: troyan :: wylaczenie polecen !op i !deop z reakcji bota
// 06.10.19 :: troyan :: unban na starcie, set mode i nowe polecenia 
// 06.10.24 :: troyan :: naprawa restartu
// 07.08.28 :: emfor :: naprawa, po zmianach na serwerze IRCnet'u, tryby kanalow 
// 08.02.21 :: Sou :: usuniecie newsa o #nelderim-gt
// 08.10.08 :: emfor :: wyloczenie !online
// 08.12.09 :: juri :: dodanie !stadko zamiast online dla wgladu ekipy
// 09.08.03 :: emfor :: reanimacja BOT'a ;)
// 10.02.08 :: juri :: zmiany w komendach

using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Data.Odbc;
using System.Net;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using Server;
using Server.Gumps;
using Server.Prompts;
using Server.Mobiles;
using Server.Network;
using Server.Accounting;
using Server.Commands;
using Server.Items;

namespace Server.Misc
{
	public class IRCBot
	{
		#region Klasy
		
		public class Abuser
		{
			private string m_Name;
			private DateTime m_Time;
			private int m_Count;
			
			public static Hashtable Table;
			
			public Abuser( string name )
			{
				m_Name = name;
				m_Time = DateTime.Now;
				m_Count = 1;
			}
			
			public int Count
			{
				get { return m_Count; }
				set 
				{
					m_Count = ( value >= 0 ) ? value : 0;
					m_Time = DateTime.Now;
					
					if ( Config.IRCBotLogi ) m_Logger.WriteLine( String.Format( "IRCBot: rejestruje naduzycie nr {0} uzytkownika {1}", m_Count, m_Name ) );
				}
			}
			public DateTime LastViolation { get { return m_Time; } }
			
			public static bool Mark( string name )
			{
				return Mark( name, true );
			}
			
			public static bool Mark( string name, bool violent )
			{
				bool ban = false;
				
				if ( Table == null )
					Table = new Hashtable();
				
				Abuser ab = Table[ name ] as Abuser;
				
				if ( ab == null )
					Table.Add( name, new Abuser( name ) );
				else
				{
					if ( ab.LastViolation + TimeSpan.FromHours( 1 ) < DateTime.Now )
						ab.Count = 1;
					else if ( ab.LastViolation + TimeSpan.FromMinutes( 15 ) < DateTime.Now )
						ab.Count += violent ? 1 : 0;
					else if ( ab.LastViolation + TimeSpan.FromMinutes( 1 ) < DateTime.Now )
						ab.Count += violent ? 2 : 1;
					else
						ab.Count += violent ? 4 : 2;
					
					if ( ab.Count > 7 )
					{
						ban = true;
						ab.Count = 0;
					}
				}
				
				return ban;
			}
		}
		
		public class IRCChannel
		{
			public class IRCNews
			{
				private string m_Text;
				private int m_Priority;
				
				public IRCNews( string text ) : this( text, 0 )
				{
				}
				
				public IRCNews( string text, int p )
				{
					m_Text = text;
					m_Priority = p;
				}
				
				public int Priority { get { return m_Priority; } }
				public string Text { get { return m_Text; } }
			}
				
			private string m_Name;
			private string m_Pass;
			private AccessLevel m_Level;
			private DateTime m_LastActivity;
			private ArrayList m_News;
			
			public string Name { get { return m_Name; } }
			public string Pass { get { return ( m_Pass == null ) ? " " : m_Pass; } }
			public AccessLevel Level { get { return m_Level; } }
			public bool Active
			{
				get { return m_LastActivity + TimeSpan.FromMinutes( 15 ) > DateTime.Now; }
				set { m_LastActivity = DateTime.Now; }
			}
			
			public IRCChannel( string name, string pass, AccessLevel level )
			{
				m_Name = name;
				m_Pass = pass;
				m_Level = level;
				m_LastActivity = DateTime.Now;
				m_News = new ArrayList();
			}
			
			public IRCChannel( string name, string pass ) : this( name, pass, AccessLevel.Counselor )
			{
			}
			
			public IRCChannel( string name ) : this( name, null, AccessLevel.Player )
			{
			}
		
			public void NewsRegister( IRCNews n )
			{
				for ( int i = 0; i <= n.Priority; i++ )
					m_News.Add( n );
			}
			
			public string NewsGet()
			{
				try
				{
					return (  m_News.Count == 0 || ( 0.05 * m_News.Count * Utility.RandomDouble() ) < 0.1 )
						? "" : ( ( IRCNews ) m_News[ Utility.Random( m_News.Count ) ] ).Text;
				}
				catch( Exception exc )
				{
					if ( Config.IRCBotLogi ) m_Logger.WriteLine( exc.ToString() );
				}
				
				return "Blad modulu propagandy";
			}
		}
		
		public class IRCConnectionRestartTimer : Timer
		{
			public IRCConnectionRestartTimer() : base( TimeSpan.FromSeconds( 180 ) )
			{
			}
	
			protected override void OnTick()
			{
				try
				{
					if ( Config.IRCBotEnabled || Config.IRCBotTests )
					{
						Console.WriteLine( "IRCBot: restartuje..." );
						m_ConnectThread = new Thread( new ThreadStart( ConnectIRC ) );
						m_ConnectThread.Start();
					}
				}
				catch
				{
					Console.WriteLine( "IRCBot: proba nawiazania polaczenia nie powiodla sie! Ponawiam." );
					if ( Config.IRCBotLogi ) m_Logger.WriteLine( "IRCBot: proba nawiazania polaczenia nie powiodla sie! Ponawiam." );
					new IRCConnectionRestartTimer().Start();
				}
			}
		}
			
		public class AbortThreadTimer : Timer
		{
			private Thread m_Thread;
			
			public AbortThreadTimer( Thread th ) : base( TimeSpan.FromSeconds( 15 ) )
			{
				m_Thread = th;
			}	
			protected override void OnTick()
			{
				try
				{
					if ( m_Thread != null )
						m_Thread.Abort();
				}
				catch ( Exception exc )
				{
					if ( Config.IRCBotLogi ) m_Logger.WriteLine( exc.ToString() );
				}
			}
		}
		
		public class IRCNews
		{
			private string m_News;
			private IRCChannel m_Channel;
			
			public string News { get { return m_News; } }
			public IRCChannel Channel { get { return m_Channel; } }
			
			public IRCNews( string news, IRCChannel channel )
			{
				m_News = news;
				m_Channel = channel;
			}
		}
		
		#endregion
		#region Pola statyczne
		
		private static bool Enabled { get { return Config.IRCBotEnabled || Config.IRCBotTests; } }
		private static bool Active { get { return Enabled && m_IsAlive; } }
		
		private static bool m_Restart = true;
		private static bool m_IsAlive = false;
		
		private static string m_Server = "irc.pirc.pl";  //"irc.ircnet.pl"; //lub arch.edu.pl
		private static string m_Pass = "ThunD3r807t";
		private static int m_Port = 6666;
		private static string m_BotName;
		private static string m_ShardName = "UONelderim";
		private static ArrayList m_Channels;
		
		private static string[] m_PreChannelIRC;
		
		public static TcpClient TcpClnt;
		private static Thread m_ReadThread;
		private static Thread m_ConnectThread;
		
		private static DateTime m_LastPing;
		
		private static ArrayList m_News;
		
		private static FileLogger m_Logger;
		
		public static string GetTimeStamp()
		{
			DateTime now = DateTime.Now;
			return String.Format( "{0}-{1}-{2}", now.Year, now.Month, now.Day );
		}
		#endregion
		
		public static void Initialize()
		{
			try
			{
				if( !Directory.Exists( "Logi/IRCBot" ) )
					Directory.CreateDirectory( "Logi/IRCBot" );
				
				string filename = Path.Combine( Core.BaseDirectory, "Logi/IRCBot/" + GetTimeStamp() + ".log" );
				
				if ( Config.IRCBotLogi ) m_Logger = new FileLogger( filename, true );
				
				m_News = new ArrayList();
				
				m_BotName = Config.IRCBotEnabled ? "Nelderim" : "Nelderim";
				
				m_PreChannelIRC = new string[ 3 ] {
					"USER nelderim localhost 0.0.0.0 :UO Nelderim shard BOT\r\n",
					"NICK " + m_BotName + "\r\n",
					"nickserv : id " + m_Pass + "t\r\n"
				};
				
				m_PreChannelIRC = ( Config.IRCBotEnabled ) ? new string[ 3 ] {
					"USER nelderim localhost 0.0.0.0 :UO Nelderim shard BOT\r\n",
					"NICK " + m_BotName + "\r\n",
          "nickserv : id " + m_Pass + "t\r\n"
				} : new string[ 3 ]  {
					"USER nelderim localhost 0.0.0.0 :UO Nelderim shard test BOT\r\n",
					"NICK " + m_BotName + "\r\n",
					"nickserv : id " + m_Pass + "\r\n"
				};
				
				m_Channels = new ArrayList();
				
				m_Channels.Add( new IRCChannel( "#nelderim" ) );
				//m_Channels.Add( new IRCChannel( "#nelderim-help" ) );
				m_Channels.Add( new IRCChannel( "#nelderim-team", "kapucynek" ) );
				
				RegisterNews( "2Glosuj na swoj shard -> http://uo.toplista.pl/?we=nelderim", 4 );
                RegisterNews("2Glosuj na swoj shard -> http://uo.toplista.pl/?we=nelderim", 3);
				//RegisterNews( "5Problem w grze? -> PageGM -> priv do mnie -> @operator #nelderim-help", 2 );
				RegisterNews( "5Problem w grze? -> PageGM -> priv do mnie", 2 );
				//RegisterNews( "6#nelderim-help -> nasz kanal pomocy", 2 );
				RegisterNews( "4Kultura & zakaz GT na kanale", 2 );
				RegisterNews( "4Kultura obowiazuje przede wszystkim Ekipe!", AccessLevel.Counselor );
				RegisterNews( "2Podstawowa metoda zglaszania problemow w grze jest PageGM" );
				RegisterNews( "2Opis mechaniki w wiekszosci znajduje sie na http://uo.stratics.com/ & http://guide.uo.com/" );

				if ( Enabled )
				{
					Console.WriteLine( "IRCBot: startuje..." );
					m_ConnectThread = new Thread( new ThreadStart( ConnectIRC ) );
					m_ConnectThread.Start();
				}
			}
			catch ( Exception exc )
			{
				if ( Config.IRCBotLogi ) m_Logger.WriteLine( exc.ToString() );
			}
			
			CommandSystem.Register( "IRCBot", AccessLevel.GameMaster, new CommandEventHandler( IRCBotRestart_OnCommand ) );
		}
		
		[Usage( "IRCBot [restart|start|stop|say] [[#channel] [kolor] [:message]]" )]
		[Description( "Restartuje | zatrzymuje | startuje IRCBota, lub przekazuje na wskazane kanaly wiadomosc." +
		             "Parametr #all posle wiadomosc na wszystkie kanaly. " +
		             "Wiadomosc nalezy koniecznie rozpoczac dwukropkiem!" )]
		private static void IRCBotRestart_OnCommand( CommandEventArgs arg )
		{
			try
			{
				Mobile m = arg.Mobile;
				
				#region test
				
				if ( arg.Length == 1 && arg.GetString( 0 ) == "test" && Config.IRCBotTests )
				{
					if ( !Active )
					{
						m.SendMessage( 38, "IRCBot: Jestem wylaczony!" );
						return;
					}
					
					if ( Config.IRCBotLogi ) m_Logger.WriteLine( "IRCBot: Testuje na polecenie..." );
					m.SendMessage( 38, "IRCBot: Testuje na polecenie..." );
					
					SayToIRC( "Testuje kanal domyslny (kolor default)" );
					SayToIRC( "Testuje kanal domyslny (kolor 20)", 20 );
					SayToIRC( "Testuje na wszystkich kanalach (kolor 23)", 23, "all" );
					SayToIRC( "Testuje na wszystkich kanalach (kolor domyslny)", "all" );
					SayToIRC( "Testuje kanal #nelderim-team (kolor 7)", 7, "#nelderim-team" );
				}
				#endregion
				#region restart
				else if ( arg.Length == 1 && arg.GetString( 0 ) == "restart" && m.AccessLevel > AccessLevel.GameMaster )
				{
					if ( !Enabled )
						m.SendMessage( 38, "IRCBot: Aktualnie jestem wylaczony! Uzyj polecenia start" );
					else
					{
						if ( Config.IRCBotLogi ) m_Logger.WriteLine( "IRCBot: Restartuje na polecenie..." );
						m.SendMessage( 38, "IRCBot: Restartuje na polecenie..." );
							
						m_IsAlive = false;
					}
				}
				#endregion
				#region stop
				else if ( arg.Length == 1 && arg.GetString( 0 ) == "stop" && m.AccessLevel > AccessLevel.Seer )
				{
					if ( !Enabled )
						m.SendMessage( 38, "IRCBot: Aktualnie jestem wylaczony!..." );
					else
					{
						if ( Config.IRCBotLogi ) m_Logger.WriteLine( "IRCBot: Zatrzymuje na polecenie..." );
						m.SendMessage( 38, "IRCBot: Zatrzymuje na polecenie..." );
							
						m_IsAlive = false;
						
						Config.IRCBotEnabled = false;
						Config.IRCBotTests = false;
					}
				}
				#endregion
				#region start
				else if ( arg.Length == 1 && arg.GetString( 0 ) == "start" && m.AccessLevel > AccessLevel.Seer )
				{
					if ( Active )
						m.SendMessage( 38, "IRCBot: Ja juz dzialam!" );
					else
					{
						if ( Config.IRCBotLogi ) m_Logger.WriteLine( "IRCBot: Startuje na polecenie..." );
						m.SendMessage( 38, "IRCBot: Startuje na polecenie..." );
						
						Config.IRCBotEnabled = true;
						Config.IRCBotTests = false;	
						
						new IRCConnectionRestartTimer().Start();
					}
				}
				#endregion
				#region tstart
				else if ( arg.Length == 1 && arg.GetString( 0 ) == "tstart" && m.AccessLevel > AccessLevel.Seer )
				{
					if ( Config.IRCBotLogi ) m_Logger.WriteLine( "IRCBot: Uruchamiam sie w trybie testowym..." );
					m.SendMessage( 38, "IRCBot: Uruchamiam sie w trybie testowym..." );
						
					m_IsAlive = false;
					Config.IRCBotEnabled = false;
					Config.IRCBotTests = true;
				}
				#endregion
				#region say
				else if ( arg.Length > 1 && arg.GetString( 0 ) == "say" && arg.ArgString.IndexOf( ':' ) > -1  )
				{
					string channel = "default";
					string message = "Wiadomosc pusta ;)";
					int color = 4;
					
					if ( arg.GetString( 1 ).StartsWith( "#" ) && arg.Length > 2 ) 
					{
						channel = arg.GetString( 1 );
						
						if ( channel == "#all" ) channel = "all";
					}
					
					if ( arg.Length > 3 && arg.GetInt32( 2 ) > 0 )
						color = arg.GetInt32( 2 );
					else if ( arg.Length > 2 && arg.GetInt32( 1 ) > 0 )
						color = arg.GetInt32( 1 );
					
					message = arg.ArgString.Substring( arg.ArgString.IndexOf( ':' ) + 1 );
					
					SayToIRC( message, color, channel );
					m.SendMessage( 38, "IRCBot: Przesylam wiadomosc na wskazane kanaly..." );
				}
				#endregion
				else
					m.SendMessage( "IRCBot [restart|start|stop|say] [[#channel] [kolor] [:message]]" );
			}
			catch ( Exception exc )
			{
				exc.ToString();
			}
		}
    
		private static void ConnectIRC()
		{
			#region Connect
			
			try
			{
				#region TcpClient
				TcpClnt = new TcpClient( m_Server, m_Port );
				
				Thread.Sleep( 500 );
				
				SendToIRC("USER nelderim 0 * :Nelderim Shard BOT\r\n");
				SendToIRC("NICK Nelderim\r\n");
				#endregion
				#region Identyfikacja
				String m_IRCBuffer = null;
				bool loop = true;						
				Stream m_Stream = TcpClnt.GetStream();
				StreamReader m_IrcReader = new StreamReader(m_Stream);
				
				while(loop)
				{
					m_IRCBuffer = m_IrcReader.ReadLine();

					if(m_IRCBuffer.Contains("PING"))
					{
						string[] commandParts = new string[m_IRCBuffer.Split(' ').Length];
						commandParts = m_IRCBuffer.Split(' ');
						Thread.Sleep( 500 );
						SendToIRC(String.Format("PONG {0}\r\n", commandParts[1]));
					}
					else if(m_IRCBuffer.Contains((char)1 + "VERSION" + (char)1))
					{
						Thread.Sleep( 500 );
						SendToIRC("NOTICE "+ m_Server +" :" + (char)1 + "Nelderim Shard BOT 1.1" + (char)1 + "\r\n");
					}
					else if(m_IRCBuffer.Contains("IDENTIFY"))
					{
						SendToIRC("PRIVMSG NickServ :IDENTIFY " + m_Pass + "\r\n");
						loop = false;
					}
				}

				Thread.Sleep( 2000 );
				
				#endregion
				#region UNBAN, JOIN, KEY
				
				foreach ( IRCChannel ic in m_Channels )
				{
					if ( ic.Pass != null )
					{
						SendToIRC( Unban( ic ) );
						Thread.Sleep( 2000 );
					}
					
					SendToIRC( JoinChannel( ic ) );
					Thread.Sleep( 1000 );
					
					if ( ic.Pass != null )
					{
						SendToIRC( Mode( ic ) );
						Thread.Sleep( 500 );
						SendToIRC( Key( ic ) );
						Thread.Sleep( 500 );
					}
					
					ic.Active = true;
				}
				
				#endregion		
			}
			catch ( Exception exc )
			{
				if ( Config.IRCBotLogi )
				{
					m_Logger.WriteLine( "IRCBot: proba nawiazania polaczenia nie powiodla sie! Ponawiam." );
					m_Logger.WriteLine( exc.ToString() );
				}
				new IRCConnectionRestartTimer().Start();
				AbortThread( m_ConnectThread );
				return;
			}	
			
			#endregion
			#region Start
			
			try 
			{
				m_ReadThread = new Thread( new ThreadStart( WatchIRC ) );
				m_ReadThread.Start();
				m_IsAlive = true;
				
				#region Say Hallo!
				
				if ( m_Restart )
				{
					SayToAll( "2Server: UP" );
					m_Restart = false;
				}
				else
					SayToAll( "2Wrocilem! Male problemy z polaczeniem ;)" );
				
				#endregion
				
			}
			catch ( Exception exc )
			{
				if ( Config.IRCBotLogi ) 
				{
					m_Logger.WriteLine( "IRCBot: proba nawiazania polaczenia nie powiodla sie! Ponawiam." );
					m_Logger.WriteLine( exc.ToString() );
				}
				new IRCConnectionRestartTimer().Start();
				AbortThread( m_ConnectThread );
				AbortThread( m_ReadThread );
			}
			
			#endregion		
		}
		
		private static void WatchIRC()
		{
			m_IsAlive = true;
			m_LastPing = DateTime.Now;
			
			do
			{
				#region WatchIRC
				
				Thread.Sleep( 300 );
				if ( Config.IRCBotLogi ) m_Logger.WriteLine( "CYK" );
				
				try
				{ 
					if ( DateTime.Now - m_LastPing  < TimeSpan.FromSeconds( 440 ) )
					{
						#region Pre
					
						String m_IRCBuffer = null;
						String m_MessageSender = null;
						string[] m_MessageParts = null;
						int m_NameLength = 0;
						
						Stream m_Stream = TcpClnt.GetStream();
						
						byte[] m_IncomingBytes = new byte[1024];
						int m_ReadBy = m_Stream.Read( m_IncomingBytes, 0, 1024 );
							
						try
						{
							for ( int i = 0; i < m_ReadBy; i++ )
								m_IRCBuffer += Convert.ToChar( m_IncomingBytes[i] );
			
							if ( Config.IRCBotLogi ) m_Logger.WriteLine( m_IRCBuffer );
								
							char[] m_Separator = { ' ' };
							m_MessageParts = m_IRCBuffer.Split( m_Separator );
			
							m_NameLength = m_MessageParts[0].IndexOf( "~" );
							m_NameLength--;
			
							if ( m_NameLength <= 0 )
								m_NameLength = m_MessageParts[0].IndexOf( "!" );
								
							if ( m_NameLength > 0 )
								m_MessageSender = m_MessageParts[0].Substring( 0, m_NameLength );
						}
						catch( Exception exc )
						{
							if ( Config.IRCBotLogi ) 
							{
								m_Logger.WriteLine( exc.ToString() );
								m_Logger.WriteLine( "IRCBot: Blad interpretacji komunikatow IRC! Ignoruje..." );
							}
						}
						
						#endregion
						#region Analiza
						
						try
						{
							if ( m_IRCBuffer != null )
							{
								switch( m_MessageParts[1] )
								{
									#region PRIVMSG	
									case "PRIVMSG":
									{
										string channelName = m_MessageParts[2];
										
										IRCChannel channel = IRCBot.GetChannel( channelName );
										
										String m_IRCMessage = null;
										
										for ( int i = 3; i < m_MessageParts.Length; i++ )
											m_IRCMessage += " " + m_MessageParts[i];
						
										m_IRCMessage = m_IRCMessage.Replace( "\r", "" );
										m_IRCMessage = m_IRCMessage.Replace( "\n", "" );
							
										if ( m_IRCMessage[ 2 ] == '!' )
										{
											string command = "";

											for( int i = 3; i < m_IRCMessage.Length && m_IRCMessage[ i ] != ' '; i++ )
												command += m_IRCMessage[i];
											
											switch( command )
											{
												case "hej": case "witaj":
												{
													SayToIRC( "hej", 2, channel );
													
													if ( Abuser.Mark( m_MessageSender.Substring( 1 ), ( GetAccess( channel ) == AccessLevel.Player ) ) )
														Ban( m_MessageParts[0].Substring( 1 ), channel );
													
													break;
												}
												case "channels": case "kanaly": 
												{
													if ( GetAccess( channel ) > AccessLevel.Player )
													{
														for ( int i = 0; i < m_Channels.Count; i++ )
														{
															IRCChannel ic =  m_Channels[ i ] as IRCChannel;
														
															SayToIRC( "[ " + i + " ] (active=" + ic.Active + ") -> " + ic.Name , channelName );
														}
													}
													else 
													{
														if ( Abuser.Mark( m_MessageSender.Substring( 1 ) ) )
															Ban( m_MessageParts[0].Substring( 1 ), channel );
														
														SayToIRC( "4Nie slucham takich cieniasow!" , channel );
													}
													
													break;
												}
												case "echo":
												{
													if ( GetAccess( channel ) > AccessLevel.Player )
													{
														string echo = "";
													
														for( int i = 8; i < m_IRCMessage.Length; i++ )
															echo += m_IRCMessage[i];
														
														SayToAll( echo );
													}
													else 
													{
														if ( Abuser.Mark( m_MessageSender.Substring( 1 ) ) )
															Ban( m_MessageParts[0].Substring( 1 ), channel );
														
														SayToIRC( "4Nie slucham takich cieniasow!" , channel );
													}
													
													break;
												}
												case "gm":
												{
													if ( GetAccess( channel ) > AccessLevel.Player )
													{
														int count = 0;
														
														lock ( NetState.Instances )
														{
															foreach ( NetState state in NetState.Instances )
															{
																Mobile m = state.Mobile;
												
																if ( m != null && m.AccessLevel > AccessLevel.Player && m.AccessLevel < AccessLevel.Seer )
																	count++;
															}
														}
														
														string txt = "Aktualnie graczami opiekuje sie " + count + " GMow";
													
														SayToIRC( txt , channel );
													}
													else 
													{
														if ( Abuser.Mark( m_MessageSender.Substring( 1 )  ) )
															Ban( m_MessageParts[0].Substring( 1 ), channel );
														
														SayToIRC( "4Niezrozumiale polcenie!" , channel );
													}
													
													break;
												}
												case "bc":
												{
													if ( GetAccess( channel ) > AccessLevel.Player )
													{
														string text = "";
														
														for( int i = 6; i < m_IRCMessage.Length; i++ )
															text += m_IRCMessage[i];
														
														CommandHandlers.BroadcastMessage( AccessLevel.Player, 0x482, text );
														SayToIRC( "4Aj aj sir! Rozglaszam informacje!" , channel );
													}
													else 
													{
														if ( Abuser.Mark( m_MessageSender.Substring( 1 )  ) )
															Ban( m_MessageParts[0].Substring( 1 ), channelName );
														
														SayToIRC( "4Nie slucham takich cieniasow!" , channel );
													}
													
													break;
												}
												case "date": case "data": case "czas": case "time":
												{
													DateTime now = DateTime.Now;

													string date = String.Format("Jest {0}", now);
													SayToIRC( date , channel );

													string dateNel = String.Format("W grze jest {0}", NelderimDateTime.TransformToString( now, NelderimDateTimeFormat.LongIs ));
													SayToIRC( dateNel , channel );
													
													break;
												}
												case "online": case "gracze":
												{
													string txt = "Aktualnie mam " + NetState.Instances.Count + " graczy";
													
													SayToIRC( txt , 6, channel );
													
													break;
												}
												case "up":
												{
													TimeSpan ts = DateTime.Now - ServerTime.ServerStart;
													
													string txt = "Jestem aktywny juz ";
													
													txt += ts.Days > 0 ? ts.Days + " dni, " : "";
													txt += ts.Hours > 0 ? ts.Hours + " godzin i " : "";
													txt += ts.Minutes + " minut";	
													
													SayToIRC( txt , 12, channel );
													
													break;
												}
												case "kox":
												{
													int choice = Utility.Random( 3 );
													
													switch ( choice )
													{
														case 0:
															SayToIRC( "Ponoc zle wplywa na plodnosc" , channel ); break;
														case 1:
															SayToIRC( "ROX!" , channel ); break;
														case 2:
															SayToIRC( "Zarzad Shardu Informuje - KOX zle wplywa na zdrowie psychiczne gracza" , channel ); break;
														default:
															SayToIRC( "MAKROCHECK!!!" , channel ); break;
													}
													
													if ( Abuser.Mark( m_MessageSender.Substring( 1 )  ) )
														Ban( m_MessageParts[0].Substring( 1 ), channel );
													
													break;
												}
												case "ekipa":
												{
													SayToIRC( "Nie chce sie podlizywac, ale oni sa najlepsi" , channel );
													
													if ( Abuser.Mark( m_MessageSender.Substring( 1 )  ) )
														Ban( m_MessageParts[0].Substring( 1 ), channel );
													
													break;
												}
												case "zuo":
												{
													int choice = Utility.Random( 4 );
													
													switch ( choice )
													{
														case 0:
															SayToIRC( "Idz sie puknij w leb!" , channel ); break;
														case 1:
															SayToIRC( "Sou^^!" , channel ); break;
														default:
															SayToIRC( "I nie damy sobie wmowic, ze biale jest biale, a czarne jest czarne!" , channel ); break;
													}
													
													if ( Abuser.Mark( m_MessageSender.Substring( 1 )  ) )
														Ban( m_MessageParts[0].Substring( 1 ), channel );
													
													break;
												}
												case "Elg`cahlxukuth":
												{
													SayToIRC( "Mrooooook....." , channel );
													
													if ( Abuser.Mark( m_MessageSender.Substring( 1 )  ) )
														Ban( m_MessageParts[0].Substring( 1 ), channel );
													
													break;
												}
												case "mortuus":
												{
													SayToIRC( "Mrooooook....." , channel );
													
													if ( Abuser.Mark( m_MessageSender.Substring( 1 )  ) )
														Ban( m_MessageParts[0].Substring( 1 ), channel );
													
													break;
												}
												/*case "troyan":
												{
													SayToIRC( "Tatko?! Gdzie?" , channel );
													
													if ( Abuser.Mark( m_MessageSender.Substring( 1 )  ) )
														Ban( m_MessageParts[0].Substring( 1 ), channel );
													
													break;
												}*/
												/*case "Migalart":
												{
													SayToIRC( "e-firiend miga jest moim e-friendem", 13 , channel );
													
													if ( Abuser.Mark( m_MessageSender.Substring( 1 )  ) )
														Ban( m_MessageParts[0].Substring( 1 ), channel );
													
													break;
												}*/
												/*case "Sou":
												{
													SayToIRC( "Sou -> Suo -> Zuo!", 13 , channel );
													
													if ( Abuser.Mark( m_MessageSender.Substring( 1 )  ) )
														Ban( m_MessageParts[0].Substring( 1 ), channel );
													
													break;
												}*/
												case "macrocheck":
												{
													SayToIRC( m_MessageSender + " - mowisz? masz!" , channel );
													
													if ( Abuser.Mark( m_MessageSender.Substring( 1 )  ) )
														Ban( m_MessageParts[0].Substring( 1 ), channel );
													
													break;
												}
												/*case "antimacro":
												case "antymakro":
												{
													int choice = Utility.Random( 3 );
													
													switch ( choice )
													{
														case 0:
															SayToIRC( "Jest po to, zeby grac, a nie odpalac makra i siedziec na IRCu! GRAJ NIE GADASZ!" , channel ); break;
														case 1:
															SayToIRC( "Na trzy wszyscy dziekuja LogoSowi!" , channel ); break;
														default:
															SayToIRC( "Blokada AntyMacro (LogoS) - MAMY CIE!" , channel ); break;
													}

													if ( Abuser.Mark( m_MessageSender.Substring( 1 )  ) )
														Ban( m_MessageParts[0].Substring( 1 ), channel );
													
													break;
												}*/
												case "pk":
												{
													SayToIRC( "pe pe pepeka pepepepe pe-ka!" , channel );
													
													if ( Abuser.Mark( m_MessageSender.Substring( 1 )  ) )
														Ban( m_MessageParts[0].Substring( 1 ), channel );
													
													break;
												}
												case "grab":
												{
													SayToIRC( "To zmyslne polecenie pozwala automatycznie zbierac lupy" , channel );
													
													if ( Abuser.Mark( m_MessageSender.Substring( 1 ) ) )
														Ban( m_MessageParts[0].Substring( 1 ), channel );
													
													break;
												}
												/*
												case "help":
												{
													if ( channel.Name != "#nelderim-help" )
													{
														SayToIRC( "Pomocy udzielaja operatorzy kanalu #nelderim-help" , channel );
														SayToIRC( "Jesli chcesz zglosic problem operatorom kanalu #nelderim-team zglos sie do mnie" , channel );
													}
													
													break;
												} */
												case "www": case "nelderim":
												{
													SayToIRC( "http://nelderim.org" , channel );
													
													if ( Abuser.Mark( m_MessageSender.Substring( 1 ), false ) )
															Ban( m_MessageParts[0].Substring( 1 ), channel );
														
													break;
												}
												/*case "down": case "crash":
												{
													SayToIRC( "A ti ti! Nio! Nio!", 5 , channel );
													
													if ( Abuser.Mark( m_MessageSender.Substring( 1 ), false ) )
															Ban( m_MessageParts[0].Substring( 1 ), channel );
														
													break;
												}*/
												case "ban":
												{
													SayToIRC( "As You wish... MASTERRRR", 4 , channel );
													
													Ban( m_MessageParts[0].Substring( 1 ), channel );
														
													break;
												}
												case "podania":
												{
													string ConnectionString = string.Format( "DRIVER={0};SERVER={1};DATABASE={2};UID={3};PASSWORD={4};", Config.DatabaseDriver_Acc, Config.DatabaseServer_Acc, Config.DatabaseName_Acc, Config.DatabaseUserID_Acc, Config.DatabasePassword_Acc );
	
													try
													{
														ArrayList ToCreateFromDB = new ArrayList( );
														OdbcConnection Connection = new OdbcConnection( ConnectionString );

														Connection.Open( );
														OdbcCommand Command = Connection.CreateCommand( );

														Command.CommandText = string.Format( "SELECT * FROM podania_nelderim WHERE info_od_bota='tak'" );
														OdbcDataReader reader = Command.ExecuteReader( );
														
														int nowe_podania = 0;
														
														while( reader.Read( ) )
														{
															nowe_podania += 1;
														}
														reader.Close( );
														
														string ta, tb, tc = "";
														if (nowe_podania == 1){ta = "o"; tb = "e"; tc = "ie";}
														else if(nowe_podania > 4) {ta = "o"; tb = "ych";}
														else {ta = "y"; tb = "e"; tc = "ia";}
														
														if (nowe_podania > 0)
														{
															string txt = "Pojawil"+ ta +" sie " + nowe_podania + " now"+ tb +" podan" + tc;
															SayToIRC( txt, 6, channel );
														}
														else
														{
															SayToIRC( "Brak niesprawdzonych podan.", 6, channel );
														}
														
														Connection.Close( );
													}
													catch( Exception e )
													{
														Console.WriteLine( "System: Problem przy sprawdzaniu czy sa nowe podania!" );
														Console.WriteLine( e );
													}
	
													break;
												}
												case "":
												{
													break;
												}
												//case "seen": case "op": case "deop": break;
												default:
												{
													SayToIRC( "4Niezrozumiale polcenie!" , channel );
													
													if ( Abuser.Mark( m_MessageSender.Substring( 1 ) ) )
															Ban( m_MessageParts[0].Substring( 1 ), channel );
													
													break;
												}
											}
										}
										else if ( channelName == "Nelderim" )
										{
											SayToIRC( "4" + m_MessageSender.Substring( 1 ) +": " + m_IRCMessage.Substring( 2 ), "#nelderim-team" );
										}
										
										break;
									}
									#endregion
									#region JOIN|PART|KICK
									case "JOIN": case "PART": case "KICK":
									{
										string channelName = m_MessageParts[2];											
										IRCChannel channel = IRCBot.GetChannel( channelName.Substring( 1 ) );
										
										if ( Config.IRCBotLogi ) m_Logger.WriteLine( String.Format( "Aktywizujê kana³ {0}", channelName.Substring( 1 ) ) );
										
										if ( channel != null )
											channel.Active = true;
										
										break;
									}
									#endregion
									default:
										break;
								}
								
								#region PING
								
								switch ( m_MessageParts[0] )
								{
									case "PING":
										{
											String m_Pong = null;
										
											for ( int i=1; i<m_MessageParts.Length; i++ )
												m_Pong += " " + m_MessageParts[i];
			
											SendToIRC( "PONG " + m_Pong.Substring( 2 ) );
											
											if ( Config.IRCBotLogi ) m_Logger.WriteLine( "PONG" );
											
											m_LastPing = DateTime.Now;
											
											break;
										}
									case "ERROR":
										{
											if ( Config.IRCBotLogi ) m_Logger.WriteLine( "IRCBot: Serwer IRC oglosil blad polaczenia! Restartuje..." );
											m_IsAlive = false;
											
											break;
										}
									default:
										break;
								}
								
								#endregion
							}
						}
						catch( Exception exc )
						{
							if ( Config.IRCBotLogi ) 
							{
								m_Logger.WriteLine( exc.ToString() );
								m_Logger.WriteLine( "IRCBot: Blad interpretacji polecenia! Ignoruje..." );
							}
						}
						#endregion
					}
					else 
					{
						m_IsAlive = false;
						if ( Config.IRCBotLogi ) m_Logger.WriteLine( "IRCBot: polaczenie z hostem przedawnilo sie! Restartuje..." );
					}
					
					#region Kontrola aktywnosci
					
					foreach ( IRCChannel ic in m_Channels )
					{
						if ( m_IsAlive && !ic.Active )
						{
							SendToIRC( JoinChannel( ic ) );
							if ( Config.IRCBotLogi ) m_Logger.WriteLine( String.Format( "IRCBot: kanal {0} wyglada na nieaktywy - probuje dolaczyc...", ic.Name ) );
							ic.Active = true;
						}
						
						if ( Utility.Random( 1500 ) == 500 )
						{
							string text = ic.NewsGet();
							
							if ( text != "" )
								SayToIRC( text, ic );
						}
					}
					
					#endregion
				
					SayToIRC();
				}
				catch( Exception exc )
				{
					m_IsAlive = false;
					if ( Config.IRCBotLogi ) 
					{
						m_Logger.WriteLine( "IRCBot: polaczenie z hostem zerwane! Restartuje..." );
						m_Logger.WriteLine( exc.ToString() );
					}
				}
				
				#endregion
			} 
			while( m_IsAlive == true );
			
			new IRCConnectionRestartTimer().Start();
			AbortThread( m_ConnectThread );
			AbortThread( m_ReadThread );
			TcpClnt.Close();
		}
		
		private static void AbortThread( Thread th )
		{
			new AbortThreadTimer( th ).Start();
		}
		
		#region Simple IRC commands
		
		private static string JoinChannel( IRCChannel channel )
		{
			return channel != null ? "JOIN " + channel.Name + " " + channel.Pass +"\r\n" : "";
		}
		
		public static void Ban( string name, string channel )
		{
			Ban( name, GetChannel( channel ) );
		}
		
		private static void Ban( string n, IRCChannel ic )
		{
			try
			{
				string txt = "SIO!\r\n";
				
				string[] name = new string[3] 
				{
					n.Substring( 0, n.IndexOf( "!" ) ),
					n.Substring( n.IndexOf( "!" ) + 1, n.IndexOf( "@" ) - n.IndexOf( "!" ) - 1 ),
					n.Substring( n.IndexOf( "@" ) + 1 )
				};
				
				switch ( Utility.Random( 7 ) )
				{
					case 0: txt = "skonczyla sie moja cierpliwosc!\r\n"; break;
					case 1: txt = "ile mozna! mam lepsze zajecia!\r\n"; break;
					case 2: txt = "nie podobasz mi sie!\r\n"; break;
					case 3: txt = "bo zupa byla za slona...\r\n"; break;
					case 4: txt = "cya!\r\n"; break;
					case 5: txt = "sztuczna yntelygencja ownz!\r\n"; break;
				}
				
				if ( Config.IRCBotLogi )
				{
					m_Logger.WriteLine( "MODE " + ic.Name + " +b *" + name[ 0 ] + "*!*@*\r\n" );
					m_Logger.WriteLine( "MODE " + ic.Name + " +b *!*" + name[ 1 ] + "*@*\r\n" );
					m_Logger.WriteLine( "MODE " + ic.Name + " +b *!*@" + name[ 2 ] + "\r\n" );
				}
				
				if ( ic != null )
				{
					if ( ( ( string ) name[ 0 ] ).Length > 2 ) SendToIRC( "MODE " + ic.Name + " +b *" + name[ 0 ] + "*!*@*\r\n" );
					if ( ( ( string ) name[ 1 ] ).Length > 2 ) SendToIRC( "MODE " + ic.Name + " +b *!*" + name[ 1 ] + "*@*\r\n" );
					SendToIRC( "MODE " + ic.Name + " +b *!*@" + name[ 2 ] + "\r\n" );
					SendToIRC( "KICK " + ic.Name + " " + name[ 0 ] + " :" + txt );
				}
			}
			catch( Exception exc )
			{
				if ( Config.IRCBotLogi ) m_Logger.WriteLine( exc.ToString() );
			}
		}
		
		private static string Unban( IRCChannel channel )
		{
			return channel != null ? "PRIVMSG ChanServ : UNBAN " + channel.Name +"\r\n" : "";
		}
		
		private static string Mode( IRCChannel channel ) // emfor
		{
      if(channel.Name == "#nelderim" || channel.Name == "#nelderim-help")
          return channel != null ? "MODE " + channel.Name + " +nt\r\n" : "";
      else
          return channel != null ? "MODE " + channel.Name + " +ntps\r\n" : "";
		}
		
		private static string Key( IRCChannel channel )
		{
			return channel != null ? "MODE " + channel.Name + " +k " + channel.Pass +"\r\n" : "";
		}
		
		private static void SetTopic( string topic, IRCChannel ic )
		{
			if ( ic != null )
				SendToIRC( "TOPIC " + ic.Name + " " + topic + "\r\n" );
		}
		
		public static void SetTopic( string topic, string channel )
		{
			SetTopic( topic, GetChannel( channel ) );
		}
		
		private static void GetTopic( IRCChannel ic )
		{
			if ( ic != null ) 
				SendToIRC( "TOPIC " + ic.Name + "\r\n" );
		}
		
		public static void SetNick( string nick )
		{
			SendToIRC( "NICK " + nick + "\r\n" );
		}
		
		#endregion
		#region IRCChannel functions
		
		private static AccessLevel GetAccess( string channel )
		{
			IRCChannel ic = GetChannel( channel );
			
			return GetAccess( ic );
		}
		
		private static AccessLevel GetAccess( IRCChannel ic )
		{
			return ic == null ? AccessLevel.Player : ic.Level;
		}
		
		private static IRCChannel GetChannel( string channel )
		{
			foreach ( IRCChannel ic in m_Channels )
			{
				if ( channel.ToLower() == ic.Name )
				{
					ic.Active = true;
					return ic;
				}
			}
			
			return null;
		}
		
		private static IRCChannel GetChannel()
		{
			if ( m_Channels.Count > 0 )
				return m_Channels[ 0 ] as IRCChannel;
			
			return null;
		}
		
		#endregion
		#region Say To IRC
		
		public static void SayToIRC( string str, int color, string channel )
		{
			bool bold = color > 15;
			
			color = color < 0 ? 0 : color > 30 ? 0 : bold ? color - 16 : color;
			
			if ( bold )
				str = String.Format( "{0}", str );
		
			if ( color != 0 )
				str = String.Format( "{1}{0}", str, color );
			
			SayToIRC( str, channel );
		}
		
		public static void SayToIRC( string str, int color )
		{
			SayToIRC( str, color, "default" );
		}
		
		public static void SayToIRC( string str )
		{
			SayToIRC( str, GetChannel() );
		}
		
		public static void SayToIRC( string str, string channel )
		{
			switch ( channel )
			{
				case "default":
					SayToIRC( str ); break;
				case "all":
					SayToAll( str ); break;
				default:
					SayToIRC( str, GetChannel( channel ) ); break;
			}
		}
		
		public static void SayToIRC( string str, int color, IRCChannel ic )
		{
			bool bold = color > 15;
			
			color = color < 0 ? 0 : color > 30 ? 0 : bold ? color - 16 : color;
			
			if ( bold )
				str = String.Format( "{0}", str );
		
			if ( color != 0 )
				str = String.Format( "{1}{0}", str, color );
			
			SayToIRC( str, ic );
		}
		
		private static void SayToIRC( string str, IRCChannel ic )
		{
			if ( Config.IRCBotLogi )
				m_Logger.WriteLine( "PRIVMSG " + ic.Name + " :" + str + "\r\n" );
			
			lock ( m_News )
			{
				if ( m_News == null )
					m_News = new ArrayList();
				
				if ( ic != null && Active )
					m_News.Add( new IRCNews( str, ic ) );
			}
		}
		
		private static void SayToIRC()
		{
			try
			{
				lock ( m_News )
				{
					foreach( IRCNews news in m_News )
						SendToIRC( "PRIVMSG " + news.Channel.Name + " :" + news.News + "\r\n" );
					
					m_News.Clear();
				}
			}
			catch( Exception exc )
			{
				if ( Config.IRCBotLogi ) m_Logger.WriteLine( exc.ToString() );
				
				lock ( m_News )
				{
					m_News.Clear();
				}
			}
		}
		
		public static void SendToIRC( string str )
		{
			try
			{
				Stream m_Stream = TcpClnt.GetStream();
		
				ASCIIEncoding m_AsciiEncode = new ASCIIEncoding();
				byte[] m_IncomingBytes = m_AsciiEncode.GetBytes( str );
				m_Stream.Write( m_IncomingBytes, 0, m_IncomingBytes.Length );
				
				if ( Config.IRCBotLogi ) m_Logger.WriteLine( str );
			}
			catch ( Exception exc )
			{
				if ( Config.IRCBotLogi )
				{
					m_Logger.WriteLine( "IRCBot: Blad podczas proby przeslania komunikatu! Ignoruje..." );
					m_Logger.WriteLine( exc.ToString() );
				}
			}
		}
		
		public static void SayToAll( string message )
		{
			foreach ( IRCChannel ic in m_Channels )
				SayToIRC( message, ic );
		}
		
		public static void RegisterNews( string text )
		{
			RegisterNews( text, 0, AccessLevel.Player );
		}
		
		public static void RegisterNews( string text, int p )
		{
			RegisterNews( text, p, AccessLevel.Player );
		}
		
		public static void RegisterNews( string text, AccessLevel lev )
		{
			RegisterNews( text, 0, lev );
		}
		
		public static void RegisterNews( string text, int p, AccessLevel lev )
		{
			IRCChannel.IRCNews news = new IRCChannel.IRCNews( text, p );
			
			foreach( IRCChannel ic in m_Channels )
			{
				if ( ic.Level == lev )
					ic.NewsRegister( news );
			}
		}
		
		#endregion
	}
}
