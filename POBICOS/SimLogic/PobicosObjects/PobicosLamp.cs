using Microsoft.Xna.Framework;
using PobicosLibrary;
using System.Collections.Generic;
using System;
using System.IO;
using POBICOS.SimLogic.Scenarios;
using POBICOS.SimLogic.PobicosObjects;
using System.Diagnostics;

namespace POBICOS.SimLogic
{
	/// <summary>
	/// Class implementing Lamp
	/// </summary>
	class PobicosLamp : SimObject, PobicosLibrary.IPobicosView, IPobicosObjects
	{
		/// <summary>BISP Library <o>model</o></summary>
		private IModel pobicosModel;

		/// <summary><o>Enum</o> indicating Lamp state</summary>
		public ObjectState objectState;

		/// <summary>POBICOS Event identifier</summary>
		private int eventID = 0;

		/// <summary>POBICOS model XML config file</summary>
		public string configFile;
		
		/// <summary>Gets POBICOS Event identifier</summary>
		public int EventID
		{
			get
			{
				return eventID++;
			}
		}

		/// <summary><o>Enum</o> indicating Lamp state</summary>
		public enum ObjectState
		{ 
			ON = 0,	//turned ON
			OFF,	//turned OFF
		}

		/// <summary>
		/// <o>Lamp</o> constructor
		/// </summary>
		/// <param name="game">game where object shall be placed</param>
		/// <param name="modelFile">3D model file</param>
		/// <param name="room">room where object will be</param>
		/// <param name="_configFile">XML POBICOS config file</param>
		public PobicosLamp(Game game, string modelFile, Room room, string _configFile)
			: base(game, modelFile, room)
		{
			this.configFile = _configFile;
			//read XML config
			List<IPobicosModel> models = PobicosLibrary.AdminTools.ReadConfiguration(configFile);

			//initiate POBICOS model
			foreach (PobicosLibrary.Model model in models)
			{
                SimScenario.Client.RegisterModel(model);
				model.RegisterObserver(this);
				this.Model = model;
			}
		}

		#region IPobicosView Members
		/// <summary>
		/// Sends return value for POBICOS Event
		/// </summary>
		/// <param name="callID">POBICOS event identifier</param>
		/// <param name="returnValue">value to be returned in a response to POBICOS event</param>
		public void EventReturn(string callID, string returnValue)
		{
			throw new System.NotImplementedException();
		}

		/// <summary>
		/// Handling POBICOS instructions
		/// </summary>
		/// <param name="instruction">POBICOS instruction</param>
		/// <param name="callID">POBICOS instruction Identifier</param>
		/// <param name="param">POBICOS instruction parameters</param>
		public void Instruction(String instruction, string callID, string param)
		{
			if (POBICOS.enablePerformanceLog) 
				Trace.TraceInformation("Performance;" + (DateTime.Now - POBICOS.timeStarted) + ";Instruction Received;" + configFile + ";" + instruction);
			//switch lamp
           InstructionsList  instr = (InstructionsList)Enum.Parse(typeof(InstructionsList), instruction);
		   
			switch (instr)
			{
				case InstructionsList.SwitchOn:
					if (objectState.Equals(ObjectState.OFF))
					{
						objectState = ObjectState.ON;
						SimScenario.SwitchLight(this.model.room, 1.8f, true);
					}
					break;

				case InstructionsList.SwitchOff:
					if (objectState.Equals(ObjectState.ON))
					{
						objectState = ObjectState.OFF;
						SimScenario.SwitchLight(base.model.room, 1.8f, false);
					}
					break;
			}
		}

		/// <summary>
		/// Sends return value for POBICOS Instruction
		/// </summary>
		/// <param name="callID">POBICOS Instruction identifier</param>
		/// <param name="returnValue">value to be returned in a response to POBICOS Instruction</param>
		public void InstructionReturn(string callID, string returnValue)
		{
			throw new System.NotImplementedException();
		}

		#endregion

		#region IView Members
		/// <summary>
		/// Gets or sets <o>BISP Library</o> model
		/// </summary>
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

		/// <summary>
		/// Update <o>BISP Library</o> model
		/// </summary>
		/// <param name="model"><o>BISP Library</o> model</param>
		public void Update(PobicosLibrary.IModel model)
		{
			throw new System.NotImplementedException();
		}

		#endregion

		#region IPobicosObjects Members
		/// <summary>
		/// Implement object and 3D model specific actions performed during interaction with player
		/// </summary>
		void IPobicosObjects.Interact()
		{
			//send switch event
			SimScenario.Client.Event(this, EventsList.ponge_originated_event_switch_originated_event, EventID.ToString(), null);

			//switch light in room
			if (this.objectState.Equals(PobicosLamp.ObjectState.OFF))
			{
				this.objectState = PobicosLamp.ObjectState.ON;
				SimScenario.SwitchLight(this.model.room, 1.8f, true);
			}
			else if (this.objectState.Equals(PobicosLamp.ObjectState.ON))
			{
				this.objectState = PobicosLamp.ObjectState.OFF;
				SimScenario.SwitchLight(this.model.room, 1.8f, false);
			}
		}

		/// <summary>
		/// Return Lamp position
		/// </summary>
		/// <returns>3D point indicating object's position</returns>
		Vector3 IPobicosObjects.Position()
		{
			return model.Transformation.Translate;
		}

		/// <summary>
		/// Change object's light intensity
		/// </summary>
		/// <param name="difference">light change factor</param>
		/// <param name="room">room where light intensity is being changed</param>
		void IPobicosObjects.SwitchLight(float difference, Room room)
		{
			if (model.room.Equals(room) || room.Equals(Room.All))
			{
				model.basicEffectManager.Light0Direction *= new Vector3(difference);
				model.basicEffectManager.Light1Direction *= new Vector3(difference);
				model.basicEffectManager.Light2Direction *= new Vector3(difference);
			}
		}

		/// <summary>
		/// Implement Lamp specific draw mehod
		/// </summary>
		/// <param name="gameTime">Time since last update</param>
		void IPobicosObjects.Draw(GameTime gameTime)
		{
			model.Draw(gameTime);
		}

		/// <summary>
		/// Implement Lamp specific update mehod
		/// </summary>
		/// <param name="gameTime">Time since last update</param>
		void IPobicosObjects.Update(GameTime gameTime)
		{
			model.Update(gameTime);
		}

		/// <summary>
		/// Helps in searching POBICOS objects by name
		/// </summary>
		/// <param name="name">searched object's name</param>
		/// <param name="room">searched object's location (room)</param>
		/// <returns>null if this model's name is not the same as searched name. <o>Object</o> if this object was the searched one.</returns>
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
