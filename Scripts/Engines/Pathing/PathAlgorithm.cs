using System;

namespace Server.PathAlgorithms
{
	public abstract class PathAlgorithm
	{
		public abstract bool CheckCondition( Mobile m, Map map, Point3D start, Point3D goal );
		public abstract Direction[] Find( Mobile m, Map map, Point3D start, Point3D goal );

	}
}