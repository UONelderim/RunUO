using System;
using System.Collections;
using Server.Network;
using Server.Mobiles;
using Server.Items;
using Server.Misc;
using Server.Commands;

namespace Server.Gumps 
{
    public class StatsGump : Gump
    {
		public static void Initialize()
		{
			Register( "mystats", AccessLevel.Player, new CommandEventHandler( MyStats_OnCommand ) );
        }
    
		public static void Register( string command, AccessLevel access, CommandEventHandler handler )
		{
            CommandSystem.Register(command, access, handler);
		}

		[Usage( "MyStats" )]
		[Description( "Opens Stats Gump." )]
		public static void MyStats_OnCommand( CommandEventArgs e )
		{
			Mobile from = e.Mobile;
			from.CloseGump( typeof( StatsGump ) );
			from.SendGump( new StatsGump( from ) );
				
        }
   
        public StatsGump ( Mobile from ) : base ( 100,100 )
        {
           // configuration
            int LRCCap = 100;
            int LMCCap = 40;
            double BandageSpeedCap = 2.0;
            int SwingSpeedCap = 100;
            int HCICap = 45;
            int DCICap = 45;
            int FCCap = 4; 
            int FCRCap = 4;
            int DamageIncreaseCap = 100;
            int SDICap = 100;
            int ReflectDamageCap = 100;
            int SSICap = 100;
            
            // OSI configuration - prob the default in RUNUO
            /*
            int LRCCap = 100;
            int LMCCap = 40;
            double BandageSpeedCap = 2.0;
            int SwingSpeedCap = 100;
            int HCICap = 45;
            int DCICap = 45;
            int FCCap = 4; // FC 4 For Paladin, otherwise FC 2 for Mage
            int FCRCap = 4;
            int DamageIncreaseCap = 100;
            int SDICap = 100;
            int ReflectDamageCap = 100;
            int SSICap = 100;
            */

            int LRC = AosAttributes.GetValue( from, AosAttribute.LowerRegCost ) > LRCCap ? LRCCap : AosAttributes.GetValue( from, AosAttribute.LowerRegCost );
            int LMC = AosAttributes.GetValue( from, AosAttribute.LowerManaCost ) > LMCCap ? LMCCap : AosAttributes.GetValue( from, AosAttribute.LowerManaCost );
            double BandageSpeed = ( 2.0 + (0.5 * ((double)(205 - from.Dex) / 10)) ) < BandageSpeedCap ? BandageSpeedCap : ( 2.0 + (0.5 * ((double)(205 - from.Dex) / 10)) );
            TimeSpan SwingSpeed = (from.Weapon as BaseWeapon).GetDelay(from) > TimeSpan.FromSeconds(SwingSpeedCap) ? TimeSpan.FromSeconds(SwingSpeedCap) : (from.Weapon as BaseWeapon).GetDelay(from);
            int HCI = AosAttributes.GetValue( from, AosAttribute.AttackChance ) > HCICap ? HCICap : AosAttributes.GetValue( from, AosAttribute.AttackChance );
            int DCI = AosAttributes.GetValue( from, AosAttribute.DefendChance ) > DCICap ? DCICap : AosAttributes.GetValue( from, AosAttribute.DefendChance );
            int FC = AosAttributes.GetValue( from, AosAttribute.CastSpeed ) > FCCap ? FCCap : AosAttributes.GetValue( from, AosAttribute.CastSpeed );
            int FCR = AosAttributes.GetValue( from, AosAttribute.CastRecovery ) > FCRCap ? FCRCap : AosAttributes.GetValue( from, AosAttribute.CastRecovery );
            int DamageIncrease = AosAttributes.GetValue( from, AosAttribute.WeaponDamage ) > DamageIncreaseCap ? DamageIncreaseCap : AosAttributes.GetValue( from, AosAttribute.WeaponDamage );
            int SDI = AosAttributes.GetValue( from, AosAttribute.SpellDamage ) > SDICap ? SDICap : AosAttributes.GetValue( from, AosAttribute.SpellDamage );
            int ReflectDamage = AosAttributes.GetValue( from, AosAttribute.ReflectPhysical ) > ReflectDamageCap ? ReflectDamageCap : AosAttributes.GetValue( from, AosAttribute.ReflectPhysical );
            int SSI = AosAttributes.GetValue( from, AosAttribute.WeaponSpeed ) > SSICap ? SSICap : AosAttributes.GetValue( from, AosAttribute.WeaponSpeed );


			Dragable = true;
			Closable = true;
			Resizable = false;
			Disposable = false;

			AddPage(0);
			AddBackground(20, 56, 588, 375, 3500);
			AddBackground(240, 240, 78, 33, 9300);
			AddBackground(100, 240, 78, 33, 9300);
            AddBackground(100, 120, 78, 33, 9300);
			AddBackground(100, 160, 78, 33, 9300);
			AddBackground(100, 200, 78, 33, 9300);
            AddBackground(240, 120, 88, 33, 9300);
            AddBackground(240, 160, 88, 33, 9300);
            AddBackground(460, 120, 119, 33, 9300);
            AddBackground(390, 160, 68, 33, 9300);
            AddBackground(510, 160, 68, 33, 9300);
            AddBackground(390, 330, 61, 33, 9300);
            AddBackground(480, 210, 98, 33, 9300);
			AddBackground(480, 250, 98, 33, 9300);
			AddBackground(480, 290, 98, 33, 9300);
            AddBackground(390, 370, 61, 33, 9300);
			AddBackground(510, 330, 61, 33, 9300);
			AddBackground(100, 280, 78, 33, 9300);
			AddBackground(100, 320, 78, 33, 9300);
			AddBackground(100, 360, 78, 33, 9300);
			AddBackground(270, 280, 53, 33, 9300);
			AddBackground(270, 320, 53, 33, 9300);
			AddBackground(270, 360, 53, 33, 9300);
			AddBackground(380, 70, 61, 33, 9300);
            AddBackground(20, 10, 588, 55, 3600);
            AddBackground(520, 370, 61, 33, 9300);
            AddImageTiled(580, 110, 5, 302, 2701);
            AddImageTiled(330, 110, 5, 302, 2701); 
			AddImageTiled(44, 110, 5, 302, 2701); 
            AddImageTiled(46, 110, 539, 5, 2700); 
            AddImageTiled(46, 410, 539, 5, 2700);
            AddButton(570, 20, 1151, 1150, 0, GumpButtonType.Reply, 0);
            AddHtml(40, 30, 555, 29, "<basefont size=5 color=#F5F3E4><center>Stats Gump", false, false);
            AddHtml(530, 20, 60, 33, "<basefont size=5 color=#F5F3E4>Close", false, false);
			AddLabel(60, 77, 0, String.Format( "{0}", from.Name ) );
			AddHtml(50, 126, 44, 33, "<basefont size=5 color=#1F1E1C><center>Sila", false, false);
            AddLabel(110, 127, 0, String.Format(" {0} + {1}", from.RawStr, from.Str - from.RawStr ) ); //str
			AddHtml(50, 166, 44, 33, "<basefont size=5 color=#1F1E1C><center>Zrecznosc", false, false);
            AddLabel(110, 167, 0, String.Format(" {0} + {1}", from.RawDex, from.Dex - from.RawDex ) ); //dex
			AddHtml(50, 206, 44, 33, "<basefont size=5 color=#1F1E1C><center>Inteligencja", false, false);
            AddLabel(110, 207, 0, String.Format(" {0} + {1}", from.RawInt, from.Int - from.RawInt ) ); //int
			AddHtml(180, 126, 55, 33, "<basefont size=5 color=#1F1E1C><center>Slawa", false, false);
            AddLabel(250, 127, 0, String.Format(" {0} ", from.Fame ) ); //fame		
			AddHtml(180, 166, 55, 33, "<basefont size=5 color=#1F1E1C><center>Karma", false, false);		
			AddLabel(250, 167, 0, String.Format(" {0} ", from.Karma ) ); //karma
			AddHtml(340, 126, 107, 29, "<basefont size=5 color=#1F1E1C><center>Poswiecenie", false, false);
			AddLabel(460, 127, 0, String.Format(" {0} ", from.TithingPoints) ); //tith points
			AddHtml(340, 166, 44, 29, "<basefont size=5 color=#1F1E1C><center>LRC", false, false);
            AddLabel(400, 167, 0, String.Format(" {0} %", LRC ) ); //lrc
			AddHtml(460, 166, 44, 29, "<basefont size=5 color=#1F1E1C><center>LMC", false, false);
			AddLabel(520, 167, 0, String.Format(" {0} %", LMC ) ); //lmc
			AddHtml(340, 216, 127, 29, "<basefont size=5 color=#1F1E1C><center>Szybkosc leczenia", false, false);
			AddLabel(490, 217, 0, String.Format(" {0:0.0}s", new DateTime(TimeSpan.FromSeconds( BandageSpeed ).Ticks).ToString("s.ff") ) ); //bandage speed
            //AddLabel( 490, 217, 0, String.Format(" {0:0.0} s", BandageSpeed ); //bandage speed
			AddHtml(340, 256, 127, 29, "<basefont size=5 color=#1F1E1C><center>SSI", false, false);
			AddLabel(490, 257, 0, String.Format(" {0}s", new DateTime(SwingSpeed.Ticks).ToString("s.ff") ) ); //swing speed
			AddHtml(340, 296, 127, 29, "<basefont size=5 color=#1F1E1C><center>HCI/DCI", false, false);
			AddLabel(490, 297, 0, String.Format(" {0} / {1}", HCI, DCI ) ); //hci/dci
			AddHtml(340, 336, 44, 33, "<basefont size=5 color=#1F1E1C><center>SDI", false, false);
			AddLabel(400, 337, 0, String.Format(" {0} %", SDI ) ); //sdi
			AddHtml(340, 376, 44, 33, "<basefont size=5 color=#1F1E1C><center>SSI", false, false);
			AddLabel(400, 377, 0, String.Format(" {0} %", SSI ) ); //ssi
			AddHtml(50, 246, 44, 33, "<basefont size=5 color=#1F1E1C><center>FC", false, false);
			AddLabel(110, 247, 0, String.Format(" {0}", FC ) ); //fc
			AddHtml(190, 246, 44, 33, "<basefont size=5 color=#1F1E1C><center>FCR", false, false);
			AddLabel(250, 247, 0, String.Format(" {0}", FCR ) ); //fcr
			AddHtml(460, 336, 44, 33, "<basefont size=5 color=#1F1E1C><center>DI", false, false);
			AddLabel(520, 337, 0, String.Format(" {0} %", DamageIncrease ) ); //di
			AddHtml(50, 286, 44, 33, "<basefont size=5 color=#1F1E1C><center>Hits", false, false);
			AddLabel(110, 287, 0, String.Format(" {0} + {1}", from.Hits - AosAttributes.GetValue( from, AosAttribute.BonusHits ), AosAttributes.GetValue( from, AosAttribute.BonusHits ) ) ); //hits
			AddHtml(50, 326, 44, 33, "<basefont size=5 color=#1F1E1C><center>Stam", false, false);
			AddLabel(110, 327, 0, String.Format(" {0} + {1}", from.Stam - AosAttributes.GetValue( from, AosAttribute.BonusStam ), AosAttributes.GetValue( from, AosAttribute.BonusStam ) ) ); //stamina
			AddHtml(50, 366, 49, 33, "<basefont size=5 color=#1F1E1C><center>Mana", false, false);
			AddLabel(110, 367, 0, String.Format(" {0} + {1}", from.Mana - AosAttributes.GetValue( from, AosAttribute.BonusMana ), AosAttributes.GetValue( from, AosAttribute.BonusMana ) ) ); //mana
			AddHtml(180, 286, 88, 33, "<basefont size=5 color=#1F1E1C><center>HP Regen", false, false);
			AddLabel(280, 290, 0, String.Format(" {0}", AosAttributes.GetValue( from, AosAttribute.RegenHits ) ) ); //hp regen
			AddHtml(180, 326, 88, 33, "<basefont size=5 color=#1F1E1C><center>Sta Regen", false, false);
			AddLabel(280, 327, 0, String.Format(" {0}", AosAttributes.GetValue( from, AosAttribute.RegenStam ) ) ); //s regen
			AddHtml(180, 366, 88, 33, "<basefont size=5 color=#1F1E1C><center>ManRegen", false, false);
			AddLabel(280, 367, 0, String.Format(" {0}", AosAttributes.GetValue( from, AosAttribute.RegenMana ) ) ); //m regen
            AddHtml(455, 376, 60, 33, "<basefont size=5 color=#1F1E1C><center>Reflect", false, false);
			AddLabel(530, 377, 0, String.Format(" {0} %", ReflectDamage ) ); //reflect
			AddHtml(330, 76, 44, 33, "<basefont size=5 color=#1F1E1C><center>Morderstwa", false, false);
			AddLabel(390, 77, 0, String.Format(" {0} ", from.Kills) ); //kills
			
        }
    
		public override void OnResponse( NetState sender, RelayInfo info )
		{
			Mobile from = sender.Mobile;

			switch ( info.ButtonID )
			{
				case 1:
				{
					break;
				}
			}
		}
    }
    
    
    
}