using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Engines.Craft
{
	public class DefTrapCrafting : CraftSystem
	{
		public override SkillName MainSkill
		{
			get{ return SkillName.Tinkering; }
		}

		public override string GumpTitleString
		{
			get { return "<basefont color=#FFFFFF><CENTER>TRAPCRAFTING MENU</CENTER></basefont>"; } 
		}

		private static CraftSystem m_CraftSystem;

		public static CraftSystem CraftSystem
		{
			get
			{
				if ( m_CraftSystem == null )
					m_CraftSystem = new DefTrapCrafting();

				return m_CraftSystem;
			}
		}

		public override double GetChanceAtMin( CraftItem item )
		{
			return 0.0; // 0%
		}

		private DefTrapCrafting() : base( 1, 1, 1.25 )// base( 1, 2, 1.7 )
		{
		}

		public override int CanCraft( Mobile from, BaseTool tool, Type itemType )
		{
			if ( tool.Deleted || tool.UsesRemaining < 0 )
				return 1044038; // You have worn out your tool!
			else if ( !BaseTool.CheckAccessible( tool, from ) )
				return 1044263; // The tool must be on your person to use.

			return 0;
		}

		public override void PlayCraftEffect( Mobile from )
		{
            from.PlaySound( 0x241 ); 
		}

		private class InternalTimer : Timer
		{
			private Mobile m_From;

			public InternalTimer( Mobile from ) : base( TimeSpan.FromSeconds( 0.7 ) )
			{
				m_From = from;
			}

			protected override void OnTick()
			{
				m_From.PlaySound( 0x1C6 );
			}
		}

		public override int PlayEndingEffect( Mobile from, bool failed, bool lostMaterial, bool toolBroken, int quality, bool makersMark, CraftItem item )
		{
			if ( toolBroken )
				from.SendLocalizedMessage( 1044038 );

			if ( failed )
			{
				if ( lostMaterial )
					return 1044043;
				else
					return 1044157;
			}
			else
			{
				from.PlaySound( 0x1c6 );

				if ( quality == 0 )
					return 502785;
				else if ( makersMark && quality == 2 )
					return 1044156;
				else if ( quality == 2 )
					return 1044155;
				else
					return 1044154;
			}
		}

		public override void InitCraftList()
		{
			int index = -1;

            #region Components
            index = AddCraft(typeof( TrapFrame ), "Komponenty", "Podstawa", 20.0, 40.0, typeof( IronIngot ), "Sztaby", 2, "Potrzebujesz wi??cej sztab.");
            AddRes(index, typeof(Leather), "Sk??ra", 2, "Potrzebujesz wi??cej sk??r.");
            AddRes(index, typeof(Board), "Deska", 1, "Potrzeba wi??cej desek.");

            index = AddCraft(typeof( TrapSpike ), "Komponenty", "Kolec do pu??apki", 25.0, 45.0, typeof(Bolt), "Be??t", 1, "Potrzeba wi??cej be??t??w.");
            AddRes(index, typeof( Springs ), "spr????yny", 1, "Potrzeba wi??cej spr????yn");

            index = AddCraft(typeof( TrapCrystalTrigger ), "Komponenty", "Kryszta?? do pu??apki", 60.0, 80.0, typeof( PowerCrystal ), "kryszta?? mocy", 1, "Potrzeba wi??cej kryszta????w mocy.");
            AddRes(index, typeof( DullCopperIngot ), "Sztaby Matowej miedzi", 2, "Potrzebujesz wi??cej sztab matowej miedzi.");
            AddRes(index, typeof( Springs ), "spr????yny", 1, "Potrzeba wi??cej spr????yn");

            index = AddCraft(typeof( TrapCrystalSensor ), "Komponenty", "Kryszta??owy wykrywacz", 90.0, 110.0, typeof( Salt ), "s??l", 1, "Potrzebujesz wi??cej soli.");
            AddRes(index, typeof( TrapCrystalTrigger ), "Kryszta?? do pu??apki", 1, "Potrzeba wi??cej kryszta????w do pu??apki.");
            #endregion

            #region Explosive Traps
            index = AddCraft(typeof(ExplosiveLesserTrap), "Wybuchowe pu??apki", "Pomniejsza wybuchowa pu??apka", 35.0, 55.0, typeof(TrapFrame), "Podstawa", 1, "Potrzeba wi??cej podstaw do pu??apki.");
            AddRes(index, typeof(LesserExplosionPotion), "S??aba mikstura eksplozji", 1, "Potrzeba wi??cej mikstur eksplozji.");
            AddRes(index, typeof(TrapSpike), "Kolec do pu??apki", 1, "Potrzeba wi??cej kolc??w do pu??apki.");
            AddRes(index, typeof(Gears), "Przek??adnie", 2, "Potrzeba wi??cej przek??adni.");

            index = AddCraft(typeof(ExplosiveRegularTrap), "Wybuchowe pu??apki", "Wybuchowa pu??apka", 50.0, 70.0, typeof(TrapFrame), "Podstawa", 1, "Potrzeba wi??cej podstaw do pu??apki.");
            AddRes(index, typeof(ExplosionPotion), "Mikstura eksplozji", 1, "Potrzeba wi??cej mikstur eksplozji.");
           AddRes(index, typeof(TrapSpike), "Kolec do pu??apki", 1, "Potrzeba wi??cej kolc??w do pu??apki.");
            AddRes(index, typeof(Gears), "Przek??adnie", 2, "Potrzeba wi??cej przek??adni.");

            index = AddCraft(typeof(ExplosiveGreaterTrap), "Wybuchowe pu??apki", "Powi??ksza wybuchowa pu??apka", 65.0, 85.0, typeof(TrapFrame), "Podstawa", 1, "Potrzeba wi??cej podstaw do pu??apki.");
            AddRes(index, typeof(GreaterExplosionPotion), "Mikstura silnej eksplozji", 1, "Potrzeba wi??cej mikstur eksplozji.");
            AddRes(index, typeof(TrapSpike), "Kolec do pu??apki", 1, "Potrzeba wi??cej kolc??w do pu??apki.");
            AddRes(index, typeof(Gears), "Przek??adnie", 2, "Potrzeba wi??cej przek??adni.");
            #endregion

            #region Freezing Traps
            index = AddCraft(typeof(FreezingLesserTrap), "Zamra??aj??ce pu??apki", "Pomniejsza zamra??aj??ca pu??apka", 50.0, 70.0, typeof(TrapFrame), "Podstawa", 1, "Potrzeba wi??cej podstaw do pu??apki.");
            AddRes(index, typeof(DryIce), "Suchy l??d", 1, "Potrzeba wi??cej suchego lodu.");
            AddRes(index, typeof( TrapCrystalTrigger ), "Kryszta?? do pu??apki", 1, "Potrzeba wi??cej kryszta????w do pu??apki.");
           AddRes(index, typeof(Gears), "Przek??adnie", 2, "Potrzeba wi??cej przek??adni.");

            index = AddCraft(typeof(FreezingRegularTrap), "Zamra??aj??ce pu??apki", "zamra??aj??ca pu??apka", 65.0, 85.0, typeof(TrapFrame), "Podstawa", 1, "Potrzeba wi??cej podstaw do pu??apki.");
            AddRes(index, typeof(DryIce), "Suchy l??d", 2, "Potrzeba wi??cej suchego lodu.");
            AddRes(index, typeof( TrapCrystalTrigger ), "Kryszta?? do pu??apki", 1, "Potrzeba wi??cej kryszta????w do pu??apki.");
            AddRes(index, typeof(Gears), "Przek??adnie", 2, "Potrzeba wi??cej przek??adni.");

            index = AddCraft(typeof(FreezingGreaterTrap), "Zamra??aj??ce pu??apki", "Powi??ksza zamra??aj??ca pu??apka", 90.0, 110.0, typeof(TrapFrame), "Podstawa", 1, "Potrzeba wi??cej podstaw do pu??apki.");
            AddRes(index, typeof(DryIce), "Suchy l??d", 3, "Potrzeba wi??cej suchego lodu.");
            AddRes(index, typeof( TrapCrystalTrigger ), "Kryszta?? do pu??apki", 1, "Potrzeba wi??cej kryszta????w do pu??apki.");
            AddRes(index, typeof(Gears), "Przek??adnie", 2, "Potrzeba wi??cej przek??adni.");
            #endregion

            #region Lightning Traps
            index = AddCraft(typeof(LightningLesserTrap), "Pora??aj??ce pu??apki", "Pomniejsza pora??aj??ca pu??apka", 45.0, 65.0, typeof(TrapFrame), "Podstawa", 1, "Potrzeba wi??cej podstaw do pu??apki.");
            AddRes(index, typeof(LightningScroll), "Zw??j Czaru Piorun??w", 10, "Potrzeba wi??cej zwoj??w.");
            AddRes(index, typeof( TrapCrystalTrigger ), "Kryszta?? do pu??apki", 1, "Potrzeba wi??cej kryszta????w do pu??apki.");
            AddRes(index, typeof(Gears), "Przek??adnie", 2, "Potrzeba wi??cej przek??adni.");

            index = AddCraft(typeof(LightningRegularTrap), "Pora??aj??ce pu??apki", "pora??aj??ca pu??apka", 60.0, 80.0, typeof(TrapFrame), "Podstawa", 1, "Potrzeba wi??cej podstaw do pu??apki.");
             AddRes(index, typeof(LightningScroll), "Zw??j Czaru Piorun??w", 15, "Potrzeba wi??cej zwoj??w.");
            AddRes(index, typeof( TrapCrystalTrigger ), "Kryszta?? do pu??apki", 1, "Potrzeba wi??cej kryszta????w do pu??apki.");
            AddRes(index, typeof(Gears), "Przek??adnie", 2, "Potrzeba wi??cej przek??adni.");

            index = AddCraft(typeof(LightningGreaterTrap), "Pora??aj??ce pu??apki", "Powi??ksza pora??aj??ca pu??apka", 75.0, 95.0, typeof(TrapFrame), "Podstawa", 1, "Potrzeba wi??cej podstaw do pu??apki.");
             AddRes(index, typeof(LightningScroll), "Zw??j Czaru Piorun??w", 20, "Potrzeba wi??cej zwoj??w.");
            AddRes(index, typeof( TrapCrystalTrigger ), "Kryszta?? do pu??apki", 1, "Potrzeba wi??cej kryszta????w do pu??apki.");
            AddRes(index, typeof(Gears), "Przek??adnie", 2, "Potrzeba wi??cej przek??adni.");

            #endregion

            #region Paralysis Traps
            index = AddCraft(typeof(ParalysisLesserTrap), "Parali??uj??ce pu??apki", "Pomniejsza parali??uj??ca pu??apka", 40.0, 60.0, typeof(TrapFrame), "Podstawa", 1, "Potrzeba wi??cej podstaw do pu??apki.");
           AddRes(index, typeof(DragonsBlood), "Krew Smoka", 1, "Potrzeba wi??cej krwii smoka.");
            AddRes(index, typeof( TrapCrystalTrigger ), "Kryszta?? do pu??apki", 1, "Potrzeba wi??cej kryszta????w do pu??apki.");
            AddRes(index, typeof(Gears), "Przek??adnie", 2, "Potrzeba wi??cej przek??adni.");

            index = AddCraft(typeof(ParalysisRegularTrap), "Parali??uj??ce pu??apki", "Parali??uj??ca pu??apk", 55.0, 75.0, typeof(TrapFrame), "Podstawa", 1, "Potrzeba wi??cej podstaw do pu??apki.");
            AddRes(index, typeof(DragonsBlood), "Krew Smoka", 2, "Potrzeba wi??cej krwii smoka.");
            AddRes(index, typeof( TrapCrystalTrigger ), "Kryszta?? do pu??apki", 1, "Potrzeba wi??cej kryszta????w do pu??apki.");
            AddRes(index, typeof(Gears), "Przek??adnie", 2, "Potrzeba wi??cej przek??adni.");

            index = AddCraft(typeof(ParalysisGreaterTrap), "Parali??uj??ce pu??apki", "Pomniejsza parali??uj??ca pu??apk", 70.0, 90.0, typeof(TrapFrame), "Podstawa", 1, "Potrzeba wi??cej podstaw do pu??apki.");
            AddRes(index, typeof(DragonsBlood), "Krew Smoka", 3, "Potrzeba wi??cej krwii smoka.");
            AddRes(index, typeof( TrapCrystalTrigger ), "Kryszta?? do pu??apki", 1, "Potrzeba wi??cej kryszta????w do pu??apki.");
            AddRes(index, typeof(Gears), "Przek??adnie", 2, "Potrzeba wi??cej przek??adni.");
            #endregion

            #region Other Traps
            index = AddCraft(typeof(BladeSpiritTrap), "Inne urz??dzenia", "Pu??apka ducha ostrzy", 35.0, 55.0, typeof(TrapFrame), "Podstawa", 1, "Potrzeba wi??cej podstaw do pu??apki.");
            AddRes(index, typeof(TrappedGhost), "uwi??ziony duch", 1, "Potrzeba wi??cej uwi??zionych duch??w.");
            AddRes(index, typeof(Hammer), "m??otek", 1, "Potrzeba wi??cej m??otk??w.");
            AddRes(index, typeof(CrescentBlade), "p????ksi????ycowe ostrza", 4, "Potrzeba wi??cej po??ksi????ycowych ostrzy.");

            index = AddCraft(typeof(GhostTrap), "Inne urz??dzenia", "Pu??apka duchowa", 75.0, 95.0, typeof(TrapFrame), "Podstawa", 1, "Potrzeba wi??cej podstaw do pu??apki.");
            AddRes(index, typeof(Bottle), "pusta butelka", 1, "Potrzebujesz wi??cej pustych butelek.");
            AddRes(index, typeof( TrapCrystalTrigger ), "Kryszta?? do pu??apki", 1, "Potrzeba wi??cej kryszta????w do pu??apki.");
            AddRes(index, typeof(Garlic), "czosnek", 40, "Potrzeba wi??cej czosnku.");

            index = AddCraft(typeof(TrapDetector), "Inne urz??dzenia", "wykrywacz pu??apek", 55.0, 75.0, typeof(TrapFrame), "Podstawa", 1, "Potrzeba wi??cej podstaw do pu??apki.");
            AddRes(index, typeof(Springs), "spr????yny", 4, "Potrzeba wi??cej spr????yn");
            AddRes(index, typeof(Hammer), "Hm??otek", 4, "Potrzeba wi??cej m??otk??w");
            AddRes(index, typeof(Buckler), "puklerz", 1, "Potrzeba wi??cej puklerzy.");

            index = AddCraft(typeof(TrapTest), "Inne urz??dzenia", "pu??apka testowa", 25.0, 45.0, typeof(TrapFrame), "Podstawa", 1, "Potrzeba wi??cej podstaw do pu??apki.");
            AddRes(index, typeof(Leather), "sk??ra", 2, "Potrzeba wi??cej sk??r.");
            AddRes(index, typeof(Gears), "Przek??adnie", 2, "Potrzeba wi??cej przek??adni.");
            AddRes(index, typeof( Springs ), "spr????yny", 1, "Potrzeba wi??cej spr????yn");
            #endregion

            #region Poison Dart Traps
            index = AddCraft(typeof(PoisonLesserDartTrap), "Pu??apki z truj??cymi strza??kami", "Pu??apka z lekk?? trucizn??", 25.0, 45.0, typeof(TrapFrame), "Podstawa", 1, "Potrzeba wi??cej podstaw do pu??apki.");
            AddRes(index, typeof(LesserPoisonPotion), "S??aba trucizna", 1, "Potrzebujesz wi??cej trucizny.");
            AddRes(index, typeof(TrapSpike), "Kolec do pu??apki", 1, "Potrzeba wi??cej kolc??w do pu??apki.");
            AddRes(index, typeof(Gears), "Przek??adnie", 2, "Potrzeba wi??cej przek??adni.");
            
            index = AddCraft(typeof(PoisonRegularDartTrap), "Pu??apki z truj??cymi strza??kami", "Pu??apka z trucizn??", 40.0, 60.0, typeof(TrapFrame), "Podstawa", 1, "Potrzeba wi??cej podstaw do pu??apki.");
            AddRes(index, typeof(PoisonPotion), "trucizna", 1, "Potrzebujesz wi??cej trucizny.");
             AddRes(index, typeof(TrapSpike), "Kolec do pu??apki", 1, "Potrzeba wi??cej kolc??w do pu??apki.");
            AddRes(index, typeof(Gears), "Przek??adnie", 2, "Potrzeba wi??cej przek??adni.");

            index = AddCraft(typeof(PoisonGreaterDartTrap), "Pu??apki z truj??cymi strza??kami", "Pu??apka z mocn?? trucizn??", 55.0, 75.0, typeof(TrapFrame), "Podstawa", 1, "Potrzeba wi??cej podstaw do pu??apki.");
            AddRes(index, typeof(GreaterPoisonPotion), "mocna trucizna", 1, "Potrzebujesz wi??cej trucizny.");
             AddRes(index, typeof(TrapSpike), "Kolec do pu??apki", 1, "Potrzeba wi??cej kolc??w do pu??apki.");
            AddRes(index, typeof(Gears), "Przek??adnie", 2, "Potrzeba wi??cej przek??adni.");

            index = AddCraft(typeof(PoisonDeadlyDartTrap), "Pu??apki z truj??cymi strza??kami", "Pu??apka ze ??mierteln?? trucizn??", 70.0, 90.0, typeof(TrapFrame), "Podstawa", 1, "Potrzeba wi??cej podstaw do pu??apki.");
            AddRes(index, typeof(DeadlyPoisonPotion), "??miertelna trucizna", 1, "Potrzebujesz wi??cej trucizny.");
             AddRes(index, typeof(TrapSpike), "Kolec do pu??apki", 1, "Potrzeba wi??cej kolc??w do pu??apki.");
            AddRes(index, typeof(Gears), "Przek??adnie", 2, "Potrzeba wi??cej przek??adni.");
            #endregion
        }
	}
}