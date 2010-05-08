﻿using POBICOS.Helpers;
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
				if (simScenario.GetObjectByName("smoke") == null)
				{
					ScenarioBuilder.PutSmoke(Game, activeHuman.model.Translate);
					simScenario.eventSent = false;
				}
			}
			if (inputHelper.IsKeyJustPressed(Keys.R))
			{
				//if (simScenario.GetObjectByName("Fire") == null)
					ScenarioBuilder.PutFire(Game, activeHuman.model.Translate);
			}
			if (inputHelper.IsKeyJustPressed(Keys.L))
			{
				PobicosLamp lamp = simScenario.GetPobicosObjectByName("lampOn");
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

			SimObject smoke = simScenario.GetObjectByName("smoke");
			SimObject sensor = simScenario.GetObjectByName("SmokeSensor");

			if(smoke!=null && sensor !=null)
				if (CheckIntersection(smoke.model, sensor.model) && !simScenario.eventSent)
				{
					SimScenario.client.Event((SmokeSensor)simScenario.GetObjectByName("SmokeSensor"), EventsList.SmokeEvent, "666", null);
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

			return so1BB.Intersects(so2BB);
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
