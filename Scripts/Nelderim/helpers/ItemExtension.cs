using Server.Items;

namespace Server.Helpers
{
	public static class ItemExtension
	{
		public static void ReplaceWith(this Item oldItem, Item newItem)
		{
			if (oldItem.ParentEntity is BaseContainer)
			{
				BaseContainer bc = (BaseContainer)oldItem.ParentEntity;
				bc.AddItem(newItem);
			}
			else
			{
				if (oldItem.Map != null && oldItem.Map != Map.Internal)
				{
					newItem.MoveToWorld(oldItem.Location, oldItem.Map);
				}
			}

			if (oldItem.IsLockedDown)
			{
				newItem.IsLockedDown = true;
			}

			if (oldItem.IsSecure)
			{
				newItem.IsSecure = true;
			}
			oldItem.Delete();
		}
	}
}