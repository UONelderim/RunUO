using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Commands;
using Server.Commands.Generic;
using Server.Mobiles;
using System.IO;
using Server.Misc;

namespace Server
{
	public class DownplanAlfa : Timer
	{
		private static bool m_ShutDown = false;
		
		public static bool ShutDown
		{
			get{ return m_ShutDown; }
		}
		
		public static void Initialize()
		{
			CommandSystem.Register( "Down120", AccessLevel.GameMaster, new CommandEventHandler( DownPlanowanyAlfa_OnCommand ) );
		}
		
		public static string FormatTimeSpan( TimeSpan ts )
		{
			return String.Format( "{0:D2}:{1:D2}:{2:D2}:{3:D2}", ts.Days, ts.Hours % 24, ts.Minutes % 60, ts.Seconds % 60 );
		}

		[Usage( "Down120" )]
		[Description( "Wylaczenie serwera po 2 minutach od wpisana komendy." )]
		
		public static void DownPlanowanyAlfa_OnCommand( CommandEventArgs e )
		{
			if ( m_ShutDown || QuickRestart.Restarting || AutoRestart.Restarting|| Restartb.Restarting || Down.ShutDown || Downb.ShutDown || ResplanAlfa.Restarting )
			{
				e.Mobile.SendMessage( 38, "Serwer jest podczas Restartu lub Wylanczania." );
				return;
			}
			
			Console.WriteLine("");
			Console.WriteLine("Wylaczenie serwera nastapi za 2 minuty.");
			Console.WriteLine("");
			List<Mobile> mobs = new List<Mobile>( World.Mobiles.Values );
			
			foreach ( Mobile m in mobs ) 
			{
				if (m != null && m is PlayerMobile)
				{
					m.CloseGump ( typeof( ZamykanieSwiataGump) );
					m.SendGump ( new ZamykanieSwiataGump() );
				}
			} 
			
			m_ShutDown = true;
			Zapis(true);
		}

		public DownplanAlfa() : base( TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(10.0)  )
		{
			Priority = TimerPriority.OneMinute;
		}

		public static void Zapis(bool normal)
		{
			StartDownAlfa rTimer = new StartDownAlfa(normal);
			rTimer.Start();
		}

		public static void DownStr()
		{ 	
			Save();
			Timer.DelayCall( TimeSpan.FromSeconds( 4.0 ), new TimerCallback( DownDrugi ) );	
		}
		
		public static void Save()
		{
			World.Broadcast( 0x21, true, "Rozpoczal sie zapis i wylaczenie Servera!" );
			World.Broadcast( 0x35, true, "Rozpoczyna sie zapis swiata" );
			World.Save(); 
			World.Broadcast( 0x35, true, "Zapis swiata zakonczyl sie pomyslnie" );
		}
		
		public static void DownDrugi()
		{
			Core.Process.Kill();
		}
	}
	
	public class StartDownAlfa : Timer
	{
		public bool m_Normal;
		public byte count = 120;
		public StartDownAlfa (bool normal) : base (TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(10.0))
		{
			m_Normal = normal;
		}
		
		protected override void OnTick()
		{
			if(count == 120)
			{
				World.Broadcast(1154,true, "Pozostalo {0} Sekund", count);
				count -= 10;
			}
			else if(count == 1)
			{
				World.Broadcast(1154,true, "Pozostala 1 Sekunda");
				count -= 1;
			}
			else if(count == 0)
			{
				Stop();
				DownplanAlfa.DownStr();
			}
			else
			{
				World.Broadcast(1154,true, "Pozostalo {0} Sekund", count);
				count -= 10;
			}
		}
	}
}
