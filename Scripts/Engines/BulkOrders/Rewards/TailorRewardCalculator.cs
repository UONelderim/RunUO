using System;
using Server.Items;

namespace Server.Engines.BulkOrders
{
	public sealed class TailorRewardCalculator : RewardCalculator
	{
		#region Constructors
		private static readonly ConstructCallback Cloth = new ConstructCallback(CreateCloth);
		private static readonly ConstructCallback Sandals = new ConstructCallback(CreateSandals);
		private static readonly ConstructCallback RunicKit = new ConstructCallback(CreateRunicKit);
		private static readonly ConstructCallback DecorLesser = new ConstructCallback(CreateDecorLesser);
		private static readonly ConstructCallback DecorGreater = new ConstructCallback(CreateDecorGreater);
		private static readonly ConstructCallback PowerScroll = new ConstructCallback(CreatePowerScroll);
		private static readonly ConstructCallback PowderForLeather = new ConstructCallback(CreatePowderForLeather);

		private static readonly ConstructCallback AncientSewingKit = new ConstructCallback(CreateAncientSewingKit);

		//private static readonly ConstructCallback ClothingBlessDeed = new ConstructCallback( CreateCBD );

		private static int[][] m_ClothHues = new int[][]
			{
				new int[]{ 0x483, 0x48C, 0x488, 0x48A },
				new int[]{ 0x495, 0x48B, 0x486, 0x485 },
				new int[]{ 0x48D, 0x490, 0x48E, 0x491 },
				new int[]{ 0x48F, 0x494, 0x484, 0x497 },
				new int[]{ 0x489, 0x47F, 0x482, 0x47E }
			};

		private static Item CreateCloth(int type) {
			if (type >= 0 && type < m_ClothHues.Length) {
				UncutCloth cloth = new UncutCloth(100);
				cloth.Hue = m_ClothHues[type][Utility.Random(m_ClothHues[type].Length)];
				return cloth;
			}

			throw new InvalidOperationException();
		}

		private static int[] m_SandalHues = new int[]
			{
				0x489, 0x47F, 0x482,
				0x47E, 0x48F, 0x494,
				0x484, 0x497
			};

		private static Item CreateSandals(int type) {
			return new Sandals(m_SandalHues[Utility.Random(m_SandalHues.Length)]);
		}

		private static Type[] m_LesserDecorTypes = new Type[]
		{
			typeof(LightFlowerTapestryEastDeed),
			typeof(LightFlowerTapestrySouthDeed),
			typeof(DarkFlowerTapestryEastDeed),
			typeof(DarkFlowerTapestrySouthDeed),

			typeof(BrownBearRugEastDeed),
			typeof(BrownBearRugSouthDeed),
			typeof(PolarBearRugEastDeed),
			typeof(PolarBearRugSouthDeed),

			typeof(SmallStretchedHideEastDeed),
			typeof(SmallStretchedHideSouthDeed),
			typeof(MediumStretchedHideEastDeed),
			typeof(MediumStretchedHideSouthDeed)
		};

		private static Item CreateDecorLesser(int type) {

			var lesserDecorType = m_LesserDecorTypes[Utility.Random(m_LesserDecorTypes.Length)];
			
			Item item = (Item)Activator.CreateInstance(lesserDecorType);
			return item;
		}

		private static Type[] m_GreaterDecorTypes = new Type[]
		{
			typeof(BlueCouchEastAddonDeed),
			typeof(BlueCouchNorthAddonDeed),
			typeof(BlueCouchNorthAddonDeed),
			typeof(BlueCouchWestAddonDeed),

			typeof(GreyDrkFPSouth3AddonDeed)
		};
		
		private static Item CreateDecorGreater(int type) {
			var greaterDecorType = m_GreaterDecorTypes[Utility.Random(m_GreaterDecorTypes.Length)];
			
			Item item = (Item)Activator.CreateInstance(greaterDecorType);
			return item;
		}

		private static Item CreatePowderForLeather(int type) {
			return new PowderForLeather();
		}

		private static Item CreateRunicKit(int type) {
			if (type >= 1 && type <= 3)
				return new RunicSewingKit(CraftResource.RegularLeather + type, 60 - (type * 15));

			throw new InvalidOperationException();
		}

		private static Item CreatePowerScroll(int type) {
			if (type == 5 || type == 10 || type == 15 || type == 20)
				return new PowerScroll(SkillName.Tailoring, 100 + type);

			throw new InvalidOperationException();
		}
			private static Item CreateAncientSewingKit(int type) {
			if (type == 10 || type == 15 || type == 30 || type == 60)
				return new AncientSewingKit(type);

			throw new InvalidOperationException();
		}
		/*
		private static Item CreateCBD( int type )
		{
			return new ClothingBlessDeed();
		}
		*/
		#endregion

