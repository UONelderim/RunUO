using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Accounting;
using Server.Engines.XmlSpawner2;

namespace Server.Misc
{
    public class CharacterCreation
    {
        public static void Initialize()
        {
            // Register our event handler
            EventSink.CharacterCreated += new CharacterCreatedEventHandler( EventSink_CharacterCreated );
        }

        private static void AddBackpack( Mobile m )
        {
            Container pack = m.Backpack;

            if ( pack == null )
            {
                pack = new Backpack();
                pack.Movable = false;

                m.AddItem( pack );
            }

            PackItem( m, new RedBook( "ksiazka", m.Name, 20, true ) );
            PackItem( m, new BankCheck( 5000 ) ); // Starting gold can be customized here
            PackItem( m, new Dagger() );
            PackItem( m, new Candle() );
        }

        private static Item MakeNewbie( Item item )
        {
            if ( !Core.AOS )
                item.LootType = LootType.Newbied;

            return item;
        }

        private static void PlaceItemIn( Container parent, int x, int y, Item item )
        {
            parent.AddItem( item );
            item.Location = new Point3D( x, y, 0 );
        }

        private static Item MakePotionKeg( PotionEffect type, int hue )
        {
            PotionKeg keg = new PotionKeg();

            keg.Held = 100;
            keg.Type = type;
            keg.Hue = hue;

            return MakeNewbie( keg );
        }

