using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WinStrip.Entity;
using WinStrip.Utilities;
using WinStrip.EntityTransfer;
using System.Web.Script.Serialization;

namespace WinStrip
{
    public partial class FormStep : Form
    {
        public Step Step { get; set; }

        List<StripProgram> Programs;
        List<ProgramParameter> Parameters;
        ColorDialog ColorDialog;
        Serial serial;
        bool doNotSendToDevice = false;

        public FormStep(Step step, List<StripProgram> programs, ColorDialog colorDialog, Serial serial)
        {

            Step = step;
            Parameters = new List<ProgramParameter>();
            Programs = programs;
            ColorDialog = colorDialog;
            this.serial = serial;
            

            InitializeComponent();
        }

        private void StepToForm()
        {
            trackBarFrom.Value = Step.From;
            ValuesToControls(Step.ValuesAndColors.brightness, Step.ValuesAndColors.delay, Step.ValuesAndColors.values, Step.ValuesAndColors.colors);
            //comboPrograms.SelectedIndex = 0;
            if (Step.ValuesAndColors.com > -1 && Step.ValuesAndColors.com < Programs.Count) 
            {
                int i = comboPrograms.Items.IndexOf(Programs[Step.ValuesAndColors.com].name);
                if (i > -1)
                {
                    comboPrograms.SelectedIndex = i;
                    doNotSendToDevice = false;
                    if (serial.isConnected)
                        serial.WriteJson(Step.ValuesAndColorsToJson());
                }
                else
                {
                    
                }
                    
            }

        }

        private void FormToStep()
        {
            Step.ValuesAndColors.com = comboPrograms.SelectedIndex;
            Step.From= trackBarFrom.Value;
            Step.ValuesAndColors.delay = trackBarDelay.Value;
            Step.ValuesAndColors.brightness = trackBarBrightness.Value;

            Step.ValuesAndColors.values[0] = trackBarValue1.Value;
            Step.ValuesAndColors.values[1] = trackBarValue2.Value;
            Step.ValuesAndColors.values[2] = trackBarValue3.Value;

            Step.ValuesAndColors.colors[0] = (new SColor(btnColor1.BackColor)).ToUlong();
            Step.ValuesAndColors.colors[1] = (new SColor(btnColor2.BackColor)).ToUlong();
            Step.ValuesAndColors.colors[2] = (new SColor(btnColor3.BackColor)).ToUlong();
            Step.ValuesAndColors.colors[3] = (new SColor(btnColor4.BackColor)).ToUlong();
            Step.ValuesAndColors.colors[4] = (new SColor(btnColor5.BackColor)).ToUlong();
            Step.ValuesAndColors.colors[5] = (new SColor(btnColor6.BackColor)).ToUlong();

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            FormToStep();
            DialogResult = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void FormStep_Load(object sender, EventArgs e)
        {
            doNotSendToDevice = true;
            Programs.ForEach(m => comboPrograms.Items.Add(m.name));
            
            StepToForm();
        }

        private void comboPrograms_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void ValueControl_ValueChanged(object sender, EventArgs e)
        {
            var typeName = sender.GetType().Name;
            if (typeName == "TrackBar")
            {
                var control = (TrackBar)sender;
                SetControlValue(GetControlValueFromName(control.Name), control.Value);
                if (serial.isConnected)
                    SendValuesToDevice();
            }
            else if (typeName == "NumericUpDown")
            {
                var control = (NumericUpDown)sender;
                SetControlValue(GetControlValueFromName(control.Name), (int)control.Value);
            }
        }

        private void buttonColor_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            ColorDialog.Color = button.BackColor;

            if (ColorDialog.ShowDialog() == DialogResult.OK)
            {
                button.BackColor = ColorDialog.Color;
                SendColorsToDevice();
            }
        }

