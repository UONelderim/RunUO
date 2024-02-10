using Nelderim.Speech;
using Nelderim;
using System.Collections.Generic;
using System.Net.Sockets;
using Server.Network;

namespace Server.Items
{
	public partial class BaseDoor : ILockpickable
	{
		public virtual void LockPick(Mobile from)
		{
			if (typeof(BaseHouseDoor).IsAssignableFrom(GetType()))
			{
				from.SendMessage("Chcialbys...");
				return;
			}
			if (Open)
			{
				from.SendMessage("Po co wywazac otwarte drzwi?");
				return;
			}
			if (from.AccessLevel == AccessLevel.Player && !from.InRange(GetWorldLocation(), 2))
			{
				from.LocalOverheadMessage(MessageType.Regular, 0x3B2, 1019045); // I can't reach that.
				return;
			}

			Use(from, true);
		}

		public Mobile Picker
		{
			get { return null; }
			set { }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int MaxLockLevel
		{
			get { return BaseDoorExt.Get(this).MaxLockLevel; }
			set { BaseDoorExt.Get(this).MaxLockLevel = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int LockLevel
		{
			get { return BaseDoorExt.Get(this).LockLevel; }
			set { BaseDoorExt.Get(this).LockLevel = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int RequiredSkill
		{
			get { return BaseDoorExt.Get(this).RequiredSkill; }
			set { BaseDoorExt.Get(this).RequiredSkill = value; }
		}
	}

	class BaseDoorExt : NExtension<BaseDoorExtInfo>
	{
		private static string ModuleName = "BaseDoor";

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
			foreach (KeyValuePair<Serial, BaseDoorExtInfo> kvp in m_ExtensionInfo)
			{
				if (World.FindItem(kvp.Key) == null)
					toRemove.Add(kvp.Key);
			}
			foreach (Serial serial in toRemove)
			{
				BaseDoorExtInfo removed;
				m_ExtensionInfo.TryRemove(serial, out removed);
			}
		}
	}

	class BaseDoorExtInfo : NExtensionInfo
	{
		private int m_LockLevel = 0, m_MaxLockLevel, m_RequiredSkill;

		[CommandProperty(AccessLevel.GameMaster)]
		public int MaxLockLevel
		{
			get { return m_MaxLockLevel; }
			set { m_MaxLockLevel = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int LockLevel
		{
			get { return m_LockLevel; }
			set { m_LockLevel = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public int RequiredSkill
		{
			get { return m_RequiredSkill; }
			set { m_RequiredSkill = value; }
		}
	
		public BaseDoorExtInfo()
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			writer.Write((int)0); // version

			writer.Write((int)m_LockLevel);
			writer.Write((int)m_MaxLockLevel);
			writer.Write((int)m_RequiredSkill);
		}

		public override void Deserialize(GenericReader reader)
		{
			int version = reader.ReadInt();

			m_LockLevel = reader.ReadInt();
			m_MaxLockLevel = reader.ReadInt();
			m_RequiredSkill = reader.ReadInt();
		}
	}
}