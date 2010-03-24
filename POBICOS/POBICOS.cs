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

namespace POBICOS
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class POBICOS : Microsoft.Xna.Framework.Game
	{
		public GraphicsDeviceManager graphics;
		Axis3D myAxis3D;

		InputHelper inputHelper;

		public POBICOS()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";

			inputHelper = new InputHelper(PlayerIndex.One);
			Services.AddService(typeof(InputHelper), inputHelper);

			Components.Add(new SimScreen(this, ScenarioBuilder.Scenarios.Flat));
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// TODO: Add your initialization logic here
			myAxis3D = new Axis3D(graphics.GraphicsDevice);
			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			myAxis3D.LoadContent(0, 0, 0, base.Services.GetService(typeof(CameraManager)) as CameraManager);

			// Calculate the aspect ratio for the model
			float aspectRatio = (float)graphics.GraphicsDevice.Viewport.Width /
										graphics.GraphicsDevice.Viewport.Height;
		}

		/// <summary>
		/// UnloadContent will be called once per game and is the place to unload
		/// all content.
		/// </summary>
		protected override void UnloadContent()
		{
			myAxis3D.UnloadContent();
			
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			// Allows the game to exit
			if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Escape))
				this.Exit();

			myAxis3D.Draw();
			//myAxis3D.worldMatrix *= Matrix.CreateRotationY(0.01f) * Matrix.CreateRotationX(0.01f);

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			myAxis3D.Draw();

			base.Draw(gameTime);
		}
	}
}
