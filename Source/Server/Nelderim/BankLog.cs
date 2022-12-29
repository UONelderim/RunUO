using System;
using System.IO;
using Server;

namespace Nelderim
{
    public class BankLog
    {
        private static string GetTimeStamp()
        {
            DateTime now = DateTime.Now;
            return String.Format( "{0}-{1}-{2}", now.Year, now.Month, now.Day );
        }
		
        private static string LogPath = Path.Combine( "Logi/Bank", String.Format( "Bank {0}.log", GetTimeStamp() ) );

        static BankLog()
        {
            if( !Directory.Exists( "Logi" ) )
                Directory.CreateDirectory( "Logi" );
				
            string directory = "Logi/Bank";
				
            if( !Directory.Exists( directory ) )
                Directory.CreateDirectory( directory );
        }
        
        public static void Log(Mobile from, int amount, string desc)
        {
            using (StreamWriter writer = new StreamWriter(LogPath, true))
            {
                writer.WriteLine("{0} {1}({2}) {3} {4}", DateTime.Now, from.Serial, from.Name, amount, desc);
            }
        }
    }
}