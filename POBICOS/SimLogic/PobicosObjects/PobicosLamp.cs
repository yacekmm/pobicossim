using Microsoft.Xna.Framework;
using PobicosLibrary;
using System.Collections.Generic;
using System;
using System.IO;
using POBICOS.SimLogic.Scenarios;
using POBICOS.SimLogic.PobicosObjects;

namespace POBICOS.SimLogic
{
	class PobicosLamp : SimObject, PobicosLibrary.IPobicosView, IPobicosObjects
	{
		private IModel pobicosModel;
		public ObjectState objectState;
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
			ON = 0,
			OFF,
		}

		public PobicosLamp(Game game, string modelFile, Room room, string configFile)
			: base(game, modelFile, room)
		{
			List<IPobicosModel> models = PobicosLibrary.AdminTools.readConfiguration(configFile);

			foreach (PobicosLibrary.Model model in models)
			{
                SimScenario.Client.RegisterModel(model);
				model.AddObserver(this);
				this.Model = model;
			}
		}

		#region IPobicosView Members

		public void EventReturn(string callID, string returnValue)
		{
			throw new System.NotImplementedException();
		}

		public void Instruction(String instruction, string callID, string param)
		{
           InstructionsList  instr = (InstructionsList)Enum.Parse(typeof(InstructionsList), instruction);
		   //SimScenario ss = this.Game.Services.GetService(typeof(SimScenario)) as SimScenario;
		   
			switch (instr)
			{
				case InstructionsList.SwitchOn:
					if (objectState.Equals(ObjectState.OFF))
					{
						objectState = ObjectState.ON;
						SimScenario.SwitchLight(this.model.room, true);
					}
					break;

				case InstructionsList.SwitchOff:
					if (objectState.Equals(ObjectState.ON))
					{
						objectState = ObjectState.OFF;
						SimScenario.SwitchLight(base.model.room, false);
					}
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
				pobicosModel = (IModel)value;
			}
		}

		public void Update(PobicosLibrary.IModel model)
		{
			throw new System.NotImplementedException();
		}

		#endregion

		//public void Interact()
		//{
		//    SimScenario.Client.Event(this, EventsList.ponge_originated_event_switch_originated_event, null, null);

		//    if (this.objectState.Equals(PobicosLamp.ObjectState.OFF))
		//    {
		//        this.objectState = PobicosLamp.ObjectState.ON;
		//        SimScenario.SwitchLight(this.model.room, true);
		//    }
		//    else if (this.objectState.Equals(PobicosLamp.ObjectState.ON))
		//    {
		//        this.objectState = PobicosLamp.ObjectState.OFF;
		//        SimScenario.SwitchLight(this.model.room, false);
		//    }
		//}

		#region IPobicosObjects Members

		void IPobicosObjects.Interact()
		{
			SimScenario.Client.Event(this, EventsList.ponge_originated_event_switch_originated_event, EventID.ToString(), null);

			if (this.objectState.Equals(PobicosLamp.ObjectState.OFF))
			{
				this.objectState = PobicosLamp.ObjectState.ON;
				SimScenario.SwitchLight(this.model.room, true);
			}
			else if (this.objectState.Equals(PobicosLamp.ObjectState.ON))
			{
				this.objectState = PobicosLamp.ObjectState.OFF;
				SimScenario.SwitchLight(this.model.room, false);
			}
		}

		Vector3 IPobicosObjects.Position()
		{
			return model.Transformation.Translate;
		}

		void IPobicosObjects.SwitchLight(float difference, Room room)
		{
			if (model.room.Equals(room) || room.Equals(Room.All))
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
