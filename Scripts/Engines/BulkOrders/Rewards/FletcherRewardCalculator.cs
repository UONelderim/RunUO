using System;
using System.Collections.Generic;
using Server;
using Server.Items;

namespace Server.Engines.BulkOrders
{
	public sealed class FletcherRewardCalculator : RewardCalculator
	{
		#region Constructors

		private static readonly ConstructCallback SturdyAxe = new ConstructCallback(CreateSturdyAxe);
		private static readonly ConstructCallback TreefellowsAxe = new ConstructCallback(CreateTreefellowsAxe);
		private static readonly ConstructCallback WoodProspectorsTool = new ConstructCallback(CreateWoodProspectorsTool);
		private static readonly ConstructCallback LumberjackingGloves = new ConstructCallback(CreateLumberjackingGloves);
		private static readonly ConstructCallback PowerScroll = new ConstructCallback(CreatePowerScroll);
		private static readonly ConstructCallback PowderForWood = new ConstructCallback( CreatePowderForWood );
		private static readonly ConstructCallback BowRares = new ConstructCallback(CreateBowRares);
		private static readonly ConstructCallback Equiver = new ConstructCallback(CreateEquiver);
		private static readonly ConstructCallback RunicTool = new ConstructCallback(CreateRunicTool);
		private static readonly ConstructCallback AncientTool = new ConstructCallback(CreateAncientTool);


		private static Item CreateSturdyAxe(int type) {
			// Wzmocniona siekiera
			return new SturdyAxe();
		}

		private static Item CreateTreefellowsAxe(int type) {
			// Odpowiednik gorniczego GargoylesPickaxe.

			return new TreefellowsAxe();
		}

		private static Item CreateWoodProspectorsTool(int type) {
			// Odpowiednik gorniczego ProspectorsTool.
			return new WoodProspectorsTool();
		}

		private static Item CreateLumberjackingGloves(int type) {
			// Rekawiczki dodajace skilla drwalnictwo.
			if (type == 1)
				return new LeatherGlovesOfLumberjacking(1);
			else if (type == 3)
				return new StuddedGlovesOfLumberjacking(3);
			else if (type == 5)
				return new RingmailGlovesOfLumberjacking(5);

			throw new InvalidOperationException();
		}

		private static Item CreateBowRares(int type) {
			// Ozdobne itemki. Propozycje:
			// 0x0F41 - strzaly
			// 0x1BFD - belty
			// 0x1BD6 - szafty
			// 0x1BD9 - deski
			// 0x1BDF - klody
			// 0x155D - dekoracja na sciane: luk i strzaly
			// 0x1B9B - kupa chrustu (galezie)
			// 0x2D2A - luk elfi
			// 0x0E56 - pien z wbita siekiera

			/*
            switch( Utility.Random(3) )
			{
				case 2:
					return new HorseShoes();
				case 1:
					return new ForgedMetal();
				default:
					{
						switch ( Utility.Random(4) )
						{
							case 3:
								return new IronWire();
							case 2:
								return new CopperWire();
							case 1:
								return new GoldWire();
							default:
								return new SilverWire();
						}
					}
            }
            */
			return null;
		}


		private static Item CreateEquiver(int type) {
			switch (Utility.Random(4)) {
				case 0: return new QuiverOfBlight();
				case 1: return new QuiverOfFire();
				case 2: return new QuiverOfIce();
				case 3: return new QuiverOfLightning();
			}
			throw new InvalidOperationException();
		}

		private static Item CreatePowerScroll(int type) {
			if (type == 5 || type == 10 || type == 15 || type == 20)
				return new PowerScroll(SkillName.Fletching, 100 + type);

			throw new InvalidOperationException();
		}

		private static Item CreatePowderForWood(int type) {
			return new PowderForWood();
		}

		private static Item CreateRunicTool(int type) {
			if (type >= 1 && type <= 6)
				return new RunicFletcherTools(CraftResource.RegularWood + type, 45 - (type * 5));

			throw new InvalidOperationException();
		}

		private static Item CreateAncientTool(int type) {
			if (type == 10 || type == 15 || type == 30 || type == 60)
				return new AncientFletcherTools(type);

			throw new InvalidOperationException();
		}
		#endregion

		public static readonly FletcherRewardCalculator Instance = new FletcherRewardCalculator();



		public override int ComputePoints(SmallBOD bod) {
			return ComputePoints(bod.AmountMax, bod.RequireExceptional, bod.Material, bod.Material2, false);
		}

		public override int ComputePoints(LargeBOD bod) {
			return ComputePoints(bod.AmountMax, bod.RequireExceptional, bod.Material, bod.Material2, true);
		}

