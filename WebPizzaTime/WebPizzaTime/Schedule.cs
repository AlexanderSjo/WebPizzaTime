using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace WebPizzaTime
{
    public class Schedule
    {
        public int ContractTimeMinutes { get; set; }
        public DateTimeOffset Date { get; set; }
        public bool IsFullDayAbsence { get; set; }
        public string Name { get; set; }
        public string PersonId { get; set; } 
        public List<Projection> Projection { get; set; }
    }

    public class Projection
    {
        public string Color { get; set; }
        public string Description { get; set; }
        public DateTimeOffset Start { get; set; }
        public int Minutes { get; set; }
    }

    public class ScheduleResult
    {
        public List<Schedule> Schedules { get; set; }
    }

    public class RootObject
    {
        public RootObject(JObject data)
        {
            ScheduleResult = data["ScheduleResult"].ToObject<ScheduleResult>();
        }

        internal ScheduleResult ScheduleResult { get; set; }
    }
}