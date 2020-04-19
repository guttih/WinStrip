namespace WinStrip
{
    partial class FormExportCode
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
            this.numericUpDownLedCount = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.comboType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.comboColorScheme = new System.Windows.Forms.ComboBox();
            this.comboDataPin = new System.Windows.Forms.ComboBox();
            this.comboClockPin = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.checkHasClockPIn = new System.Windows.Forms.CheckBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLedCount)).BeginInit();
            this.SuspendLayout();
            // 
            // numericUpDownLedCount
            // 
            this.numericUpDownLedCount.Location = new System.Drawing.Point(36, 97);
            this.numericUpDownLedCount.Name = "numericUpDownLedCount";
            this.numericUpDownLedCount.Size = new System.Drawing.Size(75, 20);
            this.numericUpDownLedCount.TabIndex = 0;
            this.numericUpDownLedCount.ValueChanged += new System.EventHandler(this.numericUpDownLedCount_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(33, 79);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Number of leds";
            // 
            // btnExport
            // 
            this.btnExport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnExport.Location = new System.Drawing.Point(36, 249);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 23);
            this.btnExport.TabIndex = 2;
            this.btnExport.Text = "Export";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Location = new System.Drawing.Point(196, 249);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // comboType
            // 
            this.comboType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboType.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.comboType.FormattingEnabled = true;
            this.comboType.Location = new System.Drawing.Point(36, 146);
            this.comboType.Name = "comboType";
            this.comboType.Size = new System.Drawing.Size(75, 21);
            this.comboType.Sorted = true;
            this.comboType.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(33, 130);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Type";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(188, 130);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Color scheme";
            // 
            // comboColorScheme
            // 
            this.comboColorScheme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboColorScheme.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.comboColorScheme.FormattingEnabled = true;
            this.comboColorScheme.Location = new System.Drawing.Point(193, 146);
            this.comboColorScheme.Name = "comboColorScheme";
            this.comboColorScheme.Size = new System.Drawing.Size(78, 21);
            this.comboColorScheme.Sorted = true;
            this.comboColorScheme.TabIndex = 5;
            // 
            // comboDataPin
            // 
            this.comboDataPin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboDataPin.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.comboDataPin.FormattingEnabled = true;
            this.comboDataPin.Location = new System.Drawing.Point(36, 197);
            this.comboDataPin.Name = "comboDataPin";
            this.comboDataPin.Size = new System.Drawing.Size(75, 21);
            this.comboDataPin.TabIndex = 7;
            this.comboDataPin.SelectedIndexChanged += new System.EventHandler(this.comboDataPin_SelectedIndexChanged);
            // 
            // comboClockPin
            // 
            this.comboClockPin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboClockPin.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.comboClockPin.FormattingEnabled = true;
            this.comboClockPin.Location = new System.Drawing.Point(191, 197);
            this.comboClockPin.Name = "comboClockPin";
            this.comboClockPin.Size = new System.Drawing.Size(80, 21);
            this.comboClockPin.TabIndex = 8;
            this.comboClockPin.SelectedIndexChanged += new System.EventHandler(this.comboClockPin_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(33, 181);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Data pin";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(188, 181);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(51, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Clock pin";
            // 
            // checkHasClockPIn
            // 
            this.checkHasClockPIn.AutoSize = true;
            this.checkHasClockPIn.Checked = true;
            this.checkHasClockPIn.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkHasClockPIn.Location = new System.Drawing.Point(189, 97);
            this.checkHasClockPIn.Name = "checkHasClockPIn";
            this.checkHasClockPIn.Size = new System.Drawing.Size(91, 17);
            this.checkHasClockPIn.TabIndex = 9;
            this.checkHasClockPIn.Text = "Has clock pin";
            this.checkHasClockPIn.UseVisualStyleBackColor = true;
            this.checkHasClockPIn.CheckedChanged += new System.EventHandler(this.checkHasClockPIn_CheckedChanged);
            // 
            // textBox2
            // 
            this.textBox2.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.textBox2.Location = new System.Drawing.Point(12, 12);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(296, 60);
            this.textBox2.TabIndex = 14;
            this.textBox2.Text = "What kind of strip will the micro controller to be controlling? \r\n\r\nYou can selec" +
    "t the strip values here or alternatively change the code yourself after exportin" +
    "g it.\r\n";
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.Location = new System.Drawing.Point(266, 59);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(29, 13);
            this.linkLabel1.TabIndex = 15;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Help";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // FormExport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(307, 287);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.checkHasClockPIn);
            this.Controls.Add(this.comboClockPin);
            this.Controls.Add(this.comboDataPin);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboColorScheme);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.comboType);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnExport);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numericUpDownLedCount);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormExport";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Export code";
            this.Load += new System.EventHandler(this.FormExport_Load);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownLedCount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NumericUpDown numericUpDownLedCount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ComboBox comboType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comboColorScheme;
        private System.Windows.Forms.ComboBox comboDataPin;
        private System.Windows.Forms.ComboBox comboClockPin;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.CheckBox checkHasClockPIn;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.LinkLabel linkLabel1;
    }
}