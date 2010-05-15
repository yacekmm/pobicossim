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

		public static Game thisGame;
		public enum Scenarios
		{ 
			Flat
		}

		public static SimScenario BuildScenario(Game game, Scenarios scenario)
		{
			thisGame = game;	//tymczasowe rozwiązanie
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
			simScenario = SimScenario.Instance;

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
			Human human = new Human(game, "girl", testEffect, Room.Living);
			human.isActive = true;
			human.Transformation = new Transformation(new Vector3(2.0f, 0.3f, -2.0f), new Vector3(0.0f, 180.0f, 0.0f), new Vector3(0.8f));
			human.Initialize();
			float sin = (float)Math.Sin(MathHelper.ToRadians(human.model.Rotate.Y));
			float cos = (float)Math.Cos(MathHelper.ToRadians(human.model.Rotate.Y));
			human.direction = new Vector3(sin, 0, cos);

			human.model.basicEffectManager.Light0Direction *= 1.5f;
			human.model.basicEffectManager.Light1Direction *= 1.5f;
			human.model.basicEffectManager.Light2Direction *= 1.5f;

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

			SimScenario.Client.Connect();
		}

		#region Create Building
		private static void BuildOutsideArea(Game game, float roomOffsetY)
		{
			#region room dimensions
			float roomSizeX = 11;
			float roomSizeZ = 8;

			float roomOffsetX = -5;
			float roomOffsetZ = 0;

			float roomBorderX = roomSizeX + roomOffsetX;
			float roomBorderZ = -roomSizeZ + roomOffsetZ;

			Room room = Room.Outside;
			#endregion

			#region Environment
			SimObject skybox = new SimObject(game, SimAssetsPath.MODELS_ENVIRONMENT_PATH + "skybox", testEffect, room);
			skybox.Transformation = new Transformation(Vector3.Zero, new Vector3(90, 90, 90), Vector3.One * 40);
			skybox.Initialize();
			skybox.model.basicEffectManager.Light0Direction = new Vector3(1, 0, -1);
			skybox.model.basicEffectManager.Light1Direction = new Vector3(-1, 0, 1);
			skybox.model.basicEffectManager.Light2Direction = new Vector3(0, 1, 0);
			skybox.model.basicEffectManager.SpecularPower = 100;
			simScenario.staticObjectList.Add(skybox);

			SimObject grass = new SimObject(game, SimAssetsPath.MODELS_ENVIRONMENT_PATH + "grass", testEffect, room);
			grass.Transformation = new Transformation(Vector3.Zero, new Vector3(90, 180, 0), new Vector3(41));
			grass.Initialize();
			simScenario.staticObjectList.Add(grass);

			SimObject sunporch = new SimObject(game, SimAssetsPath.MODELS_ENVIRONMENT_PATH + "sunporch", testEffect, room);
			sunporch.Transformation = new Transformation(new Vector3(7, 0, -11), new Vector3(0), new Vector3(1.4f));
			sunporch.Initialize();
			SimScenario.furnitureList.Add(sunporch);
			#endregion

			#region Walls & basement
			SimObject basement = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "basement_tex_v3", testEffect, room);
			basement.Transformation = new Transformation(new Vector3(-5.0f, roomOffsetY - 0.3f, roomOffsetZ), Vector3.Zero, Vector3.One);
			basement.Initialize();
			basement.model.basicEffectManager.SpecularPower = 60;
			simScenario.staticObjectList.Add(basement);

			SimObject wall29 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_5_2_outside_base_total_tex_v5", testEffect, room);
			wall29.Transformation = new Transformation(new Vector3(roomBorderX - 5, roomOffsetY, roomOffsetZ), new Vector3(-90, 0, 0), Vector3.One);
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

			Room room = Room.Toilet;
			#endregion

			#region Walls & floor
			SimObject floor7 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "floor7", testEffect, room);
			floor7.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ), Vector3.Zero,
														new Vector3(roomSizeX, roomSizeY, roomSizeZ));
			floor7.Initialize();
			floor7.model.basicEffectManager.SpecularPower = 60;
			simScenario.staticObjectList.Add(floor7);

			SimObject wall20 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_bathroom_v2", testEffect, room);
			wall20.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ), Vector3.Zero, Vector3.One);
			wall20.Initialize();
			wall20.model.basicEffectManager.SpecularPower = 60;
			simScenario.staticObjectList.Add(wall20);
			#endregion

			#region furniture
			SimObject commode = new SimObject(game, SimAssetsPath.MODELS_FURNITURE_PATH + "commode", testEffect, room);
			commode.Transformation = new Transformation(new Vector3(roomOffsetX+0.05f, roomOffsetY, roomOffsetZ - roomSizeZ / 2),
														new Vector3(0.0f, 90.0f, 0.0f),
														new Vector3(1.1f));
			commode.Initialize();
			SimScenario.furnitureList.Add(commode);

			SimObject sink = new SimObject(game, SimAssetsPath.MODELS_FURNITURE_PATH + "sink", testEffect, room);
			sink.Transformation = new Transformation(new Vector3(roomOffsetX + 1, roomOffsetY+0.3f, roomOffsetZ - 0.05f),
														new Vector3(0.0f, 180.0f, 0.0f),
														new Vector3(0.35f));
			sink.Initialize();
			SimScenario.furnitureList.Add(sink);  
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

			Room room = Room.Anteroom;
			#endregion

			#region Walls & floor
			SimObject floor6 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "floor6_v2", testEffect, room);
			floor6.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ), Vector3.Zero,
														Vector3.One);
			floor6.Initialize();
			floor6.model.basicEffectManager.SpecularPower = 60;
			simScenario.staticObjectList.Add(floor6);

			SimObject wall17 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_anteroom_v2", testEffect, room);
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

			Room room = Room.Kitchen;
			#endregion

			#region Walls & floor
			SimObject floor5 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "floor5_v4", testEffect, room);
			floor5.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ), Vector3.Zero,
														Vector3.One);
			floor5.Initialize();
			floor5.model.basicEffectManager.SpecularPower = 60;
			simScenario.staticObjectList.Add(floor5);

			SimObject wall15 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_kitchen_v2", testEffect, room);
			wall15.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ), Vector3.Zero, Vector3.One);
			wall15.Initialize();
			wall15.model.basicEffectManager.SpecularPower = 60;
			simScenario.staticObjectList.Add(wall15);
			#endregion

			#region furniture
			Thermometer kitchenThermometer = new Thermometer(game, "Thermometer_v2", testEffect, room,
										SimAssetsPath.POBICOS_OBJECTS_PATH + "fireGarTherm_3.xml");
			kitchenThermometer.Transformation = new Transformation(new Vector3(roomOffsetX + 0.05f, roomSizeY / 2 + roomOffsetY, roomOffsetZ - roomSizeZ * 0.9f),
														new Vector3(0, 90, 0),
														new Vector3(0.17f));
			kitchenThermometer.Initialize();
			kitchenThermometer.model.basicEffectManager.preferPerPixelLighting = true;
			SimScenario.pobicosObjectList.Add(kitchenThermometer);

			SimObject kitchen2 = new SimObject(game, SimAssetsPath.MODELS_FURNITURE_PATH + "kitchen2", testEffect, room);
			kitchen2.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ),
														new Vector3(0, -90, 0),
														new Vector3(2.5f, 1.5f, 3.0f));
			kitchen2.Initialize();
			simScenario.staticObjectList.Add(kitchen2);
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
 
			Room room = Room.Garage;
			#endregion

			#region Walls & floor
			SimObject floor4 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "floor4", testEffect, room);
			floor4.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ), Vector3.Zero,
														new Vector3(roomSizeX, roomSizeY, roomSizeZ));
			floor4.Initialize();
			floor4.model.basicEffectManager.SpecularPower = 60;
			simScenario.staticObjectList.Add(floor4);

			SimObject wall11 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_garage_v2", testEffect, room);
			wall11.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ), Vector3.Zero, Vector3.One);
			wall11.Initialize();
			wall11.model.basicEffectManager.SpecularPower = 60;
			simScenario.staticObjectList.Add(wall11);
			#endregion

			#region furniture
			#region POBICOS
			SmokeSensor garageSmokeSensor = new SmokeSensor(game, "SmokeSensor", testEffect, room,
														SimAssetsPath.POBICOS_OBJECTS_PATH + "SmokeSensor_5.xml");
			garageSmokeSensor.Transformation = new Transformation(new Vector3(roomOffsetX + roomSizeX / 2, roomSizeY + roomOffsetY, roomBorderZ / 2 -0.5f),
														new Vector3(0.0f, 0.0f, 0.0f),
														new Vector3(0.08f, 0.05f, 0.08f));
			garageSmokeSensor.Initialize();
			SimScenario.pobicosObjectList.Add(garageSmokeSensor);

			Thermometer garageThermometer = new Thermometer(game, "Thermometer_v2", testEffect, room,
									SimAssetsPath.POBICOS_OBJECTS_PATH + "fireGarTherm_2.xml");
			garageThermometer.Transformation = new Transformation(new Vector3(roomOffsetX + 0.05f, roomSizeY / 2 + roomOffsetY, roomOffsetZ - roomSizeZ * 0.75f),
														new Vector3(0, 90, 0),
														new Vector3(0.17f));
			garageThermometer.Initialize();
			garageThermometer.model.basicEffectManager.preferPerPixelLighting = true;
			SimScenario.pobicosObjectList.Add(garageThermometer); 
			#endregion

			#region Other
			SimObject car = new SimObject(game, SimAssetsPath.MODELS_FURNITURE_PATH + "car", testEffect, room);
			car.Transformation = new Transformation(new Vector3(roomOffsetX + roomSizeX / 2, roomOffsetY, roomBorderZ / 2 + 0.3f),
														new Vector3(0.0f, 0.0f, 0.0f),
														new Vector3(0.23f));
			car.Initialize();
			SimScenario.furnitureList.Add(car);
			#endregion
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

			Room room = Room.Bedroom;
			#endregion

			#region Walls & floor
			SimObject floor3 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "floor3", testEffect, Room.Bedroom);
			floor3.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ), Vector3.Zero,
														new Vector3(roomSizeX, roomSizeY, roomSizeZ));
			floor3.Initialize();
			floor3.model.basicEffectManager.SpecularPower = 60;
			simScenario.staticObjectList.Add(floor3);

			SimObject wall7 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_bedroom_v2", testEffect, Room.Bedroom);
			wall7.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ), Vector3.Zero, Vector3.One);
			wall7.Initialize();
			wall7.model.basicEffectManager.SpecularPower = 60;
			simScenario.staticObjectList.Add(wall7);
			#endregion

			#region furniture
			#region POBICOS
			Tv bedroomTv = new Tv(game, "tv_v3", testEffect, room,
												SimAssetsPath.POBICOS_OBJECTS_PATH + "fireTv.xml");
			bedroomTv.Transformation = new Transformation(new Vector3(roomOffsetX + 0.4f, roomOffsetY, roomBorderZ + 0.4f),
														new Vector3(0.0f, 45.0f, 0.0f),
														new Vector3(2.2f));
			bedroomTv.Initialize();
			bedroomTv.model.basicEffectManager.preferPerPixelLighting = true;
			SimScenario.pobicosObjectList.Add(bedroomTv);  
			#endregion

			#region Other
			SimObject bed = new SimObject(game, SimAssetsPath.MODELS_FURNITURE_PATH + "bed", testEffect, room);
			bed.Transformation = new Transformation(new Vector3(roomBorderX, roomOffsetY, roomOffsetZ - roomSizeZ/2 - 0.5f),
														new Vector3(0.0f, -90.0f, 0.0f),
														new Vector3(0.6f));
			bed.Initialize();
			SimScenario.furnitureList.Add(bed);   
			#endregion
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

			Room room = Room.Dining;
			#endregion

			#region Walls & floor
			#region POBICOS
			SimObject floor2 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "floor2_v2", testEffect, room);
			floor2.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ + 1), Vector3.Zero,
														Vector3.One);
			floor2.Initialize();
			floor2.model.basicEffectManager.SpecularPower = 60;
			simScenario.staticObjectList.Add(floor2);

			SimObject wall4 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_dining_v2", testEffect, room);
			wall4.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ + 1), Vector3.Zero, Vector3.One);
			wall4.Initialize();
			wall4.model.basicEffectManager.SpecularPower = 60;
			simScenario.staticObjectList.Add(wall4); 
			#endregion

			#region Other
			float tmpScale = 0.7f;

			SimObject table = new SimObject(game, SimAssetsPath.MODELS_FURNITURE_PATH + "table", testEffect, room);
			table.Transformation = new Transformation(new Vector3(roomOffsetX + roomSizeX / 2, roomOffsetY, roomOffsetZ - roomSizeZ / 2),
														new Vector3(0.0f, 0.0f, 0.0f),
														tmpScale * new Vector3(0.9f, 0.8f, 0.8f));
			table.Initialize();
			SimScenario.furnitureList.Add(table);

			SimObject chair1 = new SimObject(game, SimAssetsPath.MODELS_FURNITURE_PATH + "chair", testEffect, room);
			chair1.Transformation = new Transformation(new Vector3(roomOffsetX + roomSizeX / 2 - 0.3f, roomOffsetY, roomOffsetZ - roomSizeZ / 2 - 0.3f),
														new Vector3(0.0f, 35.0f, 0.0f),
														tmpScale * new Vector3(0.55f));
			chair1.Initialize();
			SimScenario.furnitureList.Add(chair1);

			SimObject chair2 = new SimObject(game, SimAssetsPath.MODELS_FURNITURE_PATH + "chair", testEffect, room);
			chair2.Transformation = new Transformation(new Vector3(roomOffsetX + roomSizeX / 2 + 0.3f, roomOffsetY, roomOffsetZ - roomSizeZ / 2 - 0.25f),
														new Vector3(0.0f, 0.0f, 0.0f),
														tmpScale * new Vector3(0.55f));
			chair2.Initialize();
			SimScenario.furnitureList.Add(chair2);

			SimObject chair3 = new SimObject(game, SimAssetsPath.MODELS_FURNITURE_PATH + "chair", testEffect, room);
			chair3.Transformation = new Transformation(new Vector3(roomOffsetX + roomSizeX / 2 - 0.3f, roomOffsetY, roomOffsetZ - roomSizeZ / 2 + 0.25f),
														new Vector3(0.0f, 180.0f, 0.0f),
														tmpScale * new Vector3(0.55f));
			chair3.Initialize();
			SimScenario.furnitureList.Add(chair3);

			SimObject chair4 = new SimObject(game, SimAssetsPath.MODELS_FURNITURE_PATH + "chair", testEffect, room);
			chair4.Transformation = new Transformation(new Vector3(roomOffsetX + roomSizeX / 2 + 0.3f, roomOffsetY, roomOffsetZ - roomSizeZ / 2 + 0.25f),
														new Vector3(0.0f, 180.0f, 0.0f),
														tmpScale * new Vector3(0.55f));
			chair4.Initialize();
			SimScenario.furnitureList.Add(chair4); 
			#endregion
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

			Room room = Room.Living;
			#endregion

			#region Walls & floor
			SimObject floor1 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "floor_v2", testEffect, room);
			floor1.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ),
														new Vector3(0,0,0),
														new Vector3(roomSizeX, 1, roomSizeZ));
			floor1.Initialize();
			floor1.model.basicEffectManager.SpecularPower = 60;
			simScenario.staticObjectList.Add(floor1);

			SimObject wall2 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_living_v2", testEffect, room);
			wall2.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ),
															Vector3.Zero,
															Vector3.One);
			wall2.Initialize();
			wall2.model.basicEffectManager.SpecularPower = 60;
			simScenario.staticObjectList.Add(wall2);
			#endregion

			#region furniture
			#region POBICOS
			Tv tv = new Tv(game, "tv_v3", testEffect, room,
														SimAssetsPath.POBICOS_OBJECTS_PATH + "fireMonitor.xml");
			tv.Transformation = new Transformation(new Vector3(roomBorderX - 0.1f, roomOffsetY/* + 0.15f*/ , -roomBorderZ / 2 + 0.15f),
														new Vector3(0.0f, -90.0f, 0.0f),
														new Vector3(2.0f));
			tv.Initialize();
			tv.model.basicEffectManager.preferPerPixelLighting = true;
			SimScenario.pobicosObjectList.Add(tv);

			PobicosLamp lamp = new PobicosLamp(game, "lampOn", testEffect, room,
												SimAssetsPath.POBICOS_OBJECTS_PATH + "lamp.xml");
			lamp.Transformation = new Transformation(new Vector3(roomBorderX / 2, roomOffsetY, -roomBorderZ + 0.35f),
														new Vector3(0.0f, -90.0f, 0.0f),
														new Vector3(0.15f, 0.15f, 0.15f));
			lamp.objectState = PobicosLamp.ObjectState.OFF;
			lamp.Initialize();
			lamp.model.basicEffectManager.preferPerPixelLighting = true;
			SimScenario.pobicosObjectList.Add(lamp);

			SmokeSensor smokeSensor = new SmokeSensor(game, "SmokeSensor", testEffect, room,
												SimAssetsPath.POBICOS_OBJECTS_PATH + "SmokeSensor_5.xml");
			smokeSensor.Transformation = new Transformation(new Vector3(roomBorderX / 2, roomSizeY + roomOffsetY, -roomBorderZ / 2),
														new Vector3(0.0f, 0.0f, 0.0f),
														new Vector3(0.08f, 0.05f, 0.08f));
			smokeSensor.Initialize();
			SimScenario.pobicosObjectList.Add(smokeSensor);

			Thermometer livingThermometer = new Thermometer(game, "Thermometer_v2", testEffect, room,
									SimAssetsPath.POBICOS_OBJECTS_PATH + "fireGarTherm_1.xml");
			livingThermometer.Transformation = new Transformation(new Vector3(roomOffsetX + 0.5f, roomSizeY / 2 + roomOffsetY, -roomBorderZ + 0.05f),
														new Vector3(0),
														new Vector3(0.17f));
			livingThermometer.Initialize();
			livingThermometer.model.basicEffectManager.preferPerPixelLighting = true;
			SimScenario.pobicosObjectList.Add(livingThermometer);  
			#endregion

			#region Other
			//SimObject livingSet = new SimObject(game, SimAssetsPath.MODELS_FURNITURE_PATH + "living_set", testEffect, room);
			//livingSet.Transformation = new Transformation(new Vector3(roomBorderX - 0.05f, roomOffsetY, roomOffsetZ - roomSizeZ / 2 + 0.5f),
			//                                            new Vector3(0.0f, -90.0f, 0.0f),
			//                                            new Vector3(1.4f));
			//livingSet.Initialize();
			//SimScenario.furnitureList.Add(livingSet);

			SimObject connery = new SimObject(game, SimAssetsPath.MODELS_FURNITURE_PATH + "Connery", testEffect, room);
			connery.Transformation = new Transformation(new Vector3(roomOffsetX + 2, roomOffsetY, roomOffsetZ - 1),
														new Vector3(0.0f, 180.0f, 0.0f),
														new Vector3(0.55f));
			connery.Initialize();
			SimScenario.furnitureList.Add(connery);

			SimObject couch = new SimObject(game, SimAssetsPath.MODELS_FURNITURE_PATH + "Couch", testEffect, room);
			couch.Transformation = new Transformation(new Vector3(roomOffsetX + 1, roomOffsetY, -roomBorderZ / 2),
														new Vector3(0.0f, 90.0f, 0.0f),
														new Vector3(0.5f));
			couch.Initialize();
			SimScenario.furnitureList.Add(couch);

			SimObject bench = new SimObject(game, SimAssetsPath.MODELS_FURNITURE_PATH + "bench", testEffect, room);
			bench.Transformation = new Transformation(new Vector3(roomOffsetX + 2, roomOffsetY, -roomBorderZ / 2),
														new Vector3(0.0f, 90.0f, 0.0f),
														0.8f * new Vector3(0.4f, 0.8f, 1.3f));
			bench.Initialize();
			SimScenario.furnitureList.Add(bench);

			#endregion
			#endregion
		}
		
		#endregion

		public static void PutSmoke(Game game, Vector3 position, float scale)
		{
			position.Y += 0.6f;

			SimObject smoke = new SimObject(thisGame, "smoke", testEffect, simScenario.GetActiveHuman().model.room);
			smoke.Transformation = new Transformation(position, Vector3.Zero, new Vector3(0.05f * scale));
			smoke.Initialize();
			SimScenario.movingObjectList.Add(smoke);
		}

		public static void PutFire(Game game, Vector3 position)
		{
			
			
			int counter=0;
			foreach (SimObject so in SimScenario.movingObjectList)
				if (so.name.Contains("Fire"))
					counter++;

			if (counter > 2)
				SimScenario.movingObjectList.Remove(simScenario.GetObjectByName("Fire"));

			SimObject fire = new SimObject(game, "Fire", testEffect, simScenario.GetActiveHuman().model.room);
			fire.Transformation = new Transformation(position, Vector3.Zero, new Vector3(0.3f));
			fire.Initialize();
			fire.model.basicEffectManager.Light0Direction *= 3;
			fire.model.basicEffectManager.Light1Direction *= 3;
			fire.model.basicEffectManager.Light2Direction *= 3;
			SimScenario.movingObjectList.Add(fire);
		}
		
		public static void AddCameras(Game game)
		{
            float aspectRatio = (float)game.GraphicsDevice.Viewport.Width / game.GraphicsDevice.Viewport.Height;

			ThirdPersonCamera followCamera = new ThirdPersonCamera();
			followCamera.SetChaseParameters(1.0f, 7.0f, 5.0f, 8.0f);

			followCamera.ChaseSpeed = simScenario.GetActiveHuman().movementSpeed;

			followCamera.Target = simScenario.GetActiveHuman().Transformation.Translate + simScenario.cameraUpOffset;
			followCamera.UpVector = Vector3.Up;
			followCamera.Position = new Vector3(2.0f, 4.0f, 2.5f);

			Vector3 cameraOffset = new Vector3(3, 5, 0);
			followCamera.ChasePosition = followCamera.Target +
				cameraOffset.X * Vector3.Right +
				cameraOffset.Y * Vector3.Up +
				cameraOffset.Z * simScenario.GetActiveHuman().direction;
			followCamera.ChaseDirection = simScenario.GetActiveHuman().direction;

			followCamera.AspectRatio = aspectRatio;
			followCamera.FovY = 60;
			followCamera.NearPlane = 0.1f;
			followCamera.FarPlane = 300;

			followCamera.EyeRotate = new Vector3(0);
			followCamera.Rotate = 90;
			followCamera.EyeRotateVelocity = new Vector3(3);
			followCamera.IsFirstTimeChase = true;

			simScenario.cameraManager = new CameraManager();
			simScenario.cameraManager.Add("FollowCamera", followCamera);
		}
	}
}
