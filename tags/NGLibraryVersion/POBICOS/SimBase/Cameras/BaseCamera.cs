using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

namespace POBICOS.SimBase.Cameras
{
	/// <summary>
	/// Base Camera Class. Keeps variables common for each camera
	/// </summary>
    public abstract class BaseCamera
    {
        // Perspective projection parameters
		/// <summary>Perspective projection parameters: fovY (field of view)</summary>
        float fovy;
		/// <summary>Perspective projection parameters: aspect ratio</summary>
        float aspectRatio;
		/// <summary>Perspective projection parameters: near plane</summary>
        float nearPlane;
		/// <summary>Perspective projection parameters: far plane</summary>
        float farPlane;

        // Position and target
		/// <summary>Camera parameters: position</summary>
        Vector3 position;
		/// <summary>Camera parameters: target</summary>
        Vector3 target;

        // orientation vectors
		/// <summary>Camera orientation vectors: heading vector</summary>
        Vector3 headingVec;
		/// <summary>Camera orientation vectors: strafe vector</summary>
        Vector3 strafeVec;
		/// <summary>Camera orientation vectors: up vector</summary>
        Vector3 upVec;

        // Matrices and updates
		/// <summary>Camera flag: update view is necessary</summary>
        protected bool needUpdateView;
		/// <summary>Camera flag: update projection is necessary</summary>
        protected bool needUpdateProjection;
		/// <summary>Camera flag: update frustum is necessary</summary>
        protected bool needUpdateFrustum;
		/// <summary>Camera Matrix: view Matrix</summary>
        protected Matrix viewMatrix;
		/// <summary>Camera Matrix: projection Matrix</summary>
        protected Matrix projectionMatrix;

        // Frustum
		/// <summary>Camera's Bounding frustum</summary>
        BoundingFrustum frustum;

        #region Properties
		/// <summary>Gets or sets fovY (field of view)</summary>
        public float FovY
        {
            get
            {
                return fovy;
            }
            set
            {
                fovy = value;
                needUpdateProjection = true;
            }
        }

		/// <summary>Gets or sets aspect ratio</summary>
        public float AspectRatio
        {
            get
            {
                return aspectRatio;
            }
            set
            {
                aspectRatio = value;
                needUpdateProjection = true;
            }
        }

		/// <summary>Gets or sets near plane</summary>
        public float NearPlane
        {
            get
            {
                return nearPlane;
            }
            set
            {
                nearPlane = value;
                needUpdateProjection = true;
            }
        }

		/// <summary>Gets or sets far plane</summary>
        public float FarPlane
        {
            get
            {
                return farPlane;
            }
            set
            {
                farPlane = value;
                needUpdateProjection = true;
            }
        }

		/// <summary>Gets or sets camera position</summary>
        public Vector3 Position
        {
            get
            {
                return position;
            }
            set
            {
                SetLookAt(value, target, upVec);
            }
        }

		/// <summary>Gets or sets camera target</summary>
        public Vector3 Target
        {
            get
            {
                return target;
            }
            set
            {
                SetLookAt(position, value, upVec);
            }
        }

		/// <summary>Gets or sets camera up vector</summary>
        public Vector3 UpVector
        {
            get
            {
                return upVec;
            }
            set
            {
                SetLookAt(position, target, value);
            }
        }

		/// <summary>Gets camera heading vector</summary>
        public Vector3 HeadingVector
        {
            get
            {
                return headingVec;
            }
        }

		/// <summary>Gets camera right vector</summary>
        public Vector3 RightVector
        {
            get
            {
                return strafeVec;
            }
        }

		/// <summary>Gets camera view matrix</summary>
        public Matrix View
        {
            get
            {
                if (needUpdateView)
                    UpdateView();

                return viewMatrix;
            }
        }

		/// <summary>Gets camera projection matrix</summary>
        public Matrix Projection
        {
            get
            {
                if (needUpdateProjection)
                    UpdateProjection();

                return projectionMatrix;
            }
        }

		/// <summary>Gets camera Bounding Frustum</summary>
        public BoundingFrustum Frustum
        {
            get
            {
                if (needUpdateView)
                    UpdateView();
                if (needUpdateProjection)
                    UpdateProjection();
                if (needUpdateFrustum)
                    UpdateFrustum();

                return frustum;
            }
        }
        #endregion

		/// <summary>
		/// <o>BaseCamera</o> constructor
		/// </summary>
		/// <remarks>Creates default settings</remarks>
        public BaseCamera()
        {
            // Default camera configuration
            SetPerspectiveFov(45.0f, 1.0f, 0.1f, 10000.0f);
            SetLookAt(new Vector3(10.0f, 10.0f, 10.0f), Vector3.Zero, new Vector3(0.0f, 1.0f, 0.0f));

            needUpdateView = true;
            needUpdateProjection = true;
        }

		/// <summary>
		/// Sets perspective field of view
		/// </summary>
		/// <param name="fovy">fovy (field of view)</param>
		/// <param name="aspectRatio">aspect ratio</param>
		/// <param name="nearPlane">near plane</param>
		/// <param name="farPlane">far plane</param>
        public void SetPerspectiveFov(float fovy, float aspectRatio, float nearPlane, float farPlane)
        {
            this.fovy = fovy;
            this.aspectRatio = aspectRatio;
            this.nearPlane = nearPlane;
            this.farPlane = farPlane;

            needUpdateProjection = true;
        }

		/// <summary>
		/// Sets 3D point where camera is looking
		/// </summary>
		/// <param name="cameraPos">camera position</param>
		/// <param name="cameraTarget">camera target point</param>
		/// <param name="cameraUp">camera up vector</param>
        public void SetLookAt(Vector3 cameraPos, Vector3 cameraTarget, Vector3 cameraUp)
        {
            this.position = cameraPos;
            this.target = cameraTarget;
            this.upVec = cameraUp;

            headingVec = cameraTarget - cameraPos;
            headingVec.Normalize();
            upVec = cameraUp;
            strafeVec = Vector3.Cross(headingVec, upVec);

            needUpdateView = true;
        }

		/// <summary>
		/// Update camera Matrices: View
		/// </summary>
        protected virtual void UpdateView()
        {
            viewMatrix = Matrix.CreateLookAt(position, target, upVec);

            needUpdateView = false;
            needUpdateFrustum = true;
        }

		/// <summary>
		/// Update camera Matrices: Projection
		/// </summary>
		protected virtual void UpdateProjection()
        {
            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                        MathHelper.ToRadians(fovy), aspectRatio, nearPlane, farPlane);

            needUpdateProjection = false;
            needUpdateFrustum = true;
        }

		/// <summary>
		/// Update camera frustum
		/// </summary>
        protected virtual void UpdateFrustum()
        {
            frustum = new BoundingFrustum(viewMatrix * projectionMatrix);

            needUpdateFrustum = false;
        }

		/// <summary>
		/// Update game view
		/// </summary>
		/// <param name="time">time since last update</param>
        public abstract void Update(GameTime time);
    }

}
