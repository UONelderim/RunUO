// 20.04.19 :: Maupishon :: dodanie Runebook do listy sprzedawcy
using System;
using System.Collections;
using Server.Items;
using Server.Spells.Necromancy;
using Server.Spells.Ninjitsu;
using Server.Spells;

namespace Server.Mobiles
{
	public class SBScribe: SBInfo
	{
		private ArrayList m_BuyInfo = new InternalBuyInfo();
		private IShopSellInfo m_SellInfo = new InternalSellInfo();

		public SBScribe()
		{
		}

		public override IShopSellInfo SellInfo { get { return m_SellInfo; } }
		public override ArrayList BuyInfo { get { return m_BuyInfo; } }

		public class InternalBuyInfo : ArrayList
		{
			public InternalBuyInfo()
			{
				Add( new GenericBuyInfo( typeof( ScribesPen ), 30,  50, 0xFBF, 0 ) );
				Add( new GenericBuyInfo( typeof( BlankScroll ), 10, 50, 0x0E34, 0 ) );
				Add( new GenericBuyInfo( typeof( BrownBook ), 15, 50, 0xFEF, 0 ) );
				Add( new GenericBuyInfo( typeof( TanBook ), 15, 50, 0xFF0, 0 ) );
				Add( new GenericBuyInfo( typeof( BlueBook ), 15, 50, 0xFF2, 0 ) );
				Add( new GenericBuyInfo( "1041267", typeof( Runebook ), 35000, 10, 0xEFA, 0x461 ) );

				//Bushido
				Add(new GenericBuyInfo(typeof(HonorableExecutionScroll), 3000, 10, 0x1F71, 137));
				Add(new GenericBuyInfo(typeof(ConfidenceScroll), 3000, 10, 0x1F72, 137));
				Add(new GenericBuyInfo(typeof(CounterAttackScroll), 3000, 10, 0x1F72, 137));

				//Chivalry
				Add(new GenericBuyInfo(typeof(CloseWoundsScroll), 1000, 10, 0x1F6E, 1150));
				Add(new GenericBuyInfo(typeof(RemoveCurseScroll), 1000, 10, 0x1F6E, 1150));
				Add(new GenericBuyInfo(typeof(CleanseByFireScroll), 1000, 10, 0x1F6D, 1150));
				Add(new GenericBuyInfo(typeof(ConsecrateWeaponScroll), 1000, 10, 0x1F6D, 1150));
				Add(new GenericBuyInfo(typeof(DivineFuryScroll), 1000, 10, 0x1F6D, 1150));

				//Ninjitsu
				Add(new GenericBuyInfo(typeof(AnimalFormScroll), 3750, 10, 0x1F6F, 1000));
				Add(new GenericBuyInfo(typeof(MirrorImageScroll), 3750, 10, 0x1F70, 1000));
				Add(new GenericBuyInfo(typeof(FocusAttackScroll), 3750, 10, 0x1F6F, 1000));
				Add(new GenericBuyInfo(typeof(BackstabScroll), 3750, 10, 0x1F70, 1000));

				//Necromancy
				Add(new GenericBuyInfo(typeof(CurseWeaponScroll), 1900, 10, 0x2263, 0));
				Add(new GenericBuyInfo(typeof(BloodOathScroll), 1900, 10, 0x2261, 0));
				Add(new GenericBuyInfo(typeof(CorpseSkinScroll), 1900, 10, 0x2262, 0));
				Add(new GenericBuyInfo(typeof(EvilOmenScroll), 1900, 10, 0x2264, 0));
				Add(new GenericBuyInfo(typeof(PainSpikeScroll), 1900, 10, 0x2268, 0));
				Add(new GenericBuyInfo(typeof(WraithFormScroll), 1900, 10, 0x226F, 0));
				Add(new GenericBuyInfo(typeof(MindRotScroll), 1900, 10, 0x2267, 0));
				Add(new GenericBuyInfo(typeof(SummonFamiliarScroll), 1900, 105, 0x226B, 0));
				Add(new GenericBuyInfo(typeof(AnimateDeadScroll), 1900, 10, 0x2260, 0));

				//Pelne ksiegi
				Add(new GenericBuyInfo(typeof(FullBushidoBook), 40000, 10, 0x238C, 0));
				Add(new GenericBuyInfo(typeof(FullChivalryBook), 40000, 10, 0x2252, 0));
				Add(new GenericBuyInfo(typeof(FullNinjitsuBook), 40000, 10, 0x23A0, 0));
				Add(new GenericBuyInfo(typeof(FullNecrobook), 40000, 10, 0x2253, 0));
				Add(new GenericBuyInfo(typeof(FullSpellbook), 60000, 10, 0xEFA, 0));

			}
		}

		public class InternalSellInfo : GenericSellInfo
		{
			public InternalSellInfo()
			{
				Add( typeof( ScribesPen ), 8 );
				Add( typeof( BrownBook ), 7 );
				Add( typeof( TanBook ), 7 );
				Add( typeof( BlueBook ), 7 );
				Add( typeof( BlankScroll ), 2 );
				Add( typeof( Spellbook ), 50 );
			}
		}
	}
}