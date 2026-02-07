using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class ExampleTest : Setup
{
  
    [Test]
    public async Task CheckMeetAndPLay()
    {
        int index = 0;
        string[] laneId = {

"88053",
"73655",
"71704",
"71258",
"88001",
"88184",
"88000",
"76240",
"28541",
"31534",
"31104",
"24703",
"26955",
"76406",
"76356",
"70466",
"75507",
"70607",
"70722",
"70396",
"71084",
"88153",
"30676",
"216837"
};

        int[] amountOfLanes = {
4,
19,
4,
6,
15,
5,
11,
4,
5,
2,
4,
3,
6,
4,
3,
2,
2,
4,
3,
5,
2,
1,
4,
4
 };

        foreach (string id in laneId) {
            index++;
            CsvLogger.Log(index.ToString());
            await LogForACertainId(id, amountOfLanes[index-1]); }

    }


  

    public async Task LogForACertainId(string id, int amountOfLanes)
    {
        int amountOfTimeslotsForToday;
        CsvLogger.Log(id);
        await Page.GotoAsync($"https://meetandplay.nl/club/{id}?sport=padel");

        if(await _pedelPom._404.IsVisibleAsync())
        {
            CsvLogger.Log($"404 - Club  {id} niet gevonden");
            return;
        }
         await Expect(Page).ToHaveTitleAsync(new Regex("KNLTB Meet & Play | Makkelijk en snel tennissen of padellen bij jou in de buurt"));
        await _pedelPom.CheckAndAccpetCookies();
        string baanNaam = await _pedelPom._headerTitle.InnerTextAsync();
        string baanAddress = await _pedelPom._baanAddress.InnerTextAsync();
        CsvLogger.Log(baanNaam);
        CsvLogger.Log(DateTime.Now.ToString("dd-MM-yyyy :hh:mm:ss"));
        if (await _pedelPom._warningNoOpenLanes.IsVisibleAsync())
        {
            await _pedelPom.ChangeDayWhenNoTimeSlotAvailable();
            await Task.Delay(1000);
        }
        if (!await _pedelPom._warningNoOpenLanes.IsVisibleAsync())
        {
            //does it for all timeslots
            //await Task.Delay(2000);
            //amountOfTimeslotsForToday = await _timeSelectAllOptions.CountAsync();
            //for (int i = 1; i < amountOfTimeslotsForToday + 1; i++)
            //{
            //    await GetSpecifickTimeslot(i).ClickAsync();
            //    CsvLogger.Log(await GetSpecifickTimeslot(i).InnerTextAsync());
            //    await LogLanesForSelectedTimeslot( amountOfLanes);
            //}

            //does it for first timeslot only
            await Task.Delay(2000);
            await _pedelPom.GetSpecifickTimeslot(1).ClickAsync();
            await _pedelPom.MakeSureOnly60MinutesRentDurationIsActive();
                CsvLogger.Log(await _pedelPom.GetSpecifickTimeslot(1).InnerTextAsync());
                await _pedelPom.LogLanesForSelectedTimeslot( amountOfLanes);
        }
    }



}