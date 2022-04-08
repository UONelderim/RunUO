// 24.04.20 :: Maupishon :: zwi�kszenie gainow statow i skilli

using System;
using Server;
using Server.Mobiles;

namespace Server.Misc
{
    public class SkillCheck
    {
        private static readonly bool AntiMacroCode = false;        //Change this to false to disable anti-macro code

        public static TimeSpan AntiMacroExpire = TimeSpan.FromMinutes( 5.0 ); //How long do we remember targets/locations?
        public const int Allowance = 300;    //How many times may we use the same location/target for gain
        private const int LocationSize = 1; //The size of eeach location, make this smaller so players dont have to move as far
        private const int V = 0;
        private static bool[] UseAntiMacro = new bool[]
        {
            // true if this skill uses the anti-macro code, false if it does not
            false,// Alchemy = 0,
            false,// Anatomy = 1,
            false,// AnimalLore = 2,
            false,// ItemID = 3,
            false,// ArmsLore = 4,
            false,// Parry = 5,
            false,// Begging = 6,
            false,// Blacksmith = 7,
            false,// Fletching = 8,
            false,// Peacemaking = 9,
            false,// Camping = 10,
            false,// Carpentry = 11,
            false,// Cartography = 12,
            false,// Cooking = 13,
            false,// DetectHidden = 14,
            false,// Discordance = 15,
            false,// EvalInt = 16,
            false,// Healing = 17,
            false,// Fishing = 18,
            false,// Zielarstwo = 19,
            false,// Herding = 20,
            false,// Hiding = 21,
            false,// Provocation = 22,
            false,// Inscribe = 23,
            false,// Lockpicking = 24,
            false,// Magery = 25,
            false,// MagicResist = 26,
            false,// Tactics = 27,
            false,// Snooping = 28,
            false,// Musicianship = 29,
            false,// Poisoning = 30,
            false,// Archery = 31,
            false,// SpiritSpeak = 32,
            false,// Stealing = 33,
            false,// Tailoring = 34,
            false,// AnimalTaming = 35,
            false,// TasteID = 36,
            false,// Tinkering = 37,
            false,// Tracking = 38,
            false,// Veterinary = 39,
            false,// Swords = 40,
            false,// Macing = 41,
            false,// Fencing = 42,
            false,// Wrestling = 43,
            false,// Lumberjacking = 44,
            false,// Mining = 45,
            false,// Meditation = 46,
            false,// Stealth = 47,
            false,// RemoveTrap = 48,
            false,// Necromancy = 49,
            false,// Focus = 50,
            false,// Chivalry = 51
            false,// Bushido = 52
            false,//Ninjitsu = 53
            false // Spellweaving = 54
        };

