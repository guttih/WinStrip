using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace WinStrip
{
    public partial class FormExport : BaseForm
    {
        public List<string> FileContent;
        public string CodeFilePath { get; set; }

        const string DEFINE_NUM_LEDS      = "#define NUM_LEDS ";
        const string DEFINE_STRIP_TYPE    = "#define STRIP_TYPE ";
        const string DEFINE_CLOCK_PIN     = "#define CLOCK_PIN ";
        const string DEFINE_DATA_PIN      = "#define DATA_PIN ";
        const string DEFINE_COLOR_SCHEME  = "#define COLOR_SCHEME ";
        const string CONST_CHAR_STRIPTYPE = "const char* stripType = ";
        const string PINMODE_CLOCK_PIN    = "pinMode(CLOCK_PIN";
        const string FASTLED_ADDLEDS      = "FastLED.addLeds<STRIP_TYPE, DATA_PIN, ";
        public FormExport(string codeFilePath)
        {
            CodeFilePath = codeFilePath;
            InitializeComponent();
            
        }

        private void FormExport_Load(object sender, EventArgs e)
        {
            FileContent = ReadFileToList(CodeFilePath);
            
            InitValues();
            /*
            #define STRIP_TYPE WS2801
            #define COLOR_SCHEME RBG
            */
            
        }

        

        private void InitValues()
        {
            comboType.Items.Add("WS2801");
            comboType.Items.Add("WS2813");
            comboType.Items.Add("APA102");
            comboColorScheme.Items.Add("RGB");
            comboColorScheme.Items.Add("RBG");
            comboColorScheme.Items.Add("GRB");
            comboColorScheme.Items.Add("GBR");
            comboColorScheme.Items.Add("BRG");
            comboColorScheme.Items.Add("BGR");

            InitComboPinValues(comboClockPin);
            InitComboPinValues(comboDataPin);

            int ledCount = ExtractIntegerFromLineInListWichStartsWith(DEFINE_NUM_LEDS, FileContent);
            numericUpDownLedCount.Value = ledCount;

            SelectWordFromCodeWhereLineStartsWith(comboType, DEFINE_STRIP_TYPE);
            SelectWordFromCodeWhereLineStartsWith(comboColorScheme, DEFINE_COLOR_SCHEME);
            SelectWordFromCodeWhereLineStartsWith(comboClockPin, DEFINE_CLOCK_PIN);
            SelectWordFromCodeWhereLineStartsWith(comboDataPin, DEFINE_DATA_PIN);
        }

        private void InitComboPinValues(ComboBox comboPin)
        { 
            comboPin.Items.Add("0");
            comboPin.Items.Add("1");
            comboPin.Items.Add("2");
            comboPin.Items.Add("3");
            comboPin.Items.Add("5");
            comboPin.Items.Add("12");
            comboPin.Items.Add("13");
            comboPin.Items.Add("14");
            comboPin.Items.Add("15");
            comboPin.Items.Add("16");
            comboPin.Items.Add("17");
            comboPin.Items.Add("18");
            comboPin.Items.Add("19");
            comboPin.Items.Add("21");
            comboPin.Items.Add("22");
            comboPin.Items.Add("23");
            comboPin.Items.Add("23");
            comboPin.Items.Add("25");
            comboPin.Items.Add("26");
            comboPin.Items.Add("27");
            comboPin.Items.Add("32");
            comboPin.Items.Add("33");
        }

        private void SelectWordFromCodeWhereLineStartsWith(ComboBox combo, string startsWithMe, int defaultIfstartsWithMeNotFound = 0)
        {
            if (combo.Items.Count < 1)
                return;

            int lineIndex = IndexOfLineWhichStartsWith(startsWithMe, FileContent);
            if (lineIndex > -1)
            {
                string line = FileContent[lineIndex];
                string val = ExtractFirstWordFromCodeWhichStartsWith(startsWithMe, line);
                int i = combo.Items.IndexOf(val);
                if (i < 0)
                    i = defaultIfstartsWithMeNotFound;
                
                combo.SelectedIndex = i;
            }
        }

        internal void WriteSelectedContentToFile(string inoFilePath)
        {
            File.WriteAllLines(inoFilePath, FileContent);
        }

        private string ExtractFirstWordFromCodeWhichStartsWith(string startsWith, string line)
        {
            line = line.Trim();
            var val = line.Remove(0, startsWith.Length);
            val = val.TrimStart();
            if (val.Length < 1)
                return "";

            string ret = "";
            int i = 0;
            char c;
            while (( i < val.Length ))
            {
                c = val[i++];
                if (char.IsWhiteSpace(c) || c == ';' || c == '/')
                    break;
                ret += c;
            }
            return ret;
        }

        int IndexOfLineWhichStartsWith(string startsWith, List<string> lines)
        {
            for(int i = 0; i<lines.Count; i++)
            {
                string line = lines[i];
                line = line.TrimStart();
                if (line.StartsWith(startsWith))
                {
                    return i;
                }
            }
            return -1;
        }

        private string replaceWordThatStartsWith(string currentStringWithExistingValue, string existingWord, string newWord)
        {
            var i = currentStringWithExistingValue.IndexOf(existingWord);
            if (i < 0)
                throw new IntegerNotFoundInLineException("Existing integer value is was not");
            string ret = currentStringWithExistingValue.Remove(i, existingWord.Length);
            ret = ret.Insert(i, newWord);
            return ret;
        }

        private int ExtractIntegerFromLineWichStartsWith(string startsWith, string stringToExtractFrom)
        {
            var line = stringToExtractFrom;
            line = line.TrimStart();
            line = line.TrimEnd();
            //line.TrimEnd(';');

            string ret = "",
                    strNum = line.Substring(startsWith.Length).TrimStart().TrimEnd();
            char c;
            int i = -1,
                len = strNum.Length;
            if (len < 1)
                throw new IntegerNotFoundInLineException();

            do
            {
                c = strNum[++i];

                if (!(c >= '0' && c <= '9'))
                    break;

                ret += c;
            } while (i < len -1);

            if (ret.Length < 1)
                throw new IntegerNotFoundInLineException();

            return Convert.ToInt32(ret);
        }
        private int ExtractIntegerFromLineInListWichStartsWith(string startsWith, List<string> lines)
        {
            int lineIndex = IndexOfLineWhichStartsWith(startsWith, lines);
            if (lineIndex < 0)
                throw new StartsWithFoundInFileException();
            
            string line = lines[lineIndex];

            return ExtractIntegerFromLineWichStartsWith(startsWith, line);
            
        }

        private List<string> ReadFileToList(string filePath)
        {
            var list = new List<string>();
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                string line;
                while ((line = streamReader.ReadLine()) != null)
                {
                    list.Add(line);
                }
            }
            return list;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void SetStringTextToContent(string startWith, string newText)
        {
            int lineIndex = IndexOfLineWhichStartsWith(startWith, FileContent);
            if (lineIndex > -1)
            {
                string line = FileContent[lineIndex];
                string oldWord = ExtractFirstWordFromCodeWhichStartsWith(startWith, line);
                if (oldWord.Length > 0)
                {
                    string newWord = newText;
                    if (oldWord.StartsWith("\"") && oldWord.EndsWith("\""))
                    {
                        newWord = $"\"{newWord}\"";
                    }
                    string newLine = replaceWordThatStartsWith(line, oldWord, newWord);
                    FileContent[lineIndex] = newLine;
                }
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            SetIntegerTextToContent(DEFINE_NUM_LEDS     , Convert.ToInt32(numericUpDownLedCount.Value).ToString());
            
            SetIntegerTextToContent(DEFINE_DATA_PIN     , comboDataPin.Text);
            SetStringTextToContent (DEFINE_STRIP_TYPE   , comboType.Text);
            SetStringTextToContent (CONST_CHAR_STRIPTYPE, comboType.Text);
            SetStringTextToContent(DEFINE_COLOR_SCHEME  , comboColorScheme.Text);

            if (checkHasClockPIn.Checked)
            {
                SetIntegerTextToContent(DEFINE_CLOCK_PIN, comboClockPin.Text);
            }
            else
            {
                //no clock pin so let's remove it
                int lineIndex = IndexOfLineWhichStartsWith(PINMODE_CLOCK_PIN, FileContent);
                if (lineIndex > -1)
                {
                    FileContent.RemoveAt(lineIndex);
                    lineIndex = IndexOfLineWhichStartsWith(FASTLED_ADDLEDS, FileContent);
                    if (lineIndex > -1)
                    {
                        SetStringTextToContent(FASTLED_ADDLEDS, "");
                    }
                }
                /*
                 const string PINMODE_CLOCK_PIN    = "pinMode(CLOCK_PIN";
                 const string FASTLED_ADDLEDS      = "FastLED.addLeds<STRIP_TYPE, DATA_PIN, ";
                 */


            }

            DialogResult = DialogResult.OK;
            Close();
        }

        private void SetIntegerTextToContent(string startsWith, string newText)
        {
            int lineIndex;
            string line, newLine;

            lineIndex = IndexOfLineWhichStartsWith(startsWith, FileContent);
            if (lineIndex > -1) { 
                line = FileContent[lineIndex];
                int oldValue = ExtractIntegerFromLineWichStartsWith(startsWith, line);
                newLine = replaceWordThatStartsWith(line, oldValue.ToString(), newText);
                FileContent[lineIndex] = newLine;
            }
        }

        private void numericUpDownLedCount_ValueChanged(object sender, EventArgs e)
        {
            SetButtonState();
        }

        private void SetButtonState()
        {
            bool enable = numericUpDownLedCount.Value > 0;
            if (enable) {
                if (checkHasClockPIn.Checked)
                {
                    enable = comboClockPin.Text != comboDataPin.Text;
                }
            }
            btnExport.Enabled = enable;
        }

        private void checkHasClockPIn_CheckedChanged(object sender, EventArgs e)
        {
            comboClockPin.Enabled = checkHasClockPIn.Checked;
            SetButtonState();
        }

        private void comboDataPin_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetButtonState();
        }

        private void comboClockPin_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetButtonState();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            VisitHelpUrl("exportcode.html");
        }
    }

    [Serializable]
    internal class StartsWithFoundInFileException : Exception
    {
        public StartsWithFoundInFileException()
        {
        }

        public StartsWithFoundInFileException(string message) : base(message)
        {
        }
    }
    
    [Serializable]
    class IntegerNotFoundInLineException : IOException
    {
        public IntegerNotFoundInLineException() : base()
        {
        }

        public IntegerNotFoundInLineException(string message) : base(message)
        {
        }
    }
}
