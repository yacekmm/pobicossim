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

namespace POBICOS.SimLogic
{
	public enum EffectList
	{
		Basic,
		ShaderSpecular,
		Shader,
		Textured,
		XNAModel
	}

	public class SimModel : DrawableGameComponent
	{
		public Model model;
		Transformation transformation;
		public Matrix worldMatrix = Matrix.Identity;
		
		//public Matrix[] bones;
		//SpriteBatch spriteBatch;

		public BoundingBox modelBoundingBox;
		public BoundingSphere modelBoundingSphere;

		public VertexBuffer vertexBuffer;

		CameraManager cameraManager;
		LightManager lightManager;
		LightMaterial lightMaterial;
		ModelEffect modelEffect;
		BasicEffectManager basicEffectManager;

		Effect effect;
		EffectList effectUsed = EffectList.Basic;
		Vector3 lightDirection = new Vector3(0.2f, 0.1f, 0.1f);

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

		public SimModel(Game game, string modelPath, EffectList effectToUse)
			:base(game)
		{
			try
			{
				model = Game.Content.Load<Model>(SimAssetsPath.MODELS_PATH + modelPath);
				effectUsed = effectToUse;
				effectUsed = EffectList.Basic;

				Dictionary<string, object> modelTag = (Dictionary<string, object>)model.Tag;
				modelBoundingBox = (BoundingBox)modelTag["ModelBoudingBox"];

				switch (effectUsed)
				{
					case EffectList.ShaderSpecular:
						ApplyEffectShaderSpec(game);
						break;
					case EffectList.Shader:
						ApplyEffectShader(game);
						break;
					case EffectList.Textured:
						ApplyEffectTextured(game);
						break;
					case EffectList.XNAModel:
						ApplyEffectXNAModel(game);
						break;
					case EffectList.Basic:
						break;
				}

				//lightMaterial = new LightMaterial();
				if(!effectUsed.Equals(EffectList.Basic))
					modelEffect = new ModelEffect(effect);
			}
			catch (Exception e)
			{
				Console.WriteLine("Exception in SimModel Constructor: " + e.Message);
			}
			
		}

		private void ApplyEffectShader(Game game)
		{
			effect = Game.Content.Load<Effect>(SimAssetsPath.EFFECTS_PATH + "Diffuse");

			foreach (ModelMesh m in model.Meshes)
				foreach (ModelMeshPart part in m.MeshParts)
				{
					Vector4 diff;
					try
					{
						diff = new Vector4(part.Effect.Parameters["DiffuseColor"].GetValueVector3(), 1.0f);
					}
					catch (Exception)
					{
						diff = part.Effect.Parameters["DiffuseColor"].GetValueVector4();
					}
					part.Effect = effect.Clone(game.GraphicsDevice);
					part.Effect.Parameters["DiffuseColor"].SetValue(diff);
				}
		}

		private void ApplyEffectXNAModel(Game game)
		{
			effect = Game.Content.Load<Effect>(SimAssetsPath.EFFECTS_PATH + "Model");

			foreach (ModelMesh m in model.Meshes)
				foreach (ModelMeshPart part in m.MeshParts)
				{
					Vector3 diff;
					try
					{
						diff = part.Effect.Parameters["DiffuseColor"].GetValueVector3();
					}
					catch (Exception)
					{
						diff = part.Effect.Parameters["diffuseColor"].GetValueVector3();
					}
					part.Effect = effect.Clone(game.GraphicsDevice);
					part.Effect.Parameters["diffuseColor"].SetValue(diff);
				}
		}

		private void ApplyEffectShaderSpec(Game game)
		{
			effect = Game.Content.Load<Effect>(SimAssetsPath.EFFECTS_PATH + "ShaderSpec");

			foreach (ModelMesh m in model.Meshes)
				foreach (ModelMeshPart part in m.MeshParts)
				{
					Vector4 diff;
					try
					{
						diff = new Vector4(part.Effect.Parameters["DiffuseColor"].GetValueVector3(), 1.0f);
					}
					catch (Exception)
					{
						diff = part.Effect.Parameters["vDiffuseColor"].GetValueVector4();
					}
					part.Effect = effect.Clone(game.GraphicsDevice);
					part.Effect.Parameters["vDiffuseColor"].SetValue(diff);
				}
		}

