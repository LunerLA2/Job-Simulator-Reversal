using System.Collections.Generic;
using UnityEngine;

namespace OwlchemyVR
{
	public class PickupableItem : GrabbableItem
	{
		private bool isHiddenFromZones;

		public bool IsHiddenFromZones
		{
			get
			{
				return isHiddenFromZones;
			}
		}

		public override void Awake()
		{
			base.Awake();
			r.maxAngularVelocity = 100f;
			if (!GenieManager.AreAnyJobGenieModesActive())
			{
				return;
			}
			if (GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.NoGravityMode))
			{
				if (r.drag < 1.5f)
				{
					r.drag = 1.5f;
				}
				if (r.angularDrag < 0.5f)
				{
					r.angularDrag = 0.5f;
				}
			}
			if (!GenieManager.DoesContainGenieMode(GlobalStorage.Instance.CurrentGenieModes, JobGenieCartridge.GenieModeTypes.RubberMode))
			{
				return;
			}
			List<Collider> list = new List<Collider>();
			GetComponentsInChildren(true, list);
			for (int i = 0; i < list.Count; i++)
			{
				Collider collider = list[i];
				if (!collider.isTrigger)
				{
					if (collider.sharedMaterial == null)
					{
						collider.sharedMaterial = GenieManager.GetRubberModePhysicMaterial();
					}
					else
					{
						collider.material.bounciness = GenieManager.GetRubberModePhysicMaterial().bounciness;
					}
				}
			}
		}

		public override void Grab(InteractionHandController interactableHand)
		{
			base.Grab(interactableHand);
			if (r != null)
			{
				r.isKinematic = false;
			}
			interactableHand.AddConnectedBody(r, this);
		}

		public override bool Release(InteractionHandController interactableHand, Vector3 applyVelocity, Vector3 applyAngVelocity, bool wasSwappedBetweenHands = false)
		{
			if (base.CurrInteractableHand != interactableHand)
			{
				Debug.LogWarning("Current hand is not the same hand as the one that released, this should not happen");
			}
			interactableHand.RemoveConnectedBody();
			base.Release(interactableHand, applyVelocity, applyAngVelocity, wasSwappedBetweenHands);
			if (!float.IsNaN(applyVelocity.x) && !float.IsNaN(applyVelocity.y) && !float.IsNaN(applyVelocity.z))
			{
				r.velocity = applyVelocity;
			}
			r.angularVelocity = applyAngVelocity;
			return true;
		}

		public void SetHiddenFromZones(bool value)
		{
			isHiddenFromZones = value;
		}
	}
}
