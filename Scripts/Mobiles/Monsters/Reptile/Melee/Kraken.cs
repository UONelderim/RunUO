using System;
using System.Collections;
using Server.Items;
using Server.Targeting;

namespace Server.Mobiles
{
	[CorpseName( "zwloki krakena" )]
	public class Kraken : BaseCreature
	{
		[Constructable]
		public Kraken() : base( AIType.AI_Melee, FightMode.Closest, 12, 1, 0.2, 0.4 )
		{
			Name = "kraken";
			Body = 77;
			BaseSoundID = 353;

			SetStr( 756, 780 );
			SetDex( 226, 245 );
			SetInt( 26, 40 );

			SetHits( 454, 468 );
			SetMana( 0 );

			SetDamage( 19, 33 );

			SetDamageType( ResistanceType.Physical, 70 );
			SetDamageType( ResistanceType.Cold, 30 );

			SetResistance( ResistanceType.Physical, 45, 55 );
			SetResistance( ResistanceType.Fire, 30, 40 );
			SetResistance( ResistanceType.Cold, 30, 40 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 10, 20 );

			SetSkill( SkillName.MagicResist, 15.1, 20.0 );
			SetSkill( SkillName.Tactics, 45.1, 60.0 );
			SetSkill( SkillName.Wrestling, 45.1, 60.0 );

			Fame = 11000;
			Karma = -11000;

			VirtualArmor = 50;

			CanSwim = true;
			CantWalk = true;

			if( Utility.RandomDouble() < .10 )
			{
			Rope rope = new Rope();
			rope.ItemID = 0x14F8;
			PackItem( rope );
			}
			
			if( Utility.RandomDouble() < .15 )
				PackItem( new MessageInABottle() );
				
			PackItem( new SpecialFishingNet() ); //Confirm?
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.Rich );
		}

		public override int Hides{ get{ return 3; } }
		public override HideType HideType{ get{ return HideType.Barbed; } }
		
		public override int TreasureMapLevel{ get{ return 4; } }

		public Kraken( Serial serial ) : base( serial )
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
		}
	}
}
