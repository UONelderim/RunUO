using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Engines.BulkOrders
{
    public sealed class HunterRewardCalculator : RewardCalculator
    {
        private static Item SelectRandomType(Dictionary<Type, int> objects) {
            int rand;
            while (true) {
                rand = Utility.Random(100);

                List<Type> keys = new List<Type>(objects.Keys);
                int size = objects.Count;
                Random randa = new Random();
                Type randomKey = keys[randa.Next(size)];
                int randomeElement = objects[randomKey];

                if (randomeElement < rand)
                    return (Item)Activator.CreateInstance(randomKey);
            }
        }

        #region Constructors
        private static readonly ConstructCallback Pigments = new ConstructCallback(CreatePigments);
        private static readonly ConstructCallback TransPowders = new ConstructCallback(CreateTransPowders);
        private static readonly ConstructCallback DurabilityPowder = new ConstructCallback(CreateDurabilityPowder);
        private static readonly ConstructCallback DecoMinor = new ConstructCallback(CreateDecoMinor);
        private static readonly ConstructCallback Talismans = new ConstructCallback(CreateTalismans);
        private static readonly ConstructCallback PetResurrectPotion = new ConstructCallback(CreatePetResurrectPotion);
        private static readonly ConstructCallback DecoMajor = new ConstructCallback(CreateDecoMajor);
        private static readonly ConstructCallback Artifacts = new ConstructCallback(CreateArtifacts);

        private static Item CreatePigments(int type) {
            return new BasePigment(type);
        }

        private static Item CreateTransPowders(int type) {
            return new PowderOfTranslocation(type);
        }

        private static Item CreateDurabilityPowder(int type) {
            int uses = Utility.Random(2) + 1; // 1-2
            switch (Utility.Random(4)) {
                default:
                case 0: return new PowderForWood(uses);
                case 1: return new PowderForMetal(uses);
                case 2: return new PowderForLeather(uses);
                case 3: return new PowderForTinkering(uses);
            }
        }

        private static Dictionary<Type, int> m_minorDecoTypes = new Dictionary<Type, int>
        {
            {typeof(FurCape), 15},
            {typeof(NBearMask), 15},
            {typeof(NDeerMask), 15},
            {typeof(Arrows), 10},
            {typeof(CrossBowBolts), 10},
            {typeof(Rope), 10},
            {typeof(Whip), 10},
            {typeof(WhisperingRose), 5},
            {typeof(RoseOfTrinsic), 5},
            {typeof(carpet3sDeed), 5},
            {typeof(carpet4sDeed), 5},
            {typeof(carpet5sDeed), 5},
            {typeof(carpet6sDeed), 5}
        };
        
        private static Item CreateDecoMinor(int type) {
            return SelectRandomType(m_minorDecoTypes);
        }

        private enum TalizmanType
        {
            Level2,
            Level3,
        }

        private static Item CreateTalismans(int type) {
            switch ((TalizmanType)type)
            {
                case TalizmanType.Level3: return new TalismanLevel3();
                default: return new TalismanLevel2();
            }
        }

        private static Item CreatePetResurrectPotion(int type) {
            return new PetResurrectPotion();
        }

        private static Dictionary<Type, int> m_majorDecoTypes = new Dictionary<Type, int>()
        {
            {typeof(figurka01), 50},
            {typeof(figurka02), 50},
            {typeof(figurka03), 50},
            {typeof(figurka04), 50},
            {typeof(figurka05), 50},
            {typeof(figurka06), 50},
            {typeof(figurka07), 50},
            {typeof(figurka08), 50},
            {typeof(figurka09), 50},
            {typeof(figurka10), 50},
            {typeof(figurka11), 50},
            {typeof(figurka12), 50},
            {typeof(figurka13), 50},
            {typeof(figurka14), 50},
            {typeof(figurka15), 50},
            {typeof(figurka16), 50},
            {typeof(figurka17), 50},
            {typeof(figurka18), 50},
            {typeof(figurka19), 50},
            {typeof(figurka20), 50},
            {typeof(figurka21), 50},
            {typeof(figurka22), 50},
            {typeof(figurka23), 50},
            {typeof(figurka24), 50},
            {typeof(figurka25), 50},
            {typeof(figurka26), 50},
            {typeof(figurka27), 50},
            {typeof(figurka28), 50},
            {typeof(figurka29), 50},
            {typeof(figurka30), 50},
            {typeof(SmallEmptyPot), 20},
            {typeof(LargeEmptyPot), 20},
            {typeof(PottedPlant), 20},
            {typeof(PottedPlant1), 20},
            {typeof(PottedPlant2), 20},
            {typeof(PottedTree), 20},
            {typeof(PottedTree1), 20},
            {typeof(PottedTree2), 20},
            {typeof(PottedTree3), 20},
            {typeof(PottedTree4), 20},
            {typeof(BoilingCauldronEastAddonDeed), 10},
            {typeof(BoilingCauldronNorthAddonDeed), 10},
            {typeof(IronWire), 10},
            {typeof(CopperWire), 10},
            {typeof(SilverWire), 10},
            {typeof(GoldWire), 10},
            {typeof(carpet3mDeed), 5},
            {typeof(carpet4mDeed), 5},
            {typeof(carpet5mDeed), 5},
            {typeof(carpet6mDeed), 5},
            {typeof(CreepyPortraitE), 5},
            {typeof(CreepyPortraitS), 5},
            {typeof(DisturbingPortraitE), 5},
            {typeof(DisturbingPortraitS), 5},
            {typeof(UnsettlingPortraitE), 5},
            {typeof(UnsettlingPortraitS), 5}
        };

        private static Item CreateDecoMajor(int type) {
            return SelectRandomType(m_majorDecoTypes);
        }

        private static Type[] m_artLvl1 = new Type[]
        {
            typeof(Raikiri),
            typeof(PeasantsBokuto),
            typeof(PixieSwatter),
            typeof(Frostbringer),
            typeof(SzyjaGeriadoru),
            typeof(BlazenskieSzczescie),
            typeof(KulawyMagik),
            typeof(KilofZRuinTwierdzy),
            typeof(SkalpelDoktoraBrandona),
            typeof(JaszczurzySzal),
            typeof(OblivionsNeedle),
            typeof(Bonesmasher),
            typeof(ColdForgedBlade),
            typeof(DaimyosHelm),
            typeof(LegsOfStability),
            typeof(AegisOfGrace),
            typeof(AncientFarmersKasa),
            typeof(StudniaOdnowy)
        };

        private static Type[] m_artLvl2 = new Type[]
        {
            typeof(Tyrfing),
            typeof(Arteria),
            typeof(ArcticDeathDealer),
            typeof(CavortingClub),
            typeof(Quernbiter),
            typeof(PromienSlonca),
            typeof(SwordsOfProsperity),
            typeof(TeczowaNarzuta),
            typeof(SmoczeKosci),
            typeof(RekawiceFredericka),
            typeof(OdbijajacyStrzaly),
            typeof(HuntersHeaddress),
            typeof(BurglarsBandana),
            typeof(SpodnieOswiecenia),
            typeof(KiltZycia),
            typeof(ArkanaZywiolow),
            typeof(OstrzeCienia),
            typeof(TalonBite),
            typeof(SilvanisFeywoodBow),
            typeof(BrambleCoat),
            typeof(OrcChieftainHelm),
            typeof(ShroudOfDeciet),
            typeof(CaptainJohnsHat),
            typeof(EssenceOfBattle),
        };
        
        private static Type[] m_artLvl3 = new Type[]
        {
            typeof(HebanowyPlomien),
            typeof(PomstaGrima),
            typeof(MaskaSmierci),
            typeof(SmoczyNos),
            typeof(StudniaOdnowy),
            typeof(Aegis),
            typeof(HanzosBow),
            typeof(MagicznySaif),
            typeof(StrzalaAbarisa),
            typeof(FangOfRactus),
            typeof(RighteousAnger),
            typeof(Stormgrip),
            typeof(LeggingsOfEmbers),
            typeof(SmoczeJelita),
            typeof(SongWovenMantle),
            typeof(StitchersMittens),
            typeof(FeyLeggings),
            //typeof(PadsOfTheCuSidhe),
            typeof(DjinnisRing),
            typeof(PendantOfTheMagi),
        };

        private static Type[] m_artLvl4 = new Type[]
        {
            typeof(Stormgrip),
            typeof(FeyLeggings),
            typeof(PendantOfTheMagi), // maszyjnik trzech krolow
            typeof(EssenceOfBattle),
            typeof(HuntersHeaddress),
            typeof(SpodnieOswiecenia),
            typeof(OrcChieftainHelm),
            typeof(KulawyMagik),
            typeof(SmoczyNos), // nogawice ozdobione luskami smoka
        };


        private enum ArtType
        {
            Art1,
            Art2,
            Art3,
            Art4
        }

        public static Item CreateArtifacts(int type)
        {
            Type itemType;
            switch ((ArtType)type)
            {
                case ArtType.Art4: itemType = Utility.RandomList(m_artLvl4);
                    break;
                case ArtType.Art3: itemType = Utility.RandomList(m_artLvl3);
                    break;
                case ArtType.Art2: itemType = Utility.RandomList(m_artLvl2);
                    break;
                default: itemType = Utility.RandomList(m_artLvl1);
                    break;
            }
            
            Item art = (Item)Activator.CreateInstance(itemType);
            ((IIdentifiable)art).Identified = true;
            return art;
        }


        //private static Item CreateDung( int type )
        //{
        //    return new HorseDung();
        //}

        //private static Item CreateShoes( int type )
        //{
        //    return new HorseShoes();
        //}

        //private static Item CreateAqFishingNet( int type )
        //{
        //    return new AquariumFishingNet();
        //}		
        #endregion

        public static readonly HunterRewardCalculator Instance = new HunterRewardCalculator();

        public override int ComputePoints(int quantity, bool exceptional, BulkMaterialType material, int itemCount, Type type) {
            return 0;
        }
        public override int ComputeGold(int quantity, bool exceptional, BulkMaterialType material, int itemCount, Type type) {
            return 0;
        }

        private static int[] GoldPerCreature = new int[] {30, 100, 300, 600};

        public int ComputeGold(int creatureAmount, int level)
        {
            int levelIndex = (level - 1);
            int gold = GoldPerCreature[levelIndex] * creatureAmount;

            return Utility.RandomMinMax((int)(gold * 0.95), (int)(gold * 1.05)); // plus minus 5%
        }

        public override int ComputeGold(SmallBOD bod)
        {
            return ComputeGold(bod.AmountMax, bod.Level);
        }

        public override int ComputeGold(LargeBOD bod)
        {
            return bod.Entries.Length * ComputeGold(bod.AmountMax, bod.Level);
        }

        public override int ComputePoints(SmallBOD bod) {
            int basePoints = 0;
            int levelPoints = 0;
            switch (bod.Level)
            {
                default:
                case 1: levelPoints = 0; break;
                case 2: levelPoints = 100; break;
                case 3: levelPoints = 200; break;
                case 4: levelPoints = 500; break;
            }
            int amountPoints;
            switch (bod.AmountMax) {
                case 10: amountPoints = 10; break;
                case 15: amountPoints = 25; break;
                case 20: amountPoints = 50; break;
                default: amountPoints = 0; break;
            }
            return basePoints + levelPoints + amountPoints;
        }

        public override int ComputePoints(LargeBOD bod) {
            int basePoints = 200;
            int levelPoints = 0;
            switch (bod.Level)
            {
                case 1: levelPoints = 0; break;
                case 2: levelPoints = 100; break;
                case 3: levelPoints = 200; break;
                case 4: levelPoints = 500; break;
            }
            int amountPoints;
            switch (bod.AmountMax) {
                case 10: amountPoints = 10; break;
                case 15: amountPoints = 25; break;
                case 20: amountPoints = 50; break;
                default: amountPoints = 0; break;
            }
            return basePoints + levelPoints + amountPoints;
        }

        static int Amount(int x) {
            return x;
        }

        public HunterRewardCalculator() {

            const int PIGMENT_1 = 0;
            const int PIGMENT_2 = 1;

            // Konstrukcja new RewardItem( ilosc procent ze zostanie wybrany, grupa)
            // Konstrukcja new RewardItem( ilosc procent ze zostanie wybrany, grupa, typ) // typ moze byc uzyty np przy rozroznieniu poziomu talizmanow czy losowania artefaktow

            // !! Nagrody w kazdym wierszu musza byc posortowane wg szansy wypadania: w kolejnosci od najwiekszej (po lewej) do najmniejszej (po prawej) !!
            // !! (w przeciwnym wypadku beda sie losowac z niepoprawna szansa) !!

            Groups = new RewardGroup[]
                {
                    new RewardGroup(  0, new RewardItem(60, DecoMinor), new RewardItem(20, Pigments, PIGMENT_1), new RewardItem(20, TransPowders, Amount(10))),
                    new RewardGroup( 25, new RewardItem(50, DecoMinor), new RewardItem(30, Pigments, PIGMENT_1), new RewardItem(20, TransPowders, Amount(13))),
                    new RewardGroup( 50, new RewardItem(40, DecoMinor), new RewardItem(40, Pigments, PIGMENT_1), new RewardItem(20, TransPowders, Amount(15))),

                    new RewardGroup(100, new RewardItem(20, DecoMinor), new RewardItem(20, DecoMajor), new RewardItem(20, Pigments, PIGMENT_1), new RewardItem(20, Pigments, PIGMENT_2), new RewardItem(20, TransPowders, Amount(20))),
                    new RewardGroup(125, new RewardItem(30, DecoMajor), new RewardItem(30, Pigments, PIGMENT_2), new RewardItem(10, Pigments, PIGMENT_1), new RewardItem(10, DecoMinor), new RewardItem(10, TransPowders, Amount(20)), new RewardItem(10, DurabilityPowder)),
                    new RewardGroup(150, new RewardItem(40, DecoMajor), new RewardItem(40, Pigments, PIGMENT_2), new RewardItem(20, DurabilityPowder)),

                    new RewardGroup(200, new RewardItem(60, DecoMajor), new RewardItem(20, DurabilityPowder), new RewardItem(20, Artifacts, (int)ArtType.Art1)),
                    new RewardGroup(225, new RewardItem(40, Artifacts, (int)ArtType.Art1), new RewardItem(30, DecoMajor), new RewardItem(15, Talismans, (int)TalizmanType.Level2), new RewardItem(10, DurabilityPowder), new RewardItem(5, PetResurrectPotion)),
                    new RewardGroup(250, new RewardItem(60, Artifacts, (int)ArtType.Art1), new RewardItem(30, Talismans, (int)TalizmanType.Level2), new RewardItem(10, PetResurrectPotion)),

                    new RewardGroup(300, new RewardItem(40, Talismans, (int)TalizmanType.Level2), new RewardItem(40, Artifacts, (int)ArtType.Art2), new RewardItem(20, Artifacts, (int)ArtType.Art1)),
                    new RewardGroup(325, new RewardItem(50, Artifacts, (int)ArtType.Art2), new RewardItem(35, Talismans, (int)TalizmanType.Level2), new RewardItem(10, Artifacts, (int)ArtType.Art1), new RewardItem(5, PetResurrectPotion)),
                    new RewardGroup(350, new RewardItem(60, Artifacts, (int)ArtType.Art2), new RewardItem(30, Talismans, (int)TalizmanType.Level2), new RewardItem(10, PetResurrectPotion)),

                    new RewardGroup(400, new RewardItem(40, Talismans, (int)TalizmanType.Level3), new RewardItem(40, Artifacts, (int)ArtType.Art3), new RewardItem(20, Artifacts, (int)ArtType.Art2)),
                    new RewardGroup(425, new RewardItem(55, Artifacts, (int)ArtType.Art3), new RewardItem(35, Talismans, (int)TalizmanType.Level3), new RewardItem(10, Artifacts, (int)ArtType.Art2)),
                    new RewardGroup(450, new RewardItem(70, Artifacts, (int)ArtType.Art3), new RewardItem(30, Talismans, (int)TalizmanType.Level3)),

                    new RewardGroup(500, new RewardItem(60, Talismans, (int)TalizmanType.Level3), new RewardItem(10, Talismans, (int)TalizmanType.Level2), new RewardItem(20, Artifacts, (int)ArtType.Art2), new RewardItem(10, Artifacts, (int)ArtType.Art1)),
                    new RewardGroup(525, new RewardItem(70, Talismans, (int)TalizmanType.Level3), new RewardItem(15, Artifacts, (int)ArtType.Art2), new RewardItem(5, Artifacts, (int)ArtType.Art1), new RewardItem(5, Artifacts, (int)ArtType.Art3), new RewardItem(5, Artifacts, (int)ArtType.Art4)),
                    new RewardGroup(550, new RewardItem(80, Talismans, (int)TalizmanType.Level3), new RewardItem(10, Artifacts, (int)ArtType.Art3), new RewardItem(10, Artifacts, (int)ArtType.Art4)),

                    new RewardGroup(700, new RewardItem(60, Artifacts, (int)ArtType.Art4), new RewardItem(40, Artifacts, (int)ArtType.Art3)),
                    new RewardGroup(725, new RewardItem(70, Artifacts, (int)ArtType.Art4), new RewardItem(30, Artifacts, (int)ArtType.Art3)),
                    new RewardGroup(750, new RewardItem(80, Artifacts, (int)ArtType.Art4), new RewardItem(20, Artifacts, (int)ArtType.Art3)),
                };
        }
    }
}
