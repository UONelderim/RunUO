using System;
using System.Collections;
using Server;
using Server.Items;
using System.Collections.Generic;
using Server.Accounting;

namespace Server.Mobiles
{
	[CorpseName( "zwloki mrocznego rycerza" )]
	public class DemonKnight : BaseCreature
    {
        public override double DifficultyScalar{ get{ return 1.10; }}

		public override bool IgnoreYoungProtection { get { return Core.ML; } }

        public override void AddWeaponAbilities()
        {
            WeaponAbilities.Add( WeaponAbility.DoubleStrike, 0.133 );
            WeaponAbilities.Add( WeaponAbility.WhirlwindAttack, 0.133 );
            WeaponAbilities.Add( WeaponAbility.CrushingBlow, 0.133 );
        }

		public override void OnDeath( Container c )
		{
			base.OnDeath( c );

            ArtifactHelper.ArtifactDistribution(this);
		}

		[Constructable]
		public DemonKnight() : base( AIType.AI_BattleMage, FightMode.Closest, 12, 1, 0.2, 0.4 )
		{
			Name = NameList.RandomName( "demon knight" );
			Title = "Pan Mroku";
			Body = 318;
			BaseSoundID = 0x165;

			SetStr( 500 );
			SetDex( 100 );
			SetInt( 1000 );

			SetHits( 30000 );
			SetMana( 5000 );

			SetDamage( 17, 21 );

			SetDamageType( ResistanceType.Physical, 20 );
			SetDamageType( ResistanceType.Fire, 20 );
			SetDamageType( ResistanceType.Cold, 20 );
			SetDamageType( ResistanceType.Poison, 20 );
			SetDamageType( ResistanceType.Energy, 20 );

			SetResistance( ResistanceType.Physical, 30 );
			SetResistance( ResistanceType.Fire, 30 );
			SetResistance( ResistanceType.Cold, 30 );
			SetResistance( ResistanceType.Poison, 30 );
			SetResistance( ResistanceType.Energy, 30 );

			SetSkill( SkillName.DetectHidden, 80.0 );
			SetSkill( SkillName.EvalInt, 100.0 );
			SetSkill( SkillName.Magery, 100.0 );
			SetSkill( SkillName.Meditation, 120.0 );
			SetSkill( SkillName.MagicResist, 150.0 );
			SetSkill( SkillName.Tactics, 100.0 );
			SetSkill( SkillName.Wrestling, 120.0 );

			Fame = 28000;
			Karma = -28000;

			VirtualArmor = 64;
		}

		public override void GenerateLoot()
		{
			AddLoot( LootPack.SuperBoss, 2 );
			AddLoot( LootPack.HighScrolls, Utility.RandomMinMax( 6, 60 ) );
		}

		public override bool BardImmune{ get{ return !Core.SE; } }
		public override bool Unprovokable{ get{ return Core.SE; } }
		public override bool AreaPeaceImmune { get{ return Core.SE; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }

		public override int TreasureMapLevel{ get{ return 1; } }

		private static bool m_InHere;

		public override void OnDamage( int amount, Mobile from, bool willKill )
		{
			if ( from != null && from != this && !m_InHere )
			{
				m_InHere = true;
				AOS.Damage( from, this, Utility.RandomMinMax( 8, 20 ), 100, 0, 0, 0, 0 );

				MovingEffect( from, 0xECA, 10, 0, false, false, 0, 0 );
				PlaySound( 0x491 );

				if ( 0.05 > Utility.RandomDouble() )
					Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), new TimerStateCallback( CreateBones_Callback ), from );

				m_InHere = false;
			}
		}

		public virtual void CreateBones_Callback( object state )
		{
			Mobile from = (Mobile)state;
			Map map = from.Map;

			if ( map == null )
				return;

			int count = Utility.RandomMinMax( 1, 3 );

			for ( int i = 0; i < count; ++i )
			{
				int x = from.X + Utility.RandomMinMax( -1, 1 );
				int y = from.Y + Utility.RandomMinMax( -1, 1 );
				int z = from.Z;

				if ( !map.CanFit( x, y, z, 16, false, true ) )
				{
					z = map.GetAverageZ( x, y );

					if ( z == from.Z || !map.CanFit( x, y, z, 16, false, true ) )
						continue;
				}

				UnholyBone bone = new UnholyBone();

				bone.Hue = 0;
				bone.Name = "unholy bones";
				bone.ItemID = Utility.Random( 0xECA, 9 );

				bone.MoveToWorld( new Point3D( x, y, z ), map );
			}
		}

		public DemonKnight( Serial serial ) : base( serial )
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