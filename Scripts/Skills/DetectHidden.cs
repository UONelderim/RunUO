// 10.07.2012 :: zombie :: przebudowa - http://forum.nelderim.org/viewtopic.php?p=3879#3879

using System;
using System.Collections;
using System.Collections.Generic;
using Server.Multis;
using Server.Factions;
using Server.Targeting;
using Server.Items;
using Server.Regions;
using Server.Mobiles;
using Server.Gumps;

namespace Server.SkillHandlers
{
    public class DetectHidden
    {
        public static void Initialize()
        {
            SkillInfo.Table[(int)SkillName.DetectHidden].Callback = new SkillUseCallback(OnUse);
        }

        public static TimeSpan PassiveDelay = TimeSpan.FromSeconds(25.0);

        public static TimeSpan OnUse(Mobile src)
        {
            src.CloseGump(typeof(DetectHiddenMenu));
            src.SendGump(new DetectHiddenMenu());
            return TimeSpan.FromSeconds(0.0);
        }

        #region Orignalna detekcja
        public static TimeSpan OnUseOriginal(Mobile src)
        {
            src.SendLocalizedMessage(500819); //Gdzie chcesz wykrywac?
            src.Target = new InternalTarget();

            return TimeSpan.FromSeconds(10.0);
        }

        public static void UsePassiveSkill(Mobile src)
        {
            //src.SendMessage( "PassiveDetect: " + DateTime.Now );
            UseSkill(src, src.Location, true);
            if (src is PlayerMobile)
            {
                PlayerMobile pm = (PlayerMobile)src;
                pm.NextPassiveDetectHiddenCheck = DateTime.Now + PassiveDelay;
            }
        }

        public static void UseSkill(Mobile src, Point3D p)
        {
            UseSkill(src, p, false);
        }

        public static void UseSkill(Mobile src, Point3D p, bool passive)
        {
            if (DateTime.Now < src.NextSkillTime)
            {
                src.SendSkillMessage();
                return;
            }

            int range = 8;

            // wylaczenie pasywnego wykrywania dla GM
            if (passive && src.AccessLevel > AccessLevel.Player)
                return;

            BaseHouse house = BaseHouse.FindHouseAt(p, src.Map, 16);
            bool inHouse = (house != null && house.IsFriend(src));
            bool foundAnyone = false;

            if (passive)
                range = 5;
            else if (inHouse)
                range = 22;

            IPooledEnumerable mobilesInRange = src.Map.GetMobilesInRange(p, range);

            foreach (Mobile trg in mobilesInRange)
            {
                if (trg.Hidden && src != trg)
                {
                    double chance = GetMobileDetectChance(src, trg, passive);
                    //src.SendMessage( trg.Name + " detect chance: " + chance );

                    if (trg.AccessLevel == AccessLevel.Player && ((inHouse && house.IsInside(trg)) || chance > Utility.RandomDouble()))
                    {
                        if (trg is Mobiles.ShadowKnight && (trg.X != p.X || trg.Y != p.Y))
                            continue;

                        trg.RevealingAction();
                        DetectionEffect(trg);

                        trg.SendLocalizedMessage(500814); // Zostales odkryty!
                        foundAnyone = true;
                    }
                }
            }

            mobilesInRange.Free();

            IPooledEnumerable itemsInRange = src.Map.GetItemsInRange(p, range);

            foreach (Item item in itemsInRange)
            {
                if (item is BaseTrap)
                {
                    if (item.Visible)
                        continue;

                    BaseTrap trap = (BaseTrap)item;
                    double chance = GetTrapDetectChance(src, trap, passive);
                    //src.SendMessage( "{0} trap detect chance: {1} ", trap.Level, chance );

                    trap.Visible = true;
                    MessageHelper.SendLocalizedMessageTo(item, src, 500851, 0x59);
                    DetectionEffect(trap);

                    if (DetectTinkerTrap(src, p, range)) // Tinker Traps
                        foundAnyone = true;              // Tinker Traps
                    
                }
            }

            itemsInRange.Free();

            if (!foundAnyone)
            {
                src.SendLocalizedMessage(500817); // Nie wykryles tu nikogo.
            }
            else
            {
                src.SendMessage("Udalo ci sie cos wykryc!");
            }

            // przyrost umiejetnosci:
            if (!passive)
            {
                src.CheckSkill(SkillName.DetectHidden, -1, 101);
                src.NextSkillTime = DateTime.Now + TimeSpan.FromSeconds(8.0);
            }
        }
         
