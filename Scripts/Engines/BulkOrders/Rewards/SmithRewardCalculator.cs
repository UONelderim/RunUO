using System;
using Server.Items;

namespace Server.Engines.BulkOrders
{
	public sealed class SmithRewardCalculator : RewardCalculator
	{
		#region Constructors
		private static readonly ConstructCallback SturdyShovel = CreateSturdyShovel;
		private static readonly ConstructCallback IronIngots = CreateIronIngots;
		private static readonly ConstructCallback MiningGloves = CreateMiningGloves;
		private static readonly ConstructCallback GargoylesPickaxe = CreateGargoylesPickaxe;
		private static readonly ConstructCallback ProspectorsTool = CreateProspectorsTool;
		private static readonly ConstructCallback PowderForMetal = CreatePowderForMetal;
		private static readonly ConstructCallback RunicHammer = CreateRunicHammer;
		private static readonly ConstructCallback PowerScroll = CreatePowerScroll;
		private static readonly ConstructCallback ColoredAnvil = CreateColoredAnvil;
		private static readonly ConstructCallback AncientHammer = CreateAncientHammer;
		private static readonly ConstructCallback MetalRares = CreateMetalRares;

		private static Item CreateSturdyShovel(int type) {
			return new SturdyShovel();
		}

		private static Item CreateIronIngots(int type) {
			return new IronIngot(10);
		}

		private static Item CreateMiningGloves(int type) {
			if (type == 5)
				return new LeatherGlovesOfMining(5);
			else if (type == 10)
				return new StuddedGlovesOfMining(10);
			else if (type == 15)
				return new RingmailGlovesOfMining(15);

			throw new InvalidOperationException();
		}

		private static Item CreateGargoylesPickaxe(int type) {
			return new GargoylesPickaxe();
		}

		private static Item CreateProspectorsTool(int type) {
			return new ProspectorsTool();
		}

		
		private static Item CreatePowderForMetal( int type )
		{
			return new PowderForMetal();
		}
		

		private static Item CreateRunicHammer(int type) {
			if (type >= 1 && type <= 8)
				return new RunicHammer(CraftResource.Iron + type, Core.AOS ? (55 - (type * 5)) : 50);

			throw new InvalidOperationException();
		}

		private static Item CreatePowerScroll(int type) {
			if (type == 5 || type == 10 || type == 15 || type == 20)
				return new PowerScroll(SkillName.Blacksmith, 100 + type);

			throw new InvalidOperationException();
		}

		private static Item CreateColoredAnvil(int type) {
			// Generate an anvil deed, not an actual anvil.
			//return new ColoredAnvilDeed();

			return new ColoredAnvil();
		}

		private static Item CreateAncientHammer(int type) {
			if (type == 10 || type == 15 || type == 30 || type == 60)
				return new AncientSmithyHammer(type);

			throw new InvalidOperationException();
		}

		private static Type[] m_metalRareTypes = new Type[]
		{
			typeof(HorseShoes),
			typeof(ForgedMetal),
			typeof(IronWire),
			typeof(CopperWire),
			typeof(GoldWire),
			typeof(SilverWire),
		};
		
		private static Item CreateMetalRares(int type) {
			var metalRareType = m_metalRareTypes[Utility.Random(m_metalRareTypes.Length)];
			
			Item item = (Item)Activator.CreateInstance(metalRareType);
			return item;
		}
		#endregion

		public static readonly SmithRewardCalculator Instance = new();

