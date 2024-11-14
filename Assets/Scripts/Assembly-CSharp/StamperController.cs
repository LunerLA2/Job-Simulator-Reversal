using System;
using OwlchemyVR;
using UnityEngine;

public class StamperController : MonoBehaviour
{
	private const float DISTANCE_STAMP_THRESHOLD = 0.035f;

	private const float DISTANCE_BREAK_AWAY = 0.02f;

	[SerializeField]
	private Transform stamper;

	[SerializeField]
	private Transform lever;

	[SerializeField]
	private GrabbableItem leverGrabbable;

	[SerializeField]
	private GrabbableHinge hinge;

	[SerializeField]
	private float minAngle;

	[SerializeField]
	private float maxAngle;

	[SerializeField]
	private float stamperLocalYMin;

	[SerializeField]
	private float stamperLocalYMax;

	[SerializeField]
	private Transform raycastPosition;

	[SerializeField]
	private Transform placeStampRaycastPosition;

	[SerializeField]
	private Transform[] cornerPositionChecks;

	[SerializeField]
	private Collider stampBareSurfaceCollider;

	[SerializeField]
	private StampWorldItemSwitch[] worldItemSwitches;

	[SerializeField]
	private GameObject stampPrefab;

	[SerializeField]
	private AudioClip leverSound;

	[SerializeField]
	private AudioSourceHelper leverSource;

	[SerializeField]
	private AudioClip stampSound;

	[SerializeField]
	private MeshRenderer baseMesh;

	[SerializeField]
	private MeshRenderer stampMesh;

	[SerializeField]
	private MeshRenderer leverMesh;

	[SerializeField]
	private Sprite[] endlessModeRandomSprites;

	private float ratio;

	private RaycastHit hit;

	private float distance;

	private float lastDistance;

	private float lastValidRatio;

	private float lastValidLeverRotation;

	private float recentlyInHandTimer;

	private float lastStampTime;

	private ObjectPool<GameObject> stampPool;

	private void Awake()
	{
	}

	private void OnEnable()
	{
		GrabbableItem grabbableItem = leverGrabbable;
		grabbableItem.OnReleased = (Action<GrabbableItem>)Delegate.Combine(grabbableItem.OnReleased, new Action<GrabbableItem>(LeverReleased));
	}

	private void OnDisable()
	{
		GrabbableItem grabbableItem = leverGrabbable;
		grabbableItem.OnReleased = (Action<GrabbableItem>)Delegate.Remove(grabbableItem.OnReleased, new Action<GrabbableItem>(LeverReleased));
	}

	public void SetupStamper(GameObject stampPrefab, Material meshMaterial, StampWorldItemSwitch[] worldItemSwitches)
	{
		this.stampPrefab = stampPrefab;
		baseMesh.sharedMaterial = meshMaterial;
		leverMesh.sharedMaterial = meshMaterial;
		stampMesh.sharedMaterial = meshMaterial;
		this.worldItemSwitches = worldItemSwitches;
		stampPool = new ObjectPool<GameObject>(this.stampPrefab, 10, true, true, GlobalStorage.Instance.ContentRoot, Vector3.zero);
	}

