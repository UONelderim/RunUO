using System;
using Server;

namespace Server.Items
{
	public class LeggingsOfBane : ChainLegs
	{
        public override int LabelNumber { get { return 1061100; } } // Nogawice Jadu
        public override int InitMinHits { get { return 50; } }
        public override int InitMaxHits { get { return 50; } }

		public override int BasePoisonResistance{ get{ return 36; } }

		[Constructable]
		public LeggingsOfBane()
		{
			Hue = 0x4F5;
			Attributes.BonusStam = 10;
			Attributes.AttackChance = 20;
		}

		public LeggingsOfBane( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 2 );
		}
		
		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if( version <= 1 )
			{
				if( this.HitPoints > 255 || this.MaxHitPoints > 255 )
					this.HitPoints = this.MaxHitPoints = 255;
			}

			if ( version < 1 )
			{
				if ( Hue == 0x559 )
					Hue = 0x4F5;

				if ( ArmorAttributes.DurabilityBonus == 0 )
					ArmorAttributes.DurabilityBonus = 100;

				PoisonBonus = 0;
			}
		}
	}
}