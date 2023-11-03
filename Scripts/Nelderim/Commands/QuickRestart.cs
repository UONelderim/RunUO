using System;
using Server;
using System.IO;
using System.Diagnostics;
using Microsoft.CSharp;
using System.Text;
using System.Threading;
using System.CodeDom;
using System.CodeDom.Compiler;
using Server.Commands;
using System.Collections;
using System.Collections.Generic;
using Server.Mobiles;
using Server.Gumps;
using Server.Misc;

namespace Server
{
	public class QuickRestart
	{
		private static bool m_Restarting = false;
		
		private static Mobile m_Mobile;
		private static bool m_Debug;
		private static bool m_Service;
		private static bool m_Profiling;
		private static bool m_HaltOnWarning;

		public static bool Restarting
		{
			get{ return m_Restarting; }
		}

		public static void Initialize()
		{
			CommandSystem.Register( "QuickRestart", AccessLevel.Owner, new CommandEventHandler( QuickRestart_OnCommand ) );
		}
		
		public static string FormatTimeSpan( TimeSpan ts )
		{
			return String.Format( "{0:D2}:{1:D2}:{2:D2}:{3:D2}", ts.Days, ts.Hours % 24, ts.Minutes % 60, ts.Seconds % 60 );
		}

		[Usage( "QuickRestart" )]
		[Description( "Szybki restart serwera - kompilowanie skryptow podczas UP-a." )]
		public static void QuickRestart_OnCommand( CommandEventArgs e )
		{
			string m_Args = e.ArgString.ToLower();

			if( m_Args.IndexOf( "-debug" ) != -1 )
				m_Debug = true;
			if( m_Args.IndexOf( "-service" ) != -1 )
				m_Service = true;
			if( m_Args.IndexOf( "-profile" ) != -1 )
				m_Profiling = true;
			if( m_Args.IndexOf( "-haltonwarning" ) != -1 )
				m_HaltOnWarning = true;

			if ( m_Restarting || AutoRestart.Restarting || Restartb.Restarting || Down.ShutDown || Downb.ShutDown || ResplanAlfa.Restarting || DownplanAlfa.ShutDown )
			{
				e.Mobile.SendMessage( 38, "Serwer jest podczas Restartu lub Wylaczania." );
				return;
			}
			
			string txt = "Serwer jest aktualnie przygotowywany do restartu, prosze sie nie logowac!";

			e.Mobile.SendMessage( "Wtorny Restart zostal wlaczony." );
			Console.WriteLine( "" );
			Console.WriteLine( "Uruchomiono Wtorny Restart..." );
			Console.WriteLine( "" );

			m_Restarting = true;
			m_Mobile = e.Mobile;

			new Thread(new ThreadStart(CheckRestart)).Start();
		}

		public static void CheckRestart()
		{
			string[] files = ScriptCompiler.GetScripts( "*.cs" );

			if ( files.Length == 0 )
			{
				m_Mobile.SendMessage("No scripts found to compile!");
				return;
			}

			if(AlreadyCached(files))
			{
				m_Mobile.SendMessage("Skrypty zostaly przetworzone. Rozpoczyna sie Restart...");
				DoSave(false, false);
				return;
			}

			using ( CSharpCodeProvider provider = new CSharpCodeProvider() )
			{
				string path = GetUnusedPath( "Scripts.CS" );

				CompilerParameters parms = new CompilerParameters( ScriptCompiler.GetReferenceAssemblies(), path, m_Debug );

				string defines = ScriptCompiler.GetDefines();

				if ( defines != null )
					parms.CompilerOptions = string.Format( "/D:{0}", defines );

				m_Mobile.SendMessage("Kompilacja skryptow C#, prosze czekac...");
				World.Broadcast(138, true, "Przygotowywanie Servera do Restartu...");

				CompilerResults results = provider.CompileAssemblyFromFile( parms, files );

				if ( results.Errors.Count > 0 )
				{
					ConsoleColor color = Console.ForegroundColor;
          for (int i = 0; i < results.Errors.Count; i++)
          {
	          if (!(results.Errors[i].IsWarning))
	          {
							Console.ForegroundColor = ConsoleColor.Red;
	            Console.WriteLine("QuickRestart: Warning: {0}", results.Errors[i].ToString());
	          }
          }
          Console.ForegroundColor = color;
                    
					m_Mobile.SendMessage("Wystapil blad przy komplikowaniu skryptow, server nie zostanie zrestartowany!");
					World.Broadcast(138, true, "Przerwano Restart Servera...");
					Console.WriteLine( "" );
					Console.WriteLine( "Wtorny restart zostal przerwany !!!" );
					Console.WriteLine( "" );
					m_Restarting = false;
					return;
				}

				if ( Path.GetFileName( path ) == "Scripts.CS.dll.new" )
				{
					try
					{
						byte[] hashCode = GetHashCode( path, files, false );

						using ( FileStream fs = new FileStream( "Output/Scripts.CS.hash.new", FileMode.Create, FileAccess.Write, FileShare.None ) )
						{
							using ( BinaryWriter bin = new BinaryWriter( fs ) )
							{
								bin.Write( hashCode, 0, hashCode.Length );
							}
						}
					}
					catch { }
				}

				m_Mobile.SendMessage("Server zostal poprawnie przygotowany do restartu. Restart...");
				DoSave(true, true);
			}
		}

