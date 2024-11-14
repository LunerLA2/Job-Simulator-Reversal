using UnityEngine;

public class ONSPAudioSource : MonoBehaviour
{
	[SerializeField]
	private bool enableSpatialization = true;

	[SerializeField]
	private float gain;

	[SerializeField]
	private bool useInvSqr;

	[SerializeField]
	private float near = 1f;

	[SerializeField]
	private float far = 10f;

	[SerializeField]
	private bool disableRfl;

	private bool dirtyParams = true;

	public bool EnableSpatialization
	{
		get
		{
			return enableSpatialization;
		}
		set
		{
			enableSpatialization = value;
			dirtyParams = true;
		}
	}

	public float Gain
	{
		get
		{
			return gain;
		}
		set
		{
			gain = Mathf.Clamp(value, 0f, 24f);
			dirtyParams = true;
		}
	}

	public bool UseInvSqr
	{
		get
		{
			return useInvSqr;
		}
		set
		{
			useInvSqr = value;
			dirtyParams = true;
		}
	}

	public float Near
	{
		get
		{
			return near;
		}
		set
		{
			near = Mathf.Clamp(value, 0f, 1000000f);
			dirtyParams = true;
		}
	}

	public float Far
	{
		get
		{
			return far;
		}
		set
		{
			far = Mathf.Clamp(value, 0f, 1000000f);
			dirtyParams = true;
		}
	}

	public bool DisableRfl
	{
		get
		{
			return disableRfl;
		}
		set
		{
			disableRfl = value;
			dirtyParams = true;
		}
	}

	private void Awake()
	{
		AudioSource source = GetComponent<AudioSource>();
		SetParameters(ref source);
	}

	private void Start()
	{
	}

	private void Update()
	{
		AudioSource source = GetComponent<AudioSource>();
		if (dirtyParams)
		{
			SetParameters(ref source);
			dirtyParams = false;
		}
	}

	public void SetParameters(ref AudioSource source)
	{
		source.spatialize = enableSpatialization;
		source.SetSpatializerFloat(0, gain);
		if (useInvSqr)
		{
			source.SetSpatializerFloat(1, 1f);
		}
		else
		{
			source.SetSpatializerFloat(1, 0f);
		}
		source.SetSpatializerFloat(2, near);
		source.SetSpatializerFloat(3, far);
		if (disableRfl)
		{
			source.SetSpatializerFloat(4, 1f);
		}
		else
		{
			source.SetSpatializerFloat(4, 0f);
		}
	}
}
