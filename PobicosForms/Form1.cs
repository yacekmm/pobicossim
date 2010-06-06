using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;
using PobicosLibrary;

namespace PobicosForms
{
    public partial class Form1 : Form
    {
        public static Client  client =  new Client();
        
        public Form1()
        {
            InitializeComponent();
           // AdminTools.eventLog.EntryWritten += new System.Diagnostics.EntryWrittenEventHandler(eventLog_EntryWritten);
        }

        void eventLog_EntryWritten(object sender, System.Diagnostics.EntryWrittenEventArgs e)
        {
            if (richTextBox1.InvokeRequired)
            {
                richTextBox1.Invoke(new addTextDelegate(addTextCallback), new object[] { e.Entry.Message });
            }
            else
            {
                addTextCallback(e.Entry.Message);
            }



        }
        private delegate void addTextDelegate(string text);

        private void addTextCallback(string text)
        {
            richTextBox1.Text += text + "\n";
        }



        private void połączToolStripMenuItem_Click(object sender, EventArgs e)
        {
            client.Connect();
            
        }

        private void wczytajToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    List<IPobicosModel> models = PobicosLibrary.AdminTools.readConfiguration(ofd.FileName);
                    foreach (PobicosLibrary.Model model in models)
                    {
                        client.RegisterModel(model);
                       // this.
                        ViewForm viewForm = new ViewForm(model);
                        
                        model.AddObserver(viewForm);
                        viewForm.MdiParent = this;
                        viewForm.Text = model.Name;
                        

                        viewForm.Show();

                    }

                }
            }
        }

        private void rozłączToolStripMenuItem_Click(object sender, EventArgs e)
        {

            client.Disconnect();

        }

        private void Form1_TextChanged(object sender, EventArgs e)
        {
            MessageBox.Show(sender.ToString());
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            MessageBox.Show(sender.ToString());

        }

        private void zamknijToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        #region IPobicosView Members

        public void Update(IPobicosModel model)
        {

        }

        #endregion

        private void kaskadowoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.LayoutMdi(MdiLayout.Cascade);
        }

        private void horyzontalnieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }

        private void wertykalnieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        private void ułóżIkonyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }
    }
}
