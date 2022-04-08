using System;
using System.Collections;
using Server.Items;
using Server.Items.Crops;

namespace Server.Mobiles
{
	public class SBBowyer : SBInfo
	{
		private ArrayList m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBBowyer()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override ArrayList BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : ArrayList
		{
			public InternalBuyInfo()
			{
				Add( new GenericBuyInfo( typeof( FletcherTools ), 30, 50, 0x1022, 0 ) );

                Add(new GenericBuyInfo(typeof(Bow), 43, 50, 0x13B2, 0));
                Add(new GenericBuyInfo(typeof(CompositeBow), 47, 50, 0x26C2, 0));
                Add(new GenericBuyInfo(typeof(Yumi), 51, 50, 0x27A5, 0));

                Add(new GenericBuyInfo(typeof(Crossbow), 45, 50, 0xF50, 0));
                Add(new GenericBuyInfo(typeof(RepeatingCrossbow), 47, 100, 0x26C3, 0));
                Add(new GenericBuyInfo(typeof(HeavyCrossbow), 49, 100, 0x13FD, 0));

                Add(new GenericBuyInfo(typeof(Bolt), 3, 200, 0x1BFB, 0));                
                Add(new GenericBuyInfo(typeof(Arrow), 3, 200, 0xF3F, 0));
                Add(new GenericBuyInfo(typeof(FukiyaDarts), 3, 200, 0x2806, 0));

				Add(new GenericBuyInfo(typeof(Fukiya), 20, 20, 0x27AA, 0));
			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( FletcherTools ), 8 );

                Add(typeof(Log), 1);
                Add(typeof(Board), 1);

                Add(typeof(BowstringLeather), 1);
                Add(typeof(BowstringGut), 1);
                Add(typeof(BowstringCannabis), 1);
                Add(typeof(BowstringSilk), 1);

                Add(typeof(Feather), 1);
                Add(typeof(Leather), 1);
                Add(typeof(Gut), 1);
                Add(typeof(CannabisFiber), 1);
                Add(typeof(SilkFiber), 1);

                Add(typeof(Shaft), 1);
                Add(typeof(Arrow), 2);
                Add(typeof(Bolt), 1);
                Add(typeof(FukiyaDarts), 1);
				Add(typeof(Fukiya), 10);
			}
		}
	}
}