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
	/// <summary>
	/// Class managing all objects in simulator.
	/// </summary>
	/// <remarks>Created to make ability to augment functionality of animated models. By now each simulator model is controlled by this class</remarks>
	public class SimObject : DrawableGameComponent
	{
		/// <summary>
		/// 3D model filename. Often used as a model identifier.
		/// </summary>
		public string name = "";
		
		/// <summary>
		/// <o>SimModel</o> variable holding model.
		/// </summary>
		public SimModel model;

		/// <summary>
		/// Gets or sets model's transformation
		/// </summary>
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

		/// <summary>
		/// <o>SimObject</o> Constructor
		/// </summary>
		/// <param name="game"><o>Game</o> parameter</param>
		/// <param name="filename">3D model file name (and path). Supported file types: *.fbx, *.x.</param>
		/// <param name="room">Enum pointing the room where this model is located</param>
		public SimObject(Game game, string filename, Room room)
			: base(game)
		{
			model = new SimModel(Game, filename, room);
			name = filename;
		}

		/// <summary>
		/// Allows to load custom content
		/// </summary>
		protected override void LoadContent()
		{
			base.LoadContent();
		}

		/// <summary>
		/// Initialization code
		/// </summary>
		public override void Initialize()
		{
			base.Initialize();
		}

		/// <summary>
		/// Update model
		/// </summary>
		/// <param name="time">Time since last update</param>
		public override void Update(GameTime time)
		{
			model.Update(time);
			//base.Update(time);
		}

		/// <summary>
		/// Draw Model
		/// </summary>
		/// <param name="gameTime">Time since last update</param>
		public override void Draw(GameTime gameTime)
		{
			model.Draw(gameTime);

			//base.Draw(gameTime);
		}
	}
}
