using UnityEngine;

public static class Vector3Extensions
{
	public static string ToStringPrecise(this Vector3 vec)
	{
		return "(" + vec.x + ", " + vec.y + ", " + vec.z + ")";
	}

	public static Vector3 Multiply(Vector3 v1, Vector3 v2)
	{
		return new Vector3(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
	}
}
