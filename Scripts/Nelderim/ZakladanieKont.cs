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
	public class SprawdzaniePodan : Timer
	{
		public static void Initialize()
		{
			if( Config.AccSystem_AutoAccCreation )
			{
				new SprawdzaniePodan().Start();
			}
		}

		public SprawdzaniePodan() : base( TimeSpan.FromSeconds( 17.0 ), Config.AccSystem_CreateAccDelay )
		{
			Priority = TimerPriority.OneSecond;
		}

		protected override void OnTick()
		{
			ZakladanieKont.SprawdzKonta();
		}
	}
	
	public class ZakladanieKont
	{
		public static void Initialize()
		{
			CommandSystem.Register( "Konto", AccessLevel.Owner, new CommandEventHandler( Konto_OnCommand ) );    
		}
		
		[Usage( "Konto" )]
		[Description( "Sprawdzenie podan i dodanie nowych kont" )]
		public static void Konto_OnCommand( CommandEventArgs args )
		{
			SprawdzKonta();
		}
	
		private static int QueryCount = 0; //Offset 0 values.

		static string ConnectionString = string.Format( "DRIVER={0};SERVER={1};DATABASE={2};UID={3};PASSWORD={4};",
			Config.DatabaseDriver_Acc, Config.DatabaseServer_Acc, Config.DatabaseName_Acc, Config.DatabaseUserID_Acc, Config.DatabasePassword_Acc );
		
		public static void SprawdzKonta()
		{
			try
			{
				ArrayList ToCreateFromDB = new ArrayList( );
				OdbcConnection Connection = new OdbcConnection( ConnectionString );

				Connection.Open( );
				OdbcCommand Command = Connection.CreateCommand( );
				
				if ( Config.Server_Primary )
				{
					if ( Config.AccSystem_GMConfirm )
					{
						Command.CommandText = string.Format( "SELECT * FROM podania_nelderim WHERE potwierdzone='tak' AND status='klucz_potwierdzony_sprawdzic_przez_gm'" );
					}
					else
					{
						Command.CommandText = string.Format( "SELECT * FROM podania_nelderim WHERE potwierdzone='tak' AND status='sprawdzone_przez_gm'" );
					}
					OdbcDataReader reader = Command.ExecuteReader( );
					
					QueryCount += 1;

					while( reader.Read( ) )
					{
						string email = reader.GetString( 1 );
						string login = reader.GetString( 2 );
						string haslo = reader.GetString( 3 );
						string zm_ip = reader.GetString( 5 );

						if( Accounts.GetAccount( login ) == null )// sprawdzanie czy dany login juz istnieje w bazie danych
						{
							ToCreateFromDB.Add( Accounts.AddAccount( login, haslo, email ) );
							Account acct = Accounts.GetAccount( login ) as Account;
							acct.SetTag( "email", email ); 
							acct.SetTag( "zmienny IP", zm_ip ); 
						}
					}
					reader.Close( );
					
					foreach( Account a in ToCreateFromDB )
					{
						QueryCount += 1;

						if ( !Config.AccSystem_GMConfirm )
						{
							Command.CommandText = string.Format( "UPDATE podania_nelderim SET sprawdzone='tak', sprawdzone_data='{0:D2}:{1:D2}:{2:D2} {3:D4}-{4:D2}-{5:D2}', status='konto_utworzone_wysylanie_informacji' WHERE login='{6}' AND sprawdzone='nie'", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, a.Username );
							Command.ExecuteNonQuery( );
						}
						else
						{
							Command.CommandText = string.Format( "UPDATE podania_nelderim SET sprawdzone='tak', sprawdzone_data='{0:D2}:{1:D2}:{2:D2} {3:D4}-{4:D2}-{5:D2}', status='konto_utworzone_wysylanie_informacji' WHERE login='{6}' AND sprawdzone='nie'", DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second, DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, a.Username );
							Command.ExecuteNonQuery( );
						}
					}

					Connection.Close( );

					if(ToCreateFromDB.Count > 0)
					{
						if(ToCreateFromDB.Count == 1)
						{
							Console.WriteLine( "System: Utworzono {0} Konto", ToCreateFromDB.Count );
							string txt = "Jedno z podan zostalo sprawdzone, a konto utworzone, mozna sie logowac.";
							IRCBot.SayToIRC( txt, 10, "#nelderim" );
						}
						else
						{
							Console.WriteLine( "System: Utworzono {0} Konta", ToCreateFromDB.Count );
							string txt = "Podania zostaly sprawdzone, a konta utworzone, mozna sie logowac.";
							IRCBot.SayToIRC( txt, 10, "#nelderim" );
						}
					}
				}
				else
				{
					Command.CommandText = string.Format( "SELECT * FROM podania_nelderim WHERE status='konto_gotowe' AND synch_secondary=''" );
					
					OdbcDataReader reader = Command.ExecuteReader( );
					
					QueryCount += 1;

					while( reader.Read( ) )
					{
						string email = reader.GetString( 1 );
						string login = reader.GetString( 2 );
						string haslo = reader.GetString( 3 );
						string zm_ip = reader.GetString( 5 );

						if( Accounts.GetAccount( login ) == null )// sprawdzanie czy dany login juz istnieje w bazie danych
						{
							ToCreateFromDB.Add( Accounts.AddAccount( login, haslo, email ) );
							Account acct = Accounts.GetAccount( login ) as Account;
							acct.SetTag( "email", email ); 
							acct.SetTag( "zmienny IP", zm_ip ); 
						}
					}
					reader.Close( );
					
					foreach( Account a in ToCreateFromDB )
					{
						QueryCount += 1;

						Command.CommandText = string.Format( "UPDATE podania_nelderim SET synch_secondary='tak' WHERE login='{0}'", a.Username );
						Command.ExecuteNonQuery( );
					}
					
					Connection.Close( );

					if(ToCreateFromDB.Count > 0)
					{
						if(ToCreateFromDB.Count == 1)
						{
							Console.WriteLine( "System: Zsynchronizowano/Utworzono {0} Konto", ToCreateFromDB.Count );
						}
						else
						{
							Console.WriteLine( "System: Zsynchronizowano/Utworzono {0} Konta", ToCreateFromDB.Count );
						}
					}
				}
			}
			catch( Exception e )
			{
				Console.WriteLine( "System: Problem przy tworzeniu kont!" );
				Console.WriteLine( e );
			}
		}
	}
}