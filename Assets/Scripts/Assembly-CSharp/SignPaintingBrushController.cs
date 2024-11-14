using System;
using OwlchemyVR;
using UnityEngine;

public class SignPaintingBrushController : MonoBehaviour
{
	[SerializeField]
	private MeshRenderer colorRenderer;

	[SerializeField]
	private Transform tipReference;

	[SerializeField]
	private WorldItem myWorldItem;

	[SerializeField]
	private ParticleImpactZone particleImpactZone;

	private Color currentColor;

	private bool hasBeenDippedOnce;

	private RaycastHit[] raycast;

	[SerializeField]
	private Transform paintTip;

	public Transform PaintTip
	{
		get
		{
			return paintTip;
		}
	}

	public Color CurrentColor
	{
		get
		{
			return currentColor;
		}
	}

	public bool HasBeenDippedOnce
	{
		get
		{
			return hasBeenDippedOnce;
		}
	}

	private void OnEnable()
	{
		ParticleImpactZone obj = particleImpactZone;
		obj.OnSpecificParticleAppliedUpdate = (Action<ParticleImpactZone, WorldItemData, Vector3>)Delegate.Combine(obj.OnSpecificParticleAppliedUpdate, new Action<ParticleImpactZone, WorldItemData, Vector3>(SpecificFluidPouredOnAtPosition));
	}

	private void OnDisable()
	{
		ParticleImpactZone obj = particleImpactZone;
		obj.OnSpecificParticleAppliedUpdate = (Action<ParticleImpactZone, WorldItemData, Vector3>)Delegate.Remove(obj.OnSpecificParticleAppliedUpdate, new Action<ParticleImpactZone, WorldItemData, Vector3>(SpecificFluidPouredOnAtPosition));
	}

	private void SpecificFluidPouredOnAtPosition(ParticleImpactZone zone, WorldItemData data, Vector3 position)
	{
		ChangeBrushColor(zone.ParticleCollectionZone.CalculateCombinedFluidColor());
	}

	private void LateUpdate()
	{
		raycast = Physics.RaycastAll(tipReference.position, base.transform.up, 0.03f);
		for (int i = 0; i < raycast.Length; i++)
		{
			if (raycast[i].collider.isTrigger)
			{
				ParticleImpactZonePointer component = raycast[i].collider.gameObject.GetComponent<ParticleImpactZonePointer>();
				if (component != null && component.ParticleImpactZone != null && component.ParticleImpactZone.ParticleCollectionZone != null && component.ParticleImpactZone.ParticleCollectionZone.GetTotalQuantity() > 0f)
				{
					ChangeBrushColor(component.ParticleImpactZone.ParticleCollectionZone.CalculateCombinedFluidColor());
				}
			}
		}
	}

	public void UsedEvent()
	{
		GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "USED");
	}

	public void ChangeBrushColor(Color color)
	{
		hasBeenDippedOnce = true;
		currentColor = color;
		colorRenderer.material.SetColor("_DiffColor", currentColor);
		GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "ACTIVATED");
	}
}
