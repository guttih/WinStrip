namespace WinStrip
{
    partial class FormMain
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
            this.btnSend = new System.Windows.Forms.Button();
            this.comboPorts = new System.Windows.Forms.ComboBox();
            this.labelStatus = new System.Windows.Forms.Label();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPrograms = new System.Windows.Forms.TabPage();
            this.label6 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.labelDescription = new System.Windows.Forms.Label();
            this.comboPrograms = new System.Windows.Forms.ComboBox();
            this.tabManual = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnClearText2 = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.tabCPU = new System.Windows.Forms.TabPage();
            this.labelNotImplemented = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.btnColor1 = new System.Windows.Forms.Button();
            this.btnColor2 = new System.Windows.Forms.Button();
            this.btnColor3 = new System.Windows.Forms.Button();
            this.btnColor4 = new System.Windows.Forms.Button();
            this.btnColor5 = new System.Windows.Forms.Button();
            this.btnColor6 = new System.Windows.Forms.Button();
            this.btnSendColors = new System.Windows.Forms.Button();
            this.textBoxDelay = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnGetAll = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPrograms.SuspendLayout();
            this.tabManual.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.tabCPU.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSend
            // 
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.Location = new System.Drawing.Point(495, 300);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 0;
            this.btnSend.Text = "Send";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // comboPorts
            // 
            this.comboPorts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.comboPorts.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboPorts.FormattingEnabled = true;
            this.comboPorts.Location = new System.Drawing.Point(23, 374);
            this.comboPorts.Name = "comboPorts";
            this.comboPorts.Size = new System.Drawing.Size(92, 21);
            this.comboPorts.TabIndex = 8;
            // 
            // labelStatus
            // 
            this.labelStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.labelStatus.Location = new System.Drawing.Point(0, 406);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(609, 20);
            this.labelStatus.TabIndex = 9;
            this.labelStatus.Text = "labelStatus labelStatus labelStatus labelStatus labelStatus labelStatus labelStat" +
    "us labelStatus labelStatus labelStatus labelStatus labelStatus labelStatus label" +
    "Status labelStatus labelStatus ";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPrograms);
            this.tabControl1.Controls.Add(this.tabManual);
            this.tabControl1.Controls.Add(this.tabCPU);
            this.tabControl1.Location = new System.Drawing.Point(13, 13);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(584, 355);
            this.tabControl1.TabIndex = 10;
            // 
            // tabPrograms
            // 
            this.tabPrograms.BackColor = System.Drawing.SystemColors.Control;
            this.tabPrograms.Controls.Add(this.btnSendColors);
            this.tabPrograms.Controls.Add(this.btnGetAll);
            this.tabPrograms.Controls.Add(this.textBoxDelay);
            this.tabPrograms.Controls.Add(this.groupBox1);
            this.tabPrograms.Controls.Add(this.dataGridView1);
            this.tabPrograms.Controls.Add(this.label6);
            this.tabPrograms.Controls.Add(this.label3);
            this.tabPrograms.Controls.Add(this.label4);
            this.tabPrograms.Controls.Add(this.label5);
            this.tabPrograms.Controls.Add(this.labelDescription);
            this.tabPrograms.Controls.Add(this.comboPrograms);
            this.tabPrograms.Location = new System.Drawing.Point(4, 22);
            this.tabPrograms.Name = "tabPrograms";
            this.tabPrograms.Padding = new System.Windows.Forms.Padding(3);
            this.tabPrograms.Size = new System.Drawing.Size(576, 329);
            this.tabPrograms.TabIndex = 1;
            this.tabPrograms.Text = "Programs";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 154);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(60, 13);
            this.label6.TabIndex = 1;
            this.label6.Text = "Parameters";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 13);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(35, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Name";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 63);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(60, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Description";
            // 
            // labelDescription
            // 
            this.labelDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.labelDescription.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.labelDescription.Location = new System.Drawing.Point(7, 78);
            this.labelDescription.Name = "labelDescription";
            this.labelDescription.Size = new System.Drawing.Size(426, 58);
            this.labelDescription.TabIndex = 1;
            this.labelDescription.Text = "label3";
            // 
            // comboPrograms
            // 
            this.comboPrograms.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboPrograms.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboPrograms.FormattingEnabled = true;
            this.comboPrograms.Location = new System.Drawing.Point(7, 29);
            this.comboPrograms.Name = "comboPrograms";
            this.comboPrograms.Size = new System.Drawing.Size(201, 21);
            this.comboPrograms.TabIndex = 0;
            this.comboPrograms.SelectedIndexChanged += new System.EventHandler(this.comboPrograms_SelectedIndexChanged);
            // 
            // tabManual
            // 
            this.tabManual.BackColor = System.Drawing.SystemColors.Control;
            this.tabManual.Controls.Add(this.label2);
            this.tabManual.Controls.Add(this.label1);
            this.tabManual.Controls.Add(this.btnClearText2);
            this.tabManual.Controls.Add(this.textBox2);
            this.tabManual.Controls.Add(this.textBox1);
            this.tabManual.Controls.Add(this.btnSend);
            this.tabManual.Location = new System.Drawing.Point(4, 22);
            this.tabManual.Name = "tabManual";
            this.tabManual.Padding = new System.Windows.Forms.Padding(3);
            this.tabManual.Size = new System.Drawing.Size(576, 329);
            this.tabManual.TabIndex = 0;
            this.tabManual.Text = "Manual";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Send";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 61);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(56, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Responce";
            // 
            // btnClearText2
            // 
            this.btnClearText2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnClearText2.Location = new System.Drawing.Point(11, 300);
            this.btnClearText2.Name = "btnClearText2";
            this.btnClearText2.Size = new System.Drawing.Size(75, 23);
            this.btnClearText2.TabIndex = 2;
            this.btnClearText2.Text = "Clear";
            this.btnClearText2.UseVisualStyleBackColor = true;
            this.btnClearText2.Click += new System.EventHandler(this.btnClearText2_Click);
            // 
            // textBox2
            // 
            this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox2.Location = new System.Drawing.Point(6, 77);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox2.Size = new System.Drawing.Size(559, 217);
            this.textBox2.TabIndex = 1;
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(6, 24);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(564, 20);
            this.textBox1.TabIndex = 0;
            this.textBox1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(6, 170);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.Size = new System.Drawing.Size(427, 153);
            this.dataGridView1.TabIndex = 2;
            // 
            // tabCPU
            // 
            this.tabCPU.BackColor = System.Drawing.SystemColors.Control;
            this.tabCPU.Controls.Add(this.labelNotImplemented);
            this.tabCPU.Font = new System.Drawing.Font("Segoe Print", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabCPU.Location = new System.Drawing.Point(4, 22);
            this.tabCPU.Name = "tabCPU";
            this.tabCPU.Padding = new System.Windows.Forms.Padding(3);
            this.tabCPU.Size = new System.Drawing.Size(576, 329);
            this.tabCPU.TabIndex = 2;
            this.tabCPU.Text = "CPU monitoring";
            // 
            // labelNotImplemented
            // 
            this.labelNotImplemented.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelNotImplemented.Font = new System.Drawing.Font("Segoe Print", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelNotImplemented.Location = new System.Drawing.Point(3, 3);
            this.labelNotImplemented.Name = "labelNotImplemented";
            this.labelNotImplemented.Size = new System.Drawing.Size(570, 323);
            this.labelNotImplemented.TabIndex = 0;
            this.labelNotImplemented.Text = "Not yet implemented!";
            this.labelNotImplemented.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnColor6);
            this.groupBox1.Controls.Add(this.btnColor5);
            this.groupBox1.Controls.Add(this.btnColor4);
            this.groupBox1.Controls.Add(this.btnColor3);
            this.groupBox1.Controls.Add(this.btnColor2);
            this.groupBox1.Controls.Add(this.btnColor1);
            this.groupBox1.Location = new System.Drawing.Point(450, 78);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(94, 245);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Colors";
            // 
            // btnColor1
            // 
            this.btnColor1.Location = new System.Drawing.Point(13, 19);
            this.btnColor1.Name = "btnColor1";
            this.btnColor1.Size = new System.Drawing.Size(75, 23);
            this.btnColor1.TabIndex = 0;
            this.btnColor1.Text = "Color 1";
            this.btnColor1.UseVisualStyleBackColor = true;
            this.btnColor1.Click += new System.EventHandler(this.buttonColor_Click);
            // 
            // btnColor2
            // 
            this.btnColor2.Location = new System.Drawing.Point(13, 48);
            this.btnColor2.Name = "btnColor2";
            this.btnColor2.Size = new System.Drawing.Size(75, 23);
            this.btnColor2.TabIndex = 0;
            this.btnColor2.Text = "Color 2";
            this.btnColor2.UseVisualStyleBackColor = true;
            this.btnColor2.Click += new System.EventHandler(this.buttonColor_Click);
            // 
            // btnColor3
            // 
            this.btnColor3.Location = new System.Drawing.Point(13, 77);
            this.btnColor3.Name = "btnColor3";
            this.btnColor3.Size = new System.Drawing.Size(75, 23);
            this.btnColor3.TabIndex = 0;
            this.btnColor3.Text = "Color 3";
            this.btnColor3.UseVisualStyleBackColor = true;
            this.btnColor3.Click += new System.EventHandler(this.buttonColor_Click);
            // 
            // btnColor4
            // 
            this.btnColor4.Location = new System.Drawing.Point(13, 106);
            this.btnColor4.Name = "btnColor4";
            this.btnColor4.Size = new System.Drawing.Size(75, 23);
            this.btnColor4.TabIndex = 0;
            this.btnColor4.Text = "Color 4";
            this.btnColor4.UseVisualStyleBackColor = true;
            this.btnColor4.Click += new System.EventHandler(this.buttonColor_Click);
            // 
            // btnColor5
            // 
            this.btnColor5.Location = new System.Drawing.Point(13, 135);
            this.btnColor5.Name = "btnColor5";
            this.btnColor5.Size = new System.Drawing.Size(75, 23);
            this.btnColor5.TabIndex = 0;
            this.btnColor5.Text = "Color 5";
            this.btnColor5.UseVisualStyleBackColor = true;
            this.btnColor5.Click += new System.EventHandler(this.buttonColor_Click);
            // 
            // btnColor6
            // 
            this.btnColor6.Location = new System.Drawing.Point(13, 164);
            this.btnColor6.Name = "btnColor6";
            this.btnColor6.Size = new System.Drawing.Size(75, 23);
            this.btnColor6.TabIndex = 0;
            this.btnColor6.Text = "Color 6";
            this.btnColor6.UseVisualStyleBackColor = true;
            this.btnColor6.Click += new System.EventHandler(this.buttonColor_Click);
            // 
            // btnSendColors
            // 
            this.btnSendColors.Location = new System.Drawing.Point(463, 29);
            this.btnSendColors.Name = "btnSendColors";
            this.btnSendColors.Size = new System.Drawing.Size(75, 23);
            this.btnSendColors.TabIndex = 4;
            this.btnSendColors.Text = "Send";
            this.btnSendColors.UseVisualStyleBackColor = true;
            this.btnSendColors.Click += new System.EventHandler(this.btnSendColors_Click);
            // 
            // textBoxDelay
            // 
            this.textBoxDelay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxDelay.Location = new System.Drawing.Point(225, 29);
            this.textBoxDelay.Name = "textBoxDelay";
            this.textBoxDelay.Size = new System.Drawing.Size(89, 20);
            this.textBoxDelay.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(224, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 13);
            this.label3.TabIndex = 1;
            this.label3.Text = "Delay";
            // 
            // btnGetAll
            // 
            this.btnGetAll.Location = new System.Drawing.Point(358, 29);
            this.btnGetAll.Name = "btnGetAll";
            this.btnGetAll.Size = new System.Drawing.Size(75, 23);
            this.btnGetAll.TabIndex = 6;
            this.btnGetAll.Text = "Read all";
            this.btnGetAll.UseVisualStyleBackColor = true;
            this.btnGetAll.Click += new System.EventHandler(this.btnGetAll_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(609, 426);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.comboPorts);
            this.MinimumSize = new System.Drawing.Size(212, 160);
            this.Name = "FormMain";
            this.Text = "WinStrip";
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabPrograms.ResumeLayout(false);
            this.tabPrograms.PerformLayout();
            this.tabManual.ResumeLayout(false);
            this.tabManual.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.tabCPU.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.ComboBox comboPorts;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabManual;
        private System.Windows.Forms.TabPage tabPrograms;
        private System.Windows.Forms.Button btnClearText2;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.ComboBox comboPrograms;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TabPage tabCPU;
        private System.Windows.Forms.Label labelNotImplemented;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnColor1;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Button btnColor6;
        private System.Windows.Forms.Button btnColor5;
        private System.Windows.Forms.Button btnColor4;
        private System.Windows.Forms.Button btnColor3;
        private System.Windows.Forms.Button btnColor2;
        private System.Windows.Forms.Button btnSendColors;
        private System.Windows.Forms.TextBox textBoxDelay;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnGetAll;
    }
}

