using System;
using System.Collections.Generic;
using System.Data;
using System.ComponentModel;
using System.Linq;
using System.Management;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using WinStrip.Entity;
using WinStrip.EntityTransfer;
using WinStrip.Utilities;

namespace WinStrip
{
    public partial class FormMain : Form
    {
        
        Serial serial;
        List<StripProgram> programs;
        List<ProgramParameter> parameters;
        public int PortSpeed { get; set; }
        List<Theme> Themes { get; set; }
        public int ThemeSelectedIndex { get; private set; }

        public FormMain()
        {
            ThemeSelectedIndex = -1;
            PortSpeed = 500000;
            InitializeComponent();

            serial = new Serial();
            parameters = new List<ProgramParameter>();

            textBoxCustomSend.ContextMenu = new ContextMenu();
            for (SerialCommand i = 0; i < SerialCommand.COUNT; i++)
            {

                MenuItem item = textBoxCustomSend.ContextMenu.MenuItems.Add(i.ToString());
                item.Click += new EventHandler(textBoxCustomSenditem_Click);
            }
        }

        void textBoxCustomSenditem_Click(object sender, EventArgs e)
        {
            MenuItem clickedItem = sender as MenuItem;
            textBoxCustomSend.Text = clickedItem.Text;
        }


        private void ShowAndSetParamNames()
        {
            groupBoxValue1.Visible = parameters.Count > 0;
            groupBoxValue2.Visible = parameters.Count > 1;
            groupBoxValue3.Visible = parameters.Count > 2;
            
            if (groupBoxValue1.Visible) groupBoxValue1.Text = parameters[0].Name;
            if (groupBoxValue2.Visible) groupBoxValue2.Text = parameters[1].Name;
            if (groupBoxValue3.Visible) groupBoxValue3.Text = parameters[2].Name;

        }

        private Theme CreateDefaultTheme()
        {
            var theme = new Theme
            {
                Name = "Default",
                Steps = new List<Step> {
                    new Step {      From = 0,
                                    Values = new StripValues { delay=0, com=2, brightness =   1, values = new List<int>  {0,0,0} },
                                    Colors = new StripColors {   colors = new List<ulong>  {      255, 16711680, 32768, 255, 16777215, 10824234 } }
                                },
                    new Step {      From = 10,
                                    Values = new StripValues { delay=0, com=2, brightness = 255, values = new List<int> {0,0,0} },
                                    Colors = new StripColors {   colors = new List<ulong> {      255, 16711680, 32768, 255, 16777215, 10824234 } }
                                },
                    new Step {      From = 30,
                                    Values = new StripValues { delay=0, com=2, brightness = 177, values = new List<int> {0,0,0} },
                                    Colors = new StripColors {   colors = new List<ulong> {    65535, 16711680, 32768, 255, 16777215, 10824234 } }
                                },
                    new Step {      From = 50,
                                    Values = new StripValues { delay=0, com=2, brightness = 240, values = new List<int> {0,0,0} },
                                    Colors = new StripColors {   colors = new List<ulong> {    65280, 16711680, 32768, 255, 16777215, 10824234 } }
                                },
                    new Step {      From = 60,
                                    Values = new StripValues { delay=0, com=2, brightness = 180, values = new List<int> {0,0,0} },
                                    Colors = new StripColors {   colors = new List<ulong> { 16711808, 16711680, 32768, 255, 16777215, 10824234 } }
                             },
                    new Step {      From = 70,
                                    Values = new StripValues { delay=0, com=2, brightness = 255, values = new List<int> {0,0,0} },
                                    Colors = new StripColors {   colors = new List<ulong> { 16711680, 16711680, 32768, 255, 16777215, 10824234 } }
                             },
                    new Step {      From = 90,
                                    Values = new StripValues { delay=2, com=7, brightness = 102, values = new List<int> {32,50,0} },
                                    Colors = new StripColors {   colors = new List<ulong> { 16711680,        0, 32768, 255, 16777215, 10824234 } }
                             },
                    new Step {      From = 95,
                                    Values = new StripValues { delay=2, com=7, brightness = 255, values = new List<int> {32,20,0} },
                                    Colors = new StripColors {   colors = new List<ulong> { 16711680,        0, 32768, 255, 16777215, 10824234 } }
                             },
                    new Step {      From = 99,
                                    Values = new StripValues { delay=1, com=7, brightness = 255, values = new List<int> {32,20,0} },
                                    Colors = new StripColors {   colors = new List<ulong> { 16711680,        0, 32768, 255, 16777215, 10824234 } }
                             }
                }
            };


            return theme;
        }
        