        public static void DetectionEffect(object trg)
        {
            int itemID = 0x375A, speed = 9, duration = 20, effect = 5049, sound = 0x1FD;

            if (trg is Mobile)
            {
                Mobile m = (Mobile)trg;
                m.FixedParticles(itemID, speed, duration, effect, EffectLayer.Head);
                m.PlaySound(0x1FD);
            }
            else if (trg is BaseTrap)
            {
                Item item = (Item)trg;
                Effects.SendLocationParticles(EffectItem.Create(item.Location, item.Map, EffectItem.DefaultDuration), itemID, speed, duration, effect);
                Effects.PlaySound(item.Location, item.Map, sound);
            }
        }

        public static double GetTrapDetectChance(Mobile src, BaseTrap trap, bool passive)
        {
            double srcDetect = src.Skills[SkillName.DetectHidden].Value;
            double trgHiding = 0, trgStealth = 0, reqDetect = 0;

            switch (trap.Level)
            {
                case BaseTrap.TrapLevel.Small:
                    trgHiding = 50; trgStealth = 50; reqDetect = 50;
                    break;
                case BaseTrap.TrapLevel.Medium:
                    trgHiding = 80; trgStealth = 80; reqDetect = 80;
                    break;
                case BaseTrap.TrapLevel.Big:
                    trgHiding = 100; trgStealth = 100; reqDetect = 100;
                    break;
            }

            if (srcDetect < reqDetect)
                return 0;

            return GetDetectChance(srcDetect, trgHiding, trgStealth, GetDistanceMod(src, trap.Location, passive));
        }
   // Tinker Traps (Start).
        // =====================
        public static bool DetectTinkerTrap(Mobile mobile, Point3D location, int range)
        {
            IPooledEnumerable itemsInRange = mobile.Map.GetItemsInRange(location, range);
            foreach (Item item in itemsInRange)
            {
                if (item is BaseTinkerTrap)
                {
                    BaseTinkerTrap trap = (BaseTinkerTrap)item;
                    double detectMin = trap.DisarmingSkillReq - 10;
                    double detectMax = trap.DisarmingSkillReq + 10;
                    if ((mobile.CheckTargetSkill(SkillName.DetectHidden, trap, detectMin, detectMax)) || (trap.Owner == mobile))
                    {
                        trap.Visible = true;
                        return true;
                    }
                    return false;
                }
                return false;
            }
            itemsInRange.Free();
            return false;
        }
        // ==================
        // Tinker Traps (End)
        public static double GetMobileDetectChance(Mobile src, Mobile trg, bool passive)
        {
            double distMod = GetDistanceMod(src, trg.Location, passive);

            return GetDetectChance(src.Skills[SkillName.DetectHidden].Value, trg.Skills[SkillName.Hiding].Value, trg.Skills[SkillName.Stealth].Value, distMod);
        }

        public static double GetDetectChance(double srcDetect, double trgHiding, double trgStealth, double distMod)
        {
            return (srcDetect * distMod) / ((trgHiding + trgStealth) / 2);
        }

        public static double GetDistanceMod(Mobile src, Point3D trgLoc, bool passive)
        {
            double distMod = 0.8;

            if (passive)
            {
                double distance = src.GetDistanceToSqrt(trgLoc);

                if (distance <= 1)
                    distMod = 0.8;
                else if (distance <= 2)
                    distMod = 0.6;
                else if (distance <= 3)
                    distMod = 0.4;
                else if (distance <= 4)
                    distMod = 0.2;
                else
                    distMod = 0.1;

                //src.SendMessage( "distance: {0}, mod: {1}", distance, distMod );
            }

            return distMod;
        }

