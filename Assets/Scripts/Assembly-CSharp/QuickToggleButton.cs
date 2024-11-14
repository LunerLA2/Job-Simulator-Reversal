using UnityEngine;

public class QuickToggleButton : MonoBehaviour
{
	[SerializeField]
	private Transform controller;

	private bool currentState;

	private Vector3 cachedPosition;

	private void Awake()
	{
		cachedPosition = controller.localPosition;
	}

	public void ToggleObjects()
	{
		currentState = !currentState;
	}

	private void Update()
	{
		if (currentState)
		{
			controller.localPosition = Vector3.Lerp(controller.localPosition, Vector3.zero, Time.deltaTime);
		}
		else
		{
			controller.localPosition = Vector3.Lerp(controller.localPosition, cachedPosition, Time.deltaTime);
		}
	}
}
