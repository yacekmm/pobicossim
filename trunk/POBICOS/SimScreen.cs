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

namespace POBICOS
{
	class SimScreen : DrawableGameComponent
	{
		ScenarioBuilder.Scenarios currentScenario;
		SimScenario simScenario;

		InputHelper inputHelper;

		public SimScreen(Game game, ScenarioBuilder.Scenarios scenario)
			: base(game)
		{
			this.currentScenario = scenario;
		}

		public override void Initialize()
		{
			inputHelper = Game.Services.GetService(typeof(InputHelper)) as InputHelper;
			if (inputHelper == null)
				throw new InvalidOperationException("Cannot find an input service");

			base.Initialize();
		}

		protected override void LoadContent()
		{
			simScenario = ScenarioBuilder.BuildScenario(Game, currentScenario);
			
			base.LoadContent();
		}

		protected override void UnloadContent()
		{
			base.UnloadContent();
			SimScenario.Client.Disconnect();
		}

		private void UpdateInput()
		{
			Human activeHuman = simScenario.GetActiveHuman();
			ThirdPersonCamera activeCamera = (ThirdPersonCamera)simScenario.cameraManager.ActiveCamera;

			float cameraSpeed = 0.02f;
			Vector3 cameraUpOffset = simScenario.cameraUpOffset;

			bool needUpdateCameraRotation = false;

			if (inputHelper.IsKeyPressed(Keys.PageUp))
			{
				Vector3 direction = activeHuman.Transformation.Translate - activeCamera.Position;
				activeCamera.Position += direction * cameraSpeed;
				if (activeCamera.Position.Y < 1.6f)
					activeCamera.Position -= direction * cameraSpeed;
			}
            if (inputHelper.IsKeyPressed(Keys.PageDown))
            {
				Vector3 direction = activeHuman.Transformation.Translate - activeCamera.Position;
				activeCamera.Position -= direction * cameraSpeed;
				if (activeCamera.Position.Y < 1.6f)
					activeCamera.Position += direction * cameraSpeed;
            }
			if (inputHelper.IsKeyPressed(Keys.Left))
			{
				activeCamera.Rotate += 1.5f;
				needUpdateCameraRotation = true;
			}
			if (inputHelper.IsKeyPressed(Keys.Right))
			{
				activeCamera.Rotate -= 1.5f;
				needUpdateCameraRotation = true;
			}
			if (inputHelper.IsKeyPressed(Keys.Up))
			{
				activeCamera.Position += Vector3.Up * cameraSpeed * 2;
			}
			if (inputHelper.IsKeyPressed(Keys.Down))
			{
				activeCamera.Position += Vector3.Down * cameraSpeed * 2;
				if (activeCamera.Position.Y < 1.6f)
					activeCamera.Position -= Vector3.Down * cameraSpeed*2;
			}
            if (inputHelper.IsKeyPressed(Keys.W))
            {
				Vector3 oldPosition = activeHuman.Transformation.Translate;
                CollsionChecker.Move(ref activeHuman, true);
                activeCamera.Target = activeHuman.Transformation.Translate + cameraUpOffset;
				activeCamera.Position += activeHuman.Transformation.Translate - oldPosition;

				UpdatePlayerHeight();
            }
			if (inputHelper.IsKeyPressed(Keys.S))
			{
				Vector3 oldPosition = activeHuman.Transformation.Translate;
                CollsionChecker.Move(ref activeHuman, false);
				activeCamera.Target = activeHuman.Transformation.Translate + cameraUpOffset;
				activeCamera.Position += activeHuman.Transformation.Translate - oldPosition;

				UpdatePlayerHeight();
			}
			 if (inputHelper.IsKeyPressed(Keys.A))
			{
                activeHuman.model.Rotate += new Vector3(0, 3F, 0);
				activeCamera.Rotate -= 3;
				needUpdateCameraRotation = true;

				float sin = (float)Math.Sin(MathHelper.ToRadians(activeHuman.model.Rotate.Y));
				float cos = (float)Math.Cos(MathHelper.ToRadians(activeHuman.model.Rotate.Y));
				activeHuman.direction = new Vector3(sin, 0, cos);

				UpdatePlayerHeight();
			}
			if (inputHelper.IsKeyPressed(Keys.D))
			{
                activeHuman.model.Rotate += new Vector3(0, -3F, 0);
				activeCamera.Rotate += 3;
				needUpdateCameraRotation = true;

				float sin = (float)Math.Sin(MathHelper.ToRadians(activeHuman.model.Rotate.Y));
				float cos = (float)Math.Cos(MathHelper.ToRadians(activeHuman.model.Rotate.Y));
				activeHuman.direction = new Vector3(sin, 0, cos);

				UpdatePlayerHeight();
			}
			if (inputHelper.IsKeyJustPressed(Keys.F))
			{
				ScenarioBuilder.PutSmoke(new Vector3(activeHuman.model.Translate.X + 0.15f * activeHuman.direction.X,
										0.45f,	
										activeHuman.model.Translate.Z + 0.15f * activeHuman.direction.Z), 0.8f);
				simScenario.eventSent = false;
			}
			if (inputHelper.IsKeyJustPressed(Keys.R))
			{
				ScenarioBuilder.PutFire(activeHuman.model.Translate + 0.35f * activeHuman.direction);
			}
			if (inputHelper.IsKeyJustPressed(Keys.Enter))
			{
				simScenario.InteractWithObject(activeHuman.model, 0.7f);
			}
			if (inputHelper.IsKeyJustPressed(Keys.C))
			{
				if (SimScenario.Client.Running)
					SimScenario.Client.Disconnect();
				else
					SimScenario.Client.Connect();
			}
			if (inputHelper.IsKeyJustPressed(Keys.M))
			{
				PobicosLamp lamp;
				lamp = (PobicosLamp)simScenario.GetPobicosObjectByName("lampOn");
				Console.WriteLine(lamp.model.basicEffectManager.Light0Direction.Length().ToString());
			}

			if (needUpdateCameraRotation)
			{
				float radius = (float)Math.Sqrt(Math.Pow(activeCamera.Position.X - activeHuman.Transformation.Translate.X, 2) +
					Math.Pow(activeCamera.Position.Z - activeHuman.Transformation.Translate.Z, 2));

				activeCamera.Position = new Vector3(
					(float)Math.Cos(MathHelper.ToRadians(activeCamera.Rotate)) * radius + activeCamera.Target.X,
					activeCamera.Position.Y,
					(float)Math.Sin(MathHelper.ToRadians(activeCamera.Rotate)) * radius + activeCamera.Target.Z);
			}
		}
		
