using Server;
using Server.Commands;
using Server.Commands.Generic;
using Server.Items;

namespace Nelderim.Commands
{
    public class Decor
    {
        public static void Initialize()
        {
            TargetCommands.Register(new AddDecor());
            TargetCommands.Register(new RemoveDecor());
        }

        public class AddDecor : BaseCommand
        {
            public AddDecor()
            {
                AccessLevel = AccessLevel.Counselor;
                Supports = CommandSupport.Simple;
                Commands = new string[] { "AddDecor" };
                ObjectTypes = ObjectTypes.All;
                Usage = "AddDecor itemID [hue]";
                Description = "";
            }

            public override bool ValidateArgs(BaseCommandImplementor impl, CommandEventArgs e)
            {
                return e.Length >= 1;
            }

            public override void Execute(CommandEventArgs e, object obj)
            {
                Mobile from = e.Mobile;

                IPoint3D p = obj as IPoint3D;

                if (p == null)
                    return;

                int itemId;
                if (!int.TryParse(e.GetString(0), out itemId))
                {
                    from.SendMessage("Invalid itemid");
                    return;
                }

                int hue = e.GetInt32(1);

                Static decor = new Static(itemId);
                decor.Hue = hue;
                decor.MoveToWorld(new Point3D(p), from.Map);
            }
        }

        public class RemoveDecor : BaseCommand
        {
            public RemoveDecor()
            {
                AccessLevel = AccessLevel.Counselor;
                Supports = CommandSupport.AllNPCs | CommandSupport.AllItems;
                Commands = new string[] { "RemoveDecor" };
                ObjectTypes = ObjectTypes.Both;
                Usage = "RemoveDecor";
                Description = "";
            }

            public override void Execute(CommandEventArgs e, object obj)
            {
                Static p = obj as Static;
                if (p == null)
                {
                    e.Mobile.SendMessage("This is not a decor");
                    return;
                }

                p.Delete();
            }
        }
    }
}