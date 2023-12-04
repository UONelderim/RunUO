using System;
using System.Collections.Generic;
using Server.ContextMenus;
using Server.Multis;
using Server.Network;

namespace Server.Items
{
	public class TillerMan : Item
	{
		private BaseBoat m_Boat;

		public TillerMan( BaseBoat boat ) : base( 0x3E4E )
		{
			m_Boat = boat;
			Movable = false;
		}

		public TillerMan( Serial serial ) : base(serial)
		{
		}

		public void SetFacing( Direction dir )
		{
			switch ( dir )
			{
				case Direction.South: ItemID = 0x3E4B; break;
				case Direction.North: ItemID = 0x3E4E; break;
				case Direction.West:  ItemID = 0x3E50; break;
				case Direction.East:  ItemID = 0x3E53; break;
			}
		}

		public override void GetProperties( ObjectPropertyList list )
		{
			base.GetProperties( list );

			list.Add( m_Boat.Status );
		}

		public void Say( int number )
		{
			PublicOverheadMessage( MessageType.Regular, 0x3B2, number );
		}

		public void Say( int number, string args )
		{
			PublicOverheadMessage( MessageType.Regular, 0x3B2, number, args );
		}

		public override void AddNameProperty( ObjectPropertyList list )
		{
			if ( m_Boat != null && m_Boat.ShipName != null )
				list.Add( 1042884, m_Boat.ShipName ); // the tiller man of the ~1_SHIP_NAME~
			else
				base.AddNameProperty( list );
		}

		public override void OnSingleClick( Mobile from )
		{
			if ( m_Boat != null && m_Boat.ShipName != null )
				LabelTo( from, 1042884, m_Boat.ShipName ); // the tiller man of the ~1_SHIP_NAME~
			else
				base.OnSingleClick( from );
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( m_Boat != null && m_Boat.Contains( from ) )
				m_Boat.BeginRename( from );
			else if ( m_Boat != null )
				m_Boat.BeginDryDock( from );
		}

		public override bool OnDragDrop( Mobile from, Item dropped )
		{
			if ( dropped is MapItem && m_Boat != null && m_Boat.CanCommand( from ) && m_Boat.Contains( from ) )
			{
				m_Boat.AssociateMap( (MapItem) dropped );
			}

			return false;
		}

		public override void OnAfterDelete()
		{
			if ( m_Boat != null )
				m_Boat.Delete();
		}
		
		 public override void GetContextMenuEntries(Mobile from, List<ContextMenuEntry> list)
        {
            base.GetContextMenuEntries(from, list);
            if (BaseBoat.FindBoatAt(from, from.Map) != null)
                list.Add(new CleanWayEntry(this));
        }

        private class CleanWayEntry : ContextMenuEntry
        {
            private TillerMan _tillerMan;

            public CleanWayEntry(TillerMan tillerMan) : base(154)
            {
                _tillerMan = tillerMan;
            }

            public override void OnClick()
            {
                var boat = _tillerMan.m_Boat;
                var offset = Math.Max(boat.StarboardOffset.X, boat.PortOffset.X) + 1;
                var eable = boat.GetItemsInRange(Math.Max(boat.TillerManDistance, boat.HoldDistance) + 2);
                List<Corpse> corpses = new List<Corpse>();
                foreach (Item item in eable)
                {
                    if (item is Corpse corpse)
                    {
                       corpses.Add(corpse);
                    }
                }
                if (corpses.Count == 0)
                {
                    _tillerMan.PublicOverheadMessage(MessageType.Regular, 0, true, "Nie ma czego czyscic kapitanie");
                    return;
                }
                foreach (var corpse in corpses)
                {
                    switch (boat.Facing)
                    {
                        case Direction.East:
                        case Direction.West:
                            corpse.Y += corpse.Y > boat.Y ? offset : -offset;
                            break;
                        case Direction.North:
                        case Direction.South:
                            corpse.X += corpse.X > boat.X ? offset : -offset;
                            break;
                    }
                }
                _tillerMan.PublicOverheadMessage(MessageType.Regular, 0, true, "Wyczyscilem poklad kapitanie");
            }
        }

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 0 );//version

			writer.Write( m_Boat );
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			switch ( version )
			{
				case 0:
				{
					m_Boat = reader.ReadItem() as BaseBoat;

					if ( m_Boat == null )
						Delete();

					break;
				}
			}
		}
	}
}