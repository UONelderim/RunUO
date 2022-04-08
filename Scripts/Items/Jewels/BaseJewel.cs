using System;
using Server.Engines.Craft;
using Server.Network;

namespace Server.Items
{
	public enum GemType
	{
		None,
		StarSapphire,
		Emerald,
		Sapphire,
		Ruby,
		Citrine,
		Amethyst,
		Tourmaline,
		Amber,
		Diamond
	}
    
    // 23.09.2012 :: zombie :: IWearableDurability
	public abstract class BaseJewel : Item, ICraftable, IWearableDurability, IIdentifiable
	{
		private int m_MaxHitPoints;
		private int m_HitPoints;
		
		private AosAttributes m_AosAttributes;
		private AosElementAttributes m_AosResistances;
		private AosSkillBonuses m_AosSkillBonuses;
		private CraftResource m_Resource;
		private GemType m_GemType;
		private bool m_Identified;
		private IdLevel m_IdentifiedLevel;

		[CommandProperty( AccessLevel.GameMaster )]
		public int MaxHitPoints
		{
			get{ return m_MaxHitPoints; }
			set{ m_MaxHitPoints = value; InvalidateProperties(); }
		}
		
		[CommandProperty( AccessLevel.GameMaster )]
		public bool Identified
		{
			get{ return m_Identified; }
			set{ m_Identified = value; InvalidateProperties(); }
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
		
		[CommandProperty( AccessLevel.Player )]
		public AosAttributes Attributes
		{
			get{ return m_AosAttributes; }
			set{}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public AosElementAttributes Resistances
		{
			get{ return m_AosResistances; }
			set{}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public AosSkillBonuses SkillBonuses
		{
			get{ return m_AosSkillBonuses; }
			set{}
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public CraftResource Resource
		{
			get{ return m_Resource; }
			set{ m_Resource = value; Hue = CraftResources.GetHue( m_Resource ); }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public GemType GemType
		{
			get{ return m_GemType; }
			set{ m_GemType = value; InvalidateProperties(); }
		}

		public override int PhysicalResistance{ get{ return m_AosResistances.PhysicalId(m_Identified); } }
		public override int FireResistance{ get{ return m_AosResistances.FireId(m_Identified); } }
		public override int ColdResistance{ get{ return m_AosResistances.ColdId(m_Identified); } }
		public override int PoisonResistance{ get{ return m_AosResistances.PoisonId(m_Identified); } }
		public override int EnergyResistance{ get{ return m_AosResistances.EnergyId(m_Identified); } }
		public virtual int BaseGemTypeNumber{ get{ return 0; } }

        // 23.09.2012 :: zombie
		public virtual int InitMinHits{ get{ return 40; } }
		public virtual int InitMaxHits{ get{ return 50; } }
		
		public virtual bool CanFortify{ get{ return true; } }
		
		public void UnscaleDurability()
		{
			int scale = 100;

			m_HitPoints = ((m_HitPoints * 100) + (scale - 1)) / scale;
			m_MaxHitPoints = ((m_MaxHitPoints * 100) + (scale - 1)) / scale;
			InvalidateProperties();
		}

		public void ScaleDurability()
		{
			int scale = 100;

			m_HitPoints = ((m_HitPoints * scale) + 99) / 100;
			m_MaxHitPoints = ((m_MaxHitPoints * scale) + 99) / 100;
			InvalidateProperties();
		}
		
        public virtual int OnHit( BaseWeapon weapon, int damageTaken )
		{
			int Absorbed = Utility.RandomMinMax( 1, 4 );

			damageTaken -= Absorbed;

			if ( damageTaken < 0 ) 
				damageTaken = 0;

			if ( Config.ItemDurabilityLostChance > Utility.Random( 100 ) )
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
						((Mobile)Parent).LocalOverheadMessage(MessageType.Regular, 0x3B2, 1061185); // Twoja bizuteria sie rozpada!
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

			return damageTaken;
		}
        // zombie

		public override int LabelNumber
		{
			get
			{
				if ( m_GemType == GemType.None )
					return base.LabelNumber;

				return BaseGemTypeNumber + (int)m_GemType - 1;
			}
		}

		public override void OnAfterDuped( Item newItem )
		{
			BaseJewel jewel = newItem as BaseJewel;

			if ( jewel == null )
				return;

			jewel.m_AosAttributes = new AosAttributes( newItem, m_AosAttributes );
			jewel.m_AosResistances = new AosElementAttributes( newItem, m_AosResistances );
			jewel.m_AosSkillBonuses = new AosSkillBonuses( newItem, m_AosSkillBonuses );
		}
		
		public virtual int ArtifactRarity{ get{ return 0; } }

		public BaseJewel( int itemID, Layer layer ) : base( itemID )
		{
			m_AosAttributes = new AosAttributes( this );
			m_AosResistances = new AosElementAttributes( this );
			m_AosSkillBonuses = new AosSkillBonuses( this );
			m_Resource = CraftResource.Iron;
			m_GemType = GemType.None;

			Layer = layer;
			
			m_HitPoints = m_MaxHitPoints = Utility.RandomMinMax( InitMinHits, InitMaxHits );
		}

		public override void OnAdded( object parent )
		{
			if ( Core.AOS && parent is Mobile )
			{
				Mobile from = (Mobile)parent;

				m_AosSkillBonuses.AddTo( from, m_Identified );

				int strBonus = m_AosAttributes.BonusStrId(m_Identified);
				int dexBonus = m_AosAttributes.BonusDexId(m_Identified);
				int intBonus = m_AosAttributes.BonusIntId(m_Identified);

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

				from.CheckStatTimers();
			}
			// XmlAttachment check for OnEquip and CanEquip
			if(parent is Mobile)
			{
				if(Server.Engines.XmlSpawner2.XmlAttach.CheckCanEquip(this, (Mobile)parent))
				{
					Server.Engines.XmlSpawner2.XmlAttach.CheckOnEquip(this, (Mobile)parent);
				} 
				else
				{
					((Mobile)parent).AddToBackpack(this);
				}
			}
		}

		public override void OnRemoved( object parent )
		{
			if ( Core.AOS && parent is Mobile )
			{
				Mobile from = (Mobile)parent;

				m_AosSkillBonuses.Remove();

				string modName = this.Serial.ToString();

				from.RemoveStatMod( modName + "Str" );
				from.RemoveStatMod( modName + "Dex" );
				from.RemoveStatMod( modName + "Int" );

				from.CheckStatTimers();
			}
			// XmlAttachment check for OnRemoved
			Server.Engines.XmlSpawner2.XmlAttach.CheckOnRemoved(this, parent);
		}

		public BaseJewel( Serial serial ) : base( serial )
		{
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			int prop;
			
            if ( !Config.ItemIDSystemEnabled )
				m_Identified = true;

			#region Propsy zawsze wyswietlane
			if ( (prop = ArtifactRarity) > 0 )
				list.Add( 1061078, prop.ToString() ); // artifact rarity ~1_val~
				
			if ( m_HitPoints >= 0 && m_MaxHitPoints > 0 )
				list.Add( 1060639, "{0}\t{1}", m_HitPoints, m_MaxHitPoints ); // durability ~1_val~ / ~2_val~
			#endregion
			
			#region Zliczanie propsow i przypisywanie lvl
			// 14.10.2012 mortuus - zliczanie propsow przeniesione do klasy ItemIdentification
			m_IdentifiedLevel = ItemIdentification.PropertiesCount(m_AosAttributes, m_AosSkillBonuses, SlayerName.None, SlayerName.None, m_AosResistances, null, null, 0);
			if( m_IdentifiedLevel == IdLevel.None )
				m_Identified = true;
			#endregion
			
            // 15.10.2012 :: zombie            
            if( !m_Identified )
                list.Add( ItemIdentification.UnidentifiedString );

            if ( m_AosSkillBonuses != null ) 
                m_AosSkillBonuses.GetProperties( list, m_Identified );

            ItemIdentification.AddNameProperty(AosAttribute.WeaponDamage, 1060401, m_AosAttributes.WeaponDamage, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.DefendChance, 1060408, m_AosAttributes.DefendChance, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.BonusDex, 1060409, m_AosAttributes.BonusDex, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.EnhancePotions, 1060411, m_AosAttributes.EnhancePotions, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.CastRecovery, 1060412, m_AosAttributes.CastRecovery, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.CastSpeed, 1060413, m_AosAttributes.CastSpeed, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.AttackChance, 1060415, m_AosAttributes.AttackChance, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.BonusHits, 1060431, m_AosAttributes.BonusHits, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.BonusInt, 1060432, m_AosAttributes.BonusInt, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.LowerManaCost, 1060433, m_AosAttributes.LowerManaCost, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.LowerRegCost, 1060434, m_AosAttributes.LowerRegCost, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.Luck, 1060436, m_AosAttributes.Luck, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.BonusMana, 1060439, m_AosAttributes.BonusMana, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.RegenMana, 1060440, m_AosAttributes.RegenMana, ref list, m_Identified);
          //  ItemIdentification.AddNameProperty(AosAttribute.NightSight, 1060441, m_AosAttributes.NightSight, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.ReflectPhysical, 1060442, m_AosAttributes.ReflectPhysical, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.RegenStam, 1060443, m_AosAttributes.RegenStam, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.RegenHits, 1060444, m_AosAttributes.RegenHits, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.SpellChanneling, 1060482, m_AosAttributes.SpellChanneling, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.SpellDamage, 1060483, m_AosAttributes.SpellDamage, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.BonusStam, 1060484, m_AosAttributes.BonusStam, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.BonusStr, 1060485, m_AosAttributes.BonusStr, ref list, m_Identified);
            ItemIdentification.AddNameProperty(AosAttribute.WeaponSpeed, 1060486, m_AosAttributes.WeaponSpeed, ref list, m_Identified );
                        
            if ( m_Identified )
                base.AddResistanceProperties( list );
            else
            {
                ItemIdentification.AddNameProperty(AosWeaponAttribute.ResistPhysicalBonus, 1071178, m_AosResistances.Physical, ref list, m_Identified);
                ItemIdentification.AddNameProperty(AosWeaponAttribute.ResistFireBonus, 1071177, m_AosResistances.Fire, ref list, m_Identified);
                ItemIdentification.AddNameProperty(AosWeaponAttribute.ResistColdBonus, 1071175, m_AosResistances.Cold, ref list, m_Identified);
                ItemIdentification.AddNameProperty(AosWeaponAttribute.ResistPoisonBonus, 1071179, m_AosResistances.Poison, ref list, m_Identified);
                ItemIdentification.AddNameProperty(AosWeaponAttribute.ResistEnergyBonus, 1071176, m_AosResistances.Energy, ref list, m_Identified);
            }
            // zombie

			// mod to display attachment properties
			Server.Engines.XmlSpawner2.XmlAttach.AddAttachmentProperties(this, list);
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 3 ); // version
			
			//writer.Write( (bool)m_Identified );

			//writer.Write( (int)m_IdentifiedLevel );

			writer.WriteEncodedInt( (int) m_MaxHitPoints );
			writer.WriteEncodedInt( (int) m_HitPoints );

			writer.WriteEncodedInt( (int) m_Resource );
			writer.WriteEncodedInt( (int) m_GemType );

			m_AosAttributes.Serialize( writer );
			m_AosResistances.Serialize( writer );
			m_AosSkillBonuses.Serialize( writer );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 5:
				case 4:
				{
					if ( version == 4 )
					{
						/*m_Identified = */reader.ReadBool();
						/*m_IdentifiedLevel = (IdLevel)*/reader.ReadInt();
					}
					
					goto case 3;
				}
				case 3:
				{
					m_MaxHitPoints = reader.ReadEncodedInt();
					m_HitPoints = reader.ReadEncodedInt();

					goto case 2;
				}
				case 2:
				{
					m_Resource = (CraftResource)reader.ReadEncodedInt();
					m_GemType = (GemType)reader.ReadEncodedInt();

					goto case 1;
				}
				case 1:
				{
					m_AosAttributes = new AosAttributes( this, reader );
					m_AosResistances = new AosElementAttributes( this, reader );
					m_AosSkillBonuses = new AosSkillBonuses( this, reader );

					if ( Core.AOS && Parent is Mobile )
						m_AosSkillBonuses.AddTo( (Mobile)Parent, m_Identified );

					int strBonus = m_AosAttributes.BonusStr;
					int dexBonus = m_AosAttributes.BonusDex;
					int intBonus = m_AosAttributes.BonusInt;

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

					break;
				}
				case 0:
				{
					m_AosAttributes = new AosAttributes( this );
					m_AosResistances = new AosElementAttributes( this );
					m_AosSkillBonuses = new AosSkillBonuses( this );

					break;
				}
			}

			if ( version < 2 )
			{
				m_Resource = CraftResource.Iron;
				m_GemType = GemType.None;
			}

             // 23.09.2012 :: zombie 
            if ( m_MaxHitPoints == 0 && m_HitPoints == 0 )
                m_HitPoints = m_MaxHitPoints = Utility.RandomMinMax( InitMinHits, InitMaxHits );
            // zombie
		}
		#region ICraftable Members

		public int OnCraft( int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, Type typeRes2,  BaseTool tool, CraftItem craftItem, int resHue )
		{
			Type resourceType = typeRes;

			if ( resourceType == null )
				resourceType = craftItem.Ressources.GetAt( 0 ).ItemType;

			Resource = CraftResources.GetFromType( resourceType );

			CraftContext context = craftSystem.GetContext( from );

			if ( context != null && context.DoNotColor )
				Hue = 0;

			if ( 1 < craftItem.Ressources.Count )
			{
				resourceType = craftItem.Ressources.GetAt( 1 ).ItemType;

				if ( resourceType == typeof( StarSapphire ) )
					GemType = GemType.StarSapphire;
				else if ( resourceType == typeof( Emerald ) )
					GemType = GemType.Emerald;
				else if ( resourceType == typeof( Sapphire ) )
					GemType = GemType.Sapphire;
				else if ( resourceType == typeof( Ruby ) )
					GemType = GemType.Ruby;
				else if ( resourceType == typeof( Citrine ) )
					GemType = GemType.Citrine;
				else if ( resourceType == typeof( Amethyst ) )
					GemType = GemType.Amethyst;
				else if ( resourceType == typeof( Tourmaline ) )
					GemType = GemType.Tourmaline;
				else if ( resourceType == typeof( Amber ) )
					GemType = GemType.Amber;
				else if ( resourceType == typeof( Diamond ) )
					GemType = GemType.Diamond;
			}

			return 1;
		}

		#endregion
	}
}