    //////////////////////////////////
   //			           //
  //      Scripted by Raelis      //
 //		             	 //
//////////////////////////////////
using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "zwloki smoka" )]
	public class YoungDragon : BaseCreature
	{
		[Constructable]
		public YoungDragon () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Mlody Smok";
			Hue = Utility.RandomList( 1157, 1175, 1172, 1171, 1170, 1169, 1168, 1167, 1166, 1165 );
			Body = Utility.RandomList( 60, 61 );
			BaseSoundID = 362;

			SetStr( 696, 725 );
			SetDex( 86, 115 );
			SetInt( 346, 366 );

			SetHits( 700, 950 );

			SetDamage( 14, 20 );

			SetDamageType( ResistanceType.Physical, 100 );
			SetDamageType( ResistanceType.Fire, 25 );
			SetDamageType( ResistanceType.Cold, 25 );
			SetDamageType( ResistanceType.Poison, 25 );
			SetDamageType( ResistanceType.Energy, 25 );

			SetResistance( ResistanceType.Physical, 60 );
			SetResistance( ResistanceType.Fire, 60 );
			SetResistance( ResistanceType.Cold, 60 );
			SetResistance( ResistanceType.Poison, 60 );
			SetResistance( ResistanceType.Energy, 60 );

			SetSkill( SkillName.Magery, 85.1, 95.0 );
			SetSkill( SkillName.Meditation, 85.1, 95.0 );
			SetSkill( SkillName.EvalInt, 85.1, 95.0 );
			SetSkill( SkillName.MagicResist, 90.1, 95.0 );
			SetSkill( SkillName.Tactics, 64.3, 79.0 );
			SetSkill( SkillName.Wrestling, 64.3, 79.0 );
			SetSkill( SkillName.Anatomy, 64.3, 79.0 );

			Fame = 5500;
			Karma = -5500;

			VirtualArmor = 60;

			PackGold( 1400, 1800 );
			PackMagicItems( 3, 4, 0.95, 0.95 );
			PackMagicItems( 3, 4, 0.80, 0.65 );
			PackMagicItems( 3, 4, 0.80, 0.65 );

			if ( Utility.RandomDouble() <= 0.35 )
			{
				int amount = Utility.RandomMinMax( 1, 5 );

				PackItem( new DragonDust(amount) );
			}
		}

		public YoungDragon( Serial serial ) : base( serial )
		{
		}

		private DateTime m_NextBreathe;

		public override void OnActionCombat()
		{
			Mobile combatant = Combatant;

			if ( combatant == null || combatant.Deleted || combatant.Map != Map || !InRange( combatant, 12 ) || !CanBeHarmful( combatant ) || !InLOS( combatant ) )
				return;

			if ( DateTime.Now >= m_NextBreathe )
			{
				Breathe( combatant );

				m_NextBreathe = DateTime.Now + TimeSpan.FromSeconds( 12.0 + (3.0 * Utility.RandomDouble()) ); // 12-15 seconds
			}
		}

		public void Breathe( Mobile m )
		{
			DoHarmful( m );

			new BreatheTimer( m, this ).Start();

			this.Frozen = true;

			this.MovingParticles( m, 0x1FBC, 1, 0, false, true, ( this.Hue - 1 ), 0, 9502, 6014, 0x11D, EffectLayer.Waist, 0 );
		}


		private class BreatheTimer : Timer
		{
			private YoungDragon d;
			private Mobile m_Mobile;

			public BreatheTimer( Mobile m, YoungDragon owner ) : base( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 1.0 ) )
			{
				d = owner;
				m_Mobile = m;
			}

			protected override void OnTick()
			{
				int damagemin = d.Hits / 20;
				int damagemax = d.Hits / 25;
				d.Frozen = false;

				m_Mobile.PlaySound( 0x11D );
				AOS.Damage( m_Mobile, Utility.RandomMinMax( damagemin, damagemax ), 0, 100, 0, 0, 0 );
				Stop();
			}
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