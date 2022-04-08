using System;
using System.Collections;
using Server.Items;

namespace Server.Mobiles
{
	public class SBBeekeeper : SBInfo
	{
		private ArrayList m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBBeekeeper()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override ArrayList BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : ArrayList
		{
			public InternalBuyInfo()
			{
				Add( new GenericBuyInfo( typeof( JarHoney ), 3, 50, 0x9EC, 0 ) );
				Add( new GenericBuyInfo( typeof( Beeswax ), 3, 50, 0x1422, 0 ) );
				Add( new GenericBuyInfo( typeof( SackFlour ), 10, 50, 0x1039, 0 ) );
				Add( new GenericBuyInfo( typeof( SheafOfHay ), 4, 50, 0xF36, 0 ) );
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( JarHoney ), 1 );
				Add( typeof( Beeswax ), 1 );
				Add( typeof( SackFlour ), 1 );
				Add( typeof( SheafOfHay ), 1 );
			}
		}
	}
}