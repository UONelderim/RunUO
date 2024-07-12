using System;
using System.Collections;
using System.Collections.Generic;
using Server.Network;
using Server.Engines.Craft;
using Server.Factions;
using AMA = Server.Items.ArmorMeditationAllowance;
using AMT = Server.Items.ArmorMaterialType;
using ABT = Server.Items.ArmorBodyType;
using Nelderim.ExtraCraftResource;

namespace Server.Items
{
	public abstract class BaseArmor : Item, IScissorable, IFactionItem, ICraftable, IWearableDurability, IIdentifiable
	{
        #region StrengthRequriaments
        public enum ArmorByStrength
        {
            PlateChest = 0,
            PlateLegs,
            PlateArms,
            PlateHelm,
            PlateGloves,
            PlateGorget,

            DragonChest = 10,
            DragonLegs,
            DragonArms,
            DragonHelm,
            DragonGloves,

            ChainChest = 20,
            ChainLegs,
            ChainHelm,

            BoneChest = 30,
            BoneLegs,
            BoneArms,
            BoneGloves,
            BoneHelm,

            RingChest = 40,
            RingLegs,
            RingArms,
            RingGloves,

            StuddedChest = 50,
            StuddedLegs,
            StuddedArms,
            StuddedGloves,
            StuddedGorget,

            LeatherChest = 60,
            LeatherLegs,
            LeatherArms,
            LeatherGloves,
            LeatherGorget,
            LeatherCap,
        };

        private static Hashtable m_ArmorStrReq;
        public static Hashtable ArmorStrReq
        {
            get
            {
                if( m_ArmorStrReq == null )
                {
                    m_ArmorStrReq = new Hashtable();
                    m_ArmorStrReq[ArmorByStrength.PlateChest]    = 70;
                    m_ArmorStrReq[ArmorByStrength.PlateLegs]     = 70;
                    m_ArmorStrReq[ArmorByStrength.PlateArms]     = 60;
                    m_ArmorStrReq[ArmorByStrength.PlateHelm]     = 60;
                    m_ArmorStrReq[ArmorByStrength.PlateGloves]   = 60;
                    m_ArmorStrReq[ArmorByStrength.PlateGorget]   = 45;

                    m_ArmorStrReq[ArmorByStrength.DragonChest]   = 75;
                    m_ArmorStrReq[ArmorByStrength.DragonLegs]    = 75;
                    m_ArmorStrReq[ArmorByStrength.DragonArms]    = 65;
                    m_ArmorStrReq[ArmorByStrength.DragonGloves]  = 65;
                    m_ArmorStrReq[ArmorByStrength.DragonHelm]    = 65;

                    m_ArmorStrReq[ArmorByStrength.ChainChest]    = 55;
                    m_ArmorStrReq[ArmorByStrength.ChainLegs]     = 55;
                    m_ArmorStrReq[ArmorByStrength.ChainHelm]     = 50;

                    m_ArmorStrReq[ArmorByStrength.BoneChest]     = 60;
                    m_ArmorStrReq[ArmorByStrength.BoneLegs]      = 55;
                    m_ArmorStrReq[ArmorByStrength.BoneArms]      = 55;
                    m_ArmorStrReq[ArmorByStrength.BoneGloves]    = 55;
                    m_ArmorStrReq[ArmorByStrength.BoneHelm]      = 40;

                    m_ArmorStrReq[ArmorByStrength.RingChest]     = 40;
                    m_ArmorStrReq[ArmorByStrength.RingLegs]      = 40;
                    m_ArmorStrReq[ArmorByStrength.RingArms]      = 40;
                    m_ArmorStrReq[ArmorByStrength.RingGloves]    = 40;

                    m_ArmorStrReq[ArmorByStrength.StuddedChest]  = 35;
                    m_ArmorStrReq[ArmorByStrength.StuddedLegs]   = 30;
                    m_ArmorStrReq[ArmorByStrength.StuddedArms]   = 25;
                    m_ArmorStrReq[ArmorByStrength.StuddedGloves] = 25;
                    m_ArmorStrReq[ArmorByStrength.StuddedGorget] = 25;

                    m_ArmorStrReq[ArmorByStrength.LeatherChest]  = 25;
                    m_ArmorStrReq[ArmorByStrength.LeatherLegs]   = 20;
                    m_ArmorStrReq[ArmorByStrength.LeatherArms]   = 20;
                    m_ArmorStrReq[ArmorByStrength.LeatherGloves] = 20;
                    m_ArmorStrReq[ArmorByStrength.LeatherGorget] = 20;
                    m_ArmorStrReq[ArmorByStrength.LeatherCap]    = 20;

                    return m_ArmorStrReq;
                }
                else
                    return m_ArmorStrReq;
            }
        }
        
        #endregion

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



		/* Armor internals work differently now (Jun 19 2003)
		 * 
		 * The attributes defined below default to -1.
		 * If the value is -1, the corresponding virtual 'Aos/Old' property is used.
		 * If not, the attribute value itself is used. Here's the list:
		 *  - ArmorBase
		 *  - StrBonus
		 *  - DexBonus
		 *  - IntBonus
		 *  - StrReq
		 *  - DexReq
		 *  - IntReq
		 *  - MeditationAllowance
		 */

		// Instance values. These values must are unique to each armor piece.
		private int m_MaxHitPoints;
		private int m_HitPoints;
		private Mobile m_Crafter;
		private ArmorQuality m_Quality;
		private ArmorDurabilityLevel m_Durability;
		private ArmorProtectionLevel m_Protection;
		private CraftResource m_Resource;
        //private CraftResource m_Resource2;
		private bool m_Identified, m_PlayerConstructed;
		private IdLevel m_IdentifiedLevel;
		private int m_PhysicalBonus, m_FireBonus, m_ColdBonus, m_PoisonBonus, m_EnergyBonus;

		private AosAttributes m_AosAttributes;
		private AosArmorAttributes m_AosArmorAttributes;
		private AosSkillBonuses m_AosSkillBonuses;

		// Overridable values. These values are provided to override the defaults which get defined in the individual armor scripts.
		private int m_ArmorBase = -1;
		private int m_StrBonus = -1, m_DexBonus = -1, m_IntBonus = -1;
		private int m_StrReq = -1, m_DexReq = -1, m_IntReq = -1;
		private AMA m_Meditate = (AMA)(-1);


		public virtual bool AllowMaleWearer{ get{ return true; } }
		public virtual bool AllowFemaleWearer{ get{ return true; } }

		public abstract AMT MaterialType{ get; }

		public virtual int RevertArmorBase{ get{ return ArmorBase; } }
		public virtual int ArmorBase{ get{ return 0; } }

		public virtual AMA DefMedAllowance{ get{ return AMA.None; } }
		public virtual AMA AosMedAllowance{ get{ return DefMedAllowance; } }
		public virtual AMA OldMedAllowance{ get{ return DefMedAllowance; } }


		public virtual int AosStrBonus{ get{ return 0; } }
		public virtual int AosDexBonus{ get{ return 0; } }
		public virtual int AosIntBonus{ get{ return 0; } }
		public virtual int AosStrReq{ get{ return 0; } }
		public virtual int AosDexReq{ get{ return 0; } }
		public virtual int AosIntReq{ get{ return 0; } }


		public virtual int OldStrBonus{ get{ return 0; } }
		public virtual int OldDexBonus{ get{ return 0; } }
		public virtual int OldIntBonus{ get{ return 0; } }
		public virtual int OldStrReq{ get{ return 0; } }
		public virtual int OldDexReq{ get{ return 0; } }
		public virtual int OldIntReq{ get{ return 0; } }

		public virtual bool CanFortify{ get{ return true; } }

