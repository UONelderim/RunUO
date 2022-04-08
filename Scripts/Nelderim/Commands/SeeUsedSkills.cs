// 05.04.12  :: troyan
// 05.11.17 :: troyan :: logowanie
// 06.03.25 :: troyan :: modyfikacje + lokalizacja

using System;
using Server;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Commands
{
    public class SeeUsedSkills
    {
        public static void Initialize()
        {
			Register( "SeeUsedSkills", AccessLevel.Counselor, new CommandEventHandler( SeeUsedSkillsCommand ) );
			Register( "SUS", AccessLevel.Counselor, new CommandEventHandler( SeeUsedSkillsCommand ) );
            Register( "ph", AccessLevel.Counselor, new CommandEventHandler( SeePowerHourCommand ) );
        }
		
        public static void Register( string command, AccessLevel access, CommandEventHandler handler )
        {
            CommandSystem.Register( command, access, handler );
        }

        [Usage( "SeeUsedSkills <span|off>" )]
		[Aliases( "SUS" )]
        [Description( "Pokazuje uzywane przez cel skille do wylaczenia (off) lub przeminiecia 60 sekund (lub czasu span)." )]
        public static void SeeUsedSkillsCommand( CommandEventArgs e )
        {
			if ( e.Length > 0 )
			{
				if ( e.GetString( 0 ) == "off" )
					e.Mobile.Target = new SeeUsedSkillsTarget( "off" );
				else 
					e.Mobile.Target = new SeeUsedSkillsTarget( e.GetInt32( 0 ) );
			}
            else
                e.Mobile.Target = new SeeUsedSkillsTarget( );
        }
		
        public class SeeUsedSkillsTarget : Target
        {
			private TimeSpan Span = TimeSpan.FromMinutes( 1 );
			private bool Stop = false;
            
            public SeeUsedSkillsTarget( ) : base ( -1, false, TargetFlags.None )
            { 
            }
			
			public SeeUsedSkillsTarget( int span ) : base ( -1, false, TargetFlags.None )
            {	
                Span = TimeSpan.FromSeconds ( span );
            }
			
			public SeeUsedSkillsTarget( string off ) : base ( -1, false, TargetFlags.None )
            {	
                Stop = true;
            }
			
            protected override void OnTarget( Mobile from, object targeted )
            {
                if ( targeted is PlayerMobile )
                {
					PlayerMobile pm = targeted as PlayerMobile;
					
					// 05.11.17 :: troyan :: logowanie
					string log = from.AccessLevel + " " + CommandLogging.Format( from );
					log += " tried to See " + CommandLogging.Format( targeted );
					log += " Used Skills [SUS]";
					
					CommandLogging.WriteLine( from, log );
					
					if ( pm.AccessLevel >= from.AccessLevel )
						from.SendLocalizedMessage( 505849 ); // Masz zbyt niskie uprawnienia do sledzenia poczynan celu!
					else
					{
						if ( Stop )
						{
							pm.StopTracking( from );
							from.SendLocalizedMessage( 505850 ); // Zaprzestano sledzenia celu.
						}
						else
						{
							pm.StartTracking( from, DateTime.Now + Span );
						    from.SendLocalizedMessage( 505851, pm.Name ); // "Rozpoczeto sledzenie poczynan celu -> {0}"
						}
					}
                }
                else
                	from.SendLocalizedMessage( 505852 ); // "Cel nie jest graczem."
            }
        }

        [Usage( "ph" )]
        [Description( "Informuje czy gracz ma uruchomione Power Hour" )]
        public static void SeePowerHourCommand( CommandEventArgs e )
        {
           e.Mobile.Target = new SeePowerHourTarget( );
        }
		
        public class SeePowerHourTarget : Target
        {           
            public SeePowerHourTarget( ) : base ( -1, false, TargetFlags.None )
            { 
            }
			
            protected override void OnTarget( Mobile from, object targeted )
            {
                if ( targeted is PlayerMobile )
                {
					PlayerMobile pm = targeted as PlayerMobile;
					
					if ( pm.HasPowerHour )
                    {
                        from.SendLocalizedMessage(1063287);//Gracz ma uruchomiony Power Hour.
                    }
                    else
                    {
                        from.SendLocalizedMessage(1063288);//Gracz nie ma uruchomionego Power Hour.
                    }
                }
                else
                	from.SendLocalizedMessage( 505852 ); // "Cel nie jest graczem."
            }
        }
    }
}
