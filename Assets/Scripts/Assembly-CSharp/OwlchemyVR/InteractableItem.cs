using System;
using System.Collections.Generic;
using UnityEngine;

namespace OwlchemyVR
{
	[RequireComponent(typeof(WorldItem))]
	public class InteractableItem : MonoBehaviour
	{
		private WorldItem worldItem;

		public Action<InteractableItem> OnSelected;

		public Action<InteractableItem> OnDeselected;

		private List<InteractionController> currSelectingInteractionControllers = new List<InteractionController>();

		public string ItemName
		{
			get
			{
				if (worldItem != null)
				{
					return worldItem.ItemName;
				}
				return string.Empty;
			}
		}

		public WorldItemData WorldItemData
		{
			get
			{
				if (worldItem != null)
				{
					return worldItem.Data;
				}
				return null;
			}
		}

		public WorldItem WorldItem
		{
			get
			{
				return worldItem;
			}
		}

		public bool IsSelected
		{
			get
			{
				return currSelectingInteractionControllers.Count > 0;
			}
		}

		private void Awake()
		{
			worldItem = GetComponent<WorldItem>();
		}

		public void SelectItem(InteractionController interactionHand)
		{
			if (currSelectingInteractionControllers.Count == 0 || !currSelectingInteractionControllers.Contains(interactionHand))
			{
				currSelectingInteractionControllers.Add(interactionHand);
				if (currSelectingInteractionControllers.Count == 1 && OnSelected != null)
				{
					OnSelected(this);
				}
			}
		}

		public void DeselectItem(InteractionController interactionHand)
		{
			if (currSelectingInteractionControllers.Contains(interactionHand))
			{
				currSelectingInteractionControllers.Remove(interactionHand);
				if (currSelectingInteractionControllers.Count == 0 && OnDeselected != null)
				{
					OnDeselected(this);
				}
			}
		}

		public void ForceUnSelectAll()
		{
			if (currSelectingInteractionControllers.Count > 0)
			{
				for (int i = 0; i < currSelectingInteractionControllers.Count; i++)
				{
					currSelectingInteractionControllers[i].ForceDeselect(this);
				}
				currSelectingInteractionControllers.Clear();
				if (OnDeselected != null)
				{
					OnDeselected(this);
				}
			}
		}

		public void ManualSetWorldItemData(WorldItemData data)
		{
			worldItem.ManualSetData(data);
		}
	}
}
