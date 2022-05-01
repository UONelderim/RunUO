using System;
using Server.Network;
using Server.Mobiles;
using Server;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace Nelderim.Speech
{
	public class Translate
	{
		public static void Initialize()
		{
			// Register our speech handler
			EventSink.Speech += new SpeechEventHandler( EventSink_Speech );
		}
		
		private static void EventSink_Speech( SpeechEventArgs args )
		{
			PlayerMobile from = args.Mobile as PlayerMobile;
			string mySpeech = args.Speech;
 			if (from == null || mySpeech == null || args.Type == MessageType.Emote || from.LanguageSpeaking == SpeechLang.Powszechny) {
				return;
			}
			
			args.Blocked = true;
			int tileLength = 15;
			
			switch ( args.Type )
			{
				case MessageType.Yell:
					tileLength = 18;
					break;
				case MessageType.Whisper:
					tileLength = 1;
					break;
			}

			foreach (Mobile m in from.Map.GetMobilesInRange(from.Location, tileLength))
			{
				String mySpeechTranslated = "";
				if (m.Player) {
					PlayerMobile pm = m as PlayerMobile;

					if (pm.LanguagesKnown.Get(from.LanguageSpeaking) || from == m) {
						from.SayTo(pm, String.Format("[{0}] ", from.LanguageSpeaking.ToString()) + mySpeech);
					} else {
						from.SayTo(pm, Translate.CommonToForeign(mySpeech, from.LanguageSpeaking));
					}
				} else {
					m.OnSpeech(args);
				}			
			}			
		}

		public static String CommonToForeign(String speech, SpeechLang lang)
		{
			if (lang == SpeechLang.Powszechny)
				return speech;

			switch (lang)
			{
				case SpeechLang.Krasnoludzki: return TranslateUsingDict(speech, LanguagesDictionary.Krasnoludzki);
				case SpeechLang.Elficki: return TranslateUsingDict(speech, LanguagesDictionary.Elficki);
				case SpeechLang.Drowi: return TranslateUsingDict(speech, LanguagesDictionary.Drowi);
				case SpeechLang.Jarlowy: return TranslateUsingDict(speech, LanguagesDictionary.Jarlowy);
				case SpeechLang.Demoniczny: return TranslateUsingWordsList(speech, LanguagesDictionary.Demoniczny);
				case SpeechLang.Orkowy: return TranslateUsingDict(speech, LanguagesDictionary.Orkowy);
				case SpeechLang.Nieumarlych: return TranslateUsingSentencesList(LanguagesDictionary.Nieumarlych);
				case SpeechLang.Belkot: return TranslateUsingWordsList(speech, LanguagesDictionary.Belkot);
			}
			return "";
		}

		private static Random random = new Random();
		public static String RandomWord(int length) {
			const string chars = "abcdefghijklmnopqrstuvwxyz";
			return new string(Enumerable.Repeat(chars, length)
			  .Select(s => s[random.Next(s.Length)]).ToArray());
		}

		public static String TranslateUsingDict(String speech, Dictionary<String, String> dict) {
			string translatedWord;
			StringBuilder sb = new StringBuilder(speech.Length);
			foreach (string word in speech.Split(' ')) {
				if (word.StartsWith("*")) {
					translatedWord = word.Substring(1);
				} else if (dict.ContainsKey(word.ToLower())) {
					translatedWord = dict[word.ToLower()];
				} else {
					translatedWord = dict.ElementAt(Math.Abs(word.GetHashCode()) % dict.Count).Value;
				}
				if (translatedWord.Length > 0 && word.Length > 0 && Char.IsUpper(word[0])) {
					char upperChar = Char.ToUpper(translatedWord[0]);
					sb.Append(upperChar);
					sb.Append(translatedWord.Substring(1));
				} else {
					sb.Append(translatedWord);
				}
				sb.Append(" ");
			}
			return sb.ToString(); ;
		}

		public static String TranslateUsingWordsList(String speech, List<String> list) {
			string translatedWord;
			StringBuilder sb = new StringBuilder(speech.Length);
			foreach (string word in speech.Split(' ')) {
				if (word.StartsWith("*")) {
					sb.Append(word);
				} else {
					translatedWord = list[Math.Abs(word.GetHashCode()) % list.Count];
					if (translatedWord.Length > 0 && word.Length > 0 && Char.IsUpper(word[0])) {
						char upperChar = Char.ToUpper(translatedWord[0]);
						sb.Append(upperChar);
						sb.Append(translatedWord.Substring(1));
					} else {
						sb.Append(translatedWord);
					}	
				}
				sb.Append(" ");
			}
			return sb.ToString(); ;
		}

		public static String TranslateUsingSentencesList(List<String> list) {
			return list[random.Next(list.Count)];

		}
	}
}

