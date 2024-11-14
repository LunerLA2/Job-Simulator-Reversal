using OwlchemyVR;
using UnityEngine;

public class Stackable : MonoBehaviour
{
	[SerializeField]
	private Collider c;

	private Rigidbody r;

	private PickupableItem pickupableItem;

	private GroupMasterPickupableItem masterPickupable;

	private StackableController stackableController;

	public PickupableItem PickupableItem
	{
		get
		{
			return pickupableItem;
		}
	}

	private void Awake()
	{
		r = GetComponent<Rigidbody>();
		pickupableItem = GetComponent<PickupableItem>();
		base.enabled = false;
	}

	public bool IsStackingReady(Collider stackingOnTopOfCollider, float maxRotationOffset = 30f)
	{
		bool result = false;
		if (masterPickupable == null && !pickupableItem.IsCurrInHand && r.velocity.magnitude < 0.01f && r.angularVelocity.magnitude < 0.01f)
		{
			float num = Vector3.Angle(base.transform.up, Vector3.up);
			if (num < maxRotationOffset || num > 180f - maxRotationOffset)
			{
				RaycastHit[] array = Physics.RaycastAll(direction: (!(num < 90f)) ? c.transform.up : (-c.transform.up), origin: c.bounds.center, maxDistance: 1f);
				for (int i = 0; i < array.Length; i++)
				{
					if (array[i].collider == stackingOnTopOfCollider)
					{
						result = true;
						break;
					}
				}
			}
		}
		return result;
	}

	public void ApplyStacking(GroupMasterPickupableItem masterPickupable, StackableController stackableController)
	{
		this.masterPickupable = masterPickupable;
		this.stackableController = stackableController;
		r.isKinematic = true;
		base.gameObject.layer = 0;
		GetComponent<SelectedChangeOutlineController>().SetSpecialHighlight(true);
		base.enabled = true;
		pickupableItem.enabled = false;
	}

	public void SetAsDisabledStackableLevel()
	{
		base.enabled = false;
		Object.Destroy(GetComponent<PickupableItem>());
		Object.Destroy(GetComponent<Rigidbody>());
	}

	private void OnCollisionStay(Collision collision)
	{
		if (base.enabled)
		{
			Stackable stackableFromCollider = stackableController.GetStackableFromCollider(collision.collider);
			if (stackableFromCollider != null && stackableFromCollider.IsStackingReady(c, 30f))
			{
				stackableController.AddIngredient(stackableFromCollider);
			}
		}
	}
}
