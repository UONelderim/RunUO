using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Server;
using Server.Accounting;
using Server.Commands;
using Server.Engines.Help;
using Server.Network;
using Server.Regions;

namespace Server.Misc
{
	public enum PasswordProtection
	{
		None,
		Crypt,
		NewCrypt
	}

	public class AccountHandler
	{
		private static StreamWriter m_Output;
		
		private static string GetTimeStamp()
		{
			DateTime now = DateTime.Now;
			return String.Format( "{0}-{1}-{2} {3}:{4:D2}:{5:D2}", now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second );
		}
		
		private static string GetTimeStamp_Short()
		{
			DateTime now = DateTime.Now;
			return String.Format( "{0}-{1}-{2}", now.Year, now.Month, now.Day );
		}
		
		private static bool RestrictDeletion = !TestCenter.Enabled;

		public static PasswordProtection ProtectPasswords = PasswordProtection.NewCrypt;

		private static StreamWriter writer;
		
		private static AccessLevel m_LockdownLevel;

		public static AccessLevel LockdownLevel
		{
			get{ return m_LockdownLevel; }
			set{ m_LockdownLevel = value; }
		}

		private static CityInfo[] StartingCities = new CityInfo[]
			{
				new CityInfo( "Yew",		"The Empath Abbey",			633,	858,	0  ),
				new CityInfo( "Minoc",		"The Barnacle",				2476,	413,	15 ),
				new CityInfo( "Britain",	"Sweet Dreams Inn",			1496,	1628,	10 ),
				new CityInfo( "Moonglow",	"The Scholars Inn",			4408,	1168,	0  ),
				new CityInfo( "Trinsic",	"The Traveler's Inn",		1845,	2745,	0  ),
				new CityInfo( "Magincia",	"The Great Horns Tavern",	3734,	2222,	20 ),
				new CityInfo( "Jhelom",		"The Mercenary Inn",		1374,	3826,	0  ),
				new CityInfo( "Skara Brae",	"The Falconer's Inn",		618,	2234,	0  ),
				new CityInfo( "Vesper",		"The Ironwood Inn",			2771,	976,	0  ),
				new CityInfo( "Haven",		"Buckler's Hideaway",		3667,	2625,	0  )
			};

		public static void Initialize()
		{
			EventSink.DeleteRequest += new DeleteRequestEventHandler( EventSink_DeleteRequest );
			EventSink.AccountLogin += new AccountLoginEventHandler( EventSink_AccountLogin );
			EventSink.GameLogin += new GameLoginEventHandler( EventSink_GameLogin );

			if ( Config.ChangePasswordEnabled )
				CommandSystem.Register( "Haslo", AccessLevel.Player, new CommandEventHandler( Password_OnCommand ) );
				
			if ( !Directory.Exists( "Logi" ) )
				Directory.CreateDirectory( "Logi" );

			string directory = "Logi/Polaczenia";

			if ( !Directory.Exists( directory ) )
				Directory.CreateDirectory( directory );

			try
			{
				m_Output = new StreamWriter( Path.Combine( directory, String.Format( "Polaczenia {0}.log", GetTimeStamp_Short() ) ), true );

				m_Output.AutoFlush = true;

				m_Output.WriteLine( "##############################" );
				m_Output.WriteLine( "Polaczena z Sesji: {0}", DateTime.Now );
				m_Output.WriteLine();
			}
			catch
			{
			}
		}

		[Usage( "Haslo <noweHaslo> <powtorzHaslo>" )]
		[Description( "Changes the password of the commanding players account. Requires the same C-class IP address as the account's creator." )]
		public static void Password_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			Account acct = from.Account as Account;

			if ( acct == null )
				return;

			IPAddress[] accessList = acct.LoginIPs;

			NetState ns = from.NetState;

			if ( ns == null )
				return;

