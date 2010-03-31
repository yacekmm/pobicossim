using POBICOS.SimBase;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using POBICOS.SimBase.Cameras;
using System;
using POBICOS.SimLogic.Scenarios;
using System.Collections.Generic;
using POBICOS.SimBase.Materials;
using POBICOS.SimBase.Effects;
using POBICOS.SimBase.Lights;

//using Ox.Engine;

namespace POBICOS.SimLogic
{
	public class SimModel : DrawableGameComponent
	{
		public Model model;
		Transformation transformation;
		public Matrix worldMatrix = Matrix.Identity;

		public BoundingBox modelBoundingBox;
		public BoundingSphere modelBoundingSphere;

		public VertexBuffer vertexBuffer;

		CameraManager cameraManager;
		LightManager lightManager;
		LightMaterial lightMaterial;
		ModelEffect modelEffect;

		Effect effect;

		bool isInitialized = false;

		#region Properites
		public LightMaterial LightMaterial
		{
			get
			{
				return lightMaterial;
			}
			set
			{
				lightMaterial = value;
			}
		}
		public Transformation Transformation
		{
			get
			{
				return transformation;
			}
			set
			{
				transformation = value;
			}
		}

		public Vector3 Rotate
		{
			get 
			{ 
				return transformation.Rotate;
			}
			set
			{
				transformation.Rotate = value;
			}
		}

		public Vector3 Scale
		{
			get
			{
				return transformation.Scale;
			}
			set
			{
				transformation.Scale = value;
			}
		}

		public Vector3 Translate
		{
			get
			{
				return transformation.Translate;
			}
			set
			{
				transformation.Translate = value;
			}
		}
		#endregion

		public SimModel(Game game, string modelPath)
			:base(game)
		{
			try
			{
				effect = Game.Content.Load<Effect>(SimAssetsPath.EFFECTS_PATH + "effects");
				//effect = Game.Content.Load<Effect>(SimAssetsPath.EFFECTS_PATH + "Model");
				effect.CurrentTechnique = effect.Techniques["Colored"];
				
				model = Game.Content.Load<Model>(SimAssetsPath.MODELS_PATH + modelPath);

				foreach (ModelMesh m in model.Meshes)
				    foreach (ModelMeshPart part in m.MeshParts)
				        part.Effect = effect.Clone(game.GraphicsDevice);

				Dictionary<string, object> modelTag = (Dictionary<string, object>)model.Tag;
				modelBoundingBox = (BoundingBox)modelTag["ModelBoudingBox"];
				//lightMaterial = new LightMaterial();
				//modelEffect = new ModelEffect(model.Meshes[0].Effects[0]);
			}
			catch (Exception e)
			{
				Console.WriteLine("Exception in SimModel Constructor: " + e.Message);
			}
			
		}

		private void DrawBoundingBox()
		{
			//1
			DrawLine(modelBoundingBox.Min, 
						new Vector3(modelBoundingBox.Max.X, modelBoundingBox.Min.Y, modelBoundingBox.Min.Z));
			//2
			DrawLine(new Vector3(modelBoundingBox.Max.X, modelBoundingBox.Min.Y, modelBoundingBox.Min.Z), 
						new Vector3(modelBoundingBox.Max.X, modelBoundingBox.Min.Y, modelBoundingBox.Max.Z));
			//3
			DrawLine(new Vector3(modelBoundingBox.Max.X, modelBoundingBox.Min.Y, modelBoundingBox.Max.Z),
						new Vector3(modelBoundingBox.Min.X, modelBoundingBox.Min.Y, modelBoundingBox.Max.Z));
			//4
			DrawLine(new Vector3(modelBoundingBox.Min.X, modelBoundingBox.Min.Y, modelBoundingBox.Max.Z),
						modelBoundingBox.Min);
			//5
			DrawLine(modelBoundingBox.Min,
						new Vector3(modelBoundingBox.Min.X, modelBoundingBox.Max.Y, modelBoundingBox.Min.Z));
			//6
			DrawLine(new Vector3(modelBoundingBox.Min.X, modelBoundingBox.Max.Y, modelBoundingBox.Min.Z),
						new Vector3(modelBoundingBox.Max.X, modelBoundingBox.Max.Y, modelBoundingBox.Min.Z));
			//7
			DrawLine(new Vector3(modelBoundingBox.Max.X, modelBoundingBox.Max.Y, modelBoundingBox.Min.Z),
						new Vector3(modelBoundingBox.Max.X, modelBoundingBox.Min.Y, modelBoundingBox.Min.Z));
			//8
			DrawLine(new Vector3(modelBoundingBox.Max.X, modelBoundingBox.Max.Y, modelBoundingBox.Min.Z),
						modelBoundingBox.Max);
			//9
			DrawLine(modelBoundingBox.Max,
						new Vector3(modelBoundingBox.Max.X, modelBoundingBox.Min.Y, modelBoundingBox.Max.Z));
			//10
			DrawLine(modelBoundingBox.Max,
						new Vector3(modelBoundingBox.Min.X, modelBoundingBox.Max.Y, modelBoundingBox.Max.Z));
			//11
			DrawLine(new Vector3(modelBoundingBox.Min.X, modelBoundingBox.Max.Y, modelBoundingBox.Max.Z),
						new Vector3(modelBoundingBox.Min.X, modelBoundingBox.Min.Y, modelBoundingBox.Max.Z));
			//12
			DrawLine(new Vector3(modelBoundingBox.Min.X, modelBoundingBox.Max.Y, modelBoundingBox.Max.Z),
						new Vector3(modelBoundingBox.Min.X, modelBoundingBox.Max.Y, modelBoundingBox.Min.Z));
		}

