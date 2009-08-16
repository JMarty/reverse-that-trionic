using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace T7Tool.Forms
{
    public partial class InfoFormOK : Form
    {
        public InfoFormOK(string a_info)
        {
            InitializeComponent();
            InfoLabel.Text = a_info;
        }

        public void setText(string a_text)
        {
            InfoLabel.Text = a_text;
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void InfoForm_Load(object sender, EventArgs e)
        {

        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}