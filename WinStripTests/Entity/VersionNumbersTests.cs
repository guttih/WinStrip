using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using WinStrip.Entity;
using System.Globalization;
namespace WinStrip.Utilities.Tests
{
    [TestClass()]
    public class VersionNumbersTests
    {
        [TestMethod()]
        public void RandomTestsTest()
        {
            var ver1 = new VersionNumbers("1.1");
            Assert.AreEqual(1, ver1.Major);
            Assert.AreEqual(1, ver1.Minor);
            Assert.AreEqual(0, ver1.Patch);
            Assert.AreEqual(0, ver1.Build);
            
            var ver2 = new VersionNumbers("1.1.0.0");
            Assert.AreEqual(1, ver2.Major);
            Assert.AreEqual(1, ver2.Minor);
            Assert.AreEqual(0, ver2.Patch);
            Assert.AreEqual(0, ver2.Build);

            Assert.IsTrue(ver1.Compare(ver2) == 0);
            Assert.IsFalse(ver1 == ver2);
            Assert.IsTrue(ver1.Equal(ver2));
            Assert.IsTrue(ver2.Equal(ver1));
        }

        [TestMethod()]
        public void ConstructorWithInvalidStringParameterTest()
        {
            Assert.ThrowsException<ArgumentException>(() => { var i = new VersionNumbers(""); });
            Assert.ThrowsException<ArgumentException>(() => { var i = new VersionNumbers("x"); });
            Assert.ThrowsException<ArgumentException>(() => { var i = new VersionNumbers("1.s"); });
            Assert.ThrowsException<ArgumentException>(() => { var i = new VersionNumbers(". "); });
            Assert.ThrowsException<ArgumentException>(() => { var i = new VersionNumbers(" ."); });
            Assert.ThrowsException<ArgumentException>(() => { var i = new VersionNumbers(" . "); });
            Assert.ThrowsException<ArgumentException>(() => { var i = new VersionNumbers("1.2.3.4.5"); });
        }

        [TestMethod()]
        public void ConstructorWithValidStringParameterTest()
        {
            var ver = new VersionNumbers("1");                      Assert.IsTrue(ver.Major ==   1 && ver.Minor ==    0 && ver.Patch ==     0 && ver.Build ==       0);
                ver = new VersionNumbers("1.2");                    Assert.IsTrue(ver.Major ==   1 && ver.Minor ==    2 && ver.Patch ==     0 && ver.Build ==       0);
                ver = new VersionNumbers("1.2.3");                  Assert.IsTrue(ver.Major ==   1 && ver.Minor ==    2 && ver.Patch ==     3 && ver.Build ==       0);
                ver = new VersionNumbers("600.7000.80000.9000000"); Assert.IsTrue(ver.Major == 600 && ver.Minor == 7000 && ver.Patch == 80000 && ver.Build == 9000000);
        }

        [TestMethod()]
        public void IsEqualTest()
        {
            Assert.IsTrue(new VersionNumbers().Equal(new VersionNumbers()));
            Assert.IsTrue(new VersionNumbers("1.0").Equal(new VersionNumbers("1.0")));
            Assert.IsTrue(new VersionNumbers("1.0").Equal(new VersionNumbers("1")));
            Assert.IsTrue(new VersionNumbers("1").Equal(new VersionNumbers("1.0")));
        }

        [TestMethod()]
        public void VersionNumbersAreLargerTest()
        {
            Assert.IsTrue (new VersionNumbers("2").Larger(new VersionNumbers("1")));
            Assert.IsFalse(new VersionNumbers("1").Larger(new VersionNumbers("2")));
            Assert.IsFalse(new VersionNumbers("1").Larger(new VersionNumbers("1")));

            Assert.IsTrue(new VersionNumbers("1.2").Larger(new VersionNumbers("1.1")));
            Assert.IsFalse(new VersionNumbers("1.1").Larger(new VersionNumbers("1.2")));
            Assert.IsFalse(new VersionNumbers("1.1").Larger(new VersionNumbers("1.1")));

            Assert.IsTrue(new VersionNumbers("1.2.3").Larger(new VersionNumbers("1.2.1")));
            Assert.IsFalse(new VersionNumbers("1.2.1").Larger(new VersionNumbers("1.2.3")));
            Assert.IsFalse(new VersionNumbers("1.2.1").Larger(new VersionNumbers("1.2.1")));

            Assert.IsTrue(new VersionNumbers("1.2.3.1").Larger(new VersionNumbers("1.2.3.0")));
            Assert.IsFalse(new VersionNumbers("1.2.3.0").Larger(new VersionNumbers("1.2.3.1")));
            Assert.IsFalse(new VersionNumbers("1.2.3.0").Larger(new VersionNumbers("1.2.3.0")));
        }

        [TestMethod()]
        public void VersionNumbersAreLessTest()
        {
            Assert.IsTrue(new VersionNumbers("1").Less(new VersionNumbers("2")));
            Assert.IsFalse(new VersionNumbers("2").Less(new VersionNumbers("1")));
            Assert.IsFalse(new VersionNumbers("1").Less(new VersionNumbers("1")));

            Assert.IsTrue(new VersionNumbers("1.1").Less(new VersionNumbers("1.2")));
            Assert.IsFalse(new VersionNumbers("1.2").Less(new VersionNumbers("1.1")));
            Assert.IsFalse(new VersionNumbers("1.1").Less(new VersionNumbers("1.1")));

            Assert.IsTrue(new VersionNumbers("1.2.1").Less(new VersionNumbers("1.2.3")));
            Assert.IsFalse(new VersionNumbers("1.2.3").Less(new VersionNumbers("1.2.1")));
            Assert.IsFalse(new VersionNumbers("1.2.1").Less(new VersionNumbers("1.2.1")));

            Assert.IsTrue(new VersionNumbers("1.2.3.0").Less(new VersionNumbers("1.2.3.1")));
            Assert.IsFalse(new VersionNumbers("1.2.3.1").Less(new VersionNumbers("1.2.3.0")));
            Assert.IsFalse(new VersionNumbers("1.2.3.0").Less(new VersionNumbers("1.2.3.0")));
        }
    }
}