using System;
using System.Collections;
using System.Collections.Generic;
using Server.Items;

namespace Server.Mobiles
{
	public class SBArtTrader : SBInfo
	{
		private ArrayList m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBArtTrader()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override ArrayList BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : ArrayList
		{
			public InternalBuyInfo()
			{  
				Add( new GenericBuyInfo( typeof( ArtLootBox ), 2000, 5, 0x2DF3, 0 , new object[]{ArtLootBox.ArtLootType.Random}) );
				Add( new GenericBuyInfo( typeof( ArtLootBox ), 3000, 5, 0x2DF3, 0 , new object[]{ArtLootBox.ArtLootType.Boss}) );
				Add( new GenericBuyInfo( typeof( ArtLootBox ), 3000, 5, 0x2DF3, 0 , new object[]{ArtLootBox.ArtLootType.Miniboss}) );
				Add( new GenericBuyInfo( typeof( ArtLootBox ), 2500, 5, 0x2DF3, 0 , new object[]{ArtLootBox.ArtLootType.Paragon}) );
				Add( new GenericBuyInfo( typeof( ArtLootBox ), 4500, 5, 0x2DF3, 0 , new object[]{ArtLootBox.ArtLootType.Doom}) );
				Add( new GenericBuyInfo( typeof( ArtLootBox ), 3000, 5, 0x2DF3, 0 , new object[]{ArtLootBox.ArtLootType.Hunter}) );
				Add( new GenericBuyInfo( typeof( ArtLootBox ), 3000, 5, 0x2DF3, 0 , new object[]{ArtLootBox.ArtLootType.Cartography}) );
				Add( new GenericBuyInfo( typeof( ArtLootBox ), 3000, 5, 0x2DF3, 0 , new object[]{ArtLootBox.ArtLootType.Fishing}) );
			}
		}

		public class InternalSellInfo : IShopSellInfo
		{
			private Dictionary<Type, int> m_Table = new Dictionary<Type, int>();
			private Type[] m_Types;
			
			public InternalSellInfo()
			{
				foreach (Type type in ArtifactHelper.AllArtifacts)
				{
					Add(type, 100);
				}
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
				int price;
				double scalar = 1.0;
				m_Table.TryGetValue( item.GetType(), out price );

				if ( item is IDurability ) {
					IDurability art = (IDurability)item;
					scalar = (double)art.HitPoints / art.InitMaxHits;
				}

				price = (int)(price * scalar);
				if ( price < 1 )
					price = 1;

				return price;
			}

			public int GetBuyPriceFor(Item item)
			{
				return 1;
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
