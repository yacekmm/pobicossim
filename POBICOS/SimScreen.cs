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
			simScenario.client.Disconnect();
		}

		private void UpdateInput()
		{
			Human activeHuman = simScenario.GetActiveHuman();

			float cameraSpeed = 0.03f;

			if (inputHelper.IsKeyPressed(Keys.Up))
				simScenario.cameraManager.ActiveCamera.Position += Vector3.Up * cameraSpeed;
            if (inputHelper.IsKeyPressed(Keys.Down))
            {
                simScenario.cameraManager.ActiveCamera.Position += Vector3.Down * cameraSpeed;
                if (simScenario.cameraManager.ActiveCamera.Position.Y < 0)
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
                simScenario.cameraManager.ActiveCamera.Target = activeHuman.Transformation.Translate;
            }
			if (inputHelper.IsKeyPressed(Keys.S))
			{
                float sin = (float)Math.Sin(activeHuman.model.Rotate.Y * Math.PI / 180);
                float cos = (float)Math.Cos(activeHuman.model.Rotate.Y * Math.PI / 180);
                Vector3 direction = new Vector3(-sin, 0, -cos);



                activeHuman.model.Translate += direction * activeHuman.movementSpeed;
				               
				simScenario.cameraManager.ActiveCamera.Target = activeHuman.Transformation.Translate;
			}
			 if (inputHelper.IsKeyPressed(Keys.A))
			{
                activeHuman.model.Rotate += new Vector3(0, 3F, 0);
				//activeHuman.model.Translate += Vector3.Left * activeHuman.movementSpeed;
				simScenario.cameraManager.ActiveCamera.Target = activeHuman.Transformation.Translate;
			}
			if (inputHelper.IsKeyPressed(Keys.D))
			{
                activeHuman.model.Rotate += new Vector3(0, -3F, 0);
				//activeHuman.model.Translate += Vector3.Right * activeHuman.movementSpeed;
				simScenario.cameraManager.ActiveCamera.Target = activeHuman.Transformation.Translate;
			}
			if (inputHelper.IsKeyJustPressed(Keys.F))
			{
				if (simScenario.GetObjectByName("smoke") == null)
				{
					ScenarioBuilder.PutFire(Game, activeHuman.model.Translate);
					simScenario.eventSent = false;
				}
			}
			if (inputHelper.IsKeyJustPressed(Keys.L))
			{
				PobicosLamp lamp = simScenario.GetPobicosObjectByName("lampOn");
				simScenario.client.Event(lamp, EventsList.PONGE_ORIGINATED_EVENT_SWITCH_ORIGINATED_EVENT, null, null);
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
		}
		
		public override void Update(GameTime gameTime)
		{
			simScenario.UpdateHumans(gameTime);
			simScenario.UpdateStaticObjects(gameTime);
			simScenario.UpdateMovingObjects(gameTime);
			simScenario.UpdatePobicosObjects(gameTime);

			inputHelper.Update();
			UpdateInput();

			SimObject smoke = simScenario.GetObjectByName("smoke");
			SimObject sensor = simScenario.GetObjectByName("SmokeSensor");

			if(smoke!=null && sensor !=null)
				if (CheckIntersection(smoke.model, sensor.model) && !simScenario.eventSent)
				{
					simScenario.client.Event((SmokeSensor)simScenario.GetObjectByName("SmokeSensor"), EventsList.smokeEvent, "666", null);
					simScenario.eventSent = true;
				}

			base.Update(gameTime);
		}

		private bool CheckIntersection(SimModel m1, SimModel m2)
		{
			BoundingBox so1BB;
			so1BB.Min = Vector3.Transform(m1.modelBoundingBox.Min, m1.Transformation.Matrix);
			so1BB.Max = Vector3.Transform(m1.modelBoundingBox.Max, m1.Transformation.Matrix);

			BoundingBox so2BB;
			so2BB.Min = Vector3.Transform(m2.modelBoundingBox.Min, m2.Transformation.Matrix);
			so2BB.Max = Vector3.Transform(m2.modelBoundingBox.Max, m2.Transformation.Matrix);

			if (so1BB.Intersects(so2BB))
			{
				simScenario.GetObjectByName("tv").Transformation.Rotate += (new Vector3(0.0f, 2.0f, 0.0f));
				return true;
			}
			else return false;
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
