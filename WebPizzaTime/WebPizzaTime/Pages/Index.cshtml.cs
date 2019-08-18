using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebPizzaTime.Pages
{
    public class Index : PageModel
    {
        public string NeededMembers { get; set; }

        public async Task OnGet()
        {
            try
            {
                var schedules = await Controller.GetSchedulesFromApi();
                var (dictOfPossibleTimes, listOfPossibleTimes) = Controller.GetPossibleTimes(schedules);

                ViewData["amountOfAvailableMembers"] = dictOfPossibleTimes.Count.ToString();
                ViewData["date"] = listOfPossibleTimes.ToList().FirstOrDefault().FirstOrDefault().Date
                    .ToString("yyyy-MM-dd");
            }
            catch (Exception e)
            {
                ViewData["errorMessage"] = e;
            }
        }

        public async Task OnPost()
        {
            try
            {
                NeededMembers = Request.Form[nameof(NeededMembers)];

                var schedules = await Controller.GetSchedulesFromApi();
                var (dictOfPossibleTimes, listOfPossibleTimes) = Controller.GetPossibleTimes(schedules);
                var isNumeric = int.TryParse(NeededMembers, out int n);

                ViewData["amountOfAvailableMembers"] = dictOfPossibleTimes.Count.ToString();

                if (!isNumeric)
                {
                    ViewData["errorMessage"] = "Not a valid number, please try again.";
                }
                else if (Convert.ToInt32(NeededMembers) < 0 ||
                         Convert.ToInt32(NeededMembers) > dictOfPossibleTimes.Count)
                {
                    ViewData["errorMessage"] =
                        $"Input needs to be between 0 and {dictOfPossibleTimes.Count}, please try again.";
                }
                else
                {
                    var standUpTimes = Controller.GetStandUpTimes(NeededMembers, listOfPossibleTimes);
                    var amountOfStandUpTimes = standUpTimes.Count.ToString();

                    ViewData["standUpTimes"] = standUpTimes;
                    ViewData["amountOfStandUpTimes"] = amountOfStandUpTimes;
                    ViewData["neededMembers"] = NeededMembers;
                    ViewData["date"] = listOfPossibleTimes.ToList().FirstOrDefault().FirstOrDefault().Date
                        .ToString("yyyy-MM-dd");
                }
            }
            catch (Exception e)
            {
                ViewData["errorMessage"] = e;
            }
        }
    }
}