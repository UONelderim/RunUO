using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;
using Server.Network;
using Server.ContextMenus;
using Server.Mobiles;
using Server.Misc;
using Server.Engines.BulkOrders;
using Server.Regions;
using Server.Factions;
using Server.Nelderim;
using System.Text.RegularExpressions;
using Nelderim.Towns;

namespace Server.Mobiles
{
    public enum VendorShoeType
    {
        None,
        Shoes,
        Boots,
        Sandals,
        ThighBoots
    }

    public abstract class BaseVendor : BaseCreature, IVendor
    {
        // 26.06.2012 :: zombie :: system plotek
        #region Rumors

        public virtual void SayRumor( Mobile from, SpeechEventArgs e )
        {
            try
            {
                String speech = e.Speech;
                List<RumorRecord> RumorsList = RumorsSystem.GetRumors( this, PriorityLevel.VeryLow );

                if ( RumorsList != null && RumorsList.Count > 0 )
                {
                    RumorsList.Reverse();

                    for ( int index = 0; index < RumorsList.Count; index++ )
                    {
                        RumorRecord rumor = (RumorRecord)RumorsList[index];

                        char[] separator = new char[] { Convert.ToChar( " " ) };

                        String[] keywords = rumor.KeyWord.Split( separator );
                        for ( int a = 0; a < keywords.Length; a++ )
                        {
                            if ( Regex.IsMatch( speech, keywords[a].Replace(",",""), RegexOptions.IgnoreCase ) && !e.Handled )
                            {
                                e.Handled = true;
                                this.Direction = this.GetDirectionTo( from.Location );

                                if ( !CheckVendorAccess( from ) )
                                    return;

                                from.SendGump( new SayRumorGump( this as Mobile, rumor ) );

                                Say( 00505131, rumor.Text ); // "Slyszalem ze, ~1_RUMOR~"
                                return;
                            }
                        }
                    }
                }
            }
            catch ( Exception exc )
            {
                Console.WriteLine( exc.ToString() );
            }
        }

        public virtual void SayAboutRumors( Mobile from )
        {
            try
            {
                List<RumorRecord> RumorsList = RumorsSystem.GetRumors( this, PriorityLevel.VeryLow );

                this.Direction = this.GetDirectionTo( from.Location );

                if ( !CheckVendorAccess( from ) )
                    return;

                if ( RumorsList == null || RumorsList.Count == 0 )
                {
                    Say( 00505132 ); // Pfff... nuda, cisza, spokoj... nic do rozmowy przy mlodym winie.
                    return;
                }

                switch ( RumorsList.Count )
                {
                    case 1:
                        Say( from.Female ? 505525 : 505524, ( RumorsList[ 0 ] as RumorRecord ).Title ); // Co ciekawego? Slyszalas o ___?
                        break;
                    case 2:
                        Say( 505523, String.Format( "{0}\t{1}", ( RumorsList[ 0 ] as RumorRecord ).Title, ( RumorsList[ 1 ] as RumorRecord ).Title ) ); // Mam dwie nowiny. O ___ i o ___.
                        break;
                    default:
                        string text = "";
                        
                        for( int i=0; i < RumorsList.Count; i++ )
                        {
                            RumorRecord rum = ( RumorRecord )RumorsList[ i ];
        
                            text += String.Format( "{0}{1}{2}", ( i + 1 == RumorsList.Count ) ? " lub o " : " ", rum.Title, ( i + 1 == RumorsList.Count ) ? "" : "," );
                        }
                        
                        Say( 505522, text ); // AH! Mnogosc nowin! Chcesz, opowiem Ci o ___
                        break;
                }

            }
            catch ( Exception exc )
            {
                Console.WriteLine( exc.ToString() );
            }
        }

        private DateTime m_LastRumor;

        #endregion
        // zombie

        private const int MaxSell = 500;

        protected abstract ArrayList SBInfos{ get; }

        private ArrayList m_ArmorBuyInfo = new ArrayList();
        private ArrayList m_ArmorSellInfo = new ArrayList();

        private DateTime m_LastRestock;

        public override bool CanTeach{ get{ return true; } }

        public override bool CheckTeach(SkillName skill, Mobile from)
        {
            if (!base.CheckTeach(skill, from))
                return false;

            // Sprawdzanie dostepnosci budynku
            if (!IsAssignedBuildingWorking())
            {
                return false;
            }

            return true;
        }

        public override bool PlayerRangeSensitive{ get{ return true; } }

        public virtual bool IsActiveVendor{ get{ return true; } }
        public virtual bool IsActiveBuyer{ get{ return IsActiveVendor; } } // response to vendor SELL
        public virtual bool IsActiveSeller{ get{ return IsActiveVendor; } } // repsonse to vendor BUY

        public virtual NpcGuild NpcGuild{ get{ return NpcGuild.None; } }

        public virtual bool IsInvulnerable{ get{ return true; } }

        public override bool ShowFameTitle{ get{ return false; } }

        public virtual bool IsValidBulkOrder( Item item )
        {
            return false;
        }

        public virtual Item CreateBulkOrder( Mobile from, bool fromContextMenu )
        {
            return null;
        }

        public virtual bool SupportsBulkOrders( Mobile from )
        {
            return false;
        }

        public virtual TimeSpan GetNextBulkOrder( Mobile from )
        {
            return TimeSpan.Zero;
        }

        public virtual void OnSuccessfulBulkOrderReceive( Mobile from )
        {
        }

        #region Modyfikacja ceny w zaleznosci od podatkow
        public virtual int GetPriceScalar(Mobile from)
        {
            int scalar = 100;

            if (TownAssigned != Towns.None)
            {
                Towns m_fromTown = TownDatabase.GetCitizenCurrentCity(from);
                if (m_fromTown == TownAssigned)
                {
                    scalar += TownDatabase.GetTown(TownAssigned).TaxesForThisTown;
                }
                else if (m_fromTown == Towns.None)
                {
                    scalar += TownDatabase.GetTown(TownAssigned).TaxesForNoTown;
                }
                else
                {
                    scalar += TownDatabase.GetTown(TownAssigned).TaxesForOtherTowns;
                }
            }

            return scalar;
        }

        public virtual int PayToVault(Mobile from, int amount)
        {
            if (TownAssigned != Towns.None)
            {
                int scalar = GetPriceScalar(from);

                if (scalar == 100)
                {
                    
                    return 0;
                }
                else if (scalar > 100)
                {
                    TownDatabase.GetTown(TownAssigned).Resources.ResourceIncreaseAmount(TownResourceType.Zloto, (int)(amount * scalar / 100 - amount));
                    return 1;
                }
                else
                {
                    TownDatabase.GetTown(TownAssigned).Resources.ResourceIncreaseAmount(TownResourceType.Zloto, (int)(amount * scalar / 100 - amount));
                    return -1;
                }
            }

            return 0;
        }

        public void UpdateBuyInfo(Mobile from)
        {
            int priceScalar = GetPriceScalar(from);
            
            IBuyItemInfo[] buyinfo = (IBuyItemInfo[])m_ArmorBuyInfo.ToArray( typeof( IBuyItemInfo ) );

            if ( buyinfo != null )
            {
                foreach ( IBuyItemInfo info in buyinfo )
                    info.PriceScalar = priceScalar;
            }
        }
        #endregion

