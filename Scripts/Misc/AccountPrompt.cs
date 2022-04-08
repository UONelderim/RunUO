using System;
using Server;
using Server.Accounting;

namespace Server.Misc
{
	public class AccountPrompt
	{
		public static void Initialize()
		{
			if ( Accounts.Count == 0 && !Core.Service )
			{
				Account a = new Account( "owner", "nelderim" );
				a.AccessLevel = AccessLevel.Owner;
			}
		}
	}
}