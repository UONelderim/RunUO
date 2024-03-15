using System;
using Server;
using System.Collections;
using Server.Items;
using Server.Targeting;
using Server.Misc;

namespace Server.Mobiles
{
	[CorpseName( "pajecze zwloki" )]
	public class PhaseSpider : BaseCreature
	{
		[Constructable]
		public PhaseSpider () : base( AIType.AI_BattleMage, FightMode.Closest, 12, 1, 0.2, 0.4 )
		{
			Name = "Pajak Przenikajacy";
			Body = 20;
			BaseSoundID = 0x388;
			Tamable = false;
			Hue = 0x8000 + 0x4000 + 0x1800 + 0xF8;

			SetStr( 166, 195 );
			SetDex( 180, 200);
			SetInt( 46, 70 );

			SetHits( 200, 300 );
			SetMana( 500 );

			SetDamage( 20, 30 );

			SetDamageType( ResistanceType.Physical, 50 );
			SetDamageType( ResistanceType.Energy, 50 );

			SetResistance( ResistanceType.Physical, 55, 65 );
			SetResistance( ResistanceType.Fire, 15, 25 );
			SetResistance( ResistanceType.Cold, 15, 25 );
			SetResistance( ResistanceType.Poison, 99, 120 );
			SetResistance( ResistanceType.Energy, 80,85 );

			SetSkill( SkillName.MagicResist, 70.0, 90.0 );
			SetSkill( SkillName.Tactics, 60.1, 70.0 );
			SetSkill( SkillName.Wrestling, 90.0, 120.0 );
			SetSkill( SkillName.Magery, 100.0, 120.0 );
			SetSkill( SkillName.EvalInt, 78.1, 100.0 );
			SetSkill( SkillName.Meditation, 100.0 );

		

			Fame = 3000;
			Karma = -3000;

			

		}
		public override void GenerateLoot()
		{
			AddLoot( LootPack.FilthyRich, 2 );
		}

		public override bool AutoDispel{ get{ return true; } }
		public override Poison PoisonImmune{ get{ return Poison.Lethal; } }


		public void DrainLife()
		{
			ArrayList list = new();

			IPooledEnumerable eable = GetMobilesInRange( 2 );
			foreach ( Mobile m in eable )
			{
				if ( m == this || !CanBeHarmful( m ) )
					continue;

                if (m is BaseCreature creature && (creature.Controlled || creature.Summoned || creature.Team != this.Team))
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

				m.SendMessage( "{Pajecza magia wysysa zycie z Ciebie!}" );

				int toDrain = Utility.RandomMinMax( 5, 15 );

				Hits += toDrain;
				m.Damage( toDrain, this );
			}
		}

		public override void OnGaveMeleeAttack( Mobile defender )
		{
			base.OnGaveMeleeAttack( defender );

			if ( 0.1 >= Utility.RandomDouble() )
				DrainLife();
		}

		public override void OnGotMeleeAttack( Mobile attacker )
		{
			base.OnGotMeleeAttack( attacker );

			if ( 0.1 >= Utility.RandomDouble() )
				DrainLife();
		}
		
		private DateTime m_NextAbilityTime;

		public override void OnThink()
		{
			if ( DateTime.Now >= m_NextAbilityTime )
			{
				Mobile combatant = this.Combatant;

				if ( combatant != null && combatant.Map == this.Map && combatant.InRange( this, 12 ) && IsEnemy( combatant ) && !UnderEffect( combatant ) )
				{
					m_NextAbilityTime = DateTime.Now + TimeSpan.FromSeconds( Utility.RandomMinMax( 20, 30 ) );

					

					this.Say( true, "niedojrzale potomstwo pajaka zaczyna gryzc" );

					m_Table[combatant] = Timer.DelayCall( TimeSpan.FromSeconds( 0.5 ), TimeSpan.FromSeconds( 7.0 ), new TimerStateCallback( DoEffect ), new object[]{ combatant, 0 } );
				}
			}

			base.OnThink();
		}

		private static readonly Hashtable m_Table = new();

		public static bool UnderEffect( Mobile m )
		{
			return m_Table.Contains( m );
		}

		public static void StopEffect( Mobile m, bool message )
		{
			Timer t = (Timer)m_Table[m];

			if ( t != null )
			{
				if ( message )
					m.PublicOverheadMessage( Network.MessageType.Emote, m.SpeechHue, true, "*Otwarty ogien wypala potomstwo pajaka *" );

				t.Stop();
				m_Table.Remove( m );
			}
		}

		public void DoEffect( object state )
		{
			object[] states = (object[])state;

			Mobile m = (Mobile)states[0];
			int count = (int)states[1];

			if ( !m.Alive )
			{
				StopEffect( m, false );
			}
			else
			{
                if (m.FindItemOnLayer(Layer.TwoHanded) is Torch torch && torch.Burning)
                {
                    StopEffect(m, true);
                }
                else
                {
                    if ((count % 4) == 0)
                    {
                        m.LocalOverheadMessage(Network.MessageType.Emote, m.SpeechHue, true, "* niedojrzale potomstwo pajaka zaczyna gryzc *");
                        m.NonlocalOverheadMessage(Network.MessageType.Emote, m.SpeechHue, true, String.Format("* {0} jest oszolomiony przez niedojrzale potomstwo pajaka*", m.Name));
                    }

                    m.FixedParticles(0x91C, 10, 180, 9539, EffectLayer.Waist);
                    m.PlaySound(0x00E);
                    m.PlaySound(0x1BC);

                    AOS.Damage(m, this, Utility.RandomMinMax(30, 40) - (Core.AOS ? 0 : 10), 100, 0, 0, 0, 0);

                    states[1] = count + 1;

                    if (!m.Alive)
                        StopEffect(m, false);
                }
            }
		}


		public PhaseSpider( Serial serial ) : base( serial )
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