using PobicosLibrary;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace POBICOS.SimLogic.PobicosObjects
{
	class SmokeSensor : SimObject, PobicosLibrary.IPobicosView
	{
		//private IPobicosModel pobicosModel;
		public ObjectState objectState = ObjectState.IDLE;
		Client myClient;

		public enum ObjectState
		{
			IDLE = 0,
			ALARM
		}

		public SmokeSensor(Game game, string modelFile, EffectList effectToUse, Room room, string configFile)
			: base(game, modelFile, effectToUse, room)
		{
			List<IPobicosModel> models = PobicosLibrary.AdminTools.readConfiguration(configFile);
			foreach (PobicosLibrary.Model model in models)
			{
				model.AddObserver(this);
				myClient = game.Services.GetService(typeof(Client)) as Client;
			 	myClient.RegisterModel(model);
			}
			//if (myClient.Connect())
			//{
				//this.Model = (IPobicosModel)models[0];
			//}
			//else Console.WriteLine("Błąd połączenia");
		}

		#region IPobicosView Members

		public void EventReturn(string callID, string returnValue)
		{
		}

		public void Instruction(string instruction, string callID, string param)
		{
		}

		#endregion

		#region IView Members

		public void Update(IModel model)
		{
		}

		public IModel Model
		{
			get
			{
				return null;// pobicosModel;
			}
			set
			{
				//pobicosModel = value;
			}
		}

		#endregion
	}
}
