using System.Collections.Generic;
using Nelderim;

namespace Server.Items
{
	public partial class Corpse
	{
		[CommandProperty( AccessLevel.GameMaster )]
		public double CampingCarved
		{
			get { return CorpseExt.Get( this ).CampingCarved; }
			set { CorpseExt.Get( this ).CampingCarved = value; }
		}
	}

	class CorpseExt : NExtension<CorpseExtInfo>
	{
		public static string ModuleName = "CorpseExt";

		public static void Initialize()
		{
			EventSink.WorldSave += new WorldSaveEventHandler( Save );
			Load( ModuleName );
		}

		public static void Save( WorldSaveEventArgs args )
		{
			Cleanup();
			Save( args, ModuleName );
		}

		private static void Cleanup()
		{
			List<Serial> toRemove = new List<Serial>();
			foreach ( KeyValuePair<Serial, CorpseExtInfo> kvp in m_ExtensionInfo )
			{
				if ( World.FindItem( kvp.Key ) == null )
					toRemove.Add( kvp.Key );
			}
			foreach ( Serial serial in toRemove )
			{
				CorpseExtInfo removed;
				m_ExtensionInfo.TryRemove( serial, out removed );
			}
		}
	}

	class CorpseExtInfo : NExtensionInfo
	{
		private double m_CampingCarved;
		public double CampingCarved { get { return m_CampingCarved; } set { m_CampingCarved = value; } }

		public CorpseExtInfo()
		{
			m_CampingCarved = -1.0;
		}

		public override void Serialize( GenericWriter writer )
		{
			writer.Write( (int)0 ); //version
			writer.Write( m_CampingCarved );
		}

		public override void Deserialize( GenericReader reader )
		{
			int version = 0;
			if (Fix)
				version = reader.ReadInt(); //version  
			m_CampingCarved = reader.ReadDouble();
		}
	}
}
