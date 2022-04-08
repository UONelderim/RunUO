using Server.Accounting;
using System;
using System.Data.Odbc;
using System.Net;
using System.Collections;
using System.Text;
using Server;
using Server.Misc;
using Server.Network;
using Server.Commands;
using Server.Commands.Generic;

namespace Server
{
	public class SprawdzanieNowychPodan : Timer
	{
		public static void Initialize()
		{
			if( Config.AccSystem_AutoAccCreation )
			{
				new SprawdzanieNowychPodan().Start();
			}
		}

		public SprawdzanieNowychPodan() : base( TimeSpan.FromSeconds( 28.0 ), Config.AccSystem_AccCheckDelay )
		{
			Priority = TimerPriority.OneSecond;
		}

		protected override void OnTick()
		{
			NowePodania.SprawdzNowePodania();
		}
	}
	
	public class NowePodania
	{
		public static void Initialize()
		{
			CommandSystem.Register( "NowePodania", AccessLevel.Owner, new CommandEventHandler( NowePodania_OnCommand ) );    
		}
		
		[Usage( "NowePodania" )]
		[Description( "Sprawdza czy sa nowe podania i poprzez bota informuje ekipe" )]
		public static void NowePodania_OnCommand( CommandEventArgs args )
		{
			SprawdzNowePodania();
		}

		static string ConnectionString = string.Format( "DRIVER={0};SERVER={1};DATABASE={2};UID={3};PASSWORD={4};",
			Config.DatabaseDriver_Acc, Config.DatabaseServer_Acc, Config.DatabaseName_Acc, Config.DatabaseUserID_Acc, Config.DatabasePassword_Acc );
		
		public static void SprawdzNowePodania()
		{
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
					IRCBot.SayToIRC( txt, 6, "#nelderim-team" );
				}
				
				Connection.Close( );
			}
			catch( Exception e )
			{
				Console.WriteLine( "System: Problem przy sprawdzaniu czy sa nowe podania!" );
				Console.WriteLine( e );
			}
		}
	}
}