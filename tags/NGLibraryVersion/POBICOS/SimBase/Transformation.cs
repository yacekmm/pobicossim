using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

namespace POBICOS.SimBase
{
	/// <summary>
	/// Helper class unifying transformation operations
	/// </summary>
    public class Transformation
    {
        // Translation, rotate and scale in the x, y, z axis
		
		/// <summary>location update</summary>
        Vector3 translate;

		/// <summary>rotation update</summary>
        Vector3 rotate;

		/// <summary>scale update</summary>
        Vector3 scale;

        // Matrix handling
		/// <summary>Flag indicating if objects needs transformation update</summary>
        bool needUpdate;

		/// <summary>Object's matrix</summary>
        Matrix matrix;

        #region Properties
		/// <summary>
		/// Gets or sets object's translation
		/// </summary>
		public Vector3 Translate
		{
			get
			{
				return translate;
			}
			set
			{
				translate = value;
				needUpdate = true;
			}
		}

		/// <summary>
		/// Gets or sets object's rotation
		/// </summary>
		public Vector3 Rotate
		{
			get
			{
				return rotate;
			}
			set
			{
				rotate = value;
				needUpdate = true;
			}
		}

		/// <summary>
		/// Gets or sets object's scale
		/// </summary>
		public Vector3 Scale
		{
			get
			{
				return scale;
			}
			set
			{
				scale = value;
				needUpdate = true;
			}
		}

		/// <summary>
		/// Gets or sets object's world matrix
		/// </summary>
        public Matrix Matrix
		{
            get
            {
                if (needUpdate)
                {
                    matrix =
                        Matrix.CreateScale(scale) *
						Matrix.CreateRotationY(MathHelper.ToRadians(rotate.Y)) *
						Matrix.CreateRotationX(MathHelper.ToRadians(rotate.X)) *
						Matrix.CreateRotationZ(MathHelper.ToRadians(rotate.Z)) *
                        Matrix.CreateTranslation(translate);

                    needUpdate = false;
                }
                return matrix;
            }
        }
        #endregion

		/// <summary>
		/// <o>Transformation</o> constructor
		/// </summary>
        public Transformation()
            : this(Vector3.Zero, Vector3.Zero, Vector3.One)
        {
			//default world matrix
			matrix = Matrix.Identity;

            needUpdate = false;
        }

		/// <summary>
		/// Sets object transformation
		/// </summary>
		/// <param name="translate">desired position</param>
		/// <param name="rotate">desired rotation</param>
		/// <param name="scale">desired scale</param>
        public Transformation(Vector3 translate, Vector3 rotate, Vector3 scale)
        {
            this.translate = translate;
            this.rotate = rotate;
            this.scale = scale;

            needUpdate = true;
			matrix = Matrix.Identity;
        }
    }
}