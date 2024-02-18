using System;
using Server.Gumps;
using Server.Mobiles;
using Server.Network;

namespace Server.Commands
{
    public class XmlSlow
    {
        public static void Initialize()
        {
            CommandSystem.Register("xmlslow", AccessLevel.Administrator, OnCommand);
        }

        public static void OnCommand(CommandEventArgs e)
        {
            PlayerMobile pm = (PlayerMobile)e.Mobile;
            pm.CloseGump(typeof(XmlSlowGump));
            pm.SendGump(new XmlSlowGump());
        }
    }

    public class XmlSlowGump : Gump
    {
        public XmlSlowGump() : base(10,10)
        {
            AddPage(0);
            AddBackground(0, 0, 330, 120, 5054);
            AddLabel(10,10, 0, "XmlSpawner Slow Spawners");
            AddLabel(10, 40, 0, "Enabled");
            AddCheck(90, 40, 0xD2, 0xD3, XmlSpawner.SpawnerTimer.LogSlow, 1);
            AddLabel(10, 70, 0, "Threshold ms");
            AddBackground(90, 60, 80, 40, 0xDAC);
            AddTextEntry(115, 70, 50, 20, 0, 2, XmlSpawner.SpawnerTimer.SlowThreshold.TotalMilliseconds.ToString());
            AddButton(200, 70, 0xEF, 0xF0, 1, GumpButtonType.Reply, 0);
        }

        public override void OnResponse(NetState sender, RelayInfo info)
        {
            var from = sender.Mobile;
            if (info.ButtonID == 1)
            {
                if (info.Switches.Length > 0)
                {
                    XmlSpawner.SpawnerTimer.LogSlow = true;
                    XmlSpawner.SpawnerTimer.LogTarget = from;
                    from.SendMessage(62, "Slow spawners logging enabled");
                }
                else
                {
                    XmlSpawner.SpawnerTimer.LogSlow = false;
                    XmlSpawner.SpawnerTimer.LogTarget = null;
                    from.SendMessage(32, "Slow spawners logging disabled");
                }

                if(Int32.TryParse(info.TextEntries[0].Text, out var ms))
                {
                    XmlSpawner.SpawnerTimer.SlowThreshold = TimeSpan.FromMilliseconds(ms);
                }
                else
                {
                    from.SendMessage("Invalid");
                }
                from.SendGump(new XmlSlowGump());
            }
        }
    }
}