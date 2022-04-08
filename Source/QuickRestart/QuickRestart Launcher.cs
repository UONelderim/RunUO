/***********************************************
*					       *
* This script was made by Jeff Boulanger.      *
*					       *
* Email: jeff@runuo.com			       *
*					       *
************************************************
*
* Original Idea By: milt, AKA Pokey
*
* Email: pylon2007@gmail.com
*
* AIM: TrueBornStunna
*
* Website: www.pokey.f13nd.net
*
* Version: 1.0.0
*
* Release Date: June 30, 2006
*
************************************************/
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Diagnostics;

public class Launcher
{
    static bool _debug = false;
    static bool _service = false;
    static bool _profile = false;
    static bool _haltonwarning = false;

    public static void Main( string[] args )
    {
        for( int i = 0; i < args.Length; ++i )
        {
            if( args[i].ToLower() == "-debug" )
                _debug = true;
            else if( args[i].ToLower() == "-service" )
                _service = true;
            else if( args[i].ToLower() == "-profile" )
                _profile = true;
            else if( args[i].ToLower() == "-haltonwarning" )
                _haltonwarning = true;
        }

        string dllpath = "Output\\Scripts.CS.dll";
        string newpath = "Output\\Scripts.CS.dll.new";
        string hashpath = "Output\\Scripts.CS.hash";
        string newhashpath = "Output\\Scripts.CS.hash.new";

        Console.Write( "Waiting for Nelderim.exe to exit..." );
        Process[] processes = Process.GetProcessesByName( "Nelderim.exe" );

        while( processes.Length > 0 )
        {
            Thread.Sleep( 1500 );
            processes = Process.GetProcessesByName( "Nelderim.exe" );
        }

        Thread.Sleep( 6000 );

        Console.WriteLine( "done." );

        Console.Write( "Checking for {0}...", newpath );
        if( File.Exists( Path.Combine( Directory.GetCurrentDirectory(), newpath ) ) )
        {
            try
            {
                Console.Write( "Renaming the file..." );
                MoveFile( newpath, dllpath );
                Console.Write( "done." );
            }
            catch( Exception e )
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine( "an error occured while trying to move the file." );
                Console.WriteLine( "<Do you wish to log this exception?[Y/N]>" );
                if( Console.ReadKey().Key == ConsoleKey.Y )
                    LogError( e );
                
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }
        else
            Console.WriteLine( "not found." );

        Console.Write( "Checking for {0}...", newhashpath );
        if( File.Exists( Path.Combine( Directory.GetCurrentDirectory(), newhashpath ) ) )
        {
            try
            {
                Console.Write( "Renaming the file..." );
                MoveFile( newhashpath, hashpath );
                Console.Write( "done." );
            }
            catch( Exception e )
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine( "an error occured while trying to move the file." );
                Console.WriteLine( "<Do you wish to log this exception?[Y/N]>" );
                if( Console.ReadKey().Key == ConsoleKey.Y )
                    LogError( e );
                Console.ForegroundColor = ConsoleColor.White;
            }
        }
        else
            Console.WriteLine( "not found." );

        if( File.Exists( "Nelderim.exe" ) )
            Process.Start( "Nelderim.exe", Arguments() );

