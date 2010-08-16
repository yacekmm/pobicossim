using PobicosLibrary;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using POBICOS.SimLogic.Scenarios;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace POBICOS.SimLogic.PobicosObjects
{
	/// <summary>
	/// Class implementing TV set
	/// </summary>
	class Tv : SimObject, PobicosLibrary.IPobicosView, IPobicosObjects
	{
		/// <summary>NGLibrary <o>model</o></summary>
		private IModel pobicosModel;

		/// <summary>POBICOS model XML config file</summary>
		public string configFile;
		
		/// <summary><o>Enum</o> indicating TV state</summary>
		private ObjectState objectState = ObjectState.OFF;

		/// <summary>text message to be displayed on TV screen</summary>
		private string messageOnScreen = "";

		/// <summary><o>Enum</o> indicating TV state</summary>
		public enum ObjectState
		{
			ON = 0,	//shows TV audition
			OFF,	//turned off
			ALERT	//displaying text message originated from POBICOS system
		}

		#region Properties
		/// <summary>
		/// Gets or sets TV state
		/// </summary>
		/// <remarks>Also performs actions specific for each state (puts texture or writes a text).</remarks>
		public ObjectState TvState
		{
			get
			{
				return objectState;
			}
			set
			{
				objectState = value;
				base.model.basicEffectManager.currentTexture = (int)objectState;
				if (objectState.Equals(ObjectState.ALERT))
					base.model.basicEffectManager.TextToWrite = messageOnScreen;
				else
					base.model.basicEffectManager.writeOnObject = false;
			}
		}
		#endregion

		/// <summary>
		/// <o>Tv</o> constructor
		/// </summary>
		/// <param name="game">game where object shall be placed</param>
		/// <param name="modelFile">3D model file</param>
		/// <param name="room">room where object will be</param>
		/// <param name="_configFile">XML POBICOS config file</param>
		public Tv(Game game, string modelFile, Room room, string _configFile)
			: base(game, modelFile, room)
		{
			this.configFile = _configFile;
			//read XML config
			List<IPobicosModel> models = PobicosLibrary.AdminTools.ReadConfiguration(configFile);

			//initiate POBICOS model
			foreach (PobicosLibrary.Model model in models)
			{
				model.RegisterObserver(this);
			 	SimScenario.Client.RegisterModel(model);
				this.Model = model;
			}

			//apply custom 3D model effect
			base.model.basicEffectManager.texturesEnabled = true;
			base.model.basicEffectManager.textures = new Texture2D[3];
			base.model.basicEffectManager.texturedMeshName = "Screen2";
			base.model.basicEffectManager.textures[0] = game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_PATH + "tv_on_1");
			base.model.basicEffectManager.textures[1] = game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_PATH + "tv_off_1");
			base.model.basicEffectManager.textures[2] = game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_FONT_PATH + "arial-0");
			base.model.basicEffectManager.currentTexture = (int)objectState;

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
		}

		/// <summary>
		/// Handling POBICOS instructions
		/// </summary>
		/// <param name="instruction">POBICOS instruction</param>
		/// <param name="callID">POBICOS instruction Identifier</param>
		/// <param name="param">POBICOS instruction parameters</param>
		public void Instruction(string instruction, string callID, string param)
		{
			Trace.TraceInformation("Performance;" + (DateTime.Now - POBICOS.timeStarted) + ";Instruction Received;" + configFile + ";" + instruction);
			
			if (instruction.Equals("889192448") || instruction.Equals(InstructionsList.ConveyMessageByText.ToString()))
			{
				messageOnScreen = param.Substring(2, param.LastIndexOf('"') -2);
				TvState = ObjectState.ALERT;
			}
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
			//turn ON/OFF tv
			if (this.TvState.Equals(ObjectState.OFF))
				TvState = ObjectState.ON;
			else
				TvState = ObjectState.OFF;
		}

		/// <summary>
		/// Return TV position
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
		/// Implement TV specific draw mehod
		/// </summary>
		/// <param name="gameTime">Time since last update</param>
		void IPobicosObjects.Draw(GameTime gameTime)
		{
			model.Draw(gameTime);
		}

		/// <summary>
		/// Implement TV specific update mehod
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
