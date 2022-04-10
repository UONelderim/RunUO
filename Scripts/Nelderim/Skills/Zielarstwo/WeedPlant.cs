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
	public class WeedPlant : Item
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
		private int m_GrowingTime;          // czas osiagniecia dojarzlosci rosliny w sekundach (od posadzenia do mozliwosci zbioru)
											//private bool m_DisableSeed;			// pozwala zablokowac uzyskiwanie sadzonek z danego egzemplarza krzaczka
		private double m_SkillMin;          // Prog skilla umozliwiajacy zbior
		private double m_SkillMax;          // Prog skilla gwarantujacy udany zbior
		private double m_SkillDestroy;      // Prog skilla powodujacy niszczenie krzaczka podczas proby zbioru

		public virtual int SeedAmount { get { return 1; } }   // ilosc uzyskiwanych nasion
		public virtual bool GivesSeed { get { return false; } } // czy dany typ zielska daje sadzonki? (FALSE dla zbieractwa [regi nekro])

        public virtual int CropAmount(Mobile from)   // ilosc uzyskiwanego plon
		{
			double skill = WeedHelper.GetHighestSkillValue(from, SkillsRequired);
			return (int)Math.Round(skill / 100 * 12);
		}

        protected static SkillName[] defaultSkillsRequired = new SkillName[] { WeedHelper.MainWeedSkill };
        public virtual SkillName[] SkillsRequired { get { return defaultSkillsRequired; } }
        public override bool ForceShowProperties { get { return true; } }

		[CommandProperty(AccessLevel.GameMaster)]
		public int GrowingTime
		{
			get { return m_GrowingTime; }
			set { m_GrowingTime = value; }
		}

		/* DisableSeed
		[CommandProperty( AccessLevel.GameMaster )]
		public int DisableSeed
		{
			get{ return m_DisableSeed; }
			set{ m_DisableSeed = value; }
		}*/

		[CommandProperty(AccessLevel.GameMaster)]
		public double SkillMin
		{
			get { return m_SkillMin; }
			set { m_SkillMin = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public double SkillMax
		{
			get { return m_SkillMax; }
			set { m_SkillMax = value; }
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public double SkillDestroy
		{
			get { return m_SkillDestroy; }
			set { m_SkillDestroy = value; }
		}

		public WeedPlant(int itemID) : base(itemID)
		{
			m_PlantedTime = DateTime.Now;

			Movable = false;
		}

		public WeedPlant(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0); // version
			writer.Write((int)m_GrowingTime);
			//writer.Write( (bool) m_DisableSeed );
			writer.Write((double)m_SkillMin);
			writer.Write((double)m_SkillMax);
			writer.Write((double)m_SkillDestroy);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();

			// version 0:
			m_GrowingTime = reader.ReadInt();
			//m_DisableSeed = reader.ReadBool();
			m_SkillMin = reader.ReadDouble();
			m_SkillMax = reader.ReadDouble();
			m_SkillDestroy = reader.ReadDouble();

		}

		// Funkcja determinujaca sukces w uzyskaniu szczepki podczas zbioru:
		public virtual bool CheckSeedGain(Mobile from)
		{
			if (!GivesSeed /* || m_DisableSeed */ )
				return false;

			// 10% przy 40 skilla,  30% przy 100 skilla
			return WeedHelper.CheckSkills(from, SkillsRequired, 40, 10, 100, 30);
		}

		public virtual void CreateCrop(Mobile from, int count) { }
		public virtual void CreateSeed(Mobile from, int count) { }

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

			if (m_PlantedTime.AddSeconds(m_GrowingTime) > DateTime.Now)
			{
				from.SendMessage(MsgPlantTooYoung); // Roslina jest jeszcze niedojrzala.
				return;
			}

			double skill = WeedHelper.GetHighestSkillValue(from, SkillsRequired);

			if (skill < m_SkillMin)
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

			from.CheckSkill(WeedHelper.MainWeedSkill, m_SkillMin, m_SkillMax); // koks zielarstwa na krzaczku

			if (WeedHelper.CheckSkills(from, SkillsRequired, m_SkillMin, m_SkillMax))
			{
				from.SendMessage(MsgSuccesfull);    // Udalo ci sie zebrac surowiec.
				CreateCrop(from, CropAmount(from));

				if (CheckSeedGain(from))
				{
					from.SendMessage(MsgGotSeed);   // Udalo ci sie zebrac szczepke rosliny!
					CreateSeed(from, SeedAmount);
				}

				this.Delete();
			}
			else
			{
				from.SendMessage(MsgFailToGet); // Nie udalo ci sie zebrac surowica.
				if (from.Skills[WeedHelper.MainWeedSkill].Value >= m_SkillDestroy)
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