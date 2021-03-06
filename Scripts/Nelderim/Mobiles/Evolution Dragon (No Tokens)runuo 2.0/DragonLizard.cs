    //////////////////////////////////
   //			           //
  //      Scripted by Raelis      //
 //		             	 //
//////////////////////////////////
using System;
using Server.Items;
using Server.Mobiles;

namespace Server.Mobiles
{
	[CorpseName( "zwloki smoczego jaszczura" )]
	public class DragonLizard : BaseCreature
	{
		[Constructable]
		public DragonLizard() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Name = "Smoczy Jaszczur";
			Body = 0xCE;
			Hue = Utility.RandomList( 1157, 1175, 1172, 1171, 1170, 1169, 1168, 1167, 1166, 1165 );
			BaseSoundID = 0x5A;

			SetStr( 596, 625 );
			SetDex( 86, 105 );
			SetInt( 226, 246 );

			SetHits( 600, 850 );

			SetDamage( 13, 19 );

			SetDamageType( ResistanceType.Physical, 100 );
			SetDamageType( ResistanceType.Fire, 25 );
			SetDamageType( ResistanceType.Cold, 25 );
			SetDamageType( ResistanceType.Poison, 25 );
			SetDamageType( ResistanceType.Energy, 25 );

			SetResistance( ResistanceType.Physical, 40 );
			SetResistance( ResistanceType.Fire, 40 );
			SetResistance( ResistanceType.Cold, 40 );
			SetResistance( ResistanceType.Poison, 40 );
			SetResistance( ResistanceType.Energy, 40 );

			SetSkill( SkillName.Magery, 70.1, 80.0 );
			SetSkill( SkillName.Meditation, 70.1, 80.0 );
			SetSkill( SkillName.EvalInt, 70.1, 80.0 );
			SetSkill( SkillName.MagicResist, 75.1, 80.0 );
			SetSkill( SkillName.Tactics, 49.3, 64.0 );
			SetSkill( SkillName.Wrestling, 49.3, 64.0 );
			SetSkill( SkillName.Anatomy, 49.3, 64.0 );

			Fame = 3000;
			Karma = -3000;

			VirtualArmor = 50;

			PackGold( 900, 1300 );
			PackMagicItems( 3, 3, 0.95, 0.95 );
			PackMagicItems( 3, 3, 0.80, 0.65 );

			if ( Utility.RandomDouble() <= 0.25 )
			{
				int amount = Utility.RandomMinMax( 1, 5 );

				PackItem( new DragonDust(amount) );
			}
		}

		public DragonLizard(Serial serial) : base(serial)
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

			this.MovingParticles( m, 0x1FAB, 1, 0, false, true, ( this.Hue - 1 ), 0, 9502, 6014, 0x11D, EffectLayer.Waist, 0 );
		}

		private class BreatheTimer : Timer
		{
			private DragonLizard d;
			private Mobile m_Mobile;

			public BreatheTimer( Mobile m, DragonLizard owner ) : base( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 1.0 ) )
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

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int) 0);
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}
}