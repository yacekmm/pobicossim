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
	public enum EffectMode
	{ 
		Basic,
		MultiPoint
	}
	public class Game1 : Microsoft.Xna.Framework.Game
	{
		EffectMode effectMode = EffectMode.Basic;
		//EffectMode effectMode = EffectMode.MultiPoint;
		GraphicsDeviceManager graphics;

		Model model, model2;
		Matrix world = Matrix.Identity;
		Matrix[] bones;
		Matrix projection, view;

		Vector3 lightPos = new Vector3(0);
		Vector3 cameraEye = new Vector3(0);

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
			switch (effectMode)
			{ 
				case EffectMode.Basic:
					effect = new BasicEffect(graphics.GraphicsDevice, null);
					break;
				case EffectMode.MultiPoint:
					//multiEffect = this.Content.Load<Effect>("ShaderMultiPointMine");
					multiEffect = this.Content.Load<Effect>("ShaderMultiPoint");
					break;
			}
			base.Initialize();
		}

		private void SetBasicEffect()
		{
			effect.World = world;
			effect.EnableDefaultLighting();
			effect.Projection = projection;
			effect.View = view;
		}

		private void SetMultiEffect()
		{
			multiEffect.Parameters["matWorld"].SetValue(world);
			multiEffect.Parameters["matWorldViewProj"].SetValue(world * view * projection);
			multiEffect.Parameters["vecEye"].SetValue(new Vector4(cameraEye, 0));

			multiEffect.Parameters["vecLightPos"].SetValue(lightPos);
			multiEffect.Parameters["LightRange"].SetValue(50);
			multiEffect.Parameters["LightColor"].SetValue(Color.White.ToVector4());
		}

		protected override void LoadContent()
		{
			//model = this.Content.Load<Model>("wall_windows_3_4_joined_x");
			model = this.Content.Load<Model>("wall_test_2");
			//model = this.Content.Load<Model>("Sphere6");
			world *= Matrix.CreateScale(0.4f);
			world *= Matrix.CreateTranslation(new Vector3(0, 0, -1));

			projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(90), 
				graphics.GraphicsDevice.DisplayMode.Width / graphics.GraphicsDevice.DisplayMode.Height, 0.1f, 10);

			view = Matrix.CreateLookAt(cameraEye, new Vector3(0, 0, -1), Vector3.Up);

			bones = new Matrix[model.Bones.Count];
			model.CopyAbsoluteBoneTransformsTo(bones);

			switch (effectMode)
			{ 
				case EffectMode.Basic:
					SetBasicEffect();
					//effect.DiffuseColor = model.Meshes[0].Effects[0].Parameters["DiffuseColor"].GetValueVector3();
					break;
				case EffectMode.MultiPoint:
					SetMultiEffect();
					multiEffect.Parameters["DiffuseColor"].SetValue(new Vector4(model.Meshes[0].Effects[0].Parameters["DiffuseColor"].GetValueVector3(), 1));
					break;
			}

			ApplyEffect();
		}

		private void ApplyEffect()
		{
			foreach (ModelMesh mesh in model.Meshes)
				foreach (ModelMeshPart meshPart in mesh.MeshParts)
					switch (effectMode)
					{
						case EffectMode.Basic:
							meshPart.Effect = effect.Clone(graphics.GraphicsDevice);
							break;
						case EffectMode.MultiPoint:
							meshPart.Effect = multiEffect.Clone(graphics.GraphicsDevice);
							break;
					}
		}

		protected override void UnloadContent()
		{
		}

		protected override void Update(GameTime gameTime)
		{
			UpdateKeyboard();
			base.Update(gameTime);
		}

		private void UpdateKeyboard()
		{
			float cameraSpeed = 0.02f;
			bool effectChanged = false;
			if (Keyboard.GetState().IsKeyDown(Keys.Escape))
				this.Exit();
			if (Keyboard.GetState().IsKeyDown(Keys.Left))
			{
				cameraEye += new Vector3(-cameraSpeed, 0, 0);
				effectChanged = true;
			}
			if (Keyboard.GetState().IsKeyDown(Keys.Right))
			{
				cameraEye += new Vector3(cameraSpeed, 0, 0);
				effectChanged = true;
			}
			if (Keyboard.GetState().IsKeyDown(Keys.Up))
			{
				cameraEye += new Vector3(0, cameraSpeed, 0);
				effectChanged = true;
			}
			if (Keyboard.GetState().IsKeyDown(Keys.Down))
			{
				cameraEye += new Vector3(0, -cameraSpeed, 0);
				effectChanged = true;
			}
			if (Keyboard.GetState().IsKeyDown(Keys.PageUp))
			{
				cameraEye += new Vector3(0, 0, -cameraSpeed);
				effectChanged = true;
			}
			if (Keyboard.GetState().IsKeyDown(Keys.PageDown))
			{
				cameraEye += new Vector3(0, 0, cameraSpeed);
				effectChanged = true;
			}

			if (effectChanged)
			{
				view = Matrix.CreateLookAt(cameraEye, new Vector3(0, 0, -1), Vector3.Up);
				ApplyEffect();
			}
		}

		protected override void Draw(GameTime gameTime)
		{
			switch(effectMode)
			{
				case EffectMode.Basic:
					SetBasicEffect();
					break;
				case EffectMode.MultiPoint:
					SetMultiEffect();
					break;
			}
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