		private RewardType[] m_Types = new RewardType[]
			{
				// Armors
				new( 200, typeof( RingmailGloves ), typeof( RingmailChest ), typeof( RingmailArms ), typeof( RingmailLegs ) ),
				new( 300, typeof( ChainCoif ), typeof( ChainLegs ), typeof( ChainChest ) ),
				new( 400, typeof( PlateArms ), typeof( PlateLegs ), typeof( PlateHelm ), typeof( PlateGorget ), typeof( PlateGloves ), typeof( PlateChest ) ),

				// Weapons
				new( 200, typeof( Bardiche ), typeof( Halberd ) ),
				new( 300, typeof( Dagger ), typeof( ShortSpear ), typeof( Spear ), typeof( WarFork ), typeof( Kryss ) ),	//OSI put the dagger in there.  Odd, ain't it.
				new( 350, typeof( Axe ), typeof( BattleAxe ), typeof( DoubleAxe ), typeof( ExecutionersAxe ), typeof( LargeBattleAxe ), typeof( TwoHandedAxe ) ),
				new( 350, typeof( Broadsword ), typeof( Cutlass ), typeof( Katana ), typeof( Longsword ), typeof( Scimitar ), /*typeof( ThinLongsword ),*/ typeof( VikingSword ) ),
				new( 350, typeof( WarAxe ), typeof( HammerPick ), typeof( Mace ), typeof( Maul ), typeof( WarHammer ), typeof( WarMace ) )
			};

		public override int ComputePoints(int quantity, bool exceptional, BulkMaterialType material, int itemCount, Type type) {
			int points = 0;

			if (quantity == 10)
				points += 10;
			else if (quantity == 15)
				points += 25;
			else if (quantity == 20)
				points += 50;

			if (exceptional)
				points += 200;

			if (itemCount > 1)
				points += LookupTypePoints(m_Types, type);

			if (material >= BulkMaterialType.DullCopper && material <= BulkMaterialType.Valorite)
				points += 200 + (50 * (material - BulkMaterialType.DullCopper));

			return points;
		}

