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

namespace WinStrip
{
    public partial class FormMain : Form
    {
        Serial serial;
        public FormMain()
        {
            InitializeComponent();

            serial = new Serial();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            labelStatus.Text = "";
            InitCombo();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            var text = textBox1.Text;

            var cmd = new StripCommand { Name = "nafn", Litur=33 };
            var json = new JavaScriptSerializer().Serialize(cmd);


            serial.WriteLine(json);
            try { 
                var str = serial.ReadLine();
                labelStatus.Text = str;
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
            btnSend.Enabled = textBox1.Text.Length > 0 && serial.isConnected;
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
                        return;
                    }
                    index++;
                }
            }
            labelStatus.Text = "Unable to connect to any com port";
        }
    }
}