		private void DrawLine(Vector3 start, Vector3 end)
		{
			int vertexCount = 2;

			VertexPositionColor[] vertices = new VertexPositionColor[vertexCount];

			vertices[0] = new VertexPositionColor(start, Color.Red);
			vertices[1] = new VertexPositionColor(end, Color.Red);

			//Fill the vertex buffer with the vertices
			vertexBuffer = new VertexBuffer(Game.GraphicsDevice, vertexCount * VertexPositionColor.SizeInBytes,
											BufferUsage.WriteOnly);
			vertexBuffer.SetData<VertexPositionColor>(vertices);

			Game.GraphicsDevice.VertexDeclaration = new VertexDeclaration(Game.GraphicsDevice, VertexPositionColor.VertexElements);
			Game.GraphicsDevice.Vertices[0].SetSource(vertexBuffer, 0, VertexPositionColor.SizeInBytes);

			Game.GraphicsDevice.DrawPrimitives(PrimitiveType.LineList, 0, 1);
		}

		public override void Initialize()
		{
			lightManager = Game.Services.GetService(typeof(LightManager)) as LightManager;
			cameraManager = Game.Services.GetService(typeof(CameraManager)) as CameraManager;

			if (lightManager == null || cameraManager == null)
				throw new InvalidOperationException();
			isInitialized = true;

			base.Initialize();
		}


		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
		}


		private void SetEffectMaterial()
		{
			// Get the first two lights from the light manager
			PointLight light0 = PointLight.NoLight;
			PointLight light1 = PointLight.NoLight;
			if (lightManager.Count > 0)
			{
				light0 = lightManager[0] as PointLight;
				if (lightManager.Count > 1)
					light1 = lightManager[1] as PointLight;
			}

			// Lights
			modelEffect.AmbientLightColor = lightManager.AmbientLightColor;
			modelEffect.Light1Position = light0.Position;
			modelEffect.Light1Color = light0.Color;
			modelEffect.Light2Position = light1.Position;
			modelEffect.Light2Color = light1.Color;

			// Configure material
			modelEffect.DiffuseColor = lightMaterial.DiffuseColor;
			modelEffect.SpecularColor = lightMaterial.SpecularColor;
			modelEffect.SpecularPower = lightMaterial.SpecularPower;

			// Camera and world transformations
			//if (transformation != null) 
			modelEffect.World = Transformation.Matrix;
			modelEffect.View = cameraManager.ActiveCamera.View;
			modelEffect.Projection = cameraManager.ActiveCamera.Projection;
		}

		public override void Draw(GameTime gameTime)
		{
			if (!isInitialized)
				Initialize();

			//GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;
			Vector3 lightDirection = new Vector3(0.2f, 0.2f, 0.2f);
			lightDirection.Normalize();

			effect.Begin();
			foreach (EffectPass pass in effect.CurrentTechnique.Passes)
			{
				pass.Begin();
				foreach (ModelMesh m in model.Meshes)
				{
					foreach (Effect currentEffect in m.Effects)
					{
						currentEffect.CurrentTechnique = currentEffect.Techniques["Textured"];

						currentEffect.Parameters["xView"].SetValue(cameraManager.ActiveCamera.View);
						currentEffect.Parameters["xProjection"].SetValue(cameraManager.ActiveCamera.Projection);
						currentEffect.Parameters["xWorld"].SetValue(Transformation.Matrix);

						currentEffect.Parameters["xEnableLighting"].SetValue(true);
						currentEffect.Parameters["xLightDirection"].SetValue(lightDirection);
						currentEffect.Parameters["xAmbient"].SetValue(0.2f);
					}
					m.Draw();
				}
				pass.End();
			}
			effect.End();
			

			//foreach (ModelMesh m in model.Meshes)
			//{
			//    foreach (BasicEffect ef in m.Effects)
			//    {
			//        if (transformation != null)
			//            ef.World = Transformation.Matrix;
			//        ef.Projection = cameraManager.ActiveCamera.Projection;
			//        ef.View = cameraManager.ActiveCamera.View;
			//        ef.LightingEnabled = true;
			//        //ef.AmbientLightColor = new Vector3(1.0f, 1.0f, 1.0f);
			//        ef.DirectionalLight0.Direction = Vector3.Zero;
			//        ef.DirectionalLight0.DiffuseColor = Vector3.One;
			//        ef.DirectionalLight0.SpecularColor = Vector3.One;
			//        ef.DirectionalLight0.Enabled = true;
			//        //ef.EnableDefaultLighting();
			//    }
			//    m.Draw();
			//}


			//DrawBoundingBox();

			base.Draw(gameTime);
		}
	}
}
