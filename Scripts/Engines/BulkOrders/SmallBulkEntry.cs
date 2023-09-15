using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Server;

namespace Server.Engines.BulkOrders
{
	public class SmallBulkEntry
	{
		private static Dictionary<Type, int> m_HunterLevelCache = new Dictionary<Type, int>();

		public static int GetHunterBulkLevel( Type type )
		{
			int result;
			if ( m_HunterLevelCache.TryGetValue( type, out result ) )
				return result;
			Console.WriteLine( "!!!WARNING!!!" );
			Console.WriteLine( "Not found HunterBulkLevel for " + type.ToString() );
			return 1;
		}

		public static int GetHunterBulkLevel( SmallBOD bod )
		{
			int result;
			if ( bod != null && bod is SmallHunterBOD && m_HunterLevelCache.TryGetValue( bod.Type, out result ) )
				return result;
			Console.WriteLine( "!!!WARNING!!!" );
			Console.WriteLine( "Not found HunterBulkLevel for " + bod.ToString() );
			return 1;
		}

		public static int GetHunterBulkLevel( LargeBOD bod )
		{
			int result;
			if ( bod != null && bod is LargeHunterBOD && bod.Entries != null && bod.Entries.Length > 0 && bod.Entries[0] != null && bod.Entries[0].Details != null )
				if ( m_HunterLevelCache.TryGetValue( bod.Entries[0].Details.Type, out result ) )
					return result;
			Console.WriteLine( "!!!WARNING!!!" );
			Console.WriteLine( "Not found HunterBulkLevel for " + bod.ToString() );
			return 1;
		}

		static SmallBulkEntry()
		{
			//To fill HunterLevelCache
			SmallBulkEntry[] x;
			x = Hunter1;
			x = Hunter2;
			x = Hunter3;
            x = Hunter4;
        }

		private Type m_Type;
		private int m_Number;
		private int m_Graphic;
		//private int m_Level;

		public Type Type{ get{ return m_Type; } }
		public int Number{ get{ return m_Number; } }
		public int Graphic{ get{ return m_Graphic; } }
		public int Level{ get{ return GetHunterBulkLevel(m_Type); } }

		public SmallBulkEntry( Type type, int number, int graphic )
		{
			m_Type = type; ;
			m_Number = number;
			m_Graphic = graphic;
			//m_Level = level;
		}

		public static SmallBulkEntry[] BlacksmithWeapons
		{
			get{ return GetEntries( "Blacksmith", "weapons" ); }
		}

		public static SmallBulkEntry[] BlacksmithArmor
		{
			get{ return GetEntries( "Blacksmith", "armor" ); }
		}

		public static SmallBulkEntry[] TailorCloth
		{
			get{ return GetEntries( "Tailoring", "cloth" ); }
		}

		public static SmallBulkEntry[] TailorLeather
		{
			get{ return GetEntries( "Tailoring", "leather" ); }
		}

		public static SmallBulkEntry[] BowFletcher {
			get { return GetEntries("BowFletching", "weapons"); }
		}

		#region Hunter
		public static SmallBulkEntry[] Hunter1
		{
			get{ return GetHunterEntries( "Hunting", "small-1-lvl" ); }
		}
		
		public static SmallBulkEntry[] Hunter2
		{
			get{ return GetHunterEntries( "Hunting", "small-2-lvl" ); }
		}
		
		public static SmallBulkEntry[] Hunter3
		{
			get{ return GetHunterEntries( "Hunting", "small-3-lvl" ); }
        }

        public static SmallBulkEntry[] Hunter4
        {
            get { return GetHunterEntries("Hunting", "small-4-lvl"); }
        }

        #endregion

        private static Hashtable m_Cache;

		public static SmallBulkEntry[] GetEntries( string type, string name )
		{
			if ( m_Cache == null )
				m_Cache = new Hashtable();

			Hashtable table = (Hashtable)m_Cache[type];

			if ( table == null )
				m_Cache[type] = table = new Hashtable();

			SmallBulkEntry[] entries = (SmallBulkEntry[])table[name];

			if ( entries == null )
				table[name] = entries = LoadEntries( type, name );

			return entries;
		}

		public static SmallBulkEntry[] LoadEntries( string type, string name )
		{
			return LoadEntries( String.Format( "Data/Bulk Orders/{0}/{1}.cfg", type, name ) );
		}

