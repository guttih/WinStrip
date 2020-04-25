using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using WinStrip.Entity;
using WinStrip.Enums;

namespace WinStrip.Utilities
{
    public static class ProgramArgumentsHandler
    {
        /// <summary>
        /// Runs a command based on program arguments
        /// </summary>
        /// <param name="args"></param>
        /// <returns>
        /// null: if the application should continue running.   
        /// array of strings if if the application should stop running
        /// </returns>
        internal static string[] Execute(string[] args)
        {
            if (args.Length < 2 || string.IsNullOrWhiteSpace(args[1]))
                return null;

            string fyrst = args[1].Trim().ToUpper();

            ProgramArgument command = ProgramArgumentHelper.GetEnum(fyrst);

            switch(command)
            {
                case ProgramArgument.BUILDFORSETUP: return BuildForSetup(args);
            }

            return null;
        }

        private static string[] BuildForSetup(string[] args)
        {
            var content = System.IO.File.ReadAllText(@"..\..\release.json");

            var serializer = new JavaScriptSerializer();
            VersionInformation versionInfo;

            try
            {
                versionInfo = serializer.Deserialize<VersionInformation>(content);
            }
            catch (Exception)
            {
                return new string[] { "Unable to Deserialize release.json" };
            }

            var frm = new BaseForm();
            var ver = frm.Version;
            versionInfo.Version = frm.VersionString;
            content = serializer.Serialize(versionInfo);
            System.IO.File.WriteAllText(@"..\..\release.json",content);
            

            return new string[] { "BuildForSetup Done" };



        }
    }
}
