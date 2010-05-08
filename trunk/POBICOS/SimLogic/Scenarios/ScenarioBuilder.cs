using System.Collections.Generic;
using Microsoft.Xna.Framework;
using POBICOS.SimBase.Cameras;
using POBICOS.SimBase.Lights;
using System;
using Microsoft.Xna.Framework.Content;
using POBICOS.SimBase;
using PobicosLibrary;
using POBICOS.SimBase.Effects;
using POBICOS.SimLogic.PobicosObjects;
using Microsoft.Xna.Framework.Graphics;

namespace POBICOS.SimLogic.Scenarios
{
	class ScenarioBuilder
	{
		public static EffectList testEffect = EffectList.Basic;

		public static SimScenario simScenario;

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
			simScenario = new SimScenario();

			AddHumans(game);

			//Cameras and Lights
			AddCameras(game);
			
			simScenario.lightManager = new LightManager();
			simScenario.lightManager.AmbientLightColor = new Vector3(1.0f, 1.0f, 1.0f);
			simScenario.lightManager.Add("MainLight",
				new PointLight(new Vector3(1, 3, -1), new Vector3(0.2f, 0.2f, 0.2f)));
			simScenario.lightManager.Add("CameraLight",
				new PointLight(Vector3.Zero, Vector3.One));

			game.Services.AddService(typeof(CameraManager), simScenario.cameraManager);
			game.Services.AddService(typeof(LightManager), simScenario.lightManager);
            
			game.Services.AddService(typeof(BasicEffectManager), simScenario.basicEffectManager);
			game.Services.AddService(typeof(SimScenario), simScenario);

			
			BuildStaticObjects(game);

			return simScenario;
		}

		private static void AddHumans(Game game)
		{
			//Human
			Human human = new Human(game, "Sphere6", testEffect, Room.Living);
			human.isActive = true;
			//human.Transformation = new Transformation(Vector3.Zero, Vector3.Zero, Vector3.One);
			human.Transformation = new Transformation(new Vector3(2.0f, 0.3f, -2.0f), new Vector3(0.0f, 0.0f, 0.0f), new Vector3(1.0f, 1.0f, 1.0f));
			human.Initialize();
			simScenario.humanList.Add(human);
		}


		private static void BuildStaticObjects(Game game)
		{
			float offsetY = 0.3f;
			BuildBedroomArea(game, offsetY);
			BuildLivingArea(game, offsetY);
            BuildDiningArea(game, offsetY);
            BuildGarageArea(game, offsetY);
            BuildKitchenArea(game, offsetY);
            BuildAnteroomArea(game, offsetY);
            BuildBathroomArea(game, offsetY);
            BuildOutsideArea(game, offsetY);

			SimScenario.client.Connect();
		}

		#region Create Building
		private static void BuildOutsideArea(Game game, float roomOffsetY)
		{
            #region skybox & ground
			SimObject skybox = new SimObject(game, SimAssetsPath.MODELS_ENVIRONMENT_PATH + "skybox", testEffect, Room.Outside);
            skybox.Transformation = new Transformation(Vector3.Zero, new Vector3(90,90,90), Vector3.One * 40);
            skybox.Initialize();
			skybox.model.basicEffectManager.Light0Direction = new Vector3(1, 0, -1);
			skybox.model.basicEffectManager.Light1Direction = new Vector3(-1, 0, 1);
			skybox.model.basicEffectManager.Light2Direction = new Vector3(0,1,0);
			skybox.model.basicEffectManager.SpecularPower = 100;
            simScenario.staticObjectList.Add(skybox);

            SimObject grass= new SimObject(game, SimAssetsPath.MODELS_ENVIRONMENT_PATH + "grass", testEffect, Room.Outside);
            grass.Transformation = new Transformation(Vector3.Zero, new Vector3(90, 180, 0), new Vector3(41));
            grass.Initialize();
            simScenario.staticObjectList.Add(grass);
            #endregion

            #region room dimensions
            float roomSizeX = 11;
			float roomSizeZ = 8;

			float roomOffsetX = -5;
			float roomOffsetZ = 0;

			float roomBorderX = roomSizeX + roomOffsetX;
			float roomBorderZ = -roomSizeZ + roomOffsetZ; 
			#endregion

			#region Walls & basement
			SimObject basement = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "basement_tex_v2", testEffect, Room.Outside);
			basement.Transformation = new Transformation(new Vector3(-5.0f, roomOffsetY - 0.3f, roomOffsetZ), Vector3.Zero, Vector3.One);
			basement.Initialize();
			basement.model.basicEffectManager.SpecularPower = 60;
			simScenario.staticObjectList.Add(basement);