        Process.GetCurrentProcess().Kill();
    }

    private static string Arguments()
    {
        StringBuilder sb = new StringBuilder();

        if( _debug )
            sb.Append( " -debug" );
        if( _service )
            sb.Append( " -service" );
        if( _profile )
            sb.Append( " -profile" );
        if( _haltonwarning )
            sb.Append( " -haltonwarning" );

        return sb.ToString();
    }

    private static void MoveFile( string source, string destination )
    {
        if( !File.Exists( Path.Combine( Directory.GetCurrentDirectory(), source ) ) )
            throw new System.IO.FileNotFoundException( "File not found",
                Path.Combine( Directory.GetCurrentDirectory(), source ) );

        DateTime writeTime = File.GetLastWriteTime( source );

        if( File.Exists( destination ) )
            File.Delete( destination );

        FileStream reader = null;
        FileStream writer = null;

        try
        {
            reader = new FileStream( source, FileMode.Open );
            writer = new FileStream( destination, FileMode.OpenOrCreate );

            byte[] buffer = new byte[512];
            int totalSent = 0;
            int percent = 0;

            while( totalSent < reader.Length )
            {
                int chunkSize = ( int )Math.Min( ( long )buffer.Length, reader.Length - reader.Position );

                reader.Read( buffer, 0, chunkSize );
                writer.Write( buffer, 0, chunkSize );

                totalSent += chunkSize;

                int newpercent = GetPercent( totalSent, reader.Length );

                if( newpercent > percent )
                {
                    UpdatePercent( newpercent );
                    percent = newpercent;
                }
            }
        }
        catch( Exception e )
        {
            throw e;
        }
        finally
        {
            reader.Close();
            writer.Close();
        }        

        File.SetLastWriteTime( destination, writeTime );

        File.Delete( source );
    }

    private static void UpdatePercent( int percent )
    {
        int oldX = Console.CursorLeft;
        int oldY = Console.CursorTop;
        int totalblocks = 20;
        int blocks = percent / 5;
        int remainingblocks = totalblocks - blocks;

        Console.SetCursorPosition( 0, oldY + 1 );

        Console.Write( "[" );

        for( int i = 0; i < blocks; i++ )
            DrawBlock( percent );

        for( int i = 0; i < remainingblocks; i++ )
            Console.Write( "=" );

        Console.Write( "]{0}%", percent );
        Console.SetCursorPosition( oldX, oldY );
    }


    private static void DrawBlock( int percent )
    {
        ConsoleColor oldColor = Console.BackgroundColor;
        Console.BackgroundColor = GetColor( percent );
        Console.Write( " " );
        Console.BackgroundColor = oldColor;
    }

    private static ConsoleColor GetColor( int percent )
    {
        if( percent < 20 )
            return ConsoleColor.DarkRed;
        else if( percent < 40 )
            return ConsoleColor.Red;
        else if( percent < 60 )
            return ConsoleColor.Yellow;
        else if( percent < 80 )
            return ConsoleColor.Green;
        else
            return ConsoleColor.Blue;
    }

    private static int GetPercent( int totalSent, long p )
    {
        return ( int )( ( ( long )totalSent * 100 ) / p );
    }

    private static void LogError( Exception e )
    {
        Logger.Add( "[" + DateTime.Now.ToString( "yyyy-MM-dd hh:mm:ss" ) + "] " + e.ToString() );
    }
    public class Logger
    {
        private static string m_Path = Directory.GetCurrentDirectory();

        private static bool m_Virgin = true;

        private static FileStream m_File;
        public static FileStream File
        {
            get
            {
                if( m_File == null )
                    m_File = new FileStream( Path.Combine( m_Path, ( "Logi//QuickStartLog" + GetTimeStamp() + ".log" ) ), FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite );

                return m_File;
            }
        }

        private static string GetTimeStamp()
        {
            DateTime now = DateTime.Now;

            return String.Format( "{0}-{1}-{2}-{3}-{4}-{5}",
                            now.Day,
                            now.Month,
                            now.Year,
                            now.Hour,
                            now.Minute,
                            now.Second
                    );
        }

        public static void Close()
        {
            m_Writer.Close();
            m_File.Close();
            m_File = null;
            m_Writer = null;
        }

        private static StreamWriter m_Writer;
        public static StreamWriter Writer
        {
            get
            {
                if( m_Writer == null )
                    m_Writer = new StreamWriter( File );
                return m_Writer;
            }
        }

        public static void Add( string text )
        {
            if( m_Virgin )
            {
                m_Virgin = false;
                string str = String.Format( "[{0}] Logging started.", DateTime.Now.ToString( "yyyy-MM-dd hh:mm:ss" ) );
                Writer.WriteLine( str );
            }

            Writer.WriteLine( "" );
            Writer.WriteLine( text );

            Close();
        }
    }
}