using UnityEngine;

public static class TransformExtensions
{
	public static void SetToDefaultPosRotScale(this Transform t)
	{
		t.localPosition = Vector3.zero;
		t.localRotation = Quaternion.identity;
		t.localScale = Vector3.one;
	}

	public static void SetLocalPositionXOnly(this Transform t, float x)
	{
		t.localPosition = new Vector3(x, t.localPosition.y, t.localPosition.z);
	}

	public static void SetLocalPositionYOnly(this Transform t, float y)
	{
		t.localPosition = new Vector3(t.localPosition.x, y, t.localPosition.z);
	}

	public static void SetLocalPositionZOnly(this Transform t, float z)
	{
		t.localPosition = new Vector3(t.localPosition.x, t.localPosition.y, z);
	}

	public static void SetGlobalPositionXOnly(this Transform t, float x)
	{
		t.position = new Vector3(x, t.position.y, t.position.z);
	}

	public static void SetGlobalPositionYOnly(this Transform t, float y)
	{
		t.position = new Vector3(t.position.x, y, t.position.z);
	}

	public static void SetGlobalPositionZOnly(this Transform t, float z)
	{
		t.position = new Vector3(t.position.x, t.position.y, z);
	}
}
