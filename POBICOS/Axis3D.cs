using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using POBICOS.SimBase.Cameras;

namespace POBICOS
{
	public class Axis3D
	{
		private GraphicsDevice device;
		private VertexBuffer vertexBuffer;
		private BasicEffect effect;
		public Matrix worldMatrix = Matrix.Identity;

		public CameraManager cameraManager;

		public Axis3D(GraphicsDevice graphicsDevice)
		{
			device = graphicsDevice;
		}

		/// <summary>
		/// Creates Cartesian 3D Axis
		/// </summary>
		/// <param name="XCenter">X coordinate of the center</param>
		/// <param name="YCenter">Y coordinate of the center</param>
		/// <param name="ZCenter">Z coordinate of the center</param>
		private void CreateAxis3D(float XCenter, float YCenter, float ZCenter)
		{ 
			//Size of 3D Axis
			float axisLength = 1f;
			//number of Vertices used
			int vertexCount = 22;

			VertexPositionColor[] vertices = new VertexPositionColor[vertexCount];
			//X axis
			vertices[0] = new VertexPositionColor(new Vector3(XCenter - axisLength, YCenter, ZCenter), Color.White);
			vertices[1] = new VertexPositionColor(new Vector3(XCenter + axisLength, YCenter, ZCenter), Color.White);
			//Y axis
			vertices[2] = new VertexPositionColor(new Vector3(XCenter, YCenter - axisLength, ZCenter), Color.White);
			vertices[3] = new VertexPositionColor(new Vector3(XCenter, YCenter + axisLength, ZCenter), Color.White);
			//Z axis
			vertices[4] = new VertexPositionColor(new Vector3(XCenter, YCenter, ZCenter - axisLength), Color.White);
			vertices[5] = new VertexPositionColor(new Vector3(XCenter, YCenter, ZCenter + axisLength), Color.White);

			//"X" letter
			vertices[6] = new VertexPositionColor(new Vector3(XCenter + axisLength - 0.1f, YCenter + 0.05f, ZCenter), Color.White);
			vertices[7] = new VertexPositionColor(new Vector3(XCenter + axisLength - 0.05f, YCenter + 0.2f, ZCenter), Color.White);
			vertices[8] = new VertexPositionColor(new Vector3(XCenter + axisLength - 0.05f, YCenter + 0.05f, ZCenter), Color.White);
			vertices[9] = new VertexPositionColor(new Vector3(XCenter + axisLength - 0.1f, YCenter + 0.2f, ZCenter), Color.White);

			// "Y" letter near Y axis
			vertices[10] = new VertexPositionColor(new Vector3(XCenter + 0.075f, YCenter + axisLength - 0.125f, ZCenter), Color.White);
			vertices[11] = new VertexPositionColor(new Vector3(XCenter + 0.075f, YCenter + axisLength - 0.2f, ZCenter), Color.White);
			vertices[12] = new VertexPositionColor(new Vector3(XCenter + 0.075f, YCenter + axisLength - 0.125f, ZCenter), Color.White);
			vertices[13] = new VertexPositionColor(new Vector3(XCenter + 0.1f, YCenter + axisLength - 0.05f, ZCenter), Color.White);
			vertices[14] = new VertexPositionColor(new Vector3(XCenter + 0.075f, YCenter + axisLength - 0.125f, ZCenter), Color.White);
			vertices[15] = new VertexPositionColor(new Vector3(XCenter + 0.05f, YCenter + axisLength - 0.05f, ZCenter), Color.White);
			
			// "Z" letter near Z axis
			vertices[16] = new VertexPositionColor(new Vector3(XCenter, YCenter + 0.05f, ZCenter + axisLength - 0.1f), Color.White);
			vertices[17] = new VertexPositionColor(new Vector3(XCenter, YCenter + 0.05f, ZCenter + axisLength - 0.05f), Color.White);
			vertices[18] = new VertexPositionColor(new Vector3(XCenter, YCenter + 0.05f, ZCenter + axisLength - 0.1f), Color.White);
			vertices[19] = new VertexPositionColor(new Vector3(XCenter, YCenter + 0.2f, ZCenter + axisLength - 0.05f), Color.White);
			vertices[20] = new VertexPositionColor(new Vector3(XCenter, YCenter + 0.2f, ZCenter + axisLength - 0.1f), Color.White);
			vertices[21] = new VertexPositionColor(new Vector3(XCenter, YCenter + 0.2f, ZCenter + axisLength - 0.05f), Color.White);
			
			//Fill the vertex buffer with the vertices
			vertexBuffer = new VertexBuffer(device, vertexCount * VertexPositionColor.SizeInBytes,
											BufferUsage.WriteOnly);
			vertexBuffer.SetData<VertexPositionColor>(vertices);
		}

		public void LoadContent(float XCenter, float YCenter, float ZCenter, CameraManager cameraManager)
		{
			effect = new BasicEffect(device, null);
			this.cameraManager = cameraManager;
			effect.View = cameraManager.ActiveCamera.View;
			effect.Projection = cameraManager.ActiveCamera.Projection;
			//float aspectRatio = (float)device.Viewport.Width / device.Viewport.Height;
			//effect.View = Matrix.CreateLookAt(new Vector3(0.0f, 0.0f, 3.0f), Vector3.Zero, Vector3.Up);
			//effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f),
			//																aspectRatio, 1.0f, 10.0f);
			effect.LightingEnabled = false;

			CreateAxis3D(XCenter, YCenter, ZCenter);
		}

		public void UnloadContent()
		{
			if (vertexBuffer != null)
			{
				vertexBuffer.Dispose();
				vertexBuffer = null;
			}
			if (effect != null)
			{
				effect.Dispose();
				effect = null;
			}
		}

		public void Draw()
		{
			effect.World = worldMatrix;
			effect.Projection = cameraManager.ActiveCamera.Projection;
			effect.View = cameraManager.ActiveCamera.View;
			device.VertexDeclaration = new VertexDeclaration(device, VertexPositionColor.VertexElements);
			device.Vertices[0].SetSource(vertexBuffer, 0, VertexPositionColor.SizeInBytes);

			effect.Begin();
				foreach (EffectPass CurrentPass in effect.CurrentTechnique.Passes)
				{
					CurrentPass.Begin();
						device.DrawPrimitives(PrimitiveType.LineList, 0, 11);
					CurrentPass.End();
				}
			effect.End();
		}

	}
}
