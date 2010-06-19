using PobicosLibrary;
using Microsoft.Xna.Framework;
using POBICOS.SimLogic.Scenarios;
using System.Collections.Generic;
using System;

namespace POBICOS.SimLogic.PobicosObjects
{
	/// <summary>
	/// Class implementing Dawn Detector
	/// </summary>
	class DawnDetector : SimObject, IPobicosView, IPobicosObjects
	{
		/// <summary>NGLibrary <o>model</o></summary>
		private IModel pobicosModel;

		/// <summary>POBICOS Event identifier</summary>
		private int eventID = 0;

		/// <summary>currently detected brightness value</summary>
		private float brightness;

		/// <summary>minimum detected brightness value</summary>
		private float minBrightness = 100;

		/// <summary>maximum detected brightness value</summary>
		private float maxBrightness = 400;

		/// <summary>Gets POBICOS Event identifier</summary>
		public int EventID
		{
			get
			{
				return eventID++;
			}
		}

		/// <summary>
		/// Gets or sets brightness value
		/// </summary>
		public float Brightness
		{
			get
			{
				return brightness;
			}
			set
			{
				brightness = value;
			}
		}

		/// <summary>
		/// <o>DawnDetector</o> constructor
		/// </summary>
		/// <param name="game">game where object shall be placed</param>
		/// <param name="modelFile">3D model file</param>
		/// <param name="room">room where object will be</param>
		/// <param name="configFile">XML POBICOS config file</param>
		public DawnDetector(Game game, string modelFile, Room room, string configFile)
			: base(game, modelFile, room)
		{
			//read XML config
			List<IPobicosModel> models = PobicosLibrary.AdminTools.readConfiguration(configFile);

			//initiate POBICOS model
			foreach (PobicosLibrary.Model model in models)
			{
                SimScenario.Client.RegisterModel(model);
				model.AddObserver(this);
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
			//return brightness value
			InstructionsList instr = (InstructionsList)Enum.Parse(typeof(InstructionsList), instruction);

			if (instr.Equals(InstructionsList.GetBrightness))
				SimScenario.Client.InstructionReturn((IPobicosModel)this.Model, callID, ((int)Brightness).ToString());
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
		public void Interact()
		{
			return;
		}

		/// <summary>
		/// Return Dawn Detector position
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
		public void SwitchLight(float difference, Room room)
		{
			//switch light
			if (model.room.Equals(room) || room.Equals(Room.All))
			{
				model.basicEffectManager.Light0Direction *= new Vector3(difference);
				model.basicEffectManager.Light1Direction *= new Vector3(difference);
				model.basicEffectManager.Light2Direction *= new Vector3(difference);
			}

			//detect current brightness
			Brightness = CalculateBrightness(model.basicEffectManager.Light0Direction.Length());
		}

		/// <summary>
		/// Calculates current bightness detected by dawn detector model
		/// </summary>
		/// <param name="Light0Length">light intensity</param>
		/// <returns>detected brightness (luxes)</returns>
		private float CalculateBrightness(float Light0Length)
		{
			float realBrightness;

			//scale brightness from simulator units into real world units
			realBrightness = (Light0Length - SimScenario.minLight0Length) / (SimScenario.maxLight0Length - SimScenario.minLight0Length);
			realBrightness *= maxBrightness - minBrightness;
			realBrightness += minBrightness;

			//return brightness
			return realBrightness;
		}

		/// <summary>
		/// Helps in searching POBICOS objects by name
		/// </summary>
		/// <param name="name">searched object's name</param>
		/// <param name="room">searched object's location (room)</param>
		/// <returns>null if this model's name is not the same as searched name. <o>Object</o> if this object was the searched one.</returns>
		public object GetByName(string name, Room room)
		{
			if (this.name.Contains(name) && this.model.room.Equals(room))
				return (Object)this;
			else
				return null;
		}

		#endregion
	}
}