			if ( e.Length == 0 )
			{
				from.SendMessage( "Musisz podac nowe haslo." );
				return;
			}
			else if ( e.Length == 1 )
			{
				from.SendMessage( "Aby zmienic haslo musisz podac nowe i je powtorzyc. Uzyj komendy wedlug przykladu:" );
				from.SendMessage( "[Haslo \"(noweHaslo)\" \"(powtorzHaslo)\"" );
				return;
			}

			string pass = e.GetString( 0 );
			string pass2 = e.GetString( 1 );

			if ( pass != pass2 )
			{
				from.SendMessage( "Podane hasla nie pasuja do siebie." );
				return;
			}

			bool isSafe = true;

			for ( int i = 0; isSafe && i < pass.Length; ++i )
				isSafe = ( pass[i] >= 0x20 && pass[i] < 0x80 );

			if ( !isSafe )
			{
				from.SendMessage( "Halo jest niepoprawne." );
				return;
			}

			try
			{
				IPAddress ipAddress = ns.Address;

				if( !Directory.Exists( "Logi/Zmiana_Hasla" ) )
						Directory.CreateDirectory( "Logi/Zmiana_Hasla" );
				
				string LogPath = Path.Combine( "Logi/Zmiana_Hasla", String.Format( "Zmiana_Hasla {0}.log", GetTimeStamp_Short() ) );
				
				writer = new StreamWriter( LogPath, true );
				writer.AutoFlush = true;
				writer.WriteLine( "{0}: Uzytkownik: {1}, z adresu IP: {2} zmienil haslo!", GetTimeStamp(), acct, ipAddress );
				writer.Close();

				acct.SetPassword( pass );
				from.SendMessage( "Haslo do twojego konta zostalo zmienione." );
			}
			catch
			{
			}
		}

		private static void EventSink_DeleteRequest( DeleteRequestEventArgs e )
		{
			NetState state = e.State;
			int index = e.Index;

			Account acct = state.Account as Account;

			if ( acct == null )
			{
				state.Dispose();
			}
			else if ( index < 0 || index >= acct.Length )
			{
				state.Send( new DeleteResult( DeleteResultType.BadRequest ) );
				state.Send( new CharacterListUpdate( acct ) );
			}
			else
			{
				Mobile m = acct[index];

				if ( m == null )
				{
					state.Send( new DeleteResult( DeleteResultType.CharNotExist ) );
					state.Send( new CharacterListUpdate( acct ) );
				}
				else if ( m.NetState != null )
				{
					state.Send( new DeleteResult( DeleteResultType.CharBeingPlayed ) );
					state.Send( new CharacterListUpdate( acct ) );
				}
				else if ( RestrictDeletion && DateTime.Now < (m.CreationTime + Config.CharDeleteDelay) )
				{
					state.Send( new DeleteResult( DeleteResultType.CharTooYoung ) );
					state.Send( new CharacterListUpdate( acct ) );
				}
				else if ( m.AccessLevel == AccessLevel.Player && Region.Find( m.LogoutLocation, m.LogoutMap ).GetRegion( typeof( JailRegion ) ) != null )	//Don't need to check current location, if netstate is null, they're logged out
				{
					state.Send( new DeleteResult( DeleteResultType.BadRequest ) );
					state.Send( new CharacterListUpdate( acct ) );
				}
				else
				{
					Console.WriteLine( "Uzytkownik: {0}: Usuwa postac {1} (0x{2:X})", state, index, m.Serial.Value );
					m_Output.WriteLine( "Uzytkownik: {0}: Usuwa postac {1} (0x{2:X})", state, index, m.Serial.Value );

					acct.Comments.Add( new AccountComment( "System", String.Format( "Character #{0} {1} deleted by {2}", index + 1, m, state ) ) );

					m.Delete();
					state.Send( new CharacterListUpdate( acct ) );
				}
			}
		}

		public static bool CanCreate( IPAddress ip )
		{
			if ( !IPTable.ContainsKey( ip ) )
				return true;

			return ( IPTable[ip] < Config.MaxAccountsPerIP );
		}

