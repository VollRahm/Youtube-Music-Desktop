namespace Youtube_Music.Forms
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            this._label1 = new System.Windows.Forms.Label();
            this.outDvcCb = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // _label1
            // 
            this._label1.AutoSize = true;
            this._label1.Location = new System.Drawing.Point(12, 20);
            this._label1.Name = "_label1";
            this._label1.Size = new System.Drawing.Size(79, 13);
            this._label1.TabIndex = 0;
            this._label1.Text = "Output Device:";
            // 
            // outDvcCb
            // 
            this.outDvcCb.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.outDvcCb.FormattingEnabled = true;
            this.outDvcCb.Location = new System.Drawing.Point(97, 17);
            this.outDvcCb.Name = "outDvcCb";
            this.outDvcCb.Size = new System.Drawing.Size(319, 21);
            this.outDvcCb.TabIndex = 1;
            this.outDvcCb.SelectedIndexChanged += new System.EventHandler(this.outDvcCb_SelectedIndexChanged);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(428, 73);
            this.Controls.Add(this.outDvcCb);
            this.Controls.Add(this._label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SettingsForm";
            this.Text = "Settings";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingsForm_FormClosing);
            this.Shown += new System.EventHandler(this.SettingsForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _label1;
        private System.Windows.Forms.ComboBox outDvcCb;
    }
}