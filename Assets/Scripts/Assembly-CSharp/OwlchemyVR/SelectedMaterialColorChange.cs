using System;
using System.Collections.Generic;
using UnityEngine;

namespace OwlchemyVR
{
	[RequireComponent(typeof(InteractableItem))]
	public class SelectedMaterialColorChange : MonoBehaviour
	{
		[SerializeField]
		private Renderer[] renderers;

		private InteractableItem interactableItem;

		private List<Material> mats;

		private List<Color> defaultColors;

		private void Awake()
		{
			interactableItem = GetComponent<InteractableItem>();
			mats = new List<Material>();
			defaultColors = new List<Color>();
			for (int i = 0; i < renderers.Length; i++)
			{
				Material material = renderers[i].material;
				mats.Add(material);
				defaultColors.Add(material.color);
			}
		}

		private void OnEnable()
		{
			InteractableItem obj = interactableItem;
			obj.OnSelected = (Action<InteractableItem>)Delegate.Combine(obj.OnSelected, new Action<InteractableItem>(ItemSelected));
			InteractableItem obj2 = interactableItem;
			obj2.OnDeselected = (Action<InteractableItem>)Delegate.Combine(obj2.OnDeselected, new Action<InteractableItem>(ItemDeselected));
		}

		private void OnDisable()
		{
			InteractableItem obj = interactableItem;
			obj.OnSelected = (Action<InteractableItem>)Delegate.Remove(obj.OnSelected, new Action<InteractableItem>(ItemSelected));
			InteractableItem obj2 = interactableItem;
			obj2.OnDeselected = (Action<InteractableItem>)Delegate.Remove(obj2.OnDeselected, new Action<InteractableItem>(ItemDeselected));
		}

		private void ItemSelected(InteractableItem item)
		{
			Color highlightColor = GameSettings.Instance.HighlightColor;
			for (int i = 0; i < mats.Count; i++)
			{
				mats[i].color = highlightColor;
			}
		}

		private void ItemDeselected(InteractableItem item)
		{
			for (int i = 0; i < mats.Count; i++)
			{
				mats[i].color = defaultColors[i];
			}
		}
	}
}
