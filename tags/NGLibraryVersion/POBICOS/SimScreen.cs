using POBICOS.Helpers;
using Microsoft.Xna.Framework;
using POBICOS.SimLogic.Scenarios;
using System;
using POBICOS.SimLogic;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using POBICOS.SimBase.Cameras;
using POBICOS.SimLogic.PobicosObjects;
using POBICOS.SimBase;
using System.Diagnostics;

namespace POBICOS
{
	/// <summary>
	/// Handles all events, input and actions during application execution.
	/// </summary>
	/// <remarks>Initializes all other classes, updates screen and performing update logic, drawing objects on screen</remarks>
	class SimScreen : DrawableGameComponent
	{
		/// <summary>
		/// Enum pointing which scenario is choosen to be build.
		/// </summary>
		ScenarioBuilder.Scenarios currentScenario;

		/// <summary>
		/// scenario controller used for executing changes and performing actions (i.e. interaction with user) in environment
		/// </summary>
		SimScenario simScenario;

		/// <summary>
		/// Helper class collecting keyboard input events
		/// </summary>
		InputHelper inputHelper;

		/// <summary>
		/// SimScreen constructor
		/// </summary>
		/// <remarks>Constructs <o>DrawableGameComponent</o> instance</remarks>
		/// <param name="game">current Game instance</param>
		/// <param name="scenario">chosen scenario</param>
		public SimScreen(Game game, ScenarioBuilder.Scenarios scenario)
			: base(game)
		{
			this.currentScenario = scenario;
		}

		/// <summary>
		/// Initializes XNA environment
		/// </summary>
		/// <remarks>Stores <c>inputHelper</c> in a <o>GameServiceContainer</o></remarks>
		/// <exception cref="InvalidOperationException">If <c>inputHelper</c> was not initialized and is empty</exception>
		public override void Initialize()
		{
			inputHelper = Game.Services.GetService(typeof(InputHelper)) as InputHelper;
			if (inputHelper == null)
				throw new InvalidOperationException("Cannot find an input service: inputHelper");

			base.Initialize();
		}

		/// <summary>
		/// Building scenario
		/// </summary>
		/// <remarks>responsible for creating all 3D models in simulator</remarks>
		protected override void LoadContent()
		{
			simScenario = ScenarioBuilder.BuildScenario(Game, currentScenario);
			
			base.LoadContent();
		}

		/// <summary>
		/// Unloading game content
		/// </summary>
		/// <remarks>Called when application is being closed. Unloads all 3D models and disconnect application from POBICOS Management Server</remarks>
		protected override void UnloadContent()
		{
			base.UnloadContent();

			//Disconnect from management server
			SimScenario.Client.Disconnect();
		}