        private class InternalTarget : Target
        {
            public InternalTarget()
                : base(12, true, TargetFlags.None)
            {
            }

            protected override void OnTarget(Mobile src, object targ)
            {
                Point3D p;

                if (targ is Mobile)
                    p = ((Mobile)targ).Location;
                else if (targ is Item)
                    p = ((Item)targ).Location;
                else if (targ is IPoint3D)
                    p = new Point3D((IPoint3D)targ);
                else
                    p = src.Location;

                UseSkill(src, p);
            }
        }
        #endregion

        #region Nowa detekcja
        public static void Detection(Mobile from)
        {
            Mobile src = from;

            if (DateTime.Now < src.NextSkillTime)
            {
                src.SendSkillMessage();
                return;
            }

            double srcSkill = src.Skills[SkillName.DetectHidden].Value;
            int range = (int)(srcSkill / 13.0);

            bool detectedanyone = false;

            if (range > 0)
            {
                ArrayList inRangeArray = new ArrayList();

                IPooledEnumerable eable = src.GetMobilesInRange(range);
                foreach (Mobile trg in eable)
                {
                    if (trg is PlayerMobile)
                    {
                        PlayerMobile test = trg as PlayerMobile;

                        if (trg.Hidden && src != trg)
                        {
                            double ss = srcSkill + Utility.Random(21) - 10;
                            double ts = trg.Skills[SkillName.Hiding].Value + Utility.Random(25) - 10;
                            if ((src.AccessLevel >= trg.AccessLevel) && (ss >= ts))
                            {
                                detectedanyone = true;
                                Detecting(src, trg);
                                inRangeArray.Add(trg);
                            }
                        }
                    }
                }
                eable.Free();

                if (detectedanyone)
                    src.SendMessage("Wyczules kogos w poblizu!");
                else
                    src.SendMessage("Nikogo nie wyczules w poblizu.");

                InternalTimer pause = new InternalTimer(src, inRangeArray);
                pause.Start();

                src.NextSkillTime = DateTime.Now + TimeSpan.FromSeconds(8.0);
            }
        }

        private static void Detecting(Mobile from, Mobile targ)
        {

            PlayerMobile pm = (PlayerMobile)targ;

            List<Mobile> list = pm.VisibilityList;

            list.Add(from);

            if (Utility.InUpdateRange(from, targ))
            {
                if (from.CanSee(targ))
                {
                    from.Send(new Network.MobileIncoming(from, targ));
                }
            }
        }

        private class InternalTimer : Timer
        {
            private Mobile m_From;
            private ArrayList m_InRange;

            public InternalTimer(Mobile from, ArrayList inRange2)
                : base(TimeSpan.FromSeconds(15.0))
            {
                m_From = from;
                m_InRange = inRange2;
            }

            protected override void OnTick()
            {
                foreach (Mobile target in m_InRange)
                {
                    Mobile TargetMobile = target;
                    PlayerMobile TargetPlayerMobile = target as PlayerMobile;

                    TargetPlayerMobile.VisibilityList.Remove(m_From);

                    if (TargetMobile.Hidden && m_From != TargetPlayerMobile)
                    {
                        if (!m_From.CanSee(TargetPlayerMobile) && Utility.InUpdateRange(m_From, TargetMobile))
                            m_From.Send(TargetPlayerMobile.RemovePacket);
                    }
                }
                m_InRange.Clear();
            }
        }
        #endregion
    }

    public class PassiveDetectHiddenTimer : Timer
    {
        private Mobile m_From;

        public PassiveDetectHiddenTimer(Mobile from)
            : base(TimeSpan.FromSeconds(0.25), DetectHidden.PassiveDelay)
        {
            m_From = from;
            Priority = TimerPriority.FiftyMS;
        }

        protected override void OnTick()
        {
            DetectHidden.UsePassiveSkill(m_From);
        }
    }
}
