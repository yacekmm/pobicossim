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
			//simScenario.UnloadPobicosObjects();
			SimScenario.client.Disconnect();
		}

		private void UpdateInput()
		{
			Human activeHuman = simScenario.GetActiveHuman();

			float cameraSpeed = 0.03f;
			Vector3 cameraUpOffset = simScenario.cameraUpOffset;

			if (inputHelper.IsKeyPressed(Keys.Up))
				simScenario.cameraManager.ActiveCamera.Position += Vector3.Up * cameraSpeed;
            if (inputHelper.IsKeyPressed(Keys.Down))
            {
                simScenario.cameraManager.ActiveCamera.Position += Vector3.Down * cameraSpeed;
				if (simScenario.cameraManager.ActiveCamera.Position.Y < 1.4f)
					simScenario.cameraManager.ActiveCamera.Position -= Vector3.Down * cameraSpeed;
            }
            if (inputHelper.IsKeyPressed(Keys.Left))
                simScenario.cameraManager.ActiveCamera.Position += Vector3.Left * cameraSpeed;
            if (inputHelper.IsKeyPressed(Keys.Right))
                simScenario.cameraManager.ActiveCamera.Position += Vector3.Right * cameraSpeed;
            if (inputHelper.IsKeyPressed(Keys.PageUp))
                simScenario.cameraManager.ActiveCamera.Position += Vector3.Forward * cameraSpeed;
            if (inputHelper.IsKeyPressed(Keys.PageDown))
                simScenario.cameraManager.ActiveCamera.Position += Vector3.Backward * cameraSpeed;
            if (inputHelper.IsKeyPressed(Keys.W))
            {
                float sin = (float)Math.Sin(activeHuman.model.Rotate.Y * Math.PI / 180);
                float cos = (float)Math.Cos(activeHuman.model.Rotate.Y * Math.PI / 180);
                Vector3 direction = new Vector3(sin, 0, cos);
                CollsionChecker.Move(ref activeHuman, direction);
               // activeHuman.model.Translate += direction * activeHuman.movementSpeed;
                simScenario.cameraManager.ActiveCamera.Target = activeHuman.Transformation.Translate + cameraUpOffset;

				UpdatePlayerHeight();
            }
			if (inputHelper.IsKeyPressed(Keys.S))
			{
                float sin = (float)Math.Sin(activeHuman.model.Rotate.Y * Math.PI / 180);
                float cos = (float)Math.Cos(activeHuman.model.Rotate.Y * Math.PI / 180);
                Vector3 direction = new Vector3(-sin, 0, -cos);

                activeHuman.model.Translate += direction * activeHuman.movementSpeed;
				simScenario.cameraManager.ActiveCamera.Target = activeHuman.Transformation.Translate + cameraUpOffset;

				UpdatePlayerHeight();
			}
			 if (inputHelper.IsKeyPressed(Keys.A))
			{
                activeHuman.model.Rotate += new Vector3(0, 3F, 0);
				simScenario.cameraManager.ActiveCamera.Target = activeHuman.Transformation.Translate + cameraUpOffset;

				UpdatePlayerHeight();
			}
			if (inputHelper.IsKeyPressed(Keys.D))
			{
                activeHuman.model.Rotate += new Vector3(0, -3F, 0);
				simScenario.cameraManager.ActiveCamera.Target = activeHuman.Transformation.Translate + cameraUpOffset;

				UpdatePlayerHeight();
			}
			if (inputHelper.IsKeyJustPressed(Keys.F))
			{
				ScenarioBuilder.PutSmoke(Game, activeHuman.model.Translate, 1);
				simScenario.eventSent = false;
			}
			if (inputHelper.IsKeyJustPressed(Keys.R))
			{
				ScenarioBuilder.PutFire(Game, activeHuman.model.Translate);
			}
			if (inputHelper.IsKeyJustPressed(Keys.L))
			{
				PobicosLamp lamp = (PobicosLamp)simScenario.GetPobicosObjectByName("lampOn");
				SimScenario.client.Event(lamp, EventsList.ponge_originated_event_switch_originated_event, null, null);
				if (lamp.objectState.Equals(PobicosLamp.ObjectState.OFF))
				{
					lamp.objectState = PobicosLamp.ObjectState.ON;
					simScenario.SwitchLight(lamp.model.room, true);
				}
				else if (lamp.objectState.Equals(PobicosLamp.ObjectState.ON))
				{
					lamp.objectState = PobicosLamp.ObjectState.OFF;
					simScenario.SwitchLight(lamp.model.room, false);
				}
			}
			if (inputHelper.IsKeyJustPressed(Keys.T))
			{
				((Tv)simScenario.GetPobicosObjectByName("tv_v3", Room.Living)).Switch();
				((Tv)simScenario.GetPobicosObjectByName("tv_v3", Room.Bedroom)).Switch();
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
			SmokeSensor sensor = ((SmokeSensor)simScenario.GetPobicosObjectByName("SmokeSensor", Room.Living));
			SmokeSensor sensorGarage = ((SmokeSensor)simScenario.GetPobicosObjectByName("SmokeSensor", Room.Garage));

			foreach (SimObject smoke in SimScenario.movingObjectList)
				if (smoke.name.Contains("smoke"))
				{
					if (sensor != null)
						if (CheckIntersection(smoke.model, sensor.model, 0.5f) && !simScenario.eventSent)
						{
							if (gameTime.TotalGameTime.Seconds - sensor.lastEventTime.Seconds > 8)
							{
								SimScenario.client.Event((SmokeSensor)simScenario.GetPobicosObjectByName("SmokeSensor", Room.Living), EventsList.SmokeEvent, "666", null);
								sensor.lastEventTime = gameTime.TotalGameTime;
							}
							simScenario.eventSent = true;
						}

					if (sensorGarage != null)
						if (CheckIntersection(smoke.model, sensorGarage.model, 0.5f) && !simScenario.eventSent)
						{
							if (gameTime.TotalGameTime.Seconds - sensorGarage.lastEventTime.Seconds > 8)
							{
								SimScenario.client.Event(sensorGarage, EventsList.SmokeDetected, "111", null);
								sensorGarage.lastEventTime = gameTime.TotalGameTime;
							}
							simScenario.eventSent = true;
						}
				}
		}

		private bool CheckIntersection(SimModel m1, SimModel m2, float tolerance)
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
