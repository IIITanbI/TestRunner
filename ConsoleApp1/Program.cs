using Microsoft.VisualStudio.TestPlatform.Common.Utilities;
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
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace ConsoleApp1
{
    public class FakeSettingsProvider : ISettingsProvider
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
        public string SettingsXml
        {
            get
            {
                return null;
                var e = XElement.Load(@"D:\Visual Studio 2015\Projects\BrowserTestAdapter\Tests\runsettings.runsettings");
                var str = e.ToString();
                return str;
            }
        }
    }
    class FakeContext : IDiscoveryContext
    {
        public IRunSettings RunSettings
        {
            get
            {
                var e = XElement.Load(@"D:\Visual Studio 2015\Projects\BrowserTestAdapter\Tests\runsettings.runsettings");
                //var e = XElement.Load(@"D:\Visual Studio 2015\Projects\BrowserTestAdapter\Tests\settings.testsettings");
                var str = e.ToString();
                var run = RunSettingsUtilities.CreateAndInitializeRunSettings(str);
                return run;
            }
        }
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

        public IRunSettings RunSettings
        {
            get
            {
                // return new FakeRunSettings();
                var e = XElement.Load(@"D:\Visual Studio 2015\Projects\BrowserTestAdapter\Tests\runsettings.runsettings");
                //var e = XElement.Load(@"D:\Visual Studio 2015\Projects\BrowserTestAdapter\Tests\settings.testsettings");
                var str = e.ToString();
                var run = RunSettingsUtilities.CreateAndInitializeRunSettings(str);
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
        static void test()
        {
            var _a = Assembly.LoadFrom(@"Microsoft.VisualStudio.QualityTools.TMI.dll");
            var type = _a.GetType("Microsoft.VisualStudio.TestTools.TestManagement.Tmi");
            var handler = _a.GetTypes().FirstOrDefault(t => t.Name == "TmiWarningEventHandler");
            var instance = Activator.CreateInstance(handler, new object[] { null, null });

            var m = type.GetMethod("GetTestTypeInfosForExtension");

            var res = m.Invoke(null, new object[] { @"D:\Visual Studio 2015\Projects\BrowserTestAdapter\Tests\bin\Debug\Tests.dll" });
        }
        private static Assembly ResolveDependentAssembly(object sender, ResolveEventArgs args)
        {
            Console.WriteLine(args.Name + "   " + sender.GetType().Name);

            var requestingAssemblyLocation = args.RequestingAssembly?.Location;

            if (args.Name != null)
            {
                var assemblyName = new AssemblyName(args.Name);
                string targetPath;

                DirectoryInfo dir = new DirectoryInfo(@"D:\Visual Studio 2015\Projects\BrowserTestAdapter\ConsoleApp1\bin\Debug\ForTest");
                var files = dir.GetFiles(assemblyName.Name + ".dll", SearchOption.AllDirectories);

                targetPath = files.FirstOrDefault()?.FullName;
                if (targetPath == null)
                    return null;
                //if (requestingAssemblyLocation != null)
                //    targetPath = Path.Combine(Path.GetDirectoryName(requestingAssemblyLocation), string.Format("{0}.dll", assemblyName.Name));
                //else
                //    targetPath = Path.Combine(string.Format("{0}.dll", assemblyName.Name));
                assemblyName.CodeBase = targetPath; //This alone won't force the assembly to load from here!

                //We have to use LoadFile here, otherwise we won't load a differing
                //version, regardless of the codebase because only LoadFile
                //will actually load a *new* assembly if it's at a different path
                //See: http://msdn.microsoft.com/en-us/library/b61s44e8(v=vs.110).aspx
                return Assembly.LoadFile(assemblyName.CodeBase);
            }

            return null;
        }

        static List<string> LoadFrom(string folder, SearchOption so = SearchOption.AllDirectories)
        {
            var dlls = Directory.GetFiles(folder, "*.dll", so);
            var notLoaded = new List<string>();
            foreach (var dll in dlls)
            {
                if (dll.Contains("ForTest"))
                {
                    //  continue;
                }

                try
                {
                    AssemblyName an = AssemblyName.GetAssemblyName(dll);
                    Assembly.Load(an);
                    continue;
                }
                catch (FileLoadException fle)
                {
                }
                catch (Exception ex)
                {
                }

                try
                {
                    Assembly.LoadFrom(dll);
                    continue;
                }
                catch (Exception ex)
                {

                }
                notLoaded.Add(dll);
            }

            return notLoaded;
        }
        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.AssemblyResolve += ResolveDependentAssembly;

            // LoadFrom(@"C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\Common7\IDE\Extensions\TestPlatform");
            // LoadFrom(@"C:\Program Files (x86)\Microsoft Visual Studio\2017\Professional\Common7\IDE\CommonExtensions\Microsoft\TestWindow\Team Explorer");
            // LoadFrom(@"D:\Visual Studio 2015\Projects\BrowserTestAdapter\ConsoleApp1\bin\Debug\ForTest", SearchOption.TopDirectoryOnly);
            //LoadFrom(Environment.CurrentDirectory, SearchOption.AllDirectories);
            // test();
            //    AppDomain.CurrentDomain.AssemblyResolve += ResolveDependentAssembly;
            var ass = Assembly.LoadFrom(@"D:\Visual Studio 2015\Projects\BrowserTestAdapter\ConsoleApp1\bin\Debug\Microsoft.VisualStudio.TestPlatform.Extensions.TrxLogger.dll");
            var type = ass.GetType("Microsoft.VisualStudio.TestPlatform.Extensions.TrxLogger.TrxLogger");
            var instance = Activator.CreateInstance(type);
            var method = type.GetMethods().Where(m => m.Name == "Initialize").First();

            var ev = new MyEvents();
            method.Invoke(instance, new object[] { ev, @"D:\Visual Studio 2015\Projects\BrowserTestAdapter\ConsoleApp1\bin\Debug\Results" });

            ITestDiscoverer discoverer = new MSTestDiscoverer();
            string folder = @"D:\Visual Studio 2015\Projects\BrowserTestAdapter\Tests\bin\Debug";
            var sources = Directory.EnumerateFiles(folder).Where(file => Path.GetExtension(file) == ".dll").ToList();

            Environment.CurrentDirectory = @"D:\Visual Studio 2015\Projects\BrowserTestAdapter\ConsoleApp1\bin\Debug\ForTest";
            var fakeSink = new FakeTestCaseDiscoverySink();
            discoverer.DiscoverTests(sources, new FakeContext(), new FakeLogger(), fakeSink);
            Console.WriteLine("TEST CASES = " + fakeSink.testCases.Count);
            ITestExecutor executor = new MSTestExecutor();
            executor.RunTests(fakeSink.testCases, new FakeRunContext(), new FakeFrameworkHandle(ev));
            // ev.SendTestRunComplete(new List<AttachmentSet>());
        }
    }
}
