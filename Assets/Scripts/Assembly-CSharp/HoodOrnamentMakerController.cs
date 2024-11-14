using System;
using System.Collections;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class HoodOrnamentMakerController : MonoBehaviour
{
	[SerializeField]
	private bool doShake;

	[SerializeField]
	private ItemCollectionZone modelZone;

	[SerializeField]
	private Transform modelBase;

	[SerializeField]
	private GrabbableSlider doorSlider;

	[SerializeField]
	private Replica hoodOrnamentPrefab;

	[SerializeField]
	private Transform shakeTransform;

	[SerializeField]
	private float fabricationDuration = 1.5f;

	[SerializeField]
	private GameObject particleSystemsRoot;

	private WorldItem myWorldItem;

	private ParticleSystem[] particleSystems;

	[SerializeField]
	private AudioClip compactorOp;

	[SerializeField]
	private AudioSourceHelper compactorSourceHelper;

	private void Awake()
	{
		myWorldItem = GetComponent<WorldItem>();
	}

	private void Start()
	{
		if (particleSystemsRoot != null)
		{
			particleSystems = new ParticleSystem[particleSystemsRoot.transform.childCount];
			for (int i = 0; i < particleSystems.Length; i++)
			{
				particleSystems[i] = particleSystemsRoot.transform.GetChild(i).GetComponent<ParticleSystem>();
			}
		}
	}

	private void OnEnable()
	{
		doorSlider.OnLowerLocked += Fabricate;
		ItemCollectionZone itemCollectionZone = modelZone;
		itemCollectionZone.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(itemCollectionZone.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(ItemEnteredModelZone));
		ItemCollectionZone itemCollectionZone2 = modelZone;
		itemCollectionZone2.OnItemsInCollectionRemoved = (Action<ItemCollectionZone, PickupableItem>)Delegate.Combine(itemCollectionZone2.OnItemsInCollectionRemoved, new Action<ItemCollectionZone, PickupableItem>(ItemExitedModelZone));
	}

	private void OnDisable()
	{
		doorSlider.OnLowerLocked -= Fabricate;
		ItemCollectionZone itemCollectionZone = modelZone;
		itemCollectionZone.OnItemsInCollectionAdded = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(itemCollectionZone.OnItemsInCollectionAdded, new Action<ItemCollectionZone, PickupableItem>(ItemEnteredModelZone));
		ItemCollectionZone itemCollectionZone2 = modelZone;
		itemCollectionZone2.OnItemsInCollectionRemoved = (Action<ItemCollectionZone, PickupableItem>)Delegate.Remove(itemCollectionZone2.OnItemsInCollectionRemoved, new Action<ItemCollectionZone, PickupableItem>(ItemExitedModelZone));
	}

	private void Fabricate(GrabbableSlider door, bool isInitial)
	{
		if (!isInitial && modelZone.ItemsInCollection.Count != 0 && myWorldItem != null && PerformFabricate())
		{
			GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "USED");
			StartCoroutine(ShakeRoutine());
		}
	}

	private void ItemEnteredModelZone(ItemCollectionZone zone, PickupableItem item)
	{
		WorldItem component = item.GetComponent<WorldItem>();
		if (component != null)
		{
			GameEventsManager.Instance.ItemAppliedToItemActionOccurred(component.Data, myWorldItem.Data, "ATTACHED_TO");
			GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "ACTIVATED");
		}
	}

	private void ItemExitedModelZone(ItemCollectionZone zone, PickupableItem item)
	{
		WorldItem component = item.GetComponent<WorldItem>();
		if (component != null)
		{
			GameEventsManager.Instance.ItemAppliedToItemActionOccurred(component.Data, myWorldItem.Data, "DEATTACHED_FROM");
			GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "DEACTIVATED");
		}
	}

	private bool PerformFabricate()
	{
		bool result = false;
		List<PickupableItem> list = new List<PickupableItem>();
		for (int i = 0; i < modelZone.ItemsInCollection.Count; i++)
		{
			PickupableItem pickupableItem = modelZone.ItemsInCollection[i];
			if (!(pickupableItem == null) && !(pickupableItem.GetComponent<Replica>() != null))
			{
				list.Add(pickupableItem);
				WorldItemData worldItemData = pickupableItem.InteractableItem.WorldItemData;
				GameEventsManager.Instance.ItemAppliedToItemActionOccurred(worldItemData, myWorldItem.Data, "DESTROYED_BY");
				GameEventsManager.Instance.ItemActionOccurred(worldItemData, "DESTROYED");
				GameEventsManager.Instance.ItemAppliedToItemActionOccurred(worldItemData, myWorldItem.Data, "ADDED_TO");
			}
		}
		if (list.Count > 0)
		{
			result = true;
		}
		for (int j = 0; j < list.Count; j++)
		{
			MeshRenderer[] componentsInChildren = list[j].GetComponentsInChildren<MeshRenderer>();
			Bounds bounds = default(Bounds);
			for (int k = 0; k < componentsInChildren.Length; k++)
			{
				MeshRenderer meshRenderer = componentsInChildren[k];
				if (meshRenderer == null)
				{
					continue;
				}
				GameObject gameObject = meshRenderer.gameObject;
				if (meshRenderer.enabled && gameObject.activeInHierarchy && gameObject.layer != 17)
				{
					if (k == 0)
					{
						bounds = meshRenderer.bounds;
					}
					else
					{
						bounds.Encapsulate(meshRenderer.bounds);
					}
				}
			}
			Vector3 position = bounds.center - Vector3.up * bounds.size.y / 2f;
			if (position.y <= 0.5f)
			{
				position = list[j].transform.position;
			}
			Replica replica = UnityEngine.Object.Instantiate(hoodOrnamentPrefab, position, Quaternion.identity) as Replica;
			replica.gameObject.RemoveCloneFromName();
			replica.transform.SetParent(GlobalStorage.Instance.ContentRoot, true);
			replica.transform.position = position;
			replica.CopyModels(modelBase, new PickupableItem[1] { list[j] });
			UnityEngine.Object.Destroy(list[j].gameObject);
		}
		return result;
	}

	private void StartParticles()
	{
		for (int i = 0; i < particleSystems.Length; i++)
		{
			particleSystems[i].Play();
		}
	}

	private void StopParticles()
	{
		for (int i = 0; i < particleSystems.Length; i++)
		{
			particleSystems[i].Stop();
		}
	}

	private IEnumerator ShakeRoutine()
	{
		doorSlider.Grabbable.enabled = false;
		float time = 0f;
		float initShakeX = shakeTransform.localPosition.x;
		float newShakeX = initShakeX;
		compactorSourceHelper.SetClip(compactorOp);
		compactorSourceHelper.Play();
		StartParticles();
		while (time < fabricationDuration)
		{
			newShakeX = initShakeX + Mathf.Sin(time * 100f) / 200f;
			if (doShake)
			{
				shakeTransform.localPosition = new Vector3(newShakeX, shakeTransform.localPosition.y, shakeTransform.localPosition.z);
			}
			time += Time.deltaTime;
			yield return null;
		}
		StopParticles();
		shakeTransform.localPosition = new Vector3(initShakeX, shakeTransform.localPosition.y, shakeTransform.localPosition.z);
		doorSlider.Grabbable.enabled = true;
	}
}