        private class BulkOrderInfoEntry : ContextMenuEntry
        {
            private Mobile m_From;
            private BaseVendor m_Vendor;

            public BulkOrderInfoEntry( Mobile from, BaseVendor vendor ) : base( 6152, 6 )
            {
                m_From = from;
                m_Vendor = vendor;
            }

            public override void OnClick()
            {
                if (m_Vendor.CheckVendorAccess(m_From)){
                    if (m_Vendor.SupportsBulkOrders(m_From)) {
                        TimeSpan ts = m_Vendor.GetNextBulkOrder(m_From);

                        int totalSeconds = (int)ts.TotalSeconds;
                        int totalHours = (totalSeconds + 3599) / 3600;
                        int totalMinutes = (totalSeconds + 59) / 60;

                        if (((Core.SE) ? totalMinutes == 0 : totalHours == 0)) {
                            m_From.SendLocalizedMessage(1049038); // You can get an order now.

                            if (Core.AOS) {
                                Item bulkOrder = m_Vendor.CreateBulkOrder(m_From, true);

                                if (bulkOrder is LargeBOD)
                                    m_From.SendGump(new LargeBODAcceptGump(m_From, (LargeBOD)bulkOrder));
                                else if (bulkOrder is SmallBOD)
                                    m_From.SendGump(new SmallBODAcceptGump(m_From, (SmallBOD)bulkOrder));
                            }
                        } else {
                            int oldSpeechHue = m_Vendor.SpeechHue;
                            m_Vendor.SpeechHue = 0x3B2;

                            if (Core.SE)
                                m_Vendor.SayTo(m_From, 1072058, totalMinutes.ToString()); // An offer may be available in about ~1_minutes~ minutes.
                            else
                                m_Vendor.SayTo(m_From, 1049039, totalHours.ToString()); // An offer may be available in about ~1_hours~ hours.

                            m_Vendor.SpeechHue = oldSpeechHue;
                        }
                    }
                }
            }
        }

        public BaseVendor( string title ) : base( AIType.AI_Vendor, FightMode.None, 2, 1, 0.5, 2 )
        {
            LoadSBInfo();

            this.Title = title;
            InitBody();
            InitOutfit();

            Container pack;
            //these packs MUST exist, or the client will crash when the packets are sent
            pack = new Backpack();
            pack.Layer = Layer.ShopBuy;
            pack.Movable = false;
            pack.Visible = false;
            AddItem( pack );

            pack = new Backpack();
            pack.Layer = Layer.ShopResale;
            pack.Movable = false;
            pack.Visible = false;
            AddItem( pack );

            m_LastRestock = DateTime.Now;
        }

        // 23.09.2012 :: zombie
        protected override void Init()
        {
            RaceGenerator.Init( this );

            base.Init();
        }
        // zombie

        public BaseVendor( Serial serial ) : base( serial )
        {
        }

        public DateTime LastRestock
        {
            get
            {
                return m_LastRestock;
            }
            set
            {
                m_LastRestock = value;
            }
        }

        public Container BuyPack
        {
            get
            {
                Container pack = FindItemOnLayer( Layer.ShopBuy ) as Container;

                if ( pack == null )
                {
                    pack = new Backpack();
                    pack.Layer = Layer.ShopBuy;
                    pack.Visible = false;
                    AddItem( pack );
                }

                return pack;
            }
        }
        
        protected virtual Type Currency
        {
            get { return typeof(Gold); }
        }

        public abstract void InitSBInfo();

        public virtual bool IsTokunoVendor{ get{ return ( Map == Map.Tokuno ); } }

        protected void LoadSBInfo()
        {
            m_LastRestock = DateTime.Now;

            for ( int i = 0; i < m_ArmorBuyInfo.Count; ++i )
            {
                GenericBuyInfo buy = m_ArmorBuyInfo[i] as GenericBuyInfo;

                if ( buy != null )
                    buy.DeleteDisplayEntity();
            }

            SBInfos.Clear();

            InitSBInfo();

            m_ArmorBuyInfo.Clear();
            m_ArmorSellInfo.Clear();

            for ( int i = 0; i < SBInfos.Count; i++ )
            {
                SBInfo sbInfo = (SBInfo)SBInfos[i];
                m_ArmorBuyInfo.AddRange( sbInfo.BuyInfo );
                m_ArmorSellInfo.Add( sbInfo.SellInfo );
            }
        }

        public virtual bool GetGender()
        {
            return Utility.RandomBool();
        }

        public virtual void InitBody()
        {
            InitStats( 100, 100, 25 );

            SpeechHue = Utility.RandomDyedHue();
            Hue = Utility.RandomSkinHue();

            if ( IsInvulnerable && !Core.AOS )
                NameHue = 0x35;

            // 08.07.2012 :: zombie :: usuniecie losowania imion (imiona generujemy w InitRace)
            if ( Female = GetGender() )
            {
                Body = 0x191;
                //Name = NameList.RandomName( "female" );
            }
            else
            {
                Body = 0x190;
                //Name = NameList.RandomName( "male" );
            }
            // zombie
        }

        public virtual int GetRandomHue()
        {
            switch ( Utility.Random( 5 ) )
            {
                default:
                case 0: return Utility.RandomBlueHue();
                case 1: return Utility.RandomGreenHue();
                case 2: return Utility.RandomRedHue();
                case 3: return Utility.RandomYellowHue();
                case 4: return Utility.RandomNeutralHue();
            }
        }

        public virtual int GetShoeHue()
        {
            if ( 0.1 > Utility.RandomDouble() )
                return 0;

            return Utility.RandomNeutralHue();
        }

        public virtual VendorShoeType ShoeType
        {
            get{ return VendorShoeType.Shoes; }
        }

        public virtual int RandomBrightHue()
        {
            if ( 0.1 > Utility.RandomDouble() )
                return Utility.RandomList( 0x62, 0x71 );

            return Utility.RandomList( 0x03, 0x0D, 0x13, 0x1C, 0x21, 0x30, 0x37, 0x3A, 0x44, 0x59 );
        }

        public virtual void CheckMorph()
        {
            if ( CheckGargoyle() )
                return;

            if ( CheckNecromancer() )
                return;

            CheckTokuno();
        }

        public virtual bool CheckTokuno()
        {
            if ( this.Map != Map.Tokuno )
                return false;

            NameList n;

            if ( Female )
                n = NameList.GetNameList( "tokuno female" );
            else
                n = NameList.GetNameList( "tokuno male" );

            if ( !n.ContainsName( this.Name ) )
                TurnToTokuno();

            return true;
        }

        public virtual void TurnToTokuno()
        {
            if ( Female )
                this.Name = NameList.RandomName( "tokuno female" );
            else
                this.Name = NameList.RandomName( "tokuno male" );
        }

        public virtual bool CheckGargoyle()
        {
            Map map = this.Map;

            if ( map != Map.Ilshenar )
                return false;

            if ( !Region.IsPartOf( "Gargoyle City" ) )
                return false;

            if ( Body != 0x2F6 || (Hue & 0x8000) == 0 )
                TurnToGargoyle();

            return true;
        }

