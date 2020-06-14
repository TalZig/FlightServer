using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Web;

namespace FlightMobileAppServer.Models
{
    public class Command
    {
        [JsonPropertyName("Aileron")]
        public double Aileron { get; set; }
        [JsonPropertyName("Rudder")]
        public double Rudder { get; set; }
        [JsonPropertyName("Elevator")]
        public double Elevator { get; set; }
        [JsonPropertyName("Throttle")]
        public double Throttle { get; set; }

        // Set the values from post command.
        public void SetValuesFromPost(Command command)
        {
            Aileron = command.Aileron;
            Rudder = command.Rudder;
            Elevator = command.Elevator;
            Throttle = command.Throttle;
        }
    }
}