	private void LateUpdate()
	{
		ratio = hinge.NormalizedAngle;
		if (recentlyInHandTimer > 0f)
		{
			recentlyInHandTimer -= Time.deltaTime;
		}
		if (leverGrabbable.IsCurrInHand || recentlyInHandTimer > 0f || ratio > 0.1f)
		{
			if (Physics.Raycast(raycastPosition.position, Vector3.down, out hit, 1f, 256))
			{
				distance = Vector3.Distance(raycastPosition.position, hit.point);
				if (distance <= 0.035f && lastDistance > 0.035f && (hit.collider.gameObject.layer == 8 || hit.collider.gameObject.layer == 0))
				{
					if (!leverSource.IsPlaying)
					{
						leverSource.SetClip(leverSound);
						leverSource.Play();
					}
					DoStamp();
				}
				lastDistance = distance;
			}
			if (distance >= 0.035f || ratio <= lastValidRatio)
			{
				lastValidRatio = ratio;
				lastValidLeverRotation = lever.eulerAngles.x;
				stamper.localPosition = new Vector3(stamper.localPosition.x, Mathf.Lerp(stamperLocalYMin, stamperLocalYMax, ratio), stamper.localPosition.z);
			}
			else if (distance <= 0.02f)
			{
				if (hit.collider.gameObject.layer == 8 || hit.collider.gameObject.layer == 0)
				{
					if (!leverSource.IsPlaying)
					{
						leverSource.SetClip(leverSound);
						leverSource.Play();
					}
					DoStamp();
				}
				if (leverGrabbable.CurrInteractableHand != null)
				{
					leverGrabbable.CurrInteractableHand.ManuallyReleaseJoint();
				}
			}
			if (lever.eulerAngles.x > lastValidLeverRotation)
			{
				lever.eulerAngles = new Vector3(lastValidLeverRotation, lever.eulerAngles.y, lever.eulerAngles.z);
			}
		}
		else
		{
			stamper.localPosition = new Vector3(stamper.localPosition.x, Mathf.Lerp(stamperLocalYMin, stamperLocalYMax, ratio), stamper.localPosition.z);
		}
	}

	private void LeverReleased(GrabbableItem item)
	{
		recentlyInHandTimer = 1f;
	}

	private void DoStamp()
	{
		if (Time.time - lastStampTime < 0.2f)
		{
			return;
		}
		lastStampTime = Time.time;
		RaycastHit hitInfo;
		if (!Physics.Raycast(placeStampRaycastPosition.position, Vector3.down, out hitInfo, 1f, 256) || !(hitInfo.collider != stampBareSurfaceCollider) || !(hitInfo.collider.attachedRigidbody != null) || hitInfo.collider.isTrigger)
		{
			return;
		}
		WorldItem component = hitInfo.collider.attachedRigidbody.GetComponent<WorldItem>();
		if (!(component != null))
		{
			return;
		}
		bool flag = true;
		for (int i = 0; i < cornerPositionChecks.Length; i++)
		{
			RaycastHit hitInfo2;
			if (Physics.Raycast(cornerPositionChecks[i].position, Vector3.down, out hitInfo2, 1f, 256))
			{
				if (hitInfo2.collider != hitInfo.collider)
				{
					flag = false;
				}
			}
			else
			{
				flag = false;
			}
		}
		if (!flag)
		{
			return;
		}
		AudioManager.Instance.Play(hitInfo.point, stampSound, 1f, 1f);
		WorldItemData worldItemData = null;
		for (int j = 0; j < worldItemSwitches.Length; j++)
		{
			if (worldItemSwitches[j].FromData == component.Data)
			{
				GameEventsManager.Instance.ItemActionOccurred(worldItemSwitches[j].FromData, "ACTIVATED");
				component.ManualSetData(worldItemSwitches[j].ToData);
				worldItemData = worldItemSwitches[j].ToData;
				break;
			}
		}
		GameObject gameObject = stampPool.Fetch(hitInfo.point + Vector3.up * 0.0035f, Quaternion.Euler(0f, base.transform.eulerAngles.y, 0f));
		if (JobBoardManager.instance != null && JobBoardManager.instance.EndlessModeStatusController != null && endlessModeRandomSprites.Length > 0)
		{
			SpriteRenderer componentInChildren = gameObject.GetComponentInChildren<SpriteRenderer>();
			componentInChildren.sprite = endlessModeRandomSprites[UnityEngine.Random.Range(0, endlessModeRandomSprites.Length)];
		}
		gameObject.transform.parent = hitInfo.collider.transform;
		if (worldItemData != null)
		{
			GameEventsManager.Instance.ItemActionOccurred(worldItemData, "CREATED");
		}
	}

	private void OnDrawGizmos()
	{
		for (int i = 0; i < cornerPositionChecks.Length; i++)
		{
			Gizmos.DrawLine(cornerPositionChecks[i].position, cornerPositionChecks[i].position + Vector3.down * 0.2f);
		}
	}
}
