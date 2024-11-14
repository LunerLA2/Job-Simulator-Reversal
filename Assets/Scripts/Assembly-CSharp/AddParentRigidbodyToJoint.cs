using UnityEngine;

public class AddParentRigidbodyToJoint : MonoBehaviour
{
	private void Start()
	{
		if (base.transform.parent != null)
		{
			Rigidbody componentInParent = base.transform.parent.GetComponentInParent<Rigidbody>();
			if (componentInParent != null)
			{
				GetComponent<Joint>().connectedBody = componentInParent;
			}
		}
		else
		{
			Debug.Log("Parent not yet set");
		}
	}
}
