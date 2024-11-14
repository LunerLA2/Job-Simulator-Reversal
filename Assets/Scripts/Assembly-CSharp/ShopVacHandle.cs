using OwlchemyVR;
using UnityEngine;

public class ShopVacHandle : MonoBehaviour
{
	[SerializeField]
	private AttachablePoint attachPoint;

	[SerializeField]
	private AttachableObject interactionAttach;

	[SerializeField]
	private WorldItemData vacuumAttachmentItemData;

	[SerializeField]
	private WorldItemData pumpAttachmentItemData;

	private void Start()
	{
		attachPoint.OnObjectWasAttached += AttachmentConnected;
		attachPoint.OnObjectWasDetached += AttachmentDisconnected;
	}

	private void OnDestroy()
	{
		attachPoint.OnObjectWasAttached -= AttachmentConnected;
		attachPoint.OnObjectWasDetached -= AttachmentDisconnected;
	}

	private void Update()
	{
	}

	private void AttachmentConnected(AttachablePoint point, AttachableObject cd)
	{
	}

	private void AttachmentDisconnected(AttachablePoint point, AttachableObject cd)
	{
	}
}
