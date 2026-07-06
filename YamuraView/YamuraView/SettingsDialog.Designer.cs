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
            lblSSID = new Label();
            txtSSID = new TextBox();
            lblPassword = new Label();
            txtPassword = new TextBox();
            btnOK = new Button();
            btnCancel = new Button();
            btnTogglePasswordVis = new Button();
            lblFileZilla = new Label();
            txtFileZilla = new TextBox();
            btnBrowseFileZilla = new Button();
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
            // lblSSID
            // 
            lblSSID.AutoSize = true;
            lblSSID.Location = new Point(12, 82);
            lblSSID.Name = "lblSSID";
            lblSSID.Size = new Size(56, 15);
            lblSSID.TabIndex = 4;
            lblSSID.Text = "WiFi SSID";
            // 
            // txtSSID
            // 
            txtSSID.Location = new Point(140, 78);
            txtSSID.Name = "txtSSID";
            txtSSID.Size = new Size(238, 23);
            txtSSID.TabIndex = 4;
            // 
            // lblPassword
            // 
            lblPassword.AutoSize = true;
            lblPassword.Location = new Point(12, 114);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(83, 15);
            lblPassword.TabIndex = 5;
            lblPassword.Text = "WiFi Password";
            // 
            // txtPassword
            // 
            txtPassword.Location = new Point(140, 110);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(215, 23);
            txtPassword.TabIndex = 5;
            txtPassword.UseSystemPasswordChar = true;
            // 
            // btnTogglePasswordVis
            //
            btnTogglePasswordVis.Location = new Point(361, 110);
            btnTogglePasswordVis.Name = "btnTogglePasswordVis";
            btnTogglePasswordVis.Size = new Size(21, 23);
            btnTogglePasswordVis.TabIndex = 6;
            btnTogglePasswordVis.Text = "S";
            btnTogglePasswordVis.UseVisualStyleBackColor = true;
            btnTogglePasswordVis.Click += btnTogglePasswordVis_Click;
            //
            // lblFileZilla
            //
            lblFileZilla.AutoSize = true;
            lblFileZilla.Location = new Point(12, 146);
            lblFileZilla.Name = "lblFileZilla";
            lblFileZilla.Size = new Size(108, 15);
            lblFileZilla.TabIndex = 7;
            lblFileZilla.Text = "FileZilla Server Exe";
            //
            // txtFileZilla
            //
            txtFileZilla.Location = new Point(140, 142);
            txtFileZilla.Name = "txtFileZilla";
            txtFileZilla.Size = new Size(200, 23);
            txtFileZilla.TabIndex = 8;
            //
            // btnBrowseFileZilla
            //
            btnBrowseFileZilla.Location = new Point(348, 141);
            btnBrowseFileZilla.Name = "btnBrowseFileZilla";
            btnBrowseFileZilla.Size = new Size(30, 25);
            btnBrowseFileZilla.TabIndex = 9;
            btnBrowseFileZilla.Text = "...";
            btnBrowseFileZilla.UseVisualStyleBackColor = true;
            btnBrowseFileZilla.Click += btnBrowseFileZilla_Click;
            //
            // btnOK
            //
            btnOK.Location = new Point(140, 182);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(80, 30);
            btnOK.TabIndex = 10;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            btnOK.Click += btnOK_Click;
            //
            // btnCancel
            //
            btnCancel.Location = new Point(230, 182);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(80, 30);
            btnCancel.TabIndex = 11;
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
            ClientSize = new Size(396, 228);
            Controls.Add(btnTogglePasswordVis);
            Controls.Add(lblFolder);
            Controls.Add(txtFolder);
            Controls.Add(btnBrowseFolder);
            Controls.Add(lblConfig);
            Controls.Add(txtConfig);
            Controls.Add(btnBrowseConfig);
            Controls.Add(lblSSID);
            Controls.Add(txtSSID);
            Controls.Add(lblPassword);
            Controls.Add(txtPassword);
            Controls.Add(lblFileZilla);
            Controls.Add(txtFileZilla);
            Controls.Add(btnBrowseFileZilla);
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
        private Label lblSSID;
        private TextBox txtSSID;
        private Label lblPassword;
        private TextBox txtPassword;
        private Button btnOK;
        private Button btnCancel;
        private Button btnTogglePasswordVis;
        private Label lblFileZilla;
        private TextBox txtFileZilla;
        private Button btnBrowseFileZilla;
    }
}
