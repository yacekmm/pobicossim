using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PobicosLibrary;

namespace PobicosThermometer
{
    public partial class ThermoForm : Form,IPobicosView
    {
        private Client client;
        
        public ThermoForm()
        {
            InitializeComponent();

        }
        private void Initialize()
        {
            List<IPobicosModel> models;           
            client = new Client();
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {

                    models = AdminTools.readConfiguration(ofd.FileName);

                    if (models.Count != 1)
                    {
                        MessageBox.Show("Zła liczba modeli, zamknięcie aplikacji");
                        Application.Exit();
                    }
                    models[0].AddObserver(this);
                    client.RegisterModel(models[0]);
                    if (!client.Connect())
                    {
                        MessageBox.Show(" Connection error! ");
                        Application.Exit();
                    }
                   tempTextBox.Text = "20";
                   // tempTextBox.BindingContext

                    //CurrencyManager c = (CurrencyManager)b;
                }
                

            }
        }

        #region IPobicosView Members

        public void EventReturn(string callID, string returnValue)
        {
            throw new NotImplementedException();
        }

        public void Instruction(InstructionsList instruction, string callID, string param)
        {
            if (instruction.Equals(InstructionsList.pongiGetTemp))
            {
                client.InstructionReturn((IPobicosModel)this.Model, callID, tempTextBox.Text); //Model.Definition.Tables["result"].Rows[0]["value"].ToString());
            }
        }

        #endregion

        #region IView Members

        public void Update(IModel model)
        {
            throw new NotImplementedException();
        }
        IPobicosModel _model;
        public IModel Model
        {
            get
            {
                return _model;
            }
            set
            {
                _model = (IPobicosModel) value;
            }
        }

        #endregion

        private void ThermoForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (client != null)
                client.Disconnect();
        }

        private void ThermoForm_Load(object sender, EventArgs e)
        {
            Initialize();
            
        }

        #region IPobicosView Members




        #endregion
    }
}
