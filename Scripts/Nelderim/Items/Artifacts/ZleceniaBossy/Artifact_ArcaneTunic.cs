using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{
    public class ArcaneTunic : LeatherChest
    {
        public override int InitMinHits { get { return 60; } }
        public override int InitMaxHits { get { return 60; } }

        private int originalDefenseChance;

        private DateTime nextUseTime; // Stores the next allowed use time
        private Timer resetTimer; // Timer for resetting DefendChance

        [Constructable]
        public ArcaneTunic()
        {
            Name = "Tunika Arkanisty z Thila";
            Hue = 0x556;
            Attributes.DefendChance = 0;
            Attributes.CastSpeed = 1;
            Attributes.LowerManaCost = 10;
            Attributes.LowerRegCost = 5;
            Attributes.SpellDamage = 4;

            originalDefenseChance = Attributes.DefendChance;

            resetTimer = new InternalResetTimer(this);
            resetTimer.Start();
        }

        public override void AddNameProperties(ObjectPropertyList list)
        {
            base.AddNameProperties(list);
            list.Add(1049644, "Dotkniecie symbolu wyrytego na piersi tuniki powoduje zwiekszenie umiejetnosci unikania ciosow");
        }
        
        
        public override void OnRemoved(object parent)
        {
            base.OnRemoved(parent);

            Mobile character = parent as Mobile;

            if (character != null && character.Backpack != null && this.Layer == Layer.InnerTorso)
            {
                // Decrease Attributes.DefendChance by 10 when the item is removed from the chest to the backpack
                Attributes.DefendChance -= 10;
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!IsChildOf(from.FindItemOnLayer(Layer.InnerTorso)))
            {
                if (DateTime.UtcNow < nextUseTime)
                {
                    from.SendMessage("Musisz odczekac jeszcze troche czasu przed uzyciem tego ponownie.");
                    return;
                }

                if (Attributes.DefendChance < 10) // Nie wyjdzie poza 10 DCI
                {
                    Attributes.DefendChance += 10;
                    from.SendMessage("Twoja umiejetnosc unikania ciosow wzrasta.");
                }
                else if (Attributes.DefendChance >= 10)
                {
                    from.SendMessage("Twoja umiejetnosc unikania ciosow jest juz na maksymalnym poziomie.");
                }
        
                nextUseTime = DateTime.UtcNow + TimeSpan.FromMinutes(5); // Cooldown 5 min
            }
        }


        private void ResetDefenseChance()
        {
            Attributes.DefendChance = originalDefenseChance;
        }

        private class InternalResetTimer : Timer
        {
            private readonly ArcaneTunic m_Tunic;

            public InternalResetTimer(ArcaneTunic tunic) : base(TimeSpan.FromMinutes(5.0))
            {
                m_Tunic = tunic;
                Priority = TimerPriority.OneSecond;
            }

            protected override void OnTick()
            {
                if (m_Tunic != null && !m_Tunic.Deleted)
                {
                    m_Tunic.ResetDefenseChance();
                }
            }
        }

        public ArcaneTunic(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0); // Version
            writer.Write(originalDefenseChance);
            writer.Write(nextUseTime);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            originalDefenseChance = reader.ReadInt();
            nextUseTime = reader.ReadDateTime();
        }
    }
}
