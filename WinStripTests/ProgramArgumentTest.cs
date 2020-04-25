using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WinStrip.Utilities;

namespace WinStrip.Enums
{
    [TestClass]
    public class ProgramArgumentTest
    {
        [TestMethod]
        public void ConvertAllEnumsToAndFromString()
        {
            System.Array values = Enum.GetValues(typeof(ProgramArgument));
            foreach(var valEnum in values)
            {
                var val = (ProgramArgument)valEnum;
                if (val < ProgramArgument.COUNT)
                {
                    string str = val.ToString();
                    Assert.IsTrue(val == ProgramArgumentHelper.GetEnum(val.ToString()));
                }
                
            }
        }
    }
}