        public virtual bool CheckNecromancer()
        {
            Map map = this.Map;

            if ( map != Map.Malas )
                return false;

            if ( !Region.IsPartOf( "Umbra" ) )
                return false;

            if ( Hue != 0x83E8 )
                TurnToNecromancer();

            return true;
        }

        public override void OnAfterSpawn()
        {
            CheckMorph();
        }

        protected override void OnMapChange( Map oldMap )
        {
            base.OnMapChange( oldMap );

            CheckMorph();

            LoadSBInfo();
        }

        public virtual int GetRandomNecromancerHue()
        {
            switch ( Utility.Random( 20 ) )
            {
                case 0: return 0;
                case 1: return 0x4E9;
                default: return Utility.RandomList( 0x485, 0x497 );
            }
        }

        public virtual void TurnToNecromancer()
        {
            for ( int i = 0; i < this.Items.Count; ++i )
            {
                Item item = this.Items[i];

                if ( item is Hair || item is Beard )
                    item.Hue = 0;
                else if ( item is BaseClothing || item is BaseWeapon || item is BaseArmor || item is BaseTool )
                    item.Hue = GetRandomNecromancerHue();
            }

            HairHue = 0;
            FacialHairHue = 0;

            Hue = 0x83E8;
        }

        public virtual void TurnToGargoyle()
        {
            for ( int i = 0; i < this.Items.Count; ++i )
            {
                Item item = this.Items[i];

                if ( item is BaseClothing || item is Hair || item is Beard )
                    item.Delete();
            }

            HairItemID = 0;
            FacialHairItemID = 0;

            Body = 0x2F6;
            Hue = RandomBrightHue() | 0x8000;
            Name = NameList.RandomName( "gargoyle vendor" );

            CapitalizeTitle();
        }

        public virtual void CapitalizeTitle()
        {
            string title = this.Title;

            if ( title == null )
                return;

            string[] split = title.Split( ' ' );

            for ( int i = 0; i < split.Length; ++i )
            {
                if ( Insensitive.Equals( split[i], "the" ) )
                    continue;

                if ( split[i].Length > 1 )
                    split[i] = Char.ToUpper( split[i][0] ) + split[i].Substring( 1 );
                else if ( split[i].Length > 0 )
                    split[i] = Char.ToUpper( split[i][0] ).ToString();
            }

            this.Title = String.Join( " ", split );
        }

        public virtual int GetHairHue()
        {
            return Utility.RandomHairHue();
        }

        public virtual void InitOutfit()
        {
            switch ( Utility.Random( 3 ) )
            {
                case 0: AddItem( new FancyShirt( GetRandomHue() ) ); break;
                case 1: AddItem( new Doublet( GetRandomHue() ) ); break;
                case 2: AddItem( new Shirt( GetRandomHue() ) ); break;
            }

            switch ( ShoeType )
            {
                case VendorShoeType.Shoes: AddItem( new Shoes( GetShoeHue() ) ); break;
                case VendorShoeType.Boots: AddItem( new Boots( GetShoeHue() ) ); break;
                case VendorShoeType.Sandals: AddItem( new Sandals( GetShoeHue() ) ); break;
                case VendorShoeType.ThighBoots: AddItem( new ThighBoots( GetShoeHue() ) ); break;
            }

            int hairHue = GetHairHue();

            Utility.AssignRandomHair( this, hairHue );
            Utility.AssignRandomFacialHair( this, hairHue );

            if ( Female )
            {
                switch ( Utility.Random( 6 ) )
                {
                    case 0: AddItem( new ShortPants( GetRandomHue() ) ); break;
                    case 1:
                    case 2: AddItem( new Kilt( GetRandomHue() ) ); break;
                    case 3:
                    case 4:
                    case 5: AddItem( new Skirt( GetRandomHue() ) ); break;
                }
            }
            else
            {
                switch ( Utility.Random( 2 ) )
                {
                    case 0: AddItem( new LongPants( GetRandomHue() ) ); break;
                    case 1: AddItem( new ShortPants( GetRandomHue() ) ); break;
                }
            }

            PackGold( 100, 200 );
        }

		public override bool HandlesOnSpeech( Mobile from )
		{
			return true;
		}

        public override void OnSpeech(SpeechEventArgs e)
        {
            base.OnSpeech(e);

            if (e.Handled || !e.Mobile.InRange(this, 3))
                return;

            if (!checkAccess(e.Mobile))
                return;

            int[] keywords = e.Keywords;
            string speech = e.Speech.ToLower();

            if (Regex.IsMatch(e.Speech, "zlecen", RegexOptions.IgnoreCase))
            {
                e.Handled = true;

				if (checkWillingness(e.Mobile))
				{
                    if (e.Speech.ToLower() == "zlecen" || Regex.IsMatch(e.Speech, "^zlecen..?$", RegexOptions.IgnoreCase))
                        OnLazySpeech();
                    else
                        ProvideBulkOrder(e.Mobile);
				}
			}
        }
        public void OnLazySpeech() {
            string[] responses = new string[] {
                "To nie karczma! Zachowuj sie chamie niemyty!",
                "Heeeeeee?",
                "Nie rozumiem.",
                "To do mnie mamroczesz?",
                "Sam spierdalaj!",
                "Wypad! Bo wezwe straz!",
                "Ta, jasne.",
                "Coraz glupsi ci mieszczanie.",
                "Terefere",
                "Tiruriru",
                "Co tam belkoczesz pod nosem.",
                "Mow wyrazniej bo nie rozumiem.",
                "Masz jakies uposledzenie umyslowe Panie?",
                "Nie rozumiem o co ci chodzi.",
                "A moze tak troche szacunku?"};
            string response = responses[Utility.Random(responses.Length)];
            Say(response);
        }

        private void ProvideBulkOrder(Mobile m)
        {
			if (SupportsBulkOrders(m))
			{
				TimeSpan ts = GetNextBulkOrder(m);

				int totalSeconds = (int)ts.TotalSeconds;
				int totalHours = (totalSeconds + 3599) / 3600;
				int totalMinutes = (totalSeconds + 59) / 60;

				if (totalMinutes == 0)
				{
					Item bulkOrder = CreateBulkOrder(m, true);

					// wrzuc zlecenie odrazu do plecaka
					if (bulkOrder != null)
					{
						if (m.PlaceInBackpack(bulkOrder))
						{
							m.SendLocalizedMessage(1045152); // The bulk order deed has been placed in your backpack.
						}
						else
						{
							m.SendLocalizedMessage(1045150); // There is not enough room in your backpack for the deed.
							bulkOrder.Delete();
						}
					}
				}
				else
				{
					int oldSpeechHue = SpeechHue;
					SpeechHue = 0x3B2;

					if (Core.SE)
						SayTo(m, 1072058, totalMinutes.ToString()); // An offer may be available in about ~1_minutes~ minutes.
					else
						SayTo(m, 1049039, totalHours.ToString()); // An offer may be available in about ~1_hours~ hours.

					SpeechHue = oldSpeechHue;
				}
			}
		}

