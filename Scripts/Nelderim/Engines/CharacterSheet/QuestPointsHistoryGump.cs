#region References

using Nelderim.CharacterSheet;
using Server.Mobiles;

#endregion

namespace Server.Gumps
{
    public class QuestPointsHistoryGump : Gump
    {
        public QuestPointsHistoryGump(PlayerMobile viewer, PlayerMobile player) 
            : base(0, 0)
        {
            Dragable = true;
            Closable = true;
            Resizable = false;
            Disposable = false;

            bool gmView = viewer.AccessLevel >= AccessLevel.Counselor;

				AddPage(0);
            AddBackground(0, 0, 600, 600, 5054);
            
            AddImageTiled(10, 10, gmView ? 85 : 155, 550, 1416); 
            AddImageTiled(171, 10, 60, 550, 1416); 
            AddLabel(13, 13, 0x480, "Kiedy");
            if (gmView)
                AddLabel(98, 13, 0x480, "GM");
            AddLabel(173, 13, 0x480, "Punkty");
            AddLabel(233, 13, 0x480, "Powód");

            int y = 38;
            foreach (var qphe in CharacterSheet.Get(player).QuestPointsHistoryEntries.Reverse())
            {
                AddLabel(13, y, 0x480, qphe.DateTime.ToShortDateString());
                if (gmView)
                    AddLabel(98, y, 0x480, qphe.GameMaster);
                AddLabel(173, y, 0x480, qphe.Change.ToString("+#;-#;0")); //Formatting to always add sign
                AddLabelCropped(233, y, 350, 18, 0x480, qphe.Reason);
                y += 25;
                if (y > 560) break;
            }
            
            AddButton(558, 568, 4005, 4007, 0, GumpButtonType.Reply, 0);
            AddLabel(500, 568, 925, "Zamknij");
        }
    }
}