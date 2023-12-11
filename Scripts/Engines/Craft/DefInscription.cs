using System;
using Server.ACC.CSS.Systems.Undead;
using Server.ACC.CSS.Systems.Ancient;
using Server.ACC.CSS.Systems.Druid;
using Server.ACC.CSS.Systems.Bard;
using Server.ACC.CSS.Systems.Avatar;
using Server.ACC.CSS.Systems.Rogue;
using Server.ACC.CSS.Systems.Ranger;
using Server.ACC.CSS.Systems.Cleric;
using Server.Items;
using Server.Mobiles;
using Server.Spells;
using Server.Spells.Bushido;
using System.Collections.Generic;

namespace Server.Engines.Craft
{
    public class DefInscription : CraftSystem
    {
        public override SkillName MainSkill
        {
            get { return SkillName.Inscribe; }
        }

        public override int GumpTitleNumber
        {
            get { return 1044009; } // <CENTER>INSCRIPTION MENU</CENTER>
        }

        private static CraftSystem m_CraftSystem;

        public static CraftSystem CraftSystem
        {
            get
            {
                if (m_CraftSystem == null)
                    m_CraftSystem = new DefInscription();

                return m_CraftSystem;
            }
        }

        public override double GetChanceAtMin(CraftItem item)
        {
            return 0.0; // 0%
        }

        public void SetMinChanceForScrollAtGM(double minSkill, ref double maxSkill)
        {
            // make sure we have at least 66.7% chance to make a scroll at GM skill:
            if ((100.0 - minSkill) / (maxSkill - minSkill) < 0.667)
                maxSkill = minSkill + (100.0 - minSkill) / 0.667;
        }

        private DefInscription() : base(1, 1, 1.25)// base( 1, 1, 3.0 )
        {
        }

        public override int CanCraft(Mobile from, BaseTool tool, Type typeItem)
        {
            if (tool == null || tool.Deleted || tool.UsesRemaining < 0)
                return 1044038; // You have worn out your tool!
            else if (!BaseTool.CheckAccessible(tool, from))
                return 1044263; // The tool must be on your person to use.

            if (typeItem != null)
            {
                object o = Activator.CreateInstance(typeItem);

                if (o is SpellScroll)
                {
                    SpellScroll scroll = (SpellScroll)o;
                    Spellbook book = Spellbook.Find(from, scroll.SpellID);

                    bool hasSpell = (book != null && book.HasSpell(scroll.SpellID));

                    scroll.Delete();

                    return (hasSpell ? 0 : 1042404); // null : You don't have that spell!
                }
                else if (o is Item)
                {
                    ((Item)o).Delete();
                }
            }

            return 0;
        }

        public override void PlayCraftEffect(Mobile from)
        {
            from.PlaySound(0x249);
        }

        private static Type typeofSpellScroll = typeof(SpellScroll);