        private static void FillBankAOS( Mobile m )
        {
            BankBox bank = m.BankBox;

            // The new AOS bankboxes don't have powerscrolls, they are automatically 'applied':

            for ( int i = 0; i < PowerScroll.Skills.Length; ++i )
                m.Skills[PowerScroll.Skills[ i ]].Cap = 120.0;

            m.StatCap = 250;


            Container cont;


            // Begin box of money
            cont = new WoodenBox();
            cont.ItemID = 0xE7D;
            cont.Hue = 0x489;

            PlaceItemIn( cont, 16, 51, new BankCheck( 500000 ) );
            PlaceItemIn( cont, 28, 51, new BankCheck( 250000 ) );
            PlaceItemIn( cont, 40, 51, new BankCheck( 100000 ) );
            PlaceItemIn( cont, 52, 51, new BankCheck( 100000 ) );
            PlaceItemIn( cont, 64, 51, new BankCheck(  50000 ) );

            PlaceItemIn( cont, 16, 115, new Factions.Silver( 9000 ) );
            PlaceItemIn( cont, 34, 115, new Gold( 60000 ) );

            PlaceItemIn( bank, 18, 169, cont );
            // End box of money


            // Begin bag of potion kegs
            cont = new Backpack();
            cont.Name = "Various Potion Kegs";

            PlaceItemIn( cont,  45, 149, MakePotionKeg( PotionEffect.CureGreater, 0x2D ) );
            PlaceItemIn( cont,  69, 149, MakePotionKeg( PotionEffect.HealGreater, 0x499 ) );
            PlaceItemIn( cont,  93, 149, MakePotionKeg( PotionEffect.PoisonDeadly, 0x46 ) );
            PlaceItemIn( cont, 117, 149, MakePotionKeg( PotionEffect.RefreshTotal, 0x21 ) );
            PlaceItemIn( cont, 141, 149, MakePotionKeg( PotionEffect.ExplosionGreater, 0x74 ) );

            PlaceItemIn( cont, 93, 82, new Bottle( 1000 ) );

            PlaceItemIn( bank, 53, 169, cont );
            // End bag of potion kegs


            // Begin bag of tools
            cont = new Bag();
            cont.Name = "Tool Bag";

            PlaceItemIn( cont, 30,  35, new TinkerTools( 1000 ) );
            PlaceItemIn( cont, 60,  35, new HousePlacementTool() );
            PlaceItemIn( cont, 90,  35, new DovetailSaw( 1000 ) );
            PlaceItemIn( cont, 30,  68, new Scissors() );
            PlaceItemIn( cont, 45,  68, new MortarPestle( 1000 ) );
            PlaceItemIn( cont, 75,  68, new ScribesPen( 1000 ) );
            PlaceItemIn( cont, 90,  68, new SmithHammer( 1000 ) );
            PlaceItemIn( cont, 30, 118, new TwoHandedAxe() );
            PlaceItemIn( cont, 60, 118, new FletcherTools( 1000 ) );
            PlaceItemIn( cont, 90, 118, new SewingKit( 1000 ) );

            PlaceItemIn( cont, 36, 51, new RunicHammer( CraftResource.DullCopper, 1000 ) );
            PlaceItemIn( cont, 42, 51, new RunicHammer( CraftResource.ShadowIron, 1000 ) );
            PlaceItemIn( cont, 48, 51, new RunicHammer( CraftResource.Copper, 1000 ) );
            PlaceItemIn( cont, 54, 51, new RunicHammer( CraftResource.Bronze, 1000 ) );
            PlaceItemIn( cont, 61, 51, new RunicHammer( CraftResource.Gold, 1000 ) );
            PlaceItemIn( cont, 67, 51, new RunicHammer( CraftResource.Agapite, 1000 ) );
            PlaceItemIn( cont, 73, 51, new RunicHammer( CraftResource.Verite, 1000 ) );
            PlaceItemIn( cont, 79, 51, new RunicHammer( CraftResource.Valorite, 1000 ) );

            PlaceItemIn( cont, 36, 55, new RunicSewingKit( CraftResource.SpinedLeather, 1000 ) );
            PlaceItemIn( cont, 42, 55, new RunicSewingKit( CraftResource.HornedLeather, 1000 ) );
            PlaceItemIn( cont, 48, 55, new RunicSewingKit( CraftResource.BarbedLeather, 1000 ) );

            PlaceItemIn( bank, 118, 169, cont );
            // End bag of tools


            // Begin bag of archery ammo
            cont = new Bag();
            cont.Name = "Bag Of Archery Ammo";

            PlaceItemIn( cont, 48, 76, new Arrow( 5000 ) );
            PlaceItemIn( cont, 72, 76, new Bolt( 5000 ) );

            PlaceItemIn( bank, 118, 124, cont );
            // End bag of archery ammo


            // Begin bag of treasure maps
            cont = new Bag();
            cont.Name = "Bag Of Treasure Maps";

            PlaceItemIn(cont, 30, 35, new TreasureMap(1, Map.Felucca));
            PlaceItemIn(cont, 45, 35, new TreasureMap(2, Map.Felucca));
            PlaceItemIn(cont, 60, 35, new TreasureMap(3, Map.Felucca));
            PlaceItemIn(cont, 75, 35, new TreasureMap(4, Map.Felucca));
            PlaceItemIn(cont, 90, 35, new TreasureMap(5, Map.Felucca));
            PlaceItemIn(cont, 90, 35, new TreasureMap(6, Map.Felucca));

            PlaceItemIn(cont, 30, 50, new TreasureMap(1, Map.Felucca));
            PlaceItemIn(cont, 45, 50, new TreasureMap(2, Map.Felucca));
            PlaceItemIn(cont, 60, 50, new TreasureMap(3, Map.Felucca));
            PlaceItemIn(cont, 75, 50, new TreasureMap(4, Map.Felucca));
            PlaceItemIn(cont, 90, 50, new TreasureMap(5, Map.Felucca));
            PlaceItemIn(cont, 90, 50, new TreasureMap(6, Map.Felucca));

            PlaceItemIn( cont, 55, 100, new Lockpick( 30 ) );
            PlaceItemIn( cont, 60, 100, new Pickaxe() );

            PlaceItemIn( bank, 98, 124, cont );
            // End bag of treasure maps


            // Begin bag of raw materials
            cont = new Bag();
            cont.Hue = 0x835;
            cont.Name = "Raw Materials Bag";

            PlaceItemIn( cont, 92, 60, new BarbedLeather( 5000 ) );
            PlaceItemIn( cont, 92, 68, new HornedLeather( 5000 ) );
            PlaceItemIn( cont, 92, 76, new SpinedLeather( 5000 ) );
            PlaceItemIn( cont, 92, 84, new Leather( 5000 ) );

            PlaceItemIn( cont, 30, 118, new Cloth( 5000 ) );
            PlaceItemIn( cont, 30,  84, new Board( 5000 ) );
            PlaceItemIn( cont, 57,  80, new BlankScroll( 500 ) );

            PlaceItemIn( cont, 30,  35, new DullCopperIngot( 5000 ) );
            PlaceItemIn( cont, 37,  35, new ShadowIronIngot( 5000 ) );
            PlaceItemIn( cont, 44,  35, new CopperIngot( 5000 ) );
            PlaceItemIn( cont, 51,  35, new BronzeIngot( 5000 ) );
            PlaceItemIn( cont, 58,  35, new GoldIngot( 5000 ) );
            PlaceItemIn( cont, 65,  35, new AgapiteIngot( 5000 ) );
            PlaceItemIn( cont, 72,  35, new VeriteIngot( 5000 ) );
            PlaceItemIn( cont, 79,  35, new ValoriteIngot( 5000 ) );
            PlaceItemIn( cont, 86,  35, new IronIngot( 5000 ) );

            PlaceItemIn( cont, 30,  59, new RedScales( 5000 ) );
            PlaceItemIn( cont, 36,  59, new YellowScales( 5000 ) );
            PlaceItemIn( cont, 42,  59, new BlackScales( 5000 ) );
            PlaceItemIn( cont, 48,  59, new GreenScales( 5000 ) );
            PlaceItemIn( cont, 54,  59, new WhiteScales( 5000 ) );
            PlaceItemIn( cont, 60,  59, new BlueScales( 5000 ) );

            PlaceItemIn( bank, 98, 169, cont );
            // End bag of raw materials


            // Begin bag of spell casting stuff
            cont = new Backpack();
            cont.Hue = 0x480;
            cont.Name = "Spell Casting Stuff";

            PlaceItemIn( cont, 45, 105, new Spellbook( UInt64.MaxValue ) );
            PlaceItemIn( cont, 65, 105, new NecromancerSpellbook( (UInt64)0xFFFF ) );
            PlaceItemIn( cont, 85, 105, new BookOfChivalry( (UInt64)0x3FF ) );
            PlaceItemIn( cont, 105, 105, new BookOfBushido() );    //Default ctor = full
            PlaceItemIn( cont, 125, 105, new BookOfNinjitsu() ); //Default ctor = full

            Runebook runebook = new Runebook( 10 );
            runebook.CurCharges = runebook.MaxCharges;
            PlaceItemIn( cont, 145, 105, runebook );

            Item toHue = new BagOfReagents( 150 );
            toHue.Hue = 0x2D;
            PlaceItemIn( cont, 45, 150, toHue );

            toHue = new BagOfNecroReagents( 150 );
            toHue.Hue = 0x488;
            PlaceItemIn( cont, 65, 150, toHue );

            PlaceItemIn( cont, 140, 150, new BagOfAllReagents( 500 ) );

            for ( int i = 0; i < 9; ++i )
                PlaceItemIn( cont, 45 + (i * 10), 75, new RecallRune() );

            PlaceItemIn( cont, 141, 74, new FireHorn() );

            PlaceItemIn( bank, 78, 169, cont );
            // End bag of spell casting stuff


            // Begin bag of ethereals
            cont = new Backpack();
            cont.Hue = 0x490;
            cont.Name = "Bag Of Ethy's!";

            PlaceItemIn( cont, 45, 66, new EtherealHorse() );
            PlaceItemIn( cont, 69, 82, new EtherealOstard() );
            PlaceItemIn( cont, 93, 99, new EtherealLlama() );
            PlaceItemIn( cont, 117, 115, new EtherealKirin() );
            PlaceItemIn( cont, 45, 132, new EtherealUnicorn() );
            PlaceItemIn( cont, 69, 66, new EtherealRidgeback() );
            PlaceItemIn( cont, 93, 82, new EtherealSwampDragon() );
            PlaceItemIn( cont, 117, 99, new EtherealBeetle() );

            PlaceItemIn( bank, 38, 124, cont );
            // End bag of ethereals


            // Begin first bag of artifacts
            cont = new Backpack();
            cont.Hue = 0x48F;
            cont.Name = "Bag of Artifacts";

            PlaceItemIn( cont, 45, 66, new TitansHammer() );
            PlaceItemIn( cont, 69, 82, new InquisitorsResolution() );
            PlaceItemIn( cont, 93, 99, new BladeOfTheRighteous() );
            PlaceItemIn( cont, 117, 115, new ZyronicClaw() );

            PlaceItemIn( bank, 58, 124, cont );
            // End first bag of artifacts


            // Begin second bag of artifacts
            cont = new Backpack();
            cont.Hue = 0x48F;
            cont.Name = "Bag of Artifacts";

            PlaceItemIn( cont, 45, 66, new GauntletsOfNobility() );
            PlaceItemIn( cont, 69, 82, new MidnightBracers() );
            PlaceItemIn( cont, 93, 99, new VoiceOfTheFallenKing() );
            PlaceItemIn( cont, 117, 115, new OrnateCrownOfTheHarrower() );
            PlaceItemIn( cont, 45, 132, new HelmOfInsight() );
            PlaceItemIn( cont, 69, 66, new HolyKnightsBreastplate() );
            PlaceItemIn( cont, 93, 82, new ArmorOfFortune() );
            PlaceItemIn( cont, 117, 99, new TunicOfFire() );
            PlaceItemIn( cont, 45, 115, new LeggingsOfBane() );
            PlaceItemIn( cont, 69, 132, new ArcaneShield() );
            PlaceItemIn( cont, 93, 66, new Aegis() );
            PlaceItemIn( cont, 117, 82, new RingOfTheVile() );
            PlaceItemIn( cont, 45, 99, new BraceletOfHealth() );
            PlaceItemIn( cont, 69, 115, new RingOfTheElements() );
            PlaceItemIn( cont, 93, 132, new OrnamentOfTheMagician() );
            PlaceItemIn( cont, 117, 66, new DivineCountenance() );
            PlaceItemIn( cont, 45, 82, new JackalsCollar() );
            PlaceItemIn( cont, 69, 99, new HuntersHeaddress() );
            PlaceItemIn( cont, 93, 115, new HatOfTheMagi() );
            PlaceItemIn( cont, 117, 132, new ShadowDancerLeggings() );
            PlaceItemIn( cont, 45, 66, new SpiritOfTheTotem() );
            PlaceItemIn( cont, 69, 82, new BladeOfInsanity() );
            PlaceItemIn( cont, 93, 99, new AxeOfTheHeavens() );
            PlaceItemIn( cont, 117, 115, new TheBeserkersMaul() );
            PlaceItemIn( cont, 45, 132, new Frostbringer() );
            PlaceItemIn( cont, 69, 66, new BreathOfTheDead() );
            PlaceItemIn( cont, 93, 82, new TheDragonSlayer() );
            PlaceItemIn( cont, 117, 99, new BoneCrusher() );
            PlaceItemIn( cont, 45, 115, new StaffOfTheMagi() );
            PlaceItemIn( cont, 69, 132, new SerpentsFang() );
            PlaceItemIn( cont, 93, 66, new LegacyOfTheDreadLord() );
            PlaceItemIn( cont, 117, 82, new TheTaskmaster() );
            PlaceItemIn( cont, 45, 99, new TheDryadBow() );

            PlaceItemIn( bank, 78, 124, cont );
            // End second bag of artifacts

            // Begin bag of minor artifacts
            cont = new Backpack();
            cont.Hue = 0x48F;
            cont.Name = "Bag of Minor Artifacts";


            PlaceItemIn( cont, 45, 66, new LunaLance() );
            PlaceItemIn( cont, 69, 82, new VioletCourage() );
            PlaceItemIn( cont, 93, 99, new CavortingClub() );
            PlaceItemIn( cont, 117, 115, new CaptainQuacklebushsCutlass() );
            PlaceItemIn( cont, 45, 132, new NightsKiss() );
            PlaceItemIn( cont, 69, 66, new ShipModelOfTheHMSCape() );
            PlaceItemIn( cont, 93, 82, new AdmiralsHeartyRum() );
            PlaceItemIn( cont, 117, 99, new CandelabraOfSouls() );
            PlaceItemIn( cont, 45, 115, new IolosLute() );
            PlaceItemIn( cont, 69, 132, new GwennosHarp() );
            PlaceItemIn( cont, 93, 66, new ArcticDeathDealer() );
            PlaceItemIn( cont, 117, 82, new EnchantedTitanLegBone() );
            PlaceItemIn( cont, 45, 99, new NoxRangersHeavyCrossbow() );
            PlaceItemIn( cont, 69, 115, new BlazeOfDeath() );
            PlaceItemIn( cont, 93, 132, new DreadPirateHat() );
            PlaceItemIn( cont, 117, 66, new BurglarsBandana() );
            PlaceItemIn( cont, 45, 82, new GoldBricks() );
            PlaceItemIn( cont, 69, 99, new AlchemistsBauble() );
            PlaceItemIn( cont, 93, 115, new PhillipsWoodenSteed() );
            PlaceItemIn( cont, 117, 132, new PolarBearMask() );
            PlaceItemIn( cont, 45, 66, new BowOfTheJukaKing() );
            PlaceItemIn( cont, 69, 82, new GlovesOfThePugilist() );
            PlaceItemIn( cont, 93, 99, new OrcishVisage() );
            PlaceItemIn( cont, 117, 115, new StaffOfPower() );
            PlaceItemIn( cont, 45, 132, new ShieldOfInvulnerability() );
            PlaceItemIn( cont, 69, 66, new HeartOfTheLion() );
            PlaceItemIn( cont, 93, 82, new ColdBlood() );
            PlaceItemIn( cont, 117, 99, new GhostShipAnchor() );
            PlaceItemIn( cont, 45, 115, new SeahorseStatuette() );
            PlaceItemIn( cont, 69, 132, new WrathOfTheDryad() );
            PlaceItemIn( cont, 93, 66, new PixieSwatter() );

            for( int i = 0; i < 10; i++ )
                PlaceItemIn( cont, 117, 128, new MessageInABottle( Utility.RandomBool() ? Map.Trammel : Map.Felucca, 4 ) );

            PlaceItemIn( bank, 18, 124, cont );

            if( TreasuresOfTokuno.Enabled )
            {
                cont = new Bag();
                cont.Hue = 0x501;
                cont.Name = "Tokuno Minor Artifacts";

                PlaceItemIn( cont, 42, 70, new Exiler() );
                PlaceItemIn( cont, 38, 53, new HanzosBow() );
                PlaceItemIn( cont, 45, 40, new TheDestroyer() );
                PlaceItemIn( cont, 92, 80, new DragonNunchaku() );
                PlaceItemIn( cont, 42, 56, new PeasantsBokuto() );
                PlaceItemIn( cont, 44, 71, new TomeOfEnlightenment() );
                //PlaceItemIn( cont, 35, 35, new ChestOfHeirlooms() );    //Chest of Heirlooms
                PlaceItemIn( cont, 29,  0, new HonorableSwords() );
                PlaceItemIn( cont, 49, 85, new AncientUrn() );
                PlaceItemIn( cont, 51, 58, new FluteOfRenewal() );
                PlaceItemIn( cont, 70, 51, new PigmentsOfTokuno() );
                PlaceItemIn( cont, 40, 79, new AncientSamuraiDo() );
                PlaceItemIn( cont, 51, 61, new LegsOfStability() );
                PlaceItemIn( cont, 88, 78, new GlovesOfTheSun() );
                PlaceItemIn( cont, 55, 62, new AncientFarmersKasa() );
                PlaceItemIn( cont, 55, 83, new ArmsOfTacticalExcellence() );
                PlaceItemIn( cont, 50, 85, new DaimyosHelm() );
                PlaceItemIn( cont, 52, 78, new BlackLotusHood() );
                PlaceItemIn( cont, 52, 79, new DemonForks() );
                PlaceItemIn( cont, 33, 49, new PilferedDancerFans() );

                PlaceItemIn( bank, 58, 124, cont );
            }

            if( Core.SE )    //This bag came only after SE.
            {
                cont = new Bag();
                cont.Name = "Bag of Bows";

                PlaceItemIn( cont, 31, 84, new Bow() );
                PlaceItemIn( cont, 78, 74, new CompositeBow() );
                PlaceItemIn( cont, 53, 71, new Crossbow() );
                PlaceItemIn( cont, 56, 39, new HeavyCrossbow() );
                PlaceItemIn( cont, 82, 72, new RepeatingCrossbow() );
                PlaceItemIn( cont, 49, 45, new Yumi() );

                for( int i = 0; i < cont.Items.Count; i++ )
                {
                    BaseRanged bow = cont.Items[i] as BaseRanged;

                    if( bow != null )
                    {
                        bow.Attributes.WeaponSpeed = 35;
                        bow.Attributes.WeaponDamage = 35;
                    }
                }

                PlaceItemIn( bank, 108, 135, cont );
            }
        }

