using System;
using Server;
using Server.Mobiles;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Misc;
using Server.Spells;

namespace Server.Mobiles
{
	[CorpseName( "resztkii wladcy piaskow" )]
	public class WladcaPiaskowBoss : BaseCreature
	{
        // 10.10.2012 :: zombie
        public override double DifficultyScalar{ get{ return 1.05; }}
        // zombie


        public override double AttackMasterChance { get { return 0.75; } }
        public override double SwitchTargetChance { get { return 0.15; } }
		public override Poison PoisonImmune{ get{ return Poison.Deadly; } }
		public override int Meat{ get{ return 2; } }
		public override bool CanRummageCorpses{ get{ return true; } }
		public override bool AutoDispel{ get{ return true; } }
                    
	    public override void AddWeaponAbilities()
        {
            WeaponAbilities.Add( WeaponAbility.CrushingBlow, 0.4 );
            WeaponAbilities.Add( WeaponAbility.Feint, 0.4 );
            WeaponAbilities.Add( WeaponAbility.ForceOfNature, 0.4 );
        }

		[Constructable]
		public WladcaPiaskowBoss() : base( AIType.AI_Melee, FightMode.Closest, 13, 1, 0.25, 0.5 )
		{			
			Body = 0x316;

			Name = "władca piasku";
			BaseSoundID = 0x452;
            Hue = 2107;

			SetStr( 767, 945 );
			SetDex( 106, 115 );
			SetInt( 46, 70 );

			SetHits( 5000, 6000 );

			SetDamage( 20, 30 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 45, 55 );
			SetResistance( ResistanceType.Fire, 40, 50 );
			SetResistance( ResistanceType.Cold, 25, 35 );
			SetResistance( ResistanceType.Poison, 25, 35 );
			SetResistance( ResistanceType.Energy, 25, 35 );

			SetSkill( SkillName.MagicResist, 125.1, 140.0 );
			SetSkill( SkillName.Ninjitsu, 125.1, 140.0 );
			SetSkill( SkillName.Tracking, 125.1, 140.0 );
			SetSkill( SkillName.Bushido, 80.0, 120.0 );
			SetSkill( SkillName.Tactics, 90.1, 100.0 );
			SetSkill( SkillName.Wrestling, 90.1, 100.0 );

			Fame = 15000;
			Karma = -15000;

			VirtualArmor = 70;

						
			}
			public override void OnDeath( Container c )
		{
			base.OnDeath( c );

            ArtifactHelper.ArtifactDistribution(this);
		}

			public override bool OnBeforeDeath()
		{
			AddLoot( LootPack.AncientScrolls );
			return base.OnBeforeDeath( );
		}
		public override void GenerateLoot()
		{
			AddLoot( LootPack.AosSuperBoss );
			// 07.01.2013 :: szczaw :: usuniecie PackGold
			//PackGold(800, 1000 );
			AddLoot( LootPack.Gems, 6 );
		}

		public override void OnDamagedBySpell( Mobile caster )
		{
			if ( caster == this )
				return;

			SpawnEarthElemental( caster );
		}

		public void SpawnEarthElemental( Mobile target )
		{
			Map map = target.Map;

			if ( map == null )
				return;

			int minos = 0;

			IPooledEnumerable eable = GetMobilesInRange( 10 );
			foreach ( Mobile m in eable )
			{
				if ( m is EarthElemental )
					++minos;
			}
			eable.Free();

			if ( minos < 10 )
			{
				BaseCreature mino = new EarthElemental();
				/*mino.Summoned = true;
				mino.Criminal = true;
				mino.SummonMaster == WladcaPiaskowBoss;*/
				mino.DeleteCorpseOnDeath = true;
				

				mino.Team = this.Team;

				Point3D loc = target.Location;
				bool validLocation = false;

				for (int j = 0; !validLocation && j < 10; ++j) {
					int x = target.X + Utility.Random(3) - 1;
					int y = target.Y + Utility.Random(3) - 1;
					int z = map.GetAverageZ(x, y);

					if (validLocation = map.CanFit(x, y, this.Z, 16, false, false))
						loc = new Point3D(x, y, Z);
					else if (validLocation = map.CanFit(x, y, z, 16, false, false))
						loc = new Point3D(x, y, z);
				}

				mino.MoveToWorld( loc, map );

				mino.Combatant = target;
			}
		}

		public WladcaPiaskowBoss( Serial serial ) : base( serial )
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
