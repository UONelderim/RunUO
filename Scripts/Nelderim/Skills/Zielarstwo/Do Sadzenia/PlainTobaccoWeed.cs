using System;
using Server.Network;
using System.Collections;
using System.Collections.Generic;
using Server;
using Server.Targets;
using Server.Items;
using Server.Targeting;
using Server.Spells;
using Server.Mobiles;

namespace Server.Items.Crops
{


    public class PlainTobaccoSapling : WeedSeedZiolaUprawne
    {
        public override Type PlantType => typeof(PlainTobaccoPlant);

        [Constructable]
        public PlainTobaccoSapling(int amount) : base(amount, 0x0CB0)
        {
            Hue = 2129;
            Name = "Szczepka tytoniu pospolitego";
            Stackable = true;
        }

        [Constructable]
        public PlainTobaccoSapling() : this(1)
        {
        }

        public PlainTobaccoSapling(Serial serial) : base(serial)
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

    public class PlainTobaccoPlant : WeedPlantZiolaUprawne
    {
        public override Type SeedType => typeof(PlainTobaccoSapling);
        public override Type CropType => typeof(PlainTobaccoCrop);

        [Constructable]
        public PlainTobaccoPlant() : base(0x0C97)
        {
            Hue = 2129;
            Name = "Tyton pospolity";
            Stackable = true;
        }

        public PlainTobaccoPlant(Serial serial) : base(serial)
        {
            //m_plantedTime = DateTime.Now;	// ???
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }

    public class PlainTobaccoCrop : WeedCropZiolaUprawne
    {
        public override Type ReagentType => typeof(PlainTobacco);

        [Constructable]
        public PlainTobaccoCrop(int amount) : base(amount, 0x0C93)
        {
            Hue = 2129;
            Name = "Swieze liscie tytoniu pospolitego";
            Stackable = true;
        }

        [Constructable]
        public PlainTobaccoCrop() : this(1)
        {
        }

        public PlainTobaccoCrop(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
            writer.Write((int)0);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
            int version = reader.ReadInt();
        }
    }


}