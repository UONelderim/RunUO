using System;
using System.Collections.Generic;

namespace Server
{
    public class Drow : Race
    {
        private static Race instance = null;
        public Drow( int raceID, int raceIndex ) : base( raceID, raceIndex, 400, 401, 402, 403, Expansion.None )
        {
        }

        public static Race Instance {
            get {
                return instance;
            }
        }

        public static Race Init(int raceID, int raceIndex) {
            if (instance == null) {
                instance = new Drow(raceID, raceIndex);
            }
            return instance;
        }

        protected override string[] Names
        {
            get { return new string[7] { "Drow", "Drowa", "Drowowi", "Drowa", "Drowem", "Drowie", "Drowie" }; }
        }

        protected override string[] PluralNames
        {
            get { return new string[7] { "Drowy", "Drowow", "Drowom", "Drowy", "Drowami", "Drowach", "Drowy" }; }
        }

        public override int DescNumber { get { return 505819; } }

        public override int[] SkinHues
		{
            get { return new int[] { 1109,1108,1107,1106,1409,1897,1898,1899,1908,1907,1906,1905,2106,2105,2306,2305 }; }
        }
 
        public override int[] HairHues 
        {
            get { return new int[] { 1102,1103,1104,1105,1106,1107,1108,1109,1900,1901,1902,1903,1904,1905,1906,1907,1908,2101,2102,2103,2104,2105,2106,2301 }; }
        }

        public override FacialHairItemID[] FacialHairStyles
        {
            get
            {
                return new FacialHairItemID[]
                {
                    FacialHairItemID.None,              
                    FacialHairItemID.Goatee,    
                    FacialHairItemID.Mustache,          
                    FacialHairItemID.Vandyke,
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
                    HairItemID.PonyTail,      HairItemID.Pageboy, 
                    HairItemID.Receeding, 
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
                    HairItemID.None,        HairItemID.Short,   HairItemID.Long,
                    HairItemID.PonyTail,    HairItemID.Pageboy,
                    HairItemID.Buns,        HairItemID.TwoPigTails,
                    HairItemID.Krisna
                };
            }
        }
    }
}
