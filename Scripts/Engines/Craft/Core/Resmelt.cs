using System;
using Server;
using Server.Targeting;
using Server.Items;

namespace Server.Engines.Craft
{
    public abstract class RecycleHelper
    {
        public virtual void Do(Mobile from, CraftSystem craftSystem, BaseTool tool)
        {
        }

        public virtual int Label
        {
            get { return 0; }
        }

        public virtual string LabelString
        {
            get { return ""; }
        }
    }

    public class Rechop : RecycleHelper
    {
        public override int Label { get { return 1061191; } }

        public override void Do(Mobile from, CraftSystem craftSystem, BaseTool tool)
        {
			int num = craftSystem.CanCraft( from, tool, null );

            if (num > 0)
            {
                from.SendGump(new CraftGump(from, craftSystem, tool, num));
            }
            else
            {
                from.Target = new InternalTarget(craftSystem, tool);
                from.SendLocalizedMessage(1044273); // Target an item to recycle.
            }
        }

        private class InternalTarget : Target
        {
            private CraftSystem m_CraftSystem;
            private BaseTool m_Tool;

            public InternalTarget(CraftSystem craftSystem, BaseTool tool)
                : base(2, false, TargetFlags.None)
            {
                m_CraftSystem = craftSystem;
                m_Tool = tool;
            }

            private bool Rechop(Mobile from, Item item, CraftResource resource)
            {
                return Rechop(from, item, resource, false);
            }

            private bool Rechop(Mobile from, Item item, CraftResource resource, bool lowMaterial)
            {
                try
                {
                    Item recycled1 = GetResourceItem(item.GetType(), GetFirstResource(item, resource), lowMaterial, false);
                    Item recycled2 = GetResourceItem(item.GetType(), GetSecondResource(item), lowMaterial, true);

                    if (recycled1 == null && recycled2 == null)
                        return false;

                    item.Delete();

                    if (recycled1 != null)
                        from.AddToBackpack(recycled1);
                    if (recycled2 != null)
                        from.AddToBackpack(recycled2);

                    from.PlaySound(0x13E);
                    return true;
                }
                catch
                {
                }

                return false;
            }

            private Item GetResourceItem(Type itemType, CraftResource resource, bool lowMaterial, bool useSecondResource)
            {
                Type resourceObjectType = GetObjectTypeForResource(resource);
                if (resourceObjectType == null)
                    return null;

                int resourceProductionAmount = GetResourceAmountForProduct(itemType, useSecondResource);
                if ((resourceProductionAmount < 2 && !useSecondResource) || resourceProductionAmount <= 0)
                    return null; // Za malo drewna aby odzyskac deske

                Item log = (Item)Activator.CreateInstance(resourceObjectType);
                if (lowMaterial)
                    log.Amount = 1;
                else
                    log.Amount = Math.Max(1, resourceProductionAmount / 2);

                return log;
            }

            private CraftResource GetFirstResource(Item item, CraftResource resource)
            {
                if (item is BaseArmor)
                    return ((BaseArmor)item).Resource;
                else if (item is BaseWeapon)
                    return ((BaseWeapon)item).Resource;
                else
                    return resource; // e.g. carpenter's furniture
            }

            private CraftResource GetSecondResource(Item item)
            {
                if (item is BaseArmor)
                    return ((BaseArmor)item).Resource2;
                else if (item is BaseWeapon)
                    return ((BaseWeapon)item).Resource2;
                else
                    return CraftResource.None;
            }

            private Type GetObjectTypeForResource(CraftResource resource)
            {
                CraftResourceType craftResourceType = CraftResources.GetType(resource);
                if (craftResourceType != CraftResourceType.Wood && CraftResources.GetType(resource) != CraftResourceType.Bowstring)
                    return null;

                int resourceTypeIndex = craftResourceType == CraftResourceType.Wood ? 1 : 0;
                CraftResourceInfo info = CraftResources.GetInfo(resource);

                if (info == null || resourceTypeIndex >= info.ResourceTypes.Length)
                    return null;

                return info.ResourceTypes[resourceTypeIndex];
            }

