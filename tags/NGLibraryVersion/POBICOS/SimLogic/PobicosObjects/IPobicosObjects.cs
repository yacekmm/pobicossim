using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace POBICOS.SimLogic.PobicosObjects
{
	/// <summary>
	/// Definitions of methods specific for POBICOS-enabled objects
	/// </summary>
	interface IPobicosObjects
	{
		/// <summary>
		/// Implement actions that will be performed by object when user wants to interact with it
		/// </summary>
		void Interact();

		/// <summary>
		/// Gets model position
		/// </summary>
		/// <returns>3D model position</returns>
		Vector3 Position();

		/// <summary>
		/// Change 3D model lighting intensity.
		/// </summary>
		/// <param name="difference">light cange factor</param>
		/// <param name="room">room where light is being changed</param>
		void SwitchLight(float difference, Room room);

		/// <summary>
		/// Allows to perform object specific drawing actions
		/// </summary>
		/// <param name="gameTime">time since lat update</param>
		void Draw(GameTime gameTime);

		/// <summary>
		/// Allows to perform object specific update actions
		/// </summary>
		/// <param name="gameTime">time since lat update</param>
		void Update(GameTime gameTime);

		/// <summary>
		/// Helps objects searching
		/// </summary>
		/// <param name="name">searched object's name</param>
		/// <param name="room">room where object is being searched</param>
		/// <returns>searched POBICOS object or null when not found</returns>
		Object GetByName(string name, Room room);
	}
}
