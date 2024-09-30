using System;
using System.Collections.Generic;
using Nelderim;
using Server.Engines.BulkOrders;

namespace Server.Items
{
	public partial class Corpse
	{
		[CommandProperty( AccessLevel.GameMaster )]
		public double CampingCarved
		{
			get => CorpseExt.Get( this ).CampingCarved;
			set => CorpseExt.Get( this ).CampingCarved = value;
		}

		public List<Mobile> Hunters { get; } = new();
		public List<SmallHunterBOD> HunterBods { get; } = new();

        private DateTime lastCampingSkillcheck = DateTime.MinValue;
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
				m_ExtensionInfo.TryRemove( serial, out _ );
			}
		}
	}

	class CorpseExtInfo : NExtensionInfo
	{
		public double CampingCarved { get; set; } = -1.0;

		public override void Serialize( GenericWriter writer )
		{
			writer.Write( (int)0 ); //version
			writer.Write( CampingCarved );
		}

		public override void Deserialize( GenericReader reader )
		{
			int version = 0;
			if (Fix)
				version = reader.ReadInt(); //version  
			CampingCarved = reader.ReadDouble();
		}
	}
}
