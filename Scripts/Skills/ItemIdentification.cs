using System;
using Server;
using Server.Targeting;
using Server.Mobiles;
using System.Collections.Generic;

namespace Server.Items
{
	public enum IdLevel
	{
		None,
		Level1,
		Level2,
		Level3,
		Level4,
		Level5,
        Level6
	}
	
	public interface IIdentifiable
	{
		bool Identified { set; get; }
		IdLevel IdentifyLevel { set; get; }
		//bool ItemIdProperties();		
	}
	
	public class ItemIdentification
	{
        // NOTE: look into BaseRunicTool.cs to find the caps of any magic property

        private static string lowIntensity    = "BBFFAA";
        private static string mediumIntensity = "FFAA44";
        private static string highIntensity   = "FF3333";

        private static int medium_of_8 = 4;
        private static int high_of_8   = 7;

        private static int medium_of_15 = 7;
        private static int high_of_15   = 11;

        private static int medium_of_50 = 25;
        private static int high_of_50   = 35;

		public static void Initialize()
		{
			SkillInfo.Table[(int)SkillName.ItemID].Callback = new SkillUseCallback( OnUse );
		}

		public static TimeSpan OnUse( Mobile from )
		{
			from.SendLocalizedMessage( 500343 ); // Co chcesz zidentyfikowac?
			from.Target = new InternalTarget();

			if ( from.AccessLevel == AccessLevel.Player )
				return TimeSpan.FromSeconds( 3.0 );

			return TimeSpan.Zero;
		}

        // 15.10.2012 :: zombie
        public static void AddNameProperty( int unidentNum, int identNum, int val, ref ObjectPropertyList list, bool identified )
        {
            if ( val == 0 )
                return;

            if ( identified )
                list.Add( identNum, val.ToString() );
            else
            {
                list.Add(unidentNum);
            }
        }

