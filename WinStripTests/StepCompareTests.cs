using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WinStrip.Entity;
using System.Linq;
using WinStrip.Utilities;

namespace WinStrip.StepCompareTests
{
    [TestClass]
    public class StepCompareTests
    {
        [TestMethod]
        public void Sort()
        {
            var list = new List<Step>();
            list.Add(new Step(10));
            list.Add(new Step(11));
            list.Add(new Step(10));
            list.Add(new Step(9));

            //Sort is sorted in reverse order.  that is high to low
            list.Sort(new Step());

            Assert.AreEqual(list[0].From, 9);
            Assert.AreEqual(list[1].From, 10);
            Assert.AreEqual(list[2].From, 10);
            Assert.AreEqual(list[3].From, 11);
        }

        [TestMethod]
        public void ComparisonOperatorEqualEqualNullTest()
        {
            var a1 = new Step(1);

            Step x = null;

            Assert.IsFalse(a1 == null);
            Assert.IsFalse(null == a1);

            Assert.IsTrue(x == null);
            Assert.IsTrue(null == x);

            Assert.IsFalse(x == a1);
            Assert.IsFalse(a1 == x);

            #pragma warning disable CS1718 // Comparison made to same variable
            Assert.IsTrue(x == x);
            #pragma warning restore CS1718 // Comparison made to same variable
            
            

            
        }

        [TestMethod]
        public void ComparisonOperatorEqualNotEqualNullTest()
        {
            var a1 = new Step(1);
            Step x = null;
            Assert.IsTrue(a1 != null);
            Assert.IsTrue(null != a1);

            Assert.IsTrue(a1 !=  x);
            Assert.IsTrue( x != a1);

            #pragma warning disable CS1718 // Comparison made to same variable
            Assert.IsFalse(x != x);
            #pragma warning restore CS1718 // Comparison made to same variable
        }

        [TestMethod]
        public void ComparisonOperatorEqualEqualTest()
        {
            var a01 = new Step();
            var a02 = new Step(0);
            var a1  = new Step(1);
            
            Assert.IsTrue( a01 == a02 );
            Assert.IsTrue( a02 == a01 );

            Assert.IsFalse( a01 == a1  );
            Assert.IsFalse( a02 == a1  );
            Assert.IsFalse( a1  == a01 );
        }

        [TestMethod]
        public void ComparisonOperatorNotEqualTest()
        {
            var a0 = new Step(0);
            var a1 = new Step(1);
            var a2 = new Step(2);
            var ax = new Step(2);

            Assert.IsTrue(a0 != a1);
            Assert.IsTrue(a1 != a0);
            Assert.IsTrue(a1 != a2);
            Assert.IsTrue(a2 != a1);
            Assert.IsFalse(a2 != ax);
        }

        [TestMethod]
        public void ComparisonOperatoLessTest()
        {
            var a0 = new Step(0);
            var a1 = new Step(1);
            var a2 = new Step(2);
            var ax = new Step(2);

            Assert.IsTrue(a0 < a1);
            Assert.IsTrue(a0 < a2);
            Assert.IsFalse(a1 < a0);
            Assert.IsFalse(a2 < a0);
            Assert.IsFalse(ax < a2);
            Assert.IsFalse(a2 < ax);

        }
    }
}
