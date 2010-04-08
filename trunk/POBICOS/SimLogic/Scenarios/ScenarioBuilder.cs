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
			game.Services.AddService(typeof(Client), simScenario.client);
			game.Services.AddService(typeof(BasicEffectManager), simScenario.basicEffectManager);
			game.Services.AddService(typeof(SimScenario), simScenario);

			//Human
			Human human = new Human(game, "Sphere6", testEffect, Room.Living);
			human.isActive = true;
			//human.Transformation = new Transformation(Vector3.Zero, Vector3.Zero, Vector3.One);
			human.Transformation = new Transformation(new Vector3(2.0f, 0.3f, -2.0f), new Vector3(0.0f, 0.0f, 0.0f), new Vector3(1.0f, 1.0f, 1.0f));
			human.Initialize();

			simScenario.humanList.Add(human);

			BuildStaticObjects(game);

			return simScenario;
		}

		private static void BuildStaticObjects(Game game)
		{
			float offsetY = 0.3f;
			BuildLivingArea(game, offsetY);
			BuildDiningArea(game, offsetY);
			BuildBedroomArea(game, offsetY);
			BuildGarageArea(game, offsetY);
			BuildKitchenArea(game, offsetY);
			BuildAnteroomArea(game, offsetY);
			BuildBathroomArea(game, offsetY);
			BuildOutsideArea(game, offsetY);
		}

		private static void BuildOutsideArea(Game game, float roomOffsetY)
		{
			float roomSizeX = 11;
			float roomSizeZ = 8;
			float roomSizeY = 1;

			float roomOffsetX = -5;
			float roomOffsetZ = 0;

			float roomBorderX = roomSizeX + roomOffsetX;
			float roomBorderZ = -roomSizeZ + roomOffsetZ;

			SimObject basement = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "basement", testEffect, Room.Outside);
			basement.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY - 0.3f, roomOffsetZ), Vector3.Zero, Vector3.One);
			basement.Initialize();
			simScenario.staticObjectList.Add(basement);

			SimObject wall24 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_windows_8_1", testEffect, Room.Outside);
			wall24.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ), Vector3.Zero, Vector3.One);
			wall24.Initialize();
			simScenario.staticObjectList.Add(wall24);

			SimObject wall25 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_windows_3_14", testEffect, Room.Outside);
			wall25.Transformation = new Transformation(new Vector3(roomOffsetX + 3, roomOffsetY, roomOffsetZ), Vector3.Zero, Vector3.One);
			wall25.Initialize();
			simScenario.staticObjectList.Add(wall25);

			SimObject wall26 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_windows_1_4", testEffect, Room.Outside);
			wall26.Transformation = new Transformation(new Vector3(roomOffsetX + 3, roomOffsetY, roomOffsetZ + 1), Vector3.Zero, Vector3.One);
			wall26.Initialize();
			simScenario.staticObjectList.Add(wall26);

			SimObject wall27 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_windows_3_15", testEffect, Room.Outside);
			wall27.Transformation = new Transformation(new Vector3(roomOffsetX + 3, roomOffsetY, roomOffsetZ + 1), Vector3.Zero, Vector3.One);
			wall27.Initialize();
			simScenario.staticObjectList.Add(wall27);

			SimObject wall28 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_windows_1_5", testEffect, Room.Outside);
			wall28.Transformation = new Transformation(new Vector3(roomOffsetX + 6, roomOffsetY, roomOffsetZ), Vector3.Zero, Vector3.One);
			wall28.Initialize();
			simScenario.staticObjectList.Add(wall28);

			SimObject wall29 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_windows_5_2", testEffect, Room.Outside);
			wall29.Transformation = new Transformation(new Vector3(roomBorderX - 5, roomOffsetY, roomOffsetZ), Vector3.Zero, Vector3.One);
			wall29.Initialize();
			simScenario.staticObjectList.Add(wall29);

			SimObject wall30 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_windows_5_3", testEffect, Room.Outside);
			wall30.Transformation = new Transformation(new Vector3(roomBorderX, roomOffsetY, roomOffsetZ), Vector3.Zero, Vector3.One);
			wall30.Initialize();
			simScenario.staticObjectList.Add(wall30);

			SimObject wall31 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_windows_4_7", testEffect, Room.Outside);
			wall31.Transformation = new Transformation(new Vector3(roomBorderX - 4, roomOffsetY, roomBorderZ + 3), Vector3.Zero, Vector3.One);
			wall31.Initialize();
			simScenario.staticObjectList.Add(wall31);

			SimObject wall32 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_windows_3_16", testEffect, Room.Outside);
			wall32.Transformation = new Transformation(new Vector3(roomBorderX - 4, roomOffsetY, roomBorderZ + 3), Vector3.Zero, Vector3.One);
			wall32.Initialize();
			simScenario.staticObjectList.Add(wall32);
			
			SimObject wall33 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_windows_7_1", testEffect, Room.Outside);
			wall33.Transformation = new Transformation(new Vector3(roomOffsetX +7, roomOffsetY, roomBorderZ), Vector3.Zero, Vector3.One);
			wall33.Initialize();
			simScenario.staticObjectList.Add(wall33);
		}

		private static void BuildBathroomArea(Game game, float roomOffsetY)
		{
			float roomSizeX = 3;
			float roomSizeZ = 1;
			float roomSizeY = 1;

			float roomOffsetX = -2;
			float roomOffsetZ = 1;

			float roomBorderX = roomSizeX + roomOffsetX;
			float roomBorderZ = -roomSizeZ + roomOffsetZ;

			SimObject floor7 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "floor7", testEffect, Room.Toilet);
			floor7.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ), Vector3.Zero,
														new Vector3(roomSizeX, roomSizeY, roomSizeZ));
			floor7.Initialize();
			simScenario.staticObjectList.Add(floor7);

			SimObject wall20 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_windows_3_12", testEffect, Room.Toilet);
			wall20.Transformation = new Transformation(new Vector3(roomBorderX, roomOffsetY, roomBorderZ), Vector3.Zero, Vector3.One);
			wall20.Initialize();
			simScenario.staticObjectList.Add(wall20);

			SimObject wall21 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_windows_1_2", testEffect, Room.Toilet);
			wall21.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomBorderZ), Vector3.Zero, Vector3.One);
			wall21.Initialize();
			simScenario.staticObjectList.Add(wall21);

			SimObject wall22 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_windows_1_3", testEffect, Room.Toilet);
			wall22.Transformation = new Transformation(new Vector3(roomBorderX, roomOffsetY, roomOffsetZ), Vector3.Zero, Vector3.One);
			wall22.Initialize();
			simScenario.staticObjectList.Add(wall22);

			SimObject wall23 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_windows_3_13", testEffect, Room.Toilet);
			wall23.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ), Vector3.Zero, Vector3.One);
			wall23.Initialize();
			simScenario.staticObjectList.Add(wall23);
		}

		private static void BuildAnteroomArea(Game game, float roomOffsetY)
		{
			float roomSizeX = 4;
			float roomSizeZ = 1;
			float roomSizeY = 1;

			float roomOffsetX = -2;
			float roomOffsetZ = 0;

			float roomBorderX = roomSizeX + roomOffsetX;
			float roomBorderZ = -roomSizeZ + roomOffsetZ;

			SimObject floor6 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "floor6", testEffect, Room.Anteroom);
			floor6.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ), Vector3.Zero,
														new Vector3(roomSizeX, roomSizeY, roomSizeZ));
			floor6.Initialize();
			simScenario.staticObjectList.Add(floor6);

			SimObject wall17 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_windows_3_10", testEffect, Room.Anteroom);
			wall17.Transformation = new Transformation(new Vector3(roomBorderX - 1, roomOffsetY, roomBorderZ), Vector3.Zero, Vector3.One);
			wall17.Initialize();
			simScenario.staticObjectList.Add(wall17);

			SimObject wall18 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_windows_1", testEffect, Room.Anteroom);
			wall18.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomBorderZ), Vector3.Zero, Vector3.One);
			wall18.Initialize();
			simScenario.staticObjectList.Add(wall18);

			SimObject wall19 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_windows_3_11", testEffect, Room.Anteroom);
			wall19.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ), Vector3.Zero, Vector3.One);
			wall19.Initialize();
			simScenario.staticObjectList.Add(wall19);
		}

		private static void BuildKitchenArea(Game game, float roomOffsetY)
		{
			float roomSizeX = 3;
			float roomSizeZ = 3;
			float roomSizeY = 1;

			float roomOffsetX = -2;
			float roomOffsetZ = -1;

			float roomBorderX = roomSizeX + roomOffsetX;
			float roomBorderZ = -roomSizeZ + roomOffsetZ;

			SimObject floor5 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "floor5", testEffect, Room.Kitchen);
			floor5.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ), Vector3.Zero,
														new Vector3(roomSizeX + 1, roomSizeY, roomSizeZ));
			floor5.Initialize();
			simScenario.staticObjectList.Add(floor5);

			SimObject wall15 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_windows_3_8", testEffect, Room.Kitchen);
			wall15.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomBorderZ), Vector3.Zero, Vector3.One);
			wall15.Initialize();
			simScenario.staticObjectList.Add(wall15);

			SimObject wall16 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_windows_3_9", testEffect, Room.Kitchen);
			wall16.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ), Vector3.Zero, Vector3.One);
			wall16.Initialize();
			simScenario.staticObjectList.Add(wall16);
		}

		private static void BuildGarageArea(Game game, float roomOffsetY)
		{
			float roomSizeX = 3;
			float roomSizeZ = 4;
			float roomSizeY = 1;

			float roomOffsetX = -5;
			float roomOffsetZ = 0;

			float roomBorderX = roomSizeX + roomOffsetX;
			float roomBorderZ = -roomSizeZ + roomOffsetZ;

			SimObject floor4 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "floor4", testEffect, Room.Garage);
			floor4.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ), Vector3.Zero,
														new Vector3(roomSizeX, roomSizeY, roomSizeZ));
			floor4.Initialize();
			simScenario.staticObjectList.Add(floor4);

			SimObject wall11 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_windows_3_6", testEffect, Room.Garage);
			wall11.Transformation = new Transformation(new Vector3(roomBorderX, roomOffsetY, roomBorderZ), Vector3.Zero, Vector3.One);
			wall11.Initialize();
			simScenario.staticObjectList.Add(wall11);

			SimObject wall12 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_windows_4_5", testEffect, Room.Garage);
			wall12.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomBorderZ), Vector3.Zero, Vector3.One);
			wall12.Initialize();
			simScenario.staticObjectList.Add(wall12);

			SimObject wall13 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_windows_3_7", testEffect, Room.Garage);
			wall13.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ), Vector3.Zero, Vector3.One);
			wall13.Initialize();
			simScenario.staticObjectList.Add(wall13);

			SimObject wall14 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_windows_4_6", testEffect, Room.Garage);
			wall14.Transformation = new Transformation(new Vector3(roomBorderX, roomOffsetY, roomOffsetZ), Vector3.Zero, Vector3.One);
			wall14.Initialize();
			simScenario.staticObjectList.Add(wall14);
		}

		private static void BuildBedroomArea(Game game, float roomOffsetY)
		{
			float roomSizeX = 3;
			float roomSizeZ = 4;
			float roomSizeY = 1;

			float roomOffsetX = -5;
			float roomOffsetZ = -4;

			float roomBorderX = roomSizeX + roomOffsetX;
			float roomBorderZ = -roomSizeZ + roomOffsetZ;

			SimObject floor3 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "floor3", testEffect, Room.Bedroom);
			floor3.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ), Vector3.Zero,
														new Vector3(roomSizeX, roomSizeY, roomSizeZ));
			floor3.Initialize();
			simScenario.staticObjectList.Add(floor3);

			SimObject wall7 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_windows_3_3", testEffect, Room.Bedroom);
			wall7.Transformation = new Transformation(new Vector3(roomBorderX, roomOffsetY, roomBorderZ), Vector3.Zero, Vector3.One);
			wall7.Initialize();
			simScenario.staticObjectList.Add(wall7);

			SimObject wall8 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_windows_3_4", testEffect, Room.Bedroom);
			wall8.Transformation = new Transformation(new Vector3(roomBorderX, roomOffsetY, roomBorderZ), Vector3.Zero, Vector3.One);
			wall8.Initialize();
			simScenario.staticObjectList.Add(wall8);

			SimObject wall9 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_windows_4_4", testEffect, Room.Bedroom);
			wall9.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomBorderZ), Vector3.Zero, Vector3.One);
			wall9.Initialize();
			simScenario.staticObjectList.Add(wall9);

			SimObject wall10 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_windows_3_5", testEffect, Room.Bedroom);
			wall10.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ), Vector3.Zero, Vector3.One);
			wall10.Initialize();
			simScenario.staticObjectList.Add(wall10);
		}

		private static void BuildDiningArea(Game game, float roomOffsetY)
		{
			float roomSizeX = 4;
			float roomSizeZ = 3;
			float roomSizeY = 1;

			float roomOffsetX = -2;
			float roomOffsetZ = -5;

			float roomBorderX = roomSizeX + roomOffsetX;
			float roomBorderZ = - roomSizeZ + roomOffsetZ;

			SimObject floor2 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "floor2", testEffect, Room.Dining);
			floor2.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ+1), Vector3.Zero,
														new Vector3(roomSizeX, roomSizeY, roomSizeZ+1));
			floor2.Initialize();
			simScenario.staticObjectList.Add(floor2);

			SimObject wall4 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_windows_3", testEffect, Room.Dining);
			wall4.Transformation = new Transformation(new Vector3(roomBorderX, roomOffsetY, roomOffsetZ), Vector3.Zero, Vector3.One);
			wall4.Initialize();
			simScenario.staticObjectList.Add(wall4);

			SimObject wall5 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_windows_4_3", testEffect, Room.Dining);
			wall5.Transformation = new Transformation(new Vector3(roomBorderX, roomOffsetY, roomBorderZ), Vector3.Zero, Vector3.One);
			wall5.Initialize();
			simScenario.staticObjectList.Add(wall5);

			SimObject wall6 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_windows_3_2", testEffect, Room.Dining);
			wall6.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomBorderZ), Vector3.Zero, Vector3.One);
			wall6.Initialize();
			simScenario.staticObjectList.Add(wall6);
		}

		private static void BuildLivingArea(Game game, float roomOffsetY)
		{
			float roomSizeX = 4.0f;
			float roomSizeZ = 5.0f;
			float roomSizeY = 1.0f;

			float roomOffsetX = 2;
			float roomOffsetZ = 0;

			float roomBorderX = roomSizeX + roomOffsetX;
			float roomBorderZ = roomSizeZ + roomOffsetZ;

			SimObject floor1 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "floor", testEffect, Room.Living);
			floor1.Transformation = new Transformation(new Vector3(roomOffsetX, roomOffsetY, roomOffsetZ),
														Vector3.Zero,
														new Vector3(roomSizeX , 1.0f, roomBorderZ));
			floor1.Initialize();
			simScenario.staticObjectList.Add(floor1);

			SimObject wall1 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_windows_4", testEffect, Room.Living);
			wall1.Transformation = new Transformation(new Vector3(0,roomOffsetY,0),
														Vector3.Zero,
														Vector3.One);
			wall1.Initialize();
			simScenario.staticObjectList.Add(wall1);

			SimObject wall2 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_windows_5", testEffect, Room.Living);
			wall2.Transformation = new Transformation(new Vector3(roomBorderX, roomOffsetY, roomOffsetZ),
															Vector3.Zero,
															Vector3.One);
			wall2.Initialize();
			simScenario.staticObjectList.Add(wall2);

			SimObject wall3 = new SimObject(game, SimAssetsPath.MODELS_BUILDING_PATH + "wall_windows_4_2", testEffect, Room.Living);
			wall3.Transformation = new Transformation(new Vector3(roomBorderX, roomOffsetY, -roomSizeZ), Vector3.Zero, Vector3.One);
			wall3.Initialize();
			simScenario.staticObjectList.Add(wall3);

			//SimObject smokeSensor = new SimObject(game, "SmokeSensor", testEffect, Room.Living);
			//smokeSensor.Transformation = new Transformation(new Vector3(roomBorderX / 2, roomSizeY + roomOffsetY, -roomBorderZ / 2),
			//                                            new Vector3(0.0f, 0.0f, 0.0f),
			//                                            new Vector3(0.08f, 0.05f, 0.08f));
			//smokeSensor.Initialize();
			//simScenario.staticObjectList.Add(smokeSensor);

			SimObject tv = new SimObject(game, "tv", testEffect, Room.Living);
			tv.Transformation = new Transformation(new Vector3(roomBorderX - (0.5f / 2), roomOffsetY, -roomBorderZ / 2),
														new Vector3(0.0f, -90.0f, 0.0f),
														new Vector3(0.2f, 0.2f, 0.2f));
			tv.Initialize();
			simScenario.staticObjectList.Add(tv);

			PobicosLamp lamp = new PobicosLamp(game, "lampOn", testEffect, Room.Living,
												SimAssetsPath.POBICOS_OBJECTS_PATH + "LampTestTossim_res.xml");
			lamp.Transformation = new Transformation(new Vector3(roomBorderX / 2, roomOffsetY, -roomBorderZ + 0.35f),
														new Vector3(0.0f, -90.0f, 0.0f),
														new Vector3(0.15f, 0.15f, 0.15f));
			lamp.objectState = PobicosLamp.ObjectState.OFF;
			lamp.Initialize();
			simScenario.pobicosObjectList.Add(lamp);

			//SmokeSensor smokeSensor = new SmokeSensor(game, "SmokeSensor", testEffect, Room.Living,
			//                                    SimAssetsPath.POBICOS_OBJECTS_PATH + "SmokeSensor.xml");
			//smokeSensor.Transformation = new Transformation(new Vector3(roomBorderX / 2, roomSizeY + roomOffsetY, -roomBorderZ / 2),
			//                                            new Vector3(0.0f, 0.0f, 0.0f),
			//                                            new Vector3(0.08f, 0.05f, 0.08f));
			//smokeSensor.Initialize();
			//simScenario.staticObjectList.Add(smokeSensor);
			//smokeSensor.simScenario.pobicosObjectList.Add(smokeSensor);

			simScenario.client.Connect();
		}


		public static void PutFire(Game game, Vector3 position)
		{
			SimObject smoke = new SimObject(game, "smoke", testEffect, simScenario.GetActiveHuman().model.room);
			smoke.Transformation = new Transformation(position, Vector3.Zero, new Vector3(0.05f, 0.05f, 0.05f));
			smoke.Initialize();
			simScenario.movingObjectList.Add(smoke);
		}
		
		public static void AddCameras(Game game)
		{
            float aspectRatio = (float)game.GraphicsDevice.Viewport.Width / game.GraphicsDevice.Viewport.Height;

			ThirdPersonCamera followCamera = new ThirdPersonCamera();
			followCamera.SetPerspectiveFov(60.0f, aspectRatio, 0.1f, 700);
			followCamera.SetChaseParameters(3.0f, 9.0f, 7.0f, 14.0f);
			followCamera.SetLookAt(new Vector3(2.0f, 4.0f, 2.5f), new Vector3(2.0f, 0.0f, -2.0f), Vector3.Up);

			simScenario.cameraManager = new CameraManager();
			simScenario.cameraManager.Add("FollowCamera", followCamera);
		}
	}
}