		/// <summary>
		/// Collects info about Keyboard State
		/// </summary>
		/// <remarks>Checks if keyboard state has changed, which keys (if any) are pressed and reflects these changes in environment.
		/// Responsible for moving a <o>Humans</o>, Camera, putting fire or smoke and interacting with environment.</remarks>
		private void UpdateInput()
		{
			//Get Human and Camera in order to update its position
			Human activeHuman = simScenario.GetActiveHuman();
			ThirdPersonCamera activeCamera = (ThirdPersonCamera)simScenario.cameraManager.ActiveCamera;

			//camera location and movement speed parameters
			float cameraSpeed = 0.02f;
			Vector3 cameraUpOffset = simScenario.cameraUpOffset;
			bool needUpdateCameraRotation = false;

			//Zoom in camera on player
			if (inputHelper.IsKeyPressed(Keys.PageUp))
			{
				Vector3 direction = activeHuman.Transformation.Translate - activeCamera.Position;
				activeCamera.Position += direction * cameraSpeed;
				if (activeCamera.Position.Y < 1.6f)
					activeCamera.Position -= direction * cameraSpeed;
			}
			//Zoom out camera
            if (inputHelper.IsKeyPressed(Keys.PageDown))
            {
				Vector3 direction = activeHuman.Transformation.Translate - activeCamera.Position;
				activeCamera.Position -= direction * cameraSpeed;
				if (activeCamera.Position.Y < 1.6f)
					activeCamera.Position += direction * cameraSpeed;
            }
			//rotate camera around the player (clockwise)
			if (inputHelper.IsKeyPressed(Keys.Left))
			{
				activeCamera.Rotate += 1.5f;
				needUpdateCameraRotation = true;
			}
			//rotate camera around the player (anti-clockwise)
			if (inputHelper.IsKeyPressed(Keys.Right))
			{
				activeCamera.Rotate -= 1.5f;
				needUpdateCameraRotation = true;
			}
			//move camera up
			if (inputHelper.IsKeyPressed(Keys.Up))
			{
				activeCamera.Position += Vector3.Up * cameraSpeed * 2;
			}
			//move camera down
			if (inputHelper.IsKeyPressed(Keys.Down))
			{
				activeCamera.Position += Vector3.Down * cameraSpeed * 2;
				if (activeCamera.Position.Y < 1.6f)
					activeCamera.Position -= Vector3.Down * cameraSpeed*2;
			}
			//Steering player: forward
            if (inputHelper.IsKeyPressed(Keys.W))
            {
				Vector3 oldPosition = activeHuman.Transformation.Translate;
				//move human with collision check
                CollsionChecker.Move(ref activeHuman, true);

				//Update camera position
				activeCamera.Target = activeHuman.Transformation.Translate + cameraUpOffset;
				activeCamera.Position += activeHuman.Transformation.Translate - oldPosition;

				//ensure that player touches ground
				UpdatePlayerHeight();
            }
			//Steering player: backward
			if (inputHelper.IsKeyPressed(Keys.S))
			{
				Vector3 oldPosition = activeHuman.Transformation.Translate;
				//move human with collision check
				CollsionChecker.Move(ref activeHuman, false);

				//Update camera position
				activeCamera.Target = activeHuman.Transformation.Translate + cameraUpOffset;
				activeCamera.Position += activeHuman.Transformation.Translate - oldPosition;

				//ensure that player touches ground
				UpdatePlayerHeight();
			}
			////Steering player: rotate left
			if (inputHelper.IsKeyPressed(Keys.A))
			{
				//rotate model
                activeHuman.model.Rotate += new Vector3(0, 3F, 0);
				activeCamera.Rotate -= 3;
				needUpdateCameraRotation = true;

				//calculate new human heading vector
				float sin = (float)Math.Sin(MathHelper.ToRadians(activeHuman.model.Rotate.Y));
				float cos = (float)Math.Cos(MathHelper.ToRadians(activeHuman.model.Rotate.Y));
				activeHuman.direction = new Vector3(sin, 0, cos);
			}
			////Steering player: rotate right
			if (inputHelper.IsKeyPressed(Keys.D))
			{
				//rotate model
                activeHuman.model.Rotate += new Vector3(0, -3F, 0);
				activeCamera.Rotate += 3;
				needUpdateCameraRotation = true;

				//calculate new human heading vector
				float sin = (float)Math.Sin(MathHelper.ToRadians(activeHuman.model.Rotate.Y));
				float cos = (float)Math.Cos(MathHelper.ToRadians(activeHuman.model.Rotate.Y));
				activeHuman.direction = new Vector3(sin, 0, cos);
			}
			//Steering player: generate smoke
			if (inputHelper.IsKeyJustPressed(Keys.F))
			{
				ScenarioBuilder.PutSmoke(new Vector3(activeHuman.model.Translate.X + 0.15f * activeHuman.direction.X,
										0.45f,	
										activeHuman.model.Translate.Z + 0.15f * activeHuman.direction.Z), 0.8f);
				simScenario.eventSent = false;
			}
			//Steering player: Put fire
			if (inputHelper.IsKeyJustPressed(Keys.R))
			{
				ScenarioBuilder.PutFire(activeHuman.model.Translate + 0.35f * activeHuman.direction);
			}
			//Steering player: interact with nearby object
			if (inputHelper.IsKeyJustPressed(Keys.Enter))
			{
				simScenario.InteractWithObject(activeHuman.model, 0.7f);
			}

			//move camera around the player
			if (needUpdateCameraRotation)
			{
				//calculate camera radius basing on human location and rotation
				float radius = (float)Math.Sqrt(Math.Pow(activeCamera.Position.X - activeHuman.Transformation.Translate.X, 2) +
					Math.Pow(activeCamera.Position.Z - activeHuman.Transformation.Translate.Z, 2));

				//change camera position
				activeCamera.Position = new Vector3(
					(float)Math.Cos(MathHelper.ToRadians(activeCamera.Rotate)) * radius + activeCamera.Target.X,
					activeCamera.Position.Y,
					(float)Math.Sin(MathHelper.ToRadians(activeCamera.Rotate)) * radius + activeCamera.Target.Z);
			}
		}
		
