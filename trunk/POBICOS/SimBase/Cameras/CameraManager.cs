using System;
using System.Collections.Generic;
using System.Text;

namespace POBICOS.SimBase.Cameras
{
	/// <summary>
	/// Gives ability to control all cameras in game
	/// </summary>
    public class CameraManager
    {
        /// <summary>Store active camera index</summary>
        int activeCameraIndex;
		/// <summary>Store active camera</summary>
        BaseCamera activeCamera;

		/// <summary>Sorted list containing all cameras</summary>
        SortedList<string, BaseCamera> cameras;

        #region Properties
		/// <summary>Gets index of the active camera</summary>
        public int ActiveCameraIndex
        {
            get
            {
                return activeCameraIndex;
            }
        }

		/// <summary>Gets active camera</summary>
        public BaseCamera ActiveCamera
        {
            get
            {
                return activeCamera;
            }
        }

		/// <summary>Gets active camera basing on index</summary>
        public BaseCamera this[int index]
        {
            get
            {
                return cameras.Values[index];
            }
        }

		/// <summary>Gets active camera basing on id</summary>
        public BaseCamera this[string id]
        {
            get
            {
                return cameras[id];
            }
        }

		/// <summary>Gets camera count</summary>
        public int Count
        {
            get
            {
                return cameras.Count;
            }
        }
        #endregion

		/// <summary>
		/// <o>CameraManager</o> constructor
		/// </summary>
        public CameraManager()
        {
            cameras = new SortedList<string, BaseCamera>(4);
            activeCameraIndex = -1;
        }

		/// <summary>
		/// set active camera using index
		/// </summary>
		/// <param name="cameraIndex">index of the camera that is going to be active</param>
        public void SetActiveCamera(int cameraIndex)
        {
            activeCameraIndex = cameraIndex;
            activeCamera = cameras[cameras.Keys[cameraIndex]];
        }

		/// <summary>
		/// set active camera using identifier
		/// </summary>
		/// <param name="cameraIndex">identifier of the camera that is going to be active</param>
		public void SetActiveCamera(string id)
        {
            activeCameraIndex = cameras.IndexOfKey(id);
            activeCamera = cameras[id];
        }

		/// <summary>
		/// Remove all cameras
		/// </summary>
        public void Clear()
        {
            cameras.Clear();
            activeCamera = null;
            activeCameraIndex = -1;
        }

		/// <summary>
		/// Add new camera
		/// </summary>
		/// <param name="id">camera identifier</param>
		/// <param name="camera">camera</param>
        public void Add(string id, BaseCamera camera)
        {
            cameras.Add(id, camera);

            if (activeCamera == null)
            {
                activeCamera = camera;
                activeCameraIndex = cameras.IndexOfKey(id);
            }
        }

		/// <summary>
		/// Remove camera from game
		/// </summary>
		/// <param name="id">camera identifier</param>
        public void Remove(string id)
        {
            cameras.Remove(id);
        }
    }
}
