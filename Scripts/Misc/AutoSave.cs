using System;
using System.IO;
using Server;
using Server.Commands;

namespace Server.Misc
{
	public class AutoSave : Timer
	{
		public static void Initialize()
		{
			new AutoSave().Start();
			CommandSystem.Register( "SetSaves", AccessLevel.Administrator, new CommandEventHandler( SetSaves_OnCommand ) );
		}

		private static bool m_SavesEnabled = true;

		public static bool SavesEnabled
		{
			get{ return m_SavesEnabled; }
			set{ m_SavesEnabled = value; }
		}

		[Usage( "SetSaves <true | false>" )]
		[Description( "Enables or disables automatic shard saving." )]
		public static void SetSaves_OnCommand( CommandEventArgs e )
		{
			if ( e.Length == 1 )
			{
				m_SavesEnabled = e.GetBoolean( 0 );
				e.Mobile.SendMessage( "Saves have been {0}.", m_SavesEnabled ? "enabled" : "disabled" );
			}
			else
			{
				e.Mobile.SendMessage( "Format: SetSaves <true | false>" );
			}
		}

		public AutoSave() : base( Config.SaveDelay - Config.SaveWarningDelay, Config.SaveDelay )
		{
			Priority = TimerPriority.OneMinute;
		}

		protected override void OnTick()
		{
			if ( !m_SavesEnabled || AutoRestart.Restarting )
				return;

			if ( Config.SaveWarningDelay == TimeSpan.Zero )
			{
				Save();
			}
			else
			{
				int s = (int)Config.SaveWarningDelay.TotalSeconds;
				int m = s / 60;
				s %= 60;

				if ( m > 0 && s > 0 )
					World.Broadcast( 0x35, true, "Swiat zostanie zapisany za {0} minute{1} i {2} skund{3}.", m, m != 1 ? "e" : "", s, s != 1 ? "de" : "e" );
				else if ( m > 0 )
					World.Broadcast( 0x35, true, "Swiat zostanie zapisany za {0} minut{1}.", m, m != 1 ? "e" : "" );
				else
					World.Broadcast( 0x35, true, "Swiat zostanie zapisany za {0} sekun{1}.", s, s != 1 ? "d" : "de" );

				Timer.DelayCall( Config.SaveWarningDelay, new TimerCallback( Save ) );
			}
		}

		public static void Save()
		{
			if ( AutoRestart.Restarting )
				return;

			try {
				Backup();
			}
			catch (Exception e) { Console.WriteLine("UWAGA: Automatyczny backup NIEUDANY: {0}", e); }

			World.Save();
		}

