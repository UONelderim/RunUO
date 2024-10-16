

using Nelderim.CharacterSheet;
using Nelderim.Towns;
using Server.Mobiles;
using Server.Network;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Server.Items
{
	public class Gong : Item
	{
		[PropertyObject]
		public class Races
		{
			[CommandProperty(AccessLevel.GameMaster)]
			public bool Tamael { get; set; } = true;

			[CommandProperty(AccessLevel.GameMaster)]
			public bool Jarling { get; set; } = true;

			[CommandProperty(AccessLevel.GameMaster)]
			public bool Krasnolud { get; set; } = true;

			[CommandProperty(AccessLevel.GameMaster)]
			public bool Elf { get; set; } = true;

			[CommandProperty(AccessLevel.GameMaster)]
			public bool Drow { get; set; } = true;

			[CommandProperty(AccessLevel.GameMaster)]
			public bool OtherOrNone { get; set; } = true;

			public void Serialize(GenericWriter writer)
			{
				writer.Write((int)0); // version
				writer.Write((bool)Tamael);
				writer.Write((bool)Jarling);
				writer.Write((bool)Krasnolud);
				writer.Write((bool)Elf);
				writer.Write((bool)Drow);
				writer.Write((bool)OtherOrNone);
			}

			public void Deserialize(GenericReader reader)
			{
				int version = reader.ReadInt();
				Tamael = reader.ReadBool();
				Jarling = reader.ReadBool();
				Krasnolud = reader.ReadBool();
				Elf = reader.ReadBool();
				Drow = reader.ReadBool();
				OtherOrNone = reader.ReadBool();
			}

			public bool Matches(PlayerMobile pm)
			{
				var playerRace = pm.Race;
				if (playerRace == Server.Tamael.Instance)
				{
					return Tamael;
				}
				else if (playerRace == Server.Jarling.Instance)
				{
					return Jarling;
				}
				else if (playerRace == Server.Krasnolud.Instance)
				{
					return Krasnolud;
				}
				else if (playerRace == Server.Elf.Instance)
				{
					return Elf;
				}
				else if (playerRace == Server.Drow.Instance)
				{
					return Drow;
				}
				else
				{
					return OtherOrNone;
				}
			}
		}

		[PropertyObject]
		public class Citizienships
		{
			[CommandProperty(AccessLevel.GameMaster)]
			public bool None { get; set; } = true;

			[CommandProperty(AccessLevel.GameMaster)]
			public bool Tasandora { get; set; } = true;

			[CommandProperty(AccessLevel.GameMaster)]
			public bool Garlan { get; set; } = true;

			[CommandProperty(AccessLevel.GameMaster)]
			public bool LDelmah { get; set; } = true;

			[CommandProperty(AccessLevel.GameMaster)]
			public bool Lotharn { get; set; } = true;

			[CommandProperty(AccessLevel.GameMaster)]
			public bool Twierdza { get; set; } = true;

			public void Serialize(GenericWriter writer)
			{
				writer.Write((int)0); // version
				writer.Write((bool)None);
				writer.Write((bool)Tasandora);
				writer.Write((bool)Garlan);
				writer.Write((bool)LDelmah);
				writer.Write((bool)Lotharn);
				writer.Write((bool)Twierdza);
			}

			public void Deserialize(GenericReader reader)
			{
				int version = reader.ReadInt();
				None = reader.ReadBool();
				Tasandora = reader.ReadBool();
				Garlan = reader.ReadBool();
				LDelmah = reader.ReadBool();
				Lotharn = reader.ReadBool();
				Twierdza = reader.ReadBool();
			}

			public bool Matches(PlayerMobile pm)
			{
				if (TownDatabase.IsCitizenOfAnyTown(pm))
				{
					Towns playerCity = TownDatabase.IsCitizenOfWhichTown(pm);
					switch (playerCity)
					{
						case Towns.None: return None;
						case Towns.Tasandora: return Tasandora;
						case Towns.Garlan: return Garlan;
						case Towns.Twierdza: return Twierdza;
						case Towns.LDelmah: return LDelmah;
						case Towns.Lotharn: return Lotharn;
						default:
							{
								Console.WriteLine("WARNING: w klasie Gong nie ma przypisanej reakcji na rase " + playerCity.ToString());
								return false;
							}
					}
				}
				else
				{
					return None;
				}
			}
		}

		[PropertyObject]
		public class Areas
		{
			[CommandProperty(AccessLevel.GameMaster)]
			public bool GLOBAL { get; set; } = true;

			[CommandProperty(AccessLevel.GameMaster)]
			public bool Tasandora { get; set; }

			[CommandProperty(AccessLevel.GameMaster)]
			public bool Celendir { get; set; }

			[CommandProperty(AccessLevel.GameMaster)]
			public bool Talas { get; set; }

			[CommandProperty(AccessLevel.GameMaster)]
			public bool Tafroel { get; set; }

			[CommandProperty(AccessLevel.GameMaster)]
			public bool Ethrod { get; set; }

			[CommandProperty(AccessLevel.GameMaster)]
			public bool Ferion { get; set; }

			[CommandProperty(AccessLevel.GameMaster)]
			public bool Tingref { get; set; }

			[CommandProperty(AccessLevel.GameMaster)]
			public bool Uk { get; set; }

			[CommandProperty(AccessLevel.GameMaster)]
			public bool SnieznaPrzystan { get; set; }

			[CommandProperty(AccessLevel.GameMaster)]
			public bool SnieznaGarlan { get; set; }

			[CommandProperty(AccessLevel.GameMaster)]
			public bool Twierdza { get; set; }

			[CommandProperty(AccessLevel.GameMaster)]
			public bool Lotharn { get; set; }

			[CommandProperty(AccessLevel.GameMaster)]
			public bool LDelmah { get; set; }

			[CommandProperty(AccessLevel.GameMaster)]
			public bool NoamuthQuortek { get; set; }

			[CommandProperty(AccessLevel.GameMaster)]
			public bool Tirassa { get; set; }

			[CommandProperty(AccessLevel.GameMaster)]
			public bool Przemytnicy { get; set; }

			public void Serialize(GenericWriter writer)
			{
				writer.Write((int)0); // version
				writer.Write((bool)GLOBAL);
				writer.Write((bool)Tasandora);
				writer.Write((bool)Celendir);
				writer.Write((bool)Talas);
				writer.Write((bool)Tafroel);
				writer.Write((bool)Ethrod);
				writer.Write((bool)Ferion);
				writer.Write((bool)Tingref);
				writer.Write((bool)Uk);
				writer.Write((bool)SnieznaPrzystan);
				writer.Write((bool)SnieznaGarlan);
				writer.Write((bool)Twierdza);
				writer.Write((bool)Lotharn);
				writer.Write((bool)LDelmah);
				writer.Write((bool)NoamuthQuortek);
				writer.Write((bool)Tirassa);
				writer.Write((bool)Przemytnicy);
			}

			public void Deserialize(GenericReader reader)
			{
				int version = reader.ReadInt();
				GLOBAL = reader.ReadBool();
				Tasandora = reader.ReadBool();
				Celendir = reader.ReadBool();
				Talas = reader.ReadBool();
				Tafroel = reader.ReadBool();
				Ethrod = reader.ReadBool();
				Ferion = reader.ReadBool();
				Tingref = reader.ReadBool();
				Uk = reader.ReadBool();
				SnieznaPrzystan = reader.ReadBool();
				SnieznaGarlan = reader.ReadBool();
				Twierdza = reader.ReadBool();
				Lotharn = reader.ReadBool();
				LDelmah = reader.ReadBool();
				NoamuthQuortek = reader.ReadBool();
				Tirassa = reader.ReadBool();
				Przemytnicy = reader.ReadBool();
			}

			public bool Matches(PlayerMobile pm)
			{
				if (GLOBAL)
					return true;

				foreach (var region in pm.Map.Regions.Values)
				{
					if (region.Contains(pm.Location))
					{
						var r = region.ToString();
						if (Tasandora && (r == "Tasandora" || r == "Tasandora_Kopalnia" || r == "Twierdza_Kopalnia" || r == "Tasandora_Housing" || r == "SwiatyniaKonca_Krypty" || r == "Tasandora_Kanaly"))
							return true;
						if (Celendir && (r == "Celendir"))
							return true;
						if (Talas && (r == "Talas"))
							return true;
						if (Tafroel && (r == "Tafroel"))
							return true;
						if (Ethrod && (r == "Ethrod" || r == "Ethrod_Kopalnia"))
							return true;
						if (Ferion && (r == "Ferion"))
							return true;
						if (Tingref && (r == "Tingref"))
							return true;
						if (Uk && (r == "Uk"))
							return true;
						if (SnieznaPrzystan && (r == "SnieznaPrzystan"))
							return true;
						if (SnieznaGarlan && (r == "Garlan" || r == "Garlan_Kopalnia" || r == "Garlan_Housing"))
							return true;
						if (Twierdza && (r == "Twierdza" || r == "Twierdza_Housing" || r == "Twierdza_Dungeon"))
							return true;
						if (Lotharn && (r == "Lotharn" || r == "Enedh_Kopalnia"))
							return true;
						if (LDelmah && (r == "L'Delmah" || r == "NoamuthQuortek_Housing"))
							return true;
						if (NoamuthQuortek && (r == "NoamuthQuortek" || r == "NoamuthQuortek_Kopalnia" || r == "SwiatyniaLoethe" || r == "NoamuthQuortek_Housing" || r == "Drowy_Dungeon"))
							return true;
						if (Tirassa && (r == "Tirassa" || r == "Tirassa_Kopalnia"))
							return true;
						if (Przemytnicy && (r == "KryjowkaPrzemytnikow" || r == "JaskiniaPrzemytnikow" || r == "ZagubionaKopalnia"))
							return true;
					}
				}
				return false;
			}
		}

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Disabled { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public bool AnnounceOnlyGM { get; set; }

		[CommandProperty(AccessLevel.GameMaster)]
		public Races AnnounceRace { get; set; } = new();

		[CommandProperty(AccessLevel.GameMaster)]
		public Citizienships AnnounceCitizienship { get; set; } = new();

		[CommandProperty(AccessLevel.GameMaster)]
		public Races HearRace { get; set; } = new();

		[CommandProperty(AccessLevel.GameMaster)]
		public Citizienships HearCitizienship { get; set; } = new();

		[CommandProperty(AccessLevel.GameMaster)]
		public Areas HearRange { get; set; } = new();

		[CommandProperty(AccessLevel.GameMaster)]
		public string AnnouncedMessage { get; set; }

		private static string DefaultAnnounceMessage(PlayerMobile triggerPlayer) => "Z okolicy " + triggerPlayer.Map.ToString() + " " + triggerPlayer.Location.ToString() + " roznosi sie dzwiek czyjejs obecnosci.";

		[CommandProperty(AccessLevel.GameMaster)]
		public int AnnouncedMessageHue { get; set; } = 53;

		[CommandProperty(AccessLevel.GameMaster)]
		public string TriggerMessage { get; set; } = "Uzyles gongu rozglaszajac swiatu swoja obecnosc w tym miejscu.";

		[CommandProperty(AccessLevel.GameMaster)]
		public int LocalSound { get; set; } = -1;

		private DateTime m_LastUsage;

		private static TimeSpan m_Cooldown = TimeSpan.FromMinutes(5);

		[Constructable]
		public Gong() : base(0x1C12)
		{
			Name = "Gong";
			Label1 = "(Jego uzycie rozglosi swiatu twoja obecnosc w tym miejscu)";
			Movable = false;
		}

		public Gong(Serial serial)
			: base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int)0); // version

			writer.Write((bool)Disabled);
			writer.Write((bool)AnnounceOnlyGM);
			writer.Write((int)LocalSound);
			writer.Write((string)AnnouncedMessage);
			writer.Write((int)AnnouncedMessageHue);
			writer.Write((string)TriggerMessage);
			AnnounceRace.Serialize(writer);
			AnnounceCitizienship.Serialize(writer);
			HearRace.Serialize(writer);
			HearCitizienship.Serialize(writer);
			HearRange.Serialize(writer);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();

			Disabled = reader.ReadBool();
			AnnounceOnlyGM = reader.ReadBool();
			LocalSound = reader.ReadInt();
			AnnouncedMessage = reader.ReadString();
			AnnouncedMessageHue = reader.ReadInt();
			TriggerMessage = reader.ReadString();
			AnnounceRace.Deserialize(reader);
			AnnounceCitizienship.Deserialize(reader);
			HearRace.Deserialize(reader);
			HearCitizienship.Deserialize(reader);
			HearRange.Deserialize(reader);
		}

		public override void OnDoubleClick(Mobile from)
		{
			var pm = from as PlayerMobile;
			if (pm == null || Disabled)
				return;

			if (!pm.InRange(GetWorldLocation(), 2) || !pm.InLOS(this))
			{
				pm.SendLocalizedMessage(500446); // That is too far away.
				return;
			}

			if (AnnounceOnlyGM && pm.AccessLevel < AccessLevel.Counselor)
			{
				pm.SendMessage("Brak dostepu.");
				return;
			}

			if (!AnnounceRace.Matches(pm) || !AnnounceCitizienship.Matches(pm))
			{
				pm.SendMessage("To nie jest przeznaczone dla takich, jak ty!");
				return;
			}

			var cooldown = (pm.AccessLevel < AccessLevel.Counselor) ? m_Cooldown: TimeSpan.FromMilliseconds(Math.Min(m_Cooldown.TotalMilliseconds, TimeSpan.FromSeconds(5).TotalMilliseconds));
			if ( DateTime.Now - m_LastUsage  < cooldown)
			{
				pm.SendMessage("Trzeba chwile odczekac przed ponownym uzyciem tego przedmiotu.");
				return;
			}

			var announceText = (AnnouncedMessage == null || AnnouncedMessage == "") ? DefaultAnnounceMessage(pm) : AnnouncedMessage;

			if (TriggerMessage != null)
				pm.SendMessage(TriggerMessage);
			else
				pm.SendMessage("Uzyles gongu rozglaszajac swiatu swoja obecnosc w tym miejscu.");

			foreach (NetState ns in NetState.Instances)
			{
				PlayerMobile listener = ns.Mobile as PlayerMobile;
				if (listener != null && HearRace.Matches(listener) && HearCitizienship.Matches(listener) && HearRange.Matches(pm))
				{
					listener.SendMessage(AnnouncedMessageHue, announceText);
				}
			}

			m_LastUsage = DateTime.Now;
		}
	}
}