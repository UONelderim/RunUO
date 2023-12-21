using System;
using Server;
using Server.Mobiles;

namespace Server.Items
{
    public class Fajka : BaseEarrings
    {
        private class SmokeTimer : Timer
        {
            private Mobile m_Mobile;
            private bool m_Executed;

            public SmokeTimer(Mobile mobile) : base(TimeSpan.FromSeconds(2), TimeSpan.Zero) 
            {
                m_Mobile = mobile;
                Priority = TimerPriority.OneSecond;
                m_Executed = false;


                Start();
            }

            private void InitialEffectsCallback(object state)
            {
                int tyton = Core.AOS ? 4 : 5;

                // Check if there is enough Tyton in the backpack
                if (m_Mobile.Backpack == null || m_Mobile.Backpack.GetAmount(typeof(Tyton)) < tyton)
                {
                    m_Mobile.SendMessage("Za malo tytoniu, moj Panie.");
                    return; // Exit the method if the condition is not met
                }

                // Continue with the rest of the action if there is enough Tyton
                Effects.SendLocationParticles(EffectItem.Create(m_Mobile.Location, m_Mobile.Map, EffectItem.DefaultDuration), 0x3728, 1, 13, 9965);

                m_Mobile.PlaySound(0x15F);
                m_Mobile.SendMessage("Dym tytoniowy napelnia Twe pluca");
                m_Mobile.RevealingAction();

                m_Mobile.Backpack.ConsumeUpTo(typeof(Tyton), tyton);

                m_Executed = true;
            }


            protected override void OnTick()
            {
                if (!m_Executed)
                {
                    InitialEffectsCallback(null);
                    Stop(); // Stop the timer after it has executed once
                }
                else
                {
                    Interval = TimeSpan.FromSeconds(2);
                    Start();
                }
            }

        }

        [Constructable]
        public Fajka() : base(0x17B3)
        {
            Weight = 0.1;
            Name = "Fajka";
            Light = LightType.Circle150;
        }

        public override void OnDoubleClick(Mobile from)
        {
            SmokeTimer timer = new SmokeTimer(from);
            from.SendMessage("Probujesz odpalic fajke");
        }

        public Fajka(Serial serial) : base(serial)
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
    }
}
