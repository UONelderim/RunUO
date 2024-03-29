﻿using System;
using Server;

namespace Server.Items
{
    public class Gungnir : Pike
    {
        public override int LabelNumber { get { return 1065768; } } // Gungnir
        public override int InitMinHits { get { return 60; } }
        public override int InitMaxHits { get { return 60; } }

        [Constructable]
        public Gungnir()
        {
            Hue = 1151;
            Attributes.DefendChance = 10;
            Attributes.AttackChance = 10;
            Attributes.WeaponDamage = 40;
            Attributes.BonusStr = 10;
        }

        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy)
        {
            phys = 70;
            fire = 0;
            cold = 0;
            pois = 0;
            nrgy = 30;
        }

        public Gungnir(Serial serial)
            : base(serial)
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
