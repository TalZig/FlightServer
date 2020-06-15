using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlightServer.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FlightServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientTcpController : ControllerBase
    {
        private static ITCPClient clientTcp;
        public ClientTcpController(ITCPClient clientTcpFromStartup)
        {
            clientTcp = clientTcpFromStartup;
        }

        // GET: api/ClientTcp/5
        [HttpGet("{connect}", Name = "Get")]
        public ActionResult<string> GetConnect(string connect)
        {
            string statusOfConnection;
            if (connect == "Connect")
            {
                statusOfConnection = clientTcp.Connect("127.0.0.1", 5403);
            }
            else if (connect == "Disconnect")
            {
                statusOfConnection = clientTcp.Disconnect();
            }
            else
            {
                return BadRequest();
            }
            if (statusOfConnection != "Ok")
            {
                return NotFound(statusOfConnection);
            }
            return Ok();
        }
    }
}
