using PobicosLibrary;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using POBICOS.SimLogic.Scenarios;

namespace POBICOS.SimLogic.PobicosObjects
{
	/// <summary>
	/// Class implementing SmokeSensors
	/// </summary>
	class SmokeSensor : SimObject, PobicosLibrary.IPobicosView, IPobicosObjects
	{
		/// <summary>NGLibrary <o>model</o></summary>
		private IModel pobicosModel;

		/// <summary>Last moment when event was sent by Smoke sensor (avoid flooding Management Server)</summary>
		public TimeSpan lastEventTime = new TimeSpan();
		
		/// <summary>POBICOS Event identifier</summary>
		private int eventID = 0;

		/// <summary>Gets POBICOS Event identifier</summary>
		public int EventID
		{
			get
			{
				return eventID++;
			}
		}

		/// <summary>
		/// <o>SmokeSensor</o> constructor
		/// </summary>
		/// <param name="game">game where object shall be placed</param>
		/// <param name="modelFile">3D model file</param>
		/// <param name="room">room where object will be</param>
		/// <param name="configFile">XML POBICOS config file</param>
		public SmokeSensor(Game game, string modelFile, Room room, string configFile)
			: base(game, modelFile, room)
		{
			//read XML config
			List<IPobicosModel> models = PobicosLibrary.AdminTools.ReadConfiguration(configFile);

			//initiate POBICOS model
			foreach (PobicosLibrary.Model model in models)
			{
				model.RegisterObserver(this);
			 	SimScenario.Client.RegisterModel(model);
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
		}

		/// <summary>
		/// Handling POBICOS instructions
		/// </summary>
		/// <param name="instruction">POBICOS instruction</param>
		/// <param name="callID">POBICOS instruction Identifier</param>
		/// <param name="param">POBICOS instruction parameters</param>
		public void Instruction(string instruction, string callID, string param)
		{
		}

		#endregion

		#region IView Members
		/// <summary>
		/// Update <o>NGLibrary</o> model
		/// </summary>
		/// <param name="model"><o>NGLibrary</o> model</param>
		public void Update(IModel model)
		{
		}

		/// <summary>
		/// Gets or sets <o>NGLibrary</o> model
		/// </summary>
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

		#region IPobicosObjects Members
		/// <summary>
		/// Implement object and 3D model specific actions performed during interaction with player
		/// </summary>
		void IPobicosObjects.Interact()
		{
			return;
		}

		/// <summary>
		/// Return Smoke Sensor position
		/// </summary>
		/// <returns>3D point indicating object's position</returns>
		public Vector3 Position()
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
		/// Implement Smoke Sensor specific draw mehod
		/// </summary>
		/// <param name="gameTime">Time since last update</param>
		void IPobicosObjects.Draw(GameTime gameTime)
		{
			model.Draw(gameTime);
		}

		/// <summary>
		/// Implement Smoke Sensor specific update mehod
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
