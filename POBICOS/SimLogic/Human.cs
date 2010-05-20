using Microsoft.Xna.Framework;
using POBICOS.SimBase.Cameras;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace POBICOS.SimLogic
{
	public class Human : SimObject
	{
		//this human has focus
		public bool isActive = false;
		public float movementSpeed = 0.05f;
		public Vector3 direction;

		public Human(Game game, string filename, Room room)
			: base(game, filename, room)
		{ 
		}

		protected override void LoadContent()
		{
			base.LoadContent();
		}

		public override void Update(GameTime time)
		{
			base.Update(time);
			//Console.WriteLine(this.Transformation.Matrix.Translation.ToString());
		}

		public override void Draw(GameTime time)
		{
			base.Draw(time);
		}
	}
}