        private void SaveThemes(List<Theme> themes)
        {
            //Sort all themes
            themes.Sort(new Theme());
            //Sort all steps in all themes so that highest From value will be first.
            themes.ForEach(theme => theme.SortStepsAndFix());
            
            var str = (new JavaScriptSerializer()).Serialize(themes);
            Properties.Settings.Default.Themes = str;
            Properties.Settings.Default.Save();
        }

        
        void LoadThemes()
        {
            //Properties.Settings.Default.Themes = ""; Properties.Settings.Default.Save();
            var str = Properties.Settings.Default.Themes;

            if (string.IsNullOrEmpty(str))
            {
                SaveThemes(new List<Theme> { CreateDefaultTheme() });
                str = Properties.Settings.Default.Themes;
            }

            var ser = new JavaScriptSerializer();
            var themeList = ser.Deserialize<List<Theme>>(str);
            
            Themes = themeList;
            

            // now let's populate form
            ThemesToForm();
        }

        void ThemesToForm()
        {
            if (Themes == null)
                return;
            comboThemes.Items.Clear();
            Themes.ForEach(t => comboThemes.Items.Add(t.Name));
            comboThemes.SelectedIndex = 0;
        }



        private void FormMain_Load(object sender, EventArgs e)
        {
            LoadThemes();
            labelStatus.Text = "";
            InitCombo();
            GetHardwareFromDevice();

            radioButtonCpuTesting.Checked = true;
            EnableDeviceRelatedControls(serial.isConnected);
        }