        private static void FillBankbox( Mobile m )
        {
            if ( Core.AOS )
            {
                FillBankAOS( m );
                return;
            }

            BankBox bank = m.BankBox;

            bank.DropItem( new BankCheck( 1000000 ) );

            // Full spellbook
            Spellbook book = new Spellbook();

            book.Content = ulong.MaxValue;

            bank.DropItem( book );

            Bag bag = new Bag();

            for ( int i = 0; i < 5; ++i )
                bag.DropItem( new Moonstone( MoonstoneType.Felucca ) );

            // Felucca moonstones
            bank.DropItem( bag );

            bag = new Bag();

            for ( int i = 0; i < 5; ++i )
                bag.DropItem( new Moonstone( MoonstoneType.Trammel ) );

            // Trammel moonstones
            bank.DropItem( bag );

            // Treasure maps
            bank.DropItem(new TreasureMap(1, Map.Felucca));
            bank.DropItem(new TreasureMap(2, Map.Felucca));
            bank.DropItem(new TreasureMap(3, Map.Felucca));
            bank.DropItem(new TreasureMap(4, Map.Felucca));
            bank.DropItem(new TreasureMap(5, Map.Felucca));

            // Bag containing 50 of each reagent
            bank.DropItem( new BagOfReagents( 50 ) );

            // Craft tools
            bank.DropItem( MakeNewbie( new Scissors() ) );
            bank.DropItem( MakeNewbie( new SewingKit( 1000 ) ) );
            bank.DropItem( MakeNewbie( new SmithHammer( 1000 ) ) );
            bank.DropItem( MakeNewbie( new FletcherTools( 1000 ) ) );
            bank.DropItem( MakeNewbie( new DovetailSaw( 1000 ) ) );
            bank.DropItem( MakeNewbie( new MortarPestle( 1000 ) ) );
            bank.DropItem( MakeNewbie( new ScribesPen( 1000 ) ) );
            bank.DropItem( MakeNewbie( new TinkerTools( 1000 ) ) );

            // A few dye tubs
            bank.DropItem( new Dyes() );
            bank.DropItem( new DyeTub() );
            bank.DropItem( new DyeTub() );
            bank.DropItem( new BlackDyeTub() );

            DyeTub darkRedTub = new DyeTub();

            darkRedTub.DyedHue = 0x485;
            darkRedTub.Redyable = false;

            bank.DropItem( darkRedTub );

            // Some food
            bank.DropItem( MakeNewbie( new Apple( 1000 ) ) );

            // Resources
            bank.DropItem( MakeNewbie( new Feather( 1000 ) ) );
            bank.DropItem( MakeNewbie( new BoltOfCloth( 1000 ) ) );
            bank.DropItem( MakeNewbie( new BlankScroll( 1000 ) ) );
            bank.DropItem( MakeNewbie( new Hides( 1000 ) ) );
            bank.DropItem( MakeNewbie( new Bandage( 1000 ) ) );
            bank.DropItem( MakeNewbie( new Bottle( 1000 ) ) );
            bank.DropItem( MakeNewbie( new Log( 1000 ) ) );

            bank.DropItem( MakeNewbie( new IronIngot( 5000 ) ) );
            bank.DropItem( MakeNewbie( new DullCopperIngot( 5000 ) ) );
            bank.DropItem( MakeNewbie( new ShadowIronIngot( 5000 ) ) );
            bank.DropItem( MakeNewbie( new CopperIngot( 5000 ) ) );
            bank.DropItem( MakeNewbie( new BronzeIngot( 5000 ) ) );
            bank.DropItem( MakeNewbie( new GoldIngot( 5000 ) ) );
            bank.DropItem( MakeNewbie( new AgapiteIngot( 5000 ) ) );
            bank.DropItem( MakeNewbie( new VeriteIngot( 5000 ) ) );
            bank.DropItem( MakeNewbie( new ValoriteIngot( 5000 ) ) );

            // Reagents
            bank.DropItem( MakeNewbie( new BlackPearl( 1000 ) ) );
            bank.DropItem( MakeNewbie( new Bloodmoss( 1000 ) ) );
            bank.DropItem( MakeNewbie( new Garlic( 1000 ) ) );
            bank.DropItem( MakeNewbie( new Ginseng( 1000 ) ) );
            bank.DropItem( MakeNewbie( new MandrakeRoot( 1000 ) ) );
            bank.DropItem( MakeNewbie( new Nightshade( 1000 ) ) );
            bank.DropItem( MakeNewbie( new SulfurousAsh( 1000 ) ) );
            bank.DropItem( MakeNewbie( new SpidersSilk( 1000 ) ) );

            // Some extra starting gold
            bank.DropItem( MakeNewbie( new Gold( 9000 ) ) );

            // 5 blank recall runes
            for ( int i = 0; i < 5; ++i )
                bank.DropItem( MakeNewbie( new RecallRune() ) );

            AddPowerScrolls( bank );
        }

