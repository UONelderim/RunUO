using System;
using System.Collections.Generic;
 
namespace Server
{
    public class Tamael : Race
    {
        private static Race instance = null;
        private Tamael(int raceID, int raceIndex) : base(raceID, raceIndex, 400, 401, 402, 403, Expansion.None )
        {
        }

        public static Race Instance {
            get {
                return instance;
            }
        }

        public static Race Init(int raceID, int raceIndex) {
            if(instance == null) {
                instance = new Tamael(raceID, raceIndex);
            }
            return instance;
        }

 
        protected override string[] Names 
        { 
            get { return new string[7] { "Tamael", "Tamaela", "Tamaelowi", "Tamaela", "Tamaelem", "Tamaelu", "Tamaelu" }; } 
        } 

        protected override string[] PluralNames 
        { 
            get { return new string[7] { "Tamaelowie", "Tamaelow", "Tamaelom", "Tamaelow", "Tamaelami", "Tamaelach", "Tamaelowie" }; } 
        }

        public override int DescNumber { get { return 505817; } }

        public override int[] SkinHues
        {
            get { return new int[] { 1023, 1026, 1027, 1030, 1031, 1033, 1034, 1035, 1037, 1038, 1039, 1040,
									 1041, 1042, 1043
			}; }
        }
 
        public override int[] HairHues
        {
            get { return new int[] { 1102, 1103, 1104, 1105, 1106, 1107, 1108, 1109, 1110, 1111, 1112, 1113,
									 1114, 1115, 1116, 1117, 1118, 1119, 1120, 1121, 1122, 1123, 1124, 1125,
									 1126, 1127, 1128, 1129, 1130, 1131, 1132, 1133, 1134, 1135, 1136, 1137,
									 1138, 1139, 1140, 1141, 1142, 1143, 1144, 1145, 1146, 1147, 1148, 1149
			}; }
        }
 
        public override FacialHairItemID[] FacialHairStyles
        {
            get
            {
                return new FacialHairItemID[]
                {
                    FacialHairItemID.None,
                    FacialHairItemID.LongBeard,
                    FacialHairItemID.ShortBeard,
                    FacialHairItemID.Goatee,
                    FacialHairItemID.Mustache,
                    FacialHairItemID.MediumShortBeard,
                    FacialHairItemID.MediumLongBeard,
                    FacialHairItemID.Vandyke
                };
            }
        }
 
        public override HairItemID[] MaleHairStyles
        {
            get
            {
                return new HairItemID[]
                {
                    HairItemID.None,
                    HairItemID.Short,
                    HairItemID.Long,
                    HairItemID.PonyTail,
                    HairItemID.Mohawk,
                    HairItemID.Pageboy,
                    HairItemID.Buns,
                    HairItemID.Afro,
                    HairItemID.Receeding,
                    HairItemID.TwoPigTails,
                    HairItemID.Krisna
                };
            }
        }
 
        public override HairItemID[] FemaleHairStyles
        {
            get
            {
                return new HairItemID[]
                {
                    HairItemID.None,
                    HairItemID.Short,
                    HairItemID.Long,
                    HairItemID.PonyTail,
                    HairItemID.Mohawk,
                    HairItemID.Pageboy,
                    HairItemID.Buns,
                    HairItemID.Afro,
                    HairItemID.Receeding,
                    HairItemID.TwoPigTails,
                    HairItemID.Krisna
                };
            }
        }
    }
}