namespace SdxHarness
{
    partial class DialogVisioPrompt
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
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.comboSelectPage = new System.Windows.Forms.ComboBox();
            this.groupPageInfo = new System.Windows.Forms.GroupBox();
            this.textPreviewFolder = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.buttonSelectVisioFile = new System.Windows.Forms.Button();
            this.textPageInfo = new System.Windows.Forms.TextBox();
            this.groupPageInfo.SuspendLayout();
            this.SuspendLayout();
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.Location = new System.Drawing.Point(794, 478);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(110, 38);
            this.buttonCancel.TabIndex = 0;
            this.buttonCancel.Text = "Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOk
            // 
            this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOk.Location = new System.Drawing.Point(658, 478);
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.Size = new System.Drawing.Size(104, 38);
            this.buttonOk.TabIndex = 1;
            this.buttonOk.Text = "OK";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(21, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(96, 17);
            this.label1.TabIndex = 2;
            this.label1.Text = "Select a Page";
            // 
            // comboSelectPage
            // 
            this.comboSelectPage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboSelectPage.FormattingEnabled = true;
            this.comboSelectPage.Location = new System.Drawing.Point(140, 47);
            this.comboSelectPage.Name = "comboSelectPage";
            this.comboSelectPage.Size = new System.Drawing.Size(535, 24);
            this.comboSelectPage.TabIndex = 3;
            this.comboSelectPage.SelectedIndexChanged += new System.EventHandler(this.comboSelectPage_SelectedIndexChanged);
            // 
            // groupPageInfo
            // 
            this.groupPageInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupPageInfo.Controls.Add(this.textPageInfo);
            this.groupPageInfo.Location = new System.Drawing.Point(24, 99);
            this.groupPageInfo.Name = "groupPageInfo";
            this.groupPageInfo.Size = new System.Drawing.Size(880, 369);
            this.groupPageInfo.TabIndex = 4;
            this.groupPageInfo.TabStop = false;
            this.groupPageInfo.Text = "Visio/Page Information";
            this.groupPageInfo.Enter += new System.EventHandler(this.groupPageInfo_Enter);
            // 
            // textPreviewFolder
            // 
            this.textPreviewFolder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textPreviewFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textPreviewFolder.Location = new System.Drawing.Point(140, 17);
            this.textPreviewFolder.Name = "textPreviewFolder";
            this.textPreviewFolder.ReadOnly = true;
            this.textPreviewFolder.Size = new System.Drawing.Size(677, 24);
            this.textPreviewFolder.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(21, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 17);
            this.label2.TabIndex = 6;
            this.label2.Text = "VIsio File";
            // 
            // buttonSelectVisioFile
            // 
            this.buttonSelectVisioFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSelectVisioFile.Location = new System.Drawing.Point(823, 12);
            this.buttonSelectVisioFile.Name = "buttonSelectVisioFile";
            this.buttonSelectVisioFile.Size = new System.Drawing.Size(57, 32);
            this.buttonSelectVisioFile.TabIndex = 7;
            this.buttonSelectVisioFile.Text = "...";
            this.buttonSelectVisioFile.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.buttonSelectVisioFile.UseVisualStyleBackColor = true;
            this.buttonSelectVisioFile.Click += new System.EventHandler(this.buttonSelectVisioFile_Click);
            // 
            // textPageInfo
            // 
            this.textPageInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textPageInfo.Location = new System.Drawing.Point(23, 37);
            this.textPageInfo.Multiline = true;
            this.textPageInfo.Name = "textPageInfo";
            this.textPageInfo.ReadOnly = true;
            this.textPageInfo.Size = new System.Drawing.Size(848, 319);
            this.textPageInfo.TabIndex = 0;
            // 
            // DialogVisioPrompt
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(916, 528);
            this.Controls.Add(this.buttonSelectVisioFile);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textPreviewFolder);
            this.Controls.Add(this.groupPageInfo);
            this.Controls.Add(this.comboSelectPage);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.buttonCancel);
            this.Name = "DialogVisioPrompt";
            this.Text = "Select a Visio Page";
            this.Load += new System.EventHandler(this.DialogVisioPrompt_Load);
            this.groupPageInfo.ResumeLayout(false);
            this.groupPageInfo.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboSelectPage;
        private System.Windows.Forms.GroupBox groupPageInfo;
        private System.Windows.Forms.TextBox textPreviewFolder;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button buttonSelectVisioFile;
        private System.Windows.Forms.TextBox textPageInfo;
    }
}