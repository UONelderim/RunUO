/*
 * Created by SharpDevelop.
 * User: Shazzy
 * Date: 4/12/2006
 * Time: 6:26 AM
 * 
 * 
 */

using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "resztki niecnej ksiegi zaklec" )]
	public class EvilSpellbook : BaseCreature
	{
			    public override void AddWeaponAbilities()
        {
            WeaponAbilities.Add( WeaponAbility.ConcussionBlow, 0.1 );
        }
		
		[Constructable]
		public EvilSpellbook() : base( AIType.AI_NecroMage, FightMode.Weakest, 10, 1, 0.1, 0.2 )
		{
			Name = "niecna ksiega zaklec";
			Body = 985; // 0x3D9
			BaseSoundID = 466;

			SetStr( 400, 500 );
			SetDex( 300, 350 );
			SetInt( 900, 950 );

			SetHits( 5000 );

			SetDamage( 10, 15 );

			SetDamageType( ResistanceType.Cold, 100 );
			SetDamageType( ResistanceType.Physical, 0 );

			SetResistance( ResistanceType.Physical, 60, 70 );
			SetResistance( ResistanceType.Fire, 60, 70 );
			SetResistance( ResistanceType.Poison, 60, 70 );
			SetResistance( ResistanceType.Energy, 60, 70 );
			SetResistance( ResistanceType.Cold, 60, 70 );

			SetSkill( SkillName.EvalInt, 100.0, 100.0 );
			SetSkill( SkillName.Magery, 115.0, 115.0 );
			SetSkill( SkillName.Meditation, 105.0, 105.0 );
			SetSkill(SkillName.SpiritSpeak, 60.0);
            SetSkill(SkillName.Necromancy, 80.6, 10.5);
			SetSkill( SkillName.MagicResist, 100.0, 10.0 );
			SetSkill( SkillName.Tactics, 90.0, 95.0 );
			SetSkill( SkillName.Wrestling, 80.0, 80.0 );

			Fame = 22000;
			Karma = -22000;
			
			VirtualArmor = 20;
			AddItem( new LightSource() );
		}
		
				
				public override void OnDeath( Container c )
				{
					base.OnDeath( c );

					ArtifactHelper.ArtifactDistribution(this);
				}

		public override bool BardImmune{ get{ return true; } }
        public override double AttackMasterChance { get { return 0.15; } }
        public override double SwitchTargetChance { get { return 0.15; } }
		public override double DispelDifficulty{ get{ return 135.0; } }
		public override double DispelFocus{ get{ return 45.0; } }
		public override bool AutoDispel{ get{ return false; } }
		public override Poison PoisonImmune { get { return Poison.Lethal; } }
		public override Poison HitPoison { get { return Poison.Lethal; } }
		
        public override void GenerateLoot()
        {
            AddLoot( LootPack.AosSuperBoss );
        }
		
		public EvilSpellbook( Serial serial ) : base( serial )
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