        // 14.10.2012 :: zombie
        public static double[,] SkillGains = new double[,]
        { 
         /*                       { SkillGain,  STR,    DEX,    INT     }, */
         /* [0]  Alchemy */       { 1.5,        0,      1.0,    1.0     }, 
         /* [1]  Anatomy */       { 1.75,       2.0,    0,      2.0     }, 
         /* [2]  AnimalLore */    { 1.75,       2.0,    0,      2.4     }, 
         /* [3]  ItemID */        { 3,          0,      0,      2       }, 
         /* [4]  ArmsLore */      { 1,          2.0,    0,      1.8     }, 
         /* [5]  Parry */         { 1.0,        1.5,    1.0,    0       }, 
         /* [6]  Begging */       { 10.0,       0,      1.0,    1.0     }, 
         /* [7]  Blacksmith */    { 2.5,        3.0,   	1.5,   	0       }, 
         /* [8]  Fletching */     { 2.5,        1.5,   	3.0,    0       }, 
         /* [9]  Peacemaking */   { 10.0,       0,      1.0,    1.5    }, 
         /* [10] Camping */       { 1.0,        0.1, 	0.5,    0.2  }, 
         /* [11] Carpentry */     { 2.5,        0,      1.5,   1.0     }, 
         /* [12] Cartography */   { 2.5,        0,      1,    2,      },
         /* [13] Cooking */       { 2.0,        0,      6,      4       }, 
         /* [14] DetectHidden */  { 10.0,       0,      1.6,    1     }, 
         /* [15] Discordance */   { 1.5,        0,      0.5,   1.5    }, 
         /* [16] EvalInt */       { 1.0,        2,      0,      6       }, 
         /* [17] Healing */       { 5.0,        0,      1.2,    2       },
         /* [18] Fishing */       { 8.5,        4,      6,    0       }, 
         /* [19] Zielarstwo */    { 3.5,        0,      1,    2     }, 
         /* [20] Herding */       { 1.0,        0,      0,      6       }, 
         /* [21] Hiding */        { 5.0,        0,      5,      2       },
         /* [22] Provocation */   { 11.5,       0,      1,    2    }, 
         /* [23] Inscribe */      { 1.5,       0,      1,    3     }, 
         /* [24] Lockpicking */   { 2.5,          0,      2,   1     }, 
         /* [25] Magery */        { 2.5,        2,      0,      6       }, 
         /* [26] MagicResist */   { 1.5,        4,      0,      4     }, 
         /* [27] Tactics */       { 3.0,        4,    2,      0       }, 
         /* [28] Snooping */      { 1.0,        0,      4,      2       }, 
         /* [29] Musicianship */  { 1.5,        0,      2,      1     }, 
         /* [30] Poisoning */     { 4.0,       0,      2,      1.5    },
         /* [31] Archery */       { 2.6,        2,      4,      0       }, 
         /* [32] SpiritSpeak */   { 5.75,       0.5,   0,      4       }, 
         /* [33] Stealing */      { 10.5,       0,      5,   1.5    }, 
         /* [34] Tailoring */     { 2.5,        0,      3.5,   0.5    }, 
         /* [35] AnimalTaming */  { 10.5,       4,      0,      2       }, 
         /* [36] TasteID */       { 1.5,        0,      10,      0       }, 
         /* [37] Tinkering */     { 1.5,        0,      3,    1     }, 
         /* [38] Tracking */      { 10.5,       0,      3.5,   3    }, 
         /* [39] Veterinary */    { 2.5,        0,      4,      3     }, 
         /* [40] Swords */        { 2.25,       6,      4,      0       }, 
         /* [41] Macing */        { 2.35,       6,      4,      0       }, 
         /* [42] Fencing */       { 2.15,       6,      4,      0       }, 
         /* [43] Wrestling */     { 2.5,        6,      4,      0       }, 
         /* [44] Lumberjacking */ { 5.0,        6,      2,      0       }, 
         /* [45] Mining */        { 5.0,        6.5,      2,      0       }, 
         /* [46] Meditation */    { 4.0,        0.5,   0,      6       }, 
         /* [47] Stealth */       { 6.5,        0,      6,      2       }, 
         /* [48] RemoveTrap */    { 8.5,        0,      4,      4       }, 
         /* [49] Necromancy */    { 2.5,        2,    1,    6       }, 
         /* [50] Focus */         { 0,          0,      4,      1.5    }, 
         /* [51] Chivalry */      { 2.5,        2,      0,      1.5    }, 
         /* [52] Bushido */       { 2.5,        2,      0,      1.5    }, 
         /* [53] Ninjitsu */      { 2.5,        2,      0,      1.5    }, 
         /* [54] Spellweaving */  { 2.0,        1.5,   0,      4       }
        };

        public static double[,] SkillLevels = new double[,]
        {
            // minSkill, maxSkill, hours
            // przyklad: przyblizony czas koxu od 0% do 10% skilla wynosi 0.2h (12 minut)
            {   0.0,  10, 0.25 },
            {  10.1,  20, 0.27 },
            {  20.1,  30, 0.30 },
            {  30.1,  40, 0.44 },
            {  40.1,  50, 0.63 },
            {  50.1,  60, 0.92 },
            {  60.1,  70, 1.34 },
            {  70.1,  80, 1.94 },
            {  80.1,  90, 2.82 },
            {  90.1, 100, 4.10 },
            { 100.1, 110, 4.50 },
            { 110.1, 120, 4.96 }
        };
        // zombie

