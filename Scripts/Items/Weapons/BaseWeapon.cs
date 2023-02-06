using System;
using System.Text;
using System.Collections;
using Server.Network;
using Server.Targeting;
using Server.Mobiles;
using Server.Spells;
using Server.Spells.Necromancy;
using Server.Spells.Bushido;
using Server.Spells.Ninjitsu;
using Server.Factions;
using Server.Engines.Craft;
using System.Collections.Generic;
using Server.Spells.Spellweaving;
using Nelderim.ExtraCraftResource;

namespace Server.Items
{
    public interface ISlayer
    {
        SlayerName Slayer { get; set; }
        SlayerName Slayer2 { get; set; }
    }
        
    public abstract class BaseWeapon : Item, IWeapon, IFactionItem, ICraftable, ISlayer, IDurability, IIdentifiable
    {
        private string m_EngravedText;

        [CommandProperty( AccessLevel.GameMaster )]
        public string EngravedText
        {
            get { return m_EngravedText; }
            set { m_EngravedText = value; InvalidateProperties(); }
        }

        #region Factions
        private FactionItem m_FactionState;

        public FactionItem FactionItemState
        {
            get{ return m_FactionState; }
            set
            {
                m_FactionState = value;

                if ( m_FactionState == null )
                    Hue = CraftResources.GetHue( Resource );

                LootType = ( m_FactionState == null ? LootType.Regular : LootType.Blessed );
            }
        }
        #endregion

        /* Weapon internals work differently now (Mar 13 2003)
         * 
         * The attributes defined below default to -1.
         * If the value is -1, the corresponding virtual 'Aos/Old' property is used.
         * If not, the attribute value itself is used. Here's the list:
         *  - MinDamage
         *  - MaxDamage
         *  - Speed
         *  - HitSound
         *  - MissSound
         *  - StrRequirement, DexRequirement, IntRequirement
         *  - WeaponType
         *  - WeaponAnimation
         *  - MaxRange
         */

        #region Var declarations

        // Instance values. These values are unique to each weapon.
        private WeaponDamageLevel m_DamageLevel;
        private WeaponAccuracyLevel m_AccuracyLevel;
        private WeaponDurabilityLevel m_DurabilityLevel;
        private WeaponQuality m_Quality;
        private Mobile m_Crafter;
        private Poison m_Poison;
        private int m_PoisonCharges;
        private bool m_Identified;
        private IdLevel m_IdentifiedLevel;
        private int m_Hits;
        private int m_MaxHits;
        private SlayerName m_Slayer;
        private SlayerName m_Slayer2;
        private SkillMod m_SkillMod, m_MageMod;
        private CraftResource m_Resource;
        //private CraftResource m_Resource2;
        private bool m_PlayerConstructed;

        private bool m_Cursed; // Is this weapon cursed via Curse Weapon necromancer spell? Temporary; not serialized.
        private bool m_Consecrated; // Is this weapon blessed via Consecrate Weapon paladin ability? Temporary; not serialized.

        private AosAttributes m_AosAttributes;
        private AosWeaponAttributes m_AosWeaponAttributes;
        private AosSkillBonuses m_AosSkillBonuses;
        private AosElementAttributes m_AosElementDamages;

        // Overridable values. These values are provided to override the defaults which get defined in the individual weapon scripts.
        private int m_StrReq, m_DexReq, m_IntReq;
        private int m_MinDamage, m_MaxDamage;
        private int m_HitSound, m_MissSound;
        private int m_Speed;
        private int m_MaxRange;
        private SkillName m_Skill;
        private WeaponType m_Type;
        private WeaponAnimation m_Animation;
        #endregion

        #region Virtual Properties
        public virtual WeaponAbility PrimaryAbility{ get{ return null; } }
        public virtual WeaponAbility SecondaryAbility{ get{ return null; } }

        public virtual int DefMaxRange{ get{ return 1; } }
        public virtual int DefHitSound{ get{ return 0; } }
        public virtual int DefMissSound{ get{ return 0; } }
        public virtual SkillName DefSkill{ get{ return SkillName.Swords; } }
        public virtual WeaponType DefType{ get{ return WeaponType.Slashing; } }
        public virtual WeaponAnimation DefAnimation{ get{ return WeaponAnimation.Slash1H; } }

        public virtual int AosStrengthReq{ get{ return 0; } }
        public virtual int AosDexterityReq{ get{ return 0; } }
        public virtual int AosIntelligenceReq{ get{ return 0; } }
        public virtual int AosMinDamage{ get{ return 0; } }
        public virtual int AosMaxDamage{ get{ return 0; } }
        public virtual int AosSpeed{ get{ return 0; } }
        public virtual int AosMaxRange{ get{ return DefMaxRange; } }
        public virtual int AosHitSound{ get{ return DefHitSound; } }
        public virtual int AosMissSound{ get{ return DefMissSound; } }
        public virtual SkillName AosSkill{ get{ return DefSkill; } }
        public virtual WeaponType AosType{ get{ return DefType; } }
        public virtual WeaponAnimation AosAnimation{ get{ return DefAnimation; } }

        public virtual int OldStrengthReq{ get{ return 0; } }
        public virtual int OldDexterityReq{ get{ return 0; } }
        public virtual int OldIntelligenceReq{ get{ return 0; } }
        public virtual int OldMinDamage{ get{ return 0; } }
        public virtual int OldMaxDamage{ get{ return 0; } }
        public virtual int OldSpeed{ get{ return 0; } }
        public virtual int OldMaxRange{ get{ return DefMaxRange; } }
        public virtual int OldHitSound{ get{ return DefHitSound; } }
        public virtual int OldMissSound{ get{ return DefMissSound; } }
        public virtual SkillName OldSkill{ get{ return DefSkill; } }
        public virtual WeaponType OldType{ get{ return DefType; } }
        public virtual WeaponAnimation OldAnimation{ get{ return DefAnimation; } }

        public virtual int InitMinHits{ get{ return 0; } }
        public virtual int InitMaxHits{ get{ return 0; } }

        public virtual bool CanFortify{ get{ return true; } }

        public override int PhysicalResistance{ get{ return m_AosWeaponAttributes.ResistPhysicalBonusId(m_Identified); } }
        public override int FireResistance{ get{ return m_AosWeaponAttributes.ResistFireBonusId(m_Identified); } }
        public override int ColdResistance{ get{ return m_AosWeaponAttributes.ResistColdBonusId(m_Identified); } }
        public override int PoisonResistance{ get{ return m_AosWeaponAttributes.ResistPoisonBonusId(m_Identified); } }
        public override int EnergyResistance{ get{ return m_AosWeaponAttributes.ResistEnergyBonusId(m_Identified); } }
/*
        public override int PhysicalResistance{ get{ return m_AosWeaponAttributes.ResistPhysicalBonus; } }
        public override int FireResistance{ get{ return m_AosWeaponAttributes.ResistFireBonus; } }
        public override int ColdResistance{ get{ return m_AosWeaponAttributes.ResistColdBonus; } }
        public override int PoisonResistance{ get{ return m_AosWeaponAttributes.ResistPoisonBonus; } }
        public override int EnergyResistance{ get{ return m_AosWeaponAttributes.ResistEnergyBonus; } }
*/
        public virtual SkillName AccuracySkill { get { return SkillName.Tactics; } }
        #endregion

