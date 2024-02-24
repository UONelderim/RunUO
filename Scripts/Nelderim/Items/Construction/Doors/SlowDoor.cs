using Server.Items;
using Server;
using System.Collections.Generic;
using System;

namespace Server.Items
{
	public enum DoorType
	{
		IronGateShort,
		IronGate,
		LightWoodGate,
		DarkWoodGate,
		MetalDoor,
		BarredMetalDoor,
		BarredMetalDoor2,
		RattanDoor,
		DarkWoodDoor,
		MediumWoodDoor,
		MetalDoor2,
		LightWoodDoor,
		StrongWoodDoor
	}
	public class SlowDoor : BaseSlowDoor
	{
		public static Dictionary<DoorType, int[]> m_DoorTypesInfo = new Dictionary<DoorType, int[]> {
            // { DoorType enum, {closedID, openedID, openedSound, closedSound } }
            { DoorType.IronGateShort,    new int[] { 0x84c, 0x84d, 0xEC, 0xF3} },    // ID przepisane Doors.cs
            { DoorType.IronGate,         new int[] { 0x824, 0x825, 0xEC, 0xF3 } },
			{ DoorType.LightWoodGate,    new int[] { 0x839, 0x83A, 0xEB, 0xF2 } },
			{ DoorType.DarkWoodGate,     new int[] { 0x866, 0x867, 0xEB, 0xF2 } },
			{ DoorType.MetalDoor,        new int[] { 0x675, 0x676, 0xEC, 0xF3 } },
			{ DoorType.BarredMetalDoor,  new int[] { 0x685, 0x686, 0xEC, 0xF3 } },
			{ DoorType.BarredMetalDoor2, new int[] { 0x1FED, 0x1FEE, 0xEC, 0xF3 } },
			{ DoorType.RattanDoor,       new int[] { 0x695, 0x696, 0xEB, 0xF2 } },
			{ DoorType.DarkWoodDoor,     new int[] { 0x6A5, 0x6A6, 0xF1, 0x000} },
			{ DoorType.MediumWoodDoor,   new int[] { 0x6B5, 0x6B6, 0xEA, 0xF1 } },
			{ DoorType.MetalDoor2,       new int[] { 0x6C5, 0x6C6, 0xEC, 0xF3 } },
			{ DoorType.LightWoodDoor,    new int[] { 0x6D5, 0x6D6, 0xEA, 0xF1 } },
			{ DoorType.StrongWoodDoor,   new int[] { 0x6E5, 0x6E6, 0xEA, 0xF1 } },

		};

		//[Constructable] // TODO: drzwi zbugowane: wystepuje nieskonczona petla wywolan pomiedzy BaseSlowDoor.Use() i BaseDoor.Use() kraszujaca serwer
		public SlowDoor(DoorType type, DoorFacing facing) : base(0x84c + (2 * (int)facing), 0x84d + (2 * (int)facing), 0xEC, 0xF3, BaseDoor.GetOffset(facing))
		{
			int[] info;
			if (m_DoorTypesInfo.TryGetValue(type, out info))
			{
				ClosedID = info[0] + (2 * (int)facing);
				OpenedID = info[1] + (2 * (int)facing);
				OpenedSound = info[2];
				ClosedSound = info[3];
				Offset = BaseDoor.GetOffset(facing);

				ItemID = Open ? OpenedID : ClosedID;
			}
			else
			{
				Console.WriteLine("Error: missing SlowDoor graphics/sound IDs definition for door type: " + type);
			}
		}

		public SlowDoor(Serial serial) : base(serial)
		{
		}

		public override void Serialize(GenericWriter writer) // Default Serialize method
		{
			base.Serialize(writer);

			writer.Write((int)0); // version
		}

		public override void Deserialize(GenericReader reader) // Default Deserialize method
		{
			base.Deserialize(reader);

			int version = reader.ReadInt();
		}
	}
}