		private static Dictionary<IPAddress, Int32> m_IPTable;

		public static Dictionary<IPAddress, Int32> IPTable
		{
			get
			{
				if ( m_IPTable == null )
				{
					m_IPTable = new Dictionary<IPAddress, Int32>();

					foreach ( Account a in Accounts.GetAccounts() )
						if ( a.LoginIPs.Length > 0 )
						{
							IPAddress ip = a.LoginIPs[0];

							if ( m_IPTable.ContainsKey( ip ) )
								m_IPTable[ip]++;
							else
								m_IPTable[ip] = 1;
						}
				}

				return m_IPTable;
			}
		}	

		private static Account CreateAccount( NetState state, string un, string pw )
		{
			if ( un.Length == 0 || pw.Length == 0 )
				return null;

			bool isSafe = true;

			for ( int i = 0; isSafe && i < un.Length; ++i )
				isSafe = ( un[i] >= 0x20 && un[i] < 0x80 );

			for ( int i = 0; isSafe && i < pw.Length; ++i )
				isSafe = ( pw[i] >= 0x20 && pw[i] < 0x80 );

			if ( !isSafe )
				return null;

			if ( !CanCreate( state.Address ) )
			{
				Console.WriteLine( "Polaczenie od: {0}: Konto '{1}' nie zostalo utworzone, mozna posiadac maksymalnie {2} kont{3} na adres IP.", state, un, Config.MaxAccountsPerIP, Config.MaxAccountsPerIP == 1 ? "o" : "a" );
				m_Output.WriteLine( "Polaczenie od: {0}: Konto '{1}' nie zostalo utworzone, mozna posiadac maksymalnie {2} kont{3} na adres IP.", state, un, Config.MaxAccountsPerIP, Config.MaxAccountsPerIP == 1 ? "o" : "a" );
				return null;
			}

			Console.WriteLine( "Polaczenie od: {0}: utworzono nowe konto '{1}'", state, un );
			m_Output.WriteLine( "Polaczenie od: {0}: utworzono nowe konto '{1}'", state, un );

			Account a = new Account( un, pw );

			return a;
		}

		public static void EventSink_AccountLogin( AccountLoginEventArgs e )
		{
			if ( !IPLimiter.SocketBlock && !IPLimiter.Verify( e.State.Address ) )
			{
				e.Accepted = false;
				e.RejectReason = ALRReason.InUse;

				Console.WriteLine( "Polaczenie od: {0}: Przekroczono limit polaczen", e.State );
				m_Output.WriteLine( "Polaczenie od: {0}: Przekroczono limit polaczen", e.State );

				using ( StreamWriter op = new StreamWriter( "Logi/ipLimits.log", true ) )
					op.WriteLine( "{0}\tPast IP limit threshold\t{1}", e.State, DateTime.Now );

				return;
			}

			string un = e.Username;
			string pw = e.Password;

			e.Accepted = false;
			Account acct = Accounts.GetAccount( un ) as Account;

			if ( acct == null )
			{
				if ( Config.AutoAccountCreation && un.Trim().Length > 0 )	//To prevent someone from making an account of just '' or a bunch of meaningless spaces 
				{
					e.State.Account = acct = CreateAccount( e.State, un, pw );
					e.Accepted = acct == null ? false : acct.CheckAccess( e.State );

					if ( !e.Accepted )
						e.RejectReason = ALRReason.BadComm;
				}
				else
				{
					Console.WriteLine( "Polaczenie od: {0}: Niepoprawna nazwa uzytkownika: '{1}'", e.State, un );
					m_Output.WriteLine( "Polaczenie od: {0}: Niepoprawna nazwa uzytkownika: '{1}'", e.State, un );
					e.RejectReason = ALRReason.Invalid;
				}
			}
			else if ( !acct.HasAccess( e.State ) )
			{
				Console.WriteLine( "Polaczenie od: {0}: Brak dostepu dla: '{1}'", e.State, un );
				m_Output.WriteLine( "Polaczenie od: {0}: Brak dostepu dla: '{1}'", e.State, un );
				e.RejectReason = ( m_LockdownLevel > AccessLevel.Player ? ALRReason.BadComm : ALRReason.BadPass );
			}
			else if ( !acct.CheckPassword( pw ) )
			{
				Console.WriteLine( "Polaczenie od: {0}: Niepoprawne haslo dla: '{1}'", e.State, un );
				m_Output.WriteLine( "Polaczenie od: {0}: Niepoprawne haslo dla: '{1}'", e.State, un );
				e.RejectReason = ALRReason.BadPass;
			}
			else if ( acct.Banned )
			{
				Console.WriteLine( "Polaczenie od: {0}: Konto zablokowane dla: '{1}'", e.State, un );
				m_Output.WriteLine( "Polaczenie od: {0}: Konto zablokowane dla: '{1}'", e.State, un );
				e.RejectReason = ALRReason.Blocked;
			}
			else
			{
				Console.WriteLine( "Polaczenie od: {0}: Poprawne dane logowania dla: '{1}'", e.State, un );
				m_Output.WriteLine( "Polaczenie od: {0}: Poprawne dane logowania dla: '{1}'", e.State, un );
				e.State.Account = acct;
				e.Accepted = true;

				acct.LogAccess( e.State );
			}

			if ( !e.Accepted )
				AccountAttackLimiter.RegisterInvalidAccess( e.State );
		}

