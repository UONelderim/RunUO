using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Server.Misc;
using Server.Items;
using Server.Gumps;
using Server.Multis;
using Server.Engines.Help;
using Server.ContextMenus;
using Server.Network;
using Server.Spells;
using Server.Spells.Fifth;
using Server.Spells.Seventh;
using Server.Spells.Necromancy;
using Server.Spells.Ninjitsu;
using Server.Spells.Bushido;
using Server.Targeting;
using Server.Engines.Quests;
using Server.Factions;
using Server.Regions;
using Server.Accounting;
using Server.Engines.CannedEvil;
using Server.Engines.Craft;
using Server.Spells.Spellweaving;
using Server.Engines.XmlSpawner2;
using Server.Commands;
using Nelderim.Speech;
using Nelderim.CharacterSheet;
using Nelderim.Gains;
using Server.Engines.Quests.CraftingExperiments;

namespace Server.Mobiles
{
    #region Enums

    [Flags]
    public enum PlayerFlag // First 16 bits are reserved for default-distro use, start custom flags at 0x00010000
    {
        None                 = 0x00000000,
        Glassblowing         = 0x00000001,
        Masonry              = 0x00000002,
        SandMining           = 0x00000004,
        StoneMining          = 0x00000008,
        ToggleMiningStone    = 0x00000010,
        KarmaLocked          = 0x00000020,
        AutoRenewInsurance   = 0x00000040,
        UseOwnFilter         = 0x00000080,
        PublicMyRunUO        = 0x00000100,
        PagingSquelched      = 0x00000200,
        Young                = 0x00000400,
        AcceptGuildInvites   = 0x00000800,
        DisplayChampionTitle = 0x00001000,
		ColorMetal           = 0x00002000,
        Spellweaving         = 0x00004000
    }

    public enum NpcGuild
    {
        None,
        MagesGuild,
        WarriorsGuild,
        ThievesGuild,
        RangersGuild,
        HealersGuild,
        MinersGuild,
        MerchantsGuild,
        TinkersGuild,
        TailorsGuild,
        FishermensGuild,
        BardsGuild,
        BlacksmithsGuild
    }

    public enum SolenFriendship
    {
        None,
        Red,
        Black
    }
    #endregion

    public class PlayerMobile : Mobile, IHonorTarget
    {
        private class CountAndTimeStamp
        {
            private int m_Count;
            private DateTime m_Stamp;

            public CountAndTimeStamp()
            {
            }

            public DateTime TimeStamp { get{ return m_Stamp; } }
            public int Count 
            { 
                get { return m_Count; } 
                set    { m_Count = value; m_Stamp = DateTime.Now; } 
            }
        }

        private DesignContext m_DesignContext;

        private NpcGuild m_NpcGuild;
        private DateTime m_NpcGuildJoinTime;
        private TimeSpan m_NpcGuildGameTime;
        private PlayerFlag m_Flags;
        private int m_StepsTaken;
        private int m_Profession;
        private int m_NonAutoreinsuredItems; // number of items that could not be automaitically reinsured because gold in bank was not enough

        private DateTime m_LastOnline;
        private Server.Guilds.RankDefinition m_GuildRank;

        private int m_GuildMessageHue, m_AllianceMessageHue;

        private List<Mobile> m_AllFollowers;
       
    
        private int m_TrapsActive = 0;

        [CommandProperty(AccessLevel.GameMaster)]
        public int TrapsActive
        {
            get { return m_TrapsActive; }
            set { m_TrapsActive = value; }
        }
        private Container m_GrabContainer;
        public Container GrabContainer
        {
            get { return m_GrabContainer; }
            set { m_GrabContainer = value; }        
        }

        #region Karta postaci
        public int Crest
        {
            get { return CharacterSheet.Get(this).Crest; }
            set { CharacterSheet.Get( this ).Crest = value; }
        }

        public CrestSize CrestSize
        {
            get { return CharacterSheet.Get( this ).CrestSize; }
            set { CharacterSheet.Get( this ).CrestSize = value; }
        }

        public string AppearanceAndCharacteristic
        {
            get { return CharacterSheet.Get( this ).AppearanceAndCharacteristic; }
            set { CharacterSheet.Get( this ).AppearanceAndCharacteristic = value; }
        }

        public string HistoryAndProfession
        {
            get { return CharacterSheet.Get( this ).HistoryAndProfession; }
            set { CharacterSheet.Get( this ).HistoryAndProfession = value; }
        }

        public bool AttendenceInEvents
        {
            get { return CharacterSheet.Get( this ).AttendenceInEvents; }
            set { CharacterSheet.Get( this ).AttendenceInEvents = value; }
        }
        
        public EventFrequency EventFrequencyAttendence
        {
            get { return CharacterSheet.Get( this ).EventFrequencyAttendence; }
            set { CharacterSheet.Get( this ).EventFrequencyAttendence = value; }
        }

        // TODO: Leave only QuestPoints and LastQuestPointsTime in PlayerMobile
        [CommandProperty(AccessLevel.Seer)]
        public int QuestPoints
        {
            get { return CharacterSheet.Get( this ).QuestPoints; }
            set 
            {
                CharacterSheet.Get( this ).QuestPoints = value;
                LastQuestPointsTime = DateTime.Now;
            }
        }
        
        public SortedSet<QuestPointsHistoryEntry> QuestPointsHistory
        {
            get { return CharacterSheet.Get( this ).QuestPointsHistoryEntries; }

        }


        [CommandProperty(AccessLevel.Seer)]
        public DateTime LastQuestPointsTime
        {
            get { return CharacterSheet.Get( this ).LastQuestPointsTime; }
            set { CharacterSheet.Get( this ).LastQuestPointsTime = value; }
        }
        #endregion

        #region Getters & Setters

        public List<Mobile> AllFollowers
        {
            get
            {
                if ( m_AllFollowers == null )
                    m_AllFollowers = new List<Mobile>(); ;
                return m_AllFollowers;
            }
        }

