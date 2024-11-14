using UnityEngine;

public class DoorGrateSliderManager : MonoBehaviour
{
	private float startDoorHeight;

	private float currentDoorHeight;

	private float doorAmountScalar = 0.6f;

	public Animation[] anim;

	public Transform movingObject;

	private void Start()
	{
		startDoorHeight = movingObject.position.y;
	}

	private void Update()
	{
		currentDoorHeight = movingObject.position.y;
		float num = currentDoorHeight - startDoorHeight;
		Animation[] array = anim;
		foreach (Animation animation in array)
		{
			AnimationState animationState = animation["grateTest 1"];
			animationState.enabled = true;
			animationState.weight = 1f;
			animationState.speed = 0f;
			animationState.time = num * (1f / doorAmountScalar);
		}
	}
}
