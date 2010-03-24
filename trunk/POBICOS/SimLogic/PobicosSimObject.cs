using Microsoft.Xna.Framework;
using PobicosLibrary;
using System.Collections.Generic;
using System;

namespace POBICOS.SimLogic
{
	class PobicosSimObject : SimObject, PobicosLibrary.IPobicosView//, PobicosLibrary.IPobicosView
	{
		Client client;
		private IPobicosModel pobicosModel;
		public ObjectState objectState;

		public enum ObjectState
		{ 
			ON = 0,
			OFF,
		}

		#region Properties
		public Client Client
		{
			get
			{
				return client;
			}
			//set
			//{
			//    client = value;
			//}
		}
		#endregion
		public PobicosSimObject(Game game, string modelFile, string configFile)
			: base(game, modelFile)
		{
			client = Client.Instance;

			List<IPobicosModel> models = PobicosLibrary.AdminTools.readConfiguration(configFile);
			foreach (PobicosLibrary.Model model in models)
			{
				model.AddObserver(this);
				client.RegisterModel(model);
			}
			if (client.Connect())
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

		public void Instruction(InstructionsList instruction, string callID, string param)
		{
			switch (instruction)
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
