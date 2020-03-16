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
            this.groupBoxDelay = new System.Windows.Forms.GroupBox();
            this.trackBarDelay = new System.Windows.Forms.TrackBar();
            this.numericUpDownDelay = new System.Windows.Forms.NumericUpDown();
            this.groupBoxBrightness = new System.Windows.Forms.GroupBox();
            this.trackBarBrightness = new System.Windows.Forms.TrackBar();
            this.numericUpDownBrightness = new System.Windows.Forms.NumericUpDown();
            this.btnSendAll = new System.Windows.Forms.Button();
            this.btnGetValues = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnColor6 = new System.Windows.Forms.Button();
            this.btnColor5 = new System.Windows.Forms.Button();
            this.btnColor4 = new System.Windows.Forms.Button();
            this.btnColor3 = new System.Windows.Forms.Button();
            this.btnColor2 = new System.Windows.Forms.Button();
            this.btnColor1 = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.labelDescription = new System.Windows.Forms.Label();
            this.comboPrograms = new System.Windows.Forms.ComboBox();
            this.tabManual = new System.Windows.Forms.TabPage();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnClearText2 = new System.Windows.Forms.Button();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBoxCustomSend = new System.Windows.Forms.TextBox();
            this.tabCPU = new System.Windows.Forms.TabPage();
            this.labelNotImplemented = new System.Windows.Forms.Label();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.groupBoxParameters = new System.Windows.Forms.GroupBox();
            this.trackBarValue1 = new System.Windows.Forms.TrackBar();
            this.numericUpDownValue1 = new System.Windows.Forms.NumericUpDown();
            this.groupBoxValue1 = new System.Windows.Forms.GroupBox();
            this.groupBoxValue2 = new System.Windows.Forms.GroupBox();
            this.numericUpDownValue2 = new System.Windows.Forms.NumericUpDown();
            this.trackBarValue2 = new System.Windows.Forms.TrackBar();
            this.groupBoxValue3 = new System.Windows.Forms.GroupBox();
            this.numericUpDownValue3 = new System.Windows.Forms.NumericUpDown();
            this.trackBarValue3 = new System.Windows.Forms.TrackBar();
            this.btnConnection = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPrograms.SuspendLayout();
            this.groupBoxDelay.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarDelay)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDelay)).BeginInit();
            this.groupBoxBrightness.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarBrightness)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBrightness)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.tabManual.SuspendLayout();
            this.tabCPU.SuspendLayout();
            this.groupBoxParameters.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarValue1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownValue1)).BeginInit();
            this.groupBoxValue1.SuspendLayout();
            this.groupBoxValue2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownValue2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarValue2)).BeginInit();
            this.groupBoxValue3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownValue3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarValue3)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSend
            // 
            this.btnSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSend.Location = new System.Drawing.Point(633, 386);
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
            this.comboPorts.Location = new System.Drawing.Point(23, 460);
            this.comboPorts.Name = "comboPorts";
            this.comboPorts.Size = new System.Drawing.Size(92, 21);
            this.comboPorts.TabIndex = 8;
            this.comboPorts.SelectedIndexChanged += new System.EventHandler(this.comboPorts_SelectedIndexChanged);
            // 
            // labelStatus
            // 
            this.labelStatus.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.labelStatus.Location = new System.Drawing.Point(0, 492);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(752, 20);
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
            this.tabControl1.Size = new System.Drawing.Size(727, 441);
            this.tabControl1.TabIndex = 10;
            // 
            // tabPrograms
            // 
            this.tabPrograms.BackColor = System.Drawing.SystemColors.Control;
            this.tabPrograms.Controls.Add(this.groupBoxParameters);
            this.tabPrograms.Controls.Add(this.groupBoxDelay);
            this.tabPrograms.Controls.Add(this.groupBoxBrightness);
            this.tabPrograms.Controls.Add(this.btnSendAll);
            this.tabPrograms.Controls.Add(this.btnGetValues);
            this.tabPrograms.Controls.Add(this.groupBox1);
            this.tabPrograms.Controls.Add(this.label4);
            this.tabPrograms.Controls.Add(this.label5);
            this.tabPrograms.Controls.Add(this.labelDescription);
            this.tabPrograms.Controls.Add(this.comboPrograms);
            this.tabPrograms.Location = new System.Drawing.Point(4, 22);
            this.tabPrograms.Name = "tabPrograms";
            this.tabPrograms.Padding = new System.Windows.Forms.Padding(3);
            this.tabPrograms.Size = new System.Drawing.Size(719, 415);
            this.tabPrograms.TabIndex = 1;
            this.tabPrograms.Text = "Programs";
            // 
            // groupBoxDelay
            // 
            this.groupBoxDelay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxDelay.Controls.Add(this.trackBarDelay);
            this.groupBoxDelay.Controls.Add(this.numericUpDownDelay);
            this.groupBoxDelay.Location = new System.Drawing.Point(426, 78);
            this.groupBoxDelay.Name = "groupBoxDelay";
            this.groupBoxDelay.Size = new System.Drawing.Size(82, 325);
            this.groupBoxDelay.TabIndex = 10;
            this.groupBoxDelay.TabStop = false;
            this.groupBoxDelay.Text = "Delay";
            // 
            // trackBarDelay
            // 
            this.trackBarDelay.Location = new System.Drawing.Point(23, 19);
            this.trackBarDelay.Maximum = 5000;
            this.trackBarDelay.Name = "trackBarDelay";
            this.trackBarDelay.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBarDelay.Size = new System.Drawing.Size(45, 256);
            this.trackBarDelay.TabIndex = 7;
            this.trackBarDelay.Value = 1;
            this.trackBarDelay.ValueChanged += new System.EventHandler(this.ValueControl_ValueChanged);
            // 
            // numericUpDownDelay
            // 
            this.numericUpDownDelay.Location = new System.Drawing.Point(13, 281);
            this.numericUpDownDelay.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numericUpDownDelay.Name = "numericUpDownDelay";
            this.numericUpDownDelay.Size = new System.Drawing.Size(55, 20);
            this.numericUpDownDelay.TabIndex = 8;
            this.numericUpDownDelay.ValueChanged += new System.EventHandler(this.ValueControl_ValueChanged);
            // 
            // groupBoxBrightness
            // 
            this.groupBoxBrightness.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxBrightness.Controls.Add(this.trackBarBrightness);
            this.groupBoxBrightness.Controls.Add(this.numericUpDownBrightness);
            this.groupBoxBrightness.Location = new System.Drawing.Point(514, 78);
            this.groupBoxBrightness.Name = "groupBoxBrightness";
            this.groupBoxBrightness.Size = new System.Drawing.Size(82, 325);
            this.groupBoxBrightness.TabIndex = 10;
            this.groupBoxBrightness.TabStop = false;
            this.groupBoxBrightness.Text = "Brightness";
            // 
            // trackBarBrightness
            // 
            this.trackBarBrightness.Location = new System.Drawing.Point(22, 19);
            this.trackBarBrightness.Maximum = 255;
            this.trackBarBrightness.Name = "trackBarBrightness";
            this.trackBarBrightness.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBarBrightness.Size = new System.Drawing.Size(45, 256);
            this.trackBarBrightness.TabIndex = 7;
            this.trackBarBrightness.Value = 1;
            this.trackBarBrightness.ValueChanged += new System.EventHandler(this.ValueControl_ValueChanged);
            // 
            // numericUpDownBrightness
            // 
            this.numericUpDownBrightness.Location = new System.Drawing.Point(22, 281);
            this.numericUpDownBrightness.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numericUpDownBrightness.Name = "numericUpDownBrightness";
            this.numericUpDownBrightness.Size = new System.Drawing.Size(45, 20);
            this.numericUpDownBrightness.TabIndex = 8;
            this.numericUpDownBrightness.ValueChanged += new System.EventHandler(this.ValueControl_ValueChanged);
            // 
            // btnSendAll
            // 
            this.btnSendAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSendAll.Location = new System.Drawing.Point(624, 27);
            this.btnSendAll.Name = "btnSendAll";
            this.btnSendAll.Size = new System.Drawing.Size(75, 23);
            this.btnSendAll.TabIndex = 4;
            this.btnSendAll.Text = "Send";
            this.btnSendAll.UseVisualStyleBackColor = true;
            this.btnSendAll.Click += new System.EventHandler(this.btnSendAll_Click);
            // 
            // btnGetValues
            // 
            this.btnGetValues.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGetValues.Location = new System.Drawing.Point(419, 27);
            this.btnGetValues.Name = "btnGetValues";
            this.btnGetValues.Size = new System.Drawing.Size(75, 23);
            this.btnGetValues.TabIndex = 6;
            this.btnGetValues.Text = "Get values";
            this.btnGetValues.UseVisualStyleBackColor = true;
            this.btnGetValues.Click += new System.EventHandler(this.btnGetValues_Click);
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
            this.groupBox1.Location = new System.Drawing.Point(614, 78);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(94, 325);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Colors";
            // 
            // btnColor6
            // 
            this.btnColor6.Location = new System.Drawing.Point(10, 278);
            this.btnColor6.Name = "btnColor6";
            this.btnColor6.Size = new System.Drawing.Size(75, 23);
            this.btnColor6.TabIndex = 0;
            this.btnColor6.Text = "Color 6";
            this.btnColor6.UseVisualStyleBackColor = true;
            this.btnColor6.Click += new System.EventHandler(this.buttonColor_Click);
            // 
            // btnColor5
            // 
            this.btnColor5.Location = new System.Drawing.Point(10, 223);
            this.btnColor5.Name = "btnColor5";
            this.btnColor5.Size = new System.Drawing.Size(75, 23);
            this.btnColor5.TabIndex = 0;
            this.btnColor5.Text = "Color 5";
            this.btnColor5.UseVisualStyleBackColor = true;
            this.btnColor5.Click += new System.EventHandler(this.buttonColor_Click);
            // 
            // btnColor4
            // 
            this.btnColor4.Location = new System.Drawing.Point(10, 172);
            this.btnColor4.Name = "btnColor4";
            this.btnColor4.Size = new System.Drawing.Size(75, 23);
            this.btnColor4.TabIndex = 0;
            this.btnColor4.Text = "Color 4";
            this.btnColor4.UseVisualStyleBackColor = true;
            this.btnColor4.Click += new System.EventHandler(this.buttonColor_Click);
            // 
            // btnColor3
            // 
            this.btnColor3.Location = new System.Drawing.Point(10, 121);
            this.btnColor3.Name = "btnColor3";
            this.btnColor3.Size = new System.Drawing.Size(75, 23);
            this.btnColor3.TabIndex = 0;
            this.btnColor3.Text = "Color 3";
            this.btnColor3.UseVisualStyleBackColor = true;
            this.btnColor3.Click += new System.EventHandler(this.buttonColor_Click);
            // 
            // btnColor2
            // 
            this.btnColor2.Location = new System.Drawing.Point(10, 70);
            this.btnColor2.Name = "btnColor2";
            this.btnColor2.Size = new System.Drawing.Size(75, 23);
            this.btnColor2.TabIndex = 0;
            this.btnColor2.Text = "Color 2";
            this.btnColor2.UseVisualStyleBackColor = true;
            this.btnColor2.Click += new System.EventHandler(this.buttonColor_Click);
            // 
            // btnColor1
            // 
            this.btnColor1.Location = new System.Drawing.Point(10, 19);
            this.btnColor1.Name = "btnColor1";
            this.btnColor1.Size = new System.Drawing.Size(75, 23);
            this.btnColor1.TabIndex = 0;
            this.btnColor1.Text = "Color 1";
            this.btnColor1.UseVisualStyleBackColor = true;
            this.btnColor1.Click += new System.EventHandler(this.buttonColor_Click);
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
            this.labelDescription.Size = new System.Drawing.Size(385, 58);
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
            this.comboPrograms.Size = new System.Drawing.Size(385, 21);
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
            this.tabManual.Controls.Add(this.textBoxCustomSend);
            this.tabManual.Controls.Add(this.btnSend);
            this.tabManual.Location = new System.Drawing.Point(4, 22);
            this.tabManual.Name = "tabManual";
            this.tabManual.Padding = new System.Windows.Forms.Padding(3);
            this.tabManual.Size = new System.Drawing.Size(719, 415);
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
            this.btnClearText2.Location = new System.Drawing.Point(11, 386);
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
            this.textBox2.Size = new System.Drawing.Size(702, 303);
            this.textBox2.TabIndex = 1;
            // 
            // textBoxCustomSend
            // 
            this.textBoxCustomSend.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBoxCustomSend.Location = new System.Drawing.Point(6, 24);
            this.textBoxCustomSend.Name = "textBoxCustomSend";
            this.textBoxCustomSend.Size = new System.Drawing.Size(707, 20);
            this.textBoxCustomSend.TabIndex = 0;
            this.textBoxCustomSend.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textBox1_KeyDown);
            // 
            // tabCPU
            // 
            this.tabCPU.BackColor = System.Drawing.SystemColors.Control;
            this.tabCPU.Controls.Add(this.labelNotImplemented);
            this.tabCPU.Font = new System.Drawing.Font("Segoe Print", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabCPU.Location = new System.Drawing.Point(4, 22);
            this.tabCPU.Name = "tabCPU";
            this.tabCPU.Padding = new System.Windows.Forms.Padding(3);
            this.tabCPU.Size = new System.Drawing.Size(719, 415);
            this.tabCPU.TabIndex = 2;
            this.tabCPU.Text = "CPU monitoring";
            // 
            // labelNotImplemented
            // 
            this.labelNotImplemented.Dock = System.Windows.Forms.DockStyle.Fill;
            this.labelNotImplemented.Font = new System.Drawing.Font("Segoe Print", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelNotImplemented.Location = new System.Drawing.Point(3, 3);
            this.labelNotImplemented.Name = "labelNotImplemented";
            this.labelNotImplemented.Size = new System.Drawing.Size(713, 409);
            this.labelNotImplemented.TabIndex = 0;
            this.labelNotImplemented.Text = "Not yet implemented!";
            this.labelNotImplemented.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // groupBoxParameters
            // 
            this.groupBoxParameters.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxParameters.Controls.Add(this.groupBoxValue3);
            this.groupBoxParameters.Controls.Add(this.groupBoxValue2);
            this.groupBoxParameters.Controls.Add(this.groupBoxValue1);
            this.groupBoxParameters.Location = new System.Drawing.Point(7, 145);
            this.groupBoxParameters.Name = "groupBoxParameters";
            this.groupBoxParameters.Size = new System.Drawing.Size(385, 258);
            this.groupBoxParameters.TabIndex = 11;
            this.groupBoxParameters.TabStop = false;
            this.groupBoxParameters.Text = "Parameters";
            // 
            // trackBarValue1
            // 
            this.trackBarValue1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarValue1.Location = new System.Drawing.Point(6, 23);
            this.trackBarValue1.Maximum = 5000;
            this.trackBarValue1.Name = "trackBarValue1";
            this.trackBarValue1.Size = new System.Drawing.Size(286, 45);
            this.trackBarValue1.TabIndex = 9;
            this.trackBarValue1.Value = 1;
            this.trackBarValue1.ValueChanged += new System.EventHandler(this.ValueControl_ValueChanged);
            // 
            // numericUpDownValue1
            // 
            this.numericUpDownValue1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownValue1.Location = new System.Drawing.Point(296, 30);
            this.numericUpDownValue1.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numericUpDownValue1.Name = "numericUpDownValue1";
            this.numericUpDownValue1.Size = new System.Drawing.Size(55, 20);
            this.numericUpDownValue1.TabIndex = 10;
            this.numericUpDownValue1.ValueChanged += new System.EventHandler(this.ValueControl_ValueChanged);
            // 
            // groupBoxValue1
            // 
            this.groupBoxValue1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxValue1.Controls.Add(this.numericUpDownValue1);
            this.groupBoxValue1.Controls.Add(this.trackBarValue1);
            this.groupBoxValue1.Location = new System.Drawing.Point(6, 22);
            this.groupBoxValue1.Name = "groupBoxValue1";
            this.groupBoxValue1.Size = new System.Drawing.Size(370, 75);
            this.groupBoxValue1.TabIndex = 12;
            this.groupBoxValue1.TabStop = false;
            this.groupBoxValue1.Text = "groupBoxValue1";
            // 
            // groupBoxValue2
            // 
            this.groupBoxValue2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxValue2.Controls.Add(this.numericUpDownValue2);
            this.groupBoxValue2.Controls.Add(this.trackBarValue2);
            this.groupBoxValue2.Location = new System.Drawing.Point(6, 98);
            this.groupBoxValue2.Name = "groupBoxValue2";
            this.groupBoxValue2.Size = new System.Drawing.Size(370, 75);
            this.groupBoxValue2.TabIndex = 12;
            this.groupBoxValue2.TabStop = false;
            this.groupBoxValue2.Text = "groupBoxValue2";
            // 
            // numericUpDownValue2
            // 
            this.numericUpDownValue2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownValue2.Location = new System.Drawing.Point(296, 30);
            this.numericUpDownValue2.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numericUpDownValue2.Name = "numericUpDownValue2";
            this.numericUpDownValue2.Size = new System.Drawing.Size(55, 20);
            this.numericUpDownValue2.TabIndex = 10;
            this.numericUpDownValue2.ValueChanged += new System.EventHandler(this.ValueControl_ValueChanged);
            // 
            // trackBarValue2
            // 
            this.trackBarValue2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarValue2.Location = new System.Drawing.Point(6, 23);
            this.trackBarValue2.Maximum = 5000;
            this.trackBarValue2.Name = "trackBarValue2";
            this.trackBarValue2.Size = new System.Drawing.Size(286, 45);
            this.trackBarValue2.TabIndex = 9;
            this.trackBarValue2.Value = 1;
            this.trackBarValue2.ValueChanged += new System.EventHandler(this.ValueControl_ValueChanged);
            // 
            // groupBoxValue3
            // 
            this.groupBoxValue3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBoxValue3.Controls.Add(this.numericUpDownValue3);
            this.groupBoxValue3.Controls.Add(this.trackBarValue3);
            this.groupBoxValue3.Location = new System.Drawing.Point(6, 174);
            this.groupBoxValue3.Name = "groupBoxValue3";
            this.groupBoxValue3.Size = new System.Drawing.Size(370, 75);
            this.groupBoxValue3.TabIndex = 12;
            this.groupBoxValue3.TabStop = false;
            this.groupBoxValue3.Text = "groupBoxValue3";
            // 
            // numericUpDownValue3
            // 
            this.numericUpDownValue3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.numericUpDownValue3.Location = new System.Drawing.Point(296, 30);
            this.numericUpDownValue3.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.numericUpDownValue3.Name = "numericUpDownValue3";
            this.numericUpDownValue3.Size = new System.Drawing.Size(55, 20);
            this.numericUpDownValue3.TabIndex = 10;
            this.numericUpDownValue3.ValueChanged += new System.EventHandler(this.ValueControl_ValueChanged);
            // 
            // trackBarValue3
            // 
            this.trackBarValue3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarValue3.Location = new System.Drawing.Point(6, 23);
            this.trackBarValue3.Maximum = 5000;
            this.trackBarValue3.Name = "trackBarValue3";
            this.trackBarValue3.Size = new System.Drawing.Size(286, 45);
            this.trackBarValue3.TabIndex = 9;
            this.trackBarValue3.Value = 1;
            this.trackBarValue3.ValueChanged += new System.EventHandler(this.ValueControl_ValueChanged);
            // 
            // btnConnection
            // 
            this.btnConnection.Location = new System.Drawing.Point(130, 460);
            this.btnConnection.Name = "btnConnection";
            this.btnConnection.Size = new System.Drawing.Size(75, 23);
            this.btnConnection.TabIndex = 11;
            this.btnConnection.Text = "Con/Dis";
            this.btnConnection.UseVisualStyleBackColor = true;
            this.btnConnection.Click += new System.EventHandler(this.btnConnection_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(752, 512);
            this.Controls.Add(this.btnConnection);
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
            this.groupBoxDelay.ResumeLayout(false);
            this.groupBoxDelay.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarDelay)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownDelay)).EndInit();
            this.groupBoxBrightness.ResumeLayout(false);
            this.groupBoxBrightness.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarBrightness)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBrightness)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.tabManual.ResumeLayout(false);
            this.tabManual.PerformLayout();
            this.tabCPU.ResumeLayout(false);
            this.groupBoxParameters.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.trackBarValue1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownValue1)).EndInit();
            this.groupBoxValue1.ResumeLayout(false);
            this.groupBoxValue1.PerformLayout();
            this.groupBoxValue2.ResumeLayout(false);
            this.groupBoxValue2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownValue2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarValue2)).EndInit();
            this.groupBoxValue3.ResumeLayout(false);
            this.groupBoxValue3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownValue3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBarValue3)).EndInit();
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
        private System.Windows.Forms.TextBox textBoxCustomSend;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label labelDescription;
        private System.Windows.Forms.ComboBox comboPrograms;
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
        private System.Windows.Forms.Button btnSendAll;
        private System.Windows.Forms.Button btnGetValues;
        private System.Windows.Forms.GroupBox groupBoxBrightness;
        private System.Windows.Forms.TrackBar trackBarBrightness;
        private System.Windows.Forms.NumericUpDown numericUpDownBrightness;
        private System.Windows.Forms.GroupBox groupBoxDelay;
        private System.Windows.Forms.TrackBar trackBarDelay;
        private System.Windows.Forms.NumericUpDown numericUpDownDelay;
        private System.Windows.Forms.GroupBox groupBoxParameters;
        private System.Windows.Forms.GroupBox groupBoxValue1;
        private System.Windows.Forms.NumericUpDown numericUpDownValue1;
        private System.Windows.Forms.TrackBar trackBarValue1;
        private System.Windows.Forms.GroupBox groupBoxValue3;
        private System.Windows.Forms.NumericUpDown numericUpDownValue3;
        private System.Windows.Forms.TrackBar trackBarValue3;
        private System.Windows.Forms.GroupBox groupBoxValue2;
        private System.Windows.Forms.NumericUpDown numericUpDownValue2;
        private System.Windows.Forms.TrackBar trackBarValue2;
        private System.Windows.Forms.Button btnConnection;
    }
}

