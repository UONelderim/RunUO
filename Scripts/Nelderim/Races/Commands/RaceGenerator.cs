using System;
using Server;
using Server.Targeting;
using Server.Commands;
using Server.Items;
using Server.Mobiles;
using Server.Nelderim;

namespace Server
{
    public class RaceGenerator
    {
        public static void Initialize()
        {
            CommandSystem.Register( "Rasa", AccessLevel.GameMaster, new CommandEventHandler( Appearance_OnCommand ) );
        }

        [Usage( "Rasa {<Rasa: none - 0 | Tamael - 1 | Jarling - 2 | Krasnolud - 3 | Elf - 4 | Drow - 5 | Naur - 6>}" )]
        [Description( "Zmienia wyglad i rase NPCa na losowy zgodny z jego rasa. Jesli z parametrem, to ustawia rase i zmienia wyglad." )]

        private static void Appearance_OnCommand( CommandEventArgs e )
        {
            if ( e.Length == 0 )
            {
                e.Mobile.SendMessage( "Poprawne uzycie: <[Rasa none - 0 | Tamael - 1 | Jarling - 2 | Krasnolud - 3 | Elf - 4 | Drow - 5 | Naur - 6>" );
            }
            else
            {
                int par = e.GetInt32( 0 );

                switch ( par )
                {
                    case 0: e.Mobile.Target = new AppearanceTarget( None.Instance); break;
                    case 1: e.Mobile.Target = new AppearanceTarget( Tamael.Instance); break;
                    case 2: e.Mobile.Target = new AppearanceTarget( Jarling.Instance); break;
                    case 3: e.Mobile.Target = new AppearanceTarget( Krasnolud.Instance); break;
					
                    case 4: e.Mobile.Target = new AppearanceTarget( Elf.Instance); break;
                    case 5: e.Mobile.Target = new AppearanceTarget( Drow.Instance); break;
                    case 6: e.Mobile.Target = new AppearanceTarget( Naur.Instance); break;					
                    default: e.Mobile.SendMessage( "Niepoprawny parametr" ); break;
                }
            }
        }

        private class AppearanceTarget : Target
        {
            private Race m_Race;

            public AppearanceTarget( Race race ) : base( -1, false, TargetFlags.None )
            {
                m_Race = race;
            }

            protected override void OnTarget( Mobile from, object targeted )
            {
                if ( targeted is Mobile )
                {
                    Mobile targ = (Mobile)targeted;

                    if ( targ.BodyValue == 400 || targ.BodyValue == 401 )
                    {
                        targ.Race = m_Race;
                        m_Race.MakeRandomAppearance( targ );

                        from.SendMessage( "Ustawiono rase: {0}", m_Race );
                    }
                    else
                        from.SendMessage( "Wyglada, ze cel nie jest humanoidem!" );
                }
            }
        }

        public static void Init( Mobile m )
        {
            try
            {
                if ( !m.Deleted )
                {
                    m.Female = NelderimRegionSystem.GetRegion(m.Region.Name).FemaleChance() > Utility.RandomDouble();
                    if(m.Race == None.Instance)
                        m.Race = NelderimRegionSystem.GetRegion( m.Region.Name ).RandomRace();
                    if(string.IsNullOrEmpty(m.Name))
                        m.Name = NameList.RandomName( m.Race, m.Female );
                }
            }
            catch ( Exception e )
            {
                Console.WriteLine( "RaceGenerator.Init error: " + e );
            }
        }
    }
}
