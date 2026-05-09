namespace AnagramServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            HttpServer server =
                new HttpServer("http://localhost:5050/");

            server.Start();
        }
    }
}