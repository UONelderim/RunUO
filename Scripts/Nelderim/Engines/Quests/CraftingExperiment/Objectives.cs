using System;
using Server;
using Server.Mobiles;
using Server.Items;
using System.Collections.Generic;

namespace Server.Engines.Quests.CraftingExperiments
{
    public class BringRareResourceObjective : QuestObjective
    {
        public override object Message
        {
            get
            {
                return "Przynieœ dowolne z surowców: " + ((CraftingExperiment)System).RareResourcesNames + ".";
            }
        }

        public override int MaxProgress { get { return ((CraftingExperiment)System).RareResourcesQuantity; } }

        public BringRareResourceObjective()
        {
        }

        public bool IsProperProduct(Item dropped)
        {
            CraftingExperiment qs = System as CraftingExperiment;
            if (qs == null)
                return false;

            foreach (Type rareResourceType in qs.RareResourcesTypes)
            {
                if (dropped.GetType() == rareResourceType)
                {
                    return true;
                }
            }

            return false;
        }

        public override void RenderProgress(BaseQuestGump gump)
        {
            if (!Completed)
            {
                gump.AddLabel(70, 260, 0x64, "Dostarczonych surowców:");
                gump.AddLabel(70, 280, 0x64, CurProgress.ToString());
                gump.AddLabel(100, 280, 0x64, "/");
                gump.AddLabel(130, 280, 0x64, MaxProgress.ToString());
            }
            else
            {
                base.RenderProgress(gump);
            }
        }
        public override void OnComplete()
        {
            System.AddConversation(new AfterBringRareResourceConversation());
        }
    }

    public class BringProductsObjective : QuestObjective
    {
        public override object Message
        {
            get
            {
                return "Dostarcz kilka sztuk porz¹dnych produktów... powiedzmy: " + ((CraftingExperiment)System).ProductsNames + ".";
            }
        }

        public override int MaxProgress { get { return ((CraftingExperiment)System).ProductsQuantity; } }

        public BringProductsObjective()
        {
        }

        public bool IsProperProduct(Item dropped)
        {
            CraftingExperiment qs = System as CraftingExperiment;
            if (qs == null)
                return false;

            foreach (Type productType in qs.ProductsTypes)
            {
                if (dropped.GetType() != productType)
                    continue;

                CraftResource droppedFirstResource = CraftResource.None;
                CraftResource droppedSecondResource = CraftResource.None;
                if (dropped is BaseWeapon)
                {
                    droppedFirstResource = ((BaseWeapon)dropped).Resource;
                    droppedSecondResource = ((BaseWeapon)dropped).Resource2;
                }
                else if (dropped is BaseArmor)
                {
                    droppedFirstResource = ((BaseArmor)dropped).Resource;
                    droppedSecondResource = ((BaseArmor)dropped).Resource2;
                }

                bool properFirstResource = qs.ProductsFirstResources.Length == 0 || droppedFirstResource == CraftResource.None || Array.IndexOf(qs.ProductsFirstResources, droppedFirstResource) > -1;
                bool properSecondResource = qs.ProductsSecondResources.Length == 0 || droppedSecondResource == CraftResource.None || Array.IndexOf(qs.ProductsSecondResources, droppedSecondResource) > -1;

                if (properFirstResource && properSecondResource)
                {
                    return true;
                }
            }

            return false;
        }

        public override void RenderProgress(BaseQuestGump gump)
        {
            if (!Completed)
            {
                gump.AddLabel(70, 260, 0x64, "Dostarczonych produktów:");
                gump.AddLabel(70, 280, 0x64, CurProgress.ToString());
                gump.AddLabel(100, 280, 0x64, "/");
                gump.AddLabel(130, 280, 0x64, MaxProgress.ToString());
            }
            else
            {
                base.RenderProgress(gump);
            }
        }

        public override void OnComplete()
        {
            System.AddConversation(new EndConversation()); 
        }
    }
}