        private static void AddPowerScrolls( BankBox bank )
        {
            Bag bag = new Bag();

            for ( int i = 0; i < PowerScroll.Skills.Length; ++i )
                bag.DropItem( new PowerScroll( PowerScroll.Skills[i], 120.0 ) );

            bag.DropItem( new StatCapScroll( 250 ) );

            bank.DropItem( bag );
        }

        private static void AddShirt( Mobile m, int shirtHue )
        {
            int hue = Utility.ClipDyedHue( shirtHue & 0x3FFF );

            switch ( Utility.Random( 3 ) )
            {
                case 0: EquipItem( m, new Shirt( hue ), true ); break;
                case 1: EquipItem( m, new FancyShirt( hue ), true ); break;
                case 2: EquipItem( m, new Doublet( hue ), true ); break;
            }
        }

        private static void AddPants( Mobile m, int pantsHue )
        {
            int hue = Utility.ClipDyedHue( pantsHue & 0x3FFF );

            if ( m.Female )
            {
                switch ( Utility.Random( 2 ) )
                {
                    case 0: EquipItem( m, new Skirt( hue ), true ); break;
                    case 1: EquipItem( m, new Kilt( hue ), true ); break;
                }
            }
            else
            {
                switch ( Utility.Random( 2 ) )
                {
                    case 0: EquipItem( m, new LongPants( hue ), true ); break;
                    case 1: EquipItem( m, new ShortPants( hue ), true ); break;
                }
            }
        }

        private static void AddShoes( Mobile m )
        {
                EquipItem( m, new Shoes( Utility.RandomYellowHue() ), true );
        }

        private static Mobile CreateMobile( Account a )
        {
            if ( a.Count >= a.Limit )
                return null;

            for ( int i = 0; i < a.Length; ++i )
            {
                if ( a[i] == null )
                    return (a[i] = new PlayerMobile());
            }

            return null;
        }

