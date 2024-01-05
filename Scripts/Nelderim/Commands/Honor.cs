using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Targets;
using Server.Network;
using Server.Targeting;

namespace Server.Commands
{
    public class HonorCommands
    {
        private static int m_Color;

        public static void Initialize()
        {
            Register( "Honor", AccessLevel.Player, new CommandEventHandler( HonorCommand ) );
        }
        public static void Register( string command, AccessLevel access, CommandEventHandler handler )
        {
            CommandSystem.Register( command, access, handler );
        }

        [Usage( "Honor" )]
        [Description( "Use Honor virtue on target (other than player characters)" )]
        public static void HonorCommand( CommandEventArgs e )
        {
            string toSay = e.ArgString.Trim();

            e.Mobile.Target =  new HonorTarget();
        }
		
        public class HonorTarget : Target
        {
            public HonorTarget() : base ( 12, false, TargetFlags.None )
            {
            }
            protected override void OnTarget( Mobile from, object targeted )
            {
                if (from is PlayerMobile && targeted is BaseCreature )
                {
                    PlayerMobile pm = (PlayerMobile)from;
                    BaseCreature bc = (BaseCreature)targeted;

                    HonorVirtue.Honor(pm, bc);
                }
                else
                {
                    from.SendMessage("Ta komenda dziala tylko na potwory/zwierzeta.");
                }
            }
        }
    }
}
