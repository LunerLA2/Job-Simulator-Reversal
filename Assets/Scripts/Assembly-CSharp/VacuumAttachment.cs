using System;
using System.Collections;
using System.Collections.Generic;
using OwlchemyVR;
using UnityEngine;

public class VacuumAttachment : MonoBehaviour
{
	[SerializeField]
	private AttachableObject attachableObject;

	[SerializeField]
	private Collider vacuumZone;

	[SerializeField]
	private ParticleSystem visualEffects;

	[SerializeField]
	private Transform pointingDirection;

	[SerializeField]
	private float raycastDistance = 0.5f;

	private GameObject stuckObject;

	[SerializeField]
	private WorldItemData worldItemData;

	[SerializeField]
	private List<WorldItemData> itemsToDestroy;

	private bool vacuumIsActive;

	private bool vacuumIsStuck;

	private List<GameObject> suckingObjects = new List<GameObject>();

	private void Start()
	{
		vacuumIsStuck = false;
		vacuumIsActive = false;
		vacuumZone.enabled = false;
	}

	private void OnEnable()
	{
		AttachableObject obj = attachableObject;
		obj.OnAttach = (Action<AttachableObject, AttachablePoint>)Delegate.Combine(obj.OnAttach, new Action<AttachableObject, AttachablePoint>(AttachedToHandle));
		AttachableObject obj2 = attachableObject;
		obj2.OnDetach = (Action<AttachableObject, AttachablePoint>)Delegate.Combine(obj2.OnDetach, new Action<AttachableObject, AttachablePoint>(DetachedToHandle));
	}

	private void OnDisable()
	{
		AttachableObject obj = attachableObject;
		obj.OnAttach = (Action<AttachableObject, AttachablePoint>)Delegate.Remove(obj.OnAttach, new Action<AttachableObject, AttachablePoint>(AttachedToHandle));
		AttachableObject obj2 = attachableObject;
		obj2.OnDetach = (Action<AttachableObject, AttachablePoint>)Delegate.Remove(obj2.OnDetach, new Action<AttachableObject, AttachablePoint>(DetachedToHandle));
	}

	private void FixedUpdate()
	{
		if (!vacuumIsActive)
		{
			return;
		}
		RaycastHit[] array = Physics.RaycastAll(pointingDirection.position, pointingDirection.forward, raycastDistance, 256);
		if (array.Length == 0)
		{
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			if ((bool)array[i].rigidbody)
			{
				float num = -10f;
				array[i].rigidbody.AddForce((array[i].rigidbody.position - pointingDirection.position) * num);
			}
		}
	}

	private void AttachedToHandle(AttachableObject obj, AttachablePoint cd)
	{
		vacuumIsActive = true;
		vacuumZone.enabled = true;
		visualEffects.Play();
	}

	private void DetachedToHandle(AttachableObject obj, AttachablePoint cd)
	{
		vacuumIsActive = true;
		vacuumZone.enabled = false;
		visualEffects.Stop();
	}

	private void OnTriggerEnter(Collider otherObj)
	{
		if (!otherObj.isTrigger && (vacuumIsActive || vacuumIsStuck) && otherObj.gameObject.layer == 8 && otherObj.attachedRigidbody != null)
		{
			WorldItem component = otherObj.attachedRigidbody.GetComponent<WorldItem>();
			if (component != null && itemsToDestroy.Contains(component.Data))
			{
				StartCoroutine(StartSuckingObject(otherObj.attachedRigidbody));
			}
		}
	}

	private IEnumerator StartSuckingObject(Rigidbody aObjectBody)
	{
		if (!suckingObjects.Contains(aObjectBody.gameObject))
		{
			suckingObjects.Add(aObjectBody.gameObject);
			aObjectBody.gameObject.transform.scaleTo(0.5f, 0.2f);
			aObjectBody.gameObject.transform.positionTo(0.5f, pointingDirection.position);
			yield return new WaitForSeconds(0.5f);
			UnityEngine.Object.Destroy(aObjectBody.gameObject);
			GameEventsManager.Instance.ItemActionOccurred(worldItemData, "ACTIVATED");
			suckingObjects.Remove(aObjectBody.gameObject);
		}
	}

	private void OnDrawGizmos()
	{
		if (pointingDirection != null)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawRay(pointingDirection.position, pointingDirection.forward * raycastDistance);
		}
	}
}
