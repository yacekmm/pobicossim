using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

namespace POBICOS.SimBase.Cameras
{
    public class ThirdPersonCamera : BaseCamera
    {
		/// <summary>Maximum allowed camera eye rotate</summary>
        public static float MAX_ROTATE = 15.0f;

		/// <summary>Camera chase parameters: desired chase distance</summary>
        float desiredChaseDistance;
		/// <summary>Camera chase parameters: minimum chase distance</summary>
        float minChaseDistance;
		/// <summary>Camera chase parameters: maximum chase distance</summary>
        float maxChaseDistance;
		/// <summary>Camera chase parameters: chase speed</summary>
        float chaseSpeed;
		/// <summary>Camera chase parameters: camera rotate around player</summary>
		float rotate;
		/// <summary>Camera chase parameters: chase position</summary>
        Vector3 chasePosition;
		/// <summary>Camera chase parameters: chase direction</summary>
        Vector3 chaseDirection;

		/// <summary>Camera chase parameters: eye rotate</summary>
        Vector3 eyeRotate;
		/// <summary>Camera chase parameters: eye rotate velocity</summary>
        Vector3 eyeRotateVelocity;

		/// <summary>Start chasing now</summary>
        bool isFirstTimeChase;

        #region Properties
		/// <summary>Gets or sets camera rotate</summary>
		public float Rotate
		{
			get
			{
				return rotate;
			}
			set
			{
				rotate = value;
				rotate %= 360;
			}

		}

		/// <summary>Gets or sets eye rotate</summary>
        public Vector3 EyeRotate
        {
            get
            {
                return eyeRotate;
            }
            set
            {
                eyeRotate = value;
                needUpdateView = true;
            }
        }

		/// <summary>Gets or sets eye rotate velocity</summary>
        public Vector3 EyeRotateVelocity
        {
            get
            {
                return eyeRotateVelocity;
            }
            set
            {
                eyeRotateVelocity = value;
            }
        }

		/// <summary>Initiates chasing</summary>
        public bool IsFirstTimeChase
        {
            get
            {
                return isFirstTimeChase;
            }
            set
            {
                isFirstTimeChase = value;
            }
        }

		/// <summary>Gets or sets camera chase position</summary>
        public Vector3 ChasePosition
        {
            get
            {
                return chasePosition;
            }
            set
            {
                chasePosition = value;
            }
        }

		/// <summary>Gets or sets camera chase direction</summary>
        public Vector3 ChaseDirection
        {
            get
            {
                return chaseDirection;
            }
            set
            {
                chaseDirection = value;
            }
        }

		/// <summary>Gets or sets camera chase speed</summary>
        public float ChaseSpeed
        {
            get
            {
                return chaseSpeed;
            }
            set
            {
                chaseSpeed = value;
            }
        }
        #endregion

		/// <summary>
		/// <o>ThirdPersonCamera</o> constructor
		/// </summary>
        public ThirdPersonCamera()
        {
			//set default parameters
            SetChaseParameters(2.0f, 10.0f, 5.0f, 15.0f);
            isFirstTimeChase = true;
        }

		/// <summary>
		/// Sets chase parameters
		/// </summary>
		/// <param name="chaseSpeed">chase speed</param>
		/// <param name="desiredChaseDistance">desired chase distance</param>
		/// <param name="minChaseDistance">minimum chase distance</param>
		/// <param name="maxChaseDistance">maximum chase distance</param>
        public void SetChaseParameters(float chaseSpeed, float desiredChaseDistance,
            float minChaseDistance, float maxChaseDistance)
        {
            this.chaseSpeed = chaseSpeed;
            this.desiredChaseDistance = desiredChaseDistance;
            this.minChaseDistance = minChaseDistance;
            this.maxChaseDistance = maxChaseDistance;
        }

		/// <summary>
		/// Updates camera position due to player movement
		/// </summary>
		/// <param name="elapsedTimeSeconds">time elapsed since last camera position update</param>
		/// <param name="interpolate">position should be interpolated</param>
        private void UpdateFollowPosition(float elapsedTimeSeconds, bool interpolate)
        {
            Vector3 TargetPosition = chasePosition;
            Vector3 desiredCameraPosition = chasePosition - chaseDirection * desiredChaseDistance;

            if (interpolate)
            {
                float interpolatedSpeed = MathHelper.Clamp(chaseSpeed * elapsedTimeSeconds, 0.0f, 1.0f);

                // Correct the rotate interpolation problem
                Vector3 TargetVec = desiredCameraPosition - Position;
                TargetVec.Normalize();
                if (Vector3.Dot(TargetVec, chaseDirection) < 0.5)
                    interpolatedSpeed += chaseSpeed * 0.005f;

                desiredCameraPosition = Vector3.Lerp(Position, desiredCameraPosition, interpolatedSpeed);

                // Clamp the min and max follow distances
                Vector3 TargetVector = desiredCameraPosition - TargetPosition;
                float TargetLength = TargetVector.Length();
                TargetVector /= TargetLength;

                if (TargetLength < minChaseDistance)
                {
                    desiredCameraPosition = TargetPosition +
                        TargetVector * minChaseDistance;
                }
                else if (TargetLength > maxChaseDistance)
                {
                    desiredCameraPosition = TargetPosition +
                        TargetVector * maxChaseDistance;
                }
            }

            // Needed to recalculate heading, strafe and up vectors
            SetLookAt(desiredCameraPosition, TargetPosition, UpVector);
        }

		/// <summary>
		/// Update camera
		/// </summary>
		/// <param name="time">time since last update</param>
        public override void Update(GameTime time)
        {
            float elapsedTimeSeconds = (float)time.ElapsedGameTime.TotalSeconds;

            UpdateFollowPosition(elapsedTimeSeconds, !isFirstTimeChase);

            if (isFirstTimeChase)
            {
                eyeRotate = Vector3.Zero;
                eyeRotateVelocity = Vector3.Zero;
                needUpdateView = true;

                isFirstTimeChase = false;
            }

            if (eyeRotateVelocity != Vector3.Zero)
            {
                eyeRotate += eyeRotateVelocity * elapsedTimeSeconds;
                eyeRotate.X = MathHelper.Clamp(eyeRotate.X, -MAX_ROTATE, MAX_ROTATE);
                eyeRotate.Y = MathHelper.Clamp(eyeRotate.Y, -MAX_ROTATE, MAX_ROTATE);
                eyeRotate.Z = MathHelper.Clamp(eyeRotate.Z, -MAX_ROTATE, MAX_ROTATE);
                needUpdateView = true;
            }
        }

		/// <summary>
		/// Update camera view
		/// </summary>
        protected override void UpdateView()
        {
            Vector3 newPosition = Position - Target;

            newPosition = Vector3.Transform(newPosition,
                Matrix.CreateFromAxisAngle(UpVector, MathHelper.ToRadians(eyeRotate.Y)) *
                Matrix.CreateFromAxisAngle(RightVector, MathHelper.ToRadians(eyeRotate.X)) *
                Matrix.CreateFromAxisAngle(HeadingVector, MathHelper.ToRadians(eyeRotate.Z))
                );
            viewMatrix = Matrix.CreateLookAt(newPosition + Target, Target, UpVector);

            needUpdateView = false;
            needUpdateFrustum = true;
        }
    }
}
