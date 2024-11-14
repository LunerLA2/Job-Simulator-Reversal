using OwlchemyVR;
using UnityEngine;

public class WorldSightArea : BotSightController
{
	[SerializeField]
	private WorldItemData[] itemsToTrack;

	private void Start()
	{
		for (int i = 0; i < itemsToTrack.Length; i++)
		{
			AddItemOfInterest(itemsToTrack[i]);
		}
	}
}