            private int GetResourceAmountForProduct(Type productType, bool useSecondResource)
            {
                CraftItem craftItem = m_CraftSystem.CraftItems.SearchFor(productType);

                int resourceIndex = useSecondResource ? 1 : 0;

                if (craftItem == null || resourceIndex >= craftItem.Ressources.Count)
                    return 0;

                CraftRes craftResource = craftItem.Ressources.GetAt(resourceIndex);

                return craftResource.Amount;
            }

            protected override void OnTarget(Mobile from, object targeted)
            {
                int num = m_CraftSystem.CanCraft(from, m_Tool, null);

                if (num > 0)
                {
                    from.SendGump(new CraftGump(from, m_CraftSystem, m_Tool, num));
                }
                else
                {
                    bool success = false;
                    bool lowMaterial = false;

                    if (targeted is BaseAddonDeed)
                    {
                        success = Rechop(from, (BaseAddonDeed)targeted, ((BaseAddonDeed)targeted).Resource);
                    }
                    else if (targeted is ResouceCraftableBaseLight)
                    {
                        success = Rechop(from, (ResouceCraftableBaseLight)targeted, ((ResouceCraftableBaseLight)targeted).Resource);
                    }
                    else if ( targeted is ResouceCraftable)
                    {
                        success = Rechop(from, (ResouceCraftable)targeted, ((ResouceCraftable)targeted).Resource);
                    }
                    else if (targeted is BaseWeapon)
                    {
                        BaseWeapon bw = targeted as BaseWeapon;
                        if (!bw.PlayerConstructed)
                            lowMaterial = true;
                        success = Rechop(from, bw, bw.Resource, lowMaterial);
                    }
                    else if (targeted is BaseArmor)
                    {
                        BaseArmor ba = targeted as BaseArmor;
                        if (!ba.PlayerConstructed)
                            lowMaterial = true;
                        success = Rechop(from, ba, ba.Resource, lowMaterial);
                    }
                    else if (targeted is ResouceCraftableBaseContainer)
                    {
                        if (((BaseContainer)targeted).Items.Count > 0)
                        {
                            from.SendGump(new CraftGump(from, m_CraftSystem, m_Tool, 1061194)); // Musisz oproznic pojemnik zanim porabiesz go na deski.
                            return;
                        }
                        success = Rechop(from, (ResouceCraftableBaseContainer)targeted, ((ResouceCraftableBaseContainer)targeted).Resource);
                    }
                    else if (targeted is LockableContainer)
                    {
                        if (((LockableContainer)targeted).Locked)
                        {
                            from.SendGump(new CraftGump(from, m_CraftSystem, m_Tool, 1061195)); // Nie mozesz porabac pojemnika nie wiedzac co jest w srodku.
                            return;
                        }
                        if (((BaseContainer)targeted).Items.Count > 0)
                        {
                            from.SendGump(new CraftGump(from, m_CraftSystem, m_Tool, 1061194)); // Musisz oproznic pojemnik zanim porabiesz go na deski.
                            return;
                        }
                        success = Rechop(from, (LockableContainer)targeted, ((LockableContainer)targeted).Resource);
                    }

                    if (success)
                        from.SendGump(new CraftGump(from, m_CraftSystem, m_Tool, lowMaterial ? 1061196 : 1061192)); // Porabales przedmiot odzyskujac troche drewna.
                    else
                        from.SendGump(new CraftGump(from, m_CraftSystem, m_Tool, 1061193)); // Nie mozesz odzyskac z tego drewna.
                }
            }
        }
    }

    public class Resmelt : RecycleHelper
	{
		public Resmelt()
		{
		}

        public override int Label { get { return 1044259; } }

        public override void Do(Mobile from, CraftSystem craftSystem, BaseTool tool)
		{
			int num = craftSystem.CanCraft( from, tool, null );

			if ( num > 0 )
			{
				from.SendGump( new CraftGump( from, craftSystem, tool, num ) );
			}
			else
			{
				from.Target = new InternalTarget( craftSystem, tool );
				from.SendLocalizedMessage( 1044273 ); // Target an item to recycle.
			}
		}

		private class InternalTarget : Target
		{
			private CraftSystem m_CraftSystem;
			private BaseTool m_Tool;

