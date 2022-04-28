using System;
using System.Collections.Generic;
using Nelderim;

namespace Server.Items
{
	public partial class RepairDeed
	{
		[CommandProperty(AccessLevel.GameMaster)]
		public int UsesRemaining
		{
			get { return RepairDeedExt.Get(this).UsesRemaining; }
			set { RepairDeedExt.Get(this).UsesRemaining = Math.Max(Math.Min(value, UsesMax), 0); InvalidateProperties(); }
		}

		public int UsesMax
		{
			get { return 10; } // feel free to return different values for various craft skills
		}
	}

	class RepairDeedExt : NExtension<RepairDeedExtInfo>
	{
		private static string ModuleName = "RepairDeed";

		public static void Initialize()
		{
			EventSink.WorldSave += new WorldSaveEventHandler(Save);
			Load(ModuleName);
		}

		public static void Save(WorldSaveEventArgs args)
		{
			Cleanup();
			Save(args, ModuleName);
		}

		private static void Cleanup()
		{
			List<Serial> toRemove = new List<Serial>();
			foreach (KeyValuePair<Serial, RepairDeedExtInfo> kvp in m_ExtensionInfo)
			{
				if (World.FindItem(kvp.Key) == null)
					toRemove.Add(kvp.Key);
			}
			foreach (Serial serial in toRemove)
			{
				RepairDeedExtInfo removed;
				m_ExtensionInfo.TryRemove(serial, out removed);
			}
		}
	}

	class RepairDeedExtInfo : NExtensionInfo
	{
		private int m_UsesRemaining;

		public int UsesRemaining { get { return m_UsesRemaining; } set { m_UsesRemaining = value; } }

		public RepairDeedExtInfo()
		{
			m_UsesRemaining = 1;
		}

		public override void Serialize(GenericWriter writer)
		{
			writer.Write((int)1); // version

			writer.Write(m_UsesRemaining);
		}

		public override void Deserialize(GenericReader reader)
		{
			int version = reader.ReadInt();

			m_UsesRemaining = reader.ReadInt();
		}
	}
}
