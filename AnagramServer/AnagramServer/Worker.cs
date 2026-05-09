using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace AnagramServer
{
    public class Worker
    {
        private RequestQueue queue;

        private CacheManager cache;

        private readonly string rootFolder =
            Path.Combine(
                Directory.GetCurrentDirectory(),
                "Data"
            );

        public Worker(
            RequestQueue queue,
            CacheManager cache)
        {
            this.queue = queue;
            this.cache = cache;
        }

        public void ProcessRequests()
        {
            while (true)
            {
                RequestData request =
                    queue.Dequeue();

                Process(request);
            }
        }

        private void Process(RequestData request)
        {
            try
            {
                Logger.Log(
                    $"Obrada zahteva: {request.FileName} {request.Word}"
                );

                string filePath =
                    FileSearcher.FindFile(
                        rootFolder,
                        request.FileName
                    );

                if (filePath == null)
                {
                    SendResponse(
                        request.Context,
                        404,
                        "Fajl nije pronadjen."
                    );

                    return;
                }

                string cacheKey =
                    $"{request.FileName}:{request.Word}";

                string result =
                    cache.GetOrAdd(
                        cacheKey,
                        () =>
                        {
                            return AnagramService.CountAnagrams(
                                filePath,
                                request.Word
                            );
                        });

                SendResponse(
                    request.Context,
                    200,
                    result
                );
            }
            catch (Exception ex)
            {
                Logger.Log($"GRESKA: {ex.Message}");

                SendResponse(
                    request.Context,
                    500,
                    "Interna greska servera."
                );
            }
        }

        private void SendResponse(
            HttpListenerContext context,
            int statusCode,
            string message)
        {
            context.Response.StatusCode =
                statusCode;

            byte[] data =
                Encoding.UTF8.GetBytes(message);

            context.Response.OutputStream.Write(
                data,
                0,
                data.Length
            );

            context.Response.Close();
        }
    }
}