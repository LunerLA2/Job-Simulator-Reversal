using OwlchemyVR;
using UnityEngine;

public class FireExtinguisherController : MonoBehaviour
{
	[SerializeField]
	private ParticleSystem sprayFx;

	[SerializeField]
	private ParticleSystem psvrSprayFx;

	[SerializeField]
	private WorldItemData foamFluidData;

	[SerializeField]
	private float mlOfFoamPerSec = 1000f;

	[SerializeField]
	private AudioSourceHelper audioSource;

	[SerializeField]
	private AudioClip loopingSpraySound;

	[SerializeField]
	private Transform nozzlePoint;

	[SerializeField]
	private float raycastDistance = 1f;

	[SerializeField]
	private float forcePerSecToApply = 10f;

	[SerializeField]
	private AnimationCurve forceFalloff;

	private Collider cachedNormalCollider;

	private Collider cachedSprayCollider;

	private Rigidbody cachedNormalRigidbody;

	private WorldItem cachedWorldItem;

	private ParticleCollectionZone cachedParticleCollectionZone;

	private ExtinguishableItem cachedExtinguishableItem;

	private float timeSpentOnCachedCollider;

	private bool isFiring;

	private int sprayTargetLayerMask = LayerMaskHelper.OnlyIncluding(25);

	private int normalTargetLayerMask = LayerMaskHelper.EverythingBut(20);

	public void StartFiring()
	{
		audioSource.SetLooping(true);
		audioSource.SetClip(loopingSpraySound);
		audioSource.Play();
		ResetSprayCache();
		ResetNormalCache();
		isFiring = true;
	}

	public void StopFiring()
	{
		audioSource.Stop();
		ResetSprayCache();
		ResetNormalCache();
		isFiring = false;
	}

	private void ResetSprayCache()
	{
		cachedWorldItem = null;
		cachedSprayCollider = null;
		cachedParticleCollectionZone = null;
		cachedExtinguishableItem = null;
		timeSpentOnCachedCollider = 0f;
	}

	private void ResetNormalCache()
	{
		cachedNormalCollider = null;
		cachedNormalRigidbody = null;
	}

	private void Update()
	{
		if (isFiring)
		{
			if (sprayFx != null && !sprayFx.isPlaying)
			{
				sprayFx.Play();
			}
			RaycastHit hitInfo;
			if (Physics.Raycast(nozzlePoint.position, nozzlePoint.forward, out hitInfo, raycastDistance, normalTargetLayerMask))
			{
				if (cachedNormalCollider != hitInfo.collider)
				{
					ResetNormalCache();
					cachedNormalCollider = hitInfo.collider;
					if (hitInfo.collider.attachedRigidbody != null)
					{
						cachedNormalRigidbody = hitInfo.collider.attachedRigidbody;
						cachedParticleCollectionZone = hitInfo.collider.GetComponent<ParticleCollectionZone>();
					}
				}
				else
				{
					if (cachedNormalRigidbody != null)
					{
						float num = Vector3.Distance(nozzlePoint.position, hitInfo.point);
						float num2 = forceFalloff.Evaluate(num / raycastDistance) * forcePerSecToApply;
						cachedNormalRigidbody.AddForce(nozzlePoint.forward * num2, ForceMode.Force);
					}
					if (cachedParticleCollectionZone != null)
					{
						cachedParticleCollectionZone.ApplyParticleQuantity(foamFluidData, mlOfFoamPerSec * Time.deltaTime, 0f);
					}
				}
			}
			else if (cachedNormalCollider != null)
			{
				ResetNormalCache();
			}
			if (Physics.Raycast(nozzlePoint.position, nozzlePoint.forward, out hitInfo, raycastDistance, sprayTargetLayerMask))
			{
				if (cachedSprayCollider != hitInfo.collider)
				{
					ResetSprayCache();
					cachedSprayCollider = hitInfo.collider;
					if (hitInfo.collider.attachedRigidbody != null)
					{
						GameObject gameObject = hitInfo.collider.attachedRigidbody.gameObject;
						if (gameObject != null)
						{
							cachedWorldItem = gameObject.GetComponent<WorldItem>();
							cachedExtinguishableItem = gameObject.GetComponent<ExtinguishableItem>();
						}
					}
					return;
				}
				timeSpentOnCachedCollider += Time.deltaTime;
				if (cachedExtinguishableItem != null)
				{
					cachedExtinguishableItem.ExtinguishProgress(Time.deltaTime);
					if (cachedWorldItem != null)
					{
						GameEventsManager.Instance.ItemActionOccurredWithAmount(cachedWorldItem.Data, "EXTINGUISHED_FOR_SECS", timeSpentOnCachedCollider);
					}
				}
			}
			else if (cachedSprayCollider != null)
			{
				ResetSprayCache();
			}
		}
		else if (sprayFx != null && sprayFx.isPlaying)
		{
			sprayFx.Stop();
		}
	}

	private void OnDrawGizmos()
	{
		if (nozzlePoint != null)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawRay(nozzlePoint.position, nozzlePoint.forward * raycastDistance);
		}
	}
}