		public static void EventSink_GameLogin( GameLoginEventArgs e )
		{
			if ( !IPLimiter.SocketBlock && !IPLimiter.Verify( e.State.Address ) )
			{
				e.Accepted = false;

				Console.WriteLine( "Polaczenie od: {0}: Przekroczono limit polaczen", e.State );
				m_Output.WriteLine( "Polaczenie od: {0}: Przekroczono limit polaczen", e.State );

				using ( StreamWriter op = new StreamWriter( "Logi/ipLimits.log", true ) )
					op.WriteLine( "{0}\tPast IP limit threshold\t{1}", e.State, DateTime.Now );

				return;
			}

			string un = e.Username;
			string pw = e.Password;

			Account acct = Accounts.GetAccount( un ) as Account;

			if ( acct == null )
			{
				e.Accepted = false;
			}
			else if ( !acct.HasAccess( e.State ) )
			{
				Console.WriteLine( "Polaczenie od: {0}: Brak dostepu dla: '{1}'", e.State, un );
				m_Output.WriteLine( "Polaczenie od: {0}: Brak dostepu dla: '{1}'", e.State, un );
				e.Accepted = false;
			}
			else if ( !acct.CheckPassword( pw ) )
			{
				Console.WriteLine( "Polaczenie od: {0}: Niepoprawne haslo dla: '{1}'", e.State, un );
				m_Output.WriteLine( "Polaczenie od: {0}: Niepoprawne haslo dla: '{1}'", e.State, un );
				e.Accepted = false;
			}
			else if ( acct.Banned )
			{
				Console.WriteLine( "Polaczenie od: {0}: Konto zablokowane dla: '{1}'", e.State, un );
				m_Output.WriteLine( "Polaczenie od: {0}: Konto zablokowane dla: '{1}'", e.State, un );
				e.Accepted = false;
			}
			else
			{
				acct.LogAccess( e.State );

				Console.WriteLine( "Polaczenie od: {0}: Uzytkownik '{1}' wybiera postac", e.State, un );
				m_Output.WriteLine( "Polaczenie od: {0}: Uzytkownik '{1}' wybiera postac", e.State, un );
				m_Output.WriteLine( "" );
				e.State.Account = acct;
				e.Accepted = true;
				e.CityInfo = StartingCities;
			}

			if ( !e.Accepted )
				AccountAttackLimiter.RegisterInvalidAccess( e.State );
		}
	}
}