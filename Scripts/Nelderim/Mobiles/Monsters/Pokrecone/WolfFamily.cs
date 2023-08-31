// Wolf Family
// a RunUO ver 2.0 Script
// Written by David 
// last edited 6/17/06

using System;
using Server.Mobiles;
using System.Collections;

namespace Server.Mobiles
{
	[CorpseName( "zwloki wilka" )]
	public class MotherWolf : BaseCreature
	{
		private ArrayList m_pups;
		int pupCount = Utility.RandomMinMax( 2, 5 );

		public override int Meat{ get{ return 1; } }
		public override int Hides{ get{ return 3; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Canine; } }
		public override bool CanRegenHits{ get{ return true; } }

		[CommandProperty( AccessLevel.GameMaster )]
		public bool RespawnPups
		{
			get{ return false; }
			set{ if( value ) SpawnBabies(); }
		}

		[Constructable]
        public MotherWolf() : base(AIType.AI_Melee, FightMode.Aggressor, 10, 1, 0.1, 0.3)
		{
			Name = "wilcza matka";
			Body = 25;
			BaseSoundID = 0xE5;

			SetStr( 91, 110 );
			SetDex( 76, 95 );
			SetInt( 31, 50 );

			SetHits( 42, 68 );
			SetMana( 0 );

			SetDamage( 11, 21 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 15, 25 );
			SetResistance( ResistanceType.Fire, 1, 10 );
			SetResistance( ResistanceType.Cold, 20, 25 );
			SetResistance( ResistanceType.Poison, 10, 15 );
			SetResistance( ResistanceType.Energy, 10, 15 );

			SetSkill( SkillName.MagicResist, 30.6, 45.0 );
			SetSkill( SkillName.Tactics, 50.1, 70.0 );
			SetSkill( SkillName.Wrestling, 60.1, 75.0 );

			Fame = 300;
			Karma = 0;

			VirtualArmor = 22;

			Tamable = false;

			m_pups = new ArrayList();
			Timer m_timer = new WolfFamilyTimer( this );
			m_timer.Start();
		}

		public override bool OnBeforeDeath()
		{	
			foreach( Mobile m in m_pups )
			{	
				if( m is WolfPup && m.Alive && ((WolfPup)m).ControlMaster != null )
					m.Kill();
			}
			
			return base.OnBeforeDeath();
		}
		
		public void SpawnBabies()
		{

			Defrag();
			int family = m_pups.Count;

			if( family >= pupCount )
				return;

			//Say( "family {0}, should be {1}", family, pupCount );

			Map map = this.Map;

			if ( map == null )
				return;

			int hr = (int)((this.RangeHome + 1) / 2);

			for ( int i = family; i < pupCount; ++i )
			{
				WolfPup pup = new WolfPup();

				bool validLocation = false;
				Point3D loc = this.Location;

				for ( int j = 0; !validLocation && j < 10; ++j )
				{
					int x = X + Utility.Random( 5 ) - 1;
					int y = Y + Utility.Random( 5 ) - 1;
					int z = map.GetAverageZ( x, y );

					if ( validLocation = map.CanFit( x, y, this.Z, 16, false, false ) )
						loc = new Point3D( x, y, Z );
					else if ( validLocation = map.CanFit( x, y, z, 16, false, false ) )
						loc = new Point3D( x, y, z );
				}

				pup.Mother = this;
				pup.Team = this.Team;
				pup.Home = this.Location;
				pup.RangeHome = ( hr > 4 ? 4 : hr );
				
				pup.MoveToWorld( loc, map );
				m_pups.Add( pup );
			}
		}

		protected override void OnLocationChange( Point3D oldLocation )
		{

			try
			{
				foreach( Mobile m in m_pups )
				{	
					if( m is WolfPup && m.Alive && ((WolfPup)m).ControlMaster == null )
					{
						((WolfPup)m).Home = this.Location;
					}
				}
			}
			catch{}

			base.OnLocationChange( oldLocation );
		}
		
		public void Defrag()
		{
			for ( int i = 0; i < m_pups.Count; ++i )
			{
				try
				{
					object o = m_pups[i];

					WolfPup pup = o as WolfPup;

					if ( pup == null || !pup.Alive )
					{
						m_pups.RemoveAt( i );
						--i;
					}

					else if ( pup.Controlled || pup.IsStabled )
					{
						pup.Mother = null;
						m_pups.RemoveAt( i );
						--i;
					}
				}
				catch{}
			}
		}

		public override void OnDelete()
		{
			Defrag();

			foreach( Mobile m in m_pups )
			{	
				if( m.Alive && ((WolfPup)m).ControlMaster == null )
					m.Delete();
			}

			base.OnDelete();
		}

		public MotherWolf(Serial serial) : base(serial)
		{
			m_pups = new ArrayList();
			Timer m_timer = new WolfFamilyTimer( this );
			m_timer.Start();
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);
			writer.Write((int) 0);
			writer.WriteMobileList( m_pups, true );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
			m_pups = reader.ReadMobileList();
		}
	}

	[CorpseName( "a young wolf corpse" )]
	public class WolfPup : BaseCreature
	{
		public override int Meat{ get{ return 1; } }
		public override FoodType FavoriteFood{ get{ return FoodType.Meat; } }
		public override PackInstinct PackInstinct{ get{ return PackInstinct.Canine; } }

		private MotherWolf m_mommy;

		[CommandProperty( AccessLevel.GameMaster )]
		public MotherWolf Mother
		{
			get{ return m_mommy; }
			set{ m_mommy = value; }
		}

		[Constructable]
        public WolfPup() : base(AIType.AI_Animal, FightMode.Aggressor, 10, 1, 0.2, 0.4)
		{
			Name = "mlody wilk";
			Body = 0xD9;
			BaseSoundID = 0xE5;

			SetStr( 37, 47 );
			SetDex( 38, 53 );
			SetInt( 39, 47 );

			SetHits( 17, 42 );
			SetMana( 0 );

			SetDamage( 4, 7 );

			SetDamageType( ResistanceType.Physical, 100 );

			SetResistance( ResistanceType.Physical, 10, 15 );

			SetSkill( SkillName.MagicResist, 22.1, 47.0 );
			SetSkill( SkillName.Tactics, 19.2, 31.0 );
			SetSkill( SkillName.Wrestling, 19.2, 46.0 );

			Fame = 100;
			Karma = 100;

			VirtualArmor = 10;

			Tamable = true;
			ControlSlots = 1;
			MinTameSkill = 53.1;
		}

		public override void OnCombatantChange()
		{
			if( Combatant != null && Combatant.Alive && m_mommy != null && m_mommy.Combatant == null )
				m_mommy.Combatant = Combatant;
		}

		public WolfPup(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize( writer );
			writer.Write( (int) 0 );
			writer.Write( m_mommy );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);
			int version = reader.ReadInt();
			m_mommy = (MotherWolf)reader.ReadMobile();
		}
	}

	public class WolfFamilyTimer : Timer
	{
		private MotherWolf m_from;

		public WolfFamilyTimer( MotherWolf from  ) : base( TimeSpan.FromMinutes( 1 ), TimeSpan.FromMinutes( 20 ) )
		{
			Priority = TimerPriority.OneMinute; 
			m_from = from;
		}

		protected override void OnTick()
		{
			if ( m_from != null && m_from.Alive )
				m_from.SpawnBabies();
			else
				Stop();
		}
	}
}