using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace POBICOS.SimLogic.PobicosObjects
{
	interface IPobicosObjects
	{
		void Interact();
		Vector3 Position();
		void SwitchLight(float difference, Room room);
		void Draw(GameTime gameTime);
		void Update(GameTime gameTime);
		Object GetByName(string name, Room room);
	}
}