        public virtual void Restock()
        {
            m_LastRestock = DateTime.Now;

            IBuyItemInfo[] buyInfo = this.GetBuyInfo();

            foreach ( IBuyItemInfo bii in buyInfo )
                bii.OnRestock();
        }

        public virtual void VendorBuy( Mobile from )
        {
            if ( !IsActiveSeller )
                return;

            if ( !from.CheckAlive() )
                return;

            if ( !CheckVendorAccess( from ) )
            {
                //Say( 501522 ); // I shall not treat with scum like thee!
                return;
            }

            if ( DateTime.Now - m_LastRestock > Config.RespawnItems )
                Restock();

            UpdateBuyInfo(from);

            int count = 0;
            ArrayList list;
            IBuyItemInfo[] buyInfo = this.GetBuyInfo();
            IShopSellInfo[] sellInfo = this.GetSellInfo();

            list = new ArrayList( buyInfo.Length );
            Container cont = this.BuyPack;

            List<ObjectPropertyList> opls = null;

            for (int idx=0;idx<buyInfo.Length;idx++)
            {
                IBuyItemInfo buyItem = (IBuyItemInfo)buyInfo[idx];

                if ( buyItem.Amount <= 0 || list.Count >= 250 )
                    continue;

                // NOTE: Only GBI supported; if you use another implementation of IBuyItemInfo, this will crash
                GenericBuyInfo gbi = (GenericBuyInfo) buyItem;
                IEntity disp = gbi.GetDisplayEntity();

                list.Add( new BuyItemState( buyItem.Name, cont.Serial, disp == null ? (Serial) 0x7FC0FFEE : disp.Serial, buyItem.Price, buyItem.Amount, buyItem.ItemID, buyItem.Hue ) );
                count++;

                if ( opls == null ) {
                    opls = new List<ObjectPropertyList>();
                }

                if ( disp is Item ) {
                    opls.Add( ( ( Item ) disp ).PropertyList );
                } else if ( disp is Mobile ) {
                    opls.Add( ( ( Mobile ) disp ).PropertyList );
                }
            }

            List<Item> playerItems = cont.Items;

            for ( int i = playerItems.Count - 1; i >= 0; --i )
            {
                if ( i >= playerItems.Count )
                    continue;

                Item item = playerItems[i];

                if ( (item.LastMoved + Config.RespawnSellItem) <= DateTime.Now )
                    item.Delete();
            }

            for ( int i = 0; i < playerItems.Count; ++i )
            {
                Item item = playerItems[i];

                int price = 0;
                string name = null;

                foreach( IShopSellInfo ssi in sellInfo )
                {
                    if ( ssi.IsSellable( item ) )
                    {
                        price = ssi.GetBuyPriceFor( item );
                        name = ssi.GetNameFor( item );
                        break;
                    }
                }

                if ( name != null && list.Count < 250 )
                {
                    list.Add( new BuyItemState( name, cont.Serial, item.Serial, price, item.Amount, item.ItemID, item.Hue ) );
                    count++;

                    if ( opls == null ) {
                        opls = new List<ObjectPropertyList>();
                    }

                    opls.Add( item.PropertyList );
                }
            }

            //one (not all) of the packets uses a byte to describe number of items in the list.  Osi = dumb.
            //if ( list.Count > 255 )
            //    Console.WriteLine( "Vendor Warning: Vendor {0} has more than 255 buy items, may cause client errors!", this );

            if ( list.Count > 0 )
            {
                list.Sort( new BuyItemStateComparer() );

                SendPacksTo( from );

                if ( from.NetState == null )
                    return;
                
                if ( from.NetState.IsPost6017 )
                    from.Send( new VendorBuyContent6017( list ) );
                else
                    from.Send( new VendorBuyContent( list ) );
                from.Send( new VendorBuyList( this, list ) );
                from.Send( new DisplayBuyList( this ) );
                from.Send( new MobileStatusExtended( from ) );//make sure their gold amount is sent

                if ( opls != null ) {
                    for ( int i = 0; i < opls.Count; ++i ) {
                        from.Send( opls[i] );
                    }
                }

                SayTo( from, 500186 ); // Greetings.  Have a look around.
            }
        }

        public virtual void SendPacksTo( Mobile from )
        {
            Item pack = FindItemOnLayer( Layer.ShopBuy );

            if ( pack == null )
            {
                pack = new Backpack();
                pack.Layer = Layer.ShopBuy;
                pack.Movable = false;
                pack.Visible = false;
                AddItem( pack );
            }

            from.Send( new EquipUpdate( pack ) );

            pack = FindItemOnLayer( Layer.ShopSell );

            if ( pack != null )
                from.Send( new EquipUpdate( pack ) );

            pack = FindItemOnLayer( Layer.ShopResale );

            if ( pack == null )
            {
                pack = new Backpack();
                pack.Layer = Layer.ShopResale;
                pack.Movable = false;
                pack.Visible = false;
                AddItem( pack );
            }

            from.Send( new EquipUpdate( pack ) );
        }

        public virtual bool AllowLootType(Item item)
        {
            return item.IsStandardLoot();
        }

        public virtual void VendorSell( Mobile from )
        {
            if ( !IsActiveBuyer )
                return;

            if ( !from.CheckAlive() )
                return;

            if ( !CheckVendorAccess( from ) )
            {
                //Say( 501522 ); // I shall not treat with scum like thee!
                return;
            }

            Container pack = from.Backpack;

            if ( pack != null )
            {
                IShopSellInfo[] info = GetSellInfo();

                Hashtable table = new Hashtable();

                foreach ( IShopSellInfo ssi in info )
                {
                    Item[] items = pack.FindItemsByType( ssi.Types );

                    foreach ( Item item in items )
                    {
                        if ( item is Container && ((Container)item).Items.Count != 0 )
                            continue;

                        if ( AllowLootType(item) && item.Movable && ssi.IsSellable( item ) )
                            table[item] = new SellItemState( item, ssi.GetSellPriceFor( item ), ssi.GetNameFor( item ) );
                    }
                }

                if ( table.Count > 0 )
                {
                    SendPacksTo( from );

                    from.Send( new VendorSellList( this, table ) );
                }
                else
                {
                    Say( true, "Nie masz nic, co by mnie interesowalo." );
                }
            }
        }

        public override bool OnDragDrop( Mobile from, Item dropped )
        {
            if ( dropped is SmallBOD || dropped is LargeBOD )
            {
                if (!IsAssignedBuildingWorking())
                {
                    SayTo(from, 1063883); // Miasto nie oplacilo moich uslug. Nieczynne.
                    return false;
                }
                else if ( !IsValidBulkOrder( dropped ))
                {
                    SayTo(from, 1045130); // To zamowienie jest dla jakiegos innego sprzedawcy.
                    return false;
                }
                else if(!SupportsBulkOrders( from ))
                {
                    SayTo(from, 1061065); // Nie znasz sie na rzemiosle. Nie wierze ze moglbys wykonac to zamowienie prawidlowo.
                    return false;
                }
                else if ( (dropped is SmallBOD && !((SmallBOD)dropped).Complete) || (dropped is LargeBOD && !((LargeBOD)dropped).Complete) )
                {
                    SayTo( from, 1045131 ); // You have not completed the order yet.
                    return false;
                }

                Item reward;
                int gold, fame;

                if ( dropped is SmallBOD )
                    ((SmallBOD)dropped).GetRewards( out reward, out gold, out fame );
                else
                    ((LargeBOD)dropped).GetRewards( out reward, out gold, out fame );

                from.SendSound( 0x3D );

                SayTo( from, 1045132 ); // Thank you so much!  Here is a reward for your effort.

                if ( reward != null )
                    from.AddToBackpack( reward );

                if ( gold > 1000 )
                    from.AddToBackpack( new BankCheck( gold ) );
                else if ( gold > 0 )
                    from.AddToBackpack( new Gold( gold ) );

                Titles.AwardFame( from, fame, true );

                OnSuccessfulBulkOrderReceive( from );

                dropped.Delete();
                return true;
            }

            return base.OnDragDrop( from, dropped );
        }