		public static void DoRestart(bool normal)
		{
			Misc.AutoSave.Save();

			if(normal)
			{
				if( File.Exists("QuickRestart.exe") )
				{
					Process.Start("QuickRestart.exe", Arguments());
					Core.Process.Kill();
				}
			}

			Process.Start( Core.ExePath, Arguments() );
			Core.Process.Kill();
		}

		public static void DoSave(bool normal, bool save)
		{
			if(save)
			{
				Misc.AutoSave.Save();
			}
			
			RestartTimer rTimer = new RestartTimer(normal);
			rTimer.Start();
		}
		
		public static bool AlreadyCached(string [] files)
		{
			if ( File.Exists( "Output/Scripts.CS.dll" ) )
			{
				if ( File.Exists( "Output/Scripts.CS.hash" ) )
				{
					try
					{
						byte[] hashCode = GetHashCode( "Output/Scripts.CS.dll", files, m_Debug );

						using ( FileStream fs = new FileStream( "Output/Scripts.CS.hash", FileMode.Open, FileAccess.Read, FileShare.Read ) )
						{
							using ( BinaryReader bin = new BinaryReader( fs ) )
							{
								byte[] bytes = bin.ReadBytes( hashCode.Length );

								if ( bytes.Length == hashCode.Length )
								{
									bool valid = true;

									for ( int i = 0; i < bytes.Length; ++i )
									{
										if ( bytes[i] != hashCode[i] )
										{
											valid = false;
											break;
										}
									}

									if ( valid )
									{
										m_Mobile.SendMessage("Skrypty moga byc przetwazane. Rozpoczyna sie Wturny Restart...");
										return true;
									}
								}
							}
						}
					}
					catch
					{
						m_Mobile.SendMessage("Read error. Continuing...");
					}
				}
			}
			return false;
		}

		public static string GetUnusedPath( string name )
		{
			string path = Path.Combine( Core.BaseDirectory, String.Format( "Output/{0}.dll.new", name ) );

			for ( int i = 2; File.Exists( path ) && i <= 1000; ++i )
				path = Path.Combine( Core.BaseDirectory, String.Format( "Output/{0}.{1}.dll.new", name, i ) );

			return path;
		}

		private static byte[] GetHashCode( string compiledFile, string[] scriptFiles, bool debug )
		{
			using ( MemoryStream ms = new MemoryStream() )
			{
				using ( BinaryWriter bin = new BinaryWriter( ms ) )
				{
					FileInfo fileInfo = new FileInfo( compiledFile );
					bin.Write( fileInfo.LastWriteTimeUtc.Ticks );

					foreach ( string scriptFile in scriptFiles )
					{
						fileInfo = new FileInfo( scriptFile );
						bin.Write( fileInfo.LastWriteTimeUtc.Ticks );
					}

					bin.Write( debug );
					bin.Write( Core.Version.ToString() );
					
					ms.Position = 0;

					using ( System.Security.Cryptography.SHA1 sha1 = System.Security.Cryptography.SHA1.Create() )
					{
						return sha1.ComputeHash( ms );
					}
				}
			}
		}

		private static string Arguments()
		{
			StringBuilder sb = new StringBuilder();
 
			if( m_Debug )
				sb.Append( " -debug" );
			if( m_Service )
				sb.Append( " -service" );
			if( m_Profiling )
				sb.Append( " -profile" );
			if( m_HaltOnWarning )
				sb.Append( " -haltonwarning" );

			return sb.ToString();
		}
	}
	
	public class RestartTimer : Timer
	{
		public bool m_Normal;
		public byte count = 120;
		public RestartTimer (bool normal) : base (TimeSpan.FromSeconds(1.0), TimeSpan.FromSeconds(10.0))
		{
			m_Normal = normal;
		}
		protected override void OnTick()
		{
			if(count == 120)
			{
				List<Mobile> mobs = new List<Mobile>( World.Mobiles.Values );
			
				foreach ( Mobile m in mobs ) 
				{
					if (m != null && m is PlayerMobile)
					{
						m.CloseGump ( typeof( ZamykanieSwiataGump) );
						m.SendGump ( new ZamykanieSwiataGump() );
					}
				}
				Console.WriteLine( "" );
				Console.WriteLine( "Skrypty zostly przygotowane - restart nastapi za 2 minuty" );
				Console.WriteLine( "" );
				count -= 10;
			}
			else if(count == 1)
			{
				World.Broadcast(1154,true, "1 Sekunda!");
				count -= 1;
			}
			else if(count == 0)
			{
				Stop();
				QuickRestart.DoRestart(m_Normal);
			}
			else
			{
				World.Broadcast(1154,true, "Pozostalo {0} Sekund", count);
				count -= 10;
			}
		}
	}
}