using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Items
{
    public class PowerGeneratorKey : Item
    {
        private int m_MaxRange = 5;
        private bool m_ControlPanelSolved = false;

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxRange
        {
            get { return m_MaxRange; }
            set { m_MaxRange = value; }
        }
        [Constructable]
        public PowerGeneratorKey() : base(0x3FEB)
        {
            Weight = 1.0;
            Name = "Klucz szyfrujący";
            Label1 = "*klucz pasuje do panelu kontrolnego*";
        }
        
        public void UseOn(Mobile from, PowerGenerator generator)
        {
            if (generator.ControlPanel != null)
            {
                if (!m_ControlPanelSolved)
                {
                    generator.ControlPanel.Solve(from);
                    m_ControlPanelSolved = true;
                    from.SendMessage("Panel kontrolny został rozwiązany.");
                }
                else
                {
                    from.SendMessage("Panel kontrolny został już rozwiązany.");
                }
            }
            else
            {
                from.SendMessage("Nie można uzyskać dostępu do panelu kontrolnego generatora.");
            }

            this.Delete(); // Move this line outside of the if-else block
        }


        public PowerGeneratorKey(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)2);
            writer.Write((int)m_MaxRange);
            writer.Write(m_ControlPanelSolved);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
            m_MaxRange = reader.ReadInt();
            m_ControlPanelSolved = reader.ReadBool();
        }

        public override void OnDoubleClick(Mobile from)
        {
            if (!this.IsChildOf(from.Backpack))
            {
                from.SendMessage("Twój klucz szyfrowy jest poza zasięgiem.");
                return;
            }

            PowerGenerator powerGenerator = FindPowerGenerator(from);

            if (powerGenerator != null)
            {
                UseOn(from, powerGenerator);
            }
            else
            {
                from.SendMessage("Nie można znaleźć generatora mocy w zasięgu.");
            }
        }

        private PowerGenerator FindPowerGenerator(Mobile from)
        {
            IPooledEnumerable nearbyItems = from.Map.GetItemsInRange(from.Location, m_MaxRange);

            foreach (Item item in nearbyItems)
            {
                if (item is PowerGenerator generator && generator.ControlPanel != null)
                {
                    // Check if the item's name matches the default name of the control panel
                    if (generator.ControlPanel.DefaultName.ToLower() == "panel kontrolny")
                    {
                        nearbyItems.Free();
                        return generator;
                    }
                }
            }

            nearbyItems.Free();
            return null;
        }


    }
}
