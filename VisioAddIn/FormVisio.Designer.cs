namespace VisioAddIn
{
    partial class FormVisio
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormVisio));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.actionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToSimioToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.labelStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabConvertVisioFile = new System.Windows.Forms.TabPage();
            this.groupDisplayFacilityView = new System.Windows.Forms.GroupBox();
            this.label20 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.updnScaleDrawingZ = new System.Windows.Forms.NumericUpDown();
            this.updnScaleDrawingY = new System.Windows.Forms.NumericUpDown();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.updnScaleObjectsZ = new System.Windows.Forms.NumericUpDown();
            this.updnScaleObjectsY = new System.Windows.Forms.NumericUpDown();
            this.label14 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.updnSimioZOrigin = new System.Windows.Forms.NumericUpDown();
            this.updnScaleObjectsX = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.updnScaleDrawingX = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.updnSimioYOrigin = new System.Windows.Forms.NumericUpDown();
            this.updnSimioXOrigin = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.groupVisioToDataSet = new System.Windows.Forms.GroupBox();
            this.cbAutoScaleObjects = new System.Windows.Forms.CheckBox();
            this.textVisioData = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.buttonImportVisioToFacility = new System.Windows.Forms.Button();
            this.comboVisioPages = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.buttonSelectVisioFile = new System.Windows.Forms.Button();
            this.textVisioFilePath = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.tabPreview = new System.Windows.Forms.TabPage();
            this.picPreviewDiagram = new System.Windows.Forms.PictureBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.textPreviewInfo = new System.Windows.Forms.TextBox();
            this.labelPreviewInfo = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.labelPreviewMouse = new System.Windows.Forms.Label();
            this.textPreviewMouse = new System.Windows.Forms.TextBox();
            this.tabLogs = new System.Windows.Forms.TabPage();
            this.textLogs = new System.Windows.Forms.TextBox();
            this.timerLogs = new System.Windows.Forms.Timer(this.components);
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabConvertVisioFile.SuspendLayout();
            this.groupDisplayFacilityView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.updnScaleDrawingZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.updnScaleDrawingY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.updnScaleObjectsZ)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.updnScaleObjectsY)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.updnSimioZOrigin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.updnScaleObjectsX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.updnScaleDrawingX)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.updnSimioYOrigin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.updnSimioXOrigin)).BeginInit();
            this.groupVisioToDataSet.SuspendLayout();
            this.tabPreview.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picPreviewDiagram)).BeginInit();
            this.panel1.SuspendLayout();
            this.tabLogs.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.closeToolStripMenuItem,
            this.actionsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1266, 28);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            this.menuStrip1.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.menuStrip1_ItemClicked);
            // 
            // closeToolStripMenuItem
            // 
            this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            this.closeToolStripMenuItem.Size = new System.Drawing.Size(45, 24);
            this.closeToolStripMenuItem.Text = "E&xit";
            this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
            // 
            // actionsToolStripMenuItem
            // 
            this.actionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.importToSimioToolStripMenuItem});
            this.actionsToolStripMenuItem.Name = "actionsToolStripMenuItem";
            this.actionsToolStripMenuItem.Size = new System.Drawing.Size(70, 24);
            this.actionsToolStripMenuItem.Text = "&Actions";
            this.actionsToolStripMenuItem.Click += new System.EventHandler(this.actionsToolStripMenuItem_Click);
            // 
            // importToSimioToolStripMenuItem
            // 
            this.importToSimioToolStripMenuItem.Name = "importToSimioToolStripMenuItem";
            this.importToSimioToolStripMenuItem.Size = new System.Drawing.Size(238, 26);
            this.importToSimioToolStripMenuItem.Text = "&Import to Simio Facility";
            this.importToSimioToolStripMenuItem.Click += new System.EventHandler(this.importToSimioToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.labelStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 739);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1266, 25);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // labelStatus
            // 
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(18, 20);
            this.labelStatus.Text = "...";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabConvertVisioFile);
            this.tabControl1.Controls.Add(this.tabPreview);
            this.tabControl1.Controls.Add(this.tabLogs);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 28);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(1266, 711);
            this.tabControl1.TabIndex = 5;
            // 
            // tabConvertVisioFile
            // 
            this.tabConvertVisioFile.Controls.Add(this.groupDisplayFacilityView);
            this.tabConvertVisioFile.Controls.Add(this.groupVisioToDataSet);
            this.tabConvertVisioFile.Location = new System.Drawing.Point(4, 25);
            this.tabConvertVisioFile.Name = "tabConvertVisioFile";
            this.tabConvertVisioFile.Padding = new System.Windows.Forms.Padding(3);
            this.tabConvertVisioFile.Size = new System.Drawing.Size(1258, 682);
            this.tabConvertVisioFile.TabIndex = 1;
            this.tabConvertVisioFile.Text = "Convert Visio File";
            this.tabConvertVisioFile.UseVisualStyleBackColor = true;
            // 
            // groupDisplayFacilityView
            // 
            this.groupDisplayFacilityView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupDisplayFacilityView.Controls.Add(this.label20);
            this.groupDisplayFacilityView.Controls.Add(this.label19);
            this.groupDisplayFacilityView.Controls.Add(this.label18);
            this.groupDisplayFacilityView.Controls.Add(this.updnScaleDrawingZ);
            this.groupDisplayFacilityView.Controls.Add(this.updnScaleDrawingY);
            this.groupDisplayFacilityView.Controls.Add(this.label17);
            this.groupDisplayFacilityView.Controls.Add(this.label16);
            this.groupDisplayFacilityView.Controls.Add(this.label15);
            this.groupDisplayFacilityView.Controls.Add(this.updnScaleObjectsZ);
            this.groupDisplayFacilityView.Controls.Add(this.updnScaleObjectsY);
            this.groupDisplayFacilityView.Controls.Add(this.label14);
            this.groupDisplayFacilityView.Controls.Add(this.label10);
            this.groupDisplayFacilityView.Controls.Add(this.label5);
            this.groupDisplayFacilityView.Controls.Add(this.label3);
            this.groupDisplayFacilityView.Controls.Add(this.updnSimioZOrigin);
            this.groupDisplayFacilityView.Controls.Add(this.updnScaleObjectsX);
            this.groupDisplayFacilityView.Controls.Add(this.label1);
            this.groupDisplayFacilityView.Controls.Add(this.updnScaleDrawingX);
            this.groupDisplayFacilityView.Controls.Add(this.label9);
            this.groupDisplayFacilityView.Controls.Add(this.label8);
            this.groupDisplayFacilityView.Controls.Add(this.label7);
            this.groupDisplayFacilityView.Controls.Add(this.updnSimioYOrigin);
            this.groupDisplayFacilityView.Controls.Add(this.updnSimioXOrigin);
            this.groupDisplayFacilityView.Controls.Add(this.label6);
            this.groupDisplayFacilityView.Location = new System.Drawing.Point(8, 376);
            this.groupDisplayFacilityView.Name = "groupDisplayFacilityView";
            this.groupDisplayFacilityView.Size = new System.Drawing.Size(1222, 300);
            this.groupDisplayFacilityView.TabIndex = 2;
            this.groupDisplayFacilityView.TabStop = false;
            this.groupDisplayFacilityView.Text = "Display Transform settings for the Facility View.";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.ForeColor = System.Drawing.Color.Blue;
            this.label20.Location = new System.Drawing.Point(527, 235);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(21, 17);
            this.label20.TabIndex = 32;
            this.label20.Text = "Z:";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.ForeColor = System.Drawing.Color.ForestGreen;
            this.label19.Location = new System.Drawing.Point(389, 235);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(21, 17);
            this.label19.TabIndex = 31;
            this.label19.Text = "Y:";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.ForeColor = System.Drawing.Color.Crimson;
            this.label18.Location = new System.Drawing.Point(273, 235);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(21, 17);
            this.label18.TabIndex = 30;
            this.label18.Text = "X:";
            // 
            // updnScaleDrawingZ
            // 
            this.updnScaleDrawingZ.DecimalPlaces = 2;
            this.updnScaleDrawingZ.ForeColor = System.Drawing.Color.Blue;
            this.updnScaleDrawingZ.Location = new System.Drawing.Point(554, 232);
            this.updnScaleDrawingZ.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.updnScaleDrawingZ.Name = "updnScaleDrawingZ";
            this.updnScaleDrawingZ.Size = new System.Drawing.Size(79, 22);
            this.updnScaleDrawingZ.TabIndex = 29;
            this.updnScaleDrawingZ.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.updnScaleDrawingZ.Value = new decimal(new int[] {
            40,
            0,
            0,
            65536});
            // 
            // updnScaleDrawingY
            // 
            this.updnScaleDrawingY.DecimalPlaces = 2;
            this.updnScaleDrawingY.ForeColor = System.Drawing.Color.ForestGreen;
            this.updnScaleDrawingY.Location = new System.Drawing.Point(416, 232);
            this.updnScaleDrawingY.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.updnScaleDrawingY.Name = "updnScaleDrawingY";
            this.updnScaleDrawingY.Size = new System.Drawing.Size(79, 22);
            this.updnScaleDrawingY.TabIndex = 28;
            this.updnScaleDrawingY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.updnScaleDrawingY.Value = new decimal(new int[] {
            40,
            0,
            0,
            65536});
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(6, 195);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(433, 17);
            this.label17.TabIndex = 27;
            this.label17.Text = "Remember: The 2D (X,Y) dimensions for Simio are actually X and Z";
            this.label17.Click += new System.EventHandler(this.label17_Click);
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.ForeColor = System.Drawing.Color.Blue;
            this.label16.Location = new System.Drawing.Point(527, 267);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(21, 17);
            this.label16.TabIndex = 26;
            this.label16.Text = "Z:";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.ForeColor = System.Drawing.Color.ForestGreen;
            this.label15.Location = new System.Drawing.Point(389, 267);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(21, 17);
            this.label15.TabIndex = 25;
            this.label15.Text = "Y:";
            // 
            // updnScaleObjectsZ
            // 
            this.updnScaleObjectsZ.DecimalPlaces = 2;
            this.updnScaleObjectsZ.ForeColor = System.Drawing.Color.Blue;
            this.updnScaleObjectsZ.Location = new System.Drawing.Point(554, 264);
            this.updnScaleObjectsZ.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.updnScaleObjectsZ.Name = "updnScaleObjectsZ";
            this.updnScaleObjectsZ.Size = new System.Drawing.Size(79, 22);
            this.updnScaleObjectsZ.TabIndex = 24;
            this.updnScaleObjectsZ.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.updnScaleObjectsZ.Value = new decimal(new int[] {
            10,
            0,
            0,
            65536});
            // 
            // updnScaleObjectsY
            // 
            this.updnScaleObjectsY.DecimalPlaces = 2;
            this.updnScaleObjectsY.ForeColor = System.Drawing.Color.ForestGreen;
            this.updnScaleObjectsY.Location = new System.Drawing.Point(416, 264);
            this.updnScaleObjectsY.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.updnScaleObjectsY.Name = "updnScaleObjectsY";
            this.updnScaleObjectsY.Size = new System.Drawing.Size(79, 22);
            this.updnScaleObjectsY.TabIndex = 23;
            this.updnScaleObjectsY.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.updnScaleObjectsY.Value = new decimal(new int[] {
            10,
            0,
            0,
            65536});
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.ForeColor = System.Drawing.Color.Crimson;
            this.label14.Location = new System.Drawing.Point(273, 267);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(21, 17);
            this.label14.TabIndex = 22;
            this.label14.Text = "X:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(6, 39);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(327, 17);
            this.label10.TabIndex = 19;
            this.label10.Text = "Enter the Origin location on the Simio Facility View:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 169);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(722, 17);
            this.label5.TabIndex = 18;
            this.label5.Text = "You can scale the entire drawing space and objects separately.  The number is the" +
    " ratio of Simio-Size / Visio-Size";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Blue;
            this.label3.Location = new System.Drawing.Point(527, 77);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(21, 17);
            this.label3.TabIndex = 17;
            this.label3.Text = "Z:";
            // 
            // updnSimioZOrigin
            // 
            this.updnSimioZOrigin.DecimalPlaces = 2;
            this.updnSimioZOrigin.ForeColor = System.Drawing.Color.Blue;
            this.updnSimioZOrigin.Location = new System.Drawing.Point(554, 74);
            this.updnSimioZOrigin.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.updnSimioZOrigin.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.updnSimioZOrigin.Name = "updnSimioZOrigin";
            this.updnSimioZOrigin.Size = new System.Drawing.Size(79, 22);
            this.updnSimioZOrigin.TabIndex = 16;
            this.updnSimioZOrigin.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.updnSimioZOrigin.ValueChanged += new System.EventHandler(this.updnSimioZOrigin_ValueChanged);
            // 
            // updnScaleObjectsX
            // 
            this.updnScaleObjectsX.DecimalPlaces = 2;
            this.updnScaleObjectsX.ForeColor = System.Drawing.Color.Crimson;
            this.updnScaleObjectsX.Location = new System.Drawing.Point(300, 264);
            this.updnScaleObjectsX.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.updnScaleObjectsX.Name = "updnScaleObjectsX";
            this.updnScaleObjectsX.Size = new System.Drawing.Size(79, 22);
            this.updnScaleObjectsX.TabIndex = 15;
            this.updnScaleObjectsX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.updnScaleObjectsX.Value = new decimal(new int[] {
            10,
            0,
            0,
            65536});
            this.updnScaleObjectsX.ValueChanged += new System.EventHandler(this.updnScaleObjectsX_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 267);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(163, 17);
            this.label1.TabIndex = 14;
            this.label1.Text = "Scale Visio Objects\' Size";
            // 
            // updnScaleDrawingX
            // 
            this.updnScaleDrawingX.DecimalPlaces = 2;
            this.updnScaleDrawingX.ForeColor = System.Drawing.Color.Crimson;
            this.updnScaleDrawingX.Location = new System.Drawing.Point(300, 232);
            this.updnScaleDrawingX.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            65536});
            this.updnScaleDrawingX.Name = "updnScaleDrawingX";
            this.updnScaleDrawingX.Size = new System.Drawing.Size(79, 22);
            this.updnScaleDrawingX.TabIndex = 13;
            this.updnScaleDrawingX.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.updnScaleDrawingX.Value = new decimal(new int[] {
            40,
            0,
            0,
            65536});
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 232);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(132, 17);
            this.label9.TabIndex = 12;
            this.label9.Text = "Scale Visio Drawing";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.ForeColor = System.Drawing.Color.ForestGreen;
            this.label8.Location = new System.Drawing.Point(389, 77);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(21, 17);
            this.label8.TabIndex = 11;
            this.label8.Text = "Y:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.Crimson;
            this.label7.Location = new System.Drawing.Point(273, 77);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(21, 17);
            this.label7.TabIndex = 10;
            this.label7.Text = "X:";
            // 
            // updnSimioYOrigin
            // 
            this.updnSimioYOrigin.DecimalPlaces = 2;
            this.updnSimioYOrigin.ForeColor = System.Drawing.Color.ForestGreen;
            this.updnSimioYOrigin.Location = new System.Drawing.Point(416, 74);
            this.updnSimioYOrigin.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.updnSimioYOrigin.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.updnSimioYOrigin.Name = "updnSimioYOrigin";
            this.updnSimioYOrigin.Size = new System.Drawing.Size(79, 22);
            this.updnSimioYOrigin.TabIndex = 9;
            this.updnSimioYOrigin.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.updnSimioYOrigin.ValueChanged += new System.EventHandler(this.updnSimioYOrigin_ValueChanged);
            // 
            // updnSimioXOrigin
            // 
            this.updnSimioXOrigin.DecimalPlaces = 2;
            this.updnSimioXOrigin.ForeColor = System.Drawing.Color.Crimson;
            this.updnSimioXOrigin.Location = new System.Drawing.Point(300, 74);
            this.updnSimioXOrigin.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.updnSimioXOrigin.Minimum = new decimal(new int[] {
            1000,
            0,
            0,
            -2147483648});
            this.updnSimioXOrigin.Name = "updnSimioXOrigin";
            this.updnSimioXOrigin.Size = new System.Drawing.Size(79, 22);
            this.updnSimioXOrigin.TabIndex = 8;
            this.updnSimioXOrigin.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.updnSimioXOrigin.ValueChanged += new System.EventHandler(this.updnSimioXOrigin_ValueChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 74);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(136, 17);
            this.label6.TabIndex = 7;
            this.label6.Text = "Place Visio Origin At";
            // 
            // groupVisioToDataSet
            // 
            this.groupVisioToDataSet.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupVisioToDataSet.Controls.Add(this.cbAutoScaleObjects);
            this.groupVisioToDataSet.Controls.Add(this.textVisioData);
            this.groupVisioToDataSet.Controls.Add(this.label12);
            this.groupVisioToDataSet.Controls.Add(this.buttonImportVisioToFacility);
            this.groupVisioToDataSet.Controls.Add(this.comboVisioPages);
            this.groupVisioToDataSet.Controls.Add(this.label11);
            this.groupVisioToDataSet.Controls.Add(this.buttonSelectVisioFile);
            this.groupVisioToDataSet.Controls.Add(this.textVisioFilePath);
            this.groupVisioToDataSet.Controls.Add(this.label4);
            this.groupVisioToDataSet.Location = new System.Drawing.Point(8, 21);
            this.groupVisioToDataSet.Name = "groupVisioToDataSet";
            this.groupVisioToDataSet.Size = new System.Drawing.Size(1242, 304);
            this.groupVisioToDataSet.TabIndex = 0;
            this.groupVisioToDataSet.TabStop = false;
            this.groupVisioToDataSet.Text = "Import Visio to Simio Facility";
            // 
            // cbAutoScaleObjects
            // 
            this.cbAutoScaleObjects.AutoSize = true;
            this.cbAutoScaleObjects.Location = new System.Drawing.Point(443, 245);
            this.cbAutoScaleObjects.Name = "cbAutoScaleObjects";
            this.cbAutoScaleObjects.Size = new System.Drawing.Size(180, 21);
            this.cbAutoScaleObjects.TabIndex = 33;
            this.cbAutoScaleObjects.Text = "AutoScale Visio Objects";
            this.toolTip1.SetToolTip(this.cbAutoScaleObjects, "AutoScale each Visio Object\'s size according to nominal Visio object sizes");
            this.cbAutoScaleObjects.UseVisualStyleBackColor = true;
            this.cbAutoScaleObjects.CheckedChanged += new System.EventHandler(this.cbAutoScaleObjects_CheckedChanged);
            // 
            // textVisioData
            // 
            this.textVisioData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textVisioData.Location = new System.Drawing.Point(178, 120);
            this.textVisioData.Multiline = true;
            this.textVisioData.Name = "textVisioData";
            this.textVisioData.ReadOnly = true;
            this.textVisioData.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.textVisioData.Size = new System.Drawing.Size(992, 97);
            this.textVisioData.TabIndex = 12;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(12, 125);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(141, 17);
            this.label12.TabIndex = 11;
            this.label12.Text = "Selected Information:";
            // 
            // buttonImportVisioToFacility
            // 
            this.buttonImportVisioToFacility.Location = new System.Drawing.Point(178, 237);
            this.buttonImportVisioToFacility.Name = "buttonImportVisioToFacility";
            this.buttonImportVisioToFacility.Size = new System.Drawing.Size(216, 35);
            this.buttonImportVisioToFacility.TabIndex = 10;
            this.buttonImportVisioToFacility.Text = "Import to Facility";
            this.toolTip1.SetToolTip(this.buttonImportVisioToFacility, "Import to Simio Facility using the Transform settings below");
            this.buttonImportVisioToFacility.UseVisualStyleBackColor = true;
            this.buttonImportVisioToFacility.Click += new System.EventHandler(this.buttonImportVisioToFacility_Click);
            // 
            // comboVisioPages
            // 
            this.comboVisioPages.FormattingEnabled = true;
            this.comboVisioPages.Location = new System.Drawing.Point(178, 81);
            this.comboVisioPages.Name = "comboVisioPages";
            this.comboVisioPages.Size = new System.Drawing.Size(584, 24);
            this.comboVisioPages.TabIndex = 9;
            this.comboVisioPages.SelectedIndexChanged += new System.EventHandler(this.comboVisioPages_SelectedIndexChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(12, 84);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(118, 17);
            this.label11.TabIndex = 8;
            this.label11.Text = "Select Visio Page";
            // 
            // buttonSelectVisioFile
            // 
            this.buttonSelectVisioFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSelectVisioFile.Location = new System.Drawing.Point(1176, 39);
            this.buttonSelectVisioFile.Name = "buttonSelectVisioFile";
            this.buttonSelectVisioFile.Size = new System.Drawing.Size(60, 25);
            this.buttonSelectVisioFile.TabIndex = 7;
            this.buttonSelectVisioFile.Text = "...";
            this.buttonSelectVisioFile.UseVisualStyleBackColor = true;
            this.buttonSelectVisioFile.Click += new System.EventHandler(this.buttonVisioToDsGetVisioFile_Click);
            // 
            // textVisioFilePath
            // 
            this.textVisioFilePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textVisioFilePath.Location = new System.Drawing.Point(178, 39);
            this.textVisioFilePath.Name = "textVisioFilePath";
            this.textVisioFilePath.ReadOnly = true;
            this.textVisioFilePath.Size = new System.Drawing.Size(992, 22);
            this.textVisioFilePath.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 38);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(107, 17);
            this.label4.TabIndex = 6;
            this.label4.Text = "Select Visio File";
            // 
            // tabPreview
            // 
            this.tabPreview.Controls.Add(this.picPreviewDiagram);
            this.tabPreview.Controls.Add(this.panel1);
            this.tabPreview.Location = new System.Drawing.Point(4, 25);
            this.tabPreview.Name = "tabPreview";
            this.tabPreview.Size = new System.Drawing.Size(1258, 682);
            this.tabPreview.TabIndex = 3;
            this.tabPreview.Text = "Preview";
            this.tabPreview.UseVisualStyleBackColor = true;
            // 
            // picPreviewDiagram
            // 
            this.picPreviewDiagram.BackColor = System.Drawing.SystemColors.ButtonFace;
            this.picPreviewDiagram.Dock = System.Windows.Forms.DockStyle.Fill;
            this.picPreviewDiagram.Location = new System.Drawing.Point(0, 114);
            this.picPreviewDiagram.Name = "picPreviewDiagram";
            this.picPreviewDiagram.Size = new System.Drawing.Size(1258, 568);
            this.picPreviewDiagram.TabIndex = 1;
            this.picPreviewDiagram.TabStop = false;
            this.picPreviewDiagram.Paint += new System.Windows.Forms.PaintEventHandler(this.picPreview_Paint);
            this.picPreviewDiagram.MouseMove += new System.Windows.Forms.MouseEventHandler(this.picPreviewDiagram_MouseMove);
            this.picPreviewDiagram.Resize += new System.EventHandler(this.picPreviewDiagram_Resize);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.textPreviewInfo);
            this.panel1.Controls.Add(this.labelPreviewInfo);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.labelPreviewMouse);
            this.panel1.Controls.Add(this.textPreviewMouse);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1258, 114);
            this.panel1.TabIndex = 0;
            // 
            // textPreviewInfo
            // 
            this.textPreviewInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textPreviewInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textPreviewInfo.Location = new System.Drawing.Point(187, 44);
            this.textPreviewInfo.Name = "textPreviewInfo";
            this.textPreviewInfo.ReadOnly = true;
            this.textPreviewInfo.Size = new System.Drawing.Size(1054, 24);
            this.textPreviewInfo.TabIndex = 16;
            // 
            // labelPreviewInfo
            // 
            this.labelPreviewInfo.AutoSize = true;
            this.labelPreviewInfo.Location = new System.Drawing.Point(7, 44);
            this.labelPreviewInfo.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelPreviewInfo.Name = "labelPreviewInfo";
            this.labelPreviewInfo.Size = new System.Drawing.Size(139, 17);
            this.labelPreviewInfo.TabIndex = 15;
            this.labelPreviewInfo.Text = "Diagram Information:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 16);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(442, 17);
            this.label2.TabIndex = 14;
            this.label2.Text = "A rough previes of the drawing objects contained in the SDX DataSet";
            // 
            // labelPreviewMouse
            // 
            this.labelPreviewMouse.AutoSize = true;
            this.labelPreviewMouse.Location = new System.Drawing.Point(7, 89);
            this.labelPreviewMouse.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelPreviewMouse.Name = "labelPreviewMouse";
            this.labelPreviewMouse.Size = new System.Drawing.Size(112, 17);
            this.labelPreviewMouse.TabIndex = 13;
            this.labelPreviewMouse.Text = "Mouse Location:";
            // 
            // textPreviewMouse
            // 
            this.textPreviewMouse.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textPreviewMouse.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textPreviewMouse.Location = new System.Drawing.Point(187, 84);
            this.textPreviewMouse.Name = "textPreviewMouse";
            this.textPreviewMouse.ReadOnly = true;
            this.textPreviewMouse.Size = new System.Drawing.Size(657, 24);
            this.textPreviewMouse.TabIndex = 12;
            // 
            // tabLogs
            // 
            this.tabLogs.Controls.Add(this.textLogs);
            this.tabLogs.Location = new System.Drawing.Point(4, 25);
            this.tabLogs.Name = "tabLogs";
            this.tabLogs.Size = new System.Drawing.Size(1258, 682);
            this.tabLogs.TabIndex = 2;
            this.tabLogs.Text = "Logs";
            this.tabLogs.UseVisualStyleBackColor = true;
            // 
            // textLogs
            // 
            this.textLogs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textLogs.Location = new System.Drawing.Point(0, 0);
            this.textLogs.Multiline = true;
            this.textLogs.Name = "textLogs";
            this.textLogs.ReadOnly = true;
            this.textLogs.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textLogs.Size = new System.Drawing.Size(1258, 682);
            this.textLogs.TabIndex = 0;
            this.textLogs.Text = "(No logs yet...)";
            this.textLogs.TextChanged += new System.EventHandler(this.textLogs_TextChanged);
            // 
            // timerLogs
            // 
            this.timerLogs.Interval = 1000;
            this.timerLogs.Tick += new System.EventHandler(this.timerLogs_Tick);
            // 
            // FormVisio
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1266, 764);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FormVisio";
            this.Text = "Visio Interface";
            this.Load += new System.EventHandler(this.FormVisio_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabConvertVisioFile.ResumeLayout(false);
            this.groupDisplayFacilityView.ResumeLayout(false);
            this.groupDisplayFacilityView.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.updnScaleDrawingZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.updnScaleDrawingY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.updnScaleObjectsZ)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.updnScaleObjectsY)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.updnSimioZOrigin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.updnScaleObjectsX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.updnScaleDrawingX)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.updnSimioYOrigin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.updnSimioXOrigin)).EndInit();
            this.groupVisioToDataSet.ResumeLayout(false);
            this.groupVisioToDataSet.PerformLayout();
            this.tabPreview.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picPreviewDiagram)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabLogs.ResumeLayout(false);
            this.tabLogs.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel labelStatus;
        private System.Windows.Forms.ToolStripMenuItem actionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importToSimioToolStripMenuItem;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabConvertVisioFile;
        private System.Windows.Forms.GroupBox groupVisioToDataSet;
        private System.Windows.Forms.TabPage tabLogs;
        private System.Windows.Forms.TextBox textLogs;
        private System.Windows.Forms.Button buttonSelectVisioFile;
        private System.Windows.Forms.TextBox textVisioFilePath;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button buttonImportVisioToFacility;
        private System.Windows.Forms.ComboBox comboVisioPages;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textVisioData;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.GroupBox groupDisplayFacilityView;
        private System.Windows.Forms.NumericUpDown updnScaleDrawingX;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown updnSimioYOrigin;
        private System.Windows.Forms.NumericUpDown updnSimioXOrigin;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TabPage tabPreview;
        private System.Windows.Forms.PictureBox picPreviewDiagram;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label labelPreviewMouse;
        private System.Windows.Forms.TextBox textPreviewMouse;
        private System.Windows.Forms.Timer timerLogs;
        private System.Windows.Forms.TextBox textPreviewInfo;
        private System.Windows.Forms.Label labelPreviewInfo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown updnSimioZOrigin;
        private System.Windows.Forms.NumericUpDown updnScaleObjectsX;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.NumericUpDown updnScaleObjectsZ;
        private System.Windows.Forms.NumericUpDown updnScaleObjectsY;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.NumericUpDown updnScaleDrawingZ;
        private System.Windows.Forms.NumericUpDown updnScaleDrawingY;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.CheckBox cbAutoScaleObjects;
    }
}