using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace FlightServer.Models
{
    public class Screenshot
    {
        //private string requestScreenshot = "http://localhost:8080/screenshot";
        ITCPClient clientTcp;
        private DataOfServer dataOfServer;
        

        public Screenshot(ITCPClient server, IOptions<DataOfServer> options)
        {
            clientTcp = server;
            dataOfServer = options.Value;

            /*socketServer.Connect("127.0.0.1", 5402);*/
            /*server = new Server();
            ipHttp = ip;
            portHttp = port;*/

        }

        public async Task<byte[]> GetScreenshot()
        {
            HttpResponseMessage resultTest = null;
            // Open connection with the givven externalUrlServer.
            using (HttpClient httpClient = new HttpClient())
            {
                TimeSpan timeout = new TimeSpan(0, 0, 50);
                httpClient.Timeout = timeout;
                try
                {
                    string requestScreenshot = dataOfServer.HttpAddress + "/screenshot";
                    // Get the Json as string.
                    resultTest = await httpClient.GetAsync(requestScreenshot);
                    byte[] image = await resultTest.Content.ReadAsByteArrayAsync();
                    return image;
                }
                // This server is not connect.
                catch (Exception)
                {
                    return default;
                }

            }
        }
        public string ConnectToTcp()
        {
            return clientTcp.Connect(dataOfServer.Ip, dataOfServer.Port);
        }
    }
}