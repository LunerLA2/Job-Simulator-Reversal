using OwlchemyVR;
using UnityEngine;

public class CleanupZoneTrackedItem
{
	public PickupableItem PickupableItem { get; private set; }

	public float TimeEnteredCleanupZone { get; private set; }

	public bool HasTimerFinished { get; private set; }

	public CleanupZoneTrackedItem(PickupableItem pickupable)
	{
		PickupableItem = pickupable;
		TimeEnteredCleanupZone = Time.realtimeSinceStartup;
		HasTimerFinished = false;
	}

	public void FinishTimer()
	{
		if (!HasTimerFinished)
		{
			HasTimerFinished = true;
		}
		else
		{
			Debug.LogError("Timer for " + PickupableItem.gameObject.name + " finished more than once!", PickupableItem.gameObject);
		}
	}
}
