using System;
using Server;
using System.Collections;
using Server.Network;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Engines;
using Server.Engines.Harvest;



namespace Server.Items.Crops
{


	// WeedPlant: Rosnacy krzaczek lub surowiec - do zbierania.
	public abstract class WeedPlant : Item
	{
		public virtual string MsgCantBeMounted { get { return "Nie mozesz zabrac pzedmiotu bedac konno."; } }
		public virtual string MsgMustGetCloser { get { return "Musisz podejsc blizej, aby to zebrac."; } }
		public virtual string MsgPlantTooYoung { get { return "Przedmiot jest jeszcze gotowy do zabrania."; } }
		public virtual string MsgNoChanceToGet { get { return "Twoja wiedza o tym przedmiocie jest za mala, aby go zabrac."; } }
		public virtual string MsgSuccesfull { get { return "Udalo ci sie zebrac przedmiot."; } }
		public virtual string MsgGotSeed { get { return "Udalo ci sie zebrac szczepke rosliny!"; } }
		public virtual string MsgFailToGet { get { return "Nie udalo ci sie zebrac przedmiotu."; } }
		public virtual string MsgPlantDestroyed { get { return "Zniszczyles przedmiot."; } }

		private DateTime m_PlantedTime;
		private int m_GrowingTimeInSeconds; // czas osiagniecia dojarzlosci rosliny w sekundach (od posadzenia do mozliwosci zbioru)

		public virtual Type SeedType { get { return null; } }
        public virtual Type CropType { get { return null; } }

        [CommandProperty(AccessLevel.GameMaster)]
        public virtual bool GivesSeed { get { return false; } } // czy dany typ zielska daje sadzonki? (FALSE dla zbieractwa [regi nekro])

        // Ponizej cztery parametry decydujace o szansie na zebrani plonu z krzaka
        // Przykladowo: 0% przy 0 skilla,  100% przy 100 skilla
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual double HarvestMinSkill => 0.0;
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual double HarvestChanceAtMinSkill => 0.0;
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual double HarvestMaxSkill => 100.0;
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual double HarvestChanceAtMaxSkill => 100.0;
        
		// Ponizej tego poziomu skilla krzaczek nie bedzie niszczony przy probie zbioru (umozliwi to koks, oraz zapobiega zlosliwemu niszczeniu plonow przez postacie bez skilla):
        public virtual double DestroyAtSkill => 35;


        // Ponizej cztery parametry decydujace o szansie na pozyskanie szczepki.
        // Przykladowo: 10% przy 40 skilla,  30% przy 100 skilla
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual double SeedAcquireMinSkill => 40.0;
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual double SeedAcquireChanceAtMinSkill => 10.0;
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual double SeedAcquireMaxSkill => 100.0;
        [CommandProperty(AccessLevel.GameMaster)]
        public virtual double SeedAcquireChanceAtMaxSkill => 30.0;

        protected static SkillName[] defaultSkillsRequired = new SkillName[] { WeedHelper.MainWeedSkill };
        public virtual SkillName[] SkillsRequired { get { return defaultSkillsRequired; } }
        public override bool ForceShowProperties { get { return true; } }

        [CommandProperty(AccessLevel.GameMaster)]
		public DateTime PlantedTime
        {
			get { return m_PlantedTime; }
            set { m_PlantedTime = value; }
        }

        [CommandProperty(AccessLevel.GameMaster)]
		public int GrowingTimeInSeconds
		{
			get { return m_GrowingTimeInSeconds; }
			set { m_GrowingTimeInSeconds = value; }
		}

		public WeedPlant(int itemID) : base(itemID)
        {
            m_GrowingTimeInSeconds = 0;	// Dotyczy spawnowanych na mapie. Sadzone przez graczy maja ustawiany czas w metodzie klasy SeedPlant.

            m_PlantedTime = DateTime.MinValue;

			Movable = false;
		}

		public WeedPlant(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0); // version
			writer.Write((int)m_GrowingTimeInSeconds);
			writer.Write((double) 0); // deprecated: m_SkillMin
            writer.Write((double) 0); // deprecated: m_SkillMax
            writer.Write((double) 0); // deprecated: m_SkillDestroy
			// TODO: increment the version and get rid of the deprecated data
        }

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();

			// version 0:
			m_GrowingTimeInSeconds = reader.ReadInt();

