using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class Class1
    {
        [TestMethod]
        public void Test3()
        {
            Console.WriteLine("123 TEst");
            throw new NotFiniteNumberException();
        }


        [TestMethod]
        public void Test1()
        {
            Console.WriteLine("123 TEst");
            throw new NotFiniteNumberException();
        }

        [TestMethod]
        public void Test2()
        {

        }


        [TestMethod]
        public void Test4()
        {
            Console.WriteLine("123 TEst");
        }


        //[TestMethod]
        //public void Test4(int a, int b)
        //{
        //    Console.WriteLine("123 TEst");
        //}
    }
 
}
