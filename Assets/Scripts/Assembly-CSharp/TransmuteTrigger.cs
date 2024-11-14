using UnityEngine;

public class TransmuteTrigger : MonoBehaviour
{
	[SerializeField]
	private ChemLabManager.ObjectStates transmuteTo;

	[SerializeField]
	private bool objectsInTriggerFloat;

	private Collider myCollider;

	private float waterLevel;

	private float forceFactor;

	private Vector3 uplift;

	private void Awake()
	{
		myCollider = GetComponent<Collider>();
		waterLevel = myCollider.bounds.max.y;
	}

	private void OnTriggerEnter(Collider col)
	{
		if ((bool)col.GetComponent<TransmutableObject>())
		{
			col.GetComponent<TransmutableObject>().SetState(transmuteTo);
			Debug.Log("found Transmutable Object and Set it to" + transmuteTo);
			waterLevel = myCollider.bounds.max.y;
			if (transmuteTo == ChemLabManager.ObjectStates.Frozen)
			{
				col.attachedRigidbody.velocity *= 0.2f;
			}
		}
		else
		{
			Debug.Log("Found " + col.gameObject.name + "  but not Transmuteable");
		}
	}

	private void OnTriggerStay(Collider col)
	{
		if (objectsInTriggerFloat && (bool)col.attachedRigidbody)
		{
			forceFactor = 1f - (col.transform.position.y - waterLevel);
			if (forceFactor > 0f)
			{
				uplift = -Physics.gravity * (forceFactor - col.attachedRigidbody.velocity.y * 0.04f);
				col.attachedRigidbody.AddForceAtPosition(uplift, col.bounds.center);
			}
		}
	}

	private void OnDrawGizmos()
	{
		if (objectsInTriggerFloat)
		{
			Collider component = GetComponent<Collider>();
			waterLevel = component.bounds.max.y;
			Gizmos.color = new Color(0f, 0.75f, 1f, 0.5f);
			Gizmos.DrawCube(new Vector3(base.transform.position.x, waterLevel, base.transform.position.z), new Vector3(component.bounds.size.x, 0.01f, component.bounds.size.z));
		}
	}
}
