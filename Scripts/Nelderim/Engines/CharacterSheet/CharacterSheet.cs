using Server;

namespace Nelderim.CharacterSheet
{
	class CharacterSheet : NExtension<CharacterSheetInfo>
	{
		public static string ModuleName = "CharacterSheet";

		public static void Initialize()
		{
			EventSink.WorldSave += new WorldSaveEventHandler( Save );
			Load( ModuleName );
		}

		public static void Save( WorldSaveEventArgs args )
		{
			Save( args, ModuleName );
		}
	}
}
