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
                return "W tem momencie potrzebuje troche rzadkich surowcow. Oraz kilka wyrobów... ale tym zajmiemy siê póŸniej. " +
                       "Póki co dostarcz mi ten surowiec. Przynies prosze " + ((CraftingExperiment)System).RareResourcesQuantity +
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
                return "O, to znowu ty! Przynios³eœ mi te rzadkie surowce?<BR>Eh... widzê, ¿e jeszcze nie. Nadal na nie czekam.";
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
                return "Tak! To ju¿ wszystkie surowce jakich potrzebowa³em!<BR>"
                     + "Zatem mój pierwszy problem zosta³ rozwi¹zany! Dziêkujê ci za to!<BR><BR>"
                     + "Teraz pora zaj¹æ siê samymi wyrobami...<BR>"
                     + "Do tego bêdzie mi potrzeba kilka sztuk tych przedmiotów. Zdob¹dŸ je dla mnie!";
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
                return "Witaj ponownie! Czy masz dla mnie te wyroby?<BR>Nadal czekam na ich dostawê..";
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
                return "Wspaniale, mam ju¿ wszystko, teraz mój eksperyment musi siê udaæ!<BR>"
                     + "Nie myœl jednak, ¿e zdradzê ci jego tajniki, co to to nie!<BR><BR>"
                     + "A, tak... twoja nagroda... gdzie ja to mia³em. O, jest, trzymaj proszê, mo¿e tobie siê to przyda.";
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