        private GenericBuyInfo LookupDisplayObject( object obj )
        {
            IBuyItemInfo[] buyInfo = this.GetBuyInfo();

            for ( int i = 0; i < buyInfo.Length; ++i ) {
                GenericBuyInfo gbi = (GenericBuyInfo)buyInfo[i];

                if ( gbi.GetDisplayEntity() == obj )
                    return gbi;
            }

            return null;
        }

        private void ProcessSinglePurchase( BuyItemResponse buy, IBuyItemInfo bii, ArrayList validBuy, ref int controlSlots, ref bool fullPurchase, ref int totalCost )
        {
            int amount = buy.Amount;

            if ( amount > bii.Amount )
                amount = bii.Amount;

            if ( amount <= 0 )
                return;

            int slots = bii.ControlSlots * amount;

            if ( controlSlots >= slots )
            {
                controlSlots -= slots;
            }
            else
            {
                fullPurchase = false;
                return;
            }

            totalCost += bii.Price * amount;
            validBuy.Add( buy );
        }

        private void ProcessValidPurchase( int amount, IBuyItemInfo bii, Mobile buyer, Container cont )
        {
            if ( amount > bii.Amount )
                amount = bii.Amount;

            if ( amount < 1 )
                return;

            bii.Amount -= amount;

            IEntity o = bii.GetEntity();

            if ( o is Item )
            {
                Item item = (Item)o;

                if ( item.Stackable )
                {
                    item.Amount = amount;

                    if ( cont == null || !cont.TryDropItem( buyer, item, false ) )
                        item.MoveToWorld( buyer.Location, buyer.Map );
                }
                else
                {
                    item.Amount = 1;

                    if ( cont == null || !cont.TryDropItem( buyer, item, false ) )
                        item.MoveToWorld( buyer.Location, buyer.Map );

                    for (int i=1;i<amount;i++)
                    {
                        item = bii.GetEntity() as Item;

                        if ( item != null )
                        {
                            item.Amount = 1;

                            if ( cont == null || !cont.TryDropItem( buyer, item, false ) )
                                item.MoveToWorld( buyer.Location, buyer.Map );
                        }
                    }
                }
            }
            else if ( o is Mobile )
            {
                Mobile m = (Mobile)o;

                m.Direction = (Direction)Utility.Random( 8 );
                m.MoveToWorld( buyer.Location, buyer.Map );
                m.PlaySound( m.GetIdleSound() );

                if ( m is BaseCreature )
                    ((BaseCreature)m).SetControlMaster( buyer );

                for ( int i = 1; i < amount; ++i )
                {
                    m = bii.GetEntity() as Mobile;

                    if ( m != null )
                    {
                        m.Direction = (Direction)Utility.Random( 8 );
                        m.MoveToWorld( buyer.Location, buyer.Map );

                        if ( m is BaseCreature )
                            ((BaseCreature)m).SetControlMaster( buyer );
                    }
                }
            }
        }

