using System;

namespace Server.Items
{
	public class ExecutionersCap : Item
	{
        [Constructable]
        public ExecutionersCap() : this(1)
        {
        }

		[Constructable]
		public ExecutionersCap(int amount) : base(0xF83)
		{
            Stackable = true;
			Weight = 1.0;
            Amount = amount;
		}

		public ExecutionersCap(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}
}