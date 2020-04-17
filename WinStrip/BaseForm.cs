using System.Windows.Forms;

namespace WinStrip
{
    public partial class BaseForm : Form
    {
        public string HelpRootUrl { get {
                var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                var rootUrl = Properties.Settings.Default.HelpRootUrl;
                return $"{rootUrl}/{version.Major}.{version.Minor}";
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