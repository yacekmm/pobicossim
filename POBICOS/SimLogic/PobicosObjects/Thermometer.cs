using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PobicosLibrary;
using Microsoft.Xna.Framework;
using POBICOS.SimLogic.Scenarios;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace POBICOS.SimLogic.PobicosObjects
{
	/// <summary>
	/// Class implementing Thermometer
	/// </summary>
	class Thermometer : SimObject, PobicosLibrary.IPobicosView, IPobicosObjects
	{
		/// <summary>BISP Library <o>model</o></summary>
		private IModel pobicosModel;
	    
		/// <summary>Current teperature detected by sensor</summary>
		public float temperature = 21;

		/// <summary>last room temperature check time</summary>
		public TimeSpan lastTempCheck = new TimeSpan(0);

		/// <summary>POBICOS model XML config file</summary>
		public string configFile;

		/// <summary>
		/// <o>Thermometer</o> constructor
		/// </summary>
		/// <param name="game">game where object shall be placed</param>
		/// <param name="modelFile">3D model file</param>
		/// <param name="room">room where object will be</param>
		/// <param name="_configFile">XML POBICOS config file</param>
		public Thermometer(Game game, string modelFile, Room room, string _configFile)
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
				this.Model = (IPobicosModel)model;
			}

			//apply custom 3D model effect (load textures to display temperature value)
			base.model.basicEffectManager.texturesEnabled = true;
			base.model.basicEffectManager.textures = new Texture2D[10];
			base.model.basicEffectManager.texturedMeshName = "temp0";
			base.model.basicEffectManager.texturedMeshName2 = "temp1";
			base.model.basicEffectManager.textures[0] = game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_PATH + "num_0");
			base.model.basicEffectManager.textures[1] = game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_PATH + "num_1");
			base.model.basicEffectManager.textures[2] = game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_PATH + "num_2");
			base.model.basicEffectManager.textures[3] = game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_PATH + "num_3");
			base.model.basicEffectManager.textures[4] = game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_PATH + "num_4");
			base.model.basicEffectManager.textures[5] = game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_PATH + "num_5");
			base.model.basicEffectManager.textures[6] = game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_PATH + "num_6");
			base.model.basicEffectManager.textures[7] = game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_PATH + "num_7");
			base.model.basicEffectManager.textures[8] = game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_PATH + "num_8");
			base.model.basicEffectManager.textures[9] = game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_PATH + "num_9");
			base.model.basicEffectManager.currentTexture = 2;
			base.model.basicEffectManager.currentTexture2 = 1;

			base.model.basicEffectManager.Light0Direction *= 1.2f;
			base.model.basicEffectManager.Light1Direction *= 1.2f;
			base.model.basicEffectManager.Light2Direction *= 1.2f;
		}

		#region IPobicosView Members
		/// <summary>
		/// Sends return value for POBICOS Event
		/// </summary>
		/// <param name="callID">POBICOS event identifier</param>
		/// <param name="returnValue">value to be returned in a response to POBICOS event</param>
		public void EventReturn(string callID, string returnValue)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Handling POBICOS instructions
		/// </summary>
		/// <param name="instruction">POBICOS instruction</param>
		/// <param name="callID">POBICOS instruction Identifier</param>
		/// <param name="param">POBICOS instruction parameters</param>
		public void Instruction(string instruction, string callID, string param)
		{
			if (POBICOS.enablePerformanceLog) 
				Trace.TraceInformation("Performance;" + (DateTime.Now - POBICOS.timeStarted) + ";Instruction Received;" + configFile + ";" + instruction);

			//return temperature
			InstructionsList instr = (InstructionsList)Enum.Parse(typeof(InstructionsList), instruction);
			if (instr.Equals(InstructionsList.GetTemp))
			{
				SimScenario.Client.InstructionReturn((IPobicosModel)this.Model, callID, temperature.ToString());
				if (POBICOS.enablePerformanceLog) 
					Trace.TraceInformation("Performance;" + (DateTime.Now - POBICOS.timeStarted) + ";Instruction_Return sent;" + configFile + ";" + instruction);
			}
		}

		/// <summary>
		/// Displays current detected temperature value on <o>Thermometer</o> model
		/// </summary>
		/// <remarks>Displays textures with appriopriate numers on <o>Thermometer</o> model</remarks>
		private void DisplayTemperature()
		{
			if (temperature > 9)
				this.model.basicEffectManager.currentTexture = (int)temperature / 10;
			else
				this.model.basicEffectManager.currentTexture = 0;
			
			this.model.basicEffectManager.currentTexture2 = (int)(temperature % 10);
		}

		/// <summary>
		/// Checks current temperature in environment
		/// </summary>
		/// <remarks>In normal conditions it is random value around 21 degrees.
		/// If fire is in fome temperature is increased depending on how far from fire source the
		/// thermometer model is located</remarks>
		/// <param name="maxTemperature">maximum Temperature that thermometer is able to detect</param>
		/// <param name="gameTime">time since last model update</param>
		public void CheckTemperature(float maxTemperature, GameTime gameTime)
		{
			//store last temperature check timein order to avoid flooding
			lastTempCheck = gameTime.TotalGameTime;
			Random rnd = new Random();
			
			//randomly count temperature value
			temperature = (int)(4 * (rnd.NextDouble() - 0.5f) + 21);

			//check if there is fire in environment
			foreach(SimObject so in SimScenario.movingObjectList)
				if (so.name.Contains("Fire"))
				{
					//if flames are present then increase temperature depending on distance from fire source
					float distance;
					distance = Vector3.Distance(so.model.Transformation.Translate, this.model.Transformation.Translate);

					//count temperature value
					temperature = MathHelper.Max(temperature, maxTemperature / (distance * 0.4f));
					int tmpTemperature = (int)MathHelper.Min(temperature, maxTemperature);
					temperature = tmpTemperature;
				}

			//display detected temperature
			DisplayTemperature();
		}

		#endregion

		#region IView Members
		/// <summary>
		/// Update <o>BISP Library</o> model
		/// </summary>
		/// <param name="model"><o>BISP Library</o> model</param>
		public void Update(PobicosLibrary.IModel model)
		{
			throw new NotImplementedException();
		}

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
		/// Return Thermometer position
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
		/// Implement Thermometer specific draw mehod
		/// </summary>
		/// <param name="gameTime">Time since last update</param>
		void IPobicosObjects.Draw(GameTime gameTime)
		{
			model.Draw(gameTime);
		}

		/// <summary>
		/// Implement Thermometer specific update mehod
		/// </summary>
		/// <param name="gameTime">Time since last update</param>
		void IPobicosObjects.Update(GameTime gameTime)
		{
			model.Update(gameTime);

			//periodically check temperature value
			if (Math.Abs(gameTime.TotalGameTime.Seconds - lastTempCheck.Seconds) > 5)
				CheckTemperature(99, gameTime);
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
