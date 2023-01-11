
using System;
using System.Collections.Generic;
using System.Linq;
using Server.Items;
using Server.Mobiles;

namespace Server.Commands
{
    public static class ServUoSave
    {
        private static readonly List<Type> itemsToDelete = new List<Type>
        {
            typeof(DesCityWallEast), typeof(DesCityWallSouth), typeof(kolczanwstyluzachodnim), typeof(kolczanwstylupolnocnym), 
            typeof(PowerHourScroll), typeof(TrashBarrel), typeof(FishingPole), typeof(RewardScroll), typeof(SheafOfHay), 
            typeof(ArcaneFocus)
        };

        private static readonly List<Type> mobilesToDelete = new List<Type>
        {
            typeof(IDamageableItem), typeof(PirateCaptain), typeof(Ninja), typeof(MasterMikael), typeof(Putrefier)
        };

       public static void Initialize()
        {
            CommandSystem.Register("ServUoSave", AccessLevel.GameMaster, new CommandEventHandler(ServUoSave_Command));
        }

        private static void ServUoSave_Command(CommandEventArgs e)
        {
            List<IEntity> toDelete = new List<IEntity>();
            foreach (var item in World.Items.Values.Where(x => itemsToDelete.Contains(x.GetType())))
            {
                toDelete.Add(item);
            }
            foreach (var mobile in World.Mobiles.Values.Where(x => mobilesToDelete.Contains(x.GetType())))
            {
                toDelete.Add(mobile);
            }
            foreach (var entity in toDelete)
            {
                entity.Delete();
            }
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