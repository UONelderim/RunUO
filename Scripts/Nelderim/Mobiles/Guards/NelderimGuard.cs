/***************************************************************************
 *                              NelderimGuard.cs
 *                            -------------------
 * 							  Nelderim rel. Piencu 1.0
 * 							  http:\\nelderim.org
 *
 ***************************************************************************/

using System;
using System.Collections;
using System.Text.RegularExpressions;
using Server;
using Server.Mobiles;
using Server.ContextMenus;
using Server.Gumps;
using Server.Items;
using Server.Nelderim;

namespace Server.Mobiles
{
	public enum GuardType
	{
		StandardGuard,
		ArcherGuard,
		HeavyGuard,
        MageGuard,
		MountedGuard,
		EliteGuard,
		SpecialGuard
	}
	
	public enum WarFlag
	{
		None,
		White,
		Black,
		Red,
		Green,
		Blue
	}
	
	public class BaseNelderimGuard : BaseCreature
	{
		private GuardType m_Type;
		private string m_RegionName;
		private WarFlag m_Flag;
		private WarFlag m_Enemy;
		
		[CommandProperty( AccessLevel.Counselor, AccessLevel.GameMaster )]
		public string HomeRegionName
		{
			get
			{
				return m_RegionName;
			}
			
			set
			{
				m_RegionName = value;
				
				try
				{
					if ( !RegionsEngine.MakeGuard( this, m_RegionName ) )
						m_RegionName = null;
				}
				catch ( Exception e )
				{
					Console.WriteLine( e.ToString() );
					m_RegionName = null;
				}
			}
				
		}

        // 26.06.2012 :: zombie
        public override bool IsEnemy( Mobile m )
        {
            if ( m == null )
                return false;

            // 07.11.2012 :: zombie :: tymczasowo
            if ( m is BaseNelderimGuard )
                return false;
            // zombie

            if ( m.Criminal || m.Kills >= 5 )
                return true;

            if ( WarOpponentFlag != WarFlag.None && WarOpponentFlag == ( m as BaseNelderimGuard ).WarSideFlag )
                return true;

            if ( m is BaseCreature )
            {
                BaseCreature bc = m as BaseCreature;
                if ( bc.AlwaysMurderer || ( !bc.Controlled && bc.FightMode == FightMode.Closest ) )
                    return true;

                if ( ( bc.Controlled && bc.ControlMaster != null && bc.ControlMaster.AccessLevel < AccessLevel.Counselor && ( bc.ControlMaster.Criminal || bc.ControlMaster.Kills >= 5 ) ) || 
                    ( bc.Summoned && bc.SummonMaster != null && bc.SummonMaster.AccessLevel < AccessLevel.Counselor && ( bc.SummonMaster.Criminal || bc.SummonMaster.Kills >= 5 ) ) )
                    return true;
            }

            return false;
        }
        // zombie
		public override void CriminalAction( bool message )
		{
			// Straznik nigdy nie dostanie krima.
			// Byl problem, ze gdy straznik atakowal peta/summona gracza-krima to sam dostawal krima.s
			return;
		}

		[CommandProperty( AccessLevel.Counselor, AccessLevel.GameMaster )]
		public WarFlag WarSideFlag
		{
			get { return m_Flag; }
			set 
			{
				m_Flag = value; 
				
				if ( m_Flag != WarFlag.None && m_Flag == m_Enemy )
					m_Enemy = WarFlag.None;
			}
		}
		
		[CommandProperty( AccessLevel.Counselor, AccessLevel.GameMaster )]
		public WarFlag WarOpponentFlag
		{
			get { return m_Enemy; }
			set 
			{ 
				m_Enemy = value; 
				
				if ( m_Enemy != WarFlag.None && m_Flag == m_Enemy )
					m_Flag = WarFlag.None;
			}
		}
		
		public BaseNelderimGuard( GuardType type ) : this( type, FightMode.Criminal )
		{
		}
		
		public BaseNelderimGuard( GuardType type, FightMode fmode ) : base( AIType.AI_Melee, fmode, 12, 1, 0.1, 0.4 )
		{
			m_Type = type;
			m_RegionName = null;
			m_Flag = WarFlag.None;
			m_Enemy = WarFlag.None;
			ActiveSpeed = 0.05;
			PassiveSpeed = 0.1;

			switch (type)
			{
				case GuardType.MountedGuard:
					RangePerception = 16;
					AI = AIType.AI_Mounted;
					PackGold( 40, 80 );
					break;
				case GuardType.ArcherGuard:
					RangePerception = 16;
					RangeFight = 6;
					AI = AIType.AI_Archer;
					PackGold( 30, 90 );
					break;					
				case GuardType.EliteGuard:
					RangePerception = 18;
					AI = AIType.AI_Melee;
					PackGold( 50, 100 );
					break;
				case GuardType.SpecialGuard:
					RangePerception = 20;
					AI = AIType.AI_Melee;
					PackGold( 60, 100 );
					break;
                case GuardType.HeavyGuard:
                    RangePerception = 16;
                    AI = AIType.AI_Melee;
                    PackGold( 40, 80 );
                    break;
                case GuardType.MageGuard:
                    RangePerception = 16;
                    AI = AIType.AI_Mage;
                    PackGold( 40, 80 );
                    break;
				default:
					PackGold( 20, 80 );
					break;	
			}
			
			Fame = 5000;
			Karma = 5000;

			new RaceTimer( this ).Start();
		}