			public InternalTarget( CraftSystem craftSystem, BaseTool tool ) :  base ( 2, false, TargetFlags.None )
			{
				m_CraftSystem = craftSystem;
				m_Tool = tool;
			}

			private bool Resmelt( Mobile from, Item item)
			{
				try
				{
                    Item recycled1 = RecycleItemToResource(item, GetFirstResource(item), false);
                    Item recycled2 = RecycleItemToResource(item, GetSecondResource(item), true);

                    if (recycled1 == null && recycled2 == null)
                        return false;

                    item.Delete();

                    if (recycled1 != null)
                        from.AddToBackpack(recycled1);

                    if (recycled2 != null)
                        from.AddToBackpack(recycled2);

                    from.PlaySound( 0x2A );
					from.PlaySound( 0x240 );
					return true;
				}
				catch
				{
                }

                return false;
			}

            private Item RecycleItemToResource(Item item, CraftResource resource, bool useSecondResource)
            {
                if (CraftResources.GetType(resource) != CraftResourceType.Metal && CraftResources.GetType(resource) != CraftResourceType.Scales)
                    return null;

                CraftResourceInfo info = CraftResources.GetInfo(resource);

                if (info == null || info.ResourceTypes.Length == 0)
                    return null;

                CraftItem craftItem = m_CraftSystem.CraftItems.SearchFor(item.GetType());

                int resourceIndex = useSecondResource ? 1 : 0;

                if (craftItem == null || resourceIndex >= craftItem.Ressources.Count)
                    return null;

                CraftRes craftResource = craftItem.Ressources.GetAt(resourceIndex);

                if (craftResource.Amount < 2)
                    return null; // Not enough metal to resmelt

                Type resourceType = info.ResourceTypes[0];
                Item ingot = (Item)Activator.CreateInstance(resourceType);

                if (item is DragonBardingDeed || (item is BaseArmor && ((BaseArmor)item).PlayerConstructed) || (item is BaseWeapon && ((BaseWeapon)item).PlayerConstructed) || (item is BaseClothing && ((BaseClothing)item).PlayerConstructed))
                    ingot.Amount = craftResource.Amount / 2;
                else
                    ingot.Amount = 1;

                return ingot;
            }

            private CraftResource GetFirstResource(object item)
            {
                if (item is BaseArmor)
                    return ((BaseArmor)item).Resource;
                else if (item is BaseWeapon)
                    return ((BaseWeapon)item).Resource;
                else if (item is DragonBardingDeed)
                    return ((DragonBardingDeed)item).Resource;
                else
                    return CraftResource.None;
            }

            private CraftResource GetSecondResource(object item)
            {
                if (item is BaseArmor)
                    return ((BaseArmor)item).Resource2;
                else if (item is BaseWeapon)
                    return ((BaseWeapon)item).Resource2;
                else
                    return CraftResource.None;
            }

            protected override void OnTarget( Mobile from, object targeted )
			{
				int num = m_CraftSystem.CanCraft( from, m_Tool, null );

				if ( num > 0 )
				{
					from.SendGump( new CraftGump( from, m_CraftSystem, m_Tool, num ) );
				}
				else
				{
					bool success = false;
					bool isStoreBought = false;

					if ( targeted is BaseArmor )
					{
						success = Resmelt( from, (BaseArmor)targeted);
						isStoreBought = !((BaseArmor)targeted).PlayerConstructed;
					}
					else if ( targeted is BaseWeapon )
					{
						success = Resmelt( from, (BaseWeapon)targeted);
						isStoreBought = !((BaseWeapon)targeted).PlayerConstructed;
					}
					else if ( targeted is DragonBardingDeed )
					{
						success = Resmelt( from, (DragonBardingDeed)targeted);
						isStoreBought = false;
					}

					if ( success )
						from.SendGump( new CraftGump( from, m_CraftSystem, m_Tool, isStoreBought ? 500418 : 1044270 ) ); // You melt the item down into ingots.
                    else
                        from.SendGump( new CraftGump( from, m_CraftSystem, m_Tool, 1044272 ) ); // You can't melt that down into ingots.
				}
			}
		}
	}
}