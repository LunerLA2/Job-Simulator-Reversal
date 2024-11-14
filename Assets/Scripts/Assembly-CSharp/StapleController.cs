using UnityEngine;

public class StapleController : MonoBehaviour
{
	[SerializeField]
	private bool isFired;

	[SerializeField]
	private TrailRenderer trail;

	private BoxCollider col;

	private Vector3 originalPosition;

	private void Awake()
	{
		col = GetComponent<BoxCollider>();
	}

	private void Start()
	{
		ResetState();
	}

	public void ResetState()
	{
		originalPosition = base.transform.position;
		if (isFired && trail != null)
		{
			trail.enabled = true;
			col.enabled = false;
		}
	}

	private void Update()
	{
		if (isFired && !col.enabled && Vector3.Distance(base.transform.position, originalPosition) >= 0.1f)
		{
			col.enabled = true;
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (isFired && trail != null && trail.enabled)
		{
			trail.enabled = false;
		}
	}
}
