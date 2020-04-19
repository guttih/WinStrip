using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using WinStrip.Entity;
using System.Globalization;
namespace WinStrip.Utilities.Tests
{
    [TestClass()]


    public class VersionInformationTests
    {
        private VersionInformation CreateCersionInformationWithValues(string versionString = null)
        {
            string jsonFile = @"{
    ""name"":""WinStrip"",
    ""version"":""1.2"",
    ""released"":""19.4.2020"",
    ""description"":""Appearance and help for the user was improved.  The Manual tab was renamed to Command.  The application can now run minimized and can be recovered by clicking the new tray Icon next to the clock on the windows taskbar.  From this version I beleave that this application is now usable by others."",
    ""newFeatures"": [
        {""title"":""Moved theme buttons to menu"",    ""description"":""Removed theme buttons save, load and reset and added their commands to the Menu""},
        {""title"":""Tooltips added to Main form"",""description"":""Added Tooltips to most controls and tip will also appear on the statusbar at the bottom of the form.""},
        {""title"":""Splash window"",         ""description"":""Display messages to the user when starting the application and while other actions that take some time are running.""},
        {""title"":""Launch minimized on startup"",         ""description"":""Add the ability to run the application minimized on startup, so if the app is added to the startup menu, the user will not have to minimize the app on every windows restart.""},
        {""title"":""Report bugs and feature requests"",         ""description"":""Added menu items to allow users to report bugs and feature requests.""},
        {""title"":""Release dialog"",         ""description"":""A new release dialog will be shown when checking for a update. The dialog will contain information about the new release.""}
    ],
    ""bugFixes"": [
        {""title"":""Appearance issues"",    ""description"":""Many appearance issues where fixed. ""}
    ],
    ""setup"":""WinStripSetup.msi""
}";
            
            var ret = new VersionInformation(jsonFile);

            if (versionString != null)
                ret.Version = versionString;
            return ret;
        }
        [TestMethod()]
        public void RandomTestsTest()
        {
            var verInfo = CreateCersionInformationWithValues("1.2.3.4");
            var verNum = new VersionNumbers(verInfo.Version);

            Assert.IsTrue(verNum.Major == 1 && verNum.Minor == 2 && verNum.Patch == 2 && verNum.Build == 4);
            Assert.IsTrue(verInfo.IsVersionEqual("1.2.3.4"));
            Assert.IsTrue(verInfo.VersionCompare("1.2.3.4") == 0);

        }
    }
}