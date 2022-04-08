// 05.05.19 :: troyan
// 05.09.02 :: troyan :: naprawa grafiki konia i dodanie RiddableLlama

using System;
using System.Collections;
using Server.Items;

namespace Server.Mobiles
{
	public class SBStable : SBInfo
	{
		private ArrayList m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBStable()
		{
			//Console.WriteLine("SBStable()");
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override ArrayList BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : ArrayList
		{
			public InternalBuyInfo()
			{  
				Add( new AnimalBuyInfo( 1, typeof( Cat ), 100, 5, 201, 0 ) );
				Add( new AnimalBuyInfo( 1, typeof( Dog ), 200, 5, 217, 0 ) );
                // 14.10.2012 :: zombie
                /*
				Add( new AnimalBuyInfo( 1, typeof( Horse ), 702, 10, 204, 0 ) );
				Add( new AnimalBuyInfo( 1, typeof( PackHorse ), 909, 10, 291, 0 ) );
				Add( new AnimalBuyInfo( 1, typeof( RidableLlama ), 622, 10, 220, 0 ) );
				Add( new AnimalBuyInfo( 1, typeof( PackLlama ), 736, 10, 292, 0 ) );
                */
				Add( new GenericBuyInfo( typeof( Bandage ), 10, 20, 0xE21, 0 ) ); 	
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( Bandage ), 1 );
			}
		}
	}
}