		#region GoldTable
		// No more used, for historical reasons
		/*        private static int[][][] m_GoldTable = new int[][][]
					{
						new int[][] // 1-part (regular)
						{
							new int[]{ 150, 250, 250, 400,  400,  750,  750, 1200, 1200 },
							new int[]{ 225, 375, 375, 600,  600, 1125, 1125, 1800, 1800 },
							new int[]{ 300, 500, 750, 800, 1050, 1500, 2250, 2400, 4000 }
						},
						new int[][] // 1-part (exceptional)
						{
							new int[]{ 250, 400,  400,  750,  750, 1500, 1500, 3000,  3000 },
							new int[]{ 375, 600,  600, 1125, 1125, 2250, 2250, 4500,  4500 },
							new int[]{ 500, 800, 1200, 1500, 2500, 3000, 6000, 6000, 12000 }
						},
						new int[][] // Ringmail (regular)
						{
							new int[]{ 3000,  5000,  5000,  7500,  7500, 10000, 10000, 15000, 15000 },
							new int[]{ 4500,  7500,  7500, 11250, 11500, 15000, 15000, 22500, 22500 },
							new int[]{ 6000, 10000, 15000, 15000, 20000, 20000, 30000, 30000, 50000 }
						},
						new int[][] // Ringmail (exceptional)
						{
							new int[]{  5000, 10000, 10000, 15000, 15000, 25000,  25000,  50000,  50000 },
							new int[]{  7500, 15000, 15000, 22500, 22500, 37500,  37500,  75000,  75000 },
							new int[]{ 10000, 20000, 30000, 30000, 50000, 50000, 100000, 100000, 200000 }
						},
						new int[][] // Chainmail (regular)
						{
							new int[]{ 4000,  7500,  7500, 10000, 10000, 15000, 15000, 25000,  25000 },
							new int[]{ 6000, 11250, 11250, 15000, 15000, 22500, 22500, 37500,  37500 },
							new int[]{ 8000, 15000, 20000, 20000, 30000, 30000, 50000, 50000, 100000 }
						},
						new int[][] // Chainmail (exceptional)
						{
							new int[]{  7500, 15000, 15000, 25000,  25000,  50000,  50000, 100000, 100000 },
							new int[]{ 11250, 22500, 22500, 37500,  37500,  75000,  75000, 150000, 150000 },
							new int[]{ 15000, 30000, 50000, 50000, 100000, 100000, 200000, 200000, 200000 }
						},
						new int[][] // Platemail (regular)
						{
							new int[]{  5000, 10000, 10000, 15000, 15000, 25000,  25000,  50000,  50000 },
							new int[]{  7500, 15000, 15000, 22500, 22500, 37500,  37500,  75000,  75000 },
							new int[]{ 10000, 20000, 30000, 30000, 50000, 50000, 100000, 100000, 200000 }
						},
						new int[][] // Platemail (exceptional)
						{
							new int[]{ 10000, 25000,  25000,  50000,  50000, 100000, 100000, 100000, 100000 },
							new int[]{ 15000, 37500,  37500,  75000,  75000, 150000, 150000, 150000, 150000 },
							new int[]{ 20000, 50000, 100000, 100000, 200000, 200000, 200000, 200000, 200000 }
						},
						new int[][] // 2-part weapons (regular)
						{
							new int[]{ 3000, 0, 0, 0, 0, 0, 0, 0, 0 },
							new int[]{ 4500, 0, 0, 0, 0, 0, 0, 0, 0 },
							new int[]{ 6000, 0, 0, 0, 0, 0, 0, 0, 0 }
						},
						new int[][] // 2-part weapons (exceptional)
						{
							new int[]{ 5000, 0, 0, 0, 0, 0, 0, 0, 0 },
							new int[]{ 7500, 0, 0, 0, 0, 0, 0, 0, 0 },
							new int[]{ 10000, 0, 0, 0, 0, 0, 0, 0, 0 }
						},
						new int[][] // 5-part weapons (regular)
						{
							new int[]{ 4000, 0, 0, 0, 0, 0, 0, 0, 0 },
							new int[]{ 6000, 0, 0, 0, 0, 0, 0, 0, 0 },
							new int[]{ 8000, 0, 0, 0, 0, 0, 0, 0, 0 }
						},
						new int[][] // 5-part weapons (exceptional)
						{
							new int[]{ 7500, 0, 0, 0, 0, 0, 0, 0, 0 },
							new int[]{ 11250, 0, 0, 0, 0, 0, 0, 0, 0 },
							new int[]{ 15000, 0, 0, 0, 0, 0, 0, 0, 0 }
						},
						new int[][] // 6-part weapons (regular)
						{
							new int[]{ 4000, 0, 0, 0, 0, 0, 0, 0, 0 },
							new int[]{ 6000, 0, 0, 0, 0, 0, 0, 0, 0 },
							new int[]{ 10000, 0, 0, 0, 0, 0, 0, 0, 0 }
						},
						new int[][] // 6-part weapons (exceptional)
						{
							new int[]{ 7500, 0, 0, 0, 0, 0, 0, 0, 0 },
							new int[]{ 11250, 0, 0, 0, 0, 0, 0, 0, 0 },
							new int[]{ 15000, 0, 0, 0, 0, 0, 0, 0, 0 }
						}
					};*/
		#endregion

		#region OldGoldCompute
		/*		private int ComputeType(Type type, int itemCount) {
					// Item count of 1 means it's a small BOD.
					if (itemCount == 1)
						return 0;

					int typeIdx;

					// Loop through the RewardTypes defined earlier and find the correct one.
					for (typeIdx = 0; typeIdx < 7; ++typeIdx) {
						if (m_Types[typeIdx].Contains(type))
							break;
					}

					// Types 5, 6 and 7 are Large Weapon BODs with the same rewards.
					if (typeIdx > 5)
						typeIdx = 5;

					return (typeIdx + 1) * 2;
				}

					public int ComputeGold( int quantity, bool exceptional, BulkMaterialType material, int itemCount, Type type )
						{
							int[][][] goldTable = m_GoldTable;

							int typeIndex = ComputeType( type, itemCount );
							int quanIndex = ( quantity == 20 ? 2 : quantity == 15 ? 1 : 0 );
							int mtrlIndex = ( material >= BulkMaterialType.DullCopper && material <= BulkMaterialType.Valorite ) ? 1 + (int)(material - BulkMaterialType.DullCopper) : 0;

							if ( exceptional )
								typeIndex++;

							int gold = goldTable[typeIndex][quanIndex][mtrlIndex];

							int min = (gold * 9) / 10;
							int max = (gold * 10) / 9;

							return Utility.RandomMinMax( min, max );
						}*/
		#endregion

