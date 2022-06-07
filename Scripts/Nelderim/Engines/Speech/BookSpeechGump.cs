using System;
using System.Collections.Generic;
using Server;
using Server.Commands;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;

namespace Nelderim.Speech
{
    public class BookSpeechGump : Gump
    {
        private BaseBook m_Book;
        public BookSpeechGump(PlayerMobile from, BaseBook book) : base(0, 0) {
            this.Closable = true;
            this.Disposable = true;
            this.Dragable = true;
            this.Resizable = false;

            m_Book = book;

            AddPage(0);

            List<SpeechLang> languages = from.LanguagesKnown.List;
            AddBackground(0, 0, 200, 50 + languages.Count * 30, 9260);
            AddLabel(17, 17, 0, @"Wybierz język księgi");
            int y = 40;
            foreach (SpeechLang lang in languages) {
                AddButton(20, y, book.Language == lang ? 4006 : 4005, 4007, (int)lang + 1, GumpButtonType.Reply, 1);
                AddLabel(60, y, 0, Enum.GetName(typeof(SpeechLang), lang));
                y += 30;
            }
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {

			if (info.ButtonID == 0)
				return;

            PlayerMobile pm = sender.Mobile as PlayerMobile;
            if (pm == null)
                return;

            if (!m_Book.CheckChangeLanguage(pm))
            {
                return;
            }

			SpeechLang choosenLang = (SpeechLang)info.ButtonID - 1;

			if (!pm.LanguagesKnown.Get(choosenLang))
			{
				pm.SendMessage("Nie znasz tego języka...");
				return;
			}

			m_Book.Language = choosenLang;
		}

    }

    public class BookClearContentGump : Gump
    {
		private BaseBook m_Book;
		public BookClearContentGump(PlayerMobile from, BaseBook book) : base(0, 0)
		{
			this.Closable = true;
			this.Disposable = true;
			this.Dragable = true;
			this.Resizable = false;

			m_Book = book;

			AddPage(0);

			List<SpeechLang> languages = from.LanguagesKnown.List;
			AddBackground(0, 0, 200, 50 + 45, 9260);
			AddLabel(17, 17, 0, @"Wymazać zawartość księgi?");

			AddButton(30, 50, 4005, 4007, 1, GumpButtonType.Reply, 1);
			AddLabel(60, 50, 0, @"Tak");

			AddButton(120, 50, 4005, 4007, 2, GumpButtonType.Reply, 1);
			AddLabel(150, 50, 0, @"Nie");
		}

		public override void OnResponse(NetState sender, RelayInfo info)
		{
			if (info.ButtonID != 1)
				return;

			Mobile m = sender.Mobile;

			if (!m_Book.CheckClearContent(m))
			{
				return;
			}

			m_Book.ClearContent();
			m.SendMessage("Wyczysciłeś zawartość księgi.");
		}
	}
}
