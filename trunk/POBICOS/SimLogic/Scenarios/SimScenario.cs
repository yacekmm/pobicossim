using POBICOS.SimBase.Cameras;
using POBICOS.SimBase.Lights;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PobicosLibrary;
using POBICOS.SimBase.Effects;

namespace POBICOS.SimLogic.Scenarios
{
	class SimScenario
	{
		public CameraManager cameraManager;
		public LightManager lightManager;

		public List<Human> humanList;
		public List<SimObject> staticObjectList;
		public List<SimObject> movingObjectList;
		public List<PobicosLamp> pobicosObjectList;

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
			pobicosObjectList = new List<PobicosLamp>();

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

		public PobicosLamp GetPobicosObjectByName(string name)
		{
			foreach (PobicosLamp pso in pobicosObjectList)
				if (pso.name.Contains(name))
					return pso;

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
				foreach (PobicosLamp pso in pobicosObjectList)
					if (pso.model.room.Equals(room))
					{
						pso.model.basicEffectManager.Light0Direction *= new Vector3(difference);
						pso.model.basicEffectManager.Light1Direction *= new Vector3(difference);
						pso.model.basicEffectManager.Light2Direction *= new Vector3(difference);
					}

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
			if (pobicosObjectList != null)
				foreach (PobicosLamp pso in pobicosObjectList)
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
						if((so.Transformation.Translate.Y - 0.7f) > GetObjectByName("SmokeSensor").Transformation.Translate.Y)
							removeSmoke = true;
						else
						{
							if ((so.Transformation.Translate.Y + 0.3f) > GetObjectByName("SmokeSensor").Transformation.Translate.Y)
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
			if (pobicosObjectList != null)
				foreach (PobicosLamp pso in pobicosObjectList)
					pso.Draw(gameTime);
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
