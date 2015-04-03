using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Falsy.Tests
{
    public class TestClass
    {
        public void Run(Toggle toggle)
        {
            toggle.Toggled = true;
        }

        public object Run2(string message)
        {
            return message;
        }



        public void TestEnumerable<T>(IEnumerable<T> value)
        {
            Assert.AreNotEqual(typeof(object), typeof(T));
        }


    }
}