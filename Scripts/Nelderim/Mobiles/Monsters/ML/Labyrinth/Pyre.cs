using System;
using System.Collections.Generic;
using Server.Engines.CannedEvil;
using Server.Items;
using Server.Spells;

namespace Server.Mobiles
{
    [CorpseName("zgliszcza starego feniksa")]
    public class Pyre : BaseChampion
    {
        public override ChampionSkullType SkullType => ChampionSkullType.None;
        public override Type[] DecorativeList => Type.EmptyTypes;
        public override MonsterStatuetteType[] StatueTypes => Array.Empty<MonsterStatuetteType>();
        public override bool NoGoodies => true;
        public override double DifficultyScalar => 1.15;

        [Constructable]
        public Pyre() : base(AIType.AI_Mage)
        {
            Name = "stary feniks";
            Body = 0x5;
            Hue = 0x489;

            SetStr(605, 611);
            SetDex(391, 519);
            SetInt(669, 818);

            SetHits(1783, 1939);

            SetDamage(15, 25);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Fire, 50);

            SetResistance(ResistanceType.Physical, 65);
            SetResistance(ResistanceType.Fire, 72, 74);
            SetResistance(ResistanceType.Poison, 36, 41);
            SetResistance(ResistanceType.Energy, 50, 51);

            SetSkill(SkillName.Wrestling, 121.9, 130.6);
            SetSkill(SkillName.Tactics, 114.9, 117.4);
            SetSkill(SkillName.MagicResist, 147.7, 153.0);
            SetSkill(SkillName.Poisoning, 122.8, 124.0);
            SetSkill(SkillName.Magery, 121.8, 127.8);
            SetSkill(SkillName.EvalInt, 103.6, 117.0);

            PSDropCount = 0;
        }

        public override bool OnBeforeDeath()
        {
            AddLoot(LootPack.AvatarScrolls);
            return base.OnBeforeDeath();
        }

        public override void GenerateLoot()
        {
            AddLoot(LootPack.AosUltraRich, 4);
            ArtifactHelper.ArtifactDistribution(this);
        }

        public override void AddWeaponAbilities()
        {
            WeaponAbilities.Add(WeaponAbility.ParalyzingBlow, 0.2);
            WeaponAbilities.Add(WeaponAbility.BleedAttack, 0.2);
        }

        bool tick = false;

        public override void OnThink()
        {
            tick = !tick;

            if (tick)
                return;

            List<Mobile> targets = new List<Mobile>();

            if (Map != null)
            {
                IPooledEnumerable eable = GetMobilesInRange(2);
                foreach (Mobile m in eable)
                {
                    if (this != m && SpellHelper.ValidIndirectTarget(this, m) && CanBeHarmful(m, false) &&
                        (!Core.AOS || InLOS(m)))
                    {
                        if (m is BaseCreature && ((BaseCreature)m).Controlled)
                            targets.Add(m);
                        else if (m.Player)
                            targets.Add(m);
                    }
                }

                eable.Free();
            }

            for (int i = 0; i < targets.Count; ++i)
            {
                Mobile m = targets[i];

                AOS.Damage(m, this, 5, 0, 100, 0, 0, 0);

                if (m.Player)
                    m.SendLocalizedMessage(1008112, Name); // : The intense heat is damaging you!
            }
        }

        public override int TreasureMapLevel => 5;

        public override int Feathers => 36;
        //public override bool GivesMinorArtifact{ get{ return true; } }		

        public override int GetIdleSound()
        {
            return 0x2EF;
        }

        public override int GetAttackSound()
        {
            return 0x2EE;
        }

        public override int GetAngerSound()
        {
            return 0x2EF;
        }

        public override int GetHurtSound()
        {
            return 0x2F1;
        }

        public override int GetDeathSound()
        {
            return 0x2F2;
        }

        public Pyre(Serial serial) : base(serial)
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