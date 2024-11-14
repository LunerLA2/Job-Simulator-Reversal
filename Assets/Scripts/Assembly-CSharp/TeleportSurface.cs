using System.Collections.Generic;
using UnityEngine;

public class TeleportSurface : MonoBehaviour
{
	private List<TeleportPortal> portalsOnSurface = new List<TeleportPortal>();

	private void SomethingEntered(Collider c)
	{
		if (c.attachedRigidbody != null)
		{
			TeleportPortal component = c.attachedRigidbody.GetComponent<TeleportPortal>();
			if (component != null && !portalsOnSurface.Contains(component))
			{
				portalsOnSurface.Add(component);
				CheckIfTeleportIsPossible();
			}
		}
	}

	private void SomethingExited(Collider c)
	{
		if (c.attachedRigidbody != null)
		{
			TeleportPortal component = c.attachedRigidbody.GetComponent<TeleportPortal>();
			if (component != null && portalsOnSurface.Contains(component))
			{
				portalsOnSurface.Remove(component);
				CheckIfTeleportIsPossible();
			}
		}
	}

	private void CheckIfTeleportIsPossible()
	{
		if (portalsOnSurface.Count == 2 && portalsOnSurface[0].IsFacingDown() && portalsOnSurface[1].IsFacingDown())
		{
			Debug.Log("doing teleport");
			portalsOnSurface[0].TeleportItemsTo(portalsOnSurface[1]);
			portalsOnSurface[1].TeleportItemsTo(portalsOnSurface[0]);
		}
	}
}
