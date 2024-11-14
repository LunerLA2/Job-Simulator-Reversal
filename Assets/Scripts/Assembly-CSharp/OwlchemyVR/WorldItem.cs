using System;
using UnityEngine;

namespace OwlchemyVR
{
	[RequireComponent(typeof(AudioImpactController))]
	public class WorldItem : MonoBehaviour
	{
		[SerializeField]
		private WorldItemData worldItemData;

		public Action<WorldItem> OnWorldItemDataWillChange;

		public Action<WorldItem> OnWorldItemDataHasChanged;

		private bool blockNextDisable;

		public WorldItemData Data
		{
			get
			{
				return worldItemData;
			}
		}

		public string ItemName
		{
			get
			{
				return (!(worldItemData != null)) ? string.Empty : worldItemData.ItemName;
			}
		}

		public void ManualSetData(WorldItemData data)
		{
			if (worldItemData != data)
			{
				if (OnWorldItemDataWillChange != null)
				{
					OnWorldItemDataWillChange(this);
				}
				if (worldItemData != null)
				{
					WorldItemTrackingManager.Instance.ItemRemoved(worldItemData, true);
				}
				worldItemData = data;
				if (worldItemData != null)
				{
					WorldItemTrackingManager.Instance.ItemAdded(worldItemData);
				}
				else
				{
					Debug.LogWarning("ManualSetData set a worlditemdata to null, this should never happen", base.gameObject);
				}
				if (OnWorldItemDataHasChanged != null)
				{
					OnWorldItemDataHasChanged(this);
				}
			}
		}

		private void Awake()
		{
		}

		private void OnEnable()
		{
			WorldItemTrackingManager.Instance.ItemAdded(Data);
		}

		private void OnDisable()
		{
			if (blockNextDisable)
			{
				blockNextDisable = false;
			}
			else if (WorldItemTrackingManager.InstanceNoCreate != null)
			{
				WorldItemTrackingManager.InstanceNoCreate.ItemRemoved(Data);
			}
		}

		public void HackDisableWithoutEvent()
		{
			if (WorldItemTrackingManager.InstanceNoCreate != null)
			{
				WorldItemTrackingManager.InstanceNoCreate.ItemRemoved(Data, false, true);
				blockNextDisable = true;
			}
		}
	}
}