		/// <summary>
		/// Update all objects visible on the simulation screen
		/// </summary>
		/// <param name="gameTime">Time elapsed since the last call to Update</param>
		public override void Update(GameTime gameTime)
		{
			//update all 3d objects
			simScenario.UpdateHumans(gameTime);
			simScenario.UpdateStaticObjects(gameTime);
			simScenario.UpdateMovingObjects(gameTime);
			simScenario.UpdatePobicosObjects(gameTime);

			//update lighting
			simScenario.UpdateLights(gameTime);

			//handle keyboard input
			inputHelper.Update();
			UpdateInput();

			//check if smoke is present on the game screen
			DetectSmoke(gameTime);

			//base.Update(gameTime);
		}

		/// <summary>
		/// Automaticly changes Human position along the Y axis (height over the ground)
		/// </summary>
		/// <remarks>Thanks to this method Human is available to walk constantly on the ground. Height is updated while walking on stairs,
		/// ground or floor in home depending on X and Y parameters (Human location)</remarks>
		private void UpdatePlayerHeight()
		{
			Vector3 pos = simScenario.GetActiveHuman().model.Translate;
			float factor, offset;

			if (pos.X < -5 || pos.X > 6 || pos.Z < -8 || pos.Z > 1.1f)
				pos.Y = 0;
			else
				pos.Y = 0.3f;

			//garage
			factor = 1.5f;
			if (pos.Z > 0 && pos.Z < factor && pos.X < -1.9f && pos.X> -5.1f)
				pos.Y = (factor - pos.Z)/factor * 0.3f;

			//entrance
			factor = 0.5f;
			offset = 1.1f;
			if (pos.Z > offset && pos.Z < offset + factor && pos.X < 2.44f && pos.X > 1.2f)
				pos.Y = (factor - pos.Z +offset) / factor * 0.3f;

			//balcony up
			factor = -0.5f;
			offset = -7.9f;
			if (pos.Z > offset + factor && pos.Z < offset && pos.X < 6.1f && pos.X > 2.27f)
				pos.Y = (factor - pos.Z + offset) / factor * 0.3f;

			//balcony side
			factor = 0.5f;
			offset = 5.9f;
			if (pos.X > offset && pos.X < offset + factor && pos.Z > -8.02f && pos.Z < -5.23f)
				pos.Y = (factor - pos.X + offset) / factor * 0.3f;

			simScenario.GetActiveHuman().model.Translate = pos;
		}

		/// <summary>
		/// Checks if smoke is present in simulator environment
		/// </summary>
		/// <remarks>If smoke is present, method checks if it should be detected by Smoke Sensors. 
		/// It is assessed by distance or intersection</remarks>
		/// <param name="gameTime">Time elapsed since the last call to Update</param>
		private void DetectSmoke(GameTime gameTime)
		{
			//iterate all moving objects
			foreach (SimObject smoke in SimScenario.movingObjectList)
				//check SimObject name
				if (smoke.name.Contains("smoke"))
				{
					//if smoke is present get the smoke sensors
					SmokeSensor sensor = ((SmokeSensor)simScenario.GetPobicosObjectByName("SmokeSensor", Room.Living));
					SmokeSensor sensorGarage = ((SmokeSensor)simScenario.GetPobicosObjectByName("SmokeSensor", Room.Garage));

					if (sensor != null)
						//check intersection: smoke with smoke sensor in living room and smoke sensor in garage
						if (CheckIntersection(smoke.model, sensor.model, 0.5f) && !simScenario.eventSent)
						{
							if (POBICOS.enablePerformanceLog) 
								Trace.TraceInformation("Performance;" + (DateTime.Now - POBICOS.timeStarted) + ";Fire Detected;Smoke");
							//if intersects and event was not recently sent
							if (Math.Abs(gameTime.TotalGameTime.Seconds - sensor.lastEventTime.Seconds) > 5)
							{
								//generate smoke event for POBICOS
								SimScenario.Client.Event((SmokeSensor)simScenario.GetPobicosObjectByName("SmokeSensor", Room.Living), EventsList.SmokeEvent, sensor.EventID.ToString(), null);
								if (POBICOS.enablePerformanceLog) 
									Trace.TraceInformation("Performance;" + (DateTime.Now - POBICOS.timeStarted) + ";Event Sent;Smoke Event");
								sensor.lastEventTime = gameTime.TotalGameTime;
							}
							simScenario.eventSent = true;
						}

					if (sensorGarage != null)
						if (CheckIntersection(smoke.model, sensorGarage.model, 0.5f) && !simScenario.eventSent)
						{
							if (POBICOS.enablePerformanceLog) 
								Trace.TraceInformation("Performance;" + (DateTime.Now - POBICOS.timeStarted) + ";Fire Detected;Smoke");
							//if intersects and event was not recently sent
							if (Math.Abs(gameTime.TotalGameTime.Seconds - sensorGarage.lastEventTime.Seconds) > 5)
							{
								//generate smoke event for POBICOS
								SimScenario.Client.Event(sensorGarage, EventsList.SmokeDetected, sensorGarage.EventID.ToString(), null);
								if (POBICOS.enablePerformanceLog) 
									Trace.TraceInformation("Performance;" + (DateTime.Now - POBICOS.timeStarted) + ";Event Sent;Smoke Event");
								sensorGarage.lastEventTime = gameTime.TotalGameTime;
							}
							simScenario.eventSent = true;
						}
				}
		}

