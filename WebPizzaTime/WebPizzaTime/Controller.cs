using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

[assembly: InternalsVisibleTo("WebPizzaTimeTests")]

namespace WebPizzaTime
{
    public static class Controller
    {
        internal static async Task<RootObject> GetSchedulesFromApi()
        {
            //Define baseUrl
            const string baseUrl = "http://pizzacabininc.azurewebsites.net/PizzaCabinInc.svc/schedule/2015-12-14";

            //Define HttpClient
            using (var client = new HttpClient())
            {
                //Initiate the Get Request, use the await keyword so it will execute the using statement in order.
                using (var response = await client.GetAsync(baseUrl))
                {
                    //Get the content from the response
                    using (var content = response.Content)
                    {
                        //Assign content to data variable.
                        var data = await content.ReadAsStringAsync();
                        var dataObj = JObject.Parse(data);
                        //Then create and return a new instance of RootObject(Schedule), and string interpolate your name property to your JSON object.
                        return new RootObject(dataObj);
                    }
                }
            }
        }

        internal static (Dictionary<Schedule, List<DateTimeOffset>> resultDict, List<HashSet<DateTimeOffset>> resultList
            ) GetPossibleTimes(RootObject schedules)
        {
            //Get all possible 15 min intervals for every schedule
            var resultDict = new Dictionary<Schedule, List<DateTimeOffset>>();
            var resultList = new List<HashSet<DateTimeOffset>>();
            foreach (var schedule in schedules.ScheduleResult.Schedules)
            {
                var listOfPossibleTimes = new List<DateTimeOffset>();
                var hashSetOfPossibleTimes = new HashSet<DateTimeOffset>();

                //Ignore unavailable team members
                if (schedule.IsFullDayAbsence || schedule.Projection.Count == 0)
                {
                    continue;
                }

                for (var i = 0; i < schedule.Projection.Count; i++)
                {
                    var current = schedule.Projection[i];
                    // Skip iteration if on "break"
                    if (current.Description == "Lunch" || current.Description == "Short break")
                    {
                        continue;
                    }

                    var startTime = current.Start;
                    var stopTime = current.Start.AddMinutes(current.Minutes);

                    // Also check previous iteration if there is any, 
                    if (i != 0)
                    {
                        var previousIteration = schedule.Projection[i - 1];
                        // Calculate on current iteration or last + current
                        if (previousIteration.Description != "Lunch" && previousIteration.Description != "Short break")
                        {
                            startTime = previousIteration.Start;
                        }
                    }

                    startTime = TimeRoundUp(startTime);
                    stopTime = TimeRoundDown(stopTime);
                    var possibleTimes = GetPossibleTimesFromSpan(startTime, stopTime);
                    listOfPossibleTimes.AddRange(possibleTimes);
                    foreach (var time in possibleTimes)
                    {
                        // Using HashSet, we only want unique adds. Since we also look att previous iteration, we will get duplicates
                        hashSetOfPossibleTimes.Add(time);
                    }
                }

                // Add to return objects for every "Schedule"
                resultList.Add(hashSetOfPossibleTimes);
                resultDict.Add(schedule, listOfPossibleTimes);
            }

            return (resultDict, resultList);
        }

        private static List<DateTimeOffset> GetPossibleTimesFromSpan(DateTimeOffset start, DateTimeOffset stop)
            // Adds all 15 min intervals within a span to list
        {
            var result = new List<DateTimeOffset>();
            var span = stop - start;
            // How many 15 min intervals is there in the span? Add them
            for (var i = 0; i < span.TotalMinutes / 15; i++)
            {
                result.Add(start.AddMinutes(i * 15));
            }

            return result;
        }

        internal static DateTimeOffset TimeRoundUp(DateTimeOffset input)
            //Round up DateTimeOffset to nearest 15min interval: 00, 15, 30, 45
        {
            return new DateTime(input.Year, input.Month, input.Day, input.Hour, input.Minute, 0).AddMinutes(
                input.Minute % 15 == 0 ? 0 : 15 - input.Minute % 15);
        }

        internal static DateTimeOffset TimeRoundDown(DateTimeOffset input)
            //Round down DateTimeOffset to nearest 15min interval: 00, 15, 30, 45
        {
            return new DateTime(input.Year, input.Month, input.Day, input.Hour, input.Minute, 0).AddMinutes(
                -input.Minute % 15);
        }

        internal static List<DateTimeOffset> GetStandUpTimes(string numberOfTeamMembers,
                List<HashSet<DateTimeOffset>> possibleTimes)
            // Finds all duplicates in possibleTimes. Where amount of duplicates = numberOdTeamMembers
        {
            var checkList = new List<DateTimeOffset>();
            foreach (var memberTimes in possibleTimes)
            {
                checkList.AddRange(memberTimes);
            }

            var resultList = checkList.GroupBy(x => x)
                .Where(group => group.Count() >= Convert.ToInt32(numberOfTeamMembers))
                .Select(group => group.Key).ToList();

            resultList.Sort();
            return resultList;
        }
    }
}