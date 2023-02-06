using System;
using System.Collections;
using System.Collections.Generic;

namespace Server.Mobiles
{
	public abstract class NBaseTalkingNPC : BaseVendor
	{
		public override bool IsInvulnerable
		{
			get { return false; }
		}

		protected delegate void Action(Mobile from);

		protected virtual Dictionary<Race, List<Action>> NpcActions
		{
			get { return new Dictionary<Race, List<Action>>(); }
		}

		protected virtual TimeSpan ActionDelay
		{
			get { return TimeSpan.FromSeconds(10); }
		}

		private DateTime _lastAction;

		public override void OnMovement(Mobile m, Point3D oldLocation)
		{
			base.OnMovement(m, oldLocation);

			if (NpcActions == null ||
			    IsMuted ||
			    DateTime.Now - _lastAction < ActionDelay ||
			    !(Utility.RandomDouble() < 0.25) ||
			    !m.InRange(this, 3))
			{
				return;
			}

			try
			{
				var actions = NpcActions.ContainsKey(Race)
					? NpcActions[Race]
					: NpcActions[Race.DefaultRace];
				if (actions.Count > 0)
				{
					Action action = Utility.RandomList(actions);
					action.Invoke(this);
				}
				else
				{
					Console.WriteLine("No action for npc " + Serial + " " + Name);
				}
				_lastAction = DateTime.Now;
			}
			catch (Exception ex)
			{
				Console.WriteLine("NBaseTalkingNPC error");
				Console.WriteLine(ex);
			}
		}

		protected override ArrayList SBInfos
		{
			get { return new ArrayList();  }
		}

		public override void InitSBInfo()
		{
		}

		public override void InitOutfit()
		{
			
		}

		protected override void Init()
		{
			base.Init();
			
			base.InitOutfit();
		}

		public NBaseTalkingNPC(string title) : base(title)
		{
		}

		public NBaseTalkingNPC(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer)
		{
			base.Serialize(writer);

			writer.Write(0); // version
		}

		public override void Deserialize(GenericReader reader)
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}
}
