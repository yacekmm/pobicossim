﻿using Microsoft.Xna.Framework;
using PobicosLibrary;
using System.Collections.Generic;
using System;
using System.IO;
using POBICOS.SimLogic.Scenarios;

namespace POBICOS.SimLogic
{
	class PobicosLamp : SimObject, PobicosLibrary.IPobicosView//, PobicosLibrary.IPobicosView
	{
		private IPobicosModel pobicosModel;
		public ObjectState objectState;
		Client myClient;

		public enum ObjectState
		{ 
			ON = 0,
			OFF,
		}

		public PobicosLamp(Game game, string modelFile, EffectList effectToUse, Room room, string configFile)
			: base(game, modelFile, effectToUse, room)
		{
			List<IPobicosModel> models = PobicosLibrary.AdminTools.readConfiguration(configFile);
			foreach (PobicosLibrary.Model model in models)
			{
				model.AddObserver(this);
				myClient = game.Services.GetService(typeof(Client)) as Client;
			 	myClient.RegisterModel(model);
			}
			if (myClient.Connect())
			{
				this.Model = (IPobicosModel)models[0];
			}
			else Console.WriteLine("Błąd połączenia");
		}

		#region IPobicosView Members

		public void EventReturn(string callID, string returnValue)
		{
			throw new System.NotImplementedException();
		}

		public void Instruction(String instruction, string callID, string param)
		{
           InstructionsList  instr = (InstructionsList)Enum.Parse(typeof(InstructionsList), instruction);
			switch (instr)
			{
				case InstructionsList.pongiSwitchOn:
					if(objectState.Equals(ObjectState.OFF))
						objectState = ObjectState.ON;
					break;

				case InstructionsList.pongiSwitchOff:
					if (objectState.Equals(ObjectState.ON))
						objectState = ObjectState.OFF;
					break;
			}
		}

		public void InstructionReturn(string callID, string returnValue)
		{
			throw new System.NotImplementedException();
		}

		#endregion

		#region IView Members

		public PobicosLibrary.IModel Model
		{
			get
			{
				return pobicosModel;
			}
			set
			{
				pobicosModel = (IPobicosModel)value;
			}
		}

		public void Update(PobicosLibrary.IModel model)
		{
			throw new System.NotImplementedException();
		}

		#endregion
	}
}
