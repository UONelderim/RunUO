using System;
using Server;
using System.Collections;
using Server.Network;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Engines;
using Server.Engines.Harvest;



namespace Server.Items.Crops
{


    // WeedSeed: Szczepka ziola - do sadzenia.
    public class WeedSeed : Item
    {
        public virtual string MsgCantBeMounted { get { return "Musisz stac na ziemi, aby moc to zrobic."; } }
        public virtual string MsgBadTerrain { get { return "Nie mozesz tego zrobic na tym terenie."; } }
        public virtual string MsgPlantAlreadyHere { get { return "W tym miejscu cos juz cos jest."; } }
        public virtual string MsgTooLowSkillToPlant { get { return "Nie masz wystarczajacej wiedzy, aby wykorzystac przetmiot."; } }
        public virtual string MsgPlantSuccess { get { return "Udalo ci sie zostawic przedmiot."; } }
        public virtual string MsgPlantFail { get { return "Nie udalo ci sie zostawic przedmiot, zmarnowales okazje."; } }

        // Typ terenu umozliwiajacy sadzenie:
        public virtual bool CanGrowFurrows { get { return true; } }
        public virtual bool CanGrowGrass { get { return false; } }
        public virtual bool CanGrowForest { get { return false; } }
        public virtual bool CanGrowJungle { get { return false; } }
        public virtual bool CanGrowCave { get { return false; } }
        public virtual bool CanGrowSand { get { return false; } }
        public virtual bool CanGrowSnow { get { return false; } }
        public virtual bool CanGrowSwamp { get { return false; } }

        protected static SkillName[] defaultSkillsRequired = new SkillName[] { WeedHelper.MainWeedSkill };
        public virtual SkillName[] SkillsRequired { get { return defaultSkillsRequired; } }

        public virtual double MinSkillReq { get { return 90.0; } }

        public WeedSeed(int itemID) : base(itemID)
        {
            Stackable = true;
            Weight = 0.2;
        }

        public WeedSeed(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }

        protected virtual bool CheckPlantChance(Mobile from)
        {
            from.CheckSkill(WeedHelper.MainWeedSkill, 80, 100); // zawsze mozliwy koksa zielarstwa (ale tylko ten skill)

            return WeedHelper.CheckSkills(from, SkillsRequired, 80, 100); // pozwol sadzic ziolo uzywajac innych umiejetnosci
        }

        public virtual Item CreateWeed() { return null; }

        public override void OnDoubleClick(Mobile from)
        {
            if (!from.CanBeginAction(LockKind()))
            {
                from.SendLocalizedMessage(1070062);
                return;
            }

            if (!CheckConditions(from))
                return;

            // Prog skilla umozliwiajacy sadzenie ziol:
            if (WeedHelper.GetHighestSkillValue(from, SkillsRequired) < MinSkillReq)
            {
                from.SendMessage(MsgTooLowSkillToPlant);    // Nie wiesz zbyt wiele o sadzeniu ziol.
                return;
            }

            //if ( this.BumpZ )
            //	++m_pnt.Z;

            // Sadzimy ziolo:
            from.BeginAction(LockKind());
            from.RevealingAction();
            double AnimationDelayBeforeStart = 0.5;
            double AnimationIntervalBetween = 1.75;
            int AnimationNumberOfRepeat = 2;
            // Wpierw delay i animacja wewnatrz timera, a po ostatniej animacji timer uruchamia funkcje wyrywajaca ziolo (trzeci parametr):
            new WeedTimer(from, this, this.Animate, this.PlantWeed, this.Unlock, TimeSpan.FromSeconds(AnimationDelayBeforeStart), TimeSpan.FromSeconds(AnimationIntervalBetween), AnimationNumberOfRepeat).Start();
        }

        public bool CheckConditions(Mobile from)
        {
            if (from == null || !from.Alive)
            {
                return false;
            }
            if (!IsChildOf(from.Backpack))
            {
                from.SendLocalizedMessage(1042001); // That must be in your pack for you to use it.
                return false;
            }

            if (from.Mounted)
            {
                from.SendMessage(MsgCantBeMounted); // Musisz stac na ziemi, aby moc sadzic rosliny!
                return false;
            }

            Point3D m_pnt = from.Location;
            Map m_map = from.Map;

            if (!WeedHelper.CheckCanGrow(this, m_map, m_pnt.X, m_pnt.Y))
            {
                from.SendMessage(MsgBadTerrain);    // Roslina na pewno nie urosnie na tym terenie.
                return false;
            }

            if (!WeedHelper.CheckSpace(m_pnt, m_map))
            {
                from.SendMessage(MsgPlantAlreadyHere);  // W tym miejscu cos juz rosnie.
                return false;
            }
            return true;
        }

        // Jakiego typu czynnosci nie mozna wykonywac jednoczesnie ze zrywaniem ziol:
        public object LockKind()
        {
            return typeof(HarvestOrCraftLock);
        }

        public void Unlock(Mobile from)
        {
            from.EndAction(LockKind());
        }

        public bool Animate(Mobile from)
        {
            if (!CheckConditions(from))
                return false;

            if (!from.Mounted)
                from.Animate(32, 5, 1, true, false, 0);

            return true;
        }

        public void PlantWeed(Mobile from)
        {
            if (!CheckConditions(from))
            {
                Unlock(from);
                return;
            }

            if (CheckPlantChance(from))
            {
                // Sadzenie szczepki
                Item item = CreateWeed();
                if (item != null)
                {
                    Point3D m_pnt = from.Location;
                    Map m_map = from.Map;
                    item.Location = m_pnt;
                    item.Map = m_map;
                    from.SendMessage(MsgPlantSuccess);  // Udalo ci sie zasadzic rosline.
                }
                WeedPlant plant = item as WeedPlant;
                if (plant != null)
                    plant.GrowingTime = 60 * 15;
            }
            else
            {
                from.SendMessage(MsgPlantFail); // Nie udalo ci sie zasadzic rosliny, zmarnowales szczepke.
            }

            this.Consume();
            Unlock(from);
        }

    }

}