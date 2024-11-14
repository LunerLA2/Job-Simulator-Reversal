using UnityEngine;

namespace OwlchemyVR
{
	public class HandState
	{
		public Vector3 Position;

		public Vector3 EulerAngles;

		public bool[] CommonButtons = new bool[8];

		public float[] CommonAxes = new float[8];

		public bool[] CustomButtons = new bool[8];

		public float[] CustomAxes = new float[8];

		public bool GetCommonButton(int i)
		{
			if (i < 0 || i >= CommonButtons.Length)
			{
				return false;
			}
			return CommonButtons[i];
		}

		public bool GetCustomButton(int i)
		{
			if (i < 0 || i >= CustomButtons.Length)
			{
				return false;
			}
			return CustomButtons[i];
		}

		public float GetCommonAxis(int i)
		{
			if (i < 0 || i >= CommonAxes.Length)
			{
				return 0f;
			}
			return CommonAxes[i];
		}

		public float GetCustomAxis(int i)
		{
			if (i < 0 || i >= CustomAxes.Length)
			{
				return 0f;
			}
			return CustomAxes[i];
		}
	}
}
