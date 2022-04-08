using System;
using System.Collections.Generic;
using Server;
using Server.Engines.Craft;
using Server.Factions;
using Server.Network;

namespace Server.Items
{
	public enum ClothingQuality
	{
		Low,
		Regular,
		Exceptional
	}

	public interface IArcaneEquip
	{
		bool IsArcane{ get; }
		int CurArcaneCharges{ get; set; }
		int MaxArcaneCharges{ get; set; }
	}

	public abstract class BaseClothing : Item, IDyable, IScissorable, IFactionItem, ICraftable, IWearableDurability, IIdentifiable
	{
		#region Factions
		private FactionItem m_FactionState;

		public FactionItem FactionItemState
		{
			get{ return m_FactionState; }
			set
			{
				m_FactionState = value;

				if ( m_FactionState == null )
					Hue = 0;

				LootType = ( m_FactionState == null ? LootType.Regular : LootType.Blessed );
			}
		}
		#endregion

		public virtual bool CanFortify{ get{ return true; } }

		private int m_MaxHitPoints;
		private int m_HitPoints;
		private Mobile m_Crafter;
		private ClothingQuality m_Quality;
		private bool m_Identified, m_PlayerConstructed;
		private IdLevel m_IdentifiedLevel;
		protected CraftResource m_Resource;
		private int m_StrReq = -1;

