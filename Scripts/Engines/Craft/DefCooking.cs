using System;
using Server.Items;

namespace Server.Engines.Craft
{
	public class DefCooking : CraftSystem
	{
		public override SkillName MainSkill
		{
			get	{ return SkillName.Cooking;	}
		}

		public override int GumpTitleNumber
		{
			get { return 1044003; } // <CENTER>COOKING MENU</CENTER>
		}

		private static CraftSystem m_CraftSystem;

		public static CraftSystem CraftSystem
		{
			get
			{
				if ( m_CraftSystem == null )
					m_CraftSystem = new DefCooking();

				return m_CraftSystem;
			}
		}

		public override CraftECA ECA{ get{ return CraftECA.ChanceMinusSixtyToFourtyFive; } }

		public override double GetChanceAtMin( CraftItem item )
		{
			return 0.0; // 0%
		}

		private DefCooking() : base( 1, 1, 1.25 )// base( 1, 1, 1.5 )
		{
		}

		public override int CanCraft( Mobile from, BaseTool tool, Type itemType )
		{
			if( tool == null || tool.Deleted || tool.UsesRemaining < 0 )
				return 1044038; // You have worn out your tool!
			else if ( !BaseTool.CheckAccessible( tool, from ) )
				return 1044263; // The tool must be on your person to use.

			return 0;
		}

		public override void PlayCraftEffect( Mobile from )
		{
		}

		public override int PlayEndingEffect( Mobile from, bool failed, bool lostMaterial, bool toolBroken, int quality, bool makersMark, CraftItem item )
		{
			if ( toolBroken )
				from.SendLocalizedMessage( 1044038 ); // You have worn out your tool

			if ( failed )
			{
				if ( lostMaterial )
					return 1044043; // You failed to create the item, and some of your materials are lost.
				else
					return 1044157; // You failed to create the item, but no materials were lost.
			}
			else
			{
				if ( quality == 0 )
					return 502785; // You were barely able to make this item.  It's quality is below average.
				else if ( makersMark && quality == 2 )
					return 1044156; // You create an exceptional quality item and affix your maker's mark.
				else if ( quality == 2 )
					return 1044155; // You create an exceptional quality item.
				else				
					return 1044154; // You create the item.
			}
		}

