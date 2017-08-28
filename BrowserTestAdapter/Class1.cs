namespace BrowserTestAdapter
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
    using System.Diagnostics;

    [FileExtension(".xml")]
    [DefaultExecutorUri("executor://MyTestExecutor")]
    class Discoverer : ITestDiscoverer
    {
        public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger, ITestCaseDiscoverySink discoverySink)
        {
            logger.SendMessage(TestMessageLevel.Error, "Test");
            Debug.WriteLine("gbi");
            sources?.ToList().ForEach(source => logger.SendMessage(TestMessageLevel.Error, $"Source = '{source}'"));
            TestCase tcase = new TestCase("friendlyName", new Uri("executor://MyTestExecutor"), "test source")
            {
                DisplayName = "tttttt case",
                Id = Guid.NewGuid()
            };


            discoverySink.SendTestCase(tcase);
            //a.DiscoverTests(sources, discoveryContext, logger, discoverySink);
        }
    }

    [DefaultExecutorUri("executor://MyTestExecutor")]
    class Executor : ITestExecutor
    {
        public void Cancel()
        {
            throw new NotImplementedException();
        }

        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            return;
            throw new NotImplementedException();
        }

        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            return;
            throw new NotImplementedException();
        }
    }

}
