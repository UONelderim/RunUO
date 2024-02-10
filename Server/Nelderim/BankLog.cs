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
            return $"{now.Year}-{now.Month}-{now.Day}";
        }
		
        private static string LogPath = Path.Combine( "Logi/Bank", $"Bank_{GetTimeStamp()}.log");

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
                writer.WriteLine($"{DateTime.Now} {from.Serial}({from.Name}) {amount} {desc}");
            }
        }
    }
}