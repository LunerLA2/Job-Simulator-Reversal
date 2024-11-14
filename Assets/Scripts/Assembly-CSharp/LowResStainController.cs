using System;
using UnityEngine;

public class LowResStainController : MonoBehaviour
{
	private FloorStainController stainController;

	public void Init(FloorStainController stain)
	{
		stainController = stain;
	}

	private void OnEnable()
	{
		if (stainController != null)
		{
			FloorStainController floorStainController = stainController;
			floorStainController.OnDeactivate = (Action)Delegate.Combine(floorStainController.OnDeactivate, new Action(OnMainStainDeactivated));
		}
	}

	private void OnDisable()
	{
		FloorStainController floorStainController = stainController;
		floorStainController.OnDeactivate = (Action)Delegate.Remove(floorStainController.OnDeactivate, new Action(OnMainStainDeactivated));
	}

	private void OnMainStainDeactivated()
	{
		base.gameObject.SetActive(false);
	}
}
