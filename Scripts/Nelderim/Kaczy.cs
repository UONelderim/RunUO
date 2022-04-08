//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using Server.Mobiles;

//namespace Nelderim.Scripts.Nelderim
//{
//    public class Kaczy
//    {
//		public static void Initialize() {

//            IEnumerable<Type> types = Assembly.GetAssembly(typeof(BaseCreature)).GetTypes()
//                .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(BaseCreature)));
//            foreach (Type type in types) {
//                try {
//                    BaseCreature x = (BaseCreature)type.GetConstructor(new Type[0]).Invoke(new object[0]);
//                    if (x.Tamable) {
//                        Console.WriteLine(x.GetType().ToString() + "|" + x.Name + "|" + x.MinTameSkill);
//                    }
//                    x.Delete();
//                } catch (Exception e){
//                    Console.WriteLine("Unable to create type " + type.Name);
//                    Console.WriteLine(e.ToString());
//                }
//            }
//        }
//	}
//}