        public virtual bool OnBuyItems( Mobile buyer, ArrayList list )
        {
            if ( !IsActiveSeller )
                return false;

            if ( !buyer.CheckAlive() )
                return false;

            if ( !CheckVendorAccess( buyer ) )
            {
                //Say( 501522 ); // I shall not treat with scum like thee!
                return false;
            }

            UpdateBuyInfo(buyer);

            IBuyItemInfo[] buyInfo = this.GetBuyInfo();
            IShopSellInfo[] info = GetSellInfo();
            int totalCost = 0;
            ArrayList validBuy = new ArrayList( list.Count );
            Container cont;
            bool bought = false;
            bool fromBank = false;
            bool fullPurchase = true;
            int controlSlots = buyer.FollowersMax - buyer.Followers;

            foreach ( BuyItemResponse buy in list )
            {
                Serial ser = buy.Serial;
                int amount = buy.Amount;

                if ( ser.IsItem )
                {
                    Item item = World.FindItem( ser );

                    if ( item == null )
                        continue;

                    GenericBuyInfo gbi = LookupDisplayObject( item );

                    if ( gbi != null )
                    {
                        ProcessSinglePurchase( buy, gbi, validBuy, ref controlSlots, ref fullPurchase, ref totalCost );
                    }
                    else if ( item != this.BuyPack && item.IsChildOf( this.BuyPack ) )
                    {
                        if ( amount > item.Amount )
                            amount = item.Amount;

                        if ( amount <= 0 )
                            continue;

                        foreach ( IShopSellInfo ssi in info )
                        {
                            if ( ssi.IsSellable( item ) )
                            {
                                if ( ssi.IsResellable( item ) )
                                {
                                    totalCost += ssi.GetBuyPriceFor( item ) * amount;
                                    validBuy.Add( buy );
                                    break;
                                }
                            }
                        }
                    }
                }
                else if ( ser.IsMobile )
                {
                    Mobile mob = World.FindMobile( ser );

                    if ( mob == null )
                        continue;

                    GenericBuyInfo gbi = LookupDisplayObject( mob );

                    if ( gbi != null )
                        ProcessSinglePurchase( buy, gbi, validBuy, ref controlSlots, ref fullPurchase, ref totalCost );
                }
            }//foreach

            if ( fullPurchase && validBuy.Count == 0 )
                SayTo( buyer, 500190 ); // Thou hast bought nothing!
            else if ( validBuy.Count == 0 )
                SayTo( buyer, 500187 ); // Your order cannot be fulfilled, please try again.

            if ( validBuy.Count == 0 )
                return false;

            bought = ( buyer.AccessLevel >= AccessLevel.GameMaster );

            cont = buyer.Backpack;
            if ( !bought && cont != null )
            {
                if ( cont.ConsumeTotal( this.Currency, totalCost ) )
                    bought = true;
                else if ( totalCost < 2000 )
                    SayTo( buyer, 500192 );//Begging thy pardon, but thou casnt afford that.
            }

            if ( !bought && totalCost >= 2000 )
            {
                cont = buyer.FindBankNoCreate();
                if ( cont != null && cont.ConsumeTotal( this.Currency, totalCost ) )
                {
                    bought = true;
                    fromBank = true;
                }
                else
                {
                    SayTo( buyer, 500191 ); //Begging thy pardon, but thy bank account lacks these funds.
                }
            }

            if ( !bought )
                return false;
            else
                buyer.PlaySound( 0x32 );

            cont = buyer.Backpack;
            if ( cont == null )
                cont = buyer.BankBox;

            foreach ( BuyItemResponse buy in validBuy )
            {
                Serial ser = buy.Serial;
                int amount = buy.Amount;

                if ( amount < 1 )
                    continue;

                if ( ser.IsItem )
                {
                    Item item = World.FindItem( ser );

                    if ( item == null )
                        continue;

                    GenericBuyInfo gbi = LookupDisplayObject( item );

                    if ( gbi != null )
                    {
                        ProcessValidPurchase( amount, gbi, buyer, cont );
                    }
                    else
                    {
                        if ( amount > item.Amount )
                            amount = item.Amount;

                        foreach ( IShopSellInfo ssi in info )
                        {
                            if ( ssi.IsSellable( item ) )
                            {
                                if ( ssi.IsResellable( item ) )
                                {
                                    Item buyItem;
                                    if ( amount >= item.Amount )
                                    {
                                        buyItem = item;
                                    }
                                    else
                                    {
                                        buyItem = Mobile.LiftItemDupe( item, item.Amount - amount );

                                        if ( buyItem == null )
                                            buyItem = item;
                                    }

                                    if ( cont == null || !cont.TryDropItem( buyer, buyItem, false ) )
                                        buyItem.MoveToWorld( buyer.Location, buyer.Map );

                                    break;
                                }
                            }
                        }
                    }
                }
                else if ( ser.IsMobile )
                {
                    Mobile mob = World.FindMobile( ser );

                    if ( mob == null )
                        continue;

                    GenericBuyInfo gbi = LookupDisplayObject( mob );

                    if ( gbi != null )
                        ProcessValidPurchase( amount, gbi, buyer, cont );
                }
            }//foreach

            if ( fullPurchase )
			{
				if ( buyer.AccessLevel >= AccessLevel.GameMaster )
					SayTo( buyer, 00505135 ); // Nie wezme od Ciebie ani centara. Tu masz dobra, ktorych potrzebujesz.
				else if ( fromBank )
					SayTo( buyer, 00505136, totalCost.ToString() ); // Zaplate, ~1_GOLD~ centarow, pobiore u Twego bankiera. Milo jest robic z Toba interesy.
				else
					SayTo( buyer, 00505137, totalCost.ToString() ); // Zaplata, to ~1_GOLD~ centarow. Milo jest robic z Toba interesy.
			}
			else
			{
				if ( buyer.AccessLevel >= AccessLevel.GameMaster )
					SayTo( buyer, 00505138 ); // Nie wezme od Ciebie ani centara. Niestety nie mam wszystkich dobr, ktorych potrzebujesz.
				else if ( fromBank )
					SayTo( buyer, 00505139, totalCost.ToString() ); // Zaplate, ~1_GOLD~ centarow, pobiore u Twego bankiera. Milo jest robic z Toba interesy. Niestety nie mam wszystkich dobr, ktorych potrzebujesz.
				else
					SayTo( buyer, 00505140, totalCost.ToString() ); // Zaplata, to ~1_GOLD~ centarow. Milo jest robic z Toba interesy. Niestety nie mam wszystkich dobr, ktorych potrzebujesz.
			}

            // Miasta - przelej pieniadze do skarbca lub ze skarbca
            int paidToVault = PayToVault(buyer, totalCost);
            if (paidToVault > 0)
            {
                SayTo(buyer, 1063940); // Skarbiec miasta zostal zasilony podatkiem
            }
            else if (paidToVault < 0)
            {
                SayTo(buyer, 1063941); // Skarbiec miasta zostal obiazony upustem
            }

            return true;
        }

        // 30.06.2012 :: zombie

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Smuggler
		{
			get { return TownsVendor.Get(this).TradesWithCriminals; }
			set { TownsVendor.Get( this ).TradesWithCriminals = value; }
		}

        // blokada nietolerancji
        private bool m_IntolerativeBlocked;

        [CommandProperty( AccessLevel.GameMaster )]
        public bool Blocked
        {
            get { return m_IntolerativeBlocked; }
            set { m_IntolerativeBlocked = value; }
        }

        #region Miasto

        [CommandProperty(AccessLevel.Administrator)]
        public Towns TownAssigned
        {
            get { return TownsVendor.Get( this ).TownAssigned; }
            set { TownsVendor.Get( this ).TownAssigned = value; }
        }

        [CommandProperty(AccessLevel.Administrator)]
        public TownBuildingName TownBuildingAssigned
        {
            get { return TownsVendor.Get( this ).TownBuildingAssigned; }
            set { TownsVendor.Get( this ).TownBuildingAssigned = value; }
        }

