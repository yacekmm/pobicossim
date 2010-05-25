using System.Collections.Generic;
using Microsoft.Xna.Framework;
using POBICOS.SimBase.Cameras;
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
		public static SimScenario simScenario;

		public static Game thisGame;
		public enum Scenarios
		{ 
			Flat
		}

		public static SimScenario BuildScenario(Game game, Scenarios scenario)
		{
			thisGame = game;
			game.Services.RemoveService(typeof(CameraManager));

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

			AddHumans();

			//Camera
			AddCameras();
			
			game.Services.AddService(typeof(CameraManager), simScenario.cameraManager);
			//game.Services.AddService(typeof(BasicEffectManager), simScenario.basicEffectManager);
			
			BuildStaticObjects(game);

			return simScenario;
		}

		private static void AddHumans()
		{
			Human human = new Human(thisGame, "Sphere6", Room.Living);
			human.isActive = true;
			human.Transformation = new Transformation(new Vector3(2.0f, 0.3f, -2.0f), new Vector3(0.0f, 180.0f, 0.0f), new Vector3(0.8f));
			float sin = (float)Math.Sin(MathHelper.ToRadians(human.model.Rotate.Y));
			float cos = (float)Math.Cos(MathHelper.ToRadians(human.model.Rotate.Y));
			human.direction = new Vector3(sin, 0, cos);

			SimScenario.humanList.Add(human);
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

			#region Walls & basement
			SimObject basement = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "basement_tex_v3", room);
			basement.Transformation = new Transformation(new Vector3(-5.0f, roomOffsetY - 0.3f, roomOffsetZ), Vector3.Zero, Vector3.One);
			basement.model.basicEffectManager.SpecularPower = 60;
			SimScenario.staticObjectList.Add(basement);

			SimObject wall29 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_5_2_outside_base_total_tex_v5", room);
			wall29.Transformation = new Transformation(new Vector3(roomBorderX - 5, roomOffsetY, roomOffsetZ), new Vector3(-90, 0, 0), Vector3.One);
			wall29.model.basicEffectManager.SpecularPower = 60;
			SimScenario.staticObjectList.Add(wall29);
			#endregion

			#region Environment
			SimObject skybox = new SimObject(game, SimAssetsPath.MODELS_ENVIRONMENT_PATH + "skybox", room);
			skybox.Transformation = new Transformation(Vector3.Zero, new Vector3(90, 90, 90), Vector3.One * 40);
			skybox.model.basicEffectManager.Light0Direction = new Vector3(1, 0, -1);
			skybox.model.basicEffectManager.Light1Direction = new Vector3(-1, 0, 1);
			skybox.model.basicEffectManager.Light2Direction = new Vector3(0, 1, 0);
			skybox.model.basicEffectManager.SpecularPower = 100;
			SimScenario.staticObjectList.Add(skybox);

			SimObject grass = new SimObject(game, SimAssetsPath.MODELS_ENVIRONMENT_PATH + "grass", room);
			grass.Transformation = new Transformation(Vector3.Zero, new Vector3(90, 180, 0), new Vector3(41));
			SimScenario.staticObjectList.Add(grass);
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
			SimObject floor7 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "floor7", room);
			floor7.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ), Vector3.Zero,
														new Vector3(roomSizeX, roomSizeY, roomSizeZ));
			floor7.model.basicEffectManager.SpecularPower = 60;
			SimScenario.staticObjectList.Add(floor7);

			SimObject wall20 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_bathroom_v2", room);
			wall20.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ), Vector3.Zero, Vector3.One);
			wall20.model.basicEffectManager.SpecularPower = 60;
			SimScenario.staticObjectList.Add(wall20);
			#endregion

			#region Other furniture
			SimObject commode = new SimObject(game, SimAssetsPath.MODELS_FURNITURE_PATH + "commode", room);
			commode.Transformation = new Transformation(new Vector3(roomOffsetX+0.05f, roomOffsetY, roomOffsetZ - roomSizeZ / 2),
														new Vector3(0.0f, 90.0f, 0.0f),
														new Vector3(1.1f));
			SimScenario.furnitureList.Add(commode);

			SimObject sink = new SimObject(game, SimAssetsPath.MODELS_FURNITURE_PATH + "sink", room);
			sink.Transformation = new Transformation(new Vector3(roomOffsetX + 1, roomOffsetY+0.3f, roomOffsetZ - 0.05f),
														new Vector3(0.0f, 180.0f, 0.0f),
														new Vector3(0.35f));
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
			SimObject floor6 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "floor6_v2", room);
			floor6.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ), Vector3.Zero,
														Vector3.One);
			floor6.model.basicEffectManager.SpecularPower = 60;
			SimScenario.staticObjectList.Add(floor6);

			SimObject wall17 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_anteroom_v3", room);
			wall17.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ), Vector3.Zero, Vector3.One);
			wall17.model.basicEffectManager.SpecularPower = 60;
			SimScenario.staticObjectList.Add(wall17);
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
			SimObject floor5 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "floor5_v4", room);
			floor5.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ), Vector3.Zero,
														Vector3.One);
			floor5.model.basicEffectManager.SpecularPower = 60;
			SimScenario.staticObjectList.Add(floor5);

			SimObject wall15 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_kitchen_v2", room);
			wall15.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ), Vector3.Zero, Vector3.One);
			wall15.model.basicEffectManager.SpecularPower = 60;
			SimScenario.staticObjectList.Add(wall15);
			#endregion

			#region POBICOS furniture
			Thermometer kitchenThermometer = new Thermometer(game, "Thermometer_v2", room,
										SimAssetsPath.POBICOS_OBJECTS_PATH + "fireGarTherm_3.xml");
			kitchenThermometer.Transformation = new Transformation(new Vector3(roomOffsetX + 0.05f, roomSizeY / 2 + roomOffsetY, roomOffsetZ - roomSizeZ * 0.9f),
														new Vector3(0, 90, 0),
														new Vector3(0.17f));
			kitchenThermometer.model.basicEffectManager.preferPerPixelLighting = true;
			SimScenario.pobicosObjectList.Add(kitchenThermometer);
			#endregion

			#region Other furniture
			SimObject kitchen2 = new SimObject(game, SimAssetsPath.MODELS_FURNITURE_PATH + "kitchen2_v2", room);
			kitchen2.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ),
														new Vector3(0, -90, 0),
														new Vector3(2.5f, 1.5f, 3.0f));
			SimScenario.staticObjectList.Add(kitchen2);
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
			SimObject floor4 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "floor4", room);
			floor4.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ), Vector3.Zero,
														new Vector3(roomSizeX, roomSizeY, roomSizeZ));
			floor4.model.basicEffectManager.SpecularPower = 60;
			SimScenario.staticObjectList.Add(floor4);

			SimObject wall11 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_garage_v2", room);
			wall11.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ), Vector3.Zero, Vector3.One);
			wall11.model.basicEffectManager.SpecularPower = 60;
			SimScenario.staticObjectList.Add(wall11);
			#endregion

			#region POBICOS furniture
			SmokeSensor garageSmokeSensor = new SmokeSensor(game, "SmokeSensor", room,
														SimAssetsPath.POBICOS_OBJECTS_PATH + "SmokeSensor_4.xml");
			garageSmokeSensor.Transformation = new Transformation(new Vector3(roomOffsetX + roomSizeX / 2, roomSizeY + roomOffsetY, roomBorderZ / 2 + 0.5f),
														new Vector3(0.0f, 0.0f, 0.0f),
														new Vector3(0.08f, 0.05f, 0.08f));
			SimScenario.pobicosObjectList.Add(garageSmokeSensor);

			Thermometer garageThermometer = new Thermometer(game, "Thermometer_v2", room,
									SimAssetsPath.POBICOS_OBJECTS_PATH + "fireGarTherm_2.xml");
			garageThermometer.Transformation = new Transformation(new Vector3(roomOffsetX + 0.05f, roomSizeY / 2 + roomOffsetY, roomOffsetZ - roomSizeZ * 0.75f),
														new Vector3(0, 90, 0),
														new Vector3(0.17f));
			garageThermometer.Initialize();
			garageThermometer.model.basicEffectManager.preferPerPixelLighting = true;
			SimScenario.pobicosObjectList.Add(garageThermometer); 
			#endregion

			#region Other furniture
			SimObject car = new SimObject(game, SimAssetsPath.MODELS_FURNITURE_PATH + "car_V2", room);
			car.Transformation = new Transformation(new Vector3(roomOffsetX + roomSizeX / 2, roomOffsetY, roomBorderZ / 2 - 0.1f),
														new Vector3(0.0f, 0.0f, 0.0f),
														new Vector3(0.23f));
			SimScenario.furnitureList.Add(car);
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
			SimObject floor3 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "floor3", room);
			floor3.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ), Vector3.Zero,
														new Vector3(roomSizeX, roomSizeY, roomSizeZ));
			floor3.model.basicEffectManager.SpecularPower = 60;
			SimScenario.staticObjectList.Add(floor3);

			SimObject bear = new SimObject(game, SimAssetsPath.MODELS_FURNITURE_PATH + "bear", room);
			bear.Transformation = new Transformation(new Vector3(roomOffsetX + roomSizeX/2 - 0.5f, roomOffsetY+0.01f, roomOffsetZ - 2), 
														new Vector3(180,140,0),
														new Vector3(0.15f));
			bear.model.basicEffectManager.SpecularPower = 60;
			bear.model.basicEffectManager.AmbientColor -= new Vector3(0.2f);
			SimScenario.staticObjectList.Add(bear);

			SimObject wall7 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_bedroom_v3", room);
			wall7.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ), Vector3.Zero, Vector3.One);
			wall7.model.basicEffectManager.SpecularPower = 60;
			SimScenario.staticObjectList.Add(wall7);
			#endregion

			#region POBICOS furniture
			Tv bedroomTv = new Tv(game, "tv_v3", room,
												SimAssetsPath.POBICOS_OBJECTS_PATH + "fireTv.xml");
			bedroomTv.Transformation = new Transformation(new Vector3(roomOffsetX + 0.4f, roomOffsetY, roomBorderZ + 0.4f),
														new Vector3(0.0f, 45.0f, 0.0f),
														new Vector3(2.2f));
			bedroomTv.model.basicEffectManager.preferPerPixelLighting = true;
			SimScenario.pobicosObjectList.Add(bedroomTv);  
			#endregion

			#region Other furniture
			SimObject bed = new SimObject(game, SimAssetsPath.MODELS_FURNITURE_PATH + "bed", room);
			bed.Transformation = new Transformation(new Vector3(roomBorderX, roomOffsetY, roomOffsetZ - roomSizeZ/2 - 0.5f),
														new Vector3(0.0f, -90.0f, 0.0f),
														new Vector3(0.6f));
			SimScenario.furnitureList.Add(bed);   
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
			SimObject floor2 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "floor2_v2", room);
			floor2.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ + 1), Vector3.Zero,
														Vector3.One);
			floor2.model.basicEffectManager.SpecularPower = 60;
			SimScenario.staticObjectList.Add(floor2);

			SimObject wall4 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_dining_v2", room);
			wall4.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ + 1), Vector3.Zero, Vector3.One);
			wall4.model.basicEffectManager.SpecularPower = 60;
			SimScenario.staticObjectList.Add(wall4); 
			#endregion

			#region Other furniture
			float tmpScale = 0.7f;

			SimObject table = new SimObject(game, SimAssetsPath.MODELS_FURNITURE_PATH + "table", room);
			table.Transformation = new Transformation(new Vector3(roomOffsetX + roomSizeX / 2, roomOffsetY, roomOffsetZ - roomSizeZ / 2),
														new Vector3(0.0f, 0.0f, 0.0f),
														tmpScale * new Vector3(0.9f, 0.8f, 0.8f));
			SimScenario.furnitureList.Add(table);

			SimObject chair1 = new SimObject(game, SimAssetsPath.MODELS_FURNITURE_PATH + "chair", room);
			chair1.Transformation = new Transformation(new Vector3(roomOffsetX + roomSizeX / 2 - 0.3f, roomOffsetY, roomOffsetZ - roomSizeZ / 2 - 0.3f),
														new Vector3(0.0f, 35.0f, 0.0f),
														tmpScale * new Vector3(0.55f));
			SimScenario.furnitureList.Add(chair1);

			SimObject chair2 = new SimObject(game, SimAssetsPath.MODELS_FURNITURE_PATH + "chair", room);
			chair2.Transformation = new Transformation(new Vector3(roomOffsetX + roomSizeX / 2 + 0.3f, roomOffsetY, roomOffsetZ - roomSizeZ / 2 - 0.25f),
														new Vector3(0.0f, 0.0f, 0.0f),
														tmpScale * new Vector3(0.55f));
			SimScenario.furnitureList.Add(chair2);

			SimObject chair3 = new SimObject(game, SimAssetsPath.MODELS_FURNITURE_PATH + "chair", room);
			chair3.Transformation = new Transformation(new Vector3(roomOffsetX + roomSizeX / 2 - 0.3f, roomOffsetY, roomOffsetZ - roomSizeZ / 2 + 0.25f),
														new Vector3(0.0f, 180.0f, 0.0f),
														tmpScale * new Vector3(0.55f));
			SimScenario.furnitureList.Add(chair3);

			SimObject chair4 = new SimObject(game, SimAssetsPath.MODELS_FURNITURE_PATH + "chair", room);
			chair4.Transformation = new Transformation(new Vector3(roomOffsetX + roomSizeX / 2 + 0.3f, roomOffsetY, roomOffsetZ - roomSizeZ / 2 + 0.25f),
														new Vector3(0.0f, 180.0f, 0.0f),
														tmpScale * new Vector3(0.55f));
			SimScenario.furnitureList.Add(chair4);

			SimObject fireplace = new SimObject(game, SimAssetsPath.MODELS_FURNITURE_PATH + "fireplace", room);
			fireplace.Transformation = new Transformation(new Vector3(roomOffsetX+ 0.1f, roomOffsetY, roomOffsetZ - roomSizeZ / 2),
														new Vector3(0.0f, 90.0f, 0.0f),
														Vector3.One);
			SimScenario.furnitureList.Add(fireplace); 
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
			SimObject floor1 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "floor_v2", room);
			floor1.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ),
														new Vector3(0, 0, 0),
														new Vector3(roomSizeX, 1, roomSizeZ));
			floor1.model.basicEffectManager.SpecularPower = 60;
			SimScenario.staticObjectList.Add(floor1);

			SimObject carpet = new SimObject(game, SimAssetsPath.MODELS_FURNITURE_PATH + "zebra_carpet", room);
			carpet.Transformation = new Transformation(new Vector3(roomOffsetX + roomSizeX/2, roomOffsetY+0.01f, roomOffsetZ - roomSizeZ/2),
														new Vector3(0, 35, 0),
														new Vector3(3f));
			carpet.model.basicEffectManager.SpecularPower = 60;
			SimScenario.staticObjectList.Add(carpet);

			SimObject wall2 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_living_v2", room);
			wall2.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ),
															Vector3.Zero,
															Vector3.One);
			wall2.model.basicEffectManager.SpecularPower = 60;
			SimScenario.staticObjectList.Add(wall2);
			#endregion

			#region POBICOS furniture
			Tv tv = new Tv(game, "tv_v3", room,
														SimAssetsPath.POBICOS_OBJECTS_PATH + "fireMonitor.xml");
			tv.Transformation = new Transformation(new Vector3(roomBorderX - 0.1f, roomOffsetY + 0.15f , -roomBorderZ / 2 + 0.15f),
														new Vector3(0.0f, -90.0f, 0.0f),
														new Vector3(2.0f));
			tv.model.basicEffectManager.preferPerPixelLighting = true;
			SimScenario.pobicosObjectList.Add(tv);

			PobicosLamp lamp = new PobicosLamp(game, "lampOn", room,
												SimAssetsPath.POBICOS_OBJECTS_PATH + "lamp.xml");
			lamp.Transformation = new Transformation(new Vector3(roomBorderX / 2, roomOffsetY, -roomBorderZ + 0.35f),
														new Vector3(0.0f, -90.0f, 0.0f),
														new Vector3(0.25f));
			lamp.objectState = PobicosLamp.ObjectState.OFF;
			lamp.model.basicEffectManager.preferPerPixelLighting = true;
			SimScenario.pobicosObjectList.Add(lamp);

			SmokeSensor smokeSensor = new SmokeSensor(game, "SmokeSensor", room,
												SimAssetsPath.POBICOS_OBJECTS_PATH + "SmokeSensor_5.xml");
			smokeSensor.Transformation = new Transformation(new Vector3(roomBorderX / 2, roomSizeY + roomOffsetY, -roomBorderZ / 2),
														new Vector3(0.0f, 0.0f, 0.0f),
														new Vector3(0.08f, 0.05f, 0.08f));
			SimScenario.pobicosObjectList.Add(smokeSensor);

			Thermometer livingThermometer = new Thermometer(game, "Thermometer_v2", room,
									SimAssetsPath.POBICOS_OBJECTS_PATH + "fireGarTherm_1.xml");
			livingThermometer.Transformation = new Transformation(new Vector3(roomOffsetX + 0.5f, roomSizeY / 2 + roomOffsetY, -roomBorderZ + 0.05f),
														new Vector3(0),
														new Vector3(0.17f));
			livingThermometer.model.basicEffectManager.preferPerPixelLighting = true;
			SimScenario.pobicosObjectList.Add(livingThermometer);  
			#endregion

			#region Other furniture
			SimObject livingSet = new SimObject(game, SimAssetsPath.MODELS_FURNITURE_PATH + "living_set", room);
			livingSet.Transformation = new Transformation(new Vector3(roomBorderX - 0.05f, roomOffsetY, roomOffsetZ - roomSizeZ / 2 + 0.5f),
														new Vector3(0.0f, -90.0f, 0.0f),
														new Vector3(1.4f));
			SimScenario.furnitureList.Add(livingSet);

			SimObject connery = new SimObject(game, SimAssetsPath.MODELS_FURNITURE_PATH + "Connery_v2", room);
			connery.Transformation = new Transformation(new Vector3(roomOffsetX + 2, roomOffsetY, roomOffsetZ - 1),
														new Vector3(90.0f, 180.0f, 0.0f),
														new Vector3(0.55f));
			SimScenario.furnitureList.Add(connery);

			SimObject couch = new SimObject(game, SimAssetsPath.MODELS_FURNITURE_PATH + "Couch", room);
			couch.Transformation = new Transformation(new Vector3(roomOffsetX + 1, roomOffsetY, -roomBorderZ / 2),
														new Vector3(0.0f, 90.0f, 0.0f),
														new Vector3(0.5f));
			SimScenario.furnitureList.Add(couch);

			SimObject bench = new SimObject(game, SimAssetsPath.MODELS_FURNITURE_PATH + "bench", room);
			bench.Transformation = new Transformation(new Vector3(roomOffsetX + 2, roomOffsetY, -roomBorderZ / 2),
														new Vector3(0.0f, 90.0f, 0.0f),
														0.8f * new Vector3(0.4f, 0.8f, 1.3f));
			SimScenario.furnitureList.Add(bench);

			#endregion
		}
		
		#endregion

		public static void PutSmoke(Vector3 position, float scale)
		{
			SimObject smoke = new SimObject(thisGame, "smoke", simScenario.GetActiveHuman().model.room);
			smoke.Transformation = new Transformation(position, Vector3.Zero, new Vector3(0.8f * scale));
			SimScenario.movingObjectList.Add(smoke);
		}

		public static void PutFire(Vector3 position)
		{
			int counter=0;
			foreach (SimObject so in SimScenario.movingObjectList)
				if (so.name.Contains("Fire"))
					counter++;

			if (counter > 1)
				SimScenario.movingObjectList.Remove(simScenario.GetObjectByName("Fire"));

			SimObject fire = new SimObject(thisGame, "Fire", simScenario.GetActiveHuman().model.room);
			fire.Transformation = new Transformation(position, Vector3.Zero, new Vector3(0.3f));
			fire.model.basicEffectManager.Light0Direction *= 4;
			fire.model.basicEffectManager.Light1Direction *= 4;
			fire.model.basicEffectManager.Light2Direction *= 4;
			fire.model.basicEffectManager.preferPerPixelLighting = true;
			SimScenario.movingObjectList.Add(fire);
		}
		
		public static void AddCameras()
		{
            float aspectRatio = (float)thisGame.GraphicsDevice.Viewport.Width / thisGame.GraphicsDevice.Viewport.Height;

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
