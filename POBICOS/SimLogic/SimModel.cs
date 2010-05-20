using POBICOS.SimBase;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using POBICOS.SimBase.Cameras;
using System;
using POBICOS.SimLogic.Scenarios;
using System.Collections.Generic;
using POBICOS.SimBase.Effects;

namespace POBICOS.SimLogic
{
	public enum Room
	{ 
		Living,
		Dining,
		Kitchen,
		Bedroom,
		Garage,
		Toilet,
		Anteroom,
		Outside
	}

	public class SimModel : DrawableGameComponent
	{
		public Model model;
		Transformation transformation;
		public Matrix worldMatrix = Matrix.Identity;
		public Room room;

		private BoundingBox modelBoundingBox;
		public BoundingSphere modelBoundingSphere;

		public VertexBuffer vertexBuffer;

		CameraManager cameraManager;
		public BasicEffectManager basicEffectManager;
		Effect effect;

		string modelPathTmp;

		bool isInitialized = false;

		#region Properites
		//public LightMaterial LightMaterial
		//{
		//    get
		//    {
		//        return lightMaterial;
		//    }
		//    set
		//    {
		//        lightMaterial = value;
		//    }
		//}
		public BoundingBox BoundingBox
		{
			get
			{
				BoundingBox so1BB;
				so1BB.Min = Vector3.Transform(this.modelBoundingBox.Min, this.Transformation.Matrix);
				so1BB.Max = Vector3.Transform(this.modelBoundingBox.Max, this.Transformation.Matrix);

				return so1BB;
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

		public Effect Effect
		{
			get
			{
				return effect;
			}
			set
			{
				effect = value;
			}
		}
		#endregion

		public SimModel(Game game, string modelPath, Room room)
			: base(game)
		{
			try
			{
				modelPathTmp = modelPath;
				this.room = room;
				model = Game.Content.Load<Model>(SimAssetsPath.MODELS_PATH + modelPath);

				basicEffectManager = new BasicEffectManager();

				Dictionary<string, object> modelTag = (Dictionary<string, object>)model.Tag;
				modelBoundingBox = (BoundingBox)modelTag["ModelBoudingBox"];

				effect = model.Meshes[0].Effects[0];
			}
			catch (Exception e)
			{
				Console.WriteLine("Exception in SimModel Constructor (model '" + modelPathTmp + "'): " + e.Message);
			}

		}

		#region DrawBounding Box Methods
		public void DrawBoundingBox()
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
			cameraManager = Game.Services.GetService(typeof(CameraManager)) as CameraManager;
			if (basicEffectManager == null)
				basicEffectManager = new BasicEffectManager();
			if (cameraManager == null || basicEffectManager == null)
				throw new InvalidOperationException();
			isInitialized = true;

			base.Initialize();
		}
		#endregion

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
		}

		public override void Draw(GameTime gameTime)
		{
			if (!isInitialized)
				Initialize();

			BasicEffectUsage();

			//DrawBoundingBox();

			base.Draw(gameTime);
		}


		private void BasicEffectUsage()
		{
			int counter = 0;
			foreach (ModelMesh m in model.Meshes)
			{
				counter++;
				foreach (BasicEffect ef in m.Effects)
				{
					Matrix worldMatrix = Transformation.Matrix;

					if (transformation != null)
						ef.World = worldMatrix;
					ef.Projection = cameraManager.ActiveCamera.Projection;
					ef.View = cameraManager.ActiveCamera.View;
					ef.EnableDefaultLighting();

					//ef.AmbientLightColor = basicEffectManager.AmbientColor;
					ef.DirectionalLight0.Enabled = basicEffectManager.Light0Enabled;
					ef.DirectionalLight1.Enabled = basicEffectManager.Light1Enabled;
					ef.DirectionalLight2.Enabled = basicEffectManager.Light2Enabled;

					ef.DirectionalLight0.SpecularColor = Color.White.ToVector3();
					ef.DirectionalLight1.SpecularColor = Color.White.ToVector3();
					ef.DirectionalLight2.SpecularColor = Color.White.ToVector3();

					ef.PreferPerPixelLighting = basicEffectManager.preferPerPixelLighting;

					ef.DirectionalLight0.Direction = basicEffectManager.Light0Direction;
					ef.DirectionalLight1.Direction = basicEffectManager.Light1Direction;
					ef.DirectionalLight2.Direction = basicEffectManager.Light2Direction;

					//ef.SpecularColor = ef.DiffuseColor;
					ef.SpecularColor = basicEffectManager.specularColor;
					ef.SpecularPower = basicEffectManager.SpecularPower;

					if (basicEffectManager.texturesEnabled && m.Name == basicEffectManager.texturedMeshName)
					{
						ef.TextureEnabled = basicEffectManager.texturesEnabled;
						ef.Texture = basicEffectManager.textures[basicEffectManager.currentTexture];

						ef.DirectionalLight0.Direction *= 2f;
						ef.DirectionalLight1.Direction *= 2f;
						ef.DirectionalLight2.Direction *= 2f;
						ef.PreferPerPixelLighting = false;

						ef.AmbientLightColor = basicEffectManager.AmbientColor;
					}

					if (basicEffectManager.texturesEnabled && m.Name == basicEffectManager.texturedMeshName2)
					{
						ef.TextureEnabled = basicEffectManager.texturesEnabled;
						ef.Texture = basicEffectManager.textures[basicEffectManager.currentTexture2];

						ef.DirectionalLight0.Direction *= 2f;
						ef.DirectionalLight1.Direction *= 2f;
						ef.DirectionalLight2.Direction *= 2f;
						ef.PreferPerPixelLighting = false;

						ef.AmbientLightColor = basicEffectManager.AmbientColor;
					}
				}
				m.Draw();
			}
		}
	}	
}
