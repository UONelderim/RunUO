
namespace Server.Commands
{
    public static class ServUoSave
    {
       
        public static void Initialize()
        {
            CommandSystem.Register("ServUoSave", AccessLevel.GameMaster, new CommandEventHandler(ServUoSave_Command));
        }

        private static void ServUoSave_Command(CommandEventArgs e)
        {
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