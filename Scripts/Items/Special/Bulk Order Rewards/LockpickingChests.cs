using System;
namespace Server.Items
{
    public class LockpickingChest : LockableContainer
    {
        private bool m_Locked;
        private Timer m_ResetTimer;

        [Constructable]
        public LockpickingChest() : base(0x9AA)
        {
            InitializeLockValues();
            StartTimer();
        }
        // Konstruktor do ręcznego ustawiania LockLevel i RequiredSkill
        public LockpickingChest(int lockLevel, int requiredSkill) : base(0x9AA)
        {
            Locked = true;
            LockLevel = lockLevel;
            RequiredSkill = requiredSkill;
            Weight = 4.0;

            StartTimer();
        }
        // Metoda inicjalizująca losowe wartości LockLevel i RequiredSkill
        private void InitializeLockValues()
        {
            Random random = new();

            Locked = true;
            LockLevel = random.Next(50, 91);  // Losuje LockLevel od 50 do 90
            RequiredSkill = random.Next(50, 91);  // Losuje RequiredSkill od 50 do 90
            Weight = 4.0;
        }
        // Metoda startująca timer
        private void StartTimer()
        {
            m_ResetTimer = Timer.DelayCall(TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(15), new TimerCallback(ResetLockValues));
        }
        // Metoda resetująca wartości po upływie czasu
        private void ResetLockValues()
        {
            InitializeLockValues();
            this.Locked = true;
        }
        public override void LockPick(Mobile from)
        {
            InitializeLockValues(); // Wywołuje losowanie wartości po otwarciu
            this.Locked = true;
            from.SendMessage("Skrzynia magicznie zamyka się");

            // Ponowne losowanie i zamykanie skrzyni co 15 sekund
            StartTimer();
        }
        public override void OnDelete()
        {
            // Usunięcie timera przy usuwaniu skrzyni
            if (m_ResetTimer != null)
                m_ResetTimer.Stop();
            base.OnDelete();
        }
        public LockpickingChest(Serial serial) : base(serial)
        {
            StartTimer();
        }
        public override void Serialize(GenericWriter writer)
        {
            base.Serialize(writer);

            writer.Write((int)0); // version
        }

        public override void Deserialize(GenericReader reader)
        {
            base.Deserialize(reader);

            int version = reader.ReadInt();
        }
    }
}

