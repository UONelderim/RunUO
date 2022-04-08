using System;
using Server.Network;

namespace Server
{
	public class CurrentExpansion
	{
		private static readonly Expansion Expansion = Expansion.ML;

		public static void Configure()
		{
			Core.Expansion = Expansion;

			bool Enabled = Core.AOS;

            // 25.09.2012 :: zombie :: przeniesienie do configu
			Mobile.InsuranceEnabled = Config.InsuranceEnabled;
            // zombie
			ObjectPropertyList.Enabled = Enabled;
			Mobile.VisibleDamageType = Enabled ? VisibleDamageType.Related : VisibleDamageType.None;
			Mobile.GuildClickMessage = !Enabled;
			Mobile.AsciiClickMessage = !Enabled;

			if ( Enabled )
			{
				AOS.DisableStatInfluences();

				if ( ObjectPropertyList.Enabled )
					PacketHandlers.SingleClickProps = true; // single click for everything is overriden to check object property list
			}
		}
	}
}
