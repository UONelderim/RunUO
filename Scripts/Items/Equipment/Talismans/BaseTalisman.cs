using Server.Commands;
using Server.Engines.Craft;
using Server.Mobiles;
using Server.Spells.Fifth;
using Server.Spells.First;
using Server.Spells.Fourth;
using Server.Spells.Necromancy;
using Server.Spells.Second;
using Server.Targeting;
using System;

namespace Server.Items
{
    public enum TalismanSkill
    {
        Alchemy,
        Blacksmithy,
        Fletching,
        Carpentry,
        Cartography,
        Cooking,
        Glassblowing,
        Inscription,
        Masonry,
        Tailoring,
        Tinkering
    }

    public class BaseTalisman : Item, IWearableDurability //, IVvVItem, IOwnerRestricted, ITalismanProtection, ITalismanKiller, IArtifact    // zakomentowane interfejsy = funkcjonalnosc ServUO.
    {
        public override int LabelNumber { get { return 1071023; } } // Talisman

        public virtual bool ForceShowName { get { return false; } } // used to override default summoner/removal name

        private int m_MaxHitPoints;
        private int m_HitPoints;

        [CommandProperty(AccessLevel.GameMaster)]
        public int MaxHitPoints
        {
            get
            {
                return m_MaxHitPoints;
            }
            set
            {
                m_MaxHitPoints = value;

                if (m_MaxHitPoints > 255)
                    m_MaxHitPoints = 255;

                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int HitPoints
        {
            get
            {
                return m_HitPoints;
            }
            set
            {
                if (value != m_HitPoints && MaxHitPoints > 0)
                {
                    m_HitPoints = value;

                    if (m_HitPoints < 0)
                        Delete();
                    else if (m_HitPoints > MaxHitPoints)
                        m_HitPoints = MaxHitPoints;

                    InvalidateProperties();
                }
            }
        }

        public virtual int InitMinHits { get { return 25; } }

        public virtual int InitMaxHits { get { return 25; } }

        public virtual bool CanRepair { get { return true; } }
        public virtual bool CanFortify { get { return false; } }

        #region Craft bonuses
        private TalismanSkill m_Skill;
        private int m_SuccessBonus;
        private int m_ExceptionalBonus;

        [CommandProperty(AccessLevel.GameMaster)]
        public TalismanSkill Skill
        {
            get
            {
                return m_Skill;
            }
            set
            {
                m_Skill = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public SkillName CraftSkill { get { return GetMainSkill(); } }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SuccessBonus
        {
            get
            {
                return m_SuccessBonus;
            }
            set
            {
                m_SuccessBonus = value;
                InvalidateProperties();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int ExceptionalBonus
        {
            get
            {
                return m_ExceptionalBonus;
            }
            set
            {
                m_ExceptionalBonus = value;
                InvalidateProperties();
            }
        }
#endregion

        public BaseTalisman()
            : this(GetRandomItemID())
        {
        }

        public BaseTalisman(int itemID)
            : base(itemID)
        {
            Layer = Layer.Talisman;
            Weight = 1.0;

            m_HitPoints = m_MaxHitPoints = Utility.RandomMinMax(InitMinHits, InitMaxHits);
        }

        public BaseTalisman(Serial serial)
            : base(serial)
        {
        }

        public virtual int OnHit(BaseWeapon weap, int damage)
        {
            if (m_MaxHitPoints == 0)
                return damage;

            //int chance = m_NegativeAttributes.Antique > 0 ? 50 : 25;      // Funkcjonalnosc ServUO
            int chance = 25;
            if (chance > Utility.Random(100)) // 25% chance to lower durability
            {
                if (m_HitPoints >= 1)
                {
                    HitPoints--;
                }
                else if (m_MaxHitPoints > 0)
                {
                    MaxHitPoints--;

                    if (Parent is Mobile)
                        ((Mobile)Parent).LocalOverheadMessage(Network.MessageType.Regular, 0x3B2, 1061121); // Your equipment is severely damaged.

                    if (m_MaxHitPoints == 0)
                    {
                        Delete();
                    }
                }
            }

            return damage;
        }

        public virtual void UnscaleDurability()
        {
        }

        public virtual void ScaleDurability()
        {
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            if (m_ExceptionalBonus != 0)
                list.Add(1072395, "#{0}\t{1}", GetSkillLabel(), m_ExceptionalBonus); // ~1_NAME~ Exceptional Bonus: ~2_val~%

            if (m_SuccessBonus != 0)
                list.Add(1072394, "#{0}\t{1}", GetSkillLabel(), m_SuccessBonus); // ~1_NAME~ Bonus: ~2_val~%

            base.AddResistanceProperties(list);

            if (m_MaxHitPoints > 0)
                list.Add(1060639, "{0}\t{1}", m_HitPoints, m_MaxHitPoints); // durability ~1_val~ / ~2_val~
        }

        private static void SetSaveFlag(ref SaveFlag flags, SaveFlag toSet, bool setIf)
        {
            if (setIf)
                flags |= toSet;
        }

        private static bool GetSaveFlag(SaveFlag flags, SaveFlag toGet)
        {
            return ((flags & toGet) != 0);
        }

        [Flags]
        private enum SaveFlag
        {
            None = 0x00000000,
            Attributes = 0x00000001,
            SkillBonuses = 0x00000002,
            Owner = 0x00000004,
            Protection = 0x00000008,
            Killer = 0x00000010,
            Summoner = 0x00000020,
            Removal = 0x00000040,
            OldKarmaLoss = 0x00000080,
            Skill = 0x00000100,
            SuccessBonus = 0x00000200,
            ExceptionalBonus = 0x00000400,
            MaxCharges = 0x00000800,
            Charges = 0x00001000,
            MaxChargeTime = 0x00002000,
            ChargeTime = 0x00004000,
            Blessed = 0x00008000,
            Slayer = 0x00010000,
            SAAbsorptionAttributes = 0x00020000,
            NegativeAttributes = 0x00040000,
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(4); // version

            writer.Write(null as Mobile);   // m_Creature (ServUO)

            writer.Write(false);            // _VvVItem (ServUO)
            writer.Write(null as Mobile);   // _Owner (ServUO)
            writer.Write("");               // _OwnerName (ServUO)

            writer.Write(m_MaxHitPoints);
            writer.Write(m_HitPoints);

            SaveFlag flags = SaveFlag.None;

            SetSaveFlag(ref flags, SaveFlag.Attributes, false);             // Nie zapisujemy. Te dane beda uzywane dopiero w ServUO
            SetSaveFlag(ref flags, SaveFlag.SkillBonuses, false);           // Nie zapisujemy. Te dane beda uzywane dopiero w ServUO
            SetSaveFlag(ref flags, SaveFlag.Protection, false);             // Nie zapisujemy. Te dane beda uzywane dopiero w ServUO
            SetSaveFlag(ref flags, SaveFlag.Killer, false);                 // Nie zapisujemy. Te dane beda uzywane dopiero w ServUO
            SetSaveFlag(ref flags, SaveFlag.Summoner, false);               // Nie zapisujemy. Te dane beda uzywane dopiero w ServUO
            SetSaveFlag(ref flags, SaveFlag.Removal, false);                // Nie zapisujemy. Te dane beda uzywane dopiero w ServUO
            SetSaveFlag(ref flags, SaveFlag.Skill, m_Skill != 0);
            SetSaveFlag(ref flags, SaveFlag.SuccessBonus, m_SuccessBonus != 0);
            SetSaveFlag(ref flags, SaveFlag.ExceptionalBonus, m_ExceptionalBonus != 0);
            SetSaveFlag(ref flags, SaveFlag.MaxCharges, false);             // Nie zapisujemy. Te dane beda uzywane dopiero w ServUO
            SetSaveFlag(ref flags, SaveFlag.Charges, false);                // Nie zapisujemy. Te dane beda uzywane dopiero w ServUO
            SetSaveFlag(ref flags, SaveFlag.MaxChargeTime, false);          // Nie zapisujemy. Te dane beda uzywane dopiero w ServUO
            SetSaveFlag(ref flags, SaveFlag.ChargeTime, false);             // Nie zapisujemy. Te dane beda uzywane dopiero w ServUO
            SetSaveFlag(ref flags, SaveFlag.Blessed, false);                // Nie zapisujemy. Te dane beda uzywane dopiero w ServUO
            SetSaveFlag(ref flags, SaveFlag.Slayer, false);                 // Nie zapisujemy. Te dane beda uzywane dopiero w ServUO
            SetSaveFlag(ref flags, SaveFlag.SAAbsorptionAttributes, false); // Nie zapisujemy. Te dane beda uzywane dopiero w ServUO
            SetSaveFlag(ref flags, SaveFlag.NegativeAttributes, false);     // Nie zapisujemy. Te dane beda uzywane dopiero w ServUO

            writer.WriteEncodedInt((int)flags);

            if (GetSaveFlag(flags, SaveFlag.Skill))
                writer.WriteEncodedInt((int)m_Skill);

            if (GetSaveFlag(flags, SaveFlag.SuccessBonus))
                writer.WriteEncodedInt(m_SuccessBonus);

            if (GetSaveFlag(flags, SaveFlag.ExceptionalBonus))
                writer.WriteEncodedInt(m_ExceptionalBonus);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 4: // version 4 converts SkillName to CraftSystem (thanks glassblowing and stone crafting!)
                case 3:
                    {
                        reader.ReadMobile();    // m_Creature (ServUO)
                        goto case 2;
                    }
                case 2:
                    {
                        reader.ReadBool();      // _VvVItem (ServUO)
                        reader.ReadMobile();    // _Owner (ServUO)
                        reader.ReadString();    // _OwnerName (ServUO)
                        goto case 1;
                    }
                case 1:
                    {
                        m_MaxHitPoints = reader.ReadInt();
                        m_HitPoints = reader.ReadInt();
                    }
                    goto case 0;
                case 0:
                    {
                        SaveFlag flags = (SaveFlag)reader.ReadEncodedInt();

                        if (GetSaveFlag(flags, SaveFlag.Skill))
                        {
                            if (version <= 3)
                            {
                                m_Skill = GetTalismanSkill((SkillName)reader.ReadEncodedInt());
                            }
                            else
                            {
                                m_Skill = (TalismanSkill)reader.ReadEncodedInt();
                            }
                        }

                        if (GetSaveFlag(flags, SaveFlag.SuccessBonus))
                            m_SuccessBonus = reader.ReadEncodedInt();

                        if (GetSaveFlag(flags, SaveFlag.ExceptionalBonus))
                            m_ExceptionalBonus = reader.ReadEncodedInt();

                        break;
                    }
            }
        }

        private static readonly int[] m_ItemIDs = new int[]
        {
            //0x2F58, 0x2F59, 0x2F5A, 0x2F5B // TODO: sprawdzic grafiki talizmanow
            0x2F58
        };

        public static int GetRandomItemID()
        {
            return Utility.RandomList(m_ItemIDs);
        }

        public static TalismanSkill[] Skills { get { return m_Skills; } }
        private static readonly TalismanSkill[] m_Skills = new TalismanSkill[]
        {
            TalismanSkill.Alchemy,
            TalismanSkill.Blacksmithy,
            TalismanSkill.Fletching,
            TalismanSkill.Carpentry,
            TalismanSkill.Cartography,
            TalismanSkill.Cooking,
            TalismanSkill.Glassblowing,
            TalismanSkill.Inscription,
            TalismanSkill.Masonry,
            TalismanSkill.Tailoring,
            TalismanSkill.Tinkering,
        };

        public static TalismanSkill GetRandomSkill()
        {
            return m_Skills[Utility.Random(m_Skills.Length)];
        }

        public static int GetRandomExceptional()
        {
            if (0.6 > Utility.RandomDouble())
            {
                return Utility.RandomMinMax(10, 30);
            }

            return 0;
        }

        public static int GetRandomSuccessful()
        {
            return Utility.RandomMinMax(10, 30);
        }

#region Crafting Bonuses
        /// <summary>
        /// This should only be called for version 4 conversion from SkillName to CraftSystem
        /// </summary>
        /// <param name="skill"></param>
        /// <returns></returns>
        public TalismanSkill GetTalismanSkill(SkillName skill)
        {
            switch (skill)
            {
                default:
                case SkillName.Alchemy: return TalismanSkill.Alchemy;
                case SkillName.Blacksmith: return TalismanSkill.Blacksmithy;
                case SkillName.Carpentry: return TalismanSkill.Carpentry;
                case SkillName.Cartography: return TalismanSkill.Cartography;
                case SkillName.Cooking: return TalismanSkill.Cooking;
                case SkillName.Fletching: return TalismanSkill.Fletching;
                case SkillName.Inscribe: return TalismanSkill.Inscription;
                case SkillName.Tailoring: return TalismanSkill.Tailoring;
                case SkillName.Tinkering: return TalismanSkill.Tinkering;
            }
        }

        public SkillName GetMainSkill()
        {
            switch (m_Skill)
            {
                default:
                case TalismanSkill.Alchemy: return SkillName.Alchemy;
                case TalismanSkill.Blacksmithy: return SkillName.Blacksmith;
                case TalismanSkill.Fletching: return SkillName.Fletching;
                case TalismanSkill.Carpentry: return SkillName.Carpentry;
                case TalismanSkill.Cartography: return SkillName.Cartography;
                case TalismanSkill.Cooking: return SkillName.Cooking;
                case TalismanSkill.Glassblowing: return SkillName.Alchemy;
                case TalismanSkill.Inscription: return SkillName.Inscribe;
                case TalismanSkill.Masonry: return SkillName.Carpentry;
                case TalismanSkill.Tailoring: return SkillName.Tailoring;
                case TalismanSkill.Tinkering: return SkillName.Tinkering;
            }
        }

        public int GetSkillLabel()
        {
            switch (m_Skill)
            {
                case TalismanSkill.Glassblowing: return 1072393;
                case TalismanSkill.Masonry: return 1072392;
                default: return AosSkillBonuses.GetLabel(GetMainSkill());
            }
        }

        public bool CheckSkill(CraftSystem system)
        {
            switch (m_Skill)
            {
                default: return false;
                case TalismanSkill.Alchemy: return DefAlchemy.CraftSystem == system;
                case TalismanSkill.Blacksmithy: return DefBlacksmithy.CraftSystem == system;
                case TalismanSkill.Fletching: return DefBowFletching.CraftSystem == system;
                case TalismanSkill.Carpentry: return DefCarpentry.CraftSystem == system;
                case TalismanSkill.Cartography: return DefCartography.CraftSystem == system;
                case TalismanSkill.Cooking: return DefCooking.CraftSystem == system;
                case TalismanSkill.Glassblowing: return DefGlassblowing.CraftSystem == system;
                case TalismanSkill.Inscription: return DefInscription.CraftSystem == system;
                case TalismanSkill.Masonry: return DefMasonry.CraftSystem == system;
                case TalismanSkill.Tailoring: return DefTailoring.CraftSystem == system;
                case TalismanSkill.Tinkering: return DefTinkering.CraftSystem == system;
            }
        }
        #endregion
    }
}
