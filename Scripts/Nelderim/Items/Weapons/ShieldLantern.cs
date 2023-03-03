using System;
using Server;

namespace Server.Items
{
    public class ShieldLantern : BaseShield
    {
        public override bool CanEquip(Mobile from)
        {
            if (!base.CanEquip(from))
                return false;

            if (from.Skills.Magery.Value < 70.0)
            {
                from.SendMessage("Twoja wiedza o magii jest zbyt mala, aby uzyc tego przedmiotu.");
                return false;
            }

            return true;
        }

        public override int BasePhysicalResistance { get { return 0; } }
        public override int BaseFireResistance { get { return 0; } }
        public override int BaseColdResistance { get { return 0; } }
        public override int BasePoisonResistance { get { return 0; } }
        public override int BaseEnergyResistance { get { return 0; } }
        public override int InitMinHits { get { return 40; } }
        public override int InitMaxHits { get { return 50; } }
        public override int AosStrReq { get { return 20; } }
        public override int ArmorBase { get { return 7; } }

        [Constructable]
        public ShieldLantern() : base(0xA25)
        {
            Name = "Latarnia maga";
        }

        public ShieldLantern(Serial serial) : base(serial)
        {
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0);//version
        }
    }
}