		private void ApplyEffectTextured(Game game)
		{
			effect = Game.Content.Load<Effect>(SimAssetsPath.EFFECTS_PATH + "effects");

			foreach (ModelMesh m in model.Meshes)
				foreach (ModelMeshPart part in m.MeshParts)
				{
					Vector3 diff = part.Effect.Parameters["DiffuseColor"].GetValueVector3();
					part.Effect = effect.Clone(game.GraphicsDevice);
					part.Effect.Parameters["xDiffuseColor"].SetValue(new Vector4(diff, 1.0f));
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
			basicEffectManager = Game.Services.GetService(typeof(BasicEffectManager)) as BasicEffectManager;

			if (lightManager == null || cameraManager == null || basicEffectManager == null)
				throw new InvalidOperationException();
			isInitialized = true;

			base.Initialize();
		}

		public override void Update(GameTime gameTime)
		{
			base.Update(gameTime);
		}

		//private void SetEffectMaterial()
		//{
		//    // Get the first two lights from the light manager
		//    PointLight light0 = PointLight.NoLight;
		//    PointLight light1 = PointLight.NoLight;
		//    if (lightManager.Count > 0)
		//    {
		//        light0 = lightManager[0] as PointLight;
		//        if (lightManager.Count > 1)
		//            light1 = lightManager[1] as PointLight;
		//    }

		//    // Lights
		//    modelEffect.AmbientLightColor = lightManager.AmbientLightColor;
		//    modelEffect.Light1Position = light0.Position;
		//    modelEffect.Light1Color = light0.Color;
		//    modelEffect.Light2Position = light1.Position;
		//    modelEffect.Light2Color = light1.Color;

		//    // Configure material
		//    modelEffect.DiffuseColor = lightMaterial.DiffuseColor;
		//    modelEffect.SpecularColor = lightMaterial.SpecularColor;
		//    modelEffect.SpecularPower = lightMaterial.SpecularPower;

		//    // Camera and world transformations
		//    //if (transformation != null) 
		//    modelEffect.World = Transformation.Matrix;
		//    modelEffect.View = cameraManager.ActiveCamera.View;
		//    modelEffect.Projection = cameraManager.ActiveCamera.Projection;
		//}

		public override void Draw(GameTime gameTime)
		{
			if (!isInitialized)
				Initialize();
			switch (effectUsed)
			{ 
				case EffectList.Basic:
					BasicEffectUsage();
					break;
				case EffectList.Shader:
					ShaderEffect();
					break;
				case EffectList.ShaderSpecular:
					ShaderEffectSpec();
					break;
				case EffectList.Textured:
					RiemersEffect();
					break;
				case EffectList.XNAModel:
					XNAModel();
					break;
			}
			//ShaderEffect();
			//DiffuseEffect();
			
			//DrawBoundingBox();

			base.Draw(gameTime);
		}

		private void XNAModel()
		{
			effect.CurrentTechnique = effect.Techniques["AnimatedModel"];

			effect.Begin();

			foreach (EffectPass pass in effect.CurrentTechnique.Passes)
			{
				pass.Begin();
				foreach (ModelMesh mesh in model.Meshes)
				{
					foreach (Effect ef in mesh.Effects)
					{
						ef.CurrentTechnique = effect.CurrentTechnique;

						ef.Parameters["matW"].SetValue(Transformation.Matrix);
						ef.Parameters["matV"].SetValue(cameraManager.ActiveCamera.View);
						ef.Parameters["matVI"].SetValue(Matrix.Invert(cameraManager.ActiveCamera.View));
						ef.Parameters["matWV"].SetValue(Transformation.Matrix * cameraManager.ActiveCamera.View);
						ef.Parameters["matWVP"].SetValue(Transformation.Matrix * cameraManager.ActiveCamera.View * cameraManager.ActiveCamera.Projection);

						ef.Parameters["eyePosition"].SetValue(cameraManager.ActiveCamera.Position);

						//ef.Parameters["specularColor"].SetValue(effect.Parameters["diffuseColor"].GetValueVector3());
						ef.Parameters["specularColor"].SetValue(modelEffect.SpecularColor);
						ef.Parameters["ambientLightColor"].SetValue(modelEffect.AmbientColor);
						ef.Parameters["ambientLightInt"].SetValue(modelEffect.AmbientIntensity);

						ef.Parameters["light1Color"].SetValue(modelEffect.Light1Color);
						ef.Parameters["light1Position"].SetValue(modelEffect.Light1Position);

						ef.Parameters["light2Color"].SetValue(modelEffect.Light2Color);
						ef.Parameters["light2Position"].SetValue(modelEffect.Light2Position);

						ef.Parameters["light1Dir"].SetValue(modelEffect.Light1Direction);

						ef.Parameters["specularPower"].SetValue(modelEffect.SpecularPower);
						//ef.Parameters[""].SetValue();





						////ef.Parameters["specularColor"].SetValue(effect.Parameters["diffuseColor"].GetValueVector3());
						//ef.Parameters["specularColor"].SetValue(new Vector3(0.5f));
						//ef.Parameters["ambientLightColor"].SetValue(new Vector3(1.0f));
						//ef.Parameters["ambientLightInt"].SetValue(0.1f);

						//ef.Parameters["light1Color"].SetValue(new Vector3(0.7f));
						//ef.Parameters["light1Position"].SetValue(new Vector3(light1x, light1y, light1z));

						//ef.Parameters["light2Color"].SetValue(new Vector3(0.8f));
						//ef.Parameters["light2Position"].SetValue(new Vector3(1.0f, 220.1f, -1.0f));

						//ef.Parameters["eyePosition"].SetValue(cameraManager.ActiveCamera.Position);
						//ef.Parameters["light1Dir"].SetValue(new Vector3(light1xDir, light1yDir, light1zDir));

						//ef.Parameters["specularPower"].SetValue(32.0f);
						////ef.Parameters[""].SetValue();
					}
					mesh.Draw();
				}
				pass.End();
			}
			effect.End();
		}

		private void ShaderEffectSpec()
		{
			effect.CurrentTechnique = effect.Techniques["SpecularLight"];

			effect.Begin();

			foreach (EffectPass pass in effect.CurrentTechnique.Passes)
			{
				pass.Begin();
				foreach (ModelMesh mesh in model.Meshes)
				{
					foreach (Effect ef in mesh.Effects)
					{
						ef.CurrentTechnique = effect.CurrentTechnique;

						worldMatrix = Transformation.Matrix;

						Vector4 vecEye = new Vector4(cameraManager.ActiveCamera.Position.X, cameraManager.ActiveCamera.Position.Y,
							cameraManager.ActiveCamera.Position.Z, 0);
						Vector4 vColorSpecular = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);
						Vector4 vColorAmbient = new Vector4(0.15f, 0.15f, 0.15f, 1.0f);

						Matrix worldInverse = Matrix.Invert(worldMatrix);
						Vector4 vLightDirection = new Vector4(1.0f, 1.0f, 1.0f, 1.0f);

						ef.Parameters["matWorldViewProj"].SetValue(worldMatrix * cameraManager.ActiveCamera.View * cameraManager.ActiveCamera.Projection);
						ef.Parameters["matWorld"].SetValue(worldMatrix);
						ef.Parameters["vecEye"].SetValue(vecEye);
						ef.Parameters["vecLightDir"].SetValue(vLightDirection);
						//ef.Parameters["vSpecularColor"].SetValue(vColorSpecular);
						ef.Parameters["vSpecularColor"].SetValue(ef.Parameters["vDiffuseColor"].GetValueVector4());
						ef.Parameters["vAmbient"].SetValue(vColorAmbient);
					}
					mesh.Draw();
				}
				pass.End();
			}
			effect.End();
		}

