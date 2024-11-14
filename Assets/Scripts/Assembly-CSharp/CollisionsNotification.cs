using UnityEngine;

public class CollisionsNotification : MonoBehaviour
{
	private void OnCollisionEnter(Collision collisionInfo)
	{
		MonoBehaviour.print("Detected collision between " + base.gameObject.name + " and " + collisionInfo.collider.name);
		MonoBehaviour.print("There are " + collisionInfo.contacts.Length + " point(s) of contacts");
		MonoBehaviour.print("Their relative velocity is " + collisionInfo.relativeVelocity);
	}

	private void OnCollisionStay(Collision collisionInfo)
	{
		MonoBehaviour.print(base.gameObject.name + " and " + collisionInfo.collider.name + " are still colliding");
	}

	private void OnCollisionExit(Collision collisionInfo)
	{
		MonoBehaviour.print(base.gameObject.name + " and " + collisionInfo.collider.name + " are no longer colliding");
	}
}
