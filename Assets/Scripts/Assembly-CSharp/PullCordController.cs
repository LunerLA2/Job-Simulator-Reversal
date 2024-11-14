using System;
using OwlchemyVR;
using UnityEngine;

public class PullCordController : MonoBehaviour
{
	[SerializeField]
	private Transform transformToTrack;

	[SerializeField]
	private float worldYToActivate;

	[SerializeField]
	private float worldYToDeactivate;

	[SerializeField]
	private WorldItem myWorldItem;

	public Action OnActivated;

	public Action OnDeactivated;

	private float lastYPos;

	private void Awake()
	{
		lastYPos = transformToTrack.position.y;
	}

	private void Update()
	{
		bool flag = transformToTrack.position.y <= worldYToActivate;
		bool flag2 = lastYPos <= worldYToActivate;
		bool flag3 = transformToTrack.position.y >= worldYToDeactivate;
		bool flag4 = lastYPos >= worldYToDeactivate;
		if (!flag2 && flag)
		{
			GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "ACTIVATED");
			if (OnActivated != null)
			{
				OnActivated();
			}
		}
		else if (!flag4 && flag3)
		{
			GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "DEACTIVATED");
			if (OnDeactivated != null)
			{
				OnDeactivated();
			}
		}
		lastYPos = transformToTrack.position.y;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		if (transformToTrack != null)
		{
			Gizmos.DrawWireSphere(transformToTrack.position, 0.1f);
			Gizmos.DrawWireCube(new Vector3(transformToTrack.position.x, worldYToActivate, transformToTrack.position.z), new Vector3(0.5f, 0f, 0.5f));
			Gizmos.DrawWireCube(new Vector3(transformToTrack.position.x, worldYToDeactivate, transformToTrack.position.z), new Vector3(0.5f, 0f, 0.5f));
		}
	}
}
