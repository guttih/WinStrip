﻿using System;
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
using System.IO;
using System.Net;
using Microsoft.Win32;

namespace WinStrip
{
    public partial class FormMain : BaseForm
    {

        Serial serial;
        List<StripProgram> programs;
        List<ProgramParameter> parameters;
        public int PortSpeed { get; set; }
        List<Theme> Themes { get; set; }
        public int ThemeSelectedIndex { get; private set; }
        public string LabelStatusSaveText { get; private set; }

        ToolTip toolTip1;
        private FormSplash splash;
        private bool visabilityAllowed;

        public FormMain()
        {
            LoadThemes(false);
            visabilityAllowed = OneThemeIsDefault();

            splash = new FormSplash("WinStrip", "initializing...");
           
            ThemeSelectedIndex = -1;
            PortSpeed = 500000;
            splashShow("initializing...");
            InitializeComponent();
            toolTip1 = new ToolTip();
            toolTip1.AutoPopDelay = 4000;
            serial = new Serial();
            parameters = new List<ProgramParameter>();

            textBoxManualSend.ContextMenu = new ContextMenu();
            for (SerialCommand i = 0; i < SerialCommand.COUNT; i++)
            {
                MenuItem item = textBoxManualSend.ContextMenu.MenuItems.Add(i.ToString());
                item.Click += new EventHandler(textBoxCustomSenditem_Click);
            }

            LoadInit();
           
        }

        private void LoadInit()
        {
            LoadThemes();
            SetTooltips();
            labelStatus.Text = "";
            splashShow("Searching for available com ports...");
            InitComboPorts();
            splashShow("Getting values from the micro controller...");
            GetHardwareFromDevice();

            EnableDeviceRelatedControls(serial.isConnected);
            radioButtonCpuLive.Checked = selectedComboThemeIsDefaultTheme();
            launchWinStripOnStartupToolStripMenuItem.Checked = CheckIfApplicationWillRunOnStartup();

            splashHide();
            timer1.Start();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
        }

        private void splashShow(string message, string title = null)
        {
            if (!visabilityAllowed)
                return;
            if (title == null)
                title = Text;

            splash.Set(title, message);
            if (!splash.Visible)
                splash.Show();
        }

        private void splashHide()
        {
            if (!visabilityAllowed)
                return;

            if (splash.Visible)
                splash.Hide();
        }

        protected override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(visabilityAllowed ? value : false);
        }



        private void SetTooltips()
        {
            toolTip1.SetToolTip(linkLabelPrograms, "Visit help page for the programs tab");
            toolTip1.SetToolTip(linkLabelCpu,      "Visit help page for the CPU tab");
            toolTip1.SetToolTip(linkLabelManual,   "Visit help page for the command tab");
            toolTip1.SetToolTip(textBoxManualSend, "Left click to select available commands");
            toolTip1.SetToolTip(btnGetValues,      "Download all values from the micro controller");
            toolTip1.SetToolTip(comboPrograms,     "Select strip program");
            toolTip1.SetToolTip(btnSendAll,        "Send all selected values to them micro contoller");
            toolTip1.SetToolTip(comboPorts,        "The COM port the application is connected to");
            toolTip1.SetToolTip(btnConnection, "Click to connect or disconnect the COM port");

            string format = "Slide left or right to increase or decreas the {0}.  Use spin box for more precision";
            toolTip1.SetToolTip(trackBarValue1,     string.Format(format, "value"));
            toolTip1.SetToolTip(trackBarValue2,     string.Format(format, "value"));
            toolTip1.SetToolTip(trackBarValue3,     string.Format(format, "value"));
            format = "Slide up or down to increase or decreas the {0}.  Use spin box for more precision";
            toolTip1.SetToolTip(trackBarDelay,      string.Format(format, "delay"));
            toolTip1.SetToolTip(trackBarBrightness, string.Format(format, "brightness"));

            format = "Select select a new color for colorbank {0} and send it to the micro controller";
            toolTip1.SetToolTip(btnColor1, string.Format(format, 1));
            toolTip1.SetToolTip(btnColor2, string.Format(format, 2));
            toolTip1.SetToolTip(btnColor3, string.Format(format, 3));
            toolTip1.SetToolTip(btnColor4, string.Format(format, 4));
            toolTip1.SetToolTip(btnColor5, string.Format(format, 5));
            toolTip1.SetToolTip(btnColor6, string.Format(format, 6));


            // tab CPU Monitoring
            format = "Slide left or right to increase or decreas the {0}.  Use spin box for more precision";            
            toolTip1.SetToolTip(trackBarCpuTesting, string.Format(format, "CPU load test value"));

            toolTip1.SetToolTip(comboThemes, "The active theme");
            toolTip1.SetToolTip(radioButtonCpuLive,   "Select to start monitoring the CPU and update the value automatically");
            toolTip1.SetToolTip(radioButtonCpuTesting,"Select to be able to change the CPU load value manually to test the active theme");
            toolTip1.SetToolTip(labelCpu, "The current CPU load wich will determine which theme step is active");

            // tab Manual
            toolTip1.SetToolTip(textBoxManualSend, "Type or select by right clicking a command to send to the micro controller");
            toolTip1.SetToolTip(btnManualSend, "Send the selected command to the Micro controller");
            toolTip1.SetToolTip(btnClearText2, "Clear the responce box");



        }

