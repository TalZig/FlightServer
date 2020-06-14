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
        public Screenshot screenshot;
        public ScreenshotController(Screenshot ss)
        {
            screenshot = ss;
        }
        // GET: Screenshot
        [HttpGet]
        public async Task <Stream> Get()
        {
            try
            {
                var str = await screenshot.GetScreenshot();
                return str;             
            }
            catch (Exception)
            {          
            }
           
            return default;
        }
    }
}
