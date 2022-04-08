using System;
using System.IO;
using System.Diagnostics;
using Server;
using Server.Commands;
using Server.Commands.Generic;
using System.Collections;
using Server.Items;

namespace Server
{
	public class AutoRestart : Timer
	{
		public static bool Enabled = false; // is the script enabled?

		private static TimeSpan RestartTime = TimeSpan.FromHours( 2.0 ); // time of day at which to restart
		private static TimeSpan RestartDelay = TimeSpan.FromSeconds( 4.0 ); // how long the server should remain active before restart (period of 'server wars')

		private static TimeSpan WarningDelay = TimeSpan.FromMinutes( 1.0 ); // at what interval should the shutdown message be displayed?

		private static bool m_Restarting;
		private static DateTime m_RestartTime;

		public static bool Restarting
		{
			get{ return m_Restarting; }
		}

		public static void Initialize()
		{
            // 29.10.2012 :: zombie :: Admin -> Seer
			CommandSystem.Register( "Restart", AccessLevel.Seer, new CommandEventHandler( Restart_OnCommand ) );
            // zombie
			new AutoRestart().Start();
		}
		
		public static string FormatTimeSpan( TimeSpan ts )
		{
			return String.Format( "{0:D2}:{1:D2}:{2:D2}:{3:D2}", ts.Days, ts.Hours % 24, ts.Minutes % 60, ts.Seconds % 60 );
		}
		
		[Usage( "Restart" )]
		[Description( "Restart serwera." )]
		public static void Restart_OnCommand( CommandEventArgs e )
		{
			if ( m_Restarting || QuickRestart.Restarting || Restartb.Restarting || Down.ShutDown || Downb.ShutDown || ResplanAlfa.Restarting || DownplanAlfa.ShutDown )
			{
				e.Mobile.SendMessage( 38, "Serwer jest podczas Restartu lub Wylanczania." );
				return;
			}
			else
			{
				World.Broadcast( 1272, true, "Uwaga za chwile serwer zostanie zrestartowany" );
				Enabled = true;
				m_RestartTime = DateTime.Now;
			}
		}
		
		public static void ResPlayer123_OnCommand( CommandEventArgs e )
		{
			if ( m_Restarting )
			{
				e.Mobile.SendMessage( "Serwer jest podczas restartu, zrestartowal gracz !!!" );
			}
			else
			{
				World.Broadcast( 1272, true, "Uwaga za chwile serwer zostanie zrestartowany" );
				Enabled = true;
				m_RestartTime = DateTime.Now;
			}
		}
			
		public AutoRestart() : base( TimeSpan.FromSeconds( 4.0 ), TimeSpan.FromSeconds( 4.0 ) )
		{
			Priority = TimerPriority.FiveSeconds;

			m_RestartTime = DateTime.Now.Date + RestartTime;

			if ( m_RestartTime < DateTime.Now )
				m_RestartTime += TimeSpan.FromDays( 1.0 );
		}
		private void Warning_Callback()
		{
			World.Broadcast( 1272, true, "Rozpoczal sie zapis i restart Servera!" );
		}
		private void Restart_Callback()
		{
			Process.Start( Core.ExePath );
			Core.Process.Kill();
		}
		protected override void OnTick()
		{
			if ( m_Restarting || !Enabled )
				return;

			if ( DateTime.Now < m_RestartTime )
				return;

			if ( WarningDelay > TimeSpan.Zero )
			{
				Warning_Callback();
				Timer.DelayCall( WarningDelay, WarningDelay, new TimerCallback( Warning_Callback ) );
			}

			World.Broadcast( 0x35, true, "Rozpoczyna sie zapis swiata" ); 
			World.Save();
			World.Broadcast( 0x35, true, "Zapis swiata zakonczyl sie pomyslnie" );
			
			m_Restarting = true;

			Timer.DelayCall( RestartDelay, new TimerCallback( Restart_Callback ) );
		}
	}
}