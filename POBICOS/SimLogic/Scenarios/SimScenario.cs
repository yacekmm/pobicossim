﻿using POBICOS.SimBase.Cameras;
using POBICOS.SimBase.Lights;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PobicosLibrary;

namespace POBICOS.SimLogic.Scenarios
{
	class SimScenario
	{
		public CameraManager cameraManager;
		public LightManager lightManager;

		public List<Human> humanList;
		public List<SimObject> staticObjectList;
		public List<SimObject> movingObjectList;
		public List<PobicosSimObject> pobicosObjectList;

		public string objectsConfigFile = "";

		//public bool lampOn = false;

		public SimScenario()
		{
			humanList = new List<Human>();
			staticObjectList = new List<SimObject>();
			movingObjectList = new List<SimObject>();
			pobicosObjectList = new List<PobicosSimObject>();
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
				if (so.name == name)
					return so;
			
			foreach(SimObject so in movingObjectList)
				if (so.name == name)
					return so;

			return null;
		}

		public PobicosSimObject GetPobicosObjectByName(string name)
		{
			foreach (PobicosSimObject pso in pobicosObjectList)
				if (pso.name == name)
					return pso;
			
			return null;
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
			if (pobicosObjectList != null)
				foreach (PobicosSimObject pso in pobicosObjectList)
					pso.Update(gameTime);
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
						if (so.Transformation.Translate.Y > GetObjectByName("SmokeSensor").Transformation.Translate.Y)
							removeSmoke = true;
						else
						{
							so.Transformation.Translate += new Vector3(0.0f, 0.01f, 0.0f);
							//so.Transformation.Rotate += new Vector3(0.0f, 1.5f, 0.0f);
						}
					}
				}

			if (removeSmoke)
				movingObjectList.Remove(GetObjectByName("smoke"));
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
			if (pobicosObjectList != null)
				foreach (PobicosSimObject pso in pobicosObjectList)
				{
					if (!pso.name.Equals("lampOn"))
						pso.Draw(gameTime);
					else
						if (pso.objectState.Equals(PobicosSimObject.ObjectState.ON))
							pso.Draw(gameTime);
				}
		}

		public void DrawMovingObjects(GameTime gameTime)
		{
			if (movingObjectList != null)
				foreach (SimObject so in movingObjectList)
					so.Draw(gameTime);
		}

		internal void UnloadPobicosObjects()
		{
			foreach (PobicosSimObject pso in pobicosObjectList)
				pso.Client.Disconnect();
		}
	}
}