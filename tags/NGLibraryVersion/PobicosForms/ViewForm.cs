using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using PobicosLibrary;

namespace PobicosForms
{
    public partial class ViewForm : Form, PobicosLibrary.IPobicosView
    {
        private PobicosLibrary.IPobicosModel model;
        public ViewForm(IPobicosModel model)
        {
            InitializeComponent();
            this.model = model;
            InitializeDataGrid();
           

        }

        private void InitializeDataGrid()
        {
            AdminTools.PrintDataSet(model.Definition);
            this.dataGridView1.DataSource = model.ResultTable;
            dataGridView1.AllowUserToDeleteRows = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView2.AllowUserToAddRows = false;
            dataGridView2.AllowUserToDeleteRows = false;
            dataGridView2.DataSource = model.EventTable;
            foreach (DataGridViewColumn column in dataGridView1.Columns)
            {
                if (!column.Name.Equals("Value"))
                    column.ReadOnly = true;

            }
            if (model.ResultTable != null)
            {
              //  AdminTools.eventLog.WriteEntry(model.ResultTable.DisplayExpression);
            }
            dataGridView1.CellValueChanged += new DataGridViewCellEventHandler(dataGridView1_CellValueChanged);

        }

        void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            MessageBox.Show("Value changed");
            AdminTools.PrintTable(model.ResultTable);
            //throw new NotImplementedException();
        }
        public ViewForm()
        {
            InitializeComponent();
        }

       



     

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        #region IPobicosView Members

        public void Update(PobicosLibrary.IPobicosModel model)
        {
            throw new NotImplementedException();
        }

        public void Instruction(string instructionLabel, string param)
        {
            MessageBox.Show("Instr");
         //   throw new NotImplementedException();
        }

        public void EventReturn(string callID, string returnValue)
        {
            MessageBox.Show(callID + "EventRet");
            throw new NotImplementedException();
        }

        #endregion

        #region IPobicosView Members


        public void Instruction(String instruction, string callID, string param)
        {
            throw new NotImplementedException();
        }

        public void InstructionReturn(string callID, string returnValue)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IView Members

        public void Update(IModel model)
        {
            throw new NotImplementedException();
        }

        public IModel Model
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}
