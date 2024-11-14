using System;
using System.Collections;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class SandwichBuilderController : KitchenTool
{
	[Serializable]
	private class SpecialSandwichRecipe
	{
		[SerializeField]
		private WorldItemData[] ingredientItemDatas;

		[SerializeField]
		private WorldItemData resultItemData;

		public WorldItemData ResultItemData
		{
			get
			{
				return resultItemData;
			}
		}

		public bool IsSatisfiedBy(List<WorldItemData> itemDatas)
		{
			for (int i = 0; i < ingredientItemDatas.Length; i++)
			{
				if (!itemDatas.Contains(ingredientItemDatas[i]))
				{
					return false;
				}
			}
			return true;
		}
	}

	private const float RESET_DURATION = 0.35f;

	[SerializeField]
	private WorldItem myWorldItem;

	[SerializeField]
	private AttachableSandwichPoint attachPoint;

	[SerializeField]
	private Transform rodPivot;

	[SerializeField]
	private Collider rodCollider;

	[SerializeField]
	private RigidbodyEnterExitTriggerEvents sandwichRemovalRegion;

	[SerializeField]
	private SandwichController sandwichPrefab;

	[SerializeField]
	private WorldItemData finisherWorldItemData;

	[SerializeField]
	private AudioClip sandwichFinishedSound;

	[SerializeField]
	private ParticleSystem sandwichFinishedParticles;

	[SerializeField]
	private GameObjectPrefabSpawner[] breadSpawners;

	[SerializeField]
	private SpecialSandwichRecipe[] specialSandwichRecipes;

	private SandwichBuilderState state;

	private SandwichController finishedSandwich;

	private GameObject[] breads;

	private bool didFirstSummon;

	private void Awake()
	{
		breads = new GameObject[breadSpawners.Length];
		for (int i = 0; i < breadSpawners.Length; i++)
		{
			GameObject gameObject = breadSpawners[i].SpawnPrefab();
			gameObject.GetComponent<Rigidbody>().isKinematic = true;
			gameObject.GetComponent<GrabbableItem>().enabled = false;
			breads[i] = gameObject;
		}
	}

	private void Start()
	{
		StartCoroutine(ResetAsync(true));
	}

	public override void OnSummon()
	{
		if (breads != null)
		{
			for (int i = 0; i < breadSpawners.Length; i++)
			{
				GameObject gameObject = breads[i];
				if (gameObject != null)
				{
					gameObject.GetComponent<Rigidbody>().isKinematic = false;
					gameObject.GetComponent<GrabbableItem>().enabled = true;
					if (!didFirstSummon)
					{
						gameObject.transform.SetParent(GlobalStorage.Instance.ContentRoot, true);
					}
				}
			}
			breads = null;
		}
		didFirstSummon = true;
	}

	private void OnEnable()
	{
		attachPoint.OnItemFinishedSliding += ItemFinishedSliding;
		attachPoint.OnObjectWasDetached += ItemDetached;
		attachPoint.OnObjectWasAttached += ItemAttached;
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = sandwichRemovalRegion;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Combine(rigidbodyEnterExitTriggerEvents.OnRigidbodyExitTrigger, new Action<Rigidbody>(SandwichRemoved));
	}

	private void OnDisable()
	{
		attachPoint.OnItemFinishedSliding -= ItemFinishedSliding;
		attachPoint.OnObjectWasDetached -= ItemDetached;
		attachPoint.OnObjectWasAttached -= ItemAttached;
		RigidbodyEnterExitTriggerEvents rigidbodyEnterExitTriggerEvents = sandwichRemovalRegion;
		rigidbodyEnterExitTriggerEvents.OnRigidbodyExitTrigger = (Action<Rigidbody>)Delegate.Remove(rigidbodyEnterExitTriggerEvents.OnRigidbodyExitTrigger, new Action<Rigidbody>(SandwichRemoved));
	}

	private void Update()
	{
		if (state == SandwichBuilderState.Building)
		{
			isToolBusy = attachPoint.IsBusy;
			AttachableObject topmostAttachedObject = attachPoint.TopmostAttachedObject;
			if (topmostAttachedObject != null && topmostAttachedObject.PickupableItem != null && topmostAttachedObject.PickupableItem.IsCurrInHand)
			{
				if (rodCollider.enabled)
				{
					rodCollider.enabled = false;
				}
			}
			else if (!rodCollider.enabled)
			{
				rodCollider.enabled = true;
			}
		}
		else if (state == SandwichBuilderState.Built)
		{
			isToolBusy = false;
			if (finishedSandwich == null)
			{
				StartCoroutine(ResetAsync(false));
			}
		}
		else if (state == SandwichBuilderState.Resetting)
		{
			isToolBusy = true;
		}
	}

	private void ItemDetached(AttachablePoint point, AttachableObject obj)
	{
		GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "DEACTIVATED");
	}

	private void ItemAttached(AttachablePoint point, AttachableObject obj)
	{
		GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "ACTIVATED");
	}

	private void ItemFinishedSliding(AttachableObject o)
	{
		if (attachPoint.NumAttachedObjects >= 2 && o.PickupableItem.InteractableItem.WorldItemData == finisherWorldItemData)
		{
			FinishSandwich();
		}
	}

	private void FinishSandwich()
	{
		if (state != SandwichBuilderState.Building)
		{
			return;
		}
		finishedSandwich = UnityEngine.Object.Instantiate(sandwichPrefab);
		finishedSandwich.gameObject.RemoveCloneFromName();
		finishedSandwich.transform.position = attachPoint.transform.position + new Vector3(0f, attachPoint.SandwichHeight / 2f, 0f);
		finishedSandwich.transform.SetParent(GlobalStorage.Instance.ContentRoot, true);
		List<AttachableObject> list = new List<AttachableObject>();
		list.AddRange(attachPoint.AttachedObjects);
		List<WorldItemData> list2 = new List<WorldItemData>();
		WorldItemData worldItemData = null;
		for (int i = 0; i < list.Count; i++)
		{
			AttachableObject attachableObject = list[i];
			list2.Add(attachableObject.PickupableItem.InteractableItem.WorldItemData);
			RigidbodyRemover component = attachableObject.GetComponent<RigidbodyRemover>();
			if (component != null)
			{
				component.enabled = false;
			}
			attachableObject.Detach(true, true);
			finishedSandwich.AddItem(attachableObject, i == list.Count - 1);
			if (component != null)
			{
				component.enabled = true;
			}
			GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "DEACTIVATED");
		}
		for (int j = 0; j < specialSandwichRecipes.Length; j++)
		{
			if (specialSandwichRecipes[j].IsSatisfiedBy(list2))
			{
				worldItemData = specialSandwichRecipes[j].ResultItemData;
				break;
			}
		}
		finishedSandwich.Finalize(worldItemData);
		AudioManager.Instance.Play(finishedSandwich.transform.position, sandwichFinishedSound, 1f, 1f);
		sandwichFinishedParticles.Play();
		GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "USED");
		attachPoint.GetComponent<Collider>().enabled = false;
		rodPivot.localScale = new Vector3(1f, 0f, 1f);
		state = SandwichBuilderState.Built;
	}

	private IEnumerator ResetAsync(bool isInstant)
	{
		state = SandwichBuilderState.Resetting;
		finishedSandwich = null;
		rodPivot.localScale = new Vector3(1f, 0f, 1f);
		if (!isInstant)
		{
			for (float resetTime = 0f; resetTime < 0.35f; resetTime += Time.deltaTime)
			{
				rodPivot.localScale = new Vector3(1f, resetTime / 0.35f, 1f);
				yield return null;
			}
		}
		rodPivot.localScale = Vector3.one;
		attachPoint.GetComponent<Collider>().enabled = true;
		state = SandwichBuilderState.Building;
	}

	private void SandwichRemoved(Rigidbody rb)
	{
		if (state == SandwichBuilderState.Built && finishedSandwich != null && rb.gameObject == finishedSandwich.gameObject)
		{
			StartCoroutine(ResetAsync(false));
		}
	}
}
