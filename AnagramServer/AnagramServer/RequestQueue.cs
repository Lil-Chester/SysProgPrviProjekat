using System.Collections.Generic;
using System.Threading;

namespace AnagramServer
{
    public class RequestQueue
    {
        private Queue<RequestData> queue = new();

        private readonly object queueLock = new object();

        public void Enqueue(RequestData request)
        {
            lock (queueLock)
            {
                queue.Enqueue(request);

                Monitor.Pulse(queueLock);
            }
        }

        public RequestData Dequeue()
        {
            lock (queueLock)
            {
                while (queue.Count == 0)
                {
                    Monitor.Wait(queueLock);
                }

                return queue.Dequeue();
            }
        }
    }
}