        public static void Initialize()
        {
        // Begin mod to enable XmlSpawner skill triggering
        Mobile.SkillCheckLocationHandler = new SkillCheckLocationHandler( XmlSpawnerSkillCheck.Mobile_SkillCheckLocation );
        Mobile.SkillCheckDirectLocationHandler = new SkillCheckDirectLocationHandler( XmlSpawnerSkillCheck.Mobile_SkillCheckDirectLocation );

        Mobile.SkillCheckTargetHandler = new SkillCheckTargetHandler( XmlSpawnerSkillCheck.Mobile_SkillCheckTarget );
        Mobile.SkillCheckDirectTargetHandler = new SkillCheckDirectTargetHandler( XmlSpawnerSkillCheck.Mobile_SkillCheckDirectTarget );
        // End mod to enable XmlSpawner skill triggering

        if(SkillGains.GetUpperBound(0) + 1 != SkillInfo.Table.Length) {
                throw new Exception(String.Format("Skill gains modifier list length incorrect. Expected: {0} Actual:{1}", SkillInfo.Table.Length, SkillGains.GetUpperBound(0) + 1));
            }
        }

        // 14.11.2012 :: zombie
        public static bool IsConstantGainSkill( Skill skill )
        {
            SkillName[] cgSkills = new SkillName[]
            { 
                SkillName.Hiding, SkillName.Focus, SkillName.Meditation, SkillName.Camping,
                SkillName.Anatomy, SkillName.AnimalLore, SkillName.ArmsLore, SkillName.Parry,
                SkillName.SpiritSpeak, SkillName.Mining, SkillName.Lumberjacking, SkillName.Fishing,
                SkillName.Healing, SkillName.Veterinary, SkillName.TasteID, SkillName.Stealing, 
                SkillName.Tactics, SkillName.EvalInt, SkillName.Herding, SkillName.Begging, SkillName.DetectHidden
            };
            
            foreach ( SkillName cgSkill in cgSkills )
            {
                if ( skill.SkillName == cgSkill )
                    return true;
            }

            return false;
        }
        // zombie

        public static bool Mobile_SkillCheckLocation( Mobile from, SkillName skillName, double minSkill, double maxSkill )
        {
            Skill skill = from.Skills[skillName];

            if ( skill == null )
                return false;
                            
            double value = skill.Value;

            if ( value < minSkill )
                return false; // Too difficult
            else if ( value >= maxSkill )
                return true; // No challenge

            double chance = (value - minSkill) / (maxSkill - minSkill);

            Point2D loc = new Point2D( from.Location.X / LocationSize, from.Location.Y / LocationSize );
            return CheckSkill( from, skill, loc, chance );
        }

        public static bool Mobile_SkillCheckDirectLocation( Mobile from, SkillName skillName, double chance )
        {
            Skill skill = from.Skills[skillName];

            if ( skill == null )
                return false;

            if ( chance < 0.0 )
                return false; // Too difficult
            else if ( chance >= 1.0 )
                return true; // No challenge

            Point2D loc = new Point2D( from.Location.X / LocationSize, from.Location.Y / LocationSize );
            return CheckSkill( from, skill, loc, chance );
        }

