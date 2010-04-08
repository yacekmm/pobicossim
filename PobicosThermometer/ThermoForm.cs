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
                    tempTextBox.Text = TempTrackBar.Value.ToString();
                    Binding g = TempTrackBar.DataBindings.Add("Value", _model.Definition, "result.value");
                    
                   
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

        public void Instruction(string instruction, string callID, string param)
        {
            InstructionsList instr = (InstructionsList)Enum.Parse(typeof(InstructionsList), instruction);
            if (instr.Equals(InstructionsList.pongiGetTemp))
            {
                client.InstructionReturn((IPobicosModel)this.Model, callID, Model.Definition.Tables["result"].Rows[0]["value"].ToString());
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
                client.Disconnect(false);
        }

        private void ThermoForm_Load(object sender, EventArgs e)
        {
            Initialize();
            
        }

        private void TempTrackBar_ValueChanged(object sender, EventArgs e)
        {
            tempTextBox.Text = TempTrackBar.Value.ToString();
            foreach (Binding b in TempTrackBar.DataBindings)
            {
                b.WriteValue();
            }
        }

        #region IPobicosView Members




        #endregion
    }
}
