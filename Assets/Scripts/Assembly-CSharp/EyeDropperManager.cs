using System;
using OwlchemyVR;
using UnityEngine;

public class EyeDropperManager : MonoBehaviour
{
	private float fuel;

	public Transform topOfDropperContent;

	public Transform tipOfDropperContent;

	[SerializeField]
	private SimpleDrawing simpleDrawing;

	[SerializeField]
	private MeshRenderer dropperContent1;

	[SerializeField]
	private MeshRenderer dropperContent2;

	[SerializeField]
	private ParticleSystem drops;

	[SerializeField]
	private PickupableItem pickupableItem;

	private ChemicalContainerController currentChemicalController;

	[SerializeField]
	private Animation myAnimation;

	[SerializeField]
	private LineRenderer lr;

	private bool gripDownLastFrame;

	private void OnEnable()
	{
		PickupableItem obj = pickupableItem;
		obj.OnGrabbedUpdate = (Action<GrabbableItem>)Delegate.Combine(obj.OnGrabbedUpdate, new Action<GrabbableItem>(GrabbedUpdate));
	}

	private void OnDisable()
	{
		PickupableItem obj = pickupableItem;
		obj.OnGrabbedUpdate = (Action<GrabbableItem>)Delegate.Remove(obj.OnGrabbedUpdate, new Action<GrabbableItem>(GrabbedUpdate));
	}

	private void GrabbedUpdate(GrabbableItem item)
	{
		if (!item.CurrInteractableHand.IsSqueezedButton())
		{
			gripDownLastFrame = false;
		}
		else
		{
			if (gripDownLastFrame)
			{
				return;
			}
			gripDownLastFrame = true;
			if (fuel <= 0f)
			{
				Debug.Log("Fill Started");
				RaycastHit hitInfo;
				if (Physics.Raycast(tipOfDropperContent.position, -simpleDrawing.ray.direction, out hitInfo, 0.5f))
				{
					Debug.Log("Fill Raycast Success");
					ChemicalContainerController component = hitInfo.collider.GetComponent<ChemicalContainerController>();
					Debug.Log("Fill hit" + hitInfo.collider.name);
					if (component != null)
					{
						currentChemicalController = component;
						dropperContent1.material = currentChemicalController.chemicalMaterial;
						dropperContent2.material = currentChemicalController.chemicalMaterial;
						FuelAnimation(true);
					}
				}
			}
			else if (fuel != 0f && fuel <= 3f)
			{
				Debug.Log("Drop Started");
				Drop();
			}
		}
	}

	private void Drop()
	{
		if (currentChemicalController == null)
		{
			return;
		}
		FuelAnimation(false);
		drops.startColor = currentChemicalController.chemicalColor;
		drops.Emit(1);
		simpleDrawing.brushColor = currentChemicalController.chemicalColor;
		simpleDrawing.AttemptToDraw();
		RaycastHit hitInfo;
		if (Physics.Raycast(simpleDrawing.ray, out hitInfo, float.PositiveInfinity))
		{
			TransmutableObject component = hitInfo.transform.gameObject.GetComponent<TransmutableObject>();
			if (component != null)
			{
				component.RecieveChemical(currentChemicalController.chemical);
			}
		}
	}

	private void FuelAnimation(bool fillTheFuel)
	{
		if (fillTheFuel)
		{
			myAnimation[myAnimation.clip.name].time = myAnimation.clip.events[2].time + 0.01f;
			myAnimation.Play();
		}
		else
		{
			myAnimation.Play();
		}
	}

	public void AnimationBreakPoint(float fuelInClip)
	{
		float time = myAnimation[myAnimation.clip.name].time + 0.01f;
		myAnimation.Stop();
		myAnimation[myAnimation.clip.name].time = time;
		fuel = fuelInClip;
	}
}
