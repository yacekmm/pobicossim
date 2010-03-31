using System.Collections.Generic;
using Microsoft.Xna.Framework;
using POBICOS.SimBase.Cameras;
using POBICOS.SimBase.Lights;
using System;
using Microsoft.Xna.Framework.Content;
using POBICOS.SimBase;

namespace POBICOS.SimLogic.Scenarios
{
	class ScenarioBuilder
	{
		public enum Scenarios
		{ 
			Flat
		}

		public static SimScenario BuildScenario(Game game, Scenarios scenario)
		{
			game.Services.RemoveService(typeof(CameraManager));
			game.Services.RemoveService(typeof(LightManager));

			switch (scenario)
			{
				case Scenarios.Flat:
					return CreateFlatScenario(game);

				default:
					throw new ArgumentException("Invalid simulation scenario");
			}
		}

		public static SimScenario CreateFlatScenario(Game game)
		{
			ContentManager content = game.Content;
			SimScenario simScenario = new SimScenario();

			//Cameras and Lights
			AddCameras(game, ref simScenario);
			
			simScenario.lightManager = new LightManager();
			simScenario.lightManager.AmbientLightColor = new Vector3(1.0f, 1.0f, 1.0f);
			simScenario.lightManager.Add("MainLight",
				new PointLight(new Vector3(1, 3, -1), new Vector3(0.2f, 0.2f, 0.2f)));
			simScenario.lightManager.Add("CameraLight",
				new PointLight(Vector3.Zero, Vector3.One));

			game.Services.AddService(typeof(CameraManager), simScenario.cameraManager);
			game.Services.AddService(typeof(LightManager), simScenario.lightManager);

			//Human
			Human human = new Human(game, "Sphere6");
			human.isActive = true;
			human.Transformation = new Transformation(new Vector3(2.0f, 0.0f, -2.0f), new Vector3(0.0f, 0.0f, 0.0f), new Vector3(1.0f, 1.0f, 1.0f));
			human.Initialize();

			simScenario.humanList.Add(human);

			BuildStaticObjects(game, ref simScenario);

			return simScenario;
		}

		private static void BuildStaticObjects(Game game, ref SimScenario simScenario)
		{
			float roomSizeX = 6.0f;
			float roomSizeZ = 4.0f;
			float roomSizeY = 1.0f;

			SimObject floor = new SimObject(game, "floor");
			floor.Transformation = new Transformation(	Vector3.Zero, 
														Vector3.Zero, 
														new Vector3(roomSizeX, 1.0f, roomSizeZ));
			floor.Initialize();
			simScenario.staticObjectList.Add(floor);

			SimObject wall1 = new SimObject(game, "Wall");
			wall1.Transformation = new Transformation(new Vector3(0.0f, 0.0f, -roomSizeZ),
														Vector3.Zero,
														new Vector3(roomSizeX, roomSizeY, 1.0f));
			wall1.Initialize();
			simScenario.staticObjectList.Add(wall1);

			SimObject wall2 = new SimObject(game, "Wall");
			wall2.Transformation = new Transformation(	Vector3.Zero, 
														new Vector3(0.0f, 90.0f, 0.0f),
														new Vector3(roomSizeZ, roomSizeY, 1.0f));
			wall2.Initialize();
			simScenario.staticObjectList.Add(wall2);

			SimObject wall3 = new SimObject(game, "wall_windows_6");
			wall3.Transformation = new Transformation(	Vector3.Zero,
														Vector3.Zero,
														Vector3.One);// (roomSizeX, roomSizeY, 1.0f));
			wall3.Initialize();
			simScenario.staticObjectList.Add(wall3);

			SimObject wall4 = new SimObject(game, "Wall");
			wall4.Transformation = new Transformation(new Vector3(roomSizeX, 0.0f, 0.0f),
														new Vector3(0.0f, 90.0f, 0.0f),
														new Vector3(roomSizeZ, roomSizeY, 1.0f));
			wall4.Initialize();
			simScenario.staticObjectList.Add(wall4);

			SimObject smokeSensor = new SimObject(game, "SmokeSensor");
			//smokeSensor.name = "SmokeSensor";
			smokeSensor.Transformation = new Transformation(new Vector3(roomSizeX / 2, roomSizeY, -roomSizeZ / 2),
														new Vector3(0.0f, 0.0f, 0.0f),
														new Vector3(0.08f, 0.05f, 0.08f));
			smokeSensor.Initialize();
			simScenario.staticObjectList.Add(smokeSensor);

			SimObject tv = new SimObject(game, "tv");
			tv.Transformation = new Transformation(new Vector3(roomSizeX - (0.5f / 2), 0.0f, -roomSizeZ / 2),
														new Vector3(0.0f, -90.0f, 0.0f),
														new Vector3(0.2f, 0.2f, 0.2f));
			tv.Initialize();
			simScenario.staticObjectList.Add(tv);

			PobicosSimObject lamp = new PobicosSimObject(game, "lampOn", SimAssetsPath.POBICOS_OBJECTS_PATH + "LampTestTossim_res.xml");
			lamp.Transformation = new Transformation(new Vector3(roomSizeX / 2, 0.0f, -roomSizeZ + 0.35f),
														new Vector3(0.0f, -90.0f, 0.0f),
														new Vector3(0.15f, 0.15f, 0.15f));
			lamp.objectState = PobicosSimObject.ObjectState.OFF;
			lamp.Initialize();
           // PobicosSimObject ppo = new 	PobicosSimObject(game, "untitled", SimAssetsPath.POBICOS_OBJECTS_PATH + "LampTestTossim_res.xml");
			//ppo.Transformation = new Transformation(new Vector3(roomSizeX / 2, 0.0f, -roomSizeZ + 0.35f),
			//											new Vector3(0.0f, -90.0f, 0.0f),
			//											new Vector3(0.15f, 0.15f, 0.15f));
           // ppo.Initialize();
            //simScenario.pobicosObjectList.Add(ppo);
			simScenario.pobicosObjectList.Add(lamp);
		}

		public static void PutFire(Game game, Vector3 position, ref SimScenario simScenario)
		{
			SimObject smoke = new SimObject(game, "smoke");
			smoke.Transformation = new Transformation(position, Vector3.Zero, new Vector3(0.05f, 0.05f, 0.05f));
			smoke.Initialize();
			simScenario.movingObjectList.Add(smoke);
		}
		
		public static void AddCameras(Game game, ref SimScenario simScenario)
		{
            float aspectRatio = (float)game.GraphicsDevice.Viewport.Width / 500; // game.GraphicsDevice.Viewport.Height;

			ThirdPersonCamera followCamera = new ThirdPersonCamera();
			followCamera.SetPerspectiveFov(60.0f, aspectRatio, 0.1f, 700);
			followCamera.SetChaseParameters(3.0f, 9.0f, 7.0f, 14.0f);
			followCamera.SetLookAt(new Vector3(2.0f, 4.0f, 2.5f), new Vector3(2.0f, 0.0f, -2.0f), Vector3.Up);

			simScenario.cameraManager = new CameraManager();
			simScenario.cameraManager.Add("FollowCamera", followCamera);
		}
	}
}
