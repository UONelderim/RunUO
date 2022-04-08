using System;
using System.Collections;
using Server.Network;
using Server.Items;
using Server.Targeting;

namespace Server.Spells.Chivalry
{
	public class CleanseByFireSpell : PaladinSpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Wrzaca krew", "Postaæ staje w p³omieniach, a jego krew oczyszcza siê z trucizn",
				-1,
				9002
			);

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 1.0 ); } }

		public override double RequiredSkill{ get{ return 5.0; } }
		public override int RequiredMana{ get{ return 10; } }
		public override int RequiredTithing{ get{ return 4; } }
		public override int MantraNumber{ get{ return 1060718; } } // Expor Flamus (zmienione na: Sanguis Flamus)

		public CleanseByFireSpell( Mobile caster, Item scroll ) : base( caster, scroll, m_Info )
		{
		}

		public override void OnCast()
		{
			Caster.Target = new InternalTarget( this );
		}

		public void Target( Mobile m )
		{
			if ( !m.Poisoned )
			{
				Caster.SendLocalizedMessage( 1060176 ); // "Cel wcale nie jest zatruty!"
			}
			else if ( CheckBSequence( m ) )
			{
				SpellHelper.Turn( Caster, m );

				/* Cures the target of poisons, but causes the caster to be burned by fire damage for 13-55 hit points.
				 * The amount of fire damage is lessened if the caster has high Karma.
				 */

				Poison p = m.Poison;

				if ( p != null )
				{
					// Cleanse by fire is now difficulty based 
					int chanceToCure = 10000 + (int)(Caster.Skills[SkillName.Chivalry].Value * 75) - ((p.Level + 1) * 2000);
					chanceToCure /= 100;

					if ( chanceToCure > Utility.Random( 100 ) )
					{
						if ( m.CurePoison( Caster ) )
						{
							if ( Caster != m )
								Caster.SendLocalizedMessage( 1010058 ); // "Usunales trucizne z ciala pacjenta."

							m.SendLocalizedMessage( 1010059 ); // "Trucizna zostala usunieta z twojego ciala."
						}
					}
					else
					{
						m.SendLocalizedMessage( 1010060 ); // "Nie udalo ci sie odtruc pacjenta."
					}
				}

				m.PlaySound( 0x1E0 );
				m.FixedParticles( 0x373A, 1, 15, 5012, 3, 2, EffectLayer.Waist );

				IEntity from = new Entity( Serial.Zero, new Point3D( m.X, m.Y, m.Z - 5 ), m.Map );
				IEntity to = new Entity( Serial.Zero, new Point3D( m.X, m.Y, m.Z + 45 ), m.Map );
				Effects.SendMovingParticles( from, to, 0x374B, 1, 0, false, false, 63, 2, 9501, 1, 0, EffectLayer.Head, 0x100 );

				Caster.PlaySound( 0x208 );
				Caster.FixedParticles( 0x3709, 1, 30, 9934, 0, 7, EffectLayer.Waist );

				int damage = 50 - ComputePowerValue( 4 );

				// TODO: Should caps be applied?
				if ( damage < 13 )
					damage = 13;
				else if ( damage > 55 )
					damage = 55;

				AOS.Damage( Caster, Caster, damage, 0, 100, 0, 0, 0, true );
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private CleanseByFireSpell m_Owner;

			public InternalTarget( CleanseByFireSpell owner ) : base( 12, false, TargetFlags.Beneficial )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is Mobile )
					m_Owner.Target( (Mobile) o );
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}