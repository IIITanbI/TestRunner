using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
//Test platform Object model assembly references
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;



namespace MyXmlExtension
{
    [FileExtension(".xml")]
    [DefaultExecutorUri(XmlTestExecutor.ExecutorUriString)]
    public class XmlTestDiscoverer : ITestDiscoverer
    {
        /// <summary>
        /// Discovers the tests from the source.
        /// </summary>
        /// <param name="sources">Source files to get the tests from.</param>
        /// <returns>Tests from the file.</returns>
        public void DiscoverTests(IEnumerable<string> sources, IDiscoveryContext discoveryContext, IMessageLogger logger, ITestCaseDiscoverySink discoverySink)
        {
            GetTests(sources, discoverySink);
            Console.WriteLine("From within the discoverer : {0}", Environment.CurrentDirectory);
        }

        internal static IEnumerable<TestCase> GetTests(IEnumerable<string> sources, ITestCaseDiscoverySink discoverySink)
        {
            List<TestCase> tests = new List<TestCase>();

            foreach (string source in sources)
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(source);

                var testNodes = doc.SelectNodes("//Tests/Test");
                foreach (XmlNode testNode in testNodes)
                {
                    XmlAttribute nameAttribute = testNode.Attributes["name"];
                    if (nameAttribute != null && !String.IsNullOrWhiteSpace(nameAttribute.Value))
                    {
                        var testCase = new TestCase(nameAttribute.Value, XmlTestExecutor.ExecutorUri, source);

                        // Get the outcome.
                        XmlAttribute outcomeAttribute = testNode.Attributes["outcome"];
                        if (outcomeAttribute != null && !String.IsNullOrWhiteSpace(outcomeAttribute.Value))
                        {
                            TestOutcome outcome;
                            if (Enum.TryParse<TestOutcome>(outcomeAttribute.Value, out outcome))
                            {
                                testCase.SetPropertyValue(TestResultProperties.Outcome, outcome);
                            }
                            else
                            {
                                // Default to passed if no outcome was set.
                                testCase.SetPropertyValue(TestResultProperties.Outcome, TestOutcome.Passed);
                            }
                        }

                        // Get the error message.
                        var errorMessageNode = testNode.SelectSingleNode("ErrorMessage");
                        if (errorMessageNode != null)
                        {
                            testCase.SetPropertyValue(TestResultProperties.ErrorMessage, errorMessageNode.InnerText);
                        }

                        // Get the stack trace message.
                        var errorStackTraceMessageNode = testNode.SelectSingleNode("ErrorStackTrace");
                        if (errorStackTraceMessageNode != null)
                        {
                            testCase.SetPropertyValue(TestResultProperties.ErrorStackTrace, errorStackTraceMessageNode.InnerText);
                        }

                        tests.Add(testCase);

                        if (discoverySink != null)
                        {
                            discoverySink.SendTestCase(testCase);
                        }

                    }
                }
            }

            return tests;
        }
    }
}
