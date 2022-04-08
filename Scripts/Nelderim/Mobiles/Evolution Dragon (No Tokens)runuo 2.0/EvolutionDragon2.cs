    //////////////////////////////////
   //			           //
  //      Scripted by Raelis      //
 //		             	 //
//////////////////////////////////
using System;
using System.Collections; 
using Server.Mobiles;
using Server.Items;
using Server.Network; 
using Server.Targeting;
using Server.Gumps;

namespace Server.Mobiles
{
	[CorpseName( "zwloki smoka" )]
	public class EvolutionDragon2 : BaseCreature
	{
		private Timer m_BreatheTimer;
		private DateTime m_EndBreathe;
		private Timer m_MatingTimer;
		private DateTime m_EndMating;

		private Timer m_PetLoyaltyTimer;
		private DateTime m_EndPetLoyalty;

		public DateTime EndMating{ get{ return m_EndMating; } set{ m_EndMating = value; } }

		public DateTime EndPetLoyalty{ get{ return m_EndPetLoyalty; } set{ m_EndPetLoyalty = value; } }

		public int m_Stage;
		public int m_KP;
		public bool m_AllowMating;
		public bool m_HasEgg;
		public bool m_Pregnant;

		public bool m_S1;
		public bool m_S2;
		public bool m_S3;
		public bool m_S4;
		public bool m_S5;
		public bool m_S6;