		public BaseNelderimGuard(Serial serial) : base(serial)
		{
		}
		
		public override bool AutoDispel{ get{ return true; } }
		public override bool Unprovokable{ get{ return true; } }
		public override bool Uncalmable{ get{ return true; } }
		public override bool BardImmune{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Greater; } }
		public override bool HandlesOnSpeech( Mobile from )
		{
			return true;
		}	
		public override bool ShowFameTitle
		{
		      get
		      {
		            return false;
		      }
		}	

	    public override void AddWeaponAbilities()
        {
            WeaponAbilities.Add( WeaponAbility.Dismount, 0.2 );
            WeaponAbilities.Add( WeaponAbility.BleedAttack, 0.2 );
        }

		public GuardType Type
		{
			get { return m_Type; }
		}
	
		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int) 2);
			
			// v 2
			writer.Write( ( int ) m_Flag );
			writer.Write( ( int ) m_Enemy );
			
			// v 1
			writer.Write( m_RegionName );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
			
			switch ( version )
			{
				case 2:
					{
						m_Flag = ( WarFlag ) reader.ReadInt();
						m_Enemy = ( WarFlag ) reader.ReadInt();
						goto case 1;
					}
				case 1:
					{
						if ( version < 2 )
						{
							m_Flag = WarFlag.None;
							m_Enemy = WarFlag.None;
						}
						
						m_RegionName = reader.ReadString();
						break;
					}
				default:
					{
						if ( version < 1 )
							m_RegionName = null;
						break;
					}
			}
		}
		
		public override void GenerateLoot()
		{
			AddLoot( LootPack.Poor );
		}
		
		
		private class RaceTimer : Timer
		{		
			private BaseNelderimGuard m_Target;
			
			public RaceTimer( BaseNelderimGuard target ) : base( TimeSpan.FromMilliseconds( 250 ) )
			{
				m_Target = target;
				Priority = TimerPriority.FiftyMS;
			}

			protected override void OnTick()
			{
				try
				{
					if ( !m_Target.Deleted )
						RegionsEngine.MakeGuard( m_Target );
						
				}
				catch ( Exception e )
				{
					Console.WriteLine( e.ToString() );
				}
			}
		}
	}
	
	#region straznik podstawowy
	
	[CorpseName( "zwloki straznika" )]
	public class StandardNelderimGuard : BaseNelderimGuard
	{
		[Constructable]
		public StandardNelderimGuard() : base ( GuardType.StandardGuard ) {}
	
		public StandardNelderimGuard(Serial serial) : base(serial)
		{
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
	
	[CorpseName( "zwloki straznika" )]
	public class Guard : BaseNelderimGuard
	{
		[Constructable]
		public Guard() : base ( GuardType.StandardGuard ) {}
	
		public Guard(Serial serial) : base(serial)
		{
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
	
	#endregion

    #region straznik czarujacy
    [CorpseName( "zwloki straznika" )]
    public class MageNelderimGuard : BaseNelderimGuard
    {
        public override void AddWeaponAbilities()
        {
            WeaponAbilities.Add( WeaponAbility.ParalyzingBlow, 0.225 );
            WeaponAbilities.Add( WeaponAbility.Disarm, 0.225 );
        }

        [Constructable]
        public MageNelderimGuard() : base( GuardType.MageGuard, FightMode.Criminal ) { }

        public MageNelderimGuard( Serial serial ) : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }
    #endregion

    #region straznik ciezki
    [CorpseName( "zwloki straznika" )]
    public class HeavyNelderimGuard : BaseNelderimGuard
    {
        public override void AddWeaponAbilities()
        {
            WeaponAbilities.Add( WeaponAbility.WhirlwindAttack, 0.225 );
            WeaponAbilities.Add( WeaponAbility.BleedAttack, 0.225 );
        }

        [Constructable]
        public HeavyNelderimGuard() : base( GuardType.HeavyGuard, FightMode.Criminal ) { }

        public HeavyNelderimGuard( Serial serial ) : base( serial )
        {
        }

        public override void Serialize( GenericWriter writer )
        {
            base.Serialize( writer );

            writer.Write( (int)0 );
        }

        public override void Deserialize( GenericReader reader )
        {
            base.Deserialize( reader );

            int version = reader.ReadInt();
        }
    }
    #endregion

    #region straznik konny

    [CorpseName( "zwloki straznika" )]
	public class MountedNelderimGuard : BaseNelderimGuard
	{
        public override void AddWeaponAbilities()
        {
            WeaponAbilities.Add( WeaponAbility.Disarm, 0.225 );
            WeaponAbilities.Add( WeaponAbility.BleedAttack, 0.225 );
        }

        [Constructable]
        public MountedNelderimGuard() : base( GuardType.MountedGuard, FightMode.Criminal ) { }
	
		public MountedNelderimGuard(Serial serial) : base(serial)
		{
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
	
	[CorpseName( "zwloki straznika" )]
	public class MGuard : BaseNelderimGuard
	{
        public override void AddWeaponAbilities()
        {
            WeaponAbilities.Add( WeaponAbility.Disarm, 0.225 );
            WeaponAbilities.Add( WeaponAbility.BleedAttack, 0.225 );
        }

		[Constructable]
        public MGuard() : base( GuardType.MountedGuard, FightMode.Criminal ) { }
	
		public MGuard(Serial serial) : base(serial)
		{
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
	
	#endregion
	
	#region straznik lucznik
	
	[CorpseName( "zwloki straznika" )]
	public class ArcherNelderimGuard : BaseNelderimGuard
	{
        public override void AddWeaponAbilities()
        {
            WeaponAbilities.Add( WeaponAbility.ParalyzingBlow, 0.2 );
            WeaponAbilities.Add( WeaponAbility.ArmorIgnore, 0.2 );
        }

		[Constructable]
        public ArcherNelderimGuard() : base( GuardType.ArcherGuard, FightMode.Criminal ) { }
	
		public ArcherNelderimGuard(Serial serial) : base(serial)
		{
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
	
	[CorpseName( "zwloki straznika" )]
	public class AGuard : BaseNelderimGuard
	{
        public override void AddWeaponAbilities()
        {
            WeaponAbilities.Add( WeaponAbility.ParalyzingBlow, 0.2 );
            WeaponAbilities.Add( WeaponAbility.ArmorIgnore, 0.2 );
        }

		[Constructable]
        public AGuard() : base( GuardType.ArcherGuard, FightMode.Criminal ) { }
	
		public AGuard(Serial serial) : base(serial)
		{
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
	
	#endregion
	
	#region straznik elitarny
	
	[CorpseName( "zwloki straznika" )]
	public class EliteNelderimGuard : BaseNelderimGuard
	{
        public override void AddWeaponAbilities()
        {
            WeaponAbilities.Add( WeaponAbility.WhirlwindAttack, 0.25 );
            WeaponAbilities.Add( WeaponAbility.BleedAttack, 0.25 );
        }

		[Constructable]
		public EliteNelderimGuard() : base ( GuardType.EliteGuard, FightMode.Criminal ) {}
	
		public EliteNelderimGuard(Serial serial) : base(serial)
		{
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
	
	[CorpseName( "zwloki straznika" )]
	public class EGuard : BaseNelderimGuard
	{
        public override void AddWeaponAbilities()
        {
            WeaponAbilities.Add( WeaponAbility.WhirlwindAttack, 0.25 );
            WeaponAbilities.Add( WeaponAbility.BleedAttack, 0.25 );
        }

		[Constructable]
		public EGuard() : base ( GuardType.EliteGuard, FightMode.Criminal ) {}
	
		public EGuard(Serial serial) : base(serial)
		{
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
	
	#endregion
	
	#region straznik specjalny
	
	[CorpseName( "zwloki straznika" )]
	public class SpecialNelderimGuard : BaseNelderimGuard
	{
        public override void AddWeaponAbilities()
        {
            WeaponAbilities.Add( WeaponAbility.Disarm, 0.5 );
            WeaponAbilities.Add( WeaponAbility.BleedAttack, 0.5 );
        }

		[Constructable]
		public SpecialNelderimGuard() : base ( GuardType.SpecialGuard ) {}
	
		public SpecialNelderimGuard(Serial serial) : base(serial)
		{
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
	
	[CorpseName( "zwloki straznika" )]
	public class SGuard : BaseNelderimGuard
	{
        public override void AddWeaponAbilities()
        {
            WeaponAbilities.Add( WeaponAbility.Disarm, 0.5 );
            WeaponAbilities.Add( WeaponAbility.BleedAttack, 0.5 );
        }

        [Constructable]
		public SGuard() : base ( GuardType.SpecialGuard ) {}
	
		public SGuard(Serial serial) : base(serial)
		{
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
	
	#endregion
}

