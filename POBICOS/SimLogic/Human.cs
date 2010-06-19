using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using POBICOS.SimBase.Cameras;

namespace POBICOS.SimLogic
{
	/// <summary>
	/// Player class
	/// </summary>
	/// <remarks>Allows to create and steer player in game</remarks>
	public class Human : SimObject
	{
		/// <summary>this human has focus</summary>
		public bool isActive = false;

		/// <summary>Player movement speed</summary>
		public float movementSpeed = 0.05f;

		/// <summary>Player heading vector</summary>
		public Vector3 direction;

		/// <summary>
		/// <o>Human</o> constructor
		/// </summary>
		/// <param name="game">The Game that the game component should be attached to.</param>
		/// <param name="filename">3D model filename</param>
		/// <param name="room">Room where player is located</param>
		public Human(Game game, string filename, Room room)
			: base(game, filename, room)
		{ 
		}

		/// <summary>
		/// Loads custom content
		/// </summary>
		protected override void LoadContent()
		{
			base.LoadContent();
		}

		/// <summary>
		/// Update screen
		/// </summary>
		/// <param name="time">Time since last update</param>
		public override void Update(GameTime time)
		{
			base.Update(time);
		}

		/// <summary>
		/// Drawing method
		/// </summary>
		/// <param name="time">Time since last update</param>
		public override void Draw(GameTime time)
		{
			base.Draw(time);
		}
	}
}