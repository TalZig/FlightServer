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
        private FlightGearClient flightGear;
        private static Command commandManager = new Command();

        public const string ThrottleLocation = "/controls/engines/current-engine/throttle";
        public const string ElevatorLocation = "/controls/flight/elevator";
        public const string AileronLocation = "/controls/flight/aileron";
        public const string RudderLocation = "/controls/flight/rudder";

        public CommandController(FlightGearClient flightGear1)
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
        public ActionResult<string> Post([FromBody]Command value)
        {
            string resultOfSendingToServer;
            resultOfSendingToServer = flightGear.UpdateTcpSetValues(AileronLocation, value.Aileron);
            if (resultOfSendingToServer != FlightGearClient.EverythingIsGood)
            {
                return NotFound(resultOfSendingToServer);
            }
            commandManager.Aileron = value.Aileron;
            resultOfSendingToServer = flightGear.UpdateTcpSetValues(ThrottleLocation, value.Throttle);
            if (resultOfSendingToServer != FlightGearClient.EverythingIsGood)
            {
                return NotFound(resultOfSendingToServer);
            }
            commandManager.Throttle = value.Throttle;
            resultOfSendingToServer = flightGear.UpdateTcpSetValues(ElevatorLocation, value.Elevator);
            if (resultOfSendingToServer != FlightGearClient.EverythingIsGood)
            {
                return NotFound(resultOfSendingToServer);
            }
            commandManager.Elevator = value.Elevator;
            resultOfSendingToServer = flightGear.UpdateTcpSetValues(RudderLocation, value.Rudder);
            if (resultOfSendingToServer != FlightGearClient.EverythingIsGood)
            {
                return NotFound(resultOfSendingToServer);
            }
            commandManager.Rudder = value.Rudder;
            return Ok();
            /*commandManager.SetValuesFromPost(value);
            // Call the methood that will update the values and update the flightGear.
            flightGear.setValues(value);
            if (!flightGear.checkSuccessInPost(value))
            {
                //return "Dont success".
            }

            flightGear.UpdateTcpSetValues(value);*/

        }
    }
}