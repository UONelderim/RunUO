using System;
using Server.Mobiles;
using Server.Gumps;
using Server.Items;

namespace Server.Items
{
    public class WrotaTilki : Moongate
    {
        private static TimeSpan m_DDT = TimeSpan.FromSeconds(300.0); // czas rozpadu w sekundach

        public override int LabelNumber { get { return 1048047; } } // a Moongate

        [Constructable]
        public WrotaTilki()
        {
            Timer.DelayCall(m_DDT, new TimerCallback(Delete));
            Name = "Wrota Tilki";
            Hue = 1281;
            Dispellable = false;
            TargetMap = Map.Felucca;
            Target = new Point3D(5591, 246, 0);
        }

        public WrotaTilki(Serial serial)
            : base(serial)
        {
        }


        public override bool OnMoveOver(Mobile m)
        {
            base.OnMoveOver(m);

            if (!ValidateUse(m, false))
            {
                return false;
            }

            return true;
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