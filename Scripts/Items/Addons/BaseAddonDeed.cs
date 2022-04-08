using System;
using System.Collections.Generic;
using Server;
using Server.Multis;
using Server.Targeting;
using Server.Engines.Craft;

namespace Server.Items
{
	[Flipable( 0x14F0, 0x14EF )]
	public abstract class BaseAddonDeed : Item, ICraftable
	{
		public abstract BaseAddon Addon{ get; }

        private CraftResource m_Resource;

        [CommandProperty(AccessLevel.GameMaster)]
        public CraftResource Resource
        {
            get
            {
                return m_Resource;
            }
            set
            {
                if (m_Resource != value)
                {
                    m_Resource = value;

                    if (ColorFromResource)
                        Hue = CraftResources.GetHue(m_Resource);

                    InvalidateProperties();
                }
            }
        }

        public virtual bool ColorFromResource
        {
            get { return true; }
        }

        public int OnCraft(int quality, bool makersMark, Mobile from, CraftSystem craftSystem, Type typeRes, Type typeRes2, BaseTool tool, CraftItem craftItem, int resHue)
        {
            Resource = CraftResources.GetFromType(typeRes);
            return quality;
        }

        public override void AddNameProperty(ObjectPropertyList list)
        {
            if (Name == null)
                list.Add(LabelNumber);
            else
                list.Add(Name);

            int woodType = BaseArmor.ResourceNameNumber(m_Resource);
            if (woodType != 0)
                list.Add(woodType);
        }

		public BaseAddonDeed() : base( 0x14F0 )
		{
			Weight = 1.0;

			if ( !Core.AOS )
				LootType = LootType.Newbied;
		}

		public BaseAddonDeed( Serial serial ) : base( serial )
		{
		}

		public override void Serialize( GenericWriter writer )
		{
			base.Serialize( writer );

			writer.Write( (int) 1 ); // version

            writer.Write((int)m_Resource);
		}

		public override void Deserialize( GenericReader reader )
		{
			base.Deserialize( reader );

			int version = reader.ReadInt();

			if ( Weight == 0.0 )
				Weight = 1.0;

            switch (version)
            {
                case 1:
                {
                    m_Resource = (CraftResource)reader.ReadInt();
                    goto case 0;
                }
                case 0:
                    break;
            }
		}

		public override void OnDoubleClick( Mobile from )
		{
			if ( IsChildOf( from.Backpack ) )
				from.Target = new InternalTarget( this );
			else
				from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
		}
		
		#region Mondain's Legacy
		public virtual void DeleteDeed()
		{
			Delete();
		}
		#endregion

		private class InternalTarget : Target
		{
			private BaseAddonDeed m_Deed;

			public InternalTarget( BaseAddonDeed deed ) : base( -1, true, TargetFlags.None )
			{
				m_Deed = deed;

				CheckLOS = false;
			}

			protected override void OnTarget( Mobile from, object targeted )
			{
				IPoint3D p = targeted as IPoint3D;
				Map map = from.Map;

				if ( p == null || map == null || m_Deed.Deleted )
					return;

				if ( m_Deed.IsChildOf( from.Backpack ) )
				{
					BaseAddon addon = m_Deed.Addon;

					Server.Spells.SpellHelper.GetSurfaceTop( ref p );

					BaseHouse house = null;

					AddonFitResult res = addon.CouldFit( p, map, from, ref house );

					if ( res == AddonFitResult.Valid )
						addon.MoveToWorld( new Point3D( p ), map );
					else if ( res == AddonFitResult.Blocked )
						from.SendLocalizedMessage( 500269 ); // You cannot build that there.
					else if ( res == AddonFitResult.NotInHouse )
						from.SendLocalizedMessage( 500274 ); // You can only place this in a house that you own!
					else if ( res == AddonFitResult.DoorsNotClosed )
						from.SendMessage( "You must close all house doors before placing this." );
					else if ( res == AddonFitResult.DoorTooClose )
						from.SendLocalizedMessage( 500271 ); // You cannot build near the door.
					else if ( res == AddonFitResult.NoWall )
						from.SendLocalizedMessage( 500268 ); // This object needs to be mounted on something.
					
					if ( res == AddonFitResult.Valid )
					{
						m_Deed.Delete();
                        addon.Resource = m_Deed.Resource;
						house.Addons.Add( addon );
					}
					else
					{
						addon.Delete();
					}
				}
				else
				{
					from.SendLocalizedMessage( 1042001 ); // That must be in your pack for you to use it.
				}
			}
		}
	}
}