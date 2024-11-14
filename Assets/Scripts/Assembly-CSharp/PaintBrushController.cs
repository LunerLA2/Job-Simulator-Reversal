using System;
using OwlchemyVR;
using UnityEngine;

public class PaintBrushController : MonoBehaviour
{
	[SerializeField]
	private MeshRenderer colorRenderer;

	[SerializeField]
	private Transform tipReference;

	[SerializeField]
	private WorldItem myWorldItem;

	[SerializeField]
	private ParticleImpactZone particleImpactZone;

	[SerializeField]
	private WorldItemData[] worldItemsThatTransferColorToBrush;

	private Color currentColor;

	private bool hasBeenDippedOnce;

	private RaycastHit[] raycast;

	private void Start()
	{
		currentColor = Color.white;
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
			ParticleImpactZonePointer component = raycast[i].collider.gameObject.GetComponent<ParticleImpactZonePointer>();
			if (component != null && component.ParticleImpactZone != null && component.ParticleImpactZone.ParticleCollectionZone != null && component.ParticleImpactZone.ParticleCollectionZone.GetTotalQuantity() > 0f)
			{
				ChangeBrushColor(component.ParticleImpactZone.ParticleCollectionZone.CalculateCombinedFluidColor());
			}
		}
	}

	private void OnTriggerEnter(Collider col)
	{
		if (hasBeenDippedOnce && col.GetComponent<Paintable>() != null)
		{
			MeshRenderer component = col.GetComponent<MeshRenderer>();
			if (component != null)
			{
				component.material.color = currentColor;
				component.enabled = true;
				GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "USED");
			}
			else
			{
				Debug.LogError("No meshRenderer found on paintableObject: " + col.gameObject.name, col.gameObject);
			}
		}
	}

	private void ChangeBrushColor(Color color)
	{
		hasBeenDippedOnce = true;
		currentColor = color;
		colorRenderer.material.SetColor("_DiffColor", currentColor);
		GameEventsManager.Instance.ItemActionOccurred(myWorldItem.Data, "ACTIVATED");
	}

	private void OnCollisionEnter(Collision col)
	{
		WorldItem componentInParent = col.collider.gameObject.GetComponentInParent<WorldItem>();
		if (componentInParent != null && Array.IndexOf(worldItemsThatTransferColorToBrush, componentInParent.Data) > -1)
		{
			ChangeBrushColor(componentInParent.Data.OverallColor);
		}
	}
}