        #region Getters & Setters
        [CommandProperty( AccessLevel.GameMaster )]
        public AosAttributes Attributes
        {
            get{ return m_AosAttributes; }
            set{}
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public AosWeaponAttributes WeaponAttributes
        {
            get{ return m_AosWeaponAttributes; }
            set{}
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public AosSkillBonuses SkillBonuses
        {
            get{ return m_AosSkillBonuses; }
            set{}
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public AosElementAttributes AosElementDamages
        {
            get { return m_AosElementDamages; }
            set { }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool Cursed
        {
            get{ return m_Cursed; }
            set{ m_Cursed = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool Consecrated
        {
            get{ return m_Consecrated; }
            set{ m_Consecrated = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool Identified
        {
            get{ return m_Identified; }
            set{UnscaleDurability(); m_Identified = value; ScaleDurability(); InvalidateProperties(); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public IdLevel IdentifyLevel
        {
            get{ return m_IdentifiedLevel; }
            set{ m_IdentifiedLevel = value; InvalidateProperties(); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int HitPoints
        {
            get{ return m_Hits; }
            set
            {
                if ( m_Hits == value )
                    return;

                if ( value > m_MaxHits )
                    value = m_MaxHits;

                m_Hits = value;

                InvalidateProperties();
            }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int MaxHitPoints
        {
            get{ return m_MaxHits; }
            set{ m_MaxHits = value; InvalidateProperties(); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int PoisonCharges
        {
            get{ return m_PoisonCharges; }
            set{ m_PoisonCharges = value; InvalidateProperties(); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public Poison Poison
        {
            get{ return m_Poison; }
            set{ m_Poison = value; InvalidateProperties(); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public WeaponQuality Quality
        {
            get{ return m_Quality; }
            set{ UnscaleDurability(); m_Quality = value; ScaleDurability(); InvalidateProperties(); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public Mobile Crafter
        {
            get{ return m_Crafter; }
            set{ m_Crafter = value; InvalidateProperties(); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public SlayerName Slayer
        {
            get{ return (m_Identified) ? m_Slayer : SlayerName.None ; }
            set{ m_Slayer = value; InvalidateProperties(); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public SlayerName Slayer2
        {
            get { return (m_Identified) ? m_Slayer2 : SlayerName.None; }
            set { m_Slayer2 = value; InvalidateProperties(); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public CraftResource Resource
        {
            get{ return m_Resource; }
            set{ UnscaleDurability(); m_Resource = value; Hue = CraftResources.GetHue( m_Resource ); InvalidateProperties(); ScaleDurability(); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public CraftResource Resource2
        {
            get{ return ExtraCraftResource.Get(this).Resource2; }
            set{ UnscaleDurability(); ExtraCraftResource.Get( this ).Resource2 = value; /*Hue = CraftResources.GetHue( m_Resource );*/ InvalidateProperties(); ScaleDurability(); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public WeaponDamageLevel DamageLevel
        {
            get{ return m_DamageLevel; }
            set{ m_DamageLevel = value; InvalidateProperties(); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public WeaponDurabilityLevel DurabilityLevel
        {
            get{ return m_DurabilityLevel; }
            set{ UnscaleDurability(); m_DurabilityLevel = value; InvalidateProperties(); ScaleDurability(); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public bool PlayerConstructed
        {
            get{ return m_PlayerConstructed; }
            set{ m_PlayerConstructed = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int MaxRange
        {
            get{ return ( m_MaxRange == -1 ? Core.AOS ? AosMaxRange : OldMaxRange : m_MaxRange ); }
            set{ m_MaxRange = value; InvalidateProperties(); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public WeaponAnimation Animation
        {
            get{ return ( m_Animation == (WeaponAnimation)(-1) ? Core.AOS ? AosAnimation : OldAnimation : m_Animation ); } 
            set{ m_Animation = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public WeaponType Type
        {
            get{ return ( m_Type == (WeaponType)(-1) ? Core.AOS ? AosType : OldType : m_Type ); }
            set{ m_Type = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public SkillName Skill
        {
            get{ return ( m_Skill == (SkillName)(-1) ? Core.AOS ? AosSkill : OldSkill : m_Skill ); }
            set{ m_Skill = value; InvalidateProperties(); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int HitSound
        {
            get{ return ( m_HitSound == -1 ? Core.AOS ? AosHitSound : OldHitSound : m_HitSound ); }
            set{ m_HitSound = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int MissSound
        {
            get{ return ( m_MissSound == -1 ? Core.AOS ? AosMissSound : OldMissSound : m_MissSound ); }
            set{ m_MissSound = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int MinDamage
        {
            get{ return ( m_MinDamage == -1 ? Core.AOS ? AosMinDamage : OldMinDamage : m_MinDamage ); }
            set{ m_MinDamage = value; InvalidateProperties(); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int MaxDamage
        {
            get{ return ( m_MaxDamage == -1 ? Core.AOS ? AosMaxDamage : OldMaxDamage : m_MaxDamage ); }
            set{ m_MaxDamage = value; InvalidateProperties(); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int Speed
        {
            get{ return ( m_Speed == -1 ? Core.AOS ? AosSpeed : OldSpeed : m_Speed ); }
            set{ m_Speed = value; InvalidateProperties(); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int StrRequirement
        {
            get{ return ( m_StrReq == -1 ? Core.AOS ? AosStrengthReq : OldStrengthReq : m_StrReq ); }
            set{ m_StrReq = value; InvalidateProperties(); }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int DexRequirement
        {
            get{ return ( m_DexReq == -1 ? Core.AOS ? AosDexterityReq : OldDexterityReq : m_DexReq ); }
            set{ m_DexReq = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public int IntRequirement
        {
            get{ return ( m_IntReq == -1 ? Core.AOS ? AosIntelligenceReq : OldIntelligenceReq : m_IntReq ); }
            set{ m_IntReq = value; }
        }

        [CommandProperty( AccessLevel.GameMaster )]
        public WeaponAccuracyLevel AccuracyLevel
        {
            get
            {
                return m_AccuracyLevel;
            }
            set
            {
                if ( m_AccuracyLevel != value )
                {
                    m_AccuracyLevel = value;

                    if ( UseSkillMod )
                    {
                        if ( m_AccuracyLevel == WeaponAccuracyLevel.Regular )
                        {
                            if ( m_SkillMod != null )
                                m_SkillMod.Remove();

                            m_SkillMod = null;
                        }
                        else if ( m_SkillMod == null && Parent is Mobile )
                        {
                            m_SkillMod = new DefaultSkillMod( AccuracySkill, true, (int)m_AccuracyLevel * 5 );
                            ((Mobile)Parent).AddSkillMod( m_SkillMod );
                        }
                        else if ( m_SkillMod != null )
                        {
                            m_SkillMod.Value = (int)m_AccuracyLevel * 5;
                        }
                    }

                    InvalidateProperties();
                }
            }
        }

        // 22.09.2012 :: zombie :: zmiany w wytrzymalosci broni
        [CommandProperty( AccessLevel.GameMaster )]
        public int InitHitPoints
        {
            get { return 40; }
        }
        // zombie

        #endregion

        public override void OnAfterDuped( Item newItem )
        {
            BaseWeapon weap = newItem as BaseWeapon;

            if ( weap == null )
                return;

            weap.m_AosAttributes = new AosAttributes( newItem, m_AosAttributes );
            weap.m_AosElementDamages = new AosElementAttributes( newItem, m_AosElementDamages );
            weap.m_AosSkillBonuses = new AosSkillBonuses( newItem, m_AosSkillBonuses );
            weap.m_AosWeaponAttributes = new AosWeaponAttributes( newItem, m_AosWeaponAttributes );
        }

        public virtual void UnscaleDurability()
        {
            int scale = 100 + GetDurabilityBonus();

            m_Hits = ((m_Hits * 100) + (scale - 1)) / scale;
            m_MaxHits = ((m_MaxHits * 100) + (scale - 1)) / scale;
            InvalidateProperties();
        }

        public virtual void ScaleDurability()
        {
            int scale = 100 + GetDurabilityBonus();

            m_Hits = ((m_Hits * scale) + 99) / 100;
            m_MaxHits = ((m_MaxHits * scale) + 99) / 100;
            InvalidateProperties();
        }

        public int GetDurabilityBonus()
        {
            int bonus = 0;

            if ( m_Quality == WeaponQuality.Exceptional )
                bonus += 20;

            switch ( m_DurabilityLevel )
            {
                case WeaponDurabilityLevel.Durable: bonus += 20; break;
                case WeaponDurabilityLevel.Substantial: bonus += 50; break;
                case WeaponDurabilityLevel.Massive: bonus += 70; break;
                case WeaponDurabilityLevel.Fortified: bonus += 100; break;
                case WeaponDurabilityLevel.Indestructible: bonus += 120; break;
            }

            if ( Core.AOS )
            {
                bonus += m_AosWeaponAttributes.DurabilityBonusId(m_Identified);

                CraftResourceInfo resInfo = CraftResources.GetInfo( m_Resource );
                CraftAttributeInfo attrInfo = null;

                if ( resInfo != null )
                    attrInfo = resInfo.AttributeInfo;

                if ( attrInfo != null )
                    bonus += attrInfo.WeaponDurability;

                CraftResourceInfo resInfo2 = CraftResources.GetInfo( Resource2 );
                CraftAttributeInfo attrInfo2 = null;

                if ( resInfo2 != null )
                    attrInfo2 = resInfo2.AttributeInfo;

                if ( attrInfo2 != null )
                    bonus += attrInfo2.WeaponDurability;
            }

            return bonus;
        }

        public int GetLowerStatReqId(bool identified)
        {
            if ( !Core.AOS )
                return 0;

            int v = m_AosWeaponAttributes.LowerStatReqId(identified);

            CraftResourceInfo info = CraftResources.GetInfo( m_Resource );

            if ( info != null )
            {
                CraftAttributeInfo attrInfo = info.AttributeInfo;

                if ( attrInfo != null )
                    v += attrInfo.WeaponLowerRequirements;
            }

            CraftResourceInfo info2 = CraftResources.GetInfo( Resource2 );

            if ( info2 != null )
            {
                CraftAttributeInfo attrInfo2 = info2.AttributeInfo;

                if ( attrInfo2 != null )
                    v += attrInfo2.WeaponLowerRequirements;
            }

            if ( v > 100 )
                v = 100;

            return v;
        }
        
        public int GetLowerStatReq()
        {
            return GetLowerStatReqId(m_Identified);
        }

        public static void BlockEquip( Mobile m, TimeSpan duration )
        {
            if ( m.BeginAction( typeof( BaseWeapon ) ) )
                new ResetEquipTimer( m, duration ).Start();
        }

        private class ResetEquipTimer : Timer
        {
            private Mobile m_Mobile;

            public ResetEquipTimer( Mobile m, TimeSpan duration ) : base( duration )
            {
                m_Mobile = m;
            }

            protected override void OnTick()
            {
                m_Mobile.EndAction( typeof( BaseWeapon ) );
            }
        }

        public override bool CheckConflictingLayer( Mobile m, Item item, Layer layer )
        {
            if ( base.CheckConflictingLayer( m, item, layer ) )
                return true;

            if ( this.Layer == Layer.TwoHanded && layer == Layer.OneHanded )
            {
                m.SendLocalizedMessage( 500214 ); // You already have something in both hands.
                return true;
            }
            else if ( this.Layer == Layer.OneHanded && layer == Layer.TwoHanded && !(item is BaseShield) && !(item is BaseEquipableLight) )
            {
                m.SendLocalizedMessage( 500215 ); // You can only wield one weapon at a time.
                return true;
            }

            return false;
        }

        public override bool AllowSecureTrade( Mobile from, Mobile to, Mobile newOwner, bool accepted )
        {
            if ( !Ethics.Ethic.CheckTrade( from, to, newOwner, this ) )
                return false;

            return base.AllowSecureTrade( from, to, newOwner, accepted );
        }

        public virtual Race RequiredRace { get { return null; } }    //On OSI, there are no weapons with race requirements, this is for custom stuff

        public override bool CanEquip( Mobile from )
        {
            if ( !Ethics.Ethic.CheckEquip( from, this ) )
                return false;

            if( RequiredRace != null && from.Race != RequiredRace )
            {
                /*if( RequiredRace == Race.Elf )
                    from.SendLocalizedMessage( 1072203 ); // Only Elves may use this.
                else*/
                    from.SendMessage( "Only {0} may use this.", RequiredRace.PluralName );

                return false;
            }
            else if ( from.Dex < DexRequirement )
            {
                from.SendMessage( "You are not nimble enough to equip that." );
                return false;
            } 
            else if ( from.Str < AOS.Scale( StrRequirement, 100 - GetLowerStatReq() ) )
            {
                from.SendLocalizedMessage( 500213 ); // You are not strong enough to equip that.
                return false;
            }
            else if ( from.Int < IntRequirement )
            {
                from.SendMessage( "You are not smart enough to equip that." );
                return false;
            }
            else if ( !from.CanBeginAction( typeof( BaseWeapon ) ) )
            {
                return false;
            }
            else
            {
                // XmlAttachment check for CanEquip
                if (!Server.Engines.XmlSpawner2.XmlAttach.CheckCanEquip(this, from))
                {
                    return false;
                }
                else
                {
                    return base.CanEquip(from);
                }
            }
        }

        public virtual bool UseSkillMod{ get{ return !Core.AOS; } }

        public override bool OnEquip( Mobile from )
        {
            int strBonus = m_AosAttributes.BonusStrId(m_Identified);
            int dexBonus = m_AosAttributes.BonusDexId(m_Identified);
            int intBonus = m_AosAttributes.BonusIntId(m_Identified);

            if ( (strBonus != 0 || dexBonus != 0 || intBonus != 0) )
            {
                Mobile m = from;

                string modName = this.Serial.ToString();

                if ( strBonus != 0 )
                    m.AddStatMod( new StatMod( StatType.Str, modName + "Str", strBonus, TimeSpan.Zero ) );

                if ( dexBonus != 0 )
                    m.AddStatMod( new StatMod( StatType.Dex, modName + "Dex", dexBonus, TimeSpan.Zero ) );

                if ( intBonus != 0 )
                    m.AddStatMod( new StatMod( StatType.Int, modName + "Int", intBonus, TimeSpan.Zero ) );
            }

            from.NextCombatTime = DateTime.Now + GetDelay( from );

            if ( UseSkillMod && m_AccuracyLevel != WeaponAccuracyLevel.Regular )
            {
                if ( m_SkillMod != null )
                    m_SkillMod.Remove();

                m_SkillMod = new DefaultSkillMod( AccuracySkill, true, (int)m_AccuracyLevel * 5 );
                from.AddSkillMod( m_SkillMod );
            }

            if ( Core.AOS && m_AosWeaponAttributes.MageWeaponId(m_Identified) != 0 && m_AosWeaponAttributes.MageWeaponId(m_Identified) != 30 )
            {
                if ( m_MageMod != null )
                    m_MageMod.Remove();

                m_MageMod = new DefaultSkillMod( SkillName.Magery, true, -30 + m_AosWeaponAttributes.MageWeaponId(m_Identified) );
                from.AddSkillMod( m_MageMod );
            }

            // XmlAttachment check for OnEquip
            Server.Engines.XmlSpawner2.XmlAttach.CheckOnEquip(this, from);

            return true;
        }

        public override void OnAdded( object parent )
        {
            base.OnAdded( parent );

            if ( parent is Mobile )
            {
                Mobile from = (Mobile)parent;

                if ( Core.AOS )
                    m_AosSkillBonuses.AddTo( from, m_Identified );

                from.CheckStatTimers();
                from.Delta( MobileDelta.WeaponDamage );
            }
        }

        public override void OnRemoved( object parent )
        {
            if ( parent is Mobile )
            {
                Mobile m = (Mobile)parent;
                BaseWeapon weapon = m.Weapon as BaseWeapon;

                string modName = this.Serial.ToString();

                m.RemoveStatMod( modName + "Str" );
                m.RemoveStatMod( modName + "Dex" );
                m.RemoveStatMod( modName + "Int" );

                if ( weapon != null )
                    m.NextCombatTime = DateTime.Now + weapon.GetDelay( m );

                if ( UseSkillMod && m_SkillMod != null )
                {
                    m_SkillMod.Remove();
                    m_SkillMod = null;
                }

                if ( m_MageMod != null )
                {
                    m_MageMod.Remove();
                    m_MageMod = null;
                }

                if ( Core.AOS )
                    m_AosSkillBonuses.Remove();

                ImmolatingWeaponSpell.StopImmolating(this, (Mobile)parent);

                m.CheckStatTimers();

                m.Delta( MobileDelta.WeaponDamage );
            }
            
            // XmlAttachment check for OnRemoved
            Server.Engines.XmlSpawner2.XmlAttach.CheckOnRemoved(this, parent);
        }

        public virtual SkillName GetUsedSkill( Mobile m, bool checkSkillAttrs )
        {
            SkillName sk;

            if ( checkSkillAttrs && m_AosWeaponAttributes.UseBestSkillId(m_Identified) != 0 )
            {
                double swrd = m.Skills[SkillName.Swords].Value;
                double fenc = m.Skills[SkillName.Fencing].Value;
                double mcng = m.Skills[SkillName.Macing].Value;
                double val;

                sk = SkillName.Swords;
                val = swrd;

                if ( fenc > val ){ sk = SkillName.Fencing; val = fenc; }
                if ( mcng > val ){ sk = SkillName.Macing; val = mcng; }
            }
            else if ( m_AosWeaponAttributes.MageWeaponId(m_Identified) != 0 )
            {
                if ( m.Skills[SkillName.Magery].Value > m.Skills[Skill].Value )
                    sk = SkillName.Magery;
                else
                    sk = Skill;
            }
            else
            {
                sk = Skill;

                if ( sk != SkillName.Wrestling && !m.Player && !m.Body.IsHuman && m.Skills[SkillName.Wrestling].Value > m.Skills[sk].Value )
                    sk = SkillName.Wrestling;
            }

            return sk;
        }

        public virtual double GetAttackSkillValue( Mobile attacker, Mobile defender )
        {
            return attacker.Skills[GetUsedSkill( attacker, true )].Value;
        }

        public virtual double GetDefendSkillValue( Mobile attacker, Mobile defender )
        {
            return defender.Skills[GetUsedSkill( defender, true )].Value;
        }

        private static bool CheckAnimal( Mobile m, Type type )
        {
            return AnimalForm.UnderTransformation( m, type );
        }

		// 08.09.2013 :: mortuus - Liczenie szansy na trafienie przeniesiono do osobnej funkcji statycznej - do uzytku wszedzie.
		public static double HitChance(double atkValue, double defValue, Mobile attacker, Mobile defender)
		{
            BaseWeapon atkWeapon = attacker.Weapon as BaseWeapon;
            BaseWeapon defWeapon = defender.Weapon as BaseWeapon;

			double ourValue, theirValue;

            int bonus = atkWeapon.GetHitChanceBonus();

            if ( Core.AOS )
            {
                if ( atkValue <= -20.0 )
                    atkValue = -19.9;

                if ( defValue <= -20.0 )
                    defValue = -19.9;

                // Hit Chance Increase = 45%
                int atkChance = AosAttributes.GetValue( attacker, AosAttribute.AttackChance );
                if ( atkChance > 45 )
                    atkChance = 45;

                bonus += atkChance;

                if ( Spells.Chivalry.DivineFurySpell.UnderEffect( attacker ) )
                    bonus += 10; // attacker gets 10% bonus when they're under divine fury

                if ( CheckAnimal( attacker, typeof( GreyWolf ) ) || CheckAnimal( attacker, typeof( BakeKitsune ) ) )
                    bonus += 20; // attacker gets 20% bonus when under Wolf or Bake Kitsune form

                if ( HitLower.IsUnderAttackEffect( attacker ) )
                    bonus -= 25; // Under Hit Lower Attack effect -> 25% malus

                WeaponAbility ability = WeaponAbility.GetCurrentAbility( attacker );

                if ( ability != null )
                    bonus += ability.AccuracyBonus;

                SpecialMove move = SpecialMove.GetCurrentMove( attacker );

                if ( move != null )
                    bonus += move.GetAccuracyBonus( attacker );

                // Max Hit Chance Increase = 45%
                if ( bonus > 45 )
                    bonus = 45;

                ourValue = (atkValue + 20.0) * (100 + bonus);

                // Defense Chance Increase = 45%
                bonus = AosAttributes.GetValue( defender, AosAttribute.DefendChance );
                if ( bonus > 45 )
                    bonus = 45;

                if ( Spells.Chivalry.DivineFurySpell.UnderEffect( defender ) )
                    bonus -= 20; // defender loses 20% bonus when they're under divine fury

                if ( HitLower.IsUnderDefenseEffect( defender ) )
                    bonus -= 25; // Under Hit Lower Defense effect -> 25% malus

                ForceArrow.ForceArrowInfo info = ForceArrow.GetInfo(attacker, defender);

                if (info != null && info.Defender == defender)
                    bonus -= info.DefenseChanceMalus;

                int blockBonus = 0;

                if ( Block.GetBonus( defender, ref blockBonus ) )
                    bonus += blockBonus;

                int surpriseMalus = 0;

                if ( SurpriseAttack.GetMalus( defender, ref surpriseMalus ) )
                    bonus -= surpriseMalus;

                int discordanceEffect = 0;

                // Defender loses -0/-28% if under the effect of Discordance.
                if ( SkillHandlers.Discordance.GetEffect( attacker, ref discordanceEffect ) )
                    bonus -= discordanceEffect;

                // Defense Chance Increase = 45%
                if ( bonus > 45 )
                    bonus = 45;

                theirValue = (defValue + 20.0) * (100 + bonus);

                bonus = 0;
            }
            else
            {
                if ( atkValue <= -50.0 )
                    atkValue = -49.9;

                if ( defValue <= -50.0 )
                    defValue = -49.9;

                ourValue = (atkValue + 50.0);
                theirValue = (defValue + 50.0);
            }

            double chance = ourValue / (theirValue * 2.0);

            chance *= 1.0 + ((double)bonus / 100);

            if ( Core.AOS && chance < 0.02 )
                chance = 0.02;

			return chance;
		}

        public virtual bool CheckHit( Mobile attacker, Mobile defender )
        {
            BaseWeapon atkWeapon = attacker.Weapon as BaseWeapon;
            BaseWeapon defWeapon = defender.Weapon as BaseWeapon;

            Skill atkSkill = attacker.Skills[atkWeapon.Skill];
            Skill defSkill = defender.Skills[defWeapon.Skill];

            double atkValue = atkWeapon.GetAttackSkillValue( attacker, defender );
            double defValue = defWeapon.GetDefendSkillValue( attacker, defender );
			// 09.09.2013 :: mortuus - liczenie szansy na trafienie przeniesione do HitChance()
			double chance = HitChance(atkValue, defValue, attacker, defender);

            return attacker.CheckSkill( atkSkill.SkillName, chance );

            //return ( chance >= Utility.RandomDouble() );
        }

        public virtual TimeSpan GetDelay( Mobile m )
        {
            int speed = this.Speed;

            if ( speed == 0 )
                return TimeSpan.FromHours( 1.0 );

            double delayInSeconds;

            if ( Core.SE )
            {
                /*
                 * This is likely true for Core.AOS as well... both guides report the same
                 * formula, and both are wrong.
                 * The old formula left in for AOS for legacy & because we aren't quite 100%
                 * Sure that AOS has THIS formula
                 */
                int bonus = AosAttributes.GetValue( m, AosAttribute.WeaponSpeed );

                if ( Spells.Chivalry.DivineFurySpell.UnderEffect( m ) )
                    bonus += 10;

                // Bonus granted by successful use of Honorable Execution.
                bonus += HonorableExecution.GetSwingBonus( m );

                if( DualWield.Registry.Contains( m ) )
                    bonus += ((DualWield.DualWieldTimer)DualWield.Registry[m]).BonusSwingSpeed;

                TransformContext context = TransformationSpellHelper.GetContext( m );

                if( context != null && context.Spell is ReaperFormSpell )
                    bonus += ((ReaperFormSpell)context.Spell).SwingSpeedBonus;

                int discordanceEffect = 0;

                // Discordance gives a malus of -0/-28% to swing speed.
                if ( SkillHandlers.Discordance.GetEffect( m, ref discordanceEffect ) )
                    bonus -= discordanceEffect;

                if( EssenceOfWindSpell.IsDebuffed( m ) )
                    bonus -= EssenceOfWindSpell.GetSSIMalus( m );

                if ( bonus > 60 )
                    bonus = 60;

                speed = (int)Math.Floor( speed * (bonus + 100.0) / 100.0 );

                if ( speed <= 0 )
                    speed = 1;

                int ticks = (int)Math.Floor( (80000.0 / ((m.Stam + 100) * speed)) - 2 );

                // Swing speed currently capped at one swing every 1.25 seconds (5 ticks).
                if ( ticks < 5 )
                    ticks = 5;

                delayInSeconds = ticks * 0.25;
            }
            else if ( Core.AOS )
            {
                int v = (m.Stam + 100) * speed;

                int bonus = AosAttributes.GetValue( m, AosAttribute.WeaponSpeed );

                if ( Spells.Chivalry.DivineFurySpell.UnderEffect( m ) )
                    bonus += 10;

                int discordanceEffect = 0;

                // Discordance gives a malus of -0/-28% to swing speed.
                if ( SkillHandlers.Discordance.GetEffect( m, ref discordanceEffect ) )
                    bonus -= discordanceEffect;

                v += AOS.Scale( v, bonus );

                if ( v <= 0 )
                    v = 1;

                delayInSeconds = Math.Floor( 40000.0 / v ) * 0.5;

                // Maximum swing rate capped at one swing per second 
                // OSI dev said that it has and is supposed to be 1.25
                if ( delayInSeconds < 1.25 )
                    delayInSeconds = 1.25;
            }
            else
            {
                int v = (m.Stam + 100) * speed;

                if ( v <= 0 )
                    v = 1;

                delayInSeconds = 15000.0 / v;
            }

            return TimeSpan.FromSeconds( delayInSeconds );
        }

        public virtual void OnBeforeSwing( Mobile attacker, Mobile defender )
        {
            WeaponAbility a = WeaponAbility.GetCurrentAbility( attacker );

            if( a != null && !a.OnBeforeSwing( attacker, defender ) )
                WeaponAbility.ClearCurrentAbility( attacker );

            SpecialMove move = SpecialMove.GetCurrentMove( attacker );

            if( move != null && !move.OnBeforeSwing( attacker, defender ) )
                SpecialMove.ClearCurrentMove( attacker );
        }

        public virtual TimeSpan OnSwing( Mobile attacker, Mobile defender )
        {
            return OnSwing( attacker, defender, 1.0 );
        }

        public virtual TimeSpan OnSwing( Mobile attacker, Mobile defender, double damageBonus )
        {
            bool canSwing = true;

            if ( Core.AOS )
            {
                canSwing = ( !attacker.Paralyzed && !attacker.Frozen );

                if ( canSwing )
                {
                    Spell sp = attacker.Spell as Spell;

                    canSwing = ( sp == null || !sp.IsCasting || !sp.BlocksMovement );
                }
            }

            if ( canSwing && attacker.HarmfulCheck( defender ) )
            {
                attacker.DisruptiveAction();

                if ( attacker.NetState != null )
                    attacker.Send( new Swing( 0, attacker, defender ) );

                if ( attacker is BaseCreature )
                {
                    BaseCreature bc = (BaseCreature)attacker;
                    WeaponAbility ab = bc.GetWeaponAbility();

                    if ( ab != null )
                    {
                        // 16.07.2012 :: zombie :: sprawdzanie szansy przeniesione do BaseCreature -> GetWeaponAbility()
                        //if ( bc.WeaponAbilityChance > Utility.RandomDouble() )
                            WeaponAbility.SetCurrentAbility( bc, ab );
                        //else
                        //WeaponAbility.ClearCurrentAbility( bc );
                        // zombie
                    }
                }

                if ( CheckHit( attacker, defender ) )
                    OnHit( attacker, defender, damageBonus );
                else
                    OnMiss( attacker, defender );
            }

            return GetDelay( attacker );
        }

        #region Sounds
        public virtual int GetHitAttackSound( Mobile attacker, Mobile defender )
        {
            int sound = attacker.GetAttackSound();

            if ( sound == -1 )
                sound = HitSound;

            return sound;
        }

        public virtual int GetHitDefendSound( Mobile attacker, Mobile defender )
        {
            return defender.GetHurtSound();
        }

        public virtual int GetMissAttackSound( Mobile attacker, Mobile defender )
        {
            if ( attacker.GetAttackSound() == -1 )
                return MissSound;
            else
                return -1;
        }

        public virtual int GetMissDefendSound( Mobile attacker, Mobile defender )
        {
            return -1;
        }
        #endregion

        // 12.11.2012 :: zombie :: przeniesione z RunUO 2.2
        public static bool CheckParry( Mobile defender )
        {
            if ( defender == null )
                return false;

            BaseShield shield = defender.FindItemOnLayer( Layer.TwoHanded ) as BaseShield;

            double parry = defender.Skills[SkillName.Parry].Value;
            double bushidoNonRacial = defender.Skills[SkillName.Bushido].NonRacialValue;
            double bushido = defender.Skills[SkillName.Bushido].Value;

            if ( shield != null )
            {
                double chance = (parry - bushidoNonRacial) / 400.0;    // As per OSI, no negitive effect from the Racial stuffs, ie, 120 parry and '0' bushido with humans

                if ( chance < 0 ) // chance shouldn't go below 0
                    chance = 0;                

                // Parry/Bushido over 100 grants a 5% bonus.
                if ( parry >= 100.0 || bushido >= 100.0)
                    chance += 0.05;

                // Evasion grants a variable bonus post ML. 50% prior.
                if ( Evasion.IsEvading( defender ) )
                    chance *= Evasion.GetParryScalar( defender );

                // Low dexterity lowers the chance.
                if ( defender.Dex < 80 )
                    chance = chance * (20 + defender.Dex) / 100;

                return defender.CheckSkill( SkillName.Parry, chance );
            }
            else if ( !(defender.Weapon is Fists) && !(defender.Weapon is BaseRanged) )
            {
                BaseWeapon weapon = defender.Weapon as BaseWeapon;

                double divisor = (weapon.Layer == Layer.OneHanded) ? 48000.0 : 41140.0;

                double chance = (parry * bushido) / divisor;

                double aosChance = parry / 800.0;

                // Parry or Bushido over 100 grant a 5% bonus.
                if( parry >= 100.0 )
                {
                    chance += 0.05;
                    aosChance += 0.05;
                }
                else if( bushido >= 100.0 )
                {
                    chance += 0.05;
                }

                // Evasion grants a variable bonus post ML. 50% prior.
                if( Evasion.IsEvading( defender ) )
                    chance *= Evasion.GetParryScalar( defender );

                // Low dexterity lowers the chance.
                if( defender.Dex < 80 )
                    chance = chance * (20 + defender.Dex) / 100;

                if ( chance > aosChance )
                    return defender.CheckSkill( SkillName.Parry, chance );
                else
                    return (aosChance > Utility.RandomDouble()); // Only skillcheck if wielding a shield & there's no effect from Bushido
            }

            return false;
        }
        // zombie

        // 12.11.2012 :: zombie :: przeniosione z RunUO 2.2
        public virtual int AbsorbDamageAOS( Mobile attacker, Mobile defender, int damage )
        {
            bool blocked = false;
            int originaldamage = damage;

            if ( defender.Player || defender.Body.IsHuman )
            {
                blocked = CheckParry( defender );

                if ( blocked )
                {
                    defender.FixedEffect( 0x37B9, 10, 16 );
                    damage = 0;

                    // Successful block removes the Honorable Execution penalty.
                    HonorableExecution.RemovePenalty( defender );

                    if ( CounterAttack.IsCountering( defender ) )
                    {
                        BaseWeapon weapon = defender.Weapon as BaseWeapon;

                        if ( weapon != null )
                        {
                            defender.FixedParticles(0x3779, 1, 15, 0x158B, 0x0, 0x3, EffectLayer.Waist);
                            weapon.OnSwing( defender, attacker );
                        }

                        CounterAttack.StopCountering( defender );
                    }

                    if ( Confidence.IsConfident( defender ) )
                    {
                        defender.SendLocalizedMessage( 1063117 ); // Your confidence reassures you as you successfully block your opponent's blow.

                        double bushido = defender.Skills.Bushido.Value;

                        defender.Hits += Utility.RandomMinMax( 1, (int)(bushido / 12) );
                        defender.Stam += Utility.RandomMinMax( 1, (int)(bushido / 5) );
                    }

                    BaseShield shield = defender.FindItemOnLayer( Layer.TwoHanded ) as BaseShield;

                    if ( shield != null )
                    {
                        shield.OnHit( this, damage );

                        // XmlAttachment check for OnArmorHit
                        Server.Engines.XmlSpawner2.XmlAttach.OnArmorHit(attacker, defender, shield, this, originaldamage);
                    }
                }
            }
            // zombie

            if ( !blocked )
            {
                // 23.09.2012 :: zombie :: dodanie spellbooka, jeweli i ubran, modyfikacja %
                List<Item> armorItems = new List<Item>();

                double positionChance = Utility.RandomDouble();
                
                if ( positionChance < 0.05 )
                {
                    Item armorItem = defender.NeckArmor;

                    if ( !( armorItem is Necklace ) )
                        armorItems.Add( armorItem );
                }
                else if ( positionChance < 0.10 )
                    armorItems.Add( defender.HandArmor );
                else if ( positionChance < 0.22 )
                    armorItems.Add( defender.ArmsArmor );
                else if ( positionChance < 0.35 )
                    armorItems.Add( defender.HeadArmor );
                else if ( positionChance < 0.55 )
                {
                    armorItems.Add( defender.LegsArmor );
                    armorItems.Add( defender.FindItemOnLayer( Layer.OuterLegs ) );
                    armorItems.Add( defender.FindItemOnLayer( Layer.Shoes ) );
                }
                else if ( positionChance < 0.57 )
                    armorItems.Add( defender.FindItemOnLayer( Layer.Ring ) );
                else if ( positionChance < 0.59 )
                   armorItems.Add( defender.FindItemOnLayer( Layer.Bracelet ) );
                else if ( positionChance < 0.61 )
                    armorItems.Add( defender.FindItemOnLayer( Layer.Earrings ) );
                else if ( positionChance < 0.63 )
                {
                    Item armorItem = defender.FindItemOnLayer( Layer.Neck );

                    if ( armorItem is Necklace )
                        armorItems.Add( armorItem );
                }
                else if ( positionChance < 0.65 )
                    armorItems.Add( defender.FindItemOnLayer( Layer.Talisman ) );
                else if ( positionChance < 0.72 )
                {
                    Item armorItem = defender.FindItemOnLayer( Layer.OneHanded );

                    if ( armorItem is Spellbook )
                        armorItems.Add( armorItem );
                }
                else
                {
                    armorItems.Add( defender.ChestArmor );
                    armorItems.Add( defender.FindItemOnLayer( Layer.Cloak ) );
                    armorItems.Add( defender.FindItemOnLayer( Layer.OuterTorso ) );
                    armorItems.Add( defender.FindItemOnLayer( Layer.MiddleTorso) );
                    armorItems.Add( defender.FindItemOnLayer( Layer.Waist) );
                }

                foreach ( Item item in armorItems )
                {
                    if ( item == null )
                        continue;

                    IWearableDurability armor = item as IWearableDurability;

                    if ( armor != null )
                    {
                        armor.OnHit( this, damage ); // call OnHit to lose durability

                        // XmlAttachment check for OnArmorHit
                        damage -= Server.Engines.XmlSpawner2.XmlAttach.OnArmorHit( attacker, defender, item, this, originaldamage );
                    }
                }
                // zombie
            }

            return damage;
        }

        public virtual int AbsorbDamage( Mobile attacker, Mobile defender, int damage )
        {
            if ( Core.AOS )
                return AbsorbDamageAOS( attacker, defender, damage );

            double chance = Utility.RandomDouble();

            Item armorItem;

            if( chance < 0.07 )
                armorItem = defender.NeckArmor;
            else if( chance < 0.14 )
                armorItem = defender.HandArmor;
            else if( chance < 0.28 )
                armorItem = defender.ArmsArmor;
            else if( chance < 0.43 )
                armorItem = defender.HeadArmor;
            else if( chance < 0.65 )
                armorItem = defender.LegsArmor;
            else
                armorItem = defender.ChestArmor;

            IWearableDurability armor = armorItem as IWearableDurability;

            if ( armor != null )
                damage = armor.OnHit( this, damage );

            BaseShield shield = defender.FindItemOnLayer( Layer.TwoHanded ) as BaseShield;
            if ( shield != null )
                damage = shield.OnHit( this, damage );

            // XmlAttachment check for OnArmorHit
            damage -= Server.Engines.XmlSpawner2.XmlAttach.OnArmorHit(attacker, defender, armorItem, this, damage);
            damage -= Server.Engines.XmlSpawner2.XmlAttach.OnArmorHit(attacker, defender, shield, this, damage);

            int virtualArmor = defender.VirtualArmor + defender.VirtualArmorMod;

            if ( virtualArmor > 0 )
            {
                double scalar;

                if ( chance < 0.14 )
                    scalar = 0.07;
                else if ( chance < 0.28 )
                    scalar = 0.14;
                else if ( chance < 0.43 )
                    scalar = 0.15;
                else if ( chance < 0.65 )
                    scalar = 0.22;
                else
                    scalar = 0.35;

                int from = (int)(virtualArmor * scalar) / 2;
                int to = (int)(virtualArmor * scalar);

                damage -= Utility.Random( from, (to - from) + 1 );
            }

            return damage;
        }

        public virtual int GetPackInstinctBonus( Mobile attacker, Mobile defender )
        {
            if ( attacker.Player || defender.Player )
                return 0;

            BaseCreature bc = attacker as BaseCreature;

            if ( bc == null || bc.PackInstinct == PackInstinct.None || (!bc.Controlled && !bc.Summoned) )
                return 0;

            Mobile master = bc.ControlMaster;

            if ( master == null )
                master = bc.SummonMaster;

            if ( master == null )
                return 0;

            int inPack = 1;

            IPooledEnumerable eable = defender.GetMobilesInRange( 1 );
            foreach ( Mobile m in eable )
            {
                if ( m != attacker && m is BaseCreature )
                {
                    BaseCreature tc = (BaseCreature)m;

                    if ( (tc.PackInstinct & bc.PackInstinct) == 0 || (!tc.Controlled && !tc.Summoned) )
                        continue;

                    Mobile theirMaster = tc.ControlMaster;

                    if ( theirMaster == null )
                        theirMaster = tc.SummonMaster;

                    if ( master == theirMaster && tc.Combatant == defender )
                        ++inPack;
                }
            }
            eable.Free();

            if ( inPack >= 5 )
                return 100;
            else if ( inPack >= 4 )
                return 75;
            else if ( inPack >= 3 )
                return 50;
            else if ( inPack >= 2 )
                return 25;

            return 0;
        }

        private static bool m_InDoubleStrike;

        public static bool InDoubleStrike
        {
            get{ return m_InDoubleStrike; }
            set{ m_InDoubleStrike = value; }
        }

        public void OnHit( Mobile attacker, Mobile defender )
        {
            OnHit( attacker, defender, 1.0 );
        }

        public virtual void OnHit( Mobile attacker, Mobile defender, double damageBonus )
        {
            if ( MirrorImage.HasClone( defender ) && (defender.Skills.Ninjitsu.Value / 150.0) > Utility.RandomDouble() )
            {
                Clone bc;

                IPooledEnumerable eable = defender.GetMobilesInRange( 4 );
                foreach ( Mobile m in eable )
                {
                    bc = m as Clone;

                    if ( bc != null && bc.Summoned && bc.SummonMaster == defender )
                    {
                        attacker.SendLocalizedMessage( 1063141 ); // Your attack has been diverted to a nearby mirror image of your target!
                        defender.SendLocalizedMessage( 1063140 ); // You manage to divert the attack onto one of your nearby mirror images.

                        /*
                         * TODO: What happens if the Clone parries a blow?
                         * And what about if the attacker is using Honorable Execution
                         * and kills it?
                         */

                        defender = m;
                        break;
                    }
                }
                eable.Free();
            }

            PlaySwingAnimation( attacker );
            PlayHurtAnimation( defender );

            attacker.PlaySound( GetHitAttackSound( attacker, defender ) );
            defender.PlaySound( GetHitDefendSound( attacker, defender ) );

            int damage = ComputeDamage( attacker, defender );

            #region Damage Multipliers
            /*
             * The following damage bonuses multiply damage by a factor.
             * Capped at x3 (300%).
             */
            //double factor = 1.0;
            int percentageBonus = 0;

            WeaponAbility a = WeaponAbility.GetCurrentAbility( attacker );
            SpecialMove move = SpecialMove.GetCurrentMove( attacker );

            if( a != null )
            {
                //factor *= a.DamageScalar;
                percentageBonus += (int)(a.DamageScalar * 100) - 100;
            }

            if( move != null )
            {
                //factor *= move.GetDamageScalar( attacker, defender );
                percentageBonus += (int)(move.GetDamageScalar( attacker, defender ) * 100) - 100;
            }

            //factor *= damageBonus;
            percentageBonus += (int)(damageBonus * 100) - 100;

            CheckSlayerResult cs = CheckSlayers( attacker, defender );

            if ( cs != CheckSlayerResult.None )
            {
                if ( cs == CheckSlayerResult.Slayer )
                    defender.FixedEffect( 0x37B9, 10, 5 );

                //factor *= 2.0;
                percentageBonus += 100;
            }

            if ( !attacker.Player )
            {
                if ( defender is PlayerMobile )
                {
                    PlayerMobile pm = (PlayerMobile)defender;

                    if( pm.EnemyOfOneType != null && pm.EnemyOfOneType != attacker.GetType() )
                    {
                        //factor *= 2.0;
                        percentageBonus += 100;
                    }
                }
            }
            else if ( !defender.Player )
            {
                if ( attacker is PlayerMobile )
                {
                    PlayerMobile pm = (PlayerMobile)attacker;

                    if ( pm.WaitingForEnemy )
                    {
                        pm.EnemyOfOneType = defender.GetType();
                        pm.WaitingForEnemy = false;
                    }

                    if ( pm.EnemyOfOneType == defender.GetType() )
                    {
                        defender.FixedEffect( 0x37B9, 10, 5, 1160, 0 );
                        //factor *= 1.5;
                        percentageBonus += 50;
                    }
                }
            }

            int packInstinctBonus = GetPackInstinctBonus( attacker, defender );

            if( packInstinctBonus != 0 )
            {
                //factor *= 1.0 + (double)packInstinctBonus / 100.0;
                percentageBonus += packInstinctBonus;
            }

            if( m_InDoubleStrike )
            {
                //factor *= 0.9; // 10% loss when attacking with double-strike
                percentageBonus -= 10;
            }

            TransformContext context = TransformationSpellHelper.GetContext( defender );

            if( (Slayer == SlayerName.Silver || Slayer2 == SlayerName.Silver) && context != null && context.Spell is NecromancerSpell && context.Type != typeof( HorrificBeastSpell ) )
            {
                //factor *= 1.25; // Every necromancer transformation other than horrific beast takes an additional 25% damage
                percentageBonus += 25;
            }

            if ( attacker is PlayerMobile && !(Core.ML && defender is PlayerMobile ))
            {
                PlayerMobile pmAttacker = (PlayerMobile) attacker;

                if( pmAttacker.HonorActive && pmAttacker.InRange( defender, 1 ) )
                {
                    //factor *= 1.25;
                    percentageBonus += 25;
                }

                if( pmAttacker.SentHonorContext != null && pmAttacker.SentHonorContext.Target == defender )
                {
                    //pmAttacker.SentHonorContext.ApplyPerfectionDamageBonus( ref factor );
                    percentageBonus += pmAttacker.SentHonorContext.PerfectionDamageBonus;
                }
            }

            percentageBonus += ForceOfNature.GetBonus(attacker, defender);

            percentageBonus = Math.Min( percentageBonus, 300 );

            //damage = (int)(damage * factor);
            damage = AOS.Scale( damage, 100 + percentageBonus );
            #endregion

            if ( attacker is BaseCreature )
                ((BaseCreature)attacker).AlterMeleeDamageTo( defender, ref damage );

            if ( defender is BaseCreature )
                ((BaseCreature)defender).AlterMeleeDamageFrom( attacker, ref damage );

            damage = AbsorbDamage( attacker, defender, damage );

            if ( !Core.AOS && damage < 1 )
                damage = 1;
            else if ( Core.AOS && damage == 0 ) // parried
            {
                if ( a != null && a.Validate( attacker ) /*&& a.CheckMana( attacker, true )*/ ) // Parried special moves have no mana cost 
                {
                    a = null;
                    WeaponAbility.ClearCurrentAbility( attacker );

                    attacker.SendLocalizedMessage( 1061140 ); // Your attack was parried!
                }
            }

            AddBlood( attacker, defender, damage );

            int phys, fire, cold, pois, nrgy, chaos, direct;

            GetDamageTypes( attacker, out phys, out fire, out cold, out pois, out nrgy );

            // 13.07.2012 :: zombie
            // TODO: dodac chaos i direct dmg z RunUO 2.2?
            chaos  = 0; 
            direct = 0;
            // zombie

            if ( Core.ML && this is BaseRanged )
            {
                BaseQuiver quiver = attacker.FindItemOnLayer( Layer.Cloak ) as BaseQuiver;

                if ( quiver != null )
                    quiver.AlterBowDamage( ref phys, ref fire, ref cold, ref pois, ref nrgy, ref chaos, ref direct );
            }

            if ( m_Consecrated )
            {
                phys = defender.PhysicalResistance;
                fire = defender.FireResistance;
                cold = defender.ColdResistance;
                pois = defender.PoisonResistance;
                nrgy = defender.EnergyResistance;

                int low = phys, type = 0;

                if ( fire < low ){ low = fire; type = 1; }
                if ( cold < low ){ low = cold; type = 2; }
                if ( pois < low ){ low = pois; type = 3; }
                if ( nrgy < low ){ low = nrgy; type = 4; }

                phys = fire = cold = pois = nrgy = chaos = direct = 0;

                if ( type == 0 ) phys = 100;
                else if ( type == 1 ) fire = 100;
                else if ( type == 2 ) cold = 100;
                else if ( type == 3 ) pois = 100;
                else if ( type == 4 ) nrgy = 100;
            }

            int damageGiven = damage;

            if ( a != null && !a.OnBeforeDamage( attacker, defender ) )
            {
                WeaponAbility.ClearCurrentAbility( attacker );
                a = null;
            }

            if ( move != null && !move.OnBeforeDamage( attacker, defender ) )
            {
                SpecialMove.ClearCurrentMove( attacker );
                move = null;
            }

            if (ImmolatingWeaponSpell.IsImmolating(attacker, this))
            {
                int d = ImmolatingWeaponSpell.GetImmolatingDamage(attacker);
                d = AOS.Damage(defender, attacker, d, 0, 100, 0, 0, 0);

                ImmolatingWeaponSpell.DoDelayEffect(attacker, defender);

                if (d > 0)
                {
                    defender.Damage(d);
                }
            }

            if (Feint.Registry.ContainsKey(defender) && Feint.Registry[defender].Enemy == attacker)
                damage -= (int)(damage * ((double)Feint.Registry[defender].DamageReduction / 100));

            WeaponAbility weavabil;
            bool bladeweaving = Bladeweave.BladeWeaving(attacker, out weavabil);
            bool ignoreArmor = ( a is ArmorIgnore || (move != null && move.IgnoreArmor( attacker )) || (bladeweaving && weavabil is ArmorIgnore));

            damageGiven = AOS.Damage( defender, attacker, damage, ignoreArmor, phys, fire, cold, pois, nrgy );

            double propertyBonus = ( move == null ) ? 1.0 : move.GetPropertyBonus( attacker );

            if ( Core.AOS )
            {
                int lifeLeech = 0;
                int stamLeech = 0;
                int manaLeech = 0;
                int wraithLeech = 0;

                if ( (int)(m_AosWeaponAttributes.HitLeechHitsId(m_Identified) * propertyBonus) > Utility.Random( 100 ) )
                    lifeLeech += 30; // HitLeechHits% chance to leech 30% of damage as hit points

                if ( (int)(m_AosWeaponAttributes.HitLeechStamId(m_Identified) * propertyBonus) > Utility.Random( 100 ) )
                    stamLeech += 100; // HitLeechStam% chance to leech 100% of damage as stamina

                if ( (int)(m_AosWeaponAttributes.HitLeechManaId(m_Identified) * propertyBonus) > Utility.Random( 100 ) )
                    manaLeech += 40; // HitLeechMana% chance to leech 40% of damage as mana

                if ( m_Cursed )
                    lifeLeech += 50; // Additional 50% life leech for cursed weapons (necro spell)

                context = TransformationSpellHelper.GetContext( attacker );

                if ( context != null && context.Type == typeof( VampiricEmbraceSpell ) )
                    lifeLeech += 20; // Vampiric embrace gives an additional 20% life leech

                if ( context != null && context.Type == typeof( WraithFormSpell ) )
                {
                    wraithLeech = (5 + (int)((15 * attacker.Skills.SpiritSpeak.Value) / 100)); // Wraith form gives an additional 5-20% mana leech

                    // Mana leeched by the Wraith Form spell is actually stolen, not just leeched.
                    defender.Mana -= AOS.Scale( damageGiven, wraithLeech );

                    manaLeech += wraithLeech;
                }

                if ( lifeLeech != 0 )
                    attacker.Hits += AOS.Scale( damageGiven, lifeLeech );

                if ( stamLeech != 0 )
                    attacker.Stam += AOS.Scale( damageGiven, stamLeech );

                if ( manaLeech != 0 )
                    attacker.Mana += AOS.Scale( damageGiven, manaLeech );

                if ( lifeLeech != 0 || stamLeech != 0 || manaLeech != 0 )
                    attacker.PlaySound( 0x44D );
            }

            if ( m_MaxHits > 0 && ((MaxRange <= 1 && (defender is Slime || defender is ToxicElemental)) || Config.WeaponDurabilityLostChance > Utility.Random(100)) ) // Stratics says 50% chance, seems more like 4%..
            {
                if ( MaxRange <= 1 && (defender is Slime || defender is ToxicElemental) )
                    attacker.LocalOverheadMessage( MessageType.Regular, 0x3B2, 500263 ); // *Acid blood scars your weapon!*

                if ( Core.AOS && m_AosWeaponAttributes.SelfRepairId(m_Identified) > Utility.Random( 10 ) )
                {
                    HitPoints += 2;
                }
                else
                {
                    if ( m_Hits > 0 )
                    {
                        if(--HitPoints < 5 && Parent is Mobile) {
                            ((Mobile)Parent).LocalOverheadMessage(MessageType.Regular, 0x3B2, 1061182); // Twoja bron sie rozpada!
                        }
                    }
                    else if ( m_MaxHits > 1 )
                    {
                        --MaxHitPoints;                       
                    }
                    else
                    {
                        Delete();
                    }
                }
            }

            if ( attacker is VampireBatFamiliar )
            {
                BaseCreature bc = (BaseCreature)attacker;
                Mobile caster = bc.ControlMaster;

                if ( caster == null )
                    caster = bc.SummonMaster;

                if ( caster != null && caster.Map == bc.Map && caster.InRange( bc, 2 ) )
                    caster.Hits += damage;
                else
                    bc.Hits += damage;
            }

            if ( Core.AOS )
            {
                int physChance = (int)(m_AosWeaponAttributes.HitPhysicalAreaId(m_Identified) * propertyBonus);
                int fireChance = (int)(m_AosWeaponAttributes.HitFireAreaId(m_Identified) * propertyBonus);
                int coldChance = (int)(m_AosWeaponAttributes.HitColdAreaId(m_Identified) * propertyBonus);
                int poisChance = (int)(m_AosWeaponAttributes.HitPoisonAreaId(m_Identified) * propertyBonus);
                int nrgyChance = (int)(m_AosWeaponAttributes.HitEnergyAreaId(m_Identified) * propertyBonus);

                if ( physChance != 0 && physChance > Utility.Random( 100 ) )
                    DoAreaAttack( attacker, defender, 0x10E,   50, 100, 0, 0, 0, 0 );

                if ( fireChance != 0 && fireChance > Utility.Random( 100 ) )
                    DoAreaAttack( attacker, defender, 0x11D, 1160, 0, 100, 0, 0, 0 );

                if ( coldChance != 0 && coldChance > Utility.Random( 100 ) )
                    DoAreaAttack( attacker, defender, 0x0FC, 2100, 0, 0, 100, 0, 0 );

                if ( poisChance != 0 && poisChance > Utility.Random( 100 ) )
                    DoAreaAttack( attacker, defender, 0x205, 1166, 0, 0, 0, 100, 0 );

                if ( nrgyChance != 0 && nrgyChance > Utility.Random( 100 ) )
                    DoAreaAttack( attacker, defender, 0x1F1,  120, 0, 0, 0, 0, 100 );

                int maChance = (int)(m_AosWeaponAttributes.HitMagicArrowId(m_Identified) * propertyBonus);
                int harmChance = (int)(m_AosWeaponAttributes.HitHarmId(m_Identified) * propertyBonus);
                int fireballChance = (int)(m_AosWeaponAttributes.HitFireballId(m_Identified) * propertyBonus);
                int lightningChance = (int)(m_AosWeaponAttributes.HitLightningId(m_Identified) * propertyBonus);
                int dispelChance = (int)(m_AosWeaponAttributes.HitDispelId(m_Identified) * propertyBonus);

                if ( maChance != 0 && maChance > Utility.Random( 100 ) )
                    DoMagicArrow( attacker, defender );

                if ( harmChance != 0 && harmChance > Utility.Random( 100 ) )
                    DoHarm( attacker, defender );

                if ( fireballChance != 0 && fireballChance > Utility.Random( 100 ) )
                    DoFireball( attacker, defender );

                if ( lightningChance != 0 && lightningChance > Utility.Random( 100 ) )
                    DoLightning( attacker, defender );

                if ( dispelChance != 0 && dispelChance > Utility.Random( 100 ) )
                    DoDispel( attacker, defender );

                int laChance = (int)(m_AosWeaponAttributes.HitLowerAttackId(m_Identified) * propertyBonus);
                int ldChance = (int)(m_AosWeaponAttributes.HitLowerDefendId(m_Identified) * propertyBonus);

                if ( laChance != 0 && laChance > Utility.Random( 100 ) )
                    DoLowerAttack( attacker, defender );

                if ( ldChance != 0 && ldChance > Utility.Random( 100 ) )
                    DoLowerDefense( attacker, defender );
            }

            if ( attacker is BaseCreature )
                ((BaseCreature)attacker).OnGaveMeleeAttack( defender );

            if ( defender is BaseCreature )
                ((BaseCreature)defender).OnGotMeleeAttack( attacker );

            if ( a != null )
                a.OnHit( attacker, defender, damage );

            if ( move != null )
                move.OnHit( attacker, defender, damage );

            if ( defender is IHonorTarget && ((IHonorTarget)defender).ReceivedHonorContext != null )
                ((IHonorTarget)defender).ReceivedHonorContext.OnTargetHit( attacker );

            if ( !(this is BaseRanged) )
            {
                if ( AnimalForm.UnderTransformation( attacker, typeof( GiantSerpent ) ) )
                    defender.ApplyPoison( attacker, Poison.Lesser );

                if ( AnimalForm.UnderTransformation( defender, typeof( BullFrog ) ) )
                    attacker.ApplyPoison( defender, Poison.Regular );
            }
            // hook for attachment OnWeaponHit method
            Engines.XmlSpawner2.XmlAttach.OnWeaponHit(this, attacker, defender, damageGiven);
            ACC.CSS.Systems.PlayerEvent.InvokeHitByWeapon( attacker, defender, damageGiven, a );
        }

        public virtual double GetAosDamage( Mobile attacker, int bonus, int dice, int sides )
        {
            int damage = Utility.Dice( dice, sides, bonus ) * 100;
            int damageBonus = 0;

            // Inscription bonus
            int inscribeSkill = attacker.Skills[SkillName.Inscribe].Fixed;

            damageBonus += inscribeSkill / 200;

            if ( inscribeSkill >= 1000 )
                damageBonus += 5;

            if ( attacker.Player )
            {
                // Int bonus
                damageBonus += (attacker.Int / 10);

                // SDI bonus
                damageBonus += AosAttributes.GetValue( attacker, AosAttribute.SpellDamage );
            }

            damage = AOS.Scale( damage, 100 + damageBonus );

            return damage / 100;
        }

        public virtual double GetAosSpellDamage(Mobile attacker, Mobile defender, int bonus, int dice, int sides) {
            int damage = Utility.Dice(dice, sides, bonus) * 100;
            int damageBonus = 0;

            int inscribeSkill = attacker.Skills[SkillName.Inscribe].Fixed;
            int inscribeBonus = (inscribeSkill + (1000 * (inscribeSkill / 1000))) / 200;

            damageBonus += inscribeBonus;
            damageBonus += attacker.Int / 10;
            damageBonus += SpellHelper.GetSpellDamageBonus(attacker, defender, SkillName.Magery, attacker is PlayerMobile && defender is PlayerMobile);
            damage = AOS.Scale(damage, 100 + damageBonus);

            if (defender != null && Feint.Registry.ContainsKey(defender) && Feint.Registry[defender].Enemy == attacker)
                damage -= (int)(damage * ((double)Feint.Registry[defender].DamageReduction / 100));

            // All hit spells use 80 eval
            int evalScale = 30 + ((9 * 800) / 100);

            damage = AOS.Scale(damage, evalScale);

            return damage / 100;
        }

        #region Do<AoSEffect>
        public virtual void DoMagicArrow( Mobile attacker, Mobile defender )
        {
            if ( !attacker.CanBeHarmful( defender, false ) )
                return;

            attacker.DoHarmful( defender );

            double damage = GetAosSpellDamage( attacker, defender, 10, 1, 4 );

            attacker.MovingParticles( defender, 0x36E4, 5, 0, false, true, 3006, 4006, 0 );
            attacker.PlaySound( 0x1E5 );

            SpellHelper.Damage( TimeSpan.FromSeconds( 1.0 ), defender, attacker, damage, 0, 100, 0, 0, 0 );
        }

        public virtual void DoHarm( Mobile attacker, Mobile defender )
        {
            if ( !attacker.CanBeHarmful( defender, false ) )
                return;

            attacker.DoHarmful( defender );

            double damage = GetAosSpellDamage( attacker, defender, 17, 1, 5 );

            if ( !defender.InRange( attacker, 2 ) )
                damage *= 0.25; // 1/4 damage at > 2 tile range
            else if ( !defender.InRange( attacker, 1 ) )
                damage *= 0.50; // 1/2 damage at 2 tile range

            defender.FixedParticles( 0x374A, 10, 30, 5013, 1153, 2, EffectLayer.Waist );
            defender.PlaySound( 0x0FC );

            SpellHelper.Damage( TimeSpan.Zero, defender, attacker, damage, 0, 0, 100, 0, 0 );
        }

        public virtual void DoFireball( Mobile attacker, Mobile defender )
        {
            if ( !attacker.CanBeHarmful( defender, false ) )
                return;

            attacker.DoHarmful( defender );

            double damage = GetAosSpellDamage( attacker, defender, 19, 1, 5 );

            attacker.MovingParticles( defender, 0x36D4, 7, 0, false, true, 9502, 4019, 0x160 );
            attacker.PlaySound( 0x15E );

            SpellHelper.Damage( TimeSpan.FromSeconds( 1.0 ), defender, attacker, damage, 0, 100, 0, 0, 0 );
        }

        public virtual void DoLightning( Mobile attacker, Mobile defender )
        {
            if ( !attacker.CanBeHarmful( defender, false ) )
                return;

            attacker.DoHarmful( defender );

            double damage = GetAosSpellDamage( attacker, defender, 23, 1, 4 );

            defender.BoltEffect( 0 );

            SpellHelper.Damage( TimeSpan.Zero, defender, attacker, damage, 0, 0, 0, 0, 100 );
        }

        public virtual void DoDispel( Mobile attacker, Mobile defender )
        {
            bool dispellable = false;

            if ( defender is BaseCreature )
                dispellable = ((BaseCreature)defender).Summoned && !((BaseCreature)defender).IsAnimatedDead;

            if ( !dispellable )
                return;

            if ( !attacker.CanBeHarmful( defender, false ) )
                return;

            attacker.DoHarmful( defender );

            Spells.MagerySpell sp = new Spells.Sixth.DispelSpell( attacker, null );

            if ( sp.CheckResisted( defender ) )
            {
                defender.FixedEffect( 0x3779, 10, 20 );
            }
            else
            {
                Effects.SendLocationParticles( EffectItem.Create( defender.Location, defender.Map, EffectItem.DefaultDuration ), 0x3728, 8, 20, 5042 );
                Effects.PlaySound( defender, defender.Map, 0x201 );

                defender.Delete();
            }
        }

        public virtual void DoLowerAttack( Mobile from, Mobile defender )
        {
            if ( HitLower.ApplyAttack( defender ) )
            {
                defender.PlaySound( 0x28E );
                Effects.SendTargetEffect( defender, 0x37BE, 1, 4, 0xA, 3 );
            }
        }

        public virtual void DoLowerDefense( Mobile from, Mobile defender )
        {
            if ( HitLower.ApplyDefense( defender ) )
            {
                defender.PlaySound( 0x28E );
                Effects.SendTargetEffect( defender, 0x37BE, 1, 4, 0x23, 3 );
            }
        }

        public virtual void DoAreaAttack( Mobile from, Mobile defender, int sound, int hue, int phys, int fire, int cold, int pois, int nrgy )
        {
            Map map = from.Map;

            if ( map == null )
                return;

            List<Mobile> list = new List<Mobile>();

            IPooledEnumerable eable = from.GetMobilesInRange( 10 );
            foreach ( Mobile m in eable )
            {
                if ( from != m && defender != m && SpellHelper.ValidIndirectTarget( from, m ) && from.CanBeHarmful( m, false ) && ( !Core.ML || from.InLOS( m ) ) )
                    list.Add( m );
            }
            eable.Free();

            if ( list.Count == 0 )
                return;

            Effects.PlaySound( from.Location, map, sound );

            // TODO: What is the damage calculation?

            for ( int i = 0; i < list.Count; ++i )
            {
                Mobile m = list[i];

                double scalar = (11 - from.GetDistanceToSqrt( m )) / 10;

                if ( scalar > 1.0 )
                    scalar = 1.0;
                else if ( scalar < 0.0 )
                    continue;

                from.DoHarmful( m, true );
                m.FixedEffect( 0x3779, 1, 15, hue, 0 );
                AOS.Damage( m, from, (int)(GetBaseDamage( from ) * scalar), phys, fire, cold, pois, nrgy );
            }
        }
        #endregion

        public virtual CheckSlayerResult CheckSlayers( Mobile attacker, Mobile defender )
        {
            BaseWeapon atkWeapon = attacker.Weapon as BaseWeapon;
            SlayerEntry atkSlayer = SlayerGroup.GetEntryByName( atkWeapon.Slayer );
            SlayerEntry atkSlayer2 = SlayerGroup.GetEntryByName( atkWeapon.Slayer2 );

            if ( atkSlayer != null && atkSlayer.Slays( defender )  || atkSlayer2 != null && atkSlayer2.Slays( defender ) )
                return CheckSlayerResult.Slayer;

            if ( !Core.SE )
            {
                ISlayer defISlayer = Spellbook.FindEquippedSpellbook( defender );

                if( defISlayer == null )
                    defISlayer = defender.Weapon as ISlayer;

                if( defISlayer != null )
                {
                    SlayerEntry defSlayer = SlayerGroup.GetEntryByName( defISlayer.Slayer );
                    SlayerEntry defSlayer2 = SlayerGroup.GetEntryByName( defISlayer.Slayer2 );

                    if( defSlayer != null && defSlayer.Group.OppositionSuperSlays( attacker ) || defSlayer2 != null && defSlayer2.Group.OppositionSuperSlays( attacker ) )
                        return CheckSlayerResult.Opposition;
                }
            }

            return CheckSlayerResult.None;
        }

        public virtual void AddBlood( Mobile attacker, Mobile defender, int damage )
        {
            if ( damage > 0 )
            {
                new Blood().MoveToWorld( defender.Location, defender.Map );

                int extraBlood = (Core.SE ? Utility.RandomMinMax( 3, 4 ) : Utility.RandomMinMax( 0, 1 ) );

                for( int i = 0; i < extraBlood; i++ )
                {
                    new Blood().MoveToWorld( new Point3D(
                        defender.X + Utility.RandomMinMax( -1, 1 ),
                        defender.Y + Utility.RandomMinMax( -1, 1 ),
                        defender.Z ), defender.Map );
                }
            }
        }

        public virtual void GetDamageTypes( Mobile wielder, out int phys, out int fire, out int cold, out int pois, out int nrgy )
        {
            if( wielder is BaseCreature )
            {
                BaseCreature bc = (BaseCreature)wielder;

                phys = bc.PhysicalDamage;
                fire = bc.FireDamage;
                cold = bc.ColdDamage;
                pois = bc.PoisonDamage;
                nrgy = bc.EnergyDamage;
            }
            else
            {
                fire = m_AosElementDamages.Fire;
                cold = m_AosElementDamages.Cold;
                pois = m_AosElementDamages.Poison;
                nrgy = m_AosElementDamages.Energy;

                phys = 100 - fire - cold - pois - nrgy;

                CraftResourceInfo resInfo = CraftResources.GetInfo( m_Resource );

                if( resInfo != null )
                {
                    CraftAttributeInfo attrInfo = resInfo.AttributeInfo;

                    if( attrInfo != null )
                    {
                        int left = phys;

                        left = ApplyCraftAttributeElementDamage( attrInfo.WeaponColdDamage,        ref cold, left );
                        left = ApplyCraftAttributeElementDamage( attrInfo.WeaponEnergyDamage,    ref nrgy, left );
                        left = ApplyCraftAttributeElementDamage( attrInfo.WeaponFireDamage,        ref fire, left );
                        left = ApplyCraftAttributeElementDamage( attrInfo.WeaponPoisonDamage,    ref pois, left );

                        phys = left;
                    }
                }

                CraftResourceInfo resInfo2 = CraftResources.GetInfo( Resource2 );

                if( resInfo2 != null )
                {
                    CraftAttributeInfo attrInfo2 = resInfo2.AttributeInfo;

                    if( attrInfo2 != null )
                    {
                        int left = phys;

                        left = ApplyCraftAttributeElementDamage( attrInfo2.WeaponColdDamage,        ref cold, left );
                        left = ApplyCraftAttributeElementDamage( attrInfo2.WeaponEnergyDamage,    ref nrgy, left );
                        left = ApplyCraftAttributeElementDamage( attrInfo2.WeaponFireDamage,        ref fire, left );
                        left = ApplyCraftAttributeElementDamage( attrInfo2.WeaponPoisonDamage,    ref pois, left );

                        phys = left;
                    }
                }
            }
        }

        private int ApplyCraftAttributeElementDamage( int attrDamage, ref int element, int totalRemaining )
        {
            if( totalRemaining <= 0 )
                return 0;

            if ( attrDamage <= 0 )
                return totalRemaining;

            int appliedDamage = attrDamage;

            if ( (appliedDamage + element) > 100 )
                appliedDamage = 100 - element;

            if( appliedDamage > totalRemaining )
                appliedDamage = totalRemaining;

            element += appliedDamage;

            return totalRemaining - appliedDamage;
        }

        public virtual void OnMiss( Mobile attacker, Mobile defender )
        {
            PlaySwingAnimation( attacker );
            attacker.PlaySound( GetMissAttackSound( attacker, defender ) );
            defender.PlaySound( GetMissDefendSound( attacker, defender ) );

            WeaponAbility ability = WeaponAbility.GetCurrentAbility( attacker );

            if ( ability != null )
                ability.OnMiss( attacker, defender );

            SpecialMove move = SpecialMove.GetCurrentMove( attacker );

            if ( move != null )
                move.OnMiss( attacker, defender );

            if ( defender is IHonorTarget && ((IHonorTarget)defender).ReceivedHonorContext != null )
                ((IHonorTarget)defender).ReceivedHonorContext.OnTargetMissed( attacker );
        }

        public virtual void GetBaseDamageRange( Mobile attacker, out int min, out int max )
        {
            if ( attacker is BaseCreature )
            {
                BaseCreature c = (BaseCreature)attacker;

                if ( c.DamageMin >= 0 )
                {
                    min = c.DamageMin;
                    max = c.DamageMax;
                    return;
                }

                if ( this is Fists && !attacker.Body.IsHuman )
                {
                    min = attacker.Str / 28;
                    max = attacker.Str / 28;
                    return;
                }
            }

            min = MinDamage;
            max = MaxDamage;
        }

        public virtual double GetBaseDamage( Mobile attacker )
        {
            int min, max;

            GetBaseDamageRange( attacker, out min, out max );

            return Utility.RandomMinMax( min, max );
        }

        public virtual double GetBonus( double value, double scalar, double threshold, double offset )
        {
            double bonus = value * scalar;

            if ( value >= threshold )
                bonus += offset;

            return bonus / 100;
        }

        public virtual int GetHitChanceBonus()
        {
            if ( !Core.AOS )
                return 0;

            int bonus = 0;

            switch ( m_AccuracyLevel )
            {
                case WeaponAccuracyLevel.Accurate:        bonus += 02; break;
                case WeaponAccuracyLevel.Surpassingly:    bonus += 04; break;
                case WeaponAccuracyLevel.Eminently:        bonus += 06; break;
                case WeaponAccuracyLevel.Exceedingly:    bonus += 08; break;
                case WeaponAccuracyLevel.Supremely:        bonus += 10; break;
            }

            return bonus;
        }

        public virtual int GetDamageBonus()
        {
            int bonus = VirtualDamageBonus;

            switch (m_Quality)
            {
                case WeaponQuality.Low: bonus -= 20; break;
                case WeaponQuality.Exceptional: bonus += 20; break;
            }

            // zombie - 17-06-2012 - DI bonus z resourcow
            CraftResourceInfo resInfo = CraftResources.GetInfo(m_Resource);
            if (resInfo != null)
            {
                CraftAttributeInfo attrInfo = resInfo.AttributeInfo;

                if (attrInfo != null)
                    bonus += attrInfo.WeaponDamageIncrease;
            }

            CraftResourceInfo resInfo2 = CraftResources.GetInfo(Resource2);
            if (resInfo2 != null)
            {
                CraftAttributeInfo attrInfo2 = resInfo2.AttributeInfo;

                if (attrInfo2 != null)
                    bonus += attrInfo2.WeaponDamageIncrease;
            }

            // cap 50% DI na broni (bez DamageLevel)
            int maxBonus = 50 - m_AosAttributes.WeaponDamageId(m_Identified);
            bonus = bonus > maxBonus ? maxBonus : bonus;

            switch (m_DamageLevel)
            {
                case WeaponDamageLevel.Ruin: bonus += 15; break;
                case WeaponDamageLevel.Might: bonus += 20; break;
                case WeaponDamageLevel.Force: bonus += 25; break;
                case WeaponDamageLevel.Power: bonus += 30; break;
                case WeaponDamageLevel.Vanq: bonus += 35; break;
            }

            return bonus > 0 ? bonus : 0;
            // zombie
        }

        public virtual void GetStatusDamage( Mobile from, out int min, out int max )
        {
            int baseMin, baseMax;

            GetBaseDamageRange( from, out baseMin, out baseMax );

            if ( Core.AOS )
            {
                min = Math.Max( (int)ScaleDamageAOS( from, baseMin, false ), 1 );
                max = Math.Max( (int)ScaleDamageAOS( from, baseMax, false ), 1 );
            }
            else
            {
                min = Math.Max( (int)ScaleDamageOld( from, baseMin, false ), 1 );
                max = Math.Max( (int)ScaleDamageOld( from, baseMax, false ), 1 );
            }
        }

        public virtual double ScaleDamageAOS( Mobile attacker, double damage, bool checkSkills )
        {
            if ( checkSkills )
            {
                attacker.CheckSkill( SkillName.Tactics, 0.0, attacker.Skills[SkillName.Tactics].Cap ); // Passively check tactics for gain
                attacker.CheckSkill( SkillName.Anatomy, 0.0, attacker.Skills[SkillName.Anatomy].Cap ); // Passively check Anatomy for gain

                if ( Type == WeaponType.Axe )
                    attacker.CheckSkill( SkillName.Lumberjacking, 0.0, 100.0 ); // Passively check Lumberjacking for gain
            }

            #region Physical bonuses
            /*
             * These are the bonuses given by the physical characteristics of the mobile.
             * No caps apply.
             */
            double strengthBonus = GetBonus( attacker.Str,                                        0.300, 100.0,  5.00 );
            double  anatomyBonus = GetBonus( attacker.Skills[SkillName.Anatomy].Value,            0.500, 100.0,  5.00 );
            double  tacticsBonus = GetBonus( attacker.Skills[SkillName.Tactics].Value,            0.625, 100.0,  6.25 );
            double   lumberBonus = GetBonus( attacker.Skills[SkillName.Lumberjacking].Value,    0.200, 100.0, 10.00 );

            if ( Type != WeaponType.Axe )
                lumberBonus = 0.0;
            #endregion

            #region Modifiers
            /*
             * The following are damage modifiers whose effect shows on the status bar.
             * Capped at 100% total.
             */
            int damageBonus = AosAttributes.GetValue( attacker, AosAttribute.WeaponDamage );

            // Horrific Beast transformation gives a +25% bonus to damage.
            if( TransformationSpellHelper.UnderTransformation( attacker, typeof( HorrificBeastSpell ) ) )
                damageBonus += 25;

            // Divine Fury gives a +10% bonus to damage.
            if ( Spells.Chivalry.DivineFurySpell.UnderEffect( attacker ) )
                damageBonus += 10;

            int defenseMasteryMalus = 0;

            // Defense Mastery gives a -50%/-80% malus to damage.
            if ( Server.Items.DefenseMastery.GetMalus( attacker, ref defenseMasteryMalus ) )
                damageBonus -= defenseMasteryMalus;

            int discordanceEffect = 0;

            // Discordance gives a -2%/-48% malus to damage.
            if ( SkillHandlers.Discordance.GetEffect( attacker, ref discordanceEffect ) )
                damageBonus -= discordanceEffect * 2;

            if ( damageBonus > 100 )
                damageBonus = 100;
            #endregion

            double totalBonus = strengthBonus + anatomyBonus + tacticsBonus + lumberBonus + ((double)(GetDamageBonus() + damageBonus) / 100.0);

            return damage + (int)(damage * totalBonus);
        }

        public virtual int VirtualDamageBonus{ get{ return 0; } }

        public virtual int ComputeDamageAOS( Mobile attacker, Mobile defender )
        {
            return (int)ScaleDamageAOS( attacker, GetBaseDamage( attacker ), true );
        }

        public virtual double ScaleDamageOld( Mobile attacker, double damage, bool checkSkills )
        {
            if ( checkSkills )
            {
                attacker.CheckSkill( SkillName.Tactics, 0.0, attacker.Skills[SkillName.Tactics].Cap ); // Passively check tactics for gain
                attacker.CheckSkill( SkillName.Anatomy, 0.0, attacker.Skills[SkillName.Anatomy].Cap ); // Passively check Anatomy for gain

                if ( Type == WeaponType.Axe )
                    attacker.CheckSkill( SkillName.Lumberjacking, 0.0, 100.0 ); // Passively check Lumberjacking for gain
            }

            /* Compute tactics modifier
             * :   0.0 = 50% loss
             * :  50.0 = unchanged
             * : 100.0 = 50% bonus
             */
            double tacticsBonus = (attacker.Skills[SkillName.Tactics].Value - 50.0) / 100.0;

            /* Compute strength modifier
             * : 1% bonus for every 5 strength
             */
            double strBonus = (attacker.Str / 5.0) / 100.0;

            /* Compute anatomy modifier
             * : 1% bonus for every 5 points of anatomy
             * : +10% bonus at Grandmaster or higher
             */
            double anatomyValue = attacker.Skills[SkillName.Anatomy].Value;
            double anatomyBonus = (anatomyValue / 5.0) / 100.0;

            if ( anatomyValue >= 100.0 )
                anatomyBonus += 0.1;

            /* Compute lumberjacking bonus
             * : 1% bonus for every 5 points of lumberjacking
             * : +10% bonus at Grandmaster or higher
             */
            double lumberBonus;

            if ( Type == WeaponType.Axe )
            {
                double lumberValue = attacker.Skills[SkillName.Lumberjacking].Value;

                lumberBonus = (lumberValue / 5.0) / 100.0;

                if ( lumberValue >= 100.0 )
                    lumberBonus += 0.1;
            }
            else
            {
                lumberBonus = 0.0;
            }

            // New quality bonus:
            double qualityBonus = ((int)m_Quality - 1) * 0.2;

            // Apply bonuses
            damage += (damage * tacticsBonus) + (damage * strBonus) + (damage * anatomyBonus) + (damage * lumberBonus) + (damage * qualityBonus) + ((damage * VirtualDamageBonus) / 100);

            // Old quality bonus:
#if false
            /* Apply quality offset
             * : Low         : -4
             * : Regular     :  0
             * : Exceptional : +4
             */
            damage += ((int)m_Quality - 1) * 4.0;
#endif

            /* Apply damage level offset
             * : Regular : 0
             * : Ruin    : 1
             * : Might   : 3
             * : Force   : 5
             * : Power   : 7
             * : Vanq    : 9
             */
            if ( m_DamageLevel != WeaponDamageLevel.Regular )
                damage += (2.0 * (int)m_DamageLevel) - 1.0;

            // Halve the computed damage and return
            damage /= 2.0;

            return ScaleDamageByDurability( (int)damage );
        }

        public virtual int ScaleDamageByDurability( int damage )
        {
            int scale = 100;

            if ( m_MaxHits > 0 && m_Hits < m_MaxHits )
                scale = 50 + ((50 * m_Hits) / m_MaxHits);

            return AOS.Scale( damage, scale );
        }

        public virtual int ComputeDamage( Mobile attacker, Mobile defender )
        {
            if ( Core.AOS )
                return ComputeDamageAOS( attacker, defender );

            return (int)ScaleDamageOld( attacker, GetBaseDamage( attacker ), true );
        }

        public virtual void PlayHurtAnimation( Mobile from )
        {
            int action;
            int frames;

            switch ( from.Body.Type )
            {
                case BodyType.Sea:
                case BodyType.Animal:
                {
                    action = 7;
                    frames = 5;
                    break;
                }
                case BodyType.Monster:
                {
                    action = 10;
                    frames = 4;
                    break;
                }
                case BodyType.Human:
                {
                    action = 20;
                    frames = 5;
                    break;
                }
                default: return;
            }

            if ( from.Mounted )
                return;

            from.Animate( action, frames, 1, true, false, 0 );
        }

        public virtual void PlaySwingAnimation( Mobile from )
        {
            int action;

            switch ( from.Body.Type )
            {
                case BodyType.Sea:
                case BodyType.Animal:
                {
                    action = Utility.Random( 5, 2 );
                    break;
                }
                case BodyType.Monster:
                {
                    switch ( Animation )
                    {
                        default:
                        case WeaponAnimation.Wrestle:
                        case WeaponAnimation.Bash1H:
                        case WeaponAnimation.Pierce1H:
                        case WeaponAnimation.Slash1H:
                        case WeaponAnimation.Bash2H:
                        case WeaponAnimation.Pierce2H:
                        case WeaponAnimation.Slash2H: action = Utility.Random( 4, 3 ); break;
                        case WeaponAnimation.ShootBow:  return; // 7
                        case WeaponAnimation.ShootXBow: return; // 8
                    }

                    break;
                }
                case BodyType.Human:
                {
                    if ( !from.Mounted )
                    {
                        action = (int)Animation;
                    }
                    else
                    {
                        switch ( Animation )
                        {
                            default:
                            case WeaponAnimation.Wrestle:
                            case WeaponAnimation.Bash1H:
                            case WeaponAnimation.Pierce1H:
                            case WeaponAnimation.Slash1H: action = 26; break;
                            case WeaponAnimation.Bash2H:
                            case WeaponAnimation.Pierce2H:
                            case WeaponAnimation.Slash2H: action = 29; break;
                            case WeaponAnimation.ShootBow: action = 27; break;
                            case WeaponAnimation.ShootXBow: action = 28; break;
                        }
                    }

                    break;
                }
                default: return;
            }

            from.Animate( action, 7, 1, true, false, 0 );
        }

        #region Serialization/Deserialization
        private static void SetSaveFlag( ref SaveFlag flags, SaveFlag toSet, bool setIf )
        {
            if ( setIf )
                flags |= toSet;
        }

        private static bool GetSaveFlag( SaveFlag flags, SaveFlag toGet )
        {
            return ( (flags & toGet) != 0 );
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int) 9 ); // version

            SaveFlag flags = SaveFlag.None;

            SetSaveFlag( ref flags, SaveFlag.DamageLevel,        m_DamageLevel != WeaponDamageLevel.Regular );
            SetSaveFlag( ref flags, SaveFlag.AccuracyLevel,        m_AccuracyLevel != WeaponAccuracyLevel.Regular );
            SetSaveFlag( ref flags, SaveFlag.DurabilityLevel,    m_DurabilityLevel != WeaponDurabilityLevel.Regular );
            SetSaveFlag( ref flags, SaveFlag.Quality,            m_Quality != WeaponQuality.Regular );
            SetSaveFlag( ref flags, SaveFlag.Hits,                m_Hits != 0 );
            SetSaveFlag( ref flags, SaveFlag.MaxHits,            m_MaxHits != 0 );
            SetSaveFlag( ref flags, SaveFlag.Slayer,            m_Slayer != SlayerName.None );
            SetSaveFlag( ref flags, SaveFlag.Poison,            m_Poison != null );
            SetSaveFlag( ref flags, SaveFlag.PoisonCharges,        m_PoisonCharges != 0 );
            SetSaveFlag( ref flags, SaveFlag.Crafter,            m_Crafter != null );
            SetSaveFlag( ref flags, SaveFlag.Identified,        m_Identified != false );
            //SetSaveFlag( ref flags, SaveFlag.IdentifiedLevel,    m_IdentifiedLevel != IdLevel.None );
            SetSaveFlag( ref flags, SaveFlag.StrReq,            m_StrReq != -1 );
            SetSaveFlag( ref flags, SaveFlag.DexReq,            m_DexReq != -1 );
            SetSaveFlag( ref flags, SaveFlag.IntReq,            m_IntReq != -1 );
            SetSaveFlag( ref flags, SaveFlag.MinDamage,            m_MinDamage != -1 );
            SetSaveFlag( ref flags, SaveFlag.MaxDamage,            m_MaxDamage != -1 );
            SetSaveFlag( ref flags, SaveFlag.HitSound,            m_HitSound != -1 );
            SetSaveFlag( ref flags, SaveFlag.MissSound,            m_MissSound != -1 );
            SetSaveFlag( ref flags, SaveFlag.Speed,                m_Speed != -1 );
            SetSaveFlag( ref flags, SaveFlag.MaxRange,            m_MaxRange != -1 );
            SetSaveFlag( ref flags, SaveFlag.Skill,                m_Skill != (SkillName)(-1) );
            SetSaveFlag( ref flags, SaveFlag.Type,                m_Type != (WeaponType)(-1) );
            SetSaveFlag( ref flags, SaveFlag.Animation,            m_Animation != (WeaponAnimation)(-1) );
            SetSaveFlag( ref flags, SaveFlag.Resource,            m_Resource != DefaultResource );
            SetSaveFlag( ref flags, SaveFlag.xAttributes,        !m_AosAttributes.IsEmpty );
            SetSaveFlag( ref flags, SaveFlag.xWeaponAttributes,    !m_AosWeaponAttributes.IsEmpty );
            SetSaveFlag( ref flags, SaveFlag.PlayerConstructed,    m_PlayerConstructed );
            SetSaveFlag( ref flags, SaveFlag.SkillBonuses,        !m_AosSkillBonuses.IsEmpty );
            SetSaveFlag( ref flags, SaveFlag.Slayer2,            m_Slayer2 != SlayerName.None );
            SetSaveFlag( ref flags, SaveFlag.ElementalDamages,    !m_AosElementDamages.IsEmpty );
            //SetSaveFlag( ref flags, SaveFlag.Resource2,            m_Resource2 != DefaultResource2 );
            SetSaveFlag( ref flags, SaveFlag.EngravedText, !String.IsNullOrEmpty( m_EngravedText ) );

            writer.Write( (int) flags );

            if ( GetSaveFlag( flags, SaveFlag.DamageLevel ) )
                writer.Write( (int) m_DamageLevel );

            if ( GetSaveFlag( flags, SaveFlag.AccuracyLevel ) )
                writer.Write( (int) m_AccuracyLevel );

            if ( GetSaveFlag( flags, SaveFlag.DurabilityLevel ) )
                writer.Write( (int) m_DurabilityLevel );

            if ( GetSaveFlag( flags, SaveFlag.Quality ) )
                writer.Write( (int) m_Quality );

            if ( GetSaveFlag( flags, SaveFlag.Hits ) )
                writer.Write( (int) m_Hits );

            if ( GetSaveFlag( flags, SaveFlag.MaxHits ) )
                writer.Write( (int) m_MaxHits );

            if ( GetSaveFlag( flags, SaveFlag.Slayer ) )
                writer.Write( (int) m_Slayer );

            if ( GetSaveFlag( flags, SaveFlag.Poison ) )
                Poison.Serialize( m_Poison, writer );

            if ( GetSaveFlag( flags, SaveFlag.PoisonCharges ) )
                writer.Write( (int) m_PoisonCharges );

            if ( GetSaveFlag( flags, SaveFlag.Crafter ) )
                writer.Write( (Mobile) m_Crafter );

            if ( GetSaveFlag( flags, SaveFlag.StrReq ) )
                writer.Write( (int) m_StrReq );

            if ( GetSaveFlag( flags, SaveFlag.DexReq ) )
                writer.Write( (int) m_DexReq );

            if ( GetSaveFlag( flags, SaveFlag.IntReq ) )
                writer.Write( (int) m_IntReq );

            if ( GetSaveFlag( flags, SaveFlag.MinDamage ) )
                writer.Write( (int) m_MinDamage );

            if ( GetSaveFlag( flags, SaveFlag.MaxDamage ) )
                writer.Write( (int) m_MaxDamage );

            if ( GetSaveFlag( flags, SaveFlag.HitSound ) )
                writer.Write( (int) m_HitSound );

            if ( GetSaveFlag( flags, SaveFlag.MissSound ) )
                writer.Write( (int) m_MissSound );

            if ( GetSaveFlag( flags, SaveFlag.Speed ) )
                writer.Write( (int) m_Speed );

            if ( GetSaveFlag( flags, SaveFlag.MaxRange ) )
                writer.Write( (int) m_MaxRange );

            if ( GetSaveFlag( flags, SaveFlag.Skill ) )
                writer.Write( (int) m_Skill );

            if ( GetSaveFlag( flags, SaveFlag.Type ) )
                writer.Write( (int) m_Type );

            if ( GetSaveFlag( flags, SaveFlag.Animation ) )
                writer.Write( (int) m_Animation );

            if ( GetSaveFlag( flags, SaveFlag.Resource ) )
                writer.Write( (int) m_Resource );

            if ( GetSaveFlag( flags, SaveFlag.xAttributes ) )
                m_AosAttributes.Serialize( writer );

            if ( GetSaveFlag( flags, SaveFlag.xWeaponAttributes ) )
                m_AosWeaponAttributes.Serialize( writer );

            if ( GetSaveFlag( flags, SaveFlag.SkillBonuses ) )
                m_AosSkillBonuses.Serialize( writer );

            if ( GetSaveFlag( flags, SaveFlag.Slayer2 ) )
                writer.Write( (int)m_Slayer2 );

            if( GetSaveFlag( flags, SaveFlag.ElementalDamages ) )
                m_AosElementDamages.Serialize( writer );

            //if( GetSaveFlag( flags, SaveFlag.IdentifiedLevel ) )
            //    writer.Write( (int)m_IdentifiedLevel );
                
            //writer.Write( (int)m_IdentifiedLevel );

            //if ( GetSaveFlag( flags, SaveFlag.Resource2 ) )
            //    writer.Write( (int) m_Resource2 );

            if ( GetSaveFlag( flags, SaveFlag.EngravedText ) )
                writer.Write( (string)m_EngravedText );
        }

        [Flags]
        private enum SaveFlag : long
        {
            None                    = 0x00000000,
            DamageLevel             = 0x00000001,
            AccuracyLevel           = 0x00000002,
            DurabilityLevel         = 0x00000004,
            Quality                 = 0x00000008,
            Hits                    = 0x00000010,
            MaxHits                 = 0x00000020,
            Slayer                  = 0x00000040,
            Poison                  = 0x00000080,
            PoisonCharges           = 0x00000100,
            Crafter                 = 0x00000200,
            Identified              = 0x00000400,
            StrReq                  = 0x00000800,
            DexReq                  = 0x00001000,
            IntReq                  = 0x00002000,
            MinDamage               = 0x00004000,
            MaxDamage               = 0x00008000,
            HitSound                = 0x00010000,
            //IdentifiedLevel         = 0x00020000,
            MissSound               = 0x00020000,
            Speed                   = 0x00040000,
            MaxRange                = 0x00080000,
            Skill                   = 0x00100000,
            Type                    = 0x00200000,
            Animation               = 0x00400000,
            Resource                = 0x00800000,
            xAttributes             = 0x01000000,
            xWeaponAttributes       = 0x02000000,
            PlayerConstructed       = 0x04000000,
            SkillBonuses            = 0x08000000,
            Slayer2                 = 0x10000000,
            ElementalDamages        = 0x20000000,
            //Resource2               = 0x40000000,
            EngravedText            = 0x40000000
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();

            SaveFlag flags = SaveFlag.None;

            switch ( version )
            {
                case 11:
                case 10:
                {
                    if(version == 10)
                        flags = (SaveFlag)reader.ReadLong();
                    goto case 5;
                }
                case 9:
                case 8:
                case 7:
                case 6:
                case 5:
                {
                    if(version != 10)
                        flags = (SaveFlag)reader.ReadInt();

                    if ( GetSaveFlag( flags, SaveFlag.DamageLevel ) )
                    {
                        m_DamageLevel = (WeaponDamageLevel)reader.ReadInt();

                        if ( m_DamageLevel > WeaponDamageLevel.Vanq )
                            m_DamageLevel = WeaponDamageLevel.Ruin;
                    }

                    if ( GetSaveFlag( flags, SaveFlag.AccuracyLevel ) )
                    {
                        m_AccuracyLevel = (WeaponAccuracyLevel)reader.ReadInt();

                        if ( m_AccuracyLevel > WeaponAccuracyLevel.Supremely )
                            m_AccuracyLevel = WeaponAccuracyLevel.Accurate;
                    }

                    if ( GetSaveFlag( flags, SaveFlag.DurabilityLevel ) )
                    {
                        m_DurabilityLevel = (WeaponDurabilityLevel)reader.ReadInt();

                        if ( m_DurabilityLevel > WeaponDurabilityLevel.Indestructible )
                            m_DurabilityLevel = WeaponDurabilityLevel.Durable;
                    }

                    if ( GetSaveFlag( flags, SaveFlag.Quality ) )
                        m_Quality = (WeaponQuality)reader.ReadInt();
                    else
                        m_Quality = WeaponQuality.Regular;

                    if ( GetSaveFlag( flags, SaveFlag.Hits ) )
                        m_Hits = reader.ReadInt();

                    if ( GetSaveFlag( flags, SaveFlag.MaxHits ) )
                        m_MaxHits = reader.ReadInt();

                    if ( GetSaveFlag( flags, SaveFlag.Slayer ) )
                        m_Slayer = (SlayerName)reader.ReadInt();

                    if ( GetSaveFlag( flags, SaveFlag.Poison ) )
                        m_Poison = Poison.Deserialize( reader );

                    if ( GetSaveFlag( flags, SaveFlag.PoisonCharges ) )
                        m_PoisonCharges = reader.ReadInt();

                    if ( GetSaveFlag( flags, SaveFlag.Crafter ) )
                        m_Crafter = reader.ReadMobile();

                    if ( GetSaveFlag( flags, SaveFlag.Identified ) )
                        m_Identified = ( version >= 6 || reader.ReadBool() );

                    if ( GetSaveFlag( flags, SaveFlag.StrReq ) )
                        m_StrReq = reader.ReadInt();
                    else
                        m_StrReq = -1;

                    if ( GetSaveFlag( flags, SaveFlag.DexReq ) )
                        m_DexReq = reader.ReadInt();
                    else
                        m_DexReq = -1;

                    if ( GetSaveFlag( flags, SaveFlag.IntReq ) )
                        m_IntReq = reader.ReadInt();
                    else
                        m_IntReq = -1;

                    if ( GetSaveFlag( flags, SaveFlag.MinDamage ) )
                        m_MinDamage = reader.ReadInt();
                    else
                        m_MinDamage = -1;

                    if ( GetSaveFlag( flags, SaveFlag.MaxDamage ) )
                        m_MaxDamage = reader.ReadInt();
                    else
                        m_MaxDamage = -1;

                    if ( GetSaveFlag( flags, SaveFlag.HitSound ) )
                        m_HitSound = reader.ReadInt();
                    else
                        m_HitSound = -1;

                    if ( version > 9 && version < 11 )
                    {
                        if ( GetSaveFlag( flags, (SaveFlag)0x00020001 ) )
                            m_MissSound = reader.ReadInt();
                        else
                            m_MissSound = -1;
                    }
                    else
                    {
                        if ( GetSaveFlag( flags, SaveFlag.MissSound ) )
                            m_MissSound = reader.ReadInt();
                        else
                            m_MissSound = -1;
                    }

                    if ( GetSaveFlag( flags, SaveFlag.Speed ) )
                        m_Speed = reader.ReadInt();
                    else
                        m_Speed = -1;

                    if ( GetSaveFlag( flags, SaveFlag.MaxRange ) )
                        m_MaxRange = reader.ReadInt();
                    else
                        m_MaxRange = -1;

                    if ( GetSaveFlag( flags, SaveFlag.Skill ) )
                        m_Skill = (SkillName)reader.ReadInt();
                    else
                        m_Skill = (SkillName)(-1);

                    if ( GetSaveFlag( flags, SaveFlag.Type ) )
                        m_Type = (WeaponType)reader.ReadInt();
                    else
                        m_Type = (WeaponType)(-1);

                    if ( GetSaveFlag( flags, SaveFlag.Animation ) )
                        m_Animation = (WeaponAnimation)reader.ReadInt();
                    else
                        m_Animation = (WeaponAnimation)(-1);

                    if ( GetSaveFlag( flags, SaveFlag.Resource ) )
                        m_Resource = (CraftResource)reader.ReadInt();
                    else
                        m_Resource = DefaultResource;

                    if ( GetSaveFlag( flags, SaveFlag.xAttributes ) )
                        m_AosAttributes = new AosAttributes( this, reader );
                    else
                        m_AosAttributes = new AosAttributes( this );

                    if ( GetSaveFlag( flags, SaveFlag.xWeaponAttributes ) )
                        m_AosWeaponAttributes = new AosWeaponAttributes( this, reader );
                    else
                        m_AosWeaponAttributes = new AosWeaponAttributes( this );

                    if ( UseSkillMod && m_AccuracyLevel != WeaponAccuracyLevel.Regular && Parent is Mobile )
                    {
                        m_SkillMod = new DefaultSkillMod( AccuracySkill, true, (int)m_AccuracyLevel * 5 );
                        ((Mobile)Parent).AddSkillMod( m_SkillMod );
                    }

                    if ( version < 7 && m_AosWeaponAttributes.MageWeaponId(m_Identified) != 0 )
                        m_AosWeaponAttributes.MageWeapon = 30 - m_AosWeaponAttributes.MageWeaponId(m_Identified);

                    if ( Core.AOS && m_AosWeaponAttributes.MageWeaponId(m_Identified) != 0 && m_AosWeaponAttributes.MageWeaponId(m_Identified) != 30 && Parent is Mobile )
                    {
                        m_MageMod = new DefaultSkillMod( SkillName.Magery, true, -30 + m_AosWeaponAttributes.MageWeaponId(m_Identified) );
                        ((Mobile)Parent).AddSkillMod( m_MageMod );
                    }

                    if ( GetSaveFlag( flags, SaveFlag.PlayerConstructed ) )
                        m_PlayerConstructed = true;

                    if( GetSaveFlag( flags, SaveFlag.SkillBonuses ) )
                        m_AosSkillBonuses = new AosSkillBonuses( this, reader );
                    else
                        m_AosSkillBonuses = new AosSkillBonuses( this );

                    if( GetSaveFlag( flags, SaveFlag.Slayer2 ) )
                        m_Slayer2 = (SlayerName)reader.ReadInt();

                    if( GetSaveFlag( flags, SaveFlag.ElementalDamages ) )
                        m_AosElementDamages = new AosElementAttributes( this, reader );
                    else
                        m_AosElementDamages = new AosElementAttributes( this );

                    if ( version > 9 && version < 11 )
                    {
                        if ( GetSaveFlag( flags, (SaveFlag)0x00020000 ) )
                            /*m_IdentifiedLevel = (IdLevel)*/
                            reader.ReadInt();

                        /*m_IdentifiedLevel = (IdLevel)*/
                        reader.ReadInt();


                        if ( GetSaveFlag( flags, (SaveFlag)0x40000000 ) )
                            Resource2 = (CraftResource)reader.ReadInt();
                        else
                            Resource2 = DefaultResource2;
                    }

                    if ( version > 9 && version < 11 )
                    {
                        if ( GetSaveFlag( flags, (SaveFlag)0x80000000 ) )
                            m_EngravedText = reader.ReadString();  
                    }
                    else
                    {
                        if ( GetSaveFlag( flags, SaveFlag.EngravedText ) )
                            m_EngravedText = reader.ReadString();
                    }

                    break;
                }
                case 4:
                {
                    m_Slayer = (SlayerName)reader.ReadInt();

                    goto case 3;
                }
                case 3:
                {
                    m_StrReq = reader.ReadInt();
                    m_DexReq = reader.ReadInt();
                    m_IntReq = reader.ReadInt();

                    goto case 2;
                }
                case 2:
                {
                    m_Identified = reader.ReadBool();

                    goto case 1;
                }
                case 1:
                {
                    m_MaxRange = reader.ReadInt();

                    goto case 0;
                }
                case 0:
                {
                    if ( version == 0 )
                        m_MaxRange = 1; // default

                    if ( version < 5 )
                    {
                        m_Resource = CraftResource.Iron;
                        m_AosAttributes = new AosAttributes( this );
                        m_AosWeaponAttributes = new AosWeaponAttributes( this );
                        m_AosElementDamages = new AosElementAttributes( this );
                        m_AosSkillBonuses = new AosSkillBonuses( this );
                    }

                    m_MinDamage = reader.ReadInt();
                    m_MaxDamage = reader.ReadInt();

                    m_Speed = reader.ReadInt();

                    m_HitSound = reader.ReadInt();
                    m_MissSound = reader.ReadInt();

                    m_Skill = (SkillName)reader.ReadInt();
                    m_Type = (WeaponType)reader.ReadInt();
                    m_Animation = (WeaponAnimation)reader.ReadInt();
                    m_DamageLevel = (WeaponDamageLevel)reader.ReadInt();
                    m_AccuracyLevel = (WeaponAccuracyLevel)reader.ReadInt();
                    m_DurabilityLevel = (WeaponDurabilityLevel)reader.ReadInt();
                    m_Quality = (WeaponQuality)reader.ReadInt();

                    m_Crafter = reader.ReadMobile();

                    m_Poison = Poison.Deserialize( reader );
                    m_PoisonCharges = reader.ReadInt();

                    if ( m_StrReq == OldStrengthReq )
                        m_StrReq = -1;

                    if ( m_DexReq == OldDexterityReq )
                        m_DexReq = -1;

                    if ( m_IntReq == OldIntelligenceReq )
                        m_IntReq = -1;

                    if ( m_MinDamage == OldMinDamage )
                        m_MinDamage = -1;

                    if ( m_MaxDamage == OldMaxDamage )
                        m_MaxDamage = -1;

                    if ( m_HitSound == OldHitSound )
                        m_HitSound = -1;

                    if ( m_MissSound == OldMissSound )
                        m_MissSound = -1;

                    if ( m_Speed == OldSpeed )
                        m_Speed = -1;

                    if ( m_MaxRange == OldMaxRange )
                        m_MaxRange = -1;

                    if ( m_Skill == OldSkill )
                        m_Skill = (SkillName)(-1);

                    if ( m_Type == OldType )
                        m_Type = (WeaponType)(-1);

                    if ( m_Animation == OldAnimation )
                        m_Animation = (WeaponAnimation)(-1);

                    if ( UseSkillMod && m_AccuracyLevel != WeaponAccuracyLevel.Regular && Parent is Mobile )
                    {
                        m_SkillMod = new DefaultSkillMod( AccuracySkill, true, (int)m_AccuracyLevel * 5);
                        ((Mobile)Parent).AddSkillMod( m_SkillMod );
                    }

                    break;
                }
            }

            if ( Core.AOS && Parent is Mobile )
                m_AosSkillBonuses.AddTo( (Mobile)Parent, m_Identified );

            int strBonus = m_AosAttributes.BonusStrId(m_Identified);
            int dexBonus = m_AosAttributes.BonusDexId(m_Identified);
            int intBonus = m_AosAttributes.BonusIntId(m_Identified);

            if ( this.Parent is Mobile && (strBonus != 0 || dexBonus != 0 || intBonus != 0) )
            {
                Mobile m = (Mobile)this.Parent;

                string modName = this.Serial.ToString();

                if ( strBonus != 0 )
                    m.AddStatMod( new StatMod( StatType.Str, modName + "Str", strBonus, TimeSpan.Zero ) );

                if ( dexBonus != 0 )
                    m.AddStatMod( new StatMod( StatType.Dex, modName + "Dex", dexBonus, TimeSpan.Zero ) );

                if ( intBonus != 0 )
                    m.AddStatMod( new StatMod( StatType.Int, modName + "Int", intBonus, TimeSpan.Zero ) );
            }

            if ( Parent is Mobile )
                ((Mobile)Parent).CheckStatTimers();

            if ( m_Hits <= 0 && m_MaxHits <= 0 )
            {
                // 22.06.2012 :: zombie
                m_Hits = m_MaxHits = InitHitPoints; // Utility.RandomMinMax( InitMinHits, InitMaxHits );
                // zombie
            }

            if ( version < 6 )
                m_PlayerConstructed = true; // we don't know, so, assume it's crafted
        }
        #endregion

		public virtual CraftResource DefaultResource{ get{ return CraftResource.Iron; } }
        public virtual CraftResource DefaultResource2{ get{ return CraftResource.None; } }

        public BaseWeapon( int itemID ) : base( itemID )
        {
            Layer = (Layer)ItemData.Quality;

            m_Quality = WeaponQuality.Regular;
            m_StrReq = -1;
            m_DexReq = -1;
            m_IntReq = -1;
            m_MinDamage = -1;
            m_MaxDamage = -1;
            m_HitSound = -1;
            m_MissSound = -1;
            m_Speed = -1;
            m_MaxRange = -1;
            m_Skill = (SkillName)(-1);
            m_Type = (WeaponType)(-1);
            m_Animation = (WeaponAnimation)(-1);

            m_Resource = CraftResource.Iron;
            ExtraCraftResource.Get( this ).Resource2 = CraftResource.None;

            // 22.09.2012 :: zombie :: zmiany w wytrzymalosci broni
            if (InitMinHits == 0 || InitMaxHits == 0)
            {
                m_Hits = m_MaxHits = InitHitPoints;
            }
            else
            {
                m_Hits = m_MaxHits = Utility.RandomMinMax(InitMinHits, InitMaxHits);
            }
            // zombie

            m_AosAttributes = new AosAttributes( this );
            m_AosWeaponAttributes = new AosWeaponAttributes( this );
            m_AosSkillBonuses = new AosSkillBonuses( this );
            m_AosElementDamages = new AosElementAttributes( this );
        }

        public BaseWeapon( Serial serial ) : base( serial )
        {
        }

        private string GetNameString()
        {
            string name = this.Name;

            if ( name == null )
                name = String.Format( "#{0}", LabelNumber );

            return name;
        }

        [Hue, CommandProperty( AccessLevel.GameMaster )]
        public override int Hue
        {
            get{ return base.Hue; }
            set{ base.Hue = value; InvalidateProperties(); }
        }

        public int GetElementalDamageHue()
        {
            int phys, fire, cold, pois, nrgy;
            GetDamageTypes( null, out phys, out fire, out cold, out pois, out nrgy );
            //Order is Cold, Energy, Fire, Poison, Physical left

            int currentMax = 50;
            int hue = 0;

            if( pois >= currentMax )
            {
                hue = 1267 + (pois - 50) / 10;
                currentMax = pois;
            }

            if( fire >= currentMax )
            {
                hue = 1255 + (fire - 50) / 10;
                currentMax = fire;
            }

            if( nrgy >= currentMax )
            {
                hue = 1273 + (nrgy - 50) / 10;
                currentMax = nrgy;
            }

            if( cold >= currentMax )
            {
                hue = 1261 + (cold - 50) / 10;
                currentMax = cold;
            }

            return hue;
        }

        public static int ResourceNameNumber(CraftResource res)
        {
            int oreType = BaseArmor.ResourceNameNumber( res );
            if( oreType == 0 )
            {
                switch (res)
                {
                    case CraftResource.BowstringLeather:	oreType = 1032743; break; // ze skorzana cieciwa
                    case CraftResource.BowstringCannabis:	oreType = 1032744; break; // z konopna cieciwa
                    case CraftResource.BowstringSilk:		oreType = 1032745; break; // z jedwabna cieciwa
                    case CraftResource.BowstringGut:		oreType = 1032746; break; // z jelitowa cieciwa
                    case CraftResource.BowstringFiresteed:	oreType = 1032747; break; // z wlosia ognistego rumaka
                    case CraftResource.BowstringUnicorn:	oreType = 1032748; break; // z wlosia jednorozca
                    case CraftResource.BowstringNightmare:	oreType = 1032749; break; // z wlosia koszmara
                    case CraftResource.BowstringKirin:		oreType = 1032750; break; // z wlosia ki-rina
                    case CraftResource.BowstringSpined:		oreType = 1032767; break; // z cieciwa z niebieskiej skory
                    case CraftResource.BowstringHorned:		oreType = 1032768; break; // z cieciwa z czerwonej skory
                    case CraftResource.BowstringBarbed:		oreType = 1032769; break; // z cieciwa z zielonej skory
                    default: oreType = 0; break;
                }
            }
            return oreType;
        }

        public bool IsCraftable() {
            CraftSystem[] craftSystems = { DefBlacksmithy.CraftSystem, DefBowFletching.CraftSystem, DefCarpentry.CraftSystem, DefTailoring.CraftSystem };
            foreach (CraftSystem craftSystem in craftSystems) {
                CraftItem craftItem = craftSystem.CraftItems.SearchFor(this.GetType());
                if (craftItem != null && craftItem.Ressources.Count != 0) return true;
            }
            return false;
        }

        public override void AddNameProperty( ObjectPropertyList list )
        {
            //    list.Add( 1053099, "#{0}\t{1}", oreType, GetNameString() ); // ~1_oretype~ ~2_armortype~

            if ( Name == null )
                list.Add( LabelNumber );
            else
                list.Add( Name );
            if (IsCraftable()) {
                int oreType = ResourceNameNumber(m_Resource);
                if (oreType != 0)
                    list.Add(oreType);

                oreType = ResourceNameNumber(Resource2);
                if (oreType != 0)
                    list.Add(oreType);
            }

            if ( !String.IsNullOrEmpty( m_EngravedText ) )
                list.Add( 1062613, m_EngravedText );
        }

        public override bool AllowEquipedCast( Mobile from )
        {
            if ( base.AllowEquipedCast( from ) )
                return true;

            return ( m_AosAttributes.SpellChannelingId(m_Identified) != 0 );
        }

        public virtual int ArtifactRarity
        {
            get{ return 0; }
        }

        public virtual int GetLuckBonus()
        {
            int bonus = 0;

            CraftResourceInfo resInfo = CraftResources.GetInfo( m_Resource );
            if ( resInfo != null )
            {
                CraftAttributeInfo attrInfo = resInfo.AttributeInfo;
                if ( attrInfo != null )
                    bonus += attrInfo.WeaponLuck;
            }

            CraftResourceInfo resInfo2 = CraftResources.GetInfo( Resource2 );
            if ( resInfo2 != null )
            {
                CraftAttributeInfo attrInfo2 = resInfo2.AttributeInfo;
                if ( attrInfo2 != null )
                    bonus += attrInfo2.WeaponLuck;
            }

            return bonus;
        }

        public override void GetProperties( ObjectPropertyList list )
        {
            base.GetProperties( list );

            #region Automatyczna identyfikacja jesli przedmiot jest wykonany przez gracza
            if ( m_PlayerConstructed == true || !Config.ItemIDSystemEnabled )
            {
                m_Identified = true;
            }
            #endregion
            
            int phys, fire, cold, pois, nrgy;

            #region Factions
            if ( m_FactionState != null )
                list.Add( 1041350 ); // faction item
            #endregion

            #region Propsy zawsze wyswietlane
            if ( m_Crafter != null )
                list.Add( 1050043, m_Crafter.Name ); // crafted by ~1_NAME~

            if ( m_Quality == WeaponQuality.Exceptional )
                list.Add( 1060636 ); // exceptional

            /*if( RequiredRace == Race.Elf )
                list.Add( 1075086 ); // Elves Only*/

            if ( this is IUsesRemaining && ((IUsesRemaining)this).ShowUsesRemaining )
                    list.Add( 1060584, ((IUsesRemaining)this).UsesRemaining.ToString() ); // uses remaining: ~1_val~

            if ( ArtifactRarity > 0 )
                list.Add( 1061078, ArtifactRarity.ToString() ); // artifact rarity ~1_val~
                
            list.Add( 1061168, "{0}\t{1}", MinDamage.ToString(), MaxDamage.ToString() ); // weapon damage ~1_val~ - ~2_val~

            if ( Core.ML )
                list.Add( 1061167, String.Format( "{0}s", Speed ) ); // weapon speed ~1_val~
            else
                list.Add( 1061167, Speed.ToString() );

            if ( MaxRange > 1 )
                list.Add( 1061169, MaxRange.ToString() ); // range ~1_val~
                    
            if ( m_Poison != null && m_PoisonCharges > 0 )
                list.Add( 1062412 + m_Poison.Level, m_PoisonCharges.ToString() );
                
            if ( m_Hits >= 0 && m_MaxHits > 0 )
                list.Add( 1060639, "{0}\t{1}", m_Hits, m_MaxHits ); // durability ~1_val~ / ~2_val~
                
            int strReq = AOS.Scale( StrRequirement, 100 - GetLowerStatReq() );

            if ( strReq > 0 )
                list.Add( 1061170, strReq.ToString() ); // strength requirement ~1_val~

            if ( Layer == Layer.TwoHanded )
                list.Add( 1061171 ); // two-handed weapon
            else
                list.Add( 1061824 ); // one-handed weapon
                
            if ( Core.SE || m_AosWeaponAttributes.UseBestSkillId(m_Identified) == 0 )
            {
                switch ( Skill )
                {
                    case SkillName.Swords:  list.Add( 1061172 ); break; // skill required: swordsmanship
                    case SkillName.Macing:  list.Add( 1061173 ); break; // skill required: mace fighting
                    case SkillName.Fencing: list.Add( 1061174 ); break; // skill required: fencing
                    case SkillName.Archery: list.Add( 1061175 ); break; // skill required: archery
                }
            }
            
            GetDamageTypes( null, out phys, out fire, out cold, out pois, out nrgy );
            
            if ( phys != 0 )
                list.Add( 1060403, phys.ToString() ); // physical damage ~1_val~%
            
            if ( fire != 0 )
                list.Add( 1060405, fire.ToString() ); // fire damage ~1_val~%
    
            if ( cold != 0 )
                list.Add( 1060404, cold.ToString() ); // cold damage ~1_val~%
    
            if ( pois != 0 )
                list.Add( 1060406, pois.ToString() ); // poison damage ~1_val~%
    
            if ( nrgy != 0 )
                list.Add( 1060407, nrgy.ToString() ); // energy damage ~1_val

            #endregion
            
            #region Zliczanie propsow i przypisywanie lvl
            // 14.10.2012 mortuus - zliczanie propsow przeniesione do klasy ItemIdentification
            m_IdentifiedLevel = ItemIdentification.PropertiesCount(m_AosAttributes, m_AosSkillBonuses, m_Slayer, m_Slayer2, null, m_AosWeaponAttributes, null, 0);
            if( m_IdentifiedLevel == IdLevel.None )
                m_Identified = true;
            #endregion

            // 15.10.2012 :: zombie            
            if( !m_Identified )
                list.Add( ItemIdentification.UnidentifiedString );

            if ( m_AosSkillBonuses != null ) 
                m_AosSkillBonuses.GetProperties( list, m_Identified );

            if ( m_Identified )
                base.AddResistanceProperties( list );
            else
            {
                ItemIdentification.AddNameProperty(AosWeaponAttribute.ResistPhysicalBonus, 1071178, m_AosWeaponAttributes.ResistPhysicalBonus, ref list, m_Identified);
                ItemIdentification.AddNameProperty(AosWeaponAttribute.ResistFireBonus, 1071177, m_AosWeaponAttributes.ResistFireBonus, ref list, m_Identified );
                ItemIdentification.AddNameProperty(AosWeaponAttribute.ResistColdBonus, 1071175, m_AosWeaponAttributes.ResistColdBonus, ref list, m_Identified );
                ItemIdentification.AddNameProperty(AosWeaponAttribute.ResistPoisonBonus, 1071179, m_AosWeaponAttributes.ResistPoisonBonus, ref list, m_Identified );
                ItemIdentification.AddNameProperty(AosWeaponAttribute.ResistEnergyBonus, 1071176, m_AosWeaponAttributes.ResistEnergyBonus, ref list, m_Identified );
            }

            SlayerName[] slayerNames = new SlayerName[] { m_Slayer, m_Slayer2 };
            foreach ( SlayerName slayerName in slayerNames )
            {
                if( slayerName != SlayerName.None )
                {
                    ItemIdentification.AddNameSlayerProperty(slayerName, ref list, m_Identified);
                }
            }

            ItemIdentification.AddNameProperty(AosWeaponAttribute.UseBestSkill, 1060400, m_AosWeaponAttributes.UseBestSkill, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.WeaponDamage, 1060401, GetDamageBonus() + m_AosAttributes.WeaponDamage, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.DefendChance, 1060408, m_AosAttributes.DefendChance, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.EnhancePotions, 1060411, m_AosAttributes.EnhancePotions, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.CastRecovery, 1060412, m_AosAttributes.CastRecovery, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.CastSpeed, 1060413, m_AosAttributes.CastSpeed, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.AttackChance, 1060415, GetHitChanceBonus() + m_AosAttributes.AttackChance, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosWeaponAttribute.HitColdArea, 1060416, m_AosWeaponAttributes.HitColdArea, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosWeaponAttribute.HitDispel, 1060417, m_AosWeaponAttributes.HitDispel, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosWeaponAttribute.HitEnergyArea, 1060418, m_AosWeaponAttributes.HitEnergyArea, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosWeaponAttribute.HitFireArea, 1060419, m_AosWeaponAttributes.HitFireArea, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosWeaponAttribute.HitFireball, 1060420, m_AosWeaponAttributes.HitFireball, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosWeaponAttribute.HitHarm, 1060421, m_AosWeaponAttributes.HitHarm, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosWeaponAttribute.HitLeechHits, 1060422, m_AosWeaponAttributes.HitLeechHits, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosWeaponAttribute.HitLightning, 1060423, m_AosWeaponAttributes.HitLightning, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosWeaponAttribute.HitLowerAttack, 1060424, m_AosWeaponAttributes.HitLowerAttack, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosWeaponAttribute.HitLowerDefend, 1060425, m_AosWeaponAttributes.HitLowerDefend, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosWeaponAttribute.HitMagicArrow, 1060426, m_AosWeaponAttributes.HitMagicArrow, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosWeaponAttribute.HitLeechMana, 1060427, m_AosWeaponAttributes.HitLeechMana, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosWeaponAttribute.HitPhysicalArea, 1060428, m_AosWeaponAttributes.HitPhysicalArea, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosWeaponAttribute.HitPoisonArea, 1060429, m_AosWeaponAttributes.HitPoisonArea, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosWeaponAttribute.HitLeechStam, 1060430, m_AosWeaponAttributes.HitLeechStam, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.BonusDex, 1060409, m_AosAttributes.BonusDex, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.BonusHits, 1060431, m_AosAttributes.BonusHits, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.BonusInt, 1060432, m_AosAttributes.BonusInt, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.LowerManaCost, 1060433, m_AosAttributes.LowerManaCost, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.LowerRegCost, 1060434, m_AosAttributes.LowerRegCost, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosWeaponAttribute.LowerStatReq, 1060435, GetLowerStatReqId(true), ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.Luck, 1060436, GetLuckBonus() + m_AosAttributes.Luck, ref list, m_Identified);
            if( m_AosWeaponAttributes.MageWeapon != 0 )
            {
                int MageWeaponMinus = 30 - m_AosWeaponAttributes.MageWeapon;
                if( MageWeaponMinus == 0 )
                    ItemIdentification.AddNameProperty(AosWeaponAttribute.MageWeapon, 1071168, 666, ref list, m_Identified);
                else
                    ItemIdentification.AddNameProperty(AosWeaponAttribute.MageWeapon, 1060438, MageWeaponMinus, ref list, m_Identified);
            }
            ItemIdentification.AddNameProperty(AosAttribute.BonusMana, 1060439, m_AosAttributes.BonusMana, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.RegenMana, 1060440, m_AosAttributes.RegenMana, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.NightSight, 1060441, m_AosAttributes.NightSight, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.ReflectPhysical, 1060442, m_AosAttributes.ReflectPhysical, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.RegenStam, 1060443, m_AosAttributes.RegenStam, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.RegenHits, 1060444, m_AosAttributes.RegenHits, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosWeaponAttribute.SelfRepair, 1060450, m_AosWeaponAttributes.SelfRepair, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.SpellChanneling, 1060482, m_AosAttributes.SpellChanneling, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.SpellDamage, 1060483, m_AosAttributes.SpellDamage, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.BonusStam, 1060484, m_AosAttributes.BonusStam, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.BonusStr, 1060485, m_AosAttributes.BonusStr, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.WeaponSpeed, 1060486, m_AosAttributes.WeaponSpeed, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosWeaponAttribute.DurabilityBonus, 1060410, m_AosWeaponAttributes.DurabilityBonus, ref list, m_Identified);
            // zombie

            // mod to display attachment properties
            Server.Engines.XmlSpawner2.XmlAttach.AddAttachmentProperties(this, list);
        }

        public override void OnSingleClick( Mobile from )
        {
            List<EquipInfoAttribute> attrs = new List<EquipInfoAttribute>();

            if ( DisplayLootType )
            {
                if ( LootType == LootType.Blessed )
                    attrs.Add( new EquipInfoAttribute( 1038021 ) ); // blessed
                else if ( LootType == LootType.Cursed )
                    attrs.Add( new EquipInfoAttribute( 1049643 ) ); // cursed
            }

            #region Factions
            if ( m_FactionState != null )
                attrs.Add( new EquipInfoAttribute( 1041350 ) ); // faction item
            #endregion

            if ( m_Quality == WeaponQuality.Exceptional )
                attrs.Add( new EquipInfoAttribute( 1018305 - (int)m_Quality ) );

            if ( m_Identified || from.AccessLevel >= AccessLevel.GameMaster )
            {
                if( m_Slayer != SlayerName.None )
                {
                    SlayerEntry entry = SlayerGroup.GetEntryByName( m_Slayer );
                    if( entry != null )
                        attrs.Add( new EquipInfoAttribute( entry.Title ) );
                }

                if( m_Slayer2 != SlayerName.None )
                {
                    SlayerEntry entry = SlayerGroup.GetEntryByName( m_Slayer2 );
                    if( entry != null )
                        attrs.Add( new EquipInfoAttribute( entry.Title ) );
                }

                if ( m_DurabilityLevel != WeaponDurabilityLevel.Regular )
                    attrs.Add( new EquipInfoAttribute( 1038000 + (int)m_DurabilityLevel ) );

                if ( m_DamageLevel != WeaponDamageLevel.Regular )
                    attrs.Add( new EquipInfoAttribute( 1038015 + (int)m_DamageLevel ) );

                if ( m_AccuracyLevel != WeaponAccuracyLevel.Regular )
                    attrs.Add( new EquipInfoAttribute( 1038010 + (int)m_AccuracyLevel ) );
            }
            else if( Slayer != SlayerName.None || Slayer2 != SlayerName.None || m_DurabilityLevel != WeaponDurabilityLevel.Regular || m_DamageLevel != WeaponDamageLevel.Regular || m_AccuracyLevel != WeaponAccuracyLevel.Regular )
                attrs.Add( new EquipInfoAttribute( 1038000 ) ); // Unidentified

            if ( m_Poison != null && m_PoisonCharges > 0 )
                attrs.Add( new EquipInfoAttribute( 1017383, m_PoisonCharges ) );

            int number;

            if ( Name == null )
            {
                number = LabelNumber;
            }
            else
            {
                this.LabelTo( from, Name );
                number = 1041000;
            }

            if ( attrs.Count == 0 && Crafter == null && Name != null )
                return;

            EquipmentInfo eqInfo = new EquipmentInfo( number, m_Crafter, false, attrs.ToArray() );

            from.Send( new DisplayEquipmentInfo( this, eqInfo ) );
        }

        private static BaseWeapon m_Fists; // This value holds the default--fist--weapon

        public static BaseWeapon Fists
        {
            get{ return m_Fists; }
            set{ m_Fists = value; }
        }

        #region ICraftable Members

        public int OnCraft( int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, Type typeRes2, BaseTool tool, CraftItem craftItem, int resHue )
        {
            Quality = (WeaponQuality)quality;

            if ( makersMark )
                Crafter = from;

            PlayerConstructed = true;

			Type resourceType = typeRes;
            /*
			if ( resourceType == null )
				resourceType = craftItem.Ressources.GetAt( 0 ).ItemType;
            */
            Resource = CraftResources.GetFromType( resourceType );

			Type resourceType2 = typeRes2;
            /*
			if ( resourceType2 == null )
            {
                if( craftItem.Ressources.Count >= 2 )
                {
                    resourceType2 = craftItem.Ressources.GetAt( 1 ).ItemType;
                    Resource2 = CraftResources.GetFromType( resourceType2 );
                }
                else
                    Resource2 = CraftResource.None;
            }
            else */
                Resource2 = CraftResources.GetFromType( resourceType2 );

            if ( Core.AOS )
            {
                CraftContext context = craftSystem.GetContext( from );

                if ( context != null && context.DoNotColor )
                    Hue = 0;

                if ( tool is BaseRunicTool )
                    ((BaseRunicTool)tool).ApplyAttributesTo( this );

                if ( Quality == WeaponQuality.Exceptional )
                {
                    if ( Attributes.WeaponDamage > 35 )
                        Attributes.WeaponDamage -= 20;
                    else
                        Attributes.WeaponDamage = 15;

                    if( Core.ML )
                    {
                        Attributes.WeaponDamage += (int)(from.Skills.ArmsLore.Value / 20);

                        if ( Attributes.WeaponDamage > 50 )
                            Attributes.WeaponDamage = 50;

                        from.CheckSkill( SkillName.ArmsLore, 0, 100 );
                    }
                }
            }
            else if ( tool is BaseRunicTool )
            {
                CraftResource thisResource = CraftResources.GetFromType( resourceType );

                if ( thisResource == ((BaseRunicTool)tool).Resource )
                {
                    Resource = thisResource;

                    CraftContext context = craftSystem.GetContext( from );

                    if ( context != null && context.DoNotColor )
                        Hue = 0;

                    switch ( thisResource )
                    {
                        case CraftResource.DullCopper:
                        {
                            Identified = true;
                            DurabilityLevel = WeaponDurabilityLevel.Durable;
                            AccuracyLevel = WeaponAccuracyLevel.Accurate;
                            break;
                        }
                        case CraftResource.ShadowIron:
                        {
                            Identified = true;
                            DurabilityLevel = WeaponDurabilityLevel.Durable;
                            DamageLevel = WeaponDamageLevel.Ruin;
                            break;
                        }
                        case CraftResource.Copper:
                        {
                            Identified = true;
                            DurabilityLevel = WeaponDurabilityLevel.Fortified;
                            DamageLevel = WeaponDamageLevel.Ruin;
                            AccuracyLevel = WeaponAccuracyLevel.Surpassingly;
                            break;
                        }
                        case CraftResource.Bronze:
                        {
                            Identified = true;
                            DurabilityLevel = WeaponDurabilityLevel.Fortified;
                            DamageLevel = WeaponDamageLevel.Might;
                            AccuracyLevel = WeaponAccuracyLevel.Surpassingly;
                            break;
                        }
                        case CraftResource.Gold:
                        {
                            Identified = true;
                            DurabilityLevel = WeaponDurabilityLevel.Indestructible;
                            DamageLevel = WeaponDamageLevel.Force;
                            AccuracyLevel = WeaponAccuracyLevel.Eminently;
                            break;
                        }
                        case CraftResource.Agapite:
                        {
                            Identified = true;
                            DurabilityLevel = WeaponDurabilityLevel.Indestructible;
                            DamageLevel = WeaponDamageLevel.Power;
                            AccuracyLevel = WeaponAccuracyLevel.Eminently;
                            break;
                        }
                        case CraftResource.Verite:
                        {
                            Identified = true;
                            DurabilityLevel = WeaponDurabilityLevel.Indestructible;
                            DamageLevel = WeaponDamageLevel.Power;
                            AccuracyLevel = WeaponAccuracyLevel.Exceedingly;
                            break;
                        }
                        case CraftResource.Valorite:
                        {
                            Identified = true;
                            DurabilityLevel = WeaponDurabilityLevel.Indestructible;
                            DamageLevel = WeaponDamageLevel.Vanq;
                            AccuracyLevel = WeaponAccuracyLevel.Supremely;
                            break;
                        }
                    }
                }
            }

            return quality;
        }

        #endregion
    }

    public enum CheckSlayerResult
    {
        None,
        Slayer,
        Opposition
    }
}
