using System;
using OwlchemyVR;
using UnityEngine;

public class ToolChooserOverrideKey : MonoBehaviour
{
	[SerializeField]
	private AttachablePoint keyHole;

	[SerializeField]
	private WorldItemData keyWorldItemData;

	[SerializeField]
	private KitchenToolStasher toolChooser;

	[SerializeField]
	private int overrideIndex;

	private int indexBeforeOverride;

	[SerializeField]
	private AudioClip keyForceEject;

	[SerializeField]
	private GameObject[] objsToShowOnAttach;

	[SerializeField]
	private GameObject[] objsToShowOnDetach;

	[SerializeField]
	private ItemRecyclerManager recycleManager;

	private RecycleInfo keyRecycleInfo;

	private void OnEnable()
	{
		keyHole.OnObjectWasAttached += KeyAttached;
		keyHole.OnObjectWasDetached += KeyDetached;
		KitchenToolStasher kitchenToolStasher = toolChooser;
		kitchenToolStasher.OnWillSwitchToToolIndex = (Action<int>)Delegate.Combine(kitchenToolStasher.OnWillSwitchToToolIndex, new Action<int>(ChooserSwitchingToIndex));
	}

	private void OnDisable()
	{
		keyHole.OnObjectWasAttached -= KeyAttached;
		keyHole.OnObjectWasDetached -= KeyDetached;
		KitchenToolStasher kitchenToolStasher = toolChooser;
		kitchenToolStasher.OnWillSwitchToToolIndex = (Action<int>)Delegate.Remove(kitchenToolStasher.OnWillSwitchToToolIndex, new Action<int>(ChooserSwitchingToIndex));
	}

	private void Awake()
	{
		SetVisibleStateOfObjects(objsToShowOnAttach, false);
		SetVisibleStateOfObjects(objsToShowOnDetach, true);
	}

	private void KeyAttached(AttachablePoint attachPoint, AttachableObject attachedObject)
	{
		toolChooser.RequestModeChange(overrideIndex);
		SetVisibleStateOfObjects(objsToShowOnAttach, true);
		SetVisibleStateOfObjects(objsToShowOnDetach, false);
	}

	private void KeyDetached(AttachablePoint attachPoint, AttachableObject detachedObject)
	{
		toolChooser.RequestModeChange(indexBeforeOverride);
		SetVisibleStateOfObjects(objsToShowOnAttach, false);
		SetVisibleStateOfObjects(objsToShowOnDetach, true);
	}

	private void ChooserSwitchingToIndex(int index)
	{
		if (index != overrideIndex)
		{
			indexBeforeOverride = index;
			PhysicsEjectKey();
		}
	}

	private void PhysicsEjectKey()
	{
		AttachableObject attachedObject = keyHole.GetAttachedObject(0);
		if (attachedObject == null)
		{
			return;
		}
		attachedObject.Detach();
		for (int i = 0; i < recycleManager.RecyclingData.RecycleInfos.Count; i++)
		{
			if (recycleManager.RecyclingData.RecycleInfos[i].WorldItemsToMonitor[0] == keyWorldItemData)
			{
				keyRecycleInfo = recycleManager.RecyclingData.RecycleInfos[i];
			}
		}
		UnityEngine.Object.Destroy(attachedObject.gameObject);
		recycleManager.RespawnOneItem(keyRecycleInfo);
	}

	private void SetVisibleStateOfObjects(GameObject[] objects, bool state)
	{
		for (int i = 0; i < objects.Length; i++)
		{
			objects[i].SetActive(state);
		}
	}
}
