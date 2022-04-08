using System;
using System.IO;
using System.Collections;
using Server;

namespace Server.Engines.BulkOrders
{
	public class LargeBulkEntry
	{
		private LargeBOD m_Owner;
		private int m_Amount;
		private SmallBulkEntry m_Details;

		public LargeBOD Owner{ get{ return m_Owner; } set{ m_Owner = value; } }
		public int Amount{ get{ return m_Amount; } set{ m_Amount = value; if ( m_Owner != null ) m_Owner.InvalidateProperties(); } }
		public SmallBulkEntry Details{ get{ return m_Details; } }

		public static SmallBulkEntry[] LargeRing
		{
			get{ return GetEntries( "Blacksmith", "largering" ); }
		}

		public static SmallBulkEntry[] LargePlate
		{
			get{ return GetEntries( "Blacksmith", "largeplate" ); }
		}

		public static SmallBulkEntry[] LargeChain
		{
			get{ return GetEntries( "Blacksmith", "largechain" ); }
		}

		public static SmallBulkEntry[] LargeAxes
		{
			get{ return GetEntries( "Blacksmith", "largeaxes" ); }
		}

		public static SmallBulkEntry[] LargeFencing
		{
			get{ return GetEntries( "Blacksmith", "largefencing" ); }
		}

		public static SmallBulkEntry[] LargeMaces
		{
			get{ return GetEntries( "Blacksmith", "largemaces" ); }
		}

		public static SmallBulkEntry[] LargePolearms
		{
			get{ return GetEntries( "Blacksmith", "largepolearms" ); }
		}

		public static SmallBulkEntry[] LargeSwords
		{
			get{ return GetEntries( "Blacksmith", "largeswords" ); }
		}


		public static SmallBulkEntry[] BoneSet
		{
			get{ return GetEntries( "Tailoring", "boneset" ); }
		}

		public static SmallBulkEntry[] Farmer
		{
			get{ return GetEntries( "Tailoring", "farmer" ); }
		}

		public static SmallBulkEntry[] FemaleLeatherSet
		{
			get{ return GetEntries( "Tailoring", "femaleleatherset" ); }
		}

		public static SmallBulkEntry[] FisherGirl
		{
			get{ return GetEntries( "Tailoring", "fishergirl" ); }
		}

		public static SmallBulkEntry[] Gypsy
		{
			get{ return GetEntries( "Tailoring", "gypsy" ); }
		}

		public static SmallBulkEntry[] HatSet
		{
			get{ return GetEntries( "Tailoring", "hatset" ); }
		}

		public static SmallBulkEntry[] Jester
		{
			get{ return GetEntries( "Tailoring", "jester" ); }
		}

		public static SmallBulkEntry[] Lady
		{
			get{ return GetEntries( "Tailoring", "lady" ); }
		}

		public static SmallBulkEntry[] MaleLeatherSet
		{
			get{ return GetEntries( "Tailoring", "maleleatherset" ); }
		}

		public static SmallBulkEntry[] Pirate
		{
			get{ return GetEntries( "Tailoring", "pirate" ); }
		}

		public static SmallBulkEntry[] ShoeSet
		{
			get{ return GetEntries( "Tailoring", "shoeset" ); }
		}

		public static SmallBulkEntry[] StuddedSet
		{
			get{ return GetEntries( "Tailoring", "studdedset" ); }
		}

		public static SmallBulkEntry[] TownCrier
		{
			get{ return GetEntries( "Tailoring", "towncrier" ); }
		}

		public static SmallBulkEntry[] Wizard
		{
			get{ return GetEntries( "Tailoring", "wizard" ); }
		}

		public static SmallBulkEntry[] BowFletcher {
			get { return GetEntries("BowFletching", "weapons"); }
		}

		#region Hunter
		public static SmallBulkEntry[] Animal_1
		{
			get{ return GetHunterEntries( "Hunting", "Animal_1"); }
		}

		public static SmallBulkEntry[] Animal_2
		{
			get{ return GetHunterEntries( "Hunting", "Animal_2"); }
		}

		public static SmallBulkEntry[] Ants
		{
			get{ return GetHunterEntries( "Hunting", "Ants"); }
		}

		public static SmallBulkEntry[] Elementals_1
		{
			get{ return GetHunterEntries( "Hunting", "Elementals_1"); }
		}

		public static SmallBulkEntry[] Elementals_2
		{
			get{ return GetHunterEntries( "Hunting", "Elementals_2"); }
		}

		public static SmallBulkEntry[] Gargoyles
		{
			get{ return GetHunterEntries( "Hunting", "Gargoyles"); }
		}

		public static SmallBulkEntry[] Horda_1
		{
			get{ return GetHunterEntries( "Hunting", "Horda_1"); }
		}

		public static SmallBulkEntry[] Horda_2
		{
			get{ return GetHunterEntries( "Hunting", "Horda_2"); }
		}

		public static SmallBulkEntry[] Horda_3
		{
			get{ return GetHunterEntries( "Hunting", "Horda_3"); }
		}

