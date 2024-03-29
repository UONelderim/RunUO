using System;
using System.Collections.Generic;
using Server.Items;

namespace Server
{
    public class Naur : Race
    {
        private static Race instance = null;
        public Naur( int raceID, int raceIndex ) : base( raceID, raceIndex, 400, 401, 402, 403, Expansion.None )
        {
        }

        public static Race Instance {
            get {
                return instance;
            }
        }

        public static Race Init(int raceID, int raceIndex) {
            if (instance == null) {
                instance = new Naur(raceID, raceIndex);
            }
            return instance;
        }

        protected override string[] Names 
        { 
            get { return new string[7] { "Naur", "Naura", "Naurowi", "Naura", "Naurem", "Naurze", "Naur" }; } 
        } 

        protected override string[] PluralNames 
        { 
            get { return new string[7] { "Naurowie", "Naurow", "Naurom", "Naurow", "Naurami", "Naurach", "Naurowie" }; } 
        } 
 
        public override int DescNumber { get { return 505817; } }

        public override int[] SkinHues
        {
            get { return new int[] { 1146, 1147, 1148, 1149
			}; }
        }
 
        public override int[] HairHues
        { 
            get { return new int[] { 1102, 1103, 1104, 1105, 1106, 1107, 1108, 1109, 1007, 1008, 1014, 1015,
									 1021, 1022, 1028, 1029, 1035, 1036, 1043, 1044, 1049, 1050, 1051, 1057,
									 1058
			}; }
        }
 
        public override int[] FacialHairStyles
        {
            get
            {
                return new int[]
                {
                    Beard.Human.Clean,
                    Beard.Human.Goatee,
                    Beard.Human.Vandyke                    
                };
            }
        }
 
        public override int[] MaleHairStyles
        {
            get
            {
                return new int[]
                {
                    Hair.Human.Bald,
                    Hair.Human.Short,
                    Hair.Human.PonyTail,
                    Hair.Human.Mohawk,
                    Hair.Human.Afro,
                    Hair.Human.Receeding,

                };
            }
        }
 
        public override int[] FemaleHairStyles
        {
            get
            {
                return new int[]
                {
                    Hair.Human.Short,
                    Hair.Human.PonyTail,
                    Hair.Human.Mohawk,
                    Hair.Human.Afro,
                    Hair.Human.Receeding,
                };
            }
        }
    }
}