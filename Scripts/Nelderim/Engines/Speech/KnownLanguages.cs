using Server;
using System;
using System.Collections.Generic;

namespace Nelderim.Speech
{
	[Flags]
	public enum SpeechLang
	{
		Powszechny = 0x00,
		Krasnoludzki = 0x01,
		Elficki = 0x02,
		Drowi = 0x04,
		Jarlowy = 0x08,
		Demoniczny = 0x10,
		Orkowy = 0x20,
		Nieumarlych = 0x40,
		Belkot = 0x80
	}

	[PropertyObject]
	public class KnownLanguages
	{
		private SpeechLang m_Languages;

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Krasnoludzki {
			get { return Get(SpeechLang.Krasnoludzki); }
			set { Set(SpeechLang.Krasnoludzki, value); }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Elficki {
			get { return Get(SpeechLang.Elficki); }
			set { Set(SpeechLang.Elficki, value); }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Drowi {
			get { return Get(SpeechLang.Drowi); }
			set { Set(SpeechLang.Drowi, value); }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Jarlowy {
			get { return Get(SpeechLang.Jarlowy); }
			set { Set(SpeechLang.Jarlowy, value); }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Demoniczny {
			get { return Get(SpeechLang.Demoniczny); }
			set { Set(SpeechLang.Demoniczny, value); }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Orkowy {
			get { return Get(SpeechLang.Orkowy); }
			set { Set(SpeechLang.Orkowy, value); }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Nieumarlych {
			get { return Get(SpeechLang.Nieumarlych); }
			set { Set(SpeechLang.Nieumarlych, value); }
		}

		public bool Get(SpeechLang lang) {
			return m_Languages.HasFlag(lang);
		}

		public void Set(SpeechLang lang, bool value) {
			if (value)
				m_Languages |= lang;
			else
				m_Languages &= ~lang;
		}

		public KnownLanguages() : this( SpeechLang.Powszechny )
		{ 
		}

		public KnownLanguages(SpeechLang languages) {
			m_Languages = languages;
		}

		public SpeechLang Value {
			get { return m_Languages; }
		}

		public List<SpeechLang> List {
			get {
				List<SpeechLang> result = new List<SpeechLang>();
				foreach (SpeechLang lang in Enum.GetValues(typeof(SpeechLang)))
					if (Get(lang)) 
						result.Add(lang);

				return result;
			}
		}
		public override string ToString() {
			return "...";
		}

		public static implicit operator KnownLanguages( SpeechLang languages )
		{
			return new KnownLanguages( languages );
		}
	}
}
