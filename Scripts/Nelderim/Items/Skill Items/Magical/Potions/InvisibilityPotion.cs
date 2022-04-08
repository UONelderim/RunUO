using System;
using Server.Network;
using Server;
using System.Collections;

namespace Server.Items
{
	public class InvisibilityPotion : BasePotion
	{
		public TimeSpan Cooldown { get { return TimeSpan.FromMinutes(5.0); }  }

		[Constructable]
		public InvisibilityPotion() : base(0xF0B, PotionEffect.Invisibility) {
			Weight = 0.5;
			Movable = true;
			Hue = 553;
			Name = "Mikstura Niewidzialnosci";
		}

		public InvisibilityPotion(Serial serial) : base(serial) {
		}

		public override void Serialize(GenericWriter writer) {
			base.Serialize(writer);

			writer.Write((int)0); // version
		}

		public override void Deserialize(GenericReader reader) {
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}

		public override void Drink(Mobile from) {
			if (from.BeginAction(typeof(InvisibilityPotion))) {
				if (from.InRange(this.GetWorldLocation(), 1)) {
					from.Hidden = true;
					this.Consume();
					from.AddToBackpack(new Bottle());

					Timer.DelayCall(Cooldown, new TimerStateCallback(Cooldown_Callback), from);
				} else {
					from.LocalOverheadMessage(MessageType.Regular, 906, 1019045); // I can't reach that. 
				}
			} else {
				from.SendMessage("Musisz poczekac zanim uzyjesz tej mikstury ponownie");
			}
		}

		private static void Cooldown_Callback(object state) {
			((Mobile)state).EndAction(typeof(InvisibilityPotion));
		}
	}
}
