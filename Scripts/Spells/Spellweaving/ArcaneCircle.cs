using System;
using System.Collections.Generic;
using Server.Items;
using Server.Mobiles;

namespace Server.Spells.Spellweaving
{
	public class ArcaneCircleSpell : ArcanistSpell
	{
		private static SpellInfo m_Info = new SpellInfo(
				"Arcane Circle", "Myrshalee",
				-1
			);

		public override TimeSpan CastDelayBase { get { return TimeSpan.FromSeconds( 0.5 ); } }

		public override double RequiredSkill { get { return 0.0; } }
		public override int RequiredMana { get { return 24; } }

		public ArcaneCircleSpell( Mobile caster, Item scroll )
			: base( caster, scroll, m_Info )
		{
		}

		public override bool CheckCast()
		{
			if( !IsValidLocation( Caster.Location, Caster.Map ) )
			{
				Caster.SendLocalizedMessage( 1072705 ); // You must be standing on an arcane circle, pentagram or abbatoir to use this spell.
				return false;
			}

			//if (GetArcanists().Count < 2) {
			//	Caster.SendLocalizedMessage(1080452); //There are not enough spellweavers present to create an Arcane Focus.
			//	return false;
			//}

			return base.CheckCast();
		}

		public override void OnCast()
		{
			if( CheckSequence() )
			{
				Caster.FixedParticles( 0x3779, 10, 20, 0x0, EffectLayer.Waist );
				Caster.PlaySound( 0x5C0 );

				List<Mobile> Arcanists = GetArcanists();

				TimeSpan duration = TimeSpan.FromHours( Math.Max( 1, (int)(Caster.Skills.Spellweaving.Value / 24) ) );

				duration += TimeSpan.FromHours(Math.Min(6, Arcanists.Count));

				int strengthBonus = Math.Min( Arcanists.Count, IsBonus( Caster.Location, Caster.Map ) ? 6 : 5 );	//The Sanctuary is a special, single location place

				for( int i = 0; i < Arcanists.Count; i++ )
					GiveArcaneFocus( Arcanists[i], duration, strengthBonus );
			}

			FinishSequence();
		}

		private static bool IsBonus(Point3D p, Map m) {
			return false;
			//return (m == Map.Trammel || m == Map.Felucca) &&
			//	(p.X == 6267 && p.Y == 131) ||
			//	(p.X == 6589 && p.Y == 178) ||
			//	(p.X == 1431 && p.Y == 1696); // new brit bank
		}

		private static bool IsValidLocation( Point3D location, Map map )
		{
			Tile lt = map.Tiles.GetLandTile( location.X, location.Y );         // Land   Tiles            

			if( IsValidTile( lt.ID ) && lt.Z == location.Z )
				return true;

			Tile[] tiles = map.Tiles.GetStaticTiles( location.X, location.Y ); // Static Tiles

			for( int i = 0; i < tiles.Length; ++i )
			{
				Tile t = tiles[i];
				ItemData id = TileData.ItemTable[t.ID & 0x3FFF];

				int tand = t.ID & 0x3FFF;

				if( t.Z + id.CalcHeight != location.Z )
					continue;
				else if( IsValidTile( tand ) )
					return true;
			}

			IPooledEnumerable eable = map.GetItemsInRange( location, 0 );      // Added  Tiles

			foreach( Item item in eable )
			{
				if( item == null || item.Z != location.Z )
					continue;
				else if( IsValidTile( item.ItemID ) )
				{
					eable.Free();
					return true;
				}
			}

			eable.Free();
			return false;
		}

		public static bool IsValidTile( int itemID )
		{
			//Per OSI, Center tile only
			return (itemID == 0xFEA || itemID == 0x1216 || itemID == 0x307F || itemID == 0x1D10 || itemID == 0x1D0F || itemID == 0x1D1F || itemID == 0x1D12);   // Pentagram center, Abbatoir center, Arcane Circle Center, Bloody Pentagram has 4 tiles at center
		}

		private List<Mobile> GetArcanists()
		{
			List<Mobile> weavers = new List<Mobile>();

			weavers.Add( Caster );

			//OSI Verified: Even enemies/combatants count
			IPooledEnumerable eable = Caster.GetMobilesInRange( 1 );
			foreach( Mobile m in eable )	//Range verified as 1
			{
				if(m != Caster && Caster.CanBeBeneficial(m, false) && m.Skills.Spellweaving.Value >= 10.0 && !(m is Clone))	
				{
					weavers.Add( m );
				}
				// Everyone gets the Arcane Focus, power capped elsewhere
			}
			eable.Free();

			return weavers;
		}

		private void GiveArcaneFocus( Mobile to, TimeSpan duration, int strengthBonus )
		{
			if( to == null )	//Sanity
				return;

			ArcaneFocus focus = FindArcaneFocus( to );

			if( focus == null )
			{
				ArcaneFocus f = new ArcaneFocus( duration, strengthBonus );
				if( to.PlaceInBackpack( f ) )
				{
					f.SendTimeRemainingMessage( to );
					to.SendLocalizedMessage( 1072740 ); // An arcane focus appears in your backpack.
				}
				else
				{
					f.Delete();
				}

			}
			else		//OSI renewal rules: the new one will override the old one, always.
			{
				to.SendLocalizedMessage( 1072828 ); // Your arcane focus is renewed.
				focus.LifeSpan = duration;
				focus.CreationTime = DateTime.Now;
				focus.StrengthBonus = strengthBonus;
				focus.InvalidateProperties();
				focus.SendTimeRemainingMessage( to );
			}
		}
	}
}