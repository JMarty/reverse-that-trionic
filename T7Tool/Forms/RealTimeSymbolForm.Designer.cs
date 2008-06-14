namespace T7Tool.Forms
{
    partial class RealTimeSymbolForm
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
            this.Hide();
           /* if (disposing && (components != null))
            {
                components.Dispose();
            }*/
           // base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.symbolListBox = new System.Windows.Forms.ListBox();
            this.dataTabControl = new System.Windows.Forms.TabControl();
            this.hexTabPage = new System.Windows.Forms.TabPage();
            this.hexBox = new Be.Windows.Forms.HexBox();
            this.twoDTabPage = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.DataTypeLabel = new System.Windows.Forms.Label();
            this.SymbolNameLabel = new System.Windows.Forms.Label();
            this.SymbolLengthLabel = new System.Windows.Forms.Label();
            this.AddressLabel = new System.Windows.Forms.Label();
            this.dataTabControl.SuspendLayout();
            this.hexTabPage.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // symbolListBox
            // 
            this.symbolListBox.FormattingEnabled = true;
            this.symbolListBox.Location = new System.Drawing.Point(5, 32);
            this.symbolListBox.Name = "symbolListBox";
            this.symbolListBox.Size = new System.Drawing.Size(208, 563);
            this.symbolListBox.TabIndex = 0;
            this.symbolListBox.SelectedIndexChanged += new System.EventHandler(this.symbolListBox_SelectedIndexChanged);
            // 
            // dataTabControl
            // 
            this.dataTabControl.Controls.Add(this.hexTabPage);
            this.dataTabControl.Controls.Add(this.twoDTabPage);
            this.dataTabControl.Location = new System.Drawing.Point(219, 249);
            this.dataTabControl.Name = "dataTabControl";
            this.dataTabControl.SelectedIndex = 0;
            this.dataTabControl.Size = new System.Drawing.Size(631, 348);
            this.dataTabControl.TabIndex = 1;
            // 
            // hexTabPage
            // 
            this.hexTabPage.Controls.Add(this.hexBox);
            this.hexTabPage.Location = new System.Drawing.Point(4, 22);
            this.hexTabPage.Name = "hexTabPage";
            this.hexTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.hexTabPage.Size = new System.Drawing.Size(623, 322);
            this.hexTabPage.TabIndex = 0;
            this.hexTabPage.Text = "Hex";
            this.hexTabPage.UseVisualStyleBackColor = true;
            // 
            // hexBox
            // 
            this.hexBox.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.hexBox.LineInfoForeColor = System.Drawing.Color.Empty;
            this.hexBox.LineInfoVisible = true;
            this.hexBox.Location = new System.Drawing.Point(0, 0);
            this.hexBox.Name = "hexBox";
            this.hexBox.ShadowSelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(60)))), ((int)(((byte)(188)))), ((int)(((byte)(255)))));
            this.hexBox.Size = new System.Drawing.Size(620, 325);
            this.hexBox.StringViewVisible = true;
            this.hexBox.TabIndex = 0;
            this.hexBox.UseFixedBytesPerLine = true;
            this.hexBox.Click += new System.EventHandler(this.hexBox_Change);
            this.hexBox.Enter += new System.EventHandler(this.hexBox_Change);
            this.hexBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.hexBox_ChangeComplete);

            // 
            // twoDTabPage
            // 
            this.twoDTabPage.Location = new System.Drawing.Point(4, 22);
            this.twoDTabPage.Name = "twoDTabPage";
            this.twoDTabPage.Padding = new System.Windows.Forms.Padding(3);
            this.twoDTabPage.Size = new System.Drawing.Size(623, 322);
            this.twoDTabPage.TabIndex = 1;
            this.twoDTabPage.Text = "2-D";
            this.twoDTabPage.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.DataTypeLabel);
            this.groupBox1.Controls.Add(this.SymbolNameLabel);
            this.groupBox1.Controls.Add(this.SymbolLengthLabel);
            this.groupBox1.Controls.Add(this.AddressLabel);
            this.groupBox1.Location = new System.Drawing.Point(219, 32);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(627, 205);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Symbol information";
            // 
            // DataTypeLabel
            // 
            this.DataTypeLabel.AutoSize = true;
            this.DataTypeLabel.Location = new System.Drawing.Point(33, 156);
            this.DataTypeLabel.Name = "DataTypeLabel";
            this.DataTypeLabel.Size = new System.Drawing.Size(56, 13);
            this.DataTypeLabel.TabIndex = 4;
            this.DataTypeLabel.Text = "Data type:";
            // 
            // SymbolNameLabel
            // 
            this.SymbolNameLabel.AutoSize = true;
            this.SymbolNameLabel.Location = new System.Drawing.Point(33, 28);
            this.SymbolNameLabel.Name = "SymbolNameLabel";
            this.SymbolNameLabel.Size = new System.Drawing.Size(38, 13);
            this.SymbolNameLabel.TabIndex = 3;
            this.SymbolNameLabel.Text = "Name:";
            // 
            // SymbolLengthLabel
            // 
            this.SymbolLengthLabel.AutoSize = true;
            this.SymbolLengthLabel.Location = new System.Drawing.Point(33, 113);
            this.SymbolLengthLabel.Name = "SymbolLengthLabel";
            this.SymbolLengthLabel.Size = new System.Drawing.Size(43, 13);
            this.SymbolLengthLabel.TabIndex = 1;
            this.SymbolLengthLabel.Text = "Length:";
            // 
            // AddressLabel
            // 
            this.AddressLabel.AutoSize = true;
            this.AddressLabel.Location = new System.Drawing.Point(33, 70);
            this.AddressLabel.Name = "AddressLabel";
            this.AddressLabel.Size = new System.Drawing.Size(48, 13);
            this.AddressLabel.TabIndex = 0;
            this.AddressLabel.Text = "Address:";
            this.AddressLabel.Click += new System.EventHandler(this.label1_Click);
            // 
            // RealTimeSymbolForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(856, 606);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.dataTabControl);
            this.Controls.Add(this.symbolListBox);
            this.Name = "RealTimeSymbolForm";
            this.Text = "Real Time Tuning";
            this.Load += new System.EventHandler(this.RealTimeSymbolForm_Load);
            this.Disposed += new System.EventHandler(this.RealTimeSymbolForm_Disposed);
            this.dataTabControl.ResumeLayout(false);
            this.hexTabPage.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox symbolListBox;
        private System.Windows.Forms.TabControl dataTabControl;
        private System.Windows.Forms.TabPage hexTabPage;
        private System.Windows.Forms.TabPage twoDTabPage;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label AddressLabel;
        private System.Windows.Forms.Label DataTypeLabel;
        private System.Windows.Forms.Label SymbolNameLabel;
        private System.Windows.Forms.Label SymbolLengthLabel;
        private Be.Windows.Forms.HexBox hexBox;
    }
}