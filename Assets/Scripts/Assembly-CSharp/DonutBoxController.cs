using UnityEngine;

public class DonutBoxController : MonoBehaviour
{
	[SerializeField]
	private GameObject boxContentsPrefab;

	[SerializeField]
	private Transform boxContentsLocation;

	[SerializeField]
	private FreezeAttachpointsWhenHingeLocked freeze;

	[SerializeField]
	private GrabbableHinge hinge;

	private bool hasSpawned;

	private void OnEnable()
	{
		hinge.OnLowerUnlocked += HingeUnlocked;
		hinge.OnUpperUnlocked += HingeUnlocked;
	}

	private void OnDisable()
	{
		hinge.OnLowerUnlocked -= HingeUnlocked;
		hinge.OnUpperUnlocked -= HingeUnlocked;
	}

	private void HingeUnlocked(GrabbableHinge hinge)
	{
		if (!hasSpawned)
		{
			hasSpawned = true;
			GameObject gameObject = Object.Instantiate(boxContentsPrefab, boxContentsLocation.position, boxContentsLocation.rotation, boxContentsLocation) as GameObject;
			gameObject.transform.SetToDefaultPosRotScale();
			AttachablePoint[] componentsInChildren = gameObject.GetComponentsInChildren<AttachablePoint>();
			freeze.RegisterAttachablePoints(componentsInChildren, false);
		}
	}
}
