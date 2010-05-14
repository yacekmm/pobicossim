using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using POBICOS.SimLogic.Scenarios;
using POBICOS.Helpers;
using POBICOS.SimBase.Cameras;
using System.Diagnostics;
using POBICOS.SimLogic;

namespace POBICOS
{
	public class POBICOS : Microsoft.Xna.Framework.Game
	{
		public GraphicsDeviceManager graphics;
		//Axis3D myAxis3D;

		InputHelper inputHelper;

		public POBICOS()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

			inputHelper = new InputHelper(PlayerIndex.One);
			Services.AddService(typeof(InputHelper), inputHelper);

			Components.Add(new SimScreen(this, ScenarioBuilder.Scenarios.Flat));
		}

		protected override void Initialize()
		{
			//myAxis3D = new Axis3D(graphics.GraphicsDevice);
			graphics.PreferMultiSampling = true;
			graphics.GraphicsDevice.RenderState.MultiSampleAntiAlias = true;
			graphics.GraphicsDevice.PresentationParameters.MultiSampleQuality = 0;
			graphics.GraphicsDevice.PresentationParameters.MultiSampleType = MultiSampleType.FourSamples;

			base.Initialize();
		}

		protected override void LoadContent()
		{
			//myAxis3D.LoadContent(0, 0, 0, base.Services.GetService(typeof(CameraManager)) as CameraManager);

			float aspectRatio = (float)graphics.GraphicsDevice.Viewport.Width /
										graphics.GraphicsDevice.Viewport.Height;
            CollsionChecker.Initialize();
		}

		protected override void UnloadContent()
		{
			//myAxis3D.UnloadContent();
			
		}

		protected override void Update(GameTime gameTime)
		{
			if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Escape))
				this.Exit();

			//myAxis3D.Draw();

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			//myAxis3D.Draw();

			base.Draw(gameTime);
		}
	}
}
