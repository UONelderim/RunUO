using System;
using System.Collections;
using Server;
using Server.Items;
using System.Collections.Generic;

namespace Server.Engines.BulkOrders
{
	[TypeAlias( "Scripts.Engines.BulkOrders.SmallBOD" )]
	public abstract class SmallBOD : Item
	{
		private int m_AmountCur, m_AmountMax;
		private Type m_Type;
		private int m_Number;
		private int m_Graphic;	// Holds level for hunting bulks and graphic for crafting bods.
		                        // Never use this variable directly, use Level or Graphic instead.
		private bool m_RequireExceptional;
		private BulkMaterialType m_Material;
		private BulkMaterialType m_Material2;

		[CommandProperty( AccessLevel.GameMaster )]
		public int AmountCur{ get{ return m_AmountCur; } set{ m_AmountCur = value; InvalidateProperties(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int AmountMax{ get{ return m_AmountMax; } set{ m_AmountMax = value; InvalidateProperties(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public Type Type{ get{ return m_Type; } set{ m_Type = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int Number{ get{ return m_Number; } set{ m_Number = value; InvalidateProperties(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int Graphic{ get{ return (this is SmallHunterBOD) ? 0 : m_Graphic; } set{ if (!(this is SmallHunterBOD)) m_Graphic = value; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public int Level  { get{ return SmallBulkEntry.GetHunterBulkLevel(this); } /*set{ if (  this is SmallHunterBOD ) m_Graphic = value; }*/ }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool RequireExceptional{ get{ return m_RequireExceptional; } set{ m_RequireExceptional = value; InvalidateProperties(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public BulkMaterialType Material{ get{ return m_Material; } set{ m_Material = value; InvalidateProperties(); } }

		[CommandProperty(AccessLevel.GameMaster)]
		public BulkMaterialType Material2 { get { return m_Material2; } set { m_Material2 = value; InvalidateProperties(); } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool Complete{ get{ return ( m_AmountCur == m_AmountMax ); } }

		public override int LabelNumber{ get{ return 1045151; } } // a bulk order deed

		[Constructable]
		public SmallBOD( int hue, int amountMax, Type type, int number, int graphic, bool requireExeptional, BulkMaterialType material, BulkMaterialType material2 ) : base( Core.AOS ? 0x2258 : 0x14EF )
		{
			Weight = 0.1;
			Hue = hue; // Blacksmith: 0x44E; Tailoring: 0x483
			LootType = LootType.Blessed;

			m_AmountMax = amountMax;
			m_Type = type;
			m_Number = number;
			Graphic = graphic;
			//Level = graphic;
			m_RequireExceptional = requireExeptional;
			m_Material = material;
			m_Material2 = material2;
		}

		public SmallBOD() : base( Core.AOS ? 0x2258 : 0x14EF )
		{
			Weight = 0.1;
			LootType = LootType.Blessed;
		}

		public static BulkMaterialType GetRandomMaterial( BulkMaterialType defaultType, BulkMaterialType start, double[] chances )
		{
			double random = Utility.RandomDouble();

			for ( int i = 0; i < chances.Length; ++i )
			{
				if ( random < chances[i] )
					return ( i == 0 ? defaultType : start + (i - 1) );

				random -= chances[i];
			}

			return defaultType;
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			list.Add( 1060654 ); // small bulk order

			if ( m_RequireExceptional )
				list.Add( 1045141 ); // All items must be exceptional.

			if ( m_Material != BulkMaterialType.None )
				list.Add( SmallBODGump.GetMaterialNumberFor( m_Material ) ); // All items must be made with x material.

			if (m_Material2 != BulkMaterialType.None)
				list.Add(SmallBODGump.GetMaterialNumberFor(m_Material2)); // All items must be made with x material.

			list.Add( 1060656, m_AmountMax.ToString() ); // amount to make: ~1_val~
			list.Add( 1060658, "#{0}\t{1}", m_Number, m_AmountCur ); // ~1_val~: ~2_val~
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( IsChildOf( from.Backpack ) )
				from.SendGump( new SmallBODGump( from, this ) );
			else
				from.SendLocalizedMessage( 1045156 ); // You must have the deed in your backpack to use it.
		}

		public void BeginCombine( Mobile from )
		{
			if ( m_AmountCur < m_AmountMax )
				from.Target = new SmallBODTarget( this );
			else
				from.SendLocalizedMessage( 1045166 ); // The maximum amount of requested items have already been combined to this deed.
		}

		public abstract List<Item> ComputeRewards( bool full );
		public abstract int ComputeGold();
		public abstract int ComputeFame();

		public virtual void GetRewards( out Item reward, out int gold, out int fame )
		{
			reward = null;
			gold = ComputeGold();
			fame = ComputeFame();

			List<Item> rewards = ComputeRewards( false );

			if ( rewards.Count > 0 )
			{
				reward = rewards[Utility.Random( rewards.Count )];

				for ( int i = 0; i < rewards.Count; ++i )
				{
					if ( rewards[i] != reward )
						rewards[i].Delete();
				}
			}
		}

		public static BulkMaterialType GetMaterial( CraftResource resource )
		{
			switch ( resource )
			{
				case CraftResource.DullCopper:			return BulkMaterialType.DullCopper;
				case CraftResource.ShadowIron:			return BulkMaterialType.ShadowIron;
				case CraftResource.Copper:				return BulkMaterialType.Copper;
				case CraftResource.Bronze:				return BulkMaterialType.Bronze;
				case CraftResource.Gold:				return BulkMaterialType.Gold;
				case CraftResource.Agapite:				return BulkMaterialType.Agapite;
				case CraftResource.Verite:				return BulkMaterialType.Verite;
				case CraftResource.Valorite:			return BulkMaterialType.Valorite;
				case CraftResource.SpinedLeather:		return BulkMaterialType.Spined;
				case CraftResource.HornedLeather:		return BulkMaterialType.Horned;
				case CraftResource.BarbedLeather:		return BulkMaterialType.Barbed;
				case CraftResource.OakWood:				return BulkMaterialType.Oak;
				case CraftResource.AshWood:				return BulkMaterialType.Ash;
				case CraftResource.YewWood:				return BulkMaterialType.Yew;
				case CraftResource.Heartwood:			return BulkMaterialType.Heartwood;
				case CraftResource.Bloodwood:			return BulkMaterialType.Bloodwood;
				case CraftResource.Frostwood:			return BulkMaterialType.Frostwood;
				case CraftResource.BowstringLeather:	return BulkMaterialType.BowstringLeather;
				case CraftResource.BowstringGut:		return BulkMaterialType.BowstringGut;
				case CraftResource.BowstringCannabis:	return BulkMaterialType.BowstringCannabis;
				case CraftResource.BowstringSilk:		return BulkMaterialType.BowstringSilk;
			}

			return BulkMaterialType.None;
		}

		public virtual void EndCombine( Mobile from, object o )
		{
			if ( o is Item && ((Item)o).IsChildOf( from.Backpack ) )
			{
				Type objectType = o.GetType();

				if ( m_AmountCur >= m_AmountMax )
				{
					from.SendLocalizedMessage( 1045166 ); // The maximum amount of requested items have already been combined to this deed.
				}
				else if ( m_Type == null || (objectType != m_Type && !objectType.IsSubclassOf( m_Type )) || (!(o is BaseWeapon) && !(o is BaseArmor) && !(o is BaseClothing)) )
				{
					from.SendLocalizedMessage( 1045169 ); // The item is not in the request.
				}
				else
				{
					BulkMaterialType material = BulkMaterialType.None;
					BulkMaterialType material2 = BulkMaterialType.None;

					if ( o is BaseArmor )
						material = GetMaterial( ((BaseArmor)o).Resource );
					else if ( o is BaseClothing )
						material = GetMaterial( ((BaseClothing)o).Resource );
					else if ( o is BaseWeapon) {
						material = GetMaterial(((BaseWeapon)o).Resource);
						material2 = GetMaterial(((BaseWeapon)o).Resource2);
					}

					if ( m_Material >= BulkMaterialType.DullCopper && m_Material <= BulkMaterialType.Valorite && material != m_Material )
					{
						from.SendLocalizedMessage( 1045168 ); // Przedmiot nie jest wykonany z wlasciwej rudy.
					}
					else if ( m_Material >= BulkMaterialType.Spined && m_Material <= BulkMaterialType.Barbed && material != m_Material )
					{
						from.SendLocalizedMessage( 1049352 ); // Przedmiot nie jest wykonany z wlasciwej skory.
					} 
					else if (m_Material >= BulkMaterialType.Oak && m_Material <= BulkMaterialType.Frostwood && material != m_Material) 
					{
						from.SendLocalizedMessage(1049800); // Przedmiot nie jest wykonany z wlasciwego drewna.
					}  
					else if (m_Material2 >= BulkMaterialType.BowstringLeather && m_Material2 <= BulkMaterialType.BowstringSilk && material2 != m_Material2) 
					{
						from.SendLocalizedMessage(1049801); // Przedmiot nie jest wykonany z wlasciwej cieciwy.
					} 
					else
					{
						bool isExceptional = false;

						if ( o is BaseWeapon )
							isExceptional = ( ((BaseWeapon)o).Quality == WeaponQuality.Exceptional );
						else if ( o is BaseArmor )
							isExceptional = ( ((BaseArmor)o).Quality == ArmorQuality.Exceptional );
						else if ( o is BaseClothing )
							isExceptional = ( ((BaseClothing)o).Quality == ClothingQuality.Exceptional );

						if ( m_RequireExceptional && !isExceptional )
						{
							from.SendLocalizedMessage( 1045167 ); // The item must be exceptional.
						}
						else
						{

							((Item)o).Delete();
							++AmountCur;

							from.SendLocalizedMessage( 1045170 ); // The item has been combined with the deed.

							from.SendGump( new SmallBODGump( from, this ) );

							if ( m_AmountCur < m_AmountMax )
								BeginCombine( from );
						}
					}
				}
			}
			else
			{
				from.SendLocalizedMessage( 1045158 ); // You must have the item in your backpack to target it.
			}
		}

		public SmallBOD( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			int version = 2;

			writer.Write( (int)version ); // version
			//writer.Write( (int)m_Material2 );

			writer.Write( m_AmountCur );
			writer.Write( m_AmountMax );
			writer.Write( m_Type == null ? null : m_Type.FullName );
			writer.Write( m_Number );
			writer.Write( m_Graphic );
			writer.Write( m_RequireExceptional );
			writer.Write( (int) m_Material );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 2:
					goto case 0;
				case 1: 
				{
					m_Material2 = (BulkMaterialType)reader.ReadInt();
					goto case 0;
				}
				case 0:
				{
					m_AmountCur = reader.ReadInt();
					m_AmountMax = reader.ReadInt();

					string type = reader.ReadString();

					if ( type != null )
						m_Type = ScriptCompiler.FindTypeByFullName( type );

					m_Number = reader.ReadInt();
					int graphic = reader.ReadInt();
					//Level = graphic;
					Graphic = graphic;
					m_RequireExceptional = reader.ReadBool();
					m_Material = (BulkMaterialType)reader.ReadInt();

					break;
				}
			}

			if ( Weight != 0.1 )
				Weight = 0.1;

			if ( Core.AOS && ItemID == 0x14EF )
				ItemID = 0x2258;

			if ( Parent == null && Map == Map.Internal && Location == Point3D.Zero )
				Delete();
		}
	}
}