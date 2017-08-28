using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//Test platform Object model assembly references
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Adapter;
using Microsoft.VisualStudio.TestPlatform.ObjectModel.Logging;

namespace MyXmlExtension
{
    [ExtensionUri(XmlTestExecutor.ExecutorUriString)]
    public class XmlTestExecutor : ITestExecutor
    {
        #region Constants

        /// <summary>
        /// The Uri used to identify the XmlTestExecutor.
        /// </summary>
        public const string ExecutorUriString = "executor://XmlTestExecutor";

        /// <summary>
        /// The Uri used to identify the XmlTestExecutor.
        /// </summary>
        public static readonly Uri ExecutorUri = new Uri(XmlTestExecutor.ExecutorUriString);

        /// <summary>
        /// specifies whether execution is cancelled or not
        /// </summary>
        private bool m_cancelled;

        #endregion

        #region ITestExecutor

        /// <summary>
        /// Runs the tests.
        /// </summary>
        /// <param name="sources">Path to files to look for tests in.</param>
        /// <param name="runContext">Context to use when executing the tests.</param>
        /// <param param name="frameworkHandle">Handle to the framework to record results and to do framework operations.</param>
        public void RunTests(IEnumerable<string> sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            IEnumerable<TestCase> tests = XmlTestDiscoverer.GetTests(sources, null);

            RunTests(tests, runContext, frameworkHandle);
            
        }
        /// <summary>
        /// Runs the tests.
        /// </summary>
        /// <param name="tests">Tests to be run.</param>
        /// <param name="runContext">Context to use when executing the tests.</param>
        /// <param param name="frameworkHandle">Handle to the framework to record results and to do framework operations.</param>
        public void RunTests(IEnumerable<TestCase> tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
        {
            m_cancelled = false;

            foreach (TestCase test in tests)
            {
                if (m_cancelled)
                {
                    break;
                }

                // Setup the test result as indicated by the test case.
                var testResult = new TestResult(test);
                testResult.Outcome = (TestOutcome)test.GetPropertyValue(TestResultProperties.Outcome);
                testResult.ErrorMessage = (string)test.GetPropertyValue(TestResultProperties.ErrorMessage);
                testResult.ErrorStackTrace = (string)test.GetPropertyValue(TestResultProperties.ErrorStackTrace);

                frameworkHandle.RecordResult(testResult);
            }
        }

        /// <summary>
        /// Cancel the execution of the tests
        /// </summary>
        public void Cancel()
        {
            m_cancelled = true;
        }

        #endregion

    }
}
