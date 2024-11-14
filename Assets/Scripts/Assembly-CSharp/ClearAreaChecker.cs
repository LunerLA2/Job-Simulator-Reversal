using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ClearAreaChecker : MonoBehaviour
{
	private BoxCollider boxCollider;

	private RaycastHit[] hits;

	private void Awake()
	{
		hits = new RaycastHit[1];
		boxCollider = GetComponent<BoxCollider>();
		if (boxCollider.center != Vector3.zero)
		{
			Debug.LogError("Don't use boxCollider.center, move the transform instead!", base.gameObject);
		}
		boxCollider.isTrigger = true;
		boxCollider.enabled = false;
	}

	public int NumberOfCollidersInArea()
	{
		return Physics.BoxCastNonAlloc(base.transform.position, boxCollider.size / 2f, base.transform.forward, hits, base.transform.rotation, 0f, LayerMaskHelper.OnlyIncluding(8, 0), QueryTriggerInteraction.Ignore);
	}
}
