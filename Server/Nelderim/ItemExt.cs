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
	}
}