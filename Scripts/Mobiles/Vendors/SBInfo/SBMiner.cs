using System;
using System.Collections;
using Server.Items;

namespace Server.Mobiles
{
	public class SBMiner: SBInfo
	{
		private ArrayList m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBMiner()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override ArrayList BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : ArrayList
		{
			public InternalBuyInfo()
			{
				Add( new GenericBuyInfo( typeof( Bag ), 50, 50, 0xE76, 0 ) );
				Add( new GenericBuyInfo( typeof( Candle ), 10, 50, 0xA28, 0 ) );
				Add( new GenericBuyInfo( typeof( Torch ), 20, 50, 0xF6B, 0 ) );
				Add( new GenericBuyInfo( typeof( Lantern ), 30, 50, 0xA25, 0 ) );
				//Add( new GenericBuyInfo( typeof( OilFlask ), 8, 10, 0x####, 0 ) );
				Add( new GenericBuyInfo( typeof( Pickaxe ), 30, 50, 0xE86, 0 ) );
				Add( new GenericBuyInfo( typeof( Shovel ), 30, 50, 0xF39, 0 ) );
				Add( new GenericBuyInfo( typeof( IronIngot ), 10, 50, 0x1BF2, 0 ) );
				
				//Add( new GenericBuyInfo( "Wydobycie dobrej jakosci piasku", typeof( SandMiningBook ), 10000, 2, 0xFF4, 0 ) );
				//Add( new GenericBuyInfo( "Wydobycie dobrej jakosci granitu", typeof( StoneMiningBook ), 10000, 2, 0xFBE, 0 ) );
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( Pickaxe ), 8 );
				Add( typeof( Shovel ), 6 );
				Add( typeof( Lantern ), 1 );
				//Add( typeof( OilFlask ), 4 );
				Add( typeof( Torch ), 3 );
				Add( typeof( Bag ), 3 );
				Add( typeof( Candle ), 3 );
				Add( typeof( IronIngot ), 3 );
				//Add( typeof( SandMiningBook ), 3000 );
				//Add( typeof( StoneMiningBook ), 3000 );
			}
		}
	}
}