		public override void InitCraftList()
		{
			int index = -1;

			/* Begin Ingredients */
			index = AddCraft( typeof( SackFlour ), 1044495, 1024153, 0.0, 100.0, typeof( WheatSheaf ), 1044489, 2, 1044490 );
			SetNeedMill( index, true );

			index = AddCraft( typeof( Dough ), 1044495, 1024157, 0.0, 100.0, typeof( SackFlour ), 1044468, 1, 1044253 );
			AddRes( index, typeof( BaseBeverage ), 1046458, 1, 1044253 );

			index = AddCraft( typeof( SweetDough ), 1044495, 1041340, 0.0, 100.0, typeof( Dough ), 1024157, 1, 1044253 );
			AddRes( index, typeof( JarHoney ), 1044472, 1, 1044253 );

			index = AddCraft( typeof( CakeMix ), 1044495, 1041002, 0.0, 100.0, typeof( SackFlour ), 1044468, 1, 1044253 );
			AddRes( index, typeof( SweetDough ), 1044475, 1, 1044253 );

			index = AddCraft( typeof( CookieMix ), 1044495, 1024159, 0.0, 100.0, typeof( JarHoney ), 1044472, 1, 1044253 );
			AddRes( index, typeof( SweetDough ), 1044475, 1, 1044253 );
			/* End Ingredients */

			/* Begin Preparations */
			index = AddCraft( typeof( UnbakedQuiche ), 1044496, 1041339, 0.0, 100.0, typeof( Dough ), 1044469, 1, 1044253 );
			AddRes( index, typeof( Eggs ), 1044477, 1, 1044253 );

			// TODO: This must also support chicken and lamb legs
			index = AddCraft( typeof( UnbakedMeatPie ), 1044496, 1041338, 0.0, 100.0, typeof( Dough ), 1044469, 1, 1044253 );
			AddRes( index, typeof( RawRibs ), 1044482, 1, 1044253 );

			index = AddCraft( typeof( UncookedSausagePizza ), 1044496, 1041337, 0.0, 100.0, typeof( Dough ), 1044469, 1, 1044253 );
			AddRes( index, typeof( Sausage ), 1044483, 1, 1044253 );

			index = AddCraft( typeof( UncookedCheesePizza ), 1044496, 1041341, 0.0, 100.0, typeof( Dough ), 1044469, 1, 1044253 );
			AddRes( index, typeof( CheeseWheel ), 1044486, 1, 1044253 );

			index = AddCraft( typeof( UnbakedFruitPie ), 1044496, 1041334, 0.0, 100.0, typeof( Dough ), 1044469, 1, 1044253 );
			AddRes( index, typeof( Pear ), 1044481, 1, 1044253 );

			index = AddCraft( typeof( UnbakedPeachCobbler ), 1044496, 1041335, 0.0, 100.0, typeof( Dough ), 1044469, 1, 1044253 );
			AddRes( index, typeof( Peach ), 1044480, 1, 1044253 );

			index = AddCraft( typeof( UnbakedApplePie ), 1044496, 1041336, 0.0, 100.0, typeof( Dough ), 1044469, 1, 1044253 );
			AddRes( index, typeof( Apple ), 1044479, 1, 1044253 );

			index = AddCraft( typeof( UnbakedPumpkinPie ), 1044496, 1041342, 0.0, 100.0, typeof( Dough ), 1044469, 1, 1044253 );
			AddRes( index, typeof( Pumpkin ), 1044484, 1, 1044253 );

			if ( Core.SE )
			{
				index = AddCraft( typeof( GreenTea ), 1044496, 1030315, 80.0, 130.0, typeof( GreenTeaBasket ), 1030316, 1, 1044253 );
				AddRes( index, typeof( BaseBeverage ), 1046458, 1, 1044253 );
				SetNeededExpansion( index, Expansion.SE );
				SetNeedOven( index, true );

				index = AddCraft( typeof( WasabiClumps ), 1044496, 1029451, 70.0, 120.0, typeof( BaseBeverage ), 1046458, 1, 1044253 );
				AddRes( index, typeof( WoodenBowlOfPeas ), 1025633, 3, 1044253 );
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( SushiRolls ), 1044496, 1030303, 90.0, 120.0, typeof( BaseBeverage ), 1046458, 1, 1044253 );
				AddRes( index, typeof( RawFishSteak ), 1044476, 10, 1044253 );
				SetNeededExpansion( index, Expansion.SE );

				index = AddCraft( typeof( SushiPlatter ), 1044496, 1030305, 90.0, 120.0, typeof( BaseBeverage ), 1046458, 1, 1044253 );
				AddRes( index, typeof( RawFishSteak ), 1044476, 10, 1044253 );
				SetNeededExpansion( index, Expansion.SE );
			}

			index = AddCraft( typeof( TribalPaint ), 1044496, 1040000, 80.0, 80.0, typeof( SackFlour ), 1044468, 1, 1044253 );
			AddRes( index, typeof( TribalBerry ), 1046460, 1, 1044253 );
			
			index = AddCraft( typeof( AquariumFood ), 1044496, 1074819, 50.0, 80.0, typeof( SackFlour ), 1044468, 1, 1044253 );
			AddRes( index, typeof( Ginseng), "Zen-szen", 3, 1044253 );
			
			index = AddCraft( typeof( VacationWafer ), 1044496, 1074431, 50.0, 80.0, typeof( SackFlour ), 1044468, 1, 1044253 );
			AddRes( index, typeof( Garlic ), "czosnek", 3, 1044253 );
			
			
			index = AddCraft( typeof( PetBondingPotion ), "Wywary" , "wywar oswajacza" , 80.0, 100.0, typeof( BaseBeverage ), 1046458, 5, 1044253 );
			AddRes( index, typeof( Gold ), "zloto" , 8000, 1044253 );
			AddRes( index, typeof( TribalBerry ), "tubylcza jagoda" , 5, 1044253 );
			AddRes( index, typeof( Beeswax ), "wosk" , 35, 1044253 );

			index = AddCraft( typeof( AdmiralsHeartyRum ), "Wywary" , "Krasnoludzki Rum" , 45.0, 78.0, typeof( BaseBeverage ), 1046458, 1, 1044253 );
			AddRes( index, typeof( SheafOfHay ), "snop siana" , 2, 1044253 );
			AddRes( index, typeof( SmallFlask ), 1044610, 1, 1044253 );

			index = AddCraft( typeof( WywarSily ), "Wywary" , "Wywar Sily" , 90.0, 150.0, typeof( BaseBeverage ), 1046458, 5, 1044253 );
			AddRes( index, typeof( Onion ), "cebula" , 10, 1044253 );
			AddRes( index, typeof( Carrot ), "marchew" , 10, 1044253 );
			AddRes( index, typeof( JarHoneyGryczany ), "sloik miodu gryczanego" , 1, 1044253 );

