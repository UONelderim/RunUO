using System;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class GenericSellInfo : IShopSellInfo
	{
		private Dictionary<Type, int> m_Table = new Dictionary<Type, int>();
		private Type[] m_Types;

        // 08.07.2012 :: zombie
        public Dictionary<Type, int> Table
        {
            get { return m_Table; }
        }
        // zombie

		public GenericSellInfo()
		{
		}

		public void Add( Type type, int price )
		{
			m_Table[type] = price;
			m_Types = null;
		}

        private float ResourceScalar(Item item)
        {
            float m_scalar = 1.0f;
            CraftResource m_item_resource;
            if (item is BaseArmor)
            {
                m_item_resource = ((BaseArmor)item).Resource;
            }
            else if (item is BaseWeapon)
            {
                m_item_resource = ((BaseWeapon)item).Resource;
            }
            else
            {
                m_item_resource = CraftResource.None;
            }
            switch (m_item_resource)
            {
                case CraftResource.None:
                    m_scalar = 1.0f;
                    break;

                // Metal
                case CraftResource.Iron:
                    m_scalar = 1.0f;
                    break;
                case CraftResource.DullCopper:
                case CraftResource.ShadowIron:
                case CraftResource.Copper:
                    m_scalar = 1.1f;
                    break;
                case CraftResource.Bronze:
                case CraftResource.Gold:
                case CraftResource.Agapite:
                    m_scalar = 1.2f;
                    break;
                case CraftResource.Verite:
                case CraftResource.Valorite:
                    m_scalar = 1.4f;
                    break;
				case CraftResource.Platinum:
					m_scalar = 1.0f;
					break;
				case CraftResource.RegularLeather:
                    m_scalar = 1.0f;
                    break;
                case CraftResource.SpinedLeather:
                    m_scalar = 1.1f;
                    break;
                case CraftResource.HornedLeather:
                case CraftResource.BarbedLeather:
                    m_scalar = 1.3f;
                    break;

                // Scales
                case CraftResource.RedScales:
                    break;
                case CraftResource.YellowScales:
                    break;
                case CraftResource.BlackScales:
                    break;
                case CraftResource.GreenScales:
                    break;
                case CraftResource.WhiteScales:
                    break;
                case CraftResource.BlueScales:
                    break;

                // Wood
                case CraftResource.RegularWood:
                    m_scalar = 1.0f;
                    break;
                case CraftResource.OakWood:
                case CraftResource.AshWood:
                    m_scalar = 1.1f;
                    break;
                case CraftResource.YewWood:
                case CraftResource.Heartwood:
                    m_scalar = 1.2f;
                    break;
                case CraftResource.Bloodwood:
                case CraftResource.Frostwood:
                    m_scalar = 1.4f;
                    break;

                //Bowstring
                case CraftResource.BowstringLeather:
                    break;
                case CraftResource.BowstringCannabis:
                    break;
                case CraftResource.BowstringSilk:
                    break;
                case CraftResource.BowstringGut:
                    break;
                case CraftResource.BowstringFiresteed:
                    break;
                case CraftResource.BowstringUnicorn:
                    break;
                case CraftResource.BowstringNightmare:
                    break;
                case CraftResource.BowstringKirin:
                    break;
                default:
                    break;
            }
            return m_scalar;
        }

		public int GetSellPriceFor( Item item )
		{
			int price = 0;
			m_Table.TryGetValue( item.GetType(), out price );

			if ( item is BaseArmor ) {
				BaseArmor armor = (BaseArmor)item;

				if ( armor.Quality == ArmorQuality.Low )
					price = (int)( price * 0.60 );
				else if ( armor.Quality == ArmorQuality.Exceptional )
					price = (int)( price * 1.25 );

				price += 100 * (int)armor.Durability;

				price += 100 * (int)armor.ProtectionLevel;

                price = (int)(price * ResourceScalar(item));

				if ( price < 1 )
					price = 1;
			}
			else if ( item is BaseWeapon ) {
				BaseWeapon weapon = (BaseWeapon)item;

				if ( weapon.Quality == WeaponQuality.Low )
					price = (int)( price * 0.60 );
				else if ( weapon.Quality == WeaponQuality.Exceptional )
					price = (int)( price * 1.25 );

				price += 100 * (int)weapon.DurabilityLevel;

				price += 100 * (int)weapon.DamageLevel;

                price = (int)(price * ResourceScalar(item));

				if ( price < 1 )
					price = 1;
			}
			else if ( item is BaseBeverage ) {
				int price1 = price, price2 = price;

				if ( item is Pitcher )
				{ price1 = 3; price2 = 5; }
				else if ( item is BeverageBottle )
				{ price1 = 3; price2 = 3; }
				else if ( item is Jug )
				{ price1 = 6; price2 = 6; }

				BaseBeverage bev = (BaseBeverage)item;

				if ( bev.IsEmpty || bev.Content == BeverageType.Milk )
					price = price1;
				else
					price = price2;
			}

			return price;
		}

		public int GetBuyPriceFor( Item item )
		{
			return (int)( 1.90 * GetSellPriceFor( item ) );
		}

		public Type[] Types
		{
			get
			{
				if ( m_Types == null )
				{
					m_Types = new Type[m_Table.Keys.Count];
					m_Table.Keys.CopyTo( m_Types, 0 );
				}

				return m_Types;
			}
		}

		public string GetNameFor( Item item )
		{
			if ( item.Name != null )
				return item.Name;
			else
				return item.LabelNumber.ToString();
		}

		public bool IsSellable( Item item )
		{
			if ( item.LootType == LootType.Cursed )
		        return false;

			return IsInList( item.GetType() );
		}
	 
		public bool IsResellable( Item item )
		{
			//if ( item.Hue != 0 )
				//return false;

			return IsInList( item.GetType() );
		}

		public bool IsInList( Type type )
		{
			return m_Table.ContainsKey( type );
		}
	}
}
