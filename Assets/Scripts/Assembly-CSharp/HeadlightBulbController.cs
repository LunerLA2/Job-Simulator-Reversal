using System;
using OwlchemyVR;
using UnityEngine;

public class HeadlightBulbController : MonoBehaviour
{
	private const float fluidLerpTime = 2f;

	[SerializeField]
	private ParticleCollectionZone collectionZone;

	[SerializeField]
	private MeshRenderer headLightMr;

	[SerializeField]
	private AttachableObject attachableObject;

	private Color currentColor;

	private Color targetColor;

	private float collectionStartTime;

	private bool canChangeColor;

	private void Awake()
	{
		currentColor = headLightMr.material.color;
		targetColor = headLightMr.material.color;
	}

	private void OnEnable()
	{
		ParticleCollectionZone particleCollectionZone = collectionZone;
		particleCollectionZone.OnParticleIsCollecting = (Action<ParticleCollectionZone, WorldItemData, float>)Delegate.Combine(particleCollectionZone.OnParticleIsCollecting, new Action<ParticleCollectionZone, WorldItemData, float>(UpdateHeadlight));
		AttachableObject obj = attachableObject;
		obj.OnAttach = (Action<AttachableObject, AttachablePoint>)Delegate.Combine(obj.OnAttach, new Action<AttachableObject, AttachablePoint>(OnAttach));
		AttachableObject obj2 = attachableObject;
		obj2.OnDetach = (Action<AttachableObject, AttachablePoint>)Delegate.Combine(obj2.OnDetach, new Action<AttachableObject, AttachablePoint>(OnDetach));
		canChangeColor = attachableObject.CurrentlyAttachedTo == null;
		collectionZone.gameObject.SetActive(canChangeColor);
	}

	private void OnDisable()
	{
		ParticleCollectionZone particleCollectionZone = collectionZone;
		particleCollectionZone.OnParticleIsCollecting = (Action<ParticleCollectionZone, WorldItemData, float>)Delegate.Remove(particleCollectionZone.OnParticleIsCollecting, new Action<ParticleCollectionZone, WorldItemData, float>(UpdateHeadlight));
		AttachableObject obj = attachableObject;
		obj.OnAttach = (Action<AttachableObject, AttachablePoint>)Delegate.Remove(obj.OnAttach, new Action<AttachableObject, AttachablePoint>(OnAttach));
		AttachableObject obj2 = attachableObject;
		obj2.OnDetach = (Action<AttachableObject, AttachablePoint>)Delegate.Remove(obj2.OnDetach, new Action<AttachableObject, AttachablePoint>(OnDetach));
	}

	private void OnAttach(AttachableObject attachableObject, AttachablePoint attachablePoint)
	{
		canChangeColor = false;
		collectionZone.gameObject.SetActive(canChangeColor);
	}

	private void OnDetach(AttachableObject attachableObject, AttachablePoint attachablePoint)
	{
		canChangeColor = true;
		collectionZone.gameObject.SetActive(canChangeColor);
	}

	private void UpdateHeadlight(ParticleCollectionZone particleCollectionZone, WorldItemData worldItemData, float quantity)
	{
		if (!(collectionZone.GetTotalQuantity() < 1f))
		{
			if (targetColor != worldItemData.OverallColor)
			{
				currentColor = headLightMr.material.color;
				collectionStartTime = Time.timeSinceLevelLoad;
				targetColor = worldItemData.OverallColor;
			}
			else if (Time.timeSinceLevelLoad - collectionStartTime > 2f)
			{
				headLightMr.material.color = targetColor;
				return;
			}
			float t = (Time.timeSinceLevelLoad - collectionStartTime) / 2f;
			headLightMr.material.color = Color.Lerp(currentColor, targetColor, t);
		}
	}

	public Renderer GetHeadlightRenderer()
	{
		return headLightMr;
	}
}
