using System;
using System.Collections;
using Server.Misc;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "zwłoki jaśniejącego szczuroczłeka" )]
	public class Chiikkaha : RatmanMage
	{	
		public override double DifficultyScalar{ get{ return 1.02; } }
		[Constructable]
		public Chiikkaha() : base()
		{
			Name = "Chiikkaha - Szur o Wielu zębach";
			Body = 0x8F;
			BaseSoundID = 437;

			SetStr( 450, 476 );
			SetDex( 157, 179 );
			SetInt( 251, 275 );

			SetDamage( 10, 17 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 40, 45 );
			SetResistance( ResistanceType.Fire, 10, 20 );
			SetResistance( ResistanceType.Cold, 10, 20 );
			SetResistance( ResistanceType.Poison, 10, 20 );
			SetResistance( ResistanceType.Energy, 70, 80 );

			SetSkill( SkillName.EvalInt, 70.1, 80.0 );
			SetSkill( SkillName.Magery, 70.1, 80.0 );
			SetSkill( SkillName.MagicResist, 65.1, 90.0 );
			SetSkill( SkillName.Tactics, 50.1, 75.0 );
			SetSkill( SkillName.Wrestling, 50.1, 75.0 );

			Fame = 7500;
			Karma = -7500;
		}	

		public Chiikkaha( Serial serial ) : base( serial )
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
