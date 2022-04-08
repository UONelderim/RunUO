using System;
using System.IO;
using Nelderim.CharacterSheet;
using Server.Accounting;
using Server.Commands;
using Server.Mobiles;

namespace Server
{
	public class Start
	{
		public static void Initialize()
		{
			Console.WriteLine( "" );
			Console.WriteLine( "Aktualna wersja oskryptowania SVN: Distro {0}, Core: {1}, Scripts: {2}", Config.SVN_Distro, Config.SVN_Core, Config.SVN_Scripts );
			Console.WriteLine( "" );
			CommandSystem.Register( "qph", AccessLevel.Player, new CommandEventHandler( Status_OnCommand ) ); 
		}

		private static void Status_OnCommand(CommandEventArgs e)
		{
			string filePath = "qph.csv";
			try
			{
				if (!File.Exists(filePath))
				{
					Console.WriteLine("Error: " + filePath + " does not exist");
					return;
				}

				StreamReader sr = new StreamReader(filePath);
				while(sr.Peek() >= 0)
				{
					string line = sr.ReadLine();
					if (line == null) break;
					var strings = line.Split(',');
					DateTime dt = DateTime.Parse(strings[0]);
					string gm = strings[1];
					string acc = strings[2];
					string chr = strings[3];
					int points = Int32.Parse(strings[4]);
					string reason = strings[5];
					QuestPointsHistoryEntry qphe = new QuestPointsHistoryEntry(dt, gm, points, reason);
					
					IAccount iacc = Accounts.GetAccount(acc);
					if (iacc == null)
					{

						Console.WriteLine(dt + "Nie znaleziono konta " + acc);
						continue;
					}

					Mobile mob = null;
					for (var i = 0; i < iacc.Count; i++)
					{
						Mobile m = iacc[i];
						if (m != null && m.Name == chr)
						{
							mob = m;
							break;
						}
					}

					if (mob == null)
					{
						Console.WriteLine(dt + "Nie znaleziono postaci " + chr + " dla konta " + acc);
					}
					else if(mob is PlayerMobile)
					{
						PlayerMobile pm = (PlayerMobile)mob;
						pm.QuestPointsHistory.Add(qphe);
					}
				}
			}
			catch (Exception ex)
			{
				// ignored
			}
		}
	}
}
