using System;
using Server;


namespace Server.Misc
{
    public class RaceDefinitions
    {
        public static void Configure()
        {
            /* Here we configure all races. Some notes:
             * 
             * 1) The first 32 races are reserved for core use.
             * 2) Race 0x7F is reserved for core use.
             * 3) Race 0xFF is reserved for core use.
             * 4) Changing or removing any predefined races may cause server instability.
             */

            RegisterRace(None.Init(0, 0));

            RegisterRace(Tamael.Init(0x21, 0x21));
            RegisterRace(Jarling.Init( 0x22, 0x22 ) );
            RegisterRace(Naur.Init( 0x23, 0x23 ) );
            RegisterRace(Elf.Init( 0x24, 0x24 ) );
            RegisterRace(Drow.Init( 0x25, 0x25 ) );
            RegisterRace(Krasnolud.Init( 0x26, 0x26 ) ); 
        }

        public static void RegisterRace( Race race )
        {
            Race.Races[race.RaceIndex] = race;
            Race.AllRaces.Add( race );
        }
    }
}