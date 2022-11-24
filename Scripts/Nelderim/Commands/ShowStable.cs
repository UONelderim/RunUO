using System;
using System.Globalization;
using System.Linq;
using Server;
using Server.Mobiles;
using Server.Targeting;

namespace Server.Commands
{
	public class ShowStable
	{
		public static void Initialize()
		{
			CommandSystem.Register("ShowStable", AccessLevel.GameMaster, new CommandEventHandler(ShowStable_Command));
		}
		
		public static void ShowStable_Command (CommandEventArgs e)
		{
			if (e.Arguments.Length >= 1)
			{
				try
				{
					Serial serial = (Serial)Convert.ToInt32(e.Arguments[0], 16);
					ShowStable_OnTarget(e.Mobile, World.FindMobile(serial));
				}
				catch
				{
					e.Mobile.SendMessage("Invalid serial");
				}
			}
			else
			{
				e.Mobile.SendMessage("Wskaz istote:");
				e.Mobile.Target = new BondTimeTarget();
			}
		}

		public static void ShowStable_OnTarget(Mobile from, Mobile m)
		{
			if (m is BaseCreature)
			{
				ShowStableMaster(from, (BaseCreature)m);
			}	
			else if (m is PlayerMobile)
			{
				ListStabledCreatures(from, (PlayerMobile)m);
			}
			else
			{
				from.SendMessage("Nie wiem co to jest.");
			}
		}

		public static void ListStabledCreatures(Mobile from, PlayerMobile pm)
		{
			foreach (var mobile in pm.Stabled)
			{
				from.SendMessage(mobile.GetType().Name + " " + mobile.Serial +  " " + mobile.Name);
			}
		}

		public static void ShowStableMaster(Mobile from, BaseCreature bc)
		{
			var owner = World.Mobiles.Values.FirstOrDefault(m => m.Stabled.Contains(bc));
			if(owner != null)
				from.SendMessage(owner.Serial + " " + owner.Name);
			else
			{
				from.SendMessage("Not found");
			}
		}
		
		public class BondTimeTarget : Target
		{
			public BondTimeTarget() : base(15, false, TargetFlags.None)
			{
			}

			protected override void OnTarget(Mobile from, object target)
			{
				if (target is Mobile)
				{
					ShowStable_OnTarget(from, (Mobile)target);
				}
				else
				{
					from.SendMessage("Musisz wybrac istote.");
				}
			}
		}
	}
}

