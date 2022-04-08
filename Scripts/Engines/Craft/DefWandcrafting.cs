using System;
using Server;
using Server.Items;
using Server.Factions;
using Server.Targeting;

namespace Server.Engines.Craft
{
    public class DefWandCrafting : CraftSystem
    {
        public override SkillName MainSkill
        {
            get { return SkillName.ItemID; }
        }

        public override int GumpTitleNumber
        {
            get { return 1044000; } // <CENTER>MENU TWORZENIA PRZEDMIOTOW IDENTYFIKACJI</CENTER>
        }

        private static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get
            {
                if (m_CraftSystem == null)
                    m_CraftSystem = new DefWandCrafting();

                return m_CraftSystem;
            }
        }

        private DefWandCrafting() : base(1, 1, 1.25)// base( 1, 1, 3.0 )
        {
        }

        public override double GetChanceAtMin(CraftItem item)
        {
            if (item.NameNumber == 1044258 || item.NameNumber == 1046445) // potion keg and faction trap removal kit
                return 0.5; // 50%

            return 0.0; // 0%
        }

        public override int CanCraft(Mobile from, BaseTool tool, Type itemType)
        {
            if (tool == null || tool.Deleted || tool.UsesRemaining < 0)
                return 1044038; // You have worn out your tool!
            else if (!BaseTool.CheckAccessible(tool, from))
                return 1044263; // The tool must be on your person to use.
            else if (itemType != null && (itemType.IsSubclassOf(typeof(BaseFactionTrapDeed)) || itemType == typeof(FactionTrapRemovalKit)) && Faction.Find(from) == null)
                return 1044573; // You have to be in a faction to do that.

            return 0;
        }

        public override void PlayCraftEffect(Mobile from)
        {
            from.PlaySound(0x241);
        }

        public override int PlayEndingEffect(Mobile from, bool failed, bool lostMaterial, bool toolBroken, int quality, bool makersMark, CraftItem item)
        {
            if (toolBroken)
                from.SendLocalizedMessage(1044038); // You have worn out your tool

            if (failed)
            {
                if (lostMaterial)
                    return 1044043; // You failed to create the item, and some of your materials are lost.
                else
                    return 1044157; // You failed to create the item, but no materials were lost.
            }
            else
            {
                if (quality == 0)
                    return 502785; // You were barely able to make this item.  It's quality is below average.
                else if (makersMark && quality == 2)
                    return 1044156; // You create an exceptional quality item and affix your maker's mark.
                else if (quality == 2)
                    return 1044155; // You create an exceptional quality item.
                else
                    return 1044154; // You create the item.
            }
        }

        public override bool ConsumeOnFailure(Mobile from, Type resourceType, CraftItem craftItem)
        {
            if (resourceType == typeof(Silver))
                return false;

            return base.ConsumeOnFailure(from, resourceType, craftItem);
        }

        public override void InitCraftList()
        {
            int index = -1;

            #region Wands Identification
            index = AddCraft(typeof(IDWand10uses), 1044115, 1032601, 100.0, 100.0, typeof(PowderOfTranslocation), 1029912, 1, 1029973);
            AddRes(index, typeof(ZoogiFungus), 1029911, 20, 1029975);
            AddRes(index, typeof(BarkFragment), 1032687, 1, 1029975);

            index = AddCraft(typeof(IDWand20uses), 1044115, 1032602, 100.0, 100.0, typeof(PowderOfTranslocation), 1029912, 1, 1029973);
            AddRes(index, typeof(ZoogiFungus), 1029911, 40, 1029975);
            AddRes(index, typeof(BarkFragment), 1032687, 1, 1029975);

            index = AddCraft(typeof(IDWand30uses), 1044115, 1032603, 100.0, 100.0, typeof(PowderOfTranslocation), 1029912, 1, 1029973);
            AddRes(index, typeof(ZoogiFungus), 1029911, 60, 1029975);
            AddRes(index, typeof(BarkFragment), 1032687, 1, 1029975);

            index = AddCraft(typeof(IDWand50uses), 1044115, 1032604, 100.0, 100.0, typeof(PowderOfTranslocation), 1029912, 1, 1029973);
            AddRes(index, typeof(ZoogiFungus), 1029911, 100, 1029975);
            AddRes(index, typeof(BarkFragment), 1032687, 1, 1029975);
            #endregion

            #region Scrolls Identification
            index = AddCraft(typeof(IdentificationScrollLvl1), 1065949, 1065950, 25.0, 50.0, typeof(BlankScroll), 1023636, 1, 1044574);
            AddRes(index, typeof(ZoogiFungus), 1029911, 2, 1029975);

            index = AddCraft(typeof(IdentificationScrollLvl2), 1065949, 1065951, 45.0, 70.0, typeof(BlankScroll), 1023636, 1, 1044574);
            AddRes(index, typeof(ZoogiFungus), 1029911, 4, 1029975);

            index = AddCraft(typeof(IdentificationScrollLvl3), 1065949, 1065952, 65.0, 85.0, typeof(BlankScroll), 1023636, 1, 1044574);
            AddRes(index, typeof(ZoogiFungus), 1029911, 6, 1029975);

            index = AddCraft(typeof(IdentificationScrollLvl4), 1065949, 1065953, 75.0, 95.0, typeof(BlankScroll), 1023636, 1, 1044574);
            AddRes(index, typeof(ZoogiFungus), 1029911, 8, 1029975);
            #endregion

            #region Blank scrolls
            index = AddCraft(typeof(BlankScroll), 1044294, 1044377, 40.0, 80.0, typeof(Cloth), 1044286, 1, 1044253);
            AddRes(index, typeof(Board), 1015101, 1, 1044253);
            index = AddCraft(typeof(BlankScroll), 1044294, 1064804, 40.0, 80.0, typeof(Cloth), 1044286, 1, 1044253);
            AddRes(index, typeof(Board), 1015101, 1, 1044253);
            SetUseAllRes(index, true);
            #endregion

            MarkOption = false;
            Repair = false;
            CanEnhance = false;
        }
    }
}