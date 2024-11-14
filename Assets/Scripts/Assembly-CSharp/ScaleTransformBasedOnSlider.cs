using UnityEngine;

public class ScaleTransformBasedOnSlider : MonoBehaviour
{
	[SerializeField]
	private GrabbableSlider slider;

	[SerializeField]
	private Transform transformToScale;

	[SerializeField]
	private Vector3 scaleAtUpper = Vector3.one;

	[SerializeField]
	private Vector3 scaleAtLower = Vector3.one;

	private void Update()
	{
		transformToScale.localScale = Vector3.Lerp(scaleAtLower, scaleAtUpper, slider.NormalizedOffset);
	}
}
