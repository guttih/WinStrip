using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using WinStrip.Entity;

namespace WinStrip
{
    public partial class FormThemeImportExport : BaseForm
    {
        private bool Importing;
        private bool JustCloseTheForm;
        private List<Theme> ThemeList;
        /// <summary>
        /// Constructor to use if you are going to import themes
        /// </summary>
        public FormThemeImportExport()
        {
            // Importing themes
            InitializeComponent();
            Init();
            string pathToFile = AskForFileToImport($"WinStrip themes|*{ThemeFileExtendion}");
            if (pathToFile != null)
            {
                ThemeList = OpenThemeListFile(pathToFile);
                if (ThemeList.Count < 1)
                {
                    MessageBox.Show(this, "Unable to import any themes!", "No themes to imported", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    JustCloseTheForm = true;
                    return;
                }
                ThemesToForm(ThemeList);
                textBoxFileName.Text = pathToFile;
            }
            else {

                JustCloseTheForm = true;
            }
        }

        /// <summary>
        /// Constructor to use if you are going to export themes
        /// </summary>
        public FormThemeImportExport(List<Theme> themeList)
        {
            //Exporting themes
            InitializeComponent();

            Init(themeList);
            ThemesToForm(ThemeList);
        }
        private List<Theme> OpenThemeListFile(string pathToFile)
        {
            var content = File.ReadAllText(pathToFile);
            try { 
                var ser = new JavaScriptSerializer();
                var themeList = ser.Deserialize<List<Theme>>(content);
                return themeList;
            } catch (Exception)
            {
                return new List<Theme>();
            }

            
        }


        /// <summary>
        /// Asks the user to browse to a file and select it
        /// </summary>
        /// <param name="filter"></param>
        /// <returns>Success: a string with path to a file.  Fail: null</returns>
        private string AskForFileToImport(string filter = null)
        {
            var dlg = new OpenFileDialog();
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (filter != null)
                dlg.Filter = filter;
            dlg.Title = $"Please select a theme";
            dlg.CheckFileExists = true;
            dlg.Multiselect = false;

            var ret = dlg.ShowDialog();
            if (ret == DialogResult.OK)
                return dlg.FileName;
            return null;

        }

        private void Init(List<Theme> themeList = null)
        {
            JustCloseTheForm = false;
            ThemeList = new List<Theme>();
            Importing = themeList == null;
            if (!Importing) {
                
                foreach (var item in themeList)
                    ThemeList.Add(item);

                Text = "Exporting themes";
                btnOk.Text = "&Export";
                textBoxFileName.Text = $"themes{ThemeFileExtendion}";
            } else
            {
                Text = "Importing themes";
                btnOk.Text = "&Import";
            }

            textBoxFileName.ReadOnly = Importing;

            textBoxInfo.Text = GetInfoText();
        }

        private string GetInfoText()
        {
            if (Importing)
            {
                return
@"    Please select the themes you want to import and
    press Import when you are happy with your selection.";
            }

            return
@"  Please select the themes you want to export and press the 
   Export button when you are happy with your selection.";
        }

        private void ThemesToForm(List<Theme> themeList)
        {
            checkedListBox1.Items.Clear();
            foreach (var item in themeList)
            {
                checkedListBox1.Items.Add(item.Name, true);
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            bool success = false;
            if (!Importing)
                success = ExportFormThemesToFile();
            else
                success = IsThemeSelected();
            if (success)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private bool IsThemeSelected()
        {
            return checkedListBox1.CheckedItems.Count > 0;
        }

        private bool ExportFormThemesToFile()
        {
            List<Theme> exportList = SelectedThemes();
            if (exportList.Count < 1) {
                MessageBox.Show(this, "There are no themes to export!", "No themes to export", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description = "Please select the folder to place the themes";
            folderBrowserDialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {

                var toPath = $"{folderBrowserDialog.SelectedPath}\\{textBoxFileName.Text}";
                bool overwriteFile = false;
                if (File.Exists(toPath))
                {
                    if (MessageBox.Show($"The file \r\n\r\n\"{toPath}\"\r\n\r\n already exists.\r\n\r\n Do you want to overwrite it?",
                                 "File already exists",
                                 MessageBoxButtons.YesNo,
                                 MessageBoxIcon.Warning) == DialogResult.No)
                    {
                        return false;
                    }
                    overwriteFile = true;
                }

                
                try
                {
                    var str = (new JavaScriptSerializer()).Serialize(exportList);
                    if (overwriteFile)
                        File.Delete(toPath);
                    File.WriteAllText(toPath, str);
                    return true;
                    
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, $"There was an error converting export list to text\n\n {ex.Message}", "Error Serializing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            return false;
        }

        public List<Theme> SelectedThemes()
        {
            var list = new List<Theme>();
            int index;
            foreach (var item in checkedListBox1.CheckedItems)
            {
                var name = item.ToString();

                index = ThemeList.FindIndex(a => a.Name == name);
                if (index > -1)
                    list.Add(ThemeList[index]);
                else
                    throw new Exception("An unexpected error, name does not exist");
            }
            return list;
        }

        private void CloseCancel()
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            CloseCancel();
        }

        private void setAllCheckState(bool check)
        {
            CheckState state = check ? CheckState.Checked : CheckState.Unchecked;
            for (int i = 0; i < checkedListBox1.Items.Count; i++)
            {
                checkedListBox1.SetItemCheckState(i, state);
            }
        }

        private void radioButtonAll_Click(object sender, EventArgs e)
        {
            setAllCheckState(true);
        }

        private void radioButtonNone_Click(object sender, EventArgs e)
        {
            setAllCheckState(false);
        }

        private void textBoxFileName_TextChanged(object sender, EventArgs e)
        {
            SetButtonState();
        }

        private void SetButtonState()
        {
            if (!Importing) { 
                string str = textBoxFileName.Text;
                bool isValid = !string.IsNullOrWhiteSpace(str) && str.Length > 1 + ThemeFileExtendion.Length && str.EndsWith(ThemeFileExtendion) && str.IndexOf('\\') == -1;
                if (isValid)
                {
                    var invalidChars = Path.GetInvalidFileNameChars();
                    isValid = str.IndexOfAny(invalidChars) == -1;
                }
                btnOk.Enabled = isValid;
            } else
            {
                btnOk.Enabled = IsThemeSelected();
            }
        }

        private void FormThemeImportExport_Load(object sender, EventArgs e)
        {
            if (JustCloseTheForm)
                CloseCancel();
        }

        private void textBoxFileName_KeyUp(object sender, KeyEventArgs e)
        {
            SetButtonState();
        }
    }
}