        private void SendColorsToDevice(StripColors colors = null)
        {
            if (colors == null)
            {
                colors = new StripColors
                {
                    colors = GetButtonColors(),
                };
            }
            var serializer = new JavaScriptSerializer();
            var str = serializer.Serialize(colors);
            serial.WriteLine(str);

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

        private void SetControlValue(ValueControls control, int newValue)
        {
            switch (control)
            {
                case ValueControls.BRIGHTNESS:
                    if (trackBarBrightness.Value != newValue) trackBarBrightness.Value = newValue;
                    if (numericUpDownBrightness.Value != newValue) numericUpDownBrightness.Value = newValue;
                    break;
                case ValueControls.DELAY:
                    if (trackBarDelay.Value != newValue) trackBarDelay.Value = newValue;
                    if (numericUpDownDelay.Value != newValue) numericUpDownDelay.Value = newValue;
                    break;
                case ValueControls.VALUE1:
                    if (trackBarValue1.Value != newValue) trackBarValue1.Value = newValue;
                    if (numericUpDownValue1.Value != newValue) numericUpDownValue1.Value = newValue;
                    break;
                case ValueControls.VALUE2:
                    if (trackBarValue2.Value != newValue) trackBarValue2.Value = newValue;
                    if (numericUpDownValue2.Value != newValue) numericUpDownValue2.Value = newValue;
                    break;
                case ValueControls.VALUE3:
                    if (trackBarValue3.Value != newValue) trackBarValue3.Value = newValue;
                    if (numericUpDownValue3.Value != newValue) numericUpDownValue3.Value = newValue;
                    break;
                case ValueControls.FROM:

                    if (trackBarFrom.Value != newValue) trackBarFrom.Value = newValue;
                    if (numericUpDownFrom.Value != newValue) numericUpDownFrom.Value = newValue;
                    break;
            }
        }

        ValueControls GetControlValueFromName(string Name)
        {
            switch (Name)
            {
                case "trackBarBrightness":
                case "numericUpDownBrightness": return ValueControls.BRIGHTNESS;

                case "trackBarDelay":
                case "numericUpDownDelay": return ValueControls.DELAY;

                case "trackBarValue1":
                case "numericUpDownValue1": return ValueControls.VALUE1;

                case "trackBarValue2":
                case "numericUpDownValue2": return ValueControls.VALUE2;

                case "trackBarValue3":
                case "numericUpDownValue3": return ValueControls.VALUE3;

                case "trackBarCpuTesting":
                case "numericUpDownCpuTesting": return ValueControls.CPUTESTING;

                case "trackBarFrom":
                case "numericUpDownFrom": return ValueControls.FROM;

            }
            return ValueControls.INVALID;
        }

        private void comboPrograms_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            var name = comboBox.Text;
            labelDescription.Text = "";
            foreach (var program in Programs)
            {
                if (name.Equals(program.name))
                {
                    labelDescription.Text = program.description.Replace("\n", "\r\n");
                    Parameters.Clear();

                    foreach (var value in program.values)
                    {
                        Parameters.Add(new ProgramParameter { Name = value, Value = 0 });
                    }
                    ShowAndSetParamNames();
                    if (serial.isConnected)
                        SendValuesToDevice();

                    return;
                }
            }
        }

        private List<int> GetValues()
        {
            var list = new List<int>();
            if (groupBoxValue1.Visible) list.Add(trackBarValue1.Value);
            if (groupBoxValue2.Visible) list.Add(trackBarValue2.Value);
            if (groupBoxValue3.Visible) list.Add(trackBarValue3.Value);

            return list;
        }

        private void SendValuesToDevice(StripValues values = null)
        {
            if (doNotSendToDevice)
                return;

            if (values == null)
            {
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

        private void ShowAndSetParamNames()
        {
            groupBoxValue1.Visible = Parameters.Count > 0;
            groupBoxValue2.Visible = Parameters.Count > 1;
            groupBoxValue3.Visible = Parameters.Count > 2;

            if (groupBoxValue1.Visible) groupBoxValue1.Text = Parameters[0].Name;
            if (groupBoxValue2.Visible) groupBoxValue2.Text = Parameters[1].Name;
            if (groupBoxValue3.Visible) groupBoxValue3.Text = Parameters[2].Name;

        }

        private void ValuesToControls(int brightness, int delay, List<int> values, List<ulong> colors)
        {
            SetControlValue(ValueControls.DELAY, delay);
            SetControlValue(ValueControls.BRIGHTNESS, brightness);

            int initialCount = values.Count;
            if (initialCount < 3) values.Add(0);
            if (initialCount < 2) values.Add(0);
            if (initialCount < 1) values.Add(0);

            initialCount = colors.Count;
            if (initialCount < 6) colors.Add(0);
            if (initialCount < 5) colors.Add(0);
            if (initialCount < 4) colors.Add(0);
            if (initialCount < 5) colors.Add(0);
            if (initialCount < 6) colors.Add(0);
            if (initialCount < 1) colors.Add(0);

            SetControlValue(ValueControls.VALUE1, values[0]);
            SetControlValue(ValueControls.VALUE2, values[1]);
            SetControlValue(ValueControls.VALUE3, values[2]);

            btnColor1.BackColor = new SColor(colors[0]).Color;
            btnColor2.BackColor = new SColor(colors[1]).Color;
            btnColor3.BackColor = new SColor(colors[2]).Color;
            btnColor4.BackColor = new SColor(colors[3]).Color;
            btnColor5.BackColor = new SColor(colors[4]).Color;
            btnColor6.BackColor = new SColor(colors[5]).Color;



        }
    }
}
