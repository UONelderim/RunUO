using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Net;
using Server.Commands;

namespace Server.Accounting
{
	public class Accounts
	{
		private static Dictionary<string, IAccount> m_Accounts = new Dictionary<string, IAccount>();

        public static Dictionary<string, IAccount> Table
        {
            get { return m_Accounts; }
        }

		private static Hashtable m_AccsByMail = new Hashtable( );

		public static void Initialize()
		{
			CommandSystem.Register( "ClearIPs", AccessLevel.Administrator, new CommandEventHandler( ClearIPs_OnCommand ) );
			CommandSystem.Register( "DumpAccounts", AccessLevel.Administrator, new CommandEventHandler( DumpAccounts_OnCommand ) );
			//CommandSystem.Register( "WrongAccounts", AccessLevel.Administrator, new CommandEventHandler( WrongAccounts_OnCommand ) );
			CommandSystem.Register( "CleanAccounts", AccessLevel.Administrator, new CommandEventHandler( CleanAccounts_OnCommand ) );
		}
		
		[Usage( "ClearIPs <account>" )]
		[Description( "Zeruje liste IP dla wszystkich kont, lub konta <account>" )]
		public static void ClearIPs_OnCommand( CommandEventArgs e )
		{			
			try
			{
				if ( e.Length ==0 ) 
				{
					foreach ( Account a in GetAccounts() )
					{
						e.Mobile.SendMessage( "Zeruje liste IP konta: " + a.Username );
						a.LoginIPs = new IPAddress[0];
					}
				}	
				else
				{	
					string login = e.GetString( 0 );
				
					foreach ( Account a in GetAccounts() )
					{
						if ( a.Username == login ) 
						{
							e.Mobile.SendMessage( "Zeruje liste IP konta: " + a.Username );
							a.LoginIPs = new IPAddress[0];
						}
					}
				}
			}
			catch( Exception exc)
			{
				Console.WriteLine( exc.ToString() );
			}
		}
		
		[Usage( "DumpAccounts" )]
		[Description( "Zapisauje w pliku tekstowym wszystkie konta serwera" )]
		public static void DumpAccounts_OnCommand( CommandEventArgs e )
		{	
			try
			{
				StreamWriter writer = new StreamWriter("Logi/accounts.log", false);
		
				foreach ( Account a in GetAccounts() )
				{
					writer.Write( a.Username );
					writer.Write( "	" );
					writer.WriteLine( a.AccessLevel );
				}
					
				writer.Close();
			}
			catch ( Exception exc )
			{
				Console.WriteLine( exc.ToString() );
			}
		}
		
		/*[Usage( "WrongAccounts <ban|delete>" )]
		[Description( "Wylapuje konta niezgadzajace sie z lista kont graczy na forum. " +
		             "Opcja <ban> blokuje konta. " +
		             "Opcja <delete> kasuje zle konta." )]
		public static void WrongAccounts_OnCommand( CommandEventArgs e )
		{	
			int task = 0; // 0 - list | 1 - ban | 2 - delete
			
			try
			{
				if ( e.Length > 0 )
				{
					if( e.GetString(0) == "ban" )
						task = 1;
					else if( e.GetString(0) == "delete" )
						task = 2;
				}
			
				StreamWriter writer = new StreamWriter("Logi/wrongacc.log", false);
					
				#region XML
					
				XmlTextReader xml = new XmlTextReader( "Logi/uonelderim_users.xml" );
				xml.WhitespaceHandling = WhitespaceHandling.None;
				XmlValidatingReader validXML = new XmlValidatingReader(xml);
				validXML.ValidationType = ValidationType.DTD;
				XmlDocument doc = new XmlDocument();
				doc.Load(validXML);
					                                      
				ArrayList forumAccounts = new ArrayList();
					
				XmlElement root = doc["nelderim"];
				
				foreach ( XmlElement uonelderim_users in root.GetElementsByTagName( "uonelderim_users" ) )
				{
					XmlElement username = uonelderim_users.GetElementsByTagName("username").Item( 0 ) as XmlElement;
					String Username = username.InnerText;
					
					forumAccounts.Add( ( object ) Username.ToLower() );
				}
				Console.WriteLine( "Liczba kont na forum - {0}", forumAccounts.Count );
					
				#endregion
					
				int count = 0;
				ArrayList toDelete = new ArrayList();
					
				foreach ( Account a in GetAccounts() )
				{
					if ( !forumAccounts.Contains( ( object ) a.Username.ToLower() ) && a.AccessLevel == AccessLevel.Player )
					{
						writer.WriteLine( a.Username + " ( " + a.LastLogin.ToString() + " )" );
						e.Mobile.SendMessage( a.Username );
						
						if ( task == 1 )
							a.Banned = true;
						else if ( task == 2 )
							toDelete.Add( a );
						count++;
					}
				}
				Console.WriteLine( "Liczba zlych kont - {0}", count );
				e.Mobile.SendMessage( "Liczba zlych kont - " + count.ToString() );
				
				if ( task == 2 )
				{
					for ( int i = toDelete.Count - 1; i >= 0; i-- )
					{
						Account a = toDelete[i] as Account;
						
						//Console.WriteLine( "i = {0}", i );
						writer.WriteLine( a.Username + " | LastLogin: " + a.LastLogin.ToString() );
						e.Mobile.SendMessage( "Usuwam konto: " + a.Username + " ( " + a.LastLogin.ToString() + " )" );
						a.Delete();
					}
				}
					
				writer.Close();
			}
			catch ( Exception exc )
			{
				Console.WriteLine( exc.ToString() );
			}
		}*/
		
