using Server.Items;

namespace Server
{
	public partial class Item
	{
		private const int StealableFlag = 0x00200000;

		[CommandProperty(AccessLevel.GameMaster)]
		public bool Stealable
		{
			get { return GetSavedFlag(StealableFlag); }
			set { SetSavedFlag(StealableFlag, value); }
		}
		
		public void ReplaceWith(Item newItem) 
		{
			if (Parent is Container container)
			{
				container.AddItem(newItem);
				newItem.Location = m_Location;
			}
			else
			{
				newItem.MoveToWorld(GetWorldLocation(), m_Map);
			}

			newItem.IsLockedDown = IsLockedDown;
			newItem.IsSecure = IsSecure;

			Delete();
		}
	}
}