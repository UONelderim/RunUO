using System;
using Server;
using Server.Items;
using Server.Multis;
using Server.Mobiles;
using Server.Network;
using Server.Targeting;

namespace Server.Spells.Chivalry
{
	public class SacredJourneySpell : PaladinSpell
	{
        // 05.07.2012 :: zombie :: wymaga PowderOfTranslocation
		private static SpellInfo m_Info = new SpellInfo(
				"Lotem ptaka", "Powoli rozpływa się w powietrzu",
				-1,
				9002
                //new int[]{ 1 },
                //Reagent.PowderOfTranslocation
			);

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 1.5 ); } }

		public override double RequiredSkill{ get{ return 15.0; } }
		public override int RequiredMana{ get{ return 10; } }
		public override int RequiredTithing{ get{ return 1; } }
		public override int MantraNumber{ get{ return 1060727; } } // Sanctum Viatas (zmienione na: Musca Viatas)
		public override bool BlocksMovement{ get{ return false; } }

		private RunebookEntry m_Entry;
		private Runebook m_Book;

		public SacredJourneySpell( Mobile caster, Item scroll ) : this( caster, scroll, null, null )
		{
		}

		public SacredJourneySpell( Mobile caster, Item scroll, RunebookEntry entry, Runebook book ) : base( caster, scroll, m_Info )
		{
			m_Entry = entry;
			m_Book = book;
		}

		public override void OnCast()
		{
			if ( m_Entry == null )
				Caster.Target = new InternalTarget( this );
			else
				Effect( m_Entry.Location, m_Entry.Map, true );
		}

		public override bool CheckCast()
		{
			if ( !base.CheckCast() )
				return false;

			if ( Factions.Sigil.ExistsOn( Caster ) )
			{
				Caster.SendLocalizedMessage( 1061632 ); // "Nie mozesz tego zrobic niosac Sigil."
				return false;
			}
			else if ( Caster.Criminal )
			{
				Caster.SendLocalizedMessage( 1005561, "", 0x22 ); // "Jestes kryminalista, i nie uciekniesz tak latwo..."
				return false;
			}
			else if ( SpellHelper.CheckCombat( Caster ) )
			{
				Caster.SendLocalizedMessage( 1061282 ); // "Nie mozesz uciec z pola bitwy za pomoca Lotu Ptaka."
				return false;
			}
			else if ( Server.Misc.WeightOverloading.IsOverloaded( Caster ) )
			{
				Caster.SendLocalizedMessage( 502359, "", 0x22 ); // "Jestes zbyt obciazony by podrozowac."
				return false;
			}

			return SpellHelper.CheckTravel( Caster, TravelCheckType.RecallFrom );
		}

		public void Effect( Point3D loc, Map map, bool checkMulti )
		{
			if ( Factions.Sigil.ExistsOn( Caster ) )
			{
				Caster.SendLocalizedMessage( 1061632 ); // "Nie mozesz tego zrobic niosac Sigil."
			}
			else if ( map == null || (!Core.AOS && Caster.Map != map) )
			{
				Caster.SendLocalizedMessage( 1005569 ); // "Nie mozesz sie teleportowac do tak odleglego swiata."
			}
			else if ( !SpellHelper.CheckTravel( Caster, TravelCheckType.RecallFrom ) )
			{
			}
			else if ( !SpellHelper.CheckTravel( Caster, map, loc, TravelCheckType.RecallTo ) )
			{
			}
			else if ( map == Map.Felucca && Caster is PlayerMobile && ((PlayerMobile)Caster).Young )
			{
				Caster.SendLocalizedMessage( 1049543 ); // "Jestes jeszcze mlody, nie powinienes podrozowac do tamtej krainy."
			}
			else if ( Caster.Kills >= 5 && map != Map.Felucca )
			{
				Caster.SendLocalizedMessage( 1019004 ); // "Nie mozesz udac sie do tej lokacji."
			}
			else if ( Caster.Criminal )
			{
				Caster.SendLocalizedMessage( 1005561, "", 0x22 ); // "Jestes kryminalista, i nie uciekniesz tak latwo..."
			}
			else if ( SpellHelper.CheckCombat( Caster ) )
			{
				Caster.SendLocalizedMessage( 1061282 ); // "Nie mozesz uciec z pola bitwy za pomoca Lotu Ptaka."
			}
			else if ( Server.Misc.WeightOverloading.IsOverloaded( Caster ) )
			{
				Caster.SendLocalizedMessage( 502359, "", 0x22 ); // "Jestes zbyt obciazony by podrozowac."
			}
			else if ( !map.CanSpawnMobile( loc.X, loc.Y, loc.Z ) )
			{
				Caster.SendLocalizedMessage( 501942 ); // "Cos blokuje docelowe miejsce podrozy."
			}
			else if ( (checkMulti && SpellHelper.CheckMulti( loc, map )) )
			{
				Caster.SendLocalizedMessage( 501942 ); // "Cos blokuje docelowe miejsce podrozy."
			}
			else if ( m_Book != null && m_Book.CurCharges <= 0 )
			{
				Caster.SendLocalizedMessage( 502412 ); // "W tym przedmiocie skonczyly sie ladunki."
			}
			else if ( CheckSequence() )
			{
				BaseCreature.TeleportPets( Caster, loc, map, true );

				if ( m_Book != null )
					--m_Book.CurCharges;

				Effects.SendLocationParticles( EffectItem.Create( Caster.Location, Caster.Map, EffectItem.DefaultDuration ), 0, 0, 0, 5033 );

				Caster.MoveToWorld( loc, map );
			}

			FinishSequence();
		}

		private class InternalTarget : Target
		{
			private SacredJourneySpell m_Owner;

			public InternalTarget( SacredJourneySpell owner ) : base( 12, false, TargetFlags.None )
			{
				m_Owner = owner;
			}

			protected override void OnTarget( Mobile from, object o )
			{
				if ( o is RecallRune )
				{
					RecallRune rune = (RecallRune)o;

					if ( rune.Marked )
						m_Owner.Effect( rune.Target, rune.TargetMap, true );
					else
						from.SendLocalizedMessage( 501805 ); // "Ta runa nie jest zaznaczona."
				}
				else if ( o is Runebook )
				{
					RunebookEntry e = ((Runebook)o).Default;

					if ( e != null )
						m_Owner.Effect( e.Location, e.Map, true );
					else
						from.SendLocalizedMessage( 502354 ); // "Cel nie jest oznaczony."
				}
				else if ( o is Key && ((Key)o).KeyValue != 0 && ((Key)o).Link is BaseBoat )
				{
					BaseBoat boat = ((Key)o).Link as BaseBoat;

					if ( !boat.Deleted && boat.CheckKey( ((Key)o).KeyValue ) )
						m_Owner.Effect( boat.GetMarkedLocation(), boat.Map, false );
					else
						from.Send( new MessageLocalized( from.Serial, from.Body, MessageType.Regular, 0x3B2, 3, 502357, from.Name, "" ) ); // "Nie moge wyznaczyc celu na podstawie tego przedmiotu."
				}
				else
				{
					from.Send( new MessageLocalized( from.Serial, from.Body, MessageType.Regular, 0x3B2, 3, 502357, from.Name, "" ) ); // "Nie moge wyznaczyc celu na podstawie tego przedmiotu."
				}
			}

			protected override void OnTargetFinish( Mobile from )
			{
				m_Owner.FinishSequence();
			}
		}
	}
}