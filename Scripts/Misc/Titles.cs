using System;
using System.Text;
using Server;
using Server.Mobiles;
using Server.Engines.CannedEvil;
using Nelderim.Towns;
using Server.Items;
using Server.Spells.Fifth;

namespace Server.Misc
{
	public class Titles
	{
		public const int MinFame = 0;
		public const int MaxFame = 15000;

		public static void AwardFame( Mobile m, int offset, bool message )
		{
			if ( offset > 0 )
			{
				if ( m.Fame >= MaxFame )
					return;

				offset -= m.Fame / 100;

				if ( offset < 0 )
					offset = 0;
			}
			else if ( offset < 0 )
			{
				if ( m.Fame <= MinFame )
					return;

				offset -= m.Fame / 100;

				if ( offset > 0 )
					offset = 0;
			}

			if ( (m.Fame + offset) > MaxFame )
				offset = MaxFame - m.Fame;
			else if ( (m.Fame + offset) < MinFame )
				offset = MinFame - m.Fame;

			m.Fame += offset;

			if ( message )
			{
				if ( offset > 40 )
					m.SendLocalizedMessage( 1019054 ); // You have gained a lot of fame.
				else if ( offset > 20 )
					m.SendLocalizedMessage( 1019053 ); // You have gained a good amount of fame.
				else if ( offset > 10 )
					m.SendLocalizedMessage( 1019052 ); // You have gained some fame.
				else if ( offset > 0 )
					m.SendLocalizedMessage( 1019051 ); // You have gained a little fame.
				else if ( offset < -40 )
					m.SendLocalizedMessage( 1019058 ); // You have lost a lot of fame.
				else if ( offset < -20 )
					m.SendLocalizedMessage( 1019057 ); // You have lost a good amount of fame.
				else if ( offset < -10 )
					m.SendLocalizedMessage( 1019056 ); // You have lost some fame.
				else if ( offset < 0 )
					m.SendLocalizedMessage( 1019055 ); // You have lost a little fame.
			}
		}

		public const int MinKarma = -15000;
		public const int MaxKarma =  15000;

		public static void AwardKarma( Mobile m, int offset, bool message )
		{
			if ( offset > 0 )
			{
				if ( m is PlayerMobile && ((PlayerMobile)m).KarmaLocked )
					return;

				if ( m.Karma >= MaxKarma )
					return;

				offset -= m.Karma / 100;

				if ( offset < 0 )
					offset = 0;
			}
			else if ( offset < 0 )
			{
				if ( m.Karma <= MinKarma )
					return;

				offset -= m.Karma / 100;

				if ( offset > 0 )
					offset = 0;
			}

			if ( (m.Karma + offset) > MaxKarma )
				offset = MaxKarma - m.Karma;
			else if ( (m.Karma + offset) < MinKarma )
				offset = MinKarma - m.Karma;

			bool wasPositiveKarma = ( m.Karma >= 0 );

			m.Karma += offset;

			if ( message )
			{
				if ( offset > 40 )
					m.SendLocalizedMessage( 1019062 ); // You have gained a lot of karma.
				else if ( offset > 20 )
					m.SendLocalizedMessage( 1019061 ); // You have gained a good amount of karma.
				else if ( offset > 10 )
					m.SendLocalizedMessage( 1019060 ); // You have gained some karma.
				else if ( offset > 0 )
					m.SendLocalizedMessage( 1019059 ); // You have gained a little karma.
				else if ( offset < -40 )
					m.SendLocalizedMessage( 1019066 ); // You have lost a lot of karma.
				else if ( offset < -20 )
					m.SendLocalizedMessage( 1019065 ); // You have lost a good amount of karma.
				else if ( offset < -10 )
					m.SendLocalizedMessage( 1019064 ); // You have lost some karma.
				else if ( offset < 0 )
					m.SendLocalizedMessage( 1019063 ); // You have lost a little karma.
			}

			if ( !Core.AOS && wasPositiveKarma && m.Karma < 0 && m is PlayerMobile && !((PlayerMobile)m).KarmaLocked )
			{
				((PlayerMobile)m).KarmaLocked = true;
				m.SendLocalizedMessage( 1042511, "", 0x22 ); // Karma is locked.  A mantra spoken at a shrine will unlock it again.
			}
		}

		public static string[] HarrowerTitles = new string[] { "Pogromca", "Przeciwnik", "Mysliwy", "Jad", "Kat", "Anihilator", "Czempion" };

