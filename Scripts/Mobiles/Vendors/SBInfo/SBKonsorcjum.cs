using System;
using System.Collections;
using Nelderim.Engines.ChaosChest;
using Server.Items;
using Server.Factions;

namespace Server.Mobiles
{
	public class SBKonsorcjum : SBInfo
	{
		private ArrayList m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBKonsorcjum()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override ArrayList BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : ArrayList
		{
			public InternalBuyInfo()
			{
                
                Add( new GenericBuyInfo( typeof( Silver ), 100, 1000, 0xEF0, 0 ) );
                Add( new GenericBuyInfo( typeof( ChaosChest ), 100000, 10, 0x1445, 0 ) );
                
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
			}
		}
	}
}