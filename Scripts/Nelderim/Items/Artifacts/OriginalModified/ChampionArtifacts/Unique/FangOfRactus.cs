using System;
using Server;

namespace Server.Items
{
	public class FangOfRactus : Kryss
	{
        public override int LabelNumber { get { return 1065846; } } // Kiel Ractusa
		public override int InitMinHits{ get{ return 60; } }
		public override int InitMaxHits{ get{ return 60; } }

		public override bool CanFortify{ get{ return true; } }

		[Constructable]
		public FangOfRactus()
		{
			Hue = 0x117;

			Attributes.SpellChanneling = 1;
			Attributes.AttackChance = 5;
			Attributes.DefendChance = 5;
			Attributes.WeaponDamage = 25;

			WeaponAttributes.HitPoisonArea = 10;
			WeaponAttributes.ResistPoisonBonus = 15;
		}

		public FangOfRactus( Serial serial ) : base( serial )
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
