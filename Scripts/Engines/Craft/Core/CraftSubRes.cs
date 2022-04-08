// szczaw :: 2013.01.13 :: refaktoryzacja
using System;

namespace Server.Engines.Craft
{
	public class CraftSubRes
	{
		public CraftSubRes( Type type, TextDefinition name, double reqSkill, object message ) : this( type, name, reqSkill, 0, message )
		{
		}

		public CraftSubRes( Type type, TextDefinition name, double reqSkill, int genericNameNumber, object message )
		{
			ItemType = type;
			NameNumber = name;
			NameString = name;
            RequiredSkill = reqSkill;
			GenericNameNumber = genericNameNumber;
			Message = message;
		}

        public readonly Type ItemType;
        public readonly string NameString;
        public readonly int NameNumber;
        public readonly int GenericNameNumber;
        public readonly object Message;
        public readonly double RequiredSkill;
	}
}