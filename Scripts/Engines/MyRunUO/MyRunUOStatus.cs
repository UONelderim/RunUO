using System;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Network;

namespace Server.Engines.MyRunUO
{
	public class MyRunUOStatus
	{
		public static void Initialize()
		{
			if ( Config.MyRunUOEnabled )
			{
				Timer.DelayCall( TimeSpan.FromSeconds( 20.0 ), Config.StatusUpdateInterval, new TimerCallback( Begin ) );

				CommandSystem.Register( "WWW", AccessLevel.Administrator, new CommandEventHandler( UpdateWebStatus_OnCommand ) );
			}
		}

		[Usage( "WWW" )]
		[Description( "Starts the process of updating the MyRunUO online status database." )]
		public static void UpdateWebStatus_OnCommand( CommandEventArgs e )
		{
			if ( m_Command == null || m_Command.HasCompleted )
			{
				Begin();
				e.Mobile.SendMessage( "Rozpoczeto aktualizacje statusu dla WWW." );
			}
			else
			{
				e.Mobile.SendMessage( "Status dla WWW zostal pomyslnie zakonczony." );
			}
		}

		private static DatabaseCommandQueue m_Command;

		public static void Begin()
		{
			if ( m_Command != null && !m_Command.HasCompleted )
				return;

			DateTime start = DateTime.Now;
			//Console.WriteLine( "MyRunUO: Updating status database" );

			try
			{
				m_Command = new DatabaseCommandQueue( "MyRunUO: Status database updated in {0:F1} seconds", "MyRunUO Status Database Thread" );

				m_Command.Enqueue( "DELETE FROM runuo_status" );

				List<NetState> online = NetState.Instances;

				for ( int i = 0; i < online.Count; ++i )
				{
					NetState ns = online[i];
					Mobile mob = ns.Mobile;

					if ( mob != null )
						m_Command.Enqueue( String.Format( "INSERT INTO runuo_status VALUES ({0}, '{1},{2},{3}', '{4}', {5}, {6})", mob.Serial.Value.ToString(), mob.X.ToString(), mob.Y.ToString(), mob.Z.ToString(), mob.Map, mob.Karma, mob.Fame ) );
				}
			}
			catch ( Exception e )
			{
				Console.WriteLine( "MyRunUO: Error updating status database" );
				Console.WriteLine( e );
			}

			if ( m_Command != null )
				m_Command.Enqueue( null );
		}
	}
}