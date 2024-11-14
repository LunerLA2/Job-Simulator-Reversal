using UnityEngine;

public class ConveyerBelt : MonoBehaviour
{
	[SerializeField]
	private float scrollingForce = 1f;

	[SerializeField]
	private float textureScrollSpeed = 1f;

	private float conveyerSpeed;

	[SerializeField]
	private TextureOffsetAnimator textureOffsetAnimator;

	private float textureOffsetAmount;

	[SerializeField]
	private Transform conveyerAudioLocationTransform;

	[SerializeField]
	private AudioClip conveyerBeltAudioClip;

	private bool isAudioPlaying;

	private bool isMoving;

	private AudioSourceHelper beltSoundRunningAudioSoruceHelper;

	public float ConveyerSpeed
	{
		get
		{
			return conveyerSpeed;
		}
	}

	private void Awake()
	{
		if (base.enabled)
		{
			SetIsMoving(true);
		}
	}

	private void OnCollisionStay(Collision collision)
	{
		Rigidbody component = collision.gameObject.GetComponent<Rigidbody>();
		component.MovePosition(component.transform.position + base.transform.forward * Time.deltaTime * conveyerSpeed * scrollingForce);
	}

	private void Update()
	{
		textureOffsetAmount += conveyerSpeed * Time.deltaTime * textureScrollSpeed;
		textureOffsetAnimator.offsetY = textureOffsetAmount;
	}

	public void SetIsMoving(bool isEnabled)
	{
		if (isEnabled)
		{
			if (!isMoving)
			{
				SetSpeed(0.35f);
			}
		}
		else if (isMoving)
		{
			SetSpeed(0f);
		}
	}

	private void SetSpeed(float s)
	{
		if (isAudioPlaying)
		{
			if (s == 0f)
			{
				if (beltSoundRunningAudioSoruceHelper != null && beltSoundRunningAudioSoruceHelper.IsFollowTargetAndClipEqual(conveyerAudioLocationTransform, conveyerBeltAudioClip))
				{
					beltSoundRunningAudioSoruceHelper.Stop();
					beltSoundRunningAudioSoruceHelper = null;
				}
				isAudioPlaying = false;
			}
		}
		else if (s > 0f)
		{
			beltSoundRunningAudioSoruceHelper = AudioManager.Instance.PlayLooping(conveyerAudioLocationTransform, conveyerBeltAudioClip, 0.5f, 1f);
			isAudioPlaying = true;
		}
		conveyerSpeed = s;
		if (conveyerSpeed > 0f)
		{
			isMoving = true;
		}
		else
		{
			isMoving = false;
		}
	}
}
