using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "zwloki zywiolaka powietrza" )]
	public class SummonedAirElemental : BaseCreature
	{
		public override double DispelDifficulty{ get{ return 117.5; } }
		public override double DispelFocus{ get{ return 45.0; } }

		[Constructable]
		public SummonedAirElemental () : base( AIType.AI_BattleMage, FightMode.Closest, 12, 1, 0.2, 0.4 )
		{
			Name = "przywolany zywiolak powietrza";
			Body = 13;
			Hue = 0x4001;
			BaseSoundID = 655;

			SetStr( 200 );
			SetDex( 200 );
			SetInt( 100 );

			SetHits( 150 );
			SetStam( 50 );

			SetDamage( 6, 9 );

			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Energy, 50 );

			SetResistance( ResistanceType.Physical, 40, 50 );
			SetResistance( ResistanceType.Fire, 30, 40 );
			SetResistance( ResistanceType.Cold, 35, 45 );
			SetResistance( ResistanceType.Poison, 50, 60 );
			SetResistance( ResistanceType.Energy, 70, 80 );

			SetSkill( SkillName.Meditation, 90.0 );
			SetSkill( SkillName.EvalInt, 70.0 );
			SetSkill( SkillName.Magery, 70.0 );
			SetSkill( SkillName.MagicResist, 60.0 );
			SetSkill( SkillName.Tactics, 100.0 );
			SetSkill( SkillName.Wrestling, 80.0 );

			VirtualArmor = 40;
			ControlSlots = 2;
		}

		public SummonedAirElemental( Serial serial ) : base( serial )
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

			if ( BaseSoundID == 263 )
				BaseSoundID = 655;
		}
	}
}