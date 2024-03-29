﻿using System;
using System.Collections.Generic;

namespace System.Runtime.CompilerServices
{
    [AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class
         | AttributeTargets.Method)]
    public sealed class ExtensionAttribute : Attribute { }
}

namespace Server
{
    public static class MobileExtension
    {
        public static bool IsNearBy( this Mobile m, Type type, int range = 3  )
        {
            IPooledEnumerable eable = m.GetObjectsInRange(range);
            foreach(var o in eable)
                if(o.GetType() == type)
                {
                    eable.Free();
                    return true;
                }
            eable.Free();
            return false;
        }

        public static bool IsNearBy( this Mobile m, object searched, int range = 3 )
        {
            IPooledEnumerable eable = m.GetObjectsInRange(range);
            foreach(var o in eable)
                if(o == searched)
                {
                    eable.Free();
                    return true;
                }
            eable.Free();
            return false;
        }

        public static bool IsNearByAny( this Mobile m, IEnumerable<Type> types, int range = 3 )
        {
            IPooledEnumerable eable = m.GetObjectsInRange(range);
            foreach (var o in eable)
            {
                foreach (Type t in types)
                {
                    if (t != o.GetType()) continue;
                    
                    eable.Free();
                    return true;
                }
            }
            eable.Free();
            return false;
        }
    }
}