        private void GetHardwareFromDevice()
        {
            if (serial.isConnected) { 
                serial.WriteLine(SerialCommand.HARDWARE.ToString());
                var strBuffer = serial.ReadLine();
                var serializer = new JavaScriptSerializer();
                var ret = serializer.Deserialize<StripHardware>(strBuffer);
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            var text = textBoxCustomSend.Text;

            serial.WriteLine(text);
            try { 
                textBox2.Text += "\r\n"+ serial.ReadLine();
            } catch (TimeoutException)
            {
                labelStatus.Text = "";
            }
            
            catch(Exception ex)
            {
                labelStatus.Text = ex.Message;
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
        }

        public void SetButtonStatus()
        {
            btnSend.Enabled = serial.isConnected;
        }
        public bool GetFullComputerDevices(string findPort)
        {
            var ret = false;
            ManagementClass processClass = new ManagementClass("Win32_PnPEntity");


            ManagementObjectCollection moCollection = processClass.GetInstances();

            foreach (var mo in moCollection)
            {
                /*Console.WriteLine("--------------------------");
                foreach (PropertyData prop in mo.Properties)
                {
                    Console.WriteLine($"{prop.Name}: {prop.Value}");
                }*/
                var property = (ManagementObject)mo;
                if (property.GetPropertyValue("Name") != null)
                {

                    var format = property.GetPropertyValue("Name").ToString();
                    if (format.Contains("COM") && (format.Contains("USB") || format.Contains("Standard Serial over Bluetooth link"))
                        )
                    {
                        var name = format;
                        Console.WriteLine(format);
                        ret = (name.IndexOf($"({findPort})", StringComparison.Ordinal) > -1) ||
                              (name.IndexOf($"Standard Serial over Bluetooth link ({findPort})", StringComparison.Ordinal) > -1);
                        if (ret)
                            break;
                    }
                }
            }
            return ret;
        }


        private bool ConnectToPort(int comboPortsItemIndex)
        {
            var nextPortName = comboPorts.Items[comboPortsItemIndex].ToString();
            labelStatus.Text = $"{nextPortName}";
            if (serial.OpenSerialPort(nextPortName, PortSpeed))
            {
                comboPorts.SelectedIndex = comboPortsItemIndex;
                labelStatus.Text = $"Connected to port {nextPortName}";
                SetPortConnectionStatus(true);
                GetAllFromDevice();
                return true;
            }
            return false;
        }


        public void InitCombo()
        {
            string[] ports = serial.getPortNames();

            comboPorts.Items.Clear();
            var list = new List<string>();
            foreach (string portName in ports)
            {
                labelStatus.Text = portName;
                if (GetFullComputerDevices(portName))
                {
                    if (!list.Contains(portName))
                    {
                        list.Add(portName);
                    }
                }
            }

            var sorted = list.OrderBy(m => m.Length).ThenBy(m => m).ToList();
            sorted.ForEach(m => comboPorts.Items.Add(m));

            //Selecting and opening port
            comboPorts.Enabled = comboPorts.Items.Count > 0;
            if (comboPorts.Enabled)
            {
                int index = 0;
                while (index < comboPorts.Items.Count)
                {
                    if (ConnectToPort(index))
                        return;
                    
                    index++;
                }
            }
            labelStatus.Text = "Unable to connect to any com port";
        }

        void EnableDeviceRelatedControls(bool bEnable)
        {
            groupBoxDelay.Enabled = bEnable;
            groupBoxBrightness.Enabled = bEnable;
            groupBoxParameters.Enabled = bEnable;
            comboPrograms.Enabled = bEnable;
            btnGetValues.Enabled = bEnable;
            btnSendAll.Enabled = bEnable;
            btnColor1.Enabled = bEnable;
            btnColor2.Enabled = bEnable;
            btnColor3.Enabled = bEnable;
            btnColor4.Enabled = bEnable;
            btnColor5.Enabled = bEnable;
            btnColor6.Enabled = bEnable;

            btnSend.Enabled = bEnable;
            if (comboPorts.Items.Count < 1)
                btnConnection.Text = "Connect";
            btnConnection.Enabled = comboPorts.Items.Count > 0;
        }

        private void SetPortConnectionStatus(bool connectionStatus)
        {
            if (connectionStatus)
            {
                btnConnection.Text = "Disconnect";
            } else
            {
                btnConnection.Text = "Connect";
            }

            btnConnection.Enabled = comboPrograms.Items.Count > 0;

            EnableDeviceRelatedControls(serial.isConnected);
        }

        private void btnGetValues_Click(object sender, EventArgs e)
        {
            GetValuesAndColorsFromDevice();
        }

        private void btnClearText2_Click(object sender, EventArgs e)
        {
            textBox2.Text = "";
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSend_Click(sender, null);
            }
        }

        private void comboPrograms_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            var name = comboBox.Text;
            labelDescription.Text = "";
            foreach (var program in programs)
            {
                if (name.Equals(program.name))
                {
                    labelDescription.Text = program.description.Replace("\n", "\r\n");
                    parameters.Clear();

                    foreach (var value in program.values)
                    {
                        parameters.Add(new ProgramParameter { Name = value, Value=0 });
                    }
                    ShowAndSetParamNames();

                    SendValuesToDevice();
                    if (name == "Reset")
                    {
                        GetValues();
                    }
                    return; 
                }
            }
        }

