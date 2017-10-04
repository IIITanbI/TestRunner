using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class Class1
    {
        public TestContext TestContext { get; set; }

        [TestMethod]
        public void Test1()
        {
            Console.WriteLine("123 TEst1");
            //throw new NotFiniteNumberException();
        }

        [TestMethod]
        public void Test2()
        {
        }

        [TestMethod]
        public void Test3()
        {
            Console.WriteLine("123 TEst3");
            //throw new NotFiniteNumberException();
        }

        [TestMethod, Priority(1), TestCategory("Khepri")]
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.TestCase", "https://tfs.chattanooga.tractmanager.com/tfs/Development;Khepri", "44832", DataAccessMethod.Sequential)]
        [Description("Filter Criteria widget: Check that user is able to specify filter criteria")]
        public void Test4()
        {
            var section = this.TestContext.DataRow["Section"].ToString();
            var data = this.TestContext.DataRow["Data"].ToString();
            var field = this.TestContext.DataRow["Field"].ToString();
            var sectionDetails = new Dictionary<string, string> { { field, data } };
            var page = new Uri(this.TestContext.DataRow["Page"].ToString(), UriKind.Relative);
            Console.WriteLine(page.ToString());
            Console.WriteLine("123 TEst");
        }


        //[TestMethod]
        //public void Test4(int a, int b)
        //{
        //    Console.WriteLine("123 TEst");
        //}
    }
}