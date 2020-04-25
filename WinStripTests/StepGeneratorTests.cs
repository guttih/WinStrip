using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using WinStrip.Entity;
using System.Globalization;
using WinStrip.Utilities;

namespace WinStrip.Utilities.StepGeneratorTests
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
        public void WhenListShouldHaveCount2()
        {
            //from same
            var step1 = new Step(0, "{\"delay\":1000,\"com\":2,\"brightness\":  1,\"values\":[0,0,0],\"colors\":[0,0,0,0,0,0]}", true);
            var step2 = new Step(0, "{\"delay\":   0,\"com\":2,\"brightness\":255,\"values\":[0,0,0],\"colors\":[16777215,16777215,16777215,16777215,16777215,16777215]}", true);
            var list = StepGenerator.StripSteps(step1, step2, new StepDifferenceParameters { From = 1, Brightness = true});                                                    
            Assert.IsTrue(list.Count == 2);
            Assert.IsTrue(list[0].From == 0);
            Assert.IsTrue(list[0].ValuesAndColors.delay == 1000);
            Assert.IsTrue(list[1].From == 0);
            Assert.IsTrue(list[1].ValuesAndColors.delay == 0);

            //from right order
            step1 = new Step(0, "{\"delay\":   0,\"com\":2,\"brightness\":  1,\"values\":[0,0,0],\"colors\":[0,0,0,0,0,0]}", true);
            step2 = new Step(1, "{\"delay\":1000,\"com\":2,\"brightness\":255,\"values\":[0,0,0],\"colors\":[16777215,16777215,16777215,16777215,16777215,16777215]}", true);
            list = StepGenerator.StripSteps(step1, step2, new StepDifferenceParameters { From = 1, Brightness = true});
            Assert.IsTrue(list.Count == 2);
            Assert.IsTrue(list[0].From == 0);
            Assert.IsTrue(list[0].ValuesAndColors.delay == 0);
            Assert.IsTrue(list[1].From == 1);
            Assert.IsTrue(list[1].ValuesAndColors.delay == 1000);

            //from reverse order
            step1 = new Step(1, "{\"delay\":   0,\"com\":2,\"brightness\":  1,\"values\":[0,0,0],\"colors\":[0,0,0,0,0,0]}", true);
            step2 = new Step(0, "{\"delay\":1000,\"com\":2,\"brightness\":255,\"values\":[0,0,0],\"colors\":[16777215,16777215,16777215,16777215,16777215,16777215]}", true);
            list = StepGenerator.StripSteps(step1, step2, new StepDifferenceParameters { From = 1, Brightness = true});
            Assert.IsTrue(list.Count == 2);
            Assert.IsTrue(list[0].From == 0);
            Assert.IsTrue(list[0].ValuesAndColors.delay == 1000);
            Assert.IsTrue(list[1].From == 1);
            Assert.IsTrue(list[1].ValuesAndColors.delay == 0);
        }

        [TestMethod()]
        public void WhenStepDifferenceParametersFromIsTrue()
        {
            var step1 = new Step(0, "{\"delay\":1000,\"com\":2,\"brightness\":  1,\"values\":[0,0,0],\"colors\":[0,0,0,0,0,0]}", true);
            var step2 = new Step(0, "{\"delay\":   0,\"com\":2,\"brightness\":255,\"values\":[0,0,0],\"colors\":[16777215,16777215,16777215,16777215,16777215,16777215]}", true);
            var list = StepGenerator.StripSteps(step1, step2, new StepDifferenceParameters { From = 1, Brightness = true});
            Assert.IsTrue(list.Count == 2);
            Assert.IsTrue(list[0].ValuesAndColors.delay == 1000);
            Assert.IsTrue(list[1].ValuesAndColors.delay == 0);
            Assert.IsTrue(list[0].ValuesAndColors.colors[0] == 0);
            Assert.IsTrue(list[1].ValuesAndColors.colors[0] == 16777215);

            step1 = new Step(0, "{\"delay\":1000,\"com\":2,\"brightness\":  1,\"values\":[0,0,0],\"colors\":[0,0,0,0,0,0]}", true);
            step2 = new Step(1, "{\"delay\":   0,\"com\":2,\"brightness\":255,\"values\":[0,0,0],\"colors\":[16777215,16777215,16777215,16777215,16777215,16777215]}", true);
            list = StepGenerator.StripSteps(step1, step2, new StepDifferenceParameters { From = 1, Brightness = true});
            Assert.IsTrue(list.Count == 2);
            Assert.IsTrue(list[0].From == 0);
            Assert.IsTrue(list[1].From == 1);
            Assert.IsTrue(list[0].ValuesAndColors.delay == 1000);
            Assert.IsTrue(list[1].ValuesAndColors.delay == 0);
            Assert.IsTrue(list[0].ValuesAndColors.colors[0] == 0);
            Assert.IsTrue(list[1].ValuesAndColors.colors[0] == 16777215);
            
            step1 = new Step(0, "{\"delay\":1000,\"com\":2,\"brightness\":  1,\"values\":[0,0,0],\"colors\":[0,0,0,0,0,0]}", true);
            step2 = new Step(2, "{\"delay\":   0,\"com\":2,\"brightness\":255,\"values\":[0,0,0],\"colors\":[16777215,16777215,16777215,16777215,16777215,16777215]}", true);
            list = StepGenerator.StripSteps(step1, step2, new StepDifferenceParameters { From = 1, Brightness = true, Delay = true });
            Assert.IsTrue(list.Count == 3);
            Assert.IsTrue(list[0].From == 0);
            Assert.IsTrue(list[1].From == 1);
            Assert.IsTrue(list[2].From == 2);
            Assert.IsTrue(list[0].ValuesAndColors.delay == 1000);
            Assert.IsTrue(list[1].ValuesAndColors.delay == 500);
            Assert.IsTrue(list[2].ValuesAndColors.delay == 0);
        }

        [TestMethod()]
        public void WhenColorsAreNone()
        {
            var step1 = new Step( 0, "{\"delay\":1000,\"com\":2,\"brightness\":  1,\"values\":[0,0,0],\"colors\":[0,0,0,0,0,0]}", true);
            var step2 = new Step(1, "{\"delay\":0,\"com\":2,\"brightness\":1,\"values\":[0,0,0],\"colors\":[]}");
            var list = StepGenerator.StripSteps(step1, step2, new StepDifferenceParameters { From = 1, Brightness = true});
            Assert.IsTrue(list.Count == 2);
            Assert.IsTrue(list[0].ValuesAndColors.delay == 1000);
            Assert.IsTrue(list[1].ValuesAndColors.delay == 0);

        }

        [TestMethod()]
        public void VariousChanges()
        {
            var step1 = new Step(31,"{\"delay\":0,\"com\":4,\"brightness\":255,\"values\":[0,0,0],\"colors\":[16777215,16711680,32768,255,16777215,10824234]}", true);
            var step2 = new Step(0, "{\"delay\":2000,\"com\":4,\"brightness\":1,\"values\":[0,0,0],\"colors\":[255,0,32768,255,16777215,10824234]}");
            var list = StepGenerator.StripSteps(step1, step2, new StepDifferenceParameters { From = 1, Brightness = true });
            Assert.IsTrue(list.Count == 32);
            Assert.IsTrue(list[0].ValuesAndColors.delay ==  2000);
            Assert.IsTrue(list[31].ValuesAndColors.delay ==    0);
            string str="";
            list.ForEach(e => str+=$"{new SColor(e.ValuesAndColors.colors[0]).ToString()}\n"   );
        }

        [TestMethod()]
        public void VariousChangesInReverse()
        {
            var step1 = new Step(0, "{\"delay\":0,\"com\":4,\"brightness\":255,\"values\":[0,0,0],\"colors\":[16777215,16711680,32768,255,16777215,10824234]}", true);
            var step2 = new Step(31, "{\"delay\":2000,\"com\":4,\"brightness\":1,\"values\":[0,0,0],\"colors\":[255,0,32768,255,16777215,10824234]}");
            var list = StepGenerator.StripSteps(step1, step2, new StepDifferenceParameters { From = 1, Brightness = true });
            Assert.IsTrue(list.Count == 32);
            Assert.IsTrue(list[31].ValuesAndColors.delay == 2000);
            Assert.IsTrue(list[0].ValuesAndColors.delay == 0);
            string str = "";
            list.ForEach(e => str += $"{new SColor(e.ValuesAndColors.colors[0]).ToString()}\n");
        }

        [TestMethod()]
        public void ValueChanges()
        {
            var step1 = new Step(0,  "{\"delay\":  0,\"com\":4,\"brightness\":255,\"values\":[ 0, 0, 0],\"colors\":[ 0, 0, 0, 0, 0, 0]}", true);
            var step2 = new Step(10, "{\"delay\":100,\"com\":4,\"brightness\":  1,\"values\":[10,10,10],\"colors\":[20,20,20,20,20,20]}", true);
            var list = StepGenerator.StripSteps(step1, step2, new StepDifferenceParameters { From = 1, Brightness = true });
            string str = list[0].ToString();
            Assert.IsTrue(list.Count == 11);
            Assert.IsTrue(list[10].ValuesAndColors.delay == 100);
            Assert.IsTrue(list[0].ValuesAndColors.delay == 0);
        }
    }
}