        public static void AddNameProperty(AosWeaponAttribute att, int identNum, int val, ref ObjectPropertyList list, bool identified)
        {
            if (val == 0)
                return;

            if (identified)
                list.Add(identNum, val.ToString());
            else
            {
                switch (att)
                {
                    case AosWeaponAttribute.LowerStatReq: // 10-100
                        if (val < 50)
                            list.Add(1071165, lowIntensity); // <BASEFONT COLOR=#~1_val~>Zmniejszone wymagania
                        else if (val < 80)
                            list.Add(1071165, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Zmniejszone wymagania
                        else
                            list.Add(1071165, highIntensity); // <BASEFONT COLOR=#~1_val~>Zmniejszone wymagania
                        break;
                    case AosWeaponAttribute.SelfRepair: // 1-3
                        if (val < 2)
                            list.Add(1071180, lowIntensity); // <BASEFONT COLOR=#~1_val~>Samonaprawianie
                        else if (val < 3)
                            list.Add(1071180, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Samonaprawianie
                        else
                            list.Add(1071180, highIntensity); // <BASEFONT COLOR=#~1_val~>Samonaprawianie
                        break;
                    case AosWeaponAttribute.HitLeechHits: // 2-50
                        if (val < medium_of_50)
                            list.Add(1071152, lowIntensity); // <BASEFONT COLOR=#~1_val~>Wysysa zycie
                        else if (val < high_of_50)
                            list.Add(1071152, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Wysysa zycie
                        else
                            list.Add(1071152, highIntensity); // <BASEFONT COLOR=#~1_val~>Wysysa zycie
                        break;
                    case AosWeaponAttribute.HitLeechStam: // 2-50
                        if (val < medium_of_50)
                            list.Add(1071160, lowIntensity); // <BASEFONT COLOR=#~1_val~>Wyssanie staminy
                        else if (val < high_of_50)
                            list.Add(1071160, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Wyssanie staminy
                        else
                            list.Add(1071160, highIntensity); // <BASEFONT COLOR=#~1_val~>Wyssanie staminy
                        break;
                    case AosWeaponAttribute.HitLeechMana: // 2-50
                        if (val < medium_of_50)
                            list.Add(1071157, lowIntensity); // <BASEFONT COLOR=#~1_val~>Wyssanie many
                        else if (val < high_of_50)
                            list.Add(1071157, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Wyssanie many
                        else
                            list.Add(1071157, highIntensity); // <BASEFONT COLOR=#~1_val~>Wyssanie many
                        break;
                    case AosWeaponAttribute.HitLowerAttack: // 2-50
                        if (val < medium_of_50)
                            list.Add(1071154, lowIntensity); // <BASEFONT COLOR=#~1_val~>Obniza zdolnosc ataku przeciwnika
                        else if (val < high_of_50)
                            list.Add(1071154, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Obniza zdolnosc ataku przeciwnika
                        else
                            list.Add(1071154, highIntensity); // <BASEFONT COLOR=#~1_val~>Obniza zdolnosc ataku przeciwnika
                        break;
                    case AosWeaponAttribute.HitLowerDefend: // 2-50
                        if (val < medium_of_50)
                            list.Add(1071155, lowIntensity); // <BASEFONT COLOR=#~1_val~>Obniza zdolnosc obrony przeciwnika
                        else if (val < high_of_50)
                            list.Add(1071155, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Obniza zdolnosc obrony przeciwnika
                        else
                            list.Add(1071155, highIntensity); // <BASEFONT COLOR=#~1_val~>Obniza zdolnosc obrony przeciwnika
                        break;
                    case AosWeaponAttribute.HitMagicArrow: // 2-50
                        if (val < medium_of_50)
                            list.Add(1071156, lowIntensity); // <BASEFONT COLOR=#~1_val~>Rzucenie czaru magiczna strzala
                        else if (val < high_of_50)
                            list.Add(1071156, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Rzucenie czaru magiczna strzala
                        else
                            list.Add(1071156, highIntensity); // <BASEFONT COLOR=#~1_val~>Rzucenie czaru magiczna strzala
                        break;
                    case AosWeaponAttribute.HitHarm: // 2-50
                        if (val < medium_of_50)
                            list.Add(1071151, lowIntensity); // <BASEFONT COLOR=#~1_val~>Rzucenie czaru krzywdy
                        else if (val < high_of_50)
                            list.Add(1071151, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Rzucenie czaru krzywdy
                        else
                            list.Add(1071151, highIntensity); // <BASEFONT COLOR=#~1_val~>Rzucenie czaru krzywdy
                        break;
                    case AosWeaponAttribute.HitFireball: // 2-50
                        if (val < medium_of_50)
                            list.Add(1071150, lowIntensity); // <BASEFONT COLOR=#~1_val~>Rzucenie czaru magiczna kula
                        else if (val < high_of_50)
                            list.Add(1071150, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Rzucenie czaru magiczna kula
                        else
                            list.Add(1071150, highIntensity); // <BASEFONT COLOR=#~1_val~>Rzucenie czaru magiczna kula
                        break;
                    case AosWeaponAttribute.HitLightning: // 2-50
                        if (val < medium_of_50)
                            list.Add(1071153, lowIntensity); // <BASEFONT COLOR=#~1_val~>Rzucenie czaru blyskawicy
                        else if (val < high_of_50)
                            list.Add(1071153, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Rzucenie czaru blyskawicy
                        else
                            list.Add(1071153, highIntensity); // <BASEFONT COLOR=#~1_val~>Rzucenie czaru blyskawicy
                        break;
                    case AosWeaponAttribute.HitDispel: // 2-50
                        if (val < medium_of_50)
                            list.Add(1071147, lowIntensity); // <BASEFONT COLOR=#~1_val~>Rzuca czar odwolania
                        else if (val < high_of_50)
                            list.Add(1071147, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Rzuca czar odwolania
                        else
                            list.Add(1071147, highIntensity); // <BASEFONT COLOR=#~1_val~>Rzuca czar odwolania
                        break;
                    case AosWeaponAttribute.HitColdArea: // 2-50
                        if (val < medium_of_50)
                            list.Add(1071146, lowIntensity); // <BASEFONT COLOR=#~1_val~>Obszarowe obrazenia od zimna
                        else if (val < high_of_50)
                            list.Add(1071146, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Obszarowe obrazenia od zimna
                        else
                            list.Add(1071146, highIntensity); // <BASEFONT COLOR=#~1_val~>Obszarowe obrazenia od zimna
                        break;
                    case AosWeaponAttribute.HitFireArea: // 2-50
                        if (val < medium_of_50)
                            list.Add(1071149, lowIntensity); // <BASEFONT COLOR=#~1_val~>Obszarowe obrazenia od ognia
                        else if (val < high_of_50)
                            list.Add(1071149, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Obszarowe obrazenia od ognia
                        else
                            list.Add(1071149, highIntensity); // <BASEFONT COLOR=#~1_val~>Obszarowe obrazenia od ognia
                        break;
                    case AosWeaponAttribute.HitPoisonArea: // 2-50
                        if (val < medium_of_50)
                            list.Add(1071159, lowIntensity); // <BASEFONT COLOR=#~1_val~>Obszarowe obrazenia od trucizny
                        else if (val < high_of_50)
                            list.Add(1071159, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Obszarowe obrazenia od trucizny
                        else
                            list.Add(1071159, highIntensity); // <BASEFONT COLOR=#~1_val~>Obszarowe obrazenia od trucizny
                        break;
                    case AosWeaponAttribute.HitEnergyArea: // 2-50
                        if (val < medium_of_50)
                            list.Add(1071148, lowIntensity); // <BASEFONT COLOR=#~1_val~>Obszarowe obrazenia od energii
                        else if (val < high_of_50)
                            list.Add(1071148, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Obszarowe obrazenia od energii
                        else
                            list.Add(1071148, highIntensity); // <BASEFONT COLOR=#~1_val~>Obszarowe obrazenia od energii
                        break;
                    case AosWeaponAttribute.HitPhysicalArea: // 2-50
                        if (val < medium_of_50)
                            list.Add(1071158, lowIntensity); // <BASEFONT COLOR=#~1_val~>Obszarowe obrazenia fizyczne
                        else if (val < high_of_50)
                            list.Add(1071158, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Obszarowe obrazenia fizyczne
                        else
                            list.Add(1071158, highIntensity); // <BASEFONT COLOR=#~1_val~>Obszarowe obrazenia fizyczne
                        break;
                    case AosWeaponAttribute.ResistPhysicalBonus:
                        if (val < 5)
                            list.Add(1071197, lowIntensity); // <BASEFONT COLOR=#~1_val~>Odpornosc fizyczna
                        else if (val < 10)
                            list.Add(1071197, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Odpornosc fizyczna
                        else
                            list.Add(1071197, highIntensity); // <BASEFONT COLOR=#~1_val~>Odpornosc fizyczna
                        break;
                    case AosWeaponAttribute.ResistFireBonus:
                        if (val < 5)
                            list.Add(1071198, lowIntensity); // <BASEFONT COLOR=#~1_val~>Odpornosc na ogien
                        else if (val < 10)
                            list.Add(1071198, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Odpornosc na ogien
                        else
                            list.Add(1071198, highIntensity); // <BASEFONT COLOR=#~1_val~>Odpornosc na ogien
                        break;
                    case AosWeaponAttribute.ResistColdBonus:
                        if (val < 5)
                            list.Add(1071199, lowIntensity); // <BASEFONT COLOR=#~1_val~>Odpornosc na zimno
                        else if (val < 10)
                            list.Add(1071199, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Odpornosc na zimno
                        else
                            list.Add(1071199, highIntensity); // <BASEFONT COLOR=#~1_val~>Odpornosc na zimno
                        break;
                    case AosWeaponAttribute.ResistPoisonBonus:
                        if (val < 5)
                            list.Add(1071200, lowIntensity); // <BASEFONT COLOR=#~1_val~>Odpornosc na trucizne
                        else if (val < 10)
                            list.Add(1071200, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Odpornosc na trucizne
                        else
                            list.Add(1071200, highIntensity); // <BASEFONT COLOR=#~1_val~>Odpornosc na trucizne
                        break;
                    case AosWeaponAttribute.ResistEnergyBonus:
                        if (val < 5)
                            list.Add(1071201, lowIntensity); // <BASEFONT COLOR=#~1_val~>Odpornosc na energie
                        else if (val < 10)
                            list.Add(1071201, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Odpornosc na energie
                        else
                            list.Add(1071201, highIntensity); // <BASEFONT COLOR=#~1_val~>Odpornosc na energie
                        break;
                    case AosWeaponAttribute.UseBestSkill:
                        list.Add(1071202, highIntensity); // <BASEFONT COLOR=#~1_val~>Uzywa najlepiej znanego stylu walki
                        break;
                    case AosWeaponAttribute.MageWeapon: // 1-10
                        if (val < 5)
                            list.Add(1071203, lowIntensity); // <BASEFONT COLOR=#~1_val~>Bron maga
                        else if (val < 8)
                            list.Add(1071203, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Bron maga
                        else
                            list.Add(1071203, highIntensity); // <BASEFONT COLOR=#~1_val~>Bron maga
                        break;
                    case AosWeaponAttribute.DurabilityBonus: // 10-100
                        if (val < 30)
                            list.Add(1071140, lowIntensity); // <BASEFONT COLOR=#~1_val~>Zwiekszona wytrzymalosc
                        else if (val < 70)
                            list.Add(1071140, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Zwiekszona wytrzymalosc
                        else
                            list.Add(1071140, highIntensity); // <BASEFONT COLOR=#~1_val~>Zwiekszona wytrzymalosc
                        break;
                    default:
                        break;
                }
            }
        }

        public static void AddNameProperty(AosArmorAttribute att, int identNum, int val, ref ObjectPropertyList list, bool identified)
        {
            if (val == 0)
                return;

            if (identified)
                list.Add(identNum, val.ToString());
            else
            {
                switch (att)
                {
                    case AosArmorAttribute.LowerStatReq: // 10-100
                        if (val < 50)
                            list.Add(1071165, lowIntensity); // <BASEFONT COLOR=#~1_val~>Zmniejszone wymagania
                        else if (val < 80)
                            list.Add(1071165, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Zmniejszone wymagania
                        else
                            list.Add(1071165, highIntensity); // <BASEFONT COLOR=#~1_val~>Zmniejszone wymagania
                        break;
                    case AosArmorAttribute.SelfRepair: // 1-3
                        if (val < 2)
                            list.Add(1071180, lowIntensity); // <BASEFONT COLOR=#~1_val~>Samonaprawianie
                        else if (val < 3)
                            list.Add(1071180, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Samonaprawianie
                        else
                            list.Add(1071180, highIntensity); // <BASEFONT COLOR=#~1_val~>Samonaprawianie
                        break;
                    case AosArmorAttribute.MageArmor:
                        list.Add(1071167, highIntensity); // <BASEFONT COLOR=#~1_val~>Zbroja maga
                        break;
                    case AosArmorAttribute.DurabilityBonus: // 10-100
                        if (val < 30)
                            list.Add(1071140, lowIntensity); // <BASEFONT COLOR=#~1_val~>Zwiekszona wytrzymalosc
                        else if (val < 70)
                            list.Add(1071140, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Zwiekszona wytrzymalosc
                        else
                            list.Add(1071140, highIntensity); // <BASEFONT COLOR=#~1_val~>Zwiekszona wytrzymalosc
                        break;
                    default:
                        break;
                }
            }
        }

        public static void AddNameSlayerProperty(SlayerName slayerName, ref ObjectPropertyList list, bool identified)
        {
            if (identified)
            {
                SlayerEntry entry = SlayerGroup.GetEntryByName(slayerName);

                if (entry != null)
                    list.Add(entry.Title);
            }
            else
            {
                if (slayerName == SlayerName.Silver ||           // nieumarlych
                    slayerName == SlayerName.Repond ||           // humanoidow
                    slayerName == SlayerName.ReptilianDeath ||   // gadow
                    slayerName == SlayerName.Exorcism ||         // demonow
                    slayerName == SlayerName.ArachnidDoom ||     // pajeczakow
                    slayerName == SlayerName.Fey ||              // fey
                    slayerName == SlayerName.ElementalBan        // zywiolakow
                   )
                    list.Add(1071128, highIntensity); // Super pogromca stworzen
                else
                    list.Add(1071129, lowIntensity); // Pogromca stworzen
            }
        }

        public static void AddNameSkillProperty(int i, SkillName skill, double val, ref ObjectPropertyList list, bool identified)
        {
            if (identified)
                list.Add(1060451 + i, "#{0}\t{1}", 1044060 + (int)skill, val);
            else
            {
                if (val < 6)
                    list.Add(1061501 + (int)skill, lowIntensity); // <BASEFONT COLOR=#~1_rgb~>zwieksza umiejetnosc (NazwaSkilla)<BASEFONT COLOR=#FFFFFF>
                else if (val < 11)
                    list.Add(1061501 + (int)skill, mediumIntensity); // <BASEFONT COLOR=#~1_rgb~>zwieksza umiejetnosc (NazwaSkilla)<BASEFONT COLOR=#FFFFFF>
                else
                    list.Add(1061501 + (int)skill, highIntensity); // <BASEFONT COLOR=#~1_rgb~>zwieksza umiejetnosc (NazwaSkilla)<BASEFONT COLOR=#FFFFFF>

                //list.Add(1071181 + order, "#{0}", 1044060 + (int)skill);
            }
        }

        public static void AddNameProperty(AosAttribute att, int identNum, int val, ref ObjectPropertyList list, bool identified)
        {
            if (val == 0)
                return;

            if (identified)
                list.Add(identNum, val.ToString());
            else
            {
                switch (att)
                {
                    case AosAttribute.RegenHits: // 1-2
                        if (val < 1)
                            list.Add(1071174, lowIntensity); // <BASEFONT COLOR=#~1_val~>Odzyskiwanie zywotnosci
                        else if (val < 2)
                            list.Add(1071174, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Odzyskiwanie zywotnosci
                        else
                            list.Add(1071174, highIntensity); // <BASEFONT COLOR=#~1_val~>Odzyskiwanie zywotnosci
                        break;
                    case AosAttribute.RegenStam: // 1-3
                        if (val < 2)
                            list.Add(1071173, lowIntensity); // <BASEFONT COLOR=#~1_val~>Odzyskiwanie staminy
                        else if (val < 3)
                            list.Add(1071173, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Odzyskiwanie staminy
                        else
                            list.Add(1071173, highIntensity); // <BASEFONT COLOR=#~1_val~>Odzyskiwanie staminy
                        break;
                    case AosAttribute.RegenMana: // 1-2
                        if (val < 1)
                            list.Add(1071170, lowIntensity); // <BASEFONT COLOR=#~1_val~>Odzyskiwanie many
                        else if (val < 2)
                            list.Add(1071170, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Odzyskiwanie many
                        else
                            list.Add(1071170, highIntensity); // <BASEFONT COLOR=#~1_val~>Odzyskiwanie many
                        break;
                    case AosAttribute.DefendChance: // 1-15
                        if (val < medium_of_15)
                            list.Add(1071138, lowIntensity); // <BASEFONT COLOR=#~1_val~>Zwieksza szanse na unikniecie ciosu
                        else if (val < high_of_15)
                            list.Add(1071138, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Zwieksza szanse na unikniecie ciosu
                        else
                            list.Add(1071138, highIntensity); // <BASEFONT COLOR=#~1_val~>Zwieksza szanse na unikniecie ciosu
                        break;
                    case AosAttribute.AttackChance: // 1-15
                        if (val < medium_of_15)
                            list.Add(1071145, lowIntensity); // <BASEFONT COLOR=#~1_val~>Zwieksza szanse na trafienie
                        else if (val < high_of_15)
                            list.Add(1071145, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Zwieksza szanse na trafienie
                        else
                            list.Add(1071145, highIntensity); // <BASEFONT COLOR=#~1_val~>Zwieksza szanse na trafienie
                        break;
                    case AosAttribute.BonusStr: // 1-8
                        if (val < medium_of_8)
                            list.Add(1071188, lowIntensity); // <BASEFONT COLOR=#~1_val~>Zwieksza sile
                        else if (val < high_of_8)
                            list.Add(1071188, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Zwieksza sile
                        else
                            list.Add(1071188, highIntensity); // <BASEFONT COLOR=#~1_val~>Zwieksza sile
                        break;
                    case AosAttribute.BonusDex: // 1-8
                        if (val < medium_of_8)
                            list.Add(1071139, lowIntensity); // <BASEFONT COLOR=#~1_val~>Zwieksza zrecznosc
                        else if (val < high_of_8)
                            list.Add(1071139, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Zwieksza zrecznosc
                        else
                            list.Add(1071139, highIntensity); // <BASEFONT COLOR=#~1_val~>Zwieksza zrecznosc
                        break;
                    case AosAttribute.BonusInt: // 1-8
                        if (val < medium_of_8)
                            list.Add(1071162, lowIntensity); // <BASEFONT COLOR=#~1_val~>Zwieksza inteligencje
                        else if (val < high_of_8)
                            list.Add(1071162, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Zwieksza inteligencje
                        else
                            list.Add(1071162, highIntensity); // <BASEFONT COLOR=#~1_val~>Zwieksza inteligencje
                        break;
                    case AosAttribute.BonusHits: // 1-8
                        if (val < medium_of_8)
                            list.Add(1071161, lowIntensity); // <BASEFONT COLOR=#~1_val~>Zwieksza zywotnosc
                        else if (val < high_of_8)
                            list.Add(1071161, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Zwieksza zywotnosc
                        else
                            list.Add(1071161, highIntensity); // <BASEFONT COLOR=#~1_val~>Zwieksza zywotnosc
                        break;
                    case AosAttribute.BonusStam: // 1-8
                        if (val < medium_of_8)
                            list.Add(1071187, lowIntensity); // <BASEFONT COLOR=#~1_val~>Zwieksza stamine
                        else if (val < high_of_8)
                            list.Add(1071187, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Zwieksza stamine
                        else
                            list.Add(1071187, highIntensity); // <BASEFONT COLOR=#~1_val~>Zwieksza stamine
                        break;
                    case AosAttribute.BonusMana: // 1-8
                        if (val < medium_of_8)
                            list.Add(1071169, lowIntensity); // <BASEFONT COLOR=#~1_val~>Zwieksza mane
                        else if (val < high_of_8)
                            list.Add(1071169, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Zwieksza mane
                        else
                            list.Add(1071169, highIntensity); // <BASEFONT COLOR=#~1_val~>Zwieksza mane
                        break;
                    case AosAttribute.WeaponDamage: // 1-50 at weapon, 1-25 at jewelry
                        if (val < 20)
                            list.Add(1071131, lowIntensity); // <BASEFONT COLOR=#~1_val~>Zwiekszenie obrazen
                        else if (val < 40)
                            list.Add(1071131, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Zwiekszenie obrazen
                        else
                            list.Add(1071131, highIntensity); // <BASEFONT COLOR=#~1_val~>Zwiekszenie obrazen
                        break;
                    case AosAttribute.WeaponSpeed: // 5-10-15-20-25-30
                        if (val < 15)
                            list.Add(1071189, lowIntensity); // <BASEFONT COLOR=#~1_val~>Zwieksza szybkosc broni
                        else if (val < 25)
                            list.Add(1071189, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Zwieksza szybkosc broni
                        else
                            list.Add(1071189, highIntensity); // <BASEFONT COLOR=#~1_val~>Zwieksza szybkosc broni
                        break;
                    case AosAttribute.SpellDamage: // 1-12
                        if (val < 6)
                            list.Add(1071186, lowIntensity); // <BASEFONT COLOR=#~1_val~>Zwieksza obrazenia czarow
                        else if (val < 10)
                            list.Add(1071186, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Zwieksza obrazenia czarow
                        else
                            list.Add(1071186, highIntensity); // <BASEFONT COLOR=#~1_val~>Zwieksza obrazenia czarow
                        break;
                    case AosAttribute.CastRecovery: // 1-3
                        if (val < 2)
                            list.Add(1071142, lowIntensity); // <BASEFONT COLOR=#~1_val~>Szybsze ukonczenie zaklecia
                        else if (val < 3)
                            list.Add(1071142, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Szybsze ukonczenie zaklecia
                        else
                            list.Add(1071142, highIntensity); // <BASEFONT COLOR=#~1_val~>Szybsze ukonczenie zaklecia
                        break;
                    case AosAttribute.CastSpeed: // -1, 0, 1 or 2
                        if (val < 0)
                        {
                            if (val < -2)
                                list.Add(1071196, lowIntensity); // <BASEFONT COLOR=#~1_val~>Spowalnia rzucanie zaklec
                            else if (val < -1)
                                list.Add(1071196, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Spowalnia rzucanie zaklec
                            else
                                list.Add(1071196, highIntensity); // <BASEFONT COLOR=#~1_val~>Spowalnia rzucanie zaklec
                        }
                        else
                        {
                            if (val < 1)
                                list.Add(1071143, lowIntensity); // <BASEFONT COLOR=#~1_val~>Szybsze rzucanie zaklec
                            else if (val < 2)
                                list.Add(1071143, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Szybsze rzucanie zaklec
                            else
                                list.Add(1071143, highIntensity); // <BASEFONT COLOR=#~1_val~>Szybsze rzucanie zaklec
                        }                      
                        break;
                    case AosAttribute.LowerManaCost: // 1-8
                        if (val < medium_of_8)
                            list.Add(1071163, lowIntensity); // <BASEFONT COLOR=#~1_val~>Zmniejsza koszt many
                        else if (val < high_of_8)
                            list.Add(1071163, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Zmniejsza koszt many
                        else
                            list.Add(1071163, highIntensity); // <BASEFONT COLOR=#~1_val~>Zmniejsza koszt many
                        break;
                    case AosAttribute.LowerRegCost: // 1-15
                        if (val < medium_of_15)
                            list.Add(1071164, lowIntensity); // <BASEFONT COLOR=#~1_val~>Zmniejsza ilosc wymaganych ziol
                        else if (val < high_of_15)
                            list.Add(1071164, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Zmniejsza ilosc wymaganych ziol
                        else
                            list.Add(1071164, highIntensity); // <BASEFONT COLOR=#~1_val~>Zmniejsza ilosc wymaganych ziol
                        break;
                    case AosAttribute.ReflectPhysical: // 1-15
                        if (val < medium_of_15)
                            list.Add(1071172, lowIntensity); // <BASEFONT COLOR=#~1_val~>Odbijanie obrazen fizycznych
                        else if (val < high_of_15)
                            list.Add(1071172, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Odbijanie obrazen fizycznych
                        else
                            list.Add(1071172, highIntensity); // <BASEFONT COLOR=#~1_val~>Odbijanie obrazen fizycznych
                        break;
                    case AosAttribute.EnhancePotions: // 5-10-15-20-25
                        if (val < 15)
                            list.Add(1071141, lowIntensity); // <BASEFONT COLOR=#~1_val~>Wzmocnienie mikstur
                        else if (val < 25)
                            list.Add(1071141, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Wzmocnienie mikstur
                        else
                            list.Add(1071141, highIntensity); // <BASEFONT COLOR=#~1_val~>Wzmocnienie mikstur
                        break;
                    case AosAttribute.Luck: // 1-100
                        if (val < 50)
                            list.Add(1071166, lowIntensity); // <BASEFONT COLOR=#~1_val~>Szczescie
                        else if (val < 80)
                            list.Add(1071166, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Szczescie
                        else
                            list.Add(1071166, highIntensity); // <BASEFONT COLOR=#~1_val~>Szczescie
                        break;
                    case AosAttribute.SpellChanneling:
                        list.Add(1060527, mediumIntensity); // <BASEFONT COLOR=#~1_val~>Przepuszczanie zaklec
                        break;
                    case AosAttribute.NightSight:
                        list.Add(1060528, lowIntensity); // <BASEFONT COLOR=#~1_val~>Zapewnia widzenie w ciemnosciach
                        break;
                    default:
                        break;
                }
            }
        }

        public static string UnidentifiedString
        {
            get { return String.Format("<BASEFONT COLOR=#{0:X6}><B>{1}</B><BASEFONT COLOR=#FFFFFF>", "CCCCFF", "Niezidentyfikowane"); }
        }

        public static int MaximumIdentificationRange
        {
            get { return 3; }
        }
		
		public static IdLevel PropertiesCount(AosAttributes att, AosSkillBonuses skill, SlayerName slay1, SlayerName slay2, AosElementAttributes res, AosWeaponAttributes weap, AosArmorAttributes armor, int additional )
		{
			List<int> props = new List<int>();
			if( att != null )
			{
				props.AddRange( new int[] {
					att.WeaponDamage,	// BaseWeapon.GetDamageBonus() dodaje dmg z materialu, z jakosci, i z jakichs dziwnych rzeczy
					att.DefendChance,
					att.BonusDex,
					att.EnhancePotions,
					att.Luck,			// BaseArmor.GetLuckBonus() dodaje tylko luck z materialu, nie potrzebne przy ItemID
					att.CastRecovery,
					att.AttackChance,	// BaseWeapon.GetHitChanceBonus() dodaje jakies dziwne rzeczy ??
					att.BonusHits,
					att.BonusInt,
					att.LowerManaCost,
					att.LowerRegCost,
					att.BonusMana,
					att.RegenMana,
					att.NightSight,
					att.ReflectPhysical,
					att.RegenStam,
					att.RegenHits,
					att.SpellDamage,
					att.BonusStam,
					att.BonusStr,
					att.WeaponSpeed,
					att.SpellChanneling					
					//att.DurabilityBonus,	// jest tez w AosWeaponAttributes
					//att.LowerStatReq		// jest tez w AosWeaponAttributes					
				} );
				if( att.SpellChanneling != 0 )
				{
					if( att.CastSpeed >= 0 ) props.Add(1);
				}
				else
				{
					if( att.CastSpeed >  0 ) props.Add(1);
				}
			}
			if( skill != null ) {
				props.AddRange( new int[] { (int)skill.Skill_1_Value, (int)skill.Skill_2_Value, (int)skill.Skill_3_Value, (int)skill.Skill_4_Value, (int)skill.Skill_5_Value } );
			}
			if( slay1 != SlayerName.None )
			{
				SlayerEntry entry = SlayerGroup.GetEntryByName( slay1 );
				if( entry != null )
					props.Add(1);
			}
			if( slay2 != SlayerName.None )
			{
				SlayerEntry entry = SlayerGroup.GetEntryByName( slay2 );
				if( entry != null )
					props.Add(1);
			}
			if( res != null )
			{
				props.AddRange( new int[] { res.Cold, res.Energy, res.Fire, res.Physical, res.Poison } );
			}
			if( weap != null )
			{
				props.AddRange( new int[] {
					weap.MageWeapon,
					weap.ResistPhysicalBonus,
					weap.ResistFireBonus,
					weap.ResistColdBonus,
					weap.ResistPoisonBonus,
					weap.ResistEnergyBonus,
					weap.UseBestSkill,
					weap.HitColdArea,
					weap.HitDispel,
					weap.HitEnergyArea,
					weap.HitFireArea,
					weap.HitFireball,
					weap.HitHarm,
					weap.HitLeechHits,
					weap.HitLightning,
					weap.HitLowerAttack,
					weap.HitLowerDefend,
					weap.HitMagicArrow,
					weap.HitLeechMana,
					weap.HitPhysicalArea,
					weap.HitPoisonArea,
					weap.HitLeechStam,
					weap.SelfRepair,
					weap.LowerStatReq,
					weap.DurabilityBonus
				} );
			}
			if( armor != null ){
				props.AddRange( new int[] {
					armor.SelfRepair,
					armor.MageArmor,
					armor.DurabilityBonus, 	// BaseArmor.GetDurabilityBonus() - dodaje z materialu i z innych dziwolagow?
					armor.LowerStatReq		// BaseArmor.GetLowerStatReq() - bonus + durability tylko z materialu, niepotrzebne przy ItemID (wystarczy info o bonus)
					
				} );
			}
			int licz = additional;
			foreach( int p in props )
			{
				if( p != 0 )
					++licz;
			}
			switch ( licz )
			{
				case 1: return IdLevel.Level1;
				case 2: return IdLevel.Level2;
				case 3: return IdLevel.Level3;
				case 4: return IdLevel.Level4;
				case 5: return IdLevel.Level5;
                case 6: return IdLevel.Level6;
			}
			return IdLevel.None;		
		}

        private static double[] IDLevels(IIdentifiable o) 
        {
            double[] progi;
            switch (o.IdentifyLevel)
            {
                case IdLevel.Level1: progi = new double[] { 00.0, 50.0 }; break;
                case IdLevel.Level2: progi = new double[] { 40.0, 80.0 }; break;
                case IdLevel.Level3: progi = new double[] { 60.0, 100.0 }; break;
                case IdLevel.Level4: progi = new double[] { 75.0, 100.0 }; break;
                case IdLevel.Level5: progi = new double[] { 90.0, 110.0 }; break;
                case IdLevel.Level6: progi = new double[] { 95.0, 110.0 }; break;
                default: progi = new double[] { 99.0, 105.0 }; break;
            }
            return progi;
        }
		
		public static bool IdentifyCheck( Mobile from, IIdentifiable o )// Uses original mobile as holder of skill
		{
			double[] progi;
            progi = IDLevels(o);			
			double skill = from.Skills[SkillName.ItemID].Value;
			double chance;
				
			if( skill <= progi[0] )
                chance = 0.0;
            else if( skill >= progi[1] )
                chance = 1.0;
            else
                chance = (skill - progi[0]) / (progi[1] - progi[0]);

			// Ulatwienie treningu: zezwalamy na przyrost umiejetnosci dla akcji o zerowej/stuprocentowej szansie powodzenia
			if ( chance == 0.0 )
            {
                from.CheckTargetSkill( SkillName.ItemID, o, 0.01 );
                from.SendLocalizedMessage( 1064633 );   // Nie masz najmniejszych szans na identyfikacje tego przedmiotu.
                return false;
            }
            else if( chance == 1.0 )
            {
                from.CheckTargetSkill( SkillName.ItemID, o, 0.01 );
                return true;
            }
            else
                return from.CheckTargetSkill( SkillName.ItemID, o, chance ) || ( skill>=100.0 );	
		}

		[PlayerVendorTarget]
		private class InternalTarget : Target
		{
			public InternalTarget() :  base ( ItemIdentification.MaximumIdentificationRange, false, TargetFlags.None )
			{
				AllowNonlocal = true;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is Item )
				{		
					if( !((Item)o).IsChildOf( from.Backpack ))
					{
						from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
						return;
					}

					if( o is IIdentifiable )
					{
						IIdentifiable ido = (IIdentifiable) o;
						if( ido.Identified == true )
						{
                            from.SendLocalizedMessage( 1064630 );   // Wiesz juz wszystko o tym przedmiocie
							return;
						}
						if ( IdentifyCheck( from, ido ) )
						{
							ido.Identified = true;
                            from.SendLocalizedMessage( 1064631 );   // Rozpoznalez magiczne cechy przedmiotu
						}
						else
						{
                            from.SendLocalizedMessage( 1064632 );   // Nie masz najmniejszych szans na identyfikacje tego przedmiotu.
						}
					}
					else {
						from.SendLocalizedMessage( 500353 ); // You are not certain...
						return;
					}
					
					return;
				}
				else if ( o is Mobile )
				{
					((Mobile)o).OnSingleClick( from );
				}
				else
				{
					from.SendLocalizedMessage( 500353 ); // You are not certain...
				}
				//allows the identify skill to reveal attachments
				//Server.Engines.XmlSpawner2.XmlAttach.RevealAttachments(from,o);
			}
		}
	}
}