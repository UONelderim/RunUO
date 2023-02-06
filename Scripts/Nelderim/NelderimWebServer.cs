﻿using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using Server;
using Server.Network;

namespace Nelderim.Scripts.Nelderim
{
    public class NelderimWebServer
    {
        private static int _maxOnline;
        
        public static void Initialize()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(Run), null);
        }

        public static void Run(object state)
        {
            string uri = "http://localhost:8001/";
            HttpListener listener = new HttpListener();
            listener.Prefixes.Add(uri);
            listener.Start();
            Console.WriteLine("Started Nelderim Web Server on " + uri);
            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                using (HttpListenerResponse resp = context.Response)
                {
                    resp.Headers.Set("Content-Type", "text/plain");
                    int players = NetState.Instances.Count;
                    if (players > _maxOnline)
                    {
                        _maxOnline = players;
                    }
                    String upTime = (DateTime.Now - ServerTime.ServerStart).ToString("c");
                    string data = String.Join("|", players, _maxOnline, upTime);
                    byte[] buffer = Encoding.UTF8.GetBytes(data);
                    resp.ContentLength64 = buffer.Length;

                    using(Stream stream = resp.OutputStream)
                        stream.Write(buffer, 0, buffer.Length);
                }
            }
        }
    }
}