		public static readonly TailorRewardCalculator Instance = new TailorRewardCalculator();

		public override int ComputePoints(int quantity, bool exceptional, BulkMaterialType material, int itemCount, Type type) {
			int points = 0;

			if (quantity == 10)
				points += 10;
			else if (quantity == 15)
				points += 25;
			else if (quantity == 20)
				points += 50;

			if (exceptional)
				points += 100;

			if (itemCount == 4)
				points += 300;
			else if (itemCount == 5)
				points += 400;
			else if (itemCount == 6)
				points += 500;

			if (material == BulkMaterialType.Spined)
				points += 50;
			else if (material == BulkMaterialType.Horned)
				points += 100;
			else if (material == BulkMaterialType.Barbed)
				points += 150;

			return points;
		}

		#region oldGold
		// No more used, for historical reasons
		/*		private static int[][][] m_AosGoldTable = new int[][][]
					{
						new int[][] // 1-part (regular)
						{
							new int[]{ 150, 150, 300, 300 },
							new int[]{ 225, 225, 450, 450 },
							new int[]{ 300, 400, 600, 750 }
						},
						new int[][] // 1-part (exceptional)
						{
							new int[]{ 300, 300,  600,  600 },
							new int[]{ 450, 450,  900,  900 },
							new int[]{ 600, 750, 1200, 1800 }
						},
						new int[][] // 4-part (regular)
						{
							new int[]{  4000,  4000,  5000,  5000 },
							new int[]{  6000,  6000,  7500,  7500 },
							new int[]{  8000, 10000, 10000, 15000 }
						},
						new int[][] // 4-part (exceptional)
						{
							new int[]{  5000,  5000,  7500,  7500 },
							new int[]{  7500,  7500, 11250, 11250 },
							new int[]{ 10000, 15000, 15000, 20000 }
						},
						new int[][] // 5-part (regular)
						{
							new int[]{  5000,  5000,  7500,  7500 },
							new int[]{  7500,  7500, 11250, 11250 },
							new int[]{ 10000, 15000, 15000, 20000 }
						},
						new int[][] // 5-part (exceptional)
						{
							new int[]{  7500,  7500, 10000, 10000 },
							new int[]{ 11250, 11250, 15000, 15000 },
							new int[]{ 15000, 20000, 20000, 30000 }
						},
						new int[][] // 6-part (regular)
						{
							new int[]{  7500,  7500, 10000, 10000 },
							new int[]{ 11250, 11250, 15000, 15000 },
							new int[]{ 15000, 20000, 20000, 30000 }
						},
						new int[][] // 6-part (exceptional)
						{
							new int[]{ 10000, 10000, 15000, 15000 },
							new int[]{ 15000, 15000, 22500, 22500 },
							new int[]{ 20000, 30000, 30000, 50000 }
						}
					};

				private static int[][][] m_OldGoldTable = new int[][][]
					{
						new int[][] // 1-part (regular)
						{
							new int[]{ 150, 150, 300, 300 },
							new int[]{ 225, 225, 450, 450 },
							new int[]{ 300, 400, 600, 750 }
						},
						new int[][] // 1-part (exceptional)
						{
							new int[]{ 300, 300,  600,  600 },
							new int[]{ 450, 450,  900,  900 },
							new int[]{ 600, 750, 1200, 1800 }
						},
						new int[][] // 4-part (regular)
						{
							new int[]{  3000,  3000,  4000,  4000 },
							new int[]{  4500,  4500,  6000,  6000 },
							new int[]{  6000,  8000,  8000, 10000 }
						},
						new int[][] // 4-part (exceptional)
						{
							new int[]{  4000,  4000,  5000,  5000 },
							new int[]{  6000,  6000,  7500,  7500 },
							new int[]{  8000, 10000, 10000, 15000 }
						},
						new int[][] // 5-part (regular)
						{
							new int[]{  4000,  4000,  5000,  5000 },
							new int[]{  6000,  6000,  7500,  7500 },
							new int[]{  8000, 10000, 10000, 15000 }
						},
						new int[][] // 5-part (exceptional)
						{
							new int[]{  5000,  5000,  7500,  7500 },
							new int[]{  7500,  7500, 11250, 11250 },
							new int[]{ 10000, 15000, 15000, 20000 }
						},
						new int[][] // 6-part (regular)
						{
							new int[]{  5000,  5000,  7500,  7500 },
							new int[]{  7500,  7500, 11250, 11250 },
							new int[]{ 10000, 15000, 15000, 20000 }
						},
						new int[][] // 6-part (exceptional)
						{
							new int[]{  7500,  7500, 10000, 10000 },
							new int[]{ 11250, 11250, 15000, 15000 },
							new int[]{ 15000, 20000, 20000, 30000 }
						}
					};

				public override int ComputeGold(int quantity, bool exceptional, BulkMaterialType material, int itemCount, Type type)
				{
					int[][][] goldTable = (Core.AOS ? m_AosGoldTable : m_OldGoldTable);

					int typeIndex = ((itemCount == 6 ? 3 : itemCount == 5 ? 2 : itemCount == 4 ? 1 : 0) * 2) + (exceptional ? 1 : 0);
					int quanIndex = (quantity == 20 ? 2 : quantity == 15 ? 1 : 0);
					int mtrlIndex = (material == BulkMaterialType.Barbed ? 3 : material == BulkMaterialType.Horned ? 2 : material == BulkMaterialType.Spined ? 1 : 0);

					int gold = goldTable[typeIndex][quanIndex][mtrlIndex];

					int min = (gold * 9) / 10;
					int max = (gold * 10) / 9;

					return Utility.RandomMinMax(min, max);
				}*/
		#endregion

