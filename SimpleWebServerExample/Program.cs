using System;
using MB.Web;

namespace SimpleWebServerExample
{
    class Program
    {
        static void Main(string[] args)
        {
            var server = new SimpleWebServer("http://localhost:8080/", "files/");

            server.Start();

            Console.WriteLine("Press key to exit ..."); 
            Console.Read();
        }
    }
}
