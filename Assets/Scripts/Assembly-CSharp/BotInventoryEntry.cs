using OwlchemyVR;
using UnityEngine;

public class BotInventoryEntry
{
	private Transform tr;

	private Rigidbody rb;

	private GrabbableItem gi;

	private BotInventoryCustomMountPosition mountPosition;

	private bool originalRBkinematicSetting;

	private bool isLarge;

	private Bounds colliderBounds;

	private Vector3 distFromBoundsCenter = Vector3.zero;

	public Transform Transform
	{
		get
		{
			return tr;
		}
	}

	public Rigidbody Rigidbody
	{
		get
		{
			return rb;
		}
	}

	public GrabbableItem GrabbableItem
	{
		get
		{
			return gi;
		}
	}

	public BotInventoryCustomMountPosition MountPosition
	{
		get
		{
			return mountPosition;
		}
	}

	public bool OriginalRBKinematicSetting
	{
		get
		{
			return originalRBkinematicSetting;
		}
	}

	public bool IsLarge
	{
		get
		{
			return isLarge;
		}
	}

	public Bounds ColliderBounds
	{
		get
		{
			return colliderBounds;
		}
	}

	public Vector3 DistFromBoundsCenter
	{
		get
		{
			return distFromBoundsCenter;
		}
	}

	public BotInventoryEntry(Transform t, bool large)
	{
		tr = t;
		rb = t.GetComponent<Rigidbody>();
		gi = t.GetComponent<GrabbableItem>();
		mountPosition = t.GetComponent<BotInventoryCustomMountPosition>();
		isLarge = large;
		if (rb != null)
		{
			originalRBkinematicSetting = rb.isKinematic;
		}
		colliderBounds = default(Bounds);
		bool flag = true;
		Quaternion rotation = t.rotation;
		t.rotation = Quaternion.identity;
		if (mountPosition != null)
		{
			if (isLarge || mountPosition.RegisterAsLargeWhenGrabbedNormally)
			{
				t.rotation = Quaternion.Euler(mountPosition.LocalRotWhenHeldAsLargeItem);
			}
			else
			{
				t.rotation = Quaternion.Euler(mountPosition.LocalRotWhenHeldAsSmallItem);
			}
		}
		Collider[] componentsInChildren = tr.GetComponentsInChildren<Collider>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			if (!componentsInChildren[i].isTrigger)
			{
				if (flag)
				{
					colliderBounds = componentsInChildren[i].bounds;
					flag = false;
				}
				else
				{
					colliderBounds.Encapsulate(componentsInChildren[i].bounds);
				}
			}
		}
		distFromBoundsCenter = colliderBounds.center - t.position;
		t.rotation = rotation;
	}
}
