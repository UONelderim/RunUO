using System;
using Server.Network;
using Server.Items;

namespace Server.Items
{
	[FlipableAttribute( 0xDF1, 0xDF0 )]
	public class MieczDalekiegoZasiegu : BaseWeapon
	{
        private int m_Range = 1;
        private int m_HitSound = 0x233;
        private int m_MissSound = 0x239;

		public override int DefMaxRange{ get{ return m_Range; } }
        public override int DefHitSound{ get{ return m_HitSound; } }
		public override int DefMissSound{ get{ return m_MissSound; } }
        
		public override int AosStrengthReq{ get{ return 0; } }
        public override int AosSpeed{ get{ return 50; } }
        public override int AosMinDamage{ get{ return 3; } }
		public override int AosMaxDamage{ get{ return 3; } }

		public override SkillName DefSkill{ get{ return SkillName.Wrestling; } }
		public override WeaponType DefType{ get{ return WeaponType.Fists; } }

        //[Constructable]
		public MieczDalekiegoZasiegu(int range, int soundHit, int soundMiss) : base( 0 )
        {
            m_Range = range;
            m_HitSound = soundHit;
            m_MissSound = soundMiss;
			Visible = false;
			Movable = false;
        }

        //[Constructable]
		public MieczDalekiegoZasiegu(int range) : this( range, 0x233, 0x239 )   // 0x233, 0x239 - dzwieki obuchow
        {
        }

		//[Constructable]
		public MieczDalekiegoZasiegu() : this(1)
		{
		}

		public MieczDalekiegoZasiegu( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

            writer.Write( (int) m_Range );
            writer.Write( (int) m_HitSound );
            writer.Write( (int) m_MissSound );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

            m_Range = reader.ReadInt();
            m_HitSound = reader.ReadInt();
            m_MissSound = reader.ReadInt();
		}
	}
}