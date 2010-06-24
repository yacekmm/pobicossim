using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PobicosLibrary;
using System.Diagnostics;

namespace MiniMW
{
    public partial class MiniMWForm : Form
    {
        public static Client client = new Client();
        public MiniMWForm()
        {
            InitializeComponent();
        }

        private void wczytajToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    foreach (IPobicosModel model in AdminTools.ReadConfiguration(ofd.FileName))
                    {
                        client.RegisterModel(model);
                    }

                }
            }
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            client.Type = clientType.NODE;
            client.Connect();
            client.CommandReceived += new Client.CommandReceivedEventHandler(client_commandReceived);
        }

        void client_commandReceived(object sender, CommandArgs args)
        {
            if (args.Command == Const.INSTR_RET)            
                MessageBox.Show("Temperatura wynosi: " + args.InstructionLabel);
            if (args.Command == Const.EVENT)
            {
                Trace.TraceInformation("EVENT w MiniMW otrzymano");
               // AdminTools.eventLog.WriteEntry("EVENT w MiniMW otrzymano");
            }
        }

        private void turnOnButton_Click(object sender, EventArgs e)
        {
            foreach (IPobicosModel model in client.Models)
            {
                //   AdminTools.PrintDataSet(model.Definition);
                try
                {
                    foreach (DataRow dr in model.Definition.Tables["definition"].Rows)
                    {
                        if (dr["definition_Text"].ToString().Contains("SwitchOn"))
                        {
                            client.Instruction(model, InstructionsList.SwitchOn,"677", null);
                        }
                    }
                }
                catch (Exception exc )
                {
                    Console.WriteLine(exc.StackTrace);
                }

            }
        }

        private void turnOffButton_Click(object sender, EventArgs e)
        {
            foreach (IPobicosModel model in client.Models)
            {
                try
                {
                    foreach (DataRow dr in model.Definition.Tables["definition"].Rows)
                    {
                        if (dr["definition_Text"].ToString().Contains("SwitchOff"))
                        {
                            client.Instruction(model, InstructionsList.SwitchOff,"33", null);
                        }
                    }
                }
                catch (Exception exc)
                {
                    Console.WriteLine(exc.StackTrace);
                }


            }
        }

        private void tempButton_Click(object sender, EventArgs e)
        {

            foreach (IPobicosModel model in client.Models)
            {
                foreach (DataTable dt in model.Definition.Tables)
                {
                    try
                    {
                        if (dt.TableName.Equals("instruction") && dt.Columns[0].ColumnName.Equals("name"))
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                if (dr["name"].ToString().Contains("GetTemp"))
                                {
                                    client.Instruction(model, InstructionsList.GetTemp,"44", null);
                                }
                            }
                        }
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine(exc.StackTrace);
                    }
                }

            }
        }

        private void MiniMWForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (client != null)
                client.Disconnect();
        }
    }
}
