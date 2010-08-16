using POBICOS.SimBase;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using POBICOS.SimBase.Cameras;
using System;
using POBICOS.SimLogic.Scenarios;
using System.Collections.Generic;
using POBICOS.SimBase.Effects;
using System.Diagnostics;
using System.Collections;

namespace POBICOS.SimLogic
{
	/// <summary>
	/// All rooms in simulation scenario.
	/// </summary>
	public enum Room
	{ 
		/// <summary>Livingroom (with TV and sofa)</summary>
		Living,
		/// <summary>Dining room (with table)</summary>
		Dining,
		/// <summary>Kitchen</summary>
		Kitchen,
		/// <summary>Bedroom</summary>
		Bedroom,
		/// <summary>Garage</summary>
		Garage,
		/// <summary>Toilet</summary>
		Toilet,
		/// <summary>Anteroom near home entrance. Room joining garage, toilet, kitchen and living room</summary>
		Anteroom,
		/// <summary>Area that is outside the house</summary>
		Outside,
		/// <summary>All rooms in home and outside of it</summary>
		All
	}

	/// <summary>
	/// Managing 3D models
	/// </summary>
	/// <remarks>Responsible for drawing models, applying effects, transforming and performing all operations on models</remarks>
	public class SimModel : DrawableGameComponent
	{
		/// <summary>XNA model</summary>
		public Model model;

		/// <summary>Room where model stands</summary>
		public Room room;

		/// <summary>Object location</summary>
		private Transformation transformation;

		/// <summary>Model world matrix</summary>
		public Matrix worldMatrix = Matrix.Identity;
		
		/// <summary>Model's bounding box</summary>
		private BoundingBox modelBoundingBox;

		/// <summary>Holds vertices for custom shapes drawing</summary>
		public VertexBuffer vertexBuffer;

		/// <summary>Camera helper</summary>
		CameraManager cameraManager;

		/// <summary>Helps managing effect that is aplied to model. All visual properties will be changed using this variable</summary>
		public BasicEffectManager basicEffectManager;

		/// <summary>3D model filename</summary>
		string modelPath;

		/// <summary>Indicates status of <o>SimModel</o> initialization state</summary>
		bool isInitialized = false;

		#region Properites
		/// <summary>
		/// Gets Bounding box taking actual model location into account
		/// </summary>
		public BoundingBox BoundingBox
		{
			get
			{
				//transform bounding box to object location
				BoundingBox so1BB;
				so1BB.Min = Vector3.Transform(this.modelBoundingBox.Min, this.Transformation.Matrix);
				so1BB.Max = Vector3.Transform(this.modelBoundingBox.Max, this.Transformation.Matrix);

				return so1BB;
			}
		}

		/// <summary>
		/// Gets or sets current object Transformation
		/// </summary>
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

		/// <summary>
		/// Gets or sets object rotation
		/// </summary>
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

		/// <summary>
		/// Gets or sets mode scale
		/// </summary>
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

		/// <summary>
		/// Gets or sets model Translate (location)
		/// </summary>
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

		/// <summary>
		/// <o>SimModel</o> Constructor
		/// </summary>
		/// <remarks>Loads 3D model and its properties from specified file and applies custom effect to it.</remarks>
		/// <param name="game">The Game that the game component should be attached to.</param>
		/// <param name="_modelPath">3D model path and filename</param>
		/// <param name="room">Room where model is located</param>
		public SimModel(Game game, string _modelPath, Room room)
			: base(game)
		{
			try
			{
				this.modelPath = _modelPath;
				this.room = room;
				
				//load model file
				model = Game.Content.Load<Model>(SimAssetsPath.MODELS_PATH + modelPath);

				//initialize effect manager
				basicEffectManager = new BasicEffectManager(game);

				//extract Bounding Box from Custom model importer
				Dictionary<string, object> modelTag = (Dictionary<string, object>)model.Tag;
				modelBoundingBox = (BoundingBox)modelTag["ModelBoudingBox"];
                
				//attach log information
				Trace.TraceInformation("SimModel Constructed (model '" + modelPath + "')");
			}
			catch (Exception e)
			{
                Trace.TraceError("Exception in SimModel Constructor (model '" + modelPath + "'): " + e.Message);
			}

		}

		#region DrawBounding Box Methods
		/// <summary>
		/// Draws bounding box around model.
		/// </summary>
		/// <remarks>should be used responsibly, because it may affect game performance due to its simple and resource demanding nature.
		/// It was created for testing purposes and currently is not used.</remarks>
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

		/// <summary>
		/// Draws line between two 3D points
		/// </summary>
		/// <param name="start">Starting 3D point</param>
		/// <param name="end">Ending 3D point</param>
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
		#endregion
		
		/// <summary>
		/// Initialization code
		/// </summary>
		/// <exception cref="InvalidOperationException">If <o>cameraManager</o> was not created until now.</exception>
		public override void Initialize()
		{
			//get game service: cameraManager
			cameraManager = Game.Services.GetService(typeof(CameraManager)) as CameraManager;

			if (cameraManager == null || basicEffectManager == null)
				throw new InvalidOperationException();

			isInitialized = true;

			base.Initialize();
		}
		
