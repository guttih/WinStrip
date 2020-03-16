using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Text;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using WinStrip.Entity;
using WinStrip.Utilities;
using StripProgram = WinStrip.Entity.StripProgram;

namespace WinStrip
{
    public partial class FormMain : Form
    {
        
        Serial serial;
        List<StripProgram> programs;
        List<ProgramParameter> parameters;
        public int PortSpeed { get; set; }
        
        public FormMain()
        {

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

        private void FormMain_Load(object sender, EventArgs e)
        {
            labelStatus.Text = "";
            InitCombo();
            GetHardwareFromDevice();
        }

        private void GetHardwareFromDevice()
        {
            serial.WriteLine(SerialCommand.HARDWARE.ToString());
            var strBuffer = serial.ReadLine();
            var serializer = new JavaScriptSerializer();
            var ret = serializer.Deserialize<StripHardware>(strBuffer);

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

            bool enabled = serial.isConnected;
            groupBoxDelay.Enabled = enabled;
            groupBoxBrightness.Enabled = enabled;
            groupBoxParameters.Enabled = enabled;
            comboPrograms.Enabled = enabled;
            btnGetValues.Enabled = enabled;
            btnSendAll.Enabled = enabled;
            btnColor1.Enabled = enabled;
            btnColor2.Enabled = enabled;
            btnColor3.Enabled = enabled;
            btnColor4.Enabled = enabled;
            btnColor5.Enabled = enabled;
            btnColor6.Enabled = enabled;

            btnSend.Enabled = enabled;
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
                                                if (trackBarBrightness.Value      != newValue)       trackBarBrightness.Value = newValue;
                                                if (numericUpDownBrightness.Value != newValue)  numericUpDownBrightness.Value = newValue;
                                                break;
                case ValueControls.DELAY:
                                                if (trackBarDelay.Value           != newValue)            trackBarDelay.Value = newValue;
                                                if (numericUpDownDelay.Value      != newValue)       numericUpDownDelay.Value = newValue;
                                                break;
                case ValueControls.VALUE1:
                                                if (trackBarValue1.Value           != newValue)      trackBarValue1.Value = newValue;
                                                if (numericUpDownValue1.Value      != newValue) numericUpDownValue1.Value = newValue;
                                                break;
                case ValueControls.VALUE2:
                                                if (trackBarValue2.Value           != newValue)      trackBarValue2.Value = newValue;
                                                if (numericUpDownValue2.Value      != newValue) numericUpDownValue2.Value = newValue;
                                                break;                             
                case ValueControls.VALUE3:                                         
                                                if (trackBarValue3.Value           != newValue)      trackBarValue3.Value = newValue;
                                                if (numericUpDownValue3.Value      != newValue) numericUpDownValue3.Value = newValue;
                                                break;
            }
        }

        private void SendValuesToDevice()
        {

            var values = new StripValues
            {
                com = comboPrograms.SelectedIndex,
                delay = trackBarDelay.Value,
                //colors     = GetButtonColors(),
                values = GetValues(),
                brightness = trackBarBrightness.Value
            };

            var serializer = new JavaScriptSerializer();
            var str = serializer.Serialize(values);
            serial.WriteLine(str);

        }

        private void SendColorsToDevice()
        {

            var values = new StripColors
            {
                colors     = GetButtonColors(),
            };

            var serializer = new JavaScriptSerializer();
            var str = serializer.Serialize(values);
            serial.WriteLine(str);

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
    }
}
