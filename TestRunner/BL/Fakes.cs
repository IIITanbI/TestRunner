namespace TestRunner.BL
{
    using Microsoft.VisualStudio.TestPlatform.ObjectModel;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
    using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;
    using System;
    using System.Collections.Generic;
    using System.Xml;

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
        public FakeFrameworkHandle()
        {

        }
        public bool EnableShutdownAfterTestRun { get; set; } = true;

        public int LaunchProcessWithDebuggerAttached(string filePath, string workingDirectory, string arguments, IDictionary<string, string> environmentVariables)
        {
            return 1;
        }

        public void RecordAttachments(IList<AttachmentSet> attachmentSets)
        {
        }

        public void RecordEnd(TestCase testCase, TestOutcome outcome)
        {
        }

        public void RecordResult(TestResult testResult)
        {
        }

        public void RecordStart(TestCase testCase)
        {
        }

        public void SendMessage(TestMessageLevel testMessageLevel, string message)
        {
            Console.WriteLine(message);
        }
    }
}