        // 19.09.2012 :: zombie
        public static bool CheckSkill( Mobile from, Skill skill, object amObj, double chance )
        {
            if ( from.Skills.Cap == 0 || skill.SkillID >= SkillGains.GetLength( 0 ) )
                return false;
            
            bool success = ( chance >= Utility.RandomDouble() );
            
            double successChance;
            if (IsConstantGainSkill(skill))
                successChance = 1.0;
            else
            {   
				if ( chance > 0.99 || chance < 0.01 )
				{
					successChance = 0.10;
				}
				else
				{
					double dist = Math.Abs( chance - 0.5 ); // jak daleko szansa odbiega od optymalnej wartosci 50%
					double close = 0.15;
					if( dist <= close )
						// optymalny gain w okolicach 15% od warto�ci optymalnej
						successChance = 1.0;    
					else
						// wraz z oddaleniem od zakresu optimum gain opada lagodniej niz liniowo
						successChance = 1 - 0.5 * Math.Pow( (dist - close)/(0.50-close), 2 );
				}
            }
            successChance            = Math.Max( 0, Math.Min( successChance, 1 ) );
            double baseSkillGc       = SkillGains[ skill.SkillID, 0 ];
            double skillLevelChance  = 0;
            double racialScalar      = 1;

            // 04.11.2012 :: zombie            
            for( int i = 0; i < SkillLevels.GetLength( 0 ); i++ )
            {
                double minSkill = SkillLevels[ i, 0 ];
                double maxSkill = SkillLevels[ i, 1 ]; 
                double hours    = SkillLevels[ i, 2 ];

                if ( skill.Base >= minSkill && skill.Base <= maxSkill )
                {
                    double uses = ( hours * 3600 ) / baseSkillGc ;
                    
                    skillLevelChance = 10 * ( maxSkill - minSkill ) / uses;

                    break;
                }
            }

            racialScalar = from.Race.SkillGainModifier(skill.SkillName);

            double skillGc = skillLevelChance * successChance * racialScalar;
            
            
            // 10.11.2012 :: zombie :: testy
            /*
            if ( skill.SkillName == SkillName.AnimalTaming ||
                skill.SkillName == SkillName.Discordance ||
                skill.SkillName == SkillName.Provocation ||
                skill.SkillName == SkillName.ItemID ||
                skill.SkillName == SkillName.Zielarstwo ||
                skill.SkillName == SkillName.Stealth )
            {
                skillGc = 1;
            }
            */
            // zombie

            if ( from is BaseCreature && ( (BaseCreature)from ).Controlled )
            {
                baseSkillGc *= 6;
                skillGc *= 6;
            }

            // 31.10.2012 :: zombie :: PowerHour
            if ( from is PlayerMobile )
            {
                PlayerMobile pm = (PlayerMobile)from;

                if ( pm.HasPowerHour )
                {
                    baseSkillGc *= 2;
                    skillGc *= 2;
                }

                if (pm.GainsDebugEnabled && skill.Lock == SkillLock.Up )
                {
					if ( skill.SkillName != SkillName.Meditation && skill.SkillName != SkillName.Focus )
						pm.SendMessage(0x40, "[{0}: {1}%] GainChance = {2}% SuccessChance = {3}", skill.Name, skill.Value, Math.Round(skillGc * 100, 2), Math.Round(successChance, 2));
                }
            }
            // zombie

            if ( from.Alive && ( (AllowGain(from, skill, amObj) && skillGc >= Utility.RandomDouble()) || skill.Base < 5.0 ) )
                Gain( from, skill, 0.1 * baseSkillGc * successChance );

            return success;
        }
        // zombie

        public static bool Mobile_SkillCheckTarget( Mobile from, SkillName skillName, object target, double minSkill, double maxSkill )
        {
            Skill skill = from.Skills[skillName];

            if ( skill == null )
                return false;

            double value = skill.Value;

            if ( value < minSkill )
                return false; // Too difficult
            else if ( value >= maxSkill )
                return true; // No challenge

            double chance = (value - minSkill) / (maxSkill - minSkill);

            return CheckSkill( from, skill, target, chance );
        }

        public static bool Mobile_SkillCheckDirectTarget( Mobile from, SkillName skillName, object target, double chance )
        {
            Skill skill = from.Skills[skillName];

            if ( skill == null )
                return false;

            if ( chance < 0.0 )
                return false; // Too difficult
            else if ( chance >= 1.0 )
                return true; // No challenge

            return CheckSkill( from, skill, target, chance );
        }

