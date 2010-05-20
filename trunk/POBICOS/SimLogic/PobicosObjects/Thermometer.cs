using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PobicosLibrary;
using Microsoft.Xna.Framework;
using POBICOS.SimLogic.Scenarios;
using Microsoft.Xna.Framework.Graphics;

namespace POBICOS.SimLogic.PobicosObjects
{
	class Thermometer : SimObject, PobicosLibrary.IPobicosView
	{
		private IModel pobicosModel;
	    
		public float temperature = 21;
		public TimeSpan lastTempCheck = new TimeSpan(0);

		public Thermometer(Game game, string modelFile, Room room, string configFile)
			: base(game, modelFile, room)
		{
			List<IPobicosModel> models = PobicosLibrary.AdminTools.readConfiguration(configFile);

			foreach (PobicosLibrary.Model model in models)
			{
				SimScenario.Client.RegisterModel(model);
				model.AddObserver(this);
				this.Model = (IPobicosModel)model;
			}

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

		public void EventReturn(string callID, string returnValue)
		{
			throw new NotImplementedException();
		}

		public void Instruction(string instruction, string callID, string param)
		{
			InstructionsList instr = (InstructionsList)Enum.Parse(typeof(InstructionsList), instruction);
			if (instr.Equals(InstructionsList.GetTemp))
			{
				//CheckTemperature(99);
				SimScenario.Client.InstructionReturn((IPobicosModel)this.Model, callID, temperature.ToString());
			}
		}

		private void DisplayTemperature()
		{
			if (temperature > 9)
				this.model.basicEffectManager.currentTexture = (int)temperature / 10;
			else
				this.model.basicEffectManager.currentTexture = 0;
			
			this.model.basicEffectManager.currentTexture2 = (int)(temperature % 10);
		}

		public void CheckTemperature(float maxTemperature, GameTime gameTime)
		{
			lastTempCheck = gameTime.TotalGameTime;
			Random rnd = new Random();
			temperature = (int)(4 * (rnd.NextDouble() - 0.5f) + 21);

			foreach(SimObject so in SimScenario.movingObjectList)
				if (so.name.Contains("Fire"))
				{
					float distance;
					distance = Vector3.Distance(so.model.Transformation.Translate, this.model.Transformation.Translate);

					temperature = MathHelper.Max(temperature, maxTemperature / (distance * 0.4f));
					int tmpTemperature = (int)MathHelper.Min(temperature, maxTemperature);
					temperature = tmpTemperature;
				}

			DisplayTemperature();
		}

		#endregion

		#region IView Members

		public void Update(PobicosLibrary.IModel model)
		{
			throw new NotImplementedException();
		}

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

		internal void Interact()
		{
			return;
		}
	}
}
