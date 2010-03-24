namespace PobicosForms
{
    partial class Form1
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

        #region Windows Form Designer generated arg2

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the arg2 editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.ączenieToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.połączToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rozłączToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.daneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zamknijToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.modelBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.oknoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.kaskadowoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.horyzontalnieToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wertykalnieToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ułóżIkonyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wczytajToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.modelBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ączenieToolStripMenuItem,
            this.daneToolStripMenuItem,
            this.oknoToolStripMenuItem,
            this.zamknijToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(601, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // ączenieToolStripMenuItem
            // 
            this.ączenieToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.połączToolStripMenuItem,
            this.rozłączToolStripMenuItem});
            this.ączenieToolStripMenuItem.Name = "ączenieToolStripMenuItem";
            this.ączenieToolStripMenuItem.Size = new System.Drawing.Size(75, 20);
            this.ączenieToolStripMenuItem.Text = "Połączenie";
            // 
            // połączToolStripMenuItem
            // 
            this.połączToolStripMenuItem.Name = "połączToolStripMenuItem";
            this.połączToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.połączToolStripMenuItem.Text = "Połącz";
            this.połączToolStripMenuItem.Click += new System.EventHandler(this.połączToolStripMenuItem_Click);
            // 
            // rozłączToolStripMenuItem
            // 
            this.rozłączToolStripMenuItem.Name = "rozłączToolStripMenuItem";
            this.rozłączToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.rozłączToolStripMenuItem.Text = "Rozłącz";
            this.rozłączToolStripMenuItem.Click += new System.EventHandler(this.rozłączToolStripMenuItem_Click);
            // 
            // daneToolStripMenuItem
            // 
            this.daneToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.wczytajToolStripMenuItem1});
            this.daneToolStripMenuItem.Name = "daneToolStripMenuItem";
            this.daneToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.daneToolStripMenuItem.Text = "Dane";
            // 
            // zamknijToolStripMenuItem
            // 
            this.zamknijToolStripMenuItem.Name = "zamknijToolStripMenuItem";
            this.zamknijToolStripMenuItem.Size = new System.Drawing.Size(62, 20);
            this.zamknijToolStripMenuItem.Text = "Zamknij";
            this.zamknijToolStripMenuItem.Click += new System.EventHandler(this.zamknijToolStripMenuItem_Click);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Left;
            this.richTextBox1.Location = new System.Drawing.Point(0, 24);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(242, 299);
            this.richTextBox1.TabIndex = 3;
            this.richTextBox1.Text = "";
            // 
            // modelBindingSource
            // 
            this.modelBindingSource.DataSource = typeof(PobicosLibrary.Model);
            // 
            // oknoToolStripMenuItem
            // 
            this.oknoToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.kaskadowoToolStripMenuItem,
            this.horyzontalnieToolStripMenuItem,
            this.wertykalnieToolStripMenuItem,
            this.ułóżIkonyToolStripMenuItem});
            this.oknoToolStripMenuItem.Name = "oknoToolStripMenuItem";
            this.oknoToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.oknoToolStripMenuItem.Text = "Okno";
            // 
            // kaskadowoToolStripMenuItem
            // 
            this.kaskadowoToolStripMenuItem.Name = "kaskadowoToolStripMenuItem";
            this.kaskadowoToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.kaskadowoToolStripMenuItem.Text = "Kaskadowo";
            this.kaskadowoToolStripMenuItem.Click += new System.EventHandler(this.kaskadowoToolStripMenuItem_Click);
            // 
            // horyzontalnieToolStripMenuItem
            // 
            this.horyzontalnieToolStripMenuItem.Name = "horyzontalnieToolStripMenuItem";
            this.horyzontalnieToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.horyzontalnieToolStripMenuItem.Text = "Horyzontalnie";
            this.horyzontalnieToolStripMenuItem.Click += new System.EventHandler(this.horyzontalnieToolStripMenuItem_Click);
            // 
            // wertykalnieToolStripMenuItem
            // 
            this.wertykalnieToolStripMenuItem.Name = "wertykalnieToolStripMenuItem";
            this.wertykalnieToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.wertykalnieToolStripMenuItem.Text = "Wertykalnie";
            this.wertykalnieToolStripMenuItem.Click += new System.EventHandler(this.wertykalnieToolStripMenuItem_Click);
            // 
            // ułóżIkonyToolStripMenuItem
            // 
            this.ułóżIkonyToolStripMenuItem.Name = "ułóżIkonyToolStripMenuItem";
            this.ułóżIkonyToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.ułóżIkonyToolStripMenuItem.Text = "Ułóż Ikony";
            this.ułóżIkonyToolStripMenuItem.Click += new System.EventHandler(this.ułóżIkonyToolStripMenuItem_Click);
            // 
            // wczytajToolStripMenuItem1
            // 
            this.wczytajToolStripMenuItem1.Name = "wczytajToolStripMenuItem1";
            this.wczytajToolStripMenuItem1.Size = new System.Drawing.Size(152, 22);
            this.wczytajToolStripMenuItem1.Text = "Wczytaj";
            this.wczytajToolStripMenuItem1.Click += new System.EventHandler(this.wczytajToolStripMenuItem_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(601, 323);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.menuStrip1);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Pobicos";
            this.TextChanged += new System.EventHandler(this.Form1_TextChanged);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.modelBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem ączenieToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem połączToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rozłączToolStripMenuItem;
        private System.Windows.Forms.BindingSource modelBindingSource;
        private System.Windows.Forms.ToolStripMenuItem daneToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zamknijToolStripMenuItem;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.ToolStripMenuItem oknoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem kaskadowoToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem horyzontalnieToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem wertykalnieToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ułóżIkonyToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem wczytajToolStripMenuItem1;
    }
}

