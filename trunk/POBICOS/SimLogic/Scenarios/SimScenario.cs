using POBICOS.SimBase.Cameras;
//using POBICOS.SimBase.Lights;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PobicosLibrary;
using POBICOS.SimBase.Effects;
using System;
using POBICOS.SimLogic.PobicosObjects;
using Microsoft.Xna.Framework.Graphics;

namespace POBICOS.SimLogic.Scenarios
{
	public class SimScenario
	{
		public CameraManager cameraManager;
		//public LightManager lightManager;
        private static SimScenario instance;
		public List<Human> humanList;
		public static List<SimObject> staticObjectList;
		public static List<SimObject> furnitureList;
		public static List<SimObject> movingObjectList;
		public static List<SimObject> pobicosObjectList;

		public Vector3 cameraUpOffset = new Vector3(0, 0.9f, 0);
        
        public static Client Client   { get; private set; }
		public BasicEffectManager basicEffectManager;

		public string objectsConfigFile = "";

		public bool eventSent = false;

        public static SimScenario Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new SimScenario();
                    SimScenario.Client = new Client();
                }

                return instance;
            }
            private set { instance = value; }
        }

		private SimScenario()
		{
            if (instance == null)
            {
                humanList = new List<Human>();
                staticObjectList = new List<SimObject>();
                movingObjectList = new List<SimObject>();
                pobicosObjectList = new List<SimObject>();
				furnitureList = new List<SimObject>();
                basicEffectManager = new BasicEffectManager();
            }
		}

		#region Getting Objects Methods
		public Human GetActiveHuman()
		{
			foreach (Human h in humanList)
				if (h.isActive)
					return h;
			
			return null;
		}

		public SimObject GetObjectByName(string name)
		{
			foreach (SimObject so in movingObjectList)
				if (so.name.Contains(name))
					return so; 
			
			foreach (SimObject so in staticObjectList)
				if (so.name.Contains(name))
					return so;

			foreach (SimObject so in furnitureList)
				if (so.name.Contains(name))
					return so;

			return null;
		}

		public Object GetPobicosObjectByName(string name, Room room)
		{
			Type type;
			if (pobicosObjectList != null)
				foreach (Object ob in pobicosObjectList)
				{
					type = ob.GetType();

					if (type.Equals(typeof(Tv)))
					{
						if (((Tv)ob).name.Contains(name) && ((Tv)ob).model.room.Equals(room))
							return ob;
					}
					else if (type.Equals(typeof(PobicosLamp)))
					{
						if (((PobicosLamp)ob).name.Contains(name) && ((PobicosLamp)ob).model.room.Equals(room))
							return ob;
					}
					else if (type.Equals(typeof(SmokeSensor)))
					{
						if (((SmokeSensor)ob).name.Contains(name) && ((SmokeSensor)ob).model.room.Equals(room))
							return ob;
					}
					else if (type.Equals(typeof(Thermometer)))
					{
						if (((Thermometer)ob).name.Contains(name) && ((Thermometer)ob).model.room.Equals(room))
							return ob;
					}
				}
			return null;
		}

		public Object GetPobicosObjectByName(string name)
		{
			Type type;
			if (pobicosObjectList != null)
				foreach (Object ob in pobicosObjectList)
				{
					type = ob.GetType();

					if (type.Equals(typeof(Tv)))
					{
						if (((Tv)ob).name.Contains(name))
							return ob;
					}
					else if (type.Equals(typeof(PobicosLamp)))
					{
						if (((PobicosLamp)ob).name.Contains(name))
							return ob;
					}
					else if (type.Equals(typeof(SmokeSensor)))
					{
						if (((SmokeSensor)ob).name.Contains(name))
							return ob;
					}
					else if (type.Equals(typeof(Thermometer)))
					{
						if (((Thermometer)ob).name.Contains(name))
							return ob;
					}
				}

			return null;
		}
		#endregion

		#region Update Methods
		public void UpdateHumans(GameTime gameTime)
		{
			if (humanList != null)
				foreach (Human h in humanList)
					h.Update(gameTime);
		}

		public void UpdateStaticObjects(GameTime gameTime)
		{
			if (staticObjectList != null)
				foreach (SimObject so in staticObjectList)
					so.Update(gameTime);

			if (furnitureList != null)
				foreach (SimObject so in furnitureList)
					so.Update(gameTime);
		}

		public void UpdatePobicosObjects(GameTime gameTime)
		{
			Type type;
			if (pobicosObjectList != null)
				foreach (Object ob in pobicosObjectList)
				{
					type = ob.GetType();
					if (type.Equals(typeof(Tv)))
						((Tv)ob).Update(gameTime);
					else if (type.Equals(typeof(PobicosLamp)))
						((PobicosLamp)ob).Update(gameTime);
					else if (type.Equals(typeof(SmokeSensor)))
						((SmokeSensor)ob).Update(gameTime);
					else if (type.Equals(typeof(Thermometer)))
						((Thermometer)ob).Update(gameTime);
				}
		}

		public void UpdateMovingObjects(GameTime gameTime)
		{
			bool removeSmoke = false;
			List<Vector3> smokePositions = new List<Vector3>();
			Random rnd = new Random();
			SimObject toRemove = null;
			float scaleFactor = 0.06f;

			if (movingObjectList != null)
				foreach (SimObject so in movingObjectList)
				{
					so.Update(gameTime);
					#region Update Smoke
					if (so.name.Equals("smoke"))
					{
						if ((so.Transformation.Translate.Y - 0.9f) > ((SmokeSensor)GetPobicosObjectByName("SmokeSensor")).Transformation.Translate.Y)
							removeSmoke = true;
						else
						{
							if ((so.Transformation.Translate.Y + 0.3f) > ((SmokeSensor)GetPobicosObjectByName("SmokeSensor")).Transformation.Translate.Y)
								so.Transformation.Scale *= new Vector3(0.98f);
							else
								so.Transformation.Scale *= new Vector3(1.006f, 0.997f, 1.0f);
							so.Transformation.Translate += new Vector3(0.0f, 0.01f, 0.0f);
						}
					}
					#endregion
					#region Update Fire
					if (so.name.Equals("Fire"))
					{
						if (gameTime.TotalGameTime.Milliseconds % 4000 == 0)
							smokePositions.Add(so.Transformation.Translate);

						if (gameTime.TotalGameTime.Milliseconds % 20 == 0)
						{
							so.model.Rotate += new Vector3(0, ((float)rnd.NextDouble() - 0.5f) * 9f, 0);
							so.model.Scale += new Vector3(((float)rnd.NextDouble() % scaleFactor) - scaleFactor/2,
								0,
								((float)rnd.NextDouble() % scaleFactor) - scaleFactor/2);
							so.model.Scale -= new Vector3(0.001f);
							
							if (so.model.Scale.Y < 0.1f)
								toRemove = so;
						}
					}
					#endregion
				}

			movingObjectList.Remove(toRemove);

			if (removeSmoke)
			{
				movingObjectList.Remove(GetObjectByName("smoke"));
				eventSent = false;
			}
			
			foreach(Vector3 pos in smokePositions)
				ScenarioBuilder.PutSmoke(new Vector3(
					pos.X + (float)rnd.NextDouble() * 0.7f - 0.5f,
					0.45f,
					pos.Z + (float)rnd.NextDouble() * 0.7f - 0.5f),
					(float)rnd.NextDouble());
		}

		internal void UpdateLights(GameTime gameTime)
		{
			//update buliding lights
			foreach (SimObject so in staticObjectList)
				so.model.basicEffectManager.Light0Direction = cameraManager.ActiveCamera.HeadingVector;

			//update skybox lights
			GetObjectByName("skybox").model.basicEffectManager.Light0Direction =
				cameraManager.ActiveCamera.HeadingVector + new Vector3(0, 1.0f, 0);
			GetObjectByName("skybox").model.basicEffectManager.Light1Direction =
				cameraManager.ActiveCamera.HeadingVector + new Vector3(-0.8f, 0.8f, 0);
			GetObjectByName("skybox").model.basicEffectManager.Light2Direction =
				cameraManager.ActiveCamera.HeadingVector + new Vector3(0.8f, 0.8f, 0);
		}
		#endregion

		#region Drawing Methods
		public void DrawHumans(GameTime gameTime)
		{
			if (humanList != null)
				foreach (Human h in humanList)
					h.Draw(gameTime);
		}

		public void DrawStaticObjects(GameTime gameTime)
		{
			if (staticObjectList != null)
				foreach (SimObject so in staticObjectList)
					so.Draw(gameTime);

			if (furnitureList != null)
				foreach (SimObject so in furnitureList)
					so.Draw(gameTime);
		}

		public void DrawPobicosObjects(GameTime gameTime)
		{
			Type type;
			if (pobicosObjectList != null)
				foreach (Object ob in pobicosObjectList)
				{
					type = ob.GetType();
				
					if (type.Equals(typeof(Tv)))
						((Tv)ob).Draw(gameTime);
					else if (type.Equals(typeof(PobicosLamp)))
						((PobicosLamp)ob).Draw(gameTime);
					else if (type.Equals(typeof(SmokeSensor)))
						((SmokeSensor)ob).Draw(gameTime);
					else if (type.Equals(typeof(Thermometer)))
					{
						((Thermometer)ob).Draw(gameTime);
						if (Math.Abs(gameTime.TotalGameTime.Seconds - ((Thermometer)ob).lastTempCheck.Seconds) > 5)
							((Thermometer)ob).CheckTemperature(99, gameTime);
					}
				}
		}

		public void DrawMovingObjects(GameTime gameTime)
		{
			if (movingObjectList != null)
				foreach (SimObject so in movingObjectList)
					so.Draw(gameTime);
		}
		#endregion

		#region Handling POBICOS connected actions
		internal void InteractWithObject(SimModel human, float distance)
		{
			Type type;
			if (pobicosObjectList != null)
				foreach (Object ob in pobicosObjectList)
				{
					type = ob.GetType();
					if (type.Equals(typeof(Tv)))
					{
						if (SimScreen.CheckIntersection(human, ((Tv)ob).model, distance))
							((Tv)ob).Interact();
					}
					else if (type.Equals(typeof(PobicosLamp)))
					{
						if (SimScreen.CheckIntersection(human, ((PobicosLamp)ob).model, distance))
							((PobicosLamp)ob).Interact();
					}
					else if (type.Equals(typeof(SmokeSensor)))
					{
						if (SimScreen.CheckIntersection(human, ((SmokeSensor)ob).model, distance))
							((SmokeSensor)ob).Interact();
					}
					else if (type.Equals(typeof(Thermometer)))
					{
						if (SimScreen.CheckIntersection(human, ((Thermometer)ob).model, distance))
							((Thermometer)ob).Interact();
					}
				}

			SimObject car = GetObjectByName("car_");
			if (car != null)
				if (SimScreen.CheckIntersection(human, car.model, distance * 2))
					ScenarioBuilder.PutFire(car.Transformation.Translate + new Vector3(0, 0.3f, 0.55f));

			SimObject kitchen = GetObjectByName("kitchen");
			if (kitchen != null)
			{
				Vector3 ovenRelativePosition = new Vector3(0.4f, 0.07f, -1.2f);
				kitchen.model.Transformation.Translate += ovenRelativePosition;

				if (SimScreen.CheckIntersection(human, kitchen.model, distance))
					ScenarioBuilder.PutFire(kitchen.Transformation.Translate + new Vector3(0, 0.2f, 0));

				kitchen.model.Transformation.Translate -= ovenRelativePosition;
			}

			SimObject couch = GetObjectByName("Couch");
			if (couch != null)
				if (SimScreen.CheckIntersection(human, couch.model, distance * 1.4f))
					ScenarioBuilder.PutFire(couch.Transformation.Translate + new Vector3(0));
		}

		public static void SwitchLight(Room room, bool value)
		{
			float difference = 1.6f;

			if (!value)
				difference = 1 / difference;

			if (staticObjectList != null)
				foreach (SimObject so in staticObjectList)
					if (so.model.room.Equals(room))
					{
						so.model.basicEffectManager.Light0Direction *= new Vector3(difference);
						so.model.basicEffectManager.Light1Direction *= new Vector3(difference);
						so.model.basicEffectManager.Light2Direction *= new Vector3(difference);
					}

			if (pobicosObjectList != null)
			{
				Type type;
				foreach (Object ob in pobicosObjectList)
				{
					type = ob.GetType();

					if (type.Equals(typeof(Tv)))
					{
						if (((Tv)ob).model.room.Equals(room))
						{
							((Tv)ob).model.basicEffectManager.Light0Direction *= new Vector3(difference);
							((Tv)ob).model.basicEffectManager.Light1Direction *= new Vector3(difference);
							((Tv)ob).model.basicEffectManager.Light2Direction *= new Vector3(difference);
						}
					}
					else if (type.Equals(typeof(PobicosLamp)))
					{
						if (((PobicosLamp)ob).model.room.Equals(room))
						{
							((PobicosLamp)ob).model.basicEffectManager.Light0Direction *= new Vector3(difference);
							((PobicosLamp)ob).model.basicEffectManager.Light1Direction *= new Vector3(difference);
							((PobicosLamp)ob).model.basicEffectManager.Light2Direction *= new Vector3(difference);
						}
					}
					else if (type.Equals(typeof(SmokeSensor)))
					{
						if (((SmokeSensor)ob).model.room.Equals(room))
						{
							((SmokeSensor)ob).model.basicEffectManager.Light0Direction *= new Vector3(difference);
							((SmokeSensor)ob).model.basicEffectManager.Light1Direction *= new Vector3(difference);
							((SmokeSensor)ob).model.basicEffectManager.Light2Direction *= new Vector3(difference);
						}
					}
					else if (type.Equals(typeof(Thermometer)))
					{
						if (((Thermometer)ob).model.room.Equals(room))
						{
							((Thermometer)ob).model.basicEffectManager.Light0Direction *= new Vector3(difference);
							((Thermometer)ob).model.basicEffectManager.Light1Direction *= new Vector3(difference);
							((Thermometer)ob).model.basicEffectManager.Light2Direction *= new Vector3(difference);
						}
					}
				}
			}

			if (movingObjectList != null)
				foreach (SimObject so in movingObjectList)
					if (so.model.room.Equals(room))
					{
						so.model.basicEffectManager.Light0Direction *= new Vector3(difference);
						so.model.basicEffectManager.Light1Direction *= new Vector3(difference);
						so.model.basicEffectManager.Light2Direction *= new Vector3(difference);
					}

			if (furnitureList != null)
				foreach (SimObject so in furnitureList)
					if (so.model.room.Equals(room))
					{
						so.model.basicEffectManager.Light0Direction *= new Vector3(difference);
						so.model.basicEffectManager.Light1Direction *= new Vector3(difference);
						so.model.basicEffectManager.Light2Direction *= new Vector3(difference);
					}
		}
		#endregion
	}
}
