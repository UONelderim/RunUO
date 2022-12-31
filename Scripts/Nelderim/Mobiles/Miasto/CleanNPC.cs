#region References

using System.Collections;
using System.Collections.Generic;

#endregion


namespace Server.Mobiles
{
	public class CleanNPC : BaseVendor
	{
		[Constructable]
		public CleanNPC() : base("")
		{
		}

		public override void InitOutfit()
		{
		}

		public CleanNPC(Serial serial) : base(serial)
		{
		}

		protected override ArrayList SBInfos
		{
			get { return new ArrayList(); }
		}

		public override void InitSBInfo()
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
