using System;
using System.Net;
using System.Net.Sockets;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Controller 
{
    class Program {
        public static async Task Main() // Main needs to return a Task here, becuase it's an async method
        {
            // Make api request
            var builder = WebApplication.CreateBuilder(); // Figure out what WebApplication builder is, and why it gets the appsettings.json file!
            var configuration = builder.Configuration;
            string requestBody = $"https://api.openweathermap.org/data/2.5/weather?lat=48.8584&lon=2.2945&appid={configuration.GetSection("API_KEY").Value}";
            
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync(requestBody);
            string responseText = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode) {
                Console.WriteLine(responseText);
            }
            // Send response to python server
            Console.WriteLine("Starting!");

            string serverIp = "127.0.0.1";
            int port = 5000;

            // Wrapping our TcpClient in a using block ensures that the socket is CLOSED once we're done! 
            // This is important, becuase if we don't tell the OS to close a socket, it gets left open, and takes up space in OS memory
            using (TcpClient tcpClient = new TcpClient(serverIp, port))
            {
                byte[] data = System.Text.Encoding.ASCII.GetBytes(responseText);

                NetworkStream stream = tcpClient.GetStream();

                stream.Write(data, 0, data.Length);

                Console.WriteLine("Done!");
            }

        }
    }
}
