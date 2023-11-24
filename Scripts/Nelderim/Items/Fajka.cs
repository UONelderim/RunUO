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

                Effects.SendLocationParticles(EffectItem.Create(m_Mobile.Location, m_Mobile.Map, EffectItem.DefaultDuration), 0x3728, 1, 13, 9965);
                
                m_Mobile.PlaySound(0x15F);
                m_Mobile.SendMessage("Dym tytoniowy napelnia Twe pluca");
                m_Mobile.RevealingAction();

                m_Executed = true; 
            }

            protected override void OnTick()
            {
                if (!m_Executed)
                {
                    InitialEffectsCallback(null);
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
            from.SendMessage("Odpalasz fajke");
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
