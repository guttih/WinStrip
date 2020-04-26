using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Windows.Forms;
using WinStrip.Utilities;
using System.Runtime.InteropServices;
using System.IO;

namespace WinStrip
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var ret = ProgramArgumentsHandler.Execute(Environment.GetCommandLineArgs());
            if (ret != null)
            {
                File.WriteAllLines("output.txt",ret);
                return;
            }

            Application.Run(new FormMain());
        }
    }
}
