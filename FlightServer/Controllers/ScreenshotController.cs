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
    [Route("screenshot")]
    [ApiController]
    public class ScreenshotController : ControllerBase
    {
        private static Screenshot screenshot;
        public ScreenshotController(Screenshot screenshotNew)
        {
            screenshot = screenshotNew;
        }
        // GET: Screenshot
        [HttpGet]
        public async Task<FileContentResult> Get()
        {
            string statusOfConnection = screenshot.ConnectToTcp();
            if (statusOfConnection != "Ok")
            {
                return default;
            }
            // Connection succeed.
            try
            {
                byte[] image = await screenshot.GetScreenshot();
                return File(image, "image/jpg");
            }
            catch (Exception)
            {
            }

            return default;
        }
    }
}