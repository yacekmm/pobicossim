namespace PobicosThermometer
{
    partial class ThermoForm
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
            this.tempTextBox = new System.Windows.Forms.TextBox();
            this.tempLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // tempTextBox
            // 
            this.tempTextBox.Location = new System.Drawing.Point(172, 12);
            this.tempTextBox.Name = "tempTextBox";
            this.tempTextBox.Size = new System.Drawing.Size(100, 20);
            this.tempTextBox.TabIndex = 0;
            // 
            // tempLabel
            // 
            this.tempLabel.AutoSize = true;
            this.tempLabel.Location = new System.Drawing.Point(96, 12);
            this.tempLabel.Name = "tempLabel";
            this.tempLabel.Size = new System.Drawing.Size(70, 13);
            this.tempLabel.TabIndex = 1;
            this.tempLabel.Text = "Temperature:";
            // 
            // ThermoForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 44);
            this.Controls.Add(this.tempLabel);
            this.Controls.Add(this.tempTextBox);
            this.MaximumSize = new System.Drawing.Size(300, 82);
            this.MinimumSize = new System.Drawing.Size(300, 82);
            this.Name = "ThermoForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Thermometer";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.ThermoForm_Load);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ThermoForm_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tempTextBox;
        private System.Windows.Forms.Label tempLabel;
    }
}

