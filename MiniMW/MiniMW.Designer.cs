namespace MiniMW
{
    partial class MiniMWForm
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
            this.menuStripMain = new System.Windows.Forms.MenuStrip();
            this.konfiguracjaToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wczytajToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ącToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lampPanel = new System.Windows.Forms.Panel();
            this.turnOffButton = new System.Windows.Forms.Button();
            this.turnOnButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.tempButton = new System.Windows.Forms.Button();
            this.menuStripMain.SuspendLayout();
            this.lampPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStripMain
            // 
            this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.konfiguracjaToolStripMenuItem,
            this.ącToolStripMenuItem});
            this.menuStripMain.Location = new System.Drawing.Point(0, 0);
            this.menuStripMain.Name = "menuStripMain";
            this.menuStripMain.Size = new System.Drawing.Size(453, 24);
            this.menuStripMain.TabIndex = 0;
            this.menuStripMain.Text = "menuStrip1";
            // 
            // konfiguracjaToolStripMenuItem
            // 
            this.konfiguracjaToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.wczytajToolStripMenuItem});
            this.konfiguracjaToolStripMenuItem.Name = "konfiguracjaToolStripMenuItem";
            this.konfiguracjaToolStripMenuItem.Size = new System.Drawing.Size(55, 20);
            this.konfiguracjaToolStripMenuItem.Text = "Config";
            // 
            // wczytajToolStripMenuItem
            // 
            this.wczytajToolStripMenuItem.Name = "wczytajToolStripMenuItem";
            this.wczytajToolStripMenuItem.Size = new System.Drawing.Size(103, 22);
            this.wczytajToolStripMenuItem.Text = "Open";
            this.wczytajToolStripMenuItem.Click += new System.EventHandler(this.wczytajToolStripMenuItem_Click);
            // 
            // ącToolStripMenuItem
            // 
            this.ącToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectToolStripMenuItem});
            this.ącToolStripMenuItem.Name = "ącToolStripMenuItem";
            this.ącToolStripMenuItem.Size = new System.Drawing.Size(81, 20);
            this.ącToolStripMenuItem.Text = "Connection";
            // 
            // connectToolStripMenuItem
            // 
            this.connectToolStripMenuItem.Name = "connectToolStripMenuItem";
            this.connectToolStripMenuItem.Size = new System.Drawing.Size(119, 22);
            this.connectToolStripMenuItem.Text = "Connect";
            this.connectToolStripMenuItem.Click += new System.EventHandler(this.connectToolStripMenuItem_Click);
            // 
            // lampPanel
            // 
            this.lampPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)));
            this.lampPanel.Controls.Add(this.turnOffButton);
            this.lampPanel.Controls.Add(this.turnOnButton);
            this.lampPanel.Location = new System.Drawing.Point(12, 27);
            this.lampPanel.Name = "lampPanel";
            this.lampPanel.Size = new System.Drawing.Size(210, 215);
            this.lampPanel.TabIndex = 1;
            // 
            // turnOffButton
            // 
            this.turnOffButton.Location = new System.Drawing.Point(68, 32);
            this.turnOffButton.Name = "turnOffButton";
            this.turnOffButton.Size = new System.Drawing.Size(75, 23);
            this.turnOffButton.TabIndex = 1;
            this.turnOffButton.Text = "Turn off";
            this.turnOffButton.UseVisualStyleBackColor = true;
            this.turnOffButton.Click += new System.EventHandler(this.turnOffButton_Click);
            // 
            // turnOnButton
            // 
            this.turnOnButton.Location = new System.Drawing.Point(68, 3);
            this.turnOnButton.Name = "turnOnButton";
            this.turnOnButton.Size = new System.Drawing.Size(75, 23);
            this.turnOnButton.TabIndex = 0;
            this.turnOnButton.Text = "Turn on";
            this.turnOnButton.UseVisualStyleBackColor = true;
            this.turnOnButton.Click += new System.EventHandler(this.turnOnButton_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tempButton);
            this.panel1.Location = new System.Drawing.Point(228, 27);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(210, 215);
            this.panel1.TabIndex = 2;
            // 
            // tempButton
            // 
            this.tempButton.Location = new System.Drawing.Point(73, 3);
            this.tempButton.Name = "tempButton";
            this.tempButton.Size = new System.Drawing.Size(75, 23);
            this.tempButton.TabIndex = 0;
            this.tempButton.Text = "Temperature";
            this.tempButton.UseVisualStyleBackColor = true;
            this.tempButton.Click += new System.EventHandler(this.tempButton_Click);
            // 
            // MiniMWForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(453, 254);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lampPanel);
            this.Controls.Add(this.menuStripMain);
            this.MainMenuStrip = this.menuStripMain;
            this.Name = "MiniMWForm";
            this.Text = "MiniMW";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MiniMWForm_FormClosed);
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            this.lampPanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.Panel lampPanel;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripMenuItem konfiguracjaToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem wczytajToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ącToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectToolStripMenuItem;
        private System.Windows.Forms.Button turnOffButton;
        private System.Windows.Forms.Button turnOnButton;
        private System.Windows.Forms.Button tempButton;
    }
}

