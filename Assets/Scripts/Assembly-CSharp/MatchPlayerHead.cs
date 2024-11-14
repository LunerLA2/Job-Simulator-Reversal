using UnityEngine;

public class MatchPlayerHead : MonoBehaviour
{
	[SerializeField]
	private bool moveX;

	[SerializeField]
	private bool moveY;

	[SerializeField]
	private bool moveZ;

	[SerializeField]
	private float adjustSpeed = 1f;

	[SerializeField]
	private bool useLimits;

	[SerializeField]
	private Vector3 minimumLocalPosition;

	[SerializeField]
	private Vector3 maximumLocalPosition;

	private Transform playerHead;

	private void Update()
	{
		if (playerHead == null)
		{
			playerHead = GlobalStorage.Instance.MasterHMDAndInputController.TrackedHmdTransform;
		}
		if (playerHead != null)
		{
			Vector3 vector = playerHead.transform.position - base.transform.position;
			Vector3 localPosition = Vector3.Lerp(base.transform.localPosition, base.transform.localPosition + vector, adjustSpeed * Time.deltaTime);
			if (useLimits)
			{
				localPosition.x = Mathf.Clamp(localPosition.x, minimumLocalPosition.x, maximumLocalPosition.x);
				localPosition.y = Mathf.Clamp(localPosition.y, minimumLocalPosition.y, maximumLocalPosition.y);
				localPosition.z = Mathf.Clamp(localPosition.z, minimumLocalPosition.z, maximumLocalPosition.z);
			}
			if (!moveX)
			{
				localPosition.x = base.transform.localPosition.x;
			}
			if (!moveY)
			{
				localPosition.y = base.transform.localPosition.y;
			}
			if (!moveZ)
			{
				localPosition.z = base.transform.localPosition.z;
			}
			base.transform.localPosition = localPosition;
		}
	}
}
