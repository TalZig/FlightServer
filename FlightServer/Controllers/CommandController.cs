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
    [Route("api/command")]
    [ApiController]
    public class CommandController : ControllerBase
    {
        private FlightGearClient flightGear;
        public CommandController(FlightGearClient flightGear1)
        {
            flightGear = flightGear1;
        }

        // POST: api/Command
        [HttpPost]
        public async Task<ActionResult<string>> Post([FromBody]Command value)
        {
            Result myResult = await flightGear.Execute(value);
            if (myResult == Result.Ok)
            {
                return Ok(FlightGearClient.EverythingIsGood);
            }
            return NotFound(AppropriateError(myResult));
        }

        private string AppropriateError(Result result)
        {
            string exceptionMsg;
            switch (result)
            {
                case Result.WriteObjectDisposedException:
                    exceptionMsg = FlightGearClient.WriteObjectDisposedException;
                    break;
                case Result.WriteInvalidOperationException:
                    exceptionMsg = FlightGearClient.WriteInvalidOperationException;
                    break;
                case Result.WriteIOException:
                    exceptionMsg = FlightGearClient.WriteIOException;
                    break;
                case Result.ReadObjectDisposedException:
                    exceptionMsg = FlightGearClient.ReadObjectDisposedException;
                    break;
                case Result.ReadInvalidOperationException:
                    exceptionMsg = FlightGearClient.ReadInvalidOperationException;
                    break;
                case Result.ReadTimeoutException:
                    exceptionMsg = FlightGearClient.ReadTimeoutException;
                    break;
                case Result.ReadIOException:
                    exceptionMsg = FlightGearClient.ReadIOException;
                    break;
                case Result.RegularException:
                    exceptionMsg = FlightGearClient.RegularException;
                    break;
                default:
                    exceptionMsg = "";
                    break;
            }
            return exceptionMsg;
        }
    }
}