		public override int ComputeGold(SmallBOD bod) {
			return ComputeGold(bod.AmountMax, bod.RequireExceptional, bod.Material, bod.Material2, false);
		}

		public override int ComputeGold(LargeBOD bod) {
			return ComputeGold(bod.AmountMax, bod.RequireExceptional, bod.Material, bod.Material2, true);
		}

		public int ComputePoints(int quantity, bool exceptional, BulkMaterialType material, BulkMaterialType material2, bool LBOD) {
			int points = 0;

			if (quantity == 10)
				points += 10;
			else if (quantity == 15)
				points += 25;
			else if (quantity == 20)
				points += 50;

			if (exceptional)
				points += 200;

			if (material >= BulkMaterialType.Oak && material <= BulkMaterialType.Frostwood)
				points += 200 + ((material - BulkMaterialType.Oak) * 70);

			if (LBOD && material2 >= BulkMaterialType.BowstringLeather && material2 <= BulkMaterialType.BowstringSilk) {
				points += 200 + ((material2 - BulkMaterialType.BowstringLeather) * 70);
			}

			return points;
		}


		public int ComputeGold(int quantity, bool exceptional, BulkMaterialType material, BulkMaterialType material2, bool LBOD) {
			int quantityMultiplier = quantity;
			int exceptionalMultiplier = exceptional ? 2 : 1;
			int materialMultiplier = 1;
			if (material != BulkMaterialType.None) {
				materialMultiplier = 3 + (material - BulkMaterialType.Oak + 1) * 2;
			}
			int material2Multiplier = 15;
			if (LBOD) {
				material2Multiplier = 200 + ((material2 - BulkMaterialType.BowstringLeather) * 70);
			}
			int gold =  quantityMultiplier * exceptionalMultiplier * materialMultiplier * material2Multiplier;

			return Utility.RandomMinMax((int)(gold * 0.95), (int)(gold * 1.05));
		}

		public override int ComputePoints(int quantity, bool exceptional, BulkMaterialType material, int itemCount, Type type) {
			throw new NotImplementedException();
		}

		public override int ComputeGold(int quantity, bool exceptional, BulkMaterialType material, int itemCount, Type type) {
			throw new NotImplementedException();
		}

		public FletcherRewardCalculator() {
			Groups = new RewardGroup[]
				{	
					new RewardGroup(   0, new RewardItem(1, SturdyAxe)),
					new RewardGroup(  50, new RewardItem(9, SturdyAxe), new RewardItem(1, LumberjackingGloves, 1)),
					new RewardGroup( 200, new RewardItem(45, TreefellowsAxe), new RewardItem(45, WoodProspectorsTool), new RewardItem(10, LumberjackingGloves, 3)),
					new RewardGroup( 400, new RewardItem(4, TreefellowsAxe), new RewardItem(4, WoodProspectorsTool), new RewardItem(2, PowderForWood)),
					new RewardGroup( 450, new RewardItem(9, PowderForWood), new RewardItem(1, LumberjackingGloves, 5)),
					new RewardGroup( 500, new RewardItem(5, PowderForWood), new RewardItem(5, Equiver)),
					new RewardGroup( 550, new RewardItem(1, Equiver)),
					new RewardGroup( 600, new RewardItem(7, Equiver), new RewardItem(3, RunicTool, 1)),
					new RewardGroup( 625, new RewardItem(6, PowerScroll, 5), new RewardItem(4, RunicTool, 1)),
					new RewardGroup( 650, new RewardItem(7, RunicTool, 1), new RewardItem(3, RunicTool, 2)),
					new RewardGroup( 675, new RewardItem(6, PowerScroll, 10), new RewardItem(4, RunicTool, 2)),
					new RewardGroup( 700, new RewardItem(7, RunicTool, 2), new RewardItem(3, RunicTool, 3)),
					new RewardGroup( 750, new RewardItem(1, AncientTool, 10)),
					new RewardGroup( 800, new RewardItem(1, PowerScroll, 15)),
					new RewardGroup( 850, new RewardItem(1, AncientTool, 15)),
					new RewardGroup( 900, new RewardItem(1, PowerScroll, 20)),
					new RewardGroup( 950, new RewardItem(7, RunicTool, 3), new RewardItem(3, RunicTool, 4)),
					new RewardGroup(1000, new RewardItem(1, AncientTool, 30)),
					new RewardGroup(1050, new RewardItem(7, RunicTool, 4), new RewardItem(3, RunicTool, 5)),
					new RewardGroup(1100, new RewardItem(1, AncientTool, 60)),
					new RewardGroup(1150, new RewardItem(7, RunicTool, 5), new RewardItem(3, RunicTool, 6)),
					new RewardGroup(1200, new RewardItem(1, RunicTool, 6)),
				};
		}
	}
}
