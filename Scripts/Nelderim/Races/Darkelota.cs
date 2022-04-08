using System;
using System.Collections.Generic;
 
namespace Server
{
    public class Darkelota : Race
    {
        public Darkelota( int raceID, int raceIndex ) : base( raceID, raceIndex, 400, 401, 402, 403, Expansion.None )
        {
        }
 
        protected override string[] Names 
        { 
            get { return new string[7] { "Darkelota", "Darkeloty", "Darkelocie", "Darkelote", "Darkelotem", "Darkelocie", "Darkelocie" }; } 
        } 

        protected override string[] PluralNames 
        { 
            get { return new string[7] { "Darkeloci", "Darkelotow", "Darkelotom", "Darkelotow", "Darkelotami", "Darkelotach", "Darkeloci" }; } 
        } 
 
        public override int DescNumber { get { return 505819; } }

        public override int[] SkinHues
        {
            get { return new int[] { 1106, 1107, 1108, 1109, 1175, 1899, 2703, 2306, 1309, 1890 }; }
        }
 
        public override int[] HairHues
        { 
            get { return new int[] { 1049, 1050, 1051, 1056, 
                                     1057, 1058, 1115, 1116, 1117,
                                     1133, 1136, 1137, 1138, 1139, 1140, 1141, 1142, 1143, 1144, 
                                     1145, 1146, 1147, 1148, 1149,
									 1102, 1103, 1104, 1105, 1106, 1107, 1108, 1109,
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