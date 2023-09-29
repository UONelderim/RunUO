using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;
using System.Text;
using System.Collections;
using Server.Network;
using Server.Spells;
using Server.Spells.Fourth;
using Server.Mobiles;

namespace Server.Items
{
    public enum TunikaAbsolutnegoLeczeniaEffect
    {
        Default,
        
    }

    public class TunikaAbsolutnegoLeczenia : LeatherChest
    {
        private TunikaAbsolutnegoLeczeniaEffect m_TunikaAbsolutnegoLeczeniaEffect;
        private int m_Charges;

        private DateTime nextUseTime; 
        private Timer resetTimer;

        [CommandProperty(AccessLevel.GameMaster)]
        public TunikaAbsolutnegoLeczeniaEffect Effect
        {
            get { return m_TunikaAbsolutnegoLeczeniaEffect; }
            set { m_TunikaAbsolutnegoLeczeniaEffect = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Charges
        {
            get { return m_Charges; }
            set { m_Charges = value; InvalidateProperties(); }
        }

        [Constructable]
        public TunikaAbsolutnegoLeczenia() : base()
        {
            Name = "Tunika Uzdrowiciela z Tasandory";
            Hue = 2321;

            Attributes.LowerRegCost = 15;


            m_TunikaAbsolutnegoLeczeniaEffect = TunikaAbsolutnegoLeczeniaEffect.Default; 
            m_Charges = Utility.RandomMinMax(5, 10); // Replace with appropriate min and max values

            resetTimer = new InternalResetTimer(this);
            resetTimer.Start();
        }

        public void ConsumeCharge(Mobile from)
        {
            --Charges;

            if (Charges == 0)
            {
                from.SendLocalizedMessage(1019073); // This item is out of charges.
            }
        }

        public virtual void ReleaseTunikaAbsolutnegoLeczeniaLock_Callback(object state)
        {
            ((Mobile)state).EndAction(typeof(TunikaAbsolutnegoLeczenia));
        }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);
            list.Add(1049644, "Dotkniecie symbolu wyrytego na piersi tuniki powoduje odtrucie");
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);
            list.Add(1060741, m_Charges.ToString());
        }

        public void Cast(Spell spell)
        {
            bool m = Movable;

            Movable = false;
            spell.Cast();
            Movable = m;
        }

        public virtual void OnTunikaAbsolutnegoLeczeniaUse(Mobile from)
        {
            from.Target = new TunikaAbsolutnegoLeczeniaTarget(this);
        }

        public virtual void DoTunikaAbsolutnegoLeczeniaTarget(Mobile from, object o)
        {
            if (Deleted || Charges <= 0 || Parent != from || o is StaticTarget || o is LandTarget)
                return;

            // Implement logic for targeting here?? Dunno, mate
        }

        public virtual bool OnTunikaAbsolutnegoLeczeniaTarget(Mobile from, object o)
        {
            return true;
        }


public override void OnDoubleClick(Mobile from)
{
    if (Parent == from)
    {
        if (Charges > 0)
        {
            OnTunikaAbsolutnegoLeczeniaUse(from);
        }
    }
    else
    {
        from.SendLocalizedMessage(502641); // You must equip this item to use it.
    }

    if (!IsChildOf(from.FindItemOnLayer(Layer.InnerTorso)))
    {
        if (DateTime.UtcNow < nextUseTime)
        {
            from.SendMessage("Musisz odczekac jeszcze troche czasu przed uzyciem tego ponownie.");
            return;
        }


        ArchCureSpell archCureSpell = new ArchCureSpell(from, null);
        archCureSpell.Cast();

        nextUseTime = DateTime.UtcNow + TimeSpan.FromMinutes(5); // Cooldown 5 min after using
    }
}





       private void ResetCastSpell()
       {
            // Implement reset logic if needed ?? Have it later I guess
       }

        private class InternalResetTimer : Timer
        {
            private readonly TunikaAbsolutnegoLeczenia m_Tunic;

            public InternalResetTimer(TunikaAbsolutnegoLeczenia tunic) : base(TimeSpan.FromMinutes(5.0))
            {
                m_Tunic = tunic;
                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                if (m_Tunic != null && !m_Tunic.Deleted)
                {
                    m_Tunic.ResetCastSpell();
                }
            }
        }

        public class TunikaAbsolutnegoLeczeniaTarget : Target
        {
            private TunikaAbsolutnegoLeczenia m_Item;

            public TunikaAbsolutnegoLeczeniaTarget(TunikaAbsolutnegoLeczenia item) : base(6, false, TargetFlags.None)
            {
                m_Item = item;
            }

            private static int GetOffset(Mobile caster)
            {
                return 5 + (int)(caster.Skills[SkillName.Magery].Value * 0.02);
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                m_Item.DoTunikaAbsolutnegoLeczeniaTarget(from, targeted);
            }
        }

        public TunikaAbsolutnegoLeczenia(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // Version
            writer.Write((int)m_TunikaAbsolutnegoLeczeniaEffect);
            writer.Write((int)m_Charges);
            writer.Write(nextUseTime);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            switch (version)
            {
                case 0:
                    {
                        m_TunikaAbsolutnegoLeczeniaEffect = (TunikaAbsolutnegoLeczeniaEffect)reader.ReadInt();
                        m_Charges = (int)reader.ReadInt();
                        break;
                    }
            }
            nextUseTime = reader.ReadDateTime();
        }
    }
}
