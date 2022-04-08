using System;
using Server;
using Server.Items;

namespace Server.Mobiles
{
	[CorpseName( "zwloki Smoka z Lodowcow Geriadoru" )]
	public class IceDragon : BaseCreature
	{
		
		public override double AttackMasterChance { get { return 0.15; } }
        public override double SwitchTargetChance { get { return 0.15; } }
		
		  public override void AddWeaponAbilities()
        {
            WeaponAbilities.Add( WeaponAbility.Bladeweave, 0.4 );
            WeaponAbilities.Add( WeaponAbility.BleedAttack, 0.222 );
            WeaponAbilities.Add( WeaponAbility.WhirlwindAttack, 0.222 );
        }
		[Constructable]
		public IceDragon () : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
            this.Name = "Smok z Lodowcow Geriadoru";
            this.Body = 104;
            this.BaseSoundID = 0x488;
            this.Hue = 1266;

            this.SetStr(100, 250);
            this.SetDex(68, 200);
            this.SetInt(1000, 1200);

            this.SetHits( 4000 );
            this.SetMana(1000, 1200);

            this.SetDamage(25, 29);

            this.SetDamageType(ResistanceType.Physical, 25);
            this.SetDamageType(ResistanceType.Cold, 75);

			SetResistance( ResistanceType.Physical, 65, 80 );
			SetResistance( ResistanceType.Fire, 90 );
			SetResistance( ResistanceType.Cold, 90 );
			SetResistance( ResistanceType.Poison, 90 );
			SetResistance( ResistanceType.Energy, 40, 50 );

            this.SetSkill(SkillName.EvalInt, 80.1, 100.0);
            this.SetSkill(SkillName.Magery, 80.1, 100.0);
            this.SetSkill(SkillName.MagicResist, 100.3, 130.0);
            this.SetSkill(SkillName.Tactics, 97.6, 100.0);
            this.SetSkill(SkillName.Wrestling, 97.6, 100.0);

            this.Fame = 22500;
            this.Karma = -22500;

            this.VirtualArmor = 90;
			AddItem( new LightSource() );

            this.Tamable = false;
		}

		public override void GenerateLoot()
		{
            this.AddLoot(LootPack.FilthyRich, 4);
            this.AddLoot(LootPack.Gems, Utility.Random(1, 5));
		}
		public override void OnDeath( Container c )
		{
			base.OnDeath( c );

            ArtifactHelper.ArtifactDistribution(this);
		}

		public override bool HasBreath
        { 
            get
            { 
                return true; 
            } 
        }
		public override int BreathFireDamage
        { 
            get
            { 
                return 0;
            } 
        }
		public override int BreathColdDamage
        { 
            get
            { 
                return 100;
            } 
        }
		public override int BreathEffectHue
        { 
            get
            { 
                return 1266; 
            } 
        }

		public override bool AutoDispel
        { 
            get
            { 
                return false; 
            } 
        }
		public override Poison PoisonImmune
        { 
            get
            { 
                return Poison.Lethal; 
            } 
        }
        public override bool ReacquireOnMovement
        {
            get
            {
                return true;
            }
        }
        public override int TreasureMapLevel
        {
            get
            {
                return 4;
            }
        }
        public override int Meat
        {
            get
            {
                return 19;
            }
        }
        public override int Hides
        {
            get
            {
                return 20;
            }
        }
        public override HideType HideType
        {
            get
            {
                return HideType.Barbed;
            }
        }
        public override int Scales
        {
            get
            {
                return 9;
            }
        }
        public override ScaleType ScaleType
        {
            get
            {
                return ScaleType.White;
            }
        }

		public IceDragon( Serial serial ) : base( serial )
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