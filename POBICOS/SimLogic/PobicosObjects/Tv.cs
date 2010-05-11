﻿using PobicosLibrary;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using POBICOS.SimLogic.Scenarios;
using Microsoft.Xna.Framework.Graphics;

namespace POBICOS.SimLogic.PobicosObjects
{
	class Tv : SimObject, PobicosLibrary.IPobicosView
	{
		private IModel pobicosModel;
		private ObjectState objectState = ObjectState.OFF;

		public enum ObjectState
		{
			ON = 0,
			OFF,
			FIRE_ALERT
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
			}
		}

		public Tv(Game game, string modelFile, EffectList effectToUse, Room room, string configFile)
			: base(game, modelFile, effectToUse, room)
		{
			List<IPobicosModel> models = PobicosLibrary.AdminTools.readConfiguration(configFile);

			foreach (PobicosLibrary.Model model in models)
			{
				model.AddObserver(this);
			 	SimScenario.client.RegisterModel(model);
				this.Model = model;
			}

			base.model.basicEffectManager.texturesEnabled = true;
			base.model.basicEffectManager.textures = new Texture2D[3];
			base.model.basicEffectManager.texturedMeshName = "Screen2";
			base.model.basicEffectManager.textures[0] = game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_PATH + "tv_on_1");
			base.model.basicEffectManager.textures[1] = game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_PATH + "tv_off_1");
			base.model.basicEffectManager.textures[2] = game.Content.Load<Texture2D>(SimAssetsPath.TEXTURES_PATH + "tv_fireAlert_1");
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
			TvState = ObjectState.FIRE_ALERT;
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

		internal void Switch()
		{
			if (this.TvState.Equals(ObjectState.ON))
				TvState = ObjectState.OFF;
			else
				TvState = ObjectState.ON;
		}
	}
}