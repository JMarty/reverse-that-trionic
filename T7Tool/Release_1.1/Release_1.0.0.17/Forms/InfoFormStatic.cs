using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace T7Tool.Forms
{
    public partial class InfoFormStatic : Form
    {
        public InfoFormStatic()
        {
            InitializeComponent();
        }

        public void setText(string a_text)
        {
            InfoLabel.Text = a_text;
        }

        private void InfoFormStatic_Load(object sender, EventArgs e)
        {

        }
    }
}