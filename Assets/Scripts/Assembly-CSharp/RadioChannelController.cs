using UnityEngine;

public class RadioChannelController : MonoBehaviour
{
	[SerializeField]
	private AudioSourceHelper audioSource;

	[SerializeField]
	private string channelNumber;

	[SerializeField]
	private string channelInfo;

	[Range(0f, 1f)]
	[SerializeField]
	private float channelPosition;

	public float ChannelPosition
	{
		get
		{
			return channelPosition;
		}
	}

	public AudioSourceHelper AudioSource
	{
		get
		{
			return audioSource;
		}
	}

	public string ChannelNumber
	{
		get
		{
			return channelNumber;
		}
	}

	public string ChannelInfo
	{
		get
		{
			return channelInfo;
		}
		set
		{
			channelInfo = value;
		}
	}
}
