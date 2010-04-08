﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PobicosLibrary;

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
                    foreach (IPobicosModel model in AdminTools.readConfiguration(ofd.FileName))
                    {
                        client.RegisterModel(model);
                    }

                }
            }
        }

        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {
            client.Type = Const.NODE;
            client.Connect();
            client.commandReceived += new Client.CommandReceivedEventHandler(client_commandReceived);
        }

        void client_commandReceived(object sender, CommandArgs args)
        {
            if (args.command == Const.INSTR_RET)            
                MessageBox.Show("Temperatura wynosi: " + args.arg2);
        }

        private void turnOnButton_Click(object sender, EventArgs e)
        {
            foreach (IPobicosModel model in client.models)
            {
                //   AdminTools.PrintDataSet(model.Definition);
                try
                {
                    foreach (DataRow dr in model.Definition.Tables["definition"].Rows)
                    {
                        if (dr["definition_Text"].ToString().Contains("SwitchOn"))
                        {
                            client.Instruction(model, InstructionsList.pongiSwitchOn,"677", null);
                        }
                    }
                }
                catch (Exception)
                {
                    // ten model tego nie ma 
                }

            }
        }

        private void turnOffButton_Click(object sender, EventArgs e)
        {
            foreach (IPobicosModel model in client.models)
            {
                try
                {
                    foreach (DataRow dr in model.Definition.Tables["definition"].Rows)
                    {
                        if (dr["definition_Text"].ToString().Contains("SwitchOff"))
                        {
                            client.Instruction(model, InstructionsList.pongiSwitchOff,"33", null);
                        }
                    }
                }
                catch (Exception)
                {
                    // ten model tego nie ma
                }


            }
        }

        private void MiniMWForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (client != null)
                client.Disconnect(true);
        }

        private void tempButton_Click(object sender, EventArgs e)
        {

            foreach (IPobicosModel model in client.models)
            {
                AdminTools.PrintDataSet(model.Definition);
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
                                    client.Instruction(model, InstructionsList.pongiGetTemp,"44", null);
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        //ten model tego nie ma
                    }
                }

            }
        }
    }
}
