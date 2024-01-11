using System;
using Server.Items;
using Server.Targeting;

namespace Server.Items
{
    public class GoldenWool : Item
    {
        [Constructable]
        public GoldenWool() : this(1)
        {
        }

        [Constructable]
        public GoldenWool(int amount) : base(0xDF8)
        {
            Stackable = true;
            Weight = 0.1;
            Amount = amount;

            Name = "Zlote runo";
            Hue = 1965;
        }

        public GoldenWool(Serial serial) : base(serial)
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