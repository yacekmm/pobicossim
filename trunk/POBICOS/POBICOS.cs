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
using POBICOS.SimBase;

namespace POBICOS
{
	/// <summary>
	/// Provides basic simulator initialization, game logic and rendering code
	/// </summary>
	public class POBICOS : Microsoft.Xna.Framework.Game
	{
		/// <summary>FPS Counter update interval </summary>
		private float updateInterval = 1.0f;
		/// <summary>Time elapsed since last FP counter Update </summary>
		private float timeSinceLastUpdate = 0.0f;
		/// <summary>Number of frames displayed in current second</summary>
	    private float framecount = 0;
		/// <summary>Frames Per Second value</summary>
		private float fps = 0;
		/// <summary>Flag indicating if FPS value should be shown</summary>
		private bool showFPS = false;
		/// <summary>Flag indicating if performance factors should be logged into application log</summary>
		public static bool enablePerformanceLog = false;

		/// <summary>configuration of graphics device</summary>
		public GraphicsDeviceManager graphics;

		/// <summary>
		/// Helper variable to handle input from keyboard
		/// </summary>
		InputHelper inputHelper;

		/// <summary>Time When appplication was started. Used mainly for logging activity</summary>
		public static DateTime timeStarted = DateTime.Now;

		/// <summary>
		/// POBICOS Constructor
		/// </summary>
		public POBICOS()
		{
			//initialize graphic device
			graphics = new GraphicsDeviceManager(this);
			//set content directory
			Content.RootDirectory = "Content";

			inputHelper = new InputHelper(PlayerIndex.One);
			Services.AddService(typeof(InputHelper), inputHelper);

			//add new game component and build scenario
			Components.Add(new SimScreen(this, ScenarioBuilder.Scenarios.Flat));
		}

		/// <summary>
		/// Called after the Game and GraphicsDevice are created, but before LoadContent.
		///     Reference page contains code sample.
		/// </summary>
		protected override void Initialize()
		{
			//Prefer multi sampling to improve graphics
			graphics.PreferMultiSampling = true;
			graphics.GraphicsDevice.RenderState.MultiSampleAntiAlias = true;
			graphics.GraphicsDevice.PresentationParameters.MultiSampleQuality = 0;
			graphics.GraphicsDevice.PresentationParameters.MultiSampleType = MultiSampleType.FourSamples;

			base.Initialize();
		}

		/// <summary>
		/// Loads custom game content
		/// </summary>
		protected override void LoadContent()
		{
			//get graphics device parameters
			float aspectRatio = (float)graphics.GraphicsDevice.Viewport.Width /
										graphics.GraphicsDevice.Viewport.Height;

			//initialize collision checker. will be used to detect collisions of Human with walls
            CollsionChecker.Initialize();
		}

		/// <summary>
		/// Unloads custom content
		/// </summary>
		protected override void UnloadContent()
		{}

		/// <summary>
		/// Updates screen
		/// </summary>
		/// <param name="gameTime">Time passed since the last call to Update.</param>
		protected override void Update(GameTime gameTime)
		{
			//allows to exit simulation
			if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Escape))
				this.Exit();
			if (inputHelper.IsKeyJustPressed(Keys.P))
			{
				showFPS = !showFPS;
				if (!showFPS)
					if(enablePerformanceLog)
						this.Window.Title = "POBICOS (performance logging)";
					else
						this.Window.Title = "POBICOS";
			}
			if (inputHelper.IsKeyJustPressed(Keys.L))
			{
				enablePerformanceLog = !enablePerformanceLog;
				if (enablePerformanceLog)
					this.Window.Title += " (performance logging)";
				else
					this.Window.Title = this.Window.Title.Substring(0, this.Window.Title.Length - 22);
			}

			base.Update(gameTime);

			if (showFPS)
			{
				//calculate FPS
				float elapsed = (float)gameTime.ElapsedRealTime.TotalSeconds;
				framecount++;
				timeSinceLastUpdate += elapsed;

				if (timeSinceLastUpdate > updateInterval)
				{
					fps = framecount / timeSinceLastUpdate; //mean fps over updateIntrval
					framecount = 0;
					timeSinceLastUpdate -= updateInterval;
				}
			}
		}

		/// <summary>
		/// Drawing items on the screen
		/// </summary>
		/// <param name="gameTime">Time passed since the last call to Update.</param>
		protected override void Draw(GameTime gameTime)
		{
			base.Draw(gameTime);

			if (showFPS && gameTime.TotalGameTime.Milliseconds == 0)
			{
				double fps_d = fps/2;
				//Display FPS on title bar
				this.Window.Title = "POBICOS (FPS: " + Math.Round(fps_d, 1).ToString() + ")";
				if (POBICOS.enablePerformanceLog)
				{
					if(!this.Window.Title.Contains("(performance logging)"))
						this.Window.Title += " (performance logging)";
				}
				Trace.TraceInformation("Performance;" + (DateTime.Now - POBICOS.timeStarted) + ";FPS;" + fps_d.ToString());
			}
		}
	}
}
