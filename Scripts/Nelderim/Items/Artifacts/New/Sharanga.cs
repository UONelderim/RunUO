﻿using System;
using Server;

namespace Server.Items
{
    public class Sharanga : HeavyCrossbow
    {
        public override int LabelNumber { get { return 1065856; } } // Sharanga
        public override int InitMinHits { get { return 60; } }
        public override int InitMaxHits { get { return 60; } }

        [Constructable]
        public Sharanga()
        {
            Hue = 1462;

            Attributes.AttackChance = 15;
			Attributes.RegenHits = 4;
            Attributes.Luck = 200;
             Attributes.WeaponDamage = 25;
        }

        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy)
        {
            phys = 50;
            fire = 50;
            cold = 0;
            pois = 0;
            nrgy = 0;
        }

        public Sharanga(Serial serial)
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

            if (Attributes.WeaponDamage != 25)
                Attributes.WeaponDamage = 25;
        }
    }
}