		public static SmallBulkEntry[] LoadEntries( string path )
		{
			path = Path.Combine( Core.BaseDirectory, path );

			List<SmallBulkEntry> list = new List<SmallBulkEntry>();

			if ( File.Exists( path ) )
			{
				using ( StreamReader ip = new StreamReader( path ) )
				{
					string line;

					while ( (line = ip.ReadLine()) != null )
					{
						if ( line.Length == 0 || line.StartsWith( "#" ) )
							continue;

						try
						{
							string[] split = line.Split( '\t' );

							if ( split.Length >= 2 )
							{
								Type type = ScriptCompiler.FindTypeByName( split[0] );
								int graphic = Utility.ToInt32( split[split.Length - 1] );

								if ( type != null && graphic > 0 )
									list.Add( new SmallBulkEntry( type, 1020000 + graphic, graphic ) );
							}
						}
						catch
						{
						}
					}
				}
			}

			return list.ToArray();
		}

		public static SmallBulkEntry[] GetHunterEntries( string type, string name )
		{
			if ( m_Cache == null )
				m_Cache = new Hashtable();

			Hashtable table = (Hashtable)m_Cache[type];

			if ( table == null )
				m_Cache[type] = table = new Hashtable();

			SmallBulkEntry[] entries = (SmallBulkEntry[])table[name];

			if ( entries == null )
			{
				if ( !Directory.Exists( "Logi" ) )
						Directory.CreateDirectory( "Logi" );

				string directory = "Logi/HunterBulkOrders";

				if ( !Directory.Exists( directory ) )
					Directory.CreateDirectory( directory );

				try
				{
					StreamWriter m_Output = new StreamWriter( Path.Combine( directory, "HunterBODs.log" ), true );
					m_Output.AutoFlush = true;
					m_Output.WriteLine( "#################################");
					m_Output.WriteLine( "Log started on {0}", DateTime.Now );
					m_Output.WriteLine();
					m_Output.Flush();
					m_Output.Close();
				}
				catch
				{
				}
			
				table[name] = entries = LoadHunterEntries( type, name );
			}

			return entries;
		}
		
		public static SmallBulkEntry[] LoadHunterEntries( string type, string name )
		{
			return LoadHunterEntries( String.Format( "Data/Bulk Orders/{0}/{1}.cfg", type, name ) );
		}
		
		public static SmallBulkEntry[] LoadHunterEntries( string path )
		{
			path = Path.Combine( Core.BaseDirectory, path );

			ArrayList list = new ArrayList();

			if ( File.Exists( path ) )
			{
				using ( StreamReader ip = new StreamReader( path ) )
				{
					string line;
					
					if ( !Directory.Exists( "Logi" ) )
						Directory.CreateDirectory( "Logi" );

					string directory = "Logi/HunterBulkOrders";

					if ( !Directory.Exists( directory ) )
						Directory.CreateDirectory( directory );


					StreamWriter m_Output = null;
				
					try
					{
						m_Output = new StreamWriter( Path.Combine( directory, "HunterBODs.log" ), true );
						m_Output.AutoFlush = true;
					}
					catch
					{
					}

					while ( (line = ip.ReadLine()) != null )
					{
						if ( line.Length == 0 || line.StartsWith( "#" ) )
							continue;

						try
						{
							string[] split = line.Split( '\t' );

							if ( split.Length >= 2 )
							{
								Type type = ScriptCompiler.FindTypeByName( split[0] );
								//int graphic = Utility.ToInt32( split[split.Length - 1] );

								if( type != null && split.Length >= 3 )
								{
									if( split.Length == 3 )
									{
										var level = Utility.ToInt32( split[2] );

										if ( m_HunterLevelCache.ContainsKey( type ) )
										{
											if ( m_HunterLevelCache[type] != level )
												throw new InvalidDataException( "Niejednolita konfiguracja HunterBulk dla typu : " + type );
										}
										else
											m_HunterLevelCache.Add( type, level );

										list.Add( new SmallBulkEntry( type, Utility.ToInt32(split[1]), 0 ) );
										
									    if(m_Output != null)
										    m_Output.WriteLine("Bulk: {0}, {1}, {2}", split[0], split[1], split[2]);
								    }/*
								    else
								    {
									    list.Add( new SmallBulkEntry( type, Utility.ToInt32(split[1]), Utility.ToInt32(split[split.Length-1]), split[2] ) );
										 
									    if(m_Output != null)
									    	m_Output.WriteLine("Bulk: {0}, {1}, {2}, {3}", split[0], split[1], split[2], split[split.Length-1]);
								    }*/
								 }
							}
						}
						catch
						{
						}
					}
					if(m_Output != null)
						m_Output.Close();
				}
			}

			return (SmallBulkEntry[])list.ToArray( typeof( SmallBulkEntry ) );
		}

	}
}