using System.Collections;
using UnityEngine;

public class TrinketSpringController : MonoBehaviour
{
	private const float DISTANCE_TO_DISABLE_COLLIDERS = 0.7f;

	private Collider[] emergencyColliders;

	[HideInInspector]
	public SpringJoint springJoint;

	[HideInInspector]
	public LineRenderer lineRenderer;

	private GameObject lineRendererObj;

	private Rigidbody rb;

	private float cachedDrag;

	private float cachedAngularDrag;

	private float cachedSleepThreshold;

	private bool colliderToggleRoutineInProgress;

	private void Awake()
	{
		rb = GetComponent<Rigidbody>();
		cachedDrag = rb.drag;
		cachedAngularDrag = rb.angularDrag;
		rb.drag = 1f;
		rb.angularDrag = 1f;
		emergencyColliders = GetComponentsInChildren<Collider>();
		springJoint = base.gameObject.AddComponent<SpringJoint>();
		springJoint.autoConfigureConnectedAnchor = false;
		springJoint.connectedAnchor = Vector3.zero;
		springJoint.spring = 100f;
		springJoint.damper = 45f;
		springJoint.enableCollision = true;
	}

	private void OnEnable()
	{
		cachedSleepThreshold = rb.sleepThreshold;
		if (rb != null)
		{
			rb.sleepThreshold = 0f;
		}
	}

	private void OnDisable()
	{
		if (rb != null)
		{
			rb.drag = cachedDrag;
			rb.angularDrag = cachedAngularDrag;
			rb.sleepThreshold = cachedSleepThreshold;
		}
	}

	private void OnDestroy()
	{
		if (colliderToggleRoutineInProgress)
		{
			StopAllCoroutines();
			for (int i = 0; i < emergencyColliders.Length; i++)
			{
				if (emergencyColliders[i] != null)
				{
					emergencyColliders[i].enabled = true;
				}
			}
		}
		if (springJoint != null)
		{
			Object.Destroy(springJoint);
		}
		if (rb != null)
		{
			rb.drag = cachedDrag;
			rb.angularDrag = cachedAngularDrag;
			rb.sleepThreshold = cachedSleepThreshold;
		}
	}

	private void Update()
	{
		if (!(springJoint == null) && !(springJoint.connectedBody == null) && Vector3.Distance(base.transform.position, springJoint.connectedBody.position) > 0.7f && !colliderToggleRoutineInProgress)
		{
			StartCoroutine(ToggleColliderRoutine());
		}
	}

	private IEnumerator ToggleColliderRoutine()
	{
		colliderToggleRoutineInProgress = true;
		for (int j = 0; j < emergencyColliders.Length; j++)
		{
			if (emergencyColliders[j] != null)
			{
				emergencyColliders[j].enabled = false;
			}
		}
		yield return new WaitForSeconds(0.25f);
		for (int i = 0; i < emergencyColliders.Length; i++)
		{
			if (emergencyColliders[i] != null)
			{
				emergencyColliders[i].enabled = true;
			}
		}
		colliderToggleRoutineInProgress = false;
	}
}
