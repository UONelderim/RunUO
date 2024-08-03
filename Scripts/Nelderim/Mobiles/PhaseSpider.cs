using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("pajecze zwloki")]
    public class PhaseSpider : BaseCreature
    {
        [Constructable]
        public PhaseSpider() : base(AIType.AI_BattleMage, FightMode.Closest, 12, 1, 0.2, 0.4)
        {
            Name = "Pajak Przenikajacy";
            Body = 20;
            BaseSoundID = 0x388;
            Tamable = false;
            Hue = 0x8000 + 0x4000 + 0x1800 + 0xF8;

            SetStr(166, 195);
            SetDex(180, 200);
            SetInt(46, 70);

            SetHits(200, 300);
            SetMana(500);

            SetDamage(20, 30);

            SetDamageType(ResistanceType.Physical, 50);
            SetDamageType(ResistanceType.Energy, 50);

            SetResistance(ResistanceType.Physical, 55, 65);
            SetResistance(ResistanceType.Fire, 15, 25);
            SetResistance(ResistanceType.Cold, 15, 25);
            SetResistance(ResistanceType.Poison, 99, 120);
            SetResistance(ResistanceType.Energy, 80, 85);

            SetSkill(SkillName.MagicResist, 70.0, 90.0);
            SetSkill(SkillName.Tactics, 60.1, 70.0);
            SetSkill(SkillName.Wrestling, 90.0, 120.0);
            SetSkill(SkillName.Magery, 100.0, 120.0);
            SetSkill(SkillName.EvalInt, 78.1, 100.0);
            SetSkill(SkillName.Meditation, 100.0);

            Fame = 3000;
            Karma = -3000;
        }

        public override bool AutoDispel => true;
        public override Poison PoisonImmune => Poison.Lethal;

        private void DrainLife()
        {
            List<Mobile> list = new();

            var eable = GetMobilesInRange(2);
            foreach (Mobile m in eable)
            {
                if (m == this || !CanBeHarmful(m))
                    continue;

                if (m is BaseCreature creature &&
                    (creature.Controlled || creature.Summoned || creature.Team != this.Team))
                    list.Add(m);
                else if (m.Player)
                    list.Add(m);
            }

            eable.Free();

            foreach (var m in list)
            {
                DoHarmful(m);

                m.FixedParticles(0x374A, 10, 15, 5013, 0x496, 0, EffectLayer.Waist);
                m.PlaySound(0x231);

                m.SendMessage("{Pajecza magia wysysa zycie z Ciebie!}");

                var toDrain = Utility.RandomMinMax(5, 15);

                Hits += toDrain;
                m.Damage(toDrain, this);
            }
        }

        public override void OnGaveMeleeAttack(Mobile defender)
        {
            base.OnGaveMeleeAttack(defender);

            if (0.1 >= Utility.RandomDouble())
                DrainLife();
        }

        public override void OnGotMeleeAttack(Mobile attacker)
        {
            base.OnGotMeleeAttack(attacker);

            if (0.1 >= Utility.RandomDouble())
                DrainLife();
        }

        private DateTime m_NextAbilityTime;

        public override void OnThink()
        {
            if (DateTime.Now >= m_NextAbilityTime)
            {
                if (Combatant != null && Combatant.Map == Map && Combatant.InRange(this, 12) && IsEnemy(Combatant) &&
                    !UnderEffect(Combatant))
                {
                    m_NextAbilityTime = DateTime.Now + TimeSpan.FromSeconds(Utility.RandomMinMax(20, 30));

                    Emote("* potomstwo pajaka zaczyna gryzc *");

                    Table[Combatant] = Timer.DelayCall(TimeSpan.FromSeconds(0.5),
                        TimeSpan.FromSeconds(7.0),
                        () => DoEffect(Combatant));
                }
            }

            base.OnThink();
        }

        private static readonly Dictionary<Mobile, Timer> Table = new();

        private static bool UnderEffect(Mobile m)
        {
            return Table.ContainsKey(m);
        }

        private static void StopEffect(Mobile m)
        {
            var timer = Table[m];

            if (timer == null) return;

            timer.Stop();
            Table.Remove(m);

            if (TimerState.ContainsKey(m))
            {
                TimerState.Remove(m);
            }
        }

        private static readonly Dictionary<Mobile, int> TimerState = new();

        private void DoEffect(Mobile m)
        {
            if (m == null || m.Deleted || !m.Alive)
            {
                StopEffect(m);
                return;
            }

            if (!TimerState.ContainsKey(m))
            {
                TimerState.Add(m, 0);
            }

            var count = TimerState[m];

            if (m.FindItemOnLayer(Layer.TwoHanded) is Torch { Burning: true })
            {
                m.Emote("* Otwarty ogien wypala potomstwo pajaka *");
                StopEffect(m);
                return;
            }

            if (count % 4 == 0)
            {
                m.Emote("* jest oszolomiony przez potomstwo pajaka *");
            }

            m.FixedParticles(0x91C, 10, 180, 9539, EffectLayer.Waist);
            m.PlaySound(0x00E);
            m.PlaySound(0x1BC);

            AOS.Damage(m, this, Utility.RandomMinMax(30, 40), 100, 0, 0, 0, 0);

            TimerState[m]++;

            if (!m.Alive)
            {
                StopEffect(m);
            }
        }

        public PhaseSpider(Serial serial) : base(serial)
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