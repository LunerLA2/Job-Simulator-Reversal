using OwlchemyVR;
using UnityEngine;

public class SimpleLargeHapticPulse : MonoBehaviour
{
	[SerializeField]
	private GrabbableItem grabbableItem;

	private HapticInfoObject hapticObject;

	private void Awake()
	{
		hapticObject = new HapticInfoObject(800f, 0.2f);
		hapticObject.DeactiveHaptic();
	}

	public void DoHaptic()
	{
		if (grabbableItem.IsCurrInHand)
		{
			hapticObject.Restart();
			if (!grabbableItem.CurrInteractableHand.HapticsController.ContainHaptic(hapticObject))
			{
				grabbableItem.CurrInteractableHand.HapticsController.AddNewHaptic(hapticObject);
			}
		}
	}
}
