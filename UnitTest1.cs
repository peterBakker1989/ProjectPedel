using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class ExampleTest : PageTest
{
    [Test]
    public async Task CheckMeetAndPLay()
    {
        int index = 0;
        string[] laneId = {"88053",
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
"216837",
"31948",
"30123",
"30567",
"30732",
"35522",
"40112",
"41223",
"45987",
"48765",
"52130",
"53217",
"58112"
};

        int[] amountOfLanes = {4,
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
4,
4,
3,
4,
3,
4,
4,
4,
3,
4,
3,
4,
3
 };

        foreach (string id in laneId) {
            index++;
            Console.WriteLine(index);
            await LogForACertainId(id, amountOfLanes[index-1]); }

    }



    public ILocator _buttonAcceptCoociekes => Page.Locator("xpath=//*[@id='CybotCookiebotDialogBodyLevelButtonLevelOptinAllowAll']");
    public ILocator _headerTitle => Page.Locator("xpath=//*[@class='club-title']");

    public ILocator _404 => Page.Locator("xpath=//*[contains(text(),'404')]");

    public ILocator _baanAddress => Page.Locator("xpath=//*[@class='mp-club-address']");
    public ILocator _inputFieldDate => Page.Locator("xpath=//input[@id='date']");
    public ILocator _warningNoOpenLanes => Page.Locator("xpath=//*[@class='alert alert-whitelabel mb-3']");

    public ILocator _timeslotContainer => Page.Locator("xpath=//*[@class='timeslots']//div[@class='timeslot-container']");

    public ILocator _timeSelectAllOptions => Page.Locator("xpath=//*[@class='mp-page-filters-times']//label");

    public ILocator _priceFieldFirstTimeSlt => Page.Locator("(//*[@class='timeslot-price']//div)[1]");

    public ILocator GetSpecifickTimeslot(int index) { return Page.Locator($"xpath=(//*[@class='mp-page-filters-times']//label)[{index}]"); }
    public ILocator GetDatePickerSpecificDateField(int day) { return Page.Locator($"xpath=//*[@data-day='{day}']"); }
    public async Task CheckAndAccpetCookies()
    {
        await Task.Delay(1000);
        if (await _buttonAcceptCoociekes.IsVisibleAsync())
            await _buttonAcceptCoociekes.ClickAsync();
    }
    public async Task ClickInputFieldDate()
    {
        await _inputFieldDate.ClickAsync();
    }



    public async Task ClickSpecificDateInDatePicker(int day)
    {
        await ClickInputFieldDate();
        await GetDatePickerSpecificDateField(day).ClickAsync();
    }

    public async Task ChangeDayWHenNoTimeSlotAvailable()
    {
        Console.WriteLine($"No lanes available for {DateTime.Now.ToString("dd-MM-yyyy")}");
        ClickSpecificDateInDatePicker(31);
    }

    public async Task LogLanesForSelectedTimeslot(int amountOfLanes)
    {
        string currentPrice = await _priceFieldFirstTimeSlt.InnerTextAsync();

        int amountOfFreeLanes = await _timeslotContainer.CountAsync();
        int amountOfTakenLanes = amountOfLanes - amountOfFreeLanes;
        Console.WriteLine($"{amountOfFreeLanes} banen beschikbaar voor {currentPrice}");
        Console.WriteLine($"{amountOfTakenLanes} verhuurd voor {currentPrice}");
    }

    public async Task LogForACertainId(string id, int amountOfLanes)
    {
        int amountOfTimeslotsForToday;
        Console.WriteLine(id);
        await Page.GotoAsync($"https://meetandplay.nl/club/{id}?sport=padel");

        if(await _404.IsVisibleAsync())
        {
            Console.WriteLine($"404 - Club  {id} niet gevonden");
            return;
        }
         await Expect(Page).ToHaveTitleAsync(new Regex("KNLTB Meet & Play | Makkelijk en snel tennissen of padellen bij jou in de buurt"));
        await CheckAndAccpetCookies();
        await Task.Delay(2000);
        string baanNaam = await _headerTitle.InnerTextAsync();
        string baanAddress = await _baanAddress.InnerTextAsync();
        Console.WriteLine(baanNaam);
        Console.WriteLine(DateTime.Now.ToString("dd-MM-yyyy :hh:mm:ss"));
        if (await _warningNoOpenLanes.IsVisibleAsync())
        {
            await ChangeDayWHenNoTimeSlotAvailable();
            await Task.Delay(1000);
        }
        if (!await _warningNoOpenLanes.IsVisibleAsync())
        {

            await Task.Delay(2000);
            amountOfTimeslotsForToday = await _timeSelectAllOptions.CountAsync();
            for (int i = 1; i < amountOfTimeslotsForToday + 1; i++)
            {
                await GetSpecifickTimeslot(i).ClickAsync();
                Console.WriteLine(await GetSpecifickTimeslot(i).InnerTextAsync());
                await LogLanesForSelectedTimeslot( amountOfLanes);
            }
        }
    }



}