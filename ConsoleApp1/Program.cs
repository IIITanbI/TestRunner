using Microsoft.VisualStudio.TestPlatform.Extensions.VSTestIntegration;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace ConsoleApp1
{
    class FakeSettingsProvider : ISettingsProvider
    {
        public void Load(XmlReader reader)
        {
            throw new NotImplementedException();
        }
    }
    class FakeRunSettings : IRunSettings
    {
        public ISettingsProvider GetSettings(string settingsName)
        {
            return new FakeSettingsProvider();
        }
        public string SettingsXml => throw new NotImplementedException();
    }
    class FakeContext : IDiscoveryContext
    {
        public IRunSettings RunSettings => null;
    }
    class FakeLogger : IMessageLogger
    {

        public void SendMessage(TestMessageLevel testMessageLevel, string message)
        {
            Console.WriteLine(message);
        }
    }
    class FakeTestCaseDiscoverySink : ITestCaseDiscoverySink
    {
        public List<TestCase> testCases = new List<TestCase>();

        public void SendTestCase(TestCase discoveredTest)
        {
            testCases.Add(discoveredTest);
            Console.WriteLine(discoveredTest);
        }
    }

    class FakeRunContext : IRunContext
    {
        public bool KeepAlive => true;

        public bool InIsolation => true;

        public bool IsDataCollectionEnabled => false;

        public bool IsBeingDebugged => false;

        public string TestRunDirectory => null;

        public string SolutionDirectory => null;

        public IRunSettings RunSettings => null;

        public ITestCaseFilterExpression GetTestCaseFilter(IEnumerable<string> supportedProperties, Func<string, TestProperty> propertyProvider)
        {
            return null;
        }
    }
    class FakeFrameworkHandle : IFrameworkHandle
    {
        MyEvents myEvents;
        public FakeFrameworkHandle(MyEvents e)
        {
            this.myEvents = e;
        }
        public bool EnableShutdownAfterTestRun { get; set; } = true;

        public int LaunchProcessWithDebuggerAttached(string filePath, string workingDirectory, string arguments, IDictionary<string, string> environmentVariables)
        {
            return 1;
        }

        public void RecordAttachments(IList<AttachmentSet> attachmentSets)
        {
            this.myEvents?.SendTestRunComplete(attachmentSets);
        }

        public void RecordEnd(TestCase testCase, TestOutcome outcome)
        {
        }

        public void RecordResult(TestResult testResult)
        {
            this.myEvents?.SendTestResult(testResult);
        }

        public void RecordStart(TestCase testCase)
        {
        }

        public void SendMessage(TestMessageLevel testMessageLevel, string message)
        {
            this.myEvents?.SendTestRunMessage(testMessageLevel, message);
            Console.WriteLine(message);
        }
    }

    class MyEvents : TestLoggerEvents
    {
        public override event EventHandler<TestRunMessageEventArgs> TestRunMessage;
        public override event EventHandler<TestResultEventArgs> TestResult;
        public override event EventHandler<TestRunCompleteEventArgs> TestRunComplete;

        public void SendTestRunMessage(TestMessageLevel testMessageLevel, string message)
        {
            this.TestRunMessage.Invoke(this, new TestRunMessageEventArgs(testMessageLevel, message));
        }
        public void SendTestResult(TestResult testResult)
        {
            this.TestResult.Invoke(this, new TestResultEventArgs(testResult));
        }
        public void SendTestRunComplete(IList<AttachmentSet> attachmentSets)
        {
            var ats = new Collection<AttachmentSet>(attachmentSets);
            this.TestRunComplete.Invoke(this, new TestRunCompleteEventArgs(null, false, false, null, ats, TimeSpan.FromMinutes(10)));
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            var ass = Assembly.LoadFrom(@"D:\Visual Studio 2015\Projects\BrowserTestAdapter\ConsoleApp1\bin\Debug\Microsoft.VisualStudio.TestPlatform.Extensions.TrxLogger.dll");
            var type = ass.GetType("Microsoft.VisualStudio.TestPlatform.Extensions.TrxLogger");
            var instance = Activator.CreateInstance(type);
            var m = type.GetMethod("Initialize");

            var ev = new MyEvents();
            m.Invoke(instance, new object[] { ev, @"D:\Visual Studio 2015\Projects\BrowserTestAdapter\ConsoleApp1\bin\Debug\Results" });

            ITestDiscoverer discoverer = new MSTestDiscoverer();
            string folder = @"D:\Visual Studio 2015\Projects\BrowserTestAdapter\Tests\bin\Debug";
            var sources = Directory.EnumerateFiles(folder).Where(file => Path.GetExtension(file) == ".dll").ToList();

            var fakeSink = new FakeTestCaseDiscoverySink();
            discoverer.DiscoverTests(sources, new FakeContext(), new FakeLogger(), fakeSink);

            var tc = fakeSink.testCases[0];

            ITestExecutor executor = new MSTestExecutor();
            executor.RunTests(fakeSink.testCases, new FakeRunContext(), new FakeFrameworkHandle(ev));

            ev.SendTestRunComplete(new List<AttachmentSet>());
        }
    }
}
