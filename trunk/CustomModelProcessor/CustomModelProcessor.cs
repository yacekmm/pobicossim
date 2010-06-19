using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace CustomModelProcessor
{
	/// <summary>
	/// This class will be instantiated by the XNA Framework Content Pipeline
	/// to apply custom processing to content data, converting an object of
	/// type TInput to TOutput. The input and output types may be the same if
	/// the processor wishes to alter data without changing its type.
	///
	/// This is a part of a Content Pipeline Extension Library project.
	/// </summary>
	[ContentProcessor(DisplayName = "CustomModelProcessor.ContentProcessor1")]
	public class CustomModelProcessor : ModelProcessor
	{
		/// <summary>
		/// Overrides default model processor
		/// </summary>
		/// <remarks>Its aim is to read model bounding boxes</remarks>
		/// <param name="input">input nodes</param>
		/// <param name="context">Content processor context</param>
		/// <returns>loaded model content</returns>
		public override ModelContent Process(NodeContent input, ContentProcessorContext context)
		{
			ModelContent model = base.Process(input, context);

			try
			{
				// Extract all points from the mesh
				List<Vector3> vertexList = new List<Vector3>();
				GetModelVertices(input, vertexList);

				// Generate bounding volumes
				BoundingBox modelBoundBox = BoundingBox.CreateFromPoints(vertexList);
				BoundingSphere modelBoundSphere = BoundingSphere.CreateFromPoints(vertexList);

				Dictionary<string, object> tagDictionary = new Dictionary<string, object>();
				tagDictionary.Add("ModelBoudingBox", modelBoundBox);
				tagDictionary.Add("ModelBoudingSphere", modelBoundSphere);
				model.Tag = tagDictionary;
			}
			catch (Exception)
			{
			}

			return model;
		}

		/// <summary>
		/// analyzes all loaded 3D model vertices
		/// </summary>
		/// <param name="node">node containing vertices</param>
		/// <param name="vertexList">Collection of vertices</param>
		private void GetModelVertices(NodeContent node, List<Vector3> vertexList)
		{
			MeshContent meshContent = node as MeshContent;
			if (meshContent != null)
			{
				for (int i = 0; i < meshContent.Geometry.Count; i++)
				{
					GeometryContent geometryContent = meshContent.Geometry[i];
					for (int j = 0; j < geometryContent.Vertices.Positions.Count; j++)
						vertexList.Add(geometryContent.Vertices.Positions[j]);
				}
			}

			foreach (NodeContent child in node.Children)
				GetModelVertices(child, vertexList);
		}
	}
}