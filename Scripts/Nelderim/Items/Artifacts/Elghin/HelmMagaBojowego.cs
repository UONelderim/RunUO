using System;
using Server;

namespace Server.Items
{
	[FlipableAttribute( 0x171C, 0x171C )]
	public class HelmMagaBojowego : LeatherCap
	{
        
        public override int InitMinHits { get { return 60; } }
        public override int InitMaxHits { get { return 60; } }

		public override int BasePhysicalResistance { get { return 10; } }
		public override int BaseFireResistance { get { return 5; } }
		public override int BaseColdResistance { get { return 5; } }
		public override int BasePoisonResistance { get { return -10; } }
		public override int BaseEnergyResistance { get { return 5; } }

		public override int AosStrReq { get { return 65; } }
		public override int OldStrReq { get { return 15; } }

		public override int ArmorBase { get { return 13; } }

		public override ArmorMaterialType MaterialType { get { return ArmorMaterialType.Leather; } }
		public override CraftResource DefaultResource { get { return CraftResource.RegularLeather; } }

		public override ArmorMeditationAllowance DefMedAllowance { get { return ArmorMeditationAllowance.All; } }

		[Constructable]
		public HelmMagaBojowego()
		{
			Hue = 942;
            Name = "Helm maga bojowego";
			Attributes.CastSpeed = 1;
			Attributes.RegenHits = 2;
            Attributes.ReflectPhysical = 5;
			Weight = 2.0;
            LootType = LootType.Cursed;
		}

		public HelmMagaBojowego( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			if ( Weight == 1.0 )
			{
				Weight = 2.0;
			}
		}
	}
}
