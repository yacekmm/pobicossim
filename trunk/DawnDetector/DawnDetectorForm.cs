using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using AForge.Imaging.Filters;
using PobicosLibrary;

namespace DawnDetector
{
    public partial class DawnDetector : Form, PobicosLibrary.IPobicosView
    {
        private BrightnessCorrection br = new BrightnessCorrection();
        private int lux = 0;
        private double brightness = 1;
        private Image original;
        private Client client = new Client();

        public DawnDetector()
        {
            InitializeComponent();
            initialize();

        }
        private void initialize()
        {            
            original = pictureBox1.Image;
            {
                List<IPobicosModel> models; 
                models = AdminTools.readConfiguration(@"dawnDetector.xml");

                if (models.Count != 1)
                {
                    MessageBox.Show("Zła liczba modeli, zamknięcie aplikacji");
                    Environment.Exit(1);
                }
                models[0].AddObserver(this);
                client.RegisterModel(models[0]);
                if (!client.Connect())
                {
                    MessageBox.Show(" Connection error! ");
                    Environment.Exit(1);
                }
            }
            dawnTimer.Enabled = true;
        }

        private delegate void settingBright(DateTime time);
        private void setBrightness(DateTime time)
        {
            if (!pictureBox1.InvokeRequired)
            {
                if (Math.Abs(12 - time.Hour) < 3)
                {
                    pictureBox1.Image = original;
                    lux = 10000;
                    _model.Definition.Tables["result"].Rows[0]["value"] = lux;
                }
                else if (Math.Abs(12 - time.Hour) > 8)
                {
                    lux = 20;
                    _model.Definition.Tables["result"].Rows[0]["value"] = lux;
                    br.AdjustValue = -0.5;                    
                    Bitmap bt = new Bitmap(original);
                    br.ApplyInPlace(bt);
                    pictureBox1.Image = bt;
                }
                else
                {
                    brightness = (double)Math.Abs(12 - time.Hour) / 20;
                    br.AdjustValue = -brightness;
                    lux = (int)(brightness * 7000);
                    _model.Definition.Tables["result"].Rows[0]["value"] = lux;
                    Bitmap bt = new Bitmap(original);
                    br.ApplyInPlace(bt);
                    pictureBox1.Image = bt;
                }
            }
            else
            {
                pictureBox1.Invoke(new settingBright(this.setBrightness), time);
            }
        }

        private void dawnTimer_Tick(object sender, EventArgs e)
        {
            dateTimePicker.Value = dateTimePicker.Value.AddHours(1.33);
            setBrightness(dateTimePicker.Value);
        }

        #region IPobicosView Members

        public void EventReturn(string callID, string returnValue)
        {
            throw new NotImplementedException();
        }
        public void Instruction(string instruction, string callID, string param)
        {
            {
                InstructionsList instr = (InstructionsList)Enum.Parse(typeof(InstructionsList), instruction);
                if (instr.Equals(InstructionsList.pongiGetBrightness))
                {
                    client.InstructionReturn((IPobicosModel)this.Model, callID, _model.Definition.Tables["result"].Rows[0]["value"].ToString());
                }
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
                _model = value as IPobicosModel;
            }
        }
        #endregion
    }
}