		[Usage( "CleanAccounts <TimeSpan>" )]
		[Description( "Kasuje wszystkie konta nie uzywane od 3 miesiecy / <TimeSpan> w dniach" )]
		public static void CleanAccounts_OnCommand( CommandEventArgs e )
		{	
			TimeSpan age = TimeSpan.FromDays( 120 );
			ArrayList toDelete = new ArrayList();
			
			if ( e.Length > 0 )
			{
				try
				{
					age = TimeSpan.FromDays( e.GetInt32( 0 ) );
				}
				catch
				{}
			}
			
			DateTime now = DateTime.Now;
			DateTime Threshold = now - age;
			
			StreamWriter writer = new StreamWriter("Logi/delacc.log", true);
			
			writer.WriteLine( "***** " + now.ToString() + " *****" );
			Console.WriteLine( Threshold );
			
			try
			{
				foreach ( Account a in GetAccounts() )
				{
					if ( a.LastLogin < Threshold )
						toDelete.Add( a );
				}
				
				for ( int i = toDelete.Count - 1; i >= 0; i-- )
				{
					Account a = toDelete[i] as Account;
					
					// Console.WriteLine( "i = {0}", i );
					writer.WriteLine( a.Username + " | LastLogin: " + a.LastLogin.ToString() );
					e.Mobile.SendMessage( "Usuwam konto: " + a.Username + " ( " + a.LastLogin.ToString() + " )" );
					a.Delete();
				}
			}
			catch ( Exception exc )
			{
				Console.WriteLine( exc.ToString() );
			}
			
			writer.Close();
		}
		
		public static void Configure()
		{
			EventSink.WorldLoad += new WorldLoadEventHandler( Load );
			EventSink.WorldSave += new WorldSaveEventHandler( Save );
		}

		static Accounts()
		{
		}

		public static int Count { get { return m_Accounts.Count; } }

		public static ICollection<IAccount> GetAccounts()
		{
#if !MONO
			return m_Accounts.Values;
#else
			return new List<IAccount>( m_Accounts.Values );
#endif
		}

		public static IAccount GetAccount( string username )
		{
			IAccount a;

			m_Accounts.TryGetValue( username, out a );

			return a;
		}
		
		/* Neotr - system zakladania kont */
		public static Account AddAccount( string user, string pass, string email )
		{
			Account a = new Account( user, pass );
			if( m_Accounts.Count == 0 )
				a.AccessLevel = AccessLevel.Administrator;

			m_Accounts[a.Username] = a;

			SetEmail( a, email );

			return a;
		}

		public static void SetEmail( Account acc, string email )
		{
			if( acc.Email == "" || acc.Email != email )
				acc.Email = email;
		}

		public static bool RegisterEmail( Account acc, string newMail )
		{
			UnregisterEmail( acc.Email );
			if( newMail == "" )
				return true;
			if( m_AccsByMail.Contains( newMail ) )
				return false;
			m_AccsByMail.Add( newMail, acc );
			return true;
		}

		public static void UnregisterEmail( string mail )
		{
			if( mail != null && mail != "" )
				m_AccsByMail.Remove( mail );
		}

		public static Account GetByMail( string email )
		{
			return m_AccsByMail[email] as Account;
		}
/* Neotr - system zakladania kont */

		public static void Add( IAccount a )
		{
			m_Accounts[a.Username] = a;
		}
		
		public static void Remove( string username )
		{
			m_Accounts.Remove( username );
		}

		public static void Load()
		{
			m_Accounts = new Dictionary<string, IAccount>( 32, StringComparer.OrdinalIgnoreCase );

			string filePath = Path.Combine( "Saves/Accounts", "accounts.xml" );

			if ( !File.Exists( filePath ) )
				return;

			XmlDocument doc = new XmlDocument();
			doc.Load( filePath );

			XmlElement root = doc["accounts"];

			foreach ( XmlElement account in root.GetElementsByTagName( "account" ) )
			{
				try
				{
					Account acct = new Account( account );
				}
				catch
				{
					Console.WriteLine( "Warning: Account instance load failed" );
				}
			}
		}

		public static void Save( WorldSaveEventArgs e )
		{
			if ( !Directory.Exists( "Saves/Accounts" ) )
				Directory.CreateDirectory( "Saves/Accounts" );

			string filePath = Path.Combine( "Saves/Accounts", "accounts.xml" );

			using ( StreamWriter op = new StreamWriter( filePath ) )
			{
				XmlTextWriter xml = new XmlTextWriter( op );

				xml.Formatting = Formatting.Indented;
				xml.IndentChar = '\t';
				xml.Indentation = 1;

				xml.WriteStartDocument( true );

				xml.WriteStartElement( "accounts" );

				xml.WriteAttributeString( "count", m_Accounts.Count.ToString() );

				foreach ( Account a in GetAccounts() )
					a.Save( xml );

				xml.WriteEndElement();

				xml.Close();
			}
		}
	}
}