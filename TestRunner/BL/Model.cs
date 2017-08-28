namespace TestRunner.BL
{
    using Microsoft.VisualStudio.TestPlatform.Extensions.VSTestIntegration;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IModel
    {
    }

    public class Model : IModel
    {
        public async Task<List<TestCase>> LoadTests(string folder)
        {
            var task = Task.Run(() =>
            {
                ITestDiscoverer discoverer = new MSTestDiscoverer();
                var sources = GetSources(folder, discoverer);
                var fakeSink = new FakeTestCaseDiscoverySink();
                discoverer.DiscoverTests(sources, new FakeContext(), new FakeLogger(), fakeSink);
                return fakeSink.testCases;
            });
            return await task;
        }
        public void RunTests(List<TestCase> tests)
        {
            ITestExecutor executor = new MSTestExecutor();
            executor.RunTests(tests, new FakeRunContext(), new FakeFrameworkHandle());
        }
        public static List<string> GetExtensions(ITestDiscoverer discoverer)
        {
            var atts = discoverer.GetType().GetCustomAttributes(true).OfType<FileExtensionAttribute>().ToList();
            var extensions = atts.Select(att => att.FileExtension).ToList();
            return extensions;
        }
        public static List<string> GetSources(string folder, ITestDiscoverer discoverer)
        {
            var extensions = GetExtensions(discoverer);
            var sources = Directory.EnumerateFiles(folder).Where(file => extensions.Contains(Path.GetExtension(file))).ToList();
            return sources;
        }
    }
}
