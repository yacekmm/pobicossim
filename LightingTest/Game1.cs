using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace LightingTest
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : Microsoft.Xna.Framework.Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;

		Model model;
		Matrix world = Matrix.Identity;
		Matrix[] bones;
		Matrix projection;

		BasicEffect effect;
		Effect multiEffect;

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}

		protected override void Initialize()
		{
			this.Content.RootDirectory = "Content";
			effect = new BasicEffect(graphics.GraphicsDevice, null);
			multiEffect = this.Content.Load<Effect>("ShaderMultiPoint");
			base.Initialize();
		}

		private void SetBasicEffect()
		{
			effect.World = world;
			effect.EnableDefaultLighting();
		}

		private void SetMultiEffect()
		{ 
			//multiEffect.Parameters[""]
		}

		protected override void LoadContent()
		{
			model = this.Content.Load<Model>("wall_windows_3_4_joined");
			world *= Matrix.CreateScale(0.4f);
			//world *= Matrix.CreateRotationY(MathHelper.ToRadians(180));

			projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), 
				graphics.GraphicsDevice.DisplayMode.Width / graphics.GraphicsDevice.DisplayMode.Height, 0.1f, 1000);

			bones = new Matrix[model.Bones.Count];
			model.CopyAbsoluteBoneTransformsTo(bones);

			SetBasicEffect();
			foreach (ModelMesh mesh in model.Meshes)
				foreach (ModelMeshPart meshPart in mesh.MeshParts)
					meshPart.Effect = effect.Clone(graphics.GraphicsDevice);
		}

		protected override void UnloadContent()
		{
		}

		protected override void Update(GameTime gameTime)
		{
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
				this.Exit();
			
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			SetBasicEffect();
			GraphicsDevice.Clear(Color.CornflowerBlue);
			int counter = 0;
			foreach (ModelMesh mesh in model.Meshes)
			{
				//counter++;
				//foreach (BasicEffect ef in mesh.Effects)
				//{
				//    ef.World = world;// *bones[counter];
				//    //ef.Projection = projection;
				//    ef.EnableDefaultLighting();
				//}
				mesh.Draw();
			}

			base.Draw(gameTime);
		}
	}
}