		public static string ComputeTitle( Mobile beholder, Mobile beheld )
		{
			StringBuilder title = new StringBuilder();

			int fame = beheld.Fame;
			int karma = beheld.Karma;

            // 21.06.2012 :: zombie :: wylaczenie pokazywania tytulu z umiejetnosci
            bool showSkillTitle = false; // beheld.ShowFameTitle && ((beholder == beheld) || (fame >= 5000));
            // zombie

			/*if ( beheld.Kills >= 5 )
			{
				title.AppendFormat( beheld.Fame >= 10000 ? "The Murderer {1} {0}" : "The Murderer {0}", beheld.Name, beheld.Female ? "Lady" : "Lord" );
			}
			else*/
			/*if ( beheld.ShowFameTitle || (beholder == beheld) )
			{
				for ( int i = 0; i < m_FameEntries.Length; ++i )
				{
					FameEntry fe = m_FameEntries[i];

					if ( fame <= fe.Fame || i == (m_FameEntries.Length - 1) ) 
					{
						KarmaEntry[] karmaEntries = fe.Karma;

						for ( int j = 0; j < karmaEntries.Length; ++j )
						{
							KarmaEntry ke = karmaEntries[j];

							if ( karma <= ke.Karma || j == (karmaEntries.Length - 1) )
							{
 								title.AppendFormat( beheld.Female ? ke.FemaleTitle : ke.MaleTitle, beheld.Name, beheld.Female ? "Lady" : "Lord" );
								break;
							}
						}

						break;
					}
				}
			}
			else*/
			{
				title.Append( beheld.Name );
			}

			if( beheld is PlayerMobile && ((PlayerMobile)beheld).DisplayChampionTitle )
			{
				PlayerMobile.ChampionTitleInfo info = ((PlayerMobile)beheld).ChampionTitles;

				if( info.Harrower > 0 )
					title.AppendFormat( ": {0} of Evil", HarrowerTitles[Math.Min( HarrowerTitles.Length, info.Harrower )-1] );
				else
				{
					int highestValue = 0;
					ChampionSpawnType highestType = ChampionSpawnType.Abyss;
					foreach (var champType in ChampionSpawnInfo.Table.Keys)
					{
						int v = info.GetValue( champType );

						if( v > highestValue )
						{
							highestValue = v;
							highestType = champType;
						}
					}

					int offset = 0;
					if( highestValue > 800 )
						offset = 3;
					else if( highestValue > 300 )
						offset = (highestValue/300);

					if( offset > 0 )
					{
						ChampionSpawnInfo champInfo = ChampionSpawnInfo.GetInfo( highestType );
						title.AppendFormat( ": {0} {1}", champInfo.LevelNames[Math.Min( offset, champInfo.LevelNames.Length ) -1], champInfo.Name );
					}
				}
			}

            string customTitle = beheld.Title;

            // 21.06.2012 :: zombie :: dodanie rasy do paperdolla
            if ((beheld.BodyValue == 400 || beheld.BodyValue == 401) && beheld.Race != None.Instance)
            {
                if (beheld is PlayerMobile && ((PlayerMobile)beheld).RaceMod != null && ((PlayerMobile)beheld).RaceMod != None.Instance)
                    customTitle += String.Format(" [{0}] ", ((PlayerMobile)beheld).RaceMod);
                else
                    customTitle += String.Format(" [{0}] ", beheld.Race);
            }
            // zombie

            // Dodanie miasta do paperdola
            bool anonymous = RaceDisguiseGump.IsDisguised(beheld) || DisguiseGump.IsDisguised(beheld) || !beheld.CanBeginAction(typeof(IncognitoSpell));
            if (beheld is PlayerMobile && TownDatabase.GetCitizenCurrentCity(beheld) != Towns.None && !anonymous)
            {
                customTitle = string.Format("{0}[{1}]", customTitle, TownDatabase.GetCitizenCurrentCity(beheld).ToString());
            }

			if ( customTitle != null && (customTitle = customTitle.Trim()).Length > 0 )
			{
				title.AppendFormat( " {0}", customTitle );
			}
			else if ( showSkillTitle && beheld.Player )
			{
				string skillTitle = GetSkillTitle( beheld );

				if ( skillTitle != null ) {
					title.Append( ", " ).Append( skillTitle );
				}
			}

			return title.ToString();
		}

