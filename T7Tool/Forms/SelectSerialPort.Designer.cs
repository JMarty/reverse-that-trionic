namespace T7Tool.Forms
{
    partial class SelectSerialPort
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.listBoxSerialPorts = new System.Windows.Forms.ListBox();
            this.radioButton9600 = new System.Windows.Forms.RadioButton();
            this.radioButton38400 = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.buttonOK = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // listBoxSerialPorts
            // 
            this.listBoxSerialPorts.FormattingEnabled = true;
            this.listBoxSerialPorts.Location = new System.Drawing.Point(15, 26);
            this.listBoxSerialPorts.Name = "listBoxSerialPorts";
            this.listBoxSerialPorts.Size = new System.Drawing.Size(152, 147);
            this.listBoxSerialPorts.TabIndex = 0;
            this.listBoxSerialPorts.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // radioButton9600
            // 
            this.radioButton9600.AutoSize = true;
            this.radioButton9600.Checked = true;
            this.radioButton9600.Location = new System.Drawing.Point(210, 45);
            this.radioButton9600.Name = "radioButton9600";
            this.radioButton9600.Size = new System.Drawing.Size(49, 17);
            this.radioButton9600.TabIndex = 1;
            this.radioButton9600.TabStop = true;
            this.radioButton9600.Text = "9600";
            this.radioButton9600.UseVisualStyleBackColor = true;
            // 
            // radioButton38400
            // 
            this.radioButton38400.AutoSize = true;
            this.radioButton38400.Location = new System.Drawing.Point(210, 68);
            this.radioButton38400.Name = "radioButton38400";
            this.radioButton38400.Size = new System.Drawing.Size(55, 17);
            this.radioButton38400.TabIndex = 2;
            this.radioButton38400.Text = "38400";
            this.radioButton38400.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(208, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Port speed";
            // 
            // buttonOK
            // 
            this.buttonOK.Location = new System.Drawing.Point(195, 150);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 4;
            this.buttonOK.Text = "Ok";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // SelectSerialPort
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(292, 194);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.radioButton38400);
            this.Controls.Add(this.radioButton9600);
            this.Controls.Add(this.listBoxSerialPorts);
            this.Name = "SelectSerialPort";
            this.Text = "Select serial port";
            this.Load += new System.EventHandler(this.SelectSerialPort_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox listBoxSerialPorts;
        private System.Windows.Forms.RadioButton radioButton9600;
        private System.Windows.Forms.RadioButton radioButton38400;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonOK;
    }
}