			index = AddCraft( typeof( WywarZrecznosci ), "Wywary" , "Wywar Zrecznosci" , 90.0, 150.0, typeof( BaseBeverage ), 1046458, 5, 1044253 );
			AddRes( index, typeof( Cabbage ), "kapusta" , 10, 1044253 );
			AddRes( index, typeof( JarHoneySpadziowy), "sloik miodu spadziwoego" , 1, 1044253 );
			AddRes( index, typeof( BottleSheepMilk ), "Butelka Owczego Mleka" , 10, 1044253 );

			index = AddCraft( typeof( WywarInteligencji ), "Wywary" , "Wywar Inteligencji" , 90.0, 150.0, typeof( BottleGoatMilk ), "Butelka Koziego Mleka", 5, 1044253 );
			AddRes( index, typeof( Corn ), "kukurydza" , 10, 1044253 );
			AddRes( index, typeof( Eggs), "jajka" , 15, 1044253 );
			AddRes( index, typeof( BottleCowMilk ), "Butelka Krowiego Mleka" , 10, 1044253 );

			index = AddCraft( typeof( OrangePetalsBrew ), "Wywary" , "Wywar Odpornosci na trucizny" , 90.0, 150.0, typeof( BottleGoatMilk ), "Butelka Koziego Mleka", 5, 1044253 );
			AddRes( index, typeof( Carrot ), "marchew" , 10, 1044253 );
			AddRes( index, typeof( Taint), "skaza" , 15, 1044253 );
			AddRes( index, typeof( Corruption ), "Zepsucie" , 10, 1044253 );
			

			
			if ( Core.SE )
			{
				index = AddCraft( typeof( EggBomb ), 1044496, 1030249, 90.0, 120.0, typeof( Eggs ), 1044477, 1, 1044253 );
				AddRes( index, typeof( SackFlour ), 1044468, 3, 1044253 );
				SetNeededExpansion( index, Expansion.SE );
			}

			index = AddCraft(typeof(WoodPulp), 1044496, 1060630, 60.0, 100.0, typeof(BarkFragment), 1032687, 1, 1044253);
			AddRes(index, typeof(BaseBeverage), 1046458, 1, 1044253);
			/* End Preparations */

			/* Begin Baking */
			index = AddCraft( typeof( BreadLoaf ), 1044497, 1024156, 0.0, 100.0, typeof( Dough ), 1044469, 1, 1044253 );
			SetNeedOven( index, true );

			index = AddCraft( typeof( Cookies ), 1044497, 1025643, 0.0, 100.0, typeof( CookieMix ), 1044474, 1, 1044253 );
			SetNeedOven( index, true );

			index = AddCraft( typeof( Cake ), 1044497, 1022537, 0.0, 100.0, typeof( CakeMix ), 1044471, 1, 1044253 );
			SetNeedOven( index, true );

			index = AddCraft( typeof( Muffins ), 1044497, 1022539, 0.0, 100.0, typeof( SweetDough ), 1044475, 1, 1044253 );
			SetNeedOven( index, true );

			index = AddCraft( typeof( Quiche ), 1044497, 1041345, 0.0, 100.0, typeof( UnbakedQuiche ), 1044518, 1, 1044253 );
			SetNeedOven( index, true );

			index = AddCraft( typeof( MeatPie ), 1044497, 1041347, 0.0, 100.0, typeof( UnbakedMeatPie ), 1044519, 1, 1044253 );
			SetNeedOven( index, true );

			index = AddCraft( typeof( SausagePizza ), 1044497, 1044517, 0.0, 100.0, typeof( UncookedSausagePizza ), 1044520, 1, 1044253 );
			SetNeedOven( index, true );

			index = AddCraft( typeof( CheesePizza ), 1044497, 1044516, 0.0, 100.0, typeof( UncookedCheesePizza ), 1044521, 1, 1044253 );
			SetNeedOven( index, true );

			index = AddCraft( typeof( FruitPie ), 1044497, 1041346, 0.0, 100.0, typeof( UnbakedFruitPie ), 1044522, 1, 1044253 );
			SetNeedOven( index, true );

			index = AddCraft( typeof( PeachCobbler ), 1044497, 1041344, 0.0, 100.0, typeof( UnbakedPeachCobbler ), 1044523, 1, 1044253 );
			SetNeedOven( index, true );

			index = AddCraft( typeof( ApplePie ), 1044497, 1041343, 0.0, 100.0, typeof( UnbakedApplePie ), 1044524, 1, 1044253 );
			SetNeedOven( index, true );

