using OwlchemyVR;
using UnityEngine;

public class FlippingTable : MonoBehaviour
{
	[SerializeField]
	private WorldItemData worldItemDataThatMustOcclude;

	[SerializeField]
	private Vector3[] rayOrigins;

	[SerializeField]
	private Vector3 rayDirection;

	[SerializeField]
	private float rayCheckDistance = 1f;

	[SerializeField]
	private Animation flipAnimation;

	private bool occluded;

	private bool canFlip = true;

	private void Update()
	{
		bool flag = false;
		bool flag2 = occluded;
		if (rayOrigins != null)
		{
			for (int i = 0; i < rayOrigins.Length; i++)
			{
				bool flag3 = false;
				Ray ray = new Ray(rayOrigins[i] + base.transform.position, rayDirection);
				RaycastHit hitInfo;
				if (Physics.Raycast(ray, out hitInfo, rayCheckDistance))
				{
					WorldItem worldItemFromClothCollider = GetWorldItemFromClothCollider(hitInfo.collider);
					if (worldItemFromClothCollider != null && worldItemFromClothCollider.Data == worldItemDataThatMustOcclude)
					{
						flag3 = true;
					}
				}
				if (!flag3)
				{
					flag = true;
				}
			}
		}
		occluded = !flag;
		if (occluded != flag2)
		{
			Flip();
		}
	}

	private void Flip()
	{
		if (canFlip)
		{
			canFlip = false;
			flipAnimation.Play();
			Invoke("FinishedFlip", flipAnimation.clip.length);
		}
	}

	private void FinishedFlip()
	{
		canFlip = true;
	}

	private WorldItem GetWorldItemFromClothCollider(Collider c)
	{
		Rigidbody attachedRigidbody = c.attachedRigidbody;
		if (attachedRigidbody != null)
		{
			return attachedRigidbody.GetComponent<WorldItem>();
		}
		return null;
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = ((!occluded) ? Color.red : Color.green);
		if (rayOrigins != null)
		{
			for (int i = 0; i < rayOrigins.Length; i++)
			{
				Gizmos.DrawLine(rayOrigins[i] + base.transform.position, rayOrigins[i] + base.transform.position + rayDirection.normalized * rayCheckDistance);
			}
		}
	}
}
