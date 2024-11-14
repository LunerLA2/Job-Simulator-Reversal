using UnityEngine;

public class HotDogRollerTool : KitchenTool
{
	[SerializeField]
	private GrabbableSlider bunDrawer;

	public override void OnDismiss()
	{
		if (!bunDrawer.IsLowerLocked)
		{
			bunDrawer.LockLower();
		}
		base.OnDismiss();
	}
}
