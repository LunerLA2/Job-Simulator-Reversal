using UnityEngine;

namespace OwlchemyVR
{
	public static class MathUtils
	{
		public static float GetAngleFromOrigin(this Vector2 v)
		{
			return (57.29578f * Mathf.Atan2(v.y, v.x) + 360f) % 360f;
		}
	}
}