        private static void EventSink_CharacterCreated( CharacterCreatedEventArgs args )
        {
            if ( !VerifyProfession( args.Profession ) )
                args.Profession = 0;

            Mobile newChar = CreateMobile( args.Account as Account );

            if ( newChar == null )
            {
                Console.WriteLine( "Login: {0}: Character creation failed, account full", args.State );
                return;
            }

            args.Mobile = newChar;
            m_Mobile = newChar;

            newChar.Player = true;
            newChar.AccessLevel = args.Account.AccessLevel;
            newChar.Female = args.Female;
            //newChar.Body = newChar.Female ? 0x191 : 0x190;

            // 720% skilla dla kazdej postaci
            newChar.SkillsCap = 7200;

            // 26.06.2012 :: zombie
            //if( Core.Expansion >= args.Race.RequiredExpansion )
                //newChar.Race = args.Race;    //Sets body
            //else
            // zombie 

            newChar.Race = Race.DefaultRace;

            //newChar.Hue = Utility.ClipSkinHue( args.Hue & 0x3FFF ) | 0x8000;
            newChar.Hue = newChar.Race.ClipSkinHue( args.Hue & 0x3FFF ) | 0x8000;

            newChar.Hunger = 20;

            bool young = false;

            if ( newChar is PlayerMobile )
            {
                PlayerMobile pm = (PlayerMobile) newChar;

                pm.Profession = args.Profession;

                if ( pm.AccessLevel == AccessLevel.Player && Config.Young )
                    young = pm.Young = true;
            }

            SetName( newChar, args.Name );

            AddBackpack( newChar );

            SetStats( newChar, args.Str, args.Dex, args.Int );
            // SetSkills( newChar, args.Skills, args.Profession );

            Race race = newChar.Race;

            if( race.ValidateHair( newChar, args.HairID ) )
            {
                newChar.HairItemID = args.HairID;
                newChar.HairHue = race.ClipHairHue( args.HairHue & 0x3FFF );
            }

            if( race.ValidateFacialHair( newChar, args.BeardID ) )
            {
                newChar.FacialHairItemID = args.BeardID;
                newChar.FacialHairHue = race.ClipHairHue( args.BeardHue & 0x3FFF );
            }

            if ( args.Profession <= 3 )
            {
                AddShirt( newChar, args.ShirtHue );
                AddPants( newChar, args.PantsHue );
                AddShoes( newChar );
            }
            EquipItem( newChar, new Robe( Utility.RandomBlueHue() ) );

            // Drop skill-ball to backpack
            newChar.Backpack.DropItem(new SkillBallNewChar((PlayerMobile) newChar));

            if( TestCenter.Enabled )
                FillBankbox( newChar );

            if ( young )
            {
                NewPlayerTicket ticket = new NewPlayerTicket();
                ticket.Owner = newChar;
                newChar.BankBox.DropItem( ticket );
            }

            CityInfo city = GetStartLocation( args, young );
            //CityInfo city = new CityInfo( "Britain", "Sweet Dreams Inn", 1496, 1628, 10, Map.Felucca );

            newChar.MoveToWorld( city.Location, city.Map );

            Console.WriteLine( "Login: {0}: New character being created (account={1})", args.State, args.Account.Username );
            Console.WriteLine( " - Character: {0} (serial={1})", newChar.Name, newChar.Serial );
            Console.WriteLine( " - Started: {0} {1} in {2}", city.City, city.Location, city.Map.ToString() );

            new WelcomeTimer( newChar ).Start();
            
            // mod to attach the XmlPoints attachment automatically to new chars
            XmlAttach.AttachTo(newChar, new XmlPoints());
        }

        public static bool VerifyProfession( int profession )
        {
            if ( profession < 0 )
                return false;
            else if ( profession < 4 )
                return true;
            else if ( Core.AOS && profession < 6 )
                return true;
            else if ( Core.SE && profession < 8 )
                return true;
            else
                return false;
        }

        private class BadStartMessage : Timer
        {
            Mobile m_Mobile;
            int m_Message;
            public BadStartMessage( Mobile m, int message ) : base( TimeSpan.FromSeconds ( 3.5 ) )
            {
                m_Mobile = m;
                m_Message = message;
                this.Start();
            }

            protected override void OnTick()
            {
                m_Mobile.SendLocalizedMessage( m_Message );
            }
        }

        // 21.06.2012 :: zombie :: modyfikacja
        private static CityInfo GetStartLocation( CharacterCreatedEventArgs args, bool isYoung )
        {
            int flags = args.State == null ? 0 : args.State.Flags;
            Mobile m = args.Mobile;

            if ( m.AccessLevel >= AccessLevel.Counselor )
            {
                //return new CityInfo( "Haven", "Tymczasowe miejsce startowe", 2911, 1132, 10, Map.Felucca );
                return new CityInfo( "Komnata Poczatku", "", 5178, 230, 1, Map.Felucca );
            }
            else
            {
                // 29.10.2012 :: zombie :: Nelderim beta: lokacja startowa
                //return new CityInfo( "Lokacja startowa", "", 2304, 2703, 0, Map.Felucca );
                // zombie

                return new CityInfo( "Komnata Poczatku", "", 5178, 230, 1, Map.Felucca );
            }
        }
        // zombie

        private static void FixStats( ref int str, ref int dex, ref int intel )
        {
            int vStr = str - 10;
            int vDex = dex - 10;
            int vInt = intel - 10;

            if ( vStr < 0 )
                vStr = 0;

            if ( vDex < 0 )
                vDex = 0;

            if ( vInt < 0 )
                vInt = 0;

            int total = vStr + vDex + vInt;

            if ( total == 0 || total == 50 )
                return;

            double scalar = 50 / (double)total;

            vStr = (int)(vStr * scalar);
            vDex = (int)(vDex * scalar);
            vInt = (int)(vInt * scalar);

            FixStat( ref vStr, (vStr + vDex + vInt) - 50 );
            FixStat( ref vDex, (vStr + vDex + vInt) - 50 );
            FixStat( ref vInt, (vStr + vDex + vInt) - 50 );

            str = vStr + 10;
            dex = vDex + 10;
            intel = vInt + 10;
        }

        private static void FixStat( ref int stat, int diff )
        {
            stat += diff;

            if ( stat < 0 )
                stat = 0;
            else if ( stat > 50 )
                stat = 50;
        }

        private static void SetStats( Mobile m, int str, int dex, int intel )
        {
            FixStats( ref str, ref dex, ref intel );

            if ( str < 10 || str > 60 || dex < 10 || dex > 60 || intel < 10 || intel > 60 || (str + dex + intel) != 80 )
            {
                str = 10;
                dex = 10;
                intel = 10;
            }

            m.InitStats( str, dex, intel );
        }

