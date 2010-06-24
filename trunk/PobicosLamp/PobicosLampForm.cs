using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PobicosLibrary;
using System.Collections;
using System.Net.Sockets;

namespace PobicosLamp
{
    public partial class PobicosLampForm : Form, PobicosLibrary.IPobicosView
    {
        private bool status = false;
        public PobicosLampForm()
        {
            InitializeComponent();
        }

        #region IPobicosView Members
        private Client client;
        private IPobicosModel _model;

        private void Initialize()
        {
            this.TopMost = true;
            client = new Client();

            List<IPobicosModel> models = PobicosLibrary.AdminTools.readConfiguration("lamp.xml");
            foreach (PobicosLibrary.Model model in models)
            {
                model.RegisterObserver(this);
                client.RegisterModel(model);
                this.Model = (IPobicosModel)models[0];

            }
            if (!client.Connect())
            {
                MessageBox.Show("Connection error");
                Application.Exit();
            }
        }
        public void Update(PobicosLibrary.IModel model)
        {
            throw new NotImplementedException();
        }
        private delegate void instructionDelegate(String instructionList, string callID, string param);
        public void Instruction(String instructionLabel, string callID, string param)
        {
            InstructionsList instr = (InstructionsList)Enum.Parse(typeof(InstructionsList), instructionLabel);
            if (pictureBox1.InvokeRequired)
                pictureBox1.Invoke(new instructionDelegate(this.Instruction), instructionLabel, callID, param);
            else
            {
                if (instr.Equals(InstructionsList.SwitchOn))
                {
                    status = true;
                    this.pictureBox1.Image = global::PobicosLamp.Properties.Resources.lampOn;


                }
                if (instr.Equals(InstructionsList.SwitchOff))
                {
                    status = false;
                    this.pictureBox1.Image = global::PobicosLamp.Properties.Resources.lampOff;
                }
            }
        }

        public void EventReturn(string callID, string returnValue)
        {
            throw new NotImplementedException();
        }

        public IModel Model
        {
            get
            {
                return _model;
            }
            set
            {
                _model = (IPobicosModel)value;
            }
        }

        #endregion

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (!status)
            {
                this.pictureBox1.Image = global::PobicosLamp.Properties.Resources.lampOn;
            }
            else
            {
                this.pictureBox1.Image = global::PobicosLamp.Properties.Resources.lampOff;
            }
            status = !status;
            client.Event(this, EventsList.ponge_originated_event_switch_originated_event,"55", null);
        }


        private void PobicosLampForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (client != null)
                client.Disconnect();
        }

        private void PobicosLampForm_Load(object sender, EventArgs e)
        {
            Initialize();
        }

        #region IPobicosView Members


        public void InstructionReturn(string callID, string returnValue)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
