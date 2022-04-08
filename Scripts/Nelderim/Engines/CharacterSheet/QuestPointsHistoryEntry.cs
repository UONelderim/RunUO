using System;
using Server;

namespace Nelderim.CharacterSheet
{
    public class QuestPointsHistoryEntry : IComparable<QuestPointsHistoryEntry>
    {
        private DateTime m_DateTime;
        private string m_GameMaster;
        private int m_Change;
        private string m_Reason;

        public QuestPointsHistoryEntry(DateTime dateTime, string gameMaster, int change, string reason)
        {
            m_DateTime = dateTime;
            m_GameMaster = gameMaster;
            m_Change = change;
            m_Reason = reason;
        }

        public QuestPointsHistoryEntry( GenericReader reader)
        {
            int version = reader.ReadInt();
            m_DateTime = reader.ReadDateTime();
            m_GameMaster = reader.ReadString();
            m_Change = reader.ReadInt();
            m_Reason = reader.ReadString();
        }

        public DateTime DateTime
        {
            get { return m_DateTime; }
            set { m_DateTime = value; }
        }

        public string GameMaster
        {
            get { return m_GameMaster; }
            set { m_GameMaster = value; }
        }

        public int Change
        {
            get { return m_Change; }
            set { m_Change = value; }
        }

        public string Reason
        {
            get { return m_Reason; }
            set { m_Reason = value; }
        }


        public int CompareTo(QuestPointsHistoryEntry other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return m_DateTime.CompareTo(other.m_DateTime);
        }

        public void Serialize(GenericWriter writer)
        {
            writer.Write((int)0); //version
            writer.Write(m_DateTime);
            writer.Write(m_GameMaster);
            writer.Write(m_Change);
            writer.Write(m_Reason);
        }
    }
}