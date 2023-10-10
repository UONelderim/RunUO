using System;
using Server;
using Server.Items;
using Server.Mobiles;
using Server.Targets;
using Server.Network;
using Server.Targeting;
using Ultima;
using Server.Commands.Generic;

namespace Server.Commands
{
    public class emote
    {
        private static int m_Color;

        public static void Initialize()
        {
            m_Color = 99;
            TargetCommands.Register(new ShowEmote());
            Register( "SetemoteColor", AccessLevel.Counselor, new CommandEventHandler( SetemoteColorCommand ) );
        }
        
		public static void Register( string command, AccessLevel access, CommandEventHandler handler )
        {
            CommandSystem.Register( command, access, handler );
        }

        public class ShowEmote : BaseCommand
        {
            public ShowEmote()
            {
                AccessLevel = AccessLevel.Counselor;
                Supports = CommandSupport.Simple;
                Commands = new string[] { "Em" };
                ObjectTypes = ObjectTypes.All;
                Usage = "Em <emote text to display>";
                Description = "Force target to emote <text>.";
            }

            public override bool ValidateArgs(BaseCommandImplementor impl, CommandEventArgs e)
            {
                return e.Length >= 1;
            }

            public override void Execute(CommandEventArgs e, object targeted)
            {
                Mobile from = e.Mobile;

                string toEmote = e.ArgString;

                if (targeted is Mobile)
                {
                    Mobile targ = (Mobile)targeted;

                    if (from != targ && from.TrueAccessLevel > targ.TrueAccessLevel)
                    {
                        CommandLogging.WriteLine(from, "{0} {1} forcing speech on {2}", from.TrueAccessLevel, CommandLogging.Format(from), CommandLogging.Format(targ));
                        targ.Say("*" + toEmote + "*");
                    }
                }
                else if (targeted is Item)
                {
                    Item targ = (Item)targeted;

                    targ.PublicOverheadMessage(MessageType.Regular, Say.Color, false, "*" + toEmote + "*");
                }
                else if (targeted is IPoint3D)
                {
                    Static emoteHolder = new TemporaryItem();
                    emoteHolder.MoveToWorld(new Point3D((IPoint3D)targeted), from.Map);

                    emoteHolder.PublicOverheadMessage(MessageType.Regular, Say.Color, false, "*" + toEmote + "*");
                }
                else
                {
                    from.SendMessage("Invalid target");
                }
            }
        }

        public static int Color
        {
            get { return m_Color; }
        }

        [Usage( "SetemoteColor <int>" )]
        [Description( "Defines the color for items speech with the emote command" )]
        public static void SetemoteColorCommand( CommandEventArgs e )
        {
            if ( e.Length <= 0 )
                e.Mobile.SendMessage( "Format: SetemoteColor \"<int>\"" );
            else
            {
                m_Color = e.GetInt32(0);
                e.Mobile.SendMessage( m_Color, "You choosed this color." );
            }
        }
    }

    class TemporaryItem : Static
    {
        private Timer m_Timer;

        private class ItemCleaner : Timer
        {
            private TemporaryItem m_Owner;
            public ItemCleaner(TemporaryItem owner) : base(TimeSpan.FromSeconds(10))
            {
                m_Owner = owner;
            }

            protected override void OnTick()
            {
                m_Owner.Delete();
            }
        }
        public TemporaryItem() : base(0x1)
        {
            Name = "";

            m_Timer = new ItemCleaner(this);
            m_Timer.Start();
        }

        public TemporaryItem(Serial serial) : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);
        }
    };
}
