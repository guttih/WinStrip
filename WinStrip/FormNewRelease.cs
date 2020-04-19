using System;
using System.Collections.Generic;
using System.Windows.Forms;
using WinStrip.Entity;

namespace WinStrip
{
    public partial class FormNewRelease : Form
    {
        private VersionInformation versionInfo;
        private const string tap = "    ";
        private const int MaxListLineLength = 80;
        private const int MaxBackWordSearchForWhitespace = 25;

        public FormNewRelease(VersionInformation versionInfo)
        {
            this.versionInfo = versionInfo;
            InitializeComponent();
        }

        string makeHeadParagraph(string head, string paragraph)
        {
            string str = $"{head.ToUpper()}\r\n";
                  str += $"{paragraph}\r\n";
            return str;
        }

        private string MakeListHead(string text)
        {
            string underline="";
            for (int i = 0; i < text.Length; i++)
                underline += "-";
            string str = $"{tap}{text     }\r\n";
                   str+= $"{tap}{underline}\r\n";
            return str;
        }

        private int findWordCutIndex(string line, int maxListLineLength)
        {
            int i             = maxListLineLength, 
                minLineLength = maxListLineLength - MaxBackWordSearchForWhitespace;
            
            if (minLineLength < 0)
                return maxListLineLength;

            char c;
            while(i > minLineLength && i > -1)
            {
                c = line[i];
                if (char.IsWhiteSpace(c))
                    return i;
                i--;
            }
            return maxListLineLength; //unable to find a word, so cutting at maxListLineLength
        }

        private string SplitIfLongLines(string line, string startLinePrefix="")
        {   
            if (string.IsNullOrEmpty(line))
                return "";

            if (line.Length <= MaxListLineLength)
                return line;
            int cutPos = findWordCutIndex(line, MaxListLineLength-startLinePrefix.Length);
            var ret = line.Remove(cutPos) + "\r\n" + startLinePrefix;
            line = line.Substring(cutPos+1);
            return ret + SplitIfLongLines(line, startLinePrefix);
        }

        

        private string MakeListItem(VesionFeature item)
        {   var str = $"\r\n";
                str =  SplitIfLongLines( $"{tap}{tap}{item.Title}",  $"{tap}{tap}" ) + "\r\n";
                str += SplitIfLongLines($"{tap}{tap} - {item.Description}", $"{tap}{tap}   ")  + "\r\n";
            return str;

        }

        private string MakeFeature(List<VesionFeature> features)
        {
            if (features.Count == 0)
                return "";

            var str = MakeListHead("New features");
                foreach (var listItem in versionInfo.NewFeatures)
                {
                    str += MakeListItem(listItem);
                }
            return str;
        }

        private void FormNewRelease_Load(object sender, EventArgs e)
        {
            labelVersion.Text = versionInfo.Version;
            labelName.Text = versionInfo.Name;
            labelDate.Text = versionInfo.Released;

            string text;
            text = $"\r\n{makeHeadParagraph("Description", versionInfo.Description)}\r\n\r\n";
            text += MakeFeature(versionInfo.NewFeatures);
            text += "\r\n\r\n";
            text += MakeFeature(versionInfo.BugFixes);
            text += "\r\n\r\n";

            if (!string.IsNullOrEmpty(versionInfo.Setup))
                text += makeHeadParagraph("Setup file", versionInfo.Setup);

            textBox.Text = text;
            int len = text.Length;
            textBox.Select(0, 0);
        }

        private void btnYes_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
            Close();
        }

        private void btnNo_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
            Close();
        }
    }
}