		private void ShaderEffect()
		{
			effect.CurrentTechnique = effect.Techniques["Diffuse"];

			effect.Begin();

			foreach (EffectPass pass in effect.CurrentTechnique.Passes)
			{
				pass.Begin();
				foreach (ModelMesh mesh in model.Meshes)
				{
					foreach (Effect ef in mesh.Effects)
					{
						ef.CurrentTechnique = effect.CurrentTechnique;

						worldMatrix = Transformation.Matrix;

						Vector4 vColorAmbient = new Vector4(1.15f, 1.15f, 1.15f, 1.0f);

						Matrix worldInverse = Matrix.Invert(worldMatrix);
						Vector3 vLightDirection = new Vector3(1.0f, 1.0f, 1.0f);

						ef.Parameters["View"].SetValue(cameraManager.ActiveCamera.View);
						ef.Parameters["World"].SetValue(worldMatrix);
						ef.Parameters["Projection"].SetValue(cameraManager.ActiveCamera.Projection);

						ef.Parameters["DiffuseLightDirection"].SetValue(vLightDirection);
						ef.Parameters["DiffuseIntensity"].SetValue(1);
						ef.Parameters["AmbientColor"].SetValue(vColorAmbient);
						ef.Parameters["AmbientIntensity"].SetValue(0.3f);
					}
					mesh.Draw();
				}
				pass.End();
			}
			effect.End();
		}

