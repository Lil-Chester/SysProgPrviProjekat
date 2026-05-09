using System;
using System.Net;
using System.Threading;

namespace AnagramServer
{
    public class HttpServer
    {
        private HttpListener listener;

        private RequestQueue queue =
            new RequestQueue();

        private CacheManager cache =
            new CacheManager();

        private const int WORKER_COUNT = 4;

        public HttpServer(string prefix)
        {
            listener = new HttpListener();

            listener.Prefixes.Add(prefix);
        }

        public void Start()
        {
            listener.Start();

            Console.WriteLine(
                "Server pokrenut na http://localhost:5050/"
            );

            Logger.Log("Server pokrenut.");

            ThreadPool.SetMaxThreads(
                WORKER_COUNT,
                WORKER_COUNT
            );

            for (int i = 0; i < WORKER_COUNT; i++)
            {
                Worker worker =
                    new Worker(queue, cache);

                ThreadPool.QueueUserWorkItem(
                    _ => worker.ProcessRequests()
                );
            }

            while (true)
            {
                HttpListenerContext context =
                    listener.GetContext();

                string file =
                    context.Request.QueryString["fajl"];

                string word =
                    context.Request.QueryString["rec"];

                if (
                    string.IsNullOrWhiteSpace(file)
                    ||
                    string.IsNullOrWhiteSpace(word)
                )
                {
                    context.Response.StatusCode = 400;

                    byte[] error =
                        System.Text.Encoding.UTF8.GetBytes(
                            "Neispravni parametri."
                        );

                    context.Response.OutputStream.Write(
                        error,
                        0,
                        error.Length
                    );

                    context.Response.Close();

                    continue;
                }

                RequestData request =
                    new RequestData
                    {
                        Context = context,
                        FileName = file,
                        Word = word
                    };

                queue.Enqueue(request);

                Logger.Log(
                    $"Primljen zahtev: {file} {word}"
                );
            }
        }
    }
}