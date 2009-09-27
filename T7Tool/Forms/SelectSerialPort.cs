using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
//using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO.Ports;

namespace T7Tool.Forms
{
    public partial class SelectSerialPort : Form
    {
        private string m_serialPort = "";
        private int m_portSpeed = 9600;
        private T7Tool m_t7tool = null;
        private string m_device = "";


        public SelectSerialPort()
        {
            InitializeComponent();
        }

        public string getPortName()
        {
            return m_serialPort;
        }

        public int getPortSpeed()
        {
            return m_portSpeed;
        }

        public void setCallbackObject(T7Tool a_t7tool)
        {
            m_t7tool = a_t7tool;
        }

        public void disablePortSpeed()
        {
            label1.Hide();
            radioButton38400.Hide();
            radioButton9600.Hide();
        }

        public void setDevice(string a_device)
        {
            m_device = a_device;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_serialPort = listBoxSerialPorts.SelectedItem.ToString();
        }


        private void SelectSerialPort_Load(object sender, EventArgs e)
        {
            string[] serialPortNames = SerialPort.GetPortNames();
            for (int i = 0; i < serialPortNames.Length; i++)
                listBoxSerialPorts.Items.Add(serialPortNames[i]);
            if (serialPortNames.Length > 0)
                listBoxSerialPorts.SelectedIndex = 0;
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            m_serialPort = listBoxSerialPorts.SelectedItem.ToString();
            buttonOK.Enabled = false;
            if (radioButton38400.Checked)
                m_portSpeed = 38400;
            else
                m_portSpeed = 9600;
            if(m_device.StartsWith("KLine"))
                m_t7tool.startKLine();
            else
                m_t7tool.startELM327();
            
        }


    }
}
