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

        public string MajorMinorVersion
        {
            get
            {
                var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                return $"{version.Major}.{version.Minor}";
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
    }
}