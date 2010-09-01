using POBICOS.SimBase.Cameras;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using PobicosLibrary;
using POBICOS.SimBase.Effects;
using System;
using POBICOS.SimLogic.PobicosObjects;
using Microsoft.Xna.Framework.Graphics;

namespace POBICOS.SimLogic.Scenarios
{
	/// <summary>
	/// Performs all actions connected with current scenario that is in place.
	/// </summary>
	/// <remarks>Responsible for updating, moving, interacting with objects, changing lights etc.</remarks>
	public class SimScenario
	{
		/// <summary><o>CameraManager</o> object</summary>
		public CameraManager cameraManager;

		/// <summary>static variable (singleton) of <o>SimScenario</o></summary>
        private static SimScenario instance;

		/// <summary>A collection of players in the simulator</summary>
		public static List<Human> humanList;

		/// <summary>Collection of static 3D objects such as walls, floors, doors</summary>
		public static List<SimObject> staticObjectList;

		/// <summary>Collection of furniture (objects in encironment for which collision check with player must be performed).</summary>
		public static List<SimObject> furnitureList;

		/// <summary>Colection of moving (animating) 3D obbjects for which specific action during rendering must be performed
		/// (i.e. changing smoke position)</summary>
		public static List<SimObject> movingObjectList;

		/// <summary>Collection of 3D objects that act as POBICOS-enabled objects. Collision check with player is performed.</summary>
		public static List<SimObject> pobicosObjectList;

		/// <summary>Camera looks slightly higher than human position.</summary>
		public Vector3 cameraUpOffset = new Vector3(0, 0.9f, 0);
        
		/// <summary>BISP Library <o>Client</o> instance hold to handle communication with POBICOS Management Server</summary>
        public static Client Client   { get; private set; }

		/// <summary>Variable controlling minimum Light0 intensity</summary>
		public static float minLight0Length = 0.45f;

		/// <summary>Variable controlling maximum Light0 intensity</summary>
		public static float maxLight0Length = 1.0f;

		/// <summary>Flag pointing if light intensity in environment should be increased (true) or decreased (false)</summary>
		bool increaseLightValue = true;

		/// <summary>Flag indicating that POBICOS event was recently sent to Management Server. Introduced to avoid flooding Server</summary>
		public bool eventSent = false;

		#region Properties
		/// <summary>
		/// Get or set singleton
		/// </summary>
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
		#endregion

		/// <summary>
		/// <o>SimScenario</o> constructor.
		/// </summary>
		private SimScenario()
		{
            if (instance == null)
            {
				//initialize all collections
                humanList = new List<Human>();
                staticObjectList = new List<SimObject>();
                movingObjectList = new List<SimObject>();
                pobicosObjectList = new List<SimObject>();
				furnitureList = new List<SimObject>();
            }
		}

		#region Getting Objects Methods
		/// <summary>
		/// Searches <o>Human</o> in <c>HumanList</c> that has <c>isActive</c> flag set to true.
		/// </summary>
		/// <returns><o>Human</o> that has focus in simulator</returns>
		public Human GetActiveHuman()
		{
			foreach (Human h in humanList)
				if (h.isActive)
					return h;
			
			return null;
		}

		/// <summary>
		/// Searches 3D object in all collections identifying it by name
		/// </summary>
		/// <param name="name">name of searched object</param>
		/// <returns><o>SimObject</o> that keeps desired model or null, when it does not exist</returns>
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

		/// <summary>
		/// Searches 3D POBICOS object in <c>pobicosObjectList</c> identifying it by name
		/// </summary>
		/// <param name="name">name of searched object</param>
		/// <param name="room">room where object will be searched</param>
		/// <returns><o>Object</o> that keeps desired model or null, when it does not exist</returns>
		public Object GetPobicosObjectByName(string name, Room room)
		{
			if (pobicosObjectList != null)
			{
				Object result;
				foreach (Object ob in pobicosObjectList)
				{
					result = ((IPobicosObjects)ob).GetByName(name, room);
					if (result != null)
						return result;
				}
			}
			return null;
		}
		#endregion

