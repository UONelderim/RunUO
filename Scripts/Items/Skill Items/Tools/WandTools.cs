using System;
using Server;
using Server.Engines.Craft;

namespace Server.Items
{
    [Flipable(0x32F8, 0x32F8)]
	public class WandTools : BaseTool
	{
		public override CraftSystem CraftSystem{ get{ return DefWandCrafting.CraftSystem; } }

		[Constructable]
        public WandTools(): base(0x32F8)
		{
			Weight = 1.0;
            Name = "Narzedzia do tworzenia przedmiotow identyfikacji";
		}

		[Constructable]
        public WandTools(int uses): base(uses, 0x32F8)
		{
			Weight = 1.0;
            Name = "Narzedzia do tworzenia przedmiotow identyfikacji";
		}

		public WandTools( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}