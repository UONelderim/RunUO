// 05.06.11 :: eth :: wylaczenie sprzedazy map Britani itp
// 06.04.01 :: troyan :: usuniecie cashbuga cenowego BlankScroll

using System;
using System.Collections;
using Server.Items;

namespace Server.Mobiles
{
	public class SBMapmaker : SBInfo
	{
		private ArrayList m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBMapmaker()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override ArrayList BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : ArrayList
		{
			public InternalBuyInfo()
			{
				//for ( int i = 0; i < PresetMapEntry.Table.Length; ++i )
				//	Add( new PresetMapBuyInfo( PresetMapEntry.Table[i], Utility.RandomMinMax( 7, 10 ), 20 ) );

				Add( new GenericBuyInfo( typeof( BlankScroll ), 10, 50, 0xEF3, 0 ) );
				Add( new GenericBuyInfo( typeof( MapmakersPen ), 30, 50, 0x0FBF, 0 ) );
				Add( new GenericBuyInfo( typeof( BlankMap ), 10, 50, 0x14EC, 0 ) );
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( BlankScroll ), 3 );
				Add( typeof( MapmakersPen ), 8 );
				Add( typeof( BlankMap ), 3 );
				Add( typeof( LocalMap ), 4 );
				Add( typeof( CityMap ), 5 );
				Add( typeof( SeaChart ), 7 );
				Add( typeof( WorldMap ), 9 );

			}
		}
	}
}
