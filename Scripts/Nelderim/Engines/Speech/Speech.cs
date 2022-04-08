using Server;
using System;
using System.Collections.Generic;
using System.IO;
using Nelderim;

namespace Nelderim.Speech
{
    class Speech : NExtension<SpeechInfo>
    {
		public static string ModuleName = "Speech";

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
