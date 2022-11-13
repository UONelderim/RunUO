using System;
using System.Collections.Generic;
using Server.Items;

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

        public override int[] FacialHairStyles
        {
            get
            {
                return new int[]
                {
                    Beard.Human.Clean,              
                    Beard.Human.Goatee,    
                    Beard.Human.Mustache,          
                    Beard.Human.Vandyke,
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
                    Hair.Human.Long, 
                    Hair.Human.PonyTail,      
                    Hair.Human.Pageboy, 
                    Hair.Human.Receeding, 
                    Hair.Human.Krisna
                };
            }
        }

        public override int[] FemaleHairStyles
        {
            get 
            {
                return new int[]
                {
                    Hair.Human.Bald,        
                    Hair.Human.Short,   
                    Hair.Human.Long,
                    Hair.Human.PonyTail,    
                    Hair.Human.Pageboy,
                    Hair.Human.Buns,        
                    Hair.Human.PigTails,
                    Hair.Human.Krisna
                };
            }
        }
    }
}
