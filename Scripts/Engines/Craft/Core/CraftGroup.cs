using System;

namespace Server.Engines.Craft
{
	public class CraftGroup
	{
		private CraftItemCol _craftItems;
        
		public CraftGroup( TextDefinition groupName )
		{
            NameString = groupName;
            NameNumber = groupName;
            _craftItems = new CraftItemCol();
		}

		public void AddCraftItem( CraftItem craftItem )
		{
            _craftItems.Add(craftItem);
		}

		public CraftItemCol CraftItems
		{
            get { return _craftItems; }
		}

        public readonly string NameString;
        public readonly int NameNumber;
	}
}