			reader.ReadDouble(); // deprecated: m_SkillMin
            reader.ReadDouble(); // deprecated: m_SkillMax
            reader.ReadDouble(); // deprecated: m_SkillDestroy
            // TODO: increment the version and get rid of the deprecated data
        }

        // Funkcja determinujaca sukces w uzyskaniu szczepki podczas zbioru:
        private bool CheckSeedGain(Mobile from)
		{
			if (!GivesSeed)
				return false;

			return WeedHelper.CheckSkills(from, SkillsRequired, SeedAcquireMinSkill, SeedAcquireChanceAtMinSkill, SeedAcquireMaxSkill, SeedAcquireChanceAtMaxSkill);
		}

		public virtual int DefaultSeedCount(Mobile from)
		{
			return 1;
		}
        public virtual int DefaultCropCount(Mobile from)
		{
			double skill = WeedHelper.GetHighestSkillValue(from, SkillsRequired);
			return (int) Math.Round(skill / 100 * 12); 
		}

		public virtual bool CreateCrop(Mobile from)
        {
            return CreateItem(CropType, DefaultCropCount(from), from);
        }
		public virtual bool CreateSeed(Mobile from)
		{
			return CreateItem(SeedType, DefaultSeedCount(from), from);
        }
		private static bool CreateItem(Type type, int amount, Mobile m)
        {
			if (amount < 1)
			{
				return false;
            }
			if (type == null || !typeof(Item).IsAssignableFrom(type))
			{
				return false;
			}

            Item seed = Activator.CreateInstance(type) as Item;
            if (seed != null)
            {
                seed.Amount = amount;
                m.AddToBackpack(seed);
                return true;
            }

            return false;
        }

        public override void OnDoubleClick(Mobile from)
		{
			if (from == null || !from.Alive)
				return;

			if (!from.CanBeginAction(LockKind()))
			{
				from.SendLocalizedMessage(1070062); // Jestes zajety czyms innym
				return;
			}

			if (from.Mounted)
			{
				from.SendMessage(MsgCantBeMounted); // Nie mozesz zbierac surowcow bedac konno.
				return;
			}

			if (!from.InRange(this.GetWorldLocation(), 2) || !from.InLOS(this))
			{
				from.SendMessage(MsgMustGetCloser); // Musisz podejsc blizej, aby to zebrac.
				return;
			}

			if (m_PlantedTime.AddSeconds(m_GrowingTimeInSeconds) > DateTime.Now)
			{
				from.SendMessage(MsgPlantTooYoung); // Roslina jest jeszcze niedojrzala.
				return;
			}

			double skill = WeedHelper.GetHighestSkillValue(from, SkillsRequired);

			if (skill < HarvestMinSkill)
			{
				from.SendMessage(MsgNoChanceToGet); // Twoja wiedza o tym surowcu jest za mala, aby go zebrac.
				return;
			}

			// Zbieranie surowca:
			from.BeginAction(LockKind());
			from.RevealingAction();
			double AnimationDelayBeforeStart = 0.5;
			double AnimationIntervalBetween = 1.75;
			int AnimationNumberOfRepeat = 2;
			// Wpierw delay i animacja wewnatrz timera, a po ostatniej animacji timer uruchamia funkcje wyrywajaca ziolo (trzeci parametr):
			new WeedTimer(from, this, this.Animate, this.PullWeed, this.Unlock, TimeSpan.FromSeconds(AnimationDelayBeforeStart), TimeSpan.FromSeconds(AnimationIntervalBetween), AnimationNumberOfRepeat).Start();
		}

		// Jakiego typu czynnosci nie mozna wykonywac jednoczesnie ze zrywaniem ziol:
		public object LockKind()
		{
			return typeof(HarvestOrCraftLock);
		}

		public void Unlock(Mobile from)
		{
			from.EndAction(LockKind());
		}

		public bool Animate(Mobile from)
		{
			if (!from.InRange(this.GetWorldLocation(), 2))
			{
				from.SendMessage("Oddaliles sie.");
				return false;
			}
			from.Direction = from.GetDirectionTo(this);
			from.Animate(32, 5, 1, true, false, 0);
			return true;
		}

		public void PullWeed(Mobile from)
		{
			if (from == null || !from.Alive)
			{
				return;
				Unlock(from);
			}

			from.CheckSkill(WeedHelper.MainWeedSkill, HarvestMinSkill, HarvestMaxSkill); // koks zielarstwa na krzaczku

			if (WeedHelper.CheckSkills(from, SkillsRequired, HarvestMinSkill, HarvestChanceAtMinSkill, HarvestMaxSkill, HarvestChanceAtMaxSkill))
			{
				if (CreateCrop(from))
                    from.SendMessage(MsgSuccesfull);    // Udalo ci sie zebrac surowiec.

                if (CheckSeedGain(from))
				{
					if(CreateSeed(from))
						from.SendMessage(MsgGotSeed);   // Udalo ci sie zebrac szczepke rosliny!
                }

				this.Delete();
			}
			else
			{
				from.SendMessage(MsgFailToGet); // Nie udalo ci sie zebrac surowica.
				if (from.Skills[WeedHelper.MainWeedSkill].Value >= DestroyAtSkill)
				{
					// Usuwanie surowca z mapy w przypadku niepowodzenia:
					this.Delete();
					from.SendMessage(MsgPlantDestroyed);    // Zniszczyles surowiec.
				}
			}
			Unlock(from);
		}


	}

}