		public override void OnAfterDuped( Item newItem )
		{
			BaseArmor armor = newItem as BaseArmor;

			if ( armor == null )
				return;

			armor.m_AosAttributes = new AosAttributes( newItem, m_AosAttributes );
			armor.m_AosArmorAttributes = new AosArmorAttributes( newItem, m_AosArmorAttributes );
			armor.m_AosSkillBonuses = new AosSkillBonuses( newItem, m_AosSkillBonuses );
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public AMA MeditationAllowance
		{
			get{ return ( m_Meditate == (AMA)(-1) ? Core.AOS ? AosMedAllowance : OldMedAllowance : m_Meditate ); }
			set{ m_Meditate = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int BaseArmorRating
		{
			get
			{
				if ( m_ArmorBase == -1 )
					return ArmorBase;
				else
					return m_ArmorBase;
			}
			set
			{ 
				m_ArmorBase = value; Invalidate(); 
			}
		}

		public double BaseArmorRatingScaled
		{
			get
			{
				return ( BaseArmorRating * ArmorScalar );
			}
		}

		public virtual double ArmorRating
		{
			get
			{
				int ar = BaseArmorRating;

				if ( m_Protection != ArmorProtectionLevel.Regular )
					ar += 10 + (5 * (int)m_Protection);

				switch ( m_Resource )
				{
					case CraftResource.DullCopper:		ar += 2; break;
					case CraftResource.ShadowIron:		ar += 4; break;
					case CraftResource.Copper:			ar += 6; break;
					case CraftResource.Bronze:			ar += 8; break;
					case CraftResource.Gold:			ar += 10; break;
					case CraftResource.Agapite:			ar += 12; break;
					case CraftResource.Verite:			ar += 14; break;
					case CraftResource.Valorite:		ar += 16; break;
					case CraftResource.SpinedLeather:	ar += 10; break;
					case CraftResource.HornedLeather:	ar += 13; break;
					case CraftResource.BarbedLeather:	ar += 16; break;

                    case CraftResource.RedScales:		ar += 18; break;
                    case CraftResource.YellowScales:	ar += 18; break;
                    case CraftResource.BlackScales:		ar += 18; break;
                    case CraftResource.GreenScales:		ar += 18; break;
                    case CraftResource.WhiteScales:		ar += 18; break;
                    case CraftResource.BlueScales:		ar += 18; break;
				}

				ar += -8 + (8 * (int)m_Quality);
				return ScaleArmorByDurability( ar );
			}
		}

		public double ArmorRatingScaled
		{
			get
			{
				return ( ArmorRating * ArmorScalar );
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int StrBonus
		{
			get{ return ( m_StrBonus == -1 ? Core.AOS ? AosStrBonus : OldStrBonus : m_StrBonus ); }
			set{ m_StrBonus = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int DexBonus
		{
			get{ return ( m_DexBonus == -1 ? Core.AOS ? AosDexBonus : OldDexBonus : m_DexBonus ); }
			set{ m_DexBonus = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int IntBonus
		{
			get{ return ( m_IntBonus == -1 ? Core.AOS ? AosIntBonus : OldIntBonus : m_IntBonus ); }
			set{ m_IntBonus = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int StrRequirement
		{
			get{ return ( m_StrReq == -1 ? Core.AOS ? AosStrReq : OldStrReq : m_StrReq ); }
			set{ m_StrReq = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int DexRequirement
		{
			get{ return ( m_DexReq == -1 ? Core.AOS ? AosDexReq : OldDexReq : m_DexReq ); }
			set{ m_DexReq = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int IntRequirement
		{
			get{ return ( m_IntReq == -1 ? Core.AOS ? AosIntReq : OldIntReq : m_IntReq ); }
			set{ m_IntReq = value; InvalidateProperties(); }
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
		public bool PlayerConstructed
		{
			get{ return m_PlayerConstructed; }
			set{ m_PlayerConstructed = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public CraftResource Resource
		{
			get
			{
				return m_Resource;
			}
			set
			{
				if ( m_Resource != value )
				{
					UnscaleDurability();

					m_Resource = value;
					Hue = CraftResources.GetHue( m_Resource );

					Invalidate();
					InvalidateProperties();

					if ( Parent is Mobile )
						((Mobile)Parent).UpdateResistances();

					ScaleDurability();
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public CraftResource Resource2
		{
			get
			{
				return ExtraCraftResource.Get( this ).Resource2;
			}
			set
			{
				if ( ExtraCraftResource.Get( this ).Resource2 != value )
				{
					UnscaleDurability();

					ExtraCraftResource.Get( this ).Resource2 = value;
					//Hue = CraftResources.GetHue( m_Resource );

					Invalidate();
					InvalidateProperties();

					if ( Parent is Mobile )
						((Mobile)Parent).UpdateResistances();

					ScaleDurability();
				}
			}
		}

		public virtual double ArmorScalar
		{
			get
			{
				int pos = (int)BodyPosition;

				if ( pos >= 0 && pos < m_ArmorScalars.Length )
					return m_ArmorScalars[pos];

				return 1.0;
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int MaxHitPoints
		{
			get{ return m_MaxHitPoints; }
			set{ m_MaxHitPoints = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int HitPoints
		{
			get 
			{
				return m_HitPoints;
			}
			set 
			{
				if ( value != m_HitPoints && MaxHitPoints > 0 )
				{
					m_HitPoints = value;

					if ( m_HitPoints < 0 )
						Delete();
					else if ( m_HitPoints > MaxHitPoints )
						m_HitPoints = MaxHitPoints;

					InvalidateProperties();
				}
			}
		}


		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Crafter
		{
			get{ return m_Crafter; }
			set{ m_Crafter = value; InvalidateProperties(); }
		}

		
		[CommandProperty( AccessLevel.GameMaster )]
		public ArmorQuality Quality
		{
			get{ return m_Quality; }
			set{ UnscaleDurability(); m_Quality = value; Invalidate(); InvalidateProperties(); ScaleDurability(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public ArmorDurabilityLevel Durability
		{
			get{ return m_Durability; }
			set{ UnscaleDurability(); m_Durability = value; ScaleDurability(); InvalidateProperties(); }
		}

		public virtual int ArtifactRarity
		{
			get{ return 0; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public ArmorProtectionLevel ProtectionLevel
		{
			get
			{
				return m_Protection;
			}
			set
			{
				if ( m_Protection != value )
				{
					m_Protection = value;

					Invalidate();
					InvalidateProperties();

					if ( Parent is Mobile )
						((Mobile)Parent).UpdateResistances();
				}
			}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public AosAttributes Attributes
		{
			get{ return m_AosAttributes; }
			set{}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public AosArmorAttributes ArmorAttributes
		{
			get{ return m_AosArmorAttributes; }
			set{}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public AosSkillBonuses SkillBonuses
		{
			get{ return m_AosSkillBonuses; }
			set{}
		}

		public int ComputeStatReq( StatType type )
		{
			int v;

			if ( type == StatType.Str )
				v = StrRequirement;
			else if ( type == StatType.Dex )
				v = DexRequirement;
			else
				v = IntRequirement;

			return AOS.Scale( v, 100 - GetLowerStatReq() );
		}

		public int ComputeStatBonus( StatType type )
		{
			if ( type == StatType.Str )
				return StrBonus + Attributes.BonusStr;
			else if ( type == StatType.Dex )
				return DexBonus + Attributes.BonusDex;
			else
				return IntBonus + Attributes.BonusInt;
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int PhysicalBonus{ get{ return m_PhysicalBonus; } set{ m_PhysicalBonus = value; InvalidateProperties(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int FireBonus{ get{ return m_FireBonus; } set{ m_FireBonus = value; InvalidateProperties(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int ColdBonus{ get{ return m_ColdBonus; } set{ m_ColdBonus = value; InvalidateProperties(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int PoisonBonus{ get{ return m_PoisonBonus; } set{ m_PoisonBonus = value; InvalidateProperties(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int EnergyBonus{ get{ return m_EnergyBonus; } set{ m_EnergyBonus = value; InvalidateProperties(); } }

		public virtual int BasePhysicalResistance{ get{ return 0; } }
		public virtual int BaseFireResistance{ get{ return 0; } }
		public virtual int BaseColdResistance{ get{ return 0; } }
		public virtual int BasePoisonResistance{ get{ return 0; } }
		public virtual int BaseEnergyResistance{ get{ return 0; } }

		public override int PhysicalResistance{ get{ return BasePhysicalResistance + GetProtOffset() + GetResourceAttrs().ArmorPhysicalResist + GetResource2Attrs().ArmorPhysicalResist + m_PhysicalBonus; } }
		public override int FireResistance{ get{ return BaseFireResistance + GetProtOffset() + GetResourceAttrs().ArmorFireResist + GetResource2Attrs().ArmorFireResist + m_FireBonus; } }
		public override int ColdResistance{ get{ return BaseColdResistance + GetProtOffset() + GetResourceAttrs().ArmorColdResist + GetResource2Attrs().ArmorColdResist + m_ColdBonus; } }
		public override int PoisonResistance{ get{ return BasePoisonResistance + GetProtOffset() + GetResourceAttrs().ArmorPoisonResist + GetResource2Attrs().ArmorPoisonResist + m_PoisonBonus; } }
		public override int EnergyResistance{ get{ return BaseEnergyResistance + GetProtOffset() + GetResourceAttrs().ArmorEnergyResist + GetResource2Attrs().ArmorEnergyResist + m_EnergyBonus; } }

        public int AllResistances { get { return PhysicalResistance + FireResistance + ColdResistance + PoisonResistance + EnergyResistance; } }

		public virtual int InitMinHits{ get{ return 0; } }
		public virtual int InitMaxHits{ get{ return 0; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public ArmorBodyType BodyPosition
		{
			get
			{
				switch ( this.Layer )
				{
					default:
					case Layer.Neck:		return ArmorBodyType.Gorget;
					case Layer.TwoHanded:	return ArmorBodyType.Shield;
					case Layer.Gloves:		return ArmorBodyType.Gloves;
					case Layer.Helm:		return ArmorBodyType.Helmet;
					case Layer.Arms:		return ArmorBodyType.Arms;

					case Layer.InnerLegs:
					case Layer.OuterLegs:
					case Layer.Pants:		return ArmorBodyType.Legs;

					case Layer.InnerTorso:
					case Layer.OuterTorso:
					case Layer.Shirt:		return ArmorBodyType.Chest;
				}
			}
		}

        // 22.09.2012 :: zombie :: zmiany w wytrzymalosci zbroi
        [CommandProperty( AccessLevel.GameMaster )]
        public int InitHitPoints
        {
            get
            {
                Dictionary<ArmorMaterialType, int> hits = new Dictionary<ArmorMaterialType, int>();

                hits[ ArmorMaterialType.Leather ]   = 15;
                hits[ ArmorMaterialType.Studded ]   = 19;
                hits[ ArmorMaterialType.Ringmail ]  = 19;
                hits[ ArmorMaterialType.Chainmail ] = 23;
                hits[ ArmorMaterialType.Bone ]      = 27;
                hits[ ArmorMaterialType.Plate ]     = 32;
                hits[ ArmorMaterialType.Dragon ]    = 37;

                int hp = hits.ContainsKey( MaterialType ) ? hits[ MaterialType ] : Utility.RandomMinMax( InitMinHits, InitMaxHits );

                if ( BodyPosition == ArmorBodyType.Shield ) {
					if ( MaterialType == ArmorMaterialType.Plate )
						hp = 40;
					else
						hp = 30;	// drewniana tarcza
				}
                //else if ( BodyPosition == ArmorBodyType.Helmet && MaterialType != ArmorMaterialType.Leather )
                //    hp = 25;
                
                /**
                 * niektore czesci niszcza sie szybciej, dlatego przydzielamy im odpowiednio niewielkie bonusy do max dura:
                 * Gorget + 1, Gloves + 1, Helmet +2, Arms +2, Legs +3, Chest +5
                 */
                if ( ArmorScalar < 1 )
                    hp += (int)(ArmorScalar * 15);

                return hp;
            }
        }
        // zombie

		public void DistributeBonuses( int amount )
		{
			for ( int i = 0; i < amount; ++i )
			{
				switch ( Utility.Random( 5 ) )
				{
					case 0: ++m_PhysicalBonus; break;
					case 1: ++m_FireBonus; break;
					case 2: ++m_ColdBonus; break;
					case 3: ++m_PoisonBonus; break;
					case 4: ++m_EnergyBonus; break;
				}
			}

			InvalidateProperties();
		}

        public CraftAttributeInfo GetResourceAttrs()
        {
            return GetResourceAttrs(m_Resource);
        }
        public CraftAttributeInfo GetResource2Attrs()
        {
            return GetResourceAttrs( Resource2 );
        }
		public CraftAttributeInfo GetResourceAttrs(CraftResource resource)
		{
			CraftResourceInfo info = CraftResources.GetInfo( resource );

			if ( info == null )
				return CraftAttributeInfo.Blank;

			return info.AttributeInfo;
		}

		public int GetProtOffset()
		{
			switch ( m_Protection )
			{
				case ArmorProtectionLevel.Guarding: return 1;
				case ArmorProtectionLevel.Hardening: return 2;
				case ArmorProtectionLevel.Fortification: return 3;
				case ArmorProtectionLevel.Invulnerability: return 4;
			}

			return 0;
		}

		public void UnscaleDurability()
		{
			int scale = 100 + GetDurabilityBonus();

			m_HitPoints = ((m_HitPoints * 100) + (scale - 1)) / scale;
			m_MaxHitPoints = ((m_MaxHitPoints * 100) + (scale - 1)) / scale;
			InvalidateProperties();
		}

		public void ScaleDurability()
		{
			int scale = 100 + GetDurabilityBonus();

			m_HitPoints = ((m_HitPoints * scale) + 99) / 100;
			m_MaxHitPoints = ((m_MaxHitPoints * scale) + 99) / 100;
			InvalidateProperties();
		}

		public int GetDurabilityBonusId(bool identified)
		{
			int bonus = 0;

			if ( m_Quality == ArmorQuality.Exceptional )
				bonus += 20;

			switch ( m_Durability )
			{
				case ArmorDurabilityLevel.Durable: bonus += 20; break;
				case ArmorDurabilityLevel.Substantial: bonus += 50; break;
				case ArmorDurabilityLevel.Massive: bonus += 70; break;
				case ArmorDurabilityLevel.Fortified: bonus += 100; break;
				case ArmorDurabilityLevel.Indestructible: bonus += 120; break;
			}

			if ( Core.AOS )
			{
				bonus += m_AosArmorAttributes.DurabilityBonusId(identified);

				CraftResourceInfo resInfo = CraftResources.GetInfo( m_Resource );
				CraftAttributeInfo attrInfo = null;
				if ( resInfo != null )
					attrInfo = resInfo.AttributeInfo;
                if ( attrInfo != null )
                    bonus += attrInfo.ArmorDurability;

				CraftResourceInfo resInfo2 = CraftResources.GetInfo( Resource2 );
				CraftAttributeInfo attrInfo2 = null;
				if ( resInfo2 != null )
					attrInfo2 = resInfo2.AttributeInfo;
                if ( attrInfo2 != null )
                    bonus += attrInfo2.ArmorDurability;
			}

			return bonus;
		}
		
		public int GetDurabilityBonus()
		{
			return GetDurabilityBonusId(m_Identified);
		}

		public bool Scissor( Mobile from, Scissors scissors )
		{
			if ( !IsChildOf( from.Backpack ) )
			{
				from.SendLocalizedMessage( 502437 ); // Items you wish to cut must be in your backpack.
				return false;
			}

			if ( Ethics.Ethic.IsImbued( this ) )
			{
				from.SendLocalizedMessage( 502440 ); // Scissors can not be used on that to produce anything.
				return false;
			}

			CraftSystem system = DefTailoring.CraftSystem;

			CraftItem item = system.CraftItems.SearchFor( GetType() );

			if ( item != null && item.Ressources.Count >= 1 && item.Ressources.GetAt( 0 ).Amount >= 2 )
			{
				try
				{
					foreach (Type resType in CraftResources.GetInfo(m_Resource).ResourceTypes)
					{
						Item res = (Item)Activator.CreateInstance(resType);

						ScissorHelper(from, res, m_PlayerConstructed ? (item.Ressources.GetAt(0).Amount / 2) : 1);
					}
					return true;
				}
				catch
				{
				}
			}

			from.SendLocalizedMessage( 502440 ); // Scissors can not be used on that to produce anything.
			return false;
		}

		private static double[] m_ArmorScalars = { 0.09, 0.09, 0.14, 0.17, 0.21, 0.30 };

		public static double[] ArmorScalars
		{
			get
			{
				return m_ArmorScalars;
			}
			set
			{
				m_ArmorScalars = value;
			}
		}

		public static void ValidateMobile( Mobile m )
		{
			for ( int i = m.Items.Count - 1; i >= 0; --i )
			{
				if ( i >= m.Items.Count )
					continue;

				Item item = m.Items[i];

				if ( item is BaseArmor )
				{
					BaseArmor armor = (BaseArmor)item;

					if( armor.RequiredRace != null && m.Race != armor.RequiredRace )
					{
						/*if( armor.RequiredRace == Race.Elf )
							m.SendLocalizedMessage( 1072203 ); // Only Elves may use this.
						else*/
							m.SendMessage( "Only {0} may use this.", armor.RequiredRace.PluralName );

						m.AddToBackpack( armor );
					}
					else if ( !armor.AllowMaleWearer && !m.Female && m.AccessLevel < AccessLevel.GameMaster )
					{
						if ( armor.AllowFemaleWearer )
							m.SendLocalizedMessage( 1010388 ); // Only females can wear this.
						else
							m.SendMessage( "You may not wear this." );

						m.AddToBackpack( armor );
					}
					else if ( !armor.AllowFemaleWearer && m.Female && m.AccessLevel < AccessLevel.GameMaster )
					{
						if ( armor.AllowMaleWearer )
							m.SendLocalizedMessage( 1063343 ); // Only males can wear this.
						else
							m.SendMessage( "You may not wear this." );

						m.AddToBackpack( armor );
					}
				}
			}
		}

		public int GetLowerStatReqId(bool identified)
		{
			if ( !Core.AOS )
				return 0;

			int v = m_AosArmorAttributes.LowerStatReqId(identified);

			CraftResourceInfo info = CraftResources.GetInfo( m_Resource );
			if ( info != null )
			{
				CraftAttributeInfo attrInfo = info.AttributeInfo;
				if ( attrInfo != null )
					v += attrInfo.ArmorLowerRequirements;
			}

			CraftResourceInfo info2 = CraftResources.GetInfo( Resource2 );
			if ( info2 != null )
			{
				CraftAttributeInfo attrInfo2 = info2.AttributeInfo;
				if ( attrInfo2 != null )
					v += attrInfo2.ArmorLowerRequirements;
			}

			if ( v > 100 )
				v = 100;

			return v;
		}
		
		public int GetLowerStatReq()
		{
			return GetLowerStatReqId(m_Identified);
		}

		public override void OnAdded( object parent )
		{
			if ( parent is Mobile )
			{
				Mobile from = (Mobile)parent;

				if ( Core.AOS )
					m_AosSkillBonuses.AddTo( from, m_Identified );

				from.Delta( MobileDelta.Armor ); // Tell them armor rating has changed
			}
		}

		public virtual double ScaleArmorByDurability( double armor )
		{
			int scale = 100;

			if ( m_MaxHitPoints > 0 && m_HitPoints < m_MaxHitPoints )
				scale = 50 + ((50 * m_HitPoints) / m_MaxHitPoints);

			return ( armor * scale ) / 100;
		}

		protected void Invalidate()
		{
			if ( Parent is Mobile )
				((Mobile)Parent).Delta( MobileDelta.Armor ); // Tell them armor rating has changed
		}

		public BaseArmor( Serial serial ) :  base( serial )
		{
		}

		private static void SetSaveFlag( ref SaveFlag flags, SaveFlag toSet, bool setIf )
		{
			if ( setIf )
				flags |= toSet;
		}

		private static bool GetSaveFlag( SaveFlag flags, SaveFlag toGet )
		{
			return ( (flags & toGet) != 0 );
		}

		[Flags]
		private enum SaveFlag
		{
			None				= 0x00000000,
			Attributes			= 0x00000001,
			ArmorAttributes		= 0x00000002,
			PhysicalBonus		= 0x00000004,
			FireBonus			= 0x00000008,
			ColdBonus			= 0x00000010,
			PoisonBonus			= 0x00000020,
			EnergyBonus			= 0x00000040,
			Identified			= 0x00000080,
			MaxHitPoints		= 0x00000100,
			HitPoints			= 0x00000200,
			Crafter				= 0x00000400,
			Quality				= 0x00000800,
			Durability			= 0x00001000,
			Protection			= 0x00002000,
			Resource			= 0x00004000,
			BaseArmor			= 0x00008000,
			StrBonus			= 0x00010000,
			DexBonus			= 0x00020000,
			IntBonus			= 0x00040000,
			StrReq				= 0x00080000,
			DexReq				= 0x00100000,
			IntReq				= 0x00200000,
			MedAllowance		= 0x00400000,
			SkillBonuses		= 0x00800000,
			PlayerConstructed	= 0x01000000
			//IdentifiedLevel		= 0x02000000,
            //Resource2			= 0x04000000
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 7 ); // version

			SaveFlag flags = SaveFlag.None;

			SetSaveFlag( ref flags, SaveFlag.Attributes,		!m_AosAttributes.IsEmpty );
			SetSaveFlag( ref flags, SaveFlag.ArmorAttributes,	!m_AosArmorAttributes.IsEmpty );
			SetSaveFlag( ref flags, SaveFlag.PhysicalBonus,		m_PhysicalBonus != 0 );
			SetSaveFlag( ref flags, SaveFlag.FireBonus,			m_FireBonus != 0 );
			SetSaveFlag( ref flags, SaveFlag.ColdBonus,			m_ColdBonus != 0 );
			SetSaveFlag( ref flags, SaveFlag.PoisonBonus,		m_PoisonBonus != 0 );
			SetSaveFlag( ref flags, SaveFlag.EnergyBonus,		m_EnergyBonus != 0 );
			SetSaveFlag( ref flags, SaveFlag.Identified,		m_Identified != false );
			//SetSaveFlag( ref flags, SaveFlag.IdentifiedLevel,		m_IdentifiedLevel != IdLevel.None );
			SetSaveFlag( ref flags, SaveFlag.MaxHitPoints,		m_MaxHitPoints != 0 );
			SetSaveFlag( ref flags, SaveFlag.HitPoints,			m_HitPoints != 0 );
			SetSaveFlag( ref flags, SaveFlag.Crafter,			m_Crafter != null );
			SetSaveFlag( ref flags, SaveFlag.Quality,			m_Quality != ArmorQuality.Regular );
			SetSaveFlag( ref flags, SaveFlag.Durability,		m_Durability != ArmorDurabilityLevel.Regular );
			SetSaveFlag( ref flags, SaveFlag.Protection,		m_Protection != ArmorProtectionLevel.Regular );
			SetSaveFlag( ref flags, SaveFlag.Resource,			m_Resource != DefaultResource );
			SetSaveFlag( ref flags, SaveFlag.BaseArmor,			m_ArmorBase != -1 );
			SetSaveFlag( ref flags, SaveFlag.StrBonus,			m_StrBonus != -1 );
			SetSaveFlag( ref flags, SaveFlag.DexBonus,			m_DexBonus != -1 );
			SetSaveFlag( ref flags, SaveFlag.IntBonus,			m_IntBonus != -1 );
			SetSaveFlag( ref flags, SaveFlag.StrReq,			m_StrReq != -1 );
			SetSaveFlag( ref flags, SaveFlag.DexReq,			m_DexReq != -1 );
			SetSaveFlag( ref flags, SaveFlag.IntReq,			m_IntReq != -1 );
			SetSaveFlag( ref flags, SaveFlag.MedAllowance,		m_Meditate != (AMA)(-1) );
			SetSaveFlag( ref flags, SaveFlag.SkillBonuses,		!m_AosSkillBonuses.IsEmpty );
			SetSaveFlag( ref flags, SaveFlag.PlayerConstructed,	m_PlayerConstructed != false );
            //SetSaveFlag( ref flags, SaveFlag.Resource2,			m_Resource2 != DefaultResource2 );

			writer.WriteEncodedInt( (int) flags );

			if ( GetSaveFlag( flags, SaveFlag.Attributes ) )
				m_AosAttributes.Serialize( writer );

			if ( GetSaveFlag( flags, SaveFlag.ArmorAttributes ) )
				m_AosArmorAttributes.Serialize( writer );

			if ( GetSaveFlag( flags, SaveFlag.PhysicalBonus ) )
				writer.WriteEncodedInt( (int) m_PhysicalBonus );

			if ( GetSaveFlag( flags, SaveFlag.FireBonus ) )
				writer.WriteEncodedInt( (int) m_FireBonus );

			if ( GetSaveFlag( flags, SaveFlag.ColdBonus ) )
				writer.WriteEncodedInt( (int) m_ColdBonus );

			if ( GetSaveFlag( flags, SaveFlag.PoisonBonus ) )
				writer.WriteEncodedInt( (int) m_PoisonBonus );

			if ( GetSaveFlag( flags, SaveFlag.EnergyBonus ) )
				writer.WriteEncodedInt( (int) m_EnergyBonus );

			if ( GetSaveFlag( flags, SaveFlag.MaxHitPoints ) )
				writer.WriteEncodedInt( (int) m_MaxHitPoints );

			if ( GetSaveFlag( flags, SaveFlag.HitPoints ) )
				writer.WriteEncodedInt( (int) m_HitPoints );

			if ( GetSaveFlag( flags, SaveFlag.Crafter ) )
				writer.Write( (Mobile) m_Crafter );

			if ( GetSaveFlag( flags, SaveFlag.Quality ) )
				writer.WriteEncodedInt( (int) m_Quality );

			if ( GetSaveFlag( flags, SaveFlag.Durability ) )
				writer.WriteEncodedInt( (int) m_Durability );

			if ( GetSaveFlag( flags, SaveFlag.Protection ) )
				writer.WriteEncodedInt( (int) m_Protection );

			if ( GetSaveFlag( flags, SaveFlag.Resource ) )
				writer.WriteEncodedInt( (int) m_Resource );

			if ( GetSaveFlag( flags, SaveFlag.BaseArmor ) )
				writer.WriteEncodedInt( (int) m_ArmorBase );

			if ( GetSaveFlag( flags, SaveFlag.StrBonus ) )
				writer.WriteEncodedInt( (int) m_StrBonus );

			if ( GetSaveFlag( flags, SaveFlag.DexBonus ) )
				writer.WriteEncodedInt( (int) m_DexBonus );

			if ( GetSaveFlag( flags, SaveFlag.IntBonus ) )
				writer.WriteEncodedInt( (int) m_IntBonus );

			if ( GetSaveFlag( flags, SaveFlag.StrReq ) )
				writer.WriteEncodedInt( (int) m_StrReq );

			if ( GetSaveFlag( flags, SaveFlag.DexReq ) )
				writer.WriteEncodedInt( (int) m_DexReq );

			if ( GetSaveFlag( flags, SaveFlag.IntReq ) )
				writer.WriteEncodedInt( (int) m_IntReq );

			if ( GetSaveFlag( flags, SaveFlag.MedAllowance ) )
				writer.WriteEncodedInt( (int) m_Meditate );

			if ( GetSaveFlag( flags, SaveFlag.SkillBonuses ) )
				m_AosSkillBonuses.Serialize( writer );
				
			//if( GetSaveFlag( flags, SaveFlag.IdentifiedLevel ) )
			//	writer.Write( (int)m_IdentifiedLevel );
				
			//if ( GetSaveFlag( flags, SaveFlag.Resource2 ) )
			//	writer.WriteEncodedInt( (int) m_Resource2 );

			//writer.Write( (int)m_IdentifiedLevel );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 9:
				case 8:
				case 7:
				case 6:
				case 5:
				{
					SaveFlag flags = (SaveFlag)reader.ReadEncodedInt();

					if ( GetSaveFlag( flags, SaveFlag.Attributes ) )
						m_AosAttributes = new AosAttributes( this, reader );
					else
						m_AosAttributes = new AosAttributes( this );

					if ( GetSaveFlag( flags, SaveFlag.ArmorAttributes ) )
						m_AosArmorAttributes = new AosArmorAttributes( this, reader );
					else
						m_AosArmorAttributes = new AosArmorAttributes( this );

					if ( GetSaveFlag( flags, SaveFlag.PhysicalBonus ) )
						m_PhysicalBonus = reader.ReadEncodedInt();

					if ( GetSaveFlag( flags, SaveFlag.FireBonus ) )
						m_FireBonus = reader.ReadEncodedInt();

					if ( GetSaveFlag( flags, SaveFlag.ColdBonus ) )
						m_ColdBonus = reader.ReadEncodedInt();

					if ( GetSaveFlag( flags, SaveFlag.PoisonBonus ) )
						m_PoisonBonus = reader.ReadEncodedInt();

					if ( GetSaveFlag( flags, SaveFlag.EnergyBonus ) )
						m_EnergyBonus = reader.ReadEncodedInt();

					if ( GetSaveFlag( flags, SaveFlag.Identified ) )
						m_Identified = ( version >= 7 || reader.ReadBool() );

					if ( GetSaveFlag( flags, SaveFlag.MaxHitPoints ) )
						m_MaxHitPoints = reader.ReadEncodedInt();

					if ( GetSaveFlag( flags, SaveFlag.HitPoints ) )
						m_HitPoints = reader.ReadEncodedInt();

					if ( GetSaveFlag( flags, SaveFlag.Crafter ) )
						m_Crafter = reader.ReadMobile();

					if ( GetSaveFlag( flags, SaveFlag.Quality ) )
						m_Quality = (ArmorQuality)reader.ReadEncodedInt();
					else
						m_Quality = ArmorQuality.Regular;

					if ( version == 5 && m_Quality == ArmorQuality.Low )
						m_Quality = ArmorQuality.Regular;

					if ( GetSaveFlag( flags, SaveFlag.Durability ) )
					{
						m_Durability = (ArmorDurabilityLevel)reader.ReadEncodedInt();

						if ( m_Durability > ArmorDurabilityLevel.Indestructible )
							m_Durability = ArmorDurabilityLevel.Durable;
					}

					if ( GetSaveFlag( flags, SaveFlag.Protection ) )
					{
						m_Protection = (ArmorProtectionLevel)reader.ReadEncodedInt();

						if ( m_Protection > ArmorProtectionLevel.Invulnerability )
							m_Protection = ArmorProtectionLevel.Defense;
					}

					if ( GetSaveFlag( flags, SaveFlag.Resource ) )
						m_Resource = (CraftResource)reader.ReadEncodedInt();
					else
						m_Resource = DefaultResource;

					if ( m_Resource == CraftResource.None )
						m_Resource = DefaultResource;

					if ( GetSaveFlag( flags, SaveFlag.BaseArmor ) )
						m_ArmorBase = reader.ReadEncodedInt();
					else
						m_ArmorBase = -1;

					if ( GetSaveFlag( flags, SaveFlag.StrBonus ) )
						m_StrBonus = reader.ReadEncodedInt();
					else
						m_StrBonus = -1;

					if ( GetSaveFlag( flags, SaveFlag.DexBonus ) )
						m_DexBonus = reader.ReadEncodedInt();
					else
						m_DexBonus = -1;

					if ( GetSaveFlag( flags, SaveFlag.IntBonus ) )
						m_IntBonus = reader.ReadEncodedInt();
					else
						m_IntBonus = -1;

					if ( GetSaveFlag( flags, SaveFlag.StrReq ) )
						m_StrReq = reader.ReadEncodedInt();
					else
						m_StrReq = -1;

					if ( GetSaveFlag( flags, SaveFlag.DexReq ) )
						m_DexReq = reader.ReadEncodedInt();
					else
						m_DexReq = -1;

					if ( GetSaveFlag( flags, SaveFlag.IntReq ) )
						m_IntReq = reader.ReadEncodedInt();
					else
						m_IntReq = -1;

					if ( GetSaveFlag( flags, SaveFlag.MedAllowance ) )
						m_Meditate = (AMA)reader.ReadEncodedInt();
					else
						m_Meditate = (AMA)(-1);

					if ( GetSaveFlag( flags, SaveFlag.SkillBonuses ) )
						m_AosSkillBonuses = new AosSkillBonuses( this, reader );

					if ( GetSaveFlag( flags, SaveFlag.PlayerConstructed ) )
						m_PlayerConstructed = true;

					if ( version > 7 && version < 9 )
					{
						if ( GetSaveFlag( flags, (SaveFlag)0x02000000 ) )
							/*m_IdentifiedLevel = (IdLevel)*/reader.ReadInt();

						/*m_IdentifiedLevel = (IdLevel)*/reader.ReadInt();

						if ( GetSaveFlag( flags, (SaveFlag)0x04000000 ) )
							Resource2 = (CraftResource)reader.ReadEncodedInt();
						else
							Resource2 = DefaultResource2;

						if ( Resource2 == CraftResource.None )
							Resource2 = DefaultResource2;
					}

					break;
				}
				case 4:
				{
					m_AosAttributes = new AosAttributes( this, reader );
					m_AosArmorAttributes = new AosArmorAttributes( this, reader );
					goto case 3;
				}
				case 3:
				{
					m_PhysicalBonus = reader.ReadInt();
					m_FireBonus = reader.ReadInt();
					m_ColdBonus = reader.ReadInt();
					m_PoisonBonus = reader.ReadInt();
					m_EnergyBonus = reader.ReadInt();
					goto case 2;
				}
				case 2:
				case 1:
				{
					m_Identified = reader.ReadBool();

					goto case 0;
				}
				case 0:
				{
					m_ArmorBase = reader.ReadInt();
					m_MaxHitPoints = reader.ReadInt();
					m_HitPoints = reader.ReadInt();
					m_Crafter = reader.ReadMobile();
					m_Quality = (ArmorQuality)reader.ReadInt();
					m_Durability = (ArmorDurabilityLevel)reader.ReadInt();
					m_Protection = (ArmorProtectionLevel)reader.ReadInt();

					AMT mat = (AMT)reader.ReadInt();

					if ( m_ArmorBase == RevertArmorBase )
						m_ArmorBase = -1;

					/*m_BodyPos = (ArmorBodyType)*/reader.ReadInt();

					if ( version < 4 )
					{
						m_AosAttributes = new AosAttributes( this );
						m_AosArmorAttributes = new AosArmorAttributes( this );
					}

					if ( version < 3 && m_Quality == ArmorQuality.Exceptional )
						DistributeBonuses( 6 );

					if ( version >= 2 )
					{
						m_Resource = (CraftResource)reader.ReadInt();
					}
					else
					{
						OreInfo info;

						switch ( reader.ReadInt() )
						{
							default:
							case 0: info = OreInfo.Iron; break;
							case 1: info = OreInfo.DullCopper; break;
							case 2: info = OreInfo.ShadowIron; break;
							case 3: info = OreInfo.Copper; break;
							case 4: info = OreInfo.Bronze; break;
							case 5: info = OreInfo.Gold; break;
							case 6: info = OreInfo.Agapite; break;
							case 7: info = OreInfo.Verite; break;
							case 8: info = OreInfo.Valorite; break;
							}

						m_Resource = CraftResources.GetFromOreInfo( info, mat );
					}

					m_StrBonus = reader.ReadInt();
					m_DexBonus = reader.ReadInt();
					m_IntBonus = reader.ReadInt();
					m_StrReq = reader.ReadInt();
					m_DexReq = reader.ReadInt();
					m_IntReq = reader.ReadInt();

					if ( m_StrBonus == OldStrBonus )
						m_StrBonus = -1;

					if ( m_DexBonus == OldDexBonus )
						m_DexBonus = -1;

					if ( m_IntBonus == OldIntBonus )
						m_IntBonus = -1;

					if ( m_StrReq == OldStrReq )
						m_StrReq = -1;

					if ( m_DexReq == OldDexReq )
						m_DexReq = -1;

					if ( m_IntReq == OldIntReq )
						m_IntReq = -1;

					m_Meditate = (AMA)reader.ReadInt();

					if ( m_Meditate == OldMedAllowance )
						m_Meditate = (AMA)(-1);

					if ( m_Resource == CraftResource.None )
					{
						if ( mat == ArmorMaterialType.Studded || mat == ArmorMaterialType.Leather )
							m_Resource = CraftResource.RegularLeather;
						else if ( mat == ArmorMaterialType.Spined )
							m_Resource = CraftResource.SpinedLeather;
						else if ( mat == ArmorMaterialType.Horned )
							m_Resource = CraftResource.HornedLeather;
						else if ( mat == ArmorMaterialType.Barbed )
							m_Resource = CraftResource.BarbedLeather;
						else
							m_Resource = CraftResource.Iron;
					}

                    // 22.06.2012 :: zombie 
                    if ( m_MaxHitPoints == 0 && m_HitPoints == 0 )
                        m_HitPoints = m_MaxHitPoints = InitHitPoints; // Utility.RandomMinMax( InitMinHits, InitMaxHits );
                    // zombie

					break;
				}
			}

			if ( m_AosSkillBonuses == null )
				m_AosSkillBonuses = new AosSkillBonuses( this );

			if ( Core.AOS && Parent is Mobile )
				m_AosSkillBonuses.AddTo( (Mobile)Parent, m_Identified );

			int strBonus = ComputeStatBonus( StatType.Str );
			int dexBonus = ComputeStatBonus( StatType.Dex );
			int intBonus = ComputeStatBonus( StatType.Int );

			if ( Parent is Mobile && (strBonus != 0 || dexBonus != 0 || intBonus != 0) )
			{
				Mobile m = (Mobile)Parent;

				string modName = Serial.ToString();

				if ( strBonus != 0 )
					m.AddStatMod( new StatMod( StatType.Str, modName + "Str", strBonus, TimeSpan.Zero ) );

				if ( dexBonus != 0 )
					m.AddStatMod( new StatMod( StatType.Dex, modName + "Dex", dexBonus, TimeSpan.Zero ) );

				if ( intBonus != 0 )
					m.AddStatMod( new StatMod( StatType.Int, modName + "Int", intBonus, TimeSpan.Zero ) );
			}

			if ( Parent is Mobile )
				((Mobile)Parent).CheckStatTimers();

			if ( version < 7 )
				m_PlayerConstructed = true; // we don't know, so, assume it's crafted
		}

		public virtual CraftResource DefaultResource{ get{ return CraftResource.Iron; } }
        public virtual CraftResource DefaultResource2{ get{ return CraftResource.None; } }

		public BaseArmor( int itemID ) :  base( itemID )
		{
			m_Quality = ArmorQuality.Regular;
			m_Durability = ArmorDurabilityLevel.Regular;
			m_Crafter = null;

			m_Resource = DefaultResource;
			ExtraCraftResource.Get( this ).Resource2 = DefaultResource2;
			Hue = CraftResources.GetHue( m_Resource );

			this.Layer = (Layer)ItemData.Quality;
            // 22.09.2012 :: zombie :: zmiany w wytrzymalosci zbroi
            if (InitMinHits == 0 || InitMaxHits == 0)
            {
                m_HitPoints = m_MaxHitPoints = InitHitPoints;
            }
            else
            {
                m_HitPoints = m_MaxHitPoints = Utility.RandomMinMax(InitMinHits, InitMaxHits);
            }
            // zombie
            
			m_AosAttributes = new AosAttributes( this );
			m_AosArmorAttributes = new AosArmorAttributes( this );
			m_AosSkillBonuses = new AosSkillBonuses( this );
		}

        public override bool AllowSecureTrade( Mobile from, Mobile to, Mobile newOwner, bool accepted )
		{
			if ( !Ethics.Ethic.CheckTrade( from, to, newOwner, this ) )
				return false;

			return base.AllowSecureTrade( from, to, newOwner, accepted );
		}

		public virtual Race RequiredRace { get { return null; } }

		public override bool CanEquip( Mobile from )
		{
			if( !Ethics.Ethic.CheckEquip( from, this ) )
				return false;

			if( from.AccessLevel < AccessLevel.GameMaster )
			{
				if( RequiredRace != null && from.Race != RequiredRace )
				{
					/*if( RequiredRace == Race.Elf )
						from.SendLocalizedMessage( 1072203 ); // Only Elves may use this.
					else*/
						from.SendMessage( "Only {0} may use this.", RequiredRace.PluralName );

					return false;
				}
				else if( !AllowMaleWearer && !from.Female )
				{
					if( AllowFemaleWearer )
						from.SendLocalizedMessage( 1010388 ); // Only females can wear this.
					else
						from.SendMessage( "You may not wear this." );

					return false;
				}
				else if( !AllowFemaleWearer && from.Female )
				{
					if( AllowMaleWearer )
						from.SendLocalizedMessage( 1063343 ); // Only males can wear this.
					else
						from.SendMessage( "You may not wear this." );

					return false;
				}
				else
				{
					int strBonus = ComputeStatBonus( StatType.Str ), strReq = ComputeStatReq( StatType.Str );
					int dexBonus = ComputeStatBonus( StatType.Dex ), dexReq = ComputeStatReq( StatType.Dex );
					int intBonus = ComputeStatBonus( StatType.Int ), intReq = ComputeStatReq( StatType.Int );

					if( from.Dex < dexReq || (from.Dex + dexBonus) < 1 )
					{
						from.SendLocalizedMessage( 502077 ); // You do not have enough dexterity to equip this item.
						return false;
					}
					else if( from.Str < strReq || (from.Str + strBonus) < 1 )
					{
						from.SendLocalizedMessage( 500213 ); // You are not strong enough to equip that.
						return false;
					}
					else if( from.Int < intReq || (from.Int + intBonus) < 1 )
					{
						from.SendMessage( "You are not smart enough to equip that." );
						return false;
					}
				}
			}

			// XmlAttachment check for CanEquip
			if(!Server.Engines.XmlSpawner2.XmlAttach.CheckCanEquip(this, from))
			{
				return false;
			} 
			else
			{
				return base.CanEquip( from );
			}
		}

		public override bool CheckPropertyConfliction( Mobile m )
		{
			if ( base.CheckPropertyConfliction( m ) )
				return true;

			if ( Layer == Layer.Pants )
				return ( m.FindItemOnLayer( Layer.InnerLegs ) != null );

			if ( Layer == Layer.Shirt )
				return ( m.FindItemOnLayer( Layer.InnerTorso ) != null );

			return false;
		}

		public override bool OnEquip( Mobile from )
		{
			from.CheckStatTimers();

			int strBonus = ComputeStatBonus( StatType.Str );
			int dexBonus = ComputeStatBonus( StatType.Dex );
			int intBonus = ComputeStatBonus( StatType.Int );

			if ( strBonus != 0 || dexBonus != 0 || intBonus != 0 )
			{
				string modName = this.Serial.ToString();

				if ( strBonus != 0 )
					from.AddStatMod( new StatMod( StatType.Str, modName + "Str", strBonus, TimeSpan.Zero ) );

				if ( dexBonus != 0 )
					from.AddStatMod( new StatMod( StatType.Dex, modName + "Dex", dexBonus, TimeSpan.Zero ) );

				if ( intBonus != 0 )
					from.AddStatMod( new StatMod( StatType.Int, modName + "Int", intBonus, TimeSpan.Zero ) );
			}

			// XmlAttachment check for OnEquip
			Server.Engines.XmlSpawner2.XmlAttach.CheckOnEquip(this, from);

			return base.OnEquip( from );
		}

		public override void OnRemoved( object parent )
		{
			if ( parent is Mobile )
			{
				Mobile m = (Mobile)parent;
				string modName = this.Serial.ToString();

				m.RemoveStatMod( modName + "Str" );
				m.RemoveStatMod( modName + "Dex" );
				m.RemoveStatMod( modName + "Int" );

				if ( Core.AOS )
					m_AosSkillBonuses.Remove();

				((Mobile)parent).Delta( MobileDelta.Armor ); // Tell them armor rating has changed
				m.CheckStatTimers();
			}

			// XmlAttachment check for OnRemoved
			Server.Engines.XmlSpawner2.XmlAttach.CheckOnRemoved(this, parent);

			base.OnRemoved( parent );
		}

		public virtual int OnHit( BaseWeapon weapon, int damageTaken )
		{
			double HalfAr = ArmorRating / 2.0;
            int Absorbed = (int)( HalfAr + HalfAr * Utility.RandomDouble() );

			damageTaken -= Absorbed;
			if ( damageTaken < 0 ) 
				damageTaken = 0;

			if ( Absorbed < 2 )
				Absorbed = 2;

			if ( Config.ItemDurabilityLostChance > Utility.Random( 100 ) )
			{
				if ( Core.AOS && m_AosArmorAttributes.SelfRepairId(m_Identified) > Utility.Random( 10 ) )
				{
					HitPoints += 2;
				}
				else
				{
					int wear;

					if ( weapon.Type == WeaponType.Bashing )
						wear = Absorbed / 2;
					else
						wear = Utility.Random( 2 );

					if ( wear > 0 && m_MaxHitPoints > 0 )
					{
						if ( m_HitPoints >= wear )
						{
							HitPoints -= wear;
							wear = 0;
						}
						else
						{
							wear -= HitPoints;
							HitPoints = 0;
						}

						if (m_HitPoints < 5 && Parent is Mobile) {
							((Mobile)Parent).LocalOverheadMessage(MessageType.Regular, 0x3B2, 1061183); // Twoja zbroja sie rozpada!
						}

						if ( wear > 0 )
						{
							if ( m_MaxHitPoints > wear )
							{
								MaxHitPoints -= wear;
							}
							else
							{
								Delete();
							}
						}
					}
				}
			}

			return damageTaken;
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

        public static int ResourceNameNumber(CraftResource res)
        {
            int oreType;
            switch (res)
            {
                case CraftResource.Iron:			oreType = 1032596; break; // iron
                case CraftResource.DullCopper:		oreType = 1053108; break; // dull copper
                case CraftResource.ShadowIron:		oreType = 1053107; break; // shadow iron
                case CraftResource.Copper:			oreType = 1053106; break; // copper
                case CraftResource.Bronze:			oreType = 1053105; break; // bronze
                case CraftResource.Gold:			oreType = 1053104; break; // golden
                case CraftResource.Agapite:			oreType = 1053103; break; // agapite
                case CraftResource.Verite:			oreType = 1053102; break; // verite
                case CraftResource.Valorite:		oreType = 1053101; break; // valorite
                case CraftResource.Platinum:		oreType = 1097280; break; // z platyny
                case CraftResource.RegularLeather:  oreType = 1032598; break; // leather
                case CraftResource.SpinedLeather:	oreType = 1061118; break; // spined
                case CraftResource.HornedLeather:	oreType = 1061117; break; // horned
                case CraftResource.BarbedLeather:	oreType = 1061116; break; // barbed
                case CraftResource.RedScales:		oreType = 1061049; break; // red
                case CraftResource.YellowScales:	oreType = 1061050; break; // yellow
                case CraftResource.BlackScales:		oreType = 1061051; break; // black
                case CraftResource.GreenScales:		oreType = 1061052; break; // green
                case CraftResource.WhiteScales:		oreType = 1061053; break; // white
                case CraftResource.BlueScales:		oreType = 1061054; break; // blue
                case CraftResource.RegularWood:		oreType = 1032597; break; // plain wood
                case CraftResource.OakWood:			oreType = 1072533; break; // oak
                case CraftResource.AshWood:			oreType = 1072534; break; // ash
                case CraftResource.YewWood:			oreType = 1072535; break; // yew
                case CraftResource.Heartwood:		oreType = 1072536; break; // heartwood
                case CraftResource.Bloodwood:		oreType = 1072538; break; // bloodwood
                case CraftResource.Frostwood:		oreType = 1072539; break; // frostwood
                default: oreType = 0; break;
            }
            return oreType;
        }

		public bool IsCraftable() {
			CraftSystem[] craftSystems = { DefBlacksmithy.CraftSystem, DefBowFletching.CraftSystem, DefCarpentry.CraftSystem, DefTailoring.CraftSystem };
			foreach(CraftSystem craftSystem in craftSystems) {
				CraftItem craftItem = craftSystem.CraftItems.SearchFor(this.GetType());
				if (craftItem != null && craftItem.Ressources.Count != 0) return true;
			}
			return false;
		}

		public override void AddNameProperty( ObjectPropertyList list )
		{
            //list.Add(1053099, "#{0}\t{1}", oreType, GetNameString()); // ~1_oretype~ ~2_armortype~
            if(Name == null)
                list.Add(LabelNumber);
            else
                list.Add(Name);

			if (IsCraftable()) {
				int oreType = ResourceNameNumber(m_Resource);
				if (oreType != 0)
					list.Add(oreType);

				oreType = ResourceNameNumber( Resource2 );
				if (oreType != 0)
					list.Add(oreType);

				if (MaterialType == ArmorMaterialType.Bone) // wyjatkowo:
					list.Add(1061120);  // 1061120: z kosci
			}
		}

		public override bool AllowEquipedCast( Mobile from )
		{
			if ( base.AllowEquipedCast( from ) )
				return true;

			return ( m_AosAttributes.SpellChanneling != 0 );
		}

		public virtual int GetLuckBonus()
		{
            int bonus = 0;

			CraftResourceInfo resInfo = CraftResources.GetInfo( m_Resource );
			if ( resInfo != null )
            {
			    CraftAttributeInfo attrInfo = resInfo.AttributeInfo;
			    if ( attrInfo!= null )
				    bonus += attrInfo.ArmorLuck;
            }

			CraftResourceInfo resInfo2 = CraftResources.GetInfo( Resource2 );
			if ( resInfo2 != null )
            {
			    CraftAttributeInfo attrInfo2 = resInfo2.AttributeInfo;
			    if ( attrInfo2!= null )
				    bonus += attrInfo2.ArmorLuck;
            }

			return bonus;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

            // szczaw :: 2013.02.02 :: poprawka gdzie jest wyswietlana informacja na temat jakosci.
            if(m_Quality == ArmorQuality.Exceptional)
                list.Add(1060636); // exceptional

			#region Automatyczna identyfikacja jesli przedmiot jest wykonany przez gracza
			if ( m_PlayerConstructed == true || !Config.ItemIDSystemEnabled )
			{
				m_Identified = true;
			}
			#endregion
			
			int prop;

			#region Factions
			if ( m_FactionState != null )
				list.Add( 1041350 ); // faction item
			#endregion
			
			#region Propsy zawsze wyswietlane
			if ( m_Crafter != null )
				list.Add( 1050043, m_Crafter.Name ); // crafted by ~1_NAME~

			/*if( RequiredRace == Race.Elf )
				list.Add( 1075086 ); // Elves Only*/
				
			if ( (prop = ArtifactRarity) > 0 )
				list.Add( 1061078, prop.ToString() ); // artifact rarity ~1_val~
				
			base.AddResistanceProperties( list );

            if (AllResistances != 0)
                list.Add(1060526, AllResistances.ToString()); // suma odpornosci ~1_val~%

			if ( m_HitPoints >= 0 && m_MaxHitPoints > 0 )
				list.Add( 1060639, "{0}\t{1}", m_HitPoints, m_MaxHitPoints ); // durability ~1_val~ / ~2_val~
			
			if ( (prop = ComputeStatReq( StatType.Str )) > 0 )
				list.Add( 1061170, prop.ToString() ); // strength requirement ~1_val~
			#endregion	
				
			#region Zliczanie propsow i przypisywanie lvl
			// 14.10.2012 mortuus - zliczanie propsow przeniesione do klasy ItemIdentification
			m_IdentifiedLevel = ItemIdentification.PropertiesCount(m_AosAttributes, m_AosSkillBonuses, SlayerName.None, SlayerName.None, null, null, m_AosArmorAttributes, 0);
			if( m_IdentifiedLevel == IdLevel.None )
				m_Identified = true;
			#endregion
			
            // 15.10.2012 :: zombie            
            if( !m_Identified )
                list.Add( ItemIdentification.UnidentifiedString );

            if ( m_AosSkillBonuses != null ) 
                m_AosSkillBonuses.GetProperties( list, m_Identified );

            ItemIdentification.AddNameProperty(AosAttribute.WeaponDamage, 1060401, m_AosAttributes.WeaponDamage, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.DefendChance, 1060408, m_AosAttributes.DefendChance, ref list, m_Identified );
            ItemIdentification.AddNameProperty(AosAttribute.BonusDex, 1060409, m_AosAttributes.BonusDex, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.EnhancePotions, 1060411, m_AosAttributes.EnhancePotions, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.CastRecovery, 1060412, m_AosAttributes.CastRecovery, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.CastSpeed, 1060413, m_AosAttributes.CastSpeed, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.AttackChance, 1060415, m_AosAttributes.AttackChance, ref list, m_Identified );
            ItemIdentification.AddNameProperty(AosAttribute.BonusHits, 1060431, m_AosAttributes.BonusHits, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.BonusInt, 1060432, m_AosAttributes.BonusInt, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.LowerManaCost, 1060433, m_AosAttributes.LowerManaCost, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.LowerRegCost, 1060434, m_AosAttributes.LowerRegCost, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosArmorAttribute.LowerStatReq, 1060435, GetLowerStatReqId(true), ref list, m_Identified);
			//ItemIdentification.AddNameProperty( 1071165, 1060435, m_AosArmorAttributes.LowerStatReq, ref list, m_Identified ); 
            ItemIdentification.AddNameProperty(AosAttribute.Luck, 1060436, GetLuckBonus() + m_AosAttributes.Luck, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosArmorAttribute.MageArmor, 1060437, m_AosArmorAttributes.MageArmor, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.BonusMana, 1060439, m_AosAttributes.BonusMana, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.RegenMana, 1060440, m_AosAttributes.RegenMana, ref list, m_Identified);
           // ItemIdentification.AddNameProperty(AosAttribute.NightSight, 1060441, m_AosAttributes.NightSight, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.ReflectPhysical, 1060442, m_AosAttributes.ReflectPhysical, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.RegenStam, 1060443, m_AosAttributes.RegenStam, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.RegenHits, 1060444, m_AosAttributes.RegenHits, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosArmorAttribute.SelfRepair, 1060450, m_AosArmorAttributes.SelfRepair, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.SpellChanneling, 1060482, m_AosAttributes.SpellChanneling, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.SpellDamage, 1060483, m_AosAttributes.SpellDamage, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.BonusStam, 1060484, m_AosAttributes.BonusStam, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.BonusStr, 1060485, m_AosAttributes.BonusStr, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.WeaponSpeed, 1060486, m_AosAttributes.WeaponSpeed, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosArmorAttribute.DurabilityBonus, 1060410, GetDurabilityBonusId(true), ref list, m_Identified );
			//ItemIdentification.AddNameProperty( 1071140, 1060410, m_AosArmorAttributes.DurabilityBonus, ref list, m_Identified );
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

			if ( m_Quality == ArmorQuality.Exceptional )
				attrs.Add( new EquipInfoAttribute( 1018305 - (int)m_Quality ) );

			if ( m_Identified || from.AccessLevel >= AccessLevel.GameMaster)
			{
				if ( m_Durability != ArmorDurabilityLevel.Regular )
					attrs.Add( new EquipInfoAttribute( 1038000 + (int)m_Durability ) );

				if ( m_Protection > ArmorProtectionLevel.Regular && m_Protection <= ArmorProtectionLevel.Invulnerability )
					attrs.Add( new EquipInfoAttribute( 1038005 + (int)m_Protection ) );
			}
			else if ( m_Durability != ArmorDurabilityLevel.Regular || (m_Protection > ArmorProtectionLevel.Regular && m_Protection <= ArmorProtectionLevel.Invulnerability) )
				attrs.Add( new EquipInfoAttribute( 1038000 ) ); // Unidentified

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

		#region ICraftable Members

		public int OnCraft( int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, Type typeRes2,  BaseTool tool, CraftItem craftItem, int resHue )
		{
			Quality = (ArmorQuality)quality;

			if ( makersMark )
				Crafter = from;

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

			PlayerConstructed = true;

			CraftContext context = craftSystem.GetContext( from );

			if ( context != null && context.DoNotColor )
				Hue = 0;

			if( Quality == ArmorQuality.Exceptional )
			{
				//DistributeBonuses( (tool is BaseRunicTool ? 6 : Core.SE ? 15 : 14) ); // Not sure since when, but right now 15 points are added, not 14.
                // zbroje tworzone narzedziem runicznym beda miec tak samo duzo resow jak zwyklym:
                DistributeBonuses( Core.SE ? 15 : 14 );

				if( Core.ML && !(this is BaseShield) )
				{
					int bonus = (int)(from.Skills.ArmsLore.Value / 20);

					for( int i = 0; i < bonus; i++ )
					{
						switch( Utility.Random( 5 ) )
						{
							case 0: m_PhysicalBonus++;	break;
							case 1: m_FireBonus++;		break;
							case 2: m_ColdBonus++;		break;
							case 3: m_EnergyBonus++;	break;
							case 4: m_PoisonBonus++;	break;
						}
					}

					from.CheckSkill( SkillName.ArmsLore, 0, 100 );
				}
			}

			if ( Core.AOS && tool is BaseRunicTool )
				((BaseRunicTool)tool).ApplyAttributesTo( this );

			return quality;
		}

		#endregion
	}
}