		public bool S1
		{
			get{ return m_S1; }
			set{ m_S1 = value; }
		}
		public bool S2
		{
			get{ return m_S2; }
			set{ m_S2 = value; }
		}
		public bool S3
		{
			get{ return m_S3; }
			set{ m_S3 = value; }
		}
		public bool S4
		{
			get{ return m_S4; }
			set{ m_S4 = value; }
		}
		public bool S5
		{
			get{ return m_S5; }
			set{ m_S5 = value; }
		}
		public bool S6
		{
			get{ return m_S6; }
			set{ m_S6 = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool AllowMating
		{
			get{ return m_AllowMating; }
			set{ m_AllowMating = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public bool HasEgg
		{
			get{ return m_HasEgg; }
			set{ m_HasEgg = value; }
		}
		[CommandProperty( AccessLevel.GameMaster )]
		public bool Pregnant
		{
			get{ return m_Pregnant; }
			set{ m_Pregnant = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int KP
		{
			get{ return m_KP; }
			set{ m_KP = value; }
		}

		[CommandProperty( AccessLevel.GameMaster )]
		public int Stage
		{
			get{ return m_Stage; }
			set{ m_Stage = value; }
		}

		[Constructable]
		public EvolutionDragon2() : base( AIType.AI_Mage, FightMode.Closest, 10, 1, 0.2, 0.4 )
		{
			Female = Utility.RandomBool();
			Name = "Smocza Larwa";
			Body = 52;
			Hue = 1346;
			BaseSoundID = 0xDB;
			Stage = 1;

			S1 = true;
			S2 = true;
			S3 = true;
			S4 = true;
			S5 = true;
			S6 = true;

			SetStr( 300, 350 );
			SetDex( 200, 250 );
			SetInt( 150, 200 );
			
			SetHits( 300, 350 );

			SetDamage( 10, 15 );

			SetDamageType( ResistanceType.Cold, 100 );
			SetDamageType( ResistanceType.Physical, 0 );

			SetResistance( ResistanceType.Physical, 35, 45 );
			SetResistance( ResistanceType.Cold, 55, 60 );
			SetResistance( ResistanceType.Fire, 0, 5 );
			SetResistance( ResistanceType.Poison, 15, 25 );
			SetResistance( ResistanceType.Energy, 0, 10 );

			SetSkill( SkillName.Magery, 50.1, 70.0 );
			SetSkill( SkillName.Meditation, 50.1, 70.0 );
			SetSkill( SkillName.EvalInt, 50.1, 70.0 );
			SetSkill( SkillName.MagicResist, 15.1, 20.0 );
			SetSkill( SkillName.Tactics, 19.3, 34.0 );
			SetSkill( SkillName.Wrestling, 19.3, 34.0 );
			SetSkill( SkillName.Anatomy, 19.3, 34.0 );

			Fame = 300;
			Karma = -300;

			VirtualArmor = 30;

            Tamable = true;
			ControlSlots = 3;
			MinTameSkill = 90.9;
			

			m_PetLoyaltyTimer = new PetLoyaltyTimer2( this, TimeSpan.FromSeconds( 5.0 ) );
			m_PetLoyaltyTimer.Start();
			m_EndPetLoyalty = DateTime.Now + TimeSpan.FromSeconds( 5.0 );
		}

		public EvolutionDragon2(Serial serial) : base(serial)
		{
		}

		public override void Damage( int amount, Mobile defender )
		{
			int kpgainmin, kpgainmax;

			if ( this.Stage == 1 )
			{
				if ( defender is BaseCreature )
				{
					BaseCreature bc = (BaseCreature)defender;

					if ( bc.Controlled != true )
					{
						kpgainmin = 5 + ( bc.HitsMax ) / 10;
						kpgainmax = 5 + ( bc.HitsMax ) / 5;

						this.KP += Utility.RandomList( kpgainmin, kpgainmax );
					}
				}

				if ( this.KP >= 25000 )
				{
					if ( this.S1 == true )
					{
						this.S1 = false;
						int hits, va, mindamage, maxdamage;

						hits = ( this.HitsMax + 50 );

						va = ( this.VirtualArmor + 10 );

						mindamage = this.DamageMin + ( 5 );
						maxdamage = this.DamageMax + ( 5 );

						this.Warmode = false;
						this.Say( "*"+ this.Name +" dorasta*");
			                        this.Name = "Smocza Larwa";
						this.SetDamage( mindamage, maxdamage );
						this.SetHits( hits );
						this.BodyValue = 89;
						this.BaseSoundID = 219;
						this.VirtualArmor = va;
                        Hue = 1346;
						this.Stage = 2;
						ControlSlots = 3;
						MinTameSkill = 94.9;

						this.SetDamageType( ResistanceType.Physical, 0 );
						this.SetDamageType( ResistanceType.Fire, 0 );
						this.SetDamageType( ResistanceType.Cold, 100 );
						this.SetDamageType( ResistanceType.Poison, 0 );
						this.SetDamageType( ResistanceType.Energy, 0 );
						
			            this.SetResistance( ResistanceType.Physical, 35, 45 );
			            this.SetResistance( ResistanceType.Cold, 55, 60 );
			            this.SetResistance( ResistanceType.Fire, 0, 5 );
			            this.SetResistance( ResistanceType.Poison, 15, 25 );
			            this.SetResistance( ResistanceType.Energy, 0, 10 );
                        
						
						this.RawStr += 100;
						this.RawInt += 15;
						this.RawDex += 15;
					}
				}
			}

			else if ( this.Stage == 2 )
			{
				if ( defender is BaseCreature )
				{
					BaseCreature bc = (BaseCreature)defender;

					if ( bc.Controlled != true )
					{
						kpgainmin = 5 + ( bc.HitsMax ) / 20;
						kpgainmax = 5 + ( bc.HitsMax ) / 10;

						this.KP += Utility.RandomList( kpgainmin, kpgainmax );
					}
				}

				if ( this.KP >= 75000 )
				{
					if ( this.S2 == true )
					{
						this.S2 = false;
						int hits, va, mindamage, maxdamage;

						hits = ( this.HitsMax + 120 );

						va = ( this.VirtualArmor + 10 );

						mindamage = this.DamageMin + ( 4 );
						maxdamage = this.DamageMax + ( 4 );

						this.Warmode = false;
						this.Say( "*"+ this.Name +" dorasta*");
						this.Name = "Smoczy Jaszczur";
						this.SetDamage( mindamage, maxdamage );
						this.SetHits( hits );
						this.BodyValue = 0xCE;
						this.BaseSoundID = 0x5A;
						this.VirtualArmor = va;
						this.Stage = 3;
						Hue = 1365;
						ControlSlots = 3;
						MinTameSkill = 96.9;

            			this.SetDamageType( ResistanceType.Physical, 0 );
						this.SetDamageType( ResistanceType.Fire, 0 );
						this.SetDamageType( ResistanceType.Cold, 100 );
						this.SetDamageType( ResistanceType.Poison, 0 );
						this.SetDamageType( ResistanceType.Energy, 0 );

			            this.SetResistance( ResistanceType.Physical, 45, 55 );
			            this.SetResistance( ResistanceType.Cold, 60, 65 );
			            this.SetResistance( ResistanceType.Fire, 5, 10 );
			            this.SetResistance( ResistanceType.Poison, 25, 35 );
			            this.SetResistance( ResistanceType.Energy, 10, 20 );
                        

						this.RawStr += 100;
						this.RawInt += 20;
						this.RawDex += 10;
					}
				}
			}

			else if ( this.Stage == 3 )
			{
				if ( defender is BaseCreature )
				{
					BaseCreature bc = (BaseCreature)defender;

					if ( bc.Controlled != true )
					{
						kpgainmin = 5 + ( bc.HitsMax ) / 30;
						kpgainmax = 5 + ( bc.HitsMax ) / 20;

						this.KP += Utility.RandomList( kpgainmin, kpgainmax );
					}
				}

				if ( this.KP >= 175000 )
				{
					if ( this.S3 == true )
					{
						this.S3 = false;
						int hits, va, mindamage, maxdamage;

						hits = ( this.HitsMax + 70 );

						va = ( this.VirtualArmor + 20 );

						mindamage = this.DamageMin + ( 5 );
						maxdamage = this.DamageMax + ( 5 );

						this.Warmode = false;
						this.Say( "*"+ this.Name +" Dorasta*");
						this.Name = "Smocze Piskle";
						this.SetDamage( mindamage, maxdamage );
						this.SetHits( hits );
						this.BodyValue = Utility.RandomList( 60, 61 );
						this.BaseSoundID = 362;
						this.VirtualArmor = va;
						this.Stage = 4;
						Hue = 1365;
						ControlSlots = 3;
						MinTameSkill = 100.9;

            			this.SetResistance( ResistanceType.Physical, 55, 60 );
						this.SetResistance( ResistanceType.Fire, 10, 15 );
						this.SetResistance( ResistanceType.Cold, 65, 70 );
						this.SetResistance( ResistanceType.Poison, 35, 40 );
						this.SetResistance( ResistanceType.Energy, 20, 25 );

						this.RawStr += 90;
						this.RawInt += 50;
						this.RawDex += 70;
					}
				}
			}

			else if ( this.Stage == 4 )
			{
				if ( defender is BaseCreature )
				{
					BaseCreature bc = (BaseCreature)defender;

					if ( bc.Controlled != true )
					{
						kpgainmin = 5 + ( bc.HitsMax ) / 50;
						kpgainmax = 5 + ( bc.HitsMax ) / 40;

						this.KP += Utility.RandomList( kpgainmin, kpgainmax );
					}
				}

				if ( this.KP >= 3750000 )
				{
					if ( this.S4 == true )
					{
						this.S4 = false;
						int hits, va, mindamage, maxdamage;

						hits = ( this.HitsMax + 230 );

						va = ( this.VirtualArmor + 18 );

						mindamage = this.DamageMin + ( 7 );
						maxdamage = this.DamageMax + ( 7 );

						this.Warmode = false;
						this.Say( "*"+ this.Name +" Dorasta*");
						this.Name = "Smok";
						this.SetDamage( mindamage, maxdamage );
						this.SetHits( hits );
						this.BodyValue = 59;
						this.VirtualArmor = va;
						this.Stage = 5;
						Hue = 1365;
						ControlSlots = 3;
						MinTameSkill = 102.9;

            			this.SetResistance( ResistanceType.Physical, 60, 65 );
						this.SetResistance( ResistanceType.Fire, 15, 20 );
						this.SetResistance( ResistanceType.Cold, 70, 75 );
						this.SetResistance( ResistanceType.Poison, 40, 45 );
						this.SetResistance( ResistanceType.Energy, 25, 30 );

						this.RawStr += 320;
						this.RawInt += 40;
						this.RawDex += 310;
					}
				}
			}

			else if ( this.Stage == 5 )
			{
				if ( defender is BaseCreature )
				{
					BaseCreature bc = (BaseCreature)defender;

					if ( bc.Controlled != true )
					{
						kpgainmin = 5 + ( bc.HitsMax ) / 160;
						kpgainmax = 5 + ( bc.HitsMax ) / 100;

						this.KP += Utility.RandomList( kpgainmin, kpgainmax );
					}
				}

				if ( this.KP >= 7750000 )
				{
					if ( this.S5 == true )
					{
						this.S5 = false;
						int hits, va, mindamage, maxdamage;

						hits = ( this.HitsMax + 200 );

						va = ( this.VirtualArmor + 15 );

						mindamage = this.DamageMin + ( 7 );
						maxdamage = this.DamageMax + ( 7 );

						this.AllowMating = true;
						this.Warmode = false;
						this.Say( "*"+ this.Name +" dorasta*");
						this.Name = "Dorosly Smok";
						this.SetDamage( mindamage, maxdamage );
						this.SetHits( hits );
						this.BodyValue = 46;
						this.VirtualArmor = va;
						this.Stage = 6;
						Hue = 1365;
						ControlSlots = 4;
						MinTameSkill = 105.9;

            			this.SetResistance( ResistanceType.Physical, 65, 66 );
						this.SetResistance( ResistanceType.Fire, 20, 22 );
						this.SetResistance( ResistanceType.Cold, 75, 80 );
						this.SetResistance( ResistanceType.Poison, 45, 46 );
						this.SetResistance( ResistanceType.Energy, 30, 32 );

						this.RawStr += 100;
						this.RawInt += 120;
						this.RawDex += 20;
					}
				}
			}

			else if ( this.Stage == 6 )
			{
				if ( defender is BaseCreature )
				{
					BaseCreature bc = (BaseCreature)defender;

					if ( bc.Controlled != true )
					{
						kpgainmin = 5 + ( bc.HitsMax ) / 540;
						kpgainmax = 5 + ( bc.HitsMax ) / 480;

						this.KP += Utility.RandomList( kpgainmin, kpgainmax );
					}
				}

				if ( this.KP >= 15000000 )
				{
					if ( this.S6 == true )
					{
						this.S6 = false;
						int hits, va, mindamage, maxdamage;

						hits = ( this.HitsMax + 200 );

						va = ( this.VirtualArmor + 20 );

						mindamage = this.DamageMin + ( 12 );
						maxdamage = this.DamageMax + ( 12 );

						this.Warmode = false;
						this.Say( "*"+ this.Name +" jest teraz starozytnym smokiem*");
						this.Title = " - Starozytny";
						this.SetDamage( mindamage, maxdamage );
						this.SetHits( hits );
						this.BodyValue = 172;
						this.VirtualArmor = va;
						this.Stage = 7;
						Hue = 2967;
						ControlSlots = 5;
						MinTameSkill = 115.9;

						SetResistance( ResistanceType.Physical, 66, 68 );
						SetResistance( ResistanceType.Fire, 20, 25 );
						SetResistance( ResistanceType.Cold, 80, 85 );
						SetResistance( ResistanceType.Poison, 46, 50 );
			      		SetResistance( ResistanceType.Energy, 32, 35 );
						
            			this.RawStr += 200;
						this.RawInt += 20;
						this.RawDex += 150;
					}
				}
			}

			else if ( this.Stage == 7 )
			{
				if ( defender is BaseCreature )
				{
					BaseCreature bc = (BaseCreature)defender;

					if ( bc.Controlled != true )
					{
						kpgainmin = 5 + ( bc.Hits ) / 740;
						kpgainmax = 5 + ( bc.Hits ) / 660;

						this.KP += Utility.RandomList( kpgainmin, kpgainmax );
					}
				}
			}

			base.Damage( amount, defender );
		}

		public override bool OnDragDrop( Mobile from, Item dropped )
		{
			PlayerMobile player = from as PlayerMobile;

			if ( player != null )
			{
				if ( dropped is DragonDust )
				{
					DragonDust dust = ( DragonDust )dropped;

					int amount = ( dust.Amount * 5 );

					this.PlaySound( 665 );
					this.KP += amount;
					dust.Delete();
					this.Say( "*"+ this.Name +" pochlona odrzywke*" );

					return false;
				}
				else
				{
				}
			}
			return base.OnDragDrop( from, dropped );
		}


                private void MatingTarget_Callback( Mobile from, object obj ) 
                { 
                           	if ( obj is EvolutionDragon2 && obj is BaseCreature ) 
                           	{ 
					BaseCreature bc = (BaseCreature)obj;
					EvolutionDragon2 ed = (EvolutionDragon2)obj;

					if ( ed.Controlled == true && ed.ControlMaster == from )
					{
						if ( ed.Female == false )
						{
							if ( ed.AllowMating == true )
							{
								this.Blessed = true;
								this.Pregnant = true;

								m_MatingTimer = new MatingTimer2( this, TimeSpan.FromDays( 3.0 ) );
								m_MatingTimer.Start();

								m_EndMating = DateTime.Now + TimeSpan.FromDays( 3.0 );
							}
							else
							{
								from.SendMessage( "Ten samiec jest za mlody!" );
							}
						}
						else
						{
							from.SendMessage( "Ten smok nie jest samcem!" );
						}
					}
					else if ( ed.Controlled == true )
					{
						if ( ed.Female == false )
						{
							if ( ed.AllowMating == true )
							{
								if ( ed.ControlMaster != null )
								{
									ed.ControlMaster.SendGump( new MatingGump2( from, ed.ControlMaster, this, ed ) );
									from.SendMessage( "Pytasz wlasciciela smoka czy zezwoli na zblizenie." );
								}
                           					else
                           					{
                              						from.SendMessage( "Ten Smok jest dziki." );
			   					}
							}
							else
							{
								from.SendMessage( "Ten samiec jest za mlody!" );
							}
						}
						else
						{
							from.SendMessage( "Ten smok nie jest samcem!" );
						}
					}
                           		else 
                           		{ 
                              			from.SendMessage( "Ten Smok jest dziki." );
			   		}
                           	} 
                           	else 
                           	{ 
                              		from.SendMessage( "To nie jest Smok!" );
			   	}
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( this.Controlled == true && this.ControlMaster == from )
			{
				if ( this.Female == true )
				{
					if ( this.AllowMating == true )
					{
						if ( this.Pregnant == true )
						{
							if ( this.HasEgg == true )
							{
								this.HasEgg = false;
								this.Pregnant = false;
								this.Blessed = false;
								from.AddToBackpack( new DragonEgg2() );
								from.SendMessage( "Smocze jajo jest teraz w twoim plecaku" );
							}
							else
							{
								from.SendMessage( "Smok nie zniosl jeszcze jaja." );
							}
						}
						else
						{
							from.SendMessage( "Zaznacz samca by mial zblizenie z samica." );
                                			from.BeginTarget( -1, false, TargetFlags.Harmful, new TargetCallback( MatingTarget_Callback ) );
						}
					}
					else
					{
						from.SendMessage( "Ta samica nie jest wystarczajaco dorosla na zblizenie!" );
					}	
				}
			}
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

			m_BreatheTimer = new BreatheTimer( m, this, this, TimeSpan.FromSeconds( 1.0 ) );
			m_BreatheTimer.Start();
			m_EndBreathe = DateTime.Now + TimeSpan.FromSeconds( 1.0 );

			this.Frozen = true;

			if ( this.Stage == 1 )
			{
				this.MovingParticles( m, 0x1FA8, 1, 0, false, true, ( this.Hue - 1 ), 0, 9502, 6014, 0x11D, EffectLayer.Waist, 0 );
			}
			else if ( this.Stage == 2 )
			{
				this.MovingParticles( m, 0x1FA9, 1, 0, false, true, ( this.Hue - 1 ), 0, 9502, 6014, 0x11D, EffectLayer.Waist, 0 );
			}
			else if ( this.Stage == 3 )
			{
				this.MovingParticles( m, 0x1FAB, 1, 0, false, true, ( this.Hue - 1 ), 0, 9502, 6014, 0x11D, EffectLayer.Waist, 0 );
			}
			else if ( this.Stage == 4 )
			{
				this.MovingParticles( m, 0x1FBC, 1, 0, false, true, ( this.Hue - 1 ), 0, 9502, 6014, 0x11D, EffectLayer.Waist, 0 );
			}
			else if ( this.Stage == 5 )
			{
				this.MovingParticles( m, 0x1FBD, 1, 0, false, true, ( this.Hue - 1 ), 0, 9502, 6014, 0x11D, EffectLayer.Waist, 0 );
			}
			else if ( this.Stage == 6 )
			{
				this.MovingParticles( m, 0x1FBF, 1, 0, false, true, ( this.Hue - 1 ), 0, 9502, 6014, 0x11D, EffectLayer.Waist, 0 );
			}
			else if ( this.Stage == 7 )
			{
				this.MovingParticles( m, 0x1FBE, 1, 0, false, true, ( this.Hue - 1 ), 0, 9502, 6014, 0x11D, EffectLayer.Waist, 0 );
			}
			else
			{
				
				this.PublicOverheadMessage( MessageType.Regular, this.SpeechHue, true, "Please call a GM if you are getting this message, they will fix the breathe, thank you :)", false );
			}
		}

		private class BreatheTimer : Timer
		{
			private EvolutionDragon2 ed;
			private Mobile m_Mobile, m_From;

			public BreatheTimer( Mobile m, EvolutionDragon2 owner, Mobile from, TimeSpan duration ) : base( duration ) 
			{
				ed = owner;
				m_Mobile = m;
				m_From = from;
				Priority = TimerPriority.TwoFiftyMS;
			}

			protected override void OnTick()
			{
				int damagemin = ed.Hits / 20;
				int damagemax = ed.Hits / 25;
				ed.Frozen = false;

				m_Mobile.PlaySound( 0x11D );
				AOS.Damage( m_Mobile, m_From, Utility.RandomMinMax( damagemin, damagemax ), 0, 100, 0, 0, 0 );
				Stop();
			}
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write((int) 1);
                        writer.Write( m_AllowMating ); 
                        writer.Write( m_HasEgg ); 
                        writer.Write( m_Pregnant ); 
                        writer.Write( m_S1 ); 
                        writer.Write( m_S2 ); 
                        writer.Write( m_S3 ); 
                        writer.Write( m_S4 ); 
                        writer.Write( m_S5 ); 
                        writer.Write( m_S6 ); 
			writer.Write( (int) m_KP );
			writer.Write( (int) m_Stage );
			writer.WriteDeltaTime( m_EndMating );
			writer.WriteDeltaTime( m_EndBreathe );
			writer.WriteDeltaTime( m_EndPetLoyalty );
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();

			switch ( version )
			{
				case 1:
				{
                        		m_AllowMating = reader.ReadBool(); 
                        		m_HasEgg = reader.ReadBool();  
                        		m_Pregnant = reader.ReadBool(); 
                        		m_S1 = reader.ReadBool(); 
                        		m_S2 = reader.ReadBool(); 
                        		m_S3 = reader.ReadBool(); 
                        		m_S4 = reader.ReadBool(); 
                        		m_S5 = reader.ReadBool(); 
                        		m_S6 = reader.ReadBool(); 
					m_KP = reader.ReadInt();
					m_Stage = reader.ReadInt();

					m_EndMating = reader.ReadDeltaTime();
					m_MatingTimer = new MatingTimer2( this, m_EndMating - DateTime.Now );
					m_MatingTimer.Start();

					m_EndBreathe = reader.ReadDeltaTime();
					m_BreatheTimer = new BreatheTimer( this, this, this, m_EndBreathe - DateTime.Now );
					m_BreatheTimer.Start();

					m_EndPetLoyalty = reader.ReadDeltaTime();
					m_PetLoyaltyTimer = new PetLoyaltyTimer2( this, m_EndPetLoyalty - DateTime.Now );
					m_PetLoyaltyTimer.Start();

					break;
				}
				case 0:
				{
					TimeSpan durationbreathe = TimeSpan.FromSeconds( 1.0 );
					TimeSpan durationmating = TimeSpan.FromDays( 3.0 );
					TimeSpan durationloyalty = TimeSpan.FromSeconds( 5.0 );

					m_BreatheTimer = new BreatheTimer( this, this, this, durationbreathe );
					m_BreatheTimer.Start();
					m_EndBreathe = DateTime.Now + durationbreathe;

					m_MatingTimer = new MatingTimer2( this, durationmating );
					m_MatingTimer.Start();
					m_EndMating = DateTime.Now + durationmating;

					m_PetLoyaltyTimer = new PetLoyaltyTimer2( this, durationloyalty );
					m_PetLoyaltyTimer.Start();
					m_EndPetLoyalty = DateTime.Now + durationloyalty;

					break;
				}
			}
		}
	}

	public class MatingTimer2 : Timer
	{ 
		private EvolutionDragon2 ed;

		public MatingTimer2( EvolutionDragon2 owner, TimeSpan duration ) : base( duration ) 
		{ 
			Priority = TimerPriority.OneSecond;
			ed = owner;
		}

		protected override void OnTick() 
		{
			ed.Blessed = false;
			ed.HasEgg = true;
			ed.Pregnant = false;
			Stop();
		}
	}
	public class PetLoyaltyTimer2 : Timer
	{ 
		private EvolutionDragon2 ed;

		public PetLoyaltyTimer2( EvolutionDragon2 owner, TimeSpan duration ) : base( duration ) 
		{ 
			Priority = TimerPriority.OneSecond;
			ed = owner;
		}

		protected override void OnTick() 
		{
			ed.Loyalty = 100;

			PetLoyaltyTimer2 lt = new PetLoyaltyTimer2( ed, TimeSpan.FromSeconds( 5.0 ) );
			lt.Start();
			ed.EndPetLoyalty = DateTime.Now + TimeSpan.FromSeconds( 5.0 );

			Stop();
		}
	}
	public class MatingGump2 : Gump
	{
		private Mobile m_From;
		private Mobile m_Mobile;
		private EvolutionDragon2 m_ED1;
		private EvolutionDragon2 m_ED2;

		public MatingGump2( Mobile from, Mobile mobile, EvolutionDragon2 ed1, EvolutionDragon2 ed2 ) : base( 25, 50 )
		{
			Closable = false; 
			Dragable = false; 

			m_From = from;
			m_Mobile = mobile;
			m_ED1 = ed1;
			m_ED2 = ed2;

			AddPage( 0 );

			AddBackground( 25, 10, 420, 200, 5054 );

			AddImageTiled( 33, 20, 401, 181, 2624 );
			AddAlphaRegion( 33, 20, 401, 181 );

			AddLabel( 125, 148, 1152, m_From.Name +" would like to mate "+ m_ED1.Name +" with" );
			AddLabel( 125, 158, 1152, m_ED2.Name +"." );

			AddButton( 100, 50, 4005, 4007, 1, GumpButtonType.Reply, 0 );
			AddLabel( 130, 50, 1152, "Pozwol na zblizenie." );
			AddButton( 100, 75, 4005, 4007, 0, GumpButtonType.Reply, 0 );
			AddLabel( 130, 75, 1152, "Nie zezwalaj na zblizenie." );
		}

		public override void OnResponse( NetState state, RelayInfo info )
		{
			Mobile from = state.Mobile; 

			if ( from == null )
				return;

			if ( info.ButtonID == 0 )
			{
				m_From.SendMessage( m_Mobile.Name +" odrzucil prosbe o zblizenie smokow." );
				m_Mobile.SendMessage( "Odrzuciles prosbe "+ m_From.Name +"." );
			}
			if ( info.ButtonID == 1 )
			{
				m_ED1.Blessed = true;
				m_ED1.Pregnant = true;

				MatingTimer2 mt = new MatingTimer2( m_ED1, TimeSpan.FromDays( 3.0 ) );
				mt.Start();
				m_ED1.EndMating = DateTime.Now + TimeSpan.FromDays( 3.0 );

				m_From.SendMessage( m_Mobile.Name +" zaakceptowal prosbe o zblizenie smokow." );
				m_Mobile.SendMessage( "Zaakceptowales prosbe "+ m_From.Name +"." );
			}
		}
	}
}