        public override int PlayEndingEffect(Mobile from, bool failed, bool lostMaterial, bool toolBroken, int quality, bool makersMark, CraftItem item)
        {
            if (toolBroken)
                from.SendLocalizedMessage(1044038); // You have worn out your tool

            if (!typeofSpellScroll.IsAssignableFrom(item.ItemType)) //  not a scroll
            {
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
            else
            {
                if (failed)
                    return 501630; // You fail to inscribe the scroll, and the scroll is ruined.
                else
                    return 501629; // You inscribe the spell and put the scroll in your backpack.
            }
        }

        // 04.07.2012 :: zombie
        private int GetRegAmount(Type regType, Type[] regs, int[] amounts, Type itemType)
        {
            try
            {
                int index = Array.IndexOf(regs, regType);
                return amounts[index];
            }
            catch
            {
                Console.WriteLine("DefInscription: nieprawidlowa liczba skladnikow dla typu: " + itemType);

                return 1;
            }
        }

        private void AddSpell(int spell, int mana, double minSkill, double maxSkill, TextDefinition group, TextDefinition start, Type type, int[] amounts, params Type[] regs)
        {
            if (amounts == null)
            {
                amounts = new int[regs.Length];
                for (int i = 0, count = amounts.Length; i < count; i++)
                    amounts[i] = 1;
            }

            //int regIndex = Array.IndexOf( Reagent.Types, regs[0].Type );
            int index = AddCraft(type, group, start + spell, minSkill, maxSkill, regs[0], GetNameRes(regs[0]), GetRegAmount(regs[0], regs, amounts, type), 501627);

            for (int i = 1, count = regs.Length; i < count; i++)
            {
                Type regType = regs[i];
                AddRes(index, regType, GetNameRes(regType), GetRegAmount(regType, regs, amounts, type), 501627);
            }

            AddRes(index, typeof(BlankScroll), 1044377, 1, 1044378);

            SetManaReq(index, mana);
        }

        private TextDefinition GetNameRes(Type type)
        {
            int id = CraftItem.ItemIDOf(type);

            return id < 0x4000 ? 1020000 + id : 1078872 + id;
        }

        private void AddMagerySpell(int spell, int circle, int mana, Type type, params Type[] regs)
        {
            AddMagerySpell(spell, circle, mana, type, null, regs);
        }

        private void AddMagerySpell(int spell, int circle, int mana, Type type, int[] amounts, params Type[] regs)
        {
            double minSkill, maxSkill;

            switch (circle)
            {
                default:
                case 1: minSkill = -25.0; maxSkill = 25.0; break;
                case 2: minSkill = -10.8; maxSkill = 39.2; break;
                case 3: minSkill = 03.5; maxSkill = 53.5; break;
                case 4: minSkill = 17.8; maxSkill = 67.8; break;
                case 5: minSkill = 32.1; maxSkill = 82.1; break;
                case 6: minSkill = 46.4; maxSkill = 96.4; break;
                case 7: minSkill = 60.7; maxSkill = 105.7; break;
                case 8: minSkill = 75.0; maxSkill = 125.0; break;
            }

            SetMinChanceForScrollAtGM(minSkill, ref maxSkill);

            // 17.09.2012 :: zombie :: cliloc - podzial na kregi
            string group = "Magia" ;

            if (circle <= 8) group = "Magia"; // 1-2 krag
          //  else if (circle <= 4) group = 1071069; // 3-4 krag
           // else if (circle <= 6) group = 1071070; // 5-6 krag
          //  else if (circle <= 8) group = 1071071; // 7-8 krag

            AddSpell(spell, mana, minSkill, maxSkill, group, 1044381, type, amounts, regs);
            // zombie
        }

        private void AddNecroSpell(int spell, int mana, double minSkill, Type type, params Type[] regs)
        {
            AddNecroSpell(spell, mana, minSkill, type, null, regs);
        }

        private void AddNecroSpell(int spell, int mana, double minSkill, Type type, int[] amounts, params Type[] regs)
        {
            double maxSkill = minSkill + 50;
            SetMinChanceForScrollAtGM(minSkill, ref maxSkill);

            AddSpell(spell, mana, minSkill, maxSkill, 1044109, 1060509, type, amounts, regs);
        }

        private void AddBushidoSpell(int spell, int mana, double minSkill, Type type, params Type[] regs)
        {
            AddBushidoSpell(spell, mana, minSkill, type, null, regs);
        }

        private void AddBushidoSpell(int spell, int mana, double minSkill, Type type, int[] amounts, params Type[] regs)
        {
            double maxSkill = minSkill + 50;
            SetMinChanceForScrollAtGM(minSkill, ref maxSkill);

            AddSpell(spell, mana, minSkill, maxSkill, 1044112, 1060595, type, amounts, regs);
        }

        private void AddNinjitsuSpell(int spell, int mana, double minSkill, Type type, params Type[] regs)
        {
            AddNinjitsuSpell(spell, mana, minSkill, type, null, regs);
        }

        private void AddNinjitsuSpell(int spell, int mana, double minSkill, Type type, int[] amounts, params Type[] regs)
        {
            double maxSkill = minSkill + 50;
            SetMinChanceForScrollAtGM(minSkill, ref maxSkill);

            AddSpell(spell, mana, minSkill, maxSkill, 1044113, 1060610, type, amounts, regs);
        }

        private void AddChivalrySpell(int spell, int mana, double minSkill, Type type, params Type[] regs)
        {
            AddChivalrySpell(spell, mana, minSkill, type, null, regs);
        }

        private void AddChivalrySpell(int spell, int mana, double minSkill, Type type, int[] amounts, params Type[] regs)
        {
            double maxSkill = minSkill + 50;
            SetMinChanceForScrollAtGM(minSkill, ref maxSkill);

            AddSpell(spell, mana, minSkill, maxSkill, 1044111, 1060493, type, amounts, regs);
        }
        // zombie

        public override void InitCraftList()
        {
            
            AddMagerySpell(0, 1, 4, typeof(ReactiveArmorScroll), Reagent.Garlic, Reagent.SpidersSilk, Reagent.SulfurousAsh);
            AddMagerySpell(1, 1, 4, typeof(ClumsyScroll), Reagent.Bloodmoss, Reagent.Nightshade);
            AddMagerySpell(2, 1, 4, typeof(CreateFoodScroll), Reagent.Garlic, Reagent.Ginseng, Reagent.MandrakeRoot);
            AddMagerySpell(3, 1, 4, typeof(FeeblemindScroll), Reagent.Nightshade, Reagent.Ginseng);
            AddMagerySpell(4, 1, 4, typeof(HealScroll), Reagent.Garlic, Reagent.Ginseng, Reagent.SpidersSilk);
            AddMagerySpell(5, 1, 4, typeof(MagicArrowScroll), Reagent.SulfurousAsh);
            AddMagerySpell(6, 1, 4, typeof(NightSightScroll), Reagent.SpidersSilk, Reagent.SulfurousAsh);
            AddMagerySpell(7, 1, 4, typeof(WeakenScroll), Reagent.Garlic, Reagent.Nightshade);

            AddMagerySpell(8, 2, 6, typeof(AgilityScroll), Reagent.Bloodmoss, Reagent.MandrakeRoot);
            AddMagerySpell(9, 2, 6, typeof(CunningScroll), Reagent.Nightshade, Reagent.MandrakeRoot);
            AddMagerySpell(10, 2, 6, typeof(CureScroll), Reagent.Garlic, Reagent.Ginseng);
            AddMagerySpell(11, 2, 6, typeof(HarmScroll), Reagent.Nightshade, Reagent.SpidersSilk);
            AddMagerySpell(12, 2, 6, typeof(MagicTrapScroll), Reagent.Garlic, Reagent.SpidersSilk, Reagent.SulfurousAsh);
            AddMagerySpell(13, 2, 6, typeof(MagicUnTrapScroll), Reagent.Bloodmoss, Reagent.SulfurousAsh);
            AddMagerySpell(14, 2, 6, typeof(ProtectionScroll), Reagent.Garlic, Reagent.Ginseng, Reagent.SulfurousAsh);
            AddMagerySpell(15, 2, 6, typeof(StrengthScroll), Reagent.Nightshade, Reagent.MandrakeRoot);

            AddMagerySpell(16, 3, 9, typeof(BlessScroll), Reagent.Garlic, Reagent.MandrakeRoot);
            AddMagerySpell(17, 3, 9, typeof(FireballScroll), Reagent.BlackPearl);
            AddMagerySpell(18, 3, 9, typeof(MagicLockScroll), Reagent.Bloodmoss, Reagent.Garlic, Reagent.SulfurousAsh);
            AddMagerySpell(19, 3, 9, typeof(PoisonScroll), Reagent.Nightshade);
            AddMagerySpell(20, 3, 9, typeof(TelekinisisScroll), Reagent.Bloodmoss, Reagent.MandrakeRoot);
            AddMagerySpell(21, 3, 9, typeof(TeleportScroll), Reagent.Bloodmoss, Reagent.MandrakeRoot);
            AddMagerySpell(22, 3, 9, typeof(UnlockScroll), Reagent.Bloodmoss, Reagent.SulfurousAsh);
            AddMagerySpell(23, 3, 9, typeof(WallOfStoneScroll), Reagent.Bloodmoss, Reagent.Garlic);

            AddMagerySpell(24, 4, 11, typeof(ArchCureScroll), Reagent.Garlic, Reagent.Ginseng, Reagent.MandrakeRoot);
            AddMagerySpell(25, 4, 11, typeof(ArchProtectionScroll), Reagent.Garlic, Reagent.Ginseng, Reagent.MandrakeRoot, Reagent.SulfurousAsh);
            AddMagerySpell(26, 4, 11, typeof(CurseScroll), Reagent.Garlic, Reagent.Nightshade, Reagent.SulfurousAsh);
            AddMagerySpell(27, 4, 11, typeof(FireFieldScroll), Reagent.BlackPearl, Reagent.SpidersSilk, Reagent.SulfurousAsh);
            AddMagerySpell(28, 4, 11, typeof(GreaterHealScroll), Reagent.Garlic, Reagent.SpidersSilk, Reagent.MandrakeRoot, Reagent.Ginseng);
            AddMagerySpell(29, 4, 11, typeof(LightningScroll), Reagent.MandrakeRoot, Reagent.SulfurousAsh);
            AddMagerySpell(30, 4, 11, typeof(ManaDrainScroll), Reagent.BlackPearl, Reagent.SpidersSilk, Reagent.MandrakeRoot);
            AddMagerySpell(31, 4, 11, typeof(RecallScroll), Reagent.BlackPearl, Reagent.Bloodmoss, Reagent.MandrakeRoot);

            AddMagerySpell(32, 5, 14, typeof(BladeSpiritsScroll), Reagent.BlackPearl, Reagent.Nightshade, Reagent.MandrakeRoot);
            AddMagerySpell(33, 5, 14, typeof(DispelFieldScroll), Reagent.BlackPearl, Reagent.Garlic, Reagent.SpidersSilk, Reagent.SulfurousAsh);
            AddMagerySpell(34, 5, 14, typeof(IncognitoScroll), Reagent.Bloodmoss, Reagent.Garlic, Reagent.Nightshade);
            AddMagerySpell(35, 5, 14, typeof(MagicReflectScroll), Reagent.Garlic, Reagent.MandrakeRoot, Reagent.SpidersSilk);
            AddMagerySpell(36, 5, 14, typeof(MindBlastScroll), Reagent.BlackPearl, Reagent.MandrakeRoot, Reagent.Nightshade, Reagent.SulfurousAsh);
            AddMagerySpell(37, 5, 14, typeof(ParalyzeScroll), Reagent.Garlic, Reagent.MandrakeRoot, Reagent.SpidersSilk);
            AddMagerySpell(38, 5, 14, typeof(PoisonFieldScroll), Reagent.BlackPearl, Reagent.Nightshade, Reagent.SpidersSilk);
            AddMagerySpell(39, 5, 14, typeof(SummonCreatureScroll), Reagent.Bloodmoss, Reagent.MandrakeRoot, Reagent.SpidersSilk);

            AddMagerySpell(40, 6, 20, typeof(DispelScroll), Reagent.Garlic, Reagent.MandrakeRoot, Reagent.SulfurousAsh);
            AddMagerySpell(41, 6, 20, typeof(EnergyBoltScroll), Reagent.BlackPearl, Reagent.Nightshade);
            AddMagerySpell(42, 6, 20, typeof(ExplosionScroll), Reagent.Bloodmoss, Reagent.MandrakeRoot);
            AddMagerySpell(43, 6, 20, typeof(InvisibilityScroll), Reagent.Bloodmoss, Reagent.Nightshade);
            AddMagerySpell(44, 6, 20, typeof(MarkScroll), new int[] { 1, 1, 1 }, Reagent.Bloodmoss, Reagent.BlackPearl, Reagent.MandrakeRoot);
            AddMagerySpell(45, 6, 20, typeof(MassCurseScroll), Reagent.Garlic, Reagent.MandrakeRoot, Reagent.Nightshade, Reagent.SulfurousAsh);
            AddMagerySpell(46, 6, 20, typeof(ParalyzeFieldScroll), Reagent.BlackPearl, Reagent.Ginseng, Reagent.SpidersSilk);
            AddMagerySpell(47, 6, 20, typeof(RevealScroll), Reagent.Bloodmoss, Reagent.SulfurousAsh);

            AddMagerySpell(48, 7, 40, typeof(ChainLightningScroll), new int[] { 3, 3 }, Reagent.Bloodmoss, Reagent.SulfurousAsh);
            AddMagerySpell(49, 7, 40, typeof(EnergyFieldScroll), new int[] { 3, 3 }, Reagent.SpidersSilk, Reagent.BlackPearl);
            AddMagerySpell(50, 7, 40, typeof(FlamestrikeScroll), new int[] { 3, 3 }, Reagent.SpidersSilk, Reagent.SulfurousAsh);
            AddMagerySpell(51, 7, 40, typeof(GateTravelScroll), new int[] { 3, 3 }, Reagent.BlackPearl, Reagent.SulfurousAsh);
            AddMagerySpell(52, 7, 40, typeof(ManaVampireScroll), new int[] { 3, 3 }, Reagent.BlackPearl, Reagent.MandrakeRoot);
            AddMagerySpell(53, 7, 40, typeof(MassDispelScroll), new int[] { 3, 3 }, Reagent.BlackPearl, Reagent.Garlic);
            AddMagerySpell(54, 7, 40, typeof(MeteorSwarmScroll), new int[] { 3, 3 }, Reagent.MandrakeRoot, Reagent.SulfurousAsh);
            AddMagerySpell(55, 7, 40, typeof(PolymorphScroll), new int[] { 3, 3 }, Reagent.Bloodmoss, Reagent.SpidersSilk);

            AddMagerySpell(56, 8, 50, typeof(EarthquakeScroll), new int[] { 5, 5 }, Reagent.Bloodmoss, Reagent.MandrakeRoot);
            AddMagerySpell(57, 8, 50, typeof(EnergyVortexScroll), new int[] { 5, 5 }, Reagent.BlackPearl, Reagent.Nightshade);
            AddMagerySpell(58, 8, 50, typeof(ResurrectionScroll), new int[] { 5, 5 }, Reagent.Garlic, Reagent.Ginseng);
            AddMagerySpell(59, 8, 50, typeof(SummonAirElementalScroll), new int[] { 5, 5 }, Reagent.Bloodmoss, Reagent.SpidersSilk);
            AddMagerySpell(60, 8, 50, typeof(SummonDaemonScroll), new int[] { 5, 5 }, Reagent.SpidersSilk, Reagent.SulfurousAsh);
            AddMagerySpell(61, 8, 50, typeof(SummonEarthElementalScroll), new int[] { 5, 5 }, Reagent.Bloodmoss, Reagent.MandrakeRoot);
            AddMagerySpell(62, 8, 50, typeof(SummonFireElementalScroll), new int[] { 5, 5 }, Reagent.Bloodmoss, Reagent.SulfurousAsh);
            AddMagerySpell(63, 8, 50, typeof(SummonWaterElementalScroll), new int[] { 5, 5 }, Reagent.Bloodmoss, Reagent.MandrakeRoot);
            // zombie

            // 15.08.2012 :: nowe regi dla scrolli necro
            if (Core.SE)
            {
                AddNecroSpell(0, 23, 39.6, typeof(AnimateDeadScroll), new int[] { 5, 5 }, Reagent.GraveDust, Reagent.DaemonBlood);
                AddNecroSpell(1, 13, 19.6, typeof(BloodOathScroll), new int[] { 5 }, Reagent.DaemonBlood);
                AddNecroSpell(2, 11, 19.6, typeof(CorpseSkinScroll), new int[] { 5, 5 }, Reagent.BatWing, Reagent.GraveDust);
                AddNecroSpell(3, 7, 19.6, typeof(CurseWeaponScroll), new int[] { 5 }, Reagent.PigIron);
                AddNecroSpell(4, 11, 19.6, typeof(EvilOmenScroll), new int[] { 5, 5 }, Reagent.BatWing, Reagent.NoxCrystal);
                AddNecroSpell(5, 11, 39.6, typeof(HorrificBeastScroll), new int[] { 5, 5 }, Reagent.BatWing, Reagent.DaemonBlood);
                AddNecroSpell(6, 23, 69.6, typeof(LichFormScroll), new int[] { 5, 5 }, Reagent.GraveDust, Reagent.DaemonBlood);
                AddNecroSpell(7, 17, 29.6, typeof(MindRotScroll), new int[] { 5, 5 }, Reagent.BatWing, Reagent.DaemonBlood);
                AddNecroSpell(8, 5, 19.6, typeof(PainSpikeScroll), new int[] { 1, 5 }, Reagent.GraveDust, Reagent.PigIron);
                AddNecroSpell(9, 17, 49.6, typeof(PoisonStrikeScroll), new int[] { 5 }, Reagent.NoxCrystal);
                AddNecroSpell(10, 29, 64.6, typeof(StrangleScroll), new int[] { 5, 5 }, Reagent.DaemonBlood, Reagent.NoxCrystal);
                AddNecroSpell(11, 17, 29.6, typeof(SummonFamiliarScroll), new int[] { 5, 5 }, Reagent.BatWing, Reagent.GraveDust);
                AddNecroSpell(12, 23, 98.6, typeof(VampiricEmbraceScroll), new int[] { 5, 5 }, Reagent.BatWing, Reagent.NoxCrystal);
                AddNecroSpell(13, 41, 79.6, typeof(VengefulSpiritScroll), new int[] { 5, 5 }, Reagent.BatWing, Reagent.PigIron);
                AddNecroSpell(14, 23, 59.6, typeof(WitherScroll), new int[] { 5, 5 }, Reagent.GraveDust, Reagent.PigIron);
                AddNecroSpell(15, 17, 79.6, typeof(WraithFormScroll), new int[] { 5, 5 }, Reagent.NoxCrystal, Reagent.PigIron);
                AddNecroSpell(16, 40, 79.6, typeof(ExorcismScroll), new int[] { 5, 5, 5, 5 }, Reagent.NoxCrystal, Reagent.GraveDust, Reagent.SulfurousAsh, Reagent.BlackPearl);
            }
            // zombie

            // 04.07.2012 :: zombie :: dodanie scrolli chivalry, bushido, ninjitsu

            // Chivalry
            AddChivalrySpell(0, 10, 0, typeof(CleanseByFireScroll), new int[] { 10, 5, 5 }, Reagent.SulfurousAsh, typeof(Amber), typeof(Ruby));
            AddChivalrySpell(1, 10, 5, typeof(CloseWoundsScroll), new int[] { 10, 5, 5 }, Reagent.MandrakeRoot, typeof(Amethyst), typeof(Citrine));
            AddChivalrySpell(2, 10, 15, typeof(ConsecrateWeaponScroll), new int[] { 20, 5, 10 }, Reagent.PigIron, typeof(Sapphire), typeof(Citrine));
            AddChivalrySpell(3, 15, 35, typeof(DispelEvilScroll), new int[] { 20, 10, 10 }, Reagent.DaemonBlood, typeof(Ruby), typeof(Tourmaline));
            AddChivalrySpell(4, 15, 25, typeof(DivineFuryScroll), new int[] { 20, 10, 10 }, Reagent.NoxCrystal, typeof(Emerald), typeof(Amethyst));
            AddChivalrySpell(5, 20, 45, typeof(EnemyOfOneScroll), new int[] { 20, 10, 15 }, Reagent.BatWing, typeof(Diamond), typeof(Emerald));
            AddChivalrySpell(6, 15, 55, typeof(HolyLightScroll), new int[] { 20, 30, 10, 15 }, Reagent.NoxCrystal, Reagent.BlackPearl, typeof(Diamond), typeof(Sapphire));
            AddChivalrySpell(7, 20, 65, typeof(NobleSacrificeScroll), new int[] { 20, 30, 10, 20 }, Reagent.GraveDust, Reagent.MandrakeRoot, typeof(StarSapphire), typeof(Diamond));
            AddChivalrySpell(8, 20, 5, typeof(RemoveCurseScroll), new int[] { 20, 5, 5 }, Reagent.Garlic, typeof(Tourmaline), typeof(Emerald));
            AddChivalrySpell(9, 20, 15, typeof(SacredJourneyScroll), new int[] { 20, 5, 10 }, Reagent.GraveDust, typeof(StarSapphire), typeof(Ruby));

            // Bushido
            AddBushidoSpell(0, 10, 25, typeof(HonorableExecutionScroll), new int[] { 10, 5, 5 }, Reagent.Ginseng, typeof(Amber), typeof(Citrine));
            AddBushidoSpell(1, 10, 25, typeof(ConfidenceScroll), new int[] { 15, 5, 5 }, Reagent.BlackPearl, typeof(Ruby), typeof(Amber));
            AddBushidoSpell(2, 10, 60, typeof(EvasionScroll), new int[] { 15, 10, 10 }, Reagent.GraveDust, typeof(Sapphire), typeof(Emerald));
            AddBushidoSpell(3, 5, 40, typeof(CounterAttackScroll), new int[] { 15, 5, 10 }, Reagent.NoxCrystal, typeof(Amethyst), typeof(Ruby));
            AddBushidoSpell(4, 5, 50, typeof(LightningStrikeScroll), new int[] { 15, 5, 10 }, Reagent.NoxCrystal, typeof(Amethyst), typeof(Ruby));
            AddBushidoSpell(5, 10, 70, typeof(MomentumStrikeScroll), new int[] { 20, 10, 10 }, Reagent.DaemonBlood, typeof(Diamond), typeof(StarSapphire));

            // Ninjitsu
            AddNinjitsuSpell(0, 20, 30, typeof(FocusAttackScroll), new int[] { 20, 20, 15}, Reagent.PigIron, Reagent.NoxCrystal, typeof(Amethyst));
            AddNinjitsuSpell(1, 30, 85, typeof(DeathStrikeScroll), new int[] { 20, 30, 20 }, Reagent.DaemonBlood, Reagent.Nightshade, typeof(Diamond));
            AddNinjitsuSpell(2, 10, 20, typeof(AnimalFormScroll), new int[] { 10, 10, 5 }, Reagent.Nightshade, typeof(DaemonBlood), typeof(Citrine));
            AddNinjitsuSpell(3, 25, 80, typeof(KiAttackScroll), new int[] { 20, 20, 20 }, Reagent.GraveDust, Reagent.BatWing, typeof(StarSapphire));
            AddNinjitsuSpell(4, 20, 60, typeof(SurpriseAttackScroll), new int[] { 20, 10, 10 }, Reagent.SpidersSilk, Reagent.PigIron, typeof(Ruby));
            AddNinjitsuSpell(5, 30, 20, typeof(BackstabScroll), new int[] { 10, 20, 5 }, Reagent.BatWing, Reagent.Bloodmoss, typeof(Amber));
            AddNinjitsuSpell(6, 15, 50, typeof(ShadowJumpScroll), new int[] { 20, 20, 15 }, Reagent.Ginseng, typeof(Amber), typeof(Citrine));
            AddNinjitsuSpell(7, 10, 40, typeof(MirrorImageScroll), new int[] { 20, 10, 10 }, Reagent.BlackPearl, Reagent.NoxCrystal, typeof(Tourmaline));
            // zombie

            // Runebook
            int index = AddCraft(typeof(Runebook), 1044294, 1041267, 45.0, 95.0, typeof(BlankScroll), 1044377, 18, 1044378);
            AddRes(index, typeof(RecallScroll), 1044445, 1, 1044253);
            AddRes(index, typeof(GateTravelScroll), 1044446, 1, 1044253);
			
			// Pet Bonding Deed
			index = AddCraft( typeof( PetBondingDeed ), 1044294, "Zwoj Oswajacza", 75.0, 100.0, typeof( Gold ), "zloto" , 8000, 1044253 );
			AddRes( index, typeof( BlankScroll ), "czyste zwoje", 200, 1044253 );
			AddRes( index, typeof( FireRuby ), "ogniowy rubin" , 5, 1044253 );
			AddRes( index, typeof( Beeswax ), "wosk" , 35, 1044253 );

           //Custom Spelle

           //Okultyzm
            index = AddCraft( typeof( UndeadAngelicFaithScroll ), "Umiejetnosci specjalne", "Demoniczny Awatar", 80.0, 110.0, typeof( Taint ), "skaza" , 20, 1044253 );
            AddRes( index, typeof( GrizzledBones ), "blade kości" , 10, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            AddRes( index, typeof( CapturedEssence ), "złapana esencja" , 2, 1044253 );
            index = AddCraft( typeof( UndeadVolcanicEruptionScroll ), "Umiejetnosci specjalne", "Erupcja Wulkaniczna", 80.0, 110.0, typeof( Taint ), "skaza" , 20, 1044253 );
            AddRes( index, typeof( GrizzledBones ), "blade kości" , 10, 1044253 );
            AddRes( index, typeof( Pumice ), "pumeks" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( UndeadSwarmOfInsectsScroll ), "Umiejetnosci specjalne", "Chmara Insektów", 80.0, 110.0, typeof( Taint ), "skaza" , 20, 1044253 );
            AddRes( index, typeof( GrizzledBones ), "blade kości" , 10, 1044253 );
            AddRes( index, typeof( ObsidianStone ), "obysdian" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( UndeadSeanceScroll ), "Umiejetnosci specjalne", "Seans", 80.0, 110.0, typeof( Taint ), "skaza" , 20, 1044253 );
            AddRes( index, typeof( DiseasedBark ), "zgniła kora" , 10, 1044253 );
            AddRes( index, typeof( FireRuby ), "ognisty rubin" , 10, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( UndeadNaturesPassageScroll ), "Umiejetnosci specjalne", "Ścieżka Śmierci", 80.0, 110.0, typeof( Taint ), "skaza" , 20, 1044253 );
            AddRes( index, typeof( DiseasedBark ), "zgniła kora" , 10, 1044253 );
            AddRes( index, typeof( ZoogiFungus ), "grzyby zoogi" , 50, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( UndeadMushroomGatewayScroll ), "Umiejetnosci specjalne", "Limbo", 90.0, 120.0, typeof( Taint ), "skaza" , 20, 1044253 );
            AddRes( index, typeof( DiseasedBark ), "zgniła kora" , 10, 1044253 );
            AddRes( index, typeof( FireRuby ), "ognisty rubin" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( UndeadLureStoneScroll ), "Umiejetnosci specjalne", "Gnijące Zwłoki", 90.0, 120.0, typeof( Taint ), "skaza" , 20, 1044253 );
            AddRes( index, typeof( DiseasedBark ), "zgniła kora" , 10, 1044253 );
            AddRes( index, typeof( Pumice ), "pumeks" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( UndeadLeafWhirlwindScroll ), "Umiejetnosci specjalne", "Piętno", 80.0, 110.0, typeof( Taint ), "skaza" , 20, 1044253 );
            AddRes( index, typeof( GrizzledBones ), "blade kości" , 10, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            AddRes( index, typeof( CapturedEssence ), "złapana esencja" , 2, 1044253 );
            index = AddCraft( typeof( UndeadHollowReedScroll ), "Umiejetnosci specjalne", "Hedonizm", 80.0, 110.0, typeof( Taint ), "skaza" , 20, 1044253 );
            AddRes( index, typeof( DiseasedBark ), "zgniła kora" , 10, 1044253 );
            AddRes( index, typeof( ObsidianStone ), "obysdian" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( UndeadHammerOfFaithScroll ), "Umiejetnosci specjalne", "Sierp Wiary Smierci", 90.0, 120.0, typeof( Taint ), "skaza" , 20, 1044253 );
            AddRes( index, typeof( GrizzledBones ), "blade kości" , 10, 1044253 );
            AddRes( index, typeof( Pumice ), "pumeks" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( UndeadGraspingRootsScroll ), "Umiejetnosci specjalne", "Uchwyt Zza Grobu", 80.0, 110.0, typeof( Taint ), "skaza" , 20, 1044253 );
            AddRes( index, typeof( GrizzledBones ), "blade kości" , 10, 1044253 );
            AddRes( index, typeof( ObsidianStone ), "obysdian" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( UndeadCauseFearScroll ), "Umiejetnosci specjalne", "Strach", 80.0, 110.0, typeof( Taint ), "skaza" , 20, 1044253 );
            AddRes( index, typeof( GrizzledBones ), "blade kości" , 10, 1044253 );
            AddRes( index, typeof( Pumice ), "pumeks" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );

            //Avatar
            index = AddCraft( typeof( AvatarSacredBoonScroll ), "Umiejetnosci specjalne", "Święty znak", 80.0, 110.0, typeof( Corruption ), "korupcja" , 20, 1044253 );
            AddRes( index, typeof( GrizzledBones ), "blade kości" , 10, 1044253 );
            AddRes( index, typeof( Pumice ), "pumeks" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( AvatarRestorationScroll ), "Umiejetnosci specjalne", "Odrodzenie", 80.0, 110.0, typeof( Corruption ), "korupcja" , 20, 1044253 );
            AddRes( index, typeof( DaemonBone ), "kości demona" , 50, 1044253 );
            AddRes( index, typeof( ObsidianStone ), "obysdian" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( AvatarMarkOfGodsScroll ), "Umiejetnosci specjalne", "Znak Bogów", 80.0, 110.0, typeof( WyrmsHeart ), "Serce Wyrma" , 20, 1044253 );
            AddRes( index, typeof( GrizzledBones ), "blade kości" , 10, 1044253 );
            AddRes( index, typeof( ZoogiFungus ), "grzyby zoogi" , 50, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( AvatarHeavensGateScroll ), "Umiejetnosci specjalne", "Niebiańska Brama", 80.0, 110.0, typeof( Taint ), "skaza" , 20, 1044253 );
            AddRes( index, typeof( GrizzledBones ), "blade kości" , 10, 1044253 );
            AddRes( index, typeof( GateTravelScroll ), "zwoje bramy" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( AvatarHeavenlyLightScroll ), "Umiejetnosci specjalne", "Niebiańskie Światło", 50.0, 70.0, typeof( WyrmsHeart ), "Serce Wyrma" , 2, 1044253 );
            AddRes( index, typeof( NightSightPotion ), "mikstura widzenia w ciemności" , 10, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( AvatarEnemyOfOneSpell ), "Umiejetnosci specjalne", "Naznaczony", 25.0, 76.0, typeof( Corruption ), "korupcja" , 20, 1044253 );
            AddRes( index, typeof( DaemonBone ), "kości demona" , 50, 1044253 );
            AddRes( index, typeof( Pumice ), "pumeks" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( AvatarArmysPaeonSpell  ), "Umiejetnosci specjalne", "Witalność Armii", 80.0, 110.0, typeof( WyrmsHeart ), "Serce Wyrma" , 20, 1044253 );
            AddRes( index, typeof( GrizzledBones ), "blade kości" , 10, 1044253 );
            AddRes( index, typeof( ZoogiFungus ), "grzyby zoogi" , 50, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( AvatarAngelicFaithSpell ), "Umiejetnosci specjalne", "Awatar Pradawnego Mnicha", 60.0, 100.0, typeof( LuminescentFungi ), "lśniące grzyby"  , 1044253 );
            AddRes( index, typeof( SpringWater ), "wiosenna woda" , 30, 1044253 );
            AddRes( index, typeof( ZoogiFungus ), "grzyby zoogi" , 50, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );

            //Druid
            index = AddCraft( typeof( DruidBlendWithForestScroll  ), "Umiejetnosci specjalne", "Jedność Z Lasem", 80.0, 110.0, typeof( LuminescentFungi ), "lśniące grzyby" , 20, 1044253 );
            AddRes( index, typeof( SpringWater ), "wiosenna woda" , 30, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            AddRes( index, typeof( CapturedEssence ), "złapana esencja" , 2, 1044253 );
            index = AddCraft( typeof( DruidFamiliarScroll ), "Umiejetnosci specjalne", "Przywołanie Przyjaciela Lasu", 80.0, 110.0, typeof( LuminescentFungi ), "lśniące grzyby"  , 20, 1044253 );
            AddRes( index, typeof( SpringWater ), "wiosenna woda" , 30, 1044253 );
            AddRes( index, typeof( DragonsBlood ), "krew smoka" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( DruidEnchantedGroveScroll), "Umiejetnosci specjalne", "Zaklęty Gaj", 90.0, 120.0, typeof( LuminescentFungi ), "lśniące grzyby"  , 1044253 );
            AddRes( index, typeof( SpringWater ), "wiosenna woda" , 30, 1044253 );
            AddRes( index, typeof( ObsidianStone ), "obysdian" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( DruidGraspingRootsScroll), "Umiejetnosci specjalne", "Szalone Korzenie", 80.0, 110.0, typeof( LuminescentFungi ), "lśniące grzyby"  , 20, 1044253 );
            AddRes( index, typeof( DiseasedBark ), "zgniła kora" , 10, 1044253 );
            AddRes( index, typeof( SpringWater ), "wiosenna woda" , 30, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( DruidHollowReedScroll), "Umiejetnosci specjalne", "Siła Natury", 80.0, 110.0, typeof( LuminescentFungi ), "lśniące grzyby"  , 20, 1044253 );
            AddRes( index, typeof( DiseasedBark ), "zgniła kora" , 10, 1044253 );
            AddRes( index, typeof( ZoogiFungus ), "grzyby zoogi" , 50, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( DruidLeafWhirlwindScroll ), "Umiejetnosci specjalne", "Wir Liści", 90.0, 120.0, typeof( LuminescentFungi ), "lśniące grzyby", 20 , 1044253 );
            AddRes( index, typeof( DiseasedBark ), "zgniła kora" , 10, 1044253 );
            AddRes( index, typeof( DarkSapphire ), "ciemny szafir" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( DruidLureStoneScroll ), "Umiejetnosci specjalne", "Ciekawy kamie", 90.0, 120.0, typeof( LuminescentFungi ), "lśniące grzyby"  , 20, 1044253 );
            AddRes( index, typeof( DiseasedBark ), "zgniła kora" , 10, 1044253 );
            AddRes( index, typeof( SpringWater ), "wiosenna woda" , 30, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( DruidMushroomGatewayScroll ), "Umiejetnosci specjalne", "Przejście Natury", 80.0, 110.0, typeof( LuminescentFungi ), "lśniące grzyby"  , 20, 1044253 );
            AddRes( index, typeof( SpringWater ), "wiosenna woda" , 30, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            AddRes( index, typeof( CapturedEssence ), "złapana esencja" , 2, 1044253 );
            index = AddCraft( typeof( DruidNaturesPassageScroll), "Umiejetnosci specjalne", "Naznaczenie", 80.0, 110.0, typeof( LuminescentFungi ), "lśniące grzyby"  , 20, 1044253 );
            AddRes( index, typeof( DiseasedBark ), "zgniła kora" , 10, 1044253 );
            AddRes( index, typeof( SpringWater ), "wiosenna woda" , 30, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( DruidPackOfBeastScroll), "Umiejetnosci specjalne", "Leśne Bestyje", 60.0, 90.0, typeof( LuminescentFungi ), "lśniące grzyby"  , 20, 1044253 );
            AddRes( index, typeof( SpringWater ), "wiosenna woda" , 30, 1044253 );
            AddRes( index, typeof( DragonsBlood ), "krew smoka" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( DruidRestorativeSoilScroll ), "Umiejetnosci specjalne", "Lecznicza Ziemia", 90.0, 120.0, typeof( LuminescentFungi ), "lśniące grzyby"  , 20, 1044253 );
            AddRes( index, typeof( SpringWater ), "wiosenna woda" , 30, 1044253 );
            AddRes( index, typeof( ObsidianStone ), "obysdian" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( DruidShieldOfEarthScroll ), "Umiejetnosci specjalne", "Tarcza Ziemi", 80.0, 110.0, typeof( LuminescentFungi ), "lśniące grzyby"  , 20, 1044253 );
            AddRes( index, typeof( SpringWater ), "wiosenna woda" , 30,1044253 );
            AddRes( index, typeof( DragonsBlood ), "krew smoka" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( DruidSpringOfLifeScroll ), "Umiejetnosci specjalne", "Źródło życia", 80.0, 110.0, typeof( LuminescentFungi ), "lśniące grzyby"  , 20, 1044253 );
            AddRes( index, typeof( SpringWater ), "wiosenna woda" , 30, 1044253 );
            AddRes( index, typeof( DragonsBlood ), "krew smoka" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( DruidStoneCircleScroll ), "Umiejetnosci specjalne", "Kamienny Krąg", 90.0, 120.0, typeof( LuminescentFungi ), "lśniące grzyby"  , 20, 1044253 );
            AddRes( index, typeof( SpringWater ), "wiosenna woda" , 30, 1044253 );
            AddRes( index, typeof( DragonsBlood ), "krew smoka" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( DruidSwarmOfInsectsScroll ), "Umiejetnosci specjalne", "Chmara Insektów", 80.0, 110.0, typeof( LuminescentFungi ), "lśniące grzyby"  , 20, 1044253 );
            AddRes( index, typeof( SpringWater ), "wiosenna woda" , 30, 1044253 );
            AddRes( index, typeof( ObsidianStone ), "obysdian" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( DruidVolcanicEruptionScroll ), "Umiejetnosci specjalne", "Erupcja Wulkaniczna", 90.0, 120.0, typeof( LuminescentFungi ), "lśniące grzyby"  ,20, 1044253 );
            AddRes( index, typeof( SpringWater ), "wiosenna woda" , 30, 1044253 );
            AddRes( index, typeof( DragonsBlood ), "krew smoka" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );

            //Ranger
            index = AddCraft( typeof( RangerSummonMountScroll ), "Umiejetnosci specjalne", "Przyzwanie Wierzcha", 80.0, 110.0, typeof( DryIce ), "suchy lód" , 20, 1044253 );
            AddRes( index, typeof( DreadHornMane ), "Grzywa Spaczonego" , 10, 1044253 );
            AddRes( index, typeof( Pumice ), "pumeks" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( RangerPhoenixFlightScroll ), "Umiejetnosci specjalne", "Lot Feniksa", 80.0, 110.0, typeof( DryIce ), "suchy lód" , 20, 1044253 );
            AddRes( index, typeof( TeleportScroll ), "zwoje teleporatcji" , 20, 1044253 );
            AddRes( index, typeof( ZoogiFungus ), "grzyby zoogi" , 50, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( RangerFamiliarScroll ), "Umiejetnosci specjalne", "Zwierzęcy kompan", 80.0, 110.0, typeof( DryIce ), "suchy lód" , 20, 1044253 );
            AddRes( index, typeof( DreadHornMane ), "Grzywa Spaczonego" , 10, 1044253 );
            AddRes( index, typeof( Pumice ), "pumeks" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( RangerNoxBowScroll ), "Umiejetnosci specjalne", "Wężowy Łuk", 80.0, 110.0, typeof( DryIce ), "suchy lód" , 20, 1044253 );
            AddRes( index, typeof( DreadHornMane ), "Grzywa Spaczonego" , 10, 1044253 );
            AddRes( index, typeof( CapturedEssence ), "złapana esencja" , 2, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( RangerLightningBowScroll ), "Umiejetnosci specjalne", "Piorunujacy Luk", 80.0, 110.0, typeof( DryIce ), "suchy lód" , 20, 1044253 );
            AddRes( index, typeof( DreadHornMane ), "Grzywa Spaczonego" , 10, 1044253 );
            AddRes( index, typeof( CapturedEssence ), "złapana esencja" , 2, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( RangerIceBowScroll ), "Umiejetnosci specjalne", "Lodowy Łuk", 80.0, 110.0, typeof( DryIce ), "suchy lód" , 20, 1044253 );
            AddRes( index, typeof( DreadHornMane ), "Grzywa Spaczonego" , 10, 1044253 );
            AddRes( index, typeof( CapturedEssence ), "złapana esencja" , 2, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( RangerFireBowScroll ), "Umiejetnosci specjalne", "Ognisty Łuk", 80.0, 110.0, typeof( DryIce ), "suchy lód" , 20, 1044253 );
            AddRes( index, typeof( DreadHornMane ), "Grzywa Spaczonego" , 10, 1044253 );
            AddRes( index, typeof( CapturedEssence ), "złapana esencja" , 2, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( RangerHuntersAimScroll ), "Umiejetnosci specjalne", "Celność łowcy", 80.0, 110.0, typeof( DryIce ), "suchy lód" , 20, 1044253 );
            AddRes( index, typeof( DreadHornMane ), "Grzywa Spaczonego" , 10, 1044253 );
            AddRes( index, typeof( ParasiticPlant ), "paraliżujący bluszcz" , 10, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );

            //Ancient
            index = AddCraft( typeof( AncientCloneScroll ), "Umiejetnosci specjalne", "Klonowanie", 80.0, 110.0, typeof( Taint ), "skaza" , 20, 1044253 );
            AddRes( index, typeof( GrizzledBones ), "blade kości" , 10, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            AddRes( index, typeof( CapturedEssence ), "złapana esencja" , 2, 1044253 );
            index = AddCraft( typeof( AncientDanceScroll ), "Umiejetnosci specjalne", "Taniec", 80.0, 110.0, typeof( Taint ), "skaza" , 20, 1044253 );
            AddRes( index, typeof( GrizzledBones ), "blade kości" , 10, 1044253 );
            AddRes( index, typeof( Pumice ), "pumeks" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( AncientDeathVortexScroll), "Umiejetnosci specjalne", "Wir Śmierci", 80.0, 110.0, typeof( Taint ), "skaza" , 20, 1044253 );
            AddRes( index, typeof( GrizzledBones ), "blade kości" , 10, 1044253 );
            AddRes( index, typeof( ObsidianStone ), "obysdian" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( AncientSeanceScroll ), "Umiejetnosci specjalne", "Seans", 80.0, 110.0, typeof( Taint ), "skaza" , 20, 1044253 );
            AddRes( index, typeof( DiseasedBark ), "zgniła kora" , 10, 1044253 );
            AddRes( index, typeof( FireRuby ), "ognisty rubin" , 10, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( AncientDouseSpell), "Umiejetnosci specjalne", "Wygaszenie", 80.0, 110.0, typeof( Taint ), "skaza" , 20, 1044253 );
            AddRes( index, typeof( DiseasedBark ), "zgniła kora" , 10, 1044253 );
            AddRes( index, typeof( ZoogiFungus ), "grzyby zoogi" , 50, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( AncientFireRingScroll), "Umiejetnosci specjalne", "Pierścień Ognia", 90.0, 120.0, typeof( Taint ), "skaza" , 20, 1044253 );
            AddRes( index, typeof( DiseasedBark ), "zgniła kora" , 10, 1044253 );
            AddRes( index, typeof( FireRuby ), "ognisty rubin" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( AncientEnchantScroll ), "Umiejetnosci specjalne", "Magiczne Nasycenie", 90.0, 120.0, typeof( Taint ), "skaza" , 20, 1044253 );
            AddRes( index, typeof( DiseasedBark ), "zgniła kora" , 10, 1044253 );
            AddRes( index, typeof( Pumice ), "pumeks" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( AncientGreatDouseScroll ), "Umiejetnosci specjalne", "Większe Wygaszenie", 80.0, 110.0, typeof( Taint ), "skaza" , 20, 1044253 );
            AddRes( index, typeof( GrizzledBones ), "blade kości" , 10, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            AddRes( index, typeof( CapturedEssence ), "złapana esencja" , 2, 1044253 );
            index = AddCraft( typeof( AncientGreatIgniteScroll ), "Umiejetnosci specjalne", "Większe Podpalenie", 80.0, 110.0, typeof( Taint ), "skaza" , 20, 1044253 );
            AddRes( index, typeof( DiseasedBark ), "zgniła kora" , 10, 1044253 );
            AddRes( index, typeof( ObsidianStone ), "obysdian" , 20, 1044253 );
            AddRes( index, typeof( FireRuby ), "ognisty rubin" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( AncientIgniteScroll ), "Umiejetnosci specjalne", "Podpalenie", 90.0, 120.0, typeof( Taint ), "skaza" , 20, 1044253 );
            AddRes( index, typeof( GrizzledBones ), "blade kości" , 10, 1044253 );
            AddRes( index, typeof( Pumice ), "pumeks" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( AncientMassMightSpell ), "Umiejetnosci specjalne", "Masowa Potęga", 80.0, 110.0, typeof( Taint ), "skaza" , 20, 1044253 );
            AddRes( index, typeof( GrizzledBones ), "blade kości" , 10, 1044253 );
            AddRes( index, typeof( ObsidianStone ), "obysdian" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( AncientCauseFearScroll ), "Umiejetnosci specjalne", "Strach", 80.0, 110.0, typeof( Taint ), "skaza" , 20, 1044253 );
            AddRes( index, typeof( GrizzledBones ), "blade kości" , 10, 1044253 );
            AddRes( index, typeof( Pumice ), "pumeks" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( AncientPeerScroll ), "Umiejetnosci specjalne", "Wizja", 80.0, 110.0, typeof( Taint ), "skaza" , 20, 1044253 );
            AddRes( index, typeof( GrizzledBones ), "blade kości" , 10, 1044253 );
            AddRes( index, typeof( Pumice ), "pumeks" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( AncientSwarmScroll ), "Umiejetnosci specjalne", "Rój", 90.0, 120.0, typeof( Taint ), "skaza" , 20, 1044253 );
            AddRes( index, typeof( GrizzledBones ), "blade kości" , 10, 1044253 );
            AddRes( index, typeof( Pumice ), "pumeks" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );

            //Bard
            index = AddCraft( typeof( BardArmysPaeonScroll ), "Umiejetnosci specjalne", "Śpiew Armii", 80.0, 110.0, typeof( Corruption ), "korupcja" , 20, 1044253 );
            AddRes( index, typeof( GrizzledBones ), "blade kości" , 10, 1044253 );
            AddRes( index, typeof( Pumice ), "pumeks" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( BardEnchantingEtudeScroll), "Umiejetnosci specjalne", "Wzmacniająca Etiuda", 80.0, 110.0, typeof( Corruption ), "korupcja" , 20, 1044253 );
            AddRes( index, typeof( DaemonBone ), "kości demona" , 50, 1044253 );
            AddRes( index, typeof( ObsidianStone ), "obysdian" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( BardEnergyCarolScroll ), "Umiejetnosci specjalne", "Pobudzająca Pieśń", 80.0, 110.0, typeof( WyrmsHeart ), "Serce Wyrma" , 20, 1044253 );
            AddRes( index, typeof( GrizzledBones ), "blade kości" , 10, 1044253 );
            AddRes( index, typeof( ZoogiFungus ), "grzyby zoogi" , 50, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( BardEnergyThrenodyScroll ), "Umiejetnosci specjalne", "Porażający Tren", 80.0, 110.0, typeof( Taint ), "skaza" , 20, 1044253 );
            AddRes( index, typeof( GrizzledBones ), "blade kości" , 10, 1044253 );
            AddRes( index, typeof( GateTravelScroll ), "zwoje bramy" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( BardFireThrenodyScroll ), "Umiejetnosci specjalne", "Palący Tren", 80.0, 110.0, typeof( Taint ), "skaza" , 20, 1044253 );
            AddRes( index, typeof( GrizzledBones ), "blade kości" , 10, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            AddRes( index, typeof( CapturedEssence ), "złapana esencja" , 2, 1044253 );
            index = AddCraft( typeof( BardFoeRequiemScroll ), "Umiejetnosci specjalne", "Soniczny Podmuch", 80.0, 110.0, typeof( Taint ), "skaza" , 20, 1044253 );
            AddRes( index, typeof( GrizzledBones ), "blade kości" , 10, 1044253 );
            AddRes( index, typeof( Pumice ), "pumeks" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( BardIceCarolScroll), "Umiejetnosci specjalne", "Pieśń Lodu", 80.0, 110.0, typeof( Taint ), "skaza" , 20, 1044253 );
            AddRes( index, typeof( GrizzledBones ), "blade kości" , 10, 1044253 );
            AddRes( index, typeof( ObsidianStone ), "obysdian" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( BardIceThrenodyScroll ), "Umiejetnosci specjalne", "Lodowy Tren", 80.0, 110.0, typeof( Taint ), "skaza" , 20, 1044253 );
            AddRes( index, typeof( DiseasedBark ), "zgniła kora" , 10, 1044253 );
            AddRes( index, typeof( FireRuby ), "ognisty rubin" , 10, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( BardKnightsMinneScroll ), "Umiejetnosci specjalne", "Wzmacniający Okrzyk", 80.0, 110.0, typeof( Taint ), "skaza" , 20, 1044253 );
            AddRes( index, typeof( GrizzledBones ), "blade kości" , 10, 1044253 );
            AddRes( index, typeof( FireRuby ), "ognisty rubin" , 10, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( BardMagesBalladScroll ), "Umiejetnosci specjalne", "Pieśń Do Magów", 80.0, 110.0, typeof( Corruption ), "korupcja" , 20, 1044253 );
            AddRes( index, typeof( GrizzledBones ), "blade kości" , 10, 1044253 );
            AddRes( index, typeof( Pumice ), "pumeks" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( BardMagicFinaleScroll ), "Umiejetnosci specjalne", "Magiczny Finał", 80.0, 110.0, typeof( Taint ), "skaza" , 20, 1044253 );
            AddRes( index, typeof( GrizzledBones ), "blade kości" , 10, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( BardPoisonCarolScroll), "Umiejetnosci specjalne", "Wężowa Pieśń", 80.0, 110.0, typeof( Taint ), "skaza" , 20, 1044253 );
            AddRes( index, typeof( GrizzledBones ), "blade kości" , 10, 1044253 );
            AddRes( index, typeof( ObsidianStone ), "obysdian" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( BardPoisonThrenodyScroll ), "Umiejetnosci specjalne", "Tren Jadu", 80.0, 110.0, typeof( Taint ), "skaza" , 20, 1044253 );
            AddRes( index, typeof( GrizzledBones ), "blade kości" , 10, 1044253 );
            AddRes( index, typeof( Pumice ), "pumeks" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( BardSheepfoeMamboScroll ), "Umiejetnosci specjalne", "Pasterska Przyśpiewka", 80.0, 110.0, typeof( Taint ), "skaza" , 20, 1044253 );
            AddRes( index, typeof( GrizzledBones ), "blade kości" , 10, 1044253 );
            AddRes( index, typeof( FireRuby ), "ognisty rubin" , 10, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( BardSinewyEtudeScroll ), "Umiejetnosci specjalne", "Przyśpiewka Górników", 80.0, 110.0, typeof( Taint ), "skaza" , 20, 1044253 );
            AddRes( index, typeof( GrizzledBones ), "blade kości" , 10, 1044253 );
            AddRes( index, typeof( FireRuby ), "ognisty rubin" , 10, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );

            //Cleric
            index = AddCraft( typeof( ClericAngelicFaithScroll ), "Umiejetnosci specjalne", "Anielska Wiara", 80.0, 110.0, typeof( Blight ), "zaraza" , 20, 1044253 );
            AddRes( index, typeof( GrizzledBones ), "blade kości" , 10, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            AddRes( index, typeof( CapturedEssence ), "złapana esencja" , 2, 1044253 );
            index = AddCraft( typeof( ClericBanishEvilScroll ), "Umiejetnosci specjalne", "Wygnanie zła", 80.0, 110.0, typeof( Blight ), "zaraza" , 20, 1044253 );
            AddRes( index, typeof( GrizzledBones ), "blade kości" , 10, 1044253 );
            AddRes( index, typeof( Pumice ), "pumeks" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( ClericDampenSpiritScroll), "Umiejetnosci specjalne", "Stłumienie Ducha", 80.0, 110.0, typeof( Blight ), "zaraza" , 20, 1044253 );
            AddRes( index, typeof( GrizzledBones ), "blade kości" , 10, 1044253 );
            AddRes( index, typeof( ObsidianStone ), "obysdian" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( ClericDivineFocusScroll ), "Umiejetnosci specjalne", "Boskie Skupienie", 80.0, 110.0, typeof( Blight ), "zaraza" , 20, 1044253 );
            AddRes( index, typeof( DiseasedBark ), "zgniła kora" , 10, 1044253 );
            AddRes( index, typeof( FireRuby ), "ognisty rubin" , 10, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( ClericHammerOfFaithScroll), "Umiejetnosci specjalne", "Topór Wiary", 80.0, 110.0, typeof( Blight ), "zaraza" , 20, 1044253 );
            AddRes( index, typeof( DiseasedBark ), "zgniła kora" , 10, 1044253 );
            AddRes( index, typeof( ZoogiFungus ), "grzyby zoogi" , 50, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( ClericPurgeScroll ), "Umiejetnosci specjalne", "Czystka", 90.0, 120.0, typeof( Blight ), "zaraza" , 20, 1044253 );
            AddRes( index, typeof( DiseasedBark ), "zgniła kora" , 10, 1044253 );
            AddRes( index, typeof( FireRuby ), "ognisty rubin" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( ClericRestorationScroll ), "Umiejetnosci specjalne", "Odrodzenie", 90.0, 120.0, typeof( Blight ), "zaraza" , 20, 1044253 );
            AddRes( index, typeof( DiseasedBark ), "zgniła kora" , 10, 1044253 );
            AddRes( index, typeof( Pumice ), "pumeks" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( ClericSacredBoonScroll ), "Umiejetnosci specjalne", "Święty znak", 80.0, 110.0, typeof( Blight ), "zaraza" , 20, 1044253 );
            AddRes( index, typeof( GrizzledBones ), "blade kości" , 10, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            AddRes( index, typeof( CapturedEssence ), "złapana esencja" , 2, 1044253 );
            index = AddCraft( typeof( ClericSacrificeScroll ), "Umiejetnosci specjalne", "Poświęcenie", 80.0, 110.0, typeof( Blight ), "zaraza" , 20, 1044253 );
            AddRes( index, typeof( DiseasedBark ), "zgniła kora" , 10, 1044253 );
            AddRes( index, typeof( FireRuby ), "ognisty rubin" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( ClericSmiteScroll ), "Umiejetnosci specjalne", "Smagnięcie", 90.0, 120.0, typeof( Blight ), "zaraza" , 20, 1044253 );
            AddRes( index, typeof( GrizzledBones ), "blade kości" , 10, 1044253 );
            AddRes( index, typeof( Pumice ), "pumeks" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( ClericTouchOfLifeScroll ), "Umiejetnosci specjalne", "Dotyk Życia", 80.0, 110.0, typeof( Blight ), "zaraza" , 20, 1044253 );
            AddRes( index, typeof( GrizzledBones ), "blade kości" , 10, 1044253 );
            AddRes( index, typeof( ObsidianStone ), "obysdian" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );
            index = AddCraft( typeof( ClericTrialByFireScroll ), "Umiejetnosci specjalne", "Próba Ognia", 80.0, 110.0, typeof( Blight ), "zaraza" , 20, 1044253 );
            AddRes( index, typeof( GrizzledBones ), "blade kości" , 10, 1044253 );
            AddRes( index, typeof( Pumice ), "pumeks" , 20, 1044253 );
            AddRes( index, typeof( Gold ), "złoto" , 2000, 1044253 );

            //Rogue
            index = AddCraft( typeof( RogueFalseCoinScroll ), "Umiejetnosci specjalne", "Falszywa moneta", 50.0, 70.0, typeof( Gold ), "zloto" , 2000, 1044253 );
            AddRes( index, typeof( Pumice ), "pumeks" , 20, 1044253 );
            index = AddCraft( typeof( RogueShieldOfEarthScroll ), 1044294, "Klody pod nogi", 70.0, 100.0, typeof( Gold ), "zloto" , 2000, 1044253 );
            AddRes( index, typeof( Pumice ), "pumeks" , 20, 1044253 );
            AddRes( index, typeof( GrizzledBones ), "blade kości" , 1, 1044253 );
            index = AddCraft( typeof( RogueCharmScroll), "Umiejetnosci specjalne", "Zaklecie", 70.0, 100.0, typeof( Gold ), "zloto" , 2000, 1044253 );
            AddRes( index, typeof( ObsidianStone ), "obysdian" , 20, 1044253 );
            AddRes( index, typeof( GrizzledBones ), "blade kości" , 1, 1044253 );
            index = AddCraft( typeof( RogueSlyFoxScroll ), "Umiejetnosci specjalne", "Przebiegła forma", 70.0, 100.0, typeof( Gold ), "zloto" , 2000, 1044253 );
            AddRes( index, typeof( CapturedEssence ), "złapana esencja" , 2, 1044253 );
            AddRes( index, typeof( DiseasedBark ), "zgniła kora" , 1, 1044253 );
            index = AddCraft( typeof( RogueShadowScroll ), "Umiejetnosci specjalne", "Cien", 70.0, 100.0, typeof( Gold ), "zloto" , 2000, 1044253 );
            AddRes( index, typeof( BladeSpiritsScroll ), "zwoj ducha ostrzy" , 5, 1044253 );
            AddRes( index, typeof( DiseasedBark ), "zgnila kora" , 1, 1044253 );
            index = AddCraft( typeof( RogueIntimidationScroll ), "Umiejetnosci specjalne", "Zastraszenie", 70.0, 100.0, typeof( Gold ), "zloto" , 2000, 1044253 );
            AddRes( index, typeof( CapturedEssence ), "złapana esencja" , 2, 1044253 );
            AddRes( index, typeof( PowderOfTranslocation ), "proszek translokacji" , 10, 1044253 );

            

            // Blank scrolls
            index = AddCraft(typeof(BlankScroll), 1044294, 1044377, 40.0, 80.0, typeof(Cloth), 1044286, 1, 1044253);
            AddRes(index, typeof(Log), 1015101, 1, 1044253);
            // Blank scrolls (using all available resources)
            index = AddCraft(typeof(BlankScroll), 1044294, 1064804, 40.0, 80.0, typeof(Cloth), 1044286, 1, 1044253);
            AddRes(index, typeof(Log), 1015101, 1, 1044253);
            SetUseAllRes(index, true);

            index = AddCraft(typeof(ScrollBinderDeed), 1044294, 3010147, 75.0, 125.0, typeof(WoodPulp), 1060630, 1, 1044253);
            SetItemHue(index, 1641);

            if (Core.AOS)
            {
                // Bulk order book
                AddCraft(typeof(Engines.BulkOrders.BulkOrderBook), 1044294, 1028793, 65.0, 115.0, typeof(BlankScroll), 1044377, 30, 1044378);
            }

            // Bulk order book
            AddCraft(typeof(Engines.BulkOrders.HuntingBulkOrderBook), 1044294, 1063956, 65.0, 115.0, typeof(BlankScroll), 1044377, 30, 1044378);

            // 07.01.2012 :: zombie :: zmiana maxSkill z 150 na 126, bo nie wychodzily expy (tak jak RunUO 2.2)
            if (Core.SE)
                AddCraft(typeof(Spellbook), 1044294, 1023834, 50.0, 126.0, typeof(BlankScroll), 1044377, 10, 1044378);

            // pozwala przerabiac kolorowe drewno na zwoje
            SetSubRes(typeof(Log), 1072643);

            // dodanie surowcow do gumpa
            AddSubRes(typeof(Log), 1072643, 00.0, 505821, 1072653);
            AddSubRes(typeof(OakLog), 1072644, 0.0, 505822, 1072653);
            AddSubRes(typeof(AshLog), 1072645, 0.0, 505823, 1072653);
            AddSubRes(typeof(YewLog), 1072646, 0.0, 505824, 1072653);
            AddSubRes(typeof(BloodwoodLog), 1072647, 0.0, 505825, 1072653);
            AddSubRes(typeof(HeartwoodLog), 1072648, 0.0, 505826, 1072653);
            AddSubRes(typeof(FrostwoodLog), 1072649, 0.0, 505827, 1072653);

            MarkOption = true;
            // 25.09.2012 :: zombie :: naprawa spellbookow
            Repair = true;
            // zombie
            RecycleHelper = new Rewrite();
        }
    }
}