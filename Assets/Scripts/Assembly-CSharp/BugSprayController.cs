using UnityEngine;

public class BugSprayController : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem mist;

	[SerializeField]
	private Transform nozzlePoint;

	[SerializeField]
	private AudioSourceHelper sprayAudio;

	[SerializeField]
	private float raycastDistance = 1f;

	private Collider cachedCollider;

	private CockroachController cachedCockroach;

	private bool isSpraying;

	private int sprayTargetLayerMask = LayerMaskHelper.OnlyIncluding(25);

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
		StopSpraying();
	}

	public void StartSpraying()
	{
		ResetCache();
		isSpraying = true;
		sprayAudio.Play();
	}

	public void StopSpraying()
	{
		ResetCache();
		isSpraying = false;
		sprayAudio.Stop();
	}

	private void ResetCache()
	{
		cachedCollider = null;
		cachedCockroach = null;
	}

	private void Update()
	{
		if (isSpraying)
		{
			if (!mist.isPlaying)
			{
				mist.Play();
			}
			RaycastHit hitInfo;
			if (Physics.Raycast(nozzlePoint.position, nozzlePoint.forward, out hitInfo, raycastDistance, sprayTargetLayerMask))
			{
				if (cachedCollider != hitInfo.collider)
				{
					ResetCache();
					cachedCollider = hitInfo.collider;
					if (hitInfo.collider.attachedRigidbody != null)
					{
						GameObject gameObject = hitInfo.collider.attachedRigidbody.gameObject;
						if (gameObject != null)
						{
							cachedCockroach = gameObject.GetComponent<CockroachController>();
						}
					}
				}
				else if (cachedCockroach != null)
				{
					cachedCockroach.Kill();
				}
			}
			else if (cachedCollider != null)
			{
				ResetCache();
			}
		}
		else if (mist.isPlaying)
		{
			mist.Stop();
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawRay(nozzlePoint.position, nozzlePoint.forward * raycastDistance);
	}
}
