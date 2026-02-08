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
    public async Task LogDataForIndividualLanes()
    {
        intputValesForLocation[] inputValues = new intputValesForLocation[]
{

        new intputValesForLocation
            {
                locationId = "88053",
                LocationName = "Barendrecht",
                laneNames = new string[] { "Padelbaan 1", "Padelbaan 2", "Padelbaan 3", "Padelbaan 4c" },
                startTime = 6,
                endTime = 22
            }

        };

        // run the test for all locations in the array
        foreach (intputValesForLocation location in inputValues)
        {
            // get value to see if lane is open or closed, if closed log and continue with next location, if open check lanes
            int roundedDate = DateTime.Now.AddHours(+1).Hour;
            // get string value of time to check for timeslot options
            string timeInString = DateTime.Now.AddHours(+1).ToString("HH:00");
            Console.WriteLine($"{timeInString}");
            
            // function to check if lane is open or closes and close the run if the lane is closed
            if (roundedDate < location.startTime || roundedDate > location.endTime)
            {
                Console.WriteLine($"Pedelbaan {location.LocationName} is closed at {roundedDate}");
                continue;
            }
            Console.WriteLine($"Pedelbaan {location.LocationName}  is open, checking lanes");
            
            //open the page and accept cookies
            try
            {
                await _pedelPom.OpenPageAndAcceptCookies(location.locationId);
            }
            catch (LocationNotFoundException)
            {
                // Stop testing this location and continue with the next one in inputValues
                CsvLogger.Log($"Skipping {location.LocationName} ({location.locationId}) because page returned 404");
                continue;
            }
            
            //checkThatThereIsNoWarningABoutNoOpelNas
            if(await _pedelPom._warningNoOpenLanes.IsVisibleAsync())
            {
                foreach (string lane in location.laneNames)
                {
                    Console.WriteLine($"{location.locationId}_{location.LocationName}_{DateTime.Now.ToString("yyyy-MM-hh_hh:mm:ss")}_{lane}; {location.locationId}; {location.LocationName}; {lane};  verhuurd; Geen lanen beschikbaar vandaag");

                    CsvLogger.Log($"{location.locationId}_{location.LocationName}_{DateTime.Now.ToString("yyyy-MM-hh_hh:mm:ss")}_{lane}; {location.locationId}; {location.LocationName}; {lane};  verhuurd; verhuurd; Geen lanen beschikbaar vandaag");

                }
                continue;
            }

            // select only 60 minutes options
            await _pedelPom.MakeSureOnly60MinutesRentDurationIsActive();

            // check next timeslot is available 
            // if available log for every lane if it is taken orn not
            if (await _pedelPom.GetSpecificTimeSlot(timeInString).IsVisibleAsync())
            {
                await _pedelPom.ClickSpecificTimeSlotOption(timeInString);
                foreach (string lane in location.laneNames)
                {
                    if (await _pedelPom.GetTimeSlotWithSpecificName(lane).IsVisibleAsync())
                    {
                        Console.WriteLine($"{location.locationId}_{location.LocationName}_{DateTime.Now.ToString("yyyy-MM-hh_hh:mm:ss")}_{lane}; {location.locationId}; {location.LocationName}; {lane};  beschikbaar; {await _pedelPom.GetPriceOfTimeSLotWithSpecificName(lane).InnerTextAsync()}");

                        CsvLogger.Log($"{location.locationId}_{location.LocationName}_{DateTime.Now.ToString("yyyy-MM-hh_hh:mm:ss")}_{lane}; {location.locationId}; {location.LocationName}; {lane};  beschikbaar; {await _pedelPom.GetPriceOfTimeSLotWithSpecificName(lane).InnerTextAsync()}");

                    }
                    else if(!await _pedelPom.GetTimeSlotWithSpecificName(lane).IsVisibleAsync())
                    {
                        Console.WriteLine($"{location.locationId}_{location.LocationName}_{DateTime.Now.ToString("yyyy-MM-hh_hh:mm:ss")}_{lane}; {location.locationId}; {location.LocationName}; {lane};  verhuurd; onbekend");

                        CsvLogger.Log($"{location.locationId}_{location.LocationName}_{DateTime.Now.ToString("yyyy-MM-hh_hh:mm:ss")}_{lane}; {location.locationId}; {location.LocationName}; {lane};  verhuurd; onbekend");

                    }

                }
            }

            // if timeslot is not availabel, log for every lane that it is taken
            else if (!await _pedelPom.GetSpecificTimeSlot(timeInString).IsVisibleAsync())
            {
                foreach (string lane in location.laneNames)
                {
                    Console.WriteLine($"{location.locationId}_{location.LocationName}_{DateTime.Now.ToString("yyyy-MM-hh_hh:mm:ss")}_{lane}; {location.locationId}; {location.LocationName}; {lane};  verhuurd; geen laan beschikbaar dit tijdslot");

                    CsvLogger.Log($"{location.locationId}_{location.LocationName}_{DateTime.Now.ToString("yyyy-MM-hh_hh:mm:ss")}_{lane}; {location.locationId}; {location.LocationName}; {lane};  verhuurd;  geen laan beschikbaar dit tijdslot");

                }
            }
        }
    }
}