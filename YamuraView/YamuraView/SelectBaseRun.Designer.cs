namespace YamuraView
{
    partial class SelectBaseRun
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
            cmbSelectRun = new ComboBox();
            label1 = new Label();
            bntCancel = new Button();
            btnOK = new Button();
            SuspendLayout();
            // 
            // cmbSelectRun
            // 
            cmbSelectRun.Font = new Font("Segoe UI", 12F);
            cmbSelectRun.FormattingEnabled = true;
            cmbSelectRun.Location = new Point(92, 9);
            cmbSelectRun.Name = "cmbSelectRun";
            cmbSelectRun.Size = new Size(200, 29);
            cmbSelectRun.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 12F);
            label1.Location = new Point(12, 12);
            label1.Name = "label1";
            label1.Size = new Size(74, 21);
            label1.TabIndex = 1;
            label1.Text = "Base Run";
            // 
            // bntCancel
            // 
            bntCancel.DialogResult = DialogResult.Cancel;
            bntCancel.Location = new Point(70, 44);
            bntCancel.Name = "bntCancel";
            bntCancel.Size = new Size(75, 23);
            bntCancel.TabIndex = 2;
            bntCancel.Text = "Cancel";
            bntCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            btnOK.DialogResult = DialogResult.OK;
            btnOK.Location = new Point(158, 44);
            btnOK.Name = "btnOK";
            btnOK.Size = new Size(75, 23);
            btnOK.TabIndex = 3;
            btnOK.Text = "OK";
            btnOK.UseVisualStyleBackColor = true;
            // 
            // SelectBaseRun
            // 
            AcceptButton = btnOK;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = bntCancel;
            ClientSize = new Size(303, 74);
            Controls.Add(btnOK);
            Controls.Add(bntCancel);
            Controls.Add(label1);
            Controls.Add(cmbSelectRun);
            Name = "SelectBaseRun";
            Text = "Select Base Run";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        public ComboBox cmbSelectRun;
        private Label label1;
        private Button bntCancel;
        private Button btnOK;
    }
}