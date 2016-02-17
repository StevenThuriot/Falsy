using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Falsy.NET;
using Horizon;
using Horizon.Forge;
using Microsoft.CSharp.RuntimeBinder;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Falsy.Tests
{
	[TestClass]
    public class FalsyTests
    {
		public dynamic FalsyNull
		{
			get { return ((object) null).Falsify(); }
		}

        [TestMethod]
        public void NullIsFalse()
        {
            var value = FalsyNull;

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

            const int value3 = 5;
            var value4 = value3.Falsify();

            if (value4 != value3)
            {
                Assert.Fail("Instance Should Equal Its Original Value");
            }
            if (value3 != value4)
            {
                Assert.Fail("Instance Should Equal Its Original Value");
            }

            const int value5 = 0;
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
            const string value = "Test";
            var falsy = value.Falsify();

            Assert.AreNotSame(value, falsy);

            string unbox = falsy;
            Assert.AreSame(value, unbox);
            Assert.AreSame(value, (string) falsy);

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
            string upper = falsy.ToUpper();

            Assert.AreEqual("TEST", upper);
            
            var test = new TestClass().Falsify();
            var toggle = new Toggle();

            Assert.IsFalse(toggle.Toggled);

            test.Run(toggle);

            Assert.IsTrue(toggle.Toggled);

            const string msg = "Message";

            string result = test.Run2(msg);

            Assert.AreEqual(msg, result);
        }

	    [TestMethod]
	    public void FalsyCanCallGenericMethods()
        {
            var t = new TestClass().Falsify();
	        var list = Enumerable.Repeat(5, 4).ToList();

            t.TestEnumerable(list);
	    }

	    [TestMethod]
	    public void FalsyCanCallStaticGenericMethods()
        {
            var falsy = "test".Falsify();
            falsy.Join("a", new List<object> { "c", "d" });
	    }

	    [TestMethod]
	    public void FalsyCanCallStaticMethods()
        {
            var falsy = "test".Falsify();
            falsy.Join("a", new List<string> { "c", "d" });
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
            bool f = (FalsyNull == false); // false
            bool f2 = (null == false.Falsify()); // false
            bool f3 = (FalsyNull == false.Falsify()); // false
            Assert.IsFalse(f);
            Assert.IsFalse(f2);
            Assert.IsFalse(f3);

            bool g = (null == FalsyNull); // true
            bool g2 = (FalsyNull == null); // true
            bool g3 = (FalsyNull == FalsyNull); // true
            Assert.IsTrue(g); //Problem case..
            Assert.IsTrue(g2); //Problem case..
            Assert.IsTrue(g3);
        }

        [TestMethod]
        public void UndefinedShouldBeFalseButNotEquivalent()
        {
            bool h = (NET.Falsy.undefined == false.Falsify()); // false
            bool h2 = (NET.Falsy.undefined == false); // false
            Assert.IsFalse(h);
            Assert.IsFalse(h2);

            bool i = (NET.Falsy.undefined == null); // true
            bool i2 = (NET.Falsy.undefined == FalsyNull); // true
            bool i3 = (FalsyNull == NET.Falsy.undefined); // true
            Assert.IsTrue(i);
            Assert.IsTrue(i2);
            Assert.IsTrue(i3);
        }

        [TestMethod]
        public void NaNShouldBeAWeirdo()
        {
            bool j = (Single.NaN.Falsify() == null); // false
            bool j2 = (Single.NaN == FalsyNull); // false
            bool j3 = (Single.NaN.Falsify() == FalsyNull); // false
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
            bool l2 = (double.NaN == FalsyNull); // false
            bool l3 = (double.NaN.Falsify() == FalsyNull); // false
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


        [TestMethod]
        public void UndefinedShouldBeFalsy()
        {
            if (NET.Falsy.undefined)
            {
                Assert.Fail("Undefined Should Be Falsy");
            }

            var falsy = new object().Falsify();
            var undefined = falsy.Test;

            if (undefined)
            {
                Assert.Fail("Undefined Should Be Falsy");
            }
        }

        [TestMethod]
        public void UndefinedPropertiesShouldReturnUndefined()
        {
            var falsy = new object().Falsify();

            var undefined = falsy.Test;

            Assert.AreEqual(NET.Falsy.undefined, undefined, "Undefined Properties Should Return Undefined");
        }

        [TestMethod, ExpectedException(typeof (RuntimeBinderException))]
        public void UndefinedPropertiesOfUndefinedPropertiesShouldCrash()
        {
            var falsy = new object().Falsify();

            var crash = falsy.Test.Test;

            Assert.Fail("Should have crashed.");
        }


        [TestMethod]
        public void DictionaryEntriesShouldBeAccessibleAsProperties()
        {
            var dictionary = new Dictionary<string, int>
                             {
                                 {"Test", 5}
                             };

            var falsy = dictionary.Falsify();

            int result = falsy.Test;
            Assert.AreEqual(5, result, "Dictionary Entries Should Be Accessible As Properties");
        }

        [TestMethod]
        public void DictionaryPropertiesShouldBeAccessible()
        {
            var dictionary = new Dictionary<string, int>
                             {
                                 {"Test", 5}
                             };

            var falsy = dictionary.Falsify();

            int result = falsy.Count;
            Assert.AreEqual(1, result, "Dictionary Properties Should Be Accessible");
        }

        [TestMethod]
        public void DictionaryEntriesShouldHavePriorityOverNormalProperties()
        {
            var dictionary = new Dictionary<string, int>
                             {
                                 {"Count", 5}
                             };

            var falsy = dictionary.Falsify();

            int result = falsy.Count;
            Assert.AreEqual(5, result, "Dictionary Entries Should Have Priority Over Normal Properties");
        }

        [TestMethod]
        public void DictionaryShouldHaveCallableMethods()
        {
            var dictionary = new Dictionary<string, int>
                             {
                                 {"Count", 5}
                             };

            var falsy = dictionary.Falsify();
            falsy.Clear();
            Assert.AreEqual(0, dictionary.Count, "Dictionary Should Have Callable Methods");
        }

        [TestMethod]
        public void DictionaryShouldGetIndexes()
        {
            var dictionary = new Dictionary<string, int>
                             {
                                 {"Test", 5}
                             };

            var falsy = dictionary.Falsify();
            int count = falsy["Test"];

            Assert.AreEqual(5, count, "Dictionary Should Get Indexes");
        }

        [TestMethod]
        public void DictionaryShouldSetIndexes()
        {
            var dictionary = new Dictionary<string, int>();

            var falsy = dictionary.Falsify();

            falsy["Test"] = 5;

            int count = falsy["Test"];
            int count2 = falsy.Test;

            Assert.AreEqual(5, count, "Dictionary Should Set Indexes");
            Assert.AreEqual(5, count2, "Dictionary Should Set Indexes");
        }

        [TestMethod]
        public void DictionaryShouldSetMembers()
        {
            var dictionary = new Dictionary<object, object>();

            var falsy = dictionary.Falsify();

            falsy.Test = 5;
            falsy.Test2 = "10";

            int count = falsy["Test"];
            int count2 = falsy.Test;
            string count3 = falsy.Test2;

            Assert.AreEqual(5, count, "Dictionary Should Set Indexes");
            Assert.AreEqual(5, count2, "Dictionary Should Set Indexes");
            Assert.AreEqual("10", count3, "Dictionary Should Set Indexes");
        }

        [TestMethod]
        public void NonGenericDictionariesShouldWork()
        {
            var dictionary = new ListDictionary();
            
            var falsy = dictionary.Falsify();

            falsy.Test = 5;
            falsy.Test2 = "10";

            int count = falsy["Test"];
            int count2 = falsy.Test;
            string count3 = falsy.Test2;

            Assert.AreEqual(5, count, "Dictionary Should Set Indexes");
            Assert.AreEqual(5, count2, "Dictionary Should Set Indexes");
            Assert.AreEqual("10", count3, "Dictionary Should Set Indexes");
        }


	    [TestMethod]
	    public void FalsyShouldBeAValidParameter()
	    {
		    var value = "this is a test".Falsify();
			var startIndex = 10.Falsify();

		    string result = value.Substring(startIndex);

			Assert.AreEqual("test", result);
	    }

		[TestMethod]
		public void FalsyShouldBeEnumerable()
		{
			const string value = "enumerable";
			var enumerable = value.Falsify();
			var i = 0;

			int length = enumerable.Length;

			Assert.AreEqual(value.Length, length);

			foreach (char character in enumerable)
			{
				var c = value[i++];
				
				Assert.AreEqual(c, character);
			}
		}

		[TestMethod]
		public void FalsyDictionaryShouldBeEnumerable()
		{
			var dictionary = new Dictionary<string, int>
			            {
							{"one", 1},
							{"two", 2},
							{"three", 3},
			            };

			var enumerable = dictionary.Falsify();

			int length = enumerable.Count;
			Assert.AreEqual(dictionary.Count, length);

			foreach (var kvp in enumerable)
			{
				string key = kvp.Key;
				int value = kvp.Value;

				var actualValue = dictionary[key];

				Assert.AreEqual(actualValue, value);
			}
		}

        [TestMethod]
        public void FalsyCanCreateTypes()
        {
            Type type = NET.Falsy
                           .Define
                           .Person(
                                   FirstName: typeof(string),
                                   LastName: typeof(string),
                                   Age: typeof(int)
                            );

            Assert.IsFalse(type.IsSealed);
            Assert.IsFalse(type.IsSerializable);

            var person =
                NET.Falsy
                   .New
                   .Person(
                           Age: 25
                    );

            Assert.IsNotNull((object)person);
            Assert.IsNull(person.FirstName);
            Assert.IsNull(person.LastName);
            Assert.AreEqual(25, person.Age);

            person.FirstName = "Steven";
            person.LastName = "Thuriot";
            person.Age = 28;

            Assert.AreEqual("Steven", person.FirstName);
            Assert.AreEqual("Thuriot", person.LastName);
            Assert.AreEqual(28, person.Age);
        }

        [TestMethod]
        public void FalsyCanCreateSealedTypes()
        {
            Type type = NET.Falsy
                           .Define
                           .Sealed()
                           .SealedPerson(
                                   FirstName: typeof(string),
                                   LastName: typeof(string),
                                   Age: typeof(int)
                            );

            Assert.IsTrue(type.IsSealed);
            Assert.IsFalse(type.IsSerializable);

            var person =
                NET.Falsy
                   .New
                   .SealedPerson(
                           Age: 25
                    );

            Assert.IsNotNull((object)person);
            Assert.IsNull(person.FirstName);
            Assert.IsNull(person.LastName);
            Assert.AreEqual(25, person.Age);

            person.FirstName = "Steven";
            person.LastName = "Thuriot";
            person.Age = 28;

            Assert.AreEqual("Steven", person.FirstName);
            Assert.AreEqual("Thuriot", person.LastName);
            Assert.AreEqual(28, person.Age);
        }

        [TestMethod]
        public void FalsyCanCreateSerializableTypes()
        {
            Type type = NET.Falsy
                           .Define
                           .Serializable()
                           .SerializablePerson(
                                   FirstName: typeof(string),
                                   LastName: typeof(string),
                                   Age: typeof(int)
                            );

            Assert.IsFalse(type.IsSealed);
            Assert.IsTrue(type.IsSerializable);

            var person =
                NET.Falsy
                   .New
                   .SerializablePerson(
                           Age: 25
                    );

            Assert.IsNotNull((object)person);
            Assert.IsNull(person.FirstName);
            Assert.IsNull(person.LastName);
            Assert.AreEqual(25, person.Age);

            person.FirstName = "Steven";
            person.LastName = "Thuriot";
            person.Age = 28;

            Assert.AreEqual("Steven", person.FirstName);
            Assert.AreEqual("Thuriot", person.LastName);
            Assert.AreEqual(28, person.Age);
        }

        [TestMethod]
        public void FalsyCanCreateSerializableSealedTypes()
        {
            Type type = NET.Falsy
                           .Define
                           .Serializable()
                           .Sealed()
                           .SerializableSealedPerson(
                                   FirstName: typeof(string),
                                   LastName: typeof(string),
                                   Age: typeof(int)
                            );

            Assert.IsTrue(type.IsSealed);
            Assert.IsTrue(type.IsSerializable);

            var person =
                NET.Falsy
                   .New
                   .SerializableSealedPerson(
                           Age: 25
                    );

            Assert.IsNotNull((object)person);
            Assert.IsNull(person.FirstName);
            Assert.IsNull(person.LastName);
            Assert.AreEqual(25, person.Age);

            person.FirstName = "Steven";
            person.LastName = "Thuriot";
            person.Age = 28;

            Assert.AreEqual("Steven", person.FirstName);
            Assert.AreEqual("Thuriot", person.LastName);
            Assert.AreEqual(28, person.Age);
        }

        [TestMethod]
	    public void FalsyCanCreateTypesWithMembers()
	    {
	        NET.Falsy
	           .Define
               .PersonWithMembers(
	                   FirstName: typeof (string),
	                   LastName: typeof (string),
	                   Age: typeof (int)
	            );

	        var person =
                NET.Falsy
	               .New
	               .PersonWithMembers(
                           Member.Property("FirstName", "Steven"),
	                       Member.Property("Age", 25)
	                );

            Assert.IsNotNull((object)person);
            Assert.AreEqual("Steven", person.FirstName);
            Assert.IsNull(person.LastName);
	        Assert.AreEqual(25, person.Age);
            
	        person.LastName = "Thuriot";
	        person.Age = 28;

	        Assert.AreEqual("Thuriot", person.LastName);
	        Assert.AreEqual(28, person.Age);
	    }

	    [TestMethod]
	    public void FalsyCanCreateTypesWithMemberDefinitions()
	    {
	        NET.Falsy
	           .Define
               .PersonWithMemberDefinitions(
                       MemberDefinition.Property("FirstName", typeof(string)),
	                   LastName: typeof (string),
	                   Age: typeof (int)
	            );

	        var person =
                NET.Falsy
	               .New
                   .PersonWithMemberDefinitions(
                           Member.Property("Age", 25),
                           FirstName: "Steven"
	                );

            Assert.IsNotNull((object)person);
            Assert.AreEqual("Steven", person.FirstName);
            Assert.IsNull(person.LastName);
	        Assert.AreEqual(25, person.Age);
            
	        person.LastName = "Thuriot";
	        person.Age = 28;

	        Assert.AreEqual("Thuriot", person.LastName);
	        Assert.AreEqual(28, person.Age);
	    }

	    [TestMethod]
	    public void FalsyCanCreateTypesWithInterfaces()
	    {
	        NET.Falsy
	           .Define
               .WithInterface(typeof(IPerson))
	           .PersonWithInterface();

            IPerson person = NET.Falsy.New.PersonWithInterface();
            Assert.IsNotNull(person);

            person.FirstName = "Steven";
            person.LastName = "Thuriot";
            person.Age = 28;

            Assert.AreEqual("Steven", person.FirstName);
            Assert.AreEqual("Thuriot", person.LastName);
            Assert.AreEqual(28, person.Age);
	    }

	    [TestMethod]
	    public void FalsyTypesCanHaveAFakeConstructor()
	    {
	        NET.Falsy
	           .Define
               .WithInterface(typeof(IPerson))
               .PersonWithFakeCtor();

            IPerson person = NET.Falsy.New.PersonWithFakeCtor(FirstName: "Steven");
            Assert.IsNotNull(person);
            
            Assert.AreEqual("Steven", person.FirstName);
            Assert.IsNull(person.LastName);
            Assert.AreEqual(0, person.Age);
	    }

	    [TestMethod]
	    public void FalsyTypesCanNotifyChanges()
	    {
	        NET.Falsy
	           .Define
               .WithInterface(typeof(IPerson))
               .NotifyChanges()
               .PersonWhoNotifiesChanges();

            IPerson person = NET.Falsy.New.PersonWhoNotifiesChanges(FirstName: "Steven");
            Assert.IsNotNull(person);
            Assert.IsTrue(person is INotifyPropertyChanged);

            var notify = (INotifyPropertyChanged)person;
	        
            var count = 0;
	        var handler = new PropertyChangedEventHandler(delegate { count++; });
	        notify.PropertyChanged += handler;

	        person.FirstName = "Test";
	        person.Age = 28;

            notify.PropertyChanged -= handler;

            Assert.AreEqual("Test", person.FirstName);
            Assert.IsNull(person.LastName);
            Assert.AreEqual(28, person.Age);
            Assert.AreEqual(2, count);
	    }

	    [TestMethod]
	    public void ChangeNotificationsAreSmart()
	    {
	        NET.Falsy
	           .Define
               .WithInterface(typeof(IPerson))
               .NotifyChanges()
               .SmartPersonWhoNotifiesChanges();

            IPerson person = NET.Falsy.New.SmartPersonWhoNotifiesChanges(FirstName: "Steven");
            Assert.IsNotNull(person);
            Assert.IsTrue(person is INotifyPropertyChanged);

            var notify = (INotifyPropertyChanged)person;
	        
            var count = 0;
	        var handler = new PropertyChangedEventHandler(delegate { count++; });
	        notify.PropertyChanged += handler;

	        person.FirstName = "Test"; //1
	        person.FirstName = "Test";
	        person.FirstName = "Test";
	        person.FirstName = "Test";
	        person.FirstName = "Test2";//2
	        person.Age = 29;//3
            person.Age = 28;//4
            person.Age = 28;
	        person.Age = 15;//5
	        person.Age = 28;//6
	        person.Age = 28;
	        person.Age = 28;

            notify.PropertyChanged -= handler;

            Assert.AreEqual("Test2", person.FirstName);
            Assert.IsNull(person.LastName);
            Assert.AreEqual(28, person.Age);
            Assert.AreEqual(6, count);
	    }



	    [TestMethod]
	    public void FalsyCanInheritTypes()
        {
            NET.Falsy
               .Define
               .InheritFrom(typeof(Parent))
               .NotifyChanges()
               .ChildWhoNotifiesChanges();

            var child = NET.Falsy.New.ChildWhoNotifiesChanges();
	        
            Assert.IsNotNull((object) child);
            Assert.IsTrue(child is INotifyPropertyChanged);
            Assert.IsTrue(child is Parent);

            var notify = (INotifyPropertyChanged)child;

            var count = 0;
            var handler = new PropertyChangedEventHandler(delegate { count++; });
            notify.PropertyChanged += handler;

	        Parent parent = child;
            Assert.AreEqual(Parent.ImAVirtualGetterValue, parent.ImAVirtualGetter);

	        parent.ImAVirtualGetterSetter = 3; //1
	        
            parent.ImAVirtualSetter = 21; //2
	        
            parent.ImANormalGetterSetter = 54; //Still 2, since not virtual

            notify.PropertyChanged -= handler;

            Assert.AreEqual(2, count);
            Assert.AreEqual(21, parent.Test3_Field);
            Assert.AreEqual(54, parent.ImANormalGetterSetter);
        }

	    [TestMethod]
	    public void InheritedTypesCanNotifySmart()
        {
            NET.Falsy
               .Define
               .InheritFrom(typeof(Parent))
               .NotifyChanges()
               .ChildWhoNotifiesChangesSmart();

            var child = NET.Falsy.New.ChildWhoNotifiesChangesSmart();
	        
            Assert.IsNotNull((object) child);
            Assert.IsTrue(child is INotifyPropertyChanged);
            Assert.IsTrue(child is Parent);

            var notify = (INotifyPropertyChanged)child;

            var count = 0;
            var handler = new PropertyChangedEventHandler(delegate { count++; });
            notify.PropertyChanged += handler;

	        Parent parent = child;
            Assert.AreEqual(Parent.ImAVirtualGetterValue, parent.ImAVirtualGetter);

	        parent.ImAVirtualGetterSetter = 3; //1
	        parent.ImAVirtualGetterSetter = 3; //1
	        parent.ImAVirtualGetterSetter = 3; //1
	        parent.ImAVirtualGetterSetter = 5; //2
	        parent.ImAVirtualGetterSetter = 5; //2
	        parent.ImAVirtualGetterSetter = 3; //3
	        
            parent.ImAVirtualSetter = 21; //4
            parent.ImAVirtualSetter = 21; //5   Since this doesn't have a getter, we can't check the previous value.
            parent.ImAVirtualSetter = 21; //6
	        
            parent.ImANormalGetterSetter = 54; //Still 6, since not virtual
            parent.ImANormalGetterSetter = 54; //Still 6, since not virtual
            parent.ImANormalGetterSetter = 55; //Still 6, since not virtual
            parent.ImANormalGetterSetter = 54; //Still 6, since not virtual

            notify.PropertyChanged -= handler;

            Assert.AreEqual(6, count);
            Assert.AreEqual(21, parent.Test3_Field);
            Assert.AreEqual(54, parent.ImANormalGetterSetter);
        }


        [TestMethod]
	    public void FalsyCanHandleEvents()
        {
            NET.Falsy
               .Define
               .WithInterface(typeof(IPerson))
               .NotifyChanges()
               .NotifyablePerson();

            IPerson person = NET.Falsy.New.NotifyablePerson();

            var count = 0;
            var handler = new PropertyChangedEventHandler(delegate { count++; });
            
            IEventCaller caller = Info.GetEvent((dynamic) person, "PropertyChanged");
            
            caller.Add(person, handler);

            person.FirstName = "Steven";

            caller.Raise(person, "Test");
            
            caller.Remove(person, handler);

            person.FirstName = "Not Steven";

            Assert.AreEqual(2, count);
        }

        [TestMethod]
        public void FalsyCanImplementInterfaceMethods()
        {
            NET.Falsy
               .Define
               .WithInterface(typeof(IPersonWithMethods))
               .PersonWithMethods();

            var instance = NET.Falsy.New.PersonWithMethods();
            Assert.IsTrue(instance is IPerson);
            Assert.IsTrue(instance is IPersonWithMethods);

            IPersonWithMethods person = instance;
            var result = person.ThisIsAMethod();

            Assert.AreEqual(0, result);
        }

        //Note to self, when declaring this method as a variable inside our previous method, a System.MethodAccessException is thrown.
        //Remember that our new method lives inside a dynamic assembly and needs access to this method to be able to call it.
        public static int Multiply(int x, int y)
        {
            Assert.AreEqual(5, x);
            Assert.AreEqual(3, y);

            return x * y;
        }

        [TestMethod]
        public void FalsyCanImplementDelegatesAsMethods()
        {
            NET.Falsy
               .Define
               .TypeWithADelegateAsMethod(
                    Multiply: new Func<int, int, int>(Multiply)
                );

            var instance = NET.Falsy.New.TypeWithADelegateAsMethod();
            bool hasMethod = Info.HasMethod(instance, "Multiply");

            Assert.IsTrue(hasMethod);

            IMethodCaller method = Info.GetSpecificMethod(instance, "Multiply", typeof(int), typeof(int));

            Assert.IsNotNull(method);
            Assert.AreEqual(typeof(int), method.ReturnType);
            Assert.IsTrue(method.ParameterTypes.Count == 2);
            Assert.IsTrue(method.ParameterTypes.All(x => x.ParameterType == typeof(int)));

            int multiplication = instance.Multiply(5, 3);

            Assert.AreEqual(15, multiplication);
        }

        [TestMethod]
        public void FalsyCanInheritAnAbstractParent()
        {
            NET.Falsy
               .Define
               .InheritFrom(typeof(AbstractParent))
               .ImplementedAbstractChild(
                    MemberDefinition.Property("FirstName", typeof(string), isVirtual: false)
                );

            var child = NET.Falsy.New.ImplementedAbstractChild();

            var hasProperty = Info.HasProperty(child, "FirstName");
            Assert.IsTrue(hasProperty);

            hasProperty = Info.HasProperty(child, "Number");
            Assert.IsTrue(hasProperty);

            string name = child.FirstName;
            Assert.IsNull(name);

            child.FirstName = "Jos";
            name = child.FirstName;
            Assert.AreEqual("Jos", name);

            int number = child.Number;
            Assert.AreEqual(default(int), number);

            child.Number = 5;
            number = child.Number;
            Assert.AreEqual(number, 5);
        }

        [TestMethod]
        public void FalsyCanCreateAWrapperType()
        {
            NET.Falsy.WrapType(typeof(Wrappee))
                     .With(typeof(IWrapper))
                     .WrappedInstance();

            var wrappee = new Wrappee();
            var wrappedInstance = NET.Falsy.Wrap.WrappedInstance(wrappee);

            bool isType = wrappedInstance is IWrapper;
            Assert.IsTrue(isType);

            IWrapper wrappedInterface = (IWrapper)wrappedInstance;

            Assert.AreEqual(5, wrappedInterface.GetNumber());

            Assert.IsNull(wrappedInterface.Name);

            wrappedInterface.Name = "Steven";
            Assert.AreEqual("Steven", wrappedInterface.Name);
            Assert.AreEqual("Steven", wrappee.Name);
        }

        [TestMethod, ExpectedException(typeof(NotSupportedException))]
        public void FalsyCanCreateAWrapperTypeAndThrowNotSupportedExceptionsOnMissingParts()
        {
            NET.Falsy.WrapType(typeof(Wrappee))
                     .With(typeof(IPerson))
                     .ThrowNotSupported()
                     .WrappedPerson();

            var wrappee = new Wrappee();
            var wrappedInstance = NET.Falsy.Wrap.WrappedPerson(wrappee);

            bool isType = wrappedInstance is IPerson;
            Assert.IsTrue(isType);

            var wrappedInterface = (IPerson)wrappedInstance;

            var age = wrappedInterface.Age;
            Assert.Fail("Should have thrown an exception");
        }


        [TestMethod]
        public void FalsyWorksOnAnonymousTypes()
        {
            var anon = new
            {
                Name = "Steven",
                Age = 28
            };

            var falsy = anon.Falsify();

            string name = falsy.Name;
            Assert.AreEqual("Steven", name);

            int age = falsy.Age;
            Assert.AreEqual(28, age);
        }

        [TestMethod]
        public void FalsyWorksOnAnonymousTypesWithMethods()
        {
            var counter = 0;
            var anon = new
            {
                Name = "Steven",
                Age = 28,
                Speak = new Func<string, string>(x => "Hello " + x),
                Count = new Action(() => counter++),
                Add = new Action<int>(x => counter += x),
            };

            var falsy = anon.Falsify();

            string spoken = falsy.Speak("Bob");
            Assert.AreEqual("Hello Bob", spoken);

            falsy.Count();
            Assert.AreEqual(1, counter);

            falsy.Add(5);
            Assert.AreEqual(6, counter);
        }

        [TestMethod]
        public void FalsyVoidMethodsReturnUndefined()
        {
            var counter = 0;
            var anon = new
            {
                Action = new Action(() => counter++),
            };

            var falsy = anon.Falsify();
            var result = falsy.Action();
            Assert.AreEqual(1, counter);
            Assert.AreEqual(NET.Falsy.undefined, result);
        }

    }
}