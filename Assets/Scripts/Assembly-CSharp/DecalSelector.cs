using OwlchemyVR;
using UnityEngine;

public class DecalSelector : MonoBehaviour
{
	[SerializeField]
	private SpriteRenderer decal;

	[SerializeField]
	private WorldItem wItem;

	private void OnTriggerEnter(Collider col)
	{
		if (!(col.attachedRigidbody == null) && (bool)col.attachedRigidbody.GetComponent<DecalGunController>())
		{
			col.attachedRigidbody.GetComponent<DecalGunController>().SetDecal(decal.sprite, wItem);
			GameEventsManager.Instance.ItemActionOccurred(wItem.Data, "ACTIVATED");
		}
	}
}
