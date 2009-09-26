using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using T7Tool.KWP;

namespace T7Tool.Forms
{
    public partial class E85Form : Form
    {
        public E85Form()
        {
            InitializeComponent();
            int level = 0;
            KWPHandler.getInstance().getE85Level(out level);
            E85Label.Text = level.ToString() + " %";
            trackBarSet.Value = level;
            string status;
            KWPHandler.getInstance().getE85AdaptionStatus(out status);
            AdaptionStatusLabel.Text = status;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int level = 0;
            KWPHandler.getInstance().getE85Level(out level);
            E85Label.Text = level.ToString() + " %";
            trackBarSet.Value = level;
            SetLevelLabel.Text = "";
            string status;
            KWPHandler.getInstance().getE85AdaptionStatus(out status);
            AdaptionStatusLabel.Text = status;

        }

        private void Set_Click(object sender, EventArgs e)
        {
            KWPHandler.getInstance().setE85Level(trackBarSet.Value);
            int level = 0;
            KWPHandler.getInstance().getE85Level(out level);
            E85Label.Text = level.ToString() + " %";
            SetLevelLabel.Text = "";
            string status;
            KWPHandler.getInstance().getE85AdaptionStatus(out status);
            AdaptionStatusLabel.Text = status;
        }

        private void trackBarSet_Scroll(object sender, EventArgs e)
        {
            SetLevelLabel.Text = trackBarSet.Value.ToString();
        }


        private void ForceAdaptionButton_Click(object sender, EventArgs e)
        {
            int level;
            KWPHandler.getInstance().forceE85Adaption();
            KWPHandler.getInstance().getE85Level(out level);
            E85Label.Text = level.ToString() + " %";
            SetLevelLabel.Text = "";
            string status;
            KWPHandler.getInstance().getE85AdaptionStatus(out status);
            AdaptionStatusLabel.Text = status;
        }

        private void E85Form_Load(object sender, EventArgs e)
        {

        }
    }
}