		private static string[] m_Backups = new string[]
		{
		#region nazwy_katalogow
			"144 Backup",
			"143 Backup",
			"142 Backup",
			"141 Backup",
			"140 Backup",
			"139 Backup",
			"138 Backup",
			"137 Backup",
			"136 Backup",
			"135 Backup",
			"134 Backup",
			"133 Backup",
			"132 Backup",
			"131 Backup",
			"130 Backup",
			"129 Backup",
			"128 Backup",
			"127 Backup",
			"126 Backup",
			"125 Backup",
			"124 Backup",
			"123 Backup",
			"122 Backup",
			"121 Backup",
			"120 Backup",
			"119 Backup",
			"118 Backup",
			"117 Backup",
			"116 Backup",
			"115 Backup",
			"114 Backup",
			"113 Backup",
			"112 Backup",
			"111 Backup",
			"110 Backup",
			"109 Backup",
			"108 Backup",
			"107 Backup",
			"106 Backup",
			"105 Backup",
			"104 Backup",
			"103 Backup",
			"102 Backup",
			"101 Backup",
			"100 Backup",
			"099 Backup",
			"098 Backup",
			"097 Backup",
			"096 Backup",
			"095 Backup",
			"094 Backup",
			"093 Backup",
			"092 Backup",
			"091 Backup",
			"090 Backup",
			"089 Backup",
			"088 Backup",
			"087 Backup",
			"086 Backup",
			"085 Backup",
			"084 Backup",
			"083 Backup",
			"082 Backup",
			"081 Backup",
			"080 Backup",
			"079 Backup",
			"078 Backup",
			"077 Backup",
			"076 Backup",
			"075 Backup",
			"074 Backup",
			"073 Backup",
			"072 Backup",
			"071 Backup",
			"070 Backup",
			"069 Backup",
			"068 Backup",
			"067 Backup",
			"066 Backup",
			"065 Backup",
			"064 Backup",
			"063 Backup",
			"062 Backup",
			"061 Backup",
			"060 Backup",
			"059 Backup",
			"058 Backup",
			"057 Backup",
			"056 Backup",
			"055 Backup",
			"054 Backup",
			"053 Backup",
			"052 Backup",
			"051 Backup",
			"050 Backup",
			"049 Backup",
			"048 Backup",
			"047 Backup",
			"046 Backup",
			"045 Backup",
			"044 Backup",
			"043 Backup",
			"042 Backup",
			"041 Backup",
			"040 Backup",
			"039 Backup",
			"038 Backup",
			"037 Backup",
			"036 Backup",
			"035 Backup",
			"034 Backup",
			"033 Backup",
			"032 Backup",
			"031 Backup",
			"030 Backup",
			"029 Backup",
			"028 Backup",
			"027 Backup",
			"026 Backup",
			"025 Backup",
			"024 Backup",
			"023 Backup",
			"022 Backup",
			"021 Backup",
			"020 Backup",
			"019 Backup",
			"018 Backup",
			"017 Backup",
			"016 Backup",
			"015 Backup",
			"014 Backup",
			"013 Backup",
			"012 Backup",
			"011 Backup",
			"010 Backup",
			"009 Backup",
			"008 Backup",
			"007 Backup",
			"006 Backup",
			"005 Backup",
			"004 Backup",
			"003 Backup",
			"002 Backup",
			"001 Backup",
			"Aktualny Backup"
		#endregion
		};

		private static void Backup()
		{
			if ( m_Backups.Length == 0 )
				return;

			string root = Path.Combine( Core.BaseDirectory, "Backups" );

			if ( !Directory.Exists( root ) )
				Directory.CreateDirectory( root );

			string[] existing = Directory.GetDirectories( root );

			for ( int i = 0; i < m_Backups.Length; ++i )
			{
				DirectoryInfo dir = Match( existing, m_Backups[i] );

				if ( dir == null )
					continue;

				if ( i > 0 )
				{
					string timeStamp = FindTimeStamp( dir.Name );

					if ( timeStamp != null )
					{
						try{ dir.MoveTo( FormatDirectory( root, m_Backups[i - 1], timeStamp ) ); }
						catch{}
					}
				}
				else
				{
					try{ dir.Delete( true ); }
					catch{}
				}
			}

			string saves = Path.Combine( Core.BaseDirectory, "Saves" );

			if ( Directory.Exists( saves ) )
				Directory.Move( saves, FormatDirectory( root, m_Backups[m_Backups.Length - 1], GetTimeStamp() ) );
		}

		private static DirectoryInfo Match( string[] paths, string match )
		{
			for ( int i = 0; i < paths.Length; ++i )
			{
				DirectoryInfo info = new DirectoryInfo( paths[i] );

				if ( info.Name.StartsWith( match ) )
					return info;
			}

			return null;
		}

		private static string FormatDirectory( string root, string name, string timeStamp )
		{
			return Path.Combine( root, String.Format( "{0} ({1})", name, timeStamp ) );
		}

		private static string FindTimeStamp( string input )
		{
			int start = input.IndexOf( '(' );

			if ( start >= 0 )
			{
				int end = input.IndexOf( ')', ++start );

				if ( end >= start )
					return input.Substring( start, end-start );
			}

			return null;
		}

		private static string GetTimeStamp()
		{
			DateTime now = DateTime.Now;
			return String.Format( "{0}-{1}-{2} {3}-{4:D2}-{5:D2}", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second );
		}
	}
}