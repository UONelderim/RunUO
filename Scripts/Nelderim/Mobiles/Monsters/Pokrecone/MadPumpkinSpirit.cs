using System;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("zwloki szalenca")]
    public class MadPumpkinSpirit : BaseCreature
    {
        private readonly Timer m_Timer;

        [Constructable]
        public MadPumpkinSpirit() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.175, 0.350)
        {
            Name = "Szaleniec";
            Body = 1247;

            SetStr(115, 137);
            SetDex(52, 71);
            SetInt(25, 39);

            SetDamage(1, 3);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 25);
            SetResistance(ResistanceType.Fire, -25);
            SetResistance(ResistanceType.Cold, 10);
            SetResistance(ResistanceType.Poison, 15);
            SetResistance(ResistanceType.Energy, 25);

            SetSkill(SkillName.MagicResist, 25.9, 34.4);
            SetSkill(SkillName.Tactics, 45.6, 54.4);
            SetSkill(SkillName.Wrestling, 50.7, 59.6);

            Fame = 1300;
            Karma = -1300;

            this.m_Timer = new InternalTimer(this);
            this.m_Timer.Start();

            PackReg(68, 127);
        }

        public override void AlterMeleeDamageFrom(Mobile from, ref int damage)
        {
            if (from != null && from != this)
            {
                if (from is PlayerMobile)
                {
                    PlayerMobile p_PlayerMobile = from as PlayerMobile;
                    Item weapon1 = p_PlayerMobile.FindItemOnLayer(Layer.OneHanded);
                    Item weapon2 = p_PlayerMobile.FindItemOnLayer(Layer.TwoHanded);

                    if (weapon1 != null)
                    {
                        if (weapon1 is BaseAxe)
                        {
                            damage *= 4;
                        }
                        else if (weapon1 is BasePoleArm)
                        {
                            damage *= 4;
                        }
                        else if (weapon1 is BaseSword)
                        {
                            damage *= 2;
                        }
                        else
                        {
                            damage += 0;
                        }
                    }
                    else if (weapon2 != null)
                    {
                        if (weapon2 is BaseAxe)
                        {
                            damage *= 4;
                        }
                        else if (weapon2 is BasePoleArm)
                        {
                            damage *= 4;
                        }
                        else if (weapon2 is BaseSword)
                        {
                            damage *= 2;
                        }
                        else
                        {
                            damage += 0;
                        }
                    }
                }
            }
        }

        public override bool HasBreath
        {
            get { return true; }
        } // pumpkin throw enabled

        public override double BreathMinDelay
        {
            get { return 15.0; }
        }

        public override double BreathMaxDelay
        {
            get { return 20.0; }
        }

        public override int BreathPhysicalDamage
        {
            get { return 100; }
        }

        public override int BreathFireDamage
        {
            get { return 0; }
        }

        public override int BreathColdDamage
        {
            get { return 0; }
        }

        public override int BreathPoisonDamage
        {
            get { return 0; }
        }

        public override int BreathEnergyDamage
        {
            get { return 0; }
        }

        public override int BreathEffectHue
        {
            get { return 0; }
        }

        public override int BreathEffectItemID
        {
            get { return 0x0C6A; }
        }

        public override int BreathEffectSound
        {
            get { return 0x5D3; }
        }

        public override int BreathAngerSound
        {
            get { return 895; }
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            base.OnMovement(m, oldLocation);

            if (m.Alive)
            {
                if (InRange(m.Location, 10) && !InRange(oldLocation, 10) && m is PlayerMobile)
                {
                    RangePerception = 200;
                    this.Combatant = m;
                }
            }
        }

        public override void OnCarve(Mobile from, Corpse corpse, Item with)
        {
            if (corpse.Carved == false)
            {
                base.OnCarve(from, corpse, with);

                corpse.AddCarvedItem(new Gold(Utility.RandomMinMax(26, 100)), from);

                from.SendMessage("You carve up gold.");
                corpse.Carved = true;
            }
        }

        public override bool CanRummageCorpses
        {
            get { return true; }
        }

        public override int GetAttackSound()
        {
            return 1429;
        }

        public override int GetIdleSound()
        {
            return 383;
        }

        public override int GetAngerSound()
        {
            return 895;
        }

        public override int GetHurtSound()
        {
            return 384;
        }

        public override int GetDeathSound()
        {
            return 897;
        }

        public MadPumpkinSpirit(Serial serial) : base(serial)
        {
            this.m_Timer = new InternalTimer(this);
            this.m_Timer.Start();
        }

        public override void OnDelete()
        {
            this.m_Timer.Stop();

            base.OnDelete();
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

        private class InternalTimer : Timer
        {
            private MadPumpkinSpirit m_Owner;
            private int m_Count = 0;

            public InternalTimer(MadPumpkinSpirit owner) : base(TimeSpan.FromSeconds(0.1), TimeSpan.FromSeconds(0.1))
            {
                m_Owner = owner;
            }

            protected override void OnTick()
            {
                if ((m_Count++ & 0x30) == 0)
                {
                    m_Owner.Direction = (Direction)(Utility.Random(8) | 0x80);
                }

                m_Owner.Move(m_Owner.Direction);
            }
        }
    }
}