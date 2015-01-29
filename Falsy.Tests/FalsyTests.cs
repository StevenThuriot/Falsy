#region License
//  
// Copyright 2015 Steven Thuriot
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//    http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
#endregion

using System;
using Falsy.NET;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Falsy.Tests
{
    [TestClass]
    public class FalsyTests
    {
        [TestMethod]
        public void NullIsFalse()
        {
            var value = ((object)null).Falsify();
            
            if (value)
            {
                Assert.Fail("Null should be false.");
            }
            
            if (!value)
            {
            }
            else
            {
                Assert.Fail("Null should be false.");
            }
        }

        [TestMethod]
        public void NotNullIsTrue()
        {
            var value = new Object().Falsify();

            if (value)
            {
                
            }
            else
            {
                Assert.Fail("Not null should be true.");
            }

            if (!value)
            {
                Assert.Fail("Not null should be true.");
            }
            
            if (value == false)
            {
                Assert.Fail("Not null should be true.");
            }
            
            if (value != true)
            {
                Assert.Fail("Not null should be true.");
            }

            if (false == value)
            {
                Assert.Fail("Not null should be true.");
            }

            if (true != value)
            {
                Assert.Fail("Not null should be true.");
            }
        }

        [TestMethod]
        public void FalsyShouldEqualFalsy()
        {
            var value1 = "".Falsify();
            var value2 = 0.Falsify();

            if (Equals(value1, value2))
            {

            }
            else
            {
                Assert.Fail("Falsy Should Equal Falsy");
            }

            if (value1 == value2)
            {

            }
            else
            {
                Assert.Fail("Falsy Should Equal Falsy");
            }

            if (value1 != value2)
            {
                Assert.Fail("Falsy Should Equal Falsy");
            }

            if (Equals(value2, value1))
            {

            }
            else
            {
                Assert.Fail("Falsy Should Equal Falsy");
            }

            if (value2 == value1)
            {

            }
            else
            {
                Assert.Fail("Falsy Should Equal Falsy");
            }

            if (value2 != value1)
            {
                Assert.Fail("Falsy Should Equal Falsy");
            }
        }


        [TestMethod]
        public void TruthyShouldEqualTruthy()
        {
            var value1 = "The Truth".Falsify();
            var value2 = 2.Falsify();

            if (Equals(value1, value2))
            {

            }
            else
            {
                Assert.Fail("Truthy Should Equal Truthy");
            }

            if (value1 == value2)
            {

            }
            else
            {
                Assert.Fail("Truthy Should Equal Truthy");
            }

            if (value1 != value2)
            {
                Assert.Fail("Truthy Should Equal Truthy");
            }

            if (Equals(value2, value1))
            {

            }
            else
            {
                Assert.Fail("Truthy Should Equal Truthy");
            }

            if (value2 == value1)
            {

            }
            else
            {
                Assert.Fail("Truthy Should Equal Truthy");
            }

            if (value2 != value1)
            {
                Assert.Fail("Truthy Should Equal Truthy");
            }
        }


        [TestMethod]
        public void TruthyShouldNotEqualFalsy()
        {
            var value1 = "The Truth".Falsify();
            var value2 = 0.Falsify();

            if (!Equals(value1, value2))
            {

            }
            else
            {
                Assert.Fail("Truthy Should Not Equal Falsy");
            }

            if (value1 != value2)
            {

            }
            else
            {
                Assert.Fail("Truthy Should Not Equal Falsy");
            }

            if (value1 == value2)
            {
                Assert.Fail("Truthy Should Not Equal Falsy");
            }

            if (!Equals(value2, value1))
            {

            }
            else
            {
                Assert.Fail("Truthy Should Not Equal Falsy");
            }

            if (value2 != value1)
            {

            }
            else
            {
                Assert.Fail("Truthy Should Not Equal Falsy");
            }

            if (value2 == value1)
            {
                Assert.Fail("Truthy Should Not Equal Falsy");
            }
        }


        [TestMethod]
        public void TruthAndTruthEqualsTruth()
        {
            var value1 = "The Truth".Falsify();
            var value2 = 2.Falsify();

            if (value1 && value2)
            {
                
            }
            else
            {
                Assert.Fail("Truth And Truth Should Equal Truth");
            }

            if (!(value1 && value2))
            {
                Assert.Fail("Truth And Truth Should Equal Truth");
            }

            if (value2 && value1)
            {
                
            }
            else
            {
                Assert.Fail("Truth And Truth Should Equal Truth");
            }

            if (!(value2 && value1))
            {
                Assert.Fail("Truth And Truth Should Equal Truth");
            }

            if (value1 && value1)
            {

            }
            else
            {
                Assert.Fail("Truth And Truth Should Equal Truth");
            }

            if (!(value1 && value1))
            {
                Assert.Fail("Truth And Truth Should Equal Truth");
            }
        }


        [TestMethod]
        public void FalsyAndTruthEqualsFalse()
        {
            var value1 = "The Truth".Falsify();
            var value2 = 0.Falsify();

            if (value1 && value2)
            {
                Assert.Fail("Falsy And Truth Should Equal False");
            }

            if (!(value1 && value2))
            {

            }
            else
            {
                Assert.Fail("Falsy And Truth Should Equal False");
            }

            if (value2 && value1)
            {
                Assert.Fail("Falsy And Truth Should Equal False");
            }

            if (!(value2 && value1))
            {

            }
            else
            {
                Assert.Fail("Falsy And Truth Should Equal False");
            }
        }


        [TestMethod]
        public void InstanceShouldEqualItsOriginalValue()
        {
            var value1 = new object();
            var value2 = value1.Falsify();

            if (value2 != value1)
            {
                Assert.Fail("Instance Should Equal Its Original Value");
            }
            if (value1 != value2)
            {
                Assert.Fail("Instance Should Equal Its Original Value");
            }
            
            var value3 = 5;
            var value4 = value3.Falsify();

            if (value4 != value3)
            {
                Assert.Fail("Instance Should Equal Its Original Value");
            }
            if (value3 != value4)
            {
                Assert.Fail("Instance Should Equal Its Original Value");
            }
            
            var value5 = 0;
            var value6 = value5.Falsify();

            if (value6 != value5)
            {
                Assert.Fail("Instance Should Equal Its Original Value");
            }
            if (value5 != value6)
            {
                Assert.Fail("Instance Should Equal Its Original Value");
            }
        }


        [TestMethod]
        public void FalsyShouldUnbox()
        {
            var value = "Test";
            var falsy = value.Falsify();

            Assert.AreNotSame(value, falsy);

            string unbox = falsy;
            Assert.AreSame(value, unbox);
            Assert.AreSame(value, (string)falsy);

            //Note: This doesn't work with object since TryConvert will not be called as object is a base class for DynamicObject, but is there anyone using it that way ?!
            //var value = new object();
            //object unbox = value.Falsify();
        }


        [TestMethod]
        public void FalsyShouldSelectTheTruth()
        {
            var truth = 20.Falsify();
            var falsy = 0.Falsify();

            var value1 = truth || falsy;
            var value2 = falsy || truth;

            Assert.AreSame(truth, value1);
            Assert.AreSame(truth, value2);
        }










        [TestMethod]
        public void TruthOrTruthEqualsTruth()
        {
            var value1 = "The Truth".Falsify();
            var value2 = 2.Falsify();

            if (value1 || value2)
            {

            }
            else
            {
                Assert.Fail("Truth Or Truth Should Equal Truth");
            }

            if (!(value1 || value2))
            {
                Assert.Fail("Truth Or Truth Should Equal Truth");
            }

            if (value2 || value1)
            {

            }
            else
            {
                Assert.Fail("Truth Or Truth Should Equal Truth");
            }

            if (!(value2 || value1))
            {
                Assert.Fail("Truth Or Truth Should Equal Truth");
            }

            if (value1 || value1)
            {

            }
            else
            {
                Assert.Fail("Truth Or Truth Should Equal Truth");
            }

            if (!(value1 || value1))
            {
                Assert.Fail("Truth Or Truth Should Equal Truth");
            }
        }

        [TestMethod]
        public void FalsyOrFalsyEqualsFalsy()
        {
            var value1 = "".Falsify();
            var value2 = 0.Falsify();

            if (value1 || value2)
            {
                Assert.Fail("Falsy Or Falsy Should Equal Falsy");
            }

            if (!(value1 || value2))
            {

            }
            else
            {
                Assert.Fail("Falsy Or Falsy Should Equal Falsy");
            }

            if (value2 || value1)
            {
                Assert.Fail("Falsy Or Falsy Should Equal Falsy");
            }

            if (!(value2 || value1))
            {

            }
            else
            {
                Assert.Fail("Falsy Or Falsy Should Equal Falsy");
            }

            if (value1 || value1)
            {
                Assert.Fail("Falsy Or Falsy Should Equal Falsy");
            }

            if (!(value1 || value1))
            {

            }
            else
            {
                Assert.Fail("Falsy Or Falsy Should Equal Falsy");
            }
        }

        [TestMethod]
        public void FalsyOrTruthEqualsTruth()
        {
            var value1 = "The Truth".Falsify();
            var value2 = 0.Falsify();

            if (value1 || value2)
            {
            }
            else
            {
                Assert.Fail("Falsy Or Truth Should Equal Truth");
            }

            if (!(value1 || value2))
            {
                Assert.Fail("Falsy Or Truth Should Equal Truth");
            }

            if (value2 || value1)
            {

            }
            else
            {
                Assert.Fail("Falsy Or Truth Should Equal Truth");
            }

            if (!(value2 || value1))
            {
                Assert.Fail("Falsy Or Truth Should Equal Truth");
            }
        }






        [TestMethod]
        public void FalsyCanCallMethods()
        {
            var falsy = "test".Falsify();
            var upper = falsy.ToUpper();

            Assert.AreEqual(upper, "TEST");


            var test = new TestClass().Falsify();
            var toggle = new Toggle();

            Assert.IsFalse(toggle.Toggled);

            test.Run(toggle);

            Assert.IsTrue(toggle.Toggled);

            var msg = "Message";

            var result = test.Run2(msg);

            Assert.AreEqual(msg, result);
        }


        [TestMethod]
        public void InverseShouldWork()
        {
            bool a = !!(0.Falsify()); // variable is set to false
            Assert.IsFalse(a);

            bool b = !!("0".Falsify()); // true
            Assert.IsTrue(b);
        }

        [TestMethod]
        public void ZeroEmptyAndFalseShouldBeEquivalent()
        {
            bool c = (false == 0.Falsify()); // true
            bool c2 = (false.Falsify() == 0); // true
            bool c3 = (false.Falsify() == 0d); // true
            bool c4 = (false.Falsify() == 0m); // true
            bool c5 = (false.Falsify() == 0.Falsify()); // true
            Assert.IsTrue(c);
            Assert.IsTrue(c2);
            Assert.IsTrue(c3);
            Assert.IsTrue(c4);
            Assert.IsTrue(c5);

            bool d = (false == "".Falsify()); // true
            bool d2 = (false.Falsify() == ""); // true
            bool d3 = (false.Falsify() == "".Falsify()); // true
            Assert.IsTrue(d);
            Assert.IsTrue(d2);
            Assert.IsTrue(d3);

            bool e = (0 == "".Falsify()); // true
            bool e2 = (0.Falsify() == ""); // true
            bool e3 = (0.Falsify() == "".Falsify()); // true
            Assert.IsTrue(e);
            Assert.IsTrue(e2);
            Assert.IsTrue(e3);
        }

        [TestMethod]
        public void NullShouldBeFalseButNotEquivalent()
        {
            bool f = (NET.Falsy.Falsify<object>(null) == false); // false
            bool f2 = (null == false.Falsify()); // false
            bool f3 = (NET.Falsy.Falsify<object>(null) == false.Falsify()); // false
            Assert.IsFalse(f);
            Assert.IsFalse(f2);
            Assert.IsFalse(f3);

            bool g = (null == NET.Falsy.Falsify<object>(null)); // true
            bool g2 = (NET.Falsy.Falsify<object>(null) == null); // true
            bool g3 = (NET.Falsy.Falsify<object>(null) == NET.Falsy.Falsify<object>(null)); // true
            Assert.IsTrue(g);//Problem case..
            Assert.IsTrue(g2);//Problem case..
            Assert.IsTrue(g3);

            //bool h = (undefined == undefined); // true
            //bool i = (undefined == null); // true
        }

        [TestMethod]
        public void NaNShouldBeAWeirdo()
        {
            bool j = (Single.NaN.Falsify() == null); // false
            bool j2 = (Single.NaN == NET.Falsy.Falsify<object>(null)); // false
            bool j3 = (Single.NaN.Falsify() == NET.Falsy.Falsify<object>(null)); // false
            Assert.IsFalse(j);
            Assert.IsFalse(j2);
            Assert.IsFalse(j3);

            bool k = (Single.NaN.Falsify() == Single.NaN); // false
            bool k2 = (Single.NaN == Single.NaN.Falsify()); // false
            bool k3 = (Single.NaN.Falsify() == Single.NaN.Falsify()); // false
            Assert.IsFalse(k);
            Assert.IsFalse(k2);
            Assert.IsFalse(k3);

            bool l = (double.NaN.Falsify() == null); // false
            bool l2 = (double.NaN == NET.Falsy.Falsify<object>(null)); // false
            bool l3 = (double.NaN.Falsify() == NET.Falsy.Falsify<object>(null)); // false
            Assert.IsFalse(l);
            Assert.IsFalse(l2);
            Assert.IsFalse(l3);

            bool m = (double.NaN.Falsify() == double.NaN); // false
            bool m2 = (double.NaN == double.NaN.Falsify()); // false
            bool m3 = (double.NaN.Falsify() == double.NaN.Falsify()); // false
            Assert.IsFalse(m);
            Assert.IsFalse(m2);
            Assert.IsFalse(m3);
        }
    }
}
