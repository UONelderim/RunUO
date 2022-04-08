using System;
using Server.Items;
using Server.Network;
using Server.Engines.Craft;

namespace Server.Items
{
	[FlipableAttribute( 0xF43, 0xF44 )]
	public class Hatchet : BaseAxe, ICraftable
	{
		public override WeaponAbility PrimaryAbility{ get{ return WeaponAbility.ArmorIgnore; } }
		public override WeaponAbility SecondaryAbility{ get{ return WeaponAbility.Disarm; } }

		public override int AosStrengthReq{ get{ return 20; } }
		public override int AosMinDamage{ get{ return 13; } }
		public override int AosMaxDamage{ get{ return 15; } }
		public override int AosSpeed{ get{ return 41; } }

		public override int OldStrengthReq{ get{ return 15; } }
		public override int OldMinDamage{ get{ return 2; } }
		public override int OldMaxDamage{ get{ return 17; } }
		public override int OldSpeed{ get{ return 40; } }

		public override int InitMinHits{ get{ return 31; } }
		public override int InitMaxHits{ get{ return 80; } }

		[Constructable]
		public Hatchet() : base( 0xF43 )
		{
			Weight = 4.0;
		}

		public Hatchet( Serial serial ) : base( serial )
		{
		}

		// 06.10.2013 :: mortuus - Ilosc uzyc oraz kolor siekierki rowniez bedzie zalezec od materialu uzytego w ich produkcji
        public int OnCraft( int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, Type typeRes2,  BaseTool tool, CraftItem craftItem, int resHue )
        {
			Type resourceType = typeRes;
            if ( resourceType == null )
                resourceType = craftItem.Ressources.GetAt( 0 ).ItemType;
			
			ShowUsesRemaining = true;

			UnscaleDurability();
			UsesRemaining = (int) (UsesRemaining * BaseTool.getMaterialBonus(resourceType));
			ScaleDurability();
			return base.OnCraft( quality, makersMark, from, craftSystem, typeRes, typeRes2, tool, craftItem, resHue );
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