			SimObject wall29 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_5_2_outside_base_total_tex_v4", testEffect, Room.Outside);
			wall29.Transformation = new Transformation(new Vector3(roomBorderX - 5, roomOffsetY, roomOffsetZ), new Vector3(0, 0, 0), Vector3.One);
			wall29.Initialize();
			wall29.model.basicEffectManager.SpecularPower = 60;
			simScenario.staticObjectList.Add(wall29);
			#endregion
		}

		private static void BuildBathroomArea(Game game, float roomOffsetY)
		{
			#region room dimensions
			float roomSizeX = 3;
			float roomSizeZ = 1;
			float roomSizeY = 1;

			float roomOffsetX = -2;
			float roomOffsetZ = 1;

			float roomBorderX = roomSizeX + roomOffsetX;
			float roomBorderZ = -roomSizeZ + roomOffsetZ; 
			#endregion

			#region Walls & floor
			SimObject floor7 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "floor7", testEffect, Room.Toilet);
			floor7.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ), Vector3.Zero,
														new Vector3(roomSizeX, roomSizeY, roomSizeZ));
			floor7.Initialize();
			floor7.model.basicEffectManager.SpecularPower = 60;
			simScenario.staticObjectList.Add(floor7);

			SimObject wall20 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_bathroom_v1", testEffect, Room.Toilet);
			wall20.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ), Vector3.Zero, Vector3.One);
			wall20.Initialize();
			wall20.model.basicEffectManager.SpecularPower = 60;
			simScenario.staticObjectList.Add(wall20);
			#endregion
		}

		private static void BuildAnteroomArea(Game game, float roomOffsetY)
		{
			#region room dimensions
			float roomSizeX = 4;
			float roomSizeZ = 1;
			//float roomSizeY = 1;

			float roomOffsetX = -2;
			float roomOffsetZ = 0;

			float roomBorderX = roomSizeX + roomOffsetX;
			float roomBorderZ = -roomSizeZ + roomOffsetZ; 
			#endregion

			#region Walls & floor
			SimObject floor6 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "floor6_v2", testEffect, Room.Anteroom);
			floor6.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ), Vector3.Zero,
														Vector3.One);
			floor6.Initialize();
			floor6.model.basicEffectManager.SpecularPower = 60;
			simScenario.staticObjectList.Add(floor6);

			SimObject wall17 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_anteroom_v1", testEffect, Room.Anteroom);
			wall17.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ), Vector3.Zero, Vector3.One);
			wall17.Initialize();
			wall17.model.basicEffectManager.SpecularPower = 60;
			simScenario.staticObjectList.Add(wall17);
			#endregion
		}

		private static void BuildKitchenArea(Game game, float roomOffsetY)
		{
			#region room dimensions
			float roomSizeX = 3;
			float roomSizeZ = 3;
			float roomSizeY = 1;

			float roomOffsetX = -2;
			float roomOffsetZ = -1;

			float roomBorderX = roomSizeX + roomOffsetX;
			float roomBorderZ = -roomSizeZ + roomOffsetZ; 
			#endregion

			#region Walls & floor
			SimObject floor5 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "floor5_v4", testEffect, Room.Kitchen);
			floor5.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ), Vector3.Zero,
														Vector3.One);
			floor5.Initialize();
			floor5.model.basicEffectManager.SpecularPower = 60;
			simScenario.staticObjectList.Add(floor5);

			SimObject wall15 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_kitchen_v1", testEffect, Room.Kitchen);
			wall15.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ), Vector3.Zero, Vector3.One);
			wall15.Initialize();
			wall15.model.basicEffectManager.SpecularPower = 60;
			simScenario.staticObjectList.Add(wall15);
			#endregion

			#region furniture
			Thermometer kitchenThermometer = new Thermometer(game, "Thermometer", testEffect, Room.Kitchen,
										SimAssetsPath.POBICOS_OBJECTS_PATH + "fireGarTherm_3.xml");
			kitchenThermometer.Transformation = new Transformation(new Vector3(roomOffsetX + 0.05f, roomSizeY / 2 + roomOffsetY, roomOffsetZ - roomSizeZ * 0.9f),
														new Vector3(0, 90, 0),
														new Vector3(0.1f));
			kitchenThermometer.Initialize();
			kitchenThermometer.model.basicEffectManager.preferPerPixelLighting = true;
			simScenario.staticObjectList.Add(kitchenThermometer); 
			#endregion
		}

		private static void BuildGarageArea(Game game, float roomOffsetY)
		{
			#region room dimensions
			float roomSizeX = 3;
			float roomSizeZ = 4;
			float roomSizeY = 1;

			float roomOffsetX = -5;
			float roomOffsetZ = 0;

			float roomBorderX = roomSizeX + roomOffsetX;
			float roomBorderZ = -roomSizeZ + roomOffsetZ; 
			#endregion

			#region Walls & floor
			SimObject floor4 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "floor4", testEffect, Room.Garage);
			floor4.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ), Vector3.Zero,
														new Vector3(roomSizeX, roomSizeY, roomSizeZ));
			floor4.Initialize();
			floor4.model.basicEffectManager.SpecularPower = 60;
			simScenario.staticObjectList.Add(floor4);

			SimObject wall11 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_garage_v1", testEffect, Room.Garage);
			wall11.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ), Vector3.Zero, Vector3.One);
			wall11.Initialize();
			wall11.model.basicEffectManager.SpecularPower = 60;
			simScenario.staticObjectList.Add(wall11);
			#endregion

			#region furniture
			SmokeSensor garageSmokeSensor = new SmokeSensor(game, "SmokeSensor", testEffect, Room.Garage,
													SimAssetsPath.POBICOS_OBJECTS_PATH + "SmokeSensor_4.xml");
			garageSmokeSensor.Transformation = new Transformation(new Vector3(roomOffsetX + roomSizeX / 2, roomSizeY + roomOffsetY, roomBorderZ / 2),
														new Vector3(0.0f, 0.0f, 0.0f),
														new Vector3(0.08f, 0.05f, 0.08f));
			garageSmokeSensor.Initialize();
			simScenario.staticObjectList.Add(garageSmokeSensor);

			Thermometer garageThermometer = new Thermometer(game, "Thermometer", testEffect, Room.Garage,
									SimAssetsPath.POBICOS_OBJECTS_PATH + "fireGarTherm_2.xml");
			garageThermometer.Transformation = new Transformation(new Vector3(roomOffsetX + 0.05f, roomSizeY / 2 + roomOffsetY, roomOffsetZ - roomSizeZ * 0.75f),
														new Vector3(0, 90, 0),
														new Vector3(0.1f));
			garageThermometer.Initialize();
			garageThermometer.model.basicEffectManager.preferPerPixelLighting = true;
			simScenario.staticObjectList.Add(garageThermometer); 
			#endregion
		}

		private static void BuildBedroomArea(Game game, float roomOffsetY)
		{
			#region room dimensions
			float roomSizeX = 3;
			float roomSizeZ = 4;
			float roomSizeY = 1;

			float roomOffsetX = -5;
			float roomOffsetZ = -4;

			float roomBorderX = roomSizeX + roomOffsetX;
			float roomBorderZ = -roomSizeZ + roomOffsetZ; 
			#endregion

			#region Walls & floor
			SimObject floor3 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "floor3", testEffect, Room.Bedroom);
			floor3.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ), Vector3.Zero,
														new Vector3(roomSizeX, roomSizeY, roomSizeZ));
			floor3.Initialize();
			floor3.model.basicEffectManager.SpecularPower = 60;
			simScenario.staticObjectList.Add(floor3);

			SimObject wall7 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_bedroom_v1", testEffect, Room.Bedroom);
			wall7.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ), Vector3.Zero, Vector3.One);
			wall7.Initialize();
			wall7.model.basicEffectManager.SpecularPower = 60;
			simScenario.staticObjectList.Add(wall7);
			#endregion

			#region furniture
			Tv bedroomTv = new Tv(game, "tv_v3", testEffect, Room.Bedroom,
											SimAssetsPath.POBICOS_OBJECTS_PATH + "fireTv.xml");
			bedroomTv.Transformation = new Transformation(new Vector3(roomOffsetX + 0.4f, roomOffsetY, roomBorderZ + 0.4f),
														new Vector3(0.0f, 45.0f, 0.0f),
														new Vector3(0.2f, 0.2f, 0.2f));
			bedroomTv.Initialize();
			bedroomTv.model.basicEffectManager.preferPerPixelLighting = true;
			simScenario.staticObjectList.Add(bedroomTv); 
			#endregion
		}

		private static void BuildDiningArea(Game game, float roomOffsetY)
		{
			#region room dimensions
			float roomSizeX = 4;
			float roomSizeZ = 3;
			//float roomSizeY = 1;

			float roomOffsetX = -2;
			float roomOffsetZ = -5;

			float roomBorderX = roomSizeX + roomOffsetX;
			float roomBorderZ = -roomSizeZ + roomOffsetZ; 
			#endregion

			#region Walls & floor
			SimObject floor2 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "floor2_v2", testEffect, Room.Dining);
			floor2.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ + 1), Vector3.Zero,
														Vector3.One);
			floor2.Initialize();
			floor2.model.basicEffectManager.SpecularPower = 60;
			simScenario.staticObjectList.Add(floor2);

			SimObject wall4 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_dining_v1", testEffect, Room.Dining);
			wall4.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ+1), Vector3.Zero, Vector3.One);
			wall4.Initialize();
			wall4.model.basicEffectManager.SpecularPower = 60;
			simScenario.staticObjectList.Add(wall4);
			#endregion
		}

		private static void BuildLivingArea(Game game, float roomOffsetY)
		{
			#region room dimensions
			float roomSizeX = 4.0f;
			float roomSizeZ = 5.0f;
            float roomSizeY = 1.0f;

			float roomOffsetX = 2;
			float roomOffsetZ = 0;

			float roomBorderX = roomSizeX + roomOffsetX;
			float roomBorderZ = roomSizeZ + roomOffsetZ; 
			#endregion

			#region Walls & floor
			SimObject floor1 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "floor_v2", testEffect, Room.Living);
			floor1.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ),
														new Vector3(0,0,0),
														new Vector3(roomSizeX, 1, roomSizeZ));
			floor1.Initialize();
			floor1.model.basicEffectManager.SpecularPower = 60;
			simScenario.staticObjectList.Add(floor1);

			SimObject wall2 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_living_v2", testEffect, Room.Living);
			wall2.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ),
															Vector3.Zero,
															Vector3.One);
			wall2.Initialize();
			wall2.model.basicEffectManager.SpecularPower = 60;
			simScenario.staticObjectList.Add(wall2);
			#endregion

			#region furniture
			Tv tv = new Tv(game, "tv_v3", testEffect, Room.Living,
													SimAssetsPath.POBICOS_OBJECTS_PATH + "fireMonitor.xml");
			tv.Transformation = new Transformation(new Vector3(roomBorderX - (0.5f / 2), roomOffsetY, -roomBorderZ / 2),
														new Vector3(0.0f, -90.0f, 0.0f),
														new Vector3(0.2f, 0.2f, 0.2f));
			tv.Initialize();
			tv.model.basicEffectManager.preferPerPixelLighting = true;
			simScenario.staticObjectList.Add(tv);

			PobicosLamp lamp = new PobicosLamp(game, "lampOn", testEffect, Room.Living,
												SimAssetsPath.POBICOS_OBJECTS_PATH + "lamp.xml");
			lamp.Transformation = new Transformation(new Vector3(roomBorderX / 2, roomOffsetY, -roomBorderZ + 0.35f),
														new Vector3(0.0f, -90.0f, 0.0f),
														new Vector3(0.15f, 0.15f, 0.15f));
			lamp.objectState = PobicosLamp.ObjectState.OFF;
			lamp.Initialize();
			lamp.model.basicEffectManager.preferPerPixelLighting = true;
			simScenario.pobicosObjectList.Add(lamp);

			SmokeSensor smokeSensor = new SmokeSensor(game, "SmokeSensor", testEffect, Room.Living,
												SimAssetsPath.POBICOS_OBJECTS_PATH + "SmokeSensor_5.xml");
			smokeSensor.Transformation = new Transformation(new Vector3(roomBorderX / 2, roomSizeY + roomOffsetY, -roomBorderZ / 2),
														new Vector3(0.0f, 0.0f, 0.0f),
														new Vector3(0.08f, 0.05f, 0.08f));
			smokeSensor.Initialize();
			simScenario.staticObjectList.Add(smokeSensor);

			Thermometer livingThermometer = new Thermometer(game, "Thermometer", testEffect, Room.Living,
									SimAssetsPath.POBICOS_OBJECTS_PATH + "fireGarTherm_1.xml");
			livingThermometer.Transformation = new Transformation(new Vector3(roomOffsetX + 0.5f, roomSizeY / 2 + roomOffsetY, -roomBorderZ + 0.05f),
														new Vector3(0),
														new Vector3(0.1f));
			livingThermometer.Initialize();
			livingThermometer.model.basicEffectManager.preferPerPixelLighting = true;
			simScenario.staticObjectList.Add(livingThermometer); 
			#endregion
		}
		
		#endregion

		public static void PutSmoke(Game game, Vector3 position)
		{
			SimObject smoke = new SimObject(game, "smoke", testEffect, simScenario.GetActiveHuman().model.room);
			smoke.Transformation = new Transformation(position, Vector3.Zero, new Vector3(0.05f, 0.05f, 0.05f));
			smoke.Initialize();
			SimScenario.movingObjectList.Add(smoke);
		}

		public static void PutFire(Game game, Vector3 position)
		{
			SimObject fire = new SimObject(game, "Fire", testEffect, simScenario.GetActiveHuman().model.room);
			fire.Transformation = new Transformation(position, Vector3.Zero, new Vector3(0.1f));
			fire.Initialize();
			SimScenario.movingObjectList.Add(fire);
		}
		
		public static void AddCameras(Game game)
		{
            float aspectRatio = (float)game.GraphicsDevice.Viewport.Width / game.GraphicsDevice.Viewport.Height;

			ThirdPersonCamera followCamera = new ThirdPersonCamera();
			followCamera.SetPerspectiveFov(60.0f, aspectRatio, 0.1f, 700);
			followCamera.SetChaseParameters(3.0f, 9.0f, 7.0f, 14.0f);
			followCamera.SetLookAt(new Vector3(2.0f, 4.0f, 2.5f), simScenario.GetActiveHuman().Transformation.Translate + simScenario.cameraUpOffset, Vector3.Up);

			simScenario.cameraManager = new CameraManager();
			simScenario.cameraManager.Add("FollowCamera", followCamera);
		}
	}
}
