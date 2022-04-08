using System;
using System.Collections;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "zwloki kitsune" )]
	public class BakeKitsune : BaseCreature
	{

		[Constructable]
		public BakeKitsune() : base( AIType.AI_BattleMage, FightMode.Closest, 12, 1, 0.2, 0.4 )
		{
			Name = "kitsune";
			Body = 246;

			SetStr( 171, 220 );
			SetDex( 126, 145 );
			SetInt( 376, 425 );

			SetHits( 301, 350 );

			SetDamage( 15, 22 );

			SetDamageType( ResistanceType.Physical, 70 );
			SetDamageType( ResistanceType.Energy, 30 );

			SetResistance( ResistanceType.Physical, 45, 65 );
			SetResistance( ResistanceType.Fire, 70, 90 );
			SetResistance( ResistanceType.Cold, 20, 40 );
			SetResistance( ResistanceType.Poison, 45, 65 );
			SetResistance( ResistanceType.Energy, 50, 60 );

			SetSkill( SkillName.EvalInt, 80.1, 90.0 );
			SetSkill( SkillName.Magery, 80.1, 90.0 );
			SetSkill( SkillName.MagicResist, 80.1, 100.0 );
			SetSkill( SkillName.Tactics, 70.1, 90.0 );
			SetSkill( SkillName.Wrestling, 50.1, 55.0 );

			Fame = 8000;
			Karma = -8000;

			Tamable = true;
			ControlSlots = 2;
			MinTameSkill = 80.7;


			if ( Utility.RandomDouble() < .25 )
				PackItem( Engines.Plants.Seed.RandomBonsaiSeed() );
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich );
			AddLoot( LootPack.Rich );
		}

		public override int Meat{ get{ return 4; } }
		public override int Hides{ get{ return 8; } }
		public override HideType HideType{ get{ return HideType.Barbed; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Fish; } }

		public override void OnGaveMeleeAttack( Mobile defender )
		{
			base.OnGaveMeleeAttack( defender );

			if ( 0.1 > Utility.RandomDouble() )
			{
				/* Blood Bath
				 * Start cliloc 1070826
				 * Sound: 0x52B
				 * 2-3 blood spots
				 * Damage: 2 hps per second for 5 seconds
				 * End cliloc: 1070824
				 */
			
				ExpireTimer timer = (ExpireTimer)m_Table[defender];

				if ( timer != null )
		{
					timer.DoExpire();
					defender.SendLocalizedMessage( 1070825 ); // The creature continues to rage!
				}
				else
					defender.SendLocalizedMessage( 1070826 ); // The creature goes into a rage, inflicting heavy damage!

				timer = new ExpireTimer( defender, this );
				timer.Start();
				m_Table[defender] = timer;
			}
		}

		private static Hashtable m_Table = new Hashtable();
	
		private class ExpireTimer : Timer
		{
			private Mobile m_Mobile;
			private Mobile m_From;
			private int m_Count;

			public ExpireTimer( Mobile m, Mobile from ) : base( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 1.0 ) )
			{
				m_Mobile = m;
				m_From = from;
				Priority = TimerPriority.TwoFiftyMS;
			}

			public void DoExpire()
			{
				Stop();
				m_Table.Remove( m_Mobile );
			}

			public void DrainLife()
			{
				if ( m_Mobile.Alive )
					m_Mobile.Damage( 2, m_From );
				else
					DoExpire();
			}

			protected override void OnTick()
			{
				DrainLife();

				if ( ++m_Count >= 5 )
				{
					DoExpire();
					m_Mobile.SendLocalizedMessage( 1070824 ); // The creature's rage subsides.
				}
			}
		}
		
		public override int GetAngerSound()
		{
			return 0x4DE;
		}

		public override int GetIdleSound()
		{
			return 0x4DD;
		}

		public override int GetAttackSound()
		{
			return 0x4DC;
		}

		public override int GetHurtSound()
		{
			return 0x4DF;
		}

		public override int GetDeathSound()
		{
			return 0x4DB;
		}

		public BakeKitsune( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );
			writer.Write( (int) 2);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );
			int version = reader.ReadInt();

			if ( version < 2 && FireResistance > 90 && PhysicalResistance < 65 && ColdResistance < 70 && PoisonResistance < 65 && EnergyResistance < 70 )
			{
			SetResistance( ResistanceType.Physical, 45, 65 );
			SetResistance( ResistanceType.Fire, 70, 90 );
			SetResistance( ResistanceType.Cold, 20, 40 );
			SetResistance( ResistanceType.Poison, 45, 65 );
			SetResistance( ResistanceType.Energy, 50, 60 );
			}
		}
	}
}
