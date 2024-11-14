using OwlchemyVR;
using UnityEngine;

public class CompanionSecurityCam : MonoBehaviour
{
	[HideInInspector]
	public CompanionUIManager uiManager;

	[SerializeField]
	private PickupableItem pickupableItem;

	private bool isApplicationQuitting;

	private void Awake()
	{
		pickupableItem.SetHiddenFromZones(true);
	}

	private void OnDestroy()
	{
		if (uiManager != null && !isApplicationQuitting)
		{
			uiManager.RecreateSecurityCameraModel();
		}
	}

	private void OnApplicationQuit()
	{
		isApplicationQuitting = true;
	}
}