		public static SmallBulkEntry[] Jukas
		{
			get{ return GetHunterEntries( "Hunting", "Jukas"); }
		}

		public static SmallBulkEntry[] Kox_1
		{
			get{ return GetHunterEntries( "Hunting", "Kox_1"); }
		}

		public static SmallBulkEntry[] Kox_2
		{
			get{ return GetHunterEntries( "Hunting", "Kox_2"); }
		}

		public static SmallBulkEntry[] Kox_3
		{
			get{ return GetHunterEntries( "Hunting", "Kox_3"); }
		}

		public static SmallBulkEntry[] Kox_4
		{
			get{ return GetHunterEntries( "Hunting", "Kox_4"); }
		}

		public static SmallBulkEntry[] Kox_5
		{
			get{ return GetHunterEntries( "Hunting", "Kox_5"); }
		}

		public static SmallBulkEntry[] Mech
		{
			get{ return GetHunterEntries( "Hunting", "Mech"); }
		}

		public static SmallBulkEntry[] Minotaurs
		{
			get{ return GetHunterEntries( "Hunting", "Minotaurs"); }
		}

		public static SmallBulkEntry[] Ophidians
		{
			get{ return GetHunterEntries( "Hunting", "Ophidians"); }
		}

		public static SmallBulkEntry[] Orcs
		{
			get{ return GetHunterEntries( "Hunting", "Orcs"); }
		}

		public static SmallBulkEntry[] OreElementals
		{
			get{ return GetHunterEntries( "Hunting", "OreElementals"); }
		}

		public static SmallBulkEntry[] Plants
		{
			get{ return GetHunterEntries( "Hunting", "Plants"); }
		}

		public static SmallBulkEntry[] Rozne
		{
			get{ return GetHunterEntries( "Hunting", "Rozne"); }
		}

		public static SmallBulkEntry[] Strong
		{
			get{ return GetHunterEntries( "Hunting", "Strong"); }
		}

		public static SmallBulkEntry[] Terathans
		{
			get{ return GetHunterEntries( "Hunting", "Terathans"); }
		}

		public static SmallBulkEntry[] Undead_1
		{
			get{ return GetHunterEntries( "Hunting", "Undead_1"); }
		}

		public static SmallBulkEntry[] Undead_2
		{
			get{ return GetHunterEntries( "Hunting", "Undead_2"); }
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
				table[name] = entries = SmallBulkEntry.LoadEntries( type, name );

			return entries;
		}

		public static LargeBulkEntry[] ConvertEntries( LargeBOD owner, SmallBulkEntry[] small )
		{
			LargeBulkEntry[] large = new LargeBulkEntry[small.Length];

			for ( int i = 0; i < small.Length; ++i )
				large[i] = new LargeBulkEntry( owner, small[i] );

			return large;
		}

		public LargeBulkEntry( LargeBOD owner, SmallBulkEntry details )
		{
			m_Owner = owner;
			m_Details = details;
		}

		public LargeBulkEntry( LargeBOD owner, GenericReader reader, int version )
		{
			m_Owner = owner;
			m_Amount = reader.ReadInt();

			Type realType = null;

			string type = reader.ReadString();

			if ( type != null )
				realType = ScriptCompiler.FindTypeByFullName( type );

			if ( version == 0 )
				// version 0:
				m_Details = new SmallBulkEntry( realType, reader.ReadInt(), reader.ReadInt() );
			else
			{
				// version 1:
				m_Details = new SmallBulkEntry( realType, reader.ReadInt(), reader.ReadInt() );
				reader.ReadInt();
			}
		}

		public void Serialize( GenericWriter writer, int version )
		{
			// version 0:
			writer.Write( m_Amount );
			writer.Write( m_Details.Type == null ? null : m_Details.Type.FullName );
			writer.Write( m_Details.Number );
			writer.Write( m_Details.Graphic );

			// version 1:
			if( version > 0 )
				writer.Write( m_Details.Level );
		}

		public static SmallBulkEntry[] GetHunterEntries( string type, string name )
		{	
			if ( m_Cache == null )
			{
				m_Cache = new Hashtable();
			}

			Hashtable table = (Hashtable)m_Cache[type]; // lista zlecen dla danego typu rzemiosla

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

				StreamWriter m_Output = null;
	
				try
				{
					m_Output = new StreamWriter( Path.Combine( directory, "HunterBODs.log" ), true );
					m_Output.AutoFlush = true;
					m_Output.WriteLine( "#################################");
					m_Output.WriteLine( "Log started on {0}", DateTime.Now );
					m_Output.WriteLine();
					m_Output.WriteLine( "### LARGE BOD ###");
					m_Output.Close();
				}
				catch
				{
				}
				
				table[name] = entries = SmallBulkEntry.LoadHunterEntries( type, name );
				
				try
				{
					m_Output = new StreamWriter( Path.Combine( directory, "HunterBODs.log" ), true );
					m_Output.AutoFlush = true;
					m_Output.WriteLine( "### LARGE BOD END ###" );
					m_Output.Close();
				}
				catch
				{
				}
			}

			return entries;
		}
	}
}