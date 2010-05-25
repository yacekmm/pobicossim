using PobicosLibrary;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using POBICOS.SimLogic.Scenarios;

namespace POBICOS.SimLogic.PobicosObjects
{
	class SmokeSensor : SimObject, PobicosLibrary.IPobicosView, IPobicosObjects
	{
		private IModel pobicosModel;
		public ObjectState objectState = ObjectState.IDLE;
		public TimeSpan lastEventTime = new TimeSpan();
		private int eventID = 0;

		public int EventID
		{
			get
			{
				return eventID++;
			}
		}

		public enum ObjectState
		{
			IDLE = 0,
			ALARM
		}

		public SmokeSensor(Game game, string modelFile, Room room, string configFile)
			: base(game, modelFile, room)
		{
			List<IPobicosModel> models = PobicosLibrary.AdminTools.readConfiguration(configFile);

			foreach (PobicosLibrary.Model model in models)
			{
				model.AddObserver(this);
			 	SimScenario.Client.RegisterModel(model);
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

		//internal void Interact()
		//{
		//    return;
		//}

		#region IPobicosObjects Members

		void IPobicosObjects.Interact()
		{
			return;
		}

		public Vector3 Position()
		{
			return model.Transformation.Translate;
		}

		void IPobicosObjects.SwitchLight(float difference, Room room)
		{
			if (model.room.Equals(room))
			{
				model.basicEffectManager.Light0Direction *= new Vector3(difference);
				model.basicEffectManager.Light1Direction *= new Vector3(difference);
				model.basicEffectManager.Light2Direction *= new Vector3(difference);
			}
		}

		void IPobicosObjects.Draw(GameTime gameTime)
		{
			model.Draw(gameTime);
		}

		void IPobicosObjects.Update(GameTime gameTime)
		{
			model.Update(gameTime);
		}

		Object IPobicosObjects.GetByName(string name, Room room)
		{
			if (this.name.Contains(name) && this.model.room.Equals(room))
				return (Object)this;
			else
				return null;
		}

		#endregion
	}
}
