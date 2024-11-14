using System.Collections;
using OwlchemyVR;
using UnityEngine;

public class GiftBoxController : MonoBehaviour
{
	private enum GiftBoxState
	{
		Wrapped = 0,
		Unwrapping = 1,
		Unwrapped = 2
	}

	private const float PULL_TAB_RESET_STRENGTH = 10f;

	[SerializeField]
	private WorldItem worldItem;

	[SerializeField]
	private Transform knot;

	[SerializeField]
	private Transform[] ribbonLoops;

	[SerializeField]
	private GrabbableItem[] ribbonPullTabs;

	[SerializeField]
	private Animation[] ribbonWrapAnimations;

	[SerializeField]
	private float ribbonUnwrapPullDistance;

	[SerializeField]
	private AttachableStackPoint lidAttachPoint;

	[SerializeField]
	private AudioClip unwrapSound;

	[SerializeField]
	private AudioClip openSound;

	[SerializeField]
	private BasePrefabSpawner[] spawnPrefabsOnOpen;

	private Vector3[] initialPullTabLocalPos;

	private Quaternion[] initialPullTabLocalRot;

	private float[] pullTabDist;

	private GiftBoxState state;

	private void Awake()
	{
		initialPullTabLocalPos = new Vector3[2];
		initialPullTabLocalRot = new Quaternion[2];
		pullTabDist = new float[2];
		for (int i = 0; i < 2; i++)
		{
			initialPullTabLocalPos[i] = ribbonPullTabs[i].transform.localPosition;
			initialPullTabLocalRot[i] = ribbonPullTabs[i].transform.localRotation;
		}
		state = GiftBoxState.Wrapped;
	}

	private void Update()
	{
		if (state != 0)
		{
			return;
		}
		for (int i = 0; i < 2; i++)
		{
			UpdateRibbonPullTab(i);
			if (state != 0)
			{
				break;
			}
			UpdateRibbonLoop(i);
		}
	}

	private void UpdateRibbonPullTab(int index)
	{
		GrabbableItem grabbableItem = ribbonPullTabs[index];
		Vector3 localPosition = grabbableItem.transform.localPosition;
		pullTabDist[index] = Vector3.Distance(localPosition, initialPullTabLocalPos[index]);
		if (pullTabDist[index] >= ribbonUnwrapPullDistance)
		{
			StartCoroutine(UnwrapAsync());
		}
		if (!grabbableItem.IsCurrInHand)
		{
			if (pullTabDist[index] > 0.001f)
			{
				Quaternion localRotation = grabbableItem.transform.localRotation;
				grabbableItem.transform.localPosition = Vector3.Lerp(localPosition, initialPullTabLocalPos[index], Time.deltaTime * 10f);
				grabbableItem.transform.localRotation = Quaternion.Slerp(localRotation, initialPullTabLocalRot[index], Time.deltaTime * 10f);
			}
			else
			{
				grabbableItem.transform.localPosition = initialPullTabLocalPos[index];
				grabbableItem.transform.localRotation = initialPullTabLocalRot[index];
			}
		}
	}

	private void UpdateRibbonLoop(int index)
	{
		Transform transform = ribbonLoops[index];
		float t = (pullTabDist[0] + pullTabDist[1]) / ribbonUnwrapPullDistance;
		float num = Mathf.Lerp(1f, 0.2f, t);
		transform.localScale = new Vector3(num, num, 1f);
	}

	private IEnumerator UnwrapAsync()
	{
		state = GiftBoxState.Unwrapping;
		if (unwrapSound != null)
		{
			AudioManager.Instance.Play(base.transform.position, unwrapSound, 1f, 1f);
		}
		for (int k = 0; k < 2; k++)
		{
			if (ribbonPullTabs[k].IsCurrInHand)
			{
				ribbonPullTabs[k].CurrInteractableHand.TryRelease();
			}
			ribbonPullTabs[k].enabled = false;
		}
		float knotSize = 1f;
		while (knotSize > 0f)
		{
			knotSize = Mathf.Max(0f, knotSize - Time.deltaTime * 3f);
			knot.localScale = Vector3.one * knotSize;
			yield return null;
		}
		knot.gameObject.SetActive(false);
		for (int j = 0; j < ribbonWrapAnimations.Length; j++)
		{
			ribbonWrapAnimations[j].Play();
			yield return new WaitForSeconds(0.05f);
		}
		yield return new WaitForSeconds(0.2f);
		AttachableObject lid = lidAttachPoint.TopmostAttachedObject;
		if (lid != null)
		{
			lid.PickupableItem.enabled = true;
		}
		lidAttachPoint.itemInteractionMode = AttachableStackPoint.StackItemInteractionMode.TopmostOnly;
		for (int i = 0; i < spawnPrefabsOnOpen.Length; i++)
		{
			spawnPrefabsOnOpen[i].SpawnPrefab();
		}
		yield return new WaitForSeconds(0.1f);
		if (unwrapSound != null)
		{
			AudioManager.Instance.Play(base.transform.position, openSound, 1f, 1f);
		}
		state = GiftBoxState.Unwrapped;
		GameEventsManager.Instance.ItemActionOccurred(worldItem.Data, "OPENED");
	}
}
