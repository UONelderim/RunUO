// 11.07.2012 :: zombie :: przebudowa
using System;

namespace Server.Items
{
	public class FlameSpurtTrap : BaseTrap
	{
		private Item m_Spurt;
		private Timer m_Timer;

        [Constructable]
		public FlameSpurtTrap() : this( 0 )
		{
		}

		[Constructable]
		public FlameSpurtTrap( int level ) : base( 0x1B71, level )
		{
		}

		public virtual void StartTimer()
		{
			if ( m_Timer == null )
				m_Timer = Timer.DelayCall( TimeSpan.FromSeconds( 1.0 ), TimeSpan.FromSeconds( 1.0 ), new TimerCallback( Refresh ) );
		}

		public virtual void StopTimer()
		{
			if ( m_Timer != null )
				m_Timer.Stop();

			m_Timer = null;
		}

		public virtual void CheckTimer()
		{
			Map map = this.Map;

			if ( map != null && map.GetSector( GetWorldLocation() ).Active )
				StartTimer();
			else
				StopTimer();
		}

		public override void OnLocationChange( Point3D oldLocation )
		{
			base.OnLocationChange( oldLocation );

			CheckTimer();
		}

		public override void OnMapChange()
		{
			base.OnMapChange();

			CheckTimer();
		}

		public override void OnSectorActivate()
		{
			base.OnSectorActivate();

			StartTimer();
		}

		public override void OnSectorDeactivate()
		{
			base.OnSectorDeactivate();

			StopTimer();
		}

		public override void OnDelete()
		{
			base.OnDelete();

			if ( m_Spurt != null )
				m_Spurt.Delete();
		}

		public virtual void Refresh()
		{
			if (Deleted)
				return;

			bool foundPlayer = false;
			IPooledEnumerable eable = GetMobilesInRange(3);

			foreach (Mobile mob in eable)
			{
				if (!mob.Player || !mob.Alive || mob.AccessLevel > AccessLevel.Player)
					continue;

				if (((Z + 8) >= mob.Z && (mob.Z + 16) > Z))
				{
					foundPlayer = true;
					break;
				}
			}
			eable.Free();

			if (!foundPlayer)
			{
				if (m_Spurt != null)
					m_Spurt.Delete();

				m_Spurt = null;
			}
			else if (m_Spurt == null || m_Spurt.Deleted)
			{
				m_Spurt = new Static(0x3709);
				m_Spurt.MoveToWorld(Location, Map);

				Effects.PlaySound(GetWorldLocation(), Map, 0x309);
			}
		}

		public FlameSpurtTrap( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 ); // version

			writer.Write( (Item) m_Spurt );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					Item item = reader.ReadItem();

					if ( item != null )
						item.Delete();

					CheckTimer();

					break;
				}
			}
		}
	}
}