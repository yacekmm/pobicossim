using PobicosLibrary;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace POBICOS.SimLogic.PobicosObjects
{
	class SmokeSensor : SimObject, PobicosLibrary.IPobicosView
	{
		private IModel pobicosModel;
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
			myClient = game.Services.GetService(typeof(Client)) as Client;

			foreach (PobicosLibrary.Model model in models)
			{
				model.AddObserver(this);
			 	myClient.RegisterModel(model);
				this.Model = model;
			}
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
