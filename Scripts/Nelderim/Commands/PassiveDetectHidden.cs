using System;
using Server;
using Server.SkillHandlers;
using Server.Mobiles;

namespace Server.Commands
{ 
	public class PassiveDetectHiddenCommand
    {
		public static void Initialize()
       	{
          	CommandSystem.Register( "wykrywanie", AccessLevel.Player, new CommandEventHandler( PassiveDetectHidden_OnCommand ) ); 
       	} 
       	
		[Usage( "Wykrywanie" )]
       	[Description( "Wlacza lub wylacza umiejetnosc biernego wykrywania" )] 
       	public static void PassiveDetectHidden_OnCommand( CommandEventArgs e ) 
       	{
            PlayerMobile pm = (PlayerMobile)e.Mobile;

            if ( pm.AccessLevel > AccessLevel.Player )
            {
                pm.SendMessage( "Ta komenda nie dziala na postaci GM." );

                return;
            }

            if ( !pm.Alive )
            {
                pm.SendMessage( "Nie mozesz uzyc tej umiejetnosci w tym stanie." );
                return;
            }

            if( pm.PassiveDetectHiddenTimer == null )
                pm.PassiveDetectHiddenTimer = new PassiveDetectHiddenTimer( pm );

            if ( pm.PassiveDetectHiddenTimer.Running )
            {
                pm.SendMessage( 0x20, "Wylaczyles bierne wykrywanie." );
                pm.PassiveDetectHiddenTimer.Stop();
            }
            else
            {
                if ( DateTime.Now >= pm.NextPassiveDetectHiddenCheck )
                {
                    pm.SendMessage(  0x40, "Wlaczyles bierne wykrywanie." );
                    pm.PassiveDetectHiddenTimer.Start();
                }
                else
                    pm.SendMessage( "Musisz chwile poczekac, zanim bedziesz mogl ponownie uzyc tej umiejetnosci." );
            }
       	} 
   	} 
} 
