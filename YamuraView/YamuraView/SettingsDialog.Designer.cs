namespace YamuraView
{
    partial class SettingsDialog : Form
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            lblFolder = new Label();
            txtFolder = new TextBox();
            btnBrowseFolder = new Button();
            lblConfig = new Label();
            txtConfig = new TextBox();
            btnBrowseConfig = new Button();
            lblColors = new Label();
            btnColor1 = new Button();
            btnColor2 = new Button();
            btnColor3 = new Button();
            btnColor4 = new Button();
            btnColor5 = new Button();
            btnColor6 = new Button();
            btnColor7 = new Button();
            btnOK = new Button();
            btnCancel = new Button();
            SuspendLayout();
            //
            // lblFolder
            //
            lblFolder.AutoSize = true;
            lblFolder.Location = new Point(12, 18);
            lblFolder.Name = "lblFolder";
            lblFolder.Size = new Size(92, 15);
            lblFolder.TabIndex = 0;
            lblFolder.Text = "Autoload Folder";
            //
            // txtFolder
            //
            txtFolder.Location = new Point(140, 14);
            txtFolder.Name = "txtFolder";
            txtFolder.Size = new Size(200, 23);
            txtFolder.TabIndex = 0;
            //
            // btnBrowseFolder
            //
            btnBrowseFolder.Location = new Point(348, 13);
            btnBrowseFolder.Name = "btnBrowseFolder";
            btnBrowseFolder.Size = new Size(30, 25);
            btnBrowseFolder.TabIndex = 1;
            btnBrowseFolder.Text = "...";
            btnBrowseFolder.UseVisualStyleBackColor = true;
            btnBrowseFolder.Click += btnBrowseFolder_Click;
            //
            // lblConfig
            //
            lblConfig.AutoSize = true;
            lblConfig.Location = new Point(12, 50);
            lblConfig.Name = "lblConfig";
            lblConfig.Size = new Size(102, 15);
            lblConfig.TabIndex = 2;
            lblConfig.Text = "Configuration File";
            //
            // txtConfig
            //
            txtConfig.Location = new Point(140, 46);
            txtConfig.Name = "txtConfig";
            txtConfig.Size = new Size(200, 23);
            txtConfig.TabIndex = 2;
            //
            // btnBrowseConfig
            //
            btnBrowseConfig.Location = new Point(348, 45);
            btnBrowseConfig.Name = "btnBrowseConfig";
            btnBrowseConfig.Size = new Size(30, 25);
            btnBrowseConfig.TabIndex = 3;
            btnBrowseConfig.Text = "...";
            btnBrowseConfig.UseVisualStyleBackColor = true;
            btnBrowseConfig.Click += btnBrowseConfig_Click;
            //
            // lblColors
            //
            lblColors.AutoSize = true;
            lblColors.Location = new Point(12, 86);
            lblColors.Name = "lblColors";
            lblColors.Size = new Size(66, 15);
            lblColors.TabIndex = 4;
            lblColors.Text = "Run Colors";
            //
            // btnColor1
            //
            btnColor1.Location = new Point(140, 82);
            btnColor1.Name = "btnColor1";
            btnColor1.Size = new Size(28, 24);
            btnColor1.TabIndex = 5;
            btnColor1.UseVisualStyleBackColor = false;
            btnColor1.Click += btnColor_Click;
            //
            // btnColor2
            //
            btnColor2.Location = new Point(170, 82);
            btnColor2.Name = "btnColor2";
            btnColor2.Size = new Size(28, 24);
            btnColor2.TabIndex = 6;
            btnColor2.UseVisualStyleBackColor = false;
            btnColor2.Click += btnColor_Click;
            //
            // btnColor3
            //
            btnColor3.Location = new Point(200, 82);
            btnColor3.Name = "btnColor3";
            btnColor3.Size = new Size(28, 24);
            btnColor3.TabIndex = 7;
            btnColor3.UseVisualStyleBackColor = false;
            btnColor3.Click += btnColor_Click;
            //
            // btnColor4
            //
            btnColor4.Location = new Point(230, 82);
            btnColor4.Name = "btnColor4";
            btnColor4.Size = new Size(28, 24);
            btnColor4.TabIndex = 8;
            btnColor4.UseVisualStyleBackColor = false;
            btnColor4.Click += btnColor_Click;
            //
            // btnColor5
            //
            btnColor5.Location = new Point(260, 82);
            btnColor5.Name = "btnColor5";
            btnColor5.Size = new Size(28, 24);
            btnColor5.TabIndex = 9;
            btnColor5.UseVisualStyleBackColor = false;
            btnColor5.Click += btnColor_Click;
            //
            // btnColor6
            //
            btnColor6.Location = new Point(290, 82);
            btnColor6.Name = "btnColor6";
            btnColor6.Size = new Size(28, 24);
            btnColor6.TabIndex = 10;
            btnColor6.UseVisualStyleBackColor = false;
            btnColor6.Click += btnColor_Click;
            //
            // btnColor7
            //
            btnColor7.Location = new Point(320, 82);
            btnColor7.Name = "btnColor7";
            btnColor7.Size = new Size(28, 24);
            btnColor7.TabIndex = 11;
            btnColor7.UseVisualStyleBackColor = false;
            btnColor7.Click += btnColor_Click;
            //
            // btnOK
            //
            btnOK.Location = new Point(140, 118);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(80, 30);
            btnOK.TabIndex = 12;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            //
            // btnCancel
            //
            btnCancel.Location = new Point(230, 118);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(80, 30);
            btnCancel.TabIndex = 13;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            //
            // SettingsDialog
            //
            AcceptButton = btnOK;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(396, 160);
            Controls.Add(lblFolder);
            Controls.Add(txtFolder);
            Controls.Add(btnBrowseFolder);
            Controls.Add(lblConfig);
            Controls.Add(txtConfig);
            Controls.Add(btnBrowseConfig);
            Controls.Add(lblColors);
            Controls.Add(btnColor1);
            Controls.Add(btnColor2);
            Controls.Add(btnColor3);
            Controls.Add(btnColor4);
            Controls.Add(btnColor5);
            Controls.Add(btnColor6);
            Controls.Add(btnColor7);
            Controls.Add(btnOK);
            Controls.Add(btnCancel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "SettingsDialog";
            Text = "Settings";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblFolder;
        private TextBox txtFolder;
        private Button btnBrowseFolder;
        private Label lblConfig;
        private TextBox txtConfig;
        private Button btnBrowseConfig;
        private Label lblColors;
        private Button btnColor1;
        private Button btnColor2;
        private Button btnColor3;
        private Button btnColor4;
        private Button btnColor5;
        private Button btnColor6;
        private Button btnColor7;
        private Button btnOK;
        private Button btnCancel;
    }
}
