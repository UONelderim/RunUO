    //////////////////////////////////
   //			           //
  //      Scripted by Raelis      //
 //		             	 //
//////////////////////////////////
using System; 
using System.Collections; 
using Server; 
using Server.Mobiles; 
using Server.Network; 
using Server.Commands;
using Server.Targeting;


namespace Server.Mobiles
{ 
	public class KPSystem
	{ 

		public static void Initialize()
		{
			CommandSystem.Register( "PKT", AccessLevel.Player, new CommandEventHandler( KP_OnCommand ) );    
		} 

		public static void KP_OnCommand( CommandEventArgs args )
		{ 
			Mobile m = args.Mobile; 
			PlayerMobile from = m as PlayerMobile; 
          
			if( from != null ) 
			{  
				from.SendMessage ( "Zaznacz smoka ktorego jestes wlascicielem by zobaczyc jego punkty doswiadczenia." );
				m.Target = new InternalTarget();
			} 
		} 

		private class InternalTarget : Target
		{
			public InternalTarget() : base( 8, false, TargetFlags.None )
			{
			}

			protected override void OnTarget( Mobile from, object obj )
			{
				if ( !from.Alive )
				{
					from.SendMessage( "Nie zrobisz tego bedac martwym." );
				}
                           	else if ( obj is EvolutionDragon && obj is BaseCreature ) 
                           	{ 
					BaseCreature bc = (BaseCreature)obj;
					EvolutionDragon ed = (EvolutionDragon)obj;

					if ( ed.Controlled == true && ed.ControlMaster == from )
					{
						ed.PublicOverheadMessage( MessageType.Regular, ed.SpeechHue, true, ed.Name +" ma "+ ed.KP +" punktów.", false );
					}
					else
					{
						from.SendMessage( "Nie kontrolujesz tego smoka!" );
					}
                           	} 
							            	else if ( obj is EvolutionDragon2 && obj is BaseCreature ) 
                           	{ 
					BaseCreature bc = (BaseCreature)obj;
					EvolutionDragon2 ed = (EvolutionDragon2)obj;

					if ( ed.Controlled == true && ed.ControlMaster == from )
					{
						ed.PublicOverheadMessage( MessageType.Regular, ed.SpeechHue, true, ed.Name +" ma "+ ed.KP +" punktów.", false );
					}
					else
					{
						from.SendMessage( "Nie kontrolujesz tego smoka!" );
					}
                           	} 
							            	else if ( obj is EvolutionDragon3 && obj is BaseCreature ) 
                           	{ 
					BaseCreature bc = (BaseCreature)obj;
					EvolutionDragon3 ed = (EvolutionDragon3)obj;

					if ( ed.Controlled == true && ed.ControlMaster == from )
					{
						ed.PublicOverheadMessage( MessageType.Regular, ed.SpeechHue, true, ed.Name +" ma "+ ed.KP +" punktów.", false );
					}
					else
					{
						from.SendMessage( "Nie kontrolujesz tego smoka!" );
					}
                           	} 
							            	else if ( obj is EvolutionDragon4 && obj is BaseCreature ) 
                           	{ 
					BaseCreature bc = (BaseCreature)obj;
					EvolutionDragon4 ed = (EvolutionDragon4)obj;

					if ( ed.Controlled == true && ed.ControlMaster == from )
					{
						ed.PublicOverheadMessage( MessageType.Regular, ed.SpeechHue, true, ed.Name +" ma "+ ed.KP +" punktów.", false );
					}
					else
					{
						from.SendMessage( "Nie kontrolujesz tego smoka!" );
					}
                           	} 
							            	else if ( obj is EvolutionDragon5 && obj is BaseCreature ) 
                           	{ 
					BaseCreature bc = (BaseCreature)obj;
					EvolutionDragon5 ed = (EvolutionDragon5)obj;

					if ( ed.Controlled == true && ed.ControlMaster == from )
					{
						ed.PublicOverheadMessage( MessageType.Regular, ed.SpeechHue, true, ed.Name +" ma "+ ed.KP +" punktów.", false );
					}
					else
					{
						from.SendMessage( "Nie kontrolujesz tego smoka!" );
					}
                           	} 
                           	else 
                           	{ 
                              		from.SendMessage( "To nie jest smok!" );
			   	}
			}
		}
	} 
} 
