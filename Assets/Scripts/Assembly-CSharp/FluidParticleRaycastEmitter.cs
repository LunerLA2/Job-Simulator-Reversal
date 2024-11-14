using OwlchemyVR;
using UnityEngine;

public class FluidParticleRaycastEmitter : MonoBehaviour
{
	public const float MAX_PARTICLE_DISTANCE = 4f;

	public const float MAX_TIME_TO_IMPACT = 2.5f;

	private const float MIN_FLOOR_Y_POS = 0f;

	private int layerMask;

	private float distanceToHitTemp;

	private RaycastHit hitTemp;

	private RaycastHit hitTemp2;

	private ParticleImpactZonePointer tempZonePointer;

	private ParticleImpactZone tempZone;

	private PickupableItem tempPickupable;

	private ItemCollectionZone tempItemCollectionZone;

	private void Awake()
	{
		layerMask = LayerMaskHelper.EverythingBut(20);
	}

	public static float GetTimeToParticleImpact(float startingHeight, float acceleration, float initialVelocity)
	{
		float num = acceleration / 2f;
		float num2 = initialVelocity * initialVelocity - 4f * num * startingHeight;
		if (num2 > 0f)
		{
			float num3 = (0f - initialVelocity + Mathf.Sqrt(num2)) / (2f * num);
			float num4 = (0f - initialVelocity - Mathf.Sqrt(num2)) / (2f * num);
			if (num3 >= 0f && num4 >= 0f)
			{
				return Mathf.Min(num3, num4);
			}
			if (num3 >= 0f)
			{
				return num3;
			}
			if (num4 >= 0f)
			{
				return num4;
			}
		}
		else if (num2 == 0f)
		{
			float num5 = (0f - initialVelocity + Mathf.Sqrt(num2)) / (2f * num);
			if (num5 >= 0f)
			{
				return num5;
			}
		}
		return float.PositiveInfinity;
	}

	public void DispensingFluidQuantity(Vector3 pos, Vector3 dir, ParticleCollectionZone ignoreSelf, Rigidbody ignoreRigidbodySelf, ref FluidParticleEmitCollisionInfo fluidParticleEmitCollisionInfo)
	{
		distanceToHitTemp = 4f;
		tempZonePointer = null;
		tempZone = null;
		RaycastHit[] array = Physics.RaycastAll(pos, dir, 4f, layerMask);
		if (array.Length == 0)
		{
			distanceToHitTemp = 0f;
			tempZone = null;
		}
		else if (array.Length > 1)
		{
			for (int i = 0; i < array.Length; i++)
			{
				for (int j = 0; j < array.Length - 1; j++)
				{
					if (array[j].distance > array[j + 1].distance)
					{
						hitTemp = array[j + 1];
						array[j + 1] = array[j];
						array[j] = hitTemp;
					}
				}
			}
		}
		int num = array.Length;
		bool flag = false;
		bool flag2 = ignoreRigidbodySelf != null;
		for (int k = 0; k < num; k++)
		{
			hitTemp = array[k];
			if (flag2 && hitTemp.rigidbody == ignoreRigidbodySelf)
			{
				continue;
			}
			tempZonePointer = hitTemp.collider.GetComponent<ParticleImpactZonePointer>();
			if (tempZonePointer != null)
			{
				tempZone = tempZonePointer.ParticleImpactZone;
				if (tempZone != null && (tempZone.ParticleCollectionZone == null || tempZone.ParticleCollectionZone != ignoreSelf))
				{
					distanceToHitTemp = hitTemp.distance;
					break;
				}
				tempZonePointer = null;
				tempZone = null;
			}
			if (hitTemp.collider.isTrigger)
			{
				continue;
			}
			distanceToHitTemp = hitTemp.distance;
			if (!(hitTemp.rigidbody != null) || k + 1 >= num)
			{
				break;
			}
			tempPickupable = hitTemp.rigidbody.GetComponent<PickupableItem>();
			if (!(tempPickupable != null))
			{
				break;
			}
			for (int l = k + 1; l < num; l++)
			{
				hitTemp2 = array[l];
				if (!hitTemp2.collider.isTrigger)
				{
					continue;
				}
				tempZonePointer = hitTemp2.collider.GetComponent<ParticleImpactZonePointer>();
				if (!(tempZonePointer != null))
				{
					continue;
				}
				tempZone = tempZonePointer.ParticleImpactZone;
				if (tempZone != null)
				{
					if (tempZone.ParticleCollectionZone != null && tempZone.ParticleCollectionZone == ignoreSelf)
					{
						tempItemCollectionZone = tempZone.ParticleCollectionZone.ContainsItemCollectionZone;
						if (tempItemCollectionZone != null && tempItemCollectionZone.DoesItemCollectionZoneContainPickupable(tempPickupable))
						{
							flag = true;
							break;
						}
						tempItemCollectionZone = null;
						tempZonePointer = null;
						tempZone = null;
					}
					else
					{
						tempZonePointer = null;
						tempZone = null;
					}
				}
				else
				{
					tempZonePointer = null;
				}
			}
			break;
		}
		if (tempZone == null)
		{
			Collider[] array2 = Physics.OverlapSphere(pos + dir * 0.01f, 0.0001f);
			if (array2.Length > 0)
			{
				foreach (Collider collider in array2)
				{
					tempZonePointer = collider.GetComponent<ParticleImpactZonePointer>();
					if (!(tempZonePointer != null))
					{
						continue;
					}
					tempZone = tempZonePointer.ParticleImpactZone;
					if (tempZone != null)
					{
						if (tempZone.ParticleCollectionZone == null || tempZone.ParticleCollectionZone != ignoreSelf)
						{
							distanceToHitTemp = 0f;
							break;
						}
						tempZonePointer = null;
						tempZone = null;
					}
					else
					{
						tempZonePointer = null;
						tempZone = null;
					}
				}
			}
		}
		if (tempZone != null && tempZone.ParticleCollectionZone != null)
		{
			float rayIntersectWithPlane = tempZone.ParticleCollectionZone.GetRayIntersectWithPlane(new Ray(pos, dir));
			if (rayIntersectWithPlane > 0f && !flag && rayIntersectWithPlane > distanceToHitTemp)
			{
				distanceToHitTemp = rayIntersectWithPlane - 0.002f;
			}
		}
		fluidParticleEmitCollisionInfo.distanceToHit = distanceToHitTemp;
		fluidParticleEmitCollisionInfo.zone = tempZone;
	}
}
