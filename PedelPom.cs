using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaywrightTests
{
    public class PedelPom: PageTest
    {
        private readonly IPage Page;

        public PedelPom(IPage page)
        {
            Page = page;
        }
        public ILocator GetInactiveRentDuration(int minutes) { return Page.Locator($"xpath=//span[@class='badge rounded-pill pill  inactive ' and text()='{minutes} minuten']"); }
        public ILocator GetActiveRentDuration(int minutes) { return Page.Locator($"xpath=//span[@class='badge rounded-pill pill  active ' and text()='{minutes} minuten']"); }
        public ILocator _firstTimeSlotOption => Page.Locator("xpath=(//*[@class='pill-filter-container']//span)[1]");
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
            {
                await _buttonAcceptCoociekes.ClickAsync();
                await Task.Delay(2000);
            }

        }
        public async Task ClickInputFieldDate()
        {
            await _inputFieldDate.ClickAsync();
        }
        public async Task ClickFirsTimeSlotOption()
        {
            await _firstTimeSlotOption.ClickAsync();
        }

        public async Task ClickFirstTimeslotOptionIfVisible()
        {
            await Task.Delay(1000);
            if (await _firstTimeSlotOption.IsVisibleAsync())
                await ClickFirsTimeSlotOption();
            await Task.Delay(1000);
        }

        public async Task MakeSUreRentDurationIsInactive(int minutes)
        {
            if(await GetActiveRentDuration(minutes).IsVisibleAsync())
            {
                await GetActiveRentDuration(minutes).ClickAsync();
            }
        }

        public async Task MakeSureRentDurationIsInactive(int minutes)
        {
            if (await GetInactiveRentDuration(minutes).IsVisibleAsync())
            {
                await GetInactiveRentDuration(minutes).ClickAsync();
            }
        }

        public async Task MakeSureOnly60MinutesRentDurationIsActive()
        {
            int[] timeSlots = { 30, 45, 75, 90, 120 };
            foreach (int timeSlot in timeSlots)
            {
                await MakeSUreRentDurationIsInactive(timeSlot);
            }
              
            await MakeSureRentDurationIsInactive(60);
        }

        public async Task ChangeDayWhenNoTimeSlotAvailable()
        {
            CsvLogger.Log($"No lanes available for {DateTime.Now.ToString("dd-MM-yyyy")}");
        }

        public async Task LogLanesForSelectedTimeslot(int amountOfLanes)
        {
            string currentPrice = await _priceFieldFirstTimeSlt.InnerTextAsync();

            int amountOfFreeLanes = await _timeslotContainer.CountAsync();
            int amountOfTakenLanes = amountOfLanes - amountOfFreeLanes;
            CsvLogger.Log($"{amountOfFreeLanes} banen beschikbaar voor {currentPrice}");
            CsvLogger.Log($"{amountOfTakenLanes} verhuurd voor {currentPrice}");
        }
    }
}
