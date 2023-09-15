using System;

namespace Server
{
	public class Start
	{
		public static void Initialize()
		{
			Console.WriteLine("");
			Console.WriteLine("Aktualna wersja oskryptowania SVN: Distro {0}, Core: {1}, Scripts: {2}",
				Config.SVN_Distro, Config.SVN_Core, Config.SVN_Scripts);
			Console.WriteLine("");
		}
	}
}
