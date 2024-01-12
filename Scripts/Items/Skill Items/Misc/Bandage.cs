using System;
using System.Collections.Generic;
using Server;
using Server.Gumps;
using Server.Items;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Items
{
	public class Bandage : Item, IDyable
	{
		public static int Range = (Core.AOS ? 2 : 1);

		public override double DefaultWeight
		{
			get { return 0.1; }
		}

		[Constructable]
		public Bandage()
			: this(1)
		{
		}

		[Constructable]
		public Bandage(int amount)
			: base(0xE21)
		{
			Stackable = true;
			Amount = amount;
		}

		public Bandage(Serial serial)
			: base(serial)
		{
		}

		public virtual bool Dye(Mobile from, DyeTub sender)
		{
			if (Deleted)
				return false;

			Hue = sender.DyedHue;

			return true;
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

		public override void OnDoubleClick(Mobile from)
		{
			if (from.InRange(GetWorldLocation(), Range))
			{
				from.RevealingAction();

				from.SendLocalizedMessage(500948); // Who will you use the bandages on?

				from.Target = new InternalTarget(this);
			}
			else
			{
				from.SendLocalizedMessage(500295); // You are too far away to do that.
			}
		}

		private class InternalTarget : Target
		{
			private Bandage m_Bandage;

			public InternalTarget(Bandage bandage)
				: base(Bandage.Range, false, TargetFlags.Beneficial)
			{
				m_Bandage = bandage;
			}

			protected override void OnTarget(Mobile from, object targeted)
			{
				if (m_Bandage.Deleted)
					return;

				if (targeted is Mobile)
				{
					if (from.InRange(m_Bandage.GetWorldLocation(), Bandage.Range))
					{
						if (BandageContext.BeginHeal(from, (Mobile)targeted) != null)
						{
							m_Bandage.Consume();
						}
					}
					else
					{
						from.SendLocalizedMessage(500295); // You are too far away to do that.
					}
				}
				else
				{
					from.SendLocalizedMessage(500970); // Bandages can not be used on that.
				}
			}
		}
	}

	public class BandageContext
	{
		private Mobile m_Healer;
		private Mobile m_Patient;
		private int m_Slips;
		private Timer m_Timer;

		public Mobile Healer { get { return m_Healer; } }
		public Mobile Patient { get { return m_Patient; } }
		public int Slips { get { return m_Slips; } set { m_Slips = value; } }
		public Timer Timer { get { return m_Timer; } }

		public void Slip()
		{
			m_Healer.SendLocalizedMessage(500961); // Your fingers slip!
			++m_Slips;
		}

		public BandageContext(Mobile healer, Mobile patient, TimeSpan delay)
		{
			m_Healer = healer;
			m_Patient = patient;

			m_Timer = new InternalTimer(this, delay);
			m_Timer.Start();
		}

		public void StopHeal()
		{
			m_Table.Remove(m_Healer);

			if (m_Timer != null)
				m_Timer.Stop();

			m_Timer = null;
		}

		private static Dictionary<Mobile, BandageContext> m_Table = new Dictionary<Mobile, BandageContext>();

		public static BandageContext GetContext(Mobile healer)
		{
			BandageContext bc = null;
			m_Table.TryGetValue(healer, out bc);
			return bc;
		}

		public static SkillName GetPrimarySkill(Mobile m)
		{
			if (!m.Player && (m.Body.IsMonster || m.Body.IsAnimal))
				return SkillName.Veterinary;
			else
				return SkillName.Healing;
		}

		public static SkillName GetSecondarySkill(Mobile m)
		{
			if (!m.Player && (m.Body.IsMonster || m.Body.IsAnimal))
				return SkillName.AnimalLore;
			else
				return SkillName.Anatomy;
		}

		
		public static bool AllowPetRessurection(Mobile healer, BaseCreature petPatient)
		{
			return AllowPetRessurection(healer, petPatient, true);
		}

		public static bool AllowPetRessurection(Mobile healer, BaseCreature petPatient, bool gump)
		{
			Mobile master = petPatient.ControlMaster;

			if (master != null && master.InRange(petPatient, 3))
			{
				if (gump)
				{
					healer.SendLocalizedMessage(503255, "", 0x502);//Udalo Ci sie wskrzesic tego zwierzecia!

					master.CloseGump(typeof(PetResurrectGump));
					master.SendGump(new PetResurrectGump(healer, petPatient));
				}

				return true;
			}
			else
			{
				List<Mobile> friends = petPatient.Friends;

				for (int i = 0; friends != null && i < friends.Count; ++i)
				{
					Mobile friend = friends[i];

					if (friend.InRange(petPatient, 3))
					{
						if (gump)
						{
							healer.SendLocalizedMessage(503255, "", 0x502);//Udalo Ci sie wskrzesic tego zwierzecia!

							friend.CloseGump(typeof(PetResurrectGump));
							friend.SendGump(new PetResurrectGump(healer, petPatient));
						}

						return true;
					}
				}

				return false;
			}
		}

		public void EndHeal()
		{
			StopHeal();

			int healerNumber = -1, patientNumber = -1;
			bool playSound = true;
			bool checkSkills = false;

			if (0.75 > Utility.RandomDouble() && Config.BleedBandages)
			{
				m_Healer.AddToBackpack(new ZakrwawioneBandaze(1));
				m_Healer.SendMessage("Udalo sie odzyskac bandaz.", 100);
			}

			SkillName primarySkill = GetPrimarySkill(m_Patient);
			SkillName secondarySkill = GetSecondarySkill(m_Patient);

			BaseCreature petPatient = m_Patient as BaseCreature;

			if (!m_Healer.Alive)
			{
				m_Healer.SendLocalizedMessage(500962, "", 38);  // You were unable to finish your work before you died.
				patientNumber = -1;
				playSound = false;
			}
			else if (!m_Healer.InRange(m_Patient, Bandage.Range))
			{
				m_Healer.SendLocalizedMessage(500963, "", 50); // You did not stay close enough to heal your target.
				patientNumber = -1;
				playSound = false;
			}
			else if (!m_Patient.Alive || (petPatient != null && petPatient.IsDeadPet)) //Wskrzeszanie graczy i zwierzat
			{
				double healing = m_Healer.Skills[primarySkill].Value;
				double anatomy = m_Healer.Skills[secondarySkill].Value;
				double chance = ((healing - 68.0) / 50.0) - (m_Slips * 0.02);

				if (((checkSkills = (healing >= 80.0 && anatomy >= 80.0)) && chance > Utility.RandomDouble()) || (Core.SE && petPatient is Factions.FactionWarHorse && petPatient.ControlMaster == m_Healer))	//TODO: Dbl check doesn't check for faction of the horse here?
				{
					if (m_Patient.Map == null || !m_Patient.Map.CanFit(m_Patient.Location, 16, false, false))
					{
						m_Healer.SendLocalizedMessage(501042, "", 0x28);//Nie mozesz wskrzesic tego pacjenta w tym miejscu!
						m_Patient.SendLocalizedMessage(502391, "", 0x28);//Nie mozesz byc tutaj wskrzeszony!
					}
					/*else if ( m_Patient.Region != null && m_Patient.Region.IsPartOf( "Khaldun" ) )
					{
						healerNumber = 1010395; // The veil of death in this area is too strong and resists thy efforts to restore life.
						patientNumber = -1;
					}*/
					else
					{
						m_Healer.SendLocalizedMessage(500965, "", 0x502);//Udalo Ci sie wskrzesic pacjenta!
						patientNumber = -1;

						m_Patient.PlaySound(0x214);
						m_Patient.FixedEffect(0x376A, 10, 16);

						if (petPatient != null && petPatient.IsDeadPet)
						{
							if( AllowPetRessurection(m_Healer, petPatient) )
								healerNumber = 1049670; // Wlasciciel stworzenia misi byc w poblizu aby wskrzesic te stworzenie.
						}
						else
						{
							m_Patient.CloseGump(typeof(ResurrectGump));
							m_Patient.SendGump(new ResurrectGump(m_Patient, m_Healer));
						}
					}
				}
				else
				{
					if (petPatient != null && petPatient.IsDeadPet)
						m_Healer.SendLocalizedMessage(503256, "", 0x28);//Nie udalo Ci sie wskrzesic tego zwierzaka!
					else
						m_Healer.SendLocalizedMessage(500966, "", 0x28);//Nie udalo Ci sie wskrzesic Twojego pacjenta!

					patientNumber = -1;
				}
			}
			else if (m_Patient.Poisoned) // Leczenie podczas zatrucia
			{
				m_Healer.SendLocalizedMessage(500969, "", 0x4F7); // Zakonczyles leczenie sukcesem.

				double healing = m_Healer.Skills[primarySkill].Value;
				double anatomy = m_Healer.Skills[secondarySkill].Value;
				double chance = ((healing - 30.0) / 50.0) - (m_Patient.Poison.Level * 0.1) - (m_Slips * 0.02);

				if ((checkSkills = (healing >= 60.0 && anatomy >= 60.0)) && chance > Utility.RandomDouble())
				{
					if (m_Patient.CurePoison(m_Healer))
					{
						healerNumber = (m_Healer == m_Patient) ? -1 : 1010058; // You have cured the target of all poisons.
						m_Patient.SendLocalizedMessage(1010059, "", 2128);// Trucizna zostala z powodzeniem usunieta z ciala.
					}
					else
					{
						healerNumber = -1;
						patientNumber = -1;
					}
				}
				else
				{
					m_Healer.SendLocalizedMessage(1010060, "", 38);// Nie udalo Ci sie usunac trucizny z ciala!
					patientNumber = -1;
				}
			}
			else if (BleedAttack.IsBleeding(m_Patient))
			{
				m_Healer.SendLocalizedMessage(1060088, "", 2128);// You bind the wound and stop the bleeding
				m_Patient.SendLocalizedMessage(1060167, "", 2128);// Udalo Ci sie wyleczyc z Krwawienia.

				BleedAttack.EndBleed(m_Patient, true);
			}
			else if (MortalStrike.IsWounded(m_Patient))
			{
				healerNumber = (m_Healer == m_Patient ? 1005000 : 1010398);
				patientNumber = -1;
				playSound = false;
			}
			else if (m_Patient.Hits == m_Patient.HitsMax)
			{
				m_Healer.SendLocalizedMessage(500967, "", 0x502);//Uleczyles troszke obrazen.
				patientNumber = -1;
			}
			else //Zwykle leczenie
			{
				checkSkills = true;
				patientNumber = -1;

				double healing = m_Healer.Skills[primarySkill].Value;
				double anatomy = m_Healer.Skills[secondarySkill].Value;
				double chance = ((healing + 10.0) / 100.0) - (m_Slips * 0.02);

				if (chance > Utility.RandomDouble())
				{
					m_Healer.SendLocalizedMessage(500969, "", 0x4F7);//Zakonczyles leczenie sukcesem.

					double min, max;

					if (Core.AOS)
					{
						min = (anatomy / 8.0) + (healing / 5.0) + 4.0;
						max = (anatomy / 6.0) + (healing / 2.5) + 4.0;
					}
					else
					{
						min = (anatomy / 5.0) + (healing / 5.0) + 3.0;
						max = (anatomy / 5.0) + (healing / 2.0) + 10.0;
					}

					double toHeal = min + (Utility.RandomDouble() * (max - min));

					if (m_Patient.Body.IsMonster || m_Patient.Body.IsAnimal)
						toHeal += m_Patient.HitsMax / 100;

					if (Core.AOS)
						toHeal -= toHeal * m_Slips * 0.35; // TODO: Verify algorithm
					else
						toHeal -= m_Slips * 4;

					if (toHeal < 1)
					{
						toHeal = 1;
						m_Healer.SendLocalizedMessage(500968, "", 38);//Nie ukonczyles w pelni leczenia, udalo Ci sie uleczyc tylko czesc ran!
					}

					m_Patient.Heal((int)toHeal, m_Healer, true);
					//m_Patient.Heal( (int) toHeal );
				}
				else
				{
					m_Healer.SendLocalizedMessage(500968, "", 38);//Nie ukonczyles w pelni leczenia, udalo Ci sie uleczyc tylko czesc ran!
					playSound = false;
				}
			}

			if (healerNumber != -1)
				m_Healer.SendLocalizedMessage(healerNumber);

			if (patientNumber != -1)
				m_Patient.SendLocalizedMessage(patientNumber);

			if (playSound)
				m_Patient.PlaySound(0x57);

			if (checkSkills)
			{
				m_Healer.CheckSkill(secondarySkill, 0.0, 120.0);
				m_Healer.CheckSkill(primarySkill, 0.0, 120.0);
			}
		}

		private class InternalTimer : Timer
		{
			private BandageContext m_Context;

			public InternalTimer(BandageContext context, TimeSpan delay)
				: base(delay)
			{
				m_Context = context;
				Priority = TimerPriority.FiftyMS;
			}

			protected override void OnTick()
			{
				m_Context.EndHeal();
			}
		}

		public static BandageContext BeginHeal(Mobile healer, Mobile patient)
		{
			bool isDeadPet = (patient is BaseCreature && ((BaseCreature)patient).IsDeadPet);

			if (patient is Golem)
			{
				healer.SendLocalizedMessage(500970, "", 0x4F7); // Nie mozesz na tym uzyc bandazy!
			}
			else if (patient is BaseCreature && (!((BaseCreature)patient).Tamable || ((BaseCreature)patient).IsAnimatedDead))
			{
				healer.SendLocalizedMessage(500951, "", 0x28); // Nie mozesz tego wyleczyc.
			}
			else if (!patient.Poisoned && patient.Hits == patient.HitsMax && !BleedAttack.IsBleeding(patient) && !isDeadPet)
			{
				healer.SendLocalizedMessage(500955, "", 0x4F7); // Nie ma zadnych obrazen ktore mozna bylo by uleczyc!
			}
			else if (!patient.Alive && (patient.Map == null || !patient.Map.CanFit(patient.Location, 16, false, false)))
			{
				healer.SendLocalizedMessage(501042, "", 0x28); // Nie mozesz wskrzesic tego pacjenta w tym miejscu!
			}
			else if (healer.CanBeBeneficial(patient, true, true))
			{
				healer.DoBeneficial(patient);

				bool onSelf = (healer == patient);
				int dex = healer.Dex;

				double seconds;
				double resDelay = (patient.Alive ? 0.0 : 5.0);

				if (onSelf)
				{
					if (Core.AOS)
					{
						if ((healer.Dex) > 159)
							seconds = 3.0;
						else
							seconds = 5.0 + (0.5 * ((double)(120 - dex) / 10)); // TODO: Verify algorithm
					}
					else
					{

						if ((healer.Dex) > 159)
							seconds = 3.0;
						else
							seconds = 9.4 + (0.6 * ((double)(120 - dex) / 10));
					}
				}
				else
				{
					if (Core.AOS && GetPrimarySkill(patient) == SkillName.Veterinary)
					{
						seconds = 2.0;

						patient.RevealingAction();
                    }
					else
					{
						if (dex >= 100)
							seconds = 3.0 + resDelay;
						else if (dex >= 40)
							seconds = 4.0 + resDelay;
						else
							seconds = 5.0 + resDelay;
					}
				}

				BandageContext context = GetContext(healer);

				if (context != null)
					context.StopHeal();
				seconds *= 1000;

				context = new BandageContext(healer, patient, TimeSpan.FromMilliseconds(seconds));

				m_Table[healer] = context;

				if (!onSelf)
					patient.SendLocalizedMessage(1008078, false, healer.Name); //  : Attempting to heal you.

				healer.SendLocalizedMessage(500956, "", 0x501); // Rozpoczales leczenie...
				return context;
			}

			return null;
		}
	}
}