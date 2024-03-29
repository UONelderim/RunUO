using System; 
using System.IO; 
using System.Collections;
using System.Collections.Generic;

namespace Nelderim.Speech
{ 
	public class LanguagesDictionary
	{
		public static Dictionary<String, String> Jarlowy = new Dictionary<String, String>();
		public static Dictionary<String, String> Krasnoludzki = new Dictionary<String, String>();
		public static Dictionary<String, String> Elficki = new Dictionary<String, String>();
		public static Dictionary<String, String> Drowi = new Dictionary<String, String>();
		public static Dictionary<String, String> Orkowy = new Dictionary<String, String>();
		public static List<String> Demoniczny = new List<string>();
		public static Dictionary<String, String> Nieumarlych = new Dictionary<String, String>();
        public static List<String> Belkot = new List<string>();

		public static void Initialize()
		{
			FillDictionary("Jarlowy", Jarlowy);
			FillDictionary("Krasnoludzki", Krasnoludzki);
			FillDictionary("Elficki", Elficki);
			FillDictionary("Drowi", Drowi);
			FillDictionary("Orkowy", Orkowy);
			FillList("Demoniczny", Demoniczny);
            FillDictionary("Nieumarlych", Nieumarlych);
			FillList("Belkot", Belkot);
		}

		private static void FillDictionary(String filename, Dictionary<String, String> dict) {
			ArrayList list = new ArrayList();
			try {
				using (StreamReader sr = new StreamReader(Path.Combine("Data","Languages", filename + ".txt"))) {
					String line;
					while ((line = sr.ReadLine()) != null) {
						if(line.Trim().Length != 0)
							list.Add(line);
					}
				}
			} catch (Exception e) {
				Console.WriteLine("The file could not be read:");
				Console.WriteLine(e.Message);
			}

			foreach (String line in list) {
				String[] parts = line.Split('=');
				if (parts.Length != 2) continue;
				dict[parts[0].ToLower()] = parts[1].ToLower();
			}
		}

		private static void FillList(String filename, List<String> list) {
			try {
				using (StreamReader sr = new StreamReader(Path.Combine("Data","Languages", filename + ".txt"))) {
					String line;
					while ((line = sr.ReadLine()) != null) {
						if(line.Trim().Length != 0)
							list.Add(line.ToLower());
					}
				}
			} catch (Exception e) {
				Console.WriteLine("The file could not be read:");
				Console.WriteLine(e.Message);
			}
		}
	}
}