		private AosAttributes m_AosAttributes;
		private AosArmorAttributes m_AosClothingAttributes;
		private AosSkillBonuses m_AosSkillBonuses;
		private AosElementAttributes m_AosResistances;

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
		public bool Identified
		{
			get{ return m_Identified; }
			set{ UnscaleDurability(); m_Identified = value; ScaleDurability(); InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public IdLevel IdentifyLevel
		{
			get{ return m_IdentifiedLevel; }
			set{ m_IdentifiedLevel = value; InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public Mobile Crafter
		{
			get{ return m_Crafter; }
			set{ m_Crafter = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int StrRequirement
		{
			get{ return ( m_StrReq == -1 ? (Core.AOS ? AosStrReq : OldStrReq) : m_StrReq ); }
			set{ m_StrReq = value; InvalidateProperties(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public ClothingQuality Quality
		{
			get{ return m_Quality; }
			set{ UnscaleDurability(); m_Quality = value; InvalidateProperties(); ScaleDurability(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool PlayerConstructed
		{
			get{ return m_PlayerConstructed; }
			set{ m_PlayerConstructed = value; }
		}

		public virtual CraftResource DefaultResource{ get{ return CraftResource.None; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public CraftResource Resource
		{
			get{ return m_Resource; }
			set{ UnscaleDurability(); m_Resource = value; Hue = CraftResources.GetHue( m_Resource ); InvalidateProperties(); ScaleDurability(); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public AosAttributes Attributes
		{
			get{ return m_AosAttributes; }
			set{}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public AosArmorAttributes ClothingAttributes
		{
			get{ return m_AosClothingAttributes; }
			set{}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public AosSkillBonuses SkillBonuses
		{
			get{ return m_AosSkillBonuses; }
			set{}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public AosElementAttributes Resistances
		{
			get{ return m_AosResistances; }
			set{}
		}

        // 22.09.2012 :: zombie :: zmiana wytrzymalosci
        [CommandProperty( AccessLevel.GameMaster )]
        public int InitHitPoints
        {
            get
            {
              //  if ( Layer == Layer.Helm )
               //     return 15;
               // else
                    return Utility.RandomMinMax( InitMinHits, InitMaxHits );
            }
        }
        // zombie

		public virtual int BasePhysicalResistance{ get{ return 0; } }
		public virtual int BaseFireResistance{ get{ return 0; } }
		public virtual int BaseColdResistance{ get{ return 0; } }
		public virtual int BasePoisonResistance{ get{ return 0; } }
		public virtual int BaseEnergyResistance{ get{ return 0; } }

		public override int PhysicalResistance{ get{ return BasePhysicalResistance + m_AosResistances.Physical; } }
		public override int FireResistance{ get{ return BaseFireResistance + m_AosResistances.Fire; } }
		public override int ColdResistance{ get{ return BaseColdResistance + m_AosResistances.Cold; } }
		public override int PoisonResistance{ get{ return BasePoisonResistance + m_AosResistances.Poison; } }
		public override int EnergyResistance{ get{ return BaseEnergyResistance + m_AosResistances.Energy; } }

        public int AllResistances { get { return PhysicalResistance + FireResistance + ColdResistance + PoisonResistance + EnergyResistance; } }

		public virtual int ArtifactRarity{ get{ return 0; } }

		public virtual int BaseStrBonus{ get{ return 0; } }
		public virtual int BaseDexBonus{ get{ return 0; } }
		public virtual int BaseIntBonus { get { return 0; } }

        public virtual int BaseWeightBonus { get { return 0; } }

		public override bool AllowSecureTrade( Mobile from, Mobile to, Mobile newOwner, bool accepted )
		{
			if ( !Ethics.Ethic.CheckTrade( from, to, newOwner, this ) )
				return false;

			return base.AllowSecureTrade( from, to, newOwner, accepted );
		}

		public virtual Race RequiredRace { get { return null; } }

		public override bool CanEquip( Mobile from )
		{
			if ( !Ethics.Ethic.CheckEquip( from, this ) )
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
					int strBonus = ComputeStatBonus( StatType.Str );
					int strReq = ComputeStatReq( StatType.Str );

					if( from.Str < strReq || (from.Str + strBonus) < 1 )
					{
						from.SendLocalizedMessage( 500213 ); // You are not strong enough to equip that.
						return false;
					}
				}
			}

			return base.CanEquip( from );
		}

		public virtual int AosStrReq{ get{ return 10; } }
		public virtual int OldStrReq{ get{ return 0; } }

        // 23.09.2012 :: zombie
		public virtual int InitMinHits{ get{ return 33; } }
		public virtual int InitMaxHits{ get{ return 33; } }
        // zombie

		public virtual bool AllowMaleWearer{ get{ return true; } }
		public virtual bool AllowFemaleWearer{ get{ return true; } }
		public virtual bool CanBeBlessed{ get{ return true; } }

		public int ComputeStatReq( StatType type )
		{
			int v;

			//if ( type == StatType.Str )
				v = StrRequirement;

			return AOS.Scale( v, 100 - GetLowerStatReq() );
		}

		public int ComputeStatBonus( StatType type )
		{
			if ( type == StatType.Str )
				return BaseStrBonus + Attributes.BonusStr;
			else if ( type == StatType.Dex )
				return BaseDexBonus + Attributes.BonusDex;
			else
				return BaseIntBonus + Attributes.BonusInt;
		}

		public virtual void AddStatBonuses( Mobile parent )
		{
			if ( parent == null )
				return;

			int strBonus = ComputeStatBonus( StatType.Str );
			int dexBonus = ComputeStatBonus( StatType.Dex );
			int intBonus = ComputeStatBonus( StatType.Int );

			if ( strBonus == 0 && dexBonus == 0 && intBonus == 0 )
				return;

			string modName = this.Serial.ToString();

			if ( strBonus != 0 )
				parent.AddStatMod( new StatMod( StatType.Str, modName + "Str", strBonus, TimeSpan.Zero ) );

			if ( dexBonus != 0 )
				parent.AddStatMod( new StatMod( StatType.Dex, modName + "Dex", dexBonus, TimeSpan.Zero ) );

			if ( intBonus != 0 )
				parent.AddStatMod( new StatMod( StatType.Int, modName + "Int", intBonus, TimeSpan.Zero ) );
		}

		public static void ValidateMobile( Mobile m )
		{
			for ( int i = m.Items.Count - 1; i >= 0; --i )
			{
				if ( i >= m.Items.Count )
					continue;

				Item item = m.Items[i];

				if ( item is BaseClothing )
				{
					BaseClothing clothing = (BaseClothing)item;

					if( clothing.RequiredRace != null && m.Race != clothing.RequiredRace )
					{
						/*if( clothing.RequiredRace == Race.Elf )
							m.SendLocalizedMessage( 1072203 ); // Only Elves may use this.
						else*/
							m.SendMessage( "Only {0} may use this.", clothing.RequiredRace.PluralName );

						m.AddToBackpack( clothing );
					}
					else if ( !clothing.AllowMaleWearer && !m.Female && m.AccessLevel < AccessLevel.GameMaster )
					{
						if ( clothing.AllowFemaleWearer )
							m.SendLocalizedMessage( 1010388 ); // Only females can wear this.
						else
							m.SendMessage( "You may not wear this." );

						m.AddToBackpack( clothing );
					}
					else if ( !clothing.AllowFemaleWearer && m.Female && m.AccessLevel < AccessLevel.GameMaster )
					{
						if ( clothing.AllowMaleWearer )
							m.SendLocalizedMessage( 1063343 ); // Only males can wear this.
						else
							m.SendMessage( "You may not wear this." );

						m.AddToBackpack( clothing );
					}
				}
			}
		}

		public int GetLowerStatReq()
		{
			if ( !Core.AOS )
				return 0;

			return m_AosClothingAttributes.LowerStatReqId(m_Identified);
		}

		public override void OnAdded( object parent )
		{
			Mobile mob = parent as Mobile;

			if ( mob != null )
			{
				if ( Core.AOS )
					m_AosSkillBonuses.AddTo( mob, m_Identified );

				AddStatBonuses( mob );
				mob.CheckStatTimers();
			}

			base.OnAdded( parent );
		}

		public override void OnRemoved( object parent )
		{
			Mobile mob = parent as Mobile;

			if ( mob != null )
			{
				if ( Core.AOS )
					m_AosSkillBonuses.Remove();

				string modName = this.Serial.ToString();

				mob.RemoveStatMod( modName + "Str" );
				mob.RemoveStatMod( modName + "Dex" );
				mob.RemoveStatMod( modName + "Int" );

				mob.CheckStatTimers();
			}

			base.OnRemoved( parent );
		}

		public virtual int OnHit( BaseWeapon weapon, int damageTaken )
		{
			int Absorbed = Utility.RandomMinMax( 1, 2 );

			damageTaken -= Absorbed;

			if ( damageTaken < 0 ) 
				damageTaken = 0;

			if ( Config.ItemDurabilityLostChance > Utility.Random( 100 ) )
			{
				if ( Core.AOS && m_AosClothingAttributes.SelfRepairId(m_Identified) > Utility.Random( 10 ) )
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
							((Mobile)Parent).LocalOverheadMessage(MessageType.Regular, 0x3B2, 1061184); // Twoje odzienie sie rozpada!
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

		public BaseClothing( int itemID, Layer layer ) : this( itemID, layer, 0 )
		{
		}

		public BaseClothing( int itemID, Layer layer, int hue ) : base( itemID )
		{
			Layer = layer;
			Hue = hue;

			m_Resource = DefaultResource;
			m_Quality = ClothingQuality.Regular;
            
            // 22.09.2012 :: zombie :: zmiany w wytrzymalosci
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
			m_AosClothingAttributes = new AosArmorAttributes( this );
			m_AosSkillBonuses = new AosSkillBonuses( this );
			m_AosResistances = new AosElementAttributes( this );
		}

		public override void OnAfterDuped( Item newItem )
		{
			BaseClothing clothing = newItem as BaseClothing;

			if ( clothing == null )
				return;

			clothing.m_AosAttributes = new AosAttributes( newItem, m_AosAttributes );
			clothing.m_AosResistances = new AosElementAttributes( newItem, m_AosResistances );
			clothing.m_AosSkillBonuses = new AosSkillBonuses( newItem, m_AosSkillBonuses );
			clothing.m_AosClothingAttributes = new AosArmorAttributes( newItem, m_AosClothingAttributes );
		}

		public BaseClothing( Serial serial ) : base( serial )
		{
		}

		public override bool AllowEquipedCast( Mobile from )
		{
			if ( base.AllowEquipedCast( from ) )
				return true;

			return ( m_AosAttributes.SpellChanneling != 0 );
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

		private string GetNameString()
		{
			string name = this.Name;

			if ( name == null )
				name = String.Format( "#{0}", LabelNumber );

			return name;
		}

		public override void AddNameProperty( ObjectPropertyList list )
		{
			int oreType = BaseArmor.ResourceNameNumber( m_Resource );

			if ( oreType != 0 )
				list.Add( 1053099, "#{0}\t{1}", oreType, GetNameString() ); // ~1_oretype~ ~2_armortype~
			else if ( Name == null )
				list.Add( LabelNumber );
			else
				list.Add( Name );
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );
			int prop;
			
			#region Factions
			if ( m_FactionState != null )
				list.Add( 1041350 ); // faction item
			#endregion
			
            if ( !Config.ItemIDSystemEnabled )
				m_Identified = true;

			#region Propsy zawsze wyswietlane
			if ( m_Crafter != null )
				list.Add( 1050043, m_Crafter.Name ); // crafted by ~1_NAME~

			if ( m_Quality == ClothingQuality.Exceptional )
				list.Add( 1060636 ); // exceptional

			/*if( RequiredRace == Race.Elf )
				list.Add( 1075086 ); // Elves Only*/

			if ( (prop = ArtifactRarity) > 0 )
				list.Add( 1061078, prop.ToString() ); // artifact rarity ~1_val~

			base.AddResistanceProperties( list );

            if (AllResistances != 0)
                list.Add(1060526, AllResistances.ToString()); // suma odpornosci ~1_val~%

			if ( (prop = ComputeStatReq( StatType.Str )) > 0 )
				list.Add( 1061170, prop.ToString() ); // strength requirement ~1_val~

			if ( m_HitPoints >= 0 && m_MaxHitPoints > 0 )
				list.Add( 1060639, "{0}\t{1}", m_HitPoints, m_MaxHitPoints ); // durability ~1_val~ / ~2_val~
			#endregion
			
			#region Zliczanie propsow i przypisywanie lvl
			// 14.10.2012 mortuus - zliczanie propsow przeniesione do klasy ItemIdentification
			m_IdentifiedLevel = ItemIdentification.PropertiesCount(m_AosAttributes, m_AosSkillBonuses, SlayerName.None, SlayerName.None, null, null, m_AosClothingAttributes, 0);
			if( m_IdentifiedLevel == IdLevel.None )
				m_Identified = true;
			#endregion

            // 15.10.2012 :: zombie            
            if( !m_Identified )
                list.Add( ItemIdentification.UnidentifiedString );

            if (BaseWeightBonus > 0)
                list.Add(1064031, BaseWeightBonus.ToString()); // Zwiekszony maksymalny udzwig ~1_val

            if ( m_AosSkillBonuses != null ) 
                m_AosSkillBonuses.GetProperties( list, m_Identified );

            ItemIdentification.AddNameProperty(AosAttribute.WeaponDamage, 1060401, m_AosAttributes.WeaponDamage, ref list, m_Identified );
            ItemIdentification.AddNameProperty(AosAttribute.DefendChance, 1060408, m_AosAttributes.DefendChance, ref list, m_Identified );
            ItemIdentification.AddNameProperty(AosAttribute.BonusDex, 1060409, m_AosAttributes.BonusDex, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.EnhancePotions, 1060411, m_AosAttributes.EnhancePotions, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.CastRecovery, 1060412, m_AosAttributes.CastRecovery, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.CastSpeed, 1060413, m_AosAttributes.CastSpeed, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.AttackChance, 1060415, m_AosAttributes.AttackChance, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.BonusHits, 1060431, m_AosAttributes.BonusHits, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.BonusInt, 1060432, m_AosAttributes.BonusInt, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.LowerManaCost, 1060433, m_AosAttributes.LowerManaCost, ref list, m_Identified );
            ItemIdentification.AddNameProperty(AosAttribute.LowerRegCost, 1060434, m_AosAttributes.LowerRegCost, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosArmorAttribute.LowerStatReq, 1060435, m_AosClothingAttributes.LowerStatReq, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.Luck, 1060436, m_AosAttributes.Luck, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosArmorAttribute.MageArmor, 1060437, m_AosClothingAttributes.MageArmor, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.BonusMana, 1060439, m_AosAttributes.BonusMana, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.RegenMana, 1060440, m_AosAttributes.RegenMana, ref list, m_Identified);
           //ItemIdentification.AddNameProperty(AosAttribute.NightSight, 1060441, m_AosAttributes.NightSight, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.ReflectPhysical, 1060442, m_AosAttributes.ReflectPhysical, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.RegenStam, 1060443, m_AosAttributes.RegenStam, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.RegenHits, 1060444, m_AosAttributes.RegenHits, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosArmorAttribute.SelfRepair, 1060450, m_AosClothingAttributes.SelfRepair, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.SpellChanneling, 1060482, m_AosAttributes.SpellChanneling, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.SpellDamage, 1060483, m_AosAttributes.SpellDamage, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.BonusStam, 1060484, m_AosAttributes.BonusStam, ref list, m_Identified );
            ItemIdentification.AddNameProperty(AosAttribute.BonusStr, 1060485, m_AosAttributes.BonusStr, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.WeaponSpeed, 1060486, m_AosAttributes.WeaponSpeed, ref list, m_Identified);
            //ItemIdentification.AddNameProperty( 1071140, 1060410, GetDurabilityBonus(), ref list, m_Identified );
            ItemIdentification.AddNameProperty(AosArmorAttribute.DurabilityBonus, 1060410, m_AosClothingAttributes.DurabilityBonus, ref list, m_Identified);
            // zombie
		}

		public override void OnSingleClick( Mobile from )
		{
			List<EquipInfoAttribute> attrs = new List<EquipInfoAttribute>();

			AddEquipInfoAttributes( from, attrs );

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

		public virtual void AddEquipInfoAttributes( Mobile from, List<EquipInfoAttribute> attrs )
		{
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

			if ( m_Quality == ClothingQuality.Exceptional )
				attrs.Add( new EquipInfoAttribute( 1018305 - (int)m_Quality ) );
		}

		#region Serialization
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
			Resource			= 0x00000001,
			Attributes			= 0x00000002,
			ClothingAttributes	= 0x00000004,
			SkillBonuses		= 0x00000008,
			Resistances			= 0x00000010,
			MaxHitPoints		= 0x00000020,
			HitPoints			= 0x00000040,
			PlayerConstructed	= 0x00000080,
			Crafter				= 0x00000100,
			Quality				= 0x00000200,
			StrReq				= 0x00000400,
			Identified			= 0x00000800,
			IdentifiedLevel	= 0x00001000
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 5 ); // version

			SaveFlag flags = SaveFlag.None;

			SetSaveFlag( ref flags, SaveFlag.Resource,			m_Resource != DefaultResource );
			SetSaveFlag( ref flags, SaveFlag.Attributes,		!m_AosAttributes.IsEmpty );
			SetSaveFlag( ref flags, SaveFlag.ClothingAttributes,!m_AosClothingAttributes.IsEmpty );
			SetSaveFlag( ref flags, SaveFlag.SkillBonuses,		!m_AosSkillBonuses.IsEmpty );
			SetSaveFlag( ref flags, SaveFlag.Resistances,		!m_AosResistances.IsEmpty );
			SetSaveFlag( ref flags, SaveFlag.MaxHitPoints,		m_MaxHitPoints != 0 );
			SetSaveFlag( ref flags, SaveFlag.HitPoints,			m_HitPoints != 0 );
			SetSaveFlag( ref flags, SaveFlag.PlayerConstructed,	m_PlayerConstructed != false );
			SetSaveFlag( ref flags, SaveFlag.Crafter,			m_Crafter != null );
			SetSaveFlag( ref flags, SaveFlag.Quality,			m_Quality != ClothingQuality.Regular );
			SetSaveFlag( ref flags, SaveFlag.StrReq,			m_StrReq != -1 );
			//SetSaveFlag( ref flags, SaveFlag.Identified,		m_Identified != false );
			//SetSaveFlag( ref flags, SaveFlag.IdentifiedLevel,	m_IdentifiedLevel != IdLevel.None );

			writer.WriteEncodedInt( (int) flags );

			if ( GetSaveFlag( flags, SaveFlag.Resource ) )
				writer.WriteEncodedInt( (int) m_Resource );

			if ( GetSaveFlag( flags, SaveFlag.Attributes ) )
				m_AosAttributes.Serialize( writer );

			if ( GetSaveFlag( flags, SaveFlag.ClothingAttributes ) )
				m_AosClothingAttributes.Serialize( writer );

			if ( GetSaveFlag( flags, SaveFlag.SkillBonuses ) )
				m_AosSkillBonuses.Serialize( writer );

			if ( GetSaveFlag( flags, SaveFlag.Resistances ) )
				m_AosResistances.Serialize( writer );

			if ( GetSaveFlag( flags, SaveFlag.MaxHitPoints ) )
				writer.WriteEncodedInt( (int) m_MaxHitPoints );

			if ( GetSaveFlag( flags, SaveFlag.HitPoints ) )
				writer.WriteEncodedInt( (int) m_HitPoints );

			if ( GetSaveFlag( flags, SaveFlag.Crafter ) )
				writer.Write( (Mobile) m_Crafter );

			if ( GetSaveFlag( flags, SaveFlag.Quality ) )
				writer.WriteEncodedInt( (int) m_Quality );

			if ( GetSaveFlag( flags, SaveFlag.StrReq ) )
				writer.WriteEncodedInt( (int) m_StrReq );
				
			//if( GetSaveFlag( flags, SaveFlag.IdentifiedLevel ) )
			//	writer.Write( (int)m_IdentifiedLevel );
				
			//writer.Write( (int)m_IdentifiedLevel );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 7:
				case 6:
				case 5:
				{
					SaveFlag flags = (SaveFlag)reader.ReadEncodedInt();

					if ( GetSaveFlag( flags, SaveFlag.Resource ) )
						m_Resource = (CraftResource)reader.ReadEncodedInt();
					else
						m_Resource = DefaultResource;

					if ( GetSaveFlag( flags, SaveFlag.Attributes ) )
						m_AosAttributes = new AosAttributes( this, reader );
					else
						m_AosAttributes = new AosAttributes( this );

					if ( GetSaveFlag( flags, SaveFlag.ClothingAttributes ) )
						m_AosClothingAttributes = new AosArmorAttributes( this, reader );
					else
						m_AosClothingAttributes = new AosArmorAttributes( this );

					if ( GetSaveFlag( flags, SaveFlag.SkillBonuses ) )
						m_AosSkillBonuses = new AosSkillBonuses( this, reader );
					else
						m_AosSkillBonuses = new AosSkillBonuses( this );

					if ( GetSaveFlag( flags, SaveFlag.Resistances ) )
						m_AosResistances = new AosElementAttributes( this, reader );
					else
						m_AosResistances = new AosElementAttributes( this );

					if ( GetSaveFlag( flags, SaveFlag.MaxHitPoints ) )
						m_MaxHitPoints = reader.ReadEncodedInt();

					if ( GetSaveFlag( flags, SaveFlag.HitPoints ) )
						m_HitPoints = reader.ReadEncodedInt();

					if ( GetSaveFlag( flags, SaveFlag.Crafter ) )
						m_Crafter = reader.ReadMobile();

					if ( GetSaveFlag( flags, SaveFlag.Quality ) )
						m_Quality = (ClothingQuality)reader.ReadEncodedInt();
					else
						m_Quality = ClothingQuality.Regular;

					if ( GetSaveFlag( flags, SaveFlag.StrReq ) )
						m_StrReq = reader.ReadEncodedInt();
					else
						m_StrReq = -1;

					if ( GetSaveFlag( flags, SaveFlag.PlayerConstructed ) )
						m_PlayerConstructed = true;
					if ( version > 5 && version < 7 )
					{
						if ( GetSaveFlag( flags, SaveFlag.Identified ) )
							m_Identified = (version >= 6 || reader.ReadBool());

						if ( GetSaveFlag( flags, SaveFlag.IdentifiedLevel ) )
							m_IdentifiedLevel = (IdLevel)reader.ReadInt();

						m_IdentifiedLevel = (IdLevel)reader.ReadInt();
					}

					break;
				}
				case 4:
				{
					m_Resource = (CraftResource)reader.ReadInt();

					goto case 3;
				}
				case 3:
				{
					m_AosAttributes = new AosAttributes( this, reader );
					m_AosClothingAttributes = new AosArmorAttributes( this, reader );
					m_AosSkillBonuses = new AosSkillBonuses( this, reader );
					m_AosResistances = new AosElementAttributes( this, reader );

					goto case 2;
				}
				case 2:
				{
					m_PlayerConstructed = reader.ReadBool();
					goto case 1;
				}
				case 1:
				{
					m_Crafter = reader.ReadMobile();
					m_Quality = (ClothingQuality)reader.ReadInt();
					break;
				}
				case 0:
				{
					m_Crafter = null;
					m_Quality = ClothingQuality.Regular;
					break;
				}
			}

			if ( version < 2 )
				m_PlayerConstructed = true; // we don't know, so, assume it's crafted

			if ( version < 3 )
			{
				m_AosAttributes = new AosAttributes( this );
				m_AosClothingAttributes = new AosArmorAttributes( this );
				m_AosSkillBonuses = new AosSkillBonuses( this );
				m_AosResistances = new AosElementAttributes( this );
			}

			if ( version < 4 )
				m_Resource = DefaultResource;

            // 22.06.2012 :: zombie
			if ( m_MaxHitPoints == 0 && m_HitPoints == 0 )
                m_HitPoints = m_MaxHitPoints = InitHitPoints; // Utility.RandomMinMax( InitMinHits, InitMaxHits );
            // zombie

			Mobile parent = Parent as Mobile;

			if ( parent != null )
			{
				if ( Core.AOS )
					m_AosSkillBonuses.AddTo( parent, m_Identified );

				AddStatBonuses( parent );
				parent.CheckStatTimers();
			}
		}
		#endregion

		public virtual bool Dye( Mobile from, DyeTub sender )
		{
			if ( Deleted )
				return false;
			else if ( RootParent is Mobile && from != RootParent )
				return false;

			Hue = sender.DyedHue;

			return true;
		}

		public virtual bool Scissor( Mobile from, Scissors scissors )
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

			if ( item != null && item.Ressources.Count == 1 && item.Ressources.GetAt( 0 ).Amount >= 2 )
			{
				try
				{
					Type resourceType = null;

					CraftResourceInfo info = CraftResources.GetInfo( m_Resource );

					if ( info != null && info.ResourceTypes.Length > 0 )
						resourceType = info.ResourceTypes[0];

					if ( resourceType == null )
						resourceType = item.Ressources.GetAt( 0 ).ItemType;

					Item res = (Item)Activator.CreateInstance( resourceType );

					ScissorHelper( from, res, m_PlayerConstructed ? (item.Ressources.GetAt( 0 ).Amount / 2) : 1 );

					res.LootType = LootType.Regular;

					return true;
				}
				catch
				{
				}
			}

			from.SendLocalizedMessage( 502440 ); // Scissors can not be used on that to produce anything.
			return false;
		}

		public void DistributeBonuses( int amount )
		{
			for ( int i = 0; i < amount; ++i )
			{
				switch ( Utility.Random( 5 ) )
				{
					case 0: ++m_AosResistances.Physical; break;
					case 1: ++m_AosResistances.Fire; break;
					case 2: ++m_AosResistances.Cold; break;
					case 3: ++m_AosResistances.Poison; break;
					case 4: ++m_AosResistances.Energy; break;
				}
			}

			InvalidateProperties();
		}


        // 15.10.2012 :: zombie
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

		public int GetDurabilityBonus()
		{
			int bonus = 0;

			if ( m_Quality == ClothingQuality.Exceptional )
				bonus += 20;

			if ( Core.AOS )
				bonus += m_AosClothingAttributes.DurabilityBonusId(m_Identified);

			return bonus;
		}

		#region ICraftable Members

		public virtual int OnCraft( int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, Type typeRes2, BaseTool tool, CraftItem craftItem, int resHue )
		{
			Quality = (ClothingQuality)quality;

			if ( makersMark )
				Crafter = from;

			if ( DefaultResource != CraftResource.None )
			{
				Type resourceType = typeRes;

				if ( resourceType == null )
					resourceType = craftItem.Ressources.GetAt( 0 ).ItemType;

				Resource = CraftResources.GetFromType( resourceType );
			}
			else
			{
				Hue = resHue;
			}

			PlayerConstructed = true;

			CraftContext context = craftSystem.GetContext( from );

			if ( context != null && context.DoNotColor )
				Hue = 0;

			return quality;
		}

		#endregion
	}
}