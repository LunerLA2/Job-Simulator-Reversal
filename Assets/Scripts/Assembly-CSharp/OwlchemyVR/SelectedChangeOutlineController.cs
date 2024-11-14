using System;
using UnityEngine;

namespace OwlchemyVR
{
	[RequireComponent(typeof(InteractableItem))]
	public class SelectedChangeOutlineController : MonoBehaviour
	{
		public MeshFilter[] meshFilters;

		public MeshRenderer[] meshRenderers;

		private InteractableItem interactableItem;

		[SerializeField]
		private ModelOutlineContainer[] modelOutlineContainer;

		[SerializeField]
		private bool delayedBuild;

		private void Awake()
		{
			interactableItem = GetComponent<InteractableItem>();
			if (meshRenderers != null && meshFilters != null && meshRenderers.Length != meshFilters.Length)
			{
				Debug.LogWarning("Getting Mesh Renderer's references - this should not be happening, run Tool/CacheGrabbableMaterial to avoid this:" + base.gameObject.name);
				meshRenderers = new MeshRenderer[meshFilters.Length];
				for (int i = 0; i < meshFilters.Length; i++)
				{
					if (!(meshFilters[i] == null))
					{
						meshRenderers[i] = meshFilters[i].GetComponent<MeshRenderer>();
					}
				}
			}
			if (!delayedBuild)
			{
				Build();
			}
		}

		public void Build()
		{
			if (this.modelOutlineContainer == null || this.modelOutlineContainer.Length == 0)
			{
				if (meshFilters != null)
				{
					this.modelOutlineContainer = new ModelOutlineContainer[meshFilters.Length];
					for (int i = 0; i < meshFilters.Length; i++)
					{
						MeshFilter meshFilter = meshFilters[i];
						ModelOutlineContainer modelOutlineContainer = UnityEngine.Object.Instantiate(GameSettings.Instance.ModelOutlinePrefab);
						modelOutlineContainer.gameObject.layer = meshFilter.gameObject.layer;
						modelOutlineContainer.transform.SetParent(meshFilter.transform, false);
						modelOutlineContainer.SetMesh(meshFilter.mesh);
						this.modelOutlineContainer[i] = modelOutlineContainer;
					}
				}
			}
			else
			{
				ItemDeselected(interactableItem);
			}
		}

		public void ForceRefreshMeshes()
		{
			if (modelOutlineContainer != null && meshFilters.Length == modelOutlineContainer.Length)
			{
				for (int i = 0; i < meshFilters.Length; i++)
				{
					modelOutlineContainer[i].SetMesh(meshFilters[i].mesh);
				}
			}
			if (interactableItem != null)
			{
				ItemDeselected(interactableItem);
			}
		}

		public void ForceConnectionToInteractableItem(InteractableItem _interactableItem)
		{
			InteractableItem obj = interactableItem;
			obj.OnSelected = (Action<InteractableItem>)Delegate.Remove(obj.OnSelected, new Action<InteractableItem>(ItemSelected));
			InteractableItem obj2 = interactableItem;
			obj2.OnDeselected = (Action<InteractableItem>)Delegate.Remove(obj2.OnDeselected, new Action<InteractableItem>(ItemDeselected));
			interactableItem = _interactableItem;
			InteractableItem obj3 = interactableItem;
			obj3.OnSelected = (Action<InteractableItem>)Delegate.Combine(obj3.OnSelected, new Action<InteractableItem>(ItemSelected));
			InteractableItem obj4 = interactableItem;
			obj4.OnDeselected = (Action<InteractableItem>)Delegate.Combine(obj4.OnDeselected, new Action<InteractableItem>(ItemDeselected));
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
			ItemDeselected(null);
		}

		private void ItemSelected(InteractableItem item)
		{
			SetModelOutlineControllerEnabled(true);
		}

		private void ItemDeselected(InteractableItem item)
		{
			SetModelOutlineControllerEnabled(false);
		}

		private void SetModelOutlineControllerEnabled(bool isEnabled)
		{
			for (int i = 0; i < modelOutlineContainer.Length; i++)
			{
				modelOutlineContainer[i].SetEnableRenderer(isEnabled);
			}
		}

		public void SetSpecialHighlight(bool isSpecial)
		{
			for (int i = 0; i < modelOutlineContainer.Length; i++)
			{
				modelOutlineContainer[i].SetSpecialHighlight(isSpecial);
			}
		}

		public void ForceUpdateItemSelected()
		{
			SetModelOutlineControllerEnabled(interactableItem.IsSelected);
		}
	}
}
