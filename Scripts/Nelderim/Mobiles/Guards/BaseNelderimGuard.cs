﻿using System;
using System.Reflection;
using Server.Items;
using Server.Nelderim;

namespace Server.Mobiles;

public enum GuardType
{
    StandardGuard,
    ArcherGuard,
    HeavyGuard,
    MageGuard,
    MountedGuard,
    EliteGuard,
    SpecialGuard
}

public enum WarFlag
{
    None,
    White,
    Black,
    Red,
    Green,
    Blue
}

public enum EnemyType
{
    Default,
    Spider
}

public class BaseNelderimGuard : BaseCreature
{
    private GuardType m_Type;
    private string m_RegionName;
    private WarFlag m_Flag;
    private WarFlag m_Enemy;
    private string m_IsEnemyFunction;

    [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
    public string HomeRegionName
    {
        get { return m_RegionName; }

        set
        {
            m_RegionName = value;

            try
            {
                if (!NelderimRegionSystem.MakeGuard(this, m_RegionName))
                    m_RegionName = null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                m_RegionName = null;
            }
        }
    }

    public bool IsEnemyOfSpider(Mobile m)
    {
        // Nie atakuj innych straznikow (obszarowka moze triggerowac walke miedzy nimi)
        if (m is BaseNelderimGuard)
            return false;

        // nie atakuj drowow i obywateli drowiego miasta (oraz ich zwierzat i przywolancow)
        if (BaseAI.IsSpidersFriend(m))
            return false;

        // atakuj wszystkich graczy
        if (m is PlayerMobile)
            return true;

        if (m is BaseCreature)
        {
            BaseCreature bc = m as BaseCreature;

            // atakuj pety i przywolance graczy
            if ((bc.Controlled && bc.ControlMaster != null && bc.ControlMaster.AccessLevel < AccessLevel.Counselor) ||
                (bc.Summoned && bc.SummonMaster != null && bc.SummonMaster.AccessLevel < AccessLevel.Counselor))
                return true;

            // nie atakuj dzikich pajakow
            if (SlayerGroup.GetEntryByName(SlayerName.SpidersDeath).Slays(m) && !bc.IsChampionSpawn && !(m is NSzeol) &&
                !(m is Mephitis))
                return false;

            // atakuj stworzenia red/krim
            if (bc.AlwaysMurderer || bc.Criminal || (!bc.Controlled && bc.FightMode == FightMode.Closest))
                return true;
        }

        return false;
    }

    public bool IsEnemyOfDefaultGuard(Mobile m)
    {
        if (m == null)
            return false;

        if (m is BaseNelderimGuard)
            return false;

        if (m.Criminal || m.Kills >= 5)
            return true;

        if (WarOpponentFlag != WarFlag.None && WarOpponentFlag == (m as BaseNelderimGuard).WarSideFlag)
            return true;

        if (m is BaseCreature)
        {
            BaseCreature bc = m as BaseCreature;
            if (bc.AlwaysMurderer || (!bc.Controlled && bc.FightMode == FightMode.Closest))
                return true;

            if ((bc.Controlled && bc.ControlMaster != null && bc.ControlMaster.AccessLevel < AccessLevel.Counselor &&
                 (bc.ControlMaster.Criminal || bc.ControlMaster.Kills >= 5)) ||
                (bc.Summoned && bc.SummonMaster != null && bc.SummonMaster.AccessLevel < AccessLevel.Counselor &&
                 (bc.SummonMaster.Criminal || bc.SummonMaster.Kills >= 5)))
                return true;
        }

        return false;
    }

    public override bool IsEnemy(Mobile m)
    {
        if (String.IsNullOrEmpty(m_IsEnemyFunction))
        {
            if (m_ConfiguredAccordingToRegion)
            {
                // no IsEnemy configuration in region data, set the default behaviour
                return IsEnemyOfDefaultGuard(m);
            }
            else
            {
                try
                {
                    NelderimGuardProfile nelderimGuardProfile =
                        NelderimRegionSystem.GetRegion(Region.Name).GuardProfile(Type);
                    if (nelderimGuardProfile != null)
                    {
                        m_IsEnemyFunction = nelderimGuardProfile.IsEnemyFunction;
                    }

                    m_ConfiguredAccordingToRegion = true;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error setting isEnemyFunction " + e.Message);
                }

                // region data not processed yet, set harmless behaviour to avoid undesirable attacks
                // (this situation only occurs briefly right after guard spawn)
                return false;
            }
        }
        else
        {
            MethodInfo method = typeof(BaseNelderimGuard).GetMethod(m_IsEnemyFunction);
            if (method != null)
            {
                // behaviour configured by region data
                return (bool)method.Invoke(this, new[] { m });
            }
            else
            {
                // ERROR situation, fallback to default (IsEnemy configured by region data doesn't match any method of NelderimGuard class):
                return IsEnemyOfDefaultGuard(m);
            }
        }
    }

    public override void CriminalAction(bool message)
    {
        // Straznik nigdy nie dostanie krima.
        // Byl problem, ze gdy straznik atakowal peta/summona gracza-krima to sam dostawal krima.s
        return;
    }

    [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
    public WarFlag WarSideFlag
    {
        get { return m_Flag; }
        set
        {
            m_Flag = value;

            if (m_Flag != WarFlag.None && m_Flag == m_Enemy)
                m_Enemy = WarFlag.None;
        }
    }

    [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
    public WarFlag WarOpponentFlag
    {
        get { return m_Enemy; }
        set
        {
            m_Enemy = value;

            if (m_Enemy != WarFlag.None && m_Flag == m_Enemy)
                m_Flag = WarFlag.None;
        }
    }

    public bool IsHuman
    {
        get { return BodyValue == 400 || BodyValue == 401; }
    }


    public BaseNelderimGuard(GuardType type) : this(type, FightMode.Criminal)
    {
    }

    public BaseNelderimGuard(GuardType type, FightMode fmode) : base(AIType.AI_Melee, fmode, 12, 1, 0.1, 0.4)
    {
        m_Type = type;
        m_RegionName = null;
        m_Flag = WarFlag.None;
        m_Enemy = WarFlag.None;
        ActiveSpeed = 0.05;
        PassiveSpeed = 0.1;

        switch (type)
        {
            case GuardType.MountedGuard:
                RangePerception = 16;
                AI = AIType.AI_Mounted;
                PackGold(40, 80);
                break;
            case GuardType.ArcherGuard:
                RangePerception = 16;
                RangeFight = 6;
                AI = AIType.AI_Archer;
                PackGold(30, 90);
                break;
            case GuardType.EliteGuard:
                RangePerception = 18;
                AI = AIType.AI_Melee;
                PackGold(50, 100);
                break;
            case GuardType.SpecialGuard:
                RangePerception = 20;
                AI = AIType.AI_Melee;
                PackGold(60, 100);
                break;
            case GuardType.HeavyGuard:
                RangePerception = 16;
                AI = AIType.AI_Melee;
                PackGold(40, 80);
                break;
            case GuardType.MageGuard:
                RangePerception = 16;
                AI = AIType.AI_Mage;
                PackGold(40, 80);
                break;
            default:
                PackGold(20, 80);
                break;
        }

        Fame = 5000;
        Karma = 5000;

        new RaceTimer(this).Start();
    }

    public BaseNelderimGuard(Serial serial) : base(serial)
    {
    }

    public override bool AutoDispel
    {
        get { return true; }
    }

    public override bool Unprovokable
    {
        get { return true; }
    }

    public override bool Uncalmable
    {
        get { return true; }
    }

    public override bool BardImmune
    {
        get { return true; }
    }

    public override Poison PoisonImmune
    {
        get { return Poison.Greater; }
    }

    public override bool HandlesOnSpeech(Mobile from)
    {
        return true;
    }

    public override bool ShowFameTitle
    {
        get { return false; }
    }

    public override void AddWeaponAbilities()
    {
        WeaponAbilities.Add(WeaponAbility.Dismount, 0.2);
        WeaponAbilities.Add(WeaponAbility.BleedAttack, 0.2);
    }

    public GuardType Type
    {
        get { return m_Type; }
    }

    [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
    public string IsEnemyFunction
    {
        get { return m_IsEnemyFunction; }
        set { m_IsEnemyFunction = value; }
    }

    [CommandProperty(AccessLevel.Counselor, AccessLevel.GameMaster)]
    public bool ConfiguredAccordingToRegion
    {
        get { return m_ConfiguredAccordingToRegion; }
        set { m_ConfiguredAccordingToRegion = value; }
    }

    public override void Serialize(GenericWriter writer)
    {
        base.Serialize(writer);

        writer.Write((int)2);

        // v 3
        // writer.Write( (string) m_IsEnemyFunction );

        // v 2
        writer.Write((int)m_Flag);
        writer.Write((int)m_Enemy);

        // v 1
        writer.Write(m_RegionName);
    }

    public override void Deserialize(GenericReader reader)
    {
        base.Deserialize(reader);

        int version = reader.ReadInt();

        switch (version)
        {
            case 3:
            {
                m_IsEnemyFunction = reader.ReadString();
                goto case 2;
            }
            case 2:
            {
                m_Flag = (WarFlag)reader.ReadInt();
                m_Enemy = (WarFlag)reader.ReadInt();
                goto case 1;
            }
            case 1:
            {
                if (version < 2)
                {
                    m_Flag = WarFlag.None;
                    m_Enemy = WarFlag.None;
                }

                m_RegionName = reader.ReadString();
                break;
            }
            default:
            {
                if (version < 1)
                    m_RegionName = null;
                break;
            }
        }
    }
    
    private class RaceTimer : Timer
    {
        private BaseNelderimGuard m_Target;

        public RaceTimer(BaseNelderimGuard target) : base(TimeSpan.FromMilliseconds(250))
        {
            m_Target = target;
            Priority = TimerPriority.FiftyMS;
        }

        protected override void OnTick()
        {
            try
            {
                if (!m_Target.Deleted)
                {
                    NelderimRegionSystem.MakeGuard(m_Target);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}