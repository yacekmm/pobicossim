using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PobicosLibrary;
using Microsoft.Xna.Framework;

namespace POBICOS.SimLogic.PobicosObjects
{
	class Thermometer : SimObject, PobicosLibrary.IPobicosView
	{
		private IModel pobicosModel;
		Client myClient;

		public int temperature = 21;

		public Thermometer(Game game, string modelFile, EffectList effectToUse, Room room, string configFile)
			: base(game, modelFile, effectToUse, room)
		{
			myClient = game.Services.GetService(typeof(Client)) as Client;
			List<IPobicosModel> models = PobicosLibrary.AdminTools.readConfiguration(configFile);
			myClient = game.Services.GetService(typeof(Client)) as Client;

			foreach (PobicosLibrary.Model model in models)
			{
				myClient.RegisterModel(model);
				model.AddObserver(this);
				this.Model = (IPobicosModel)model;
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
			if (instr.Equals(InstructionsList.GetTemp))
			{
				//client.InstructionReturn((IPobicosModel)this.Model, callID, Model.Definition.Tables["result"].Rows[0]["value"].ToString());
				myClient.InstructionReturn((IPobicosModel)this.Model, callID, temperature.ToString());
			}
		}

		#endregion

		#region IView Members

		public void Update(PobicosLibrary.IModel model)
		{
			throw new NotImplementedException();
		}

		public PobicosLibrary.IModel Model
		{
			get
			{
				return pobicosModel;
			}
			set
			{
				pobicosModel = value;
			}
		}

		#endregion
	}
}
