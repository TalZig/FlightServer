using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightMobileAppServer.Models;
using FlightServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlightServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommandController : ControllerBase
    {
        private FlightGear flightGear;
        private static Command commandManager = new Command();

        public CommandController(FlightGear flightGear1)
        {
            flightGear = flightGear1;
        }
        // GET: api/Command
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }



        // POST: api/Command
        [HttpPost]
        public void Post([FromBody]Command value)
        {
            commandManager.SetValuesFromPost(value);
            // Call the methood that will update the values and update the flightGear.
            flightGear.setValues(value);
            if (!flightGear.checkSuccessInPost(value))
            {
                //return "Dont success".
            }

            flightGear.UpdateTcpSetValues(value);

        }
    }
}