using System;
using Server;
using Server.Mobiles;
using Server.Items;

namespace Server.Engines.Quests.CraftingExperiments
{
    public class BlacksmithyExperiment : CraftingExperiment
    {
        public BlacksmithyExperiment(PlayerMobile from) : base(from)
        {
        }

        public BlacksmithyExperiment() : base() { } // for deserialization purposes

        public override object Name
        {
            get
            {
                return "Eksperyment kowala";
            }
        }

        private static Type[] m_RareResourcesTypes = new Type[] { typeof(BlueDiamond), typeof(DarkSapphire), typeof(EcruCitrine), typeof(FireRuby), typeof(PerfectEmerald), typeof(Turquoise) };
        private static string m_RareResourcesNames = "niebieski diament, czarny szafir, krysztal w kolorze ecru, ogniowy rubin, idealny bursztyn, turkusowy kamien";
        public override Type[] RareResourcesTypes { get { return m_RareResourcesTypes; } }
        public override string RareResourcesNames { get { return m_RareResourcesNames; } }
        public override int RareResourcesQuantity { get { return 1; } }


        private static Type[] m_ProductsTypes = new Type[] { typeof(BronzeShield) };
        private static CraftResource[] m_ProductsFirstResources = new CraftResource[] { CraftResource.Agapite, CraftResource.Verite, CraftResource.Valorite };
        private static CraftResource[] m_ProductsSecondResources = new CraftResource[] { };

        private static string m_ProductsNames = "okragla tarcza z agapitu, verytu, lub valorytu";
        public override Type[] ProductsTypes { get { return m_ProductsTypes; } }
        public override CraftResource[] ProductsFirstResources { get { return m_ProductsFirstResources; } }
        public override CraftResource[] ProductsSecondResources { get { return m_ProductsSecondResources; } }
        public override string ProductsNames { get { return m_ProductsNames; } }
        public override int ProductsQuantity { get { return 30; } }

        public override TalismanSkill RewardType { get { return TalismanSkill.Blacksmithy; } }
    }

    public class BowFletchingExperiment : CraftingExperiment
    {
        public BowFletchingExperiment(PlayerMobile from) : base(from)
        {
        }

        public BowFletchingExperiment() : base() { } // for deserialization purposes

        public override object Name
        {
            get
            {
                return "Eksperyment lukmistrza";
            }
        }

        private static Type[] m_RareResourcesTypes = new Type[] { typeof(BrilliantAmber) };
        private static string m_RareResourcesNames = "nieskazitelny bursztyn";
        public override Type[] RareResourcesTypes { get { return m_RareResourcesTypes; } }
        public override string RareResourcesNames { get { return m_RareResourcesNames; } }
        public override int RareResourcesQuantity { get { return 6; } }


        private static Type[] m_ProductsTypes = new Type[] { typeof(Bow) };
        private static CraftResource[] m_ProductsFirstResources = new CraftResource[] { CraftResource.Heartwood, CraftResource.Bloodwood, CraftResource.Frostwood };
        private static CraftResource[] m_ProductsSecondResources = new CraftResource[] { };

        private static string m_ProductsNames = "luk z drewna gietkiego, opalonego, lub zmarznietego";
        public override Type[] ProductsTypes { get { return m_ProductsTypes; } }
        public override CraftResource[] ProductsFirstResources { get { return m_ProductsFirstResources; } }
        public override CraftResource[] ProductsSecondResources { get { return m_ProductsSecondResources; } }
        public override string ProductsNames { get { return m_ProductsNames; } }
        public override int ProductsQuantity { get { return 30; } }

        public override TalismanSkill RewardType { get { return TalismanSkill.Fletching; } }
    }

    public class TailoringExperiment : CraftingExperiment
    {
        public TailoringExperiment(PlayerMobile from) : base(from)
        {
        }

        public TailoringExperiment() : base() { } // for deserialization purposes

        public override object Name
        {
            get
            {
                return "Eksperyment krawca";
            }
        }

