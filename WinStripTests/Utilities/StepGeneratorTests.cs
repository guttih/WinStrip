using WinStrip.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using WinStrip.Entity;
using System.Globalization;

namespace WinStrip.Utilities.Tests
{
    [TestClass()]
    public class StepGeneratorTests
    {
        [TestMethod()]
        public void StripDimToBrightTest()
        {
            var toColor = UInt32.Parse("FFFFFF", NumberStyles.AllowHexSpecifier);
            var step = new Step(0, "{\"delay\":0,\"com\":2,\"brightness\":1,\"values\":[0,0,0],\"colors\":[255,16711680,32768,255,16777215,10824234]}");
            var steps = StepGenerator.StripDimToBright(step, 0, toColor, 0, 31, 1);
            Assert.IsTrue(steps.Count == 32);
            Assert.IsTrue(steps[0].ValuesAndColors.colors[0] == 0);
            Assert.IsTrue(steps[31].ValuesAndColors.colors[0] == toColor);
        }

        [TestMethod()]
        public void StripDimToBright_WhenColorsAreNoneTest()
        {
            var toColor = UInt32.Parse("FFFFFF", NumberStyles.AllowHexSpecifier);
            var step = new Step(0, "{\"delay\":0,\"com\":2,\"brightness\":1,\"values\":[0,0,0],\"colors\":[]}");
            var steps = StepGenerator.StripDimToBright(step, 0, toColor, 0, 31, 1);
            Assert.IsTrue(steps.Count == 0);
        }

        [TestMethod()]
        public void StripTestsTest1()
        {
            var step1 = new Step(0, "{\"delay\":1000,\"com\":2,\"brightness\":  1,\"values\":[0,0,0],\"colors\":[255,16711680,32768,255,16777215,10824234]}", true);
            var step2 = new Step(0, "{\"delay\":   0,\"com\":2,\"brightness\":255,\"values\":[0,0,0],\"colors\":[255,16711680,32768,255,16777215,10824234]}", true);
            var list = StepGenerator.StripSteps(step1, step2);
            Assert.IsTrue(list.Count > 0);
        }
    }
}