
using System;
using System.Collections.Generic;
using System.Linq;
using Server.Items;

namespace Server.Commands
{
    public static class ServUoSave
    {
        private static readonly List<Type> itemsToDelete = new List<Type>
        {
            typeof(DesCityWallEast), typeof(DesCityWallSouth), typeof(kolczanwstyluzachodnim), typeof(kolczanwstylupolnocnym), typeof(PowerHourScroll)
        };

        private static readonly List<Type> mobilesToDelete = new List<Type>
        {
            typeof(IDamageableItem)
        };

       public static void Initialize()
        {
            CommandSystem.Register("ServUoSave", AccessLevel.GameMaster, new CommandEventHandler(ServUoSave_Command));
        }

        private static void ServUoSave_Command(CommandEventArgs e)
        {
            World.Items.All(x => itemsToDelete.Contains(x.GetType()));
            World.ServUOSave = true;
            try
            {
                Misc.AutoSave.Save();
            }
            finally
            {
                World.ServUOSave = false;
            }
        }
    }
}