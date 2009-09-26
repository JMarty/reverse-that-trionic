namespace T7Tool.Forms
{
    partial class E85Form
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
            this.GetE85Level = new System.Windows.Forms.Button();
            this.Set = new System.Windows.Forms.Button();
            this.trackBarSet = new System.Windows.Forms.TrackBar();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.E85Label = new System.Windows.Forms.Label();
            this.SetLevelLabel = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.AdaptionStatusLabel = new System.Windows.Forms.Label();
            this.ForceAdaptionButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarSet)).BeginInit();
            this.SuspendLayout();
            // 
            // GetE85Level
            // 
            this.GetE85Level.Location = new System.Drawing.Point(229, 18);
            this.GetE85Level.Name = "GetE85Level";
            this.GetE85Level.Size = new System.Drawing.Size(75, 23);
            this.GetE85Level.TabIndex = 0;
            this.GetE85Level.Text = "Get";
            this.GetE85Level.UseVisualStyleBackColor = true;
            this.GetE85Level.Click += new System.EventHandler(this.button1_Click);
            // 
            // Set
            // 
            this.Set.Location = new System.Drawing.Point(229, 84);
            this.Set.Name = "Set";
            this.Set.Size = new System.Drawing.Size(75, 23);
            this.Set.TabIndex = 1;
            this.Set.Text = "Set";
            this.Set.UseVisualStyleBackColor = true;
            this.Set.Click += new System.EventHandler(this.Set_Click);
            // 
            // trackBarSet
            // 
            this.trackBarSet.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.trackBarSet.Cursor = System.Windows.Forms.Cursors.Default;
            this.trackBarSet.Location = new System.Drawing.Point(23, 84);
            this.trackBarSet.Maximum = 85;
            this.trackBarSet.Name = "trackBarSet";
            this.trackBarSet.Size = new System.Drawing.Size(179, 45);
            this.trackBarSet.TabIndex = 3;
            this.trackBarSet.Scroll += new System.EventHandler(this.trackBarSet_Scroll);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 122);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(24, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "0 %";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(172, 122);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "85 %";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(27, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(91, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Current E85 level:";
            // 
            // E85Label
            // 
            this.E85Label.AutoSize = true;
            this.E85Label.Location = new System.Drawing.Point(129, 23);
            this.E85Label.Name = "E85Label";
            this.E85Label.Size = new System.Drawing.Size(0, 13);
            this.E85Label.TabIndex = 7;
            // 
            // SetLevelLabel
            // 
            this.SetLevelLabel.AutoSize = true;
            this.SetLevelLabel.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.SetLevelLabel.Location = new System.Drawing.Point(109, 122);
            this.SetLevelLabel.Name = "SetLevelLabel";
            this.SetLevelLabel.Size = new System.Drawing.Size(0, 13);
            this.SetLevelLabel.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(27, 170);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Adaption status:";
            // 
            // AdaptionStatusLabel
            // 
            this.AdaptionStatusLabel.AutoSize = true;
            this.AdaptionStatusLabel.Location = new System.Drawing.Point(129, 170);
            this.AdaptionStatusLabel.Name = "AdaptionStatusLabel";
            this.AdaptionStatusLabel.Size = new System.Drawing.Size(0, 13);
            this.AdaptionStatusLabel.TabIndex = 11;
            // 
            // ForceAdaptionButton
            // 
            this.ForceAdaptionButton.Location = new System.Drawing.Point(201, 165);
            this.ForceAdaptionButton.Name = "ForceAdaptionButton";
            this.ForceAdaptionButton.Size = new System.Drawing.Size(103, 23);
            this.ForceAdaptionButton.TabIndex = 12;
            this.ForceAdaptionButton.Text = "Force adaption";
            this.ForceAdaptionButton.UseVisualStyleBackColor = true;
            this.ForceAdaptionButton.Click += new System.EventHandler(this.ForceAdaptionButton_Click);
            // 
            // E85Form
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(343, 217);
            this.Controls.Add(this.ForceAdaptionButton);
            this.Controls.Add(this.AdaptionStatusLabel);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.SetLevelLabel);
            this.Controls.Add(this.E85Label);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.trackBarSet);
            this.Controls.Add(this.Set);
            this.Controls.Add(this.GetE85Level);
            this.Name = "E85Form";
            this.Text = "E85";
            this.Load += new System.EventHandler(this.E85Form_Load);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarSet)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button GetE85Level;
        private System.Windows.Forms.Button Set;
        private System.Windows.Forms.TrackBar trackBarSet;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label E85Label;
        private System.Windows.Forms.Label SetLevelLabel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label AdaptionStatusLabel;
        private System.Windows.Forms.Button ForceAdaptionButton;
    }
}