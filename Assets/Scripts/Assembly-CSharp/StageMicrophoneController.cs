using UnityEngine;

public class StageMicrophoneController : MonoBehaviour
{
	[SerializeField]
	private Transform eyeLevel;

	private Transform playerHead;

	[SerializeField]
	private Transform micUpDownTransform;

	[SerializeField]
	private float micMinY;

	[SerializeField]
	private float micMaxY;

	[SerializeField]
	private Transform micWireTransform;

	[SerializeField]
	private float micWireYScaleAtMinY;

	[SerializeField]
	private AudioClip micTapAudioClip;

	[SerializeField]
	private Animation micTapAnimation;

	private float tapTimer;

	private float tapTimeout = 0.1f;

	[SerializeField]
	private Transform[] feedbackLocations;

	private void OnEnable()
	{
	}

	private void OnDisable()
	{
	}

	private void ObjectEntered(Collider c)
	{
		if (!(tapTimer > 0f) && (c.gameObject.layer == 10 || c.gameObject.layer == 8))
		{
			MicTapped();
		}
	}

	private void MicTapped()
	{
		tapTimer = tapTimeout;
		micTapAnimation.Stop();
		micTapAnimation.Play();
		for (int i = 0; i < feedbackLocations.Length; i++)
		{
			AudioManager.Instance.Play(feedbackLocations[i], micTapAudioClip, 1f, 1f);
		}
	}

	private void Update()
	{
		if (playerHead == null)
		{
			playerHead = GlobalStorage.Instance.MasterHMDAndInputController.TrackedHmdTransform;
		}
		if (playerHead != null)
		{
			AlignHeightWithPlayersEyes();
		}
		if (tapTimer > 0f)
		{
			tapTimer -= Time.deltaTime;
		}
		else
		{
			tapTimer = 0f;
		}
	}

	private void AlignHeightWithPlayersEyes()
	{
		float num = playerHead.transform.position.y - eyeLevel.position.y;
		Vector3 localPosition = micUpDownTransform.localPosition;
		localPosition.y = Mathf.Lerp(localPosition.y, localPosition.y + num, Time.deltaTime);
		localPosition.y = Mathf.Clamp(localPosition.y, micMinY, micMaxY);
		micUpDownTransform.localPosition = localPosition;
		float t = Mathf.InverseLerp(micMinY, micMaxY, localPosition.y);
		Vector3 localScale = micWireTransform.localScale;
		localScale.y = Mathf.Lerp(micWireYScaleAtMinY, 1f, t);
		micWireTransform.localScale = localScale;
	}
}