        private static Type[] m_RareResourcesTypes = new Type[] { typeof(GoldenWool) };
        private static string m_RareResourcesNames = "zlote runo";
        public override Type[] RareResourcesTypes { get { return m_RareResourcesTypes; } }
        public override string RareResourcesNames { get { return m_RareResourcesNames; } }
        public override int RareResourcesQuantity { get { return 6; } }


        private static Type[] m_ProductsTypes = new Type[] { typeof(BoneChest) };
        private static CraftResource[] m_ProductsFirstResources = new CraftResource[] { CraftResource.BarbedLeather };
        private static CraftResource[] m_ProductsSecondResources = new CraftResource[] { };

        private static string m_ProductsNames = "kosciana tunika z zielonej skory";
        public override Type[] ProductsTypes { get { return m_ProductsTypes; } }
        public override CraftResource[] ProductsFirstResources { get { return m_ProductsFirstResources; } }
        public override CraftResource[] ProductsSecondResources { get { return m_ProductsSecondResources; } }
        public override string ProductsNames { get { return m_ProductsNames; } }
        public override int ProductsQuantity { get { return 30; } }

        public override TalismanSkill RewardType { get { return TalismanSkill.Tailoring; } }
    }

    public abstract class CraftingExperiment : QuestSystem
    {
        private static Type[] m_TypeReferenceTable = new Type[]
            {
                typeof( CraftingExperiments.DontOfferConversation ),
                typeof( CraftingExperiments.AcceptConversation ),
                typeof( CraftingExperiments.DuringBringRareResourceConversation ),
                typeof( CraftingExperiments.AfterBringRareResourceConversation ),
                typeof( CraftingExperiments.DuringBringProductsConversation ),
                typeof( CraftingExperiments.EndConversation ),

                typeof( CraftingExperiments.BringRareResourceObjective ),
                typeof( CraftingExperiments.BringProductsObjective )
            };

        public override Type[] TypeReferenceTable { get { return m_TypeReferenceTable; } }


        public abstract Type[] RareResourcesTypes { get; }
        public abstract string RareResourcesNames { get; }
        public abstract int RareResourcesQuantity { get; }


        public abstract Type[] ProductsTypes { get; }
        public abstract string ProductsNames { get; }
        public abstract CraftResource[] ProductsFirstResources { get; }
        public abstract CraftResource[] ProductsSecondResources { get; }
        public abstract int ProductsQuantity { get; }

        public abstract TalismanSkill RewardType { get; }

        public override object Name
        {
            get
            {
                return "Eksperyment rzemieslniczy";
            }
        }

        public override object OfferMessage
        {
            get
            {
                return "Potrzebuje pomocy w moim... eksperymencie! Pracuje nad nowym sposobem ulepszania pewnego typu przedmiotow. " +
                       "Bedzie mi do tego potrzeba troche niezwykle rzadkich surowcow. Oraz kilka wyrobów z bardzo cennego tworzywa. <BR><BR>" +
                       "Czy zechcesz mi pomoc w pozyskaniu tych przedmiotow?";
            }
        }

        public override TimeSpan RestartDelay { get { return TimeSpan.Zero; } }
        public override bool IsTutorial { get { return false; } }

        public override int Picture { get { return 0x15C9; } }

        public CraftingExperiment(PlayerMobile from) : base(from)
        {
        }
        public CraftingExperiment() : base() { }

        public override void Accept()
        {
            base.Accept();

            AddConversation(new AcceptConversation());
        }

        public void GiveRewardTo(PlayerMobile player)
        {
            MasterCraftsmanTalisman talisman = new MasterCraftsmanTalisman(1000, 12123, RewardType);

            if (player.AddToBackpack(talisman))
            {
                player.SendMessage("W nagrode otrzymales talizman.");
            }
            else
            {
                player.SendMessage("W nagrode otrzymales talizman, ktory wyl¹dowal pod twoimi stopami!");
            }

            Item gold = new Gold(Utility.RandomMinMax(7000, 8000));

            if (player.AddToBackpack(gold))
            {
                player.SendMessage("W nagrode otrzymales zloto.");
            }
            else
            {
                player.SendMessage("W nagrode otrzymales zloto, ktore wyl¹dowa³o pod twoimi stopami!");
            }
        }
    }
}