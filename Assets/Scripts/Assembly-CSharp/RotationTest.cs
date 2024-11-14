using UnityEngine;

public class RotationTest : MonoBehaviour
{
	private MeshRenderer myRenderer;

	private bool valid;

	private float min = -45f;

	private float max = 45f;

	private float deltaAngle;

	private float baseAngle = -90f;

	private void Awake()
	{
		myRenderer = GetComponent<MeshRenderer>();
	}

	private void Update()
	{
		deltaAngle = Mathf.DeltaAngle(base.transform.eulerAngles.y, baseAngle);
		valid = deltaAngle < max && deltaAngle > min;
		if (valid)
		{
			myRenderer.material.color = Color.green;
		}
		else
		{
			myRenderer.material.color = Color.red;
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawRay(base.transform.position, base.transform.forward);
	}
}