        private void buttonColor_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            colorDialog1.Color = button.BackColor;

            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                button.BackColor = colorDialog1.Color;
                SendColorsToDevice();
            }
        }

        
        private void GetValuesAndColorsFromDevice()
        {
            serial.WriteLine(SerialCommand.VALUES.ToString());
            var strBuffer = serial.ReadLine();
            var serializer = new JavaScriptSerializer();
            var ret = serializer.Deserialize<StripValues>(strBuffer);

            serial.WriteLine(SerialCommand.COLORS.ToString());
            strBuffer = serial.ReadLine();
            var colorObj = serializer.Deserialize<StripColors>(strBuffer);

            comboPrograms.SelectedIndex = ret.com;
            ValuesToControls(ret.brightness, ret.delay, ret.values, colorObj.colors);

        }

        private bool GetAllFromDevice()
        {
            try { 
                serial.WriteLine(SerialCommand.ALLSTATUS.ToString());
                var strBuffer = serial.ReadLine();
                var serializer = new JavaScriptSerializer();
                StripStatus ret = serializer.Deserialize<StripStatus>(strBuffer);
                programs = ret.programs;

                comboPrograms.Items.Clear();
                programs.ForEach(m => comboPrograms.Items.Add(m.name));
                comboPrograms.SelectedIndex = ret.com;
            
                ValuesToControls(ret.brightness, ret.delay, ret.values, ret.colors);
                return true;
            } catch(Exception ex)
            {
                return false;
            }
        }

        private void ValuesToControls(int brightness, int delay, List<int> values, List<ulong> colors)
        {
            SetControlValue(ValueControls.DELAY, delay);
            SetControlValue(ValueControls.BRIGHTNESS, brightness);

            int i;

            if (colors != null && colors.Count == 6 )
            { 
                btnColor1.BackColor = new SColor(colors[0]).Color;
                btnColor2.BackColor = new SColor(colors[1]).Color;
                btnColor3.BackColor = new SColor(colors[2]).Color;
                btnColor4.BackColor = new SColor(colors[3]).Color;
                btnColor5.BackColor = new SColor(colors[4]).Color;
                btnColor6.BackColor = new SColor(colors[5]).Color;
            }

            if (values.Count > 0) SetControlValue(ValueControls.VALUE1, values[0]);
            if (values.Count > 1) SetControlValue(ValueControls.VALUE2, values[1]);
            if (values.Count > 2) SetControlValue(ValueControls.VALUE3, values[2]);
        }
        private void SetControlValue(ValueControls control, int newValue)
        {
            switch (control)
            {
                case ValueControls.BRIGHTNESS:  
                                                if (trackBarBrightness.Value      != newValue)      trackBarBrightness.Value = newValue;
                                                if (numericUpDownBrightness.Value != newValue) numericUpDownBrightness.Value = newValue;
                                                break;
                case ValueControls.DELAY:
                                                if (trackBarDelay.Value           != newValue)           trackBarDelay.Value = newValue;
                                                if (numericUpDownDelay.Value      != newValue)      numericUpDownDelay.Value = newValue;
                                                break;
                case ValueControls.VALUE1:
                                                if (trackBarValue1.Value           != newValue)         trackBarValue1.Value = newValue;
                                                if (numericUpDownValue1.Value      != newValue)    numericUpDownValue1.Value = newValue;
                                                break;
                case ValueControls.VALUE2:
                                                if (trackBarValue2.Value           != newValue)         trackBarValue2.Value = newValue;
                                                if (numericUpDownValue2.Value      != newValue)    numericUpDownValue2.Value = newValue;
                                                break;                             
                case ValueControls.VALUE3:                                         
                                                if (trackBarValue3.Value           != newValue)         trackBarValue3.Value = newValue;
                                                if (numericUpDownValue3.Value      != newValue)    numericUpDownValue3.Value = newValue;
                                                break;
                case ValueControls.CPUTESTING:
                                                
                                                if (trackBarCpuTesting.Value       != newValue)      trackBarCpuTesting.Value = newValue;
                                                if (numericUpDownCpuTesting.Value  != newValue) numericUpDownCpuTesting.Value = newValue;
                                                UpdateLabelCpu(trackBarCpuTesting.Value);
                    break;
            }
        }

        private void UpdateLabelCpu(int value)
        {
            string val = value.ToString();
            if (labelCpu.Text != val)
            {
                labelCpu.Text = val;
            }
        }

        private void SendValuesToDevice(StripValues values = null)
        {
            if (values == null) { 
                values = new StripValues
                {
                    com = comboPrograms.SelectedIndex,
                    delay = trackBarDelay.Value,
                    //colors     = GetButtonColors(),
                    values = GetValues(),
                    brightness = trackBarBrightness.Value
                };
            }

            var serializer = new JavaScriptSerializer();
            var str = serializer.Serialize(values);
            serial.WriteLine(str);

        }

        private void SendColorsToDevice(StripColors colors = null)
        {
            if (colors == null) {
                colors = new StripColors
                {
                    colors     = GetButtonColors(),
                };
            }
            var serializer = new JavaScriptSerializer();
            var str = serializer.Serialize(colors);
            serial.WriteLine(str);

        }

        private void SendStepToDevice(Step step)
        {
            var str = step.ToJson();
            serial.WriteJson(str);
        }

        private void btnSendAll_Click(object sender, EventArgs e)
        {

            SendValuesToDevice();

        }

        private List<int> GetValues()
        {
            var list = new List<int>();
            if (groupBoxValue1.Visible) list.Add(trackBarValue1.Value);
            if (groupBoxValue2.Visible) list.Add(trackBarValue2.Value);
            if (groupBoxValue3.Visible) list.Add(trackBarValue3.Value);

            return list;
        }

        private List<ulong> GetButtonColors()
        {
            List<ulong> list = new List<ulong>();
            list.Add((new SColor(btnColor1.BackColor)).ToUlong());
            list.Add((new SColor(btnColor2.BackColor)).ToUlong());
            list.Add((new SColor(btnColor3.BackColor)).ToUlong());
            list.Add((new SColor(btnColor4.BackColor)).ToUlong());
            list.Add((new SColor(btnColor5.BackColor)).ToUlong());
            return list;
        }

        /// <summary>
        /// Converts string to int.
        /// </summary>
        /// <param name="text"></param>
        /// <returns>Success: the converted number.  Fail: 0</returns>
        private int SaveStringToInt(string text)
        {
            try
            {

                int value = 0;
                int.TryParse(text, out value);
                return value;
            }
            catch
            {
                return 0;
            }
        }

        ValueControls GetControlValueFromName(string Name)
        {
            switch(Name)
            {
                case "trackBarBrightness": 
                case "numericUpDownBrightness": return ValueControls.BRIGHTNESS;

                case "trackBarDelay":                       
                case "numericUpDownDelay":      return ValueControls.DELAY;

                case "trackBarValue1":
                case "numericUpDownValue1":     return ValueControls.VALUE1;

                case "trackBarValue2":
                case "numericUpDownValue2":     return ValueControls.VALUE2;

                case "trackBarValue3":
                case "numericUpDownValue3":     return ValueControls.VALUE3;

                case "trackBarCpuTesting":
                case "numericUpDownCpuTesting": return ValueControls.CPUTESTING;

            }
            return ValueControls.INVALID;
        }

        private void ValueControl_ValueChanged(object sender, EventArgs e)
        {
            var typeName = sender.GetType().Name;
            if (typeName == "TrackBar")
            {
                var control = (TrackBar)sender;
                SetControlValue(GetControlValueFromName(control.Name), control.Value);
                SendValuesToDevice();
            }
            else if (typeName == "NumericUpDown")
            {
                var control = (NumericUpDown)sender;
                SetControlValue(GetControlValueFromName(control.Name), (int)control.Value);
            }
        }

        private void ValueControlNoSend_ValueChanged(object sender, EventArgs e)
        {
            var typeName = sender.GetType().Name;
            if (typeName == "TrackBar")
            {
                var control = (TrackBar)sender;
                SetControlValue(GetControlValueFromName(control.Name), control.Value);
            }
            else if (typeName == "NumericUpDown")
            {
                var control = (NumericUpDown)sender;
                SetControlValue(GetControlValueFromName(control.Name), (int)control.Value);
            }
        }

        private void BrightnessSpinner_ValueChanged(object sender, EventArgs e)
        { 
            if (sender.GetType().Name == "TrackBar")
            {
                numericUpDownBrightness.Value = trackBarBrightness.Value;
            } else if (sender.GetType().Name == "NumericUpDown")
            {
                trackBarBrightness.Value  = (int)numericUpDownBrightness.Value;
            }
        }

        private void comboPorts_SelectedIndexChanged(object sender, EventArgs e)
        {
            int index = comboPorts.SelectedIndex;
            if (index > -1)
            {
                ConnectToPort(index);
            }
        }

        private void btnConnection_Click(object sender, EventArgs e)
        {
            if (serial.isConnected)
                serial.Close(); 
            else
                ConnectToPort(comboPorts.SelectedIndex);

            SetPortConnectionStatus(serial.isConnected);
        }


        private void radioButtonCpuLive_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            groupBoxCpuTest.Enabled = !radioButton.Checked;
        }

        void loadThemeToGrid(Theme theme) 
        {
            dataGridView1.Rows.Clear();

            
            dataGridView1.ColumnCount = 3;
            dataGridView1.Columns[0].Name = "From";
            dataGridView1.Columns[0].Width = 35;
            dataGridView1.Columns[1].Name = "Values";
            dataGridView1.Columns[1].Width = 240;
            dataGridView1.Columns[2].Name = "Colors";
            dataGridView1.Columns[2].Width = 240;

            theme.Steps.ForEach(s => dataGridView1.Rows.Add(new string[] { s.From.ToString(), s.ValuesToJson(), s.ColorsToJson() }));
        }

        private void comboThemes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboThemes.SelectedIndex == -1)
                return;
            var index = Themes.FindIndex(a => a.Name == comboThemes.Text);
            if (index == -1)
                return;
            ThemeSelectedIndex = index;
            loadThemeToGrid(Themes[ThemeSelectedIndex]);
        }
        private void labelCpu_TextChanged(object sender, EventArgs e)
        {
            if (ThemeSelectedIndex > -1 )
            {
                try {
                    int cpuValue = Convert.ToInt32(labelCpu.Text);
                    var theme = Themes[ThemeSelectedIndex];
                    Step step = theme.GetAppropriateStep(cpuValue);
                    /*SendValuesToDevice(step.Values);
                    SendColorsToDevice(step.Colors);*/
                    SendStepToDevice(step);
                } catch (Exception ex)
                {
                    //do nothing
                }
            }
            
        }



        private void btnLoadTheme_Click(object sender, EventArgs e)
        {
            loadThemeToGrid(Themes[ThemeSelectedIndex]);
        }

        private void btnSaveTheme_Click(object sender, EventArgs e)
        {
            var theme = new Theme(comboThemes.Text);
            bool error = false;

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                string strFrom;
                var row = dataGridView1.Rows[i];
                if (row.Cells[0].Value == null || row.Cells[1].Value == null || row.Cells[2].Value == null)
                {
                    error = true;
                    if (i == dataGridView1.Rows.Count - 1) 
                    { 
                        break; //last row is null so let's stop
                    }

                    strFrom = $" (See line number {i+1})";
                }
                else
                {
                    strFrom = row.Cells[0].Value.ToString();
                    error = !theme.AddStep(strFrom, row.Cells[1].Value.ToString(), row.Cells[2].Value.ToString());
                }
                if (error)
                {
                    MessageBox.Show(this, $"Cannot save\n\n There are invalid values in step {strFrom}", "Error adding step", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                
            }
            //we got valid steps
            Themes[ThemeSelectedIndex] = theme;
            SaveThemes(Themes);
        }

        private void btnResetThemes_Click(object sender, EventArgs e)
        {
            if ( MessageBox.Show("Are you sure you want to delete all themes and create a new default theme?", 
                                 "Reset theme", 
                                 MessageBoxButtons.YesNo, 
                                 MessageBoxIcon.Warning    ) == DialogResult.Yes )
            {

                Properties.Settings.Default.Themes = ""; Properties.Settings.Default.Save();
                LoadThemes();

            }
            
        }

        private void btnNewTheme_Click(object sender, EventArgs e)
        {
            var str = PromptDialog.ShowDialog("Please provide a new name for this theme", "Adding a new theme", 400);
            if (str.Length > 0)
            {
                var newTheme = new Theme(str);
                int i = Themes.FindIndex(t => t.Name == str);
                if (i == -1)
                {
                    Themes.Add(new Theme(str));
                    int index = comboThemes.Items.Add(str);
                    comboThemes.SelectedIndex = index;
                } else
                {
                    MessageBox.Show("There exists another theme with this name, please select another one!", "Name taken", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
                
                
            }
        }

        private void btnLoadAll_Click(object sender, EventArgs e)
        {
            LoadThemes();
        }
    }
}
