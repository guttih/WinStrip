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
        public FormMain()
        {
            InitializeComponent();

            serial = new Serial();
            parameters = new List<ProgramParameter>();
        }

        private void PopulateDataGrid()
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();
            dataGridView1.Columns.Add("name", "Name");
            dataGridView1.Columns.Add("value", "Value");
            
            var col = dataGridView1.Columns[0];
            col.ReadOnly = true;
            col.MinimumWidth = 160;
            col.SortMode = DataGridViewColumnSortMode.NotSortable;

            col = dataGridView1.Columns[1];
            dataGridView1.Columns[1].ReadOnly = false;
            col.SortMode = DataGridViewColumnSortMode.NotSortable;


            foreach (var param in parameters)
            {
                dataGridView1.Rows.Add(new string[] { param.Name, param.Value.ToString() });
            }
            

        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            labelStatus.Text = "";
            InitCombo();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            var text = textBox1.Text;

            //var json = new JavaScriptSerializer().Serialize(cmd);

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
                    var nextPortName = comboPorts.Items[index].ToString();
                    labelStatus.Text = $"{nextPortName}";
                    if (serial.OpenSerialPort(nextPortName, 500000))
                    {
                        comboPorts.SelectedIndex = index;
                        labelStatus.Text = $"Connected to port {nextPortName}";
                        GetAllStatus();

                        return;
                    }
                    index++;
                }
            }
            labelStatus.Text = "Unable to connect to any com port";
        }


        private void GetAllStatus()
        {
            var cmd = SerialCommand.ALLSTATUS.ToString();
            serial.WriteLine(cmd);
            var strBuffer = serial.ReadLine();
            var serializer = new JavaScriptSerializer();
            //programs = serializer.Deserialize<List<StripProgram>>(strBuffer);
            AllStatus status = serializer.Deserialize<AllStatus>(strBuffer);
            programs = status.programs;

            textBoxDelay.Text = status.delay.ToString();
            
            btnColor1.BackColor = new SColor(status.colors[0]).Color;
            btnColor2.BackColor = new SColor(status.colors[1]).Color;
            btnColor3.BackColor = new SColor(status.colors[2]).Color;
            btnColor4.BackColor = new SColor(status.colors[3]).Color;
            btnColor5.BackColor = new SColor(status.colors[4]).Color;
            btnColor6.BackColor = new SColor(status.colors[5]).Color;

            
            comboPrograms.Items.Clear();
            programs.ForEach(m => comboPrograms.Items.Add(m.name));
            comboPrograms.SelectedIndex = status.com;
        }

        private void btnGetAll_Click(object sender, EventArgs e)
        {
            GetAllStatus();
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
                    PopulateDataGrid();
                    return; 
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void buttonColor_Click(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            colorDialog1.Color = button.BackColor;

            if (colorDialog1.ShowDialog() == DialogResult.OK)
            {
                button.BackColor = colorDialog1.Color;
            }
        }
    }
}
