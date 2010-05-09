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

                activeHuman.model.Translate += direction * activeHuman.movementSpeed;
                simScenario.cameraManager.ActiveCamera.Target = activeHuman.Transformation.Translate + cameraUpOffset;
            }
			if (inputHelper.IsKeyPressed(Keys.S))
			{
                float sin = (float)Math.Sin(activeHuman.model.Rotate.Y * Math.PI / 180);
                float cos = (float)Math.Cos(activeHuman.model.Rotate.Y * Math.PI / 180);
                Vector3 direction = new Vector3(-sin, 0, -cos);

                activeHuman.model.Translate += direction * activeHuman.movementSpeed;
				               
				simScenario.cameraManager.ActiveCamera.Target = activeHuman.Transformation.Translate + cameraUpOffset;
			}
			 if (inputHelper.IsKeyPressed(Keys.A))
			{
                activeHuman.model.Rotate += new Vector3(0, 3F, 0);
				simScenario.cameraManager.ActiveCamera.Target = activeHuman.Transformation.Translate + cameraUpOffset;
			}
			if (inputHelper.IsKeyPressed(Keys.D))
			{
                activeHuman.model.Rotate += new Vector3(0, -3F, 0);
				simScenario.cameraManager.ActiveCamera.Target = activeHuman.Transformation.Translate + cameraUpOffset;
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
			{
				BoundingBox so1BB;
				so1BB.Min = Vector3.Transform(m1.modelBoundingBox.Min, m1.Transformation.Matrix);
				so1BB.Max = Vector3.Transform(m1.modelBoundingBox.Max, m1.Transformation.Matrix);

				BoundingBox so2BB;
				so2BB.Min = Vector3.Transform(m2.modelBoundingBox.Min, m2.Transformation.Matrix);
				so2BB.Max = Vector3.Transform(m2.modelBoundingBox.Max, m2.Transformation.Matrix);

				return so1BB.Intersects(so2BB);
			}
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
