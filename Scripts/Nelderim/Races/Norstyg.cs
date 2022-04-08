using System;
using System.Collections.Generic;
 
namespace Server
{
    public class Norstyg : Race
    {
        public Norstyg( int raceID, int raceIndex ) : base( raceID, raceIndex, 400, 401, 402, 403, Expansion.None )
        {
        }
 
        protected override string[] Names 
        { 
            get { return new string[7] { "Norstyg", "Norstyga", "Norstygowi", "Norstyga", "Norstygiem", "Norstygu", "Norstygu" }; } 
        } 

        protected override string[] PluralNames 
        { 
            get { return new string[7] { "Norstygowie", "Norstygow", "Norstygom", "Norstygow", "Norstygami", "Norstygach", "Norstygowie" }; } 
        }

        public override int DescNumber { get { return 505817; } }

        public override int[] SkinHues
        {
            get { return new int[] { 1037, 1038, 1154, 2101, 2102, 2103, 2307 }; }
        }
 
        public override int[] HairHues
        {
            get { return new int[] { 1045, 1046, 1047, 1048, 1049, 1050, 1051, 1052, 1053, 1054, 1055, 1056, 
                                     1057, 1058, 1110, 1112, 1113, 1114, 1115, 1116, 1117, 1118, 1119, 1120, 
                                     1121, 1122, 1123, 1124, 1125, 1126, 1127, 1128, 1129, 1130, 1131, 1132, 
                                     1133, 1134, 1135, 1136, 1137, 1138, 1139, 1140, 1141, 1142, 1143, 1144, 
                                     1145, 1146, 1147, 1148, 1149,
									 976
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