        public bool IsAssignedBuildingWorking()
        {
            if (TownAssigned == Towns.None)
            {
                return true;
            }
            else
            {
                if (TownDatabase.GetBuildingStatus(TownAssigned, TownBuildingAssigned) == TownBuildingStatus.Dziala)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion

        private IntoleranceBlockTimer m_BlockTimer;

        private class IntoleranceBlockTimer : Timer
        {
            private BaseVendor m_Blocked;

            public IntoleranceBlockTimer( BaseVendor blocked ) : base( TimeSpan.FromSeconds( 20 + Utility.Random( 340 ) ) )
            {
                m_Blocked = blocked;
                Priority = TimerPriority.FiveSeconds;
                m_Blocked.Blocked = true;
            }

            protected override void OnTick()
            {
                try
                {
                    if ( !m_Blocked.Deleted )
                    {
                        m_Blocked.Blocked = false;
                        m_Blocked.Say( 00505141 ); // Mam nadzieje, ze nie bede mial wiecej niechcianych klientow!
                    }
                }
                catch ( Exception e )
                {
                    Console.WriteLine( e.ToString() );
                }
            }
        }

        private class IntoleranceGuardTimer : Timer
        {
            private Mobile m_target;
            private Mobile m_source;

            public IntoleranceGuardTimer( Mobile target, Mobile source ) : base( TimeSpan.FromSeconds( 20 ) )
            {
                m_target = target;
                m_source = source;
                Priority = TimerPriority.FiveSeconds;
                target.SendLocalizedMessage( 00505144, "", 0x25 ); // Straz niezdrowo sie Toba interesuje! Lepiej zejdz jej z oczu!
            }

            protected override void OnTick()
            {
                try
                {
                    if ( !m_target.Deleted )
                    {
                        m_target.Criminal = true;
                        m_target.SendLocalizedMessage( 505133, "", 0x25 ); // Zdaje sie, ze popadles w tarapaty! Kryminalisto!
                    }
                }
                catch ( Exception e )
                {
                    Console.WriteLine( e.ToString() );
                }
            }
        }

        public bool checkAccess(Mobile from)
        {
			if (!(from is PlayerMobile))
				return false;

			PlayerMobile pm = from as PlayerMobile;

			if (pm.AccessLevel > AccessLevel.Player)
				return true;

			if (!CanSee(pm) || !InLOS(pm))
			{
				this.Emote(505163);
				return false;
			}

            return true;
		}

        public bool checkWillingness(Mobile from, bool allowMounted = false)
        {
            try
            {
                // Nie jest graczem
                if (!(from is PlayerMobile))
                {
                    this.Yell(00505124); // Odejdz!
                    return false;
                }

                PlayerMobile pm = from as PlayerMobile;

                // Jest GMem
                if (pm.AccessLevel > AccessLevel.Player)
                    return true;

                // Jest w trybie walki
                if (pm.Warmode)
                {
                    this.Yell(00505125, from.Race.GetName(Cases.Wolacz)); // Odloz bron ~1_RACE~!
                    return false;
                }
                
                if(!CanSee(pm) || !InLOS(pm)) {
                    this.Emote(505163);
                    return false;
                }

                if (!allowMounted)
                {
                    // Konno
                    if (pm.Mount != null)
                    {
                        this.Yell(00505126, from.Race.GetName(Cases.Wolacz)); // Zejdz z wierzchowca ~1_RACE~ nim sie do mnie odezwiesz!
                        return false;
                    }
                }

                // Przestepcy
                if ((from.Kills >= 5 || from.Criminal) && !Smuggler)
                {
                    this.Yell(00505127); // Takich jak ty tu nie obslugujemy!
                    return false;
                }

                // Nietolerancja
                if (RegionsEngine.ActIntolerativeHarmful(this, from, false))
                {
                    if (this.Blocked)
                    {
                        new IntoleranceGuardTimer(from, this).Start();
                        this.Yell(00505128, from.Race.GetName(Cases.Biernik)); // Mam dosc brudasow szwendajacych sie po okolicy! Straaaaaz!!! Lapac ~1_RACE~!

                        return false;
                    }
                    else
                    {
                        this.Yell(00505129, from.Race.GetName(Cases.Mianownik)); // Kolejny ~1_RACE~ smie zawracac mi glowe! Won!

                        m_BlockTimer = new IntoleranceBlockTimer(this);
                        m_BlockTimer.Start();

                        return false;
                    }
                }

                // Dzialanie budynku
                if (!IsAssignedBuildingWorking())
                {
                    Say(1063883); // Miasto nie oplacilo moich uslug. Nieczynne.
                    return false;
                }

                if (this.Blocked && this.Race != from.Race)
                {
                    m_BlockTimer.Stop();
                    m_BlockTimer = new IntoleranceBlockTimer(this);
                    m_BlockTimer.Start();
                    this.Yell(00505130, from.Race.GetName(Cases.Wolacz)); // Won! Won! Wooooon! ~1_RACE~!
                    return false;
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.ToString());
            }

            return true;
        }

        public virtual bool CheckVendorAccess( Mobile from )
        {
            return checkWillingness(from, false);
        }
        // zombie

        public virtual bool OnSellItems( Mobile seller, ArrayList list )
        {
            if ( !IsActiveBuyer )
                return false;

            if ( !seller.CheckAlive() )
                return false;

            if ( !CheckVendorAccess( seller ) )
            {
                Say( 501522 ); // I shall not treat with scum like thee!
                return false;
            }

            seller.PlaySound( 0x32 );

            IShopSellInfo[] info = GetSellInfo();
            IBuyItemInfo[] buyInfo = this.GetBuyInfo();
            int GiveCurrency = 0;
            int Sold = 0;
            Container cont;
            ArrayList delete = new ArrayList();
            ArrayList drop = new ArrayList();

            foreach ( SellItemResponse resp in list )
            {
                if ( resp.Item.RootParent != seller || resp.Amount <= 0 || !AllowLootType(resp.Item) || !resp.Item.Movable || (resp.Item is Container && ((Container)resp.Item).Items.Count != 0) )
                    continue;

                foreach( IShopSellInfo ssi in info )
                {
                    if ( ssi.IsSellable( resp.Item ) )
                    {
                        Sold++;
                        break;
                    }
                }
            }

            if ( Sold > MaxSell )
            {
                SayTo( seller, true, "You may only sell {0} items at a time!", MaxSell );
                return false;
            } 
            else if ( Sold == 0 )
            {
                return true;
            }

            foreach ( SellItemResponse resp in list )
            {
                if ( resp.Item.RootParent != seller || resp.Amount <= 0 || !AllowLootType(resp.Item) || !resp.Item.Movable || (resp.Item is Container && ((Container)resp.Item).Items.Count != 0) )
                    continue;

                foreach( IShopSellInfo ssi in info )
                {
                    if ( ssi.IsSellable( resp.Item ) )
                    {
                        int amount = resp.Amount;

                        if ( amount > resp.Item.Amount )
                            amount = resp.Item.Amount;

                        if ( ssi.IsResellable( resp.Item ) )
                        {
                            bool found = false;

                            foreach ( IBuyItemInfo bii in buyInfo )
                            {
                                if ( bii.Restock( resp.Item, amount ) )
                                {
                                    resp.Item.Consume( amount );
                                    found = true;

                                    break;
                                }
                            }

                            if ( !found )
                            {
                                cont = this.BuyPack;

                                if ( amount < resp.Item.Amount )
                                {
                                    Item item = Mobile.LiftItemDupe( resp.Item, resp.Item.Amount - amount );

                                    if ( item != null )
                                    {
                                        item.SetLastMoved();
                                        cont.DropItem( item );
                                    }
                                    else
                                    {
                                        resp.Item.SetLastMoved();
                                        cont.DropItem( resp.Item );
                                    }
                                }
                                else
                                {
                                    resp.Item.SetLastMoved();
                                    cont.DropItem( resp.Item );
                                }
                            }
                        }
                        else
                        {
                            if ( amount < resp.Item.Amount )
                                resp.Item.Amount -= amount;
                            else
                                resp.Item.Delete();
                        }

                        GiveCurrency += ssi.GetSellPriceFor( resp.Item )*amount;
                        break;
                    }
                }
            }

            if ( GiveCurrency > 0 )
            {
                while ( GiveCurrency > 60000 )
                {
                    seller.AddToBackpack( Activator.CreateInstance(this.Currency, 60000) as Item);
                    GiveCurrency -= 60000;
                }

                seller.AddToBackpack( Activator.CreateInstance(this.Currency, GiveCurrency) as Item );

                seller.PlaySound( 0x0037 );//Gold dropping sound

                if ( SupportsBulkOrders( seller ) )
                {
                    Item bulkOrder = CreateBulkOrder( seller, false );

                    if ( bulkOrder is LargeBOD )
                        seller.SendGump( new LargeBODAcceptGump( seller, (LargeBOD)bulkOrder ) );
                    else if ( bulkOrder is SmallBOD )
                        seller.SendGump( new SmallBODAcceptGump( seller, (SmallBOD)bulkOrder ) );
                }
            }
            //no cliloc for this?
            //SayTo( seller, true, "Thank you! I bought {0} item{1}. Here is your {2}gp.", Sold, (Sold > 1 ? "s" : ""), GiveGold );
            
            return true;
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int) 1 ); // version

            //writer.Write(m_TradesWithCriminals);

            //writer.Write((int)m_TownAssigned);
            //writer.Write((int)m_TownBuildingAssigned);

            ArrayList sbInfos = this.SBInfos;

            for ( int i = 0; sbInfos != null && i < sbInfos.Count; ++i )
            {
                SBInfo sbInfo = (SBInfo)sbInfos[i];
                ArrayList buyInfo = sbInfo.BuyInfo;

                for ( int j = 0; buyInfo != null && j < buyInfo.Count; ++j )
                {
                    GenericBuyInfo gbi = (GenericBuyInfo)buyInfo[j];

                    int maxAmount = gbi.MaxAmount;
                    int doubled = 0;

                    switch ( maxAmount )
                    {
                        case  40: doubled = 1; break;
                        case  80: doubled = 2; break;
                        case 160: doubled = 3; break;
                        case 320: doubled = 4; break;
                        case 640: doubled = 5; break;
                        case 999: doubled = 6; break;
                    }

                    if ( doubled > 0 )
                    {
                        writer.WriteEncodedInt( 1 + ((j * sbInfos.Count) + i) );
                        writer.WriteEncodedInt( doubled );
                    }
                }
            }

            writer.WriteEncodedInt( 0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            LoadSBInfo();

            ArrayList sbInfos = this.SBInfos;

            switch ( version )
            {
                case 4:
                case 3:
                {
                    if(version == 3)
                        Smuggler = reader.ReadBool();
                    goto case 2;
                }
                case 2:
                {
                    if ( version == 3 )
                    {
                        TownAssigned = (Towns)reader.ReadInt();
                        TownBuildingAssigned = (TownBuildingName)reader.ReadInt();
                    }
                    goto case 1;
                }
                case 1:
                {
                    int index;

                    while ( (index = reader.ReadEncodedInt()) > 0 )
                    {
                        int doubled = reader.ReadEncodedInt();

                        if ( sbInfos != null )
                        {
                            index -= 1;
                            int sbInfoIndex = index % sbInfos.Count;
                            int buyInfoIndex = index / sbInfos.Count;
 
                            if ( sbInfoIndex >= 0 && sbInfoIndex < sbInfos.Count )
                            {
                                SBInfo sbInfo = (SBInfo)sbInfos[sbInfoIndex];
                                ArrayList buyInfo = sbInfo.BuyInfo;

                                if ( buyInfo != null && buyInfoIndex >= 0 && buyInfoIndex < buyInfo.Count )
                                {
                                    GenericBuyInfo gbi = (GenericBuyInfo)buyInfo[buyInfoIndex];

                                    int amount = 20;

                                    switch ( doubled )
                                    {
                                        case 1: amount =  40; break;
                                        case 2: amount =  80; break;
                                        case 3: amount = 160; break;
                                        case 4: amount = 320; break;
                                        case 5: amount = 640; break;
                                        case 6: amount = 999; break;
                                    }

                                    gbi.Amount = gbi.MaxAmount = amount;
                                }
                            }
                        }
                    }

                    break;
                }
            }

            if ( IsParagon )
                IsParagon = false;

            if ( Core.AOS && NameHue == 0x35 )
                NameHue = -1;

            Timer.DelayCall( TimeSpan.Zero, new TimerCallback( CheckMorph ) );
        }

        public override void AddCustomContextEntries( Mobile from, List<ContextMenuEntry> list )
        {
            if ( from.Alive && IsActiveVendor && IsAssignedBuildingWorking())
            {
                if ( IsActiveSeller )
                    list.Add( new VendorBuyEntry( from, this ) );

                if ( IsActiveBuyer )
                    list.Add( new VendorSellEntry( from, this ) );

                if ( SupportsBulkOrders( from ) )
                    list.Add( new BulkOrderInfoEntry( from, this ) );

                // 26.06.2012 :: zombie :: system plotek
                list.Add( new VendorRumorsEntry( from, this ) );
                // zombie
            }

            base.AddCustomContextEntries( from, list );
        }

        public virtual IShopSellInfo[] GetSellInfo()
        {
            return (IShopSellInfo[])m_ArmorSellInfo.ToArray( typeof( IShopSellInfo ) );
        }

        public virtual IBuyItemInfo[] GetBuyInfo()
        {
            return (IBuyItemInfo[])m_ArmorBuyInfo.ToArray( typeof( IBuyItemInfo ) );
        }

        public override bool CanBeDamaged()
        {
            return !IsInvulnerable;
        }
    }
}

namespace Server.ContextMenus
{
    public class VendorBuyEntry : ContextMenuEntry
    {
        private Mobile m_From;
        private BaseVendor m_Vendor;

