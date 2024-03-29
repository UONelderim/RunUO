﻿using System;
using Server;

namespace Server.Items
{
    public class Gandiva : RepeatingCrossbow
    {
        public override int LabelNumber { get { return 1065855; } } // Gandiva
        public override int InitMinHits { get { return 60; } }
        public override int InitMaxHits { get { return 60; } }

        [Constructable]
        public Gandiva()
        {
            Hue = 2951;

			Slayer = SlayerName.ArachnidDoom;
			Attributes.WeaponDamage = 30;
            Attributes.WeaponSpeed = 30;
            Attributes.AttackChance = 15;
			Attributes.DefendChance = -15;
            WeaponAttributes.HitEnergyArea = 100;
        }

        public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy)
        {
            phys = 40;
            fire = 0;
            cold = 0;
            pois = 0;
            nrgy = 60;
        }

        public Gandiva(Serial serial)
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