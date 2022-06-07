using System;
using System.IO;
using System.Text;
using Server;
using Server.Network;
using Server.Gumps;
using Server.Multis;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Mobiles;
using Nelderim.Speech;
using Nelderim;

namespace Server.Items
{
	public partial class BaseBook
	{
		private SpeechLang m_Language;

		[CommandProperty(AccessLevel.GameMaster)]
		public SpeechLang Language
		{
			get { return BaseBookExt.Get(this).Language; }
			set { BaseBookExt.Get(this).Language = value; InvalidateProperties(); }
		}

		public override void GetProperties(ObjectPropertyList list)
		{
			base.GetProperties(list);

			list.Add(1060658, "Język\t{0}", Enum.GetName(typeof(SpeechLang), Language));
		}

		public void ClearContent()
		{
			m_Pages = new BookPageInfo[m_Pages.Length];

			for (int i = 0; i < m_Pages.Length; ++i)
				m_Pages[i] = new BookPageInfo();
		}

		private bool IsEmpty()
		{
			foreach (BookPageInfo page in Pages)
				foreach (string line in page.Lines)
					if (line.Length > 0)
						return false;

			return true;
		}

		public bool CheckChangeLanguage(Mobile from)
		{
			if (!IsAccessibleTo(from) || !from.InRange(GetWorldLocation(), 1))
			{
				from.SendMessage("Księga niedostępna.");
				return false;
			}

			if (!Writable)
			{
				from.SendMessage("Tusz zostal zalakowany, nie mozna go wymazac.");
				return false;
			}

			if (!IsEmpty())
			{
				from.SendMessage("Nie możesz zmienić języka księgi, która jest już zapisana.");
				return false;
			}

			return true;
		}

		public bool CheckClearContent(Mobile from)
		{
			if (!IsAccessibleTo(from) || !from.InRange(GetWorldLocation(), 1))
			{
				from.SendMessage("Księga niedostępna.");
				return false;
			}

			if (!Writable)
			{
				from.SendMessage("Tusz zostal zalakowany, nie mozna go wymazac.");
				return false;
			}

			return true;
		}

		class ClearContentEntry : ContextMenuEntry
		{
			Mobile m_From;
			BaseBook m_Book;
			public ClearContentEntry(Mobile from, BaseBook book) : base(6164, 12)
			{
				m_From = from;
				m_Book = book;
			}

			public override void OnClick()
			{
				if (m_Book.CheckClearContent(m_From))
					m_From.SendGump(new BookClearContentGump(m_From as PlayerMobile, m_Book));
			}
		}

		class ChangeLanguageEntry : ContextMenuEntry
		{
			Mobile m_From;
			BaseBook m_Book;
			public ChangeLanguageEntry(Mobile from, BaseBook book) : base(6165, 12)
			{
				m_From = from;
				m_Book = book;
			}

			public override void OnClick()
			{
				if (m_Book.CheckChangeLanguage(m_From))
					m_From.SendGump(new BookSpeechGump(m_From as PlayerMobile, m_Book));
			}
		}
	}

	class BaseBookExt : NExtension<BaseBookExtInfo>
	{
		private static string ModuleName = "BaseBook";

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
			foreach (KeyValuePair<Serial, BaseBookExtInfo> kvp in m_ExtensionInfo)
			{
				if (World.FindItem(kvp.Key) == null)
					toRemove.Add(kvp.Key);
			}
			foreach (Serial serial in toRemove)
			{
				BaseBookExtInfo removed;
				m_ExtensionInfo.TryRemove(serial, out removed);
			}
		}
	}

	class BaseBookExtInfo : NExtensionInfo
	{
		private SpeechLang m_Language;

		public SpeechLang Language { get { return m_Language; } set { m_Language = value; } }

		public BaseBookExtInfo()
		{
			m_Language = SpeechLang.Powszechny;
		}

		public override void Serialize(GenericWriter writer)
		{
			writer.Write((int)1); // version

			writer.Write((int)m_Language);
		}

		public override void Deserialize(GenericReader reader)
		{
			int version = reader.ReadInt();

			m_Language = (SpeechLang) reader.ReadInt();
		}
	}
}

