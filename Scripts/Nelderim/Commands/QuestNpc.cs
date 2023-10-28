using Server;
using Server.Commands;
using Server.Commands.Generic;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;
using System.Collections.Generic;

namespace Nelderim.Commands
{
    public class QuestNpcCommand
    {
        public static void Initialize()
        {
            TargetCommands.Register(new DecorateNpc());
            TargetCommands.Register(new AddQuestNpc());
            TargetCommands.Register(new RemoveQuestNpc());
        }


        public class DecorateNpc : BaseCommand
        {
            private static Dictionary<Mobile, Item> m_SelectedItems = new Dictionary<Mobile, Item>();
            public DecorateNpc()
            {
                AccessLevel = AccessLevel.Counselor;
                Supports = CommandSupport.Simple;
                Commands = new string[] { "DecorateNpc" };
                ObjectTypes = ObjectTypes.All;
                Usage = "DecorateNpc";
                Description = "";
            }

            public override void Execute(CommandEventArgs e, object targeted)
            {
                Mobile from = e.Mobile;

                string toEmote = e.ArgString;

                if (targeted is Item)
                {
                    Item it = (Item)targeted;

                    if (it.IsAccessibleTo(e.Mobile) && it.Parent == e.Mobile.Backpack)
                    {
                        m_SelectedItems[from] = it;

                        e.Mobile.SendMessage("Wskaz NPC ktorego chcesz odziac");
                        e.Mobile.BeginTarget(18, false, TargetFlags.None, new TargetCallback(OnNpcSelected));
                    }
                    else if (it.Parent != null && it.Parent is CustomQuestNpc)
                    {
                        PlayerMobile pm = from as PlayerMobile;
                        if (pm != null)
                            (it.Parent as CustomQuestNpc).RemoveCloth(it, pm);
                    }
                    else
                    {
                        e.Mobile.SendMessage("Mozesz wskazac jedynie przedmioty ze swojego plecaka lub z paperdola Questego Npc.");
                        return;
                    }
                }
            }

            private void OnNpcSelected(Mobile from, object targeted)
            {
                CustomQuestNpc npc = targeted as CustomQuestNpc;
                if (npc == null)
                {
                    from.SendMessage("Mozesz przebrac jedynie specjalnego NPC stworzonego komendą [AddQuestNpc");
                    return;
                }

                Item wearable;
                if (!m_SelectedItems.TryGetValue(from, out wearable))
                {
                    from.SendMessage("Error.");
                    return;
                }

                m_SelectedItems.Remove(from);

                PlayerMobile pm = from as PlayerMobile;
                if (pm != null)
                    npc.ReplaceCloth(wearable, pm);
            }
        }

        public class AddQuestNpc : BaseCommand
        {
            public AddQuestNpc()
            {
                AccessLevel = AccessLevel.Counselor;
                Supports = CommandSupport.Simple;
                Commands = new string[] { "AddQuestNpc" };
                ObjectTypes = ObjectTypes.All;
                Usage = "AddQuestNpc [name]";
                Description = "";
            }

            public override void Execute(CommandEventArgs e, object targeted)
            {
                if (targeted is IPoint3D)
                {
                    CustomQuestNpc npc = new CustomQuestNpc();

                    if (e.Arguments.Length > 0)
                    {
                        npc.Name = e.GetString(0);
                    }

                    npc.MoveToWorld(new Point3D(targeted as IPoint3D), e.Mobile.Map);
                }
            }
        }

        public class RemoveQuestNpc : BaseCommand
        {
            public RemoveQuestNpc()
            {
                AccessLevel = AccessLevel.Counselor;
                Supports = CommandSupport.Simple;
                Commands = new string[] { "RemoveQuestNpc" };
                ObjectTypes = ObjectTypes.All;
                Usage = "RemoveQuestNpc";
                Description = "";
            }

            public override void Execute(CommandEventArgs e, object targeted)
            {
                if (targeted is CustomQuestNpc)
                {
                    CustomQuestNpc npc = targeted as CustomQuestNpc;

                    npc.Delete();
                }
                else
                {
                    e.Mobile.SendMessage("Mozesz usunac jedynie NPC dodanego specjalna komenda (wskazany NPC nie jest typu CustomQuestNpc)");
                }
            }
        }
    }
}