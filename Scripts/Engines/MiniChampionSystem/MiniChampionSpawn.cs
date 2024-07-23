using Server.Gumps;
using Server.Mobiles;
using System;
using System.Collections.Generic;

namespace Server.Engines.MiniChamps
{
    public class MiniChamp : Item
    {
        private static readonly List<MiniChamp> Controllers = new List<MiniChamp>();

        private bool m_Active;
        private MiniChampType m_Type;
        private List<MiniChampSpawnInfo> Spawn;
        public List<Mobile> Despawns;
        private int m_Level;
        private int m_SpawnRange;
        private TimeSpan m_RestartDelay;

        [CommandProperty(AccessLevel.GameMaster)]
        public Point3D BossSpawnPoint { get; set; }

        [Constructable]
        public MiniChamp()
            : base(0xBD2)
        {
            Movable = false;
            Visible = false;
            Name = "Mini Champion Controller";

            Despawns = new List<Mobile>();
            Spawn = new List<MiniChampSpawnInfo>();
            m_RestartDelay = TimeSpan.FromMinutes(5.0);
            m_SpawnRange = 30;
            BossSpawnPoint = Point3D.Zero;

            Controllers.Add(this);
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int SpawnRange
        {
            get { return m_SpawnRange; }
            set { m_SpawnRange = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public TimeSpan RestartDelay
        {
            get { return m_RestartDelay; }
            set { m_RestartDelay = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public MiniChampType Type
        {
            get { return m_Type; }
            set { m_Type = value; InvalidateProperties(); }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public bool Active
        {
            get { return m_Active; }
            set
            {
                if (value)
                    Start();
                else
                    Stop();
            }
        }

        [CommandProperty(AccessLevel.GameMaster)]
        public int Level
        {
            get { return m_Level; }
            set { m_Level = value; InvalidateProperties(); }
        }

        public void Start()
        {
            if (m_Active || Deleted)
                return;

            m_Active = true;

            StartTimer();

            AdvanceLevel();
            InvalidateProperties();
        }

        public void Stop()
        {
            if (!m_Active || Deleted)
                return;

            m_Active = false;
            m_Level = 0;

            ClearSpawn();
            Despawn();

            MainTimer?.Stop();
            RestartTimer?.Stop();

            InvalidateProperties();
        }

        private Timer MainTimer;
        private Timer RestartTimer;

        private void StartTimer()
        {
            MainTimer = Timer.DelayCall(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), OnSlice);
        }

        private void StartRestartTimer()
        {
            RestartTimer = Timer.DelayCall(m_RestartDelay, Start);
        }

        public void Despawn()
        {
            foreach (Mobile toDespawn in Despawns)
            {
                toDespawn.Delete();
            }

            Despawns.Clear();
        }

        public void OnSlice()
        {
            if (!m_Active || Deleted)
                return;

            bool changed = false;
            bool done = true;

            foreach (MiniChampSpawnInfo spawn in Spawn)
            {
                if (spawn.Slice() && !changed)
                {
                    changed = true;
                }

                if (!spawn.Done && done)
                {
                    done = false;
                }
            }

            if (done)
            {
                AdvanceLevel();
            }

            if (m_Active)
            {
                foreach (MiniChampSpawnInfo spawn in Spawn)
                {
                    if (spawn.Respawn() && !changed)
                    {
                        changed = true;
                    }
                }
            }

            if (done || changed)
            {
                InvalidateProperties();
            }
        }

        public void ClearSpawn()
        {
            foreach (MiniChampSpawnInfo spawn in Spawn)
            {
                foreach (Mobile creature in spawn.Creatures)
                {
                    Despawns.Add(creature);
                }
            }

            Spawn.Clear();
        }

        public void AdvanceLevel()
        {
            Level++;

            MiniChampInfo info = MiniChampInfo.GetInfo(m_Type);
            MiniChampLevelInfo levelInfo = info.GetLevelInfo(Level);

            if (levelInfo != null && Level <= info.MaxLevel)
            {
                ClearSpawn();

                if (m_Type == MiniChampType.MeraktusTheTormented)
                {
                    MinotaurShouts();
                }

                foreach (MiniChampTypeInfo type in levelInfo.Types)
                {
                    Spawn.Add(new MiniChampSpawnInfo(this, type));
                }
            }
            else // begin restart
            {
                Stop();

                StartRestartTimer();
            }
        }

        private void MinotaurShouts()
        {
            int cliloc = 0;

            switch (Level)
            {
                case 1:
                    return;
                case 2:
                    cliloc = 1073370;
                    break;
                case 3:
                    cliloc = 1073367;
                    break;
                case 4:
                    cliloc = 1073368;
                    break;
                case 5:
                    cliloc = 1073369;
                    break;
            }

            IPooledEnumerable eable = GetMobilesInRange(m_SpawnRange);

            foreach (Mobile x in eable)
            {
                if (x is PlayerMobile)
                    x.SendLocalizedMessage(cliloc);
            }

            eable.Free();
        }

        public override void GetProperties(ObjectPropertyList list)
        {
            base.GetProperties(list);

            list.Add(1060658, "Type\t{0}", m_Type); // ~1_val~: ~2_val~
            list.Add(1060661, "Spawn Range\t{0}", m_SpawnRange); // ~1_val~: ~2_val~

            if (m_Active)
            {
                MiniChampInfo info = MiniChampInfo.GetInfo(m_Type);

                list.Add(1060742); // active
                list.Add("Level {0} / {1}", Level, info != null ? info.MaxLevel.ToString() : "???"); // ~1_val~: ~2_val~

                for (int i = 0; i < Spawn.Count; i++)
                {
                    Spawn[i].AddProperties(list, i + 1150301);
                }
            }
            else
            {
                list.Add(1060743); // inactive
            }
        }

        public override void OnDoubleClick(Mobile from)
        {
            from.SendGump(new PropertiesGump(from, this));
        }

        public override void OnDelete()
        {
            Controllers.Remove(this);
            Stop();

            base.OnDelete();
        }

        public MiniChamp(Serial serial)
            : base(serial)
        {
        }

        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write(0); // version

            writer.Write(BossSpawnPoint);
            writer.Write(m_Active);
            writer.Write((int)m_Type);
            writer.Write(m_Level);
            writer.Write(m_SpawnRange);
            writer.Write(m_RestartDelay);

            writer.Write(Spawn.Count);

            for (int i = 0; i < Spawn.Count; i++)
            {
                Spawn[i].Serialize(writer);
            }

            writer.Write(Despawns, true);
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();

            switch (version)
            {
                case 0:
                    {
                        Spawn = new List<MiniChampSpawnInfo>();

                        BossSpawnPoint = reader.ReadPoint3D();
                        m_Active = reader.ReadBool();
                        m_Type = (MiniChampType)reader.ReadInt();
                        m_Level = reader.ReadInt();
                        m_SpawnRange = reader.ReadInt();
                        m_RestartDelay = reader.ReadTimeSpan();

                        int spawnCount = reader.ReadInt();

                        for (int i = 0; i < spawnCount; i++)
                        {
                            Spawn.Add(new MiniChampSpawnInfo(reader));
                        }

                        Despawns = reader.ReadStrongMobileList();

                        if (m_Active)
                        {
                            StartTimer();
                        }
                        else
                        {
                            StartRestartTimer();
                        }

                        break;
                    }
            }

            Controllers.Add(this);
        }
    }
}