		public static string GetSkillTitle( Mobile mob ) {
			Skill highest = GetHighestSkill( mob );// beheld.Skills.Highest;

			if ( highest != null && highest.BaseFixedPoint >= 300 )
			{
				string skillLevel = GetSkillLevel( highest );
				string skillTitle = highest.Info.Title;

				if ( mob.Female && skillTitle.EndsWith( "man" ) )
					skillTitle = skillTitle.Substring( 0, skillTitle.Length - 3 ) + "woman";

				return String.Concat( skillLevel, " ", skillTitle );
			}

			return null;
		}

		private static Skill GetHighestSkill( Mobile m )
		{
			Skills skills = m.Skills;

			if ( !Core.AOS )
				return skills.Highest;

			Skill highest = null;

			for ( int i = 0; i < m.Skills.Length; ++i )
			{
				Skill check = m.Skills[i];

				if ( highest == null || check.BaseFixedPoint > highest.BaseFixedPoint )
					highest = check;
				else if ( highest != null && highest.Lock != SkillLock.Up && check.Lock == SkillLock.Up && check.BaseFixedPoint == highest.BaseFixedPoint )
					highest = check;
			}

			return highest;
		}

		private static string[,] m_Levels = new string[,]
			{
				{ "Neophyte",		"Neophyte",		"Neophyte"		},
				{ "Novice",			"Novice",		"Novice"		},
				{ "Apprentice",		"Apprentice",	"Apprentice"	},
				{ "Journeyman",		"Journeyman",	"Journeyman"	},
				{ "Expert",			"Expert",		"Expert"		},
				{ "Adept",			"Adept",		"Adept"			},
				{ "Master",			"Master",		"Master"		},
				{ "Grandmaster",	"Grandmaster",	"Grandmaster"	},
				{ "Elder",			"Tatsujin",		"Shinobi"		},
				{ "Legendary",		"Kengo",		"Ka-ge"			}
			};

		private static string GetSkillLevel( Skill skill )
		{
			return m_Levels[GetTableIndex( skill ), GetTableType( skill )];
		}

		private static int GetTableType( Skill skill )
		{
			switch ( skill.SkillName )
			{
				default: return 0;
				case SkillName.Bushido: return 1;
				case SkillName.Ninjitsu: return 2;
			}
		}

		private static int GetTableIndex( Skill skill )
		{
			int fp = Math.Min( skill.BaseFixedPoint, 1200 );

			return (fp - 300) / 100;
		}

