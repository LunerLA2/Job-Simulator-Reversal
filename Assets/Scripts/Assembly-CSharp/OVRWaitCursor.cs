using UnityEngine;

public class OVRWaitCursor : MonoBehaviour
{
	public Vector3 rotateSpeeds = new Vector3(0f, 0f, -60f);

	private void Update()
	{
		base.transform.Rotate(rotateSpeeds * Time.smoothDeltaTime);
	}
}
