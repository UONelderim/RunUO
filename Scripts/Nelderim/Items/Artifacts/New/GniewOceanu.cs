﻿using System;
using Server;

namespace Server.Items
{
    public class GniewOceanu : Spear
    {
        public override int LabelNumber { get { return 1065798; } } // Gniew Oceanu
        public override int InitMinHits { get { return 60; } }
        public override int InitMaxHits { get { return 60; } }

        [Constructable]
        public GniewOceanu()
        {
            Hue = 1947;
            Attributes.AttackChance = 30;
            Attributes.WeaponDamage = 40;
            Attributes.BonusStam = 10;
			Attributes.BonusMana = 5;
			WeaponAttributes.HitMagicArrow = 20;
        }
		
		public override void GetDamageTypes(Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy)
        {
            phys = 20;
            fire = 0;
            cold = 40;
            pois = 0;
            nrgy = 40;
        }

        public GniewOceanu(Serial serial)
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
