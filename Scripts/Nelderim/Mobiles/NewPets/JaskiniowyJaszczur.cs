using System;
using Server.Mobiles;

namespace Server.Mobiles
{
	// Zwykly kon w wersji podmrokowej.
	// Parametry konia, wyglad ostarda.
	[CorpseName( "zwloki jaskiniowego jaszczura" )]
	public class JaskiniowyJaszczur : BaseMount
	{
		[Constructable]
		public JaskiniowyJaszczur() : this( "jaskiniowy jaszczur" )
		{
		}

		[Constructable]
		public JaskiniowyJaszczur( string name ) : base( name, 0xDA, 0x3EA4, AIType.AI_Animal, FightMode.Aggressor, 12, 1, 0.2, 0.4 )
        {
            Hue = Utility.RandomHairHue() | 0x8000;

            BaseSoundID = 0x275;

            SetStr( 22, 98 );
			SetDex( 56, 75 );
			SetInt( 6, 10 );

			SetHits( 28, 45 );
			SetMana( 0 );

			SetDamage( 3, 4 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 15, 20 );

			SetSkill( SkillName.MagicResist, 25.1, 30.0 );
			SetSkill( SkillName.Tactics, 29.3, 44.0 );
			SetSkill( SkillName.Wrestling, 29.3, 44.0 );

			Fame = 300;
			Karma = 300;

			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = 29.1;
		}

		public override int Meat{ get{ return 3; } }
		public override int Hides{ get{ return 4; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }

		public JaskiniowyJaszczur( Serial serial ) : base( serial )
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