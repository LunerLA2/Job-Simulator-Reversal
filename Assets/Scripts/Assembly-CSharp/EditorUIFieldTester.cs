using UnityEngine;

public class EditorUIFieldTester : MonoBehaviour
{
	public enum CustomEnum
	{
		Unset = 0,
		On = 1,
		Off = 2
	}

	public int intValue;

	public float floatValue;

	public string stringValue;

	public bool boolValue;

	public Vector3 vector3Value;

	public Vector2 vector2Value;

	public Transform transformValue;

	public GameObject gameObjectValue;

	public Rigidbody rigidbodyValue;

	public Color colorValue;

	public CustomEnum customEnumValue;

	public string[] stringList;
}
