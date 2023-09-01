using System;
using System.Collections;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("zwloki czarnej jaszczurki")]
    public class BlackLizard : BaseCreature
    {
        [Constructable]
        public BlackLizard() : base(AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.2, 0.4)
        {
            Name = "czarna jaszczurka";
            Body = 734;
            Hue = 2406;
            BaseSoundID = 0x5A;

            SetStr(131, 152);
            SetDex(48, 61);
            SetInt(29, 32);

            SetHits(128, 145);
            SetMana(100);

            SetDamage(2, 5);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 30);

            SetSkill(SkillName.MagicResist, 68.9, 79.3);
            SetSkill(SkillName.Tactics, 45.6, 54.4);
            SetSkill(SkillName.Wrestling, 50.7, 59.6);

            Fame = 1000;
            Karma = -1000;

            Tamable = false;


            PackGold(3, 29);
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
                        if (weapon1 is BaseKnife)
                        {
                            damage *= 2;
                        }
                        else if (weapon1 is BaseSpear)
                        {
                            damage *= 4;
                        }
                        else
                        {
                            damage += 0;
                        }
                    }
                    else if (weapon2 != null)
                    {
                        if (weapon2 is BaseKnife)
                        {
                            damage *= 2;
                        }
                        else if (weapon2 is BaseSpear)
                        {
                            damage *= 4;
                        }
                        else
                        {
                            damage += 0;
                        }
                    }
                }
            }
        }

        public override FoodType FavoriteFood
        {
            get { return FoodType.Meat | FoodType.Fish; }
        }

        public override bool HasBreath
        {
            get { return true; }
        } // black cloud enabled

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
            get { return 2406; }
        }

        public override int BreathEffectItemID
        {
            get { return 0x113E; }
        }

        public override int BreathEffectSound
        {
            get { return 0x364; }
        }

        public override int BreathAngerSound
        {
            get { return 0x1C0; }
        }

        public override void OnDamagedBySpell(Mobile from)
        {
            if (from != null && from.Alive && 0.4 > Utility.RandomDouble())
            {
                ShootGrassWind(from);
            }
        }

        public void ShootGrassWind(Mobile to)
        {
            int damage = 8;
            this.MovingEffect(to, 0x0C4E, 10, 0, false, false);
            this.DoHarmful(to);
            this.PlaySound(0x32F); // f_shush
            AOS.Damage(to, this, damage, 100, 0, 0, 0, 0);
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            if (!IsFanned(attacker) && 0.05 > Utility.RandomDouble())
            {
                attacker.SendMessage(
                    "The black lizard fans you with gas, reducing your resistance to physical attacks.");

                ResistanceMod mod = new ResistanceMod(ResistanceType.Physical, -50);

                attacker.FixedParticles(0x3779, 10, 30, 0x34, EffectLayer.RightFoot);
                attacker.PlaySound(0x1E6);

                // This should be done in place of the normal attack damage.
                //AOS.Damage( attacker, this, Utility.RandomMinMax( 5, 10 ), 0, 0, 0, 100, 0 );

                attacker.AddResistanceMod(mod);

                ExpireTimer timer = new ExpireTimer(attacker, mod, TimeSpan.FromSeconds(10.0));
                timer.Start();
                m_Table[attacker] = timer;
            }
        }

        private static Hashtable m_Table = new Hashtable();

        public bool IsFanned(Mobile m)
        {
            return m_Table.Contains(m);
        }

        private class ExpireTimer : Timer
        {
            private Mobile m_Mobile;
            private ResistanceMod m_Mod;

            public ExpireTimer(Mobile m, ResistanceMod mod, TimeSpan delay) : base(delay)
            {
                m_Mobile = m;
                m_Mod = mod;
                Priority = TimerPriority.TwoFiftyMS;
            }

            protected override void OnTick()
            {
                m_Mobile.SendMessage("Your resistance to physical attacks has returned.");
                m_Mobile.RemoveResistanceMod(m_Mod);
                Stop();
                m_Table.Remove(m_Mobile);
            }
        }

        public override void OnCarve(Mobile from, Corpse corpse, Item with)
        {
            if (corpse.Carved == false)
            {
                base.OnCarve(from, corpse, with);

                corpse.AddCarvedItem(new SpinedHides(12), from);


                from.SendMessage("You carve up some spined hides.");
                corpse.Carved = true;
            }
        }

        public BlackLizard(Serial serial) : base(serial)
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