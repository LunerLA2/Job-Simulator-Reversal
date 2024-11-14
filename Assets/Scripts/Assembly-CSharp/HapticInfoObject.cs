using UnityEngine;

public class HapticInfoObject
{
	protected float pulseRateMicroSec;

	protected float length;

	protected float elapsed;

	protected bool doesHaveLength = true;

	protected bool isRunning = true;

	private bool isPermanent;

	protected bool isAutoSetElapsedUsingDeltaTime = true;

	private float pulseRateMultipler = 1f;

	private float deltaTimeSpeedMultiplier = 1f;

	public bool IsRunning
	{
		get
		{
			return isRunning;
		}
	}

	public bool IsPermanent
	{
		get
		{
			return isPermanent;
		}
	}

	public float PulseRateMultiplier
	{
		get
		{
			return pulseRateMultipler;
		}
		set
		{
			pulseRateMultipler = value;
		}
	}

	public float DeltaTimeSpeedMultiplier
	{
		get
		{
			return deltaTimeSpeedMultiplier;
		}
		set
		{
			deltaTimeSpeedMultiplier = value;
		}
	}

	public HapticInfoObject()
	{
	}

	public HapticInfoObject(float pulseRateMicroSec, float length, bool isAutoSetElapsedUsingDeltaTime)
	{
		elapsed = 0f;
		isRunning = true;
		this.pulseRateMicroSec = pulseRateMicroSec;
		if (length != float.PositiveInfinity)
		{
			doesHaveLength = true;
		}
		this.length = length;
		this.isAutoSetElapsedUsingDeltaTime = isAutoSetElapsedUsingDeltaTime;
	}

	public HapticInfoObject(float pulseRateMicroSec, float length)
	{
		elapsed = 0f;
		isRunning = true;
		this.pulseRateMicroSec = pulseRateMicroSec;
		if (length != float.PositiveInfinity)
		{
			doesHaveLength = true;
		}
		this.length = length;
	}

	public HapticInfoObject(float pulseRateMicroSec)
	{
		elapsed = 0f;
		isRunning = true;
		this.pulseRateMicroSec = pulseRateMicroSec;
		length = float.PositiveInfinity;
		doesHaveLength = false;
	}

	public float GetCurrPulseRateMicroSec()
	{
		return Mathf.Clamp(pulseRateMicroSec * PulseRateMultiplier, 0f, 3999f);
	}

	public void SetCurrPulseRateMicroSec(float value)
	{
		pulseRateMicroSec = value;
	}

	public void SetElapsed(float elapsed)
	{
		this.elapsed = elapsed;
	}

	public void SetElapsedAsPercentageOfLength(float percentage)
	{
		elapsed = length * percentage;
	}

	public virtual void RunHapticsUpdate(float deltaTime)
	{
		if (isRunning)
		{
			if (isAutoSetElapsedUsingDeltaTime)
			{
				elapsed += deltaTime * DeltaTimeSpeedMultiplier;
			}
			if (doesHaveLength && elapsed > length)
			{
				isRunning = false;
			}
		}
	}

	public void DeactiveHaptic()
	{
		isRunning = false;
	}

	public void SetAsPermanent()
	{
		isPermanent = true;
	}

	public void Restart()
	{
		elapsed = 0f;
		isRunning = true;
	}
}
