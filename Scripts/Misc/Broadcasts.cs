using System;
using Server;
using System.Threading;

namespace Server.Misc
{
	public class Broadcasts
	{
		public static void Initialize()
		{
			EventSink.Crashed += new CrashedEventHandler( EventSink_Crashed );
			EventSink.Shutdown += new ShutdownEventHandler( EventSink_Shutdown );
		}

		public static void EventSink_Crashed( CrashedEventArgs e )
		{
			try
			{
				World.Broadcast( 0x35, true, "Powazny Blad Serwera!!!." );
				Console.WriteLine( "Nastapil powazny blad serwera i zostaje on zrestartowany." );
				Thread.Sleep(1000);
			}
			catch
			{
			}
		}

		public static void EventSink_Shutdown( ShutdownEventArgs e )
		{
			try
			{
				World.Broadcast( 0x35, true, "Serwer zostal wylaczony." );
				Console.WriteLine( "Trwa zamykanie serwera." );
				Thread.Sleep(1000);
			}
			catch
			{
			}
		}
	}
}