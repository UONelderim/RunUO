using System;
using Server;

namespace Server.Items
{
	public class AlchemistsBauble : GoldBracelet
	{
        public override int LabelNumber { get { return 1070638; } } // Bombka alchemika
        public override int InitMinHits { get { return 45; } }
        public override int InitMaxHits { get { return 45; } }

		[Constructable]
		public AlchemistsBauble()
		{
			Hue = 0x290;
			SkillBonuses.SetValues( 0, SkillName.Magery, 10.0 );
			Attributes.EnhancePotions = 30;
			Attributes.DefendChance = 10;
			Attributes.LowerRegCost = 20;
			Resistances.Poison = 10;

            if (Utility.RandomBool())
                Attributes.SpellDamage = 8;
            else
                Attributes.WeaponDamage = 20;
		}

		public AlchemistsBauble( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();
		}
	}
}