        private static bool AllowGain( Mobile from, Skill skill, object obj )
        {
            if ( AntiMacroCode && from is PlayerMobile && UseAntiMacro[skill.Info.SkillID] )
                return ( (PlayerMobile)from ).AntiMacroCheck( skill, obj );
            else
            {
                // 16.08.2012 :: zombie :: [sus
                if ( from is PlayerMobile )
                {
                    PlayerMobile pm = (PlayerMobile)from;
                    if ( pm.Seers != null )
                    {
                        foreach ( Mobile seer in pm.Seers.Keys )
                        {
                            if ( ( DateTime ) pm.Seers[ seer ] >= DateTime.Now )
                                seer.SendMessage( 38, "[{0}]\t[{1}]", from.Name, skill.Name ); // [~1_NAME~] [~2_SKILL~]
                        }
                    }
                }
                // zombie

                return true;
            }
        }

        public enum Stat { Str, Dex, Int }

        public static void Gain( Mobile from, Skill skill, double gc )
        {
            if ( from.Region.IsPartOf( typeof( Regions.JailRegion ) ) )
                return;

            if ( from is BaseCreature && ((BaseCreature)from).IsDeadPet )
                return;

            //if ( skill.SkillName == SkillName.Focus && from is BaseCreature )
            if (skill.SkillName == SkillName.Focus) //Disabled focus gains for good
                return;

            if ( skill.Base < skill.Cap && skill.Lock == SkillLock.Up )
            {
                int toGain = 1;

                //if ( skill.Base <= 10.0 )
                    //toGain = Utility.Random( 4 ) + 1;

                Skills skills = from.Skills;

                if ( from.Player && (skills.Total / skills.Cap ) >= Utility.RandomDouble() )//( skills.Total >= skills.Cap )
                {
                    for ( int i = 0; i < skills.Length; ++i )
                    {
                        Skill toLower = skills[i];

                        if ( toLower != skill && toLower.Lock == SkillLock.Down && toLower.BaseFixedPoint >= toGain )
                        {
                            toLower.BaseFixedPoint -= toGain;
                            break;
                        }
                    }
                }

                if ( !from.Player || (skills.Total + toGain) <= skills.Cap || from is BaseCreature)
                {
                    skill.BaseFixedPoint += toGain;
                }
            }

            if ( from is PlayerMobile && skill.Lock == SkillLock.Up )
            {
                PlayerMobile pm = (PlayerMobile)from;

                // 19.09.2012 :: zombie :: gainy statow
                int rawStatTotal = from.RawStatTotal > 250 ? 250 : from.RawStatTotal;
                double statLevelChance = Math.Pow( (250.0 - 180.0) * 0.01, 3 ) * 0.0016; //testowo wartosc jak przy 180 statcapa - qwerty1234
                //double statLevelChance = Math.Pow( (250.0 - (double)rawStatTotal) * 0.01, 2.4 ) * 0.0016 + 0.0037;

                double[] statGains = new double[3];
                for ( int i = 0; i < statGains.Length; i++ )
                    statGains[i] = ( SkillGains[skill.SkillID, i + 1] * gc ) * statLevelChance;

                StatLockType[] statLocks = new StatLockType[]{ pm.StrLock, pm.DexLock, pm.IntLock };
                for( int i = 0; i < statGains.Length; i++ )
                {
                    if( statLocks[ i ] != StatLockType.Up )
                        continue;

                    // pomimo systemu gwarantowanych przyrostow nie koksamy danego stata jesli jego przyrost na danym skillu jest zerowy
                    if( SkillGains[skill.SkillID, i + 1] == 0.0 )
                        continue;

                    // gwarantowane gainy (kumulacja szans):
                    SetStatGain( pm, (Stat)i, GetStatGain( pm, (Stat)i ) + statGains[ i ] );

                    if( GetStatGain( pm, (Stat)i ) >= Utility.RandomDouble() )
                    {
                        GainStat( from, (Stat)i );
                        SetStatGain( pm, (Stat)i, 0 );
                    }
                }

                //if( pm.GainsDebugEnabled )
                    //from.SendMessage( 0x40, "[STR: {0}%] [DEX: {1}%] [INT: {2}%]", Math.Round( pm.StrGain * 100, 3 ),  Math.Round( pm.DexGain * 100, 3 ),  Math.Round( pm.IntGain * 100, 3 ) );
                // zombie
            }
        }