        public VendorBuyEntry( Mobile from, BaseVendor vendor ) : base( 6103, 8 )
        {
            m_From = from;
            m_Vendor = vendor;
            //Enabled = vendor.CheckVendorAccess( from );
        }

        public override void OnClick()
        {
            m_From.Say( 00505101 ); // Witaj, chcialbym kupic u Ciebie kilka drobiazgow
            m_Vendor.VendorBuy( this.Owner.From );
        }
    }

    public class VendorSellEntry : ContextMenuEntry
    {
        private Mobile m_From;
        private BaseVendor m_Vendor;

        public VendorSellEntry( Mobile from, BaseVendor vendor ) : base( 6104, 8 )
        {
            m_From = from;
            m_Vendor = vendor;
            //Enabled = vendor.CheckVendorAccess( from );
        }

        public override void OnClick()
        {
            m_From.Say( 00505102 ); // Witaj, chcialbym cos Ci sprzedac
            m_Vendor.VendorSell( this.Owner.From );
        }
    }

    // zombie 26.06.2012 :: zombie :: system plotek
    public class VendorRumorsEntry : ContextMenuEntry
    {
        private Mobile m_From;
        private BaseVendor m_Vendor;

        public VendorRumorsEntry( Mobile from, BaseVendor vendor ) : base( 50019, 4 )
        {
            m_From = from;
            m_Vendor = vendor;
            
            // Enabled = vendor.CheckVendorAccess( from );
        }

        public override void OnClick()
        {
            m_From.Say( 505521 ); // Powitac! I coz ciekawego w swiecie?
            m_Vendor.SayAboutRumors( m_From );
        }
    }
    // zombie
}

namespace Server
{
    public interface IShopSellInfo
    {
        //get display name for an item
        string GetNameFor( Item item );

        //get price for an item which the player is selling
        int GetSellPriceFor( Item item );

        //get price for an item which the player is buying
        int GetBuyPriceFor( Item item );

        //can we sell this item to this vendor?
        bool IsSellable( Item item );

        //What do we sell?
        Type[] Types{ get; }

        //does the vendor resell this item?
        bool IsResellable( Item item );
    }

    public interface IBuyItemInfo
    {
        //get a new instance of an object (we just bought it)
        IEntity GetEntity();

        int ControlSlots{ get; }

        int PriceScalar{ get; set; }

        //display price of the item
        int Price{ get; }

        //display name of the item
        string Name{ get; }

        //display hue
        int Hue{ get; }

        //display id
        int ItemID{ get; }

        //amount in stock
        int Amount{ get; set; }

        //max amount in stock
        int MaxAmount{ get; }

        //Attempt to restock with item, (return true if restock sucessful)
        bool Restock( Item item, int amount );

        //called when its time for the whole shop to restock
        void OnRestock();
    }
}