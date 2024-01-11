using System;
using Server;
using Server.Items;

namespace Server.Engines.Quests.CraftingExperiments
{
    public class DontOfferConversation : QuestConversation
    {
        public override object Message
        {
            get
            {
                return "Moglaby mi sie przydac twoja pomoc, ale wyglada na to ze jestes obecnie zajety innym zadaniem. " +
                       "Wroc do mnie jak juz skonczysz cokolwiek tam teraz robisz, byc moze nadal bede miec dla ciebie zajecie.";
            }
        }

        public override bool Logged { get { return false; } }

        public DontOfferConversation()
        {
        }
    }

    public class AcceptConversation : QuestConversation
    {
        public override object Message
        {
            get
            {
                return "W tem momencie potrzebuje troche rzadkich surowcow. Oraz kilka wyrobow... ale tym zajmiemy sie pozniej. " +
                       "Poki co dostarcz mi ten surowiec. Przynies prosze " + ((CraftingExperiment)System).RareResourcesQuantity +
                       " " + ((CraftingExperiment)System).RareResourcesNames + ".";
            }
        }

        public AcceptConversation()
        {
        }

        public override void OnRead()
        {
            System.AddObjective(new BringRareResourceObjective());
        }
    }

    public class DuringBringRareResourceConversation : QuestConversation
    {
        public override object Message
        {
            get
            {
                return "O, to znowu ty! Przyniosles mi te rzadkie surowce?<BR>Eh... widze, ze jeszcze nie. Nadal na nie czekam.";
            }
        }

        public override bool Logged { get { return false; } }

        public DuringBringRareResourceConversation()
        {
        }
    }

    public class AfterBringRareResourceConversation : QuestConversation
    {
        public override object Message
        {
            get
            {
                return "Tak! To juz wszystkie surowce jakich potrzebowalem!<BR>"
                     + "Zatem moj pierwszy problem zostal rozwiazany! Dziekuje ci za to!<BR><BR>"
                     + "Teraz pora zajac sie samymi wyrobami...<BR>"
                     + "Do tego bedzie mi potrzeba kilka sztuk tych przedmiotow. Zdobadz je dla mnie!";
            }
        }

        public override bool Logged { get { return true; } }

        public override void OnRead()
        {
            System.AddObjective(new BringProductsObjective());
        }

        public AfterBringRareResourceConversation()
        {
        }
    }

    public class DuringBringProductsConversation : QuestConversation
    {
        public override object Message
        {
            get
            {
                return "Witaj ponownie! Czy masz dla mnie te wyroby?<BR>Nadal czekam na ich dostawe..";
            }
        }

        public override bool Logged { get { return false; } }

        public DuringBringProductsConversation()
        {
        }
    }

    public class EndConversation : QuestConversation
    {
        public override object Message
        {
            get
            {
                return "Wspaniale, mam juz wszystko, teraz moj eksperyment musi sie udac!<BR>"
                     + "Nie mysl jednak, ze zdradze ci jego tajniki, co to to nie!<BR><BR>"
                     + "A, tak... twoja nagroda... gdzie ja to mialem. O, jest, trzymaj prosze, moze tobie sie to przyda.";
            }
        }
        public override bool Logged { get { return true; } }

        public EndConversation()
        {
        }

        public override void OnRead()
        {
            ((CraftingExperiment)System).GiveRewardTo(System.From);

            System.Complete();
        }
    }
    
}