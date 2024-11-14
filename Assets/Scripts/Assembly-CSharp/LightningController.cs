using UnityEngine;

public class LightningController : MonoBehaviour
{
	[SerializeField]
	private LineRenderer lineRenderer;

	[SerializeField]
	private Transform endPoint;

	[SerializeField]
	private float arcLength = 0.1f;

	[SerializeField]
	private float maxRandomDistance = 0.1f;

	[SerializeField]
	private float changeSpeed = 0.01f;

	private bool isPlaying;

	private int numberOfArcs;

	private int vertexCount;

	private float nextChange;

	private void Start()
	{
		numberOfArcs = (int)(arcLength * 100f);
		vertexCount = 100 / numberOfArcs + 2;
		lineRenderer.SetVertexCount(vertexCount);
		lineRenderer.SetPosition(0, base.transform.position);
		lineRenderer.SetPosition(vertexCount - 1, endPoint.position);
		SetNewPoints();
		lineRenderer.enabled = false;
	}

	private void Update()
	{
		if (isPlaying && Time.time >= nextChange)
		{
			SetNewPoints();
		}
	}

	private void SetNewPoints()
	{
		float num = 0f;
		for (int i = 1; i < vertexCount - 1; i++)
		{
			num += arcLength;
			Vector3 position = Vector3.Lerp(base.transform.position, endPoint.position, num);
			position.x += Random.Range(0f - maxRandomDistance, maxRandomDistance);
			position.y += Random.Range(0f - maxRandomDistance, maxRandomDistance);
			position.z += Random.Range(0f - maxRandomDistance, maxRandomDistance);
			lineRenderer.SetPosition(i, position);
		}
		nextChange = Time.time + changeSpeed;
	}

	public void SetEndPoint(Transform newPoint)
	{
		endPoint = newPoint;
	}

	public void Play()
	{
		lineRenderer.enabled = true;
		isPlaying = true;
	}

	public void Stop()
	{
		lineRenderer.enabled = false;
		isPlaying = false;
	}
}
