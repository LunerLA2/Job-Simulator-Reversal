using UnityEngine;

public class CarHornSqueezeController : MonoBehaviour
{
	[SerializeField]
	private GrabbableSlider grabbableSlider;

	[SerializeField]
	private Animation scrubAnimation;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void HonkHorn()
	{
		scrubAnimation.Play();
	}
}
