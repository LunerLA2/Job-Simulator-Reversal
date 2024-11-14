using UnityEngine;

[ExecuteInEditMode]
public class NewtonsCradleBallConfigurator : MonoBehaviour
{
	public float DistanceBetweenBeams = 0.16f;

	public float BeamHeight = 0.175f;

	public float BallRadius = 0.03f;

	[SerializeField]
	private Transform[] balls;

	private void Awake()
	{
		if (Application.isPlaying)
		{
			base.enabled = false;
		}
	}

	private void Update()
	{
		Vector3 localScale = Vector3.one * BallRadius * 2f;
		for (int i = 0; i < balls.Length; i++)
		{
			Transform transform = balls[i];
			float z = (float)(2 * i - balls.Length + 1) * BallRadius;
			transform.localPosition = new Vector3(0f, transform.localPosition.y, z);
			transform.localScale = localScale;
			SpringJoint[] components = transform.GetComponents<SpringJoint>();
			components[0].connectedAnchor = new Vector3((0f - DistanceBetweenBeams) / 2f, BeamHeight, z);
			components[1].connectedAnchor = new Vector3(DistanceBetweenBeams / 2f, BeamHeight, z);
		}
	}
}