        void textBoxCustomSenditem_Click(object sender, EventArgs e)
        {
            MenuItem clickedItem = sender as MenuItem;
            textBoxManualSend.Text = clickedItem.Text;
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
                    new Step { From = 0,  ValuesAndColors = new StripValuesAndColors { delay=0, com=2, brightness =   1, values = new List<int> { 0, 0,0}, colors = new List<ulong> {      255, 16711680, 32768, 255, 16777215, 10824234 } } },
                    new Step { From = 10, ValuesAndColors = new StripValuesAndColors { delay=0, com=2, brightness = 255, values = new List<int> { 0, 0,0}, colors = new List<ulong> {      255, 16711680, 32768, 255, 16777215, 10824234 } } },
                    new Step { From = 30, ValuesAndColors = new StripValuesAndColors { delay=0, com=2, brightness = 177, values = new List<int> { 0, 0,0}, colors = new List<ulong> {    65535, 16711680, 32768, 255, 16777215, 10824234 } } },
                    new Step { From = 50, ValuesAndColors = new StripValuesAndColors { delay=0, com=2, brightness = 240, values = new List<int> { 0, 0,0}, colors = new List<ulong> {    65280, 16711680, 32768, 255, 16777215, 10824234 } } },
                    new Step { From = 60, ValuesAndColors = new StripValuesAndColors { delay=0, com=2, brightness = 180, values = new List<int> { 0, 0,0}, colors = new List<ulong> { 16711808, 16711680, 32768, 255, 16777215, 10824234 } } },
                    new Step { From = 70, ValuesAndColors = new StripValuesAndColors { delay=0, com=2, brightness = 255, values = new List<int> { 0, 0,0}, colors = new List<ulong> { 16711680, 16711680, 32768, 255, 16777215, 10824234 } } },
                    new Step { From = 90, ValuesAndColors = new StripValuesAndColors { delay=2, com=7, brightness = 102, values = new List<int> {32,50,0}, colors = new List<ulong> { 16711680,        0, 32768, 255, 16777215, 10824234 } } },
                    new Step { From = 95, ValuesAndColors = new StripValuesAndColors { delay=2, com=7, brightness = 255, values = new List<int> {32,20,0}, colors = new List<ulong> { 16711680,        0, 32768, 255, 16777215, 10824234 } } },
                    new Step { From = 99, ValuesAndColors = new StripValuesAndColors { delay=1, com=7, brightness = 255, values = new List<int> {32,20,0}, colors = new List<ulong> { 16711680,        0, 32768, 255, 16777215, 10824234 } } }
                }
            };


            return theme;
        }

        private Theme CreateStepUpTheme()
        {
            var theme = new Theme
            {
                Name = "Default StepUp",
                Steps = new List<Step>()
            };

            var step = new Step(0, "{\"delay\":500,\"com\":4,\"brightness\":10,\"values\":[1,0,0],\"colors\":[255,0,32768,255,16777215,10824234]}");
            //step 100 skal ver 1 í delay
            
            var stepDown = 6;
            step.ValuesAndColors.delay += stepDown;
            for (int i = 0; i < 81; i++)
            {
                step.From = i;  step.ValuesAndColors.delay -= stepDown;
                theme.Steps.Add(new Step(step));
            }
             stepDown = 1;
            step.ValuesAndColors.delay += stepDown;
            for (int i = 80; i < 101; i++)
            {
                step.From = i; step.ValuesAndColors.delay -= stepDown;
                theme.Steps.Add(new Step(step));
            }

            return theme;
        }

        private void SaveThemes(List<Theme> themes, int selectedThemeIndex)
        {
            //Sort all themes
            themes.Sort(new Theme());
            //Sort all steps in all themes so that highest From value will be first.
            themes.ForEach(theme => theme.SortStepsAndFix());
            
            var str = (new JavaScriptSerializer()).Serialize(themes);
            Properties.Settings.Default.Themes = str;
            //Properties.Settings.Default.ThemeSelectedIndex = selectedThemeIndex;
            Properties.Settings.Default.Save();
        }

        
        void LoadThemes(bool loadThemeToFrom = true)
        {
            var str = Properties.Settings.Default.Themes;

            if (string.IsNullOrEmpty(str))
            {

                SaveThemes(new List<Theme> { CreateDefaultTheme(), CreateStepUpTheme()}, 0);
                str = Properties.Settings.Default.Themes;
            }

            var ser = new JavaScriptSerializer();
            var themeList = ser.Deserialize<List<Theme>>(str);
            ThemeSelectedIndex = 0;

            Themes = themeList;
            

            // now let's populate form
            if( loadThemeToFrom )
                ThemesToForm();
        }

        bool OneThemeIsDefault()
        {
            var index = Themes.FindIndex(t => t.Default == true);
            return  !(index > -1);
        }
        void ThemesToForm()
        {
            if (Themes == null)
                return;
            comboThemes.Items.Clear();
            if (Themes.Count > 0) 
            { 
                Themes.ForEach(t => comboThemes.Items.Add(t.Name));
                int defaultIndex = Themes.FindIndex(t => t.Default == true);
                if (defaultIndex > -1)
                {
                    comboThemes.SelectedIndex = comboThemes.FindStringExact(Themes[defaultIndex].Name);
                }
                else if (ThemeSelectedIndex > -1 && ThemeSelectedIndex < Themes.Count)
                {
                    comboThemes.SelectedIndex = comboThemes.FindStringExact(Themes[ThemeSelectedIndex].Name);
                }
            }
            SetThemeButtonsState();
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
            var text = textBoxManualSend.Text;

            splashShow($"Sending command {text}...");
            serial.WriteLine(text);
            try {
                splashShow("Waiting for a responce...");
                textBoxManualResponce.Text += "\r\n"+ serial.ReadLine();
            } catch (TimeoutException)
            {
                labelStatus.Text = "";
            } catch(Exception ex)
            {
                labelStatus.Text = ex.Message;
            }
            finally
            {
                splashHide();
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
        }

        public void SetManualControlState()
        {
            bool connected = serial.isConnected;
            textBoxManualSend.Enabled = connected;
            btnManualSend.Enabled = connected && textBoxManualSend.Text.Length > 0;
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


        private bool ConnectToPort(int comboPortsItemIndex, bool hideSplashWhenFinished = true)
        {
            var nextPortName = comboPorts.Items[comboPortsItemIndex].ToString();
            labelStatus.Text = $"{nextPortName}";
            if (serial.OpenSerialPort(nextPortName, PortSpeed))
            {
                comboPorts.SelectedIndex = comboPortsItemIndex;
                SetPortConnectionStatus(true);
                GetAllFromDevice(hideSplashWhenFinished);
                return true;
            }
            return false;
        }


        public void InitComboPorts()
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
                    if (ConnectToPort(index, false))
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

            btnManualSend.Enabled = bEnable;
            if (comboPorts.Items.Count < 1)
                btnConnection.Text = "Connect";
            btnConnection.Enabled = comboPorts.Items.Count > 0;

            labelStatus.Text = serial.isConnected ? $"Connected to {comboPorts.Text}" : "";

            SetManualControlState();
        }

        private void SetPortConnectionStatus(bool connectionStatus)
        {
            btnConnection.Text= connectionStatus? "Disconnect" : "Connect";
            btnConnection.Enabled = comboPrograms.Items.Count > 0;

            EnableDeviceRelatedControls(serial.isConnected);
        }

        private void btnGetValues_Click(object sender, EventArgs e)
        {
            GetValuesAndColorsFromDevice();
        }

        private void btnClearText2_Click(object sender, EventArgs e)
        {
            textBoxManualResponce.Text = "";
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
                    if (serial.isConnected) {
                        splashShow("Sending values to micro controller...");
                        SendValuesToDevice();
                        splashHide();
                    }
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
            var command = SerialCommand.VALUES.ToString();
            splashShow($"Sending command {command}...");
            serial.WriteLine(command);
            splashShow($"Waiting for responce from command {command}...");
            var strBuffer = serial.ReadLine();
            var serializer = new JavaScriptSerializer();
            var ret = serializer.Deserialize<StripValues>(strBuffer);

            command = SerialCommand.COLORS.ToString();
            splashShow($"Sending command {command}...");
            serial.WriteLine(command);
            splashShow($"Waiting for responce from command {command}...");
            strBuffer = serial.ReadLine();
            var colorObj = serializer.Deserialize<StripColors>(strBuffer);

            comboPrograms.SelectedIndex = ret.com;
            ValuesToControls(ret.brightness, ret.delay, ret.values, colorObj.colors);
            splashHide();
        }

        private bool GetAllFromDevice(bool hideSplashWhenFinished)
        {
            bool returnValue = true;
            try {
                var command = SerialCommand.ALLSTATUS.ToString();
                splashShow($"Sending command {command}...");
                serial.WriteLine(command);
                var strBuffer = serial.ReadLine();
                var serializer = new JavaScriptSerializer();
                StripStatus ret = serializer.Deserialize<StripStatus>(strBuffer);
                programs = ret.programs;

                comboPrograms.Items.Clear();
                programs.ForEach(m => comboPrograms.Items.Add(m.name));
                comboPrograms.SelectedIndex = ret.com;
            
                ValuesToControls(ret.brightness, ret.delay, ret.values, ret.colors);
                
            } catch(Exception)
            {
                returnValue = false;
            } finally
            {
                if (hideSplashWhenFinished)
                    splashHide();
            }

            return returnValue;


        }

        private void ValuesToControls(int brightness, int delay, List<int> values, List<ulong> colors)
        {
            SetControlValue(ValueControls.DELAY, delay);
            SetControlValue(ValueControls.BRIGHTNESS, brightness);

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
            splashShow("Sending colors to micro controller...");
            if (colors == null) {
                colors = new StripColors
                {
                    colors     = GetButtonColors(),
                };
            }
            var serializer = new JavaScriptSerializer();
            var str = serializer.Serialize(colors);
            serial.WriteLine(str);
            splashHide();
        }

        private void SendStepToDevice(Step step)
        {
            var str = step.ValuesAndColorsToJson();
            serial.WriteJson(str);
        }

        private void btnSendAll_Click(object sender, EventArgs e)
        {
            splashShow("Sending values to micro controller...");
            SendValuesToDevice();
            splashHide();

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
                if (serial.isConnected) {
                    SendValuesToDevice();
                }
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
                splashShow("Connecting...");
                ConnectToPort(index);
            }
        }

        private void btnConnection_Click(object sender, EventArgs e)
        {
            splashShow("Disconnecting...");
            if (serial.isConnected)
                serial.Close(); 
            else
                ConnectToPort(comboPorts.SelectedIndex);

            SetPortConnectionStatus(serial.isConnected);
            splashHide();
        }


        private void radioButtonCpuLive_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton radioButton = (RadioButton)sender;
            groupBoxCpuTest.Enabled = !radioButton.Checked;
        }

        void ThemeToGrid(Theme theme) 
        {
            StepsToGrid(theme.Steps);
        }

        void StepsToGrid(List<Step> steps)
        {
            dataGridView1.Rows.Clear();


            dataGridView1.ColumnCount = 2;
            dataGridView1.Columns[0].Name = "From";
            dataGridView1.Columns[0].Width = 35;
            dataGridView1.Columns[1].Name = "Values and colors";
            dataGridView1.Columns[1].Width = 600;

            steps.ForEach(s => dataGridView1.Rows.Add(new string[] { s.From.ToString(), s.ValuesAndColorsToJson() }));
            SetThemeButtonsState();
        }

        private void comboThemes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboThemes.SelectedIndex == -1) {
                ThemeSelectedIndex = -1;
                dataGridView1.Rows.Clear();
                SetThemeButtonsState();
                return;
            }
            var index = Themes.FindIndex(a => a.Name == comboThemes.Text);
            if (index == -1) 
            {
                SetThemeButtonsState();
                return;
            }
            ThemeSelectedIndex = index;
            ThemeToGrid(Themes[ThemeSelectedIndex]);


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
                    if (step != null)
                        SendStepToDevice(step);
                } catch (Exception)
                {
                    //do nothing
                }
            }
            
        }



        private void btnReloadTheme_Click(object sender, EventArgs e)
        {
            if (ThemeSelectedIndex < 0)
                return;
            ThemeToGrid(Themes[ThemeSelectedIndex]);
            var text = Themes[ThemeSelectedIndex].Name;
            
            ThemeToGrid(Themes[ThemeSelectedIndex]);
        }

        private void SaveAllThemes()
        {
            int index = Themes.FindIndex(t => t.Name == comboThemes.Text);

            Theme theme;
            if (index > -1)
                theme = new Theme(Themes[index].Name, Themes[index].Default);
            else
                theme = new Theme(comboThemes.Text);
            bool error = false;

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                string strFrom;
                var row = dataGridView1.Rows[i];
                if (row.Cells[0].Value == null || row.Cells[1].Value == null)
                {
                    error = true;
                    if (i == dataGridView1.Rows.Count - 1)
                    {
                        break; //last row is null so let's stop
                    }

                    strFrom = $" (See line number {i + 1})";
                }
                else
                {
                    strFrom = row.Cells[0].Value.ToString();
                    error = !theme.AddStep(strFrom, row.Cells[1].Value.ToString());
                }
                if (error)
                {
                    MessageBox.Show(this, $"Cannot save\n\n There are invalid values in step {strFrom}", "Error adding step", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

            }
            //we got valid steps
            if (ThemeSelectedIndex > -1 && Themes.Count > 0)
                Themes[ThemeSelectedIndex] = theme;
            SaveThemes(Themes, ThemeSelectedIndex);
        }

        private void btnSaveAllThemes_Click(object sender, EventArgs e)
        {
            SaveAllThemes();
        }

        private void btnResetAllThemes_Click(object sender, EventArgs e)
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
            var str = PromptDialog.ShowDialog("Please provide a new name for this theme", "Adding a new theme", "", 400);
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

            SetThemeButtonsState();

        }

        private void btnRenameTheme_Click(object sender, EventArgs e)
        {
            var oldName = comboThemes.Text;
            var str = PromptDialog.ShowDialog("Please provide a new name for this theme", "Adding a new theme", oldName, 400);
            if (str.Length > 0)
            {
                int i = Themes.FindIndex(t => t.Name == oldName);
                if (i != -1)
                {
                    Themes[i].Name = str;
                    comboThemes.Items.RemoveAt(comboThemes.SelectedIndex);
                    int index = comboThemes.Items.Add(str);
                    comboThemes.SelectedIndex = index;
                }
                else
                {
                    MessageBox.Show("There exists another theme with this name, please select another one!", "Name taken", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }
            }

            SetThemeButtonsState();
        }

        private bool SetDefaultTheme(bool enable)
        {
            var oldName = comboThemes.Text;
            int i = Themes.FindIndex(t => t.Name == oldName);
            if (i != -1)
            {
                string str = enable ? "to be able set this theme as default. This will cause the WinStrip to launch hidden but you will be able to access it by right-clicking on the tray icon which is near the clock on your task bar" 
                                    : "to be able to remove this theme from default.  This will disable the CPU monitoring and cause WinStrip to launch visable";
                if (MessageBox.Show($"You will need to save all themes {str}.\n\nDo you want to save all themes?",
                            "Set default theme",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Warning) == DialogResult.Yes) 
                { 
                    //remove older default themes
                    Themes.ForEach(theme => theme.Default = false);
                    Themes[i].Default = enable;
                    SaveAllThemes();
                    return true;
                }
            }
            else
            {
                MessageBox.Show("An fatal error, please restart the application.", "Could not find selected theme", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }

        private void btnSetDefaultTheme_Click(object sender, EventArgs e)
        {
           
        }

        private void btnLoadAllThemes_Click(object sender, EventArgs e)
        {
            LoadThemes();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (radioButtonCpuLive.Checked) { 
                float fCpu = performanceCounter1.NextValue();
                labelCpu.Text = Convert.ToInt32(fCpu).ToString();
            }
        }

        private void radioButtonCpuTesting_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButtonCpuTesting.Checked)
            {
                numericUpDownCpuTesting.Value = trackBarCpuTesting.Value;
            }

            SetDataGridButtonsState();
        }

        private Step ExtractSelectedStepFromStrip(bool ignoreFrom = false)
        {
            int iFrom = 0;
            if (!ignoreFrom) 
            {
                try {
                    iFrom = Convert.ToInt32(dataGridView1.SelectedRows[0].Cells[0].Value.ToString());
                } catch (Exception)
                {
                    MessageBox.Show("From is not an valid number", "Invalid number", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
            }

            Step step;
            try {
                step = new Step(iFrom, dataGridView1.SelectedRows[0].Cells[1].Value.ToString());
            } catch (Exception) {
                MessageBox.Show("Values and colors contains invalid command", "Invalid number", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }

            return step;
        }

        private List<Step> ExtractSelectedStepFromStrips()
        {
            var list = new List<Step>();
            for (int i = 0; i < dataGridView1.SelectedRows.Count; i++)
            {
                int iFrom;
                try
                {
                    iFrom = Convert.ToInt32(dataGridView1.SelectedRows[i].Cells[0].Value.ToString());
                }
                catch (Exception)
                {
                    MessageBox.Show("From is not an valid number", "Invalid number", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }

                Step step;
                try
                {
                    step = new Step(iFrom, dataGridView1.SelectedRows[i].Cells[1].Value.ToString());
                    list.Add(step);
                }
                catch (Exception)
                {
                    MessageBox.Show("Values and colors contains invalid command", "Invalid number", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
            }
            return list;
        }

        private void btnDeleteTheme_Click(object sender, EventArgs e)
        {
            var comboIndex = comboThemes.SelectedIndex;
            if (comboIndex < 0)
            {
                ThemeSelectedIndex = -1;
                SetThemeButtonsState();
                return;
            }
            var name = comboThemes.Items[comboIndex].ToString();


            int i = Themes.FindIndex(t => t.Name == name);
            if (i == -1)
            {
                dataGridView1.Rows.Clear();
                MessageBox.Show("Theme not found !", "Could not delete this theme", MessageBoxButtons.OK, MessageBoxIcon.Error);
                SetThemeButtonsState();
                return;
            }

            Themes.RemoveAt(i);
            comboThemes.Items.RemoveAt(comboIndex);
            int count = comboThemes.Items.Count;
            if (count < 1)
            {
                ThemeSelectedIndex = -1;
                dataGridView1.Rows.Clear();
                SetThemeButtonsState();
                return;
            }
            
            if (comboIndex == 0)
                comboThemes.SelectedIndex = 0;
            else if (comboIndex > 0)
            {
                comboThemes.SelectedIndex = comboIndex - 1;
            }

            SetThemeButtonsState();
        }

        public void SetThemeButtonsState()
        {
            bool atLeastOneTheme      = comboThemes.SelectedIndex > -1;

            //ComboBox Themes
            comboThemes.Enabled = comboThemes.Items.Count > 0;
            btnRenameTheme.Enabled = atLeastOneTheme;
            btnReloadTheme.Enabled = atLeastOneTheme;
            btnDeleteTheme.Enabled = atLeastOneTheme;
            SetDataGridButtonsState();
            SetDefaultThemeState();
        }

        private bool selectedComboThemeIsDefaultTheme()
        {
            int defaultIndex = Themes.FindIndex(t => t.Default == true);
            if (defaultIndex < 0)
                return false;

            return comboThemes.Text == Themes[defaultIndex].Name;
        }

        private void SetCheckDefaultTootipMessage()
        {
            string str = checkDefault.Checked ? "To disable Live CPU monitoring on startup, remove this check-mark."
                                              : "Check this mark to enable Live CPU monitoring when WinStrip is launched and use this theme to command the strip";
            toolTip1.SetToolTip(checkDefault, str);
        }
        private void SetDefaultThemeState()
        {   bool isDefault = selectedComboThemeIsDefaultTheme();
            
            checkDefault.Checked = isDefault;
            SetCheckDefaultTootipMessage();
        }

        private bool IsDatagridRowValid(DataGridViewRow row)
        {
            if (row.Cells[0].Value == null || row.Cells[1].Value == null)
            {
                return false;
            }

            return true;
        }
        public void SetDataGridButtonsState()
        {
            bool atLeastOneTheme = comboThemes.SelectedIndex > -1;
            bool atLeastOneLineInGrid = dataGridView1.Rows.Count > 1;
            bool testMode = radioButtonCpuTesting.Checked;
            btnAddRow.Enabled = testMode && programs?.Count > 0;
            btnWizard.Enabled = testMode && atLeastOneTheme;
            btnAddRow.Enabled = testMode && atLeastOneTheme;
            int selCount = dataGridView1.SelectedRows.Count;
            bool IsRowReadyForEditing = testMode &&
                                        atLeastOneTheme &&
                                        atLeastOneLineInGrid &&
                                        dataGridView1.SelectedRows.Count == 1 &&
                                        IsDatagridRowValid(dataGridView1.SelectedRows[0]);
            
            btnChangeSteps.Enabled = IsRowReadyForEditing;

            DimToBrightBlueToolStripMenuItem.Enabled = IsRowReadyForEditing;
            DimToBrightGreenToolStripMenuItem.Enabled = IsRowReadyForEditing;
            GenerateStepsToolStripMenuItem.Enabled =    dataGridView1.SelectedRows.Count == 2 && 
                                                        IsDatagridRowValid(dataGridView1.SelectedRows[1]) &&
                                                        IsDatagridRowValid(dataGridView1.SelectedRows[0]);

        }

        private void btnChangeSteps_Click(object sender, EventArgs e)
        {

            var step = ExtractSelectedStepFromStrip();
            if (step == null)
                return;
            FormStep form = new FormStep(step, programs, colorDialog1, serial);
            if (form.ShowDialog() == DialogResult.OK)
            {
                dataGridView1.SelectedRows[0].Cells[0].Value = form.Step.From;
                dataGridView1.SelectedRows[0].Cells[1].Value = form.Step.ValuesAndColorsToJson();
            }

        }

        private void dataGridView1_RowsChanged(object sender, DataGridViewRowsAddedEventArgs e)
        {
            SetDataGridButtonsState();
        }

        private void dataGridView1_RowsChanged(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            SetDataGridButtonsState();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            SetDataGridButtonsState();
            
        }

        private void btnAddRow_Click(object sender, EventArgs e)
        {
            FormStep form = new FormStep(new Step(), programs, colorDialog1, serial);
            if (form.ShowDialog() == DialogResult.OK)
            {
                dataGridView1.Rows.Add(new string[] { form.Step.From.ToString(), form.Step.ValuesAndColorsToJson() });
            }
        }

        private void LightDimToBrightStepsToGrid(ulong ulColor)
        {
            var step = ExtractSelectedStepFromStrip(false);

            if (step == null)
                return;

            StepsToGrid(StepGenerator.StripDimToBright(step, 0, ulColor, 0, 31));
        }
        private void DimToBrightBlueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LightDimToBrightStepsToGrid(0x0000ff);
        }

        private void dimToBrightGreenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LightDimToBrightStepsToGrid(0x00ff00);
        }

        private void dimToBrightRedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LightDimToBrightStepsToGrid(0xff0000);
        }

        private void GenerateStepsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var steps = ExtractSelectedStepFromStrips();
            if (steps == null)
                return;

            if (steps.Count < 2)
            {
                MessageBox.Show("Two valid rows must be selected", "Invalid number", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            var list = StepGenerator.StripSteps(steps[0], steps[1]);
            list.Reverse();
            StepsToGrid(list);
        }

        private void checkDefault_Click(object sender, EventArgs e)
        {
            bool newState = checkDefault.Checked;

            if (!SetDefaultTheme(newState))
            {
                checkDefault.Checked = !newState;
            }
        }

        private void ExportCode()
        {
            var fromPath = $"{Path.GetDirectoryName(Application.ExecutablePath)}\\Esp32Strip";
            var inoFiles = GetFilesByExtensions(new DirectoryInfo(fromPath), new string[] { ".ino" });
            if (inoFiles == null || inoFiles.Count() < 1)
            {
                
                MessageBox.Show(this, $"Could not find any .ino files in source code", "Ino file missing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            
            string inoFilePath = $"{fromPath}\\{inoFiles[0].Name}";
            var frmExport = new FormExportCode(inoFilePath);
            if (frmExport.ShowDialog() == DialogResult.OK) { 
            
                FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
                folderBrowserDialog.Description = "Please select the folder to place the code";
                folderBrowserDialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (Directory.Exists($"{folderBrowserDialog.SelectedPath}\\Arduino"))
                {
                    folderBrowserDialog.SelectedPath += "\\Arduino";
                }
                if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                {
                    var toPath = $"{folderBrowserDialog.SelectedPath}\\Esp32Strip";
               
                    try { 
                        CopyFiles(fromPath, toPath, "*.h;*.cpp");
                        inoFilePath = $"{toPath}\\{inoFiles[0].Name}";
                        frmExport.WriteSelectedContentToFile(inoFilePath);
                    } 
                    catch(Exception ex) {
                        MessageBox.Show(this, $"There was an error when copying files\n\n {ex.Message}", "Error copying source files", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }
            }
        }

        private void CopyFiles(string fromPath, string toPath, string searchPatthern)
        {
            var source = new DirectoryInfo(fromPath);

            DirectoryInfo dest = Directory.Exists(toPath)? new DirectoryInfo(toPath)
                                                         : Directory.CreateDirectory(toPath);

            searchPatthern = searchPatthern.Replace("*.", ".");
            var files = GetFilesByExtensions(source, searchPatthern.Split(';'));
            foreach(var file in files)
            {
                file.CopyTo(dest.FullName + "\\" + file.Name, true);
            }
        }

        public FileInfo[] GetFilesByExtensions(DirectoryInfo dir, string[] extensions)
        {
            if (extensions == null)
                throw new ArgumentNullException("extensions");
            IEnumerable<FileInfo> files = dir.EnumerateFiles();
            return files.Where(f => extensions.Contains(f.Extension)).ToArray();
        }

        private void linkLabelPrograms_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            VisitHelpUrl("maintabprograms.html");
        }

        private void linkLabelCpu_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            VisitHelpUrl("maintabcpu.html");
        }

        private void linkLabelManual_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            VisitHelpUrl("maintabmanual.html");
        }

        private void helpToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void exportCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ExportCode();
        }

        private void checkForUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            var url = $"{RootUrl}/release.json";
            //url = "https://guttih.com/public/projects/winstrip/release/1.0/release.json";
            string strRelease;
            var webClient = new WebClient();
            

            try { 
                strRelease = webClient.DownloadString(url);
            }
            catch (Exception)
            {
                MessageBox.Show(this, $"Unable to access version information file from server", "Error getting current version", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var serializer = new JavaScriptSerializer();
            VersionInformation versionInfo;

            try
            {
                versionInfo = serializer.Deserialize<VersionInformation>(strRelease);
            }
            catch (Exception)
            {
                MessageBox.Show(this, $"Unable to Deserialize version information", "Error deserializeing", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (versionInfo.Version == null || versionInfo.Version.Length < 3 || !versionInfo.Version.Contains('.'))
            {
                MessageBox.Show(this, $"Invalid version string information from server.", "Server version string is invalid", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if(!versionInfo.IsVersionLarger(ApplicationVersionString))
            {
                MessageBox.Show(this, $"Your version \"{ApplicationVersionString}\" is up to date", "Version up to date", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }


            var frm = new FormNewRelease(versionInfo);
            if (frm.ShowDialog() == DialogResult.Yes)
            {
                url = $"{RootUrl}/release.html?version={versionInfo.Version}";
                System.Diagnostics.Process.Start(url);
            }


        }

        private void saveAllThemesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveAllThemes();
        }

        private void reloadSavedThemesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadThemes();
        }

        private void resetAllThemesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to delete all themes and create a new default theme?",
                                 "Reset theme",
                                 MessageBoxButtons.YesNo,
                                 MessageBoxIcon.Warning) == DialogResult.Yes)
            {

                Properties.Settings.Default.Themes = ""; Properties.Settings.Default.Save();
                LoadThemes();

            }
        }

        private void saveAllThemesToolStripMenuItem_MouseHover(object sender, EventArgs e)
        {
        }

        private void saveAllThemesToolStripMenuItem_MouseLeave(object sender, EventArgs e)
        {
        }



        private void onControl_MouseLeave(object sender, EventArgs e)
        {
            labelStatus.Text = LabelStatusSaveText;
        }

        private void SetTooltipOnLabelStatus(object sender)
        {
            LabelStatusSaveText = labelStatus.Text;
            var typeName = sender.GetType().Name;

            if ( typeName == "ToolStripMenuItem")
                labelStatus.Text = ((ToolStripMenuItem)sender).ToolTipText;
            else 
                labelStatus.Text = toolTip1.GetToolTip((Control)sender);
        }

        private void onControl_MouseEnter(object sender, EventArgs e)
        {
            SetTooltipOnLabelStatus(sender);
        }

        private void OnPossibleChangeStateOfbtnManualSend(object sender, EventArgs e)
        {
            SetManualControlState();
        }

        private void checkDefault_CheckedChanged(object sender, EventArgs e)
        {
            SetCheckDefaultTootipMessage();
        }

        private void trayIconContextMenuStrip_Opening(object sender, CancelEventArgs e)
        {
            trayMenuItemShow.Visible = !Visible;
            trayMenuItemHide.Visible = Visible;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void trayMenuItemHide_Click(object sender, EventArgs e)
        {
            OnShowTrayItemClicked(false);
        }

        private void OnShowTrayItemClicked(bool show)
        {
            visabilityAllowed = true;
            base.SetVisibleCore(true);
            this.Visible = show;
            trayMenuItemShow.Visible = !show;
            trayMenuItemHide.Visible = show;
        }

        private void trayMenuItemShow_Click(object sender, EventArgs e)
        {
            OnShowTrayItemClicked(true);
        }

        /// <summary>
        /// Makes windows launch this application on startup or
        /// stops windows launching this app on startup.
        /// </summary>
        /// <param name="runWhenWindowsLaunches">
        ///     If true, this application will launch on windows startup.
        ///     If false, this application will NOT launch on windows startup.
        /// </param>
        private void AddOrRemoveApplicationOnWindowsLaunch(bool runWhenWindowsLaunches)
        {
            RegistryKey rk = Registry.CurrentUser.OpenSubKey
                ("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            var appName = ApplicationName;
            if (runWhenWindowsLaunches)
            { 
                rk.SetValue(appName, Application.ExecutablePath);
            }
            else { 
                rk.DeleteValue(appName, false);
            }

            launchWinStripOnStartupToolStripMenuItem.Checked = CheckIfApplicationWillRunOnStartup();
        }

        private bool CheckIfApplicationWillRunOnStartup()
        {
            RegistryKey WinRunKey = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", false);
            var names = WinRunKey.GetValueNames();
            return names.Contains(ApplicationName);
        }

        private void launchWinStripOnStartupToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddOrRemoveApplicationOnWindowsLaunch(!CheckIfApplicationWillRunOnStartup());
        }

        private void helpToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            VisitHelpUrl();
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var about = new FormAbout();
            about.ShowDialog();
        }

        private void featureRequestToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VisitUrl($"{RepositoryRootUrl}/issues/new?assignees=&labels=&template=feature_request.md");
        }

        private void bugReportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VisitUrl($"{RepositoryRootUrl}/issues/new?assignees=&labels=&template=bug_report.md");

        }

        private void exitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void exportThemesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var frm = new FormThemeImportExport(Themes);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show(this, "The selected themes have been successfully exported!", "Themes exported", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("You must save all themes before starting the import process.\r\nDo you want to save all themes now?",
                                 "Save all themes",
                                 MessageBoxButtons.YesNo,
                                 MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                return;
            }

            SaveAllThemes();
            LoadThemes();
            
            var frm = new FormThemeImportExport();
            if (frm.ShowDialog() == DialogResult.OK)
            {
                var list = frm.SelectedThemes();
                ImportThemes(list);
                ThemesToForm();
                MessageBox.Show(this, "Successfully imported themes", "Importing themes", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


       
        private void ImportThemes(List<Theme> importingThemes)
        {
            if (OneThemeIsDefault())
            {
                //then none of the imported can be default too
                importingThemes.ForEach(e => e.Default = false);
            }
            
            //no themes can have the same name, so let's rename the imported ones if needed
            importingThemes.ForEach(e => AddThemeSafely(Themes, e));
            
        }

        private void AddThemeSafely(List<Theme> list, Theme addMe)
        {
            int index, nameExtender;
            string testName;
            index = list.FindIndex(current => current.Name == addMe.Name);
            testName = addMe.Name;
            nameExtender = 1;
            while (index > -1)
            {
                nameExtender++;
                testName = addMe.Name + nameExtender;
                index = list.FindIndex(current => current.Name == testName);
            }

            addMe.Name = testName;
            list.Add(addMe);
        }
    }
}