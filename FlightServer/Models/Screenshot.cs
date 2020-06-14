using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace FlightServer.Models
{
    public class Screenshot
    {
        string ipHttp;
        int portHttp;
        readonly string requestScreenshot = "http://localhost:8080/screenshot?window=WindowA";
        Server server;
        ITCPClient socketServer;
        public Screenshot(ITCPClient server)
        {
            socketServer = server;
            socketServer.Connect("127.0.0.1", 5402);
            /*server = new Server();
            ipHttp = ip;
            portHttp = port;*/

        }

        public async Task<Stream> GetScreenshot()
        {
            HttpResponseMessage resultTest = null;
            // Open connection with the givven externalUrlServer.
            using (HttpClient httpClient = new HttpClient())
            {
                TimeSpan timeout = new TimeSpan(0, 0, 20);
                httpClient.Timeout = timeout;
                try
                {

                    // Get the Json as string.
                    resultTest = await httpClient.GetAsync(requestScreenshot);
                    var image = await resultTest.Content.ReadAsStreamAsync();
                    return image;
                    //File file= create(image, "image/jpg");
                    //return new File(image, "image/jpg");
                }
                // This server is not connect.
                catch (Exception)
                {
                    return default;
                }

            }
            return default;
        }
    }
}