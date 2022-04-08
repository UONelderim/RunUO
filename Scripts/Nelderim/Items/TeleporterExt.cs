using System.Collections.Generic;
using Nelderim;

namespace Server.Items
{
    public partial class Teleporter
    {
        [CommandProperty( AccessLevel.GameMaster )]
        public bool AllowGhosts
        {
            get { return TeleporterExt.Get(this).AllowGhosts; }
            set { TeleporterExt.Get(this).AllowGhosts = value; }
        }
    }
	
    class TeleporterExt : NExtension<TeleporterExtInfo>
    {
        public static string ModuleName = "Teleporter";

        public static void Initialize()
        {
            EventSink.WorldSave += Save;
            Load(ModuleName);
        }

        public static void Save(WorldSaveEventArgs args)
        {
            Cleanup();
            Save(args, ModuleName);
        }

        private static void Cleanup()
        {
            List<Serial> toRemove = new List<Serial>();
            foreach ( KeyValuePair<Serial, TeleporterExtInfo> kvp in m_ExtensionInfo )
            {
                if ( World.FindItem(kvp.Key) == null )
                    toRemove.Add(kvp.Key);
            }
            foreach ( Serial serial in toRemove )
            {
                TeleporterExtInfo removed;
                m_ExtensionInfo.TryRemove( serial, out removed );
            }
        }
    }

    class TeleporterExtInfo : NExtensionInfo
    {
        private bool m_AllowGhosts;

        public bool AllowGhosts
        {
            get { return m_AllowGhosts; }
            set { m_AllowGhosts = value; }
        }

        public TeleporterExtInfo()
        {
            AllowGhosts = true;
        }

        public override void Serialize(GenericWriter writer)
        {
            writer.Write( (int)0 ); //version
            writer.Write(AllowGhosts);
        }

        public override void Deserialize(GenericReader reader)
        {
            int version = 0;
            if (Fix)
                version = reader.ReadInt(); //version
            AllowGhosts = reader.ReadBool();
        }
    }
}