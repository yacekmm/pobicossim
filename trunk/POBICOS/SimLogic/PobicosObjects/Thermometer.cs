﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PobicosLibrary;
using Microsoft.Xna.Framework;
using POBICOS.SimLogic.Scenarios;

namespace POBICOS.SimLogic.PobicosObjects
{
	class Thermometer : SimObject, PobicosLibrary.IPobicosView
	{
		private IModel pobicosModel;
	    
		public int temperature = 21;

		public Thermometer(Game game, string modelFile, EffectList effectToUse, Room room, string configFile)
			: base(game, modelFile, effectToUse, room)
		{
			List<IPobicosModel> models = PobicosLibrary.AdminTools.readConfiguration(configFile);

			foreach (PobicosLibrary.Model model in models)
			{
				SimScenario.client.RegisterModel(model);
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
				SimScenario.client.InstructionReturn((IPobicosModel)this.Model, callID, temperature.ToString());
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