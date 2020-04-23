using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WinStrip.Entity;
using System.Linq;

namespace WinStrip.ThemeTests
{
    [TestClass]
    public class ThemeTests
    {
        [TestMethod]
        public void SortStepsAndFixTest()
        {
            Theme theme = new Theme();
            theme.Steps.Add(new Step(66));
            theme.Steps.Add(new Step(66));
            theme.Steps.Add(new Step(66));
            theme.Steps.Add(new Step(66));
            theme.Steps.Add(new Step(1));
            theme.Steps.Add(new Step(1));
            theme.Steps.Add(new Step(1));
            theme.SortStepsAndFix();

            Assert.IsTrue(theme.Steps.Count == 2);
            Assert.IsTrue(theme.Steps[0].From == 0);
            Assert.IsTrue(theme.Steps[1].From == 66);
        }
    }
}
