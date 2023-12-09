#region References

using System.Collections.Generic;

#endregion

namespace Server.Mobiles
{
	public class Druid : NBaseTalkingNPC
	{
		private static readonly Dictionary<Race, List<Action>> _Actions = new Dictionary<Race, List<Action>>
		{
			{
				Race.DefaultRace, new List<Action>
				{
					m => m.Say("Chwalmy Matkę!"),
					m => m.Say("Kochajmy Naturę"),
					m => m.Say("Psia krew..."),
					m => m.Say("Miłość do lasów i gór..."),
					m => m.Say("Miłego dnia!"),
					m => m.Say("Na Naneth! Jakie piękne to miejsce!"),
					m => m.Say("Nie powinienem mieszać tylu ziół."),
					m => m.Say("To kiedy to zebranie Kręgu?"),
					m => m.Say("Tulia to jednak wie jak zarządzać kręgiem!"),
					m => m.Say("Powrót Elfów do Lotharn, to dobry znak. Może i znajdzie się tam miejsce i dla nas..."),
				}
			}
		};

		protected override Dictionary<Race, List<Action>> NpcActions
		{
			get { return _Actions; }
		}

		[Constructable]
		public Druid() : base("- Druid")
		{
		}

		public override void OnGenderChanged(bool oldFemale)
		{
			base.OnGenderChanged(oldFemale);
			if (Female)
			{
				Title = "- Druidka";
			}
			else
			{
				Title = "- Druid";
			}
		}

		public Druid(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
		}
	}
}
