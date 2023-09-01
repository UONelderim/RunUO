using System;
using System.Collections;
using Server.Items;

namespace Server.Mobiles
{
    [CorpseName("zwloki anchimayen")]
    public class Anchimayen : BaseCreature
    {
        [Constructable]
        public Anchimayen() : base(AIType.AI_Melee, FightMode.Closest, 10, 1, 0.175, 0.350)
        {
            Name = "anchimayen";
            Body = 748;
            BaseSoundID = 0x482;
            Hue = 1281;

            SetStr(78, 98);
            SetDex(80, 94);
            SetInt(38, 60);

            SetHits(92, 120);
            SetMana(0);

            SetDamage(1, 4);

            SetDamageType(ResistanceType.Physical, 100);

            SetResistance(ResistanceType.Physical, 28);
            SetResistance(ResistanceType.Fire, 100);
            SetResistance(ResistanceType.Cold, -50);
            SetResistance(ResistanceType.Poison, 5);
            SetResistance(ResistanceType.Energy, -25);

            SetSkill(SkillName.MagicResist, 46.6, 58.7);
            SetSkill(SkillName.Tactics, 45.4, 59.7);
            SetSkill(SkillName.Wrestling, 49.4, 58.1);

            Fame = 1500;
            Karma = -1500;

            PackGold(80, 125);

            if (Utility.RandomDouble() < 0.10)
                PackItem(new MagicUnTrapScroll());

            PackItem(Loot.RandomWeapon());
            PackItem(new Bone());

            if (0.03 > Utility.RandomDouble())
                PackItem(Loot.RandomGem());
        }

        public override bool BleedImmune
        {
            get { return true; }
        }

        public override Poison PoisonImmune
        {
            get { return Poison.Regular; }
        }

        private DateTime m_LastRadiated;
        private Hashtable m_Mobiles = new Hashtable();

        protected override bool OnMove(Direction d)
        {
            if (!IsDeadBondedPet)
            {
                if (m_LastRadiated <= DateTime.Now)
                    m_LastRadiated = DateTime.Now.AddSeconds(Utility.Random(10));
                IPooledEnumerable eable = GetMobilesInRange(2);
                foreach (Mobile m in eable)
                    if (m_Mobiles[m] == null)
                        m_Mobiles[m] = Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromSeconds(1.0),
                            new TimerStateCallback(RadiationCallBack), m);
                eable.Free();
            }

            return base.OnMove(d);
        }

        public override void OnMovement(Mobile m, Point3D oldLocation)
        {
            if (m_LastRadiated <= DateTime.Now)
                m_LastRadiated = DateTime.Now.AddSeconds(Utility.Random(10));
            if (!IsDeadBondedPet && m_Mobiles[m] == null && Utility.InRange(Location, m.Location, 2) &&
                !Utility.InRange(Location, oldLocation, 2))
                m_Mobiles[m] = Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromSeconds(1.0),
                    new TimerStateCallback(RadiationCallBack), m);

            base.OnMovement(m, oldLocation);
        }

        public void RadiationCallBack(object state)
        {
            Mobile m = (Mobile)state;

            if (Deleted || !Alive || !Utility.InRange(Location, m.Location, 2))
            {
                ((Timer)m_Mobiles[m]).Stop();
                m_Mobiles[m] = null;
                return;
            }

            if (this != m && m.AccessLevel == AccessLevel.Player && m_LastRadiated <= DateTime.Now &&
                Server.Spells.SpellHelper.ValidIndirectTarget(m, this) && CanBeHarmful(m, false, false))
            {
                AOS.Damage(m, this, Utility.Random(5, 8), 0, 100, 0, 0, 0, true);
                m.RevealingAction();
                DoHarmful(m);
                m.PlaySound(0x208);
                m.FixedParticles(0x3709, 10, 30, 5052, EffectLayer.LeftFoot);
                m_LastRadiated = DateTime.Now.AddSeconds(Utility.Random(5, 5));
            }
        }

        public Anchimayen(Serial serial) : base(serial)
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