        public Server.Guilds.RankDefinition GuildRank
        {
            get
            {
                if( this.AccessLevel >= AccessLevel.GameMaster )
                    return Server.Guilds.RankDefinition.Leader;
                else
                    return m_GuildRank; 
            }
            set{ m_GuildRank = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int GuildMessageHue
        {
            get{ return m_GuildMessageHue; }
            set{ m_GuildMessageHue = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int AllianceMessageHue
        {
            get { return m_AllianceMessageHue; }
            set { m_AllianceMessageHue = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Profession
        {
            get{ return m_Profession; }
            set{ m_Profession = value; }
        }

        public int StepsTaken
        {
            get{ return m_StepsTaken; }
            set{ m_StepsTaken = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public NpcGuild NpcGuild
        {
            get{ return m_NpcGuild; }
            set{ m_NpcGuild = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public DateTime NpcGuildJoinTime
        {
            get{ return m_NpcGuildJoinTime; }
            set{ m_NpcGuildJoinTime = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public DateTime LastOnline
        {
            get{ return m_LastOnline; }
            set{ m_LastOnline = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public TimeSpan NpcGuildGameTime
        {
            get{ return m_NpcGuildGameTime; }
            set{ m_NpcGuildGameTime = value; }
        }

        private int m_ToTItemsTurnedIn;

        [CommandProperty( AccessLevel.GameMaster )]
        public int ToTItemsTurnedIn
        {
            get { return m_ToTItemsTurnedIn; }
            set { m_ToTItemsTurnedIn = value; }
        }

        private int m_ToTTotalMonsterFame;

        [CommandProperty( AccessLevel.GameMaster )]
        public int ToTTotalMonsterFame
        {
            get { return m_ToTTotalMonsterFame; }
            set { m_ToTTotalMonsterFame = value; }
        }

        #endregion
        #region - possession
        public Mobile m_PossessMob;
        public Mobile m_PossessStorageMob;
        #endregion

        #region PlayerFlags
        public PlayerFlag Flags
        {
            get{ return m_Flags; }
            set{ m_Flags = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool PagingSquelched
        {
            get{ return GetFlag( PlayerFlag.PagingSquelched ); }
            set{ SetFlag( PlayerFlag.PagingSquelched, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool ColorMetal
        {
            get{ return GetFlag( PlayerFlag.ColorMetal ); }
            set{ SetFlag( PlayerFlag.ColorMetal, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool Glassblowing
        {
            get{ return GetFlag( PlayerFlag.Glassblowing ); }
            set{ SetFlag( PlayerFlag.Glassblowing, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool Masonry
        {
            get{ return GetFlag( PlayerFlag.Masonry ); }
            set{ SetFlag( PlayerFlag.Masonry, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool SandMining
        {
            get{ return GetFlag( PlayerFlag.SandMining ); }
            set{ SetFlag( PlayerFlag.SandMining, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool StoneMining
        {
            get{ return GetFlag( PlayerFlag.StoneMining ); }
            set{ SetFlag( PlayerFlag.StoneMining, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool ToggleMiningStone
        {
            get{ return GetFlag( PlayerFlag.ToggleMiningStone ); }
            set{ SetFlag( PlayerFlag.ToggleMiningStone, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool KarmaLocked
        {
            get{ return GetFlag( PlayerFlag.KarmaLocked ); }
            set{ SetFlag( PlayerFlag.KarmaLocked, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool AutoRenewInsurance
        {
            get{ return GetFlag( PlayerFlag.AutoRenewInsurance ); }
            set{ SetFlag( PlayerFlag.AutoRenewInsurance, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool UseOwnFilter
        {
            get{ return GetFlag( PlayerFlag.UseOwnFilter ); }
            set{ SetFlag( PlayerFlag.UseOwnFilter, value ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool PublicMyRunUO
        {
            get{ return GetFlag( PlayerFlag.PublicMyRunUO ); }
            set{ SetFlag( PlayerFlag.PublicMyRunUO, value ); InvalidateMyRunUO(); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool AcceptGuildInvites
        {
            get{ return GetFlag( PlayerFlag.AcceptGuildInvites ); }
            set{ SetFlag( PlayerFlag.AcceptGuildInvites, value ); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Spellweaving 
        { 
            get { return GetFlag(PlayerFlag.Spellweaving); } 
            set { SetFlag(PlayerFlag.Spellweaving, value); } 
        }
        #endregion

        private DateTime m_AnkhNextUse;

        [CommandProperty( AccessLevel.GameMaster )]
        public DateTime AnkhNextUse
        {
            get { return m_AnkhNextUse; }
            set { m_AnkhNextUse = value; }
        }

        #region Nietolerancja
        private bool m_IntolerateAction;

        [CommandProperty( AccessLevel.GameMaster )]
        public bool Noticed
        {
            set { m_IntolerateAction = value; }
            get { return m_IntolerateAction; }
        }
        #endregion
        
        // 10.07.2012 :: zombie
        #region DetectHidden
        private DateTime m_NextPassiveDetectHiddenCheck;
        private Timer m_PassiveDetectHiddenTimer;

        public Timer PassiveDetectHiddenTimer
        {
            get { return m_PassiveDetectHiddenTimer; }
            set { m_PassiveDetectHiddenTimer = value; }
        }

        public DateTime NextPassiveDetectHiddenCheck
        {
            get { return m_NextPassiveDetectHiddenCheck; }
            set { m_NextPassiveDetectHiddenCheck = value; }
        }

     
        #endregion
        // zombie

        #region Total weight bonus
        private int m_weightBonus = 0;
        public int WeigthBonus
        {
            get { return m_weightBonus; }
            set { m_weightBonus = value; }
        }
        #endregion

        // 19.09.2012 :: zombie
        #region Stats gain

        public double StrGain
        {
            get { return Gains.Get( this ).StrGain; }
            set { Gains.Get( this ).StrGain = value; }
        }

        public double DexGain
        {
            get { return Gains.Get( this ).DexGain; }
            set { Gains.Get( this ).DexGain = value; }
        }

        public double IntGain
        {
            get { return Gains.Get( this ).IntGain; }
            set { Gains.Get( this ).IntGain = value; }
        }
        #endregion
        // zombie

        // 22.09.2012 :: zombie
        #region Racial kills

        private int[] m_RacialKills;

        public int[] RacialKills
        {
            get{ return m_RacialKills; }
            set { m_RacialKills = value; }
        }

        public int GetRacialKills( Race r )
        {
            return m_RacialKills[ r.RaceID ];
        }
        #endregion
        // zombie

        // 29.10.2012 :: zombie
        #region PowerHour

        [CommandProperty( AccessLevel.GameMaster )]
        public DateTime LastPowerHour
        {
            get { return Gains.Get( this ).LastPowerHour; }
            set { Gains.Get( this ).LastPowerHour = value; } 
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool AllowPowerHour
        {
            get { return !HasPowerHour && ( DateTime.Now.DayOfYear > LastPowerHour.DayOfYear || DateTime.Now.Year != LastPowerHour.Year ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool HasPowerHour
        {
            get { return DateTime.Now - LastPowerHour <= PowerHour.Duration; }
        }
        #endregion
        // zombie

        // 31.10.2012 :: zombie
        #region GainsDebug
        private bool m_GainsDebugEnabled;

        public bool GainsDebugEnabled 
        { 
            get { return m_GainsDebugEnabled;  }
            set { m_GainsDebugEnabled = value; }
        }
        #endregion
        // zombie

        // szczaw :: 2013.01.13 :: dodanie metod do zabierania zlota
        #region CanAfford, TryTakeGold
        // platnosc ponizej 2 000 golda tylko gotowka, powyzej mozna placic karta
        // platnosc albo z banku, albo z plecak - nie ma laczenia
        public bool CanAfford( int amount )
        {
            if(amount <= this.TotalGold)               // ...mam gotowke - problem zglowy... 
                return true;            
            else if(amount < 2000)                    // ...nie mam gotowki i nie moge placic karta...
                return false;
            else if(amount < Banker.GetBalance(this)) // ...nie mam gotowki, ale w banku bogactwo...
                return true;
            else
                return false;                         // ...nie mam gotowki, a konto zajal komornik...
        }

        public bool TryTakeGold( int amount )
        {
            if(!CanAfford(amount))                    // nie stac mnie, to mnie nie stac, na chuj drazyc temat?
                return false;

            if(amount <= this.TotalGold)
                this.Backpack.ConsumeTotal(typeof(Gold), amount);

            else
                Banker.Withdraw(this, amount);

            return true;
        }
        #endregion 

        private DateTime m_PeacedUntil;

        [CommandProperty( AccessLevel.GameMaster )]
        public DateTime PeacedUntil
        {
            get { return m_PeacedUntil; }
            set { m_PeacedUntil = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SpeechLang LanguageSpeaking {
            get { return Speech.Get( this ).LanguageSpeaking; }
            set { Speech.Get( this ).LanguageSpeaking = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public KnownLanguages LanguagesKnown {
            get { return Speech.Get( this ).LanguagesKnown; }
            set { Speech.Get( this ).LanguagesKnown = value; }
        }

        public static Direction GetDirection4( Point3D from, Point3D to )
        {
            int dx = from.X - to.X;
            int dy = from.Y - to.Y;

            int rx = dx - dy;
            int ry = dx + dy;

            Direction ret;

            if ( rx >= 0 && ry >= 0 )
                ret = Direction.West;
            else if ( rx >= 0 && ry < 0 )
                ret = Direction.South;
            else if ( rx < 0 && ry < 0 )
                ret = Direction.East;
            else
                ret = Direction.North;

            return ret;
        }

        public override bool OnDroppedItemToWorld( Item item, Point3D location )
        {
            if ( !base.OnDroppedItemToWorld( item, location ) )
                return false;

            BounceInfo bi = item.GetBounce();

            if ( bi != null )
            {
                Type type = item.GetType();

                if ( type.IsDefined( typeof( FurnitureAttribute ), true ) || type.IsDefined( typeof( DynamicFlipingAttribute ), true ) )
                {
                    object[] objs = type.GetCustomAttributes( typeof( FlipableAttribute ), true );

                    if ( objs != null && objs.Length > 0 )
                    {
                        FlipableAttribute fp = objs[0] as FlipableAttribute;

                        if ( fp != null )
                        {
                            int[] itemIDs = fp.ItemIDs;

                            Point3D oldWorldLoc = bi.m_WorldLoc;
                            Point3D newWorldLoc = location;

                            if ( oldWorldLoc.X != newWorldLoc.X || oldWorldLoc.Y != newWorldLoc.Y )
                            {
                                Direction dir = GetDirection4( oldWorldLoc, newWorldLoc );

                                if ( itemIDs.Length == 2 )
                                {
                                    switch ( dir )
                                    {
                                        case Direction.North:
                                        case Direction.South: item.ItemID = itemIDs[0]; break;
                                        case Direction.East:
                                        case Direction.West: item.ItemID = itemIDs[1]; break;
                                    }
                                }
                                else if ( itemIDs.Length == 4 )
                                {
                                    switch ( dir )
                                    {
                                        case Direction.South: item.ItemID = itemIDs[0]; break;
                                        case Direction.East: item.ItemID = itemIDs[1]; break;
                                        case Direction.North: item.ItemID = itemIDs[2]; break;
                                        case Direction.West: item.ItemID = itemIDs[3]; break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }

        public bool GetFlag( PlayerFlag flag )
        {
            return ( (m_Flags & flag) != 0 );
        }

        public void SetFlag( PlayerFlag flag, bool value )
        {
            if ( value )
                m_Flags |= flag;
            else
                m_Flags &= ~flag;
        }

        public DesignContext DesignContext
        {
            get{ return m_DesignContext; }
            set{ m_DesignContext = value; }
        }

        public static void Initialize()
        {
            if ( FastwalkPrevention )
                PacketHandlers.RegisterThrottler( 0x02, new ThrottlePacketCallback( MovementThrottle_Callback ) );

            EventSink.Login += new LoginEventHandler( OnLogin );
            EventSink.Logout += new LogoutEventHandler( OnLogout );
            EventSink.Connected += new ConnectedEventHandler( EventSink_Connected );
            EventSink.Disconnected += new DisconnectedEventHandler( EventSink_Disconnected );
        }

        public override void OnSkillInvalidated( Skill skill )
        {
            if ( Core.AOS && skill.SkillName == SkillName.MagicResist )
                UpdateResistances();
        }

        public override int GetMaxResistance( ResistanceType type )
        {
            if (AccessLevel > AccessLevel.Player)
                return int.MaxValue;

            int max = base.GetMaxResistance( type );

            if ( type != ResistanceType.Physical && 60 < max && Spells.Fourth.CurseSpell.UnderEffect( this ) )
                max = 60;

            /*if( Core.ML && this.Race == Race.Elf && type == ResistanceType.Energy )
                max += 5; //Intended to go after the 60 max from curse*/

            return max;
        }

        protected override void OnRaceChange( Race oldRace )
        {
            ValidateEquipment();
            UpdateResistances();
        }

        // 29.06.2012 :: zombie :: wylaczenie bonusu dla rasy None
        // return ( ((Core.ML && this.Race == Race.None) ? 100 : 40) + (int)(3.5 * this.Str)); 
        public override int MaxWeight 
        { 
            get 
            {
                return (40 + (int)(3.5 * this.Str) + (int)WeigthBonus); 
            } 
        }
        // zombie

        private int m_LastGlobalLight = -1, m_LastPersonalLight = -1;

        public override void OnNetStateChanged()
        {
            m_LastGlobalLight = -1;
            m_LastPersonalLight = -1;
        }

        public override void ComputeBaseLightLevels( out int global, out int personal )
        {
            global = LightCycle.ComputeLevelFor( this );

            //if ( this.LightLevel < NightSightPotion.NightSightBonus && AosAttributes.GetValue( this, AosAttribute.NightSight ) > 0)
            //    personal = NightSightPotion.NightSightBonus;
            //else
            //    personal = this.LightLevel;

            // bazowo wez bonus z mikstur/czarow:
            personal = this.LightLevel;

            // dodaj do bazy bonus z ubrania:
            if (AosAttributes.GetValue(this, AosAttribute.NightSight) > 0)
                personal += NightSightPotion.NightSightBonus;
        }

        public override void ComputeLightLevels(out int global, out int personal)
        {
            // apply region rules:
            base.ComputeLightLevels(out global, out personal);

            // apply race rules:
            if (Race.Equals(Drow.Instance))
            {
                // invert light for dark elf:
                global = LightCycle.DungeonLevel - global;
                if (global < 0)
                    global = 0;
            }
        }

        public override void CheckLightLevels( bool forceResend )
        {
            NetState ns = this.NetState;

            if ( ns == null )
                return;

            int global, personal;

            ComputeLightLevels( out global, out personal );

            if ( !forceResend )
                forceResend = ( global != m_LastGlobalLight || personal != m_LastPersonalLight );

            if ( !forceResend )
                return;

            m_LastGlobalLight = global;
            m_LastPersonalLight = personal;

            ns.Send( GlobalLightLevel.Instantiate( global ) );
            ns.Send( new PersonalLightLevel( this, personal ) );
        }

        public override int GetMinResistance( ResistanceType type )
        {
            int magicResist = (int)(Skills[SkillName.MagicResist].Value * 10);
            int min = int.MinValue;

            if ( magicResist >= 1000 )
                min = 40 + ((magicResist - 1000) / 50);
            else if ( magicResist >= 400 )
                min = (magicResist - 400) / 15;

            if ( min > MaxPlayerResistance )
                min = MaxPlayerResistance;

            int baseMin = base.GetMinResistance( type );

            if ( min < baseMin )
                min = baseMin;

            return min;
        }

        private static void OnLogin( LoginEventArgs e )
        {
            Mobile from = e.Mobile;

            CheckAtrophies( from );

            if ( AccountHandler.LockdownLevel > AccessLevel.Player )
            {
                string notice;

                Accounting.Account acct = from.Account as Accounting.Account;

                if ( acct == null || !acct.HasAccess( from.NetState ) )
                {
                    if ( from.AccessLevel == AccessLevel.Player )
                        notice = "The server is currently under lockdown. No players are allowed to log in at this time.";
                    else
                        notice = "The server is currently under lockdown. You do not have sufficient access level to connect.";

                    Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), new TimerStateCallback( Disconnect ), from );
                }
                else if ( from.AccessLevel >= AccessLevel.Administrator )
                {
                    notice = "The server is currently under lockdown. As you are an administrator, you may change this from the [Admin gump.";
                }
                else
                {
                    notice = "The server is currently under lockdown. You have sufficient access level to connect.";
                }

                from.SendGump( new NoticeGump( 1060637, 30720, notice, 0xFFC000, 300, 140, null, null ) );
            }
        }

        private bool m_NoDeltaRecursion;

        public void ValidateEquipment()
        {
            if ( m_NoDeltaRecursion || Map == null || Map == Map.Internal )
                return;

            if ( this.Items == null )
                return;

            m_NoDeltaRecursion = true;
            Timer.DelayCall( TimeSpan.Zero, new TimerCallback( ValidateEquipment_Sandbox ) );
        }

        private void ValidateEquipment_Sandbox()
        {
            try
            {
                if ( Map == null || Map == Map.Internal )
                    return;

                List<Item> items = this.Items;

                if ( items == null )
                    return;

                bool moved = false;

                int str = this.Str;
                int dex = this.Dex;
                int intel = this.Int;

                #region Factions
                int factionItemCount = 0;
                #endregion

                Mobile from = this;

                #region Ethics
                Ethics.Ethic ethic = Ethics.Ethic.Find( from );
                #endregion

                for ( int i = items.Count - 1; i >= 0; --i )
                {
                    if ( i >= items.Count )
                        continue;

                    Item item = items[i];

                    #region Ethics
                    if ( ( item.SavedFlags & 0x100 ) != 0 )
                    {
                        if ( item.Hue != Ethics.Ethic.Hero.Definition.PrimaryHue )
                        {
                            item.SavedFlags &= ~0x100;
                        }
                        else if ( ethic != Ethics.Ethic.Hero )
                        {
                            from.AddToBackpack( item );
                            moved = true;
                            continue;
                        }
                    }
                    else if ( ( item.SavedFlags & 0x200 ) != 0 )
                    {
                        if ( item.Hue != Ethics.Ethic.Evil.Definition.PrimaryHue )
                        {
                            item.SavedFlags &= ~0x200;
                        }
                        else if ( ethic != Ethics.Ethic.Evil )
                        {
                            from.AddToBackpack( item );
                            moved = true;
                            continue;
                        }
                    }
                    #endregion

                    if ( item is BaseWeapon )
                    {
                        BaseWeapon weapon = (BaseWeapon)item;

                        bool drop = false;

                        if( dex < weapon.DexRequirement )
                            drop = true;
                        else if( str < AOS.Scale( weapon.StrRequirement, 100 - weapon.GetLowerStatReq() ) )
                            drop = true;
                        else if( intel < weapon.IntRequirement )
                            drop = true;
                        else if( weapon.RequiredRace != null && weapon.RequiredRace != this.Race )
                            drop = true;

                        if ( drop )
                        {
                            string name = weapon.Name;

                            if ( name == null )
                                name = String.Format( "#{0}", weapon.LabelNumber );

                            from.SendLocalizedMessage( 1062001, name ); // You can no longer wield your ~1_WEAPON~
                            from.AddToBackpack( weapon );
                            moved = true;
                        }
                    }
                    else if ( item is BaseArmor )
                    {
                        BaseArmor armor = (BaseArmor)item;

                        bool drop = false;

                        if ( !armor.AllowMaleWearer && !from.Female && from.AccessLevel < AccessLevel.GameMaster )
                        {
                            drop = true;
                        }
                        else if ( !armor.AllowFemaleWearer && from.Female && from.AccessLevel < AccessLevel.GameMaster )
                        {
                            drop = true;
                        }
                        else if( armor.RequiredRace != null && armor.RequiredRace != this.Race )
                        {
                            drop = true;
                        }
                        else
                        {
                            int strBonus = armor.ComputeStatBonus( StatType.Str ), strReq = armor.ComputeStatReq( StatType.Str );
                            int dexBonus = armor.ComputeStatBonus( StatType.Dex ), dexReq = armor.ComputeStatReq( StatType.Dex );
                            int intBonus = armor.ComputeStatBonus( StatType.Int ), intReq = armor.ComputeStatReq( StatType.Int );

                            if( dex < dexReq || (dex + dexBonus) < 1 )
                                drop = true;
                            else if( str < strReq || (str + strBonus) < 1 )
                                drop = true;
                            else if( intel < intReq || (intel + intBonus) < 1 )
                                drop = true;
                        }

                        if ( drop )
                        {
                            string name = armor.Name;

                            if ( name == null )
                                name = String.Format( "#{0}", armor.LabelNumber );

                            if ( armor is BaseShield )
                                from.SendLocalizedMessage( 1062003, name ); // You can no longer equip your ~1_SHIELD~
                            else
                                from.SendLocalizedMessage( 1062002, name ); // You can no longer wear your ~1_ARMOR~

                            from.AddToBackpack( armor );
                            moved = true;
                        }
                    }
                    else if ( item is BaseClothing )
                    {
                        BaseClothing clothing = (BaseClothing)item;

                        bool drop = false;

                        if ( !clothing.AllowMaleWearer && !from.Female && from.AccessLevel < AccessLevel.GameMaster )
                        {
                            drop = true;
                        }
                        else if ( !clothing.AllowFemaleWearer && from.Female && from.AccessLevel < AccessLevel.GameMaster )
                        {
                            drop = true;
                        }
                        else if( clothing.RequiredRace != null && clothing.RequiredRace != this.Race )
                        {
                            drop = true;
                        }
                        else
                        {
                            int strBonus = clothing.ComputeStatBonus( StatType.Str );
                            int strReq = clothing.ComputeStatReq( StatType.Str );

                            if( str < strReq || (str + strBonus) < 1 )
                                drop = true;
                        }

                        if ( drop )
                        {
                            string name = clothing.Name;

                            if ( name == null )
                                name = String.Format( "#{0}", clothing.LabelNumber );

                            from.SendLocalizedMessage( 1062002, name ); // You can no longer wear your ~1_ARMOR~

                            from.AddToBackpack( clothing );
                            moved = true;
                        }
                    }

                    FactionItem factionItem = FactionItem.Find( item );

                    if ( factionItem != null )
                    {
                        bool drop = false;

                        Faction ourFaction = Faction.Find( this );

                        if ( ourFaction == null || ourFaction != factionItem.Faction )
                            drop = true;
                        else if ( ++factionItemCount > FactionItem.GetMaxWearables( this ) )
                            drop = true;

                        if ( drop )
                        {
                            from.AddToBackpack( item );
                            moved = true;
                        }
                    }
                }

                if ( moved )
                    from.SendLocalizedMessage( 500647 ); // Some equipment has been moved to your backpack.
            }
            catch ( Exception e )
            {
                Console.WriteLine( e );
            }
            finally
            {
                m_NoDeltaRecursion = false;
            }
        }

        public override void Delta( MobileDelta flag )
        {
            base.Delta( flag );

            if ( (flag & MobileDelta.Stat) != 0 )
                ValidateEquipment();

            if ( (flag & (MobileDelta.Name | MobileDelta.Hue)) != 0 )
                InvalidateMyRunUO();
        }

        private static void Disconnect( object state )
        {
            NetState ns = ((Mobile)state).NetState;

            if ( ns != null )
                ns.Dispose();
        }

        private static void OnLogout( LogoutEventArgs e )
        {
        }

        private static void EventSink_Connected( ConnectedEventArgs e )
        {
            PlayerMobile pm = e.Mobile as PlayerMobile;

            if ( pm != null )
            {
                pm.m_SessionStart = DateTime.Now;

                if ( pm.m_Quest != null )
                    pm.m_Quest.StartTimer();

                pm.BedrollLogout = false;
                pm.LastOnline = DateTime.Now;
            }

            Timer.DelayCall( TimeSpan.Zero, new TimerStateCallback( ClearSpecialMovesCallback ), e.Mobile );
        }

        private static void ClearSpecialMovesCallback( object state )
        {
            Mobile from = (Mobile)state;

            SpecialMove.ClearAllMoves( from );
        }

        private static void EventSink_Disconnected( DisconnectedEventArgs e )
        {
            Mobile from = e.Mobile;
            DesignContext context = DesignContext.Find( from );

            if ( context != null )
            {
                /* Client disconnected
                 *  - Remove design context
                 *  - Eject all from house
                 *  - Restore relocated entities
                 */

                // Remove design context
                DesignContext.Remove( from );

                // Eject all from house
                from.RevealingAction();

                foreach ( Item item in context.Foundation.GetItems() )
                    item.Location = context.Foundation.BanLocation;

                foreach ( Mobile mobile in context.Foundation.GetMobiles() )
                    mobile.Location = context.Foundation.BanLocation;

                // Restore relocated entities
                context.Foundation.RestoreRelocatedEntities();
            }

            PlayerMobile pm = e.Mobile as PlayerMobile;

            if ( pm != null )
            {
                pm.m_GameTime += (DateTime.Now - pm.m_SessionStart);

                if ( pm.m_Quest != null )
                    pm.m_Quest.StopTimer();

                pm.m_SpeechLog = null;
                pm.LastOnline = DateTime.Now;
            }
        }

        public override void RevealingAction()
        {
            if ( m_DesignContext != null )
                return;

            Spells.Sixth.InvisibilitySpell.RemoveTimer( this );

            base.RevealingAction();
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public override bool Hidden
        {
            get
            {
                return base.Hidden;
            }
            set
            {
                base.Hidden = value;

                RemoveBuff( BuffIcon.Invisibility );    //Always remove, default to the hiding icon EXCEPT in the invis spell where it's explicitly set

                if( !Hidden )
                {
                    RemoveBuff( BuffIcon.HidingAndOrStealth );
                }
                else// if( !InvisibilitySpell.HasTimer( this ) )
                {
                    BuffInfo.AddBuff( this, new BuffInfo( BuffIcon.HidingAndOrStealth, 1075655 ) );    //Hidden/Stealthing & You Are Hidden
                }
            }
        }

        public override void OnSubItemAdded( Item item )
        {
            if ( AccessLevel < AccessLevel.GameMaster && item.IsChildOf( this.Backpack ) )
            {
                int maxWeight = WeightOverloading.GetMaxWeight( this );
                int curWeight = Mobile.BodyWeight + this.TotalWeight;

                if ( curWeight > maxWeight )
                    this.SendLocalizedMessage( 1019035, true, String.Format( " : {0} / {1}", curWeight, maxWeight ) );
            }
        }

        public override bool CanBeHarmful( Mobile target, bool message, bool ignoreOurBlessedness )
        {
            if ( m_DesignContext != null || (target is PlayerMobile && ((PlayerMobile)target).m_DesignContext != null) )
                return false;

            if ( (target is BaseVendor && ((BaseVendor)target).IsInvulnerable) || target is PlayerVendor || target is TownCrier )
            {
                if ( message )
                {
                    if ( target.Title == null )
                        SendMessage( "{0} the vendor cannot be harmed.", target.Name );
                    else
                        SendMessage( "{0} {1} cannot be harmed.", target.Name, target.Title );
                }

                return false;
            }

            return base.CanBeHarmful( target, message, ignoreOurBlessedness );
        }

        public override bool CanBeBeneficial( Mobile target, bool message, bool allowDead )
        {
            if ( m_DesignContext != null || (target is PlayerMobile && ((PlayerMobile)target).m_DesignContext != null) )
                return false;

            return base.CanBeBeneficial( target, message, allowDead );
        }

        public override bool CheckContextMenuDisplay( IEntity target )
        {
            return ( m_DesignContext == null );
        }

        public override void OnItemAdded( Item item )
        {
            base.OnItemAdded( item );

            if ( item is BaseArmor || item is BaseWeapon )
            {
                Hits=Hits; Stam=Stam; Mana=Mana;
            }

            if ( this.NetState != null )
                CheckLightLevels( false );

            InvalidateMyRunUO();
        }

        public override void OnItemRemoved( Item item )
        {
            base.OnItemRemoved( item );

            if ( item is BaseArmor || item is BaseWeapon )
            {
                Hits=Hits; Stam=Stam; Mana=Mana;
            }

            if ( this.NetState != null )
                CheckLightLevels( false );

            InvalidateMyRunUO();
        }

        public override double ArmorRating
        {
            get
            {
                //BaseArmor ar;
                double rating = 0.0;

                AddArmorRating( ref rating, NeckArmor );
                AddArmorRating( ref rating, HandArmor );
                AddArmorRating( ref rating, HeadArmor );
                AddArmorRating( ref rating, ArmsArmor );
                AddArmorRating( ref rating, LegsArmor );
                AddArmorRating( ref rating, ChestArmor );
                AddArmorRating( ref rating, ShieldArmor );

                return VirtualArmor + VirtualArmorMod + rating;
            }
        }

        private void AddArmorRating( ref double rating, Item armor )
        {
            BaseArmor ar = armor as BaseArmor;

            if( ar != null && ( !Core.AOS || ar.ArmorAttributes.MageArmor == 0 ))
                rating += ar.ArmorRatingScaled;
        }

        #region Bonusy dla wojownika
        // Sprawdzenie czy player posiada wystarczajaca ilosc skilla by otrzymac bonus
        private double maxSkillBonusAmount()
        {
            double maxSkillAmount = 0;

            // Posiadanie niektorych skilli jest zabronione dla tego bonusa
            if (Skills.Magery.Base > 50 || Skills.Musicianship.Base > 50 || Skills.AnimalTaming.Base > 50 || Skills.Archery.Base > 50)
                return 0;

            maxSkillAmount = Skills.Fencing.Base > maxSkillAmount ? Skills.Fencing.Base : maxSkillAmount;
            maxSkillAmount = Skills.Swords.Base  > maxSkillAmount ? Skills.Swords.Base  : maxSkillAmount;
            maxSkillAmount = Skills.Macing.Base  > maxSkillAmount ? Skills.Macing.Base  : maxSkillAmount;

            maxSkillAmount = maxSkillAmount >= 70 ? maxSkillAmount : 0;

            return maxSkillAmount;
        }

        private int hitsMeleeSkillBonus()
        {
            double bonus = 0;
            double bonusSkill = maxSkillBonusAmount();
            if (bonusSkill > 0)
            {
                bonusSkill = bonusSkill > 100 ? 100 : bonusSkill;
                bonus = ((bonusSkill - 70) / 5 + 1) * 2;
                bonus = bonus > 14 ? 14 : bonus;
            }
            return (int)bonus;
        }

        public int hitsMeleeSkillRegenBonus()
        {
            double bonus = 0;
            double bonusSkill = maxSkillBonusAmount();
            if (bonusSkill >= 105)
                bonus++;
            if (bonusSkill >= 115)
                bonus++;
            return (int)bonus;
        }

        public int staminaMeleeSkillRegenBonus()
        {
            double bonus = 0;
            double bonusSkill = maxSkillBonusAmount();
            if (bonusSkill >= 100)
            {
                bonus = ((bonusSkill - 100) / 5 + 1);
                bonus = bonus > 5 ? 5 : bonus;
            }
            return (int)bonus;
        }

        public int manaMeleeSkillRegenBonus()
        {
            double bonus = 0;
            double bonusSkill = maxSkillBonusAmount();
            if (bonusSkill >= 110)
                bonus++;
            if (bonusSkill == 120)
                bonus++;
            return (int)bonus;
        }
        #endregion

        #region [Stats]Max
        [CommandProperty( AccessLevel.GameMaster )]
        public override int HitsMax
        {
            get
            {
                int strBase;
                int strOffs = GetStatOffset( StatType.Str );

                if ( Core.AOS )
                {
                    strBase = this.Str;    //this.Str already includes GetStatOffset/str
                    strOffs = AosAttributes.GetValue( this, AosAttribute.BonusHits );

                    if ( strOffs > 25 && AccessLevel <= AccessLevel.Player )
                        strOffs = 25;

                    if ( AnimalForm.UnderTransformation( this, typeof( BakeKitsune ) ) || AnimalForm.UnderTransformation( this, typeof( GreyWolf ) ) )
                        strOffs += 20;
                }
                else
                {
                    strBase = this.RawStr;
                }



                return (strBase / 2) + 50 + strOffs + hitsMeleeSkillBonus();
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public override int StamMax
        {
            get{ return base.StamMax + AosAttributes.GetValue( this, AosAttribute.BonusStam ); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public override int ManaMax
        {
            //get{ return base.ManaMax + AosAttributes.GetValue( this, AosAttribute.BonusMana ) + ((Core.ML && Race == Race.Elf) ? 20 : 0); }
            get{ return base.ManaMax + AosAttributes.GetValue( this, AosAttribute.BonusMana ); }
        }
        #endregion

        #region Stat Getters/Setters

        [CommandProperty( AccessLevel.GameMaster )]
        public override int Str
        {
            get
            {
                //if( Core.ML && this.AccessLevel == AccessLevel.Player )
                //    return Math.Min( base.Str, 150 );

                return base.Str;
            }
            set
            {
                base.Str = value;
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public override int Int
        {
            get
            {
                //if( Core.ML && this.AccessLevel == AccessLevel.Player )
                //    return Math.Min( base.Int, 150 );

                return base.Int;
            }
            set
            {
                base.Int = value;
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public override int Dex
        {
            get
            {
                //if( Core.ML && this.AccessLevel == AccessLevel.Player )
                //    return Math.Min( base.Dex, 150 );

                return base.Dex;
            }
            set
            {
                base.Dex = value;
            }
        }

        #endregion

        public override bool Move( Direction d )
        {
            NetState ns = this.NetState;

            if ( ns != null )
            {
                if ( HasGump( typeof( ResurrectGump ) ) ) {
                    if ( Alive ) {
                        CloseGump( typeof( ResurrectGump ) );
                    } else {
                        SendLocalizedMessage( 500111 ); // You are frozen and cannot move.
                        return false;
                    }
                }
            }

            TimeSpan speed = ComputeMovementSpeed( d );

            bool res;

            if ( !Alive )
                Server.Movement.MovementImpl.IgnoreMovableImpassables = true;

            res = base.Move( d );

            Server.Movement.MovementImpl.IgnoreMovableImpassables = false;

            if ( !res )
                return false;

            m_NextMovementTime += speed;

            return true;
        }

        public override bool CheckMovement( Direction d, out int newZ )
        {
            DesignContext context = m_DesignContext;

            if ( context == null )
                return base.CheckMovement( d, out newZ );

            HouseFoundation foundation = context.Foundation;

            newZ = foundation.Z + HouseFoundation.GetLevelZ( context.Level, context.Foundation );

            int newX = this.X, newY = this.Y;
            Movement.Movement.Offset( d, ref newX, ref newY );

            int startX = foundation.X + foundation.Components.Min.X + 1;
            int startY = foundation.Y + foundation.Components.Min.Y + 1;
            int endX = startX + foundation.Components.Width - 1;
            int endY = startY + foundation.Components.Height - 2;

            return ( newX >= startX && newY >= startY && newX < endX && newY < endY && Map == foundation.Map );
        }

        public override bool AllowItemUse( Item item )
        {
            return DesignContext.Check( this );
        }

        public SkillName[] AnimalFormRestrictedSkills{ get{ return m_AnimalFormRestrictedSkills; } }

        private SkillName[] m_AnimalFormRestrictedSkills = new SkillName[]
        {
            SkillName.ArmsLore,    SkillName.Begging, SkillName.Discordance, SkillName.Zielarstwo,
            SkillName.Inscribe, SkillName.ItemID, SkillName.Meditation, SkillName.Peacemaking,
            SkillName.Provocation, SkillName.RemoveTrap, SkillName.SpiritSpeak, SkillName.Stealing,    
            SkillName.TasteID
        };

        public override bool AllowSkillUse( SkillName skill )
        {
            if ( AnimalForm.UnderTransformation( this ) )
            {
                for( int i = 0; i < m_AnimalFormRestrictedSkills.Length; i++ )
                {
                    if( m_AnimalFormRestrictedSkills[i] == skill )
                    {
                        SendLocalizedMessage( 1070771 ); // You cannot use that skill in this form.
                        return false;
                    }
                }
            }

            return DesignContext.Check( this );
        }

        private bool m_LastProtectedMessage;
        private int m_NextProtectionCheck = 10;

        public virtual void RecheckTownProtection()
        {
            m_NextProtectionCheck = 10;

            Regions.GuardedRegion reg = (Regions.GuardedRegion) this.Region.GetRegion( typeof( Regions.GuardedRegion ) );
            bool isProtected = ( reg != null && !reg.IsDisabled() );

            if ( isProtected != m_LastProtectedMessage )
            {
                if ( isProtected )
                    SendLocalizedMessage( 500112 ); // You are now under the protection of the town guards.
                else
                    SendLocalizedMessage( 500113 ); // You have left the protection of the town guards.

                m_LastProtectedMessage = isProtected;
            }
        }

        public override void MoveToWorld( Point3D loc, Map map )
        {
            base.MoveToWorld( loc, map );

            RecheckTownProtection();
        }

        public override void SetLocation( Point3D loc, bool isTeleport )
        {
            if ( !isTeleport && AccessLevel == AccessLevel.Player )
            {
                // moving, not teleporting
                int zDrop = ( this.Location.Z - loc.Z );

                if ( zDrop > 20 ) // we fell more than one story
                    Hits -= ((zDrop / 20) * 10) - 5; // deal some damage; does not kill, disrupt, etc
            }

            base.SetLocation( loc, isTeleport );

            if ( isTeleport || --m_NextProtectionCheck == 0 )
                RecheckTownProtection();
        }

        public override void GetContextMenuEntries( Mobile from, List<ContextMenuEntry> list )
        {
            base.GetContextMenuEntries( from, list );

            if ( from == this )
            {
                if ( m_Quest != null )
                    m_Quest.GetContextMenuEntries( list );

                if ( Alive && InsuranceEnabled )
                {
                    list.Add( new CallbackEntry( 6201, new ContextCallback( ToggleItemInsurance ) ) );

                    if ( AutoRenewInsurance )
                        list.Add( new CallbackEntry( 6202, new ContextCallback( CancelRenewInventoryInsurance ) ) );
                    else
                        list.Add( new CallbackEntry( 6200, new ContextCallback( AutoRenewInventoryInsurance ) ) );
                }

                BaseHouse house = BaseHouse.FindHouseAt( this );

                if ( house != null )
                {
                    if ( Alive && house.InternalizedVendors.Count > 0 && house.IsOwner( this ) )
                        list.Add( new CallbackEntry( 6204, new ContextCallback( GetVendor ) ) );

                    if ( house.IsAosRules )
                        list.Add( new CallbackEntry( 6207, new ContextCallback( LeaveHouse ) ) );
                }

                if ( m_JusticeProtectors.Count > 0 )
                    list.Add( new CallbackEntry( 6157, new ContextCallback( CancelProtection ) ) );

                if( Alive )
                    list.Add( new CallbackEntry( 6210, new ContextCallback( ToggleChampionTitleDisplay ) ) );
            }
        }

        private void CancelProtection()
        {
            for ( int i = 0; i < m_JusticeProtectors.Count; ++i )
            {
                Mobile prot = m_JusticeProtectors[i];

                string args = String.Format( "{0}\t{1}", this.Name, prot.Name );

                prot.SendLocalizedMessage( 1049371, args ); // The protective relationship between ~1_PLAYER1~ and ~2_PLAYER2~ has been ended.
                this.SendLocalizedMessage( 1049371, args ); // The protective relationship between ~1_PLAYER1~ and ~2_PLAYER2~ has been ended.
            }

            m_JusticeProtectors.Clear();
        }

        #region Insurance

        private void ToggleItemInsurance()
        {
            if ( !CheckAlive() )
                return;

            BeginTarget( -1, false, TargetFlags.None, new TargetCallback( ToggleItemInsurance_Callback ) );
            SendLocalizedMessage( 1060868 ); // Target the item you wish to toggle insurance status on <ESC> to cancel
        }

        private bool CanInsure( Item item )
        {
            // 13.07.2012 :: zombie :: dodanie kolczanow
            if ( (( item is Container ) && !(item is BaseQuiver)) || item is BagOfSending || item is KeyRing )
                return false;
            // zombie

            if ( (item is Spellbook && item.LootType == LootType.Blessed)|| item is Runebook || item is PotionKeg || item is Sigil )
                return false;

            if ( item.Stackable )
                return false;

            if ( item.LootType == LootType.Cursed )
                return false;

            if ( item.ItemID == 0x204E ) // death shroud
                return false;

            return true;
        }

        private void ToggleItemInsurance_Callback( Mobile from, object obj )
        {
            if ( !CheckAlive() )
                return;

            Item item = obj as Item;

            if ( item == null || !item.IsChildOf( this ) )
            {
                BeginTarget( -1, false, TargetFlags.None, new TargetCallback( ToggleItemInsurance_Callback ) );
                SendLocalizedMessage( 1060871, "", 0x23 ); // You can only insure items that you have equipped or that are in your backpack
            }
            else if ( item.Insured )
            {
                item.Insured = false;

                SendLocalizedMessage( 1060874, "", 0x35 ); // You cancel the insurance on the item

                BeginTarget( -1, false, TargetFlags.None, new TargetCallback( ToggleItemInsurance_Callback ) );
                SendLocalizedMessage( 1060868, "", 0x23 ); // Target the item you wish to toggle insurance status on <ESC> to cancel
            }
            else if ( !CanInsure( item ) )
            {
                BeginTarget( -1, false, TargetFlags.None, new TargetCallback( ToggleItemInsurance_Callback ) );
                SendLocalizedMessage( 1060869, "", 0x23 ); // You cannot insure that
            }
            else if ( item.LootType == LootType.Blessed || item.LootType == LootType.Newbied || item.BlessedFor == from )
            {
                BeginTarget( -1, false, TargetFlags.None, new TargetCallback( ToggleItemInsurance_Callback ) );
                SendLocalizedMessage( 1060870, "", 0x23 ); // That item is blessed and does not need to be insured
                SendLocalizedMessage( 1060869, "", 0x23 ); // You cannot insure that
            }
            else
            {
                if ( !item.PayedInsurance )
                {
                    if ( Banker.Withdraw( from, 600 ) )
                    {
                        SendLocalizedMessage( 1060398, "600" ); // ~1_AMOUNT~ gold has been withdrawn from your bank box.
                        item.PayedInsurance = true;
                    }
                    else
                    {
                        SendLocalizedMessage( 1061079, "", 0x23 ); // You lack the funds to purchase the insurance
                        return;
                    }
                }

                item.Insured = true;

                SendLocalizedMessage( 1060873, "", 0x23 ); // You have insured the item

                BeginTarget( -1, false, TargetFlags.None, new TargetCallback( ToggleItemInsurance_Callback ) );
                SendLocalizedMessage( 1060868, "", 0x23 ); // Target the item you wish to toggle insurance status on <ESC> to cancel
            }
        }

        private void AutoRenewInventoryInsurance()
        {
            if ( !CheckAlive() )
                return;

            SendLocalizedMessage( 1060881, "", 0x23 ); // You have selected to automatically reinsure all insured items upon death
            AutoRenewInsurance = true;
        }

        private void CancelRenewInventoryInsurance()
        {
            if ( !CheckAlive() )
                return;

            if( Core.SE )
            {
                if( !HasGump( typeof( CancelRenewInventoryInsuranceGump ) ) )
                    SendGump( new CancelRenewInventoryInsuranceGump( this ) );
            }
            else
            {
                SendLocalizedMessage( 1061075, "", 0x23 ); // You have cancelled automatically reinsuring all insured items upon death
                AutoRenewInsurance = false;
            }
        }

        private class CancelRenewInventoryInsuranceGump : Gump
        {
            private PlayerMobile m_Player;

            public CancelRenewInventoryInsuranceGump( PlayerMobile player ) : base( 250, 200 )
            {
                m_Player = player;

                AddBackground( 0, 0, 240, 142, 0x13BE );
                AddImageTiled( 6, 6, 228, 100, 0xA40 );
                AddImageTiled( 6, 116, 228, 20, 0xA40 );
                AddAlphaRegion( 6, 6, 228, 142 );

                AddHtmlLocalized( 8, 8, 228, 100, 1071021, 0x7FFF, false, false ); // You are about to disable inventory insurance auto-renewal.

                AddButton( 6, 116, 0xFB1, 0xFB2, 0, GumpButtonType.Reply, 0 );
                AddHtmlLocalized( 40, 118, 450, 20, 1060051, 0x7FFF, false, false ); // CANCEL

                AddButton( 114, 116, 0xFA5, 0xFA7, 1, GumpButtonType.Reply, 0 );
                AddHtmlLocalized( 148, 118, 450, 20, 1071022, 0x7FFF, false, false ); // DISABLE IT!
            }

            public override void OnResponse( NetState sender, RelayInfo info )
            {
                if ( !m_Player.CheckAlive() )
                    return;

                if ( info.ButtonID == 1 )
                {
                    m_Player.SendLocalizedMessage( 1061075, "", 0x23 ); // You have cancelled automatically reinsuring all insured items upon death
                    m_Player.AutoRenewInsurance = false;
                }
                else
                {
                    m_Player.SendLocalizedMessage( 1042021 ); // Cancelled.
                }
            }
        }
        #endregion

        private void GetVendor()
        {
            BaseHouse house = BaseHouse.FindHouseAt( this );

            if ( CheckAlive() && house != null && house.IsOwner( this ) && house.InternalizedVendors.Count > 0 )
            {
                CloseGump( typeof( ReclaimVendorGump ) );
                SendGump( new ReclaimVendorGump( house ) );
            }
        }

        private void LeaveHouse()
        {
            BaseHouse house = BaseHouse.FindHouseAt( this );

            if ( house != null )
                this.Location = house.BanLocation;
        }

        private delegate void ContextCallback();

        private class CallbackEntry : ContextMenuEntry
        {
            private ContextCallback m_Callback;

            public CallbackEntry( int number, ContextCallback callback ) : this( number, -1, callback )
            {
            }

            public CallbackEntry( int number, int range, ContextCallback callback ) : base( number, range )
            {
                m_Callback = callback;
            }

            public override void OnClick()
            {
                if ( m_Callback != null )
                    m_Callback();
            }
        }

        public override void DisruptiveAction()
        {
            if( Meditating )
            {
                RemoveBuff( BuffIcon.ActiveMeditation );
            }

            base.DisruptiveAction();
        }
        public override void OnDoubleClick( Mobile from )
        {
            if ( this == from && !Warmode )
            {
                IMount mount = Mount;

                if ( mount != null && !DesignContext.Check( this ) )
                    return;
            }

            base.OnDoubleClick( from );
        }

        public override void DisplayPaperdollTo( Mobile to )
        {
            if ( DesignContext.Check( this ) )
                base.DisplayPaperdollTo( to );
        }

        private static bool m_NoRecursion;

        public override bool CheckEquip( Item item )
        {
            if ( !base.CheckEquip( item ) )
                return false;

            #region Factions
            FactionItem factionItem = FactionItem.Find( item );

            if ( factionItem != null )
            {
                Faction faction = Faction.Find( this );

                if ( faction == null )
                {
                    SendLocalizedMessage( 1010371 ); // You cannot equip a faction item!
                    return false;
                }
                else if ( faction != factionItem.Faction )
                {
                    SendLocalizedMessage( 1010372 ); // You cannot equip an opposing faction's item!
                    return false;
                }
                else
                {
                    int maxWearables = FactionItem.GetMaxWearables( this );

                    for ( int i = 0; i < Items.Count; ++i )
                    {
                        Item equiped = Items[i];

                        if ( item != equiped && FactionItem.Find( equiped ) != null )
                        {
                            if ( --maxWearables == 0 )
                            {
                                SendLocalizedMessage( 1010373 ); // You do not have enough rank to equip more faction items!
                                return false;
                            }
                        }
                    }
                }
            }
            #endregion

            if ( this.AccessLevel < AccessLevel.GameMaster && item.Layer != Layer.Mount && this.HasTrade )
            {
                BounceInfo bounce = item.GetBounce();

                if ( bounce != null )
                {
                    if ( bounce.m_Parent is Item )
                    {
                        Item parent = (Item) bounce.m_Parent;

                        if ( parent == this.Backpack || parent.IsChildOf( this.Backpack ) )
                            return true;
                    }
                    else if ( bounce.m_Parent == this )
                    {
                        return true;
                    }
                }

                SendLocalizedMessage( 1004042 ); // You can only equip what you are already carrying while you have a trade pending.
                return false;
            }

            return true;
        }

        public override bool CheckTrade( Mobile to, Item item, SecureTradeContainer cont, bool message, bool checkItems, int plusItems, int plusWeight )
        {
            int msgNum = 0;

            if ( cont == null )
            {
                if ( to.Holding != null )
                    msgNum = 1062727; // You cannot trade with someone who is dragging something.
                else if ( this.HasTrade )
                    msgNum = 1062781; // You are already trading with someone else!
                else if ( to.HasTrade )
                    msgNum = 1062779; // That person is already involved in a trade
            }

            if ( msgNum == 0 )
            {
                if ( cont != null )
                {
                    plusItems += cont.TotalItems;
                    plusWeight += cont.TotalWeight;
                }

                if ( this.Backpack == null || !this.Backpack.CheckHold( this, item, false, checkItems, plusItems, plusWeight ) )
                    msgNum = 1004040; // You would not be able to hold this if the trade failed.
                else if ( to.Backpack == null || !to.Backpack.CheckHold( to, item, false, checkItems, plusItems, plusWeight ) )
                    msgNum = 1004039; // The recipient of this trade would not be able to carry this.
                else
                    msgNum = CheckContentForTrade( item );
            }

            if ( msgNum != 0 )
            {
                if ( message )
                    this.SendLocalizedMessage( msgNum );

                return false;
            }

            return true;
        }

        private static int CheckContentForTrade( Item item )
        {
            if ( item is TrapableContainer && ((TrapableContainer)item).TrapType != TrapType.None )
                return 1004044; // You may not trade trapped items.

            if ( SkillHandlers.StolenItem.IsStolen( item ) )
                return 1004043; // You may not trade recently stolen items.

            if ( item is Container )
            {
                foreach ( Item subItem in item.Items )
                {
                    int msg = CheckContentForTrade( subItem );

                    if ( msg != 0 )
                        return msg;
                }
            }

            return 0;
        }

        public override bool CheckNonlocalDrop( Mobile from, Item item, Item target )
        {
            if ( !base.CheckNonlocalDrop( from, item, target ) )
                return false;

            if ( from.AccessLevel >= AccessLevel.GameMaster )
                return true;

            Container pack = this.Backpack;
            if ( from == this && this.HasTrade && ( target == pack || target.IsChildOf( pack ) ) )
            {
                BounceInfo bounce = item.GetBounce();

                if ( bounce != null && bounce.m_Parent is Item )
                {
                    Item parent = (Item) bounce.m_Parent;

                    if ( parent == pack || parent.IsChildOf( pack ) )
                        return true;
                }

                SendLocalizedMessage( 1004041 ); // You can't do that while you have a trade pending.
                return false;
            }

            return true;
        }

        protected override void OnLocationChange( Point3D oldLocation )
        {
            CheckLightLevels( false );

            DesignContext context = m_DesignContext;

            if ( context == null || m_NoRecursion )
                return;

            m_NoRecursion = true;

            HouseFoundation foundation = context.Foundation;

            int newX = this.X, newY = this.Y;
            int newZ = foundation.Z + HouseFoundation.GetLevelZ( context.Level, context.Foundation );

            int startX = foundation.X + foundation.Components.Min.X + 1;
            int startY = foundation.Y + foundation.Components.Min.Y + 1;
            int endX = startX + foundation.Components.Width - 1;
            int endY = startY + foundation.Components.Height - 2;

            if ( newX >= startX && newY >= startY && newX < endX && newY < endY && Map == foundation.Map )
            {
                if ( Z != newZ )
                    Location = new Point3D( X, Y, newZ );

                m_NoRecursion = false;
                return;
            }

            Location = new Point3D( foundation.X, foundation.Y, newZ );
            Map = foundation.Map;

            m_NoRecursion = false;
        }

        public override bool OnMoveOver( Mobile m )
        {
            if ( m is BaseCreature && !((BaseCreature)m).Controlled )
                return false;

            return base.OnMoveOver( m );
        }

        public override bool CheckShove( Mobile shoved )
        {
            if( TransformationSpellHelper.UnderTransformation( this, typeof( WraithFormSpell ) ) )
                return true;
            else
                return base.CheckShove( shoved );
        }


        protected override void OnMapChange( Map oldMap )
        {
            if ( (Map != Faction.Facet && oldMap == Faction.Facet) || (Map == Faction.Facet && oldMap != Faction.Facet) )
                InvalidateProperties();

            DesignContext context = m_DesignContext;

            if ( context == null || m_NoRecursion )
                return;

            m_NoRecursion = true;

            HouseFoundation foundation = context.Foundation;

            if ( Map != foundation.Map )
                Map = foundation.Map;

            m_NoRecursion = false;
        }

        public override void OnBeneficialAction( Mobile target, bool isCriminal )
        {
            if ( m_SentHonorContext != null )
                m_SentHonorContext.OnSourceBeneficialAction( target );

            base.OnBeneficialAction( target, isCriminal );
        }

        public override void OnDamage( int amount, Mobile from, bool willKill )
        {
            int disruptThreshold;

            if ( !Core.AOS )
                disruptThreshold = 0;
            else if ( from != null && from.Player )
                disruptThreshold = 18;
            else
                disruptThreshold = 25;

            if ( amount > disruptThreshold )
            {
                BandageContext c = BandageContext.GetContext( this );

                if ( c != null )
                    c.Slip();
            }

            if( Confidence.IsRegenerating( this ) )
                Confidence.StopRegenerating( this );

            WeightOverloading.FatigueOnDamage( this, amount );

            if ( m_ReceivedHonorContext != null )
                m_ReceivedHonorContext.OnTargetDamaged( from, amount );
            if ( m_SentHonorContext != null )
                m_SentHonorContext.OnSourceDamaged( from, amount );

            base.OnDamage( amount, from, willKill );
        }

        public override void Resurrect()
        {
            bool wasAlive = this.Alive;

            base.Resurrect();

            if ( this.Alive && !wasAlive )
            {
                Item deathRobe = new DeathRobe();

                if ( !EquipItem( deathRobe ) )
                    deathRobe.Delete();
   
            }
            
            Blessed = true;

            m_ResTimer = new ResurrectProtectionTimer(this);
            m_ResTimer.Start();
        }
       
        private ResurrectProtectionTimer m_ResTimer;

        private class ResurrectProtectionTimer : Timer
		{
			private PlayerMobile m_Player;

			public ResurrectProtectionTimer( PlayerMobile player ) : base( TimeSpan.FromSeconds( 5.0 ))
			{
                m_Player = player;
			}

			protected override void OnTick()
			{
                m_Player.Blessed = false;
			}
		}

        public override double RacialSkillBonus
        {
            get
            {
                // 05.07.2012 :: zombie :: wylaczenie bonusu dla ludzi
                //if( Core.ML && this.Race == Race.None )
                    //return 20.0;
                // zombie 

                return 0;
            }
        }

        private Mobile m_InsuranceAward;
        private int m_InsuranceCost;
        private int m_InsuranceBonus;

        private List<Item> m_EquipSnapshot;

        public List<Item> EquipSnapshot
        {
            get { return m_EquipSnapshot; }
        }

        private bool FindItems_Callback(Item item)
        {
            if (!item.Deleted && (item.LootType == LootType.Blessed || item.Insured))
            {
                if (this.Backpack != item.ParentEntity)
                {
                    return true;
                }
            }
            return false;
        }
 
        public override bool OnBeforeDeath()
        {
            NetState state = NetState;

            if ( state != null )
                state.CancelAllTrades();

            DropHolding();

            if (Backpack != null && !Backpack.Deleted)
            {
                List<Item> ilist = Backpack.FindItemsByType<Item>(FindItems_Callback);

                for (int i = 0; i < ilist.Count; i++)
                {
                    Backpack.AddItem(ilist[i]);
                }
            }

            m_EquipSnapshot = new List<Item>( this.Items );
            
            m_NonAutoreinsuredItems = 0;
            m_InsuranceCost = 0;
            m_InsuranceAward = base.FindMostRecentDamager( false );

            if ( m_InsuranceAward is BaseCreature )
            {
                Mobile master = ((BaseCreature)m_InsuranceAward).GetMaster();

                if ( master != null )
                    m_InsuranceAward = master;
            }

            if ( m_InsuranceAward != null && (!m_InsuranceAward.Player || m_InsuranceAward == this) )
                m_InsuranceAward = null;

            if ( m_InsuranceAward is PlayerMobile )
                ((PlayerMobile)m_InsuranceAward).m_InsuranceBonus = 0;

            if ( m_ReceivedHonorContext != null )
                m_ReceivedHonorContext.OnTargetKilled();
            if ( m_SentHonorContext != null )
                m_SentHonorContext.OnSourceKilled();

            // possessing
            if (m_PossessStorageMob != null)
            {
                Possess.MoveItems(this, m_PossessMob);
                m_PossessMob.Location = Location;
                m_PossessMob.Direction = Direction;
                m_PossessMob.Map = Map;
                m_PossessMob.Frozen = false;

                Possess.CopyProps(m_PossessStorageMob, this);
                Possess.CopySkills(m_PossessStorageMob, this);
                Possess.MoveItems(m_PossessStorageMob, this);
                m_PossessStorageMob.Delete();
                m_PossessMob.Kill();
                m_PossessMob = null;
                m_PossessStorageMob = null;
                Hidden = true;
                return false;
            }
            else
                return base.OnBeforeDeath();
            // END possessing
        }

        private bool CheckInsuranceOnDeath( Item item )
        {
            if ( InsuranceEnabled && item.Insured )
            {
                // XmlPoints mod to support overriding insurance fees/awards during challenge games
                if(XmlPoints.InsuranceIsFree(this, m_InsuranceAward))
                {
                    item.PayedInsurance = true;
                    return true;
                }
                                
                if ( AutoRenewInsurance )
                {
                    int cost = ( m_InsuranceAward == null ? 600 : 300 );

                    if ( Banker.Withdraw( this, cost ) )
                    {
                        m_InsuranceCost += cost;
                        item.PayedInsurance = true;
                    }
                    else
                    {
                        SendLocalizedMessage( 1061079, "", 0x23 ); // You lack the funds to purchase the insurance
                        item.PayedInsurance = false;
                        item.Insured = false;
                        m_NonAutoreinsuredItems++;
                    }
                }
                else
                {
                    item.PayedInsurance = false;
                    item.Insured = false;
                }

                if ( m_InsuranceAward != null )
                {
                    if ( Banker.Deposit( m_InsuranceAward, 300 ) )
                    {
                        if ( m_InsuranceAward is PlayerMobile )
                            ((PlayerMobile)m_InsuranceAward).m_InsuranceBonus += 300;
                    }
                }

                return true;
            }

            return false;
        }

        public override DeathMoveResult GetParentMoveResultFor( Item item )
        {
            if ( CheckInsuranceOnDeath( item ) )
                return DeathMoveResult.MoveToBackpack;

            DeathMoveResult res = base.GetParentMoveResultFor( item );

            if ( res == DeathMoveResult.MoveToCorpse && item.Movable && this.Young )
                res = DeathMoveResult.MoveToBackpack;

            return res;
        }

        public override DeathMoveResult GetInventoryMoveResultFor( Item item )
        {
            if ( CheckInsuranceOnDeath( item ) )
                return DeathMoveResult.MoveToBackpack;

            DeathMoveResult res = base.GetInventoryMoveResultFor( item );

            if ( res == DeathMoveResult.MoveToCorpse && item.Movable && this.Young )
                res = DeathMoveResult.MoveToBackpack;

            return res;
        }

        public override void OnDeath( Container c )
        {
            if (m_NonAutoreinsuredItems > 0)
            {
                SendLocalizedMessage(1061115);
            }

            base.OnDeath(c);

            m_EquipSnapshot = null;
            
            HueMod = -1;
            NameMod = null;
            SavagePaintExpiration = TimeSpan.Zero;

            SetHairMods( -1, -1 );
            RaceMod = null;
            HueDisguise = -1;

            PolymorphSpell.StopTimer( this );
            IncognitoSpell.StopTimer( this );
            DisguiseGump.StopTimer( this );
            RaceDisguiseGump.StopTimer(this);

            EndAction( typeof( PolymorphSpell ) );
            EndAction( typeof( IncognitoSpell ) );

            MeerMage.StopEffect( this, false );

            SkillHandlers.StolenItem.ReturnOnDeath( this, c );

            // czyszczenie statusu CBA nadanego przez okradanie:
            if ( m_PermaFlags.Count > 0 )
            {
                // Unikniemy reskilla.
                // Trzeba zgrabic zwloki zanim zlodziej sie zbierze.
                m_PermaFlags.Clear();

                if (false)  
                {
                    // nie nalezy udostepniac wszystkim zwlok, gdyz CBA zlodziej dostaje tylko okradajac reda/crima
                    if (c is Corpse)
                        ((Corpse)c).Criminal = true;

                    if (SkillHandlers.Stealing.ClassicMode)
                        Criminal = true;
                }
            }

            if ( this.Kills >= 5 && DateTime.Now >= m_NextJustAward )
            {
                Mobile m = FindMostRecentDamager( false );

                if( m is BaseCreature )
                    m = ((BaseCreature)m).GetMaster();

                if ( m != null && m is PlayerMobile && m != this )
                {
                    bool gainedPath = false;

                    int pointsToGain = 0;

                    pointsToGain += (int) Math.Sqrt( this.GameTime.TotalSeconds * 4 );
                    pointsToGain *= 5;
                    pointsToGain += (int) Math.Pow( this.Skills.Total / 250, 2 );

                    if ( VirtueHelper.Award( m, VirtueName.Justice, pointsToGain, ref gainedPath ) )
                    {
                        if ( gainedPath )
                            m.SendLocalizedMessage( 1049367 ); // You have gained a path in Justice!
                        else
                            m.SendLocalizedMessage( 1049363 ); // You have gained in Justice.

                        m.FixedParticles( 0x375A, 9, 20, 5027, EffectLayer.Waist );
                        m.PlaySound( 0x1F7 );

                        m_NextJustAward = DateTime.Now + TimeSpan.FromMinutes( pointsToGain / 3 );
                    }
                }
            }

            if ( m_InsuranceCost > 0 )
                SendLocalizedMessage( 1060398, m_InsuranceCost.ToString() ); // ~1_AMOUNT~ gold has been withdrawn from your bank box.

            if ( m_InsuranceAward is PlayerMobile )
            {
                PlayerMobile pm = (PlayerMobile)m_InsuranceAward;

                if ( pm.m_InsuranceBonus > 0 )
                    pm.SendLocalizedMessage( 1060397, pm.m_InsuranceBonus.ToString() ); // ~1_AMOUNT~ gold has been deposited into your bank box.
            }

            Mobile killer = this.FindMostRecentDamager( true );

            if ( killer is BaseCreature )
            {
                BaseCreature bc = (BaseCreature)killer;

                Mobile master = bc.GetMaster();
                if( master != null )
                    killer = master;
            }

            if ( this.Young )
            {
                if ( YoungDeathTeleport() )
                    Timer.DelayCall( TimeSpan.FromSeconds( 2.5 ), new TimerCallback( SendYoungDeathNotice ) );
            }

            Faction.HandleDeath( this, killer );

            Server.Guilds.Guild.HandleDeath( this, killer );

            if( m_BuffTable != null )
            {
                List<BuffInfo> list = new List<BuffInfo>();

                foreach( BuffInfo buff in m_BuffTable.Values )
                {
                    if( !buff.RetainThroughDeath )
                    {
                        list.Add( buff );
                    }
                }

                for( int i = 0; i < list.Count; i++ )
                {
                    RemoveBuff( list[i] );
                }
            }

            // 10.07.2012 :: zombie
            if ( m_PassiveDetectHiddenTimer != null )
                m_PassiveDetectHiddenTimer.Stop();
            // zombie
        }

        private List<Mobile> m_PermaFlags;
        private List<Mobile> m_VisList;
        private Hashtable m_AntiMacroTable;
        private TimeSpan m_GameTime;
        private TimeSpan m_ShortTermElapse;
        private TimeSpan m_LongTermElapse;
        private DateTime m_SessionStart;
        private DateTime m_LastEscortTime;
        private DateTime m_LastPetBallTime;
        private DateTime m_NextSmithBulkOrder;
        private DateTime m_NextTailorBulkOrder;
        private DateTime m_NextHunterBulkOrder;
        private DateTime m_NextBowFletcherBulkOrder;
        private DateTime m_SavagePaintExpiration;
        private SkillName m_Learning = (SkillName)(-1);

        // 16.08.2012 :: zombie
        private DateTime m_LastMacroCheck;
        
	    public DateTime LastMacroCheck
		{
			get{ return m_LastMacroCheck; }
			set{ m_LastMacroCheck = value; }
		}
        // zombie

        public SkillName Learning
        {
            get{ return m_Learning; }
            set{ m_Learning = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public TimeSpan SavagePaintExpiration
        {
            get
            {
                TimeSpan ts = m_SavagePaintExpiration - DateTime.Now;

                if ( ts < TimeSpan.Zero )
                    ts = TimeSpan.Zero;

                return ts;
            }
            set
            {
                m_SavagePaintExpiration = DateTime.Now + value;
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public TimeSpan NextSmithBulkOrder
        {
            get
            {
                TimeSpan ts = m_NextSmithBulkOrder - DateTime.Now;

                if ( ts < TimeSpan.Zero )
                    ts = TimeSpan.Zero;

                return ts;
            }
            set
            {
                try{ m_NextSmithBulkOrder = DateTime.Now + value; }
                catch{}
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public TimeSpan NextTailorBulkOrder
        {
            get
            {
                TimeSpan ts = m_NextTailorBulkOrder - DateTime.Now;

                if ( ts < TimeSpan.Zero )
                    ts = TimeSpan.Zero;

                return ts;
            }
            set
            {
                try{ m_NextTailorBulkOrder = DateTime.Now + value; }
                catch{}
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public TimeSpan NextHunterBulkOrder
        {
            get
            {
                TimeSpan ts = m_NextHunterBulkOrder - DateTime.Now;

                if ( ts < TimeSpan.Zero )
                    ts = TimeSpan.Zero;

                return ts;
            }
            set
            {
                try{ m_NextHunterBulkOrder = DateTime.Now + value; }
                catch{}
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan NextBowFletcherBulkOrder {
            get {
                TimeSpan ts = m_NextBowFletcherBulkOrder - DateTime.Now;

                if (ts < TimeSpan.Zero)
                    ts = TimeSpan.Zero;

                return ts;
            }
            set {
                try { m_NextBowFletcherBulkOrder = DateTime.Now + value; } catch { }
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public DateTime LastEscortTime
        {
            get{ return m_LastEscortTime; }
            set{ m_LastEscortTime = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public DateTime LastPetBallTime
        {
            get { return m_LastPetBallTime; }
            set { m_LastPetBallTime = value; }
        }

        // 25.06.2012 :: zombie 
        //private DateTime m_LastMOTD;

        //[CommandProperty( AccessLevel.GameMaster )]
        //public DateTime LastMOTD
        //{
        //    get { return m_LastMOTD; }
        //    set { m_LastMOTD = value; }
        //}
        // zombie

        #region SUS
        // 16.08.2012 :: zombie :: sledzenia poczynan gracza (przeniesione)

        private Hashtable m_Seers;

        public Hashtable Seers
        {
            get
            {
                return m_Seers;
            }
        }

        public void StopTracking( Mobile seer )
        {
            if ( m_Seers == null )
                return;
                
            if ( m_Seers.Contains( seer ) )
            {
                m_Seers.Remove( seer );
            }
            
            if ( m_Seers.Keys.Count == 0 )
                m_Seers = null;
        }

        public void StartTracking( Mobile seer, DateTime end )
        {
            if ( m_Seers == null )
                m_Seers = new Hashtable();
            else
            {
                // Cleanup
                
                ArrayList toDelete = new ArrayList();
                
                foreach ( Mobile m in m_Seers.Keys )
                {
                    if ( ( DateTime ) m_Seers[ m ] < DateTime.Now )
                        toDelete.Add( m );
                }
                
                foreach ( Mobile m in toDelete )
                {
                    m_Seers.Remove( m );
                }
            }
            
            if ( m_Seers.Contains( seer ) )
            {
                m_Seers[ seer ] = end;
            }
            else
            {
                m_Seers.Add( seer, end );
            }
        }

        // zombie
        #endregion

        public PlayerMobile()
        {
            m_VisList = new List<Mobile>();
            m_PermaFlags = new List<Mobile>();
            m_AntiMacroTable = new Hashtable();

            m_BOBFilter = new Engines.BulkOrders.BOBFilter();

            m_GameTime = TimeSpan.Zero;
            m_ShortTermElapse = TimeSpan.FromHours( 8.0 );
            m_LongTermElapse = TimeSpan.FromHours( 40.0 );

            m_JusticeProtectors = new List<Mobile>();
            m_GuildRank = Guilds.RankDefinition.Lowest;

            m_ChampionTitles = new ChampionTitleInfo();

            // 25.06.2012 :: zombie 
            //m_LastMOTD = DateTime.MinValue;
            // zombie

            // 22.09.2012 :: zombie
            m_RacialKills = new int[ Race.AllRaces.Count ];
            // zombie

            AutoRenewInsurance = true;


            InvalidateMyRunUO();
        }

        public override bool MutateSpeech( List<Mobile> hears, ref string text, ref object context )
        {
            if ( Alive )
                return false;

			if ( CanHearGhosts ) // mowa duszka postaci ze 100% SS bedzie zawsze zrozumiala
				return false;

			/*
            // duch jest zrozumialy jesli w poblizu znajduje sie postac znajaca 100% SS:
            if ( Core.AOS )
            {
                for ( int i = 0; i < hears.Count; ++i )
                {
                    Mobile m = hears[i];

                    if ( m != this && m.Skills[SkillName.SpiritSpeak].Value >= 100.0 )
                        return false;
                }
            }
			*/

            return base.MutateSpeech( hears, ref text, ref context );
        }

        public override void DoSpeech( string text, int[] keywords, MessageType type, int hue )
        {
            if( Guilds.Guild.NewGuildSystem && (type == MessageType.Guild || type == MessageType.Alliance) )
            {
                Guilds.Guild g = this.Guild as Guilds.Guild;
                if( g == null )
                {
                    SendLocalizedMessage( 1063142 ); // You are not in a guild!
                }
                else if( type == MessageType.Alliance )
                {
                    if( g.Alliance != null && g.Alliance.IsMember( g ) )
                    {
                        //g.Alliance.AllianceTextMessage( hue, "[Alliance][{0}]: {1}", this.Name, text );
                        g.Alliance.AllianceChat( this, text );
                        SendToStaffMessage( this, "[Alliance]: {0}", text );

                        m_AllianceMessageHue = hue;
                    }
                    else
                    {
                        SendLocalizedMessage( 1071020 ); // You are not in an alliance!
                    }
                }
                else    //Type == MessageType.Guild
                {
                    m_GuildMessageHue = hue;

                    g.GuildChat( this, text );
                    SendToStaffMessage( this, "[Guild]: {0}", text );
                }
            }
            else
            {
                base.DoSpeech( text, keywords, type, hue );
            }
        }

        private static void SendToStaffMessage( Mobile from, string text )
        {
            Packet p = null;

            IPooledEnumerable eable = from.GetClientsInRange(8);
            foreach( NetState ns in eable )
            {
                Mobile mob = ns.Mobile;

                if( mob != null && mob.AccessLevel >= AccessLevel.GameMaster && mob.AccessLevel > from.AccessLevel )
                {
                    if( p == null )
                        p = Packet.Acquire( new UnicodeMessage( from.Serial, from.Body, MessageType.Regular, from.SpeechHue, 3, from.Language, from.Name, text ) );

                    ns.Send( p );
                }
            }
            eable.Free();

            Packet.Release( p );
        }
        private static void SendToStaffMessage( Mobile from, string format, params object[] args )
        {
            SendToStaffMessage( from, String.Format( format, args ) );
        }

        public override void Damage( int amount, Mobile from )
        {
            if ( Spells.Necromancy.EvilOmenSpell.CheckEffect( this ) )
                amount = (int)(amount * 1.25);

            Mobile oath = Spells.Necromancy.BloodOathSpell.GetBloodOath( from );

            if ( oath == this )
            {
                amount = (int)(amount * 1.1);
                from.Damage( amount, from );
            }

            base.Damage( amount, from );
        }

        #region Poison
        public override ApplyPoisonResult ApplyPoison( Mobile from, Poison poison )
        {
            if ( !Alive )
                return ApplyPoisonResult.Immune;

            if ( Spells.Necromancy.EvilOmenSpell.CheckEffect( this ) )
                poison = PoisonImpl.IncreaseLevel( poison );

            ApplyPoisonResult result = base.ApplyPoison( from, poison );

            if ( from != null && result == ApplyPoisonResult.Poisoned && PoisonTimer is PoisonImpl.PoisonTimer )
                (PoisonTimer as PoisonImpl.PoisonTimer).From = from;

            return result;
        }

        public override bool CheckPoisonImmunity( Mobile from, Poison poison )
        {
            if ( this.Young )
                return true;

            return base.CheckPoisonImmunity( from, poison );
        }

        public override void OnPoisonImmunity( Mobile from, Poison poison )
        {
            if ( this.Young )
                SendLocalizedMessage( 502808 ); // You would have been poisoned, were you not new to the land of Britannia. Be careful in the future.
            else
                base.OnPoisonImmunity( from, poison );
        }
        #endregion

        public PlayerMobile( Serial s ) : base( s )
        {
            m_VisList = new List<Mobile>();
            m_AntiMacroTable = new Hashtable();
            InvalidateMyRunUO();
        }

        public List<Mobile> VisibilityList
        {
            get{ return m_VisList; }
        }

        public List<Mobile> PermaFlags
        {
            get{ return m_PermaFlags; }
        }

        public override int Luck{ get{ return AosAttributes.GetValue( this, AosAttribute.Luck ); } }

        public override bool IsHarmfulCriminal( Mobile target )
        {
            if (false) // Nieporzadane: tylko ofiara kradziezy moze bic zlodzieja.
            {
                // Wszyscy moga atakowac przylapanego zlodzieja:
                if (SkillHandlers.Stealing.ClassicMode && target is PlayerMobile && ((PlayerMobile)target).m_PermaFlags.Count > 0)
                {
                    int noto = Notoriety.Compute(this, target);

                    if (noto == Notoriety.Innocent)
                        target.Delta(MobileDelta.Noto);

                    return false;
                }
            }
            

            if ( target is BaseCreature && ((BaseCreature)target).InitialInnocent && !((BaseCreature)target).Controlled )
                return false;

            if ( Core.ML && target is BaseCreature && ((BaseCreature)target).Controlled && this == ((BaseCreature)target).ControlMaster )
                return false;

            return base.IsHarmfulCriminal( target );
        }

        public bool AntiMacroCheck( Skill skill, object obj )
        {
            if ( obj == null || m_AntiMacroTable == null || this.AccessLevel != AccessLevel.Player )
                return true;

            Hashtable tbl = (Hashtable)m_AntiMacroTable[skill];
            if ( tbl == null )
                m_AntiMacroTable[skill] = tbl = new Hashtable();

            CountAndTimeStamp count = (CountAndTimeStamp)tbl[obj];
            if ( count != null )
            {
                if ( count.TimeStamp + SkillCheck.AntiMacroExpire <= DateTime.Now )
                {
                    count.Count = 1;
                    return true;
                }
                else
                {
                    ++count.Count;
                    if ( count.Count <= SkillCheck.Allowance )
                        return true;
                    else
                        return false;
                }
            }
            else
            {
                tbl[obj] = count = new CountAndTimeStamp();
                count.Count = 1;

                return true;
            }
        }

        private void RevertHair()
        {
            SetHairMods( -1, -1 );
        }

        private Engines.BulkOrders.BOBFilter m_BOBFilter;

        public Engines.BulkOrders.BOBFilter BOBFilter
        {
            get{ return m_BOBFilter; }
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );
            int version = reader.ReadInt();
            
            switch ( version )
            {
                case 44:
                case 43:
                case 42:
                case 41:
                case 40:
                case 39:
                case 38:
                case 37:
                case 36:
                case 35:
                {
                    if(version < 43)
                        m_AnkhNextUse = reader.ReadDateTime();

                    goto case 34;
                }
                case 34: 
                {
                    if ( version < 36 )
                    {
                        LanguageSpeaking = (SpeechLang)reader.ReadInt();
                        LanguagesKnown = new KnownLanguages( (SpeechLang)reader.ReadInt() );
                    }
                    goto case 33;
                }
                case 33:
                {
                    if ( version < 38 )
                    {
                        m_GrabContainer = (Container)reader.ReadItem();
                    }
                    goto case 32;
                }
                case 32:
                {
                    if ( version < 37 )
                    {
                        Crest = reader.ReadInt();
                        CrestSize = (CrestSize)reader.ReadInt();
                        AppearanceAndCharacteristic = reader.ReadString();
                        HistoryAndProfession = reader.ReadString();
                        AttendenceInEvents = reader.ReadBool();
                        EventFrequencyAttendence = (EventFrequency)reader.ReadInt();
                        QuestPoints = reader.ReadInt();
                        LastQuestPointsTime = reader.ReadDateTime();
                    }
                    goto case 31;
                }
                case 31:
                {
                    if ( version < 39 )
                        /*m_weightBonus =*/ reader.ReadInt();

                    goto case 30;
                }
                case 30:
                {
                    if( version < 44 )
                        LastPowerHour = reader.ReadDateTime();

                    goto case 29;
                }
                case 29:
                {
                    if ( version < 40 )
                    {
                        int count = reader.ReadInt();
                        m_RacialKills = new int[count];

                        for ( int i = 0; i < count; i++ )
                            m_RacialKills[i] = reader.ReadInt();
                    }
                    goto case 28;
                }
                case 28:
                {
                    if ( version < 44 )
                    {
                        StrGain = reader.ReadDouble();
                        DexGain = reader.ReadDouble();
                        IntGain = reader.ReadDouble();
                    }
                    goto case 27;
                }
                case 27:
                {
                    if ( version < 41 )
                        /*m_LastMOTD =*/ reader.ReadDateTime();

                    goto case 26;
                }
                case 26:
                {
                    if(version < 42)
                        /*m_PeacedUntil = */reader.ReadDateTime();

                    goto case 25;
                }
                case 25:
                {
                    int recipeCount = reader.ReadInt();

                    if( recipeCount > 0 )
                    {
                        m_AcquiredRecipes = new Dictionary<int, bool>();

                        for( int i = 0; i < recipeCount; i++ )
                        {
                            int r = reader.ReadInt();
                            if( reader.ReadBool() )    //Don't add in recipies which we haven't gotten or have been removed
                                m_AcquiredRecipes.Add( r, true );
                        }
                    }
                    goto case 24;
                }
                case 24:
                {
                    m_LastHonorLoss = reader.ReadDeltaTime();
                    goto case 23;
                }
                case 23:
                {
                    m_ChampionTitles = new ChampionTitleInfo( reader );

                    goto case 22;
                }
                case 22:
                {
                    m_LastValorLoss = reader.ReadDateTime();
                    goto case 21;
                }
                case 21:
                {
                    m_ToTItemsTurnedIn = reader.ReadEncodedInt();
                    m_ToTTotalMonsterFame = reader.ReadInt();
                    goto case 20;
                }
                case 20:
                {
                    m_AllianceMessageHue = reader.ReadEncodedInt();
                    m_GuildMessageHue = reader.ReadEncodedInt();

                    goto case 19;
                }
                case 19:
                {
                    int rank = reader.ReadEncodedInt();
                    int maxRank = Guilds.RankDefinition.Ranks.Length -1;
                    if( rank > maxRank )
                        rank = maxRank;

                    m_GuildRank = Guilds.RankDefinition.Ranks[rank];
                    m_LastOnline = reader.ReadDateTime();
                    goto case 18;
                }
                case 18:
                {
                    m_SolenFriendship = (SolenFriendship) reader.ReadEncodedInt();

                    goto case 17;
                }
                case 17: // changed how DoneQuests is serialized
                case 16:
                {
                    m_Quest = QuestSerializer.DeserializeQuest( reader );

                    if ( m_Quest != null )
                        m_Quest.From = this;

                    int count = reader.ReadEncodedInt();

                    if ( count > 0 )
                    {
                        m_DoneQuests = new List<QuestRestartInfo>();

                        for ( int i = 0; i < count; ++i )
                        {
                            Type questType = QuestSerializer.ReadType( QuestSystem.QuestTypes, reader );
                            DateTime restartTime;

                            if ( version < 17 )
                                restartTime = DateTime.MaxValue;
                            else
                                restartTime = reader.ReadDateTime();

                            m_DoneQuests.Add( new QuestRestartInfo( questType, restartTime ) );
                        }
                    }

                    m_Profession = reader.ReadEncodedInt();
                    goto case 15;
                }
                case 15:
                {
                    m_LastCompassionLoss = reader.ReadDeltaTime();
                    goto case 14;
                }
                case 14:
                {
                    m_CompassionGains = reader.ReadEncodedInt();

                    if ( m_CompassionGains > 0 )
                        m_NextCompassionDay = reader.ReadDeltaTime();

                    goto case 13;
                }
                case 13: // just removed m_PayedInsurance list
                case 12:
                {
                    m_BOBFilter = new Engines.BulkOrders.BOBFilter( reader );
                    goto case 11;
                }
                case 11:
                {
                    if ( version < 13 )
                    {
                        List<Item> payed = reader.ReadStrongItemList();

                        for ( int i = 0; i < payed.Count; ++i )
                            payed[i].PayedInsurance = true;
                    }

                    goto case 10;
                }
                case 10:
                {
                    if ( reader.ReadBool() )
                    {
                        m_HairModID = reader.ReadInt();
                        m_HairModHue = reader.ReadInt();
                        m_BeardModID = reader.ReadInt();
                        m_BeardModHue = reader.ReadInt();

                        // We cannot call SetHairMods( -1, -1 ) here because the items have not yet loaded
                        Timer.DelayCall( TimeSpan.Zero, new TimerCallback( RevertHair ) );
                    }

                    goto case 9;
                }
                case 9:
                {
                    SavagePaintExpiration = reader.ReadTimeSpan();

                    if ( SavagePaintExpiration > TimeSpan.Zero )
                    {
                        BodyMod = ( Female ? 184 : 183 );
                        HueMod = 0;
                    }

                    goto case 8;
                }
                case 8:
                {
                    m_NpcGuild = (NpcGuild)reader.ReadInt();
                    m_NpcGuildJoinTime = reader.ReadDateTime();
                    m_NpcGuildGameTime = reader.ReadTimeSpan();
                    goto case 7;
                }
                case 7:
                {
                    m_PermaFlags = reader.ReadStrongMobileList();
                    goto case 6;
                }
                case 6:
                {
                    NextTailorBulkOrder = reader.ReadTimeSpan();
                    goto case 5;
                }
                case 5:
                {
                    NextSmithBulkOrder = reader.ReadTimeSpan();
                    goto case 4;
                }
                case 4:
                {
                    m_LastJusticeLoss = reader.ReadDeltaTime();
                    m_JusticeProtectors = reader.ReadStrongMobileList();
                    goto case 3;
                }
                case 3:
                {
                    m_LastSacrificeGain = reader.ReadDeltaTime();
                    m_LastSacrificeLoss = reader.ReadDeltaTime();
                    m_AvailableResurrects = reader.ReadInt();
                    goto case 2;
                }
                case 2:
                {
                    m_Flags = (PlayerFlag)reader.ReadInt();
                    goto case 1;
                }
                case 1:
                {
                    m_LongTermElapse = reader.ReadTimeSpan();
                    m_ShortTermElapse = reader.ReadTimeSpan();
                    m_GameTime = reader.ReadTimeSpan();
                    goto case 0;
                }
                case 0:
                {
                    break;
                }
            }

            //if (version < 34) {
            //    LanguageSpeaking = SpeechLang.Powszechny;
            //    LanguagesKnown = new KnownLanguages(SpeechLang.Powszechny);
            //}

            // 25.06.2012 :: zombie
            //if ( version < 27 )
            //    m_LastMOTD = DateTime.MinValue;
            // zombie

            // Professions weren't verified on 1.0 RC0
            if ( !CharacterCreation.VerifyProfession( m_Profession ) )
                m_Profession = 0;

            if ( m_PermaFlags == null )
                m_PermaFlags = new List<Mobile>();

            if ( m_JusticeProtectors == null )
                m_JusticeProtectors = new List<Mobile>();

            if ( m_BOBFilter == null )
                m_BOBFilter = new Engines.BulkOrders.BOBFilter();

            if( m_GuildRank == null )
                m_GuildRank = Guilds.RankDefinition.Member;    //Default to member if going from older verstion to new version (only time it should be null)

            if( m_LastOnline == DateTime.MinValue && Account != null )
                m_LastOnline = ((Account)Account).LastLogin;

            if( m_ChampionTitles == null )
                m_ChampionTitles = new ChampionTitleInfo();

            List<Mobile> list = this.Stabled;

            for ( int i = 0; i < list.Count; ++i )
            {
                BaseCreature bc = list[i] as BaseCreature;

                if ( bc != null )
                    bc.IsStabled = true;
            }

            CheckAtrophies( this );

            if( Hidden )    //Hiding is the only buff where it has an effect that's serialized.
                AddBuff( new BuffInfo( BuffIcon.HidingAndOrStealth, 1075655 ) );
        }
        
        public override void Serialize( GenericWriter writer )
        {
            //cleanup our anti-macro table 
            foreach ( Hashtable t in m_AntiMacroTable.Values )
            {
                ArrayList remove = new ArrayList();
                foreach ( CountAndTimeStamp time in t.Values )
                {
                    if ( time.TimeStamp + SkillCheck.AntiMacroExpire <= DateTime.Now )
                        remove.Add( time );
                }

                for (int i=0;i<remove.Count;++i)
                    t.Remove( remove[i] );
            }

            //decay our kills
            if ( m_ShortTermElapse < this.GameTime )
            {
                m_ShortTermElapse += TimeSpan.FromHours( 8 );
                if ( ShortTermMurders > 0 )
                    --ShortTermMurders;
            }

            if ( m_LongTermElapse < this.GameTime )
            {
                m_LongTermElapse += TimeSpan.FromHours( 40 );
                if ( Kills > 0 )
                    --Kills;
            }

            if (m_ResTimer != null && m_ResTimer.Running)
            {
                Blessed = false;
                m_ResTimer.Stop();
            }

            CheckAtrophies( this );

            base.Serialize( writer );
            
            writer.Write( (int) 25 ); // version
            
            if( m_AcquiredRecipes == null )
            {
                writer.Write( (int)0 );
            }
            else
            {
                writer.Write( m_AcquiredRecipes.Count );

                foreach( KeyValuePair<int, bool> kvp in m_AcquiredRecipes )
                {
                    writer.Write( kvp.Key );
                    writer.Write( kvp.Value );
                }
            }

            writer.WriteDeltaTime( m_LastHonorLoss );

            ChampionTitleInfo.Serialize( writer, m_ChampionTitles );

            writer.Write( m_LastValorLoss );
            writer.WriteEncodedInt( m_ToTItemsTurnedIn );
            writer.Write( m_ToTTotalMonsterFame );    //This ain't going to be a small #.

            writer.WriteEncodedInt( m_AllianceMessageHue );
            writer.WriteEncodedInt( m_GuildMessageHue );

            writer.WriteEncodedInt( m_GuildRank.Rank );
            writer.Write( m_LastOnline );

            writer.WriteEncodedInt( (int) m_SolenFriendship );

            QuestSerializer.Serialize( m_Quest, writer );

            if ( m_DoneQuests == null )
            {
                writer.WriteEncodedInt( (int) 0 );
            }
            else
            {
                if(World.ServUOSave)
                    m_DoneQuests.RemoveAll(q => q.QuestType == typeof(BowFletchingExperiment) || q.QuestType == typeof(BlacksmithyExperiment));
                writer.WriteEncodedInt( (int) m_DoneQuests.Count );

                for ( int i = 0; i < m_DoneQuests.Count; ++i )
                {
                    QuestRestartInfo restartInfo = m_DoneQuests[i];

                    QuestSerializer.Write( (Type) restartInfo.QuestType, QuestSystem.QuestTypes, writer );
                    writer.Write( (DateTime) restartInfo.RestartTime );
                }
            }

            writer.WriteEncodedInt( (int) m_Profession );

            writer.WriteDeltaTime( m_LastCompassionLoss );

            writer.WriteEncodedInt( m_CompassionGains );

            if ( m_CompassionGains > 0 )
                writer.WriteDeltaTime( m_NextCompassionDay );

            m_BOBFilter.Serialize( writer );

            bool useMods = ( m_HairModID != -1 || m_BeardModID != -1 );

            writer.Write( useMods );

            if ( useMods )
            {
                writer.Write( (int) m_HairModID );
                writer.Write( (int) m_HairModHue );
                writer.Write( (int) m_BeardModID );
                writer.Write( (int) m_BeardModHue );
            }

            writer.Write( SavagePaintExpiration );

            writer.Write( (int) m_NpcGuild );
            writer.Write( (DateTime) m_NpcGuildJoinTime );
            writer.Write( (TimeSpan) m_NpcGuildGameTime );

            writer.Write( m_PermaFlags, true );

            writer.Write( NextTailorBulkOrder );

            writer.Write( NextSmithBulkOrder );

            writer.WriteDeltaTime( m_LastJusticeLoss );
            writer.Write( m_JusticeProtectors, true );

            writer.WriteDeltaTime( m_LastSacrificeGain );
            writer.WriteDeltaTime( m_LastSacrificeLoss );
            writer.Write( m_AvailableResurrects );

            writer.Write( (int) m_Flags );

            writer.Write( m_LongTermElapse );
            writer.Write( m_ShortTermElapse );
            writer.Write( this.GameTime );
        }



        public static void CheckAtrophies( Mobile m )
        {
            SacrificeVirtue.CheckAtrophy( m );
            JusticeVirtue.CheckAtrophy( m );
            CompassionVirtue.CheckAtrophy( m );
            ValorVirtue.CheckAtrophy( m );
            HonorVirtue.CheckAtrophy( m );

            if( m is PlayerMobile )
                ChampionTitleInfo.CheckAtrophy( (PlayerMobile)m );
        }

        public void ResetKillTime()
        {
            m_ShortTermElapse = this.GameTime + TimeSpan.FromHours( 8 );
            m_LongTermElapse = this.GameTime + TimeSpan.FromHours( 40 );
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public DateTime SessionStart
        {
            get{ return m_SessionStart; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public TimeSpan GameTime
        {
            get
            {
                if ( NetState != null )
                    return m_GameTime + (DateTime.Now - m_SessionStart);
                else
                    return m_GameTime;
            }
        }

        public override bool CanSee( Mobile m )
        {
            if ( m is CharacterStatue )
                ((CharacterStatue)m).OnRequestedAnimation( this );

            if ( m is PlayerMobile && ((PlayerMobile)m).m_VisList.Contains( this ) )
                return true;

            // 25.09.2012 :: zombie :: arena
			if ( !ArenaRegion.CanSee( this, m ) )
				return false;
            // zombie

            return base.CanSee( m );
        }

        public override bool CanSee( Item item )
        {
            if ( m_DesignContext != null && m_DesignContext.Foundation.IsHiddenToCustomizer( item ) )
                return false;

            return base.CanSee( item );
        }

        public override void OnAfterDelete()
        {
            base.OnAfterDelete();

            Faction faction = Faction.Find( this );

            if ( faction != null )
                faction.RemoveMember( this );

            BaseHouse.HandleDeletion( this );
        }

        public override bool NewGuildDisplay { get { return Server.Guilds.Guild.NewGuildSystem; } }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            if ( Map == Faction.Facet )
            {
                PlayerState pl = PlayerState.Find( this );

                if ( pl != null )
                {
                    Faction faction = pl.Faction;

                    if ( faction.Commander == this )
                        list.Add( 1042733, faction.Definition.PropName ); // Commanding Lord of the ~1_FACTION_NAME~
                    else if ( pl.Sheriff != null )
                        list.Add( 1042734, "{0}\t{1}", pl.Sheriff.Definition.FriendlyName, faction.Definition.PropName ); // The Sheriff of  ~1_CITY~, ~2_FACTION_NAME~
                    else if ( pl.Finance != null )
                        list.Add( 1042735, "{0}\t{1}", pl.Finance.Definition.FriendlyName, faction.Definition.PropName ); // The Finance Minister of ~1_CITY~, ~2_FACTION_NAME~
                    else if ( pl.MerchantTitle != MerchantTitle.None )
                        list.Add( 1060776, "{0}\t{1}", MerchantTitles.GetInfo( pl.MerchantTitle ).Title, faction.Definition.PropName ); // ~1_val~, ~2_val~
                    else
                        list.Add( 1060776, "{0}\t{1}", pl.Rank.Title, faction.Definition.PropName ); // ~1_val~, ~2_val~
                }
            }
        }

        public override void OnSingleClick( Mobile from )
        {
            if ( Map == Faction.Facet )
            {
                PlayerState pl = PlayerState.Find( this );

                if ( pl != null )
                {
                    string text;
                    bool ascii = false;

                    Faction faction = pl.Faction;

                    if ( faction.Commander == this )
                        text = String.Concat( this.Female ? "(Commanding Lady of the " : "(Commanding Lord of the ", faction.Definition.FriendlyName, ")" );
                    else if ( pl.Sheriff != null )
                        text = String.Concat( "(The Sheriff of ", pl.Sheriff.Definition.FriendlyName, ", ", faction.Definition.FriendlyName, ")" );
                    else if ( pl.Finance != null )
                        text = String.Concat( "(The Finance Minister of ", pl.Finance.Definition.FriendlyName, ", ", faction.Definition.FriendlyName, ")" );
                    else
                    {
                        ascii = true;

                        if ( pl.MerchantTitle != MerchantTitle.None )
                            text = String.Concat( "(", MerchantTitles.GetInfo( pl.MerchantTitle ).Title.String, ", ", faction.Definition.FriendlyName, ")" );
                        else
                            text = String.Concat( "(", pl.Rank.Title.String, ", ", faction.Definition.FriendlyName, ")" );
                    }

                    int hue = ( Faction.Find( from ) == faction ? 98 : 38 );

                    PrivateOverheadMessage( MessageType.Label, hue, ascii, text, from.NetState );
                }
            }

            base.OnSingleClick( from );
        }

        protected override bool OnMove( Direction d )
        {
            if( !Core.SE )
                return base.OnMove( d );

            if( AccessLevel != AccessLevel.Player )
                return true;

            if( Hidden && DesignContext.Find( this ) == null )    //Hidden & NOT customizing a house
            {
                if( !Mounted && Skills.Stealth.Value >= 25.0 )
                {
                    bool running = (d & Direction.Running) != 0;

                    if( running )
                    {
                        if( (AllowedStealthSteps -= 2) <= 0 )
                            RevealingAction();
                    }
                    else if( AllowedStealthSteps-- <= 0 )
                    {
                        Server.SkillHandlers.Stealth.OnUse( this );
                    }            
                }
                else
                {
                    RevealingAction();
                }
            }

            return true;
        }

        private bool m_BedrollLogout;

        public bool BedrollLogout
        {
            get{ return m_BedrollLogout; }
            set{ m_BedrollLogout = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public override bool Paralyzed
        {
            get
            {
                return base.Paralyzed;
            }
            set
            {
                base.Paralyzed = value;

                if( value )
                    AddBuff( new BuffInfo( BuffIcon.Paralyze, 1075827 ) );    //Paralyze/You are frozen and can not move
                else
                    RemoveBuff( BuffIcon.Paralyze );
            }
        }

        #region Ethics
        private Ethics.Player m_EthicPlayer;

        [CommandProperty( AccessLevel.GameMaster )]
        public Ethics.Player EthicPlayer
        {
            get { return m_EthicPlayer; }
            set { m_EthicPlayer = value; }
        }
        #endregion

        #region Factions
        private PlayerState m_FactionPlayerState;

        public PlayerState FactionPlayerState
        {
            get{ return m_FactionPlayerState; }
            set{ m_FactionPlayerState = value; }
        }
        #endregion

        #region Quests
        private QuestSystem m_Quest;
        private List<QuestRestartInfo> m_DoneQuests;
        private SolenFriendship m_SolenFriendship;

        public QuestSystem Quest
        {
            get{ return m_Quest; }
            set{ m_Quest = value; }
        }

        public List<QuestRestartInfo> DoneQuests
        {
            get{ return m_DoneQuests; }
            set{ m_DoneQuests = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public SolenFriendship SolenFriendship
        {
            get{ return m_SolenFriendship; }
            set{ m_SolenFriendship = value; }
        }
        #endregion

        #region MyRunUO Invalidation
        private bool m_ChangedMyRunUO;

        public bool ChangedMyRunUO
        {
            get{ return m_ChangedMyRunUO; }
            set{ m_ChangedMyRunUO = value; }
        }

        public void InvalidateMyRunUO()
        {
            if ( !Deleted && !m_ChangedMyRunUO )
            {
                m_ChangedMyRunUO = true;
                Engines.MyRunUO.MyRunUO.QueueMobileUpdate( this );
            }
        }

        public override void OnKillsChange( int oldValue )
        {
            if ( this.Young && this.Kills > oldValue )
            {
                Account acc = this.Account as Account;

                if ( acc != null )
                    acc.RemoveYoungStatus( 0 );
            }

            InvalidateMyRunUO();
        }

        public override void OnGenderChanged( bool oldFemale )
        {
            InvalidateMyRunUO();
        }

        public override void OnGuildChange( Server.Guilds.BaseGuild oldGuild )
        {
            InvalidateMyRunUO();
        }

        public override void OnGuildTitleChange( string oldTitle )
        {
            InvalidateMyRunUO();
        }

        public override void OnKarmaChange( int oldValue )
        {
            InvalidateMyRunUO();
        }

        public override void OnFameChange( int oldValue )
        {
            InvalidateMyRunUO();
        }

        public override void OnSkillChange( SkillName skill, double oldBase )
        {
            if ( this.Young && this.SkillsTotal >= Config.YoungSkills )
            {
                Account acc = this.Account as Account;

                if ( acc != null )
                    acc.RemoveYoungStatus( 1019036 ); // You have successfully obtained a respectable skill level, and have outgrown your status as a young player!
            }

            InvalidateMyRunUO();
        }

        public override void OnAccessLevelChanged( AccessLevel oldLevel )
        {
            InvalidateMyRunUO();
        }

        public override void OnRawStatChange( StatType stat, int oldValue )
        {
            InvalidateMyRunUO();
        }

        public override void OnDelete()
        {
            if ( m_ReceivedHonorContext != null )
                m_ReceivedHonorContext.Cancel();
            if ( m_SentHonorContext != null )
                m_SentHonorContext.Cancel();

            InvalidateMyRunUO();
        }
        #endregion

        #region Fastwalk Prevention
        private static bool FastwalkPrevention = true; // Is fastwalk prevention enabled?
        private static TimeSpan FastwalkThreshold = TimeSpan.FromSeconds( 0.4 ); // Fastwalk prevention will become active after 0.4 seconds

        private DateTime m_NextMovementTime;

        public virtual bool UsesFastwalkPrevention{ get{ return ( AccessLevel < AccessLevel.Counselor ); } }

        public override TimeSpan ComputeMovementSpeed( Direction dir, bool checkTurning )
        {
            if ( checkTurning && (dir & Direction.Mask) != (this.Direction & Direction.Mask) )
                return TimeSpan.FromSeconds( 0.1 );    // We are NOT actually moving (just a direction change)

            TransformContext context = TransformationSpellHelper.GetContext( this );

            if ( context != null && context.Type == typeof( ReaperFormSpell ) )
                return Mobile.WalkFoot;

            bool running = ( (dir & Direction.Running) != 0 );

            bool onHorse = ( this.Mount != null );

            AnimalFormContext animalContext = AnimalForm.GetContext( this );

            if( onHorse || (animalContext != null && animalContext.SpeedBoost) )
                return ( running ? Mobile.RunMount : Mobile.WalkMount );

            return ( running ? Mobile.RunFoot : Mobile.WalkFoot );
        }

        public static bool MovementThrottle_Callback( NetState ns )
        {
            PlayerMobile pm = ns.Mobile as PlayerMobile;

            if ( pm == null || !pm.UsesFastwalkPrevention )
                return true;

            if ( pm.m_NextMovementTime == DateTime.MinValue )
            {
                // has not yet moved
                pm.m_NextMovementTime = DateTime.Now;
                return true;
            }

            TimeSpan ts = pm.m_NextMovementTime - DateTime.Now;

            if ( ts < TimeSpan.Zero )
            {
                // been a while since we've last moved
                pm.m_NextMovementTime = DateTime.Now;
                return true;
            }

            return ( ts < FastwalkThreshold );
        }
        #endregion

        #region Enemy of One
        private Type m_EnemyOfOneType;
        private bool m_WaitingForEnemy;

        public Type EnemyOfOneType
        {
            get{ return m_EnemyOfOneType; }
            set
            {
                Type oldType = m_EnemyOfOneType;
                Type newType = value;

                if ( oldType == newType )
                    return;

                m_EnemyOfOneType = value;

                DeltaEnemies( oldType, newType );
            }
        }

        public bool WaitingForEnemy
        {
            get{ return m_WaitingForEnemy; }
            set{ m_WaitingForEnemy = value; }
        }

        private void DeltaEnemies( Type oldType, Type newType )
        {
            IPooledEnumerable eable = GetMobilesInRange( 18 );
            foreach ( Mobile m in eable )
            {
                Type t = m.GetType();

                if ( t == oldType || t == newType )
                    Send( new MobileMoving( m, Notoriety.Compute( this, m ) ) );
            }
            eable.Free();
        }
        #endregion

        // Do pokazywania falszywej rasy w paperdollu dla postaci w kamuflazu peruki
        private Race m_RaceMod;
        public Race RaceMod
        {
            get { return m_RaceMod; }
            set { m_RaceMod = value; }
        }

        // Kolor kamuflazu incognito niezalezny od modyfikatorow koloru polimorfii itp.
        private int m_HueDisguise = -1;
        public int HueDisguise
        {
            get
            {
                return m_HueDisguise;
            }
            set
            {
                m_HueDisguise = value;
                Delta(MobileDelta.Hue);
            }
        }

        // W drugiej kolejnosci po kolorze polimorfii sprawdz tez kolor kamuflazu:
        [CommandProperty(AccessLevel.GameMaster)]
        public override int Hue
        {
            get
            {
                if (HueMod != -1)
                    return HueMod;

                if (HueDisguise != -1)
                    return HueDisguise;

                return base.Hue;
            }
            set
            {
                base.Hue = value;
            }
        }

        #region Hair and beard mods
        private int m_HairModID = -1, m_HairModHue;
        private int m_BeardModID = -1, m_BeardModHue;

        public void SetHairMods(int hairID, int beardID)
        {
            SetHairMods(hairID, beardID, 0);
        }

        public void SetHairMods(int hairID, int beardID, int hue)
        {
            if ( hairID == -1 )
                InternalRestoreHair( true, ref m_HairModID, ref m_HairModHue );
            else if ( hairID != -2 )
                InternalChangeHair( true, hairID, hue, ref m_HairModID, ref m_HairModHue );

            if ( beardID == -1 )
                InternalRestoreHair( false, ref m_BeardModID, ref m_BeardModHue );
            else if ( beardID != -2 )
                InternalChangeHair( false, beardID, hue, ref m_BeardModID, ref m_BeardModHue );
        }

        private void CreateHair( bool hair, int id, int hue )
        {
            if( hair )
            {
                //TODO Verification?
                HairItemID = id;
                HairHue = hue;
            }
            else
            {
                FacialHairItemID = id;
                FacialHairHue = hue;
            }
        }

        private void InternalRestoreHair( bool hair, ref int id, ref int hue )
        {
            if ( id == -1 )
                return;

            if ( hair )
                HairItemID = 0;
            else
                FacialHairItemID = 0;

            //if( id != 0 )
            CreateHair( hair, id, hue );

            id = -1;
            hue = 0;
        }

        private void InternalChangeHair( bool hair, int id, int hue, ref int storeID, ref int storeHue )
        {
            if ( storeID == -1 )
            {
                storeID = hair ? HairItemID : FacialHairItemID;
                storeHue = hair ? HairHue : FacialHairHue;
            }
            CreateHair( hair, id, hue );
        }
        #endregion

        #region Virtues
        private DateTime m_LastSacrificeGain;
        private DateTime m_LastSacrificeLoss;
        private int m_AvailableResurrects;

        public DateTime LastSacrificeGain{ get{ return m_LastSacrificeGain; } set{ m_LastSacrificeGain = value; } }
        public DateTime LastSacrificeLoss{ get{ return m_LastSacrificeLoss; } set{ m_LastSacrificeLoss = value; } }

        [CommandProperty( AccessLevel.GameMaster )]
        public int AvailableResurrects{ get{ return m_AvailableResurrects; } set{ m_AvailableResurrects = value; } }

        private DateTime m_NextJustAward;
        private DateTime m_LastJusticeLoss;
        private List<Mobile> m_JusticeProtectors;

        public DateTime LastJusticeLoss{ get{ return m_LastJusticeLoss; } set{ m_LastJusticeLoss = value; } }
        public List<Mobile> JusticeProtectors { get { return m_JusticeProtectors; } set { m_JusticeProtectors = value; } }

        private DateTime m_LastCompassionLoss;
        private DateTime m_NextCompassionDay;
        private int m_CompassionGains;

        public DateTime LastCompassionLoss{ get{ return m_LastCompassionLoss; } set{ m_LastCompassionLoss = value; } }
        public DateTime NextCompassionDay{ get{ return m_NextCompassionDay; } set{ m_NextCompassionDay = value; } }
        public int CompassionGains{ get{ return m_CompassionGains; } set{ m_CompassionGains = value; } }

        private DateTime m_LastValorLoss;

        public DateTime LastValorLoss { get { return m_LastValorLoss; } set { m_LastValorLoss = value; } }

        private DateTime m_LastHonorLoss;
        private DateTime m_LastHonorUse;
        private bool m_HonorActive;
        private HonorContext m_ReceivedHonorContext;
        private HonorContext m_SentHonorContext;

        public DateTime LastHonorLoss{ get{ return m_LastHonorLoss; } set{ m_LastHonorLoss = value; } }
        public DateTime LastHonorUse{ get{ return m_LastHonorUse; } set{ m_LastHonorUse = value; } }
        public bool HonorActive{ get{ return m_HonorActive; } set{ m_HonorActive = value; } }
        public HonorContext ReceivedHonorContext{ get{ return m_ReceivedHonorContext; } set{ m_ReceivedHonorContext = value; } }
        public HonorContext SentHonorContext{ get{ return m_SentHonorContext; } set{ m_SentHonorContext = value; } }
        #endregion

        #region Young system
        [CommandProperty( AccessLevel.GameMaster )]
        public bool Young
        {
            get{ return GetFlag( PlayerFlag.Young ); }
            set{ SetFlag( PlayerFlag.Young, value ); InvalidateProperties(); }
        }

        public override string ApplyNameSuffix( string suffix )
        {
            if ( Young )
            {
                if ( suffix.Length == 0 )
                    suffix = "(Young)";
                else
                    suffix = String.Concat( suffix, " (Young)" );
            }

            #region Ethics
            if ( m_EthicPlayer != null )
            {
                if ( suffix.Length == 0 )
                    suffix = m_EthicPlayer.Ethic.Definition.Adjunct.String;
                else
                    suffix = String.Concat( suffix, " ", m_EthicPlayer.Ethic.Definition.Adjunct.String );
            }
            #endregion

            if ( Core.ML && this.Map == Faction.Facet )
            {
                Faction faction = Faction.Find( this );

                if ( faction != null )
                {
                    string adjunct = String.Format( "[{0}]", faction.Definition.Abbreviation );
                    if ( suffix.Length == 0 )
                        suffix = adjunct;
                    else
                        suffix = String.Concat( suffix, " ", adjunct );
                }
            }

            return base.ApplyNameSuffix( suffix );
        }


        public override TimeSpan GetLogoutDelay()
        {
            if ( Young || BedrollLogout || TestCenter.Enabled )
                return TimeSpan.Zero;

            return base.GetLogoutDelay();
        }

        private DateTime m_LastYoungMessage = DateTime.MinValue;

        public bool CheckYoungProtection( Mobile from )
        {
            if ( !this.Young )
                return false;

            if ( Region.IsPartOf( typeof( DungeonRegion ) ) )
                return false;

            if( from is BaseCreature && ((BaseCreature)from).IgnoreYoungProtection )
                return false;

            if ( this.Quest != null && this.Quest.IgnoreYoungProtection( from ) )
                return false;

            if ( DateTime.Now - m_LastYoungMessage > TimeSpan.FromMinutes( 1.0 ) )
            {
                m_LastYoungMessage = DateTime.Now;
                SendLocalizedMessage( 1019067 ); // A monster looks at you menacingly but does not attack.  You would be under attack now if not for your status as a new citizen of Britannia.
            }

            return true;
        }

        private DateTime m_LastYoungHeal = DateTime.MinValue;

        public bool CheckYoungHealTime()
        {
            if ( DateTime.Now - m_LastYoungHeal > TimeSpan.FromMinutes( 5.0 ) )
            {
                m_LastYoungHeal = DateTime.Now;
                return true;
            }

            return false;
        }

        private static Point3D[] m_TrammelDeathDestinations = new Point3D[]
            {
                new Point3D( 1481, 1612, 20 ),
                new Point3D( 2708, 2153,  0 ),
                new Point3D( 2249, 1230,  0 ),
                new Point3D( 5197, 3994, 37 ),
                new Point3D( 1412, 3793,  0 ),
                new Point3D( 3688, 2232, 20 ),
                new Point3D( 2578,  604,  0 ),
                new Point3D( 4397, 1089,  0 ),
                new Point3D( 5741, 3218, -2 ),
                new Point3D( 2996, 3441, 15 ),
                new Point3D(  624, 2225,  0 ),
                new Point3D( 1916, 2814,  0 ),
                new Point3D( 2929,  854,  0 ),
                new Point3D(  545,  967,  0 ),
                new Point3D( 3665, 2587,  0 )
            };

        private static Point3D[] m_IlshenarDeathDestinations = new Point3D[]
            {
                new Point3D( 1216,  468, -13 ),
                new Point3D(  723, 1367, -60 ),
                new Point3D(  745,  725, -28 ),
                new Point3D(  281, 1017,   0 ),
                new Point3D(  986, 1011, -32 ),
                new Point3D( 1175, 1287, -30 ),
                new Point3D( 1533, 1341,  -3 ),
                new Point3D(  529,  217, -44 ),
                new Point3D( 1722,  219,  96 )
            };

        private static Point3D[] m_MalasDeathDestinations = new Point3D[]
            {
                new Point3D( 2079, 1376, -70 ),
                new Point3D(  944,  519, -71 )
            };

        private static Point3D[] m_TokunoDeathDestinations = new Point3D[]
            {
                new Point3D( 1166,  801, 27 ),
                new Point3D(  782, 1228, 25 ),
                new Point3D(  268,  624, 15 )
            };

        public bool YoungDeathTeleport()
        {
            if ( this.Region.IsPartOf( typeof( JailRegion ) )
                || this.Region.IsPartOf( "Samurai start location" )
                || this.Region.IsPartOf( "Ninja start location" )
                || this.Region.IsPartOf( "Ninja cave" ) )
                return false;

            Point3D loc;
            Map map;

            DungeonRegion dungeon = (DungeonRegion) this.Region.GetRegion( typeof( DungeonRegion ) );
            if ( dungeon != null && dungeon.EntranceLocation != Point3D.Zero )
            {
                loc = dungeon.EntranceLocation;
                map = dungeon.EntranceMap;
            }
            else
            {
                loc = this.Location;
                map = this.Map;
            }

            Point3D[] list;

            if ( map == Map.Trammel )
                list = m_TrammelDeathDestinations;
            else if ( map == Map.Ilshenar )
                list = m_IlshenarDeathDestinations;
            else if ( map == Map.Malas )
                list = m_MalasDeathDestinations;
            else if ( map == Map.Tokuno )
                list = m_TokunoDeathDestinations;
            else
                return false;

            Point3D dest = Point3D.Zero;
            int sqDistance = int.MaxValue;

            for ( int i = 0; i < list.Length; i++ )
            {
                Point3D curDest = list[i];

                int width = loc.X - curDest.X;
                int height = loc.Y - curDest.Y;
                int curSqDistance = width * width + height * height;

                if ( curSqDistance < sqDistance )
                {
                    dest = curDest;
                    sqDistance = curSqDistance;
                }
            }

            this.MoveToWorld( dest, map );
            return true;
        }

        private void SendYoungDeathNotice()
        {
            this.SendGump( new YoungDeathNotice() );
        }
        #endregion

        #region Speech log
        private SpeechLog m_SpeechLog;

        public SpeechLog SpeechLog{ get{ return m_SpeechLog; } }

        public override void OnSpeech( SpeechEventArgs e )
        {
            if ( SpeechLog.Enabled && this.NetState != null )
            {
                if ( m_SpeechLog == null )
                    m_SpeechLog = new SpeechLog();

                m_SpeechLog.Add( e.Mobile, e.Speech );
            }
        }
        #endregion

        #region Champion Titles
        [CommandProperty( AccessLevel.GameMaster )]
        public bool DisplayChampionTitle
        {
            get { return GetFlag( PlayerFlag.DisplayChampionTitle ); }
            set { SetFlag( PlayerFlag.DisplayChampionTitle, value ); }
        }

        private ChampionTitleInfo m_ChampionTitles;

        [CommandProperty( AccessLevel.GameMaster )]
        public ChampionTitleInfo ChampionTitles { get { return m_ChampionTitles; } set { } }

        private void ToggleChampionTitleDisplay()
        {
            if( !CheckAlive() )
                return;

            if( DisplayChampionTitle )
                SendLocalizedMessage( 1062419, "", 0x23 ); // You have chosen to hide your monster kill title.
            else
                SendLocalizedMessage( 1062418, "", 0x23 ); // You have chosen to display your monster kill title.

            DisplayChampionTitle = !DisplayChampionTitle;
        }

        [PropertyObject]
        public class ChampionTitleInfo
        {
            public static TimeSpan LossDelay = TimeSpan.FromDays( 1.0 );
            public const int LossAmount = 90;

            private class TitleInfo
            {
                private int m_Value;
                private DateTime m_LastDecay;

                public int Value { get { return m_Value; } set { m_Value = value; } }
                public DateTime LastDecay { get { return m_LastDecay; } set { m_LastDecay = value; } }

                public TitleInfo()
                {
                }

                public TitleInfo( GenericReader reader )
                {
                    int version = reader.ReadEncodedInt();

                    switch( version )
                    {
                        case 0:
                        {
                            m_Value = reader.ReadEncodedInt();
                            m_LastDecay = reader.ReadDateTime();
                            break;
                        }
                    }
                }

                public static void Serialize( GenericWriter writer, TitleInfo info )
                {
                    writer.WriteEncodedInt( (int)0 ); // version

                    writer.WriteEncodedInt( info.m_Value );
                    writer.Write( info.m_LastDecay );
                }

            }
            private TitleInfo[] m_Values;

            private int m_Harrower;    //Harrower titles do NOT decay


            public int GetValue( ChampionSpawnType type )
            {
                return GetValue( (int)type );
            }

            public void SetValue( ChampionSpawnType type, int value )
            {
                SetValue( (int)type, value );
            }

            public void Award( ChampionSpawnType type, int value )
            {
                Award( (int)type, value );
            }

            public int GetValue( int index )
            {
                if( m_Values == null || index < 0 || index >= m_Values.Length )
                    return 0;

                if( m_Values[index] == null )
                    m_Values[index] = new TitleInfo();

                return m_Values[index].Value;
            }

            public DateTime GetLastDecay( int index )
            {
                if( m_Values == null || index < 0 || index >= m_Values.Length )
                    return DateTime.MinValue;

                if( m_Values[index] == null )
                    m_Values[index] = new TitleInfo();

                return m_Values[index].LastDecay;
            }

            public void SetValue( int index, int value )
            {
                if( m_Values == null )
                    m_Values = new TitleInfo[ChampionSpawnInfo.Table.Length];

                if( value < 0 )
                    value = 0;

                if( index < 0 || index >= m_Values.Length )
                    return;

                if( m_Values[index] == null )
                    m_Values[index] = new TitleInfo();

                m_Values[index].Value = value;
            }

            public void Award( int index, int value )
            {
                if( m_Values == null )
                    m_Values = new TitleInfo[ChampionSpawnInfo.Table.Length];

                if( index < 0 || index >= m_Values.Length || value <= 0 )
                    return;

                if( m_Values[index] == null )
                    m_Values[index] = new TitleInfo();

                m_Values[index].Value += value;
            }

            public void Atrophy( int index, int value )
            {
                if( m_Values == null )
                    m_Values = new TitleInfo[ChampionSpawnInfo.Table.Length];

                if( index < 0 || index >= m_Values.Length || value <= 0 )
                    return;

                if( m_Values[index] == null )
                    m_Values[index] = new TitleInfo();

                int before = m_Values[index].Value;

                if( (m_Values[index].Value - value) < 0 )
                    m_Values[index].Value = 0;
                else
                    m_Values[index].Value -= value;

                if( before != m_Values[index].Value )
                    m_Values[index].LastDecay = DateTime.Now;
            }

            public override string ToString()
            {
                return "...";
            }

            [CommandProperty( AccessLevel.GameMaster )]
            public int Abyss { get { return GetValue( ChampionSpawnType.Abyss ); } set { SetValue( ChampionSpawnType.Abyss, value ); } }

            [CommandProperty( AccessLevel.GameMaster )]
            public int Arachnid { get { return GetValue( ChampionSpawnType.Arachnid ); } set { SetValue( ChampionSpawnType.Arachnid, value ); } }

            [CommandProperty( AccessLevel.GameMaster )]
            public int ColdBlood { get { return GetValue( ChampionSpawnType.ColdBlood ); } set { SetValue( ChampionSpawnType.ColdBlood, value ); } }

            [CommandProperty( AccessLevel.GameMaster )]
            public int ForestLord { get { return GetValue( ChampionSpawnType.ForestLord ); } set { SetValue( ChampionSpawnType.ForestLord, value ); } }

            [CommandProperty( AccessLevel.GameMaster )]
            public int SleepingDragon { get { return GetValue( ChampionSpawnType.SleepingDragon ); } set { SetValue( ChampionSpawnType.SleepingDragon, value ); } }

            [CommandProperty( AccessLevel.GameMaster )]
            public int UnholyTerror { get { return GetValue( ChampionSpawnType.UnholyTerror ); } set { SetValue( ChampionSpawnType.UnholyTerror, value ); } }

            [CommandProperty( AccessLevel.GameMaster )]
            public int VerminHorde { get { return GetValue( ChampionSpawnType.VerminHorde ); } set { SetValue( ChampionSpawnType.VerminHorde, value ); } }
            
            [CommandProperty( AccessLevel.GameMaster )]
            public int Harrower { get { return m_Harrower; } set { m_Harrower = value; } }

            public ChampionTitleInfo()
            {
            }

            public ChampionTitleInfo( GenericReader reader )
            {
                int version = reader.ReadEncodedInt();

                switch( version )
                {
                    case 0:
                    {
                        m_Harrower = reader.ReadEncodedInt();

                        int length = reader.ReadEncodedInt();
                        m_Values = new TitleInfo[length];

                        for( int i = 0; i < length; i++ )
                        {
                            m_Values[i] = new TitleInfo( reader );
                        }

                        if( m_Values.Length != ChampionSpawnInfo.Table.Length )
                        {
                            TitleInfo[] oldValues = m_Values;
                            m_Values = new TitleInfo[ChampionSpawnInfo.Table.Length];

                            for( int i = 0; i < m_Values.Length && i < oldValues.Length; i++ )
                            {
                                m_Values[i] = oldValues[i];
                            }
                        }
                        break;
                    }
                }
            }

            public static void Serialize( GenericWriter writer, ChampionTitleInfo titles )
            {
                writer.WriteEncodedInt( (int)0 ); // version

                writer.WriteEncodedInt( titles.m_Harrower );

                int length = titles.m_Values.Length;
                writer.WriteEncodedInt( length );

                for( int i = 0; i < length; i++ )
                {
                    if( titles.m_Values[i] == null )
                        titles.m_Values[i] = new TitleInfo();

                    TitleInfo.Serialize( writer, titles.m_Values[i] );
                }
            }

            public static void CheckAtrophy( PlayerMobile pm )
            {
                ChampionTitleInfo t = pm.m_ChampionTitles;
                if( t == null )
                    return;

                if( t.m_Values == null )
                    t.m_Values = new TitleInfo[ChampionSpawnInfo.Table.Length];

                for( int i = 0; i < t.m_Values.Length; i++ )
                {
                    if( (t.GetLastDecay( i ) + LossDelay) < DateTime.Now )
                    {
                        t.Atrophy( i, LossAmount );
                    }
                }
            }

            public static void AwardHarrowerTitle( PlayerMobile pm )    //Called when killing a harrower.  Will give a minimum of 1 point.
            {
                ChampionTitleInfo t = pm.m_ChampionTitles;
                if( t == null )
                    return;

                if( t.m_Values == null )
                    t.m_Values = new TitleInfo[ChampionSpawnInfo.Table.Length];

                int count = 1;

                for( int i = 0; i < t.m_Values.Length; i++ )
                {
                    if( t.m_Values[i].Value > 900 )
                        count++;
                }

                t.m_Harrower = Math.Max( count, t.m_Harrower );    //Harrower titles never decay.
            }
        }
        #endregion

        #region Recipes

        private Dictionary<int, bool> m_AcquiredRecipes;
        
        public virtual bool HasRecipe( Recipe r )
        {
            if( r == null ) 
                return false;

            return HasRecipe( r.ID );
        }

        public virtual bool HasRecipe( int recipeID )
        {
            if( m_AcquiredRecipes != null && m_AcquiredRecipes.ContainsKey( recipeID ) )
                return m_AcquiredRecipes[recipeID];

            return false;
        }

        public virtual void AcquireRecipe( Recipe r )
        {
            if( r != null )
                AcquireRecipe( r.ID );
        }

        public virtual void AcquireRecipe( int recipeID )
        {
            if( m_AcquiredRecipes == null )
                m_AcquiredRecipes = new Dictionary<int, bool>();

            m_AcquiredRecipes[recipeID] = true;
        }

        public virtual void ResetRecipes()
        {
            m_AcquiredRecipes = null;
        }
    
        [CommandProperty( AccessLevel.GameMaster )]
        public int KnownRecipes
        {
            get 
            {
                if( m_AcquiredRecipes == null )
                    return 0;

                return m_AcquiredRecipes.Count;
            }
        }
    

        #endregion

        #region Buff Icons

        public void ResendBuffs()
        {
            if( !BuffInfo.Enabled || m_BuffTable == null )
                return;

            NetState state = this.NetState;

            if( state != null && state.Version >= BuffInfo.RequiredClient )
            {
                foreach( BuffInfo info in m_BuffTable.Values )
                {
                    state.Send( new AddBuffPacket( this, info ) );
                }
            }
        }

        private Dictionary<BuffIcon, BuffInfo> m_BuffTable;

        public void AddBuff( BuffInfo b )
        {
            if( !BuffInfo.Enabled || b == null )
                return;

            RemoveBuff( b );    //Check & subsequently remove the old one.

            if( m_BuffTable == null )
                m_BuffTable = new Dictionary<BuffIcon, BuffInfo>();

            m_BuffTable.Add( b.ID, b );

            NetState state = this.NetState;

            if( state != null && state.Version >= BuffInfo.RequiredClient )
            {
                state.Send( new AddBuffPacket( this, b ) );
            }
        }

        public void RemoveBuff( BuffInfo b )
        {
            if( b == null )
                return;

            RemoveBuff( b.ID );
        }

        public void RemoveBuff( BuffIcon b )
        {
            if( m_BuffTable == null || !m_BuffTable.ContainsKey( b ) )
                return;

            BuffInfo info = m_BuffTable[b];

            if( info.Timer != null && info.Timer.Running )
                info.Timer.Stop();

            m_BuffTable.Remove( b );

            NetState state = this.NetState;

            if( state != null && state.Version >= BuffInfo.RequiredClient )
            {
                state.Send( new RemoveBuffPacket( this, b ) );
            }

            if( m_BuffTable.Count <= 0 )
                m_BuffTable = null;
        }
        #endregion
    }
}