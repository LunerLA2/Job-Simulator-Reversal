using UnityEngine;

public class KnifeSticker : MonoBehaviour
{
	public Rigidbody r;

	private float knifeStickForceRequired = 8f;

	private void OnCollisionEnter(Collision c)
	{
		if (c.contacts.Length <= 0 || !(c.contacts[0].thisCollider.name == "knifeThrow") || !(c.collider.GetComponent<KnifeWall>() != null))
		{
			return;
		}
		if (c.relativeVelocity.sqrMagnitude > knifeStickForceRequired)
		{
			float num = 50f;
			Vector3 normal = c.contacts[0].normal;
			Vector3 relativeVelocity = c.relativeVelocity;
			float num2 = Vector3.Angle(r.transform.forward, -normal);
			float num3 = Vector3.Angle(relativeVelocity, normal);
			if (num3 < num && num2 < num)
			{
				r.isKinematic = true;
				Debug.Log("Knife stick successs");
				return;
			}
			Debug.Log("Knife did not stick, rotation incorrect, storedRotationToNormal:" + num2 + ", storedVelocityToNormal:" + num3);
		}
		else
		{
			Debug.Log("Knife Did not stick velocity not reached:" + c.relativeVelocity.sqrMagnitude);
		}
	}
}
