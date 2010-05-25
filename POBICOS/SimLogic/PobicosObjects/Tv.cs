using PobicosLibrary;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using POBICOS.SimLogic.Scenarios;
using Microsoft.Xna.Framework.Graphics;

namespace POBICOS.SimLogic.PobicosObjects
{
	class Tv : SimObject, PobicosLibrary.IPobicosView, IPobicosObjects
	{
		private IModel pobicosModel;
		private ObjectState objectState = ObjectState.OFF;
		private string messageOnScreen = "";

		public enum ObjectState
		{
			ON = 0,
			OFF,
			ALERT
		}

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

		public Tv(Game game, string modelFile, Room room, string configFile)
			: base(game, modelFile, room)
		{
			List<IPobicosModel> models = PobicosLibrary.AdminTools.readConfiguration(configFile);

			foreach (PobicosLibrary.Model model in models)
			{
				model.AddObserver(this);
			 	SimScenario.Client.RegisterModel(model);
				this.Model = model;
			}

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

		public void EventReturn(string callID, string returnValue)
		{
		}

		public void Instruction(string instruction, string callID, string param)
		{
			if (instruction.Equals("889192448"))
			//if (instruction.Equals(InstructionsList.ConveyMessageByText.ToString()))
			{
				messageOnScreen = param.Substring(2, param.LastIndexOf('"') -2);
				TvState = ObjectState.ALERT;
			}
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

		//public void Interact()
		//{
		//    if (this.TvState.Equals(ObjectState.OFF))
		//        TvState = ObjectState.ON;
		//    else
		//        TvState = ObjectState.OFF;
		//}

		#region IPobicosObjects Members

		public void Interact()
		{
			if (this.TvState.Equals(ObjectState.OFF))
				TvState = ObjectState.ON;
			else
				TvState = ObjectState.OFF;
		}

		Vector3 IPobicosObjects.Position()
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
