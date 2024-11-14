using System;
using System.Collections;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class MailCartController : MonoBehaviour
{
	[SerializeField]
	private Transform cartMainTransform;

	[SerializeField]
	private MeshRenderer[] cartRenderers;

	[SerializeField]
	private Material matForGameDevJob;

	[SerializeField]
	private float defaultInDelay = 4f;

	[SerializeField]
	private float defaultOutDelay = 2f;

	[SerializeField]
	private ItemCollectionZone[] collectionZonesToLockDown;

	private List<PickupableItem> pickupablesInCollectionZone = new List<PickupableItem>();

	private List<Rigidbody> rbsToSetToNonKinematic = new List<Rigidbody>();

	[SerializeField]
	private Animation cartAnimation;

	[SerializeField]
	private AnimationClip cartAnimateIn;

	[SerializeField]
	private AnimationClip cartAnimateDunkLoop;

	[SerializeField]
	private AnimationClip cartAnimateOut;

	[SerializeField]
	private AnimationClip cartAnimateOutLeft;

	private TaskData currentTask;

	[SerializeField]
	private AudioSourceHelper audioSource;

	[SerializeField]
	private AudioClip cartMovingLoop;

	[SerializeField]
	private AudioClip cartStopAudio;

	[SerializeField]
	private AudioClip cartStartAudio;

	[SerializeField]
	private MailCartLoadInfo[] cartLoads;

	private MailCartLoadInfo[] currentLoadInfos;

	private List<Transform> cartParentedTransforms = new List<Transform>();

	private bool completed;

	private float queuedOutDelay;

	private bool queuedExitLeft;

	private GameObject loadStorage;

	public TaskData CurrentTask
	{
		get
		{
			return currentTask;
		}
	}

	private void Awake()
	{
		InitSpawnersWhileDisabled();
	}

	private void Start()
	{
		bool flag = false;
		if (OfficeManager.Instance.AlwaysLoadGameDevJob)
		{
			flag = true;
		}
		if (GenieManager.AreAnyJobGenieModesActive() && GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.OfficeModMode))
		{
			flag = true;
		}
		if (flag)
		{
			for (int i = 0; i < cartRenderers.Length; i++)
			{
				cartRenderers[i].sharedMaterial = matForGameDevJob;
			}
		}
	}

	private void InitSpawnersWhileDisabled()
	{
		loadStorage = new GameObject("MailCartLoadStorage");
		loadStorage.transform.SetParent(GlobalStorage.Instance.ContentRoot, false);
		if (cartLoads == null)
		{
			return;
		}
		for (int i = 0; i < cartLoads.Length; i++)
		{
			if (cartLoads[i].Load != null)
			{
				cartLoads[i].Load.SetActive(true);
				cartLoads[i].Load.SetActive(false);
				if (cartLoads[i].defaultParent == null)
				{
					cartLoads[i].defaultParent = cartLoads[i].Load.transform.parent;
				}
			}
		}
		for (int j = 0; j < cartLoads.Length; j++)
		{
			if (cartLoads[j].Load != null)
			{
				cartLoads[j].Load.transform.SetParent(loadStorage.transform, false);
			}
		}
	}

	public bool HasLoadForTask(TaskData task)
	{
		MailCartLoadInfo[] cartLoadInfosForTask = GetCartLoadInfosForTask(task);
		if (cartLoadInfosForTask == null || cartLoadInfosForTask.Length == 0)
		{
			return false;
		}
		return true;
	}

	public bool NeedsToTakeTaskAwayAfterPage(PageData page)
	{
		if (currentLoadInfos != null && currentLoadInfos.Length > 0)
		{
			for (int i = 0; i < currentLoadInfos.Length; i++)
			{
				if (Array.IndexOf(currentLoadInfos[i].ClearAfterPages, page) >= 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool NeedsToTakeTaskAwayOnTaskStart(TaskData task)
	{
		if (currentLoadInfos != null && currentLoadInfos.Length > 0)
		{
			for (int i = 0; i < currentLoadInfos.Length; i++)
			{
				if (task == currentLoadInfos[i].ClearOnTaskStart)
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool NeedsToTakeTaskAway(TaskData task)
	{
		MailCartLoadInfo[] cartLoadInfosForTask = GetCartLoadInfosForTask(task);
		if (cartLoadInfosForTask != null && cartLoadInfosForTask.Length > 0)
		{
			for (int i = 0; i < cartLoadInfosForTask.Length; i++)
			{
				if (cartLoadInfosForTask[i].AlwaysTakeLoadAwayAsSoonAsFinished)
				{
					return true;
				}
			}
		}
		return false;
	}

	public void BringInCart(TaskData _currentTask)
	{
		StopAllCoroutines();
		completed = false;
		base.gameObject.SetActive(true);
		cartAnimation[cartAnimateIn.name].enabled = true;
		cartAnimation[cartAnimateIn.name].weight = 1f;
		cartAnimation[cartAnimateIn.name].normalizedTime = 0f;
		cartAnimation.Sample();
		cartAnimation[cartAnimateIn.name].enabled = false;
		currentTask = _currentTask;
		StartCoroutine(BringInCartInternal(currentTask));
	}

	private IEnumerator BringInCartInternal(TaskData task)
	{
		currentLoadInfos = GetCartLoadInfosForTask(task);
		bool useCartVisuals = true;
		float extraDelay = 0f;
		queuedOutDelay = 0f;
		queuedExitLeft = false;
		if (currentLoadInfos != null)
		{
			for (int l = 0; l < currentLoadInfos.Length; l++)
			{
				if (!currentLoadInfos[l].UseCartVisuals)
				{
					useCartVisuals = false;
				}
				extraDelay += currentLoadInfos[l].ExtraDelayBeforeSendingIn;
				queuedOutDelay += currentLoadInfos[l].ExtraDelayBeforeSendingOut;
				if (currentLoadInfos[l].ExitToLeft)
				{
					queuedExitLeft = true;
				}
			}
		}
		yield return new WaitForSeconds(defaultInDelay);
		cartMainTransform.gameObject.SetActive(useCartVisuals);
		if (currentLoadInfos != null)
		{
			for (int k = 0; k < currentLoadInfos.Length; k++)
			{
				currentLoadInfos[k].Load.transform.SetParent(currentLoadInfos[k].defaultParent, false);
				currentLoadInfos[k].Load.SetActive(true);
			}
		}
		yield return new WaitForSeconds(extraDelay);
		rbsToSetToNonKinematic.Clear();
		for (int z = 0; z < collectionZonesToLockDown.Length; z++)
		{
			ItemCollectionZone zone = collectionZonesToLockDown[z];
			if (!zone.gameObject.activeInHierarchy)
			{
				continue;
			}
			for (int j = 0; j < zone.ItemsInCollection.Count; j++)
			{
				PickupableItem p = zone.ItemsInCollection[j];
				if (p != null && !p.IsCurrInHand)
				{
					Rigidbody r = p.Rigidbody;
					if (r != null && !r.isKinematic)
					{
						rbsToSetToNonKinematic.Add(r);
						r.isKinematic = true;
					}
				}
			}
		}
		audioSource.SetClip(cartMovingLoop);
		audioSource.SetLooping(true);
		audioSource.Play();
		cartAnimation.clip = cartAnimateIn;
		cartAnimation.Play();
		yield return new WaitForSeconds(cartAnimateIn.length * 0.3f);
		if (task.name == "TASK_GetDunkedON")
		{
			MonoBehaviour.print("this is a dunk task!");
			cartAnimation.CrossFade(cartAnimateDunkLoop.name, 0.8f);
		}
		yield return new WaitForSeconds(cartAnimateIn.length * 0.7f);
		for (int i = 0; i < rbsToSetToNonKinematic.Count; i++)
		{
			if (rbsToSetToNonKinematic[i] != null)
			{
				rbsToSetToNonKinematic[i].isKinematic = false;
			}
		}
		if (cartStopAudio != null)
		{
			audioSource.SetClip(cartStopAudio);
			audioSource.SetLooping(false);
			audioSource.Play();
		}
		else
		{
			audioSource.SetLooping(false);
		}
		UnparentLoad();
	}

	private void UnparentLoad()
	{
		if (currentLoadInfos == null)
		{
			return;
		}
		for (int i = 0; i < currentLoadInfos.Length; i++)
		{
			bool flag = true;
			if (i < currentLoadInfos.Length)
			{
				flag = currentLoadInfos[i].UnparentLoadWhenItArrives;
			}
			if (flag)
			{
				currentLoadInfos[i].Load.transform.SetParent(GlobalStorage.Instance.ContentRoot, true);
			}
		}
	}

	private MailCartLoadInfo[] GetCartLoadInfosForTask(TaskData taskData)
	{
		List<MailCartLoadInfo> list = new List<MailCartLoadInfo>();
		for (int i = 0; i < cartLoads.Length; i++)
		{
			if (cartLoads[i].Task == taskData)
			{
				list.Add(cartLoads[i]);
			}
		}
		return list.ToArray();
	}

	public void SendCartAway()
	{
		if (!completed)
		{
			completed = true;
			StopAllCoroutines();
			StartCoroutine(SendCartAwayInternal());
		}
	}

	private IEnumerator SendCartAwayInternal()
	{
		yield return new WaitForSeconds(defaultOutDelay + queuedOutDelay);
		StartCoroutine(SendCartAwayAudio());
		pickupablesInCollectionZone.Clear();
		for (int i = 0; i < collectionZonesToLockDown.Length; i++)
		{
			if (collectionZonesToLockDown[i].gameObject.activeInHierarchy)
			{
				PrepareZoneToBeTakenAway(collectionZonesToLockDown[i]);
			}
		}
		cartAnimation.CrossFade((!queuedExitLeft) ? cartAnimateOut.name : cartAnimateOutLeft.name, 0.5f);
		yield return new WaitForSeconds(cartAnimateOut.length);
		audioSource.Stop();
		WipeCart();
	}

	private IEnumerator SendCartAwayAudio()
	{
		if (cartStartAudio != null)
		{
			audioSource.SetClip(cartStartAudio);
			audioSource.SetLooping(false);
			audioSource.Play();
			yield return new WaitForSeconds(cartStartAudio.length);
		}
		else
		{
			audioSource.SetLooping(false);
		}
		audioSource.SetClip(cartMovingLoop);
		audioSource.SetLooping(true);
		audioSource.Play();
	}

	private void PrepareZoneToBeTakenAway(ItemCollectionZone zone)
	{
		for (int i = 0; i < zone.ItemsInCollection.Count; i++)
		{
			PickupableItem pickupableItem = zone.ItemsInCollection[i];
			pickupablesInCollectionZone.Add(pickupableItem);
			if (!(pickupableItem != null) || pickupableItem.IsCurrInHand)
			{
				continue;
			}
			cartParentedTransforms.Add(pickupableItem.transform);
			pickupableItem.enabled = false;
			pickupableItem.transform.SetParent(cartAnimation.transform, true);
			Rigidbody rigidbody = pickupableItem.Rigidbody;
			if (rigidbody != null)
			{
				ConstantForce component = rigidbody.gameObject.GetComponent<ConstantForce>();
				if (component != null)
				{
					UnityEngine.Object.Destroy(component);
				}
				UnityEngine.Object.Destroy(rigidbody);
			}
		}
	}

	private void WipeCart()
	{
		if (currentLoadInfos != null)
		{
			for (int i = 0; i < currentLoadInfos.Length; i++)
			{
				bool flag = false;
				currentLoadInfos[i].HingeLoweLockWhenLeaving();
				if (i < currentLoadInfos.Length)
				{
					flag = !currentLoadInfos[i].UnparentLoadWhenItArrives;
				}
				if (flag)
				{
					currentLoadInfos[i].Load.SetActive(false);
				}
			}
		}
		for (int j = 0; j < pickupablesInCollectionZone.Count; j++)
		{
			if (pickupablesInCollectionZone[j] != null)
			{
				UnityEngine.Object.Destroy(pickupablesInCollectionZone[j].gameObject);
			}
		}
		for (int k = 0; k < cartParentedTransforms.Count; k++)
		{
			if (cartParentedTransforms[k] != null)
			{
				UnityEngine.Object.Destroy(cartParentedTransforms[k].gameObject);
			}
		}
		base.gameObject.SetActive(false);
	}
}
