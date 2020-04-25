﻿using System.Reflection;
using System.Windows.Forms;

namespace WinStrip
{
    public partial class BaseForm : Form
    {
        public string RootUrl
        {
            get
            {
                return Properties.Settings.Default.HelpRootUrl;
            }
        }

        public string RepositoryRootUrl
        {
            get
            {
                return Properties.Settings.Default.RepositoryRootUrl;
            }
        }

        public string ThemeFileExtendion
        {
            get
            {
                return Properties.Settings.Default.ThemeFileExtendion;
            }
        }

        public string MajorMinorVersion
        {
            get
            {
                var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                return $"{version.Major}.{version.Minor}";
            }
        }
        public System.Version Version
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            }
        }
        public string HelpRootUrl { get {
                var rootUrl = Properties.Settings.Default.HelpRootUrl;
                return $"{rootUrl}/{MajorMinorVersion}";
            } 
        }

        public void VisitHelpUrl(string webpageName = null)
        {
            var href = HelpRootUrl;
            if (webpageName != null)
                href += $"/{ webpageName}";
            System.Diagnostics.Process.Start(href);
        }

        public void VisitUrl(string fullUrlToWebPage)
        {
            var href = fullUrlToWebPage;
            System.Diagnostics.Process.Start(href);
        }

        //Also refferred to as AssemblyTitle
        public string ApplicationName
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();
            // 
            // BaseForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Name = "BaseForm";
            this.Load += new System.EventHandler(this.BaseForm_Load);
            this.ResumeLayout(false);

        }

        private void BaseForm_Load(object sender, System.EventArgs e)
        {

        }
    }
}