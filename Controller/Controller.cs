using System;
using System.Net;
using System.Net.Sockets;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Controller 
{
    class Controller {
        private IConfiguration configuration;
        public Controller(IConfiguration iConfig)
        {
            configuration = iConfig;
        }
        public static void Main() 
        {
            // Make api request
            string requestBody = $"api.openweathermap.org/data/2.5/weather?lat=48.8584&lon=2.2945&appid={configuration.GetSection("API_KEY").Value}";
            static HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(requestBody);
            if (response.IsSuccessStatusCode) {
                Console.WriteLine(response.Content);
            }
            // Send response to python server
            Console.WriteLine("Starting!");

            string serverIp = "127.0.0.1";
            int port = 5000;

            // Wrapping our TcpClient in a using block ensures that the socket is CLOSED once we're done! 
            // This is important, becuase if we don't tell the OS to close a socket, it gets left open, and takes up space in OS memory
            using (TcpClient client = new TcpClient(serverIp, port))
            {
                Console.WriteLine("Enter Message :");
                string userInput = Console.ReadLine();
                byte[] data = System.Text.Encoding.ASCII.GetBytes(userInput);

                NetworkStream stream = client.GetStream();

                stream.Write(data, 0, data.Length);

                Console.WriteLine("Done!");
            }

        }
    }
}
