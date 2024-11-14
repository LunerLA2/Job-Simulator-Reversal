using System;
using UnityEngine;

namespace OwlchemyVR
{
	public class InteractionController : MonoBehaviour
	{
		public Action<InteractableItem> OnForceDeselect;

		public void ForceDeselect(InteractableItem item)
		{
			if (OnForceDeselect != null)
			{
				OnForceDeselect(item);
			}
		}
	}
}
