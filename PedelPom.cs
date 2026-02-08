using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PlaywrightTests
{
    // Thrown when a club page returns 404 so callers can skip that location.
    public class LocationNotFoundException : Exception
    {
        public LocationNotFoundException(string message) : base(message) { }
    }

    public class PedelPom: PageTest
    {
        private readonly IPage Page;

        public PedelPom(IPage page)
        {
            Page = page;
        }
        public ILocator GetInactiveRentDuration(int minutes) { return Page.Locator($"xpath=//span[@class='badge rounded-pill pill  inactive ' and text()='{minutes} minuten']"); }
        public ILocator GetActiveRentDuration(int minutes) { return Page.Locator($"xpath=//span[@class='badge rounded-pill pill  active ' and text()='{minutes} minuten']"); }

        public ILocator GetSpecificTimeSlot(string time) { return Page.Locator($"xpath=//*[@name='time' and @value='{time}']"); }

        public ILocator GetTimeSlotWithSpecificName(string laneName) { return Page.Locator($"xpath=//*[@class='timeslot-container']//*[@class='timeslot-name' and contains(text(), '{laneName}')]"); }
      
        public ILocator GetPriceOfTimeSLotWithSpecificName(string laneName) {  return Page.Locator($"xpath=//*[@class='timeslot-name' and contains(text(), '{laneName}')]/../following-sibling::*//*[@class='timeslot-price']"); }
        public ILocator _firstTimeSlotOption => Page.Locator("xpath=(//*[@class='pill-filter-container']//span)[1]");
        public ILocator _buttonAcceptCoociekes => Page.Locator("xpath=//*[@id='CybotCookiebotDialogBodyLevelButtonLevelOptinAllowAll']");
        public ILocator _404 => Page.Locator("xpath=//*[contains(text(),'404')]");
        public ILocator _warningNoOpenLanes => Page.Locator("xpath=//*[@class='alert alert-whitelabel mb-3']");


 
        public async Task CheckAndAccpetCookies()
        {
            await Task.Delay(1000);
            if (await _buttonAcceptCoociekes.IsVisibleAsync())
            {
                await _buttonAcceptCoociekes.ClickAsync();
                await Task.Delay(2000);
            }

        }

        public async Task ClickSpecificTimeSlotOption(string time)
        {
            await GetSpecificTimeSlot(time).ClickAsync();
            await Task.Delay(500);
        }

        public async Task ClickFirsTimeSlotOption()
        {
            await _firstTimeSlotOption.ClickAsync();
        }

        public async Task MakeSureRentDurationIsInactive(int minutes)
        {
            if(await GetActiveRentDuration(minutes).IsVisibleAsync())
            {
                await GetActiveRentDuration(minutes).ClickAsync();
            }
            await Expect(GetActiveRentDuration(minutes)).Not.ToBeVisibleAsync();
        }

        public async Task MakeSureRentDurationIsActive(int minutes)
        {
            if (await GetInactiveRentDuration(minutes).IsVisibleAsync())
            {
                await GetInactiveRentDuration(minutes).ClickAsync();
            }
            await Expect(GetActiveRentDuration(minutes)).ToBeVisibleAsync();
        }

        public async Task MakeSureOnly60MinutesRentDurationIsActive()
        {
            int[] timeSlots = { 30, 45, 75, 90, 120 };
            foreach (int timeSlot in timeSlots)
            {
                await MakeSureRentDurationIsInactive(timeSlot);
            }
              
            await MakeSureRentDurationIsActive(60);
        }

        public async Task OpenPageAndAcceptCookies(string id)
        {
            await Page.GotoAsync($"https://meetandplay.nl/club/{id}?sport=padel");

            if (await _404.IsVisibleAsync())
            {
                Console.WriteLine($"404 - Club  {id} niet gevonden");
                CsvLogger.Log($"404 - Club  {id} niet gevonden");
                // signal caller to skip this location
                throw new LocationNotFoundException($"Club {id} returned 404");
            }
            Console.WriteLine("club gevonden, verder met testen");
            await Expect(Page).ToHaveTitleAsync(new Regex("KNLTB Meet & Play | Makkelijk en snel tennissen of padellen bij jou in de buurt"));
            await CheckAndAccpetCookies();
        }
    }
}