		public override void Update(GameTime gameTime)
		{
			simScenario.UpdateHumans(gameTime);
			simScenario.UpdateStaticObjects(gameTime);
			simScenario.UpdateMovingObjects(gameTime);
			simScenario.UpdatePobicosObjects(gameTime);
			simScenario.UpdateLights(gameTime);

			inputHelper.Update();
			UpdateInput();
			DetectSmoke(gameTime);

			base.Update(gameTime);
		}

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

		private void DetectSmoke(GameTime gameTime)
		{
			foreach (SimObject smoke in SimScenario.movingObjectList)
				if (smoke.name.Contains("smoke"))
				{
					SmokeSensor sensor = ((SmokeSensor)simScenario.GetPobicosObjectByName("SmokeSensor", Room.Living));
					SmokeSensor sensorGarage = ((SmokeSensor)simScenario.GetPobicosObjectByName("SmokeSensor", Room.Garage));

					if (sensor != null)
						if (CheckIntersection(smoke.model, sensor.model, 0.5f) && !simScenario.eventSent)
						{
							if (Math.Abs(gameTime.TotalGameTime.Seconds - sensor.lastEventTime.Seconds) > 5)
							//if (gameTime.TotalGameTime.Seconds - sensor.lastEventTime.Seconds > 5)
							{
								SimScenario.Client.Event((SmokeSensor)simScenario.GetPobicosObjectByName("SmokeSensor", Room.Living), EventsList.SmokeEvent, sensor.EventID.ToString(), null);
								sensor.lastEventTime = gameTime.TotalGameTime;
							}
							simScenario.eventSent = true;
						}

					if (sensorGarage != null)
						if (CheckIntersection(smoke.model, sensorGarage.model, 0.5f) && !simScenario.eventSent)
						{
							if (Math.Abs(gameTime.TotalGameTime.Seconds - sensorGarage.lastEventTime.Seconds) > 5)
							//if (gameTime.TotalGameTime.Seconds - sensorGarage.lastEventTime.Seconds > 5)
							{
								SimScenario.Client.Event(sensorGarage, EventsList.SmokeDetected, sensorGarage.EventID.ToString(), null);
								sensorGarage.lastEventTime = gameTime.TotalGameTime;
							}
							simScenario.eventSent = true;
						}
				}
		}

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

		public static bool CheckIntersection(SimModel m1, Vector3 m2Translation, float tolerance)
		{
			//check intersection with specified tolerance
			if (Math.Abs(m1.Translate.Y - m2Translation.Y) < tolerance / 2)
				return Vector3.Distance(m1.Translate, m2Translation) < tolerance;
			else
				return false;
		}

		public override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(ClearOptions.DepthBuffer, Color.White, 1.0f, 255);

			simScenario.DrawHumans(gameTime);
			simScenario.DrawStaticObjects(gameTime);
			simScenario.DrawMovingObjects(gameTime);
			simScenario.DrawPobicosObjects(gameTime);

			base.Draw(gameTime);
		}
	}
}
