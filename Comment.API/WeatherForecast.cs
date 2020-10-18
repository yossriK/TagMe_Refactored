using System;

namespace Comment.API
{
    public class WeatherForecast
    {
        // ok ill be removing this file later, let me play around for now
        public DateTime Date { get; set; }

        public int TemperatureC { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

        public string Summary { get; set; }
    }
}
