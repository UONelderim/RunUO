using Server;
using System;
using System.Collections.Generic;

namespace Nelderim.CharacterSheet
{
    public enum EventFrequency
    {
        F30M,
        F60M,
        F90M,
        FLonger
    }

    public enum CrestSize
    {
        Small,
        Medium,
        Large
    }
    class CharacterSheetInfo : NExtensionInfo
    {
        private int m_Crest;
        private CrestSize m_CrestSize;
        private string m_AppearanceAndCharacteristic;
        private string m_HistoryAndProfession;
        private bool m_AttendenceInEvents;
        private EventFrequency m_EventFrequencyAttendence;
        private int m_QuestPoints;
        private DateTime m_LastQuestPointsTime;
        private SortedSet<QuestPointsHistoryEntry> m_QuestPointsHistoryEntries;

        public int Crest { get { return m_Crest; } set { m_Crest = value; } }
        public CrestSize CrestSize { get { return m_CrestSize; } set { m_CrestSize = value; } }
        public string AppearanceAndCharacteristic { get { return m_AppearanceAndCharacteristic; } set { m_AppearanceAndCharacteristic = value; } }
        public string HistoryAndProfession { get { return m_HistoryAndProfession; } set { m_HistoryAndProfession = value; } }
        public bool AttendenceInEvents { get { return m_AttendenceInEvents; } set { m_AttendenceInEvents = value; } }
        public EventFrequency EventFrequencyAttendence { get { return m_EventFrequencyAttendence; } set { m_EventFrequencyAttendence = value; } }
        public int QuestPoints { get { return m_QuestPoints; } set { m_QuestPoints = value; } }
        public DateTime LastQuestPointsTime { get { return m_LastQuestPointsTime; } set { m_LastQuestPointsTime = value; } }

        public SortedSet<QuestPointsHistoryEntry> QuestPointsHistoryEntries
        {
            get
            {
                if (m_QuestPointsHistoryEntries == null)
                    m_QuestPointsHistoryEntries = new SortedSet<QuestPointsHistoryEntry>();
                return m_QuestPointsHistoryEntries;
            }
            set { m_QuestPointsHistoryEntries = value; }
        }


        public override void Serialize( GenericWriter writer )
        {
            writer.Write( (int)1 ); //version
            writer.Write( m_Crest );
            writer.Write( (int)m_CrestSize );
            writer.Write( m_AppearanceAndCharacteristic );
            writer.Write( m_HistoryAndProfession );
            writer.Write( m_AttendenceInEvents );
            writer.Write( (int)m_EventFrequencyAttendence );
            writer.Write( m_QuestPoints );
            writer.Write( m_LastQuestPointsTime );
            writer.Write(QuestPointsHistoryEntries.Count);
            foreach (var questPointsHistoryEntry in QuestPointsHistoryEntries)
            {
                questPointsHistoryEntry.Serialize(writer);
            }
        }

        public override void Deserialize( GenericReader reader )
        {
            int version = 0;
            if (Fix)
                version = reader.ReadInt(); //version
            m_Crest = reader.ReadInt();
            m_CrestSize = (CrestSize)reader.ReadInt();
            m_AppearanceAndCharacteristic = reader.ReadString();
            m_HistoryAndProfession = reader.ReadString();
            m_AttendenceInEvents = reader.ReadBool();
            m_EventFrequencyAttendence = (EventFrequency)reader.ReadInt();
            m_QuestPoints = reader.ReadInt();
            m_LastQuestPointsTime = reader.ReadDateTime();
            if (version > 0)
            {
                var questPointsHistoryEntryCount = reader.ReadInt();
                m_QuestPointsHistoryEntries = new SortedSet<QuestPointsHistoryEntry>();
                for (int i = 0; i < questPointsHistoryEntryCount; i++)
                {
                    m_QuestPointsHistoryEntries.Add(new QuestPointsHistoryEntry(reader));
                }
            }
        }
    }
}