using System;
using System.Collections.Generic;
 
namespace Server
{
    public class Jarling : Race
    {
        private static Race instance = null;
        public Jarling( int raceID, int raceIndex ) : base( raceID, raceIndex, 400, 401, 402, 403, Expansion.None )
        {
        }

        public static Race Instance {
            get {
                return instance;
            }
        }

        public static Race Init(int raceID, int raceIndex) {
            if (instance == null) {
                instance = new Jarling(raceID, raceIndex);
            }
            return instance;
        }

        protected override string[] Names 
        { 
            get { return new string[7] { "Jarling", "Jarlinga", "Jarlingowi", "Jarlinga", "Jarlingiem", "Jarlingu", "Jarlingu" }; } 
        } 

        protected override string[] PluralNames 
        { 
            get { return new string[7] { "Jarlingowie", "Jarlingow", "Jarlingom", "Jarlingow", "Jarlingami", "Jarlingch", "Jarlingowie" }; } 
        } 
 
        public override int DescNumber { get { return 505817; } }

        public override int[] SkinHues
        {
            get { return new int[] { 1002, 1003, 1009, 1010, 1016, 1017, 1018, 1019, 1024
			}; }
        }
 
        public override int[] HairHues
        {
            get { return new int[] { 1110, 1111, 1112, 1113, 1114, 1115, 1118, 1119, 1120, 1121, 1122, 1123,
									 1126, 1127, 1128, 1129
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