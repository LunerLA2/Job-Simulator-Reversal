using UnityEngine;

public class DoorSwingAudio : MonoBehaviour
{
	private Quaternion lastRotation;

	private Quaternion currentRotation;

	[SerializeField]
	private float degreeToTick;

	[SerializeField]
	private AudioClip clip;

	private void Awake()
	{
		lastRotation = base.transform.localRotation;
	}

	private void Update()
	{
		currentRotation = base.transform.localRotation;
		if (Quaternion.Angle(lastRotation, currentRotation) > degreeToTick)
		{
			lastRotation = currentRotation;
			AudioManager.Instance.Play(base.transform.position, clip, 1f, 1f);
		}
	}
}