		///// <summary>
		///// Update model
		///// </summary>
		///// <param name="gameTime">Time since last update</param>
		//public override void Update(GameTime gameTime)
		//{
		//    //base.Update(gameTime);
		//}

		/// <summary>
		/// Draw model
		/// </summary>
		/// <param name="gameTime">Time since last update</param>
		public override void Draw(GameTime gameTime)
		{
			if (!isInitialized)
				Initialize();

			//draw using XNA's built BasicEffect
			BasicEffectUsage();

			//uncomment to draw bounding box around model
			//DrawBoundingBox();

			//base.Draw(gameTime);
		}

		/// <summary>
		/// Executes drawing model
		/// </summary>
		private void BasicEffectUsage()
		{
			try
			{
				//affect all model meshes
				foreach (ModelMesh m in model.Meshes)
				{
					//apply effect
					foreach (BasicEffect ef in m.Effects)
					{
						//apply transformations
						Matrix worldMatrix = Transformation.Matrix;

						if (transformation != null)
							ef.World = worldMatrix;
						ef.Projection = cameraManager.ActiveCamera.Projection;
						ef.View = cameraManager.ActiveCamera.View;
						ef.EnableDefaultLighting();

						//set lights
						ef.AmbientLightColor = basicEffectManager.AmbientColor;
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

						ef.SpecularColor = basicEffectManager.specularColor;
						ef.SpecularPower = basicEffectManager.SpecularPower;

						//Draw textures on specified meshes
						if (basicEffectManager.texturesEnabled && m.Name == basicEffectManager.texturedMeshName)
						{
							//turn on texturing and apply image
							ef.TextureEnabled = basicEffectManager.texturesEnabled;
							ef.Texture = basicEffectManager.textures[basicEffectManager.currentTexture];

							//light them up to make them visible
							ef.DirectionalLight0.Direction *= 2f;
							ef.DirectionalLight1.Direction *= 2f;
							ef.DirectionalLight2.Direction *= 2f;
							ef.PreferPerPixelLighting = false;
							ef.AmbientLightColor = basicEffectManager.AmbientColor;
						}

						//Draw textures on specified meshes
						if (basicEffectManager.texturesEnabled && m.Name == basicEffectManager.texturedMeshName2)
						{
							//turn on texturing and apply image
							ef.TextureEnabled = basicEffectManager.texturesEnabled;
							ef.Texture = basicEffectManager.textures[basicEffectManager.currentTexture2];

							//light them up to make them visible
							ef.DirectionalLight0.Direction *= 2f;
							ef.DirectionalLight1.Direction *= 2f;
							ef.DirectionalLight2.Direction *= 2f;
							ef.PreferPerPixelLighting = false;
							ef.AmbientLightColor = basicEffectManager.AmbientColor;
						}

						//write text on meshes specified in objectsTextured array
						if (basicEffectManager.texturesEnabled && basicEffectManager.writeOnObject)
						{
							//for each object on which texture is to be drawn
							for (int i = 0; i < basicEffectManager.objectsTextured.Length; i++ )
							{
								//ensure that you draw right texture on right mesh
								if (m.Name == basicEffectManager.objectsTextured[i])
								{
									//test if text has appropriate length to be drawn here
									if (i < basicEffectManager.textToWrite.Length)
									{
										//get texture containing correct letter
										if (basicEffectManager.letter.ContainsKey(basicEffectManager.textToWrite[i].ToString()))
										{
											//apply texture with correct letter on the mesh
											ef.Texture = (Texture2D)basicEffectManager.letter[basicEffectManager.textToWrite[i].ToString()];

											//light the mesh up to make them visible
											ef.DirectionalLight0.Direction *= 2f;
											ef.DirectionalLight1.Direction *= 2f;
											ef.DirectionalLight2.Direction *= 2f;
											ef.PreferPerPixelLighting = false;
											ef.AmbientLightColor = basicEffectManager.AmbientColor;
										}
										// else hide unused meshes (which index is higher than text length)
										else
											ef.World = Matrix.CreateTranslation(0, 0.2f, 0);
									}
									// else hide unused meshes (when no text needs to be drawn)
									else
										ef.World = Matrix.CreateTranslation(0, 0.2f, 0);
								}
							}
							
						}
						//hide unused meshes (when no text needs to be drawn or index is higher than text length)
						else if (!basicEffectManager.writeOnObject)
							for (int i = 0; i < basicEffectManager.objectsTextured.Length; i++)
								if (m.Name == basicEffectManager.objectsTextured[i])
									ef.World = Matrix.CreateTranslation(0, 0.2f, 0);
					}
					//draw this mesh
					m.Draw();
				}
			}
			catch (Exception e)
			{
                Trace.TraceError("Exception in SimModel Drawing: BasicEffectUsage() (model '" + modelPath + "'): " + e.Message);
			}
		}
	}	
}
