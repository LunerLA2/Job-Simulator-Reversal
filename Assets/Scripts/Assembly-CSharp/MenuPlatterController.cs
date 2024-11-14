using System;
using UnityEngine;

public class MenuPlatterController : MonoBehaviour
{
	[SerializeField]
	private Rigidbody rb;

	private Vector3 spawnPosition;

	private Quaternion spawnRotation;

	[SerializeField]
	private EdibleItem exitEdible;

	[SerializeField]
	[Tooltip("The trigger listener to determine when to 'poof' back to the initial position")]
	private TriggerListener exitEdibleTrigger;

	private Vector3 exitEdibleLocation;

	private Quaternion exitEdibleRotation;

	[SerializeField]
	private GameObject resetDrink;

	[SerializeField]
	[Tooltip("The trigger listener to determine when to 'poof' back to the initial position")]
	private TriggerListener resetDrinkTrigger;

	private Vector3 resetDrinkLocation;

	private Quaternion resetDrinkRotation;

	private void Awake()
	{
		exitEdibleLocation = exitEdible.transform.localPosition;
		exitEdibleRotation = exitEdible.transform.localRotation;
		resetDrinkLocation = resetDrink.transform.localPosition;
		resetDrinkRotation = resetDrink.transform.localRotation;
	}

	private void OnEnable()
	{
		TriggerListener triggerListener = exitEdibleTrigger;
		triggerListener.OnEnter = (Action<TriggerEventInfo>)Delegate.Combine(triggerListener.OnEnter, new Action<TriggerEventInfo>(OnExitEdibleTriggerListenerEnter));
	}

	private void OnDisable()
	{
		TriggerListener triggerListener = exitEdibleTrigger;
		triggerListener.OnEnter = (Action<TriggerEventInfo>)Delegate.Remove(triggerListener.OnEnter, new Action<TriggerEventInfo>(OnExitEdibleTriggerListenerEnter));
	}

	public void ResetObject()
	{
		if (rb != null)
		{
			rb.isKinematic = true;
			rb.Sleep();
			rb.angularVelocity = Vector3.zero;
			rb.velocity = Vector3.zero;
		}
		Vector3 vector = Vector3.zero;
		Vector3 forward = Vector3.zero;
		if (GlobalStorage.Instance.MasterHMDAndInputController.TrackedHmdTransform != null)
		{
			vector = GlobalStorage.Instance.MasterHMDAndInputController.TrackedHmdTransform.position;
			forward = GlobalStorage.Instance.MasterHMDAndInputController.TrackedHmdTransform.forward;
		}
		forward.y = 0f;
		Vector3 position = vector + forward.normalized * 0.5f;
		position.y = position.y / 4f * 3f;
		Quaternion rotation = Quaternion.LookRotation(forward, Vector3.up);
		rotation.eulerAngles = new Vector3(0f, rotation.eulerAngles.y - 90f, 0f);
		base.transform.position = position;
		base.transform.rotation = rotation;
		resetDrink.transform.localPosition = resetDrinkLocation;
		resetDrink.transform.localRotation = resetDrinkRotation;
		exitEdible.SetNumberOfBitesTaken(0);
		exitEdible.transform.localPosition = exitEdibleLocation;
		exitEdible.transform.localRotation = exitEdibleRotation;
	}

	private void OnExitEdibleTriggerListenerEnter(TriggerEventInfo info)
	{
	}

	private void OnResetDrinkTriggerListenerEnter(TriggerEventInfo info)
	{
		if (!info.other.isTrigger)
		{
			resetDrink.transform.localPosition = resetDrinkLocation;
			resetDrink.transform.localRotation = resetDrinkRotation;
		}
	}
}
