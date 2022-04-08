using System;
using Server;

namespace Server.Items
{
	public class RoyalGuardSurvivalKnife : SkinningKnife
	{
        public override int LabelNumber { get { return 1065830; } } // Noz Legendarnego Straznika
		public override int InitMinHits{ get{ return 50; } }
		public override int InitMaxHits{ get{ return 50; } }

		public override bool CanFortify{ get{ return true; } }

		[Constructable]
		public RoyalGuardSurvivalKnife()
		{
			Attributes.SpellChanneling = 1;
			Attributes.Luck = 140;
			Attributes.EnhancePotions = 25;

			WeaponAttributes.UseBestSkill = 1;
			WeaponAttributes.LowerStatReq = 50;
		}

		public RoyalGuardSurvivalKnife( Serial serial ) : base( serial )
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