        private static void SetName( Mobile m, string name )
        {
            name = name.Trim();

            if ( !NameVerification.Validate( name, 2, 16, true, false, true, 1, NameVerification.SpaceDashPeriodQuote ) )
                name = "Generic Player";

            m.Name = name;
        }

        private static bool ValidSkills( SkillNameValue[] skills )
        {
            int total = 0;

            for ( int i = 0; i < skills.Length; ++i )
            {
                if ( skills[i].Value < 0 || skills[i].Value > 50 )
                    return false;

                total += skills[i].Value;

                for ( int j = i + 1; j < skills.Length; ++j )
                {
                    if ( skills[j].Value > 0 && skills[j].Name == skills[i].Name )
                        return false;
                }
            }

            return ( total == 100 );
        }

        private static Mobile m_Mobile;

        private static void SetSkills( Mobile m, SkillNameValue[] skills, int prof )
        {
            bool addSkillItems = true;
            bool setSkillValues = true;
            switch ( prof )
            {
                case 1: // Warrior
                {
                    skills = new SkillNameValue[]
                        {
                            new SkillNameValue( SkillName.Anatomy, 30 ),
                            new SkillNameValue( SkillName.Healing, 45 ),
                            new SkillNameValue( SkillName.Swords, 35 ),
                            new SkillNameValue( SkillName.Tactics, 50 )
                        };

                    break;
                }
                case 2: // Magician
                {
                    skills = new SkillNameValue[]
                        {
                            new SkillNameValue( SkillName.EvalInt, 30 ),
                            new SkillNameValue( SkillName.Wrestling, 30 ),
                            new SkillNameValue( SkillName.Magery, 50 ),
                            new SkillNameValue( SkillName.Meditation, 50 )
                        };

                    break;
                }
                case 3: // Blacksmith
                {
                    skills = new SkillNameValue[]
                        {
                            new SkillNameValue( SkillName.Mining, 30 ),
                            new SkillNameValue( SkillName.ArmsLore, 30 ),
                            new SkillNameValue( SkillName.Blacksmith, 50 ),
                            new SkillNameValue( SkillName.Tinkering, 50 )
                        };

                    break;
                }
                case 4: // Necromancer
                {
                    skills = new SkillNameValue[]
                        {
                            new SkillNameValue( SkillName.Necromancy, 50 ),
                            //new SkillNameValue( SkillName.Focus, 30 ),
                            new SkillNameValue( SkillName.SpiritSpeak, 50 ),
                            new SkillNameValue( SkillName.Swords, 30 ),
                            new SkillNameValue( SkillName.Tactics, 20 )
                        };

                    break;
                }
                case 5: // Paladin
                {
                    skills = new SkillNameValue[]
                        {
                            new SkillNameValue( SkillName.Chivalry, 51 ),
                            new SkillNameValue( SkillName.Swords, 49 ),
                            //new SkillNameValue( SkillName.Focus, 30 ),
                            new SkillNameValue( SkillName.Tactics, 50 )
                        };

                    break;
                }
                case 6:    //Samurai
                {
                    skills = new SkillNameValue[]
                        {
                            new SkillNameValue( SkillName.Bushido, 50 ),
                            new SkillNameValue( SkillName.Swords, 50 ),
                            new SkillNameValue( SkillName.Anatomy, 30 ),
                            new SkillNameValue( SkillName.Healing, 30 )
                    };
                    break;
                }
                case 7:    //Ninja
                {
                    skills = new SkillNameValue[]
                        {
                            new SkillNameValue( SkillName.Ninjitsu, 50 ),
                            new SkillNameValue( SkillName.Hiding, 50 ),
                            new SkillNameValue( SkillName.Fencing, 30 ),
                            new SkillNameValue( SkillName.Stealth, 30 )
                        };
                    break;
                }
                default:
                {
                    if ( !ValidSkills( skills ) )
                        return;

                    addSkillItems = false;
                    setSkillValues = false;
                    break;
                }
            }

            //bool elf = (m.Race == Race.Elf);

            // 30.10.2012 :: zombie
            /*
            switch ( prof )
            {
                case 1: // Warrior
                {

                    EquipItem( new LeatherChest() );
                    break;
                }
                case 4: // Necromancer
                {
                    Container regs = new BagOfNecroReagents( 50 );

                    if ( !Core.AOS )
                    {
                        foreach ( Item item in regs.Items )
                            item.LootType = LootType.Newbied;
                    }

                    PackItem( regs );

                    regs.LootType = LootType.Regular;

                    
                    EquipItem( new BoneHelm() );

                    EquipItem( new BoneHarvester() );
                    EquipItem( NecroHue( new LeatherChest() ) );
                    EquipItem( NecroHue( new LeatherArms() ) );
                    EquipItem( NecroHue( new LeatherGloves() ) );
                    EquipItem( NecroHue( new LeatherGorget() ) );
                    EquipItem( NecroHue( new LeatherLegs() ) );
                    EquipItem( NecroHue( new Skirt() ) );
                    EquipItem( new Sandals( 0x8FD ) );

                    // zombie - 19-06-2012 - pusta ksiega na starcie
                    // animate dead, evil omen, pain spike, summon familiar, wraith form (ulong)0x8981 -> (ulong)0
                    Spellbook book = new NecromancerSpellbook( (ulong)0 ); 
                    // zombie

                    PackItem( book );

                    book.LootType = LootType.Blessed;

                    addSkillItems = false;

                    break;
                }
                case 5: // Paladin
                {
                    EquipItem( new Broadsword() );
                    EquipItem( new Helmet() );
                    EquipItem( new PlateGorget() );
                    EquipItem( new RingmailArms() );
                    EquipItem( new RingmailChest() );
                    EquipItem( new RingmailLegs() );
                    EquipItem( new ThighBoots( 0x748 ) );
                    EquipItem( new Cloak( 0xCF ) );
                    EquipItem( new BodySash( 0xCF ) );

                    // zombie - 19-06-2012 - pusta ksiega na starcie (ulong)0x3FF -> (ulong)0
                    Spellbook book = new BookOfChivalry( (ulong)0 );
                    // zombie

                    PackItem( book );

                    book.LootType = LootType.Blessed;

                    addSkillItems = false;

                    break;
                }
                    
                case 6: // Samurai
                {
                    addSkillItems = false;
                    EquipItem( new HakamaShita( 0x2C3 ) );
                    EquipItem( new Hakama( 0x2C3 ) );
                    EquipItem( new SamuraiTabi( 0x2C3 ) );
                    EquipItem( new TattsukeHakama( 0x22D ) );
                    EquipItem( new Bokuto() );

                    EquipItem( new LeatherJingasa() );

                    PackItem( new Scissors() );
                    PackItem( new Bandage( 50 ) );

                    Spellbook book = new BookOfBushido();
                    PackItem( book );

                    break;
                }
                case 7: // Ninja
                {
                    addSkillItems = false;
                    EquipItem( new Kasa() );
                    
                    int[] hues = new int[] { 0x1A8, 0xEC, 0x99, 0x90, 0xB5, 0x336, 0x89    };
                    //TODO: Verify that's ALL the hues for that above.

                    EquipItem( new TattsukeHakama( hues[Utility.Random(hues.Length)] ) );
                    
                    EquipItem( new HakamaShita( 0x2C3 ) );
                    EquipItem( new NinjaTabi( 0x2C3 ) );


                        EquipItem( new Tekagi() );

                    PackItem( new SmokeBomb() );

                    Spellbook book = new BookOfNinjitsu();
                    PackItem( book );

                    break;
                }
            }
            */
            // zombie

            if( setSkillValues )
            {
                for ( int i = 0; i < skills.Length; ++i )
                {
                    SkillNameValue snv = skills[i];

                    if ( snv.Value > 0 && ( snv.Name != SkillName.Stealth || prof == 7 ) && snv.Name != SkillName.RemoveTrap && snv.Name != SkillName.Spellweaving )
                    {
                        Skill skill = m.Skills[snv.Name];

                        if ( skill != null )
                        {
                            skill.BaseFixedPoint = snv.Value * 10;

                            if ( addSkillItems )
                                AddSkillItems( snv.Name, m );
                        }
                    }
                }
            }
        }

