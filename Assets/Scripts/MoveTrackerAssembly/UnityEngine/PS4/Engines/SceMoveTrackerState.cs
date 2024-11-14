using System;

namespace UnityEngine.PS4.Engines
{
	[Serializable]
	public struct SceMoveTrackerState
	{
		public Vector3 position;

		public Vector3 velocity;

		public Vector3 acceleration;

		public Quaternion orientation;

		public Vector3 angularVelocity;

		public Vector3 angularAcceleration;

		public Vector3 accelerometerPosition;

		public Vector3 accelerometerVelocity;

		public Vector3 accelerometerAcceleration;

		public float cameraPitchAngle;

		public float cameraRollAngle;

		public SceMoveButtonData pad;

		public SceMoveExtensionPortData ext;

		public ulong timestamp;

		public uint flags;
	}
}