		private void DiffuseEffect()
		{
			// Set what technique we want to render with
			effect.CurrentTechnique = effect.Techniques["Diffuse"];
				
			// Set the parameters of the shader
			effect.Parameters["World"].SetValue(Transformation.Matrix);
			effect.Parameters["View"].SetValue(cameraManager.ActiveCamera.View);
			effect.Parameters["Projection"].SetValue(cameraManager.ActiveCamera.Projection);
			effect.Parameters["AmbientColor"].SetValue(new Vector4(1, 1, 1, 1));
			effect.Parameters["AmbientIntensity"].SetValue(.05f);
			effect.Parameters["DiffuseColor"].SetValue(new Vector4(0, 1, 0, 1));
			effect.Parameters["DiffuseIntensity"].SetValue(.8f);
			effect.Parameters["DiffuseLightDirection"].SetValue(new Vector3(.5f, 1f, .6f));
				
			// Begin the effect
			effect.Begin();
			foreach (EffectPass pass in effect.CurrentTechnique.Passes)
			{
				pass.Begin();
				foreach (ModelMesh m in model.Meshes)
					m.Draw();
				pass.End();
			}
			effect.End();
		}

		private void BasicEffectUsage()
		{
			foreach (ModelMesh m in model.Meshes)
			{
				foreach (BasicEffect ef in m.Effects)
				{
					if (transformation != null)
						ef.World = Transformation.Matrix;
					ef.Projection = cameraManager.ActiveCamera.Projection;
					ef.View = cameraManager.ActiveCamera.View;
					ef.EnableDefaultLighting();

					ef.AmbientLightColor = basicEffectManager.AmbientColor;
					ef.DirectionalLight0.Enabled = basicEffectManager.Light0Enabled;
					ef.DirectionalLight1.Enabled = basicEffectManager.Light1Enabled;
					ef.DirectionalLight2.Enabled = basicEffectManager.Light2Enabled;

					ef.DirectionalLight0.Direction = basicEffectManager.Light0Direction;
					ef.DirectionalLight0.SpecularColor = basicEffectManager.Light0SpecularColor;

					ef.DirectionalLight1.DiffuseColor = ef.DiffuseColor;
					ef.DirectionalLight1.SpecularColor = basicEffectManager.Light1SpecularColor;
					ef.DirectionalLight1.Direction= basicEffectManager.Light1Direction;

					ef.SpecularColor = ef.DiffuseColor;
					ef.SpecularPower = basicEffectManager.SpecularPower;
				}
				m.Draw();
			}
		}

		private void RiemersEffect()
		{
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
						currentEffect.Parameters["xAmbientIntensity"].SetValue(0.005f);
						currentEffect.Parameters["xAmbientColor"].SetValue(new Vector4(0.3f, 0.3f, 0.3f, 1.0f));
						currentEffect.Parameters["xDiffuseIntensity"].SetValue(0.48f);
					}
					m.Draw();
				}
				pass.End();
			}
			effect.End();
		}
	}
}