        private static void EquipItem( Mobile m, Item item )
        {
            EquipItem( m, item, false );
        }

        //private static void EquipItem( Item item )
        //{
        //    EquipItem( m_Mobile, item, false );
        //}

        private static void EquipItem( Mobile m, Item item, bool mustEquip )
        {
            if ( !Core.AOS )
                item.LootType = LootType.Newbied;

            if ( m != null && m.EquipItem( item ) )
                return;

            Container pack = m.Backpack;

            if ( !mustEquip && pack != null )
                pack.DropItem( item );
            else
                item.Delete();
        }

        //private static void PackItem( Item item )
        //{
        //    PackItem( m_Mobile, item );
        //}

        private static void PackItem( Mobile m, Item item )
        {
            if ( !Core.AOS )
                item.LootType = LootType.Newbied;

            Container pack = m.Backpack;

            if ( pack != null )
                pack.DropItem( item );
            else
                item.Delete();
        }

        //private static void PackInstrument()
        //{
        //    PackInstrument(m_Mobile);
        //}

        private static void PackInstrument(Mobile m)
        {
            switch ( Utility.Random( 6 ) )
            {
                case 0: PackItem( m, new Drums() ); break;
                case 1: PackItem( m, new Harp() ); break;
                case 2: PackItem( m, new LapHarp() ); break;
                case 3: PackItem( m, new Lute() ); break;
                case 4: PackItem( m, new Tambourine() ); break;
                case 5: PackItem( m, new TambourineTassel() ); break;
            }
        }

        //private static void PackScroll( int circle )
        //{
        //    PackScroll(m_Mobile, circle);
        //}

        private static void PackScroll( Mobile m, int circle )
        {
            switch ( Utility.Random( 8 ) * (circle * 8) )
            {
                case  0: PackItem( m, new ClumsyScroll() ); break;
                case  1: PackItem( m, new CreateFoodScroll() ); break;
                case  2: PackItem( m, new FeeblemindScroll() ); break;
                case  3: PackItem( m, new HealScroll() ); break;
                case  4: PackItem( m, new MagicArrowScroll() ); break;
                case  5: PackItem( m, new NightSightScroll() ); break;
                case  6: PackItem( m, new ReactiveArmorScroll() ); break;
                case  7: PackItem( m, new WeakenScroll() ); break;
                case  8: PackItem( m, new AgilityScroll() ); break;
                case  9: PackItem( m, new CunningScroll() ); break;
                case 10: PackItem( m, new CureScroll() ); break;
                case 11: PackItem( m, new HarmScroll() ); break;
                case 12: PackItem( m, new MagicTrapScroll() ); break;
                case 13: PackItem( m, new MagicUnTrapScroll() ); break;
                case 14: PackItem( m, new ProtectionScroll() ); break;
                case 15: PackItem( m, new StrengthScroll() ); break;
                case 16: PackItem( m, new BlessScroll() ); break;
                case 17: PackItem( m, new FireballScroll() ); break;
                case 18: PackItem( m, new MagicLockScroll() ); break;
                case 19: PackItem( m, new PoisonScroll() ); break;
                case 20: PackItem( m, new TelekinisisScroll() ); break;
                case 21: PackItem( m, new TeleportScroll() ); break;
                case 22: PackItem( m, new UnlockScroll() ); break;
                case 23: PackItem( m, new WallOfStoneScroll() ); break;
            }
        }

        private static Item NecroHue( Item item )
        {
            item.Hue = 0x2C3;

            return item;
        }

