﻿using POBICOS.SimBase.Cameras;
using POBICOS.SimBase.Lights;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PobicosLibrary;
using POBICOS.SimBase.Effects;
using System;
using POBICOS.SimLogic.PobicosObjects;

namespace POBICOS.SimLogic.Scenarios
{
	class SimScenario
	{
		public CameraManager cameraManager;
		public LightManager lightManager;

		public List<Human> humanList;
		public List<SimObject> staticObjectList;
		public static List<SimObject> movingObjectList;
		public static List<Object> pobicosObjectList;

		public Vector3 cameraUpOffset = new Vector3(0, 0.9f, 0);
		public static Client client =  new Client();
		public BasicEffectManager basicEffectManager;

		public string objectsConfigFile = "";

		public bool eventSent = false;		//tymczasowe rozwiązanie

		public SimScenario()
		{
			humanList = new List<Human>();
			staticObjectList = new List<SimObject>();
			movingObjectList = new List<SimObject>();
			pobicosObjectList = new List<Object>();

			basicEffectManager = new BasicEffectManager();
		}

		public Human GetActiveHuman()
		{
			foreach (Human h in humanList)
				if (h.isActive)
					return h;
			
			return null;
		}

		public SimObject GetObjectByName(string name)
		{
			foreach (SimObject so in staticObjectList)
				if (so.name.Contains(name))
					return so;
			
			foreach(SimObject so in movingObjectList)
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

		public void SwitchLight(Room room, bool value)
		{
			float difference = 1.6f;

			if (!value)
				difference = 1/difference;

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
						((Tv)ob).model.basicEffectManager.Light0Direction *= new Vector3(difference);
						((Tv)ob).model.basicEffectManager.Light1Direction *= new Vector3(difference);
						((Tv)ob).model.basicEffectManager.Light2Direction *= new Vector3(difference);
					}
					else if (type.Equals(typeof(PobicosLamp)))
					{
						((PobicosLamp)ob).model.basicEffectManager.Light0Direction *= new Vector3(difference);
						((PobicosLamp)ob).model.basicEffectManager.Light1Direction *= new Vector3(difference);
						((PobicosLamp)ob).model.basicEffectManager.Light2Direction *= new Vector3(difference);
					}
					else if (type.Equals(typeof(SmokeSensor)))
					{
						((SmokeSensor)ob).model.basicEffectManager.Light0Direction *= new Vector3(difference);
						((SmokeSensor)ob).model.basicEffectManager.Light1Direction *= new Vector3(difference);
						((SmokeSensor)ob).model.basicEffectManager.Light2Direction *= new Vector3(difference);
					}
					else if (type.Equals(typeof(Thermometer)))
					{
						((Thermometer)ob).model.basicEffectManager.Light0Direction *= new Vector3(difference);
						((Thermometer)ob).model.basicEffectManager.Light1Direction *= new Vector3(difference);
						((Thermometer)ob).model.basicEffectManager.Light2Direction *= new Vector3(difference);
					}
				}
			}
			//if (pobicosObjectList != null)
			//    foreach (PobicosLamp pso in pobicosObjectList)
			//        if (pso.model.room.Equals(room))
			//        {
			//            pso.model.basicEffectManager.Light0Direction *= new Vector3(difference);
			//            pso.model.basicEffectManager.Light1Direction *= new Vector3(difference);
			//            pso.model.basicEffectManager.Light2Direction *= new Vector3(difference);
			//        }

			if (movingObjectList != null)
				foreach (SimObject so in movingObjectList)
					if (so.model.room.Equals(room))
					{
						so.model.basicEffectManager.Light0Direction *= new Vector3(difference);
						so.model.basicEffectManager.Light1Direction *= new Vector3(difference);
						so.model.basicEffectManager.Light2Direction *= new Vector3(difference);
					}
		}

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
		}

		public void UpdatePobicosObjects(GameTime gameTime)
		{
			Type type;
			if (pobicosObjectList != null)
				foreach (Object ob in pobicosObjectList)
				{
					type = ob.GetType();
					if(type.Equals(typeof (Tv)))
						((Tv)ob).Update(gameTime);
					else if(type.Equals(typeof (PobicosLamp)))
						((PobicosLamp)ob).Update(gameTime);
					else  if(type.Equals(typeof (SmokeSensor)))
						((SmokeSensor)ob).Update(gameTime);
					else  if(type.Equals(typeof (Thermometer)))
						((Thermometer)ob).Update(gameTime);
				}


				//foreach (PobicosLamp pso in pobicosObjectList)
				//    pso.Update(gameTime);
		}

		public void UpdateMovingObjects(GameTime gameTime)
		{
			bool removeSmoke = false;

			if (movingObjectList != null)
				foreach (SimObject so in movingObjectList)
				{
					so.Update(gameTime);
					if (so.name.Equals("smoke"))
					{
						if((so.Transformation.Translate.Y - 0.7f) > ((SmokeSensor)GetPobicosObjectByName("SmokeSensor")).Transformation.Translate.Y)
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
				}

			if (removeSmoke)
			{
				movingObjectList.Remove(GetObjectByName("smoke"));
				eventSent = false;
			}
		}

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
		}

		public void DrawPobicosObjects(GameTime gameTime)
		{
			//if (pobicosObjectList != null)
			//    foreach (PobicosLamp pso in pobicosObjectList)
			//        pso.Draw(gameTime);

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
						((Thermometer)ob).Draw(gameTime);
				}
		}

		public void DrawMovingObjects(GameTime gameTime)
		{
			if (movingObjectList != null)
				foreach (SimObject so in movingObjectList)
					so.Draw(gameTime);
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
	}
}