		public override int ComputeGold(int quantity, bool exceptional, BulkMaterialType material, int itemCount, Type type) {

			int quantityMultiplier = quantity;

			int exceptionalMultiplier = exceptional ? 2 : 1;

			int typeMultiplier = 15;
			if(itemCount > 1)
				typeMultiplier = LookupTypePoints(m_Types, type);

			int materialMultiplier = 1;
			if (material != BulkMaterialType.None) {
				materialMultiplier = (int)(2 + (material - BulkMaterialType.DullCopper) * 1.65);
			}

			int gold = quantityMultiplier * exceptionalMultiplier * typeMultiplier * materialMultiplier;

			return Utility.RandomMinMax((int)(gold * 0.95), (int)(gold * 1.05));
		}

		public SmithRewardCalculator() {
			Groups = new RewardGroup[]
				{
					new(    0, new RewardItem( 1, SturdyShovel ) ),
					new(   25, new RewardItem( 1, IronIngots ) ),
					new(   50, new RewardItem( 45, SturdyShovel ), new RewardItem( 45, IronIngots ), new RewardItem( 10, MiningGloves, 5 ) ),
					new(  200, new RewardItem( 45, GargoylesPickaxe ), new RewardItem( 45, ProspectorsTool ), new RewardItem( 10, MiningGloves, 10 ) ),
					new(  400, new RewardItem( 2, GargoylesPickaxe ), new RewardItem( 2, ProspectorsTool ), new RewardItem( 1, PowderForMetal ) ),
					new(  450, new RewardItem( 9, PowderForMetal ), new RewardItem( 1, MiningGloves, 15 ) ),
					new(  500, new RewardItem( 1, RunicHammer, 1 ) ),
					new(  550, new RewardItem( 3, RunicHammer, 1 ), new RewardItem( 2, RunicHammer, 2 ) ),
					new(  600, new RewardItem( 7, RunicHammer, 2 ), new RewardItem( 3, RunicHammer, 3 ) ),		//30% szans na wyzszy mlotek runiczny
					new(  625, new RewardItem( 3, RunicHammer, 2 ), new RewardItem( 6, PowerScroll, 5 ), new RewardItem( 1, ColoredAnvil ) ),
					new(  650, new RewardItem( 7, RunicHammer, 3 ), new RewardItem( 3, RunicHammer, 4 ) ),		//30% szans na wyzszy mlotek runiczny
					new(  675, new RewardItem( 1, ColoredAnvil ), new RewardItem( 6, PowerScroll, 10 ), new RewardItem( 3, RunicHammer, 3 ) ),
					new(  700, new RewardItem( 7, RunicHammer, 4 ), new RewardItem( 3, RunicHammer, 5 ) ),		//30% szans na wyzszy mlotek runiczny
					new(  750, new RewardItem( 1, AncientHammer, 10 ) ),
					new(  800, new RewardItem( 1, PowerScroll, 15 ) ),
					new(  850, new RewardItem( 1, AncientHammer, 15 ) ),
					new(  900, new RewardItem( 1, PowerScroll, 20 ) ),
					new(  950, new RewardItem( 7, RunicHammer, 5 ), new RewardItem( 3, RunicHammer, 6 ) ),		//30% szans na wyzszy mlotek runiczny
					new( 1000, new RewardItem( 1, AncientHammer, 30 ) ),
					new( 1050, new RewardItem( 7, RunicHammer, 6 ), new RewardItem( 3, RunicHammer, 7 ) ),		//30% szans na wyzszy mlotek runiczny
					new( 1100, new RewardItem( 1, AncientHammer, 60 ) ),
					new( 1150, new RewardItem( 7, RunicHammer, 7 ), new RewardItem( 3, RunicHammer, 8 ) ),		//30% szans na wyzszy mlotek runiczny
					new( 1200, new RewardItem( 1, RunicHammer, 8 ) )
				};
		}
	}
}
