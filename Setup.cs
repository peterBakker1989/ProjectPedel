using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlaywrightTests
{
    public class Setup : ContextTest
    {
        public IPage Page { get; private set; } = null!;
        public PedelPom _pedelPom = null!;
        [SetUp]
        public async Task BeforeEachTest()
        {
            Page = await Context.NewPageAsync();
            await Context.Tracing.StartAsync(new()
            {
                Title = $"{TestContext.CurrentContext.Test.ClassName}.{TestContext.CurrentContext.Test.Name}",
                Screenshots = true,
                Snapshots = true,
                Sources = true
            });
            _pedelPom = new PedelPom(Page);
        }

        [TearDown]
        public async Task AfterEachTest()
        {
            var traceDirectory = Path.Combine(
                TestContext.CurrentContext.TestDirectory, 
                "playwright-traces"
            );

            Directory.CreateDirectory(traceDirectory);

            var tracePath = Path.Combine(
                traceDirectory,
                $"{TestContext.CurrentContext.Test.ClassName}.{TestContext.CurrentContext.Test.Name}.zip"
            );

            await Context.Tracing.StopAsync(new() { Path = tracePath });

            if (TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed)
            {
                TestContext.WriteLine($"Trace file saved at: {tracePath}");
                TestContext.AddTestAttachment(tracePath);
            }
        }
    }

}