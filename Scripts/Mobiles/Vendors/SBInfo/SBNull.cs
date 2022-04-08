using System;
using System.Collections;
using Server.Items;

namespace Server.Mobiles
{
	public class SBNull: SBInfo
	{
		private ArrayList m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBNull()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override ArrayList BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : ArrayList
		{
			public InternalBuyInfo()
			{
				//Add( new GenericBuyInfo( typeof( BrownBook ), 15, 5, 0xFEF, 0 ) );
				//Add( new GenericBuyInfo( typeof( TanBook ), 15, 5, 0xFF0, 0 ) );
				//Add( new GenericBuyInfo( typeof( BlueBook ), 15, 5, 0xFF2, 0 ) );
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				//Add( typeof( BrownBook ), 6 );
				//Add( typeof( TanBook ), 6 );
				//Add( typeof( BlueBook ), 6 );
			}
		}
	}
}