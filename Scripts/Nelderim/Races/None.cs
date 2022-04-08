using System;
using System.Collections.Generic;

namespace Server
{
    public class None : Race
    {
        private static Race instance = null;
        private None( int raceID, int raceIndex ) : base( raceID, raceIndex, 400, 401, 402, 403, Expansion.None )
        {
            Name = "None"; //To iterate through races easily, use Names for displaying
        }
        public static Race Instance {
            get {
                return instance;
            }
        }

        public static Race Init(int raceID, int raceIndex) {
            if (instance == null) {
                instance = new None(raceID, raceIndex);
            }
            return instance;
        }

        protected override string[] Names
        {
            get { return new string[7] { "None", "Czlowieka", "Czlowiekowi", "Czlowieka", "Czlowiekiem", "Czlowieku", "Czlowieku" }; }
        }

        protected override string[] PluralNames
        {
            get { return new string[7] { "Ludzie", "Ludzi", "Ludziom", "Ludzi", "Ludzmi", "Ludziach", "Ludzie" }; }
        }

        public override int[] SkinHues
        {
            get { return new int[] { 1037, 1038, 1039, 1040, 1041, 1042, 1043, 2101, 2102, 2103, 2104, 2307, 2308, 2309, 2310, 2311 }; }
        }

        public override int[] HairHues
        {
            get { return new int[] { 1045, 1046, 1047, 1048, 1049, 1050, 1051, 1052, 1053, 1054, 1055, 1056, 
                                     1057, 1058, 1110, 1112, 1113, 1114, 1115, 1116, 1117, 1118, 1119, 1120, 
                                     1121, 1122, 1123, 1124, 1125, 1126, 1127, 1128, 1129, 1130, 1131, 1132, 
                                     1133, 1134, 1135, 1136, 1137, 1138, 1139, 1140, 1141, 1142, 1143, 1144, 
                                     1145, 1146, 1147, 1148, 1149 }; }
        }

        public override FacialHairItemID[] FacialHairStyles
        {
            get
            {
                return new FacialHairItemID[]
                {
                    FacialHairItemID.None,              FacialHairItemID.LongBeard, 
                    FacialHairItemID.ShortBeard,        FacialHairItemID.Goatee,    
                    FacialHairItemID.Mustache,          FacialHairItemID.MediumShortBeard,
                    FacialHairItemID.MediumLongBeard,   FacialHairItemID.Vandyke,
                };
            }
        }

        public override HairItemID[] MaleHairStyles
        {
            get
            {
                return new HairItemID[]
                {
                    HairItemID.None,        HairItemID.Short,       HairItemID.Long,
                    HairItemID.Pageboy,
                    HairItemID.Receeding,
                };
            }
        }

        public override HairItemID[] FemaleHairStyles
        {
            get
            {
                return new HairItemID[]
                {
                    HairItemID.Short,       HairItemID.Long,
                    HairItemID.PonyTail,    HairItemID.Pageboy,
                    HairItemID.Buns,        HairItemID.TwoPigTails,
                };
            }
        }
    }
}
