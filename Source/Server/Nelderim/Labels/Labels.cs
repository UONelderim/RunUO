using Server;
using System;
using System.Collections.Generic;
using System.IO;

namespace Nelderim
{
	public class Labels : NExtension<LabelsInfo>
	{
		public static void Cleanup()
        {
	        try
	        {
		        List<Serial> toRemove = new List<Serial>();
		        foreach (Serial serial in m_ExtensionInfo.Keys)
		        {
			        if (World.FindEntity(serial) == null)
				        toRemove.Add(serial);
		        }

		        foreach (Serial serial in toRemove)
		        {
			        LabelsInfo removed;
			        m_ExtensionInfo.TryRemove(serial, out removed);
		        }
	        }
	        catch (Exception e)
	        {
		        Console.WriteLine("Unable to perform Labels cleanup");
		        Console.WriteLine(e.Message);
	        }
        }
	}
}
