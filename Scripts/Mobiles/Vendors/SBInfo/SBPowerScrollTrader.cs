using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBPowerScrollTrader : SBInfo
	{
		private ArrayList m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		private static int BasePriceForSmallestScroll { get { return 10; } }

		private static bool HowManySmallScrollsForOneBig(int bigPsValue, out int quantity)
		{
            int howMany105ForOne110, howMany110ForOne115, howMany115ForOne120;
            ScrollBinderDeed.PowerScrollsQuantityNeededForUpgrade(105, out howMany105ForOne110);
            ScrollBinderDeed.PowerScrollsQuantityNeededForUpgrade(110, out howMany110ForOne115);
            ScrollBinderDeed.PowerScrollsQuantityNeededForUpgrade(115, out howMany115ForOne120);

            switch (bigPsValue)
            {
                case 105: quantity = 1; return true;    // x
                case 110: quantity = howMany105ForOne110; return true;  // 8
				case 115: quantity = howMany105ForOne110 * howMany110ForOne115; return true;    // 8 * 6
				case 120: quantity = howMany105ForOne110 * howMany110ForOne115 * howMany115ForOne120; return true;  // 8 * 6 * 4
				default: quantity = 60000; return false;  // invalid value
			}
        }

		public SBPowerScrollTrader()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override ArrayList BuyInfo { get { return m_BuyInfo; } }

		public static int GetBuyPrice(PowerScrollLootBox.PowerScrollLootType boxType)
		{
            int price = 60000;

            double powerScrollValue = (int)boxType;
            int numberOf105Scrolls;
            if (HowManySmallScrollsForOneBig((int)powerScrollValue, out numberOf105Scrolls))
            {
				double factor = 2.0; // za zwój od handlarza zap³acimy 2x wiêcej zwojów, ni¿ w systemie zwojów ³¹czenia

				price = (int)(factor * BasePriceForSmallestScroll * numberOf105Scrolls);
            }

            return price;
        }

		public class InternalBuyInfo : ArrayList
		{
			public InternalBuyInfo()
			{  
				Add( new GenericBuyInfo( typeof(PowerScrollLootBox), GetBuyPrice(PowerScrollLootBox.PowerScrollLootType.Wondrous), 50, 0x2DF3, 0 , new object[]{ PowerScrollLootBox.PowerScrollLootType.Wondrous}) );
				Add( new GenericBuyInfo( typeof(PowerScrollLootBox), GetBuyPrice(PowerScrollLootBox.PowerScrollLootType.Exalted), 40, 0x2DF3, 0 , new object[]{ PowerScrollLootBox.PowerScrollLootType.Exalted}) );
				Add( new GenericBuyInfo( typeof(PowerScrollLootBox), GetBuyPrice(PowerScrollLootBox.PowerScrollLootType.Mythical), 30, 0x2DF3, 0 , new object[]{ PowerScrollLootBox.PowerScrollLootType.Mythical}) );
				Add( new GenericBuyInfo( typeof(PowerScrollLootBox), GetBuyPrice(PowerScrollLootBox.PowerScrollLootType.Legendary), 20, 0x2DF3, 0 , new object[]{ PowerScrollLootBox.PowerScrollLootType.Legendary}) );
			}
		}

		public class InternalSellInfo : IShopSellInfo
		{
			private Dictionary<Type, int> m_Table = new Dictionary<Type, int>();
			private Type[] m_Types;
			
			public InternalSellInfo()
			{
				Add(typeof(PowerScroll), 1);
			}
			
			public void Add( Type type, int price )
			{
				m_Table[type] = price;
				m_Types = null;
			}
			
			public string GetNameFor(Item item)
			{
				if ( item.Name != null )
					return item.Name;
				return item.LabelNumber.ToString();
			}

			public int GetSellPriceFor(Item item)
			{
				int price = 1;

				PowerScroll ps = item as PowerScroll;
				if (ps != null)
				{
					int numberOf105Scrolls;
					if (HowManySmallScrollsForOneBig((int)ps.Value, out numberOf105Scrolls))
					{
						price = BasePriceForSmallestScroll * numberOf105Scrolls;
					}
				}

				return price;
			}

			public int GetBuyPriceFor(Item item)
			{
				return 99999;
			}

			public bool IsSellable(Item item)
			{
				return m_Table.ContainsKey(item.GetType());
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

			public bool IsResellable(Item item)
			{
				return false;
			}
		}
	}
}
