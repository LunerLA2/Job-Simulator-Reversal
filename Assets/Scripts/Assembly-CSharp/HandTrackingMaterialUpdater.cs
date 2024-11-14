using System;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class HandTrackingMaterialUpdater : MonoBehaviour
{
	private const float TRACKING_LOSS_TIME_UNTIL_MATERIAL_CHANGE = 0.25f;

	[SerializeField]
	private Morpheus_IndividualController psvrController;

	[SerializeField]
	private OculusTouch_IndividualController touchController;

	[SerializeField]
	private SteamVRCompatible_IndividualController steamVRCompatibleController;

	[SerializeField]
	private InteractionHandController interactionHandController;

	[SerializeField]
	private Renderer handRenderer;

	[SerializeField]
	private Renderer controllerRenderer;

	private bool isControllerActive;

	[SerializeField]
	private Material fullTrackingMaterial_hand;

	[SerializeField]
	private Material fullTrackingMaterial_controller;

	[SerializeField]
	private Material noTrackingMaterial;

	private List<Material> cachedGrabbedItemMaterials = new List<Material>();

	private float trackingLossTimer = 0.25f;

	private bool isTrackingVisuallyAcceptable = true;

	private bool isWaitingToUpdate;

	private void OnEnable()
	{
		interactionHandController.handVisualsChangedEvent += OnHandVisualChange;
		InteractionHandController obj = interactionHandController;
		obj.OnReleasedGrabbable = (Action<InteractionHandController, GrabbableItem>)Delegate.Combine(obj.OnReleasedGrabbable, new Action<InteractionHandController, GrabbableItem>(OnGrabbableRelease));
		InteractionHandController obj2 = interactionHandController;
		obj2.OnGrabSuccess = (Action<InteractionHandController, GrabbableItem>)Delegate.Combine(obj2.OnGrabSuccess, new Action<InteractionHandController, GrabbableItem>(OnGrabSuccess));
	}

	private void OnDisable()
	{
		interactionHandController.handVisualsChangedEvent -= OnHandVisualChange;
		InteractionHandController obj = interactionHandController;
		obj.OnReleasedGrabbable = (Action<InteractionHandController, GrabbableItem>)Delegate.Remove(obj.OnReleasedGrabbable, new Action<InteractionHandController, GrabbableItem>(OnGrabbableRelease));
		InteractionHandController obj2 = interactionHandController;
		obj2.OnGrabSuccess = (Action<InteractionHandController, GrabbableItem>)Delegate.Remove(obj2.OnGrabSuccess, new Action<InteractionHandController, GrabbableItem>(OnGrabSuccess));
	}

	private void OnHandVisualChange(bool isController)
	{
		isControllerActive = isController;
	}

	private void Update()
	{
		if (VRPlatform.GetCurrVRPlatformType() != VRPlatformTypes.PSVR || psvrController != null)
		{
		}
		if (VRPlatform.GetCurrVRPlatformType() == VRPlatformTypes.Oculus && touchController != null)
		{
			if (touchController.IsControllerTracked())
			{
				if (!isTrackingVisuallyAcceptable)
				{
					isTrackingVisuallyAcceptable = true;
					SetTrackingQualityVisual(isTrackingVisuallyAcceptable);
				}
			}
			else if (isTrackingVisuallyAcceptable)
			{
				isTrackingVisuallyAcceptable = false;
				SetTrackingQualityVisual(isTrackingVisuallyAcceptable);
			}
		}
		if (VRPlatform.GetCurrVRPlatformType() != VRPlatformTypes.SteamVR || (VRPlatform.GetCurrVRPlatformHardwareType() != VRPlatformHardwareType.OculusRift && VRPlatform.GetCurrVRPlatformHardwareType() != VRPlatformHardwareType.WindowsMR))
		{
			return;
		}
		if (steamVRCompatibleController.IsControllerTracked())
		{
			if (!isTrackingVisuallyAcceptable)
			{
				isTrackingVisuallyAcceptable = true;
				SetTrackingQualityVisual(isTrackingVisuallyAcceptable);
			}
		}
		else if (isTrackingVisuallyAcceptable)
		{
			isTrackingVisuallyAcceptable = false;
			SetTrackingQualityVisual(isTrackingVisuallyAcceptable);
		}
	}

	private void SetTrackingQualityVisual(bool isAcceptable)
	{
		if (isAcceptable)
		{
			handRenderer.material = fullTrackingMaterial_hand;
			if (controllerRenderer != null)
			{
				controllerRenderer.material = fullTrackingMaterial_controller;
			}
			if (interactionHandController.IsGrabbableCurrInHand)
			{
				ChangeGrabbableMaterials(interactionHandController.CurrGrabbedItem, true);
			}
		}
		else
		{
			if (controllerRenderer != null)
			{
				controllerRenderer.material = noTrackingMaterial;
			}
			handRenderer.material = noTrackingMaterial;
			if (interactionHandController.IsGrabbableCurrInHand)
			{
				ChangeGrabbableMaterials(interactionHandController.CurrGrabbedItem, false);
			}
		}
	}

	private void ChangeGrabbableMaterials(GrabbableItem item, bool isInTracking)
	{
		if (item == null)
		{
			Debug.LogError("You can't change the grabbable material of a null item!!");
			return;
		}
		SelectedChangeOutlineController component = item.GetComponent<SelectedChangeOutlineController>();
		if (component == null || component.meshRenderers == null || component.meshRenderers.Length == 0 || cachedGrabbedItemMaterials == null || cachedGrabbedItemMaterials.Count == 0)
		{
			return;
		}
		if (isInTracking)
		{
			if (component.meshRenderers.Length == cachedGrabbedItemMaterials.Count)
			{
				for (int i = 0; i < component.meshRenderers.Length; i++)
				{
					if (component == null || component.meshRenderers == null || component.meshRenderers.Length == 0 || cachedGrabbedItemMaterials == null || cachedGrabbedItemMaterials.Count == 0 || component.meshRenderers[i].material == null)
					{
						Debug.Log("A referenced chnaged in the middle of execution! Bailing out of the material change.");
					}
					else
					{
						component.meshRenderers[i].material = cachedGrabbedItemMaterials[i];
					}
				}
			}
			else
			{
				Debug.LogError("SelectedChangeOutlineController MeshRenderers Length did not match of cachedgrabbleItemMaterials:" + item.ToString());
			}
			return;
		}
		cachedGrabbedItemMaterials.Clear();
		for (int j = 0; j < component.meshRenderers.Length; j++)
		{
			if (component == null || component.meshRenderers == null || component.meshRenderers.Length == 0 || cachedGrabbedItemMaterials == null || cachedGrabbedItemMaterials.Count == 0 || component.meshRenderers[j] == null || component.meshRenderers[j].material == null)
			{
				Debug.Log("A referenced chnaged in the middle of execution or a material wasn't present! Bailing out of the material change.");
				continue;
			}
			cachedGrabbedItemMaterials.Add(component.meshRenderers[j].material);
			component.meshRenderers[j].material = noTrackingMaterial;
		}
	}

	private void OnGrabSuccess(InteractionHandController interactionController, GrabbableItem grabbedItem)
	{
		if (!isTrackingVisuallyAcceptable)
		{
			ChangeGrabbableMaterials(grabbedItem, false);
		}
	}

	private void OnGrabbableRelease(InteractionHandController iHandController, GrabbableItem grabbedItem)
	{
		if (!isTrackingVisuallyAcceptable)
		{
			ChangeGrabbableMaterials(grabbedItem, true);
		}
		cachedGrabbedItemMaterials.Clear();
	}
}