		#region Update Methods
		/// <summary>
		/// Update all players on the list
		/// </summary>
		/// <param name="gameTime">Time since last update</param>
		public void UpdateHumans(GameTime gameTime)
		{
			if (humanList != null)
				foreach (Human h in humanList)
					h.Update(gameTime);
		}

		/// <summary>
		/// Update all static objects and furniture
		/// </summary>
		/// <param name="gameTime">Time since last update</param>
		public void UpdateStaticObjects(GameTime gameTime)
		{
			if (staticObjectList != null)
				foreach (SimObject so in staticObjectList)
					so.Update(gameTime);

			if (furnitureList != null)
				foreach (SimObject so in furnitureList)
					so.Update(gameTime);
		}

		/// <summary>
		/// Update all POBICOS-enabled Objects
		/// </summary>
		/// <param name="gameTime">Time since last update</param>
		public void UpdatePobicosObjects(GameTime gameTime)
		{
			if (pobicosObjectList != null)
				foreach (Object ob in pobicosObjectList)
					((IPobicosObjects)ob).Update(gameTime);
		}

		/// <summary>
		/// Update all movingobjects that reuires apllying some transformation
		/// </summary>
		/// <param name="gameTime">Time since last update</param>
		public void UpdateMovingObjects(GameTime gameTime)
		{
			//true if smok should be removed, because it is not needed
			bool removeSmoke = false;
			//Collection of that holds location of each smoke model
			List<Vector3> smokePositions = new List<Vector3>();
			//randomizing fire animation
			Random rnd = new Random();
			//if SimObject needs to be removed
			SimObject toRemove = null;
			//scaling fire size
			float scaleFactor = 0.06f;

			if (movingObjectList != null)
				//iterate all moving objects
				foreach (SimObject so in movingObjectList)
				{
					so.Update(gameTime);
					#region Update Smoke
					if (so.name.Equals("smoke"))
					{
						//remove smoke if it is far over than flat (and smoke sensor). then it is not needed
						if ((so.Transformation.Translate.Y - 0.9f) > ((SmokeSensor)GetPobicosObjectByName("SmokeSensor", Room.Living)).Transformation.Translate.Y)
							removeSmoke = true;
						else
						{
							//perform smoke animation
							if ((so.Transformation.Translate.Y + 0.3f) > ((SmokeSensor)GetPobicosObjectByName("SmokeSensor", Room.Living)).Transformation.Translate.Y)
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
						//periodically generate smoke from flames
						if (gameTime.TotalGameTime.Milliseconds % 4000 == 0)
							smokePositions.Add(so.Transformation.Translate);

						//animate fire
						if (gameTime.TotalGameTime.Milliseconds % 20 == 0)
						{
							so.model.Rotate += new Vector3(0, ((float)rnd.NextDouble() - 0.5f) * 9f, 0);
							so.model.Scale += new Vector3(((float)rnd.NextDouble() % scaleFactor) - scaleFactor/2,
								0,
								((float)rnd.NextDouble() % scaleFactor) - scaleFactor/2);
							so.model.Scale -= new Vector3(0.001f);
							
							//if flames are small it may be removed from simulation
							if (so.model.Scale.Y < 0.1f)
								toRemove = so;
						}
					}
					#endregion
				}

			//remove marked object (fire) from list
			movingObjectList.Remove(toRemove);

			//remove marked object (smoke) from list
			if (removeSmoke)
			{
				movingObjectList.Remove(GetObjectByName("smoke"));
				eventSent = false;
			}
			
			//put smoke in reuired positions over the flames
			foreach(Vector3 pos in smokePositions)
				ScenarioBuilder.PutSmoke(new Vector3(
					pos.X + (float)rnd.NextDouble() * 0.7f - 0.5f,
					0.45f,
					pos.Z + (float)rnd.NextDouble() * 0.7f - 0.5f),
					(float)rnd.NextDouble());
		}

		/// <summary>
		/// Chenges light position, intensity during each screen update
		/// </summary>
		/// <param name="gameTime">Time since last update</param>
		internal void UpdateLights(GameTime gameTime)
		{
			//update buliding lights according to camera position
			foreach (SimObject so in staticObjectList)
				so.model.basicEffectManager.Light0Direction = cameraManager.ActiveCamera.HeadingVector;

			//update skybox lights according to camera position
			GetObjectByName("skybox").model.basicEffectManager.Light0Direction =
				cameraManager.ActiveCamera.HeadingVector + new Vector3(0, 1.0f, 0);
			GetObjectByName("skybox").model.basicEffectManager.Light1Direction =
				cameraManager.ActiveCamera.HeadingVector + new Vector3(-0.8f, 0.8f, 0);
			GetObjectByName("skybox").model.basicEffectManager.Light2Direction =
				cameraManager.ActiveCamera.HeadingVector + new Vector3(0.8f, 0.8f, 0);

			//periodically change light intensity (simulates sunset and sunrise)
			if (gameTime.TotalGameTime.Milliseconds % 2000 == 0)
			{
				//changing light intesity by this factor
				float difference = 1.05f;
				
				//check current light intensity (that is sensed by DawnDetector)
				float currLightValue = ((DawnDetector)GetPobicosObjectByName("DawnDetector", Room.Outside)).model.basicEffectManager.Light0Direction.Length();

				//check if light intensity is between specified limits and decide if it should be increased or decreased now
				if (currLightValue > maxLight0Length)
					increaseLightValue = false;
				else if (currLightValue < minLight0Length)
				    increaseLightValue = true;

				//change light intensity in simulator environment
				SwitchLight(Room.All, difference, increaseLightValue);
			}
		}

		#endregion

		#region Drawing Methods
		/// <summary>
		/// Draw players
		/// </summary>
		/// <param name="gameTime">Time since last update</param>
		public void DrawHumans(GameTime gameTime)
		{
			if (humanList != null)
				foreach (Human h in humanList)
					h.Draw(gameTime);
		}

		/// <summary>
		/// Draw static objects and furniture
		/// </summary>
		/// <param name="gameTime">Time since last update</param>
		public void DrawStaticObjects(GameTime gameTime)
		{
			if (staticObjectList != null)
				foreach (SimObject so in staticObjectList)
					so.Draw(gameTime);

			if (furnitureList != null)
				foreach (SimObject so in furnitureList)
					so.Draw(gameTime);
		}

		/// <summary>
		/// Draw POBICOS-enabled 3D objects
		/// </summary>
		/// <param name="gameTime">Time since last update</param>
		public void DrawPobicosObjects(GameTime gameTime)
		{
			if (pobicosObjectList != null)
				foreach (Object ob in pobicosObjectList)
					((IPobicosObjects)ob).Draw(gameTime);
		}

		/// <summary>
		/// Draw moving 3D models
		/// </summary>
		/// <param name="gameTime">Time since last update</param>
		public void DrawMovingObjects(GameTime gameTime)
		{
			if (movingObjectList != null)
				foreach (SimObject so in movingObjectList)
					so.Draw(gameTime);
		}
		#endregion

		#region Handling POBICOS connected actions
		/// <summary>
		/// Calls method that performs interaction with player and 3D object nearby
		/// </summary>
		/// <param name="human">human that origins interaction</param>
		/// <param name="distance">how far from object player may be to interact with it</param>
		internal void InteractWithObject(SimModel human, float distance)
		{
			//interact with pobicos object
			if (pobicosObjectList != null)
				foreach (Object ob in pobicosObjectList)
					//check intersection with POBICOS objects and player
					if (SimScreen.CheckIntersection(human, ((IPobicosObjects)ob).Position(), distance))
						//if player is near - interact ith object
						((IPobicosObjects)ob).Interact();

			//put fire in a car if nearby
			SimObject car = GetObjectByName("car_");
			if (car != null)
				if (SimScreen.CheckIntersection(human, car.model, distance * 2))
					ScenarioBuilder.PutFire(car.Transformation.Translate + new Vector3(0, 0.3f, 0.55f));

			//put fire in a kitchen if nearby
			SimObject kitchen = GetObjectByName("kitchen");
			if (kitchen != null)
			{
				//find oven
				Vector3 ovenRelativePosition = new Vector3(0.4f, 0.07f, -1.2f);
				kitchen.model.Transformation.Translate += ovenRelativePosition;

				//and put fire in the oven
				if (SimScreen.CheckIntersection(human, kitchen.model, distance))
					ScenarioBuilder.PutFire(kitchen.Transformation.Translate + new Vector3(0, 0.2f, 0));

				kitchen.model.Transformation.Translate -= ovenRelativePosition;
			}

			//put fire in a living room (on couch) if nearby
			SimObject couch = GetObjectByName("Couch");
			if (couch != null)
				if (SimScreen.CheckIntersection(human, couch.model, distance * 1.4f))
					ScenarioBuilder.PutFire(couch.Transformation.Translate + new Vector3(0));
		}

		/// <summary>
		/// Changes light intensity in choosen room
		/// </summary>
		/// <remarks>Method is able to decide if light should be increased or decreased basing on <paramref name="increase"/> flag.</remarks>
		/// <param name="room">in which room light intensity should be changed</param>
		/// <param name="difference">ligh intensity change factor</param>
		/// <param name="increase">increases light intensity if <c>true</c>, decreases if <c>false</c></param>
		public static void SwitchLight(Room room, float difference, bool increase)
		{
			//check if light intensity should be increased or decreased
			if (!increase)
				difference = 1 / difference;

			//change light intensity
			SwitchLight(room, difference);
		}

		/// <summary>
		/// Changes light intensity in choosen room by specified factor
		/// </summary>
		/// <remarks>Method multiplies current light intensity by <paramref name="difference"/> factor</remarks>
		/// <param name="room">in which room light intensity should be changed</param>
		/// <param name="difference">ligh intensity change factor</param>
		public static void SwitchLight(Room room, float difference)
		{
			//update static objects
			if (staticObjectList != null)
				foreach (SimObject so in staticObjectList)
					if (so.model.room.Equals(room) || room.Equals(Room.All))
					{
						so.model.basicEffectManager.Light0Direction *= new Vector3(difference);
						so.model.basicEffectManager.Light1Direction *= new Vector3(difference);
						so.model.basicEffectManager.Light2Direction *= new Vector3(difference);
					}

			//update POBICOS objects
			if (pobicosObjectList != null)
			{
				foreach (Object ob in pobicosObjectList)
					((IPobicosObjects)ob).SwitchLight(difference, room);
			}
			
			//update moving objects
			if (movingObjectList != null)
				foreach (SimObject so in movingObjectList)
					if (so.model.room.Equals(room) || room.Equals(Room.All))
					{
						so.model.basicEffectManager.Light0Direction *= new Vector3(difference);
						so.model.basicEffectManager.Light1Direction *= new Vector3(difference);
						so.model.basicEffectManager.Light2Direction *= new Vector3(difference);
					}

			//update furniture
			if (furnitureList != null)
				foreach (SimObject so in furnitureList)
					if (so.model.room.Equals(room) || room.Equals(Room.All))
					{
						so.model.basicEffectManager.Light0Direction *= new Vector3(difference);
						so.model.basicEffectManager.Light1Direction *= new Vector3(difference);
						so.model.basicEffectManager.Light2Direction *= new Vector3(difference);
					}
		}
		#endregion
	}
}
