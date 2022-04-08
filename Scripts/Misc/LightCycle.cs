using System;
using Server;
using Server.Network;
using Server.Commands;

namespace Server
{
	public class LightCycle
	{
		private static double m_Level;
		private static int m_LevelOverride = int.MinValue;
		public const int DungeonLevel = 28;
		public const int JailLevel = 9;

		static LightCycle()
		{
			Console.WriteLine("System: Uruchamianie Oswietlenia...Gotowe");
		}

		public static void Initialize()
		{
			EventSink.Login += new LoginEventHandler(OnLogin);

			CommandSystem.Register("GlobalLight", AccessLevel.GameMaster, new CommandEventHandler(Light_OnCommand));
		}

		[Usage("GlobalLight <value>")]
		[Description("Sets the current global light level.")]
		private static void Light_OnCommand(CommandEventArgs e)
		{
			if (e.Length >= 1)
			{
				LevelOverride = e.GetInt32(0);
				e.Mobile.SendMessage("Global light level override has been changed to {0}.", m_LevelOverride);
			}
			else
			{
				LevelOverride = int.MinValue;
				e.Mobile.SendMessage("Global light level override has been cleared.");
			}
		}

		public static void OnLogin(LoginEventArgs args)
		{
			Mobile m = args.Mobile;

			m.CheckLightLevels(true);
		}

		public static int LevelOverride
		{
			get
			{
				return m_LevelOverride;
			}
			set
			{
				m_LevelOverride = value;

				for (int i = 0; i < NetState.Instances.Count; ++i)
				{
					NetState ns = (NetState)NetState.Instances[i];
					Mobile m = ns.Mobile;

					if (m != null)
						m.CheckLightLevels(false);
				}
			}
		}

		public static double Level
		{
			get
			{
				return m_Level;
			}
			set
			{
				m_Level = value;
				LevelOverride = (int)m_Level;
			}
		}

		public static double CloudsDensity
		{
			get
			{
				return 1; //ServerWeather.Weather;
			}
		}

		public static double SunLight    // range ( 0 , 26 )
		{
			get
			{
				// if (minutes >= 42)  ---> range(   0,  0 )
				// if (minutes <  42)  ---> range( 9.5, 26 )
				//  0 min: 26 - 0.1652892 * 100 =  9.47108
				// 10 min: 26 - 0.1652892 *  25 = 21.86777
				// 20 min: 26 - 0.1652892 *   0 = 26.00000
				// 30 min: 26 - 0.1652892 *  25 = 21.86777
				// 40 min: 26 - 0.1652892 * 100 =  9.47108
				// 50 min:                         0.00000
				return ((ServerTime.TimeUnit < 21) ?
					(26 - 0.1652892 * Math.Pow(ServerTime.TimeUnit - 10, 2)) // Pow ---> range(0,100)
				:
					0);
			}
		}

		public static double SmallerMoon // range ( 0 , 2 )
		{
			get
			{
				// if (hour <  13) return 0.42 * day ---> range( 0 , 2 )
				// if (hour >= 13) return ...        ---> range( 2 , 0 )
				double max = 2.0;
				return ((ServerTime.Day < 13) ? (max / 12.0 * ServerTime.Day) : (max - (max / 12.0 * (ServerTime.Day - 12))));
			}
		}

		public static double LargerMoon  // range ( 0, 4 )
		{
			get
			{
				// H -- hour of week -- range( 0, 168 )   2*58=168
				// if (H <  85) ---> range( 0 , 4 )
				// if (H >= 85) ---> range( 4 , 0 )
				double max = 4.0;
				int Day = (24 * ((int)ServerTime.Month)) + (ServerTime.Day);
				return ((Day < 85) ? (max / 84.0 * Day) : (max - (max / 84.0 * (Day - 84))));
			}
		}

		public static double EvaluateGlobalLight() // 26 - range( 
		{
			double Day = SunLight;                         // range ( 0, 26 )
			double Night = 0 + LargerMoon + SmallerMoon;   // range ( 0, 15 ) --  range ( 0, 10 + 5 )
			Level = 26 - ((Day >= Night) ? Day : Night) * CloudsDensity;
			//Console.WriteLine("-----------");
			//Console.WriteLine("Day:        {0}", Day);
			//Console.WriteLine("Night:      {0}", Night);
			//Console.WriteLine("LightLevel: {0}", Level);
			return Level;
		}

		public static void SetGlobalLight()
		{
			int Message = -1;
			EvaluateGlobalLight();

			switch (ServerTime.TimeUnit)
			{
				case 0: Message = 505469; break;
				case 5: Message = 505470; break;
				case 10: Message = 505471; break;
				case 15: Message = 505472; break;
				case 20: Message = 505473; break;
				case 25: Message = 505474; break;
			}


			for (int i = 0; i < NetState.Instances.Count; ++i)
			{
				NetState ns = NetState.Instances[i];
				Mobile m = ns.Mobile;

				if (m != null)
				{
					if (Message != -1) m.SendLocalizedMessage(Message, "", 0x58);
				}
			}
		}

		public static int ComputeLevelFor(Mobile from)
		{
			if (m_LevelOverride > int.MinValue)
				return m_LevelOverride;

			return (int)Level;
		}

		public class NightSightTimer : Timer
		{
			private Mobile m_Owner;

			// 07.07.2012 :: zombie :: NightSight dziala przez 10 min (wczesniej Utility.Random( 15, 25 ))
			public NightSightTimer(Mobile owner)
				: base(TimeSpan.FromMinutes(10))
			{
				m_Owner = owner;
				Priority = TimerPriority.FiveSeconds;
			}

			protected override void OnTick()
			{
				m_Owner.EndAction(typeof(LightCycle));
				m_Owner.LightLevel = 0;
				BuffInfo.RemoveBuff(m_Owner, BuffIcon.NightSight);
			}
		}
	}
}
