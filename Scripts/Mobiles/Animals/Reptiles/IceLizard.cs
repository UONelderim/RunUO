using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles
{
    [CorpseName("zwloki lodowej jaszczurki")]
    [TypeAlias("Server.Mobiles.Icelizard")]
    public class IceLizard : BaseCreature
    {
        [Constructable]
        public IceLizard()
            : base(AIType.AI_Melee, FightMode.Closest, 12, 1, 0.2, 0.4)
        {
            Name = "lodowa jaszczurka";
            Body = 0xCE;
            Hue = 196;
            BaseSoundID = 0x5A;

            SetStr(126, 150);
            SetDex(56, 75);
            SetInt(11, 20);

            SetHits(76, 90);
            SetMana(0);

            SetDamage(6, 24);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 35, 45);
            SetResistance(ResistanceType.Cold, 30, 45);
            SetResistance(ResistanceType.Poison, 25, 35);
            SetResistance(ResistanceType.Energy, 25, 35);

            SetSkill(SkillName.MagicResist, 55.1, 70.0);
            SetSkill(SkillName.Tactics, 60.1, 80.0);
            SetSkill(SkillName.Wrestling, 60.1, 80.0);

            Fame = 3000;
            Karma = -3000;

            VirtualArmor = 40;

            Tamable = true;
            ControlSlots = 1;
            MinTameSkill = 80.7;

            PackItem(new SulfurousAsh(Utility.Random(4, 10)));
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.Meager);
        }

        // lodowa jaszczurka zieje zimnem zamiast ogniem:
        public override int BreathEffectHue { get { return 196; } }
        public override int BreathFireDamage { get { return 0; } }
        public override int BreathColdDamage { get { return 100; } }

        public override bool HasBreath { get { return true; } } // fire breath enabled
        public override int Hides { get { return 3; } }
        public override HideType HideType { get { return HideType.Spined; } }

        public IceLizard(Serial serial)
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