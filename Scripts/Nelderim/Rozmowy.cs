using System;
using System.IO;
using Server;
using Server.Accounting;
using Server.Mobiles;
using Server.Network;
using Server.Commands;

namespace Server
{
	public class Rozmowy
	{
		private static StreamWriter writer;
		
		private static string GetTimeStamp()
		{
			DateTime now = DateTime.Now;
			return String.Format( "{0}-{1}-{2}", now.Year, now.Month, now.Day );
		}
		
		private static string LogPath = Path.Combine( "Logi/Rozmowy", String.Format( "Rozmowy {0}.log", GetTimeStamp() ) );
		
		public static void Initialize()
		{
			if(Config.LogPlayerSpeech)
			{
				Console.Write( "System: Uruchamianie zapisu rozmow..." );
					
				EventSink.Speech += new SpeechEventHandler( EventSink_Speech );
				
				if( !Directory.Exists( "Logi" ) )
					Directory.CreateDirectory( "Logi" );
				
				string directory = "Logi/Rozmowy";
				
				if( !Directory.Exists( directory ) )
					Directory.CreateDirectory( directory );
				
				try
				{
					writer = new StreamWriter( LogPath, true );
					writer.AutoFlush = true;
					
					writer.WriteLine();
					writer.WriteLine( "####################################" );
					writer.WriteLine( "Rozpoczeto zapis rozmow: {0}", DateTime.Now );
					writer.WriteLine();
	
					writer.Close();
					
					Console.WriteLine( "Gotowe" );
				}
				catch( Exception e )
				{
					Console.WriteLine( "Problem ze wczytaniem zapisanych rozmow:\n{0}", e );
				}
	
				CommandSystem.Register( "RozmowyWKonsoli", AccessLevel.Administrator, new CommandEventHandler( ConsoleListen_OnCommand ) );
			}
		}
		
		public static object Format( object o )
		{
			if( o is Mobile )
			{
				Mobile m = (Mobile)o;
				
				if( m.Account == null )
					return String.Format( "{0} (no account)", m );
				else
					return String.Format( "{0} ('{1}')", m, ((Account)m.Account).Username );
			}
			else if( o is Item )
			{
				Item item = (Item)o;
				
				return String.Format( "0x{0:X} ({1})", item.Serial.Value, item.GetType().Name );
			}
			
			return o;
		}
		
		public static void WriteDroppedLog( Mobile from, string format, params object[] args ) 
		{
			WriteDroppedLog( from, String.Format( format, args ) );
		}

		public static void WriteDroppedLog( Mobile from, string text ) 
		{
			if(Config.LogPlayerSpeech)
			{
				try 
				{
					writer = new StreamWriter( LogPath, true );
					writer.AutoFlush = true;
					
					writer.WriteLine( "{0}: {1}: {2}", DateTime.Now, from.NetState, text );
					
					string path = Core.BaseDirectory;
					string name = (((Account)from.Account) == null ? from.RawName : ((Account)from.Account).Username);
					
					AppendPath( ref path, "Logi" );
					AppendPath( ref path, "DroppedItems" );
					AppendPath( ref path, from.AccessLevel.ToString() );
					path = Path.Combine( path, String.Format( "{0}.log", name ) );
					 
					using( StreamWriter w = new StreamWriter( path, true ) ) 
					{
						w.WriteLine( "{0}: {1}: {2}", DateTime.Now, from.NetState, text );
						w.Close();
					}
				}
				catch {}
			}
		}
		
		public static void WriteLine( Mobile from, string format, params object[] args )
		{
			WriteLine( from, String.Format( format, args ) );
		}
		
		public static void WriteLine( Mobile from, string text )
		{
			if(Config.LogPlayerSpeech)
			{
				try
				{
					writer = new StreamWriter( LogPath, true );
					writer.AutoFlush = true;
	
					writer.WriteLine( "{0}: {1}: {2}", DateTime.Now, from.NetState, text );
	
					writer.Close();
					
					string path = Core.BaseDirectory;
					string name = (((Account)from.Account) == null ? from.RawName : ((Account)from.Account).Username);
					
					AppendPath( ref path, "Logi" );
					AppendPath( ref path, "Rozmowy" );
					AppendPath( ref path, from.AccessLevel.ToString() );
					path = Path.Combine( path, String.Format( "{0}.log", name ) );
					
					using( StreamWriter sw = new StreamWriter( path, true ) )
					{
						sw.WriteLine( "{0}: {1}: {2}", DateTime.Now, from.NetState, text );
						sw.Close();
					}
				}
				catch{}
			}
		}

		private static char[] m_NotSafe = new char[]{ '\\', '/', ':', '<', '>', '|', '{', '}' };
		
		public static void AppendPath( ref string path, string toAppend )
		{
			path = Path.Combine( path, toAppend );
			
			if( !Directory.Exists( path ) )
				Directory.CreateDirectory( path );
		}

		public static string Safe( string ip )
		{
			if ( ip == null )
				return "null";

			ip = ip.Trim();

			if ( ip.Length == 0 )
				return "empty";

			bool isSafe = true;

			for ( int i = 0; isSafe && i < m_NotSafe.Length; ++i )
				isSafe = ( ip.IndexOf( m_NotSafe[i] ) == -1 );

			if ( isSafe )
				return ip;

			System.Text.StringBuilder sb = new System.Text.StringBuilder( ip );

			for ( int i = 0; i < m_NotSafe.Length; ++i )
				sb.Replace( m_NotSafe[i], '_' );

			return sb.ToString();
		}
		
		public static void EventSink_Speech( SpeechEventArgs e )
		{
			WriteLine( e.Mobile, "{0}: {1}", Format( e.Mobile ), e.Speech );
			
			if( Config.ShowPlayerSpeechOnConsole )
			{
				Console.WriteLine( String.Format( "{0:D2}:{1:D2}:{2:D2} - ({3})", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, ((Account)e.Mobile.Account).Username ) + e.Mobile.Name + ": " + e.Speech );
			}
		}
        
		[Usage( "RozmowyWKonsoli <true | false>")]
		[Description( "Wlacza lub wylacza pokazywanie rozmow calego swiata w Konsoli." )]
		public static void ConsoleListen_OnCommand( CommandEventArgs e )
		{
			if( e.Length == 1 )
			{
				Config.ShowPlayerSpeechOnConsole = e.GetBoolean( 0 );
				e.Mobile.SendMessage( "Pokazywanie rozmow w konsoli zostalo {0}.", Config.ShowPlayerSpeechOnConsole ? "wlaczone" : "wylaczone" );
				Console.WriteLine( "Pokazywanie rozmow w konsoli zostalo {0}.", Config.ShowPlayerSpeechOnConsole ? "wlaczone" : "wylaczone" );
			}
			else
			{
				e.Mobile.SendMessage( "Przyklad: RozmowyWKonsoli <true | false >" );
			}
		}
	}
}
