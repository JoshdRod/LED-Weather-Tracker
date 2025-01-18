using System;
using System.Net.Sockets;

namespace Controller 
{
    class C {
        public static void Main() 
        {
            Console.WriteLine("Starting!");
            
            string serverIp = "127.0.0.1";
            int port = 5000;

            // Wrapping our TcpClient in a using block ensures that the socket is CLOSED once we're done! 
            // This is important, becuase if we don't tell the OS to close a socket, it gets left open, and takes up space in OS memory
            using (TcpClient client = new TcpClient(serverIp, port))
            {
                byte[] data = System.Text.Encoding.ASCII.GetBytes("hello world!");

                NetworkStream stream = client.GetStream();

                stream.Write(data, 0, data.Length);

                Console.WriteLine("Done!");
            }

        }
    }
}