        // 30.10.2012 :: zombie :: porzadek
        public static void AddSkillItems( SkillName skill, Mobile m )
        {
            //EquipItem( new Robe( Utility.RandomBlueHue() ) );

            switch ( skill )
            {
                case SkillName.Alchemy:
                {
                    PackItem( m, new Bottle( 5 ) );
                    PackItem( m, new MortarPestle() );
                    PackItem( m, new MortarPestle() );

                    break;
                }
                case SkillName.Anatomy:
                {
                    PackItem( m, new Bandage( 10 ) );

                    int hue = Utility.RandomYellowHue();

                    break;
                }
                case SkillName.AnimalLore:
                {
                    EquipItem( m, new ShepherdsCrook() );

                    break;
                }
                case SkillName.Archery:
                {
                    PackItem( m, new Arrow( 25 ) );

                    EquipItem( m, new Bow() );
                    
                    break;
                }
                case SkillName.ArmsLore:
                {
                    switch ( Utility.Random( 3 ) )
                    {
                        case 0: EquipItem( m, new Kryss() ); break;
                        case 1: EquipItem( m, new Katana() ); break;
                        case 2: EquipItem( m, new Club() ); break;
                    }

                    break;
                }
                case SkillName.Begging:
                {
                    EquipItem( m, new GnarledStaff() );

                    break;
                }
                case SkillName.Blacksmith:
                {
                    PackItem( m, new Tongs() );
                    PackItem( m, new SmithHammer() );
                    PackItem( m, new IronIngot( 50 ) );
                    EquipItem( m, new HalfApron( Utility.RandomYellowHue() ) );
                    break;
                }
                case SkillName.Bushido:
                {
                    EquipItem( m, new Hakama() );
                    EquipItem( m, new Kasa() );
                    PackItem( m, new BookOfBushido() );
                    break;
                }
                case SkillName.Fletching:
                {
                    PackItem( m, new FletcherTools() );
                    PackItem( m, new FletcherTools() );
                    PackItem( m, new Board( 20 ) );
                    PackItem( m, new Feather( 5 ) );
                    PackItem( m, new Shaft( 5 ) );
                    break;
                }
                case SkillName.Camping:
                {
                    PackItem( m, new Bedroll() );
                    PackItem( m, new Kindling( 5 ) );
                    break;
                }
                case SkillName.Carpentry:
                {
                    PackItem( m, new Saw() );
                    PackItem( m, new Scorp() );                    
                    PackItem( m, new Board( 30 ) );
                    EquipItem( m, new HalfApron( Utility.RandomYellowHue() ) );
                    break;
                }
                case SkillName.Cartography:
                {
                    PackItem( m, new BlankMap() );
                    PackItem( m, new BlankMap() );
                    PackItem( m, new BlankMap() );
                    PackItem( m, new BlankMap() );
                    PackItem( m, new Sextant() );
                    break;
                }
                case SkillName.Cooking:
                {
                    PackItem( m, new FlourSifter() ); 
                    PackItem( m, new RollingPin() );
                    PackItem( m, new Kindling( 2 ) );
                    PackItem( m, new RawLambLeg() );
                    PackItem( m, new RawChickenLeg() );
                    PackItem( m, new RawFishSteak() );
                    PackItem( m, new SackFlour() );
                    PackItem( m, new Pitcher( BeverageType.Water ) );
                    break;
                }
                case SkillName.Chivalry:
                {
                    // zombie - 18-06-2012 - pusta ksiega na starcie 0x3FF -> 0
                    if( Core.ML )
                        PackItem( m, new BookOfChivalry( (ulong)0 ) );
                    // zombie
                    break;
                }
                case SkillName.DetectHidden:
                {
                    EquipItem( m, new Cloak( 0x455 ) );
                    break;
                }
                case SkillName.Discordance:
                {
                    PackInstrument(m);
                    break;
                }
                case SkillName.Fencing:
                {
                    EquipItem( m, new Kryss() );

                    break;
                }
                case SkillName.Fishing:
                {
                    EquipItem( m, new FishingPole() );
                    EquipItem( m, new FishingPole() );
					PackItem( m, new Dagger() );

                    int hue = Utility.RandomYellowHue();

                    EquipItem( m, new FloppyHat( Utility.RandomYellowHue() ) );

                    break;
                }
                case SkillName.Healing:
                {
                    PackItem( m, new Bandage( 50 ) );
                    PackItem( m, new Scissors() );
                    break;
                }
                case SkillName.Herding:
                {
                    EquipItem( m, new ShepherdsCrook() );

                    break;
                }
                case SkillName.Hiding:
                {
                    EquipItem( m, new Cloak( 0x455 ) );
                    break;
                }
                case SkillName.Inscribe:
                {
                    PackItem( m, new BlankScroll( 10 ) );
					PackItem( m, new ScribesPen() );
                    PackItem( m, new ScribesPen() );
                    PackItem( m, new BlueBook() );
                    break;
                }
                case SkillName.ItemID:
                {
                    EquipItem( m, new GnarledStaff() );

                    break;
                }
                case SkillName.Lockpicking:
                {
                    PackItem( m, new Lockpick( 20 ) );
                    break;
                }
                case SkillName.Lumberjacking:
                {
                    EquipItem( m, new Hatchet() );
                    EquipItem( m, new Hatchet() );
                    break;
                }
                case SkillName.Macing:
                {
                    EquipItem( m, new Club() );

                    break;
                }
                case SkillName.Magery:
                {
                    BagOfReagents regs = new BagOfReagents( 30 );

                    if ( !Core.AOS )
                    {
                        foreach ( Item item in regs.Items )
                            item.LootType = LootType.Newbied;
                    }

                    PackItem( m, regs );

                    regs.LootType = LootType.Regular;

                    //PackScroll( 0 );
                    //PackScroll( 1 );
                    //PackScroll( 2 );

                    // ksiega zawiera konkretne czary na starcie:
                    Spellbook book = new Spellbook( (ulong)0x20408 );

                    EquipItem( m, book );

                    book.LootType = LootType.Blessed;

                    EquipItem( m, new WizardsHat() );

                    break;
                }
                case SkillName.Mining:
                {
                    PackItem( m, new Pickaxe() );
                    PackItem( m, new Shovel() );
                    break;
                }
                case SkillName.Musicianship:
                {
                    PackInstrument(m);
                    break;
                }
                case SkillName.Necromancy:
                {
                    if( Core.ML )
                    {
                        Container regs = new BagOfNecroReagents( 50 );

                        PackItem( m, regs );

                        regs.LootType = LootType.Regular;

                        // 30.10.2012 :: zombie :: pusta ksiega na starcie
                        Spellbook book = new NecromancerSpellbook( (ulong)0 ); 
                        book.LootType = LootType.Blessed;
                        PackItem( m, book );
                        // zombie
                    }

                    break;
                }
                case SkillName.Ninjitsu:
                {
                    EquipItem( m, new Hakama( 0x2C3 ) );    //Only ninjas get the hued one.
                    EquipItem( m, new Kasa() );
                    PackItem( m, new BookOfNinjitsu() );
                    break;
                }
                case SkillName.Parry:
                {
                    EquipItem( m, new WoodenShield() );
                    break;
                }
                case SkillName.Peacemaking:
                {
                    PackInstrument(m);
                    break;
                }
                case SkillName.Poisoning:
                {
                    PackItem( m, new LesserPoisonPotion() );
                    PackItem( m, new LesserPoisonPotion() );
                    break;
                }
                case SkillName.Provocation:
                {
                    PackInstrument(m);
                    break;
                }
                case SkillName.Snooping:
                {
                    PackItem( m, new Lockpick( 20 ) );
                    break;
                }
                case SkillName.SpiritSpeak:
                {
                    EquipItem( m, new Cloak( 0x455 ) );
                    break;
                }
                case SkillName.Stealing:
                {
                    PackItem( m, new Lockpick( 20 ) );
                    break;
                }
                case SkillName.Swords:
                {
                    EquipItem( m, new Katana() );

                    break;
                }
                case SkillName.Tactics:
                {
                    EquipItem( m, new Katana() );

                    break;
                }
                case SkillName.Tailoring:
                {
                    PackItem( m, new BoltOfCloth( 2 ) );
                    PackItem( m, new Scissors() );
                    PackItem( m, new SewingKit() );
                    PackItem( m, new SewingKit() );
                    break;
                }
				case SkillName.Tinkering:
				{
					PackItem( m, new TinkerTools() );
                    PackItem( m, new TinkerTools() );
					PackItem( m, new IronIngot( 30 ) );
					PackItem( m, new Board( 20) );
					break;
				}
                case SkillName.Tracking:
                {
                    if ( m != null )
                    {
                        Item shoes = m.FindItemOnLayer( Layer.Shoes );

                        if ( shoes != null )
                            shoes.Delete();
                    }

                    int hue = Utility.RandomYellowHue();

                    EquipItem( m, new Boots( hue ) );

                    EquipItem( m, new SkinningKnife() );
                    break;
                }
                case SkillName.Veterinary:
                {
                    PackItem( m, new Bandage( 10 ) );
                    PackItem( m, new Scissors() );
                    break;
                }
                case SkillName.Wrestling:
                {
                    EquipItem( m, new LeatherGloves() );

                    break;
                }
            }
        }
        // zombie
    }
}