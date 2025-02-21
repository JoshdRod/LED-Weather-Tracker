using System;
using System.Net;
using System.Net.Sockets;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Controller 
{
    class Program {
        public static async Task Main() // Main needs to return a Task here, becuase it's an async method
        {
            // Make api request
            var builder = WebApplication.CreateBuilder(); // Figure out what WebApplication builder is, and why it gets the appsettings.json file!
            var configuration = builder.Configuration;
            string requestBody = $"https://api.open-meteo.com/v1/forecast?latitude={configuration.GetSection("latitude").Value}&longitude={configuration.GetSection("longitude").Value}&hourly=precipitation_probability,precipitation&forecast_hours=12";
            
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync(requestBody);

            if (response.Content is null) {
                Console.WriteLine("Error in HTTP request");
                return;
            }

            // Convert data into a percentage to send to python server
            string responseText = await response.Content.ReadAsStringAsync();
            Response responseJson = JsonConvert.DeserializeObject<Response>(responseText);

            double maxProb = Enumerable.Max(responseJson.Hourly.PrecipitationProbability);
            double maxIntensity = Enumerable.Max(responseJson.Hourly.Precipitation);

            double probability = Math.Tanh(maxProb) * Math.Tanh(maxIntensity) * 100
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

    public class Hourly
    {
        [JsonProperty("time")]
        public List<string> Time { get; set; }

        [JsonProperty("precipitation_probability")]
        public List<int> PrecipitationProbability { get; set; }

        [JsonProperty("precipitation")]
        public List<double> Precipitation { get; set; }

    }
    public class Response
    {
        [JsonProperty("hourly")]
        public Hourly Hourly {get; set;}
    }
}