        // 19.09.2012 :: zombie
        public static double GetStatGain( PlayerMobile pm, Stat stat )
        {
            switch ( stat )
            {
                case Stat.Str:
                    return pm.StrGain;
                case Stat.Dex:
                    return pm.DexGain;
                case Stat.Int:
                    return pm.IntGain;
                default:
                    return 0;
            }
        }

        public static void SetStatGain( PlayerMobile pm, Stat stat, double value )
        {
            switch ( stat )
            {
                case Stat.Str:
                    pm.StrGain = value;
                break;
                case Stat.Dex:
                    pm.DexGain = value;
                break;
                case Stat.Int:
                    pm.IntGain = value;
                break;
            }
        }
        // zombie

        public static bool CanLower( Mobile from, Stat stat )
        {
            switch ( stat )
            {
                case Stat.Str: return ( from.StrLock == StatLockType.Down && from.RawStr > 10 );
                case Stat.Dex: return ( from.DexLock == StatLockType.Down && from.RawDex > 10 );
                case Stat.Int: return ( from.IntLock == StatLockType.Down && from.RawInt > 10 );
            }

            return false;
        }

        public static bool CanRaise( Mobile from, Stat stat )
        {
            if ( !(from is BaseCreature && ((BaseCreature)from).Controlled) )
            {
                if ( from.RawStatTotal >= from.StatCap )
                    return false;
            }

            switch ( stat )
            {
                case Stat.Str: return ( from.StrLock == StatLockType.Up && from.RawStr < 125 );
                case Stat.Dex: return ( from.DexLock == StatLockType.Up && from.RawDex < 125 );
                case Stat.Int: return ( from.IntLock == StatLockType.Up && from.RawInt < 125 );
            }

            return false;
        }

        public static void IncreaseStat( Mobile from, Stat stat, bool atrophy )
        {
            atrophy = atrophy || (from.RawStatTotal >= from.StatCap);


            switch ( stat )
            {
                case Stat.Str:
                {
                    if ( atrophy )
                    {
                        if ( CanLower( from, Stat.Dex ) && (from.RawDex < from.RawInt || !CanLower( from, Stat.Int )) )
                            --from.RawDex;
                        else if ( CanLower( from, Stat.Int ) )
                            --from.RawInt;
                    }

                    if ( CanRaise( from, Stat.Str ) )
                        ++from.RawStr;

                    break;
                }
                case Stat.Dex:
                {
                    if ( atrophy )
                    {
                        if ( CanLower( from, Stat.Str ) && (from.RawStr < from.RawInt || !CanLower( from, Stat.Int )) )
                            --from.RawStr;
                        else if ( CanLower( from, Stat.Int ) )
                            --from.RawInt;
                    }

                    if ( CanRaise( from, Stat.Dex ) )
                        ++from.RawDex;

                    break;
                }
                case Stat.Int:
                {
                    if ( atrophy )
                    {
                        if ( CanLower( from, Stat.Str ) && (from.RawStr < from.RawDex || !CanLower( from, Stat.Dex )) )
                            --from.RawStr;
                        else if ( CanLower( from, Stat.Dex ) )
                            --from.RawDex;
                    }

                    if ( CanRaise( from, Stat.Int ) )
                        ++from.RawInt;

                    break;
                }
            }
        }

        // 19.09.2012 :: zombie
        public static void GainStat( Mobile from, Stat stat )
        {
            bool atrophy = ( (from.RawStatTotal / (double)from.StatCap) >= Utility.RandomDouble() );


            IncreaseStat( from, stat, atrophy );
        }
        // zombie
    }
}