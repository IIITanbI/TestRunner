using Microsoft.VisualStudio.TestPlatform.Common;
using Microsoft.VisualStudio.TestPlatform.Common.Utilities;
using Microsoft.VisualStudio.TestPlatform.MSTest.TestAdapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Utilities;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace ConsoleApp1
{
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

        IRunSettings run = null;

        public FakeRunContext()
        {
            string path = @"D:\Visual Studio 2015\Projects\BrowserTestAdapter\Tests\bin\Debug\settings.testsettings";

            if (!MSTestSettingsUtilities.IsLegacyTestSettingsFile(path))
                throw new Exception("erer");

            var runSettingsDocument = XmlRunSettingsUtilities.CreateDefaultRunSettings();
            runSettingsDocument = MSTestSettingsUtilities.Import(path, runSettingsDocument, Architecture.X86, FrameworkVersion.Framework45);
            var settingsXml = runSettingsDocument.CreateNavigator().OuterXml;
            //settingsXml = settingsXml.Replace("<ForcedLegacyMode>true</ForcedLegacyMode>", "<ForcedLegacyMode>false</ForcedLegacyMode>");
            //settingsXml = settingsXml.Replace("MSTest", "MSTestV2");

            var res = RunSettingsUtilities.CreateRunSettings(settingsXml, true);
            this.run = res;
        }

        public bool isOk = false;
        public IRunSettings RunSettings
        {
            get
            {
                if (!isOk)
                {
                    isOk = true;
                    return run;
                }
                return run;
            }
        }


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

        public bool IsCompleted { get; private set; } = false;
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
            if (this.IsCompleted) return;

            this.IsCompleted = true;
            var ats = new Collection<AttachmentSet>(attachmentSets);
            this.TestRunComplete.Invoke(this, new TestRunCompleteEventArgs(null, false, false, null, ats, TimeSpan.FromMinutes(10)));
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            string folder = @"D:\Visual Studio 2015\Projects\BrowserTestAdapter\Tests\bin\Debug";

            var ass = Assembly.LoadFrom("Microsoft.VisualStudio.TestPlatform.Extensions.TrxLogger.dll");
            var type = ass.GetType("Microsoft.VisualStudio.TestPlatform.Extensions.TrxLogger");
            var instance = Activator.CreateInstance(type);
            var m = type.GetMethod("Initialize");

            var ev = new MyEvents();
            m.Invoke(instance, new object[] { ev, folder + @"\Results1" });

            ITestDiscoverer discoverer = new MSTestDiscoverer();

            var sources = Directory.EnumerateFiles(folder).Where(file => Path.GetExtension(file) == ".dll").ToList();

            var fakeSink = new FakeTestCaseDiscoverySink();
            discoverer.DiscoverTests(sources, new FakeContext(), new FakeLogger(), fakeSink);

            var tc = fakeSink.testCases[0];

            ITestExecutor executor = new MSTestExecutor();
            var fake = new FakeRunContext();
            executor.RunTests(fakeSink.testCases, fake, new FakeFrameworkHandle(ev));

            ev.SendTestRunComplete(new List<AttachmentSet>());
        }
    }
}
