using UnityEngine;

public class ConnectJointOnSpawn : MonoBehaviour
{
	private void Start()
	{
		if (base.transform.parent != null)
		{
			Joint component = GetComponent<Joint>();
			component.connectedBody = base.transform.parent.GetComponentInParent<Rigidbody>();
			if (component.connectedBody != null && component.autoConfigureConnectedAnchor)
			{
				component.connectedAnchor = component.connectedBody.transform.InverseTransformPoint(component.transform.TransformPoint(component.anchor));
			}
		}
	}
}