		public override int ComputeGold(int quantity, bool exceptional, BulkMaterialType material, int itemCount, Type type) {
			int quantityMultiplier = quantity;

			int exceptionalMultiplier = exceptional ? 2 : 1;

			int typeMultiplier;
			if (itemCount == 6)
				typeMultiplier = 500;
			else if (itemCount == 5)
				typeMultiplier = 400;
			else if (itemCount == 4)
				typeMultiplier = 300;
			else
				typeMultiplier = 15;

			double materialMultiplier;
			if (material == BulkMaterialType.Barbed)
				materialMultiplier = 2.5;
			else if (material == BulkMaterialType.Horned)
				materialMultiplier = 2;
			else if (material == BulkMaterialType.Spined)
				materialMultiplier = 1.5;
			else
				materialMultiplier = 1;

			int gold = (int)(quantityMultiplier * exceptionalMultiplier * typeMultiplier * materialMultiplier);

			return Utility.RandomMinMax((int)(gold * 0.95),(int)(gold * 1.05));
		}

		public TailorRewardCalculator() {
			Groups = new RewardGroup[]
				{
					new RewardGroup(   0, new RewardItem( 1, Cloth, 0 ) ),
					new RewardGroup(  50, new RewardItem( 1, Cloth, 1 ) ),
					new RewardGroup( 110, new RewardItem( 1, Cloth, 2 ) ),
					new RewardGroup( 150, new RewardItem( 9, Cloth, 3 ), new RewardItem( 1, Sandals ) ),
					new RewardGroup( 175, new RewardItem( 4, Cloth, 4 ), new RewardItem( 1, Sandals ) ),
					new RewardGroup( 200, new RewardItem( 1, PowderForLeather ) ),
					new RewardGroup( 250, new RewardItem( 1, DecorLesser ) ),
					new RewardGroup( 350, new RewardItem( 1, RunicKit, 1 ) ),
					new RewardGroup( 400, new RewardItem( 1, PowderForLeather ) ),
					new RewardGroup( 410, new RewardItem( 1, PowerScroll, 5 ) ),
					new RewardGroup( 425, new RewardItem( 1, PowderForLeather ) ),
					new RewardGroup( 450, new RewardItem( 1, AncientSewingKit, 10 ) ),
					new RewardGroup( 500, new RewardItem( 1, PowerScroll, 10 ) ),
					new RewardGroup( 525, new RewardItem( 1, AncientSewingKit, 15 ) ),
					new RewardGroup( 550, new RewardItem( 1, DecorGreater ) ),
					new RewardGroup( 575, new RewardItem( 1, PowerScroll, 15 ) ),
					new RewardGroup( 600, new RewardItem( 1, RunicKit, 2 ) ),
					new RewardGroup( 650, new RewardItem( 1, AncientSewingKit, 30 ) ),
					new RewardGroup( 660, new RewardItem( 1, PowerScroll, 20 ) ),
					new RewardGroup( 675, new RewardItem( 1, AncientSewingKit, 60 ) ),
					new RewardGroup( 700, new RewardItem( 1, RunicKit, 3 ) )
				};
		}
	}
}
