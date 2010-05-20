using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using POBICOS.SimBase.Cameras;
using POBICOS.SimBase;
using POBICOS.SimLogic.Scenarios;

namespace POBICOS.SimLogic
{
	public class SimObject : DrawableGameComponent
	{
		public string name = "";
		
		public SimModel model;

		//protected CameraManager cameraManager;

		public virtual Transformation Transformation
		{
			get
			{
				return model.Transformation;
			}
			set
			{
				model.Transformation = value;
			}
		}

		public SimObject(Game game, string filename, Room room)
			: base(game)
		{
			model = new SimModel(Game, filename, room);
			name = filename;
		}

		protected override void LoadContent()
		{
			base.LoadContent();
		}

		public override void Initialize()
		{
			base.Initialize();
		}

		public override void Update(GameTime time)
		{
			model.Update(time);
			base.Update(time);
		}

		public override void Draw(GameTime gameTime)
		{
			model.Draw(gameTime);

			base.Draw(gameTime);
		}
	}
}
