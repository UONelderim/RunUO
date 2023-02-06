using System;
using System.Collections;
using Server.Items;
using Server.Engines.CannedEvil;

namespace Server.Mobiles
{
	public class Semidar : BaseChampion
	{
		public override ChampionSkullType SkullType{ get{ return ChampionSkullType.Pain; } }

		public override Type[] UniqueList{ get{ return new Type[] { typeof( GladiatorsCollar ) }; } }
		public override Type[] SharedList { get { return new Type[] { typeof(RoyalGuardSurvivalKnife), typeof(ANecromancerShroud), typeof(LieutenantOfTheBritannianRoyalGuard) }; } }
		public override Type[] DecorativeList{ get{ return new Type[] { typeof( LavaTile ), typeof( DemonSkull ) }; } }

		public override MonsterStatuetteType[] StatueTypes{ get{ return new MonsterStatuetteType[] { }; } }

		[Constructable]
		public Semidar() : base( AIType.AI_Mage )
		{
			Name = "Semidar";
			Body = 174;
			BaseSoundID = 0x4B0;

			SetStr( 502, 600 );
			SetDex( 102, 200 );
			SetInt( 601, 750 );

			SetHits( 1500 );
			SetStam( 103, 250 );

			SetDamage( 29, 35 );

			SetDamageType( ResistanceType.Physical, 75 );
			SetDamageType( ResistanceType.Fire, 25 );

			SetResistance( ResistanceType.Physical, 20, 30 );
			SetResistance( ResistanceType.Fire, 50, 60 );
			SetResistance( ResistanceType.Cold, 20, 30 );
			SetResistance( ResistanceType.Poison, 20, 30 );
			SetResistance( ResistanceType.Energy, 10, 20 );

			SetSkill( SkillName.EvalInt, 95.1, 100.0 );
			SetSkill( SkillName.Magery, 90.1, 105.0 );
			SetSkill( SkillName.Meditation, 95.1, 100.0 );
			SetSkill( SkillName.MagicResist, 120.2, 140.0 );
			SetSkill( SkillName.Tactics, 90.1, 105.0 );
			SetSkill( SkillName.Wrestling, 90.1, 105.0 );

			Fame = 24000;
			Karma = -24000;

			VirtualArmor = 20;

			PSDropCount = 4;
			TotalGoldDrop = 35000;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.UltraRich, 4 );
			AddLoot( LootPack.FilthyRich );
		}

		public override bool Unprovokable{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }

		public override void CheckReflect( Mobile caster, ref bool reflect )
		{
			if ( caster.Body.IsMale )
				reflect = true; // Always reflect if caster isn't female
		}

		public override void AlterDamageScalarFrom( Mobile caster, ref double scalar )
		{
			if ( caster.Body.IsMale )
				scalar = 20; // Male bodies always reflect.. damage scaled 20x
		}
protected override PowerScroll CreateRandomPowerScroll()
		{
			int level;
			double random = Utility.RandomDouble();

			if (0.05 >= random)
				level = 20; // 5%
			else if (0.30 >= random)
				level = 15; // 15%
			else if (0.40 >= random)
				level = 10; // 30%
			else 
				level = 5; // 50%

			return PowerScroll.CreateRandomNoCraft( level, level );
		}
		public void DrainLife()
		{
			if( this.Map == null )
				return;

			ArrayList list = new ArrayList();

			IPooledEnumerable eable = GetMobilesInRange( 2 );
			foreach ( Mobile m in eable )
			{
				if ( m == this || !CanBeHarmful( m ) )
					continue;

				if ( m is BaseCreature && (((BaseCreature)m).Controlled || ((BaseCreature)m).Summoned || ((BaseCreature)m).Team != this.Team) )
					list.Add( m );
				else if ( m.Player )
					list.Add( m );
			}
			eable.Free();

			foreach ( Mobile m in list )
			{
				DoHarmful( m );

				m.FixedParticles( 0x374A, 10, 15, 5013, 0x496, 0, EffectLayer.Waist );
				m.PlaySound( 0x231 );

				m.SendMessage( "Czujesz jak siły witalne uciekają!" );

				int toDrain = Utility.RandomMinMax( 10, 40 );

				Hits += toDrain;
				m.Damage( toDrain, this );
			}
		}
		public override void OnGaveMeleeAttack( Mobile defender )
		{
			base.OnGaveMeleeAttack( defender );

			if ( 0.25 >= Utility.RandomDouble() )
				DrainLife();
		}

		public override void OnGotMeleeAttack( Mobile attacker )
		{
			base.OnGotMeleeAttack( attacker );

			if ( 0.25 >= Utility.RandomDouble() )
				DrainLife();
		}


		public Semidar( Serial serial ) : base( serial )
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
