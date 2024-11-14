using OwlchemyVR;
using UnityEngine;

public class SafeController : KitchenTool
{
	[SerializeField]
	private WorldItem doorWorldItem;

	[SerializeField]
	private GrabbableHinge doorHinge;

	[SerializeField]
	private GrabbableItem dialGrabbableItem;

	[SerializeField]
	private GameObject[] activeWhileLocked;

	[SerializeField]
	private GameObject[] activeWhileUnlocked;

	private float considerClosedWhenLessThan = 0.1f;

	private float considerOpenWhenGreaterThan = 0.3f;

	private bool isDoorOpen;

	[SerializeField]
	[Range(0f, 360f)]
	private float targetUnlockAngle;

	[Range(0f, 90f)]
	[SerializeField]
	private float unlockAngleMargin = 0.05f;

	[SerializeField]
	private AudioClip safeSuccessClickSound;

	[SerializeField]
	private AudioClip[] safemovetick;

	private ElementSequence<AudioClip> safemoveticksequenced;

	private HapticInfoObject hapticObject;

	private bool dialLocked;

	[SerializeField]
	private ItemCollectionZone itemCollectionZone;

	[SerializeField]
	private WorldItemData worldItemToHack;

	[SerializeField]
	private Animation handleAnim;

	[SerializeField]
	private AnimationClip handleOut;

	[SerializeField]
	private AnimationClip handleIn;

	private float lastAngle;

	private void Awake()
	{
		hapticObject = new HapticInfoObject(800f, 0.2f);
		hapticObject.DeactiveHaptic();
		SetLockedState(true);
		safemoveticksequenced = new ElementSequence<AudioClip>(safemovetick);
	}

	public override void OnDismiss()
	{
		if (!doorHinge.IsLowerLocked)
		{
			doorHinge.LockLower();
		}
		for (int i = 0; i < itemCollectionZone.ItemsInCollection.Count; i++)
		{
			if (itemCollectionZone.ItemsInCollection[i] != null)
			{
				WorldItem component = itemCollectionZone.ItemsInCollection[i].GetComponent<WorldItem>();
				if (component != null && component.Data == worldItemToHack)
				{
					component.HackDisableWithoutEvent();
				}
			}
		}
		base.OnDismiss();
	}

	private void Update()
	{
		bool flag = Mathf.Abs(dialGrabbableItem.transform.localEulerAngles.z - targetUnlockAngle) > unlockAngleMargin;
		if (flag != dialLocked)
		{
			SetLockedState(flag);
		}
		if (!isDoorOpen)
		{
			if (doorHinge.NormalizedAngle >= considerOpenWhenGreaterThan)
			{
				isDoorOpen = true;
				GameEventsManager.Instance.ItemActionOccurred(doorWorldItem.Data, "OPENED");
			}
		}
		else if (doorHinge.NormalizedAngle <= considerClosedWhenLessThan)
		{
			isDoorOpen = false;
			GameEventsManager.Instance.ItemActionOccurred(doorWorldItem.Data, "CLOSED");
		}
		bool flag2 = (dialGrabbableItem.transform.localEulerAngles.z + 5f) % 15f < 1f && lastAngle != dialGrabbableItem.transform.localEulerAngles.z;
		lastAngle = dialGrabbableItem.transform.localEulerAngles.z;
		if (flag2)
		{
			AudioManager.Instance.Play(dialGrabbableItem.transform.position, safemoveticksequenced.GetNext(), 1f, 1f);
		}
	}

	private void SetLockedState(bool isLocked)
	{
		for (int i = 0; i < activeWhileLocked.Length; i++)
		{
			activeWhileLocked[i].SetActive(isLocked);
		}
		for (int j = 0; j < activeWhileUnlocked.Length; j++)
		{
			activeWhileUnlocked[j].SetActive(!isLocked);
		}
		dialLocked = isLocked;
		if (isLocked)
		{
			handleAnim.Play(handleIn.name);
		}
		else
		{
			handleAnim.Play(handleOut.name);
		}
		if (!isLocked)
		{
			DoHaptic();
			AudioManager.Instance.Play(dialGrabbableItem.transform.position, safeSuccessClickSound, 1f, 1f);
		}
	}

	private void DoHaptic()
	{
		if (dialGrabbableItem.IsCurrInHand)
		{
			hapticObject.Restart();
			if (!dialGrabbableItem.CurrInteractableHand.HapticsController.ContainHaptic(hapticObject))
			{
				dialGrabbableItem.CurrInteractableHand.HapticsController.AddNewHaptic(hapticObject);
			}
		}
	}
}
