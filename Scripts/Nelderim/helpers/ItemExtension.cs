using Server.Items;

namespace Server.Helpers
{
    public static class ItemExtension
    {
        public static void ReplaceWith(this Item oldItem, Item newItem) {
            if (oldItem.ParentEntity is BaseContainer)
            {
                BaseContainer bc = (BaseContainer)oldItem.ParentEntity;
                bc.AddItem(newItem);
            }
            oldItem.Delete();
        }
    }
}