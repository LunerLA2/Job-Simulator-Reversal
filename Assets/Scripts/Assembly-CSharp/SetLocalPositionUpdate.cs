using UnityEngine;

public class SetLocalPositionUpdate : MonoBehaviour
{
	public float xPos;

	public float yPos;

	public float zPos;

	private void Awake()
	{
		xPos = base.transform.localPosition.x;
		yPos = base.transform.localPosition.y;
		zPos = base.transform.localPosition.z;
	}

	private void Update()
	{
		base.transform.localPosition = new Vector3(xPos, yPos, zPos);
	}
}
