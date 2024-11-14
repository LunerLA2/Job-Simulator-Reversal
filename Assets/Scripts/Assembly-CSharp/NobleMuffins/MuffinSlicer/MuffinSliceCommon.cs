using System;
using UnityEngine;

namespace NobleMuffins.MuffinSlicer
{
	public class MuffinSliceCommon
	{
		private const float epsilon = 0f;

		public static Vector3 clampNormalToBicone(Vector3 input, Vector3 axis, float maximumDegrees)
		{
			float num = Mathf.Cos(maximumDegrees * ((float)Math.PI / 180f));
			float f = Vector3.Dot(input, axis);
			Vector3 vector = input;
			if (Mathf.Abs(f) < num)
			{
				float num2 = Mathf.Sign(f);
				float num3 = num - Mathf.Abs(f);
				Vector3 vector2 = axis * num3 * num2;
				float num4 = 1f;
				float num5 = 1f;
				float num6 = 100f;
				for (int num7 = 16; num7 > 0; num7--)
				{
					vector = (input + vector2 * num4).normalized;
					float num8 = Mathf.Abs(Vector3.Dot(vector, axis));
					if (num8 > num)
					{
						num6 = num4;
						num4 = (num4 + num5) / 2f;
					}
					else if (num8 < num)
					{
						num5 = num4;
						num4 = (num4 + num6) / 2f;
					}
				}
			}
			return vector;
		}

		public static Vector4 planeFromPointAndNormal(Vector3 point, Vector3 normal)
		{
			Vector4 result = normal.normalized;
			result.w = 0f - (normal.x * point.x + normal.y * point.y + normal.z * point.z);
			return result;
		}

		public static float classifyPoint(ref Vector4 plane, ref Vector3 p)
		{
			return p.x * plane.x + p.y * plane.y + p.z * plane.z + plane.w;
		}

		public static PlaneTriResult getSidePlane(ref Vector3 p, ref Vector4 plane)
		{
			double num = distanceToPoint(ref p, ref plane);
			if (num > 0.0)
			{
				return PlaneTriResult.PTR_FRONT;
			}
			return PlaneTriResult.PTR_BACK;
		}

		public static float distanceToPoint(ref Vector3 p, ref Vector4 plane)
		{
			return p.x * plane.x + p.y * plane.y + p.z * plane.z + plane.w;
		}

		public static float intersectCommon(ref Vector3 p1, ref Vector3 p2, ref Vector4 plane)
		{
			float num = distanceToPoint(ref p1, ref plane);
			Vector3 vector = p2 - p1;
			float num2 = vector.x * plane.x + vector.y * plane.y + vector.z * plane.z;
			float num3 = num - plane.w;
			return (0f - (plane.w + num3)) / num2;
		}
	}
}