        // 21.06.2012 :: zombie
        private static FameEntry[] m_FameEntries = new FameEntry[]
			{
				new FameEntry( 1249, new KarmaEntry[]
				{
					new KarmaEntry( -10000, "Wygnany {0}", "Wygnana {0}" ),
					new KarmaEntry( -5000, "Lotrowski {0}", "Lotrowska {0}" ),
					new KarmaEntry( -2500, "Lajdacki {0}", "Lajdacka {0}" ),
					new KarmaEntry( -1250, "Nieprzyjemny {0}", "Nieprzyjemna {0}"),
					new KarmaEntry( -625, "Porywczy {0}", "Porywcza {0}" ),
					new KarmaEntry( 624, "{0}", "{0}" ),
					new KarmaEntry( 1249, "Porzadny {0}", "Porzadna {0}" ),
					new KarmaEntry( 2499, "Uprzejmy {0}", "Uprzejma {0}" ),
					new KarmaEntry( 4999, "Dobry {0}", "Dobra {0}" ),
					new KarmaEntry( 9999, "Uczciwy {0}", "Uczciwa {0}" ),
					new KarmaEntry( 10000, "Godny zaufania {0}", "Godna zaufania {0}" )
				} ),
				new FameEntry( 2499, new KarmaEntry[]
				{
					new KarmaEntry( -10000, "Odrazajacy {0}", "Odrazajaca {0}" ),
					new KarmaEntry( -5000, "Podly {0}", "Podla {0}" ),
					new KarmaEntry( -2500, "Wredny {0}", "Wredna {0}" ),
					new KarmaEntry( -1250, "Niehonorowy {0}", "Niehonorowa {0}" ),
					new KarmaEntry( -625, "Niecny {0}", "Niecna {0}" ),
					new KarmaEntry( 624, "Wyrozniajacy sie {0}", "Wyrozniajaca sie {0}" ),
					new KarmaEntry( 1249, "Uczciwy {0}", "Uczciwa {0}" ),
					new KarmaEntry( 2499, "Przyzwoity {0}", "Przyzwoita {0}" ),
					new KarmaEntry( 4999, "Honorowy {0}", "Honorowa {0}" ),
					new KarmaEntry( 9999, "Powazany {0}", "Powazana {0}" ),
					new KarmaEntry( 10000, "Zacny {0}", "Zacna {0}" )
				} ),
				new FameEntry( 4999, new KarmaEntry[]
				{
					new KarmaEntry( -10000, "Niegodziwy {0}", "Niegodziwa {0}"  ),
					new KarmaEntry( -5000, "Niecny {0}", "Niecna {0}" ),
					new KarmaEntry( -2500, "Okrutny {0}", "Okrutna {0}" ),
					new KarmaEntry( -1250, "Haniebny {0}", "Haniebna {0}" ),
					new KarmaEntry( -625, "Godny pogardy {0}", "Godna pogardy {0}" ),
					new KarmaEntry( 624, "Glosny {0}", "Glosna {0}" ),
					new KarmaEntry( 1249, "Szanowany {0}", "Szanowana {0}" ),
					new KarmaEntry( 2499, "Godny szacunku {0}", "Godna szacunku {0}" ),
					new KarmaEntry( 4999, "Godny podziwu {0}", "Godna podziwu {0}" ),
					new KarmaEntry( 9999, "Znany {0}", "Znana {0}" ),
					new KarmaEntry( 10000, "Wielki {0}", "Wielka {0}" )
				} ),
				new FameEntry( 9999, new KarmaEntry[]
				{
					new KarmaEntry( -10000, "Przerazajacy {0}", "Przerazajaca {0}" ),
					new KarmaEntry( -5000, "Zly {0}", "Zla {0}" ),
					new KarmaEntry( -2500, "Nikczemny {0}", "Nikczemna {0}" ),
					new KarmaEntry( -1250, "Zlowrogi {0}", "Zlowroga {0}" ),
					new KarmaEntry( -625, "Nieslawny {0}", "Nieslawna {0}" ),
					new KarmaEntry( 624, "Slawny {0}", "Slawna {0}" ),
					new KarmaEntry( 1249, "Znamienity {0}", "Znamienita {0}" ),
					new KarmaEntry( 2499, "Oslawiony {0}", "Oslawiona {0}" ),
					new KarmaEntry( 4999, "Szlachetny {0}", "Szlachetna {0}" ),
					new KarmaEntry( 9999, "Wybitny {0}", "Wybitna {0}" ),
					new KarmaEntry( 10000, "Wspanialy {0}", "Wspaniala {0}" )
				} ),
				new FameEntry( 10000, new KarmaEntry[]
				{
					new KarmaEntry( -10000, "Przerazajacy {1} {0}", "Przerazajaca {1} {0}" ),
					new KarmaEntry( -5000, "Zly {1} {0}", "Zla {1} {0}" ),
					new KarmaEntry( -2500, "Mroczny {1} {0}", "Mroczna {1} {0}" ),
					new KarmaEntry( -1250, "Zlowrogi {1} {0}", "Zlowroga {1} {0}" ),
					new KarmaEntry( -625, "Pozbawiony honoru {1} {0}", "Pozbawiona honoru {1} {0}" ),
					new KarmaEntry( 624, "{1} {0}", "{1} {0}" ),
					new KarmaEntry( 1249, "Znamienity {1} {0}", "Znamienita {1} {0}" ),
					new KarmaEntry( 2499, "Oslawiony {1} {0}", "Oslawiona {1} {0}" ),
					new KarmaEntry( 4999, "Szlachetny {1} {0}", "Szlachetna {1} {0}" ),
					new KarmaEntry( 9999, "Wybitny {1} {0}", "Wybitna {1} {0}" ),
					new KarmaEntry( 10000, "Wspanialy {1} {0}", "Wspaniala {1} {0}" )
				} )
			};
    }
    
	public class FameEntry
	{
        private int m_Fame;
        private KarmaEntry[] m_Karma;

		public FameEntry( int fame, KarmaEntry[] karma )
		{
			m_Fame = fame;
			m_Karma = karma;
		}

        public int Fame { get { return m_Fame; } }
        public KarmaEntry[] Karma{ get { return m_Karma; } }
	}

	public class KarmaEntry
	{
        private int m_Karma;
		private string m_MaleTitle;
        private string m_FemaleTitle;

        public string FemaleTitle { get { return m_FemaleTitle; } }
        public string MaleTitle { get { return m_MaleTitle; } }
        public int Karma { get { return m_Karma; } }

		public KarmaEntry( int karma, string maleTitle, string femaleTitle )
		{
			m_Karma = karma;
			m_MaleTitle = maleTitle;
            m_FemaleTitle = femaleTitle;
		}
	}
    // zombie
}