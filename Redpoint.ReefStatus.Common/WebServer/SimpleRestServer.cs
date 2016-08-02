using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedPoint.ReefStatus.Common.WebServer
{
    using System.Net;
    using System.Threading;

    class SimpleRestServer
    {
        private readonly HttpListener listener = new HttpListener();
        private readonly Func<HttpListenerRequest, string> responderMethod;

        public SimpleRestServer(string[] prefixes, Func<HttpListenerRequest, string> method)
        {
            // URI prefixes are required, for example 
            // "http://localhost:8080/index/".
            if (prefixes == null || prefixes.Length == 0)
                throw new ArgumentException("prefixes");

            // A responder method is required
            if (method == null)
                throw new ArgumentException("method");

            foreach (string s in prefixes)
                this.listener.Prefixes.Add(s);

            this.responderMethod = method;
            this.listener.Start();
        }

        public SimpleRestServer(Func<HttpListenerRequest, string> method, params string[] prefixes)
            : this(prefixes, method) { }

        public void Run()
        {
            ThreadPool.QueueUserWorkItem((o) =>
            {
                Console.WriteLine("Webserver running...");
                try
                {
                    while (this.listener.IsListening)
                    {
                        ThreadPool.QueueUserWorkItem(
                            (c) =>
                        {
                            var ctx = c as HttpListenerContext;
                            try
                            {
                                string rstr = this.responderMethod(ctx.Request);
                                byte[] buf = Encoding.UTF8.GetBytes(rstr);
                                ctx.Response.ContentLength64 = buf.Length;
                                ctx.Response.OutputStream.Write(buf, 0, buf.Length);
                            }
                            catch { } // suppress any exceptions
                            finally
                            {
                                // always close the stream
                                ctx.Response.OutputStream.Close();
                            }
                        }, this.listener.GetContext());
                    }
                }
                catch { } // suppress any exceptions
            });
        }

        public void Stop()
        {
            this.listener.Stop();
            this.listener.Close();
        }
    }
}
