using UnityEngine;

public class TransportTubeManager : MonoBehaviour
{
	[SerializeField]
	private Transform teleporterDestination;

	[SerializeField]
	private Transform handle;

	[SerializeField]
	private Transform closedHandlePosition;

	[SerializeField]
	private float distance = 0.05f;

	private bool isOpen;

	private void Update()
	{
		isOpen = Vector3.Distance(closedHandlePosition.position, handle.position) > distance;
	}

	private void OnTriggerStay(Collider col)
	{
		if (!isOpen && (bool)col.attachedRigidbody)
		{
			col.attachedRigidbody.AddForce(-Physics.gravity * 1.5f);
			if (col.attachedRigidbody.drag < 0.5f)
			{
				col.attachedRigidbody.drag = 0.5f;
			}
		}
	}

	private void OnTriggerExit(Collider col)
	{
		if (!isOpen)
		{
			col.attachedRigidbody.Sleep();
			col.transform.position = teleporterDestination.position;
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(closedHandlePosition.position, distance);
		Gizmos.color = Color.green;
		Gizmos.DrawWireSphere(handle.position, distance);
		Gizmos.color = new Color(0f, 1f, 1f, 0.5f);
		Gizmos.DrawWireSphere(teleporterDestination.position, 0.35f);
	}
}
