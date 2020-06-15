using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FlightServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace FlightServer.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ScreenshotController : ControllerBase
    {
        private static Screenshot screenshot;
        public ScreenshotController(Screenshot ss)
        {
            screenshot = ss;
        }
        // GET: Screenshot
        [HttpGet]
        public async Task<Stream> Get()
        {
            string statusOfConnection = screenshot.ConnectToTcp();
            if (statusOfConnection != "Ok")
            {
                return default;
            }
            // Connection succeed.
            try
            {
                var image = await screenshot.GetScreenshot();
                return image;
            }
            catch (Exception)
            {
            }

            return default;
        }
    }
}