		/// <summary>
		/// Checks if two models intersects with each other
		/// </summary>
		/// <remarks>Method is checking if bounding boxes of models are intersecting when <typeparamref name="tolerance"/> 
		/// is equal or less than zero. 
		/// If <typeparamref name="tolerance"/> is greater than zero method checks if distance (on the Y axis) between models is lower than the
		/// <typeparamref name="tolerance"/> value</remarks>
		/// <param name="m1">Model to be tested</param>
		/// <param name="m2">Model to be tested</param>
		/// <param name="tolerance">If <typeparamref name="tolerance"/> value is less than or equal zero then bounding boxes 
		/// intersection is checked. Else <typeparamref name="tolerance"/> specifies maximum distance between Models that is tolerated (on Y axis).</param>
		/// <returns>true if models intersects or distance betwwen them is lower than <typeparamref name="tolerance"/> value. Else returns false.</returns>
		public static bool CheckIntersection(SimModel m1, SimModel m2, float tolerance)
		{
			if (tolerance <= 0)	//strictly check intersection
				return m1.BoundingBox.Intersects(m2.BoundingBox);
			//else check intersection but with specified tolerance
			else
				if (Math.Abs(m1.Translate.Y - m2.Translate.Y) < tolerance / 2)
					return Vector3.Distance(m1.Translate, m2.Translate) < tolerance;
				else
					return false;
		}

		/// <summary>
		/// Checks if distance between model's Y coordinate and <paramref name="m2Translation"/> is less than <paramref name="tolerance"/> parameter value.
		/// </summary>
		/// <param name="m1">Model to be tested</param>
		/// <param name="m2Translation"><o>Vector3</o> forw hich Y coordinate will be taken to test</param>
		/// <param name="tolerance">maximum distance allowed betwwen parameters Y coordinates</param>
		/// <returns>true if distance is lower than <paramref name="tolerance"/>, else returns false.</returns>
		public static bool CheckIntersection(SimModel m1, Vector3 m2Translation, float tolerance)
		{
			//check intersection with specified tolerance
			if (Math.Abs(m1.Translate.Y - m2Translation.Y) < tolerance / 2)
				return Vector3.Distance(m1.Translate, m2Translation) < tolerance;
			else
				return false;
		}

		/// <summary>
		/// Called When <o>DrawableGameComponent</o> needs to be drawn
		/// </summary>
		/// <param name="gameTime">Time elapsed since the last call to Update</param>
		public override void Draw(GameTime gameTime)
		{
			//clear screen
			GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.White, 1.0f, 255);

			//draw all 3D objects
			simScenario.DrawHumans(gameTime);
			simScenario.DrawStaticObjects(gameTime);
			simScenario.DrawMovingObjects(gameTime);
			simScenario.DrawPobicosObjects(gameTime);

			//base.Draw(gameTime);
		}
	}
}
