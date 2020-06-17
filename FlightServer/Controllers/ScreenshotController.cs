using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FlightServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FlightServer.Controllers
{
    [Route("screenshot")]
    [ApiController]
    public class ScreenshotController : ControllerBase
    {
        private static ITCPClient tcpClient;
        private static DataOfServer dataOfServer;
        public ScreenshotController(ITCPClient client, IOptions<DataOfServer> options)
        {
            dataOfServer = options.Value;
            tcpClient = client;
            tcpClient.Connect(dataOfServer.Ip, dataOfServer.Port);
        }
        // GET: Screenshot
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            if (!tcpClient.IsConnect()) { return BadRequest("Server is not connected"); }
            byte[] image;
            // Open connection with the givven externalUrlServer.
            using (HttpClient httpClient = new HttpClient())
            {
                TimeSpan timeout = new TimeSpan(0, 0, 50);
                httpClient.Timeout = timeout;
                try
                {
                    string requestScreenshot = dataOfServer.HttpAddress + "/screenshot";
                    // Get the Json as string.
                    HttpResponseMessage resultTest = await httpClient.GetAsync(requestScreenshot);
                    image = await resultTest.Content.ReadAsByteArrayAsync();
                }
                // This http is not connect.
                catch (Exception)
                {
                    image = null;
                }
            }
            if (image == null)
            {
                return NotFound("Problem in screenshot");
            }
            return File(image, "image/jpg");
        }
    }
}