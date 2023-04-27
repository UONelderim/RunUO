using Server.Items;
using Server.Network;
using System;

namespace Server.Items
{

    public abstract class BaseSlowDoor : BaseDoor
    {
        [CommandProperty(AccessLevel.GameMaster)]
        public override double CloseDelay
        {
            get
            {
                return m_CloseDelay;
            }
            set
            {
                m_CloseDelay = value;
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public override double OpenDelay
        {
            get
            {
                return m_OpenDelay;
            }
            set
            {
                m_OpenDelay = value;
            }
        }
        [CommandProperty(AccessLevel.GameMaster)]
        public bool IsOpening { get { return m_IsOpening; } }

        private double m_CloseDelay;
        private double m_OpenDelay;
        private bool m_IsOpening;
        private Timer m_Timer;

        private class OpeningSequenceTimer : Timer
        {
            private BaseSlowDoor m_Door;
            private Mobile m_From;

            public OpeningSequenceTimer(BaseSlowDoor door, Mobile from) : base(TimeSpan.FromSeconds(door.OpenDelay))
            {
                m_Door = door;
                m_From = from;
            }

            protected override void OnTick()
            {
                m_Door.FinishOpeningSequence(m_From);
            }
        }

        public BaseSlowDoor(int closedID, int openedID, int openedSound, int closedSound, Point3D offset) : base(closedID, openedID, openedSound, closedSound, offset)
        {
            m_OpenDelay = 5.0;
            m_CloseDelay = 3.0;

            m_IsOpening = false;
        }

        public override void Use(Mobile from)
        {
            if (Open)
            {
                if (m_IsOpening)
                {
                    // This is the case when the door opening sequence has been started,
                    // but the door was force opened by another linked door
                    // before the opening sequence wass finished.
                    InterruptOpeningSequence(from);
                }

                base.Use(from);  // Use default closing mechanism.
            }
            else  // Closed.
            {
                if (!m_IsOpening)
                {
                    StartOpeningSequence(from);
                }

                // Opening sequence already started.
            }
        }

        private void StartOpeningSequence(Mobile from)
        {
            m_IsOpening = true;

            this.PublicOverheadMessage(MessageType.Regular, 0, false, "*zawias spowalnia ruch drzwi*");

            if (m_Timer != null) // sanity
                m_Timer.Stop();
            m_Timer = new OpeningSequenceTimer(this, from);
            m_Timer.Start();
        }

        private void FinishOpeningSequence(Mobile from)
        {
            if (!Open)
                base.Use(from);

            m_IsOpening = false;
        }

        private void InterruptOpeningSequence(Mobile from)
        {
            m_Timer.Stop();
            m_Timer = null;
            m_IsOpening = false;
        }

        public BaseSlowDoor(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer) // Default Serialize method
        {
            base.Serialize(writer);

            writer.Write((int)0); // version

            writer.Write((double)m_OpenDelay);
            writer.Write((double)m_CloseDelay);
        }

        public override void Deserialize(GenericReader reader) // Default Deserialize method
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            m_OpenDelay = reader.ReadDouble();
            m_CloseDelay = reader.ReadDouble();
        }
    }

}