			index = AddCraft( typeof( PumpkinPie ), 1044497, 1041348, 0.0, 100.0, typeof( UnbakedPumpkinPie ), 1046461, 1, 1044253 );
			SetNeedOven( index, true );


			if ( Core.SE )
			{
				index = AddCraft( typeof( MisoSoup ), 1044497, 1030317, 60.0, 110.0, typeof( RawFishSteak ), 1044476, 1, 1044253 );
				AddRes( index, typeof( BaseBeverage ), 1046458, 1, 1044253 );
				SetNeededExpansion( index, Expansion.SE );
				SetNeedOven( index, true );

				index = AddCraft( typeof( WhiteMisoSoup ), 1044497, 1030318, 60.0, 110.0, typeof( RawFishSteak ), 1044476, 1, 1044253 );
				AddRes( index, typeof( BaseBeverage ), 1046458, 1, 1044253 );
				SetNeededExpansion( index, Expansion.SE );
				SetNeedOven( index, true );

				index = AddCraft( typeof( RedMisoSoup ), 1044497, 1030319, 60.0, 110.0, typeof( RawFishSteak ), 1044476, 1, 1044253 );
				AddRes( index, typeof( BaseBeverage ), 1046458, 1, 1044253 );
				SetNeededExpansion( index, Expansion.SE );
				SetNeedOven( index, true );

				index = AddCraft( typeof( AwaseMisoSoup ), 1044497, 1030320, 60.0, 110.0, typeof( RawFishSteak ), 1044476, 1, 1044253 );
				AddRes( index, typeof( BaseBeverage ), 1046458, 1, 1044253 );
				SetNeededExpansion( index, Expansion.SE );
				SetNeedOven( index, true );
			}
			index = AddCraft( typeof( Potrawka ), 1044497 , "pożywne klopsiki" , 55.0, 80.0, typeof( BaseBeverage ), 1046458, 5, 1044253 );
			AddRes( index, typeof( SweetDough ), 1044475, 1, 1044253 );
			AddRes( index, typeof( UnbakedQuiche ), 1044518, 1, 1044253 );
			SetNeedOven( index, true );
			index = AddCraft( typeof( PotrawkaBle ), 1044497 , "klopsiki z dynią" , 55.0, 80.0, typeof( BaseBeverage ), 1046458, 5, 1044253 );
			AddRes( index, typeof( Eggs ), 1044477, 1, 1044253 );
			AddRes( index, typeof( RawFishSteak ), 1044476, 10, 1044253 );
			SetNeedOven( index, true );
			index = AddCraft( typeof( PysznaPotrawka ), 1044497 , "tłuste klopsiki" , 55.0, 80.0, typeof( BaseBeverage ), 1046458, 5, 1044253 );
			AddRes( index, typeof( Dough ), "ciasto", 1, 1044253 );
			AddRes( index, typeof( Pumpkin ), 1044484, 1, 1044253 );
			SetNeedOven( index, true );
			/* End Baking */

			/* Begin Barbecue */
			index = AddCraft( typeof( CookedBird ), 1044498, 1022487, 0.0, 100.0, typeof( RawBird ), 1044470, 1, 1044253 );
			SetNeedHeat( index, true );
			SetUseAllRes( index, true );

			index = AddCraft( typeof( ChickenLeg ), 1044498, 1025640, 0.0, 100.0, typeof( RawChickenLeg ), 1044473, 1, 1044253 );
			SetNeedHeat( index, true );
			SetUseAllRes( index, true );

			index = AddCraft( typeof( FishSteak ), 1044498, 1022427, 0.0, 100.0, typeof( RawFishSteak ), 1044476, 1, 1044253 );
			SetNeedHeat( index, true );
			SetUseAllRes( index, true );

			index = AddCraft( typeof( FriedEggs ), 1044498, 1022486, 0.0, 100.0, typeof( Eggs ), 1044477, 1, 1044253 );
			SetNeedHeat( index, true );
			SetUseAllRes( index, true );

			index = AddCraft( typeof( LambLeg ), 1044498, 1025642, 0.0, 100.0, typeof( RawLambLeg ), 1044478, 1, 1044253 );
			SetNeedHeat( index, true );
			SetUseAllRes( index, true );

			index = AddCraft( typeof( Ribs ), 1044498, 1022546, 0.0, 100.0, typeof( RawRibs ), 1044485, 1, 1044253 );
			SetNeedHeat( index, true );
			SetUseAllRes( index, true